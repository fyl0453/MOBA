using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ServerSelectPanel : UIPanel
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button confirmButton;
    
    [SerializeField] private GameObject serverListContent;
    [SerializeField] private GameObject serverItemPrefab;
    
    private List<ServerItem> serverItems = new List<ServerItem>();
    private string selectedServerID;
    
    private void Start()
    {
        backButton.onClick.AddListener(OnBack);
        confirmButton.onClick.AddListener(OnConfirm);
        
        LoadServerList();
    }
    
    private void LoadServerList()
    {
        // 清空现有列表
        foreach (var item in serverItems)
        {
            Destroy(item.gameObject);
        }
        serverItems.Clear();
        
        // 获取服务器列表
        List<Server> servers = ServerManager.Instance.GetAllServers();
        
        foreach (Server server in servers)
        {
            GameObject serverItemGO = Instantiate(serverItemPrefab, serverListContent.transform);
            ServerItem serverItem = serverItemGO.GetComponent<ServerItem>();
            if (serverItem != null)
            {
                serverItem.Setup(server, OnServerSelect);
                serverItems.Add(serverItem);
            }
        }
        
        // 选择默认服务器
        string defaultServerID = ServerManager.Instance.GetSelectedServerID();
        OnServerSelect(ServerManager.Instance.GetServer(defaultServerID));
    }
    
    private void OnServerSelect(Server server)
    {
        selectedServerID = server.serverID;
        
        // 更新所有服务器项的选中状态
        foreach (var item in serverItems)
        {
            item.SetSelected(item.server.serverID == selectedServerID);
        }
    }
    
    private void OnConfirm()
    {
        if (!string.IsNullOrEmpty(selectedServerID))
        {
            ServerManager.Instance.SelectServer(selectedServerID);
            MainUIManager.Instance.ShowPanel("MainMenu");
        }
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("Account");
    }
}

public class ServerItem : MonoBehaviour
{
    [SerializeField] private Text serverNameText;
    [SerializeField] private Text serverStatusText;
    [SerializeField] private Text playerCountText;
    [SerializeField] private Text pingText;
    [SerializeField] private Image selectedImage;
    [SerializeField] private Button selectButton;
    
    public Server server;
    
    public void Setup(Server s, System.Action<Server> selectCallback)
    {
        server = s;
        serverNameText.text = s.serverName;
        serverStatusText.text = GetStatusText(s.status);
        playerCountText.text = $"{s.playerCount}/{s.maxPlayerCount}";
        pingText.text = $"{s.ping}ms";
        
        selectButton.onClick.AddListener(() => selectCallback?.Invoke(s));
    }
    
    public void SetSelected(bool selected)
    {
        selectedImage.gameObject.SetActive(selected);
    }
    
    private string GetStatusText(ServerStatus status)
    {
        switch (status)
        {
            case ServerStatus.Online:
                return "在线";
            case ServerStatus.Busy:
                return "繁忙";
            case ServerStatus.Full:
                return "满员";
            case ServerStatus.Maintenance:
                return "维护中";
            case ServerStatus.Offline:
                return "离线";
            default:
                return "未知";
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SocialPanel : UIPanel
{
    [SerializeField] private TabButton friendsTab;
    [SerializeField] private TabButton chatTab;
    [SerializeField] private TabButton teamTab;
    [SerializeField] private TabButton clanTab;
    
    [SerializeField] private GameObject friendsContent;
    [SerializeField] private GameObject chatContent;
    [SerializeField] private GameObject teamContent;
    [SerializeField] private GameObject clanContent;
    
    [SerializeField] private Button backButton;
    [SerializeField] private Button openFriendPanelButton;
    [SerializeField] private Button openClanPanelButton;
    
    private void Start()
    {
        friendsTab.onClick.AddListener(() => SwitchTab("Friends"));
        chatTab.onClick.AddListener(() => SwitchTab("Chat"));
        teamTab.onClick.AddListener(() => SwitchTab("Team"));
        clanTab.onClick.AddListener(() => SwitchTab("Clan"));
        backButton.onClick.AddListener(OnBack);
        openFriendPanelButton.onClick.AddListener(OnOpenFriendPanel);
        openClanPanelButton.onClick.AddListener(OnOpenClanPanel);
        
        SwitchTab("Friends");
    }
    
    private void SwitchTab(string tabName)
    {
        friendsContent.SetActive(tabName == "Friends");
        chatContent.SetActive(tabName == "Chat");
        teamContent.SetActive(tabName == "Team");
        clanContent.SetActive(tabName == "Clan");
        
        friendsTab.SetSelected(tabName == "Friends");
        chatTab.SetSelected(tabName == "Chat");
        teamTab.SetSelected(tabName == "Team");
        clanTab.SetSelected(tabName == "Clan");
        
        if (tabName == "Friends")
        {
            InitializeFriendsList();
        }
        else if (tabName == "Chat")
        {
            InitializeChat();
        }
        else if (tabName == "Team")
        {
            InitializeTeam();
        }
        else if (tabName == "Clan")
        {
            InitializeClan();
        }
    }
    
    private void InitializeFriendsList()
    {
        // 从社交系统获取好友列表
        var socialManager = SocialSystemDetailedManager.Instance;
        var friends = socialManager.GetFriendsList("player_001");
        Debug.Log("好友列表数据: " + friends.Count + " 个好友");
    }
    
    private void InitializeChat()
    {
        // 从社交系统获取聊天记录
        var socialManager = SocialSystemDetailedManager.Instance;
        var messages = socialManager.GetChatMessages("player_001");
        Debug.Log("聊天记录数据: " + messages.Count + " 条消息");
    }
    
    private void InitializeTeam()
    {
        // 从社交系统获取组队信息
        var socialManager = SocialSystemDetailedManager.Instance;
        var team = socialManager.GetTeamInfo("player_001");
        Debug.Log("组队信息: " + (team != null ? "已组队" : "未组队"));
    }
    
    private void InitializeClan()
    {
        // 从社交系统获取战队信息
        var socialManager = SocialSystemDetailedManager.Instance;
        var clan = socialManager.GetClanInfo("player_001");
        Debug.Log("战队信息: " + (clan != null ? clan.ClanName : "未加入战队"));
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
    
    private void OnOpenFriendPanel()
    {
        MainUIManager.Instance.ShowPanel("Friend");
    }
    
    private void OnOpenClanPanel()
    {
        MainUIManager.Instance.ShowPanel("Clan");
    }
}

public class TabButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image highlightImage;
    
    public System.Action onClick;
    
    private void Start()
    {
        button.onClick.AddListener(() => onClick?.Invoke());
    }
    
    public void SetSelected(bool selected)
    {
        highlightImage.gameObject.SetActive(selected);
    }
}

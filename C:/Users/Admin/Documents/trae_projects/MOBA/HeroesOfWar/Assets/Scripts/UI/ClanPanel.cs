using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClanPanel : UIPanel
{
    [SerializeField] private GameObject clanListPanel;
    [SerializeField] private GameObject createClanPanel;
    [SerializeField] private GameObject clanDetailPanel;
    
    [SerializeField] private Button clanListButton;
    [SerializeField] private Button createClanButton;
    [SerializeField] private Button backButton;
    
    // 战队列表
    [SerializeField] private GameObject clanItemPrefab;
    [SerializeField] private Transform clanListContent;
    
    // 创建战队
    [SerializeField] private InputField clanNameInput;
    [SerializeField] private Button createButton;
    
    // 战队详情
    [SerializeField] private Text clanNameText;
    [SerializeField] private Text clanLevelText;
    [SerializeField] private Text clanLeaderText;
    [SerializeField] private Text clanMembersText;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button backToClanListButton;
    
    private List<ClanItem> clanItems = new List<ClanItem>();
    private ClanData selectedClan;
    
    private void Start()
    {
        clanListButton.onClick.AddListener(() => SwitchPanel("ClanList"));
        createClanButton.onClick.AddListener(() => SwitchPanel("CreateClan"));
        backButton.onClick.AddListener(OnBack);
        createButton.onClick.AddListener(OnCreateClan);
        joinButton.onClick.AddListener(OnJoinClan);
        leaveButton.onClick.AddListener(OnLeaveClan);
        backToClanListButton.onClick.AddListener(() => SwitchPanel("ClanList"));
        
        SwitchPanel("ClanList");
    }
    
    private void SwitchPanel(string panelName)
    {
        clanListPanel.SetActive(panelName == "ClanList");
        createClanPanel.SetActive(panelName == "CreateClan");
        clanDetailPanel.SetActive(panelName == "ClanDetail");
        
        if (panelName == "ClanList")
        {
            RefreshClanList();
        }
    }
    
    private void RefreshClanList()
    {
        // 清空现有列表
        foreach (var item in clanItems)
        {
            Destroy(item.gameObject);
        }
        clanItems.Clear();
        
        // 获取战队列表
        List<ClanData> clans = ClanSystem.Instance.GetClans();
        
        foreach (ClanData clan in clans)
        {
            GameObject clanItemGO = Instantiate(clanItemPrefab, clanListContent);
            ClanItem clanItem = clanItemGO.GetComponent<ClanItem>();
            if (clanItem != null)
            {
                clanItem.Setup(clan, OnClanSelect);
                clanItems.Add(clanItem);
            }
        }
    }
    
    private void OnClanSelect(ClanData clan)
    {
        selectedClan = clan;
        ShowClanDetail(clan);
    }
    
    private void ShowClanDetail(ClanData clan)
    {
        clanNameText.text = clan.clanName;
        clanLevelText.text = "等级: " + clan.level;
        clanLeaderText.text = "队长: " + clan.leaderName;
        clanMembersText.text = "成员: " + clan.members.Count;
        
        // 检查玩家是否已加入战队
        bool isInClan = ClanSystem.Instance.GetPlayerClan(AccountManager.Instance.currentUserID) != null;
        bool isSelectedClan = isInClan && ClanSystem.Instance.GetPlayerClan(AccountManager.Instance.currentUserID).clanID == clan.clanID;
        
        joinButton.gameObject.SetActive(!isInClan);
        leaveButton.gameObject.SetActive(isSelectedClan);
        
        SwitchPanel("ClanDetail");
    }
    
    private void OnCreateClan()
    {
        string clanName = clanNameInput.text;
        if (!string.IsNullOrEmpty(clanName) && AccountManager.Instance.isLoggedIn)
        {
            bool success = ClanSystem.Instance.CreateClan(clanName, AccountManager.Instance.currentUserID, AccountManager.Instance.currentUsername);
            if (success)
            {
                clanNameInput.text = "";
                SwitchPanel("ClanList");
            }
        }
    }
    
    private void OnJoinClan()
    {
        if (selectedClan != null && AccountManager.Instance.isLoggedIn)
        {
            bool success = ClanSystem.Instance.JoinClan(AccountManager.Instance.currentUserID, AccountManager.Instance.currentUsername, selectedClan.clanID);
            if (success)
            {
                ShowClanDetail(selectedClan);
            }
        }
    }
    
    private void OnLeaveClan()
    {
        if (AccountManager.Instance.isLoggedIn)
        {
            bool success = ClanSystem.Instance.LeaveClan(AccountManager.Instance.currentUserID);
            if (success)
            {
                SwitchPanel("ClanList");
            }
        }
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("Social");
    }
}

public class ClanItem : MonoBehaviour
{
    [SerializeField] private Text clanNameText;
    [SerializeField] private Text clanLevelText;
    [SerializeField] private Text clanMembersText;
    [SerializeField] private Button selectButton;
    
    private ClanData clan;
    private System.Action<ClanData> onSelectCallback;
    
    public void Setup(ClanData c, System.Action<ClanData> selectCallback)
    {
        clan = c;
        clanNameText.text = c.clanName;
        clanLevelText.text = "等级: " + c.level;
        clanMembersText.text = "成员: " + c.members.Count;
        onSelectCallback = selectCallback;
        
        selectButton.onClick.AddListener(() => onSelectCallback?.Invoke(clan));
    }
}

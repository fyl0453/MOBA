using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MailPanel : UIPanel
{
    [SerializeField] private TabButton systemTab;
    [SerializeField] private TabButton rewardTab;
    [SerializeField] private TabButton privateTab;
    
    [SerializeField] private GameObject systemContent;
    [SerializeField] private GameObject rewardContent;
    [SerializeField] private GameObject privateContent;
    
    [SerializeField] private Button backButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button deleteAllButton;
    
    private void Start()
    {
        systemTab.onClick.AddListener(() => SwitchTab("System"));
        rewardTab.onClick.AddListener(() => SwitchTab("Reward"));
        privateTab.onClick.AddListener(() => SwitchTab("Private"));
        backButton.onClick.AddListener(OnBack);
        refreshButton.onClick.AddListener(OnRefresh);
        deleteAllButton.onClick.AddListener(OnDeleteAll);
        
        SwitchTab("System");
    }
    
    private void SwitchTab(string tabName)
    {
        systemContent.SetActive(tabName == "System");
        rewardContent.SetActive(tabName == "Reward");
        privateContent.SetActive(tabName == "Private");
        
        systemTab.SetSelected(tabName == "System");
        rewardTab.SetSelected(tabName == "Reward");
        privateTab.SetSelected(tabName == "Private");
        
        if (tabName == "System")
        {
            InitializeSystemMails();
        }
        else if (tabName == "Reward")
        {
            InitializeRewardMails();
        }
        else if (tabName == "Private")
        {
            InitializePrivateMails();
        }
    }
    
    private void InitializeSystemMails()
    {
        // 从邮件系统获取系统邮件数据
        var mailManager = MailSystemDetailedManager.Instance;
        var systemMails = mailManager.GetSystemMails("player_001");
        Debug.Log("系统邮件数据: " + systemMails.Count + " 封邮件");
    }
    
    private void InitializeRewardMails()
    {
        // 从邮件系统获取奖励邮件数据
        var mailManager = MailSystemDetailedManager.Instance;
        var rewardMails = mailManager.GetRewardMails("player_001");
        Debug.Log("奖励邮件数据: " + rewardMails.Count + " 封邮件");
    }
    
    private void InitializePrivateMails()
    {
        // 从邮件系统获取私人邮件数据
        var mailManager = MailSystemDetailedManager.Instance;
        var privateMails = mailManager.GetPrivateMails("player_001");
        Debug.Log("私人邮件数据: " + privateMails.Count + " 封邮件");
    }
    
    private void OnRefresh()
    {
        // 刷新邮件列表
        Debug.Log("刷新邮件");
    }
    
    private void OnDeleteAll()
    {
        // 删除所有邮件
        Debug.Log("删除所有邮件");
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProfilePanel : UIPanel
{
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text rankText;
    [SerializeField] private Text winRateText;
    [SerializeField] private Text totalGamesText;
    [SerializeField] private Text kdaText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text diamondText;
    
    [SerializeField] private Button editProfileButton;
    [SerializeField] private Button statisticsButton;
    [SerializeField] private Button achievementsButton;
    [SerializeField] private Button backButton;
    
    private void Start()
    {
        editProfileButton.onClick.AddListener(OnEditProfile);
        statisticsButton.onClick.AddListener(OnStatistics);
        achievementsButton.onClick.AddListener(OnAchievements);
        backButton.onClick.AddListener(OnBack);
        
        UpdateProfileInfo();
    }
    
    public override void OnShow()
    {
        base.OnShow();
        UpdateProfileInfo();
    }
    
    private void UpdateProfileInfo()
    {
        // 从职业生涯系统获取玩家信息
        var careerManager = KingCareerSystemDetailedManager.Instance;
        var careerData = careerManager.GetCareerData();
        
        playerNameText.text = careerData.PlayerName;
        levelText.text = "等级: " + careerManager.GetPlayerLevel();
        rankText.text = "段位: " + careerManager.GetPlayerLevel();
        winRateText.text = "胜率: " + careerManager.GetWinRate().ToString("F1") + "%";
        totalGamesText.text = "总场次: " + careerData.TotalGames;
        kdaText.text = "KDA: " + careerData.KDA.ToString("F1");
        
        // 从经济系统获取货币信息
        var economyManager = EconomySystemDetailedManager.Instance;
        goldText.text = "金币: " + economyManager.GetCurrencyBalance("gold");
        diamondText.text = "钻石: " + economyManager.GetCurrencyBalance("diamond");
    }
    
    private void OnEditProfile()
    {
        // 编辑个人资料
        Debug.Log("编辑个人资料");
    }
    
    private void OnStatistics()
    {
        // 查看详细统计数据
        Debug.Log("查看统计数据");
    }
    
    private void OnAchievements()
    {
        // 查看成就
        Debug.Log("查看成就");
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

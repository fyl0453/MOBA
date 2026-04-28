using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : UIPanel
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button heroSelectButton;
    [SerializeField] private Button socialButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button questButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button eventButton;
    [SerializeField] private Button mailButton;
    [SerializeField] private Button profileButton;
    [SerializeField] private Button achievementButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text diamondText;
    
    private void Start()
    {
        startGameButton.onClick.AddListener(OnStartGame);
        heroSelectButton.onClick.AddListener(OnHeroSelect);
        socialButton.onClick.AddListener(OnSocial);
        inventoryButton.onClick.AddListener(OnInventory);
        leaderboardButton.onClick.AddListener(OnLeaderboard);
        questButton.onClick.AddListener(OnQuest);
        shopButton.onClick.AddListener(OnShop);
        eventButton.onClick.AddListener(OnEvent);
        mailButton.onClick.AddListener(OnMail);
        profileButton.onClick.AddListener(OnProfile);
        achievementButton.onClick.AddListener(OnAchievement);
        settingsButton.onClick.AddListener(OnSettings);
        exitButton.onClick.AddListener(OnExit);
        
        UpdatePlayerInfo();
    }
    
    public override void OnShow()
    {
        base.OnShow();
        UpdatePlayerInfo();
    }
    
    private void UpdatePlayerInfo()
    {
        // 从职业生涯系统获取玩家信息
        var careerManager = KingCareerSystemDetailedManager.Instance;
        var careerData = careerManager.GetCareerData();
        
        playerNameText.text = careerData.PlayerName;
        levelText.text = "等级: " + careerManager.GetPlayerLevel();
        
        // 从经济系统获取货币信息
        var economyManager = EconomySystemDetailedManager.Instance;
        goldText.text = "金币: " + economyManager.GetCurrencyBalance("gold");
        diamondText.text = "钻石: " + economyManager.GetCurrencyBalance("diamond");
    }
    
    private void OnStartGame()
    {
        // 开始游戏逻辑
        Debug.Log("开始游戏");
        MainUIManager.Instance.ShowPanel("HeroSelect");
    }
    
    private void OnHeroSelect()
    {
        MainUIManager.Instance.ShowPanel("HeroSelect");
    }
    
    private void OnSocial()
    {
        MainUIManager.Instance.ShowPanel("Social");
    }
    
    private void OnInventory()
    {
        MainUIManager.Instance.ShowPanel("Inventory");
    }
    
    private void OnLeaderboard()
    {
        MainUIManager.Instance.ShowPanel("Leaderboard");
    }
    
    private void OnQuest()
    {
        MainUIManager.Instance.ShowPanel("Quest");
    }
    
    private void OnShop()
    {
        MainUIManager.Instance.ShowPanel("Shop");
    }
    
    private void OnEvent()
    {
        MainUIManager.Instance.ShowPanel("Event");
    }
    
    private void OnMail()
    {
        MainUIManager.Instance.ShowPanel("Mail");
    }
    
    private void OnProfile()
    {
        MainUIManager.Instance.ShowPanel("Profile");
    }
    
    private void OnSettings()
    {
        MainUIManager.Instance.ShowPanel("Settings");
    }
    
    private void OnAchievement()
    {
        MainUIManager.Instance.ShowPanel("Achievement");
    }
    
    private void OnExit()
    {
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementPanel : UIPanel
{
    [SerializeField] private GameObject achievementListPanel;
    [SerializeField] private GameObject achievementDetailPanel;
    
    [SerializeField] private Button backButton;
    [SerializeField] private Button allButton;
    [SerializeField] private Button completedButton;
    [SerializeField] private Button inProgressButton;
    
    // 成就列表
    [SerializeField] private GameObject achievementItemPrefab;
    [SerializeField] private Transform achievementListContent;
    
    // 成就详情
    [SerializeField] private Text achievementNameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text progressText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text rarityText;
    [SerializeField] private Text unlockDateText;
    [SerializeField] private Button backToAchievementListButton;
    
    private List<AchievementItem> achievementItems = new List<AchievementItem>();
    private Achievement selectedAchievement;
    
    private void Start()
    {
        backButton.onClick.AddListener(OnBack);
        allButton.onClick.AddListener(() => ShowAchievements("All"));
        completedButton.onClick.AddListener(() => ShowAchievements("Completed"));
        inProgressButton.onClick.AddListener(() => ShowAchievements("InProgress"));
        backToAchievementListButton.onClick.AddListener(() => SwitchPanel("AchievementList"));
        
        SwitchPanel("AchievementList");
        ShowAchievements("All");
    }
    
    private void SwitchPanel(string panelName)
    {
        achievementListPanel.SetActive(panelName == "AchievementList");
        achievementDetailPanel.SetActive(panelName == "AchievementDetail");
    }
    
    private void ShowAchievements(string filter)
    {
        // 清空现有列表
        foreach (var item in achievementItems)
        {
            Destroy(item.gameObject);
        }
        achievementItems.Clear();
        
        // 获取成就列表
        List<Achievement> achievements = new List<Achievement>();
        
        switch (filter)
        {
            case "All":
                achievements = AchievementSystem.Instance.GetAllAchievements();
                break;
            case "Completed":
                achievements = AchievementSystem.Instance.GetCompletedAchievements();
                break;
            case "InProgress":
                achievements = AchievementSystem.Instance.GetInProgressAchievements();
                break;
        }
        
        foreach (Achievement achievement in achievements)
        {
            GameObject achievementItemGO = Instantiate(achievementItemPrefab, achievementListContent);
            AchievementItem achievementItem = achievementItemGO.GetComponent<AchievementItem>();
            if (achievementItem != null)
            {
                achievementItem.Setup(achievement, OnAchievementSelect);
                achievementItems.Add(achievementItem);
            }
        }
    }
    
    private void OnAchievementSelect(Achievement achievement)
    {
        selectedAchievement = achievement;
        ShowAchievementDetail(achievement);
    }
    
    private void ShowAchievementDetail(Achievement achievement)
    {
        achievementNameText.text = achievement.achievementName;
        descriptionText.text = achievement.description;
        progressText.text = $"{achievement.currentValue}/{achievement.targetValue}";
        progressSlider.value = (float)achievement.currentValue / achievement.targetValue;
        rarityText.text = GetRarityText(achievement.rarity);
        unlockDateText.text = achievement.isCompleted ? achievement.unlockDate : "未解锁";
        
        SwitchPanel("AchievementDetail");
    }
    
    private string GetRarityText(AchievementRarity rarity)
    {
        switch (rarity)
        {
            case AchievementRarity.Common:
                return "普通";
            case AchievementRarity.Uncommon:
                return "优秀";
            case AchievementRarity.Rare:
                return "稀有";
            case AchievementRarity.Epic:
                return "史诗";
            case AchievementRarity.Legendary:
                return "传说";
            default:
                return "普通";
        }
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

public class AchievementItem : MonoBehaviour
{
    [SerializeField] private Text achievementNameText;
    [SerializeField] private Text progressText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text rarityText;
    [SerializeField] private Button selectButton;
    
    private Achievement achievement;
    private System.Action<Achievement> onSelectCallback;
    
    public void Setup(Achievement a, System.Action<Achievement> selectCallback)
    {
        achievement = a;
        achievementNameText.text = a.achievementName;
        progressText.text = $"{a.currentValue}/{a.targetValue}";
        progressSlider.value = (float)a.currentValue / a.targetValue;
        rarityText.text = GetRarityText(a.rarity);
        onSelectCallback = selectCallback;
        
        selectButton.onClick.AddListener(() => onSelectCallback?.Invoke(achievement));
    }
    
    private string GetRarityText(AchievementRarity rarity)
    {
        switch (rarity)
        {
            case AchievementRarity.Common:
                return "普通";
            case AchievementRarity.Uncommon:
                return "优秀";
            case AchievementRarity.Rare:
                return "稀有";
            case AchievementRarity.Epic:
                return "史诗";
            case AchievementRarity.Legendary:
                return "传说";
            default:
                return "普通";
        }
    }
}

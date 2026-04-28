using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestPanel : UIPanel
{
    [SerializeField] private TabButton dailyTab;
    [SerializeField] private TabButton weeklyTab;
    [SerializeField] private TabButton achievementTab;
    
    [SerializeField] private GameObject dailyContent;
    [SerializeField] private GameObject weeklyContent;
    [SerializeField] private GameObject achievementContent;
    
    [SerializeField] private Button backButton;
    
    private void Start()
    {
        dailyTab.onClick.AddListener(() => SwitchTab("Daily"));
        weeklyTab.onClick.AddListener(() => SwitchTab("Weekly"));
        achievementTab.onClick.AddListener(() => SwitchTab("Achievement"));
        backButton.onClick.AddListener(OnBack);
        
        SwitchTab("Daily");
    }
    
    private void SwitchTab(string tabName)
    {
        dailyContent.SetActive(tabName == "Daily");
        weeklyContent.SetActive(tabName == "Weekly");
        achievementContent.SetActive(tabName == "Achievement");
        
        dailyTab.SetSelected(tabName == "Daily");
        weeklyTab.SetSelected(tabName == "Weekly");
        achievementTab.SetSelected(tabName == "Achievement");
        
        if (tabName == "Daily")
        {
            InitializeDailyQuests();
        }
        else if (tabName == "Weekly")
        {
            InitializeWeeklyQuests();
        }
        else if (tabName == "Achievement")
        {
            InitializeAchievementQuests();
        }
    }
    
    private void InitializeDailyQuests()
    {
        // 从任务系统获取日常任务数据
        var questManager = QuestSystemDetailedManager.Instance;
        var dailyQuests = questManager.GetActiveQuests("player_001");
        Debug.Log("日常任务数据: " + dailyQuests.Count + " 个任务");
    }
    
    private void InitializeWeeklyQuests()
    {
        // 从任务系统获取周常任务数据
        var questManager = QuestSystemDetailedManager.Instance;
        var weeklyQuests = questManager.GetActiveQuests("player_001");
        Debug.Log("周常任务数据: " + weeklyQuests.Count + " 个任务");
    }
    
    private void InitializeAchievementQuests()
    {
        // 从任务系统获取成就任务数据
        var questManager = QuestSystemDetailedManager.Instance;
        var achievementQuests = questManager.GetCompletedQuests("player_001");
        Debug.Log("成就任务数据: " + achievementQuests.Count + " 个任务");
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

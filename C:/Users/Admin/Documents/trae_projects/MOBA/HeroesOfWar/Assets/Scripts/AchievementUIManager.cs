using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementUIManager : MonoBehaviour
{
    public static AchievementUIManager Instance { get; private set; }
    
    public Canvas achievementCanvas;
    public ScrollRect achievementsScrollRect;
    public Transform achievementsContent;
    public Text achievementNameText;
    public Text achievementDescriptionText;
    public Text achievementStatusText;
    public GameObject achievementPrefab;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        achievementCanvas.gameObject.SetActive(false);
    }
    
    public void OpenAchievementUI()
    {
        achievementCanvas.gameObject.SetActive(true);
        UpdateAchievementsList();
        ClearAchievementDetails();
    }
    
    public void CloseAchievementUI()
    {
        achievementCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateAchievementsList()
    {
        // 清空现有内容
        foreach (Transform child in achievementsContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示成就列表
        List<Achievement> achievements = AchievementManager.Instance.GetAllAchievements();
        foreach (Achievement achievement in achievements)
        {
            GameObject achievementObj = Instantiate(achievementPrefab, achievementsContent);
            Text[] texts = achievementObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = achievement.achievementName;
                texts[1].text = achievement.achievementDescription;
            }
            
            // 设置成就状态颜色
            Image[] images = achievementObj.GetComponentsInChildren<Image>();
            if (images.Length >= 1)
            {
                if (achievement.isUnlocked)
                {
                    images[0].color = Color.green;
                }
                else
                {
                    images[0].color = Color.gray;
                }
            }
            
            // 添加点击事件
            Button button = achievementObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => ShowAchievementDetails(achievement));
            }
        }
    }
    
    public void ShowAchievementDetails(Achievement achievement)
    {
        achievementNameText.text = achievement.achievementName;
        achievementDescriptionText.text = achievement.achievementDescription;
        achievementStatusText.text = achievement.isUnlocked ? "已解锁" : "未解锁";
    }
    
    public void ClearAchievementDetails()
    {
        achievementNameText.text = "";
        achievementDescriptionText.text = "";
        achievementStatusText.text = "";
    }
    
    public void UnlockAchievement(string achievementName)
    {
        AchievementManager.Instance.UnlockAchievement(achievementName);
        UpdateAchievementsList();
    }
}
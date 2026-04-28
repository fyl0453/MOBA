using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ActivityUIManager : MonoBehaviour
{
    public static ActivityUIManager Instance { get; private set; }
    
    public Canvas activityCanvas;
    public ScrollRect activitiesScrollRect;
    public Transform activitiesContent;
    public Text activityNameText;
    public Text activityDescriptionText;
    public Text activityTimeText;
    public Text activityProgressText;
    public Text activityRewardText;
    public Button joinActivityButton;
    public GameObject activityPrefab;
    
    private Activity selectedActivity;
    
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
        activityCanvas.gameObject.SetActive(false);
        joinActivityButton.onClick.AddListener(JoinActivity);
    }
    
    public void OpenActivityUI()
    {
        activityCanvas.gameObject.SetActive(true);
        UpdateActivitiesList();
        ClearActivityDetails();
    }
    
    public void CloseActivityUI()
    {
        activityCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateActivitiesList()
    {
        // 清空现有内容
        foreach (Transform child in activitiesContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示活动列表
        List<Activity> activities = ActivityManager.Instance.GetAllActivities();
        foreach (Activity activity in activities)
        {
            GameObject activityObj = Instantiate(activityPrefab, activitiesContent);
            Text[] texts = activityObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = activity.activityName;
                texts[1].text = activity.activityDescription;
                texts[2].text = activity.isActive ? "进行中" : "已结束";
            }
            
            // 设置活动状态
            Image[] images = activityObj.GetComponentsInChildren<Image>();
            if (images.Length >= 2)
            {
                // 活动状态图标
                if (activity.isActive)
                {
                    images[0].color = Color.green;
                }
                else
                {
                    images[0].color = Color.gray;
                }
                
                // 进度条
                images[1].fillAmount = activity.GetProgress();
            }
            
            // 添加点击事件
            Button button = activityObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectActivity(activity));
            }
        }
    }
    
    public void SelectActivity(Activity activity)
    {
        selectedActivity = activity;
        activityNameText.text = activity.activityName;
        activityDescriptionText.text = activity.activityDescription;
        activityTimeText.text = $"时间: {activity.startTime.ToString("yyyy-MM-dd HH:mm")} - {activity.endTime.ToString("yyyy-MM-dd HH:mm")}";
        activityProgressText.text = $"进度: {activity.progress}/{activity.targetProgress}";
        
        // 显示奖励
        string rewardText = "奖励: ";
        if (activity.reward.gold > 0) rewardText += $"{activity.reward.gold} 金币, ";
        if (activity.reward.experience > 0) rewardText += $"{activity.reward.experience} 经验, ";
        if (activity.reward.gems > 0) rewardText += $"{activity.reward.gems} 钻石, ";
        if (!string.IsNullOrEmpty(activity.reward.itemID)) rewardText += $"{activity.reward.itemCount}x {activity.reward.itemID}, ";
        if (!string.IsNullOrEmpty(activity.reward.skinID)) rewardText += $"{activity.reward.skinID}, ";
        if (!string.IsNullOrEmpty(activity.reward.heroID)) rewardText += $"{activity.reward.heroID}, ";
        
        // 移除末尾的逗号和空格
        if (rewardText.EndsWith(", "))
        {
            rewardText = rewardText.Substring(0, rewardText.Length - 2);
        }
        
        activityRewardText.text = rewardText;
        
        // 更新参与按钮状态
        if (activity.isActive)
        {
            joinActivityButton.gameObject.SetActive(true);
        }
        else
        {
            joinActivityButton.gameObject.SetActive(false);
        }
    }
    
    public void ClearActivityDetails()
    {
        selectedActivity = null;
        activityNameText.text = "";
        activityDescriptionText.text = "";
        activityTimeText.text = "";
        activityProgressText.text = "";
        activityRewardText.text = "";
        joinActivityButton.gameObject.SetActive(false);
    }
    
    public void JoinActivity()
    {
        if (selectedActivity != null && selectedActivity.isActive)
        {
            // 参与活动的逻辑
            Debug.Log($"参与活动: {selectedActivity.activityName}");
            ActivityManager.Instance.UpdateActivityProgress(selectedActivity.activityID, 1);
            UpdateActivitiesList();
            SelectActivity(selectedActivity);
        }
    }
}
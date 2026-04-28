using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ActivityManager : MonoBehaviour
{
    public static ActivityManager Instance { get; private set; }
    
    public ActivityManagerData activityData;
    
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
        LoadActivityData();
        
        if (activityData == null)
        {
            activityData = new ActivityManagerData();
        }
        
        // 确保当前玩家有活跃度数据
        EnsurePlayerActivityData();
    }
    
    private void EnsurePlayerActivityData()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        ActivityData data = activityData.GetActivityData(playerID);
        if (data == null)
        {
            data = new ActivityData(playerID);
            
            // 添加默认任务
            data.AddTask("task_login", "每日登录", "每天登录游戏", 10, 1);
            data.AddTask("task_match", "参与比赛", "参与1场比赛", 15, 1);
            data.AddTask("task_win", "赢得比赛", "赢得1场比赛", 20, 1);
            data.AddTask("task_kill", "击杀敌人", "击杀10个敌人", 10, 10);
            data.AddTask("task_assist", "获得助攻", "获得5次助攻", 5, 5);
            
            // 添加默认奖励
            data.AddReward("reward_20", 20, "Currency", "gold", 100);
            data.AddReward("reward_40", 40, "Item", "item_health_potion", 2);
            data.AddReward("reward_60", 60, "Currency", "gold", 200);
            data.AddReward("reward_80", 80, "Item", "item_mana_potion", 2);
            data.AddReward("reward_100", 100, "Currency", "gems", 50);
            
            activityData.AddActivityData(data);
            SaveActivityData();
        }
        else
        {
            // 检查是否需要重置每日任务
            string today = System.DateTime.Now.ToString("yyyy-MM-dd");
            if (data.lastResetTime != today)
            {
                data.Reset();
                SaveActivityData();
            }
        }
    }
    
    public void UpdateTaskProgress(string taskID, int progress)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        ActivityData data = activityData.GetActivityData(playerID);
        if (data != null)
        {
            ActivityTask task = data.tasks.Find(t => t.taskID == taskID);
            if (task != null && !task.isCompleted)
            {
                task.AddProgress(progress);
                if (task.isCompleted && !task.isClaimed)
                {
                    data.AddPoints(task.activityPoints);
                    task.Claim();
                }
                SaveActivityData();
            }
        }
    }
    
    public void ClaimReward(string rewardID)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        ActivityData data = activityData.GetActivityData(playerID);
        if (data != null)
        {
            ActivityRewards reward = data.rewards.Find(r => r.rewardID == rewardID);
            if (reward != null && !reward.isClaimed && data.currentPoints >= reward.requiredPoints)
            {
                reward.Claim();
                GrantReward(reward);
                SaveActivityData();
            }
        }
    }
    
    private void GrantReward(ActivityRewards reward)
    {
        switch (reward.rewardType)
        {
            case "Item":
                InventoryManager.Instance.AddItemToInventory(reward.rewardItemID, reward.quantity);
                break;
            case "Currency":
                if (reward.rewardItemID == "gold")
                {
                    ProfileManager.Instance.currentProfile.gold += reward.quantity;
                    ProfileManager.Instance.SaveProfile();
                }
                else if (reward.rewardItemID == "gems")
                {
                    ProfileManager.Instance.currentProfile.gems += reward.quantity;
                    ProfileManager.Instance.SaveProfile();
                }
                break;
        }
    }
    
    public ActivityData GetPlayerActivityData()
    {
        return activityData.GetActivityData(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public int GetCurrentActivityPoints()
    {
        ActivityData data = activityData.GetActivityData(ProfileManager.Instance.currentProfile.playerID);
        if (data != null)
        {
            return data.currentPoints;
        }
        return 0;
    }
    
    public List<ActivityTask> GetActivityTasks()
    {
        ActivityData data = activityData.GetActivityData(ProfileManager.Instance.currentProfile.playerID);
        if (data != null)
        {
            return data.tasks;
        }
        return new List<ActivityTask>();
    }
    
    public List<ActivityRewards> GetActivityRewards()
    {
        ActivityData data = activityData.GetActivityData(ProfileManager.Instance.currentProfile.playerID);
        if (data != null)
        {
            return data.rewards;
        }
        return new List<ActivityRewards>();
    }
    
    public void SaveActivityData()
    {
        string path = Application.dataPath + "/Data/activity_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, activityData);
        stream.Close();
    }
    
    public void LoadActivityData()
    {
        string path = Application.dataPath + "/Data/activity_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            activityData = (ActivityManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            activityData = new ActivityManagerData();
        }
    }
}
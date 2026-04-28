using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Activity
{
    public string ActivityID;
    public string ActivityName;
    public string Description;
    public int ActivityType;
    public DateTime StartTime;
    public DateTime EndTime;
    public bool IsActive;
    public string BannerImage;
    public string BackgroundImage;
    public string RuleDescription;
    public List<ActivityTask> Tasks;
    public List<ActivityReward> Rewards;
    public bool IsLimited;
    public int MaxParticipants;
    public int CurrentParticipants;

    public Activity(string activityID, string activityName, string description, int activityType, DateTime startTime, DateTime endTime)
    {
        ActivityID = activityID;
        ActivityName = activityName;
        Description = description;
        ActivityType = activityType;
        StartTime = startTime;
        EndTime = endTime;
        IsActive = false;
        BannerImage = "";
        BackgroundImage = "";
        RuleDescription = "";
        Tasks = new List<ActivityTask>();
        Rewards = new List<ActivityReward>();
        IsLimited = false;
        MaxParticipants = 0;
        CurrentParticipants = 0;
    }
}

[Serializable]
public class ActivityTask
{
    public string TaskID;
    public string ActivityID;
    public string TaskName;
    public string TaskDescription;
    public int TaskType;
    public int TargetValue;
    public int CurrentValue;
    public bool IsCompleted;
    public bool IsClaimed;
    public DateTime StartTime;
    public DateTime EndTime;
    public List<ActivityReward> TaskRewards;

    public ActivityTask(string taskID, string activityID, string taskName, string taskDescription, int taskType, int targetValue)
    {
        TaskID = taskID;
        ActivityID = activityID;
        TaskName = taskName;
        TaskDescription = taskDescription;
        TaskType = taskType;
        TargetValue = targetValue;
        CurrentValue = 0;
        IsCompleted = false;
        IsClaimed = false;
        StartTime = DateTime.Now;
        EndTime = DateTime.MaxValue;
        TaskRewards = new List<ActivityReward>();
    }
}

[Serializable]
public class ActivityReward
{
    public string RewardID;
    public string RewardName;
    public string RewardType;
    public int RewardValue;
    public int RequiredProgress;
    public bool IsClaimed;
    public string IconName;

    public ActivityReward(string rewardID, string rewardName, string rewardType, int rewardValue, int requiredProgress)
    {
        RewardID = rewardID;
        RewardName = rewardName;
        RewardType = rewardType;
        RewardValue = rewardValue;
        RequiredProgress = requiredProgress;
        IsClaimed = false;
        IconName = "";
    }
}

[Serializable]
public class PlayerActivityProgress
{
    public string PlayerID;
    public string ActivityID;
    public int TotalProgress;
    public int CurrentProgress;
    public List<ActivityTask> TaskProgress;
    public List<ActivityReward> ClaimedRewards;
    public bool IsParticipated;
    public DateTime JoinTime;
    public DateTime LastUpdateTime;

    public PlayerActivityProgress(string playerID, string activityID)
    {
        PlayerID = playerID;
        ActivityID = activityID;
        TotalProgress = 0;
        CurrentProgress = 0;
        TaskProgress = new List<ActivityTask>();
        ClaimedRewards = new List<ActivityReward>();
        IsParticipated = false;
        JoinTime = DateTime.Now;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class ActivitySystemData
{
    public List<Activity> Activities;
    public List<ActivityTask> AllTasks;
    public List<ActivityReward> AllRewards;
    public Dictionary<string, List<PlayerActivityProgress>> PlayerProgress;
    public List<string> FeaturedActivityIDs;
    public DateTime LastCleanupTime;

    public ActivitySystemData()
    {
        Activities = new List<Activity>();
        AllTasks = new List<ActivityTask>();
        AllRewards = new List<ActivityReward>();
        PlayerProgress = new Dictionary<string, List<PlayerActivityProgress>>();
        FeaturedActivityIDs = new List<string>();
        LastCleanupTime = DateTime.Now;
        InitializeDefaultActivities();
    }

    private void InitializeDefaultActivities()
    {
        Activity loginActivity = new Activity("activity_001", "每日登录", "每日登录领取奖励", 1, DateTime.Now, DateTime.Now.AddDays(30));
        loginActivity.IsActive = true;
        loginActivity.BannerImage = "login_banner";
        
        ActivityTask loginTask = new ActivityTask("task_001", loginActivity.ActivityID, "每日登录", "每天登录游戏", 1, 1);
        loginActivity.Tasks.Add(loginTask);
        AllTasks.Add(loginTask);
        
        ActivityReward loginReward = new ActivityReward("reward_001", "登录奖励", "currency", 100, 1);
        loginActivity.Rewards.Add(loginReward);
        AllRewards.Add(loginReward);
        
        Activities.Add(loginActivity);
        FeaturedActivityIDs.Add(loginActivity.ActivityID);
        
        Activity新手Activity = new Activity("activity_002", "新手成长", "新手任务奖励", 2, DateTime.Now, DateTime.Now.AddDays(7));
        新手Activity.IsActive = true;
        新手Activity.BannerImage = "newbie_banner";
        
        ActivityTask firstWinTask = new ActivityTask("task_002", 新手Activity.ActivityID, "首胜", "获得第一场胜利", 2, 1);
        新手Activity.Tasks.Add(firstWinTask);
        AllTasks.Add(firstWinTask);
        
        ActivityTask levelUpTask = new ActivityTask("task_003", 新手Activity.ActivityID, "等级提升", "提升到5级", 3, 5);
        新手Activity.Tasks.Add(levelUpTask);
        AllTasks.Add(levelUpTask);
        
        ActivityReward newbieReward = new ActivityReward("reward_002", "新手礼包", "skin", 1, 2);
        新手Activity.Rewards.Add(newbieReward);
        AllRewards.Add(newbieReward);
        
        Activities.Add(新手Activity);
    }

    public void AddActivity(Activity activity)
    {
        Activities.Add(activity);
    }

    public void AddTask(ActivityTask task)
    {
        AllTasks.Add(task);
    }

    public void AddReward(ActivityReward reward)
    {
        AllRewards.Add(reward);
    }

    public void AddPlayerProgress(string playerID, PlayerActivityProgress progress)
    {
        if (!PlayerProgress.ContainsKey(playerID))
        {
            PlayerProgress[playerID] = new List<PlayerActivityProgress>();
        }
        PlayerProgress[playerID].Add(progress);
    }
}

[Serializable]
public class ActivityEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string ActivityID;
    public string EventData;

    public ActivityEvent(string eventID, string eventType, string playerID, string activityID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        ActivityID = activityID;
        EventData = eventData;
    }
}

public class ActivitySystemDataManager
{
    private static ActivitySystemDataManager _instance;
    public static ActivitySystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ActivitySystemDataManager();
            }
            return _instance;
        }
    }

    public ActivitySystemData activityData;
    private List<ActivityEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private ActivitySystemDataManager()
    {
        activityData = new ActivitySystemData();
        recentEvents = new List<ActivityEvent>();
        LoadActivityData();
    }

    public void SaveActivityData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ActivitySystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, activityData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存活动系统数据失败: " + e.Message);
        }
    }

    public void LoadActivityData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ActivitySystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    activityData = (ActivitySystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载活动系统数据失败: " + e.Message);
            activityData = new ActivitySystemData();
        }
    }

    public void CreateActivityEvent(string eventType, string playerID, string activityID, string eventData)
    {
        string eventID = "activity_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ActivityEvent activityEvent = new ActivityEvent(eventID, eventType, playerID, activityID, eventData);
        recentEvents.Add(activityEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<ActivityEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Task
{
    public string TaskID;
    public string TaskName;
    public string Description;
    public int TaskType;
    public int TaskCategory;
    public int TargetValue;
    public int CurrentValue;
    public bool IsCompleted;
    public bool IsClaimed;
    public DateTime StartTime;
    public DateTime EndTime;
    public List<TaskReward> Rewards;
    public int Priority;
    public string IconName;
    public string ProgressDescription;

    public Task(string taskID, string taskName, string description, int taskType, int taskCategory, int targetValue)
    {
        TaskID = taskID;
        TaskName = taskName;
        Description = description;
        TaskType = taskType;
        TaskCategory = taskCategory;
        TargetValue = targetValue;
        CurrentValue = 0;
        IsCompleted = false;
        IsClaimed = false;
        StartTime = DateTime.Now;
        EndTime = DateTime.MaxValue;
        Rewards = new List<TaskReward>();
        Priority = 1;
        IconName = "";
        ProgressDescription = "";
    }
}

[Serializable]
public class TaskReward
{
    public string RewardID;
    public string RewardName;
    public string RewardType;
    public int RewardValue;
    public string IconName;

    public TaskReward(string rewardID, string rewardName, string rewardType, int rewardValue)
    {
        RewardID = rewardID;
        RewardName = rewardName;
        RewardType = rewardType;
        RewardValue = rewardValue;
        IconName = "";
    }
}

[Serializable]
public class PlayerTaskProgress
{
    public string PlayerID;
    public string TaskID;
    public int CurrentValue;
    public bool IsCompleted;
    public bool IsClaimed;
    public DateTime LastUpdateTime;
    public int CompletionCount;

    public PlayerTaskProgress(string playerID, string taskID)
    {
        PlayerID = playerID;
        TaskID = taskID;
        CurrentValue = 0;
        IsCompleted = false;
        IsClaimed = false;
        LastUpdateTime = DateTime.Now;
        CompletionCount = 0;
    }
}

[Serializable]
public class TaskCategory
{
    public string CategoryID;
    public string CategoryName;
    public int MaxTasks;
    public bool IsRepeatable;
    public int ResetInterval;

    public TaskCategory(string categoryID, string categoryName, int maxTasks, bool isRepeatable, int resetInterval)
    {
        CategoryID = categoryID;
        CategoryName = categoryName;
        MaxTasks = maxTasks;
        IsRepeatable = isRepeatable;
        ResetInterval = resetInterval;
    }
}

[Serializable]
public class DailyTaskData
{
    public List<Task> DailyTasks;
    public DateTime LastResetTime;
    public int CompletedTasks;
    public int TotalTasks;

    public DailyTaskData()
    {
        DailyTasks = new List<Task>();
        LastResetTime = DateTime.Now;
        CompletedTasks = 0;
        TotalTasks = 0;
    }
}

[Serializable]
public class WeeklyTaskData
{
    public List<Task> WeeklyTasks;
    public DateTime LastResetTime;
    public int CompletedTasks;
    public int TotalTasks;

    public WeeklyTaskData()
    {
        WeeklyTasks = new List<Task>();
        LastResetTime = DateTime.Now;
        CompletedTasks = 0;
        TotalTasks = 0;
    }
}

[Serializable]
public class AchievementTaskData
{
    public List<Task> AchievementTasks;
    public int CompletedAchievements;
    public int TotalAchievements;

    public AchievementTaskData()
    {
        AchievementTasks = new List<Task>();
        CompletedAchievements = 0;
        TotalAchievements = 0;
    }
}

[Serializable]
public class TaskSystemData
{
    public List<Task> AllTasks;
    public List<TaskReward> AllRewards;
    public List<TaskCategory> TaskCategories;
    public Dictionary<string, List<PlayerTaskProgress>> PlayerProgress;
    public Dictionary<string, DailyTaskData> PlayerDailyTasks;
    public Dictionary<string, WeeklyTaskData> PlayerWeeklyTasks;
    public Dictionary<string, AchievementTaskData> PlayerAchievementTasks;
    public DateTime LastSystemReset;

    public TaskSystemData()
    {
        AllTasks = new List<Task>();
        AllRewards = new List<TaskReward>();
        TaskCategories = new List<TaskCategory>();
        PlayerProgress = new Dictionary<string, List<PlayerTaskProgress>>();
        PlayerDailyTasks = new Dictionary<string, DailyTaskData>();
        PlayerWeeklyTasks = new Dictionary<string, WeeklyTaskData>();
        PlayerAchievementTasks = new Dictionary<string, AchievementTaskData>();
        LastSystemReset = DateTime.Now;
        InitializeDefaultCategories();
        InitializeDefaultTasks();
    }

    private void InitializeDefaultCategories()
    {
        TaskCategory dailyCategory = new TaskCategory("category_daily", "日常任务", 5, true, 1);
        TaskCategories.Add(dailyCategory);

        TaskCategory weeklyCategory = new TaskCategory("category_weekly", "周任务", 10, true, 7);
        TaskCategories.Add(weeklyCategory);

        TaskCategory achievementCategory = new TaskCategory("category_achievement", "成就任务", 50, false, 0);
        TaskCategories.Add(achievementCategory);
    }

    private void InitializeDefaultTasks()
    {
        
        Task dailyLogin = new Task("task_daily_login", "每日登录", "每天登录游戏", 1, 1, 1);
        TaskReward loginReward = new TaskReward("reward_login", "登录奖励", "currency", 50);
        dailyLogin.Rewards.Add(loginReward);
        AllTasks.Add(dailyLogin);
        AllRewards.Add(loginReward);

        Task dailyPlay = new Task("task_daily_play", "每日对战", "完成3场对战", 2, 1, 3);
        TaskReward playReward = new TaskReward("reward_play", "对战奖励", "exp", 100);
        dailyPlay.Rewards.Add(playReward);
        AllTasks.Add(dailyPlay);
        AllRewards.Add(playReward);

        Task weeklyWin = new Task("task_weekly_win", "每周首胜", "获得1场胜利", 3, 2, 1);
        TaskReward winReward = new TaskReward("reward_win", "胜利奖励", "currency", 200);
        weeklyWin.Rewards.Add(winReward);
        AllTasks.Add(weeklyWin);
        AllRewards.Add(winReward);

        Task achievementFirstWin = new Task("task_achievement_first_win", "首胜", "获得第一场胜利", 4, 3, 1);
        TaskReward firstWinReward = new TaskReward("reward_first_win", "首胜成就", "title", 1);
        achievementFirstWin.Rewards.Add(firstWinReward);
        AllTasks.Add(achievementFirstWin);
        AllRewards.Add(firstWinReward);
    }

    public void AddTask(Task task)
    {
        AllTasks.Add(task);
    }

    public void AddReward(TaskReward reward)
    {
        AllRewards.Add(reward);
    }

    public void AddTaskCategory(TaskCategory category)
    {
        TaskCategories.Add(category);
    }

    public void AddPlayerProgress(string playerID, PlayerTaskProgress progress)
    {
        if (!PlayerProgress.ContainsKey(playerID))
        {
            PlayerProgress[playerID] = new List<PlayerTaskProgress>();
        }
        PlayerProgress[playerID].Add(progress);
    }

    public void AddPlayerDailyTasks(string playerID, DailyTaskData dailyTasks)
    {
        PlayerDailyTasks[playerID] = dailyTasks;
    }

    public void AddPlayerWeeklyTasks(string playerID, WeeklyTaskData weeklyTasks)
    {
        PlayerWeeklyTasks[playerID] = weeklyTasks;
    }

    public void AddPlayerAchievementTasks(string playerID, AchievementTaskData achievementTasks)
    {
        PlayerAchievementTasks[playerID] = achievementTasks;
    }
}

[Serializable]
public class TaskEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string TaskID;
    public string EventData;

    public TaskEvent(string eventID, string eventType, string playerID, string taskID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        TaskID = taskID;
        EventData = eventData;
    }
}

public class TaskSystemDataManager
{
    private static TaskSystemDataManager _instance;
    public static TaskSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TaskSystemDataManager();
            }
            return _instance;
        }
    }

    public TaskSystemData taskData;
    private List<TaskEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private TaskSystemDataManager()
    {
        taskData = new TaskSystemData();
        recentEvents = new List<TaskEvent>();
        LoadTaskData();
    }

    public void SaveTaskData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TaskSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, taskData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存任务系统数据失败: " + e.Message);
        }
    }

    public void LoadTaskData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TaskSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    taskData = (TaskSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载任务系统数据失败: " + e.Message);
            taskData = new TaskSystemData();
        }
    }

    public void CreateTaskEvent(string eventType, string playerID, string taskID, string eventData)
    {
        string eventID = "task_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TaskEvent taskEvent = new TaskEvent(eventID, eventType, playerID, taskID, eventData);
        recentEvents.Add(taskEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<TaskEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
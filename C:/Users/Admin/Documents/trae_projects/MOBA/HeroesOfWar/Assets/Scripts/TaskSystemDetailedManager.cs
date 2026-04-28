using System;
using System.Collections.Generic;

public class TaskSystemDetailedManager
{
    private static TaskSystemDetailedManager _instance;
    public static TaskSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TaskSystemDetailedManager();
            }
            return _instance;
        }
    }

    private TaskSystemData taskData;
    private TaskSystemDataManager dataManager;

    private TaskSystemDetailedManager()
    {
        dataManager = TaskSystemDataManager.Instance;
        taskData = dataManager.taskData;
    }

    public void CreateTask(string taskName, string description, int taskType, int taskCategory, int targetValue, List<TaskReward> rewards)
    {
        string taskID = "task_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Task task = new Task(taskID, taskName, description, taskType, taskCategory, targetValue);
        task.Rewards = rewards;
        taskData.AddTask(task);
        foreach (TaskReward reward in rewards)
        {
            taskData.AddReward(reward);
        }
        dataManager.SaveTaskData();
        Debug.Log("创建任务成功: " + taskName);
    }

    public void UpdateTask(string taskID, string taskName, string description, int taskType, int taskCategory, int targetValue)
    {
        Task task = taskData.AllTasks.Find(t => t.TaskID == taskID);
        if (task != null)
        {
            task.TaskName = taskName;
            task.Description = description;
            task.TaskType = taskType;
            task.TaskCategory = taskCategory;
            task.TargetValue = targetValue;
            dataManager.SaveTaskData();
            Debug.Log("更新任务成功: " + taskName);
        }
    }

    public void DeleteTask(string taskID)
    {
        Task task = taskData.AllTasks.Find(t => t.TaskID == taskID);
        if (task != null)
        {
            taskData.AllTasks.Remove(task);
            dataManager.SaveTaskData();
            Debug.Log("删除任务成功: " + task.TaskName);
        }
    }

    public void AddRewardToTask(string taskID, TaskReward reward)
    {
        Task task = taskData.AllTasks.Find(t => t.TaskID == taskID);
        if (task != null)
        {
            task.Rewards.Add(reward);
            taskData.AddReward(reward);
            dataManager.SaveTaskData();
            Debug.Log("添加奖励到任务成功: " + reward.RewardName);
        }
    }

    public List<Task> GetTasksByCategory(int taskCategory)
    {
        return taskData.AllTasks.FindAll(t => t.TaskCategory == taskCategory);
    }

    public List<Task> GetTasksByType(int taskType)
    {
        return taskData.AllTasks.FindAll(t => t.TaskType == taskType);
    }

    public Task GetTask(string taskID)
    {
        return taskData.AllTasks.Find(t => t.TaskID == taskID);
    }

    public void UpdateTaskProgress(string playerID, string taskID, int progressValue)
    {
        PlayerTaskProgress progress = GetPlayerTaskProgress(playerID, taskID);
        if (progress == null)
        {
            progress = new PlayerTaskProgress(playerID, taskID);
            taskData.AddPlayerProgress(playerID, progress);
        }

        progress.CurrentValue += progressValue;
        Task task = GetTask(taskID);
        if (task != null && progress.CurrentValue >= task.TargetValue && !progress.IsCompleted)
        {
            progress.IsCompleted = true;
            progress.CompletionCount++;
            dataManager.CreateTaskEvent("task_complete", playerID, taskID, "完成任务: " + task.TaskName);
        }
        progress.LastUpdateTime = DateTime.Now;
        dataManager.SaveTaskData();
        Debug.Log("更新任务进度成功: " + taskID);
    }

    public void ClaimTaskReward(string playerID, string taskID)
    {
        PlayerTaskProgress progress = GetPlayerTaskProgress(playerID, taskID);
        if (progress != null && progress.IsCompleted && !progress.IsClaimed)
        {
            progress.IsClaimed = true;
            Task task = GetTask(taskID);
            if (task != null)
            {
                dataManager.CreateTaskEvent("task_reward_claim", playerID, taskID, "领取任务奖励: " + task.TaskName);
                Debug.Log("领取任务奖励成功: " + task.TaskName);
            }
            dataManager.SaveTaskData();
        }
    }

    public PlayerTaskProgress GetPlayerTaskProgress(string playerID, string taskID)
    {
        if (taskData.PlayerProgress.ContainsKey(playerID))
        {
            return taskData.PlayerProgress[playerID].Find(p => p.TaskID == taskID);
        }
        return null;
    }

    public List<PlayerTaskProgress> GetPlayerTasks(string playerID)
    {
        if (taskData.PlayerProgress.ContainsKey(playerID))
        {
            return taskData.PlayerProgress[playerID];
        }
        return new List<PlayerTaskProgress>();
    }

    public List<Task> GetPlayerActiveTasks(string playerID)
    {
        List<Task> activeTasks = new List<Task>();
        List<PlayerTaskProgress> playerTasks = GetPlayerTasks(playerID);
        foreach (PlayerTaskProgress progress in playerTasks)
        {
            if (!progress.IsCompleted || !progress.IsClaimed)
            {
                Task task = GetTask(progress.TaskID);
                if (task != null)
                {
                    activeTasks.Add(task);
                }
            }
        }
        return activeTasks;
    }

    public List<Task> GetPlayerCompletedTasks(string playerID)
    {
        List<Task> completedTasks = new List<Task>();
        List<PlayerTaskProgress> playerTasks = GetPlayerTasks(playerID);
        foreach (PlayerTaskProgress progress in playerTasks)
        {
            if (progress.IsCompleted)
            {
                Task task = GetTask(progress.TaskID);
                if (task != null)
                {
                    completedTasks.Add(task);
                }
            }
        }
        return completedTasks;
    }

    public List<Task> GetPlayerUnclaimedTasks(string playerID)
    {
        List<Task> unclaimedTasks = new List<Task>();
        List<PlayerTaskProgress> playerTasks = GetPlayerTasks(playerID);
        foreach (PlayerTaskProgress progress in playerTasks)
        {
            if (progress.IsCompleted && !progress.IsClaimed)
            {
                Task task = GetTask(progress.TaskID);
                if (task != null)
                {
                    unclaimedTasks.Add(task);
                }
            }
        }
        return unclaimedTasks;
    }

    public void InitializePlayerTasks(string playerID)
    {
        if (!taskData.PlayerProgress.ContainsKey(playerID))
        {
            taskData.PlayerProgress[playerID] = new List<PlayerTaskProgress>();
        }

        if (!taskData.PlayerDailyTasks.ContainsKey(playerID))
        {
            DailyTaskData dailyTaskData = new DailyTaskData();
            taskData.AddPlayerDailyTasks(playerID, dailyTaskData);
        }

        if (!taskData.PlayerWeeklyTasks.ContainsKey(playerID))
        {
            WeeklyTaskData weeklyTaskData = new WeeklyTaskData();
            taskData.AddPlayerWeeklyTasks(playerID, weeklyTaskData);
        }

        if (!taskData.PlayerAchievementTasks.ContainsKey(playerID))
        {
            AchievementTaskData achievementTaskData = new AchievementTaskData();
            taskData.AddPlayerAchievementTasks(playerID, achievementTaskData);
        }

        dataManager.SaveTaskData();
    }

    public void RefreshDailyTasks(string playerID)
    {
        if (taskData.PlayerDailyTasks.ContainsKey(playerID))
        {
            DailyTaskData dailyTaskData = taskData.PlayerDailyTasks[playerID];
            if (IsDailyTasksNeedRefresh(dailyTaskData))
            {
                dailyTaskData.DailyTasks.Clear();
                dailyTaskData.LastResetTime = DateTime.Now;
                dailyTaskData.CompletedTasks = 0;
                dailyTaskData.TotalTasks = 0;

                List<Task> dailyTasks = GetTasksByCategory(1);
                foreach (Task task in dailyTasks)
                {
                    dailyTaskData.DailyTasks.Add(task);
                    dailyTaskData.TotalTasks++;
                }

                dataManager.SaveTaskData();
                Debug.Log("刷新每日任务成功");
            }
        }
    }

    public void RefreshWeeklyTasks(string playerID)
    {
        if (taskData.PlayerWeeklyTasks.ContainsKey(playerID))
        {
            WeeklyTaskData weeklyTaskData = taskData.PlayerWeeklyTasks[playerID];
            if (IsWeeklyTasksNeedRefresh(weeklyTaskData))
            {
                weeklyTaskData.WeeklyTasks.Clear();
                weeklyTaskData.LastResetTime = DateTime.Now;
                weeklyTaskData.CompletedTasks = 0;
                weeklyTaskData.TotalTasks = 0;

                List<Task> weeklyTasks = GetTasksByCategory(2);
                foreach (Task task in weeklyTasks)
                {
                    weeklyTaskData.WeeklyTasks.Add(task);
                    weeklyTaskData.TotalTasks++;
                }

                dataManager.SaveTaskData();
                Debug.Log("刷新每周任务成功");
            }
        }
    }

    private bool IsDailyTasksNeedRefresh(DailyTaskData dailyTaskData)
    {
        return (DateTime.Now - dailyTaskData.LastResetTime).TotalDays >= 1;
    }

    private bool IsWeeklyTasksNeedRefresh(WeeklyTaskData weeklyTaskData)
    {
        return (DateTime.Now - weeklyTaskData.LastResetTime).TotalDays >= 7;
    }

    public List<Task> GetDailyTasks(string playerID)
    {
        RefreshDailyTasks(playerID);
        if (taskData.PlayerDailyTasks.ContainsKey(playerID))
        {
            return taskData.PlayerDailyTasks[playerID].DailyTasks;
        }
        return new List<Task>();
    }

    public List<Task> GetWeeklyTasks(string playerID)
    {
        RefreshWeeklyTasks(playerID);
        if (taskData.PlayerWeeklyTasks.ContainsKey(playerID))
        {
            return taskData.PlayerWeeklyTasks[playerID].WeeklyTasks;
        }
        return new List<Task>();
    }

    public List<Task> GetAchievementTasks(string playerID)
    {
        if (taskData.PlayerAchievementTasks.ContainsKey(playerID))
        {
            return taskData.PlayerAchievementTasks[playerID].AchievementTasks;
        }
        return new List<Task>();
    }

    public int GetTaskProgress(string playerID, string taskID)
    {
        PlayerTaskProgress progress = GetPlayerTaskProgress(playerID, taskID);
        if (progress != null)
        {
            return progress.CurrentValue;
        }
        return 0;
    }

    public bool IsTaskCompleted(string playerID, string taskID)
    {
        PlayerTaskProgress progress = GetPlayerTaskProgress(playerID, taskID);
        return progress != null && progress.IsCompleted;
    }

    public bool IsTaskClaimed(string playerID, string taskID)
    {
        PlayerTaskProgress progress = GetPlayerTaskProgress(playerID, taskID);
        return progress != null && progress.IsClaimed;
    }

    public void CompleteTask(string playerID, string taskID)
    {
        PlayerTaskProgress progress = GetPlayerTaskProgress(playerID, taskID);
        if (progress != null && !progress.IsCompleted)
        {
            progress.IsCompleted = true;
            progress.CompletionCount++;
            progress.LastUpdateTime = DateTime.Now;
            Task task = GetTask(taskID);
            if (task != null)
            {
                dataManager.CreateTaskEvent("task_complete", playerID, taskID, "完成任务: " + task.TaskName);
            }
            dataManager.SaveTaskData();
            Debug.Log("任务标记为完成: " + taskID);
        }
    }

    public void ResetTaskProgress(string playerID, string taskID)
    {
        PlayerTaskProgress progress = GetPlayerTaskProgress(playerID, taskID);
        if (progress != null)
        {
            progress.CurrentValue = 0;
            progress.IsCompleted = false;
            progress.IsClaimed = false;
            progress.LastUpdateTime = DateTime.Now;
            dataManager.SaveTaskData();
            Debug.Log("重置任务进度: " + taskID);
        }
    }

    public void CleanupExpiredTasks()
    {
        DateTime now = DateTime.Now;
        List<Task> expiredTasks = new List<Task>();
        foreach (Task task in taskData.AllTasks)
        {
            if (now > task.EndTime)
            {
                expiredTasks.Add(task);
            }
        }
        
        foreach (Task task in expiredTasks)
        {
            taskData.AllTasks.Remove(task);
        }
        
        if (expiredTasks.Count > 0)
        {
            dataManager.SaveTaskData();
            Debug.Log("清理过期任务成功: " + expiredTasks.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveTaskData();
    }

    public void LoadData()
    {
        dataManager.LoadTaskData();
    }

    public List<TaskEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
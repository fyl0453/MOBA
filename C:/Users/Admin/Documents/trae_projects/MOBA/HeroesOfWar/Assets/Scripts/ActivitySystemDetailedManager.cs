using System;
using System.Collections.Generic;

public class ActivitySystemDetailedManager
{
    private static ActivitySystemDetailedManager _instance;
    public static ActivitySystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ActivitySystemDetailedManager();
            }
            return _instance;
        }
    }

    private ActivitySystemData activityData;
    private ActivitySystemDataManager dataManager;

    private ActivitySystemDetailedManager()
    {
        dataManager = ActivitySystemDataManager.Instance;
        activityData = dataManager.activityData;
    }

    public void CreateActivity(string activityName, string description, int activityType, DateTime startTime, DateTime endTime, string bannerImage = "")
    {
        string activityID = "activity_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Activity activity = new Activity(activityID, activityName, description, activityType, startTime, endTime);
        activity.IsActive = true;
        activity.BannerImage = bannerImage;
        activityData.AddActivity(activity);
        dataManager.CreateActivityEvent("activity_create", "system", activityID, "创建活动: " + activityName);
        dataManager.SaveActivityData();
        Debug.Log("创建活动成功: " + activityName);
    }

    public void UpdateActivity(string activityID, string activityName, string description, int activityType, DateTime startTime, DateTime endTime)
    {
        Activity activity = activityData.Activities.Find(a => a.ActivityID == activityID);
        if (activity != null)
        {
            activity.ActivityName = activityName;
            activity.Description = description;
            activity.ActivityType = activityType;
            activity.StartTime = startTime;
            activity.EndTime = endTime;
            dataManager.SaveActivityData();
            Debug.Log("更新活动成功: " + activityName);
        }
    }

    public void ActivateActivity(string activityID)
    {
        Activity activity = activityData.Activities.Find(a => a.ActivityID == activityID);
        if (activity != null)
        {
            activity.IsActive = true;
            dataManager.CreateActivityEvent("activity_activate", "system", activityID, "激活活动: " + activity.ActivityName);
            dataManager.SaveActivityData();
            Debug.Log("激活活动成功: " + activity.ActivityName);
        }
    }

    public void DeactivateActivity(string activityID)
    {
        Activity activity = activityData.Activities.Find(a => a.ActivityID == activityID);
        if (activity != null)
        {
            activity.IsActive = false;
            dataManager.CreateActivityEvent("activity_deactivate", "system", activityID, "停用活动: " + activity.ActivityName);
            dataManager.SaveActivityData();
            Debug.Log("停用活动成功: " + activity.ActivityName);
        }
    }

    public void AddTaskToActivity(string activityID, string taskName, string taskDescription, int taskType, int targetValue)
    {
        Activity activity = activityData.Activities.Find(a => a.ActivityID == activityID);
        if (activity != null)
        {
            string taskID = "task_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            ActivityTask task = new ActivityTask(taskID, activityID, taskName, taskDescription, taskType, targetValue);
            activity.Tasks.Add(task);
            activityData.AddTask(task);
            dataManager.SaveActivityData();
            Debug.Log("添加任务到活动成功: " + taskName);
        }
    }

    public void AddRewardToActivity(string activityID, string rewardName, string rewardType, int rewardValue, int requiredProgress)
    {
        Activity activity = activityData.Activities.Find(a => a.ActivityID == activityID);
        if (activity != null)
        {
            string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            ActivityReward reward = new ActivityReward(rewardID, rewardName, rewardType, rewardValue, requiredProgress);
            activity.Rewards.Add(reward);
            activityData.AddReward(reward);
            dataManager.SaveActivityData();
            Debug.Log("添加奖励到活动成功: " + rewardName);
        }
    }

    public List<Activity> GetActiveActivities()
    {
        List<Activity> activeActivities = new List<Activity>();
        DateTime now = DateTime.Now;
        foreach (Activity activity in activityData.Activities)
        {
            if (activity.IsActive && now >= activity.StartTime && now <= activity.EndTime)
            {
                activeActivities.Add(activity);
            }
        }
        return activeActivities;
    }

    public List<Activity> GetUpcomingActivities()
    {
        List<Activity> upcomingActivities = new List<Activity>();
        DateTime now = DateTime.Now;
        foreach (Activity activity in activityData.Activities)
        {
            if (activity.IsActive && now < activity.StartTime)
            {
                upcomingActivities.Add(activity);
            }
        }
        return upcomingActivities;
    }

    public List<Activity> GetFeaturedActivities()
    {
        List<Activity> featuredActivities = new List<Activity>();
        foreach (string activityID in activityData.FeaturedActivityIDs)
        {
            Activity activity = activityData.Activities.Find(a => a.ActivityID == activityID);
            if (activity != null && activity.IsActive)
            {
                featuredActivities.Add(activity);
            }
        }
        return featuredActivities;
    }

    public Activity GetActivity(string activityID)
    {
        return activityData.Activities.Find(a => a.ActivityID == activityID);
    }

    public void JoinActivity(string playerID, string activityID)
    {
        Activity activity = GetActivity(activityID);
        if (activity == null || !activity.IsActive)
        {
            Debug.LogError("活动不存在或已结束: " + activityID);
            return;
        }

        if (activity.IsLimited && activity.CurrentParticipants >= activity.MaxParticipants)
        {
            Debug.LogError("活动参与人数已满: " + activity.ActivityName);
            return;
        }

        PlayerActivityProgress progress = GetPlayerActivityProgress(playerID, activityID);
        if (progress == null)
        {
            progress = new PlayerActivityProgress(playerID, activityID);
            progress.IsParticipated = true;
            activityData.AddPlayerProgress(playerID, progress);
            
            foreach (ActivityTask task in activity.Tasks)
            {
                ActivityTask playerTask = new ActivityTask(task.TaskID, task.ActivityID, task.TaskName, task.TaskDescription, task.TaskType, task.TargetValue);
                progress.TaskProgress.Add(playerTask);
            }

            activity.CurrentParticipants++;
            dataManager.CreateActivityEvent("activity_join", playerID, activityID, "参与活动: " + activity.ActivityName);
            dataManager.SaveActivityData();
            Debug.Log("参与活动成功: " + activity.ActivityName);
        }
    }

    public void LeaveActivity(string playerID, string activityID)
    {
        PlayerActivityProgress progress = GetPlayerActivityProgress(playerID, activityID);
        if (progress != null && progress.IsParticipated)
        {
            progress.IsParticipated = false;
            Activity activity = GetActivity(activityID);
            if (activity != null && activity.CurrentParticipants > 0)
            {
                activity.CurrentParticipants--;
            }
            dataManager.CreateActivityEvent("activity_leave", playerID, activityID, "离开活动");
            dataManager.SaveActivityData();
            Debug.Log("离开活动成功");
        }
    }

    public void UpdateTaskProgress(string playerID, string activityID, string taskID, int progressValue)
    {
        PlayerActivityProgress activityProgress = GetPlayerActivityProgress(playerID, activityID);
        if (activityProgress != null)
        {
            ActivityTask task = activityProgress.TaskProgress.Find(t => t.TaskID == taskID);
            if (task != null)
            {
                task.CurrentValue += progressValue;
                if (task.CurrentValue >= task.TargetValue && !task.IsCompleted)
                {
                    task.IsCompleted = true;
                    activityProgress.CurrentProgress++;
                    dataManager.CreateActivityEvent("task_complete", playerID, activityID, "完成任务: " + task.TaskName);
                }
                activityProgress.LastUpdateTime = DateTime.Now;
                dataManager.SaveActivityData();
                Debug.Log("更新任务进度成功: " + task.TaskName);
            }
        }
    }

    public void ClaimTaskReward(string playerID, string activityID, string taskID)
    {
        PlayerActivityProgress activityProgress = GetPlayerActivityProgress(playerID, activityID);
        if (activityProgress != null)
        {
            ActivityTask task = activityProgress.TaskProgress.Find(t => t.TaskID == taskID);
            if (task != null && task.IsCompleted && !task.IsClaimed)
            {
                task.IsClaimed = true;
                dataManager.CreateActivityEvent("task_reward_claim", playerID, activityID, "领取任务奖励: " + task.TaskName);
                dataManager.SaveActivityData();
                Debug.Log("领取任务奖励成功: " + task.TaskName);
            }
        }
    }

    public void ClaimActivityReward(string playerID, string activityID, string rewardID)
    {
        PlayerActivityProgress activityProgress = GetPlayerActivityProgress(playerID, activityID);
        if (activityProgress != null)
        {
            Activity activity = GetActivity(activityID);
            if (activity != null)
            {
                ActivityReward reward = activity.Rewards.Find(r => r.RewardID == rewardID);
                if (reward != null && activityProgress.CurrentProgress >= reward.RequiredProgress && !reward.IsClaimed)
                {
                    reward.IsClaimed = true;
                    activityProgress.ClaimedRewards.Add(reward);
                    dataManager.CreateActivityEvent("activity_reward_claim", playerID, activityID, "领取活动奖励: " + reward.RewardName);
                    dataManager.SaveActivityData();
                    Debug.Log("领取活动奖励成功: " + reward.RewardName);
                }
            }
        }
    }

    public List<ActivityTask> GetPlayerTasks(string playerID, string activityID)
    {
        PlayerActivityProgress progress = GetPlayerActivityProgress(playerID, activityID);
        if (progress != null)
        {
            return progress.TaskProgress;
        }
        return new List<ActivityTask>();
    }

    public List<ActivityReward> GetPlayerRewards(string playerID, string activityID)
    {
        PlayerActivityProgress progress = GetPlayerActivityProgress(playerID, activityID);
        if (progress != null)
        {
            return progress.ClaimedRewards;
        }
        return new List<ActivityReward>();
    }

    public int GetPlayerProgress(string playerID, string activityID)
    {
        PlayerActivityProgress progress = GetPlayerActivityProgress(playerID, activityID);
        if (progress != null)
        {
            return progress.CurrentProgress;
        }
        return 0;
    }

    public bool IsActivityParticipated(string playerID, string activityID)
    {
        PlayerActivityProgress progress = GetPlayerActivityProgress(playerID, activityID);
        return progress != null && progress.IsParticipated;
    }

    public void SetActivityAsFeatured(string activityID, bool isFeatured)
    {
        if (isFeatured && !activityData.FeaturedActivityIDs.Contains(activityID))
        {
            activityData.FeaturedActivityIDs.Add(activityID);
        }
        else if (!isFeatured && activityData.FeaturedActivityIDs.Contains(activityID))
        {
            activityData.FeaturedActivityIDs.Remove(activityID);
        }
        dataManager.SaveActivityData();
    }

    public void CleanupExpiredActivities()
    {
        DateTime now = DateTime.Now;
        List<Activity> expiredActivities = new List<Activity>();
        
        foreach (Activity activity in activityData.Activities)
        {
            if (now > activity.EndTime && activity.IsActive)
            {
                activity.IsActive = false;
                expiredActivities.Add(activity);
            }
        }
        
        if (expiredActivities.Count > 0)
        {
            dataManager.CreateActivityEvent("activity_cleanup", "system", "", "清理过期活动: " + expiredActivities.Count);
            dataManager.SaveActivityData();
            Debug.Log("清理过期活动成功: " + expiredActivities.Count);
        }
    }

    private PlayerActivityProgress GetPlayerActivityProgress(string playerID, string activityID)
    {
        if (activityData.PlayerProgress.ContainsKey(playerID))
        {
            return activityData.PlayerProgress[playerID].Find(p => p.ActivityID == activityID);
        }
        return null;
    }

    public List<PlayerActivityProgress> GetPlayerActivities(string playerID)
    {
        if (activityData.PlayerProgress.ContainsKey(playerID))
        {
            return activityData.PlayerProgress[playerID];
        }
        return new List<PlayerActivityProgress>();
    }

    public void SaveData()
    {
        dataManager.SaveActivityData();
    }

    public void LoadData()
    {
        dataManager.LoadActivityData();
    }

    public List<ActivityEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
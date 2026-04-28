[System.Serializable]
public class NewbieGuideDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<GuideModule> guideModules;
    public List<PlayerGuideProgress> playerProgresses;
    public List<GuideTask> guideTasks;
    public List<GuideReward> guideRewards;
    
    public NewbieGuideDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        guideModules = new List<GuideModule>();
        playerProgresses = new List<PlayerGuideProgress>();
        guideTasks = new List<GuideTask>();
        guideRewards = new List<GuideReward>();
    }
    
    public void AddGuideModule(GuideModule module)
    {
        guideModules.Add(module);
    }
    
    public void AddPlayerProgress(PlayerGuideProgress progress)
    {
        playerProgresses.Add(progress);
    }
    
    public void AddGuideTask(GuideTask task)
    {
        guideTasks.Add(task);
    }
    
    public void AddGuideReward(GuideReward reward)
    {
        guideRewards.Add(reward);
    }
    
    public GuideModule GetGuideModule(string moduleID)
    {
        return guideModules.Find(m => m.moduleID == moduleID);
    }
    
    public PlayerGuideProgress GetPlayerProgress(string playerID)
    {
        return playerProgresses.Find(p => p.playerID == playerID);
    }
    
    public GuideTask GetGuideTask(string taskID)
    {
        return guideTasks.Find(t => t.taskID == taskID);
    }
    
    public GuideReward GetGuideReward(string rewardID)
    {
        return guideRewards.Find(r => r.rewardID == rewardID);
    }
    
    public List<GuideModule> GetAllGuideModules()
    {
        return guideModules;
    }
    
    public List<GuideTask> GetTasksByModule(string moduleID)
    {
        return guideTasks.FindAll(t => t.moduleID == moduleID);
    }
    
    public List<GuideReward> GetRewardsByTask(string taskID)
    {
        return guideRewards.FindAll(r => r.taskID == taskID);
    }
}

[System.Serializable]
public class GuideModule
{
    public string moduleID;
    public string moduleName;
    public string moduleDescription;
    public int order;
    public bool isEnabled;
    public string prerequisiteModuleID;
    public List<string> taskIDs;
    
    public GuideModule(string id, string name, string desc, int order, string prerequisiteID = "")
    {
        moduleID = id;
        moduleName = name;
        moduleDescription = desc;
        this.order = order;
        isEnabled = true;
        prerequisiteModuleID = prerequisiteID;
        taskIDs = new List<string>();
    }
    
    public void AddTask(string taskID)
    {
        taskIDs.Add(taskID);
    }
    
    public void UpdateModule(string name, string desc, int order)
    {
        moduleName = name;
        moduleDescription = desc;
        this.order = order;
    }
}

[System.Serializable]
public class PlayerGuideProgress
{
    public string playerID;
    public List<ModuleProgress> moduleProgresses;
    public List<TaskProgress> taskProgresses;
    public List<RewardProgress> rewardProgresses;
    public int totalCompletedTasks;
    public string lastUpdateDate;
    
    public PlayerGuideProgress(string playerID)
    {
        this.playerID = playerID;
        moduleProgresses = new List<ModuleProgress>();
        taskProgresses = new List<TaskProgress>();
        rewardProgresses = new List<RewardProgress>();
        totalCompletedTasks = 0;
        lastUpdateDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddModuleProgress(ModuleProgress progress)
    {
        moduleProgresses.Add(progress);
    }
    
    public void AddTaskProgress(TaskProgress progress)
    {
        taskProgresses.Add(progress);
    }
    
    public void AddRewardProgress(RewardProgress progress)
    {
        rewardProgresses.Add(progress);
    }
    
    public void UpdateLastUpdateDate()
    {
        lastUpdateDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void IncrementCompletedTasks()
    {
        totalCompletedTasks++;
    }
}

[System.Serializable]
public class ModuleProgress
{
    public string moduleID;
    public string moduleName;
    public string status;
    public int completedTasks;
    public int totalTasks;
    public string startDate;
    public string completeDate;
    
    public ModuleProgress(string moduleID, string moduleName, int totalTasks)
    {
        this.moduleID = moduleID;
        this.moduleName = moduleName;
        status = "InProgress";
        completedTasks = 0;
        this.totalTasks = totalTasks;
        startDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        completeDate = "";
    }
    
    public void CompleteTask()
    {
        completedTasks++;
        if (completedTasks >= totalTasks)
        {
            status = "Completed";
            completeDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}

[System.Serializable]
public class TaskProgress
{
    public string taskID;
    public string taskName;
    public string status;
    public int currentProgress;
    public int targetProgress;
    public string startDate;
    public string completeDate;
    
    public TaskProgress(string taskID, string taskName, int targetProgress)
    {
        this.taskID = taskID;
        this.taskName = taskName;
        status = "InProgress";
        currentProgress = 0;
        this.targetProgress = targetProgress;
        startDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        completeDate = "";
    }
    
    public void UpdateProgress(int progress)
    {
        currentProgress = progress;
        if (currentProgress >= targetProgress)
        {
            currentProgress = targetProgress;
            status = "Completed";
            completeDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}

[System.Serializable]
public class RewardProgress
{
    public string rewardID;
    public string rewardName;
    public string status;
    public string claimDate;
    
    public RewardProgress(string rewardID, string rewardName)
    {
        this.rewardID = rewardID;
        this.rewardName = rewardName;
        status = "Available";
        claimDate = "";
    }
    
    public void Claim()
    {
        status = "Claimed";
        claimDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class GuideTask
{
    public string taskID;
    public string moduleID;
    public string taskName;
    public string taskDescription;
    public string taskType;
    public int targetProgress;
    public bool isEnabled;
    public string prerequisiteTaskID;
    public List<string> rewardIDs;
    
    public GuideTask(string id, string moduleID, string name, string desc, string type, int targetProgress, string prerequisiteTaskID = "")
    {
        taskID = id;
        this.moduleID = moduleID;
        taskName = name;
        taskDescription = desc;
        taskType = type;
        this.targetProgress = targetProgress;
        isEnabled = true;
        this.prerequisiteTaskID = prerequisiteTaskID;
        rewardIDs = new List<string>();
    }
    
    public void AddReward(string rewardID)
    {
        rewardIDs.Add(rewardID);
    }
    
    public void UpdateTask(string name, string desc, int targetProgress)
    {
        taskName = name;
        taskDescription = desc;
        this.targetProgress = targetProgress;
    }
}

[System.Serializable]
public class GuideReward
{
    public string rewardID;
    public string taskID;
    public string rewardName;
    public string rewardType;
    public string description;
    public int quantity;
    public bool isEnabled;
    
    public GuideReward(string id, string taskID, string name, string type, string desc, int quantity)
    {
        rewardID = id;
        this.taskID = taskID;
        rewardName = name;
        rewardType = type;
        description = desc;
        this.quantity = quantity;
        isEnabled = true;
    }
    
    public void UpdateReward(string name, string desc, int quantity)
    {
        rewardName = name;
        description = desc;
        this.quantity = quantity;
    }
}

[System.Serializable]
public class NewbieGuideDetailedManagerData
{
    public NewbieGuideDetailed system;
    
    public NewbieGuideDetailedManagerData()
    {
        system = new NewbieGuideDetailed("newbie_guide_detailed", "新手引导详细系统", "管理新手引导的详细功能，包括个性化引导和进度跟踪");
    }
}
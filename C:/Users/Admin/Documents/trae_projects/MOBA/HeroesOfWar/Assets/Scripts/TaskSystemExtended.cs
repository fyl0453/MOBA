[System.Serializable]
public class TaskCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    public List<ExtendedTask> tasks;
    
    public TaskCategory(string id, string name, string desc)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = desc;
        tasks = new List<ExtendedTask>();
    }
    
    public void AddTask(ExtendedTask task)
    {
        tasks.Add(task);
    }
    
    public int GetCompletedCount()
    {
        return tasks.FindAll(t => t.isCompleted).Count;
    }
    
    public int GetTotalCount()
    {
        return tasks.Count;
    }
}

[System.Serializable]
public class ExtendedTask
{
    public string taskID;
    public string taskName;
    public string taskDescription;
    public string taskType;
    public int requiredProgress;
    public int currentProgress;
    public bool isCompleted;
    public bool isClaimed;
    public List<TaskReward> rewards;
    public string startTime;
    public string endTime;
    public bool isTimeLimited;
    public int priority;
    
    public ExtendedTask(string id, string name, string desc, string type, int reqProgress)
    {
        taskID = id;
        taskName = name;
        taskDescription = desc;
        taskType = type;
        requiredProgress = reqProgress;
        currentProgress = 0;
        isCompleted = false;
        isClaimed = false;
        rewards = new List<TaskReward>();
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd");
        endTime = "";
        isTimeLimited = false;
        priority = 0;
    }
    
    public void AddReward(TaskReward reward)
    {
        rewards.Add(reward);
    }
    
    public void AddProgress(int progress)
    {
        if (!isCompleted)
        {
            currentProgress += progress;
            if (currentProgress >= requiredProgress)
            {
                currentProgress = requiredProgress;
                isCompleted = true;
            }
        }
    }
    
    public void Claim()
    {
        isClaimed = true;
    }
    
    public bool IsExpired()
    {
        if (!isTimeLimited || string.IsNullOrEmpty(endTime))
            return false;
        
        System.DateTime endDate;
        if (System.DateTime.TryParse(endTime, out endDate))
        {
            return System.DateTime.Now > endDate;
        }
        return false;
    }
}

[System.Serializable]
public class TaskManagerExtendedData
{
    public List<TaskCategory> categories;
    public List<ExtendedTask> activeTasks;
    public List<ExtendedTask> completedTasks;
    public List<ExtendedTask> expiredTasks;
    
    public TaskManagerExtendedData()
    {
        categories = new List<TaskCategory>();
        activeTasks = new List<ExtendedTask>();
        completedTasks = new List<ExtendedTask>();
        expiredTasks = new List<ExtendedTask>();
    }
    
    public void AddCategory(TaskCategory category)
    {
        categories.Add(category);
    }
    
    public void AddTask(ExtendedTask task)
    {
        activeTasks.Add(task);
    }
    
    public void MoveToCompleted(ExtendedTask task)
    {
        activeTasks.Remove(task);
        completedTasks.Add(task);
    }
    
    public void MoveToExpired(ExtendedTask task)
    {
        activeTasks.Remove(task);
        expiredTasks.Add(task);
    }
    
    public TaskCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public ExtendedTask GetTask(string taskID)
    {
        ExtendedTask task = activeTasks.Find(t => t.taskID == taskID);
        if (task == null)
        {
            task = completedTasks.Find(t => t.taskID == taskID);
        }
        if (task == null)
        {
            task = expiredTasks.Find(t => t.taskID == taskID);
        }
        return task;
    }
    
    public List<ExtendedTask> GetTasksByType(string type)
    {
        return activeTasks.FindAll(t => t.taskType == type);
    }
    
    public List<ExtendedTask> GetTasksByCategory(string categoryID)
    {
        TaskCategory category = GetCategory(categoryID);
        if (category != null)
        {
            return activeTasks.FindAll(t => category.tasks.Exists(ct => ct.taskID == t.taskID));
        }
        return new List<ExtendedTask>();
    }
}

[System.Serializable]
public class TaskProgress
{
    public string playerID;
    public Dictionary<string, int> progressData;
    public Dictionary<string, bool> completionData;
    public Dictionary<string, bool> claimedData;
    
    public TaskProgress(string player)
    {
        playerID = player;
        progressData = new Dictionary<string, int>();
        completionData = new Dictionary<string, bool>();
        claimedData = new Dictionary<string, bool>();
    }
    
    public void UpdateProgress(string taskID, int progress)
    {
        if (progressData.ContainsKey(taskID))
        {
            progressData[taskID] = progress;
        }
        else
        {
            progressData.Add(taskID, progress);
        }
    }
    
    public void MarkAsCompleted(string taskID)
    {
        completionData[taskID] = true;
    }
    
    public void MarkAsClaimed(string taskID)
    {
        claimedData[taskID] = true;
    }
    
    public int GetProgress(string taskID)
    {
        return progressData.ContainsKey(taskID) ? progressData[taskID] : 0;
    }
    
    public bool IsCompleted(string taskID)
    {
        return completionData.ContainsKey(taskID) && completionData[taskID];
    }
    
    public bool IsClaimed(string taskID)
    {
        return claimedData.ContainsKey(taskID) && claimedData[taskID];
    }
}
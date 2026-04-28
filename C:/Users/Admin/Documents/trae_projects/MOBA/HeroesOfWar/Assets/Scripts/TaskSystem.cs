[System.Serializable]
public class Task
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
    public string startDate;
    public string endDate;
    public int priority;
    public bool isActive;
    
    public Task(string id, string name, string desc, string type, int reqProgress)
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
        startDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        endDate = System.DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
        priority = 0;
        isActive = true;
    }
    
    public void AddProgress(int amount)
    {
        if (!isCompleted && isActive)
        {
            currentProgress += amount;
            if (currentProgress >= requiredProgress)
            {
                currentProgress = requiredProgress;
                isCompleted = true;
            }
        }
    }
    
    public void AddReward(string rewardType, string rewardItemID, int quantity)
    {
        TaskReward reward = new TaskReward(rewardType, rewardItemID, quantity);
        rewards.Add(reward);
    }
    
    public void ClaimRewards()
    {
        isClaimed = true;
    }
    
    public bool IsExpired()
    {
        System.DateTime endDateValue;
        if (System.DateTime.TryParse(endDate, out endDateValue))
        {
            return System.DateTime.Now > endDateValue;
        }
        return false;
    }
    
    public void Deactivate()
    {
        isActive = false;
    }
}

[System.Serializable]
public class TaskReward
{
    public string rewardType;
    public string rewardItemID;
    public int quantity;
    
    public TaskReward(string type, string itemID, int qty)
    {
        rewardType = type;
        rewardItemID = itemID;
        quantity = qty;
    }
}

[System.Serializable]
public class TaskList
{
    public string playerID;
    public List<Task> tasks;
    public int maxTasks;
    
    public TaskList(string id, int max = 20)
    {
        playerID = id;
        tasks = new List<Task>();
        maxTasks = max;
    }
    
    public void AddTask(Task task)
    {
        if (tasks.Count < maxTasks)
        {
            tasks.Add(task);
        }
    }
    
    public void RemoveTask(string taskID)
    {
        tasks.RemoveAll(t => t.taskID == taskID);
    }
    
    public void UpdateTaskProgress(string taskID, int progress)
    {
        Task task = tasks.Find(t => t.taskID == taskID);
        if (task != null)
        {
            task.AddProgress(progress);
        }
    }
    
    public Task GetTask(string taskID)
    {
        return tasks.Find(t => t.taskID == taskID);
    }
    
    public List<Task> GetActiveTasks()
    {
        return tasks.FindAll(t => t.isActive && !t.isCompleted);
    }
    
    public List<Task> GetCompletedTasks()
    {
        return tasks.FindAll(t => t.isCompleted && !t.isClaimed);
    }
    
    public List<Task> GetClaimedTasks()
    {
        return tasks.FindAll(t => t.isClaimed);
    }
    
    public List<Task> GetTasksByType(string type)
    {
        return tasks.FindAll(t => t.taskType == type);
    }
    
    public void RemoveExpiredTasks()
    {
        tasks.RemoveAll(t => t.IsExpired() && !t.isClaimed);
    }
}

[System.Serializable]
public class TaskCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    public List<string> taskIDs;
    
    public TaskCategory(string id, string name, string desc)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = desc;
        taskIDs = new List<string>();
    }
    
    public void AddTask(string taskID)
    {
        if (!taskIDs.Contains(taskID))
        {
            taskIDs.Add(taskID);
        }
    }
    
    public void RemoveTask(string taskID)
    {
        taskIDs.Remove(taskID);
    }
}
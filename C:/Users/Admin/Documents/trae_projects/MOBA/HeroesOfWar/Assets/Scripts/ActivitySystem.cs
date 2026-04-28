[System.Serializable]
public class ActivityTask
{
    public string taskID;
    public string taskName;
    public string taskDescription;
    public int activityPoints;
    public int requiredProgress;
    public int currentProgress;
    public bool isCompleted;
    public bool isClaimed;
    
    public ActivityTask(string id, string name, string desc, int points, int reqProgress)
    {
        taskID = id;
        taskName = name;
        taskDescription = desc;
        activityPoints = points;
        requiredProgress = reqProgress;
        currentProgress = 0;
        isCompleted = false;
        isClaimed = false;
    }
    
    public void AddProgress(int amount)
    {
        if (!isCompleted)
        {
            currentProgress += amount;
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
}

[System.Serializable]
public class ActivityRewards
{
    public string rewardID;
    public int requiredPoints;
    public string rewardType;
    public string rewardItemID;
    public int quantity;
    public bool isClaimed;
    
    public ActivityRewards(string id, int points, string type, string itemID, int qty)
    {
        rewardID = id;
        requiredPoints = points;
        rewardType = type;
        rewardItemID = itemID;
        quantity = qty;
        isClaimed = false;
    }
    
    public void Claim()
    {
        isClaimed = true;
    }
}

[System.Serializable]
public class ActivityData
{
    public string playerID;
    public int currentPoints;
    public List<ActivityTask> tasks;
    public List<ActivityRewards> rewards;
    public string lastResetTime;
    
    public ActivityData(string player)
    {
        playerID = player;
        currentPoints = 0;
        tasks = new List<ActivityTask>();
        rewards = new List<ActivityRewards>();
        lastResetTime = System.DateTime.Now.ToString("yyyy-MM-dd");
    }
    
    public void AddTask(string taskID, string taskName, string taskDescription, int points, int reqProgress)
    {
        ActivityTask task = new ActivityTask(taskID, taskName, taskDescription, points, reqProgress);
        tasks.Add(task);
    }
    
    public void AddReward(string rewardID, int points, string type, string itemID, int quantity)
    {
        ActivityRewards reward = new ActivityRewards(rewardID, points, type, itemID, quantity);
        rewards.Add(reward);
    }
    
    public void AddPoints(int points)
    {
        currentPoints += points;
    }
    
    public void Reset()
    {
        currentPoints = 0;
        foreach (ActivityTask task in tasks)
        {
            task.currentProgress = 0;
            task.isCompleted = false;
            task.isClaimed = false;
        }
        foreach (ActivityRewards reward in rewards)
        {
            reward.isClaimed = false;
        }
        lastResetTime = System.DateTime.Now.ToString("yyyy-MM-dd");
    }
}

[System.Serializable]
public class ActivityManagerData
{
    public List<ActivityData> activityDataList;
    
    public ActivityManagerData()
    {
        activityDataList = new List<ActivityData>();
    }
    
    public void AddActivityData(ActivityData data)
    {
        activityDataList.Add(data);
    }
    
    public ActivityData GetActivityData(string playerID)
    {
        return activityDataList.Find(d => d.playerID == playerID);
    }
}
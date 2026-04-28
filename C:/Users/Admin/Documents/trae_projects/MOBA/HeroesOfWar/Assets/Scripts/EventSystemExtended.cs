[System.Serializable]
public class EventSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Event> events;
    public List<EventTask> eventTasks;
    public List<EventReward> eventRewards;
    
    public EventSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        events = new List<Event>();
        eventTasks = new List<EventTask>();
        eventRewards = new List<EventReward>();
    }
    
    public void AddEvent(Event gameEvent)
    {
        events.Add(gameEvent);
    }
    
    public void AddEventTask(EventTask task)
    {
        eventTasks.Add(task);
    }
    
    public void AddEventReward(EventReward reward)
    {
        eventRewards.Add(reward);
    }
    
    public Event GetEvent(string eventID)
    {
        return events.Find(e => e.eventID == eventID);
    }
    
    public List<EventTask> GetTasksByEvent(string eventID)
    {
        return eventTasks.FindAll(t => t.eventID == eventID);
    }
    
    public EventReward GetReward(string rewardID)
    {
        return eventRewards.Find(r => r.rewardID == rewardID);
    }
    
    public List<Event> GetActiveEvents()
    {
        string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return events.FindAll(e => e.startTime <= currentTime && e.endTime >= currentTime && e.isEnabled);
    }
    
    public List<Event> GetUpcomingEvents()
    {
        string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return events.FindAll(e => e.startTime > currentTime && e.isEnabled);
    }
    
    public List<Event> GetExpiredEvents()
    {
        string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return events.FindAll(e => e.endTime < currentTime);
    }
}

[System.Serializable]
public class Event
{
    public string eventID;
    public string eventName;
    public string eventDescription;
    public string eventType;
    public string startTime;
    public string endTime;
    public string eventBanner;
    public string eventIcon;
    public bool isEnabled;
    public bool isLimited;
    public List<EventTask> tasks;
    public List<EventReward> rewards;
    
    public Event(string id, string name, string desc, string type, string start, string end, string banner, string icon)
    {
        eventID = id;
        eventName = name;
        eventDescription = desc;
        eventType = type;
        startTime = start;
        endTime = end;
        eventBanner = banner;
        eventIcon = icon;
        isEnabled = true;
        isLimited = false;
        tasks = new List<EventTask>();
        rewards = new List<EventReward>();
    }
    
    public void AddTask(EventTask task)
    {
        tasks.Add(task);
    }
    
    public void AddReward(EventReward reward)
    {
        rewards.Add(reward);
    }
    
    public void EnableEvent()
    {
        isEnabled = true;
    }
    
    public void DisableEvent()
    {
        isEnabled = false;
    }
    
    public bool IsActive()
    {
        string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return isEnabled && startTime <= currentTime && endTime >= currentTime;
    }
    
    public bool IsUpcoming()
    {
        string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return isEnabled && startTime > currentTime;
    }
    
    public bool IsExpired()
    {
        string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return endTime < currentTime;
    }
}

[System.Serializable]
public class EventTask
{
    public string taskID;
    public string eventID;
    public string taskName;
    public string taskDescription;
    public string taskType;
    public int requiredProgress;
    public int currentProgress;
    public bool isCompleted;
    public bool isClaimed;
    public string startTime;
    public string endTime;
    public List<string> rewardIDs;
    
    public EventTask(string id, string eventID, string name, string desc, string type, int required, string start, string end)
    {
        taskID = id;
        this.eventID = eventID;
        taskName = name;
        taskDescription = desc;
        taskType = type;
        requiredProgress = required;
        currentProgress = 0;
        isCompleted = false;
        isClaimed = false;
        startTime = start;
        endTime = end;
        rewardIDs = new List<string>();
    }
    
    public void AddReward(string rewardID)
    {
        rewardIDs.Add(rewardID);
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
    
    public bool IsActive()
    {
        string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return startTime <= currentTime && endTime >= currentTime;
    }
}

[System.Serializable]
public class EventReward
{
    public string rewardID;
    public string rewardName;
    public string rewardType;
    public string rewardItemID;
    public int quantity;
    public string rewardDescription;
    public bool isLimited;
    public int limitCount;
    public int currentCount;
    
    public EventReward(string id, string name, string type, string itemID, int qty, string desc, bool limited = false, int limit = 0)
    {
        rewardID = id;
        rewardName = name;
        rewardType = type;
        rewardItemID = itemID;
        quantity = qty;
        rewardDescription = desc;
        isLimited = limited;
        limitCount = limit;
        currentCount = 0;
    }
    
    public bool CanClaim()
    {
        return !isLimited || currentCount < limitCount;
    }
    
    public void IncrementCount()
    {
        currentCount++;
    }
}

[System.Serializable]
public class PlayerEventData
{
    public string playerID;
    public Dictionary<string, EventProgress> eventProgress;
    
    public PlayerEventData(string playerID)
    {
        this.playerID = playerID;
        eventProgress = new Dictionary<string, EventProgress>();
    }
    
    public void UpdateTaskProgress(string eventID, string taskID, int progress)
    {
        if (!eventProgress.ContainsKey(eventID))
        {
            eventProgress[eventID] = new EventProgress(eventID);
        }
        eventProgress[eventID].UpdateTaskProgress(taskID, progress);
    }
    
    public void ClaimTaskReward(string eventID, string taskID)
    {
        if (eventProgress.ContainsKey(eventID))
        {
            eventProgress[eventID].ClaimTaskReward(taskID);
        }
    }
    
    public EventProgress GetEventProgress(string eventID)
    {
        return eventProgress.ContainsKey(eventID) ? eventProgress[eventID] : null;
    }
    
    public int GetTaskProgress(string eventID, string taskID)
    {
        if (eventProgress.ContainsKey(eventID))
        {
            return eventProgress[eventID].GetTaskProgress(taskID);
        }
        return 0;
    }
    
    public bool IsTaskCompleted(string eventID, string taskID)
    {
        if (eventProgress.ContainsKey(eventID))
        {
            return eventProgress[eventID].IsTaskCompleted(taskID);
        }
        return false;
    }
    
    public bool IsTaskClaimed(string eventID, string taskID)
    {
        if (eventProgress.ContainsKey(eventID))
        {
            return eventProgress[eventID].IsTaskClaimed(taskID);
        }
        return false;
    }
}

[System.Serializable]
public class EventProgress
{
    public string eventID;
    public Dictionary<string, int> taskProgress;
    public Dictionary<string, bool> taskCompletion;
    public Dictionary<string, bool> taskClaims;
    
    public EventProgress(string eventID)
    {
        this.eventID = eventID;
        taskProgress = new Dictionary<string, int>();
        taskCompletion = new Dictionary<string, bool>();
        taskClaims = new Dictionary<string, bool>();
    }
    
    public void UpdateTaskProgress(string taskID, int progress)
    {
        taskProgress[taskID] = progress;
    }
    
    public void MarkTaskCompleted(string taskID)
    {
        taskCompletion[taskID] = true;
    }
    
    public void ClaimTaskReward(string taskID)
    {
        taskClaims[taskID] = true;
    }
    
    public int GetTaskProgress(string taskID)
    {
        return taskProgress.ContainsKey(taskID) ? taskProgress[taskID] : 0;
    }
    
    public bool IsTaskCompleted(string taskID)
    {
        return taskCompletion.ContainsKey(taskID) && taskCompletion[taskID];
    }
    
    public bool IsTaskClaimed(string taskID)
    {
        return taskClaims.ContainsKey(taskID) && taskClaims[taskID];
    }
}

[System.Serializable]
public class EventManagerData
{
    public EventSystem system;
    public List<PlayerEventData> playerEventData;
    
    public EventManagerData()
    {
        system = new EventSystem("event", "活动系统", "管理游戏活动");
        playerEventData = new List<PlayerEventData>();
    }
    
    public void AddPlayerEventData(PlayerEventData data)
    {
        playerEventData.Add(data);
    }
    
    public PlayerEventData GetPlayerEventData(string playerID)
    {
        return playerEventData.Find(pd => pd.playerID == playerID);
    }
}
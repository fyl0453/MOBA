[System.Serializable]
public class EventSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Event> events;
    public List<EventTask> eventTasks;
    public List<EventReward> eventRewards;
    public List<PlayerEvent> playerEvents;
    public List<EventParticipation> eventParticipations;
    
    public EventSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        events = new List<Event>();
        eventTasks = new List<EventTask>();
        eventRewards = new List<EventReward>();
        playerEvents = new List<PlayerEvent>();
        eventParticipations = new List<EventParticipation>();
    }
    
    public void AddEvent(Event eventObj)
    {
        events.Add(eventObj);
    }
    
    public void AddEventTask(EventTask eventTask)
    {
        eventTasks.Add(eventTask);
    }
    
    public void AddEventReward(EventReward eventReward)
    {
        eventRewards.Add(eventReward);
    }
    
    public void AddPlayerEvent(PlayerEvent playerEvent)
    {
        playerEvents.Add(playerEvent);
    }
    
    public void AddEventParticipation(EventParticipation eventParticipation)
    {
        eventParticipations.Add(eventParticipation);
    }
    
    public Event GetEvent(string eventID)
    {
        return events.Find(e => e.eventID == eventID);
    }
    
    public EventTask GetEventTask(string taskID)
    {
        return eventTasks.Find(et => et.taskID == taskID);
    }
    
    public EventReward GetEventReward(string rewardID)
    {
        return eventRewards.Find(er => er.rewardID == rewardID);
    }
    
    public PlayerEvent GetPlayerEvent(string playerEventID)
    {
        return playerEvents.Find(pe => pe.playerEventID == playerEventID);
    }
    
    public EventParticipation GetEventParticipation(string participationID)
    {
        return eventParticipations.Find(ep => ep.participationID == participationID);
    }
    
    public List<Event> GetEventsByType(string eventType)
    {
        return events.FindAll(e => e.eventType == eventType);
    }
    
    public List<Event> GetEventsByStatus(string status)
    {
        return events.FindAll(e => e.status == status);
    }
    
    public List<EventTask> GetEventTasksByEvent(string eventID)
    {
        return eventTasks.FindAll(et => et.eventID == eventID);
    }
    
    public List<EventReward> GetEventRewardsByEvent(string eventID)
    {
        return eventRewards.FindAll(er => er.eventID == eventID);
    }
    
    public List<PlayerEvent> GetPlayerEventsByUser(string userID)
    {
        return playerEvents.FindAll(pe => pe.userID == userID);
    }
    
    public List<EventParticipation> GetEventParticipationsByUser(string userID)
    {
        return eventParticipations.FindAll(ep => ep.userID == userID);
    }
}

[System.Serializable]
public class Event
{
    public string eventID;
    public string eventName;
    public string eventDescription;
    public string eventType;
    public string status;
    public string startDate;
    public string endDate;
    public string banner;
    public string icon;
    public int maxParticipants;
    public int currentParticipants;
    public bool isLimited;
    public string participationCondition;
    public List<string> taskIDs;
    public List<string> rewardIDs;
    
    public Event(string id, string name, string description, string eventType, string startDate, string endDate, bool isLimited)
    {
        eventID = id;
        eventName = name;
        eventDescription = description;
        this.eventType = eventType;
        status = "pending";
        this.startDate = startDate;
        this.endDate = endDate;
        banner = "";
        icon = "";
        maxParticipants = 0;
        currentParticipants = 0;
        this.isLimited = isLimited;
        participationCondition = "";
        taskIDs = new List<string>();
        rewardIDs = new List<string>();
    }
    
    public void Start()
    {
        status = "active";
    }
    
    public void End()
    {
        status = "completed";
    }
    
    public void Cancel()
    {
        status = "cancelled";
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
        if (taskIDs.Contains(taskID))
        {
            taskIDs.Remove(taskID);
        }
    }
    
    public void AddReward(string rewardID)
    {
        if (!rewardIDs.Contains(rewardID))
        {
            rewardIDs.Add(rewardID);
        }
    }
    
    public void RemoveReward(string rewardID)
    {
        if (rewardIDs.Contains(rewardID))
        {
            rewardIDs.Remove(rewardID);
        }
    }
    
    public void AddParticipant()
    {
        currentParticipants++;
    }
    
    public bool IsActive()
    {
        if (status != "active")
            return false;
        
        System.DateTime now = System.DateTime.Now;
        System.DateTime start = System.DateTime.Parse(startDate);
        System.DateTime end = System.DateTime.Parse(endDate);
        
        return now >= start && now <= end;
    }
    
    public bool CanParticipate()
    {
        if (!IsActive())
            return false;
        
        if (isLimited && maxParticipants > 0 && currentParticipants >= maxParticipants)
            return false;
        
        return true;
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
    public int rewardExp;
    public int rewardPoints;
    public bool isRepeatable;
    public string status;
    
    public EventTask(string id, string eventID, string taskName, string taskDescription, string taskType, int requiredProgress, int rewardExp, int rewardPoints, bool isRepeatable)
    {
        taskID = id;
        this.eventID = eventID;
        this.taskName = taskName;
        this.taskDescription = taskDescription;
        this.taskType = taskType;
        this.requiredProgress = requiredProgress;
        this.rewardExp = rewardExp;
        this.rewardPoints = rewardPoints;
        this.isRepeatable = isRepeatable;
        status = "active";
    }
    
    public void Activate()
    {
        status = "active";
    }
    
    public void Disable()
    {
        status = "disabled";
    }
    
    public void Complete()
    {
        status = "completed";
    }
}

[System.Serializable]
public class EventReward
{
    public string rewardID;
    public string eventID;
    public string rewardName;
    public string rewardType;
    public string rewardValue;
    public int quantity;
    public int requiredPoints;
    public int requiredLevel;
    public bool isEnabled;
    public string icon;
    
    public EventReward(string id, string eventID, string rewardName, string rewardType, string rewardValue, int quantity, int requiredPoints, int requiredLevel, string icon)
    {
        rewardID = id;
        this.eventID = eventID;
        this.rewardName = rewardName;
        this.rewardType = rewardType;
        this.rewardValue = rewardValue;
        this.quantity = quantity;
        this.requiredPoints = requiredPoints;
        this.requiredLevel = requiredLevel;
        isEnabled = true;
        this.icon = icon;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public void SetQuantity(int quantity)
    {
        this.quantity = quantity;
    }
    
    public void SetRequiredPoints(int requiredPoints)
    {
        this.requiredPoints = requiredPoints;
    }
}

[System.Serializable]
public class PlayerEvent
{
    public string playerEventID;
    public string userID;
    public string eventID;
    public int points;
    public int level;
    public int exp;
    public string joinDate;
    public string lastUpdateTime;
    public List<PlayerEventTask> tasks;
    public List<PlayerEventReward> rewards;
    
    public PlayerEvent(string id, string userID, string eventID)
    {
        playerEventID = id;
        this.userID = userID;
        this.eventID = eventID;
        points = 0;
        level = 1;
        exp = 0;
        joinDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        tasks = new List<PlayerEventTask>();
        rewards = new List<PlayerEventReward>();
    }
    
    public void AddPoints(int points)
    {
        this.points += points;
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddExp(int exp)
    {
        this.exp += exp;
        if (this.exp >= 100 * level)
        {
            level++;
            this.exp = 0;
        }
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddTask(PlayerEventTask task)
    {
        tasks.Add(task);
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddReward(PlayerEventReward reward)
    {
        rewards.Add(reward);
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class PlayerEventTask
{
    public string playerTaskID;
    public string taskID;
    public int currentProgress;
    public string status;
    public string lastUpdateTime;
    
    public PlayerEventTask(string id, string taskID)
    {
        playerTaskID = id;
        this.taskID = taskID;
        currentProgress = 0;
        status = "in_progress";
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateProgress(int progress)
    {
        currentProgress += progress;
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Complete()
    {
        status = "completed";
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Reset()
    {
        currentProgress = 0;
        status = "in_progress";
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class PlayerEventReward
{
    public string playerRewardID;
    public string rewardID;
    public bool isClaimed;
    public string claimTime;
    
    public PlayerEventReward(string id, string rewardID)
    {
        playerRewardID = id;
        this.rewardID = rewardID;
        isClaimed = false;
        claimTime = "";
    }
    
    public void Claim()
    {
        isClaimed = true;
        claimTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class EventParticipation
{
    public string participationID;
    public string userID;
    public string eventID;
    public string status;
    public string joinTime;
    public string leaveTime;
    public int score;
    
    public EventParticipation(string id, string userID, string eventID)
    {
        participationID = id;
        this.userID = userID;
        this.eventID = eventID;
        status = "active";
        joinTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        leaveTime = "";
        score = 0;
    }
    
    public void Leave()
    {
        status = "completed";
        leaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateScore(int score)
    {
        this.score = score;
    }
}

[System.Serializable]
public class EventSystemDetailedManagerData
{
    public EventSystemDetailed system;
    
    public EventSystemDetailedManagerData()
    {
        system = new EventSystemDetailed("event_system_detailed", "活动系统详细", "管理活动的详细功能，包括活动类型、参与条件和奖励");
    }
}
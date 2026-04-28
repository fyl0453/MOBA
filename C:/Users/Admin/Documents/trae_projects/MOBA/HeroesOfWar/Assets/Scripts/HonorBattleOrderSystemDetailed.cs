[System.Serializable]
public class HonorBattleOrderSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<HonorBattleOrder> honorBattleOrders;
    public List<HonorBattleOrderLevel> honorBattleOrderLevels;
    public List<HonorBattleOrderTask> honorBattleOrderTasks;
    public List<HonorBattleOrderReward> honorBattleOrderRewards;
    public List<HonorBattleOrderEvent> honorBattleOrderEvents;
    
    public HonorBattleOrderSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        honorBattleOrders = new List<HonorBattleOrder>();
        honorBattleOrderLevels = new List<HonorBattleOrderLevel>();
        honorBattleOrderTasks = new List<HonorBattleOrderTask>();
        honorBattleOrderRewards = new List<HonorBattleOrderReward>();
        honorBattleOrderEvents = new List<HonorBattleOrderEvent>();
    }
    
    public void AddHonorBattleOrder(HonorBattleOrder honorBattleOrder)
    {
        honorBattleOrders.Add(honorBattleOrder);
    }
    
    public void AddHonorBattleOrderLevel(HonorBattleOrderLevel honorBattleOrderLevel)
    {
        honorBattleOrderLevels.Add(honorBattleOrderLevel);
    }
    
    public void AddHonorBattleOrderTask(HonorBattleOrderTask honorBattleOrderTask)
    {
        honorBattleOrderTasks.Add(honorBattleOrderTask);
    }
    
    public void AddHonorBattleOrderReward(HonorBattleOrderReward honorBattleOrderReward)
    {
        honorBattleOrderRewards.Add(honorBattleOrderReward);
    }
    
    public void AddHonorBattleOrderEvent(HonorBattleOrderEvent honorBattleOrderEvent)
    {
        honorBattleOrderEvents.Add(honorBattleOrderEvent);
    }
    
    public HonorBattleOrder GetHonorBattleOrder(string orderID)
    {
        return honorBattleOrders.Find(hbo => hbo.orderID == orderID);
    }
    
    public HonorBattleOrderLevel GetHonorBattleOrderLevel(int level)
    {
        return honorBattleOrderLevels.Find(hbol => hbol.level == level);
    }
    
    public HonorBattleOrderTask GetHonorBattleOrderTask(string taskID)
    {
        return honorBattleOrderTasks.Find(hbot => hbot.taskID == taskID);
    }
    
    public HonorBattleOrderReward GetHonorBattleOrderReward(string rewardID)
    {
        return honorBattleOrderRewards.Find(hbor => hbor.rewardID == rewardID);
    }
    
    public HonorBattleOrderEvent GetHonorBattleOrderEvent(string eventID)
    {
        return honorBattleOrderEvents.Find(hboe => hboe.eventID == eventID);
    }
    
    public List<HonorBattleOrder> GetHonorBattleOrdersByStatus(string status)
    {
        return honorBattleOrders.FindAll(hbo => hbo.status == status);
    }
    
    public List<HonorBattleOrderTask> GetHonorBattleOrderTasksByOrder(string orderID)
    {
        return honorBattleOrderTasks.FindAll(hbot => hbot.orderID == orderID);
    }
    
    public List<HonorBattleOrderTask> GetHonorBattleOrderTasksByType(string taskType)
    {
        return honorBattleOrderTasks.FindAll(hbot => hbot.taskType == taskType);
    }
    
    public List<HonorBattleOrderReward> GetHonorBattleOrderRewardsByLevel(int level)
    {
        return honorBattleOrderRewards.FindAll(hbor => hbor.level == level);
    }
    
    public List<HonorBattleOrderEvent> GetHonorBattleOrderEventsByUser(string userID)
    {
        return honorBattleOrderEvents.FindAll(hboe => hboe.userID == userID);
    }
}

[System.Serializable]
public class HonorBattleOrder
{
    public string orderID;
    public string orderName;
    public string orderDescription;
    public string season;
    public string status;
    public string startTime;
    public string endTime;
    public int maxLevel;
    public int basePrice;
    public int premiumPrice;
    public string icon;
    public bool isEnabled;
    
    public HonorBattleOrder(string id, string orderName, string orderDescription, string season, string startTime, string endTime, int maxLevel, int basePrice, int premiumPrice, string icon)
    {
        orderID = id;
        this.orderName = orderName;
        this.orderDescription = orderDescription;
        this.season = season;
        status = "upcoming";
        this.startTime = startTime;
        this.endTime = endTime;
        this.maxLevel = maxLevel;
        this.basePrice = basePrice;
        this.premiumPrice = premiumPrice;
        this.icon = icon;
        isEnabled = true;
    }
    
    public void Start()
    {
        status = "active";
    }
    
    public void End()
    {
        status = "ended";
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public bool IsActive()
    {
        return status == "active" && isEnabled;
    }
    
    public bool IsEnded()
    {
        return status == "ended";
    }
}

[System.Serializable]
public class HonorBattleOrderLevel
{
    public int level;
    public string levelName;
    public string levelDescription;
    public int requiredExp;
    public int totalExp;
    public string icon;
    public List<string> rewards;
    
    public HonorBattleOrderLevel(int level, string levelName, string levelDescription, int requiredExp, int totalExp, string icon)
    {
        this.level = level;
        this.levelName = levelName;
        this.levelDescription = levelDescription;
        this.requiredExp = requiredExp;
        this.totalExp = totalExp;
        this.icon = icon;
        rewards = new List<string>();
    }
    
    public void AddReward(string rewardID)
    {
        rewards.Add(rewardID);
    }
    
    public bool IsReached(int exp)
    {
        return exp >= requiredExp;
    }
}

[System.Serializable]
public class HonorBattleOrderTask
{
    public string taskID;
    public string orderID;
    public string taskName;
    public string taskDescription;
    public string taskType;
    public int requiredProgress;
    public int rewardExp;
    public int rewardPoints;
    public bool isRepeatable;
    public string status;
    public string startTime;
    public string endTime;
    
    public HonorBattleOrderTask(string id, string orderID, string taskName, string taskDescription, string taskType, int requiredProgress, int rewardExp, int rewardPoints, bool isRepeatable, string startTime, string endTime)
    {
        taskID = id;
        this.orderID = orderID;
        this.taskName = taskName;
        this.taskDescription = taskDescription;
        this.taskType = taskType;
        this.requiredProgress = requiredProgress;
        this.rewardExp = rewardExp;
        this.rewardPoints = rewardPoints;
        this.isRepeatable = isRepeatable;
        status = "active";
        this.startTime = startTime;
        this.endTime = endTime;
    }
    
    public void Enable()
    {
        status = "active";
    }
    
    public void Disable()
    {
        status = "inactive";
    }
    
    public bool IsActive()
    {
        return status == "active";
    }
}

[System.Serializable]
public class HonorBattleOrderReward
{
    public string rewardID;
    public string orderID;
    public string rewardName;
    public string rewardType;
    public string rewardValue;
    public int quantity;
    public int level;
    public string tier;
    public string icon;
    public bool isEnabled;
    
    public HonorBattleOrderReward(string id, string orderID, string rewardName, string rewardType, string rewardValue, int quantity, int level, string tier, string icon)
    {
        rewardID = id;
        this.orderID = orderID;
        this.rewardName = rewardName;
        this.rewardType = rewardType;
        this.rewardValue = rewardValue;
        this.quantity = quantity;
        this.level = level;
        this.tier = tier;
        this.icon = icon;
        isEnabled = true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public bool IsAvailable(int userLevel, string userTier)
    {
        if (!isEnabled)
            return false;
        
        if (userLevel < level)
            return false;
        
        if (tier != "both" && tier != userTier)
            return false;
        
        return true;
    }
}

[System.Serializable]
public class HonorBattleOrderEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string orderID;
    public string description;
    public string timestamp;
    public string status;
    
    public HonorBattleOrderEvent(string id, string eventType, string userID, string orderID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.orderID = orderID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
    }
    
    public void MarkAsCompleted()
    {
        status = "completed";
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class HonorBattleOrderSystemDetailedManagerData
{
    public HonorBattleOrderSystemDetailed system;
    
    public HonorBattleOrderSystemDetailedManagerData()
    {
        system = new HonorBattleOrderSystemDetailed("honor_battle_order_system_detailed", "荣耀战令系统详细", "管理荣耀战令的详细功能，包括战令等级和奖励");
    }
}
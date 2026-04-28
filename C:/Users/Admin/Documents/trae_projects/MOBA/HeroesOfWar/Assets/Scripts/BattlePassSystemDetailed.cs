[System.Serializable]
public class BattlePassSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<BattlePass> battlePasses;
    public List<BattlePassLevel> battlePassLevels;
    public List<BattlePassTask> battlePassTasks;
    public List<BattlePassReward> battlePassRewards;
    public List<PlayerBattlePass> playerBattlePasses;
    public List<BattlePassEvent> battlePassEvents;
    
    public BattlePassSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        battlePasses = new List<BattlePass>();
        battlePassLevels = new List<BattlePassLevel>();
        battlePassTasks = new List<BattlePassTask>();
        battlePassRewards = new List<BattlePassReward>();
        playerBattlePasses = new List<PlayerBattlePass>();
        battlePassEvents = new List<BattlePassEvent>();
    }
    
    public void AddBattlePass(BattlePass battlePass)
    {
        battlePasses.Add(battlePass);
    }
    
    public void AddBattlePassLevel(BattlePassLevel level)
    {
        battlePassLevels.Add(level);
    }
    
    public void AddBattlePassTask(BattlePassTask task)
    {
        battlePassTasks.Add(task);
    }
    
    public void AddBattlePassReward(BattlePassReward reward)
    {
        battlePassRewards.Add(reward);
    }
    
    public void AddPlayerBattlePass(PlayerBattlePass playerBattlePass)
    {
        playerBattlePasses.Add(playerBattlePass);
    }
    
    public void AddBattlePassEvent(BattlePassEvent battlePassEvent)
    {
        battlePassEvents.Add(battlePassEvent);
    }
    
    public BattlePass GetBattlePass(string battlePassID)
    {
        return battlePasses.Find(bp => bp.battlePassID == battlePassID);
    }
    
    public BattlePassLevel GetBattlePassLevel(string levelID)
    {
        return battlePassLevels.Find(bl => bl.levelID == levelID);
    }
    
    public BattlePassTask GetBattlePassTask(string taskID)
    {
        return battlePassTasks.Find(bt => bt.taskID == taskID);
    }
    
    public BattlePassReward GetBattlePassReward(string rewardID)
    {
        return battlePassRewards.Find(br => br.rewardID == rewardID);
    }
    
    public PlayerBattlePass GetPlayerBattlePass(string playerBattlePassID)
    {
        return playerBattlePasses.Find(pbp => pbp.playerBattlePassID == playerBattlePassID);
    }
    
    public BattlePassEvent GetBattlePassEvent(string eventID)
    {
        return battlePassEvents.Find(be => be.eventID == eventID);
    }
    
    public List<BattlePass> GetActiveBattlePasses()
    {
        return battlePasses.FindAll(bp => bp.status == "active" && bp.isEnabled);
    }
    
    public List<BattlePassLevel> GetBattlePassLevelsByBattlePass(string battlePassID)
    {
        return battlePassLevels.FindAll(bl => bl.battlePassID == battlePassID);
    }
    
    public List<BattlePassTask> GetBattlePassTasksByBattlePass(string battlePassID)
    {
        return battlePassTasks.FindAll(bt => bt.battlePassID == battlePassID);
    }
    
    public List<BattlePassReward> GetBattlePassRewardsByLevel(string levelID)
    {
        return battlePassRewards.FindAll(br => br.levelID == levelID);
    }
    
    public List<PlayerBattlePass> GetPlayerBattlePassesByUser(string userID)
    {
        return playerBattlePasses.FindAll(pbp => pbp.userID == userID);
    }
}

[System.Serializable]
public class BattlePass
{
    public string battlePassID;
    public string battlePassName;
    public string battlePassDescription;
    public string status;
    public string startDate;
    public string endDate;
    public int maxLevel;
    public int basePrice;
    public int premiumPrice;
    public string currency;
    public bool isEnabled;
    public string icon;
    public string banner;
    
    public BattlePass(string id, string name, string description, int maxLevel, int basePrice, int premiumPrice, string currency)
    {
        battlePassID = id;
        battlePassName = name;
        battlePassDescription = description;
        status = "inactive";
        startDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        endDate = "";
        this.maxLevel = maxLevel;
        this.basePrice = basePrice;
        this.premiumPrice = premiumPrice;
        this.currency = currency;
        isEnabled = true;
        icon = "";
        banner = "";
    }
    
    public void Activate()
    {
        status = "active";
    }
    
    public void Deactivate()
    {
        status = "inactive";
    }
    
    public void End()
    {
        status = "ended";
        endDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class BattlePassLevel
{
    public string levelID;
    public string battlePassID;
    public int level;
    public int requiredExp;
    public string description;
    public bool isPremium;
    public List<string> rewardIDs;
    
    public BattlePassLevel(string id, string battlePassID, int level, int requiredExp, bool isPremium)
    {
        levelID = id;
        this.battlePassID = battlePassID;
        this.level = level;
        this.requiredExp = requiredExp;
        description = "";
        this.isPremium = isPremium;
        rewardIDs = new List<string>();
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
}

[System.Serializable]
public class BattlePassTask
{
    public string taskID;
    public string battlePassID;
    public string taskName;
    public string taskDescription;
    public string taskType;
    public int requiredProgress;
    public int expReward;
    public bool isRepeatable;
    public bool isPremium;
    public string status;
    
    public BattlePassTask(string id, string battlePassID, string taskName, string taskDescription, string taskType, int requiredProgress, int expReward, bool isRepeatable, bool isPremium)
    {
        taskID = id;
        this.battlePassID = battlePassID;
        this.taskName = taskName;
        this.taskDescription = taskDescription;
        this.taskType = taskType;
        this.requiredProgress = requiredProgress;
        this.expReward = expReward;
        this.isRepeatable = isRepeatable;
        this.isPremium = isPremium;
        status = "active";
    }
    
    public void Complete()
    {
        status = "completed";
    }
    
    public void Reset()
    {
        status = "active";
    }
    
    public void Disable()
    {
        status = "disabled";
    }
}

[System.Serializable]
public class BattlePassReward
{
    public string rewardID;
    public string levelID;
    public string rewardName;
    public string rewardType;
    public string rewardValue;
    public int quantity;
    public bool isPremium;
    public bool isClaimed;
    public string icon;
    
    public BattlePassReward(string id, string levelID, string rewardName, string rewardType, string rewardValue, int quantity, bool isPremium)
    {
        rewardID = id;
        this.levelID = levelID;
        this.rewardName = rewardName;
        this.rewardType = rewardType;
        this.rewardValue = rewardValue;
        this.quantity = quantity;
        this.isPremium = isPremium;
        isClaimed = false;
        icon = "";
    }
    
    public void Claim()
    {
        isClaimed = true;
    }
    
    public void Reset()
    {
        isClaimed = false;
    }
}

[System.Serializable]
public class PlayerBattlePass
{
    public string playerBattlePassID;
    public string userID;
    public string battlePassID;
    public int currentLevel;
    public int currentExp;
    public bool hasPremium;
    public List<PlayerBattlePassTask> tasks;
    public List<PlayerBattlePassReward> rewards;
    public string joinDate;
    public string lastUpdateTime;
    
    public PlayerBattlePass(string id, string userID, string battlePassID, bool hasPremium)
    {
        playerBattlePassID = id;
        this.userID = userID;
        this.battlePassID = battlePassID;
        currentLevel = 1;
        currentExp = 0;
        this.hasPremium = hasPremium;
        tasks = new List<PlayerBattlePassTask>();
        rewards = new List<PlayerBattlePassReward>();
        joinDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddExp(int exp)
    {
        currentExp += exp;
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void LevelUp()
    {
        currentLevel++;
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void SetPremium(bool hasPremium)
    {
        this.hasPremium = hasPremium;
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddTask(PlayerBattlePassTask task)
    {
        tasks.Add(task);
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddReward(PlayerBattlePassReward reward)
    {
        rewards.Add(reward);
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class PlayerBattlePassTask
{
    public string playerTaskID;
    public string taskID;
    public int currentProgress;
    public string status;
    public string lastUpdateTime;
    
    public PlayerBattlePassTask(string id, string taskID)
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
public class PlayerBattlePassReward
{
    public string playerRewardID;
    public string rewardID;
    public bool isClaimed;
    public string claimTime;
    
    public PlayerBattlePassReward(string id, string rewardID)
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
public class BattlePassEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string battlePassID;
    public string description;
    public string timestamp;
    public string status;
    
    public BattlePassEvent(string id, string eventType, string userID, string battlePassID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.battlePassID = battlePassID;
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
public class BattlePassSystemDetailedManagerData
{
    public BattlePassSystemDetailed system;
    
    public BattlePassSystemDetailedManagerData()
    {
        system = new BattlePassSystemDetailed("battle_pass_system_detailed", "通行证系统详细", "管理通行证的详细功能，包括等级、任务和奖励");
    }
}
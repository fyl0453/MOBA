[System.Serializable]
public class HeroMasterySystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<HeroMasteryLevel> heroMasteryLevels;
    public List<HeroMastery> heroMasteries;
    public List<HeroMasteryReward> heroMasteryRewards;
    public List<HeroMasteryEvent> heroMasteryEvents;
    
    public HeroMasterySystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        heroMasteryLevels = new List<HeroMasteryLevel>();
        heroMasteries = new List<HeroMastery>();
        heroMasteryRewards = new List<HeroMasteryReward>();
        heroMasteryEvents = new List<HeroMasteryEvent>();
    }
    
    public void AddHeroMasteryLevel(HeroMasteryLevel heroMasteryLevel)
    {
        heroMasteryLevels.Add(heroMasteryLevel);
    }
    
    public void AddHeroMastery(HeroMastery heroMastery)
    {
        heroMasteries.Add(heroMastery);
    }
    
    public void AddHeroMasteryReward(HeroMasteryReward heroMasteryReward)
    {
        heroMasteryRewards.Add(heroMasteryReward);
    }
    
    public void AddHeroMasteryEvent(HeroMasteryEvent heroMasteryEvent)
    {
        heroMasteryEvents.Add(heroMasteryEvent);
    }
    
    public HeroMasteryLevel GetHeroMasteryLevel(int level)
    {
        return heroMasteryLevels.Find(hml => hml.level == level);
    }
    
    public HeroMastery GetHeroMastery(string userID, string heroID)
    {
        return heroMasteries.Find(hm => hm.userID == userID && hm.heroID == heroID);
    }
    
    public HeroMasteryReward GetHeroMasteryReward(string rewardID)
    {
        return heroMasteryRewards.Find(hmr => hmr.rewardID == rewardID);
    }
    
    public HeroMasteryEvent GetHeroMasteryEvent(string eventID)
    {
        return heroMasteryEvents.Find(hme => hme.eventID == eventID);
    }
    
    public List<HeroMasteryLevel> GetHeroMasteryLevels()
    {
        return heroMasteryLevels;
    }
    
    public List<HeroMastery> GetHeroMasteriesByUser(string userID)
    {
        return heroMasteries.FindAll(hm => hm.userID == userID);
    }
    
    public List<HeroMastery> GetHeroMasteriesByHero(string heroID)
    {
        return heroMasteries.FindAll(hm => hm.heroID == heroID);
    }
    
    public List<HeroMasteryReward> GetHeroMasteryRewardsByLevel(int level)
    {
        return heroMasteryRewards.FindAll(hmr => hmr.requiredLevel == level);
    }
    
    public List<HeroMasteryEvent> GetHeroMasteryEventsByUser(string userID)
    {
        return heroMasteryEvents.FindAll(hme => hme.userID == userID);
    }
}

[System.Serializable]
public class HeroMasteryLevel
{
    public int level;
    public string levelName;
    public string levelDescription;
    public int requiredPoints;
    public string icon;
    public List<string> rewards;
    
    public HeroMasteryLevel(int level, string levelName, string levelDescription, int requiredPoints, string icon)
    {
        this.level = level;
        this.levelName = levelName;
        this.levelDescription = levelDescription;
        this.requiredPoints = requiredPoints;
        this.icon = icon;
        rewards = new List<string>();
    }
    
    public void AddReward(string rewardID)
    {
        rewards.Add(rewardID);
    }
    
    public bool IsReached(int points)
    {
        return points >= requiredPoints;
    }
}

[System.Serializable]
public class HeroMastery
{
    public string masteryID;
    public string userID;
    public string userName;
    public string heroID;
    public string heroName;
    public int masteryLevel;
    public int masteryPoints;
    public int totalGames;
    public int totalWins;
    public float winRate;
    public string lastPlayed;
    public List<string> claimedRewards;
    
    public HeroMastery(string id, string userID, string userName, string heroID, string heroName)
    {
        masteryID = id;
        this.userID = userID;
        this.userName = userName;
        this.heroID = heroID;
        this.heroName = heroName;
        masteryLevel = 1;
        masteryPoints = 0;
        totalGames = 0;
        totalWins = 0;
        winRate = 0;
        lastPlayed = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        claimedRewards = new List<string>();
    }
    
    public void AddMasteryPoints(int points)
    {
        masteryPoints += points;
        UpdateMasteryLevel();
    }
    
    public void AddGame(bool isWin)
    {
        totalGames++;
        if (isWin)
        {
            totalWins++;
        }
        winRate = (float)totalWins / totalGames * 100;
        lastPlayed = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateMasteryLevel()
    {
        // 简单的等级计算，每100点提升一级
        masteryLevel = (masteryPoints / 100) + 1;
    }
    
    public void ClaimReward(string rewardID)
    {
        if (!claimedRewards.Contains(rewardID))
        {
            claimedRewards.Add(rewardID);
        }
    }
    
    public bool HasClaimedReward(string rewardID)
    {
        return claimedRewards.Contains(rewardID);
    }
}

[System.Serializable]
public class HeroMasteryReward
{
    public string rewardID;
    public string rewardName;
    public string rewardDescription;
    public int requiredLevel;
    public string rewardType;
    public string rewardValue;
    public int quantity;
    public string icon;
    public bool isEnabled;
    
    public HeroMasteryReward(string id, string rewardName, string rewardDescription, int requiredLevel, string rewardType, string rewardValue, int quantity, string icon)
    {
        rewardID = id;
        this.rewardName = rewardName;
        this.rewardDescription = rewardDescription;
        this.requiredLevel = requiredLevel;
        this.rewardType = rewardType;
        this.rewardValue = rewardValue;
        this.quantity = quantity;
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
    
    public bool IsAvailable(int level)
    {
        if (!isEnabled)
            return false;
        
        if (level < requiredLevel)
            return false;
        
        return true;
    }
}

[System.Serializable]
public class HeroMasteryEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string heroID;
    public string description;
    public int points;
    public string timestamp;
    public string status;
    
    public HeroMasteryEvent(string id, string eventType, string userID, string heroID, string description, int points)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.heroID = heroID;
        this.description = description;
        this.points = points;
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
public class HeroMasterySystemDetailedManagerData
{
    public HeroMasterySystemDetailed system;
    
    public HeroMasterySystemDetailedManagerData()
    {
        system = new HeroMasterySystemDetailed("hero_mastery_system_detailed", "英雄熟练度系统详细", "管理英雄熟练度的详细功能，包括英雄熟练度等级和奖励");
    }
}
[System.Serializable]
public class LeaderboardSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Leaderboard> leaderboards;
    public List<LeaderboardEntry> leaderboardEntries;
    public List<LeaderboardReward> leaderboardRewards;
    public List<LeaderboardEvent> leaderboardEvents;
    
    public LeaderboardSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        leaderboards = new List<Leaderboard>();
        leaderboardEntries = new List<LeaderboardEntry>();
        leaderboardRewards = new List<LeaderboardReward>();
        leaderboardEvents = new List<LeaderboardEvent>();
    }
    
    public void AddLeaderboard(Leaderboard leaderboard)
    {
        leaderboards.Add(leaderboard);
    }
    
    public void AddLeaderboardEntry(LeaderboardEntry entry)
    {
        leaderboardEntries.Add(entry);
    }
    
    public void AddLeaderboardReward(LeaderboardReward reward)
    {
        leaderboardRewards.Add(reward);
    }
    
    public void AddLeaderboardEvent(LeaderboardEvent leaderboardEvent)
    {
        leaderboardEvents.Add(leaderboardEvent);
    }
    
    public Leaderboard GetLeaderboard(string leaderboardID)
    {
        return leaderboards.Find(l => l.leaderboardID == leaderboardID);
    }
    
    public LeaderboardEntry GetLeaderboardEntry(string entryID)
    {
        return leaderboardEntries.Find(e => e.entryID == entryID);
    }
    
    public LeaderboardReward GetLeaderboardReward(string rewardID)
    {
        return leaderboardRewards.Find(r => r.rewardID == rewardID);
    }
    
    public LeaderboardEvent GetLeaderboardEvent(string eventID)
    {
        return leaderboardEvents.Find(e => e.eventID == eventID);
    }
    
    public List<Leaderboard> GetLeaderboardsByType(string leaderboardType)
    {
        return leaderboards.FindAll(l => l.leaderboardType == leaderboardType);
    }
    
    public List<LeaderboardEntry> GetLeaderboardEntriesByLeaderboard(string leaderboardID)
    {
        return leaderboardEntries.FindAll(e => e.leaderboardID == leaderboardID);
    }
    
    public List<LeaderboardReward> GetLeaderboardRewardsByLeaderboard(string leaderboardID)
    {
        return leaderboardRewards.FindAll(r => r.leaderboardID == leaderboardID);
    }
    
    public List<LeaderboardEvent> GetLeaderboardEventsByUser(string userID)
    {
        return leaderboardEvents.FindAll(e => e.userID == userID);
    }
}

[System.Serializable]
public class Leaderboard
{
    public string leaderboardID;
    public string leaderboardName;
    public string leaderboardDescription;
    public string leaderboardType;
    public string sortBy;
    public string sortOrder;
    public int maxEntries;
    public string refreshInterval;
    public string startDate;
    public string endDate;
    public bool isEnabled;
    public bool isVisible;
    public string icon;
    public string banner;
    
    public Leaderboard(string id, string name, string description, string leaderboardType, string sortBy, string sortOrder, int maxEntries, string refreshInterval)
    {
        leaderboardID = id;
        leaderboardName = name;
        leaderboardDescription = description;
        this.leaderboardType = leaderboardType;
        this.sortBy = sortBy;
        this.sortOrder = sortOrder;
        this.maxEntries = maxEntries;
        this.refreshInterval = refreshInterval;
        startDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        endDate = "";
        isEnabled = true;
        isVisible = true;
        icon = "";
        banner = "";
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public void SetVisibility(bool visible)
    {
        isVisible = visible;
    }
    
    public void SetEndDate(string endDate)
    {
        this.endDate = endDate;
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string entryID;
    public string leaderboardID;
    public string userID;
    public string userName;
    public string userAvatar;
    public float score;
    public int rank;
    public string lastUpdateTime;
    public string entryTime;
    public Dictionary<string, string> metadata;
    
    public LeaderboardEntry(string id, string leaderboardID, string userID, string userName, string userAvatar, float score)
    {
        entryID = id;
        this.leaderboardID = leaderboardID;
        this.userID = userID;
        this.userName = userName;
        this.userAvatar = userAvatar;
        this.score = score;
        rank = 0;
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        entryTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        metadata = new Dictionary<string, string>();
    }
    
    public void UpdateScore(float score)
    {
        this.score = score;
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateRank(int rank)
    {
        this.rank = rank;
    }
    
    public void AddMetadata(string key, string value)
    {
        if (metadata.ContainsKey(key))
        {
            metadata[key] = value;
        }
        else
        {
            metadata.Add(key, value);
        }
    }
    
    public void RemoveMetadata(string key)
    {
        if (metadata.ContainsKey(key))
        {
            metadata.Remove(key);
        }
    }
}

[System.Serializable]
public class LeaderboardReward
{
    public string rewardID;
    public string leaderboardID;
    public string rewardName;
    public string rewardType;
    public string rewardValue;
    public string description;
    public int quantity;
    public int minRank;
    public int maxRank;
    public bool isEnabled;
    public string icon;
    
    public LeaderboardReward(string id, string leaderboardID, string rewardName, string rewardType, string rewardValue, string description, int quantity, int minRank, int maxRank)
    {
        rewardID = id;
        this.leaderboardID = leaderboardID;
        this.rewardName = rewardName;
        this.rewardType = rewardType;
        this.rewardValue = rewardValue;
        this.description = description;
        this.quantity = quantity;
        this.minRank = minRank;
        this.maxRank = maxRank;
        isEnabled = true;
        icon = "";
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
    
    public void SetRankRange(int minRank, int maxRank)
    {
        this.minRank = minRank;
        this.maxRank = maxRank;
    }
}

[System.Serializable]
public class LeaderboardEvent
{
    public string eventID;
    public string eventType;
    public string leaderboardID;
    public string userID;
    public string description;
    public string timestamp;
    public string status;
    public float oldScore;
    public float newScore;
    public int oldRank;
    public int newRank;
    
    public LeaderboardEvent(string id, string eventType, string leaderboardID, string userID, string description, float oldScore, float newScore, int oldRank, int newRank)
    {
        eventID = id;
        this.eventType = eventType;
        this.leaderboardID = leaderboardID;
        this.userID = userID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
        this.oldScore = oldScore;
        this.newScore = newScore;
        this.oldRank = oldRank;
        this.newRank = newRank;
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
public class LeaderboardSystemDetailedManagerData
{
    public LeaderboardSystemDetailed system;
    
    public LeaderboardSystemDetailedManagerData()
    {
        system = new LeaderboardSystemDetailed("leaderboard_system_detailed", "排行榜系统详细", "管理排行榜的详细功能，包括不同类型的排行榜和排名机制");
    }
}
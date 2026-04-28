[System.Serializable]
public class DataStatisticsSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<DataEvent> dataEvents;
    public List<DataReport> dataReports;
    
    public DataStatisticsSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        dataEvents = new List<DataEvent>();
        dataReports = new List<DataReport>();
    }
    
    public void AddDataEvent(DataEvent dataEvent)
    {
        dataEvents.Add(dataEvent);
    }
    
    public void AddDataReport(DataReport dataReport)
    {
        dataReports.Add(dataReport);
    }
    
    public List<DataEvent> GetEventsByType(string eventType)
    {
        return dataEvents.FindAll(e => e.eventType == eventType);
    }
    
    public List<DataEvent> GetEventsByPlayer(string playerID)
    {
        return dataEvents.FindAll(e => e.playerID == playerID);
    }
    
    public DataReport GetReport(string reportID)
    {
        return dataReports.Find(r => r.reportID == reportID);
    }
}

[System.Serializable]
public class DataEvent
{
    public string eventID;
    public string eventType;
    public string playerID;
    public string eventData;
    public System.DateTime eventTime;
    public string eventLocation;
    
    public DataEvent(string id, string type, string player, string data, string location = "")
    {
        eventID = id;
        eventType = type;
        playerID = player;
        eventData = data;
        eventTime = System.DateTime.Now;
        eventLocation = location;
    }
}

[System.Serializable]
public class DataReport
{
    public string reportID;
    public string reportName;
    public string reportType;
    public string reportData;
    public System.DateTime reportTime;
    public string reportDescription;
    
    public DataReport(string id, string name, string type, string data, string desc = "")
    {
        reportID = id;
        reportName = name;
        reportType = type;
        reportData = data;
        reportTime = System.DateTime.Now;
        reportDescription = desc;
    }
}

[System.Serializable]
public class PlayerStatistics
{
    public string playerID;
    public string playerName;
    public int totalMatches;
    public int totalWins;
    public int totalLosses;
    public float winRate;
    public int totalKills;
    public int totalDeaths;
    public int totalAssists;
    public float kda;
    public int totalGold;
    public int totalDamage;
    public int totalHealing;
    public int totalTimePlayed;
    public List<HeroStatistics> heroStats;
    
    public PlayerStatistics(string playerID, string playerName)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        totalMatches = 0;
        totalWins = 0;
        totalLosses = 0;
        winRate = 0f;
        totalKills = 0;
        totalDeaths = 0;
        totalAssists = 0;
        kda = 0f;
        totalGold = 0;
        totalDamage = 0;
        totalHealing = 0;
        totalTimePlayed = 0;
        heroStats = new List<HeroStatistics>();
    }
    
    public void UpdateMatchStats(int kills, int deaths, int assists, int gold, int damage, int healing, bool won, int matchTime)
    {
        totalMatches++;
        if (won)
        {
            totalWins++;
        }
        else
        {
            totalLosses++;
        }
        winRate = (float)totalWins / totalMatches;
        totalKills += kills;
        totalDeaths += deaths;
        totalAssists += assists;
        kda = totalDeaths > 0 ? (float)(totalKills + totalAssists) / totalDeaths : (float)(totalKills + totalAssists);
        totalGold += gold;
        totalDamage += damage;
        totalHealing += healing;
        totalTimePlayed += matchTime;
    }
    
    public void UpdateHeroStats(string heroID, int kills, int deaths, int assists, bool won)
    {
        HeroStatistics heroStat = heroStats.Find(hs => hs.heroID == heroID);
        if (heroStat == null)
        {
            heroStat = new HeroStatistics(heroID);
            heroStats.Add(heroStat);
        }
        heroStat.UpdateStats(kills, deaths, assists, won);
    }
}

[System.Serializable]
public class HeroStatistics
{
    public string heroID;
    public int totalMatches;
    public int totalWins;
    public float winRate;
    public int totalKills;
    public int totalDeaths;
    public int totalAssists;
    public float kda;
    
    public HeroStatistics(string heroID)
    {
        this.heroID = heroID;
        totalMatches = 0;
        totalWins = 0;
        winRate = 0f;
        totalKills = 0;
        totalDeaths = 0;
        totalAssists = 0;
        kda = 0f;
    }
    
    public void UpdateStats(int kills, int deaths, int assists, bool won)
    {
        totalMatches++;
        if (won)
        {
            totalWins++;
        }
        winRate = (float)totalWins / totalMatches;
        totalKills += kills;
        totalDeaths += deaths;
        totalAssists += assists;
        kda = totalDeaths > 0 ? (float)(totalKills + totalAssists) / totalDeaths : (float)(totalKills + totalAssists);
    }
}

[System.Serializable]
public class GameStatistics
{
    public string gameID;
    public string gameMode;
    public string mapName;
    public int totalPlayers;
    public int totalMatches;
    public int averageMatchTime;
    public float averageWinRate;
    public float averageKDA;
    public int peakConcurrentPlayers;
    public int totalRevenue;
    
    public GameStatistics(string gameID)
    {
        this.gameID = gameID;
        gameMode = "All";
        mapName = "All";
        totalPlayers = 0;
        totalMatches = 0;
        averageMatchTime = 0;
        averageWinRate = 0f;
        averageKDA = 0f;
        peakConcurrentPlayers = 0;
        totalRevenue = 0;
    }
    
    public void UpdateGameStats(int matchTime, float winRate, float kda, int revenue)
    {
        totalMatches++;
        averageMatchTime = (averageMatchTime * (totalMatches - 1) + matchTime) / totalMatches;
        averageWinRate = (averageWinRate * (totalMatches - 1) + winRate) / totalMatches;
        averageKDA = (averageKDA * (totalMatches - 1) + kda) / totalMatches;
        totalRevenue += revenue;
    }
    
    public void UpdatePlayerCount(int concurrentPlayers)
    {
        if (concurrentPlayers > peakConcurrentPlayers)
        {
            peakConcurrentPlayers = concurrentPlayers;
        }
    }
}

[System.Serializable]
public class DataStatisticsManagerData
{
    public DataStatisticsSystem system;
    public List<PlayerStatistics> playerStats;
    public List<GameStatistics> gameStats;
    
    public DataStatisticsManagerData()
    {
        system = new DataStatisticsSystem("data_statistics", "数据统计系统", "收集和分析游戏数据");
        playerStats = new List<PlayerStatistics>();
        gameStats = new List<GameStatistics>();
    }
    
    public void AddPlayerStats(PlayerStatistics stats)
    {
        playerStats.Add(stats);
    }
    
    public void AddGameStats(GameStatistics stats)
    {
        gameStats.Add(stats);
    }
    
    public PlayerStatistics GetPlayerStats(string playerID)
    {
        return playerStats.Find(ps => ps.playerID == playerID);
    }
    
    public GameStatistics GetGameStats(string gameID)
    {
        return gameStats.Find(gs => gs.gameID == gameID);
    }
    
    public void UpdatePlayerStats(string playerID, PlayerStatistics stats)
    {
        PlayerStatistics existingStats = GetPlayerStats(playerID);
        if (existingStats != null)
        {
            existingStats = stats;
        }
        else
        {
            AddPlayerStats(stats);
        }
    }
    
    public void UpdateGameStats(string gameID, GameStatistics stats)
    {
        GameStatistics existingStats = GetGameStats(gameID);
        if (existingStats != null)
        {
            existingStats = stats;
        }
        else
        {
            AddGameStats(stats);
        }
    }
}
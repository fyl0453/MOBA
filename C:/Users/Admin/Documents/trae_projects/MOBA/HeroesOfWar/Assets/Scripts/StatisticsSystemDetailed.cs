using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class MatchStatistics
{
    public string MatchID;
    public string GameMode;
    public DateTime MatchTime;
    public int Duration;
    public string WinTeam;
    public List<PlayerMatchStats> PlayerStats;
    public TeamStats BlueTeamStats;
    public TeamStats RedTeamStats;
    public string MapID;
    public int ServerID;

    public MatchStatistics(string matchID, string gameMode)
    {
        MatchID = matchID;
        GameMode = gameMode;
        MatchTime = DateTime.Now;
        Duration = 0;
        WinTeam = "";
        PlayerStats = new List<PlayerMatchStats>();
        BlueTeamStats = new TeamStats();
        RedTeamStats = new TeamStats();
        MapID = "";
        ServerID = 0;
    }
}

[Serializable]
public class PlayerMatchStats
{
    public string PlayerID;
    public string PlayerName;
    public string HeroID;
    public int Team;
    public int KillCount;
    public int DeathCount;
    public int AssistCount;
    public double Damage;
    public double Tank;
    public double Heal;
    public double Gold;
    public int Level;
    public int CS;
    public double KDA;
    public double DamagePercent;
    public double TankPercent;
    public int GoldEarned;
    public int ItemsPurchased;

    public PlayerMatchStats(string playerID, string playerName, string heroID, int team)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        HeroID = heroID;
        Team = team;
        KillCount = 0;
        DeathCount = 0;
        AssistCount = 0;
        Damage = 0.0;
        Tank = 0.0;
        Heal = 0.0;
        Gold = 0.0;
        Level = 1;
        CS = 0;
        KDA = 0.0;
        DamagePercent = 0.0;
        TankPercent = 0.0;
        GoldEarned = 0;
        ItemsPurchased = 0;
    }

    public void CalculateKDA()
    {
        if (DeathCount == 0)
        {
            KDA = KillCount + AssistCount;
        }
        else
        {
            KDA = (KillCount + AssistCount) * 1.0 / DeathCount;
        }
    }
}

[Serializable]
public class TeamStats
{
    public int TotalKills;
    public int TotalDeaths;
    public double TotalDamage;
    public double TotalTank;
    public int TowerKills;
    public int DragonKills;
    public int BaronKills;
    public double GoldDiff;

    public TeamStats()
    {
        TotalKills = 0;
        TotalDeaths = 0;
        TotalDamage = 0.0;
        TotalTank = 0.0;
        TowerKills = 0;
        DragonKills = 0;
        BaronKills = 0;
        GoldDiff = 0.0;
    }
}

[Serializable]
public class PlayerOverallStats
{
    public string PlayerID;
    public int TotalMatches;
    public int WinMatches;
    public int LoseMatches;
    public int MVPCount;
    public int SVPCount;
    public int FirstBloodCount;
    public int PentakillCount;
    public int QuadraKillCount;
    public int TripleKillCount;
    public double TotalDamage;
    public double TotalTank;
    public double TotalHeal;
    public double AverageKDA;
    public double AverageDamage;
    public double AverageTank;
    public double AverageGold;
    public double AverageCS;
    public int HighestKillMatch;
    public int HighestDamageMatch;
    public Dictionary<string, int> HeroUsageCount;
    public Dictionary<string, int> HeroWinCount;
    public Dictionary<string, double> HeroAverageKDA;
    public Dictionary<string, double> HeroAverageDamage;
    public DateTime LastMatchTime;

    public PlayerOverallStats(string playerID)
    {
        PlayerID = playerID;
        TotalMatches = 0;
        WinMatches = 0;
        LoseMatches = 0;
        MVPCount = 0;
        SVPCount = 0;
        FirstBloodCount = 0;
        PentakillCount = 0;
        QuadraKillCount = 0;
        TripleKillCount = 0;
        TotalDamage = 0.0;
        TotalTank = 0.0;
        TotalHeal = 0.0;
        AverageKDA = 0.0;
        AverageDamage = 0.0;
        AverageTank = 0.0;
        AverageGold = 0.0;
        AverageCS = 0.0;
        HighestKillMatch = 0;
        HighestDamageMatch = 0;
        HeroUsageCount = new Dictionary<string, int>();
        HeroWinCount = new Dictionary<string, int>();
        HeroAverageKDA = new Dictionary<string, double>();
        HeroAverageDamage = new Dictionary<string, double>();
        LastMatchTime = DateTime.Now;
    }

    public void UpdateAverages()
    {
        if (TotalMatches > 0)
        {
            AverageKDA = TotalMatches > 0 ? (KillCount + AssistCount) * 1.0 / Math.Max(1, DeathCount) : 0;
            AverageDamage = TotalDamage / TotalMatches;
            AverageTank = TotalTank / TotalMatches;
            AverageGold = AverageGold / TotalMatches;
            AverageCS = AverageCS / TotalMatches;
        }
    }
}

[Serializable]
public class HeroStatistics
{
    public string HeroID;
    public int TotalPicks;
    public int TotalWins;
    public int TotalBans;
    public double WinRate;
    public double AverageKDA;
    public double AverageDamage;
    public double AverageTank;
    public double PickRate;
    public double BanRate;
    public int HighestKills;
    public Dictionary<string, int> CounterHeroes;
    public Dictionary<string, int> CounteredByHeroes;

    public HeroStatistics(string heroID)
    {
        HeroID = heroID;
        TotalPicks = 0;
        TotalWins = 0;
        TotalBans = 0;
        WinRate = 0.0;
        AverageKDA = 0.0;
        AverageDamage = 0.0;
        AverageTank = 0.0;
        PickRate = 0.0;
        BanRate = 0.0;
        HighestKills = 0;
        CounterHeroes = new Dictionary<string, int>();
        CounteredByHeroes = new Dictionary<string, int>();
    }

    public void UpdateStats()
    {
        if (TotalPicks > 0)
        {
            WinRate = (double)TotalWins / TotalPicks * 100.0;
        }
    }
}

[Serializable]
public class StatisticsSystemData
{
    public List<MatchStatistics> MatchHistory;
    public Dictionary<string, PlayerOverallStats> PlayerStats;
    public Dictionary<string, HeroStatistics> HeroStats;
    public Dictionary<string, List<MatchStatistics>> PlayerMatchHistory;
    public Dictionary<string, List<PlayerMatchStats>> PlayerDetailedHistory;
    public DateTime LastCleanupTime;

    public StatisticsSystemData()
    {
        MatchHistory = new List<MatchStatistics>();
        PlayerStats = new Dictionary<string, PlayerOverallStats>();
        HeroStats = new Dictionary<string, HeroStatistics>();
        PlayerMatchHistory = new Dictionary<string, List<MatchStatistics>>();
        PlayerDetailedHistory = new Dictionary<string, List<PlayerMatchStats>>();
        LastCleanupTime = DateTime.Now;
    }

    public void AddMatchStatistics(MatchStatistics match)
    {
        MatchHistory.Add(match);
    }

    public void AddPlayerStats(string playerID, PlayerOverallStats stats)
    {
        PlayerStats[playerID] = stats;
    }
}

[Serializable]
public class StatisticsEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EventData;

    public StatisticsEvent(string eventID, string eventType, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class StatisticsSystemDataManager
{
    private static StatisticsSystemDataManager _instance;
    public static StatisticsSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new StatisticsSystemDataManager();
            }
            return _instance;
        }
    }

    public StatisticsSystemData statisticsData;
    private List<StatisticsEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private StatisticsSystemDataManager()
    {
        statisticsData = new StatisticsSystemData();
        recentEvents = new List<StatisticsEvent>();
        LoadStatisticsData();
    }

    public void SaveStatisticsData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "StatisticsSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, statisticsData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存统计数据失败: " + e.Message);
        }
    }

    public void LoadStatisticsData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "StatisticsSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    statisticsData = (StatisticsSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载统计数据失败: " + e.Message);
            statisticsData = new StatisticsSystemData();
        }
    }

    public void CreateStatisticsEvent(string eventType, string playerID, string eventData)
    {
        string eventID = "stats_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        StatisticsEvent statsEvent = new StatisticsEvent(eventID, eventType, playerID, eventData);
        recentEvents.Add(statsEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<StatisticsEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
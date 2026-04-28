using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class PlayerMatchStats
{
    public string PlayerID;
    public string PlayerName;
    public string HeroID;
    public string HeroName;
    public int Kills;
    public int Deaths;
    public int Assists;
    public int DamageDealt;
    public int DamageTaken;
    public int HealingDone;
    public int GoldEarned;
    public int MinionsKilled;
    public int TurretsDestroyed;
    public int DragonsKilled;
    public int BaronsKilled;
    public int Skill1Uses;
    public int Skill2Uses;
    public int Skill3Uses;
    public int UltimateUses;
    public float VisionScore;
    public float TimeSpentDead;
    public float TimePlayed;
    public string Items;
    public bool IsVictory;
    public int RankPointsChange;
    public string MatchResult;

    public PlayerMatchStats(string playerID, string playerName, string heroID, string heroName)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        HeroID = heroID;
        HeroName = heroName;
        Kills = 0;
        Deaths = 0;
        Assists = 0;
        DamageDealt = 0;
        DamageTaken = 0;
        HealingDone = 0;
        GoldEarned = 0;
        MinionsKilled = 0;
        TurretsDestroyed = 0;
        DragonsKilled = 0;
        BaronsKilled = 0;
        Skill1Uses = 0;
        Skill2Uses = 0;
        Skill3Uses = 0;
        UltimateUses = 0;
        VisionScore = 0f;
        TimeSpentDead = 0f;
        TimePlayed = 0f;
        Items = "";
        IsVictory = false;
        RankPointsChange = 0;
        MatchResult = "";
    }
}

[Serializable]
public class MatchStats
{
    public string MatchID;
    public string MatchType;
    public string MapName;
    public DateTime MatchStartTime;
    public DateTime MatchEndTime;
    public float MatchDuration;
    public string WinningTeam;
    public string LosingTeam;
    public List<PlayerMatchStats> PlayerStats;
    public string MatchVersion;
    public bool IsRanked;
    public string SeasonID;

    public MatchStats(string matchID, string matchType, string mapName, bool isRanked, string seasonID)
    {
        MatchID = matchID;
        MatchType = matchType;
        MapName = mapName;
        MatchStartTime = DateTime.Now;
        MatchEndTime = DateTime.MinValue;
        MatchDuration = 0f;
        WinningTeam = "";
        LosingTeam = "";
        PlayerStats = new List<PlayerMatchStats>();
        MatchVersion = "1.0.0";
        IsRanked = isRanked;
        SeasonID = seasonID;
    }
}

[Serializable]
public class PlayerStatsSummary
{
    public string PlayerID;
    public int TotalMatches;
    public int TotalWins;
    public int TotalLosses;
    public float WinRate;
    public int TotalKills;
    public int TotalDeaths;
    public int TotalAssists;
    public float KDA;
    public int TotalDamageDealt;
    public int TotalDamageTaken;
    public int TotalHealingDone;
    public int TotalGoldEarned;
    public int TotalMinionsKilled;
    public int TotalTurretsDestroyed;
    public float AverageVisionScore;
    public float AverageTimePlayed;
    public DateTime LastMatchTime;
    public string BestHero;
    public string MostPlayedHero;

    public PlayerStatsSummary(string playerID)
    {
        PlayerID = playerID;
        TotalMatches = 0;
        TotalWins = 0;
        TotalLosses = 0;
        WinRate = 0f;
        TotalKills = 0;
        TotalDeaths = 0;
        TotalAssists = 0;
        KDA = 0f;
        TotalDamageDealt = 0;
        TotalDamageTaken = 0;
        TotalHealingDone = 0;
        TotalGoldEarned = 0;
        TotalMinionsKilled = 0;
        TotalTurretsDestroyed = 0;
        AverageVisionScore = 0f;
        AverageTimePlayed = 0f;
        LastMatchTime = DateTime.MinValue;
        BestHero = "";
        MostPlayedHero = "";
    }
}

[Serializable]
public class HeroStats
{
    public string HeroID;
    public string HeroName;
    public int MatchesPlayed;
    public int Wins;
    public int Losses;
    public float WinRate;
    public int TotalKills;
    public int TotalDeaths;
    public int TotalAssists;
    public float KDA;
    public int TotalDamageDealt;
    public int TotalDamageTaken;
    public int TotalHealingDone;
    public int TotalGoldEarned;
    public float AverageVisionScore;
    public float AverageTimePlayed;
    public DateTime LastPlayed;

    public HeroStats(string heroID, string heroName)
    {
        HeroID = heroID;
        HeroName = heroName;
        MatchesPlayed = 0;
        Wins = 0;
        Losses = 0;
        WinRate = 0f;
        TotalKills = 0;
        TotalDeaths = 0;
        TotalAssists = 0;
        KDA = 0f;
        TotalDamageDealt = 0;
        TotalDamageTaken = 0;
        TotalHealingDone = 0;
        TotalGoldEarned = 0;
        AverageVisionScore = 0f;
        AverageTimePlayed = 0f;
        LastPlayed = DateTime.MinValue;
    }
}

[Serializable]
public class PlayerMatchHistory
{
    public string PlayerID;
    public List<MatchStats> MatchHistory;
    public Dictionary<string, HeroStats> HeroStats;
    public PlayerStatsSummary StatsSummary;
    public int MaxMatchHistory;

    public PlayerMatchHistory(string playerID)
    {
        PlayerID = playerID;
        MatchHistory = new List<MatchStats>();
        HeroStats = new Dictionary<string, HeroStats>();
        StatsSummary = new PlayerStatsSummary(playerID);
        MaxMatchHistory = 100;
    }
}

[Serializable]
public class MatchStatsSystemData
{
    public Dictionary<string, PlayerMatchHistory> PlayerMatchHistories;
    public Dictionary<string, MatchStats> AllMatches;
    public List<string> MatchTypes;
    public List<string> Maps;
    public int MaxMatchesStored;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public MatchStatsSystemData()
    {
        PlayerMatchHistories = new Dictionary<string, PlayerMatchHistory>();
        AllMatches = new Dictionary<string, MatchStats>();
        MatchTypes = new List<string> { "ranked", "casual", "practice", "tournament" };
        Maps = new List<string> { "Summoner's Rift", "Howling Abyss", "Twisted Treeline" };
        MaxMatchesStored = 10000;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddPlayerMatchHistory(string playerID, PlayerMatchHistory matchHistory)
    {
        PlayerMatchHistories[playerID] = matchHistory;
    }

    public void AddMatchStats(string matchID, MatchStats matchStats)
    {
        AllMatches[matchID] = matchStats;
    }
}

[Serializable]
public class StatsEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string MatchID;
    public string EventData;

    public StatsEvent(string eventID, string eventType, string playerID, string matchID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        MatchID = matchID;
        EventData = eventData;
    }
}

public class MatchStatsSystemDataManager
{
    private static MatchStatsSystemDataManager _instance;
    public static MatchStatsSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MatchStatsSystemDataManager();
            }
            return _instance;
        }
    }

    public MatchStatsSystemData statsData;
    private List<StatsEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private MatchStatsSystemDataManager()
    {
        statsData = new MatchStatsSystemData();
        recentEvents = new List<StatsEvent>();
        LoadStatsData();
    }

    public void SaveStatsData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MatchStatsSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, statsData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存对局统计系统数据失败: " + e.Message);
        }
    }

    public void LoadStatsData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MatchStatsSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    statsData = (MatchStatsSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载对局统计系统数据失败: " + e.Message);
            statsData = new MatchStatsSystemData();
        }
    }

    public void CreateStatsEvent(string eventType, string playerID, string matchID, string eventData)
    {
        string eventID = "stats_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        StatsEvent statsEvent = new StatsEvent(eventID, eventType, playerID, matchID, eventData);
        recentEvents.Add(statsEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<StatsEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
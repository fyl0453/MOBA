using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class MatchQueueEntry
{
    public string QueueID;
    public string PlayerID;
    public string PlayerName;
    public int MatchType;
    public int PlayerLevel;
    public int RankLevel;
    public List<string> TeamMembers;
    public DateTime QueueTime;
    public int EstimatedWaitTime;
    public bool IsTeam;
    public string TeamID;

    public MatchQueueEntry(string queueID, string playerID, string playerName, int matchType, int playerLevel, int rankLevel)
    {
        QueueID = queueID;
        PlayerID = playerID;
        PlayerName = playerName;
        MatchType = matchType;
        PlayerLevel = playerLevel;
        RankLevel = rankLevel;
        TeamMembers = new List<string>();
        TeamMembers.Add(playerID);
        QueueTime = DateTime.Now;
        EstimatedWaitTime = 30;
        IsTeam = false;
        TeamID = "";
    }

    public MatchQueueEntry(string queueID, string teamID, List<string> teamMembers, int matchType, int averageLevel, int averageRankLevel)
    {
        QueueID = queueID;
        PlayerID = teamMembers[0];
        PlayerName = "";
        MatchType = matchType;
        PlayerLevel = averageLevel;
        RankLevel = averageRankLevel;
        TeamMembers = teamMembers;
        QueueTime = DateTime.Now;
        EstimatedWaitTime = 30;
        IsTeam = true;
        TeamID = teamID;
    }
}

[Serializable]
public class MatchGame
{
    public string GameID;
    public string MatchType;
    public List<string> BlueTeamPlayers;
    public List<string> RedTeamPlayers;
    public DateTime StartTime;
    public DateTime EndTime;
    public string WinnerTeam;
    public int Duration;
    public List<PlayerMatchStats> PlayerStats;
    public bool IsCompleted;

    public MatchGame(string gameID, string matchType, List<string> blueTeamPlayers, List<string> redTeamPlayers)
    {
        GameID = gameID;
        MatchType = matchType;
        BlueTeamPlayers = blueTeamPlayers;
        RedTeamPlayers = redTeamPlayers;
        StartTime = DateTime.Now;
        EndTime = DateTime.MinValue;
        WinnerTeam = "";
        Duration = 0;
        PlayerStats = new List<PlayerMatchStats>();
        IsCompleted = false;
    }
}

[Serializable]
public class PlayerMatchStats
{
    public string PlayerID;
    public string PlayerName;
    public string Team;
    public int Kills;
    public int Deaths;
    public int Assists;
    public int GoldEarned;
    public int DamageDealt;
    public int DamageTaken;
    public int HealingDone;
    public int ObjectivesTaken;
    public int MinionsKilled;
    public string HeroUsed;
    public bool IsWinner;

    public PlayerMatchStats(string playerID, string playerName, string team, string heroUsed)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        Team = team;
        Kills = 0;
        Deaths = 0;
        Assists = 0;
        GoldEarned = 0;
        DamageDealt = 0;
        DamageTaken = 0;
        HealingDone = 0;
        ObjectivesTaken = 0;
        MinionsKilled = 0;
        HeroUsed = heroUsed;
        IsWinner = false;
    }
}

[Serializable]
public class MatchHistory
{
    public string HistoryID;
    public string PlayerID;
    public string GameID;
    public string MatchType;
    public DateTime MatchTime;
    public bool IsWinner;
    public int Kills;
    public int Deaths;
    public int Assists;
    public string HeroUsed;
    public int Duration;
    public string OpponentName;

    public MatchHistory(string historyID, string playerID, string gameID, string matchType, bool isWinner, int kills, int deaths, int assists, string heroUsed, int duration, string opponentName)
    {
        HistoryID = historyID;
        PlayerID = playerID;
        GameID = gameID;
        MatchType = matchType;
        MatchTime = DateTime.Now;
        IsWinner = isWinner;
        Kills = kills;
        Deaths = deaths;
        Assists = assists;
        HeroUsed = heroUsed;
        Duration = duration;
        OpponentName = opponentName;
    }
}

[Serializable]
public class MatchConfig
{
    public string ConfigID;
    public string MatchTypeName;
    public int MinPlayers;
    public int MaxPlayers;
    public int TeamSize;
    public bool AllowTeams;
    public bool IsRanked;
    public int MinLevel;
    public int MatchmakingRange;
    public int MaxWaitTime;

    public MatchConfig(string configID, string matchTypeName, int minPlayers, int maxPlayers, int teamSize, bool allowTeams, bool isRanked, int minLevel, int matchmakingRange, int maxWaitTime)
    {
        ConfigID = configID;
        MatchTypeName = matchTypeName;
        MinPlayers = minPlayers;
        MaxPlayers = maxPlayers;
        TeamSize = teamSize;
        AllowTeams = allowTeams;
        IsRanked = isRanked;
        MinLevel = minLevel;
        MatchmakingRange = matchmakingRange;
        MaxWaitTime = maxWaitTime;
    }
}

[Serializable]
public class MatchSystemData
{
    public List<MatchQueueEntry> MatchQueue;
    public List<MatchGame> ActiveGames;
    public List<MatchHistory> MatchHistories;
    public List<MatchConfig> MatchConfigs;
    public Dictionary<string, List<MatchHistory>> PlayerMatchHistory;
    public int CurrentGameCount;
    public DateTime LastCleanupTime;

    public MatchSystemData()
    {
        MatchQueue = new List<MatchQueueEntry>();
        ActiveGames = new List<MatchGame>();
        MatchHistories = new List<MatchHistory>();
        MatchConfigs = new List<MatchConfig>();
        PlayerMatchHistory = new Dictionary<string, List<MatchHistory>>();
        CurrentGameCount = 0;
        LastCleanupTime = DateTime.Now;
        InitializeDefaultConfigs();
    }

    private void InitializeDefaultConfigs()
    {
        MatchConfig normalConfig = new MatchConfig("config_normal", "普通匹配", 10, 10, 5, true, false, 1, 10, 120);
        MatchConfigs.Add(normalConfig);

        MatchConfig rankedConfig = new MatchConfig("config_ranked", "排位赛", 10, 10, 5, true, true, 5, 5, 180);
        MatchConfigs.Add(rankedConfig);

        MatchConfig aiConfig = new MatchConfig("config_ai", "人机对战", 1, 1, 1, false, false, 1, 0, 10);
        MatchConfigs.Add(aiConfig);

        MatchConfig customConfig = new MatchConfig("config_custom", "自定义", 2, 10, 5, true, false, 1, 0, 60);
        MatchConfigs.Add(customConfig);
    }

    public void AddQueueEntry(MatchQueueEntry entry)
    {
        MatchQueue.Add(entry);
    }

    public void RemoveQueueEntry(string queueID)
    {
        MatchQueueEntry entry = MatchQueue.Find(e => e.QueueID == queueID);
        if (entry != null)
        {
            MatchQueue.Remove(entry);
        }
    }

    public void AddActiveGame(MatchGame game)
    {
        ActiveGames.Add(game);
        CurrentGameCount++;
    }

    public void RemoveActiveGame(string gameID)
    {
        MatchGame game = ActiveGames.Find(g => g.GameID == gameID);
        if (game != null)
        {
            ActiveGames.Remove(game);
            CurrentGameCount--;
        }
    }

    public void AddMatchHistory(MatchHistory history)
    {
        MatchHistories.Add(history);
        if (!PlayerMatchHistory.ContainsKey(history.PlayerID))
        {
            PlayerMatchHistory[history.PlayerID] = new List<MatchHistory>();
        }
        PlayerMatchHistory[history.PlayerID].Add(history);
    }

    public void AddMatchConfig(MatchConfig config)
    {
        MatchConfigs.Add(config);
    }
}

[Serializable]
public class MatchEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string GameID;
    public string EventData;

    public MatchEvent(string eventID, string eventType, string playerID, string gameID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        GameID = gameID;
        EventData = eventData;
    }
}

public class MatchSystemDataManager
{
    private static MatchSystemDataManager _instance;
    public static MatchSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MatchSystemDataManager();
            }
            return _instance;
        }
    }

    public MatchSystemData matchData;
    private List<MatchEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private MatchSystemDataManager()
    {
        matchData = new MatchSystemData();
        recentEvents = new List<MatchEvent>();
        LoadMatchData();
    }

    public void SaveMatchData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MatchSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, matchData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存匹配系统数据失败: " + e.Message);
        }
    }

    public void LoadMatchData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MatchSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    matchData = (MatchSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载匹配系统数据失败: " + e.Message);
            matchData = new MatchSystemData();
        }
    }

    public void CreateMatchEvent(string eventType, string playerID, string gameID, string eventData)
    {
        string eventID = "match_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchEvent matchEvent = new MatchEvent(eventID, eventType, playerID, gameID, eventData);
        recentEvents.Add(matchEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<MatchEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SurrenderVote
{
    public string VoteID;
    public string MatchID;
    public string InitiatorPlayerID;
    public string InitiatorPlayerName;
    public DateTime VoteStartTime;
    public DateTime VoteEndTime;
    public int YesVotes;
    public int NoVotes;
    public int RequiredVotes;
    public int TotalPlayers;
    public bool IsPassed;
    public bool IsActive;
    public string VoteResult;

    public SurrenderVote(string matchID, string initiatorPlayerID, string initiatorPlayerName, int totalPlayers)
    {
        VoteID = "surrender_vote_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchID = matchID;
        InitiatorPlayerID = initiatorPlayerID;
        InitiatorPlayerName = initiatorPlayerName;
        VoteStartTime = DateTime.Now;
        VoteEndTime = DateTime.Now.AddMinutes(1);
        YesVotes = 0;
        NoVotes = 0;
        RequiredVotes = (int)Math.Ceiling(totalPlayers * 0.6f);
        TotalPlayers = totalPlayers;
        IsPassed = false;
        IsActive = true;
        VoteResult = "";
    }
}

[Serializable]
public class PlayerSurrenderVote
{
    public string PlayerID;
    public string PlayerName;
    public string VoteID;
    public bool HasVoted;
    public bool VoteChoice;
    public DateTime VoteTime;

    public PlayerSurrenderVote(string playerID, string playerName, string voteID, bool voteChoice)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        VoteID = voteID;
        HasVoted = false;
        VoteChoice = false;
        VoteTime = DateTime.MinValue;
    }
}

[Serializable]
public class DisconnectRecord
{
    public string RecordID;
    public string MatchID;
    public string PlayerID;
    public string PlayerName;
    public DateTime DisconnectTime;
    public DateTime ReconnectTime;
    public bool IsReconnected;
    public int DisconnectDuration;
    public string DisconnectReason;
    public bool WasPenalized;

    public DisconnectRecord(string matchID, string playerID, string playerName, string reason)
    {
        RecordID = "disconnect_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchID = matchID;
        PlayerID = playerID;
        PlayerName = playerName;
        DisconnectTime = DateTime.Now;
        ReconnectTime = DateTime.MinValue;
        IsReconnected = false;
        DisconnectDuration = 0;
        DisconnectReason = reason;
        WasPenalized = false;
    }
}

[Serializable]
public class MatchSurrenderData
{
    public string MatchID;
    public List<SurrenderVote> SurrenderVotes;
    public List<PlayerSurrenderVote> PlayerVotes;
    public List<DisconnectRecord> DisconnectRecords;
    public int SurrenderAttempts;
    public bool SurrenderEnabled;
    public DateTime LastDisconnectTime;

    public MatchSurrenderData(string matchID)
    {
        MatchID = matchID;
        SurrenderVotes = new List<SurrenderVote>();
        PlayerVotes = new List<PlayerSurrenderVote>();
        DisconnectRecords = new List<DisconnectRecord>();
        SurrenderAttempts = 0;
        SurrenderEnabled = true;
        LastDisconnectTime = DateTime.MinValue;
    }
}

[Serializable]
public class SurrenderSystemData
{
    public Dictionary<string, MatchSurrenderData> MatchSurrenderData;
    public int MinGameTimeForSurrender;
    public int VoteDuration;
    public int MaxDisconnectPenaltyMinutes;
    public bool AllowSurrender;
    public bool AutoConfirmDisconnect;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public SurrenderSystemData()
    {
        MatchSurrenderData = new Dictionary<string, MatchSurrenderData>();
        MinGameTimeForSurrender = 5;
        VoteDuration = 60;
        MaxDisconnectPenaltyMinutes = 30;
        AllowSurrender = true;
        AutoConfirmDisconnect = true;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class SurrenderEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string MatchID;
    public string PlayerID;
    public string EventData;

    public SurrenderEvent(string eventID, string eventType, string matchID, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        MatchID = matchID;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class SurrenderSystemDataManager
{
    private static SurrenderSystemDataManager _instance;
    public static SurrenderSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SurrenderSystemDataManager();
            }
            return _instance;
        }
    }

    public SurrenderSystemData surrenderData;
    private List<SurrenderEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SurrenderSystemDataManager()
    {
        surrenderData = new SurrenderSystemData();
        recentEvents = new List<SurrenderEvent>();
        LoadSurrenderData();
    }

    public void SaveSurrenderData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SurrenderSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, surrenderData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存投降系统数据失败: " + e.Message);
        }
    }

    public void LoadSurrenderData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SurrenderSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    surrenderData = (SurrenderSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载投降系统数据失败: " + e.Message);
            surrenderData = new SurrenderSystemData();
        }
    }

    public void CreateSurrenderEvent(string eventType, string matchID, string playerID, string eventData)
    {
        string eventID = "surrender_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SurrenderEvent surrenderEvent = new SurrenderEvent(eventID, eventType, matchID, playerID, eventData);
        recentEvents.Add(surrenderEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<SurrenderEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
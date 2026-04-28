using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class ReconnectionRecord
{
    public string RecordID;
    public string PlayerID;
    public string MatchID;
    public DateTime DisconnectTime;
    public DateTime ReconnectTime;
    public int DisconnectDuration;
    public string DisconnectReason;
    public bool WasReconnected;
    public string ConnectionQuality;
    public int ReconnectAttempts;

    public ReconnectionRecord(string playerID, string matchID, string reason)
    {
        RecordID = "reconnect_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PlayerID = playerID;
        MatchID = matchID;
        DisconnectTime = DateTime.Now;
        ReconnectTime = DateTime.MinValue;
        DisconnectDuration = 0;
        DisconnectReason = reason;
        WasReconnected = false;
        ConnectionQuality = "unknown";
        ReconnectAttempts = 0;
    }
}

[Serializable]
public class MatchConnectionState
{
    public string MatchID;
    public Dictionary<string, bool> PlayerConnectionStatus;
    public Dictionary<string, DateTime> PlayerDisconnectTime;
    public Dictionary<string, int> PlayerReconnectAttempts;
    public int MaxReconnectAttempts;
    public int ReconnectTimeoutSeconds;
    public bool MatchPaused;
    public DateTime PauseTime;
    public int MaxPauseDuration;

    public MatchConnectionState(string matchID)
    {
        MatchID = matchID;
        PlayerConnectionStatus = new Dictionary<string, bool>();
        PlayerDisconnectTime = new Dictionary<string, DateTime>();
        PlayerReconnectAttempts = new Dictionary<string, int>();
        MaxReconnectAttempts = 3;
        ReconnectTimeoutSeconds = 60;
        MatchPaused = false;
        PauseTime = DateTime.MinValue;
        MaxPauseDuration = 300;
    }
}

[Serializable]
public class ReconnectionSystemData
{
    public Dictionary<string, ReconnectionRecord> ReconnectionRecords;
    public Dictionary<string, MatchConnectionState> MatchConnectionStates;
    public int MaxReconnectTimeout;
    public int MaxPauseDuration;
    public bool AutoPauseEnabled;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public ReconnectionSystemData()
    {
        ReconnectionRecords = new Dictionary<string, ReconnectionRecord>();
        MatchConnectionStates = new Dictionary<string, MatchConnectionState>();
        MaxReconnectTimeout = 60;
        MaxPauseDuration = 300;
        AutoPauseEnabled = true;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class ReconnectionEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string MatchID;
    public string EventData;

    public ReconnectionEvent(string eventID, string eventType, string playerID, string matchID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        MatchID = matchID;
        EventData = eventData;
    }
}

public class ReconnectionSystemDataManager
{
    private static ReconnectionSystemDataManager _instance;
    public static ReconnectionSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ReconnectionSystemDataManager();
            }
            return _instance;
        }
    }

    public ReconnectionSystemData reconnectionData;
    private List<ReconnectionEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private ReconnectionSystemDataManager()
    {
        reconnectionData = new ReconnectionSystemData();
        recentEvents = new List<ReconnectionEvent>();
        LoadReconnectionData();
    }

    public void SaveReconnectionData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ReconnectionSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, reconnectionData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存重连系统数据失败: " + e.Message);
        }
    }

    public void LoadReconnectionData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ReconnectionSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    reconnectionData = (ReconnectionSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载重连系统数据失败: " + e.Message);
            reconnectionData = new ReconnectionSystemData();
        }
    }

    public void CreateReconnectionEvent(string eventType, string playerID, string matchID, string eventData)
    {
        string eventID = "reconnect_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ReconnectionEvent reconnectionEvent = new ReconnectionEvent(eventID, eventType, playerID, matchID, eventData);
        recentEvents.Add(reconnectionEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<ReconnectionEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class ReconnectionSystemDetailedManager
{
    private static ReconnectionSystemDetailedManager _instance;
    public static ReconnectionSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ReconnectionSystemDetailedManager();
            }
            return _instance;
        }
    }

    private ReconnectionSystemData reconnectionData;
    private ReconnectionSystemDataManager dataManager;

    private ReconnectionSystemDetailedManager()
    {
        dataManager = ReconnectionSystemDataManager.Instance;
        reconnectionData = dataManager.reconnectionData;
    }

    public void InitializeMatchConnectionState(string matchID)
    {
        if (!reconnectionData.MatchConnectionStates.ContainsKey(matchID))
        {
            MatchConnectionState state = new MatchConnectionState(matchID);
            reconnectionData.MatchConnectionStates[matchID] = state;
            dataManager.SaveReconnectionData();
            Debug.Log("初始化比赛连接状态成功");
        }
    }

    public string RecordDisconnect(string playerID, string matchID, string reason)
    {
        ReconnectionRecord record = new ReconnectionRecord(playerID, matchID, reason);
        reconnectionData.ReconnectionRecords[record.RecordID] = record;
        
        if (reconnectionData.MatchConnectionStates.ContainsKey(matchID))
        {
            MatchConnectionState state = reconnectionData.MatchConnectionStates[matchID];
            state.PlayerConnectionStatus[playerID] = false;
            state.PlayerDisconnectTime[playerID] = DateTime.Now;
            state.PlayerReconnectAttempts[playerID] = 0;
            
            if (reconnectionData.AutoPauseEnabled)
            {
                state.MatchPaused = true;
                state.PauseTime = DateTime.Now;
            }
        }
        
        dataManager.CreateReconnectionEvent("disconnect", playerID, matchID, "玩家断线: " + reason);
        dataManager.SaveReconnectionData();
        Debug.Log("记录断线成功");
        return record.RecordID;
    }

    public bool AttemptReconnect(string playerID, string matchID)
    {
        if (!reconnectionData.MatchConnectionStates.ContainsKey(matchID))
        {
            return false;
        }
        
        MatchConnectionState state = reconnectionData.MatchConnectionStates[matchID];
        
        if (!state.PlayerConnectionStatus.ContainsKey(playerID) || state.PlayerConnectionStatus[playerID])
        {
            return false;
        }
        
        if (state.PlayerReconnectAttempts.ContainsKey(playerID))
        {
            state.PlayerReconnectAttempts[playerID]++;
        }
        else
        {
            state.PlayerReconnectAttempts[playerID] = 1;
        }
        
        if (state.PlayerReconnectAttempts[playerID] > state.MaxReconnectAttempts)
        {
            dataManager.CreateReconnectionEvent("reconnect_failed", playerID, matchID, "重连次数超限");
            Debug.LogError("重连次数超限");
            return false;
        }
        
        DateTime disconnectTime = state.PlayerDisconnectTime[playerID];
        int disconnectDuration = (int)(DateTime.Now - disconnectTime).TotalSeconds;
        if (disconnectDuration > reconnectionData.MaxReconnectTimeout)
        {
            dataManager.CreateReconnectionEvent("reconnect_timeout", playerID, matchID, "重连超时");
            Debug.LogError("重连超时");
            return false;
        }
        
        return true;
    }

    public bool CompleteReconnect(string playerID, string matchID)
    {
        if (!reconnectionData.MatchConnectionStates.ContainsKey(matchID))
        {
            return false;
        }
        
        MatchConnectionState state = reconnectionData.MatchConnectionStates[matchID];
        
        ReconnectionRecord record = null;
        foreach (ReconnectionRecord r in reconnectionData.ReconnectionRecords.Values)
        {
            if (r.PlayerID == playerID && r.MatchID == matchID && !r.WasReconnected)
            {
                record = r;
                break;
            }
        }
        
        if (record != null)
        {
            record.WasReconnected = true;
            record.ReconnectTime = DateTime.Now;
            record.DisconnectDuration = (int)(record.ReconnectTime - record.DisconnectTime).TotalSeconds;
        }
        
        state.PlayerConnectionStatus[playerID] = true;
        
        CheckAllPlayersReconnected(matchID);
        
        dataManager.CreateReconnectionEvent("reconnect_success", playerID, matchID, "重连成功");
        dataManager.SaveReconnectionData();
        Debug.Log("重连成功");
        return true;
    }

    private void CheckAllPlayersReconnected(string matchID)
    {
        if (!reconnectionData.MatchConnectionStates.ContainsKey(matchID))
        {
            return;
        }
        
        MatchConnectionState state = reconnectionData.MatchConnectionStates[matchID];
        bool allReconnected = true;
        
        foreach (bool connected in state.PlayerConnectionStatus.Values)
        {
            if (!connected)
            {
                allReconnected = false;
                break;
            }
        }
        
        if (allReconnected && state.MatchPaused)
        {
            state.MatchPaused = false;
            state.PauseTime = DateTime.MinValue;
            dataManager.CreateReconnectionEvent("match_resume", "system", matchID, "比赛恢复");
            Debug.Log("所有玩家已重连，比赛恢复");
        }
    }

    public void SetPlayerConnectionQuality(string playerID, string matchID, string quality)
    {
        foreach (ReconnectionRecord record in reconnectionData.ReconnectionRecords.Values)
        {
            if (record.PlayerID == playerID && record.MatchID == matchID)
            {
                record.ConnectionQuality = quality;
                break;
            }
        }
        dataManager.SaveReconnectionData();
    }

    public ReconnectionRecord GetReconnectionRecord(string recordID)
    {
        if (reconnectionData.ReconnectionRecords.ContainsKey(recordID))
        {
            return reconnectionData.ReconnectionRecords[recordID];
        }
        return null;
    }

    public MatchConnectionState GetMatchConnectionState(string matchID)
    {
        if (reconnectionData.MatchConnectionStates.ContainsKey(matchID))
        {
            return reconnectionData.MatchConnectionStates[matchID];
        }
        return null;
    }

    public List<ReconnectionRecord> GetPlayerReconnectionHistory(string playerID)
    {
        List<ReconnectionRecord> records = new List<ReconnectionRecord>();
        foreach (ReconnectionRecord record in reconnectionData.ReconnectionRecords.Values)
        {
            if (record.PlayerID == playerID)
            {
                records.Add(record);
            }
        }
        return records;
    }

    public int GetReconnectAttempts(string playerID, string matchID)
    {
        if (reconnectionData.MatchConnectionStates.ContainsKey(matchID))
        {
            MatchConnectionState state = reconnectionData.MatchConnectionStates[matchID];
            if (state.PlayerReconnectAttempts.ContainsKey(playerID))
            {
                return state.PlayerReconnectAttempts[playerID];
            }
        }
        return 0;
    }

    public void SetMaxReconnectAttempts(string matchID, int maxAttempts)
    {
        if (reconnectionData.MatchConnectionStates.ContainsKey(matchID))
        {
            reconnectionData.MatchConnectionStates[matchID].MaxReconnectAttempts = maxAttempts;
            dataManager.SaveReconnectionData();
        }
    }

    public void SetReconnectTimeout(string matchID, int timeoutSeconds)
    {
        if (reconnectionData.MatchConnectionStates.ContainsKey(matchID))
        {
            reconnectionData.MatchConnectionStates[matchID].ReconnectTimeoutSeconds = timeoutSeconds;
            dataManager.SaveReconnectionData();
        }
    }

    public void SetAutoPauseEnabled(bool enabled)
    {
        reconnectionData.AutoPauseEnabled = enabled;
        dataManager.SaveReconnectionData();
    }

    public void CleanupOldRecords(int days = 7)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<string> recordsToRemove = new List<string>();
        
        foreach (KeyValuePair<string, ReconnectionRecord> kvp in reconnectionData.ReconnectionRecords)
        {
            if (kvp.Value.ReconnectTime < cutoffDate)
            {
                recordsToRemove.Add(kvp.Key);
            }
        }
        
        foreach (string recordID in recordsToRemove)
        {
            reconnectionData.ReconnectionRecords.Remove(recordID);
        }
        
        List<string> matchesToRemove = new List<string>();
        foreach (KeyValuePair<string, MatchConnectionState> kvp in reconnectionData.MatchConnectionStates)
        {
            if (kvp.Value.MatchPaused == false)
            {
                bool allConnected = true;
                foreach (bool connected in kvp.Value.PlayerConnectionStatus.Values)
                {
                    if (!connected)
                    {
                        allConnected = false;
                        break;
                    }
                }
                if (allConnected)
                {
                    matchesToRemove.Add(kvp.Key);
                }
            }
        }
        
        foreach (string matchID in matchesToRemove)
        {
            reconnectionData.MatchConnectionStates.Remove(matchID);
        }
        
        if (recordsToRemove.Count > 0 || matchesToRemove.Count > 0)
        {
            dataManager.CreateReconnectionEvent("cleanup", "system", "", "清理旧重连数据");
            dataManager.SaveReconnectionData();
            Debug.Log("清理旧重连数据成功");
        }
    }

    public void SaveData()
    {
        dataManager.SaveReconnectionData();
    }

    public void LoadData()
    {
        dataManager.LoadReconnectionData();
    }

    public List<ReconnectionEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
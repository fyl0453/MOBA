using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Danmaku
{
    public string DanmakuID;
    public string MatchID;
    public string PlayerID;
    public string PlayerName;
    public string Content;
    public string Color;
    public int FontSize;
    public string DanmakuType;
    public float Timestamp;
    public DateTime SendTime;
    public int LikeCount;
    public bool IsPinned;
    public bool IsBlocked;

    public Danmaku(string matchID, string playerID, string playerName, string content, string color, int fontSize, string danmakuType, float timestamp)
    {
        DanmakuID = "danmaku_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchID = matchID;
        PlayerID = playerID;
        PlayerName = playerName;
        Content = content;
        Color = color;
        FontSize = fontSize;
        DanmakuType = danmakuType;
        Timestamp = timestamp;
        SendTime = DateTime.Now;
        LikeCount = 0;
        IsPinned = false;
        IsBlocked = false;
    }
}

[Serializable]
public class DanmakuConfig
{
    public bool DanmakuEnabled;
    public bool ShowDanmaku;
    public int DanmakuDensity;
    public int FontSize;
    public string DefaultColor;
    public bool EnablePinning;
    public bool EnableLiking;
    public bool EnableFiltering;
    public int MaxDanmakuPerMinute;
    public int MaxLength;

    public DanmakuConfig()
    {
        DanmakuEnabled = true;
        ShowDanmaku = true;
        DanmakuDensity = 3;
        FontSize = 24;
        DefaultColor = "#FFFFFF";
        EnablePinning = true;
        EnableLiking = true;
        EnableFiltering = true;
        MaxDanmakuPerMinute = 30;
        MaxLength = 50;
    }
}

[Serializable]
public class PlayerDanmakuData
{
    public string PlayerID;
    public List<Danmaku> SentDanmakus;
    public List<Danmaku> LikedDanmakus;
    public DanmakuConfig DanmakuConfig;
    public int TotalDanmakusSent;
    public int TotalDanmakusLiked;
    public DateTime LastDanmakuTime;
    public bool DanmakuBanned;

    public PlayerDanmakuData(string playerID)
    {
        PlayerID = playerID;
        SentDanmakus = new List<Danmaku>();
        LikedDanmakus = new List<Danmaku>();
        DanmakuConfig = new DanmakuConfig();
        TotalDanmakusSent = 0;
        TotalDanmakusLiked = 0;
        LastDanmakuTime = DateTime.MinValue;
        DanmakuBanned = false;
    }
}

[Serializable]
public class MatchDanmakuData
{
    public string MatchID;
    public List<Danmaku> Danmakus;
    public int TotalDanmakus;
    public int MaxConcurrentDanmakus;
    public DateTime LastDanmakuTime;
    public bool DanmakuEnabled;

    public MatchDanmakuData(string matchID)
    {
        MatchID = matchID;
        Danmakus = new List<Danmaku>();
        TotalDanmakus = 0;
        MaxConcurrentDanmakus = 0;
        LastDanmakuTime = DateTime.MinValue;
        DanmakuEnabled = true;
    }
}

[Serializable]
public class DanmakuSystemData
{
    public Dictionary<string, PlayerDanmakuData> PlayerDanmakuData;
    public Dictionary<string, MatchDanmakuData> MatchDanmakuData;
    public List<string> BannedWords;
    public int MaxDanmakusPerMatch;
    public int MaxDanmakusPerPlayerPerMatch;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public DanmakuSystemData()
    {
        PlayerDanmakuData = new Dictionary<string, PlayerDanmakuData>();
        MatchDanmakuData = new Dictionary<string, MatchDanmakuData>();
        BannedWords = new List<string>();
        MaxDanmakusPerMatch = 1000;
        MaxDanmakusPerPlayerPerMatch = 100;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
        InitializeBannedWords();
    }

    private void InitializeBannedWords()
    {
        BannedWords.Add("敏感词1");
        BannedWords.Add("敏感词2");
        BannedWords.Add("敏感词3");
    }

    public void AddPlayerDanmakuData(string playerID, PlayerDanmakuData danmakuData)
    {
        PlayerDanmakuData[playerID] = danmakuData;
    }

    public void AddMatchDanmakuData(string matchID, MatchDanmakuData danmakuData)
    {
        MatchDanmakuData[matchID] = danmakuData;
    }
}

[Serializable]
public class DanmakuEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string MatchID;
    public string DanmakuID;
    public string EventData;

    public DanmakuEvent(string eventID, string eventType, string playerID, string matchID, string danmakuID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        MatchID = matchID;
        DanmakuID = danmakuID;
        EventData = eventData;
    }
}

public class DanmakuSystemDataManager
{
    private static DanmakuSystemDataManager _instance;
    public static DanmakuSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DanmakuSystemDataManager();
            }
            return _instance;
        }
    }

    public DanmakuSystemData danmakuData;
    private List<DanmakuEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private DanmakuSystemDataManager()
    {
        danmakuData = new DanmakuSystemData();
        recentEvents = new List<DanmakuEvent>();
        LoadDanmakuData();
    }

    public void SaveDanmakuData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "DanmakuSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, danmakuData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存弹幕系统数据失败: " + e.Message);
        }
    }

    public void LoadDanmakuData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "DanmakuSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    danmakuData = (DanmakuSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载弹幕系统数据失败: " + e.Message);
            danmakuData = new DanmakuSystemData();
        }
    }

    public void CreateDanmakuEvent(string eventType, string playerID, string matchID, string danmakuID, string eventData)
    {
        string eventID = "danmaku_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        DanmakuEvent danmakuEvent = new DanmakuEvent(eventID, eventType, playerID, matchID, danmakuID, eventData);
        recentEvents.Add(danmakuEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<DanmakuEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
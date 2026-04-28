using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class RealNameInfo
{
    public string RealName;
    public string IDCard;
    public int Age;
    public bool IsVerified;
    public DateTime VerifyTime;
    public string VerificationStatus;

    public RealNameInfo(string realName, string idCard, int age)
    {
        RealName = realName;
        IDCard = idCard;
        Age = age;
        IsVerified = false;
        VerifyTime = DateTime.MinValue;
        VerificationStatus = "pending";
    }
}

[Serializable]
public class PlayTimeRecord
{
    public string RecordID;
    public string PlayerID;
    public DateTime LoginTime;
    public DateTime LogoutTime;
    public float PlayDuration;
    public DateTime Date;
    public bool IsCompleted;

    public PlayTimeRecord(string recordID, string playerID, DateTime loginTime)
    {
        RecordID = recordID;
        PlayerID = playerID;
        LoginTime = loginTime;
        LogoutTime = DateTime.MinValue;
        PlayDuration = 0;
        Date = loginTime.Date;
        IsCompleted = false;
    }
}

[Serializable]
public class DailyPlayTime
{
    public string Date;
    public float TotalPlayTime;
    public int LoginCount;
    public DateTime LastLoginTime;
    public DateTime LastLogoutTime;

    public DailyPlayTime(string date)
    {
        Date = date;
        TotalPlayTime = 0;
        LoginCount = 0;
        LastLoginTime = DateTime.MinValue;
        LastLogoutTime = DateTime.MinValue;
    }
}

[Serializable]
public class PlayerAntiAddictionData
{
    public string PlayerID;
    public RealNameInfo RealNameInfo;
    public bool IsMinor;
    public List<PlayTimeRecord> PlayTimeRecords;
    public Dictionary<string, DailyPlayTime> DailyPlayTimes;
    public float TodayPlayTime;
    public float ThisWeekPlayTime;
    public DateTime LastLoginTime;
    public DateTime LastLogoutTime;
    public bool IsLoginLimited;
    public bool IsPaymentLimited;

    public PlayerAntiAddictionData(string playerID)
    {
        PlayerID = playerID;
        RealNameInfo = null;
        IsMinor = false;
        PlayTimeRecords = new List<PlayTimeRecord>();
        DailyPlayTimes = new Dictionary<string, DailyPlayTime>();
        TodayPlayTime = 0;
        ThisWeekPlayTime = 0;
        LastLoginTime = DateTime.MinValue;
        LastLogoutTime = DateTime.MinValue;
        IsLoginLimited = false;
        IsPaymentLimited = false;
    }
}

[Serializable]
public class AntiAddictionConfig
{
    public int MinorMaxDailyPlayTime;
    public int MinorMaxWeeklyPlayTime;
    public int MinorMaxSingleSession;
    public int AdultMaxDailyPlayTime;
    public int BreakReminderInterval;
    public bool EnableRealNameVerification;
    public bool EnableMinorRestriction;
    public bool EnablePaymentLimit;
    public int MinorMaxPaymentPerMonth;
    public int MinorMaxPaymentPerTransaction;

    public AntiAddictionConfig()
    {
        MinorMaxDailyPlayTime = 120;
        MinorMaxWeeklyPlayTime = 840;
        MinorMaxSingleSession = 60;
        AdultMaxDailyPlayTime = 720;
        BreakReminderInterval = 30;
        EnableRealNameVerification = true;
        EnableMinorRestriction = true;
        EnablePaymentLimit = true;
        MinorMaxPaymentPerMonth = 200;
        MinorMaxPaymentPerTransaction = 50;
    }
}

[Serializable]
public class AntiAddictionSystemData
{
    public AntiAddictionConfig Config;
    public Dictionary<string, PlayerAntiAddictionData> PlayerAntiAddictionData;
    public int TotalVerifiedPlayers;
    public int TotalMinorPlayers;
    public DateTime LastSystemUpdate;

    public AntiAddictionSystemData()
    {
        Config = new AntiAddictionConfig();
        PlayerAntiAddictionData = new Dictionary<string, PlayerAntiAddictionData>();
        TotalVerifiedPlayers = 0;
        TotalMinorPlayers = 0;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddPlayerAntiAddictionData(string playerID, PlayerAntiAddictionData data)
    {
        PlayerAntiAddictionData[playerID] = data;
    }
}

[Serializable]
public class AntiAddictionEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EventData;

    public AntiAddictionEvent(string eventID, string eventType, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class AntiAddictionSystemDataManager
{
    private static AntiAddictionSystemDataManager _instance;
    public static AntiAddictionSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AntiAddictionSystemDataManager();
            }
            return _instance;
        }
    }

    public AntiAddictionSystemData antiAddictionData;
    private List<AntiAddictionEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private AntiAddictionSystemDataManager()
    {
        antiAddictionData = new AntiAddictionSystemData();
        recentEvents = new List<AntiAddictionEvent>();
        LoadAntiAddictionData();
    }

    public void SaveAntiAddictionData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "AntiAddictionSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, antiAddictionData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存防沉迷系统数据失败: " + e.Message);
        }
    }

    public void LoadAntiAddictionData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "AntiAddictionSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    antiAddictionData = (AntiAddictionSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载防沉迷系统数据失败: " + e.Message);
            antiAddictionData = new AntiAddictionSystemData();
        }
    }

    public void CreateAntiAddictionEvent(string eventType, string playerID, string eventData)
    {
        string eventID = "antiaddiction_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AntiAddictionEvent antiAddictionEvent = new AntiAddictionEvent(eventID, eventType, playerID, eventData);
        recentEvents.Add(antiAddictionEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<AntiAddictionEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
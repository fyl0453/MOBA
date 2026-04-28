using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class RankProtectionCard
{
    public string CardID;
    public string CardType;
    public string RankTier;
    public string RankDivision;
    public int ProtectionCount;
    public string Rarity;
    public string Description;
    public string IconURL;
    public DateTime ExpiryDate;
    public bool IsUsed;
    public DateTime UsedDate;
    public DateTime AcquisitionDate;

    public RankProtectionCard(string cardType, string rankTier, string rankDivision, int protectionCount, string rarity, string description, string iconURL, int expiryDays = 30)
    {
        CardID = "protection_card_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CardType = cardType;
        RankTier = rankTier;
        RankDivision = rankDivision;
        ProtectionCount = protectionCount;
        Rarity = rarity;
        Description = description;
        IconURL = iconURL;
        ExpiryDate = DateTime.Now.AddDays(expiryDays);
        IsUsed = false;
        UsedDate = DateTime.MinValue;
        AcquisitionDate = DateTime.Now;
    }
}

[Serializable]
public class ProtectionRecord
{
    public string RecordID;
    public string PlayerID;
    public string CardID;
    public string MatchID;
    public string RankTier;
    public string RankDivision;
    public int RankPointsBefore;
    public int RankPointsAfter;
    public bool PreventedDemotion;
    public DateTime ProtectionTime;
    public string ProtectionResult;

    public ProtectionRecord(string playerID, string cardID, string matchID, string rankTier, string rankDivision, int rankPointsBefore, int rankPointsAfter, bool preventedDemotion, string protectionResult)
    {
        RecordID = "protection_record_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PlayerID = playerID;
        CardID = cardID;
        MatchID = matchID;
        RankTier = rankTier;
        RankDivision = rankDivision;
        RankPointsBefore = rankPointsBefore;
        RankPointsAfter = rankPointsAfter;
        PreventedDemotion = preventedDemotion;
        ProtectionTime = DateTime.Now;
        ProtectionResult = protectionResult;
    }
}

[Serializable]
public class PlayerRankProtectionData
{
    public string PlayerID;
    public List<RankProtectionCard> ProtectionCards;
    public List<ProtectionRecord> ProtectionRecords;
    public int TotalCardsAcquired;
    public int TotalCardsUsed;
    public int TotalDemotionsPrevented;
    public DateTime LastCardAcquisitionTime;
    public DateTime LastCardUseTime;
    public DateTime LastDemotionPreventionTime;

    public PlayerRankProtectionData(string playerID)
    {
        PlayerID = playerID;
        ProtectionCards = new List<RankProtectionCard>();
        ProtectionRecords = new List<ProtectionRecord>();
        TotalCardsAcquired = 0;
        TotalCardsUsed = 0;
        TotalDemotionsPrevented = 0;
        LastCardAcquisitionTime = DateTime.MinValue;
        LastCardUseTime = DateTime.MinValue;
        LastDemotionPreventionTime = DateTime.MinValue;
    }
}

[Serializable]
public class RankProtectionCardSystemData
{
    public Dictionary<string, PlayerRankProtectionData> PlayerRankProtectionData;
    public List<RankProtectionCard> AllProtectionCards;
    public List<ProtectionRecord> AllProtectionRecords;
    public List<string> CardTypes;
    public List<string> RankTiers;
    public int MaxCardsPerPlayer;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public RankProtectionCardSystemData()
    {
        PlayerRankProtectionData = new Dictionary<string, PlayerRankProtectionData>();
        AllProtectionCards = new List<RankProtectionCard>();
        AllProtectionRecords = new List<ProtectionRecord>();
        CardTypes = new List<string> { "normal", "premium", "seasonal" };
        RankTiers = new List<string> { "Bronze", "Silver", "Gold", "Platinum", "Diamond", "Master", "GrandMaster" };
        MaxCardsPerPlayer = 10;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddPlayerRankProtectionData(string playerID, PlayerRankProtectionData protectionData)
    {
        PlayerRankProtectionData[playerID] = protectionData;
    }

    public void AddProtectionCard(RankProtectionCard card)
    {
        AllProtectionCards.Add(card);
    }

    public void AddProtectionRecord(ProtectionRecord record)
    {
        AllProtectionRecords.Add(record);
    }
}

[Serializable]
public class RankProtectionEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string CardID;
    public string EventData;

    public RankProtectionEvent(string eventID, string eventType, string playerID, string cardID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        CardID = cardID;
        EventData = eventData;
    }
}

public class RankProtectionCardSystemDataManager
{
    private static RankProtectionCardSystemDataManager _instance;
    public static RankProtectionCardSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RankProtectionCardSystemDataManager();
            }
            return _instance;
        }
    }

    public RankProtectionCardSystemData protectionData;
    private List<RankProtectionEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private RankProtectionCardSystemDataManager()
    {
        protectionData = new RankProtectionCardSystemData();
        recentEvents = new List<RankProtectionEvent>();
        LoadProtectionData();
    }

    public void SaveProtectionData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "RankProtectionCardSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, protectionData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存段位保护卡系统数据失败: " + e.Message);
        }
    }

    public void LoadProtectionData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "RankProtectionCardSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    protectionData = (RankProtectionCardSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载段位保护卡系统数据失败: " + e.Message);
            protectionData = new RankProtectionCardSystemData();
        }
    }

    public void CreateProtectionEvent(string eventType, string playerID, string cardID, string eventData)
    {
        string eventID = "protection_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RankProtectionEvent protectionEvent = new RankProtectionEvent(eventID, eventType, playerID, cardID, eventData);
        recentEvents.Add(protectionEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<RankProtectionEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class NameChangeCard
{
    public string CardID;
    public string CardType;
    public string Rarity;
    public string Description;
    public string IconURL;
    public DateTime ExpiryDate;
    public bool IsUsed;
    public DateTime UsedDate;
    public DateTime AcquisitionDate;

    public NameChangeCard(string cardType, string rarity, string description, string iconURL, int expiryDays = 30)
    {
        CardID = "name_change_card_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CardType = cardType;
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
public class NameChangeRecord
{
    public string RecordID;
    public string PlayerID;
    public string CardID;
    public string OldName;
    public string NewName;
    public DateTime ChangeTime;
    public string ChangeReason;
    public bool IsSuccessful;

    public NameChangeRecord(string playerID, string cardID, string oldName, string newName, string changeReason, bool isSuccessful)
    {
        RecordID = "name_change_record_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PlayerID = playerID;
        CardID = cardID;
        OldName = oldName;
        NewName = newName;
        ChangeTime = DateTime.Now;
        ChangeReason = changeReason;
        IsSuccessful = isSuccessful;
    }
}

[Serializable]
public class PlayerNameChangeData
{
    public string PlayerID;
    public List<NameChangeCard> NameChangeCards;
    public List<NameChangeRecord> NameChangeRecords;
    public int TotalCardsAcquired;
    public int TotalCardsUsed;
    public int TotalNameChanges;
    public DateTime LastCardAcquisitionTime;
    public DateTime LastCardUseTime;
    public DateTime LastNameChangeTime;
    public string CurrentName;
    public int NameChangeCooldownDays;
    public DateTime NextAvailableChange;

    public PlayerNameChangeData(string playerID, string currentName)
    {
        PlayerID = playerID;
        NameChangeCards = new List<NameChangeCard>();
        NameChangeRecords = new List<NameChangeRecord>();
        TotalCardsAcquired = 0;
        TotalCardsUsed = 0;
        TotalNameChanges = 0;
        LastCardAcquisitionTime = DateTime.MinValue;
        LastCardUseTime = DateTime.MinValue;
        LastNameChangeTime = DateTime.MinValue;
        CurrentName = currentName;
        NameChangeCooldownDays = 7;
        NextAvailableChange = DateTime.MinValue;
    }
}

[Serializable]
public class NameChangeCardSystemData
{
    public Dictionary<string, PlayerNameChangeData> PlayerNameChangeData;
    public List<NameChangeCard> AllNameChangeCards;
    public List<NameChangeRecord> AllNameChangeRecords;
    public List<string> CardTypes;
    public int MaxCardsPerPlayer;
    public int MinNameLength;
    public int MaxNameLength;
    public List<string> BannedNames;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public NameChangeCardSystemData()
    {
        PlayerNameChangeData = new Dictionary<string, PlayerNameChangeData>();
        AllNameChangeCards = new List<NameChangeCard>();
        AllNameChangeRecords = new List<NameChangeRecord>();
        CardTypes = new List<string> { "normal", "premium" };
        MaxCardsPerPlayer = 5;
        MinNameLength = 2;
        MaxNameLength = 12;
        BannedNames = new List<string>();
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
        InitializeBannedNames();
    }

    private void InitializeBannedNames()
    {
        BannedNames.Add("admin");
        BannedNames.Add("system");
        BannedNames.Add("gm");
        BannedNames.Add("mod");
    }

    public void AddPlayerNameChangeData(string playerID, PlayerNameChangeData nameChangeData)
    {
        PlayerNameChangeData[playerID] = nameChangeData;
    }

    public void AddNameChangeCard(NameChangeCard card)
    {
        AllNameChangeCards.Add(card);
    }

    public void AddNameChangeRecord(NameChangeRecord record)
    {
        AllNameChangeRecords.Add(record);
    }
}

[Serializable]
public class NameChangeEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string CardID;
    public string EventData;

    public NameChangeEvent(string eventID, string eventType, string playerID, string cardID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        CardID = cardID;
        EventData = eventData;
    }
}

public class NameChangeCardSystemDataManager
{
    private static NameChangeCardSystemDataManager _instance;
    public static NameChangeCardSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NameChangeCardSystemDataManager();
            }
            return _instance;
        }
    }

    public NameChangeCardSystemData nameChangeData;
    private List<NameChangeEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private NameChangeCardSystemDataManager()
    {
        nameChangeData = new NameChangeCardSystemData();
        recentEvents = new List<NameChangeEvent>();
        LoadNameChangeData();
    }

    public void SaveNameChangeData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "NameChangeCardSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, nameChangeData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存改名卡系统数据失败: " + e.Message);
        }
    }

    public void LoadNameChangeData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "NameChangeCardSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    nameChangeData = (NameChangeCardSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载改名卡系统数据失败: " + e.Message);
            nameChangeData = new NameChangeCardSystemData();
        }
    }

    public void CreateNameChangeEvent(string eventType, string playerID, string cardID, string eventData)
    {
        string eventID = "name_change_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        NameChangeEvent nameChangeEvent = new NameChangeEvent(eventID, eventType, playerID, cardID, eventData);
        recentEvents.Add(nameChangeEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<NameChangeEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
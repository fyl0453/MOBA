using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class ExperienceCard
{
    public string CardID;
    public string CardType;
    public string AssociatedID;
    public string AssociatedName;
    public int DurationHours;
    public string Rarity;
    public string Description;
    public string IconURL;
    public DateTime ExpiryDate;
    public bool IsUsed;
    public DateTime UsedDate;
    public DateTime AcquisitionDate;

    public ExperienceCard(string cardType, string associatedID, string associatedName, int durationHours, string rarity, string description, string iconURL)
    {
        CardID = "card_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CardType = cardType;
        AssociatedID = associatedID;
        AssociatedName = associatedName;
        DurationHours = durationHours;
        Rarity = rarity;
        Description = description;
        IconURL = iconURL;
        ExpiryDate = DateTime.Now.AddHours(durationHours);
        IsUsed = false;
        UsedDate = DateTime.MinValue;
        AcquisitionDate = DateTime.Now;
    }
}

[Serializable]
public class HeroExperienceCard : ExperienceCard
{
    public string HeroType;
    public int HeroLevel;
    public bool UnlockedSkills;

    public HeroExperienceCard(string heroID, string heroName, int durationHours, string rarity, string description, string iconURL, string heroType, int heroLevel, bool unlockedSkills)
        : base("hero", heroID, heroName, durationHours, rarity, description, iconURL)
    {
        HeroType = heroType;
        HeroLevel = heroLevel;
        UnlockedSkills = unlockedSkills;
    }
}

[Serializable]
public class SkinExperienceCard : ExperienceCard
{
    public string SkinRarity;
    public string HeroID;
    public string HeroName;
    public bool HasSpecialEffects;

    public SkinExperienceCard(string skinID, string skinName, int durationHours, string rarity, string description, string iconURL, string skinRarity, string heroID, string heroName, bool hasSpecialEffects)
        : base("skin", skinID, skinName, durationHours, rarity, description, iconURL)
    {
        SkinRarity = skinRarity;
        HeroID = heroID;
        HeroName = heroName;
        HasSpecialEffects = hasSpecialEffects;
    }
}

[Serializable]
public class PlayerExperienceCardData
{
    public string PlayerID;
    public List<ExperienceCard> ExperienceCards;
    public List<string> UsedCardHistory;
    public int TotalCardsAcquired;
    public int TotalCardsUsed;
    public int TotalCardsExpired;
    public DateTime LastCardAcquisitionTime;
    public DateTime LastCardUseTime;

    public PlayerExperienceCardData(string playerID)
    {
        PlayerID = playerID;
        ExperienceCards = new List<ExperienceCard>();
        UsedCardHistory = new List<string>();
        TotalCardsAcquired = 0;
        TotalCardsUsed = 0;
        TotalCardsExpired = 0;
        LastCardAcquisitionTime = DateTime.MinValue;
        LastCardUseTime = DateTime.MinValue;
    }
}

[Serializable]
public class ExperienceCardSystemData
{
    public Dictionary<string, PlayerExperienceCardData> PlayerExperienceCardData;
    public List<ExperienceCard> AllExperienceCards;
    public List<string> CardTypes;
    public int MaxCardsPerPlayer;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public ExperienceCardSystemData()
    {
        PlayerExperienceCardData = new Dictionary<string, PlayerExperienceCardData>();
        AllExperienceCards = new List<ExperienceCard>();
        CardTypes = new List<string> { "hero", "skin" };
        MaxCardsPerPlayer = 50;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddPlayerExperienceCardData(string playerID, PlayerExperienceCardData cardData)
    {
        PlayerExperienceCardData[playerID] = cardData;
    }

    public void AddExperienceCard(ExperienceCard card)
    {
        AllExperienceCards.Add(card);
    }
}

[Serializable]
public class ExperienceCardEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string CardID;
    public string EventData;

    public ExperienceCardEvent(string eventID, string eventType, string playerID, string cardID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        CardID = cardID;
        EventData = eventData;
    }
}

public class ExperienceCardSystemDataManager
{
    private static ExperienceCardSystemDataManager _instance;
    public static ExperienceCardSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ExperienceCardSystemDataManager();
            }
            return _instance;
        }
    }

    public ExperienceCardSystemData cardData;
    private List<ExperienceCardEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private ExperienceCardSystemDataManager()
    {
        cardData = new ExperienceCardSystemData();
        recentEvents = new List<ExperienceCardEvent>();
        LoadCardData();
    }

    public void SaveCardData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ExperienceCardSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, cardData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存体验卡系统数据失败: " + e.Message);
        }
    }

    public void LoadCardData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ExperienceCardSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    cardData = (ExperienceCardSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载体验卡系统数据失败: " + e.Message);
            cardData = new ExperienceCardSystemData();
        }
    }

    public void CreateCardEvent(string eventType, string playerID, string cardID, string eventData)
    {
        string eventID = "card_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ExperienceCardEvent cardEvent = new ExperienceCardEvent(eventID, eventType, playerID, cardID, eventData);
        recentEvents.Add(cardEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<ExperienceCardEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
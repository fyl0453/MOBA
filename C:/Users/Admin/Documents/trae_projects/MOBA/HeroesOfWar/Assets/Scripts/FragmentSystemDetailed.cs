using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Fragment
{
    public string FragmentID;
    public string FragmentType;
    public string AssociatedID;
    public string AssociatedName;
    public int RequiredCount;
    public string Rarity;
    public string Description;
    public string IconURL;
    public DateTime AddedDate;

    public Fragment(string fragmentType, string associatedID, string associatedName, int requiredCount, string rarity, string description, string iconURL)
    {
        FragmentID = "fragment_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        FragmentType = fragmentType;
        AssociatedID = associatedID;
        AssociatedName = associatedName;
        RequiredCount = requiredCount;
        Rarity = rarity;
        Description = description;
        IconURL = iconURL;
        AddedDate = DateTime.Now;
    }
}

[Serializable]
public class PlayerFragmentData
{
    public string PlayerID;
    public Dictionary<string, int> FragmentInventory;
    public List<string> ExchangeHistory;
    public int TotalFragmentsCollected;
    public int TotalExchanges;
    public DateTime LastExchangeTime;
    public DateTime LastFragmentAcquisitionTime;

    public PlayerFragmentData(string playerID)
    {
        PlayerID = playerID;
        FragmentInventory = new Dictionary<string, int>();
        ExchangeHistory = new List<string>();
        TotalFragmentsCollected = 0;
        TotalExchanges = 0;
        LastExchangeTime = DateTime.MinValue;
        LastFragmentAcquisitionTime = DateTime.MinValue;
    }
}

[Serializable]
public class ExchangeOffer
{
    public string OfferID;
    public string FragmentID;
    public int RequiredFragments;
    public string RewardType;
    public string RewardID;
    public string RewardName;
    public int RewardQuantity;
    public string OfferType;
    public DateTime StartTime;
    public DateTime EndTime;
    public bool IsActive;
    public int MaxExchanges;
    public int CurrentExchanges;

    public ExchangeOffer(string fragmentID, int requiredFragments, string rewardType, string rewardID, string rewardName, int rewardQuantity, string offerType, DateTime startTime, DateTime endTime, int maxExchanges)
    {
        OfferID = "offer_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        FragmentID = fragmentID;
        RequiredFragments = requiredFragments;
        RewardType = rewardType;
        RewardID = rewardID;
        RewardName = rewardName;
        RewardQuantity = rewardQuantity;
        OfferType = offerType;
        StartTime = startTime;
        EndTime = endTime;
        IsActive = true;
        MaxExchanges = maxExchanges;
        CurrentExchanges = 0;
    }
}

[Serializable]
public class FragmentSystemData
{
    public Dictionary<string, PlayerFragmentData> PlayerFragmentData;
    public List<Fragment> AllFragments;
    public List<ExchangeOffer> ExchangeOffers;
    public List<string> FragmentTypes;
    public int MaxFragmentsPerPlayer;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public FragmentSystemData()
    {
        PlayerFragmentData = new Dictionary<string, PlayerFragmentData>();
        AllFragments = new List<Fragment>();
        ExchangeOffers = new List<ExchangeOffer>();
        FragmentTypes = new List<string> { "hero", "skin", "emblem", "item" };
        MaxFragmentsPerPlayer = 1000;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddPlayerFragmentData(string playerID, PlayerFragmentData fragmentData)
    {
        PlayerFragmentData[playerID] = fragmentData;
    }

    public void AddFragment(Fragment fragment)
    {
        AllFragments.Add(fragment);
    }

    public void AddExchangeOffer(ExchangeOffer offer)
    {
        ExchangeOffers.Add(offer);
    }
}

[Serializable]
public class FragmentEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string FragmentID;
    public string EventData;

    public FragmentEvent(string eventID, string eventType, string playerID, string fragmentID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        FragmentID = fragmentID;
        EventData = eventData;
    }
}

public class FragmentSystemDataManager
{
    private static FragmentSystemDataManager _instance;
    public static FragmentSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FragmentSystemDataManager();
            }
            return _instance;
        }
    }

    public FragmentSystemData fragmentData;
    private List<FragmentEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private FragmentSystemDataManager()
    {
        fragmentData = new FragmentSystemData();
        recentEvents = new List<FragmentEvent>();
        LoadFragmentData();
    }

    public void SaveFragmentData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "FragmentSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, fragmentData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存碎片系统数据失败: " + e.Message);
        }
    }

    public void LoadFragmentData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "FragmentSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    fragmentData = (FragmentSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载碎片系统数据失败: " + e.Message);
            fragmentData = new FragmentSystemData();
        }
    }

    public void CreateFragmentEvent(string eventType, string playerID, string fragmentID, string eventData)
    {
        string eventID = "fragment_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        FragmentEvent fragmentEvent = new FragmentEvent(eventID, eventType, playerID, fragmentID, eventData);
        recentEvents.Add(fragmentEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<FragmentEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
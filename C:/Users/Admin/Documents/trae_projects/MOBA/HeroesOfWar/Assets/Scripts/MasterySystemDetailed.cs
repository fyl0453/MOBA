using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class HeroMastery
{
    public string MasteryID;
    public string PlayerID;
    public string HeroID;
    public int Level;
    public int Exp;
    public int TotalGames;
    public int WinGames;
    public float WinRate;
    public int TotalKills;
    public int TotalAssists;
    public int TotalDamage;
    public DateTime LastPlayed;
    public string MasteryTitle;
    public bool IsUnlocked;

    public HeroMastery(string masteryID, string playerID, string heroID)
    {
        MasteryID = masteryID;
        PlayerID = playerID;
        HeroID = heroID;
        Level = 1;
        Exp = 0;
        TotalGames = 0;
        WinGames = 0;
        WinRate = 0;
        TotalKills = 0;
        TotalAssists = 0;
        TotalDamage = 0;
        LastPlayed = DateTime.MinValue;
        MasteryTitle = "见习";
        IsUnlocked = false;
    }
}

[Serializable]
public class MasteryLevel
{
    public int Level;
    public string Title;
    public int RequiredExp;
    public string Description;
    public string IconName;
    public List<MasteryReward> Rewards;

    public MasteryLevel(int level, string title, int requiredExp, string description)
    {
        Level = level;
        Title = title;
        RequiredExp = requiredExp;
        Description = description;
        IconName = "";
        Rewards = new List<MasteryReward>();
    }
}

[Serializable]
public class MasteryReward
{
    public string RewardID;
    public string RewardName;
    public string RewardType;
    public int RewardValue;
    public string IconName;

    public MasteryReward(string rewardID, string rewardName, string rewardType, int rewardValue)
    {
        RewardID = rewardID;
        RewardName = rewardName;
        RewardType = rewardType;
        RewardValue = rewardValue;
        IconName = "";
    }
}

[Serializable]
public class PlayerMasteryData
{
    public string PlayerID;
    public Dictionary<string, HeroMastery> HeroMasteries;
    public int TotalMasteryLevel;
    public int TotalMasteryExp;
    public List<string> UnlockedHeroes;
    public DateTime LastUpdateTime;

    public PlayerMasteryData(string playerID)
    {
        PlayerID = playerID;
        HeroMasteries = new Dictionary<string, HeroMastery>();
        TotalMasteryLevel = 0;
        TotalMasteryExp = 0;
        UnlockedHeroes = new List<string>();
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class MasterySystemData
{
    public List<MasteryLevel> MasteryLevels;
    public Dictionary<string, PlayerMasteryData> PlayerMasteryData;
    public DateTime LastUpdateTime;

    public MasterySystemData()
    {
        MasteryLevels = new List<MasteryLevel>();
        PlayerMasteryData = new Dictionary<string, PlayerMasteryData>();
        LastUpdateTime = DateTime.Now;
        InitializeDefaultMasteryLevels();
    }

    private void InitializeDefaultMasteryLevels()
    {
        MasteryLevel level1 = new MasteryLevel(1, "见习", 0, "初始熟练度等级");
        MasteryLevels.Add(level1);

        MasteryLevel level2 = new MasteryLevel(2, "资深", 1000, "熟练掌握英雄");
        MasteryLevels.Add(level2);

        MasteryLevel level3 = new MasteryLevel(3, "精英", 3000, "精通英雄技巧");
        MasteryLevels.Add(level3);

        MasteryLevel level4 = new MasteryLevel(4, "宗师", 6000, "英雄大师");
        MasteryLevels.Add(level4);

        MasteryLevel level5 = new MasteryLevel(5, "传说", 10000, "英雄传说");
        MasteryLevels.Add(level5);
    }

    public void AddMasteryLevel(MasteryLevel level)
    {
        MasteryLevels.Add(level);
    }

    public void AddPlayerMasteryData(string playerID, PlayerMasteryData data)
    {
        PlayerMasteryData[playerID] = data;
    }
}

[Serializable]
public class MasteryEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string HeroID;
    public string EventData;

    public MasteryEvent(string eventID, string eventType, string playerID, string heroID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        HeroID = heroID;
        EventData = eventData;
    }
}

public class MasterySystemDataManager
{
    private static MasterySystemDataManager _instance;
    public static MasterySystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MasterySystemDataManager();
            }
            return _instance;
        }
    }

    public MasterySystemData masteryData;
    private List<MasteryEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private MasterySystemDataManager()
    {
        masteryData = new MasterySystemData();
        recentEvents = new List<MasteryEvent>();
        LoadMasteryData();
    }

    public void SaveMasteryData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MasterySystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, masteryData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存熟练度系统数据失败: " + e.Message);
        }
    }

    public void LoadMasteryData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MasterySystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    masteryData = (MasterySystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载熟练度系统数据失败: " + e.Message);
            masteryData = new MasterySystemData();
        }
    }

    public void CreateMasteryEvent(string eventType, string playerID, string heroID, string eventData)
    {
        string eventID = "mastery_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MasteryEvent masteryEvent = new MasteryEvent(eventID, eventType, playerID, heroID, eventData);
        recentEvents.Add(masteryEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<MasteryEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
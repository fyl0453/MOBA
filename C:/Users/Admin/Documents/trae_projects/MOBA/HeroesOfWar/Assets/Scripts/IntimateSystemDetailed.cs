using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class IntimateRelationship
{
    public string RelationshipID;
    public string PlayerID1;
    public string PlayerID2;
    public string PlayerName1;
    public string PlayerName2;
    public int RelationshipType;
    public int IntimacyLevel;
    public int IntimacyPoints;
    public DateTime EstablishTime;
    public DateTime LastInteractionTime;
    public bool IsActive;
    public string RelationshipName;

    public IntimateRelationship(string relationshipID, string playerID1, string playerID2, string playerName1, string playerName2, int relationshipType)
    {
        RelationshipID = relationshipID;
        PlayerID1 = playerID1;
        PlayerID2 = playerID2;
        PlayerName1 = playerName1;
        PlayerName2 = playerName2;
        RelationshipType = relationshipType;
        IntimacyLevel = 1;
        IntimacyPoints = 0;
        EstablishTime = DateTime.Now;
        LastInteractionTime = DateTime.Now;
        IsActive = true;
        RelationshipName = GetRelationshipTypeName(relationshipType);
    }

    private string GetRelationshipTypeName(int type)
    {
        switch (type)
        {
            case 0: return "好友";
            case 1: return "死党";
            case 2: return "恋人";
            case 3: return "基友";
            case 4: return "闺蜜";
            default: return "好友";
        }
    }
}

[Serializable]
public class IntimacyLevelConfig
{
    public int Level;
    public int RequiredPoints;
    public string LevelName;
    public List<string> UnlockRewards;
    public string LevelDescription;

    public IntimacyLevelConfig(int level, int requiredPoints, string levelName, List<string> unlockRewards, string levelDescription)
    {
        Level = level;
        RequiredPoints = requiredPoints;
        LevelName = levelName;
        UnlockRewards = unlockRewards;
        LevelDescription = levelDescription;
    }
}

[Serializable]
public class IntimacyInteraction
{
    public string InteractionID;
    public string PlayerID;
    public string TargetPlayerID;
    public int InteractionType;
    public int PointsAdded;
    public DateTime InteractionTime;
    public string InteractionDescription;

    public IntimacyInteraction(string interactionID, string playerID, string targetPlayerID, int interactionType, int pointsAdded, string interactionDescription)
    {
        InteractionID = interactionID;
        PlayerID = playerID;
        TargetPlayerID = targetPlayerID;
        InteractionType = interactionType;
        PointsAdded = pointsAdded;
        InteractionTime = DateTime.Now;
        InteractionDescription = interactionDescription;
    }
}

[Serializable]
public class PlayerIntimacyData
{
    public string PlayerID;
    public List<IntimateRelationship> Relationships;
    public int TotalIntimacyPoints;
    public int ActiveRelationshipsCount;
    public DateTime LastInteractionTime;

    public PlayerIntimacyData(string playerID)
    {
        PlayerID = playerID;
        Relationships = new List<IntimateRelationship>();
        TotalIntimacyPoints = 0;
        ActiveRelationshipsCount = 0;
        LastInteractionTime = DateTime.MinValue;
    }
}

[Serializable]
public class IntimateSystemData
{
    public List<IntimateRelationship> AllRelationships;
    public List<IntimacyLevelConfig> LevelConfigs;
    public List<IntimacyInteraction> RecentInteractions;
    public Dictionary<string, PlayerIntimacyData> PlayerIntimacyData;
    public int MaxRelationshipsPerPlayer;
    public int DailyInteractionLimit;
    public DateTime LastSystemUpdate;

    public IntimateSystemData()
    {
        AllRelationships = new List<IntimateRelationship>();
        LevelConfigs = new List<IntimacyLevelConfig>();
        RecentInteractions = new List<IntimacyInteraction>();
        PlayerIntimacyData = new Dictionary<string, PlayerIntimacyData>();
        MaxRelationshipsPerPlayer = 10;
        DailyInteractionLimit = 50;
        LastSystemUpdate = DateTime.Now;
        InitializeLevelConfigs();
    }

    private void InitializeLevelConfigs()
    {
        LevelConfigs.Add(new IntimacyLevelConfig(1, 0, "初识", new List<string>(), "刚刚认识的朋友"));
        LevelConfigs.Add(new IntimacyLevelConfig(2, 100, "熟悉", new List<string> { "专属聊天表情" }, "逐渐熟悉的伙伴"));
        LevelConfigs.Add(new IntimacyLevelConfig(3, 300, "好友", new List<string> { "亲密关系徽章" }, "亲密的好朋友"));
        LevelConfigs.Add(new IntimacyLevelConfig(4, 600, "挚友", new List<string> { "专属头像框" }, "无话不谈的挚友"));
        LevelConfigs.Add(new IntimacyLevelConfig(5, 1000, "莫逆", new List<string> { "亲密关系称号" }, "亲密无间的伙伴"));
        LevelConfigs.Add(new IntimacyLevelConfig(6, 1500, "生死之交", new List<string> { "专属互动动作" }, "可以交付后背的伙伴"));
        LevelConfigs.Add(new IntimacyLevelConfig(7, 2100, "刎颈之交", new List<string> { "专属回城特效" }, "情同手足的兄弟"));
        LevelConfigs.Add(new IntimacyLevelConfig(8, 2800, "生死与共", new List<string> { "专属击杀特效" }, "同生共死的伙伴"));
        LevelConfigs.Add(new IntimacyLevelConfig(9, 3600, "海誓山盟", new List<string> { "专属情侣皮肤" }, "生死不渝的恋人"));
        LevelConfigs.Add(new IntimacyLevelConfig(10, 4500, "天荒地老", new List<string> { "专属情侣头像框" }, "永恒不变的感情"));
    }

    public void AddRelationship(IntimateRelationship relationship)
    {
        AllRelationships.Add(relationship);
    }

    public void AddLevelConfig(IntimacyLevelConfig config)
    {
        LevelConfigs.Add(config);
    }

    public void AddInteraction(IntimacyInteraction interaction)
    {
        RecentInteractions.Add(interaction);
        if (RecentInteractions.Count > 1000)
        {
            RecentInteractions.RemoveRange(0, RecentInteractions.Count - 1000);
        }
    }

    public void AddPlayerIntimacyData(string playerID, PlayerIntimacyData data)
    {
        PlayerIntimacyData[playerID] = data;
    }
}

[Serializable]
public class IntimateEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string TargetPlayerID;
    public string EventData;

    public IntimateEvent(string eventID, string eventType, string playerID, string targetPlayerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        TargetPlayerID = targetPlayerID;
        EventData = eventData;
    }
}

public class IntimateSystemDataManager
{
    private static IntimateSystemDataManager _instance;
    public static IntimateSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new IntimateSystemDataManager();
            }
            return _instance;
        }
    }

    public IntimateSystemData intimateData;
    private List<IntimateEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private IntimateSystemDataManager()
    {
        intimateData = new IntimateSystemData();
        recentEvents = new List<IntimateEvent>();
        LoadIntimateData();
    }

    public void SaveIntimateData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "IntimateSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, intimateData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存亲密关系系统数据失败: " + e.Message);
        }
    }

    public void LoadIntimateData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "IntimateSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    intimateData = (IntimateSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载亲密关系系统数据失败: " + e.Message);
            intimateData = new IntimateSystemData();
        }
    }

    public void CreateIntimateEvent(string eventType, string playerID, string targetPlayerID, string eventData)
    {
        string eventID = "intimate_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        IntimateEvent intimateEvent = new IntimateEvent(eventID, eventType, playerID, targetPlayerID, eventData);
        recentEvents.Add(intimateEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<IntimateEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
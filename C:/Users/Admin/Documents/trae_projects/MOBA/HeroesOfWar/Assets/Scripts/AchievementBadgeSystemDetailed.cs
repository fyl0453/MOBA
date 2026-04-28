using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Achievement
{
    public string AchievementID;
    public string AchievementName;
    public string Description;
    public string Category;
    public string Rarity;
    public int RequiredValue;
    public string ProgressType;
    public string RewardType;
    public int RewardAmount;
    public string IconURL;
    public string BadgeID;
    public bool IsRepeatable;
    public int MaxLevel;
    public DateTime AddedDate;

    public Achievement(string achievementName, string description, string category, string rarity, int requiredValue, string progressType, string rewardType, int rewardAmount, string iconURL, string badgeID, bool isRepeatable, int maxLevel)
    {
        AchievementID = "achievement_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AchievementName = achievementName;
        Description = description;
        Category = category;
        Rarity = rarity;
        RequiredValue = requiredValue;
        ProgressType = progressType;
        RewardType = rewardType;
        RewardAmount = rewardAmount;
        IconURL = iconURL;
        BadgeID = badgeID;
        IsRepeatable = isRepeatable;
        MaxLevel = maxLevel;
        AddedDate = DateTime.Now;
    }
}

[Serializable]
public class Badge
{
    public string BadgeID;
    public string BadgeName;
    public string Description;
    public string Rarity;
    public string IconURL;
    public string UnlockCondition;
    public int Level;
    public string AnimationEffect;
    public DateTime AddedDate;

    public Badge(string badgeName, string description, string rarity, string iconURL, string unlockCondition, int level, string animationEffect)
    {
        BadgeID = "badge_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BadgeName = badgeName;
        Description = description;
        Rarity = rarity;
        IconURL = iconURL;
        UnlockCondition = unlockCondition;
        Level = level;
        AnimationEffect = animationEffect;
        AddedDate = DateTime.Now;
    }
}

[Serializable]
public class PlayerAchievementProgress
{
    public string AchievementID;
    public int CurrentValue;
    public int CurrentLevel;
    public bool IsCompleted;
    public DateTime LastProgressTime;
    public DateTime CompletedTime;
    public int TotalCompletions;

    public PlayerAchievementProgress(string achievementID)
    {
        AchievementID = achievementID;
        CurrentValue = 0;
        CurrentLevel = 1;
        IsCompleted = false;
        LastProgressTime = DateTime.MinValue;
        CompletedTime = DateTime.MinValue;
        TotalCompletions = 0;
    }
}

[Serializable]
public class PlayerBadgeUnlock
{
    public string BadgeID;
    public DateTime UnlockTime;
    public string UnlockReason;
    public bool IsEquipped;
    public DateTime LastEquippedTime;

    public PlayerBadgeUnlock(string badgeID, string unlockReason)
    {
        BadgeID = badgeID;
        UnlockTime = DateTime.Now;
        UnlockReason = unlockReason;
        IsEquipped = false;
        LastEquippedTime = DateTime.MinValue;
    }
}

[Serializable]
public class PlayerAchievementData
{
    public string PlayerID;
    public Dictionary<string, PlayerAchievementProgress> AchievementProgress;
    public List<PlayerBadgeUnlock> UnlockedBadges;
    public int TotalAchievementsCompleted;
    public int TotalBadgesUnlocked;
    public int TotalPointsEarned;
    public DateTime LastAchievementTime;
    public DateTime LastBadgeUnlockTime;
    public string EquippedBadgeID;

    public PlayerAchievementData(string playerID)
    {
        PlayerID = playerID;
        AchievementProgress = new Dictionary<string, PlayerAchievementProgress>();
        UnlockedBadges = new List<PlayerBadgeUnlock>();
        TotalAchievementsCompleted = 0;
        TotalBadgesUnlocked = 0;
        TotalPointsEarned = 0;
        LastAchievementTime = DateTime.MinValue;
        LastBadgeUnlockTime = DateTime.MinValue;
        EquippedBadgeID = "";
    }
}

[Serializable]
public class AchievementBadgeSystemData
{
    public List<Achievement> AllAchievements;
    public List<Badge> AllBadges;
    public Dictionary<string, PlayerAchievementData> PlayerAchievementData;
    public List<string> AchievementCategories;
    public List<string> BadgeRarities;
    public int MaxBadgesPerPlayer;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public AchievementBadgeSystemData()
    {
        AllAchievements = new List<Achievement>();
        AllBadges = new List<Badge>();
        PlayerAchievementData = new Dictionary<string, PlayerAchievementData>();
        AchievementCategories = new List<string> { "combat", "social", "collection", "progression", "special" };
        BadgeRarities = new List<string> { "common", "uncommon", "rare", "epic", "legendary" };
        MaxBadgesPerPlayer = 50;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddAchievement(Achievement achievement)
    {
        AllAchievements.Add(achievement);
    }

    public void AddBadge(Badge badge)
    {
        AllBadges.Add(badge);
    }

    public void AddPlayerAchievementData(string playerID, PlayerAchievementData achievementData)
    {
        PlayerAchievementData[playerID] = achievementData;
    }
}

[Serializable]
public class AchievementBadgeEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string AchievementID;
    public string BadgeID;
    public string EventData;

    public AchievementBadgeEvent(string eventID, string eventType, string playerID, string achievementID, string badgeID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        AchievementID = achievementID;
        BadgeID = badgeID;
        EventData = eventData;
    }
}

public class AchievementBadgeSystemDataManager
{
    private static AchievementBadgeSystemDataManager _instance;
    public static AchievementBadgeSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AchievementBadgeSystemDataManager();
            }
            return _instance;
        }
    }

    public AchievementBadgeSystemData achievementData;
    private List<AchievementBadgeEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private AchievementBadgeSystemDataManager()
    {
        achievementData = new AchievementBadgeSystemData();
        recentEvents = new List<AchievementBadgeEvent>();
        LoadAchievementData();
    }

    public void SaveAchievementData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "AchievementBadgeSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, achievementData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存成就徽章系统数据失败: " + e.Message);
        }
    }

    public void LoadAchievementData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "AchievementBadgeSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    achievementData = (AchievementBadgeSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载成就徽章系统数据失败: " + e.Message);
            achievementData = new AchievementBadgeSystemData();
        }
    }

    public void CreateAchievementBadgeEvent(string eventType, string playerID, string achievementID, string badgeID, string eventData)
    {
        string eventID = "achievement_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AchievementBadgeEvent achievementEvent = new AchievementBadgeEvent(eventID, eventType, playerID, achievementID, badgeID, eventData);
        recentEvents.Add(achievementEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<AchievementBadgeEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
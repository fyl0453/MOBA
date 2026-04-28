using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Achievement
{
    public string AchievementID;
    public string AchievementName;
    public string Description;
    public int AchievementType;
    public int TargetValue;
    public int CurrentValue;
    public int RewardType;
    public int RewardValue;
    public bool IsUnlocked;
    public DateTime? UnlockTime;
    public string IconName;
    public int Rarity;
    public bool IsHidden;

    public Achievement(string achievementID, string achievementName, string description, int achievementType, int targetValue, int rewardType, int rewardValue)
    {
        AchievementID = achievementID;
        AchievementName = achievementName;
        Description = description;
        AchievementType = achievementType;
        TargetValue = targetValue;
        CurrentValue = 0;
        RewardType = rewardType;
        RewardValue = rewardValue;
        IsUnlocked = false;
        UnlockTime = null;
        IconName = "";
        Rarity = 1;
        IsHidden = false;
    }

    public void UpdateProgress(int value)
    {
        if (!IsUnlocked)
        {
            CurrentValue += value;
            if (CurrentValue >= TargetValue)
            {
                IsUnlocked = true;
                UnlockTime = DateTime.Now;
            }
        }
    }

    public double GetProgressPercentage()
    {
        if (TargetValue == 0) return 100.0;
        return Math.Min((double)CurrentValue / TargetValue * 100.0, 100.0);
    }
}

[Serializable]
public class Badge
{
    public string BadgeID;
    public string BadgeName;
    public string Description;
    public int BadgeType;
    public string IconName;
    public int Rarity;
    public bool IsLimited;
    public DateTime? LimitedEndTime;
    public int DisplayOrder;

    public Badge(string badgeID, string badgeName, string description, int badgeType, int rarity)
    {
        BadgeID = badgeID;
        BadgeName = badgeName;
        Description = description;
        BadgeType = badgeType;
        IconName = "";
        Rarity = rarity;
        IsLimited = false;
        LimitedEndTime = null;
        DisplayOrder = 0;
    }
}

[Serializable]
public class PlayerBadge
{
    public string PlayerID;
    public string BadgeID;
    public DateTime ObtainTime;
    public bool IsEquipped;
    public string ObtainMethod;

    public PlayerBadge(string playerID, string badgeID, string obtainMethod)
    {
        PlayerID = playerID;
        BadgeID = badgeID;
        ObtainTime = DateTime.Now;
        IsEquipped = false;
        ObtainMethod = obtainMethod;
    }
}

[Serializable]
public class AchievementCategory
{
    public int CategoryID;
    public string CategoryName;
    public string Description;
    public int DisplayOrder;
    public List<string> AchievementIDs;

    public AchievementCategory(int categoryID, string categoryName, string description, int displayOrder)
    {
        CategoryID = categoryID;
        CategoryName = categoryName;
        Description = description;
        DisplayOrder = displayOrder;
        AchievementIDs = new List<string>();
    }
}

[Serializable]
public class BadgeSystemData
{
    public List<Achievement> AllAchievements;
    public List<Badge> AllBadges;
    public Dictionary<string, List<PlayerBadge>> PlayerBadges;
    public Dictionary<string, List<Achievement>> PlayerAchievements;
    public List<AchievementCategory> Categories;
    public Dictionary<string, List<string>> PlayerEquippedBadges;
    public List<string> FeaturedBadgeIDs;
    public DateTime LastCleanupTime;

    public BadgeSystemData()
    {
        AllAchievements = new List<Achievement>();
        AllBadges = new List<Badge>();
        PlayerBadges = new Dictionary<string, List<PlayerBadge>>();
        PlayerAchievements = new Dictionary<string, List<Achievement>>();
        Categories = new List<AchievementCategory>();
        PlayerEquippedBadges = new Dictionary<string, List<string>>();
        FeaturedBadgeIDs = new List<string>();
        LastCleanupTime = DateTime.Now;
        InitializeDefaultData();
    }

    private void InitializeDefaultData()
    {
        Categories.Add(new AchievementCategory(1, "战斗成就", "战斗相关成就", 1));
        Categories.Add(new AchievementCategory(2, "社交成就", "社交相关成就", 2));
        Categories.Add(new AchievementCategory(3, "收集成就", "收集相关成就", 3));
        Categories.Add(new AchievementCategory(4, "时间成就", "时间相关成就", 4));
        Categories.Add(new AchievementCategory(5, "特殊成就", "特殊成就", 5));

        Achievement firstBlood = new Achievement("ach_001", "首血", "获得第一次击杀", 1, 1, 1, 100);
        firstBlood.IconName = "achievement_first_blood";
        AllAchievements.Add(firstBlood);

        Achievement win10 = new Achievement("ach_002", "十连胜", "获得10连胜", 1, 10, 1, 500);
        win10.IconName = "achievement_win_streak";
        AllAchievements.Add(win10);

        Achievement match100 = new Achievement("ach_003", "百场战斗", "完成100场比赛", 4, 100, 1, 1000);
        match100.IconName = "achievement_matches";
        AllAchievements.Add(match100);

        Achievement firstFriend = new Achievement("ach_004", "社交达人", "添加第一个好友", 2, 1, 1, 50);
        firstFriend.IconName = "achievement_friend";
        AllAchievements.Add(firstFriend);

        Badge rookie = new Badge("badge_001", "新手", "新手徽章", 1, 1);
        rookie.IconName = "badge_rookie";
        rookie.DisplayOrder = 1;
        AllBadges.Add(rookie);

        Badge veteran = new Badge("badge_002", "老兵", "老兵徽章", 1, 2);
        veteran.IconName = "badge_veteran";
        veteran.DisplayOrder = 2;
        AllBadges.Add(veteran);

        Badge champion = new Badge("badge_003", "冠军", "冠军徽章", 2, 3);
        champion.IconName = "badge_champion";
        champion.DisplayOrder = 3;
        AllBadges.Add(champion);
    }

    public void AddAchievement(string playerID, Achievement achievement)
    {
        if (!PlayerAchievements.ContainsKey(playerID))
        {
            PlayerAchievements[playerID] = new List<Achievement>();
        }
        PlayerAchievements[playerID].Add(achievement);
    }

    public void AddPlayerBadge(string playerID, PlayerBadge playerBadge)
    {
        if (!PlayerBadges.ContainsKey(playerID))
        {
            PlayerBadges[playerID] = new List<PlayerBadge>();
        }
        PlayerBadges[playerID].Add(playerBadge);
    }

    public void AddBadge(Badge badge)
    {
        AllBadges.Add(badge);
    }

    public void AddCategory(AchievementCategory category)
    {
        Categories.Add(category);
    }
}

[Serializable]
public class BadgeEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string BadgeID;
    public string EventData;

    public BadgeEvent(string eventID, string eventType, string playerID, string badgeID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        BadgeID = badgeID;
        EventData = eventData;
    }
}

public class BadgeSystemDataManager
{
    private static BadgeSystemDataManager _instance;
    public static BadgeSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BadgeSystemDataManager();
            }
            return _instance;
        }
    }

    public BadgeSystemData badgeData;
    private List<BadgeEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private BadgeSystemDataManager()
    {
        badgeData = new BadgeSystemData();
        recentEvents = new List<BadgeEvent>();
        LoadBadgeData();
    }

    public void SaveBadgeData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "BadgeSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, badgeData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存徽章系统数据失败: " + e.Message);
        }
    }

    public void LoadBadgeData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "BadgeSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    badgeData = (BadgeSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载徽章系统数据失败: " + e.Message);
            badgeData = new BadgeSystemData();
        }
    }

    public void CreateBadgeEvent(string eventType, string playerID, string badgeID, string eventData)
    {
        string eventID = "badge_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BadgeEvent badgeEvent = new BadgeEvent(eventID, eventType, playerID, badgeID, eventData);
        recentEvents.Add(badgeEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<BadgeEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
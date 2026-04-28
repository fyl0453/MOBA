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
    public string Category;
    public int Difficulty;
    public int TargetValue;
    public string IconName;
    public string UnlockMessage;
    public List<AchievementReward> Rewards;
    public bool IsSecret;
    public string SecretDescription;
    public DateTime CreateTime;

    public Achievement(string achievementID, string achievementName, string description, string category, int difficulty, int targetValue)
    {
        AchievementID = achievementID;
        AchievementName = achievementName;
        Description = description;
        Category = category;
        Difficulty = difficulty;
        TargetValue = targetValue;
        IconName = "";
        UnlockMessage = "";
        Rewards = new List<AchievementReward>();
        IsSecret = false;
        SecretDescription = "";
        CreateTime = DateTime.Now;
    }
}

[Serializable]
public class AchievementReward
{
    public string RewardID;
    public string RewardName;
    public string RewardType;
    public int RewardValue;
    public string IconName;

    public AchievementReward(string rewardID, string rewardName, string rewardType, int rewardValue)
    {
        RewardID = rewardID;
        RewardName = rewardName;
        RewardType = rewardType;
        RewardValue = rewardValue;
        IconName = "";
    }
}

[Serializable]
public class PlayerAchievementProgress
{
    public string PlayerID;
    public string AchievementID;
    public int CurrentValue;
    public bool IsCompleted;
    public bool IsClaimed;
    public DateTime UnlockTime;
    public DateTime LastUpdateTime;

    public PlayerAchievementProgress(string playerID, string achievementID)
    {
        PlayerID = playerID;
        AchievementID = achievementID;
        CurrentValue = 0;
        IsCompleted = false;
        IsClaimed = false;
        UnlockTime = DateTime.MinValue;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class AchievementCategory
{
    public string CategoryID;
    public string CategoryName;
    public string Description;
    public string IconName;
    public int SortOrder;
    public bool IsVisible;

    public AchievementCategory(string categoryID, string categoryName, int sortOrder)
    {
        CategoryID = categoryID;
        CategoryName = categoryName;
        Description = "";
        IconName = "";
        SortOrder = sortOrder;
        IsVisible = true;
    }
}

[Serializable]
public class AchievementSystemData
{
    public List<Achievement> AllAchievements;
    public List<AchievementReward> AllRewards;
    public List<AchievementCategory> AchievementCategories;
    public Dictionary<string, List<PlayerAchievementProgress>> PlayerProgress;
    public Dictionary<string, int> PlayerTotalAchievements;
    public DateTime LastUpdateTime;

    public AchievementSystemData()
    {
        AllAchievements = new List<Achievement>();
        AllRewards = new List<AchievementReward>();
        AchievementCategories = new List<AchievementCategory>();
        PlayerProgress = new Dictionary<string, List<PlayerAchievementProgress>>();
        PlayerTotalAchievements = new Dictionary<string, int>();
        LastUpdateTime = DateTime.Now;
        InitializeDefaultCategories();
        InitializeDefaultAchievements();
    }

    private void InitializeDefaultCategories()
    {
        AchievementCategory combatCategory = new AchievementCategory("category_combat", "战斗", 1);
        AchievementCategories.Add(combatCategory);

        AchievementCategory socialCategory = new AchievementCategory("category_social", "社交", 2);
        AchievementCategories.Add(socialCategory);

        AchievementCategory progressionCategory = new AchievementCategory("category_progression", "成长", 3);
        AchievementCategories.Add(progressionCategory);

        AchievementCategory collectionCategory = new AchievementCategory("category_collection", "收集", 4);
        AchievementCategories.Add(collectionCategory);

        AchievementCategory specialCategory = new AchievementCategory("category_special", "特殊", 5);
        AchievementCategories.Add(specialCategory);
    }

    private void InitializeDefaultAchievements()
    {
        Achievement firstWin = new Achievement("achievement_first_win", "首战告捷", "获得第一场胜利", "category_combat", 1, 1);
        AchievementReward firstWinReward = new AchievementReward("reward_first_win", "首胜奖励", "currency", 100);
        firstWin.Rewards.Add(firstWinReward);
        AllAchievements.Add(firstWin);
        AllRewards.Add(firstWinReward);

        Achievement killStreak = new Achievement("achievement_kill_streak", "连杀达人", "单场比赛获得5连杀", "category_combat", 2, 5);
        AchievementReward killStreakReward = new AchievementReward("reward_kill_streak", "连杀奖励", "title", 1);
        killStreak.Rewards.Add(killStreakReward);
        AllAchievements.Add(killStreak);
        AllRewards.Add(killStreakReward);

        Achievement levelUp = new Achievement("achievement_level_up", "等级提升", "提升到10级", "category_progression", 1, 10);
        AchievementReward levelUpReward = new AchievementReward("reward_level_up", "等级奖励", "currency", 200);
        levelUp.Rewards.Add(levelUpReward);
        AllAchievements.Add(levelUp);
        AllRewards.Add(levelUpReward);

        Achievement friend = new Achievement("achievement_friend", "友谊万岁", "添加5个好友", "category_social", 1, 5);
        AchievementReward friendReward = new AchievementReward("reward_friend", "好友奖励", "item", 1);
        friend.Rewards.Add(friendReward);
        AllAchievements.Add(friend);
        AllRewards.Add(friendReward);
    }

    public void AddAchievement(Achievement achievement)
    {
        AllAchievements.Add(achievement);
    }

    public void AddReward(AchievementReward reward)
    {
        AllRewards.Add(reward);
    }

    public void AddCategory(AchievementCategory category)
    {
        AchievementCategories.Add(category);
    }

    public void AddPlayerProgress(string playerID, PlayerAchievementProgress progress)
    {
        if (!PlayerProgress.ContainsKey(playerID))
        {
            PlayerProgress[playerID] = new List<PlayerAchievementProgress>();
        }
        PlayerProgress[playerID].Add(progress);
    }

    public void UpdatePlayerTotalAchievements(string playerID, int count)
    {
        PlayerTotalAchievements[playerID] = count;
    }
}

[Serializable]
public class AchievementEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string AchievementID;
    public string EventData;

    public AchievementEvent(string eventID, string eventType, string playerID, string achievementID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        AchievementID = achievementID;
        EventData = eventData;
    }
}

public class AchievementSystemDataManager
{
    private static AchievementSystemDataManager _instance;
    public static AchievementSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AchievementSystemDataManager();
            }
            return _instance;
        }
    }

    public AchievementSystemData achievementData;
    private List<AchievementEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private AchievementSystemDataManager()
    {
        achievementData = new AchievementSystemData();
        recentEvents = new List<AchievementEvent>();
        LoadAchievementData();
    }

    public void SaveAchievementData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "AchievementSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, achievementData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存成就系统数据失败: " + e.Message);
        }
    }

    public void LoadAchievementData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "AchievementSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    achievementData = (AchievementSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载成就系统数据失败: " + e.Message);
            achievementData = new AchievementSystemData();
        }
    }

    public void CreateAchievementEvent(string eventType, string playerID, string achievementID, string eventData)
    {
        string eventID = "achievement_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AchievementEvent achievementEvent = new AchievementEvent(eventID, eventType, playerID, achievementID, eventData);
        recentEvents.Add(achievementEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<AchievementEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using System;
using System.Collections.Generic;

public class BadgeSystemDetailedManager
{
    private static BadgeSystemDetailedManager _instance;
    public static BadgeSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BadgeSystemDetailedManager();
            }
            return _instance;
        }
    }

    private BadgeSystemData badgeData;
    private BadgeSystemDataManager dataManager;

    private BadgeSystemDetailedManager()
    {
        dataManager = BadgeSystemDataManager.Instance;
        badgeData = dataManager.badgeData;
    }

    public void CreateAchievement(string achievementName, string description, int achievementType, int targetValue, int rewardType, int rewardValue, string iconName = "", int rarity = 1, bool isHidden = false)
    {
        string achievementID = "ach_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Achievement achievement = new Achievement(achievementID, achievementName, description, achievementType, targetValue, rewardType, rewardValue);
        achievement.IconName = iconName;
        achievement.Rarity = rarity;
        achievement.IsHidden = isHidden;
        badgeData.AllAchievements.Add(achievement);
        dataManager.CreateBadgeEvent("achievement_create", "", achievementID, "创建成就: " + achievementName);
        dataManager.SaveBadgeData();
        Debug.Log("创建成就成功: " + achievementName);
    }

    public void CreateBadge(string badgeName, string description, int badgeType, int rarity, bool isLimited = false)
    {
        string badgeID = "badge_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Badge badge = new Badge(badgeID, badgeName, description, badgeType, rarity);
        badge.IsLimited = isLimited;
        badgeData.AddBadge(badge);
        dataManager.CreateBadgeEvent("badge_create", "", badgeID, "创建徽章: " + badgeName);
        dataManager.SaveBadgeData();
        Debug.Log("创建徽章成功: " + badgeName);
    }

    public void InitializePlayerAchievements(string playerID)
    {
        if (!badgeData.PlayerAchievements.ContainsKey(playerID))
        {
            badgeData.PlayerAchievements[playerID] = new List<Achievement>();
            foreach (Achievement achievement in badgeData.AllAchievements)
            {
                Achievement playerAchievement = new Achievement(achievement.AchievementID, achievement.AchievementName, achievement.Description, achievement.AchievementType, achievement.TargetValue, achievement.RewardType, achievement.RewardValue);
                playerAchievement.IconName = achievement.IconName;
                playerAchievement.Rarity = achievement.Rarity;
                playerAchievement.IsHidden = achievement.IsHidden;
                badgeData.AddAchievement(playerID, playerAchievement);
            }
            dataManager.CreateBadgeEvent("achievement_init", playerID, "", "初始化玩家成就");
            dataManager.SaveBadgeData();
            Debug.Log("初始化玩家成就: " + playerID);
        }
    }

    public void UpdateAchievementProgress(string playerID, int achievementType, int value)
    {
        if (!badgeData.PlayerAchievements.ContainsKey(playerID))
        {
            InitializePlayerAchievements(playerID);
        }

        List<Achievement> achievements = badgeData.PlayerAchievements[playerID].FindAll(a => a.AchievementType == achievementType && !a.IsUnlocked);
        foreach (Achievement achievement in achievements)
        {
            achievement.UpdateProgress(value);
            if (achievement.IsUnlocked)
            {
                dataManager.CreateBadgeEvent("achievement_unlock", playerID, achievement.AchievementID, "解锁成就: " + achievement.AchievementName);
                Debug.Log("解锁成就: " + achievement.AchievementName);
            }
        }
        dataManager.SaveBadgeData();
    }

    public void UnlockAchievement(string playerID, string achievementID)
    {
        if (!badgeData.PlayerAchievements.ContainsKey(playerID))
        {
            InitializePlayerAchievements(playerID);
        }

        Achievement achievement = badgeData.PlayerAchievements[playerID].Find(a => a.AchievementID == achievementID);
        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            achievement.UnlockTime = DateTime.Now;
            dataManager.CreateBadgeEvent("achievement_unlock", playerID, achievementID, "解锁成就: " + achievement.AchievementName);
            dataManager.SaveBadgeData();
            Debug.Log("解锁成就: " + achievement.AchievementName);
        }
    }

    public List<Achievement> GetPlayerAchievements(string playerID)
    {
        if (badgeData.PlayerAchievements.ContainsKey(playerID))
        {
            return badgeData.PlayerAchievements[playerID];
        }
        return new List<Achievement>();
    }

    public List<Achievement> GetUnlockedAchievements(string playerID)
    {
        if (badgeData.PlayerAchievements.ContainsKey(playerID))
        {
            return badgeData.PlayerAchievements[playerID].FindAll(a => a.IsUnlocked);
        }
        return new List<Achievement>();
    }

    public List<Achievement> GetLockedAchievements(string playerID)
    {
        if (badgeData.PlayerAchievements.ContainsKey(playerID))
        {
            return badgeData.PlayerAchievements[playerID].FindAll(a => !a.IsUnlocked);
        }
        return new List<Achievement>();
    }

    public List<Achievement> GetAchievementsByCategory(string playerID, int categoryID)
    {
        if (badgeData.PlayerAchievements.ContainsKey(playerID))
        {
            return badgeData.PlayerAchievements[playerID].FindAll(a => a.AchievementType == categoryID);
        }
        return new List<Achievement>();
    }

    public Achievement GetAchievement(string playerID, string achievementID)
    {
        if (badgeData.PlayerAchievements.ContainsKey(playerID))
        {
            return badgeData.PlayerAchievements[playerID].Find(a => a.AchievementID == achievementID);
        }
        return null;
    }

    public void EquipBadge(string playerID, string badgeID)
    {
        if (!HasBadge(playerID, badgeID))
        {
            Debug.LogWarning("徽章未获得");
            return;
        }

        if (!badgeData.PlayerEquippedBadges.ContainsKey(playerID))
        {
            badgeData.PlayerEquippedBadges[playerID] = new List<string>();
        }

        if (badgeData.PlayerEquippedBadges[playerID].Contains(badgeID))
        {
            Debug.LogWarning("徽章已装备");
            return;
        }

        if (badgeData.PlayerEquippedBadges[playerID].Count >= 3)
        {
            Debug.LogWarning("最多装备3个徽章");
            return;
        }

        badgeData.PlayerEquippedBadges[playerID].Add(badgeID);
        if (badgeData.PlayerBadges.ContainsKey(playerID))
        {
            PlayerBadge playerBadge = badgeData.PlayerBadges[playerID].Find(b => b.BadgeID == badgeID);
            if (playerBadge != null)
            {
                playerBadge.IsEquipped = true;
            }
        }
        dataManager.CreateBadgeEvent("badge_equip", playerID, badgeID, "装备徽章");
        dataManager.SaveBadgeData();
        Debug.Log("装备徽章成功");
    }

    public void UnequipBadge(string playerID, string badgeID)
    {
        if (badgeData.PlayerEquippedBadges.ContainsKey(playerID))
        {
            badgeData.PlayerEquippedBadges[playerID].Remove(badgeID);
            if (badgeData.PlayerBadges.ContainsKey(playerID))
            {
                PlayerBadge playerBadge = badgeData.PlayerBadges[playerID].Find(b => b.BadgeID == badgeID);
                if (playerBadge != null)
                {
                    playerBadge.IsEquipped = false;
                }
            }
            dataManager.SaveBadgeData();
            Debug.Log("卸下徽章成功");
        }
    }

    public List<string> GetEquippedBadges(string playerID)
    {
        if (badgeData.PlayerEquippedBadges.ContainsKey(playerID))
        {
            return badgeData.PlayerEquippedBadges[playerID];
        }
        return new List<string>();
    }

    public void ObtainBadge(string playerID, string badgeID, string obtainMethod)
    {
        if (HasBadge(playerID, badgeID))
        {
            Debug.LogWarning("徽章已获得");
            return;
        }

        PlayerBadge playerBadge = new PlayerBadge(playerID, badgeID, obtainMethod);
        badgeData.AddPlayerBadge(playerID, playerBadge);
        dataManager.CreateBadgeEvent("badge_obtain", playerID, badgeID, "获得徽章: " + obtainMethod);
        dataManager.SaveBadgeData();
        Debug.Log("获得徽章成功");
    }

    public bool HasBadge(string playerID, string badgeID)
    {
        if (badgeData.PlayerBadges.ContainsKey(playerID))
        {
            return badgeData.PlayerBadges[playerID].Exists(b => b.BadgeID == badgeID);
        }
        return false;
    }

    public List<PlayerBadge> GetPlayerBadges(string playerID)
    {
        if (badgeData.PlayerBadges.ContainsKey(playerID))
        {
            return badgeData.PlayerBadges[playerID];
        }
        return new List<PlayerBadge>();
    }

    public Badge GetBadge(string badgeID)
    {
        return badgeData.AllBadges.Find(b => b.BadgeID == badgeID);
    }

    public List<Badge> GetAllBadges()
    {
        return badgeData.AllBadges;
    }

    public List<Badge> GetBadgesByType(int badgeType)
    {
        return badgeData.AllBadges.FindAll(b => b.BadgeType == badgeType);
    }

    public List<Badge> GetBadgesByRarity(int rarity)
    {
        return badgeData.AllBadges.FindAll(b => b.Rarity == rarity);
    }

    public List<Badge> GetLimitedBadges()
    {
        return badgeData.AllBadges.FindAll(b => b.IsLimited);
    }

    public List<AchievementCategory> GetAllCategories()
    {
        return badgeData.Categories;
    }

    public List<Achievement> GetAllAchievements()
    {
        return badgeData.AllAchievements;
    }

    public int GetUnlockedAchievementCount(string playerID)
    {
        return GetUnlockedAchievements(playerID).Count;
    }

    public int GetTotalBadgeCount(string playerID)
    {
        return GetPlayerBadges(playerID).Count;
    }

    public double GetAchievementCompletionRate(string playerID)
    {
        if (badgeData.PlayerAchievements.ContainsKey(playerID))
        {
            int total = badgeData.PlayerAchievements[playerID].Count;
            int unlocked = GetUnlockedAchievements(playerID).Count;
            if (total > 0)
            {
                return (double)unlocked / total * 100.0;
            }
        }
        return 0.0;
    }

    public void GrantBadgeReward(string playerID, Achievement achievement)
    {
        dataManager.CreateBadgeEvent("badge_reward", playerID, "", "发放成就奖励: " + achievement.AchievementName);
        dataManager.SaveBadgeData();
        Debug.Log("发放成就奖励: " + achievement.AchievementName + " 奖励类型: " + achievement.RewardType + " 奖励值: " + achievement.RewardValue);
    }

    public List<BadgeEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveBadgeData();
    }

    public void LoadData()
    {
        dataManager.LoadBadgeData();
    }
}
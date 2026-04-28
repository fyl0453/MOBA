using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementBadgeSystemDetailedManager
{
    private static AchievementBadgeSystemDetailedManager _instance;
    public static AchievementBadgeSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AchievementBadgeSystemDetailedManager();
            }
            return _instance;
        }
    }

    private AchievementBadgeSystemData achievementData;
    private AchievementBadgeSystemDataManager dataManager;

    private AchievementBadgeSystemDetailedManager()
    {
        dataManager = AchievementBadgeSystemDataManager.Instance;
        achievementData = dataManager.achievementData;
    }

    public void InitializePlayerAchievementData(string playerID)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            PlayerAchievementData playerAchievementData = new PlayerAchievementData(playerID);
            achievementData.AddPlayerAchievementData(playerID, playerAchievementData);
            dataManager.SaveAchievementData();
            Debug.Log("初始化成就徽章数据成功");
        }
    }

    public string CreateAchievement(string achievementName, string description, string category, string rarity, int requiredValue, string progressType, string rewardType, int rewardAmount, string iconURL, string badgeID, bool isRepeatable, int maxLevel)
    {
        Achievement achievement = new Achievement(achievementName, description, category, rarity, requiredValue, progressType, rewardType, rewardAmount, iconURL, badgeID, isRepeatable, maxLevel);
        achievementData.AddAchievement(achievement);
        
        dataManager.CreateAchievementBadgeEvent("achievement_create", "system", achievement.AchievementID, "", "创建成就: " + achievementName);
        dataManager.SaveAchievementData();
        Debug.Log("创建成就成功: " + achievementName);
        return achievement.AchievementID;
    }

    public string CreateBadge(string badgeName, string description, string rarity, string iconURL, string unlockCondition, int level, string animationEffect)
    {
        Badge badge = new Badge(badgeName, description, rarity, iconURL, unlockCondition, level, animationEffect);
        achievementData.AddBadge(badge);
        
        dataManager.CreateAchievementBadgeEvent("badge_create", "system", "", badge.BadgeID, "创建徽章: " + badgeName);
        dataManager.SaveAchievementData();
        Debug.Log("创建徽章成功: " + badgeName);
        return badge.BadgeID;
    }

    public void UpdateAchievementProgress(string playerID, string achievementID, int progressAmount)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            InitializePlayerAchievementData(playerID);
        }
        
        Achievement achievement = achievementData.AllAchievements.Find(a => a.AchievementID == achievementID);
        if (achievement == null)
        {
            Debug.LogError("成就不存在: " + achievementID);
            return;
        }
        
        PlayerAchievementData playerAchievementData = achievementData.PlayerAchievementData[playerID];
        if (!playerAchievementData.AchievementProgress.ContainsKey(achievementID))
        {
            playerAchievementData.AchievementProgress[achievementID] = new PlayerAchievementProgress(achievementID);
        }
        
        PlayerAchievementProgress progress = playerAchievementData.AchievementProgress[achievementID];
        if (progress.IsCompleted && !achievement.IsRepeatable)
        {
            return;
        }
        
        progress.CurrentValue += progressAmount;
        progress.LastProgressTime = DateTime.Now;
        
        if (progress.CurrentValue >= achievement.RequiredValue && !progress.IsCompleted)
        {
            CompleteAchievement(playerID, achievementID);
        }
        else if (achievement.IsRepeatable && progress.CurrentValue >= achievement.RequiredValue * progress.CurrentLevel)
        {
            progress.CurrentLevel++;
            progress.TotalCompletions++;
            playerAchievementData.TotalAchievementsCompleted++;
            playerAchievementData.TotalPointsEarned += achievement.RewardAmount;
            playerAchievementData.LastAchievementTime = DateTime.Now;
            
            dataManager.CreateAchievementBadgeEvent("achievement_level_up", playerID, achievementID, "", "成就升级: " + achievement.AchievementName + " 等级 " + progress.CurrentLevel);
            dataManager.SaveAchievementData();
            Debug.Log("成就升级成功: " + achievement.AchievementName + " 等级 " + progress.CurrentLevel);
        }
        
        dataManager.SaveAchievementData();
    }

    private void CompleteAchievement(string playerID, string achievementID)
    {
        Achievement achievement = achievementData.AllAchievements.Find(a => a.AchievementID == achievementID);
        if (achievement == null)
        {
            return;
        }
        
        PlayerAchievementData playerAchievementData = achievementData.PlayerAchievementData[playerID];
        PlayerAchievementProgress progress = playerAchievementData.AchievementProgress[achievementID];
        
        progress.IsCompleted = true;
        progress.CompletedTime = DateTime.Now;
        progress.TotalCompletions++;
        playerAchievementData.TotalAchievementsCompleted++;
        playerAchievementData.TotalPointsEarned += achievement.RewardAmount;
        playerAchievementData.LastAchievementTime = DateTime.Now;
        
        if (!string.IsNullOrEmpty(achievement.BadgeID))
        {
            UnlockBadge(playerID, achievement.BadgeID, "完成成就: " + achievement.AchievementName);
        }
        
        dataManager.CreateAchievementBadgeEvent("achievement_complete", playerID, achievementID, achievement.BadgeID, "完成成就: " + achievement.AchievementName);
        dataManager.SaveAchievementData();
        Debug.Log("完成成就成功: " + achievement.AchievementName);
    }

    public void UnlockBadge(string playerID, string badgeID, string unlockReason)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            InitializePlayerAchievementData(playerID);
        }
        
        Badge badge = achievementData.AllBadges.Find(b => b.BadgeID == badgeID);
        if (badge == null)
        {
            Debug.LogError("徽章不存在: " + badgeID);
            return;
        }
        
        PlayerAchievementData playerAchievementData = achievementData.PlayerAchievementData[playerID];
        if (playerAchievementData.UnlockedBadges.Exists(b => b.BadgeID == badgeID))
        {
            return;
        }
        
        if (playerAchievementData.UnlockedBadges.Count >= achievementData.MaxBadgesPerPlayer)
        {
            Debug.LogError("徽章数量达到上限");
            return;
        }
        
        PlayerBadgeUnlock badgeUnlock = new PlayerBadgeUnlock(badgeID, unlockReason);
        playerAchievementData.UnlockedBadges.Add(badgeUnlock);
        playerAchievementData.TotalBadgesUnlocked++;
        playerAchievementData.LastBadgeUnlockTime = DateTime.Now;
        
        dataManager.CreateAchievementBadgeEvent("badge_unlock", playerID, "", badgeID, "解锁徽章: " + badge.BadgeName);
        dataManager.SaveAchievementData();
        Debug.Log("解锁徽章成功: " + badge.BadgeName);
    }

    public void EquipBadge(string playerID, string badgeID)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerAchievementData playerAchievementData = achievementData.PlayerAchievementData[playerID];
        if (!playerAchievementData.UnlockedBadges.Exists(b => b.BadgeID == badgeID))
        {
            Debug.LogError("徽章未解锁: " + badgeID);
            return;
        }
        
        PlayerBadgeUnlock badgeUnlock = playerAchievementData.UnlockedBadges.Find(b => b.BadgeID == badgeID);
        if (badgeUnlock != null)
        {
            foreach (PlayerBadgeUnlock unlock in playerAchievementData.UnlockedBadges)
            {
                unlock.IsEquipped = false;
            }
            
            badgeUnlock.IsEquipped = true;
            badgeUnlock.LastEquippedTime = DateTime.Now;
            playerAchievementData.EquippedBadgeID = badgeID;
            
            dataManager.CreateAchievementBadgeEvent("badge_equip", playerID, "", badgeID, "装备徽章: " + badgeID);
            dataManager.SaveAchievementData();
            Debug.Log("装备徽章成功: " + badgeID);
        }
    }

    public void UnequipBadge(string playerID)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerAchievementData playerAchievementData = achievementData.PlayerAchievementData[playerID];
        if (!string.IsNullOrEmpty(playerAchievementData.EquippedBadgeID))
        {
            PlayerBadgeUnlock badgeUnlock = playerAchievementData.UnlockedBadges.Find(b => b.BadgeID == playerAchievementData.EquippedBadgeID);
            if (badgeUnlock != null)
            {
                badgeUnlock.IsEquipped = false;
            }
            playerAchievementData.EquippedBadgeID = "";
            
            dataManager.CreateAchievementBadgeEvent("badge_unequip", playerID, "", "", "卸下徽章");
            dataManager.SaveAchievementData();
            Debug.Log("卸下徽章成功");
        }
    }

    public List<Achievement> GetAllAchievements()
    {
        return achievementData.AllAchievements;
    }

    public List<Achievement> GetAchievementsByCategory(string category)
    {
        return achievementData.AllAchievements.FindAll(a => a.Category == category);
    }

    public List<Badge> GetAllBadges()
    {
        return achievementData.AllBadges;
    }

    public List<Badge> GetBadgesByRarity(string rarity)
    {
        return achievementData.AllBadges.FindAll(b => b.Rarity == rarity);
    }

    public PlayerAchievementData GetPlayerAchievementData(string playerID)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            InitializePlayerAchievementData(playerID);
        }
        return achievementData.PlayerAchievementData[playerID];
    }

    public List<PlayerAchievementProgress> GetPlayerAchievementProgress(string playerID)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            InitializePlayerAchievementData(playerID);
        }
        return new List<PlayerAchievementProgress>(achievementData.PlayerAchievementData[playerID].AchievementProgress.Values);
    }

    public List<PlayerBadgeUnlock> GetPlayerUnlockedBadges(string playerID)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            InitializePlayerAchievementData(playerID);
        }
        return achievementData.PlayerAchievementData[playerID].UnlockedBadges;
    }

    public Achievement GetAchievement(string achievementID)
    {
        return achievementData.AllAchievements.Find(a => a.AchievementID == achievementID);
    }

    public Badge GetBadge(string badgeID)
    {
        return achievementData.AllBadges.Find(b => b.BadgeID == badgeID);
    }

    public string GetPlayerEquippedBadge(string playerID)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            return "";
        }
        return achievementData.PlayerAchievementData[playerID].EquippedBadgeID;
    }

    public int GetPlayerAchievementProgress(string playerID, string achievementID)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            return 0;
        }
        
        PlayerAchievementData playerAchievementData = achievementData.PlayerAchievementData[playerID];
        if (playerAchievementData.AchievementProgress.ContainsKey(achievementID))
        {
            return playerAchievementData.AchievementProgress[achievementID].CurrentValue;
        }
        return 0;
    }

    public bool IsAchievementCompleted(string playerID, string achievementID)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            return false;
        }
        
        PlayerAchievementData playerAchievementData = achievementData.PlayerAchievementData[playerID];
        if (playerAchievementData.AchievementProgress.ContainsKey(achievementID))
        {
            return playerAchievementData.AchievementProgress[achievementID].IsCompleted;
        }
        return false;
    }

    public bool IsBadgeUnlocked(string playerID, string badgeID)
    {
        if (!achievementData.PlayerAchievementData.ContainsKey(playerID))
        {
            return false;
        }
        return achievementData.PlayerAchievementData[playerID].UnlockedBadges.Exists(b => b.BadgeID == badgeID);
    }

    public List<string> GetAchievementCategories()
    {
        return achievementData.AchievementCategories;
    }

    public List<string> GetBadgeRarities()
    {
        return achievementData.BadgeRarities;
    }

    public void AddAchievementCategory(string category)
    {
        if (!achievementData.AchievementCategories.Contains(category))
        {
            achievementData.AchievementCategories.Add(category);
            dataManager.SaveAchievementData();
            Debug.Log("添加成就分类成功: " + category);
        }
    }

    public void RemoveAchievementCategory(string category)
    {
        if (achievementData.AchievementCategories.Contains(category))
        {
            achievementData.AchievementCategories.Remove(category);
            dataManager.SaveAchievementData();
            Debug.Log("删除成就分类成功: " + category);
        }
    }

    public void AddBadgeRarity(string rarity)
    {
        if (!achievementData.BadgeRarities.Contains(rarity))
        {
            achievementData.BadgeRarities.Add(rarity);
            dataManager.SaveAchievementData();
            Debug.Log("添加徽章稀有度成功: " + rarity);
        }
    }

    public void RemoveBadgeRarity(string rarity)
    {
        if (achievementData.BadgeRarities.Contains(rarity))
        {
            achievementData.BadgeRarities.Remove(rarity);
            dataManager.SaveAchievementData();
            Debug.Log("删除徽章稀有度成功: " + rarity);
        }
    }

    public void SaveData()
    {
        dataManager.SaveAchievementData();
    }

    public void LoadData()
    {
        dataManager.LoadAchievementData();
    }

    public List<AchievementBadgeEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
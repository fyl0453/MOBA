using System;
using System.Collections.Generic;

public class AchievementSystemDetailedManager
{
    private static AchievementSystemDetailedManager _instance;
    public static AchievementSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AchievementSystemDetailedManager();
            }
            return _instance;
        }
    }

    private AchievementSystemData achievementData;
    private AchievementSystemDataManager dataManager;

    private AchievementSystemDetailedManager()
    {
        dataManager = AchievementSystemDataManager.Instance;
        achievementData = dataManager.achievementData;
    }

    public void CreateAchievement(string achievementName, string description, string category, int difficulty, int targetValue, List<AchievementReward> rewards)
    {
        string achievementID = "achievement_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Achievement achievement = new Achievement(achievementID, achievementName, description, category, difficulty, targetValue);
        achievement.Rewards = rewards;
        achievementData.AddAchievement(achievement);
        foreach (AchievementReward reward in rewards)
        {
            achievementData.AddReward(reward);
        }
        dataManager.SaveAchievementData();
        Debug.Log("创建成就成功: " + achievementName);
    }

    public void UpdateAchievement(string achievementID, string achievementName, string description, string category, int difficulty, int targetValue)
    {
        Achievement achievement = achievementData.AllAchievements.Find(a => a.AchievementID == achievementID);
        if (achievement != null)
        {
            achievement.AchievementName = achievementName;
            achievement.Description = description;
            achievement.Category = category;
            achievement.Difficulty = difficulty;
            achievement.TargetValue = targetValue;
            dataManager.SaveAchievementData();
            Debug.Log("更新成就成功: " + achievementName);
        }
    }

    public void DeleteAchievement(string achievementID)
    {
        Achievement achievement = achievementData.AllAchievements.Find(a => a.AchievementID == achievementID);
        if (achievement != null)
        {
            achievementData.AllAchievements.Remove(achievement);
            dataManager.SaveAchievementData();
            Debug.Log("删除成就成功: " + achievement.AchievementName);
        }
    }

    public void AddRewardToAchievement(string achievementID, AchievementReward reward)
    {
        Achievement achievement = achievementData.AllAchievements.Find(a => a.AchievementID == achievementID);
        if (achievement != null)
        {
            achievement.Rewards.Add(reward);
            achievementData.AddReward(reward);
            dataManager.SaveAchievementData();
            Debug.Log("添加奖励到成就成功: " + reward.RewardName);
        }
    }

    public List<Achievement> GetAchievementsByCategory(string category)
    {
        return achievementData.AllAchievements.FindAll(a => a.Category == category);
    }

    public List<Achievement> GetAllAchievements()
    {
        return achievementData.AllAchievements;
    }

    public List<Achievement> GetSecretAchievements()
    {
        return achievementData.AllAchievements.FindAll(a => a.IsSecret);
    }

    public Achievement GetAchievement(string achievementID)
    {
        return achievementData.AllAchievements.Find(a => a.AchievementID == achievementID);
    }

    public void UpdateAchievementProgress(string playerID, string achievementID, int progressValue)
    {
        PlayerAchievementProgress progress = GetPlayerAchievementProgress(playerID, achievementID);
        if (progress == null)
        {
            progress = new PlayerAchievementProgress(playerID, achievementID);
            achievementData.AddPlayerProgress(playerID, progress);
        }

        progress.CurrentValue += progressValue;
        Achievement achievement = GetAchievement(achievementID);
        if (achievement != null && progress.CurrentValue >= achievement.TargetValue && !progress.IsCompleted)
        {
            progress.IsCompleted = true;
            progress.UnlockTime = DateTime.Now;
            progress.LastUpdateTime = DateTime.Now;
            UpdatePlayerTotalAchievements(playerID);
            dataManager.CreateAchievementEvent("achievement_unlock", playerID, achievementID, "解锁成就: " + achievement.AchievementName);
        }
        else
        {
            progress.LastUpdateTime = DateTime.Now;
        }
        dataManager.SaveAchievementData();
        Debug.Log("更新成就进度成功: " + achievementID);
    }

    public void ClaimAchievementReward(string playerID, string achievementID)
    {
        PlayerAchievementProgress progress = GetPlayerAchievementProgress(playerID, achievementID);
        if (progress != null && progress.IsCompleted && !progress.IsClaimed)
        {
            progress.IsClaimed = true;
            progress.LastUpdateTime = DateTime.Now;
            Achievement achievement = GetAchievement(achievementID);
            if (achievement != null)
            {
                dataManager.CreateAchievementEvent("achievement_reward_claim", playerID, achievementID, "领取成就奖励: " + achievement.AchievementName);
                Debug.Log("领取成就奖励成功: " + achievement.AchievementName);
            }
            dataManager.SaveAchievementData();
        }
    }

    public PlayerAchievementProgress GetPlayerAchievementProgress(string playerID, string achievementID)
    {
        if (achievementData.PlayerProgress.ContainsKey(playerID))
        {
            return achievementData.PlayerProgress[playerID].Find(p => p.AchievementID == achievementID);
        }
        return null;
    }

    public List<PlayerAchievementProgress> GetPlayerAchievements(string playerID)
    {
        if (achievementData.PlayerProgress.ContainsKey(playerID))
        {
            return achievementData.PlayerProgress[playerID];
        }
        return new List<PlayerAchievementProgress>();
    }

    public List<Achievement> GetPlayerCompletedAchievements(string playerID)
    {
        List<Achievement> completedAchievements = new List<Achievement>();
        List<PlayerAchievementProgress> playerProgress = GetPlayerAchievements(playerID);
        foreach (PlayerAchievementProgress progress in playerProgress)
        {
            if (progress.IsCompleted)
            {
                Achievement achievement = GetAchievement(progress.AchievementID);
                if (achievement != null)
                {
                    completedAchievements.Add(achievement);
                }
            }
        }
        return completedAchievements;
    }

    public List<Achievement> GetPlayerUnclaimedAchievements(string playerID)
    {
        List<Achievement> unclaimedAchievements = new List<Achievement>();
        List<PlayerAchievementProgress> playerProgress = GetPlayerAchievements(playerID);
        foreach (PlayerAchievementProgress progress in playerProgress)
        {
            if (progress.IsCompleted && !progress.IsClaimed)
            {
                Achievement achievement = GetAchievement(progress.AchievementID);
                if (achievement != null)
                {
                    unclaimedAchievements.Add(achievement);
                }
            }
        }
        return unclaimedAchievements;
    }

    public List<Achievement> GetPlayerInProgressAchievements(string playerID)
    {
        List<Achievement> inProgressAchievements = new List<Achievement>();
        List<PlayerAchievementProgress> playerProgress = GetPlayerAchievements(playerID);
        foreach (PlayerAchievementProgress progress in playerProgress)
        {
            if (!progress.IsCompleted && progress.CurrentValue > 0)
            {
                Achievement achievement = GetAchievement(progress.AchievementID);
                if (achievement != null)
                {
                    inProgressAchievements.Add(achievement);
                }
            }
        }
        return inProgressAchievements;
    }

    public int GetPlayerAchievementCount(string playerID)
    {
        if (achievementData.PlayerTotalAchievements.ContainsKey(playerID))
        {
            return achievementData.PlayerTotalAchievements[playerID];
        }
        return 0;
    }

    public float GetPlayerAchievementCompletionRate(string playerID)
    {
        int totalAchievements = achievementData.AllAchievements.Count;
        if (totalAchievements == 0)
        {
            return 0;
        }
        int completedCount = GetPlayerAchievementCount(playerID);
        return (float)completedCount / totalAchievements * 100;
    }

    public int GetAchievementProgress(string playerID, string achievementID)
    {
        PlayerAchievementProgress progress = GetPlayerAchievementProgress(playerID, achievementID);
        if (progress != null)
        {
            return progress.CurrentValue;
        }
        return 0;
    }

    public bool IsAchievementCompleted(string playerID, string achievementID)
    {
        PlayerAchievementProgress progress = GetPlayerAchievementProgress(playerID, achievementID);
        return progress != null && progress.IsCompleted;
    }

    public bool IsAchievementClaimed(string playerID, string achievementID)
    {
        PlayerAchievementProgress progress = GetPlayerAchievementProgress(playerID, achievementID);
        return progress != null && progress.IsClaimed;
    }

    public void ResetAchievementProgress(string playerID, string achievementID)
    {
        PlayerAchievementProgress progress = GetPlayerAchievementProgress(playerID, achievementID);
        if (progress != null)
        {
            progress.CurrentValue = 0;
            progress.IsCompleted = false;
            progress.IsClaimed = false;
            progress.UnlockTime = DateTime.MinValue;
            progress.LastUpdateTime = DateTime.Now;
            UpdatePlayerTotalAchievements(playerID);
            dataManager.SaveAchievementData();
            Debug.Log("重置成就进度: " + achievementID);
        }
    }

    private void UpdatePlayerTotalAchievements(string playerID)
    {
        List<PlayerAchievementProgress> playerProgress = GetPlayerAchievements(playerID);
        int completedCount = playerProgress.FindAll(p => p.IsCompleted).Count;
        achievementData.UpdatePlayerTotalAchievements(playerID, completedCount);
    }

    public void AddAchievementCategory(string categoryName, int sortOrder, string description = "")
    {
        string categoryID = "category_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AchievementCategory category = new AchievementCategory(categoryID, categoryName, sortOrder);
        category.Description = description;
        achievementData.AddCategory(category);
        dataManager.SaveAchievementData();
        Debug.Log("添加成就分类成功: " + categoryName);
    }

    public List<AchievementCategory> GetAchievementCategories()
    {
        List<AchievementCategory> categories = new List<AchievementCategory>(achievementData.AchievementCategories);
        categories.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));
        return categories;
    }

    public AchievementCategory GetAchievementCategory(string categoryID)
    {
        return achievementData.AchievementCategories.Find(c => c.CategoryID == categoryID);
    }

    public void CleanupAchievements()
    {
        
    }

    public void SaveData()
    {
        dataManager.SaveAchievementData();
    }

    public void LoadData()
    {
        dataManager.LoadAchievementData();
    }

    public List<AchievementEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
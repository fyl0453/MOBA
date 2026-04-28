[System.Serializable]
public class AchievementCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    public List<Achievement> achievements;
    
    public AchievementCategory(string id, string name, string desc)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = desc;
        achievements = new List<Achievement>();
    }
    
    public void AddAchievement(Achievement achievement)
    {
        achievements.Add(achievement);
    }
    
    public int GetCompletedCount()
    {
        return achievements.FindAll(a => a.isCompleted).Count;
    }
    
    public float GetCompletionRate()
    {
        if (achievements.Count == 0)
            return 0;
        return (float)GetCompletedCount() / achievements.Count * 100f;
    }
}

[System.Serializable]
public class AchievementReward
{
    public string rewardID;
    public string rewardType;
    public string rewardItemID;
    public int quantity;
    public string rewardDescription;
    
    public AchievementReward(string id, string type, string itemID, int qty, string desc)
    {
        rewardID = id;
        rewardType = type;
        rewardItemID = itemID;
        quantity = qty;
        rewardDescription = desc;
    }
}

[System.Serializable]
public class AchievementProgress
{
    public string playerID;
    public Dictionary<string, int> progressData;
    public Dictionary<string, bool> completionData;
    public Dictionary<string, bool> claimedData;
    
    public AchievementProgress(string player)
    {
        playerID = player;
        progressData = new Dictionary<string, int>();
        completionData = new Dictionary<string, bool>();
        claimedData = new Dictionary<string, bool>();
    }
    
    public void UpdateProgress(string achievementID, int progress)
    {
        if (progressData.ContainsKey(achievementID))
        {
            progressData[achievementID] = progress;
        }
        else
        {
            progressData.Add(achievementID, progress);
        }
    }
    
    public void MarkAsCompleted(string achievementID)
    {
        completionData[achievementID] = true;
    }
    
    public void MarkAsClaimed(string achievementID)
    {
        claimedData[achievementID] = true;
    }
    
    public int GetProgress(string achievementID)
    {
        return progressData.ContainsKey(achievementID) ? progressData[achievementID] : 0;
    }
    
    public bool IsCompleted(string achievementID)
    {
        return completionData.ContainsKey(achievementID) && completionData[achievementID];
    }
    
    public bool IsClaimed(string achievementID)
    {
        return claimedData.ContainsKey(achievementID) && claimedData[achievementID];
    }
}

[System.Serializable]
public class AchievementManagerExtendedData
{
    public List<AchievementCategory> categories;
    public List<AchievementReward> rewards;
    public List<AchievementProgress> progressData;
    public int totalAchievements;
    public int totalCompleted;
    
    public AchievementManagerExtendedData()
    {
        categories = new List<AchievementCategory>();
        rewards = new List<AchievementReward>();
        progressData = new List<AchievementProgress>();
        totalAchievements = 0;
        totalCompleted = 0;
    }
    
    public void AddCategory(AchievementCategory category)
    {
        categories.Add(category);
        totalAchievements += category.achievements.Count;
    }
    
    public void AddReward(AchievementReward reward)
    {
        rewards.Add(reward);
    }
    
    public void AddProgressData(AchievementProgress progress)
    {
        progressData.Add(progress);
    }
    
    public AchievementCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public AchievementProgress GetProgressData(string playerID)
    {
        return progressData.Find(p => p.playerID == playerID);
    }
    
    public Achievement GetAchievement(string achievementID)
    {
        foreach (AchievementCategory category in categories)
        {
            Achievement achievement = category.achievements.Find(a => a.achievementID == achievementID);
            if (achievement != null)
            {
                return achievement;
            }
        }
        return null;
    }
    
    public void UpdateCompletionCount()
    {
        totalCompleted = 0;
        foreach (AchievementCategory category in categories)
        {
            totalCompleted += category.GetCompletedCount();
        }
    }
}
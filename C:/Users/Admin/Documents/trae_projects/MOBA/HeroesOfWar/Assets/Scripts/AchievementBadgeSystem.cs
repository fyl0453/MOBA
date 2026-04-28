[System.Serializable]
public class AchievementBadgeSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AchievementBadge> badges;
    public List<BadgeCategory> categories;
    
    public AchievementBadgeSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        badges = new List<AchievementBadge>();
        categories = new List<BadgeCategory>();
    }
    
    public void AddBadge(AchievementBadge badge)
    {
        badges.Add(badge);
    }
    
    public void AddCategory(BadgeCategory category)
    {
        categories.Add(category);
    }
    
    public AchievementBadge GetBadge(string badgeID)
    {
        return badges.Find(b => b.badgeID == badgeID);
    }
    
    public BadgeCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public List<AchievementBadge> GetBadgesByCategory(string categoryID)
    {
        return badges.FindAll(b => b.categoryID == categoryID);
    }
    
    public List<AchievementBadge> GetBadgesByRarity(string rarity)
    {
        return badges.FindAll(b => b.rarity == rarity);
    }
}

[System.Serializable]
public class BadgeCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    public string categoryIcon;
    
    public BadgeCategory(string id, string name, string desc)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = desc;
        categoryIcon = "";
    }
}

[System.Serializable]
public class AchievementBadge
{
    public string badgeID;
    public string badgeName;
    public string badgeDescription;
    public string categoryID;
    public string rarity;
    public int maxLevel;
    public string badgeIcon;
    public string unlockCondition;
    public bool isHidden;
    
    public AchievementBadge(string id, string name, string desc, string category, string rarity, int maxLevel, string condition)
    {
        badgeID = id;
        badgeName = name;
        badgeDescription = desc;
        categoryID = category;
        this.rarity = rarity;
        this.maxLevel = maxLevel;
        badgeIcon = "";
        unlockCondition = condition;
        isHidden = false;
    }
    
    public void SetHidden(bool hidden)
    {
        isHidden = hidden;
    }
}

[System.Serializable]
public class PlayerBadgeData
{
    public string playerID;
    public Dictionary<string, int> badgeLevels;
    public List<string> unlockedBadges;
    public List<string> equippedBadges;
    
    public PlayerBadgeData(string playerID)
    {
        this.playerID = playerID;
        badgeLevels = new Dictionary<string, int>();
        unlockedBadges = new List<string>();
        equippedBadges = new List<string>();
    }
    
    public void UnlockBadge(string badgeID)
    {
        if (!unlockedBadges.Contains(badgeID))
        {
            unlockedBadges.Add(badgeID);
            badgeLevels[badgeID] = 1;
        }
    }
    
    public void LevelUpBadge(string badgeID)
    {
        if (badgeLevels.ContainsKey(badgeID))
        {
            badgeLevels[badgeID]++;
        }
        else
        {
            badgeLevels[badgeID] = 1;
        }
    }
    
    public void EquipBadge(string badgeID)
    {
        if (unlockedBadges.Contains(badgeID) && !equippedBadges.Contains(badgeID) && equippedBadges.Count < 5)
        {
            equippedBadges.Add(badgeID);
        }
    }
    
    public void UnequipBadge(string badgeID)
    {
        equippedBadges.Remove(badgeID);
    }
    
    public bool HasBadge(string badgeID)
    {
        return unlockedBadges.Contains(badgeID);
    }
    
    public int GetBadgeLevel(string badgeID)
    {
        return badgeLevels.ContainsKey(badgeID) ? badgeLevels[badgeID] : 0;
    }
    
    public List<string> GetEquippedBadges()
    {
        return equippedBadges;
    }
    
    public List<string> GetUnlockedBadges()
    {
        return unlockedBadges;
    }
}

[System.Serializable]
public class AchievementBadgeManagerData
{
    public AchievementBadgeSystem system;
    public List<PlayerBadgeData> playerData;
    
    public AchievementBadgeManagerData()
    {
        system = new AchievementBadgeSystem("achievement_badge_system", "成就徽章系统", "管理玩家的成就徽章");
        playerData = new List<PlayerBadgeData>();
    }
    
    public void AddPlayerData(PlayerBadgeData data)
    {
        playerData.Add(data);
    }
    
    public PlayerBadgeData GetPlayerData(string playerID)
    {
        return playerData.Find(pd => pd.playerID == playerID);
    }
}
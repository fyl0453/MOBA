using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AchievementBadgeManager : MonoBehaviour
{
    public static AchievementBadgeManager Instance { get; private set; }
    
    public AchievementBadgeManagerData badgeData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadBadgeData();
        
        if (badgeData == null)
        {
            badgeData = new AchievementBadgeManagerData();
            InitializeDefaultBadges();
        }
    }
    
    private void InitializeDefaultBadges()
    {
        // 创建徽章分类
        BadgeCategory战斗 = new BadgeCategory("category_combat", "战斗", "与战斗相关的徽章");
        BadgeCategory比赛 = new BadgeCategory("category_match", "比赛", "与比赛相关的徽章");
        BadgeCategory经济 = new BadgeCategory("category_economy", "经济", "与经济相关的徽章");
        BadgeCategory社交 = new BadgeCategory("category_social", "社交", "与社交相关的徽章");
        BadgeCategory成长 = new BadgeCategory("category_growth", "成长", "与成长相关的徽章");
        
        badgeData.system.AddCategory(战斗);
        badgeData.system.AddCategory(比赛);
        badgeData.system.AddCategory(经济);
        badgeData.system.AddCategory(社交);
        badgeData.system.AddCategory(成长);
        
        // 创建默认徽章
        AchievementBadge firstBlood = new AchievementBadge("badge_first_blood", "第一滴血", "获得第一滴血", "category_combat", "Common", 5, "获得1次第一滴血");
        AchievementBadge ace = new AchievementBadge("badge_ace", "五杀", "获得五杀", "category_combat", "Rare", 3, "获得1次五杀");
        AchievementBadge mvp = new AchievementBadge("badge_mvp", "MVP", "获得MVP", "category_match", "Epic", 10, "获得1次MVP");
        AchievementBadge winStreak = new AchievementBadge("badge_win_streak", "连胜", "获得连胜", "category_match", "Rare", 5, "获得3连胜");
        AchievementBadge goldKing = new AchievementBadge("badge_gold_king", "金币王", "成为金币王", "category_economy", "Epic", 5, "成为1次金币王");
        AchievementBadge damageKing = new AchievementBadge("badge_damage_king", "输出王", "成为输出王", "category_economy", "Epic", 5, "成为1次输出王");
        AchievementBadge friend = new AchievementBadge("badge_friend", "好友", "添加好友", "category_social", "Common", 10, "添加1个好友");
        AchievementBadge guild = new AchievementBadge("badge_guild", "公会", "加入公会", "category_social", "Common", 5, "加入1个公会");
        AchievementBadge levelUp = new AchievementBadge("badge_level_up", "升级", "提升等级", "category_growth", "Common", 20, "提升到1级");
        AchievementBadge heroMastery = new AchievementBadge("badge_hero_mastery", "英雄熟练度", "提升英雄熟练度", "category_growth", "Rare", 5, "英雄熟练度达到1级");
        
        badgeData.system.AddBadge(firstBlood);
        badgeData.system.AddBadge(ace);
        badgeData.system.AddBadge(mvp);
        badgeData.system.AddBadge(winStreak);
        badgeData.system.AddBadge(goldKing);
        badgeData.system.AddBadge(damageKing);
        badgeData.system.AddBadge(friend);
        badgeData.system.AddBadge(guild);
        badgeData.system.AddBadge(levelUp);
        badgeData.system.AddBadge(heroMastery);
        
        SaveBadgeData();
    }
    
    public void UnlockBadge(string playerID, string badgeID)
    {
        AchievementBadge badge = badgeData.system.GetBadge(badgeID);
        if (badge != null)
        {
            PlayerBadgeData playerData = GetOrCreatePlayerData(playerID);
            if (!playerData.HasBadge(badgeID))
            {
                playerData.UnlockBadge(badgeID);
                SaveBadgeData();
                Debug.Log($"成功解锁徽章: {badge.badgeName}");
            }
        }
    }
    
    public void LevelUpBadge(string playerID, string badgeID)
    {
        AchievementBadge badge = badgeData.system.GetBadge(badgeID);
        if (badge != null)
        {
            PlayerBadgeData playerData = GetOrCreatePlayerData(playerID);
            int currentLevel = playerData.GetBadgeLevel(badgeID);
            if (currentLevel < badge.maxLevel)
            {
                playerData.LevelUpBadge(badgeID);
                SaveBadgeData();
                Debug.Log($"徽章 {badge.badgeName} 升级到 {currentLevel + 1} 级");
            }
        }
    }
    
    public void EquipBadge(string playerID, string badgeID)
    {
        AchievementBadge badge = badgeData.system.GetBadge(badgeID);
        if (badge != null)
        {
            PlayerBadgeData playerData = GetOrCreatePlayerData(playerID);
            if (playerData.HasBadge(badgeID))
            {
                playerData.EquipBadge(badgeID);
                SaveBadgeData();
                Debug.Log($"成功装备徽章: {badge.badgeName}");
            }
        }
    }
    
    public void UnequipBadge(string playerID, string badgeID)
    {
        PlayerBadgeData playerData = GetOrCreatePlayerData(playerID);
        playerData.UnequipBadge(badgeID);
        SaveBadgeData();
        Debug.Log($"成功卸下徽章: {badgeID}");
    }
    
    public List<AchievementBadge> GetBadgesByCategory(string categoryID)
    {
        return badgeData.system.GetBadgesByCategory(categoryID);
    }
    
    public List<AchievementBadge> GetBadgesByRarity(string rarity)
    {
        return badgeData.system.GetBadgesByRarity(rarity);
    }
    
    public List<AchievementBadge> GetUnlockedBadges(string playerID)
    {
        PlayerBadgeData playerData = GetOrCreatePlayerData(playerID);
        List<AchievementBadge> unlockedBadges = new List<AchievementBadge>();
        
        foreach (string badgeID in playerData.GetUnlockedBadges())
        {
            AchievementBadge badge = badgeData.system.GetBadge(badgeID);
            if (badge != null)
            {
                unlockedBadges.Add(badge);
            }
        }
        
        return unlockedBadges;
    }
    
    public List<AchievementBadge> GetEquippedBadges(string playerID)
    {
        PlayerBadgeData playerData = GetOrCreatePlayerData(playerID);
        List<AchievementBadge> equippedBadges = new List<AchievementBadge>();
        
        foreach (string badgeID in playerData.GetEquippedBadges())
        {
            AchievementBadge badge = badgeData.system.GetBadge(badgeID);
            if (badge != null)
            {
                equippedBadges.Add(badge);
            }
        }
        
        return equippedBadges;
    }
    
    public int GetBadgeLevel(string playerID, string badgeID)
    {
        PlayerBadgeData playerData = GetOrCreatePlayerData(playerID);
        return playerData.GetBadgeLevel(badgeID);
    }
    
    public bool HasBadge(string playerID, string badgeID)
    {
        PlayerBadgeData playerData = GetOrCreatePlayerData(playerID);
        return playerData.HasBadge(badgeID);
    }
    
    public AchievementBadge GetBadge(string badgeID)
    {
        return badgeData.system.GetBadge(badgeID);
    }
    
    public List<BadgeCategory> GetBadgeCategories()
    {
        return badgeData.system.categories;
    }
    
    public void AddBadge(string name, string description, string category, string rarity, int maxLevel, string condition)
    {
        string badgeID = System.Guid.NewGuid().ToString();
        AchievementBadge badge = new AchievementBadge(badgeID, name, description, category, rarity, maxLevel, condition);
        badgeData.system.AddBadge(badge);
        SaveBadgeData();
    }
    
    public void AddBadgeCategory(string name, string description)
    {
        string categoryID = System.Guid.NewGuid().ToString();
        BadgeCategory category = new BadgeCategory(categoryID, name, description);
        badgeData.system.AddCategory(category);
        SaveBadgeData();
    }
    
    private PlayerBadgeData GetOrCreatePlayerData(string playerID)
    {
        PlayerBadgeData playerData = badgeData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerBadgeData(playerID);
            badgeData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public void SaveBadgeData()
    {
        string path = Application.dataPath + "/Data/achievement_badge_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, badgeData);
        stream.Close();
    }
    
    public void LoadBadgeData()
    {
        string path = Application.dataPath + "/Data/achievement_badge_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            badgeData = (AchievementBadgeManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            badgeData = new AchievementBadgeManagerData();
        }
    }
}
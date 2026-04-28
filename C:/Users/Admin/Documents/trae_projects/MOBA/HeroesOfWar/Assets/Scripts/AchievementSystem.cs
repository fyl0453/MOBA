using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AchievementSystem : MonoBehaviour
{
    public static AchievementSystem Instance { get; private set; }
    
    private AchievementData achievementData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAchievementData();
            
            if (achievementData == null)
            {
                achievementData = new AchievementData();
                InitializeDefaultAchievements();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeDefaultAchievements()
    {
        // 新手成就
        AddAchievement("achievement_first_game", "初次体验", "完成第一场游戏", 1, AchievementRarity.Common);
        AddAchievement("achievement_first_win", "初次胜利", "获得第一场胜利", 1, AchievementRarity.Common);
        AddAchievement("achievement_first_kill", "初次击杀", "击杀第一个敌人", 1, AchievementRarity.Common);
        
        // 战斗成就
        AddAchievement("achievement_kill_10", "初露锋芒", "累计击杀10个敌人", 10, AchievementRarity.Common);
        AddAchievement("achievement_kill_50", "小有所成", "累计击杀50个敌人", 50, AchievementRarity.Uncommon);
        AddAchievement("achievement_kill_100", "战场精英", "累计击杀100个敌人", 100, AchievementRarity.Rare);
        AddAchievement("achievement_kill_500", "战神", "累计击杀500个敌人", 500, AchievementRarity.Epic);
        AddAchievement("achievement_kill_1000", "传说", "累计击杀1000个敌人", 1000, AchievementRarity.Legendary);
        
        // 胜利成就
        AddAchievement("achievement_win_5", "初露峥嵘", "累计获得5场胜利", 5, AchievementRarity.Common);
        AddAchievement("achievement_win_20", "常胜将军", "累计获得20场胜利", 20, AchievementRarity.Uncommon);
        AddAchievement("achievement_win_50", "百战百胜", "累计获得50场胜利", 50, AchievementRarity.Rare);
        AddAchievement("achievement_win_100", "胜利大师", "累计获得100场胜利", 100, AchievementRarity.Epic);
        
        // 助攻成就
        AddAchievement("achievement_assist_20", "团队合作", "累计获得20次助攻", 20, AchievementRarity.Common);
        AddAchievement("achievement_assist_50", "助攻达人", "累计获得50次助攻", 50, AchievementRarity.Uncommon);
        AddAchievement("achievement_assist_100", "团队核心", "累计获得100次助攻", 100, AchievementRarity.Rare);
        
        // 经济成就
        AddAchievement("achievement_gold_10000", "财运亨通", "累计获得10000金币", 10000, AchievementRarity.Common);
        AddAchievement("achievement_gold_50000", "富甲一方", "累计获得50000金币", 50000, AchievementRarity.Uncommon);
        
        // 社交成就
        AddAchievement("achievement_friend_5", "广结良缘", "添加5个好友", 5, AchievementRarity.Common);
        AddAchievement("achievement_clan_join", "团队归属", "加入一个战队", 1, AchievementRarity.Common);
        AddAchievement("achievement_clan_create", "领袖气质", "创建一个战队", 1, AchievementRarity.Uncommon);
        
        // 英雄成就
        AddAchievement("achievement_hero_3", "英雄收藏家", "拥有3个英雄", 3, AchievementRarity.Common);
        AddAchievement("achievement_hero_5", "英雄大师", "拥有5个英雄", 5, AchievementRarity.Uncommon);
        AddAchievement("achievement_hero_10", "英雄传说", "拥有10个英雄", 10, AchievementRarity.Rare);
        
        SaveAchievementData();
    }
    
    private void AddAchievement(string id, string name, string description, int targetValue, AchievementRarity rarity)
    {
        Achievement achievement = new Achievement
        {
            achievementID = id,
            achievementName = name,
            description = description,
            targetValue = targetValue,
            currentValue = 0,
            isCompleted = false,
            rarity = rarity,
            unlockDate = null
        };
        
        achievementData.achievements.Add(achievement);
    }
    
    public void UpdateAchievementProgress(string achievementID, int value)
    {
        Achievement achievement = achievementData.achievements.Find(a => a.achievementID == achievementID);
        if (achievement != null && !achievement.isCompleted)
        {
            achievement.currentValue += value;
            if (achievement.currentValue >= achievement.targetValue)
            {
                achievement.currentValue = achievement.targetValue;
                achievement.isCompleted = true;
                achievement.unlockDate = System.DateTime.Now.ToString();
                Debug.Log($"成就解锁: {achievement.achievementName}");
                // 这里可以添加成就解锁的奖励和通知
            }
            SaveAchievementData();
        }
    }
    
    public List<Achievement> GetAllAchievements()
    {
        return achievementData.achievements;
    }
    
    public List<Achievement> GetCompletedAchievements()
    {
        return achievementData.achievements.FindAll(a => a.isCompleted);
    }
    
    public List<Achievement> GetInProgressAchievements()
    {
        return achievementData.achievements.FindAll(a => !a.isCompleted && a.currentValue > 0);
    }
    
    public List<Achievement> GetAchievementsByRarity(AchievementRarity rarity)
    {
        return achievementData.achievements.FindAll(a => a.rarity == rarity);
    }
    
    public Achievement GetAchievement(string achievementID)
    {
        return achievementData.achievements.Find(a => a.achievementID == achievementID);
    }
    
    public float GetAchievementProgress(string achievementID)
    {
        Achievement achievement = GetAchievement(achievementID);
        if (achievement != null)
        {
            return (float)achievement.currentValue / achievement.targetValue;
        }
        return 0f;
    }
    
    public int GetTotalAchievements()
    {
        return achievementData.achievements.Count;
    }
    
    public int GetCompletedAchievementsCount()
    {
        return achievementData.achievements.FindAll(a => a.isCompleted).Count;
    }
    
    public float GetOverallProgress()
    {
        int total = achievementData.achievements.Count;
        if (total == 0) return 0f;
        int completed = GetCompletedAchievementsCount();
        return (float)completed / total;
    }
    
    public void SaveAchievementData()
    {
        string path = Application.dataPath + "/Data/achievement_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, achievementData);
        stream.Close();
    }
    
    public void LoadAchievementData()
    {
        string path = Application.dataPath + "/Data/achievement_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            achievementData = (AchievementData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            achievementData = new AchievementData();
        }
    }
}

[System.Serializable]
public class AchievementData
{
    public List<Achievement> achievements = new List<Achievement>();
}

[System.Serializable]
public class Achievement
{
    public string achievementID;
    public string achievementName;
    public string description;
    public int targetValue;
    public int currentValue;
    public bool isCompleted;
    public AchievementRarity rarity;
    public string unlockDate;
}

public enum AchievementRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

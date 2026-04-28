using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AchievementManagerExtended : MonoBehaviour
{
    public static AchievementManagerExtended Instance { get; private set; }
    
    public AchievementManagerExtendedData achievementData;
    
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
        LoadAchievementData();
        
        if (achievementData == null)
        {
            achievementData = new AchievementManagerExtendedData();
            InitializeDefaultAchievements();
        }
        
        EnsurePlayerProgressData();
    }
    
    private void EnsurePlayerProgressData()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        AchievementProgress progress = achievementData.GetProgressData(playerID);
        if (progress == null)
        {
            progress = new AchievementProgress(playerID);
            achievementData.AddProgressData(progress);
            SaveAchievementData();
        }
    }
    
    private void InitializeDefaultAchievements()
    {
        // 战斗成就
        AchievementCategory combatCategory = new AchievementCategory("combat", "战斗", "与战斗相关的成就");
        combatCategory.AddAchievement(new Achievement("achievement_first_blood", "第一滴血", "获得第一滴血", 1, "首次击杀敌人"));
        combatCategory.AddAchievement(new Achievement("achievement_double_kill", "双杀", "获得双杀", 1, "一次击杀2个敌人"));
        combatCategory.AddAchievement(new Achievement("achievement_triple_kill", "三杀", "获得三杀", 1, "一次击杀3个敌人"));
        combatCategory.AddAchievement(new Achievement("achievement_quadra_kill", "四杀", "获得四杀", 1, "一次击杀4个敌人"));
        combatCategory.AddAchievement(new Achievement("achievement_penta_kill", "五杀", "获得五杀", 1, "一次击杀5个敌人"));
        combatCategory.AddAchievement(new Achievement("achievement_kill_streak", "连杀", "获得10连杀", 10, "连续击杀10个敌人"));
        combatCategory.AddAchievement(new Achievement("achievement_total_kills", "击杀大师", "累计击杀1000个敌人", 1000, "累计击杀1000个敌人"));
        achievementData.AddCategory(combatCategory);
        
        // 比赛成就
        AchievementCategory matchCategory = new AchievementCategory("match", "比赛", "与比赛相关的成就");
        matchCategory.AddAchievement(new Achievement("achievement_first_win", "首胜", "获得第一场胜利", 1, "获得第一场胜利"));
        matchCategory.AddAchievement(new Achievement("achievement_win_streak", "连胜", "获得5连胜", 5, "连续赢得5场比赛"));
        matchCategory.AddAchievement(new Achievement("achievement_total_wins", "胜利大师", "累计赢得100场比赛", 100, "累计赢得100场比赛"));
        matchCategory.AddAchievement(new Achievement("achievement_mvp", "MVP", "获得10次MVP", 10, "获得10次MVP"));
        matchCategory.AddAchievement(new Achievement("achievement_assist", "助攻王", "累计获得500次助攻", 500, "累计获得500次助攻"));
        achievementData.AddCategory(matchCategory);
        
        // 经济成就
        AchievementCategory economyCategory = new AchievementCategory("economy", "经济", "与经济相关的成就");
        economyCategory.AddAchievement(new Achievement("achievement_gold", "富甲一方", "单场比赛获得10000金币", 10000, "单场比赛获得10000金币"));
        economyCategory.AddAchievement(new Achievement("achievement_total_gold", "财富大师", "累计获得100000金币", 100000, "累计获得100000金币"));
        economyCategory.AddAchievement(new Achievement("achievement_item", "装备大师", "购买100件装备", 100, "购买100件装备"));
        achievementData.AddCategory(economyCategory);
        
        // 社交成就
        AchievementCategory socialCategory = new AchievementCategory("social", "社交", "与社交相关的成就");
        socialCategory.AddAchievement(new Achievement("achievement_friend", "好友", "添加5个好友", 5, "添加5个好友"));
        socialCategory.AddAchievement(new Achievement("achievement_guild", "公会", "加入公会", 1, "加入公会"));
        socialCategory.AddAchievement(new Achievement("achievement_mentor", "导师", "成为师父", 1, "成为师父"));
        socialCategory.AddAchievement(new Achievement("achievement_apprentice", "学徒", "成为徒弟", 1, "成为徒弟"));
        socialCategory.AddAchievement(new Achievement("achievement_team", "战队", "创建战队", 1, "创建战队"));
        achievementData.AddCategory(socialCategory);
        
        // 成长成就
        AchievementCategory growthCategory = new AchievementCategory("growth", "成长", "与成长相关的成就");
        growthCategory.AddAchievement(new Achievement("achievement_hero", "英雄收藏家", "解锁10个英雄", 10, "解锁10个英雄"));
        growthCategory.AddAchievement(new Achievement("achievement_skin", "皮肤收藏家", "拥有5个皮肤", 5, "拥有5个皮肤"));
        growthCategory.AddAchievement(new Achievement("achievement_mastery", "熟练度大师", "一个英雄达到熟练度5级", 1, "一个英雄达到熟练度5级"));
        growthCategory.AddAchievement(new Achievement("achievement_rank", "段位大师", "达到钻石段位", 1, "达到钻石段位"));
        growthCategory.AddAchievement(new Achievement("achievement_level", "等级大师", "达到30级", 30, "达到30级"));
        achievementData.AddCategory(growthCategory);
        
        // 活动成就
        AchievementCategory activityCategory = new AchievementCategory("activity", "活动", "与活动相关的成就");
        activityCategory.AddAchievement(new Achievement("achievement_sign_in", "签到达人", "连续签到7天", 7, "连续签到7天"));
        activityCategory.AddAchievement(new Achievement("achievement_task", "任务达人", "完成10个任务", 10, "完成10个任务"));
        activityCategory.AddAchievement(new Achievement("achievement_event", "活动达人", "参与5个活动", 5, "参与5个活动"));
        achievementData.AddCategory(activityCategory);
        
        // 奖励
        achievementData.AddReward(new AchievementReward("reward_gold", "Currency", "gold", 1000, "1000金币"));
        achievementData.AddReward(new AchievementReward("reward_gems", "Currency", "gems", 100, "100钻石"));
        achievementData.AddReward(new AchievementReward("reward_potion", "Item", "item_health_potion", 5, "5个生命药水"));
        achievementData.AddReward(new AchievementReward("reward_skin", "Skin", "skin_guanyu_spring", 1, "关羽新春皮肤"));
        
        SaveAchievementData();
    }
    
    public void UpdateAchievementProgress(string achievementID, int progress)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        AchievementProgress playerProgress = achievementData.GetProgressData(playerID);
        Achievement achievement = achievementData.GetAchievement(achievementID);
        
        if (playerProgress != null && achievement != null)
        {
            int currentProgress = playerProgress.GetProgress(achievementID);
            int newProgress = Mathf.Min(currentProgress + progress, achievement.requiredProgress);
            playerProgress.UpdateProgress(achievementID, newProgress);
            
            if (newProgress >= achievement.requiredProgress && !playerProgress.IsCompleted(achievementID))
            {
                playerProgress.MarkAsCompleted(achievementID);
                achievement.isCompleted = true;
                achievementData.UpdateCompletionCount();
                
                // 发放奖励
                GrantAchievementReward(achievementID);
            }
            
            SaveAchievementData();
        }
    }
    
    private void GrantAchievementReward(string achievementID)
    {
        // 根据成就ID发放不同的奖励
        switch (achievementID)
        {
            case "achievement_first_blood":
                ProfileManager.Instance.currentProfile.gold += 100;
                break;
            case "achievement_penta_kill":
                ProfileManager.Instance.currentProfile.gems += 50;
                break;
            case "achievement_win_streak":
                ProfileManager.Instance.currentProfile.gold += 500;
                break;
            case "achievement_mvp":
                ProfileManager.Instance.currentProfile.gems += 20;
                break;
            case "achievement_hero":
                ProfileManager.Instance.currentProfile.gold += 1000;
                break;
        }
        
        ProfileManager.Instance.SaveProfile();
    }
    
    public void ClaimAchievementReward(string achievementID)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        AchievementProgress playerProgress = achievementData.GetProgressData(playerID);
        
        if (playerProgress != null && playerProgress.IsCompleted(achievementID) && !playerProgress.IsClaimed(achievementID))
        {
            playerProgress.MarkAsClaimed(achievementID);
            SaveAchievementData();
        }
    }
    
    public List<AchievementCategory> GetAllCategories()
    {
        return achievementData.categories;
    }
    
    public AchievementCategory GetCategory(string categoryID)
    {
        return achievementData.GetCategory(categoryID);
    }
    
    public AchievementProgress GetPlayerProgress()
    {
        return achievementData.GetProgressData(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public int GetTotalAchievements()
    {
        return achievementData.totalAchievements;
    }
    
    public int GetCompletedAchievements()
    {
        return achievementData.totalCompleted;
    }
    
    public float GetCompletionRate()
    {
        if (achievementData.totalAchievements == 0)
            return 0;
        return (float)achievementData.totalCompleted / achievementData.totalAchievements * 100f;
    }
    
    public void SaveAchievementData()
    {
        string path = Application.dataPath + "/Data/achievement_extended_data.dat";
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
        string path = Application.dataPath + "/Data/achievement_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            achievementData = (AchievementManagerExtendedData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            achievementData = new AchievementManagerExtendedData();
        }
    }
}
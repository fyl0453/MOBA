using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    
    public AchievementList achievementList;
    
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
        
        if (achievementList == null)
        {
            achievementList = new AchievementList(ProfileManager.Instance.currentProfile.playerID);
            InitializeDefaultAchievements();
        }
    }
    
    private void InitializeDefaultAchievements()
    {
        // 击杀相关成就
        Achievement firstBlood = new Achievement("achievement_first_blood", "第一滴血", "在一场比赛中获得第一滴血", "Combat", 1);
        firstBlood.AddReward("Currency", "gold", 100);
        achievementList.AddAchievement(firstBlood);
        
        Achievement killingSpree = new Achievement("achievement_killing_spree", "连杀", "在一场比赛中连续击杀3名敌人", "Combat", 3);
        killingSpree.AddReward("Currency", "gold", 200);
        achievementList.AddAchievement(killingSpree);
        
        Achievement rampage = new Achievement("achievement_rampage", "暴走", "在一场比赛中连续击杀5名敌人", "Combat", 5);
        rampage.AddReward("Currency", "gold", 500);
        rampage.AddReward("Item", "item_health_potion", 5);
        achievementList.AddAchievement(rampage);
        
        Achievement unstoppable = new Achievement("achievement_unstoppable", "无人可挡", "在一场比赛中连续击杀7名敌人", "Combat", 7);
        unstoppable.AddReward("Currency", "gems", 100);
        unstoppable.AddReward("Item", "item_attack_potion", 3);
        achievementList.AddAchievement(unstoppable);
        
        // 比赛相关成就
        Achievement firstWin = new Achievement("achievement_first_win", "首胜", "赢得第一场比赛", "Match", 1);
        firstWin.AddReward("Currency", "gold", 300);
        firstWin.AddReward("Item", "item_mana_potion", 5);
        achievementList.AddAchievement(firstWin);
        
        Achievement winStreak = new Achievement("achievement_win_streak", "连胜", "连续赢得3场比赛", "Match", 3);
        winStreak.AddReward("Currency", "gold", 500);
        winStreak.AddReward("Item", "item_defense_potion", 3);
        achievementList.AddAchievement(winStreak);
        
        Achievement hundredWins = new Achievement("achievement_hundred_wins", "百胜", "累计赢得100场比赛", "Match", 100);
        hundredWins.AddReward("Currency", "gems", 500);
        hundredWins.AddReward("Item", "item_speed_potion", 10);
        achievementList.AddAchievement(hundredWins);
        
        // 社交相关成就
        Achievement firstFriend = new Achievement("achievement_first_friend", "第一个朋友", "添加第一个好友", "Social", 1);
        firstFriend.AddReward("Currency", "gold", 100);
        achievementList.AddAchievement(firstFriend);
        
        Achievement guildMember = new Achievement("achievement_guild_member", "公会成员", "加入一个公会", "Social", 1);
        guildMember.AddReward("Currency", "gold", 200);
        achievementList.AddAchievement(guildMember);
        
        // 物品相关成就
        Achievement firstPurchase = new Achievement("achievement_first_purchase", "首次购买", "在商店购买第一个物品", "Economy", 1);
        firstPurchase.AddReward("Currency", "gold", 50);
        achievementList.AddAchievement(firstPurchase);
        
        Achievement rich = new Achievement("achievement_rich", "有钱人", "拥有10000金币", "Economy", 10000);
        rich.AddReward("Currency", "gems", 200);
        achievementList.AddAchievement(rich);
        
        SaveAchievementData();
    }
    
    public void UpdateAchievementProgress(string achievementID, int progress)
    {
        achievementList.UpdateAchievementProgress(achievementID, progress);
        SaveAchievementData();
    }
    
    public void ClaimAchievementRewards(string achievementID)
    {
        Achievement achievement = achievementList.GetAchievement(achievementID);
        if (achievement != null && achievement.isCompleted && !achievement.isClaimed)
        {
            foreach (AchievementReward reward in achievement.rewards)
            {
                GrantReward(reward);
            }
            achievement.ClaimRewards();
            SaveAchievementData();
        }
    }
    
    private void GrantReward(AchievementReward reward)
    {
        switch (reward.rewardType)
        {
            case "Item":
                InventoryManager.Instance.AddItemToInventory(reward.rewardItemID, reward.quantity);
                break;
            case "Currency":
                if (reward.rewardItemID == "gold")
                {
                    ProfileManager.Instance.currentProfile.gold += reward.quantity;
                    ProfileManager.Instance.SaveProfile();
                }
                else if (reward.rewardItemID == "gems")
                {
                    ProfileManager.Instance.currentProfile.gems += reward.quantity;
                    ProfileManager.Instance.SaveProfile();
                }
                break;
        }
    }
    
    public List<Achievement> GetAllAchievements()
    {
        return achievementList.achievements;
    }
    
    public List<Achievement> GetCompletedAchievements()
    {
        return achievementList.GetCompletedAchievements();
    }
    
    public List<Achievement> GetUnclaimedAchievements()
    {
        return achievementList.GetUnclaimedAchievements();
    }
    
    public List<Achievement> GetAchievementsByCategory(string category)
    {
        return achievementList.GetAchievementsByCategory(category);
    }
    
    public int GetUnclaimedAchievementCount()
    {
        return achievementList.GetUnclaimedAchievements().Count;
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
        formatter.Serialize(stream, achievementList);
        stream.Close();
    }
    
    public void LoadAchievementData()
    {
        string path = Application.dataPath + "/Data/achievement_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            achievementList = (AchievementList)formatter.Deserialize(stream);
            stream.Close();
        }
    }
}
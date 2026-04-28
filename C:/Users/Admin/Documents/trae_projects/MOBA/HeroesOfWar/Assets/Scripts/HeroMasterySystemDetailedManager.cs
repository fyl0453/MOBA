using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HeroMasterySystemDetailedManager : MonoBehaviour
{
    public static HeroMasterySystemDetailedManager Instance { get; private set; }
    
    public HeroMasterySystemDetailedManagerData heroMasteryData;
    
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
        LoadHeroMasteryData();
        
        if (heroMasteryData == null)
        {
            heroMasteryData = new HeroMasterySystemDetailedManagerData();
            InitializeDefaultHeroMasterySystem();
        }
    }
    
    private void InitializeDefaultHeroMasterySystem()
    {
        // 英雄熟练度等级
        HeroMasteryLevel level1 = new HeroMasteryLevel(1, "见习", "英雄熟练度等级1", 0, "icon_mastery_1");
        HeroMasteryLevel level2 = new HeroMasteryLevel(2, "资深", "英雄熟练度等级2", 100, "icon_mastery_2");
        HeroMasteryLevel level3 = new HeroMasteryLevel(3, "精英", "英雄熟练度等级3", 200, "icon_mastery_3");
        HeroMasteryLevel level4 = new HeroMasteryLevel(4, "宗师", "英雄熟练度等级4", 300, "icon_mastery_4");
        HeroMasteryLevel level5 = new HeroMasteryLevel(5, "传说", "英雄熟练度等级5", 500, "icon_mastery_5");
        
        heroMasteryData.system.AddHeroMasteryLevel(level1);
        heroMasteryData.system.AddHeroMasteryLevel(level2);
        heroMasteryData.system.AddHeroMasteryLevel(level3);
        heroMasteryData.system.AddHeroMasteryLevel(level4);
        heroMasteryData.system.AddHeroMasteryLevel(level5);
        
        // 英雄熟练度奖励
        HeroMasteryReward reward1 = new HeroMasteryReward("reward_001", "见习奖励", "英雄熟练度等级1奖励", 1, "currency", "gold", 500, "icon_gold");
        HeroMasteryReward reward2 = new HeroMasteryReward("reward_002", "资深奖励", "英雄熟练度等级2奖励", 2, "currency", "gold", 1000, "icon_gold");
        HeroMasteryReward reward3 = new HeroMasteryReward("reward_003", "精英奖励", "英雄熟练度等级3奖励", 3, "currency", "diamond", 50, "icon_diamond");
        HeroMasteryReward reward4 = new HeroMasteryReward("reward_004", "宗师奖励", "英雄熟练度等级4奖励", 4, "currency", "diamond", 100, "icon_diamond");
        HeroMasteryReward reward5 = new HeroMasteryReward("reward_005", "传说奖励", "英雄熟练度等级5奖励", 5, "skin", "skin_hero", 1, "icon_skin");
        
        heroMasteryData.system.AddHeroMasteryReward(reward1);
        heroMasteryData.system.AddHeroMasteryReward(reward2);
        heroMasteryData.system.AddHeroMasteryReward(reward3);
        heroMasteryData.system.AddHeroMasteryReward(reward4);
        heroMasteryData.system.AddHeroMasteryReward(reward5);
        
        // 添加奖励到等级
        level1.AddReward("reward_001");
        level2.AddReward("reward_002");
        level3.AddReward("reward_003");
        level4.AddReward("reward_004");
        level5.AddReward("reward_005");
        
        // 英雄熟练度
        HeroMastery mastery1 = new HeroMastery("mastery_001", "user_001", "张三", "hero_001", "李白");
        HeroMastery mastery2 = new HeroMastery("mastery_002", "user_001", "张三", "hero_002", "韩信");
        HeroMastery mastery3 = new HeroMastery("mastery_003", "user_002", "李四", "hero_001", "李白");
        
        // 添加熟练度和游戏记录
        mastery1.AddMasteryPoints(150);
        mastery1.AddGame(true);
        mastery1.AddGame(true);
        mastery1.AddGame(false);
        mastery1.AddGame(true);
        mastery1.AddGame(true);
        
        mastery2.AddMasteryPoints(80);
        mastery2.AddGame(true);
        mastery2.AddGame(false);
        mastery2.AddGame(false);
        
        mastery3.AddMasteryPoints(250);
        mastery3.AddGame(true);
        mastery3.AddGame(true);
        mastery3.AddGame(true);
        mastery3.AddGame(true);
        mastery3.AddGame(false);
        mastery3.AddGame(true);
        
        // 领取奖励
        mastery1.ClaimReward("reward_001");
        mastery1.ClaimReward("reward_002");
        
        mastery2.ClaimReward("reward_001");
        
        mastery3.ClaimReward("reward_001");
        mastery3.ClaimReward("reward_002");
        mastery3.ClaimReward("reward_003");
        
        heroMasteryData.system.AddHeroMastery(mastery1);
        heroMasteryData.system.AddHeroMastery(mastery2);
        heroMasteryData.system.AddHeroMastery(mastery3);
        
        // 英雄熟练度事件
        HeroMasteryEvent event1 = new HeroMasteryEvent("event_001", "level_up", "user_001", "hero_001", "英雄熟练度等级提升", 150);
        HeroMasteryEvent event2 = new HeroMasteryEvent("event_002", "reward_claim", "user_001", "hero_001", "领取熟练度奖励", 0);
        HeroMasteryEvent event3 = new HeroMasteryEvent("event_003", "game_played", "user_001", "hero_001", "使用英雄游戏", 10);
        
        heroMasteryData.system.AddHeroMasteryEvent(event1);
        heroMasteryData.system.AddHeroMasteryEvent(event2);
        heroMasteryData.system.AddHeroMasteryEvent(event3);
        
        SaveHeroMasteryData();
    }
    
    // 英雄熟练度等级管理
    public HeroMasteryLevel GetHeroMasteryLevel(int level)
    {
        return heroMasteryData.system.GetHeroMasteryLevel(level);
    }
    
    public List<HeroMasteryLevel> GetHeroMasteryLevels()
    {
        return heroMasteryData.system.GetHeroMasteryLevels();
    }
    
    // 英雄熟练度管理
    public void AddHeroMastery(string userID, string userName, string heroID, string heroName)
    {
        HeroMastery existingMastery = heroMasteryData.system.GetHeroMastery(userID, heroID);
        if (existingMastery == null)
        {
            string masteryID = "mastery_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            HeroMastery newMastery = new HeroMastery(masteryID, userID, userName, heroID, heroName);
            heroMasteryData.system.AddHeroMastery(newMastery);
            
            // 创建英雄熟练度事件
            CreateHeroMasteryEvent("create", userID, heroID, "创建英雄熟练度", 0);
            
            SaveHeroMasteryData();
            Debug.Log("成功添加英雄熟练度: " + heroName);
        }
        else
        {
            Debug.LogError("英雄熟练度已存在");
        }
    }
    
    public void UpdateHeroMastery(string userID, string heroID, int points, bool isWin)
    {
        HeroMastery mastery = heroMasteryData.system.GetHeroMastery(userID, heroID);
        if (mastery != null)
        {
            int oldLevel = mastery.masteryLevel;
            mastery.AddMasteryPoints(points);
            mastery.AddGame(isWin);
            
            // 创建英雄熟练度事件
            CreateHeroMasteryEvent("game_played", userID, heroID, "使用英雄游戏", points);
            
            // 检查是否升级
            if (mastery.masteryLevel > oldLevel)
            {
                CreateHeroMasteryEvent("level_up", userID, heroID, "英雄熟练度等级提升", points);
            }
            
            SaveHeroMasteryData();
            Debug.Log("成功更新英雄熟练度: " + mastery.heroName + " 熟练度: " + mastery.masteryPoints + " 等级: " + mastery.masteryLevel);
        }
        else
        {
            // 如果英雄熟练度不存在，创建新的
            AddHeroMastery(userID, "未知用户", heroID, "未知英雄");
            UpdateHeroMastery(userID, heroID, points, isWin);
        }
    }
    
    public HeroMastery GetHeroMastery(string userID, string heroID)
    {
        return heroMasteryData.system.GetHeroMastery(userID, heroID);
    }
    
    public List<HeroMastery> GetUserHeroMasteries(string userID)
    {
        return heroMasteryData.system.GetHeroMasteriesByUser(userID);
    }
    
    public List<HeroMastery> GetHeroMasteriesByHero(string heroID)
    {
        return heroMasteryData.system.GetHeroMasteriesByHero(heroID);
    }
    
    // 英雄熟练度奖励管理
    public void AddHeroMasteryReward(string rewardName, string rewardDescription, int requiredLevel, string rewardType, string rewardValue, int quantity, string icon)
    {
        string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HeroMasteryReward heroMasteryReward = new HeroMasteryReward(rewardID, rewardName, rewardDescription, requiredLevel, rewardType, rewardValue, quantity, icon);
        heroMasteryData.system.AddHeroMasteryReward(heroMasteryReward);
        
        // 添加奖励到等级
        HeroMasteryLevel heroMasteryLevel = heroMasteryData.system.GetHeroMasteryLevel(requiredLevel);
        if (heroMasteryLevel != null)
        {
            heroMasteryLevel.AddReward(rewardID);
        }
        
        SaveHeroMasteryData();
        Debug.Log("成功添加英雄熟练度奖励: " + rewardName);
    }
    
    public void ClaimHeroMasteryReward(string userID, string heroID, string rewardID)
    {
        HeroMastery mastery = heroMasteryData.system.GetHeroMastery(userID, heroID);
        HeroMasteryReward reward = heroMasteryData.system.GetHeroMasteryReward(rewardID);
        
        if (mastery != null && reward != null && reward.IsAvailable(mastery.masteryLevel) && !mastery.HasClaimedReward(rewardID))
        {
            mastery.ClaimReward(rewardID);
            
            // 创建英雄熟练度事件
            CreateHeroMasteryEvent("reward_claim", userID, heroID, "领取熟练度奖励: " + reward.rewardName, 0);
            
            // 这里可以添加发放奖励的逻辑
            Debug.Log("发放英雄熟练度奖励: " + reward.rewardName + " 给用户: " + mastery.userName);
            
            SaveHeroMasteryData();
            Debug.Log("成功领取英雄熟练度奖励: " + reward.rewardName);
        }
        else
        {
            Debug.LogError("奖励不存在或不可领取");
        }
    }
    
    public List<HeroMasteryReward> GetHeroMasteryRewardsByLevel(int level)
    {
        return heroMasteryData.system.GetHeroMasteryRewardsByLevel(level);
    }
    
    public List<HeroMasteryReward> GetAvailableRewards(string userID, string heroID)
    {
        HeroMastery mastery = heroMasteryData.system.GetHeroMastery(userID, heroID);
        if (mastery != null)
        {
            List<HeroMasteryReward> allRewards = new List<HeroMasteryReward>();
            for (int i = 1; i <= mastery.masteryLevel; i++)
            {
                allRewards.AddRange(heroMasteryData.system.GetHeroMasteryRewardsByLevel(i));
            }
            return allRewards.FindAll(r => !mastery.HasClaimedReward(r.rewardID));
        }
        else
        {
            return new List<HeroMasteryReward>();
        }
    }
    
    // 英雄熟练度事件管理
    public string CreateHeroMasteryEvent(string eventType, string userID, string heroID, string description, int points)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HeroMasteryEvent heroMasteryEvent = new HeroMasteryEvent(eventID, eventType, userID, heroID, description, points);
        heroMasteryData.system.AddHeroMasteryEvent(heroMasteryEvent);
        SaveHeroMasteryData();
        Debug.Log("成功创建英雄熟练度事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        HeroMasteryEvent heroMasteryEvent = heroMasteryData.system.GetHeroMasteryEvent(eventID);
        if (heroMasteryEvent != null)
        {
            heroMasteryEvent.MarkAsCompleted();
            SaveHeroMasteryData();
            Debug.Log("成功标记英雄熟练度事件为完成");
        }
        else
        {
            Debug.LogError("英雄熟练度事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        HeroMasteryEvent heroMasteryEvent = heroMasteryData.system.GetHeroMasteryEvent(eventID);
        if (heroMasteryEvent != null)
        {
            heroMasteryEvent.MarkAsFailed();
            SaveHeroMasteryData();
            Debug.Log("成功标记英雄熟练度事件为失败");
        }
        else
        {
            Debug.LogError("英雄熟练度事件不存在: " + eventID);
        }
    }
    
    public List<HeroMasteryEvent> GetUserEvents(string userID)
    {
        return heroMasteryData.system.GetHeroMasteryEventsByUser(userID);
    }
    
    // 数据持久化
    public void SaveHeroMasteryData()
    {
        string path = Application.dataPath + "/Data/hero_mastery_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, heroMasteryData);
        stream.Close();
    }
    
    public void LoadHeroMasteryData()
    {
        string path = Application.dataPath + "/Data/hero_mastery_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            heroMasteryData = (HeroMasterySystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            heroMasteryData = new HeroMasterySystemDetailedManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SocialInteractionDetailedManager : MonoBehaviour
{
    public static SocialInteractionDetailedManager Instance { get; private set; }
    
    public SocialInteractionDetailedManagerData socialInteractionData;
    
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
        LoadSocialInteractionData();
        
        if (socialInteractionData == null)
        {
            socialInteractionData = new SocialInteractionDetailedManagerData();
            InitializeDefaultSocialInteractions();
        }
    }
    
    private void InitializeDefaultSocialInteractions()
    {
        // 情侣关系
        CoupleRelationship couple1 = new CoupleRelationship("couple_001", "user_001", "user_002");
        couple1.AddIntimacyPoints(50);
        
        CoupleActivity activity1 = new CoupleActivity("activity_001", "game", "一起游戏", "和情侣一起完成一局游戏", 10);
        activity1.Complete();
        couple1.AddActivity(activity1);
        
        CoupleReward reward1 = new CoupleReward("reward_001", "情侣头像框", "avatar_frame", "情侣专属头像框", 2);
        couple1.AddReward(reward1);
        
        socialInteractionData.system.AddCoupleRelationship(couple1);
        
        // 好友互动
        FriendInteraction interaction1 = new FriendInteraction("interaction_001", "user_001", "user_003", "gift", "赠送了一个皮肤体验卡");
        FriendInteraction interaction2 = new FriendInteraction("interaction_002", "user_003", "user_001", "message", "谢谢！");
        socialInteractionData.system.AddFriendInteraction(interaction1);
        socialInteractionData.system.AddFriendInteraction(interaction2);
        
        // 小游戏
        MiniGame game1 = new MiniGame("game_001", "猜英雄", "根据提示猜英雄名称", "quiz", 2, 2, 60);
        GameRule rule1 = new GameRule("rule_001", "时间限制", "每轮有10秒时间猜测");
        GameReward reward2 = new GameReward("reward_002", "经验值", "exp", "获得100经验值", 5);
        game1.AddRule(rule1);
        game1.AddReward(reward2);
        socialInteractionData.system.AddMiniGame(game1);
        
        MiniGame game2 = new MiniGame("game_002", "答题挑战", "回答游戏相关问题", "quiz", 2, 4, 120);
        GameRule rule2 = new GameRule("rule_002", "题目数量", "共10道题目");
        GameReward reward3 = new GameReward("reward_003", "金币", "gold", "获得200金币", 8);
        game2.AddRule(rule2);
        game2.AddReward(reward3);
        socialInteractionData.system.AddMiniGame(game2);
        
        MiniGame game3 = new MiniGame("game_003", "反应测试", "测试反应速度", "action", 2, 2, 30);
        GameRule rule3 = new GameRule("rule_003", "游戏机制", "点击出现的目标");
        GameReward reward4 = new GameReward("reward_004", "钻石", "diamond", "获得5钻石", 10);
        game3.AddRule(rule3);
        game3.AddReward(reward4);
        socialInteractionData.system.AddMiniGame(game3);
        
        MiniGame game4 = new MiniGame("game_004", "记忆挑战", "记忆并重复序列", "memory", 2, 2, 45);
        GameRule rule4 = new GameRule("rule_004", "难度递增", "每轮序列长度增加");
        GameReward reward5 = new GameReward("reward_005", "皮肤碎片", "skin_fragment", "获得1个皮肤碎片", 12);
        game4.AddRule(rule4);
        game4.AddReward(reward5);
        socialInteractionData.system.AddMiniGame(game4);
        
        // 社交事件
        SocialEvent ev1 = new SocialEvent("event_001", "好友聚会", "和好友一起游戏", "gathering", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"));
        ev1.AddParticipant("user_001");
        ev1.AddParticipant("user_002");
        ev1.AddParticipant("user_003");
        
        EventActivity eventActivity1 = new EventActivity("event_activity_001", "组队游戏", "和好友组队完成3局游戏", "game", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
        ev1.AddActivity(eventActivity1);
        
        socialInteractionData.system.AddSocialEvent(ev1);
        
        // 社交成就
        SocialAchievement achievement1 = new SocialAchievement("achievement_001", "友谊长存", "拥有10个好友", "friend", 10, "好友徽章");
        SocialAchievement achievement2 = new SocialAchievement("achievement_002", "情侣专属", "和情侣一起完成50局游戏", "couple", 50, "情侣称号");
        socialInteractionData.system.AddSocialAchievement(achievement1);
        socialInteractionData.system.AddSocialAchievement(achievement2);
        
        SaveSocialInteractionData();
    }
    
    // 情侣关系管理
    public void CreateCoupleRelationship(string user1ID, string user2ID)
    {
        // 检查是否已经存在情侣关系
        bool existingRelationship = socialInteractionData.system.coupleRelationships.Exists(r => 
            (r.user1ID == user1ID && r.user2ID == user2ID) || 
            (r.user1ID == user2ID && r.user2ID == user1ID));
        
        if (!existingRelationship)
        {
            string relationshipID = "couple_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            CoupleRelationship relationship = new CoupleRelationship(relationshipID, user1ID, user2ID);
            socialInteractionData.system.AddCoupleRelationship(relationship);
            SaveSocialInteractionData();
            Debug.Log("成功创建情侣关系: " + relationshipID);
        }
        else
        {
            Debug.Log("情侣关系已存在");
        }
    }
    
    public void EndCoupleRelationship(string relationshipID)
    {
        CoupleRelationship relationship = socialInteractionData.system.GetCoupleRelationship(relationshipID);
        if (relationship != null)
        {
            relationship.EndRelationship();
            SaveSocialInteractionData();
            Debug.Log("成功结束情侣关系: " + relationshipID);
        }
        else
        {
            Debug.LogError("情侣关系不存在: " + relationshipID);
        }
    }
    
    public void AddCoupleActivity(string relationshipID, string activityType, string activityName, string description, int intimacyPoints)
    {
        CoupleRelationship relationship = socialInteractionData.system.GetCoupleRelationship(relationshipID);
        if (relationship != null)
        {
            string activityID = "activity_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            CoupleActivity activity = new CoupleActivity(activityID, activityType, activityName, description, intimacyPoints);
            relationship.AddActivity(activity);
            SaveSocialInteractionData();
            Debug.Log("成功添加情侣活动: " + activityName);
        }
        else
        {
            Debug.LogError("情侣关系不存在: " + relationshipID);
        }
    }
    
    public void CompleteCoupleActivity(string relationshipID, string activityID)
    {
        CoupleRelationship relationship = socialInteractionData.system.GetCoupleRelationship(relationshipID);
        if (relationship != null)
        {
            CoupleActivity activity = relationship.activities.Find(a => a.activityID == activityID);
            if (activity != null)
            {
                activity.Complete();
                relationship.AddIntimacyPoints(activity.intimacyPoints);
                SaveSocialInteractionData();
                Debug.Log("成功完成情侣活动: " + activity.activityName);
            }
            else
            {
                Debug.LogError("活动不存在: " + activityID);
            }
        }
        else
        {
            Debug.LogError("情侣关系不存在: " + relationshipID);
        }
    }
    
    public void AddCoupleReward(string relationshipID, string rewardName, string rewardType, string description, int requiredLevel)
    {
        CoupleRelationship relationship = socialInteractionData.system.GetCoupleRelationship(relationshipID);
        if (relationship != null)
        {
            string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            CoupleReward reward = new CoupleReward(rewardID, rewardName, rewardType, description, requiredLevel);
            relationship.AddReward(reward);
            SaveSocialInteractionData();
            Debug.Log("成功添加情侣奖励: " + rewardName);
        }
        else
        {
            Debug.LogError("情侣关系不存在: " + relationshipID);
        }
    }
    
    public void ClaimCoupleReward(string relationshipID, string rewardID)
    {
        CoupleRelationship relationship = socialInteractionData.system.GetCoupleRelationship(relationshipID);
        if (relationship != null)
        {
            CoupleReward reward = relationship.rewards.Find(r => r.rewardID == rewardID);
            if (reward != null)
            {
                if (relationship.intimacyLevel >= reward.requiredLevel)
                {
                    reward.Claim();
                    SaveSocialInteractionData();
                    Debug.Log("成功领取情侣奖励: " + reward.rewardName);
                }
                else
                {
                    Debug.Log("亲密等级不足，无法领取奖励");
                }
            }
            else
            {
                Debug.LogError("奖励不存在: " + rewardID);
            }
        }
        else
        {
            Debug.LogError("情侣关系不存在: " + relationshipID);
        }
    }
    
    public List<CoupleRelationship> GetCoupleRelationshipsByUser(string userID)
    {
        return socialInteractionData.system.GetCoupleRelationshipsByUser(userID);
    }
    
    // 好友互动管理
    public void AddFriendInteraction(string user1ID, string user2ID, string interactionType, string content)
    {
        string interactionID = "interaction_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        FriendInteraction interaction = new FriendInteraction(interactionID, user1ID, user2ID, interactionType, content);
        socialInteractionData.system.AddFriendInteraction(interaction);
        SaveSocialInteractionData();
        Debug.Log("成功添加好友互动: " + interactionType);
    }
    
    public void MarkInteractionAsRead(string interactionID)
    {
        FriendInteraction interaction = socialInteractionData.system.GetFriendInteraction(interactionID);
        if (interaction != null)
        {
            interaction.MarkAsRead();
            SaveSocialInteractionData();
            Debug.Log("成功标记互动为已读: " + interactionID);
        }
        else
        {
            Debug.LogError("互动不存在: " + interactionID);
        }
    }
    
    public List<FriendInteraction> GetFriendInteractionsByUser(string userID)
    {
        return socialInteractionData.system.GetFriendInteractionsByUser(userID);
    }
    
    // 小游戏管理
    public void AddMiniGame(string name, string desc, string type, int minPlayers, int maxPlayers, int duration)
    {
        string gameID = "game_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MiniGame game = new MiniGame(gameID, name, desc, type, minPlayers, maxPlayers, duration);
        socialInteractionData.system.AddMiniGame(game);
        SaveSocialInteractionData();
        Debug.Log("成功添加小游戏: " + name);
    }
    
    public void AddGameRule(string gameID, string ruleName, string ruleDescription)
    {
        MiniGame game = socialInteractionData.system.GetMiniGame(gameID);
        if (game != null)
        {
            string ruleID = "rule_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            GameRule rule = new GameRule(ruleID, ruleName, ruleDescription);
            game.AddRule(rule);
            SaveSocialInteractionData();
            Debug.Log("成功添加游戏规则: " + ruleName);
        }
        else
        {
            Debug.LogError("游戏不存在: " + gameID);
        }
    }
    
    public void AddGameReward(string gameID, string rewardName, string rewardType, string description, int requiredScore)
    {
        MiniGame game = socialInteractionData.system.GetMiniGame(gameID);
        if (game != null)
        {
            string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            GameReward reward = new GameReward(rewardID, rewardName, rewardType, description, requiredScore);
            game.AddReward(reward);
            SaveSocialInteractionData();
            Debug.Log("成功添加游戏奖励: " + rewardName);
        }
        else
        {
            Debug.LogError("游戏不存在: " + gameID);
        }
    }
    
    public List<MiniGame> GetAllMiniGames()
    {
        return socialInteractionData.system.GetAllMiniGames();
    }
    
    public MiniGame GetMiniGame(string gameID)
    {
        return socialInteractionData.system.GetMiniGame(gameID);
    }
    
    // 社交事件管理
    public void CreateSocialEvent(string name, string desc, string type, string startTime, string endTime)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SocialEvent ev = new SocialEvent(eventID, name, desc, type, startTime, endTime);
        socialInteractionData.system.AddSocialEvent(ev);
        SaveSocialInteractionData();
        Debug.Log("成功创建社交事件: " + name);
    }
    
    public void AddParticipantToEvent(string eventID, string userID)
    {
        SocialEvent ev = socialInteractionData.system.GetSocialEvent(eventID);
        if (ev != null)
        {
            ev.AddParticipant(userID);
            SaveSocialInteractionData();
            Debug.Log("成功添加参与者到事件: " + userID);
        }
        else
        {
            Debug.LogError("事件不存在: " + eventID);
        }
    }
    
    public void AddActivityToEvent(string eventID, string activityName, string activityDescription, string activityType, string startTime, string endTime)
    {
        SocialEvent ev = socialInteractionData.system.GetSocialEvent(eventID);
        if (ev != null)
        {
            string activityID = "event_activity_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            EventActivity activity = new EventActivity(activityID, activityName, activityDescription, activityType, startTime, endTime);
            ev.AddActivity(activity);
            SaveSocialInteractionData();
            Debug.Log("成功添加活动到事件: " + activityName);
        }
        else
        {
            Debug.LogError("事件不存在: " + eventID);
        }
    }
    
    public void CompleteEventActivity(string eventID, string activityID)
    {
        SocialEvent ev = socialInteractionData.system.GetSocialEvent(eventID);
        if (ev != null)
        {
            EventActivity activity = ev.activities.Find(a => a.activityID == activityID);
            if (activity != null)
            {
                activity.Complete();
                SaveSocialInteractionData();
                Debug.Log("成功完成事件活动: " + activity.activityName);
            }
            else
            {
                Debug.LogError("活动不存在: " + activityID);
            }
        }
        else
        {
            Debug.LogError("事件不存在: " + eventID);
        }
    }
    
    public List<SocialEvent> GetSocialEventsByUser(string userID)
    {
        return socialInteractionData.system.GetSocialEventsByUser(userID);
    }
    
    // 社交成就管理
    public void AddSocialAchievement(string name, string desc, string type, int requiredValue, string reward)
    {
        string achievementID = "achievement_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SocialAchievement achievement = new SocialAchievement(achievementID, name, desc, type, requiredValue, reward);
        socialInteractionData.system.AddSocialAchievement(achievement);
        SaveSocialInteractionData();
        Debug.Log("成功添加社交成就: " + name);
    }
    
    public List<SocialAchievement> GetAllSocialAchievements()
    {
        return socialInteractionData.system.socialAchievements;
    }
    
    // 数据持久化
    public void SaveSocialInteractionData()
    {
        string path = Application.dataPath + "/Data/social_interaction_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, socialInteractionData);
        stream.Close();
    }
    
    public void LoadSocialInteractionData()
    {
        string path = Application.dataPath + "/Data/social_interaction_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            socialInteractionData = (SocialInteractionDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            socialInteractionData = new SocialInteractionDetailedManagerData();
        }
    }
}
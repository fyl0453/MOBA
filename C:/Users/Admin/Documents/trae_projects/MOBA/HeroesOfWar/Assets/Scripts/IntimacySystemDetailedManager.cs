using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class IntimacySystemDetailedManager : MonoBehaviour
{
    public static IntimacySystemDetailedManager Instance { get; private set; }
    
    public IntimacySystemDetailedManagerData intimacyData;
    
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
        LoadIntimacyData();
        
        if (intimacyData == null)
        {
            intimacyData = new IntimacySystemDetailedManagerData();
            InitializeDefaultIntimacySystem();
        }
    }
    
    private void InitializeDefaultIntimacySystem()
    {
        // 亲密关系等级
        IntimacyLevel level1 = new IntimacyLevel(1, "初识", "刚刚认识的好友", 0, "icon_level1");
        IntimacyLevel level2 = new IntimacyLevel(2, "熟悉", "逐渐熟悉的好友", 100, "icon_level2");
        IntimacyLevel level3 = new IntimacyLevel(3, "好友", "关系良好的好友", 200, "icon_level3");
        IntimacyLevel level4 = new IntimacyLevel(4, "密友", "亲密无间的好友", 300, "icon_level4");
        IntimacyLevel level5 = new IntimacyLevel(5, "死党", "生死与共的死党", 500, "icon_level5");
        IntimacyLevel level6 = new IntimacyLevel(6, "情侣", "甜蜜的情侣关系", 1000, "icon_level6");
        
        level1.AddReward("解锁基础互动");
        level2.AddReward("解锁专属头像框");
        level3.AddReward("解锁专属称号");
        level4.AddReward("解锁专属语音");
        level5.AddReward("解锁专属动作");
        level6.AddReward("解锁情侣专属特效");
        
        intimacyData.system.AddIntimacyLevel(level1);
        intimacyData.system.AddIntimacyLevel(level2);
        intimacyData.system.AddIntimacyLevel(level3);
        intimacyData.system.AddIntimacyLevel(level4);
        intimacyData.system.AddIntimacyLevel(level5);
        intimacyData.system.AddIntimacyLevel(level6);
        
        // 亲密关系
        IntimacyRelationship relationship1 = new IntimacyRelationship("relationship_001", "user_001", "张三", "user_002", "李四", "friend", false);
        IntimacyRelationship relationship2 = new IntimacyRelationship("relationship_002", "user_003", "王五", "user_004", "赵六", "couple", true, "甜蜜情侣");
        IntimacyRelationship relationship3 = new IntimacyRelationship("relationship_003", "user_001", "张三", "user_003", "王五", "friend", false);
        
        // 添加亲密度
        relationship1.AddIntimacyPoints(150);
        relationship2.AddIntimacyPoints(1200);
        relationship3.AddIntimacyPoints(80);
        
        intimacyData.system.AddIntimacyRelationship(relationship1);
        intimacyData.system.AddIntimacyRelationship(relationship2);
        intimacyData.system.AddIntimacyRelationship(relationship3);
        
        // 亲密互动
        IntimacyInteraction interaction1 = new IntimacyInteraction("interaction_001", "relationship_001", "user_001", "张三", "user_002", "李四", "gift", 10);
        IntimacyInteraction interaction2 = new IntimacyInteraction("interaction_002", "relationship_001", "user_002", "李四", "user_001", "张三", "play_together", 5);
        IntimacyInteraction interaction3 = new IntimacyInteraction("interaction_003", "relationship_002", "user_003", "王五", "user_004", "赵六", "gift", 20);
        
        intimacyData.system.AddIntimacyInteraction(interaction1);
        intimacyData.system.AddIntimacyInteraction(interaction2);
        intimacyData.system.AddIntimacyInteraction(interaction3);
        
        // 亲密事件
        IntimacyEvent event1 = new IntimacyEvent("event_001", "establish", "user_001", "user_002", "relationship_001", "建立好友关系");
        IntimacyEvent event2 = new IntimacyEvent("event_002", "establish", "user_003", "user_004", "relationship_002", "建立情侣关系");
        IntimacyEvent event3 = new IntimacyEvent("event_003", "intimacy_up", "user_002", "user_001", "relationship_001", "亲密度提升");
        
        intimacyData.system.AddIntimacyEvent(event1);
        intimacyData.system.AddIntimacyEvent(event2);
        intimacyData.system.AddIntimacyEvent(event3);
        
        SaveIntimacyData();
    }
    
    // 亲密关系管理
    public void EstablishRelationship(string user1ID, string user1Name, string user2ID, string user2Name, string relationshipType, bool isCouple, string coupleName = "")
    {
        // 检查是否已经存在关系
        List<IntimacyRelationship> relationships1 = intimacyData.system.GetIntimacyRelationshipsByUser(user1ID);
        IntimacyRelationship existingRelationship = relationships1.Find(r => r.user2ID == user2ID || r.user1ID == user2ID);
        
        if (existingRelationship == null)
        {
            string relationshipID = "relationship_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            IntimacyRelationship relationship = new IntimacyRelationship(relationshipID, user1ID, user1Name, user2ID, user2Name, relationshipType, isCouple, coupleName);
            intimacyData.system.AddIntimacyRelationship(relationship);
            
            // 创建亲密事件
            CreateIntimacyEvent("establish", user1ID, user2ID, relationshipID, "建立" + (isCouple ? "情侣" : "好友") + "关系");
            
            SaveIntimacyData();
            Debug.Log("成功建立" + (isCouple ? "情侣" : "好友") + "关系: " + user1Name + " 和 " + user2Name);
        }
        else
        {
            Debug.LogError("关系已经存在");
        }
    }
    
    public void BreakRelationship(string relationshipID, string userID)
    {
        IntimacyRelationship relationship = intimacyData.system.GetIntimacyRelationship(relationshipID);
        if (relationship != null && relationship.IsActive() && (relationship.user1ID == userID || relationship.user2ID == userID))
        {
            string targetID = relationship.user1ID == userID ? relationship.user2ID : relationship.user1ID;
            relationship.BreakRelationship();
            
            // 创建亲密事件
            CreateIntimacyEvent("break", userID, targetID, relationshipID, "解除" + (relationship.IsCouple() ? "情侣" : "好友") + "关系");
            
            SaveIntimacyData();
            Debug.Log("成功解除关系");
        }
        else
        {
            Debug.LogError("关系不存在或无权限");
        }
    }
    
    public List<IntimacyRelationship> GetUserRelationships(string userID)
    {
        return intimacyData.system.GetIntimacyRelationshipsByUser(userID);
    }
    
    public List<IntimacyRelationship> GetUserCoupleRelationships(string userID)
    {
        List<IntimacyRelationship> relationships = intimacyData.system.GetIntimacyRelationshipsByUser(userID);
        return relationships.FindAll(r => r.IsCouple() && r.IsActive());
    }
    
    // 亲密度管理
    public void AddIntimacyPoints(string relationshipID, string userID, int points, string interactionType)
    {
        IntimacyRelationship relationship = intimacyData.system.GetIntimacyRelationship(relationshipID);
        if (relationship != null && relationship.IsActive() && (relationship.user1ID == userID || relationship.user2ID == userID))
        {
            int oldLevel = relationship.intimacyLevel;
            relationship.AddIntimacyPoints(points);
            
            // 创建亲密互动
            string targetID = relationship.user1ID == userID ? relationship.user2ID : relationship.user1ID;
            string targetName = relationship.user1ID == userID ? relationship.user2Name : relationship.user1Name;
            string userName = relationship.user1ID == userID ? relationship.user1Name : relationship.user2Name;
            
            string interactionID = "interaction_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            IntimacyInteraction interaction = new IntimacyInteraction(interactionID, relationshipID, userID, userName, targetID, targetName, interactionType, points);
            intimacyData.system.AddIntimacyInteraction(interaction);
            
            // 检查是否升级
            if (relationship.intimacyLevel > oldLevel)
            {
                CreateIntimacyEvent("level_up", userID, targetID, relationshipID, "亲密度等级提升到" + relationship.intimacyLevel + "级");
            }
            else
            {
                CreateIntimacyEvent("intimacy_up", userID, targetID, relationshipID, "亲密度提升" + points + "点");
            }
            
            SaveIntimacyData();
            Debug.Log("成功添加亲密度: " + points + "点");
        }
        else
        {
            Debug.LogError("关系不存在或无权限");
        }
    }
    
    public void InteractWithFriend(string userID, string targetID, string interactionType)
    {
        // 查找关系
        List<IntimacyRelationship> relationships = intimacyData.system.GetIntimacyRelationshipsByUser(userID);
        IntimacyRelationship relationship = relationships.Find(r => (r.user1ID == targetID || r.user2ID == targetID) && r.IsActive());
        
        if (relationship != null)
        {
            // 根据互动类型添加不同的亲密度
            int points = 0;
            switch (interactionType)
            {
                case "gift":
                    points = 10;
                    break;
                case "play_together":
                    points = 5;
                    break;
                case "chat":
                    points = 1;
                    break;
                case "team_up":
                    points = 8;
                    break;
                default:
                    points = 0;
                    break;
            }
            
            AddIntimacyPoints(relationship.relationshipID, userID, points, interactionType);
        }
        else
        {
            Debug.LogError("关系不存在");
        }
    }
    
    // 亲密互动管理
    public List<IntimacyInteraction> GetRelationshipInteractions(string relationshipID)
    {
        return intimacyData.system.GetIntimacyInteractionsByRelationship(relationshipID);
    }
    
    // 亲密事件管理
    public string CreateIntimacyEvent(string eventType, string userID, string targetID, string relationshipID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        IntimacyEvent intimacyEvent = new IntimacyEvent(eventID, eventType, userID, targetID, relationshipID, description);
        intimacyData.system.AddIntimacyEvent(intimacyEvent);
        SaveIntimacyData();
        Debug.Log("成功创建亲密事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        IntimacyEvent intimacyEvent = intimacyData.system.GetIntimacyEvent(eventID);
        if (intimacyEvent != null)
        {
            intimacyEvent.MarkAsCompleted();
            SaveIntimacyData();
            Debug.Log("成功标记亲密事件为完成");
        }
        else
        {
            Debug.LogError("亲密事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        IntimacyEvent intimacyEvent = intimacyData.system.GetIntimacyEvent(eventID);
        if (intimacyEvent != null)
        {
            intimacyEvent.MarkAsFailed();
            SaveIntimacyData();
            Debug.Log("成功标记亲密事件为失败");
        }
        else
        {
            Debug.LogError("亲密事件不存在: " + eventID);
        }
    }
    
    public List<IntimacyEvent> GetUserEvents(string userID)
    {
        return intimacyData.system.GetIntimacyEventsByUser(userID);
    }
    
    // 亲密等级管理
    public IntimacyLevel GetIntimacyLevel(int level)
    {
        return intimacyData.system.GetIntimacyLevel(level);
    }
    
    public int GetMaxIntimacyLevel()
    {
        return intimacyData.system.intimacyLevels.Count;
    }
    
    // 数据持久化
    public void SaveIntimacyData()
    {
        string path = Application.dataPath + "/Data/intimacy_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, intimacyData);
        stream.Close();
    }
    
    public void LoadIntimacyData()
    {
        string path = Application.dataPath + "/Data/intimacy_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            intimacyData = (IntimacySystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            intimacyData = new IntimacySystemDetailedManagerData();
        }
    }
}
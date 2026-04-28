using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class QuickMessage
{
    public string MessageID;
    public string Content;
    public string Category;
    public string Language;
    public bool IsPreset;
    public bool IsEnabled;
    public int UsageCount;
    public DateTime LastUsedTime;

    public QuickMessage(string content, string category, string language, bool isPreset)
    {
        MessageID = "message_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Content = content;
        Category = category;
        Language = language;
        IsPreset = isPreset;
        IsEnabled = true;
        UsageCount = 0;
        LastUsedTime = DateTime.MinValue;
    }
}

[Serializable]
public class MessageCategory
{
    public string CategoryID;
    public string CategoryName;
    public string Description;
    public int Priority;
    public bool IsEnabled;

    public MessageCategory(string categoryName, string description, int priority)
    {
        CategoryID = "category_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CategoryName = categoryName;
        Description = description;
        Priority = priority;
        IsEnabled = true;
    }
}

[Serializable]
public class PlayerMessageData
{
    public string PlayerID;
    public List<QuickMessage> CustomMessages;
    public List<string> FavoriteMessageIDs;
    public List<QuickMessage> RecentMessages;
    public int TotalMessagesSent;
    public DateTime LastMessageTime;
    public bool QuickMessageEnabled;
    public int MaxCustomMessages;

    public PlayerMessageData(string playerID)
    {
        PlayerID = playerID;
        CustomMessages = new List<QuickMessage>();
        FavoriteMessageIDs = new List<string>();
        RecentMessages = new List<QuickMessage>();
        TotalMessagesSent = 0;
        LastMessageTime = DateTime.MinValue;
        QuickMessageEnabled = true;
        MaxCustomMessages = 10;
    }
}

[Serializable]
public class QuickMessageSystemData
{
    public Dictionary<string, PlayerMessageData> PlayerMessageData;
    public List<QuickMessage> PresetMessages;
    public List<MessageCategory> MessageCategories;
    public List<string> AvailableLanguages;
    public int MaxRecentMessages;
    public int MaxFavoriteMessages;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public QuickMessageSystemData()
    {
        PlayerMessageData = new Dictionary<string, PlayerMessageData>();
        PresetMessages = new List<QuickMessage>();
        MessageCategories = new List<MessageCategory>();
        AvailableLanguages = new List<string> { "zh-CN", "en-US", "ja-JP", "ko-KR" };
        MaxRecentMessages = 20;
        MaxFavoriteMessages = 10;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
        InitializePresetMessages();
        InitializeCategories();
    }

    private void InitializePresetMessages()
    {
        AddPresetMessage("准备就绪", "战斗", "zh-CN");
        AddPresetMessage("发起进攻", "战斗", "zh-CN");
        AddPresetMessage("撤退", "战斗", "zh-CN");
        AddPresetMessage("请求支援", "战斗", "zh-CN");
        AddPresetMessage("谢谢", "社交", "zh-CN");
        AddPresetMessage("抱歉", "社交", "zh-CN");
        AddPresetMessage("干得漂亮", "社交", "zh-CN");
        AddPresetMessage("敌人不见了", "战斗", "zh-CN");
        AddPresetMessage("集合团战", "战斗", "zh-CN");
        AddPresetMessage("推塔", "战略", "zh-CN");
        AddPresetMessage("拿龙", "战略", "zh-CN");
        AddPresetMessage("防守", "战略", "zh-CN");
    }

    private void InitializeCategories()
    {
        AddCategory("战斗", "战斗相关的快捷消息", 1);
        AddCategory("战略", "战略相关的快捷消息", 2);
        AddCategory("社交", "社交相关的快捷消息", 3);
        AddCategory("自定义", "玩家自定义的快捷消息", 4);
    }

    private void AddPresetMessage(string content, string category, string language)
    {
        QuickMessage message = new QuickMessage(content, category, language, true);
        PresetMessages.Add(message);
    }

    private void AddCategory(string name, string description, int priority)
    {
        MessageCategory category = new MessageCategory(name, description, priority);
        MessageCategories.Add(category);
    }

    public void AddPlayerMessageData(string playerID, PlayerMessageData messageData)
    {
        PlayerMessageData[playerID] = messageData;
    }
}

[Serializable]
public class MessageEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string MessageID;
    public string EventData;

    public MessageEvent(string eventID, string eventType, string playerID, string messageID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        MessageID = messageID;
        EventData = eventData;
    }
}

public class QuickMessageSystemDataManager
{
    private static QuickMessageSystemDataManager _instance;
    public static QuickMessageSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new QuickMessageSystemDataManager();
            }
            return _instance;
        }
    }

    public QuickMessageSystemData messageData;
    private List<MessageEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private QuickMessageSystemDataManager()
    {
        messageData = new QuickMessageSystemData();
        recentEvents = new List<MessageEvent>();
        LoadMessageData();
    }

    public void SaveMessageData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "QuickMessageSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, messageData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存快捷消息系统数据失败: " + e.Message);
        }
    }

    public void LoadMessageData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "QuickMessageSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    messageData = (QuickMessageSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载快捷消息系统数据失败: " + e.Message);
            messageData = new QuickMessageSystemData();
        }
    }

    public void CreateMessageEvent(string eventType, string playerID, string messageID, string eventData)
    {
        string eventID = "message_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MessageEvent messageEvent = new MessageEvent(eventID, eventType, playerID, messageID, eventData);
        recentEvents.Add(messageEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<MessageEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
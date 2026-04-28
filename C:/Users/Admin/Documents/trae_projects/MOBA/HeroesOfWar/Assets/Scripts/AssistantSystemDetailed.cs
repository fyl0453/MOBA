using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class GameTip
{
    public string TipID;
    public string TipTitle;
    public string TipContent;
    public int TipType;
    public string RelatedHeroID;
    public string RelatedHeroName;
    public string RelatedItemID;
    public int Priority;
    public DateTime CreateTime;
    public bool IsActive;

    public GameTip(string tipID, string tipTitle, string tipContent, int tipType)
    {
        TipID = tipID;
        TipTitle = tipTitle;
        TipContent = tipContent;
        TipType = tipType;
        RelatedHeroID = "";
        RelatedHeroName = "";
        RelatedItemID = "";
        Priority = 0;
        CreateTime = DateTime.Now;
        IsActive = true;
    }
}

[Serializable]
public class TacticalSuggestion
{
    public string SuggestionID;
    public string HeroID;
    public string SuggestionType;
    public string Title;
    public string Content;
    public string Timing;
    public int Effectiveness;
    public string Scenario;

    public TacticalSuggestion(string suggestionID, string heroID, string suggestionType, string title, string content, string timing, string scenario)
    {
        SuggestionID = suggestionID;
        HeroID = heroID;
        SuggestionType = suggestionType;
        Title = title;
        Content = content;
        Timing = timing;
        Effectiveness = 0;
        Scenario = scenario;
    }
}

[Serializable]
public class PlayerAssistantConfig
{
    public string PlayerID;
    public bool TipsEnabled;
    public bool VoiceHintsEnabled;
    public bool AutoBuyEnabled;
    public bool MapAwarenessEnabled;
    public int TipDisplayDuration;
    public int SuggestionFrequency;
    public List<string> DisabledTipCategories;
    public Dictionary<string, bool> HeroSpecificSettings;

    public PlayerAssistantConfig(string playerID)
    {
        PlayerID = playerID;
        TipsEnabled = true;
        VoiceHintsEnabled = true;
        AutoBuyEnabled = false;
        MapAwarenessEnabled = true;
        TipDisplayDuration = 5;
        SuggestionFrequency = 3;
        DisabledTipCategories = new List<string>();
        HeroSpecificSettings = new Dictionary<string, bool>();
    }
}

[Serializable]
public class InGameReminder
{
    public string ReminderID;
    public string PlayerID;
    public string ReminderType;
    public string Title;
    public string Content;
    public DateTime TriggerTime;
    public bool IsTriggered;
    public int Priority;

    public InGameReminder(string reminderID, string playerID, string reminderType, string title, string content, DateTime triggerTime, int priority)
    {
        ReminderID = reminderID;
        PlayerID = playerID;
        ReminderType = reminderType;
        Title = title;
        Content = content;
        TriggerTime = triggerTime;
        IsTriggered = false;
        Priority = priority;
    }
}

[Serializable]
public class HeroCounter
{
    public string CounterID;
    public string HeroID;
    public string CounterHeroID;
    public string CounterHeroName;
    public string CounterReason;
    public int Effectiveness;
    public List<string> Tips;

    public HeroCounter(string counterID, string heroID, string counterHeroID, string counterHeroName, string counterReason, int effectiveness)
    {
        CounterID = counterID;
        HeroID = heroID;
        CounterHeroID = counterHeroID;
        CounterHeroName = counterHeroName;
        CounterReason = counterReason;
        Effectiveness = effectiveness;
        Tips = new List<string>();
    }
}

[Serializable]
public class AssistantSystemData
{
    public List<GameTip> AllTips;
    public List<TacticalSuggestion> AllSuggestions;
    public List<HeroCounter> AllCounters;
    public Dictionary<string, PlayerAssistantConfig> PlayerConfigs;
    public List<InGameReminder> ActiveReminders;
    public Dictionary<string, List<string>> PlayerTipHistory;
    public List<string> FeaturedTipIDs;
    public DateTime LastCleanupTime;

    public AssistantSystemData()
    {
        AllTips = new List<GameTip>();
        AllSuggestions = new List<TacticalSuggestion>();
        AllCounters = new List<HeroCounter>();
        PlayerConfigs = new Dictionary<string, PlayerAssistantConfig>();
        ActiveReminders = new List<InGameReminder>();
        PlayerTipHistory = new Dictionary<string, List<string>>();
        FeaturedTipIDs = new List<string>();
        LastCleanupTime = DateTime.Now;
        InitializeDefaultTips();
    }

    private void InitializeDefaultTips()
    {
        AddTip(new GameTip("tip_001", "野区刷新", "野区将在10秒后刷新，记得击杀红蓝BUFF", 1));
        AddTip(new GameTip("tip_002", "小龙刷新", "小龙将在15秒后刷新，请做好准备", 2));
        AddTip(new GameTip("tip_003", "主宰刷新", "主宰将在30秒后刷新，建议集合团战", 2));
        AddTip(new GameTip("tip_004", "兵线推进", "当前兵线不利于我方，建议清理兵线后再团", 3));
        AddTip(new GameTip("tip_005", "装备购买", "建议购买防御装备提高生存能力", 4));
    }

    private void AddTip(GameTip tip)
    {
        AllTips.Add(tip);
        if (!FeaturedTipIDs.Contains(tip.TipID) && tip.Priority >= 3)
        {
            FeaturedTipIDs.Add(tip.TipID);
        }
    }

    public void AddSuggestion(TacticalSuggestion suggestion)
    {
        AllSuggestions.Add(suggestion);
    }

    public void AddCounter(HeroCounter counter)
    {
        AllCounters.Add(counter);
    }

    public void AddReminder(InGameReminder reminder)
    {
        ActiveReminders.Add(reminder);
    }
}

[Serializable]
public class AssistantEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EventData;

    public AssistantEvent(string eventID, string eventType, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class AssistantSystemDataManager
{
    private static AssistantSystemDataManager _instance;
    public static AssistantSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AssistantSystemDataManager();
            }
            return _instance;
        }
    }

    public AssistantSystemData assistantData;
    private List<AssistantEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private AssistantSystemDataManager()
    {
        assistantData = new AssistantSystemData();
        recentEvents = new List<AssistantEvent>();
        LoadAssistantData();
    }

    public void SaveAssistantData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "AssistantSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, assistantData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存游戏助手数据失败: " + e.Message);
        }
    }

    public void LoadAssistantData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "AssistantSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    assistantData = (AssistantSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载游戏助手数据失败: " + e.Message);
            assistantData = new AssistantSystemData();
        }
    }

    public void CreateAssistantEvent(string eventType, string playerID, string eventData)
    {
        string eventID = "assistant_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AssistantEvent assistantEvent = new AssistantEvent(eventID, eventType, playerID, eventData);
        recentEvents.Add(assistantEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<AssistantEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
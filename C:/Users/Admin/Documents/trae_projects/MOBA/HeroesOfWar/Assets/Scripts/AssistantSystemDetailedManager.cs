using System;
using System.Collections.Generic;

public class AssistantSystemDetailedManager
{
    private static AssistantSystemDetailedManager _instance;
    public static AssistantSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AssistantSystemDetailedManager();
            }
            return _instance;
        }
    }

    private AssistantSystemData assistantData;
    private AssistantSystemDataManager dataManager;

    private AssistantSystemDetailedManager()
    {
        dataManager = AssistantSystemDataManager.Instance;
        assistantData = dataManager.assistantData;
    }

    public void InitializePlayerConfig(string playerID)
    {
        if (!assistantData.PlayerConfigs.ContainsKey(playerID))
        {
            assistantData.PlayerConfigs[playerID] = new PlayerAssistantConfig(playerID);
            dataManager.CreateAssistantEvent("config_init", playerID, "初始化助手配置");
            dataManager.SaveAssistantData();
            Debug.Log("初始化玩家助手配置: " + playerID);
        }
    }

    public void CreateTip(string tipTitle, string tipContent, int tipType, string relatedHeroID = "", string relatedHeroName = "", string relatedItemID = "", int priority = 0)
    {
        string tipID = "tip_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GameTip tip = new GameTip(tipID, tipTitle, tipContent, tipType);
        tip.RelatedHeroID = relatedHeroID;
        tip.RelatedHeroName = relatedHeroName;
        tip.RelatedItemID = relatedItemID;
        tip.Priority = priority;
        assistantData.AllTips.Add(tip);
        dataManager.CreateAssistantEvent("tip_create", "", "创建提示: " + tipTitle);
        dataManager.SaveAssistantData();
        Debug.Log("创建游戏提示: " + tipTitle);
    }

    public void CreateTacticalSuggestion(string heroID, string suggestionType, string title, string content, string timing, string scenario)
    {
        string suggestionID = "suggestion_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TacticalSuggestion suggestion = new TacticalSuggestion(suggestionID, heroID, suggestionType, title, content, timing, scenario);
        assistantData.AddSuggestion(suggestion);
        dataManager.CreateAssistantEvent("suggestion_create", "", "创建战术建议: " + title);
        dataManager.SaveAssistantData();
        Debug.Log("创建战术建议: " + title);
    }

    public void CreateHeroCounter(string heroID, string counterHeroID, string counterHeroName, string counterReason, int effectiveness, List<string> tips)
    {
        string counterID = "counter_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HeroCounter counter = new HeroCounter(counterID, heroID, counterHeroID, counterHeroName, counterReason, effectiveness);
        counter.Tips = tips;
        assistantData.AddCounter(counter);
        dataManager.CreateAssistantEvent("counter_create", "", "创建克制关系: " + counterHeroName);
        dataManager.SaveAssistantData();
        Debug.Log("创建英雄克制: " + counterHeroName);
    }

    public void CreateReminder(string playerID, string reminderType, string title, string content, DateTime triggerTime, int priority = 0)
    {
        string reminderID = "reminder_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        InGameReminder reminder = new InGameReminder(reminderID, playerID, reminderType, title, content, triggerTime, priority);
        assistantData.AddReminder(reminder);
        dataManager.CreateAssistantEvent("reminder_create", playerID, "创建提醒: " + title);
        dataManager.SaveAssistantData();
        Debug.Log("创建游戏提醒: " + title);
    }

    public GameTip GetTip(string tipID)
    {
        return assistantData.AllTips.Find(t => t.TipID == tipID);
    }

    public List<GameTip> GetActiveTips()
    {
        return assistantData.AllTips.FindAll(t => t.IsActive);
    }

    public List<GameTip> GetTipsByType(int tipType)
    {
        return assistantData.AllTips.FindAll(t => t.TipType == tipType && t.IsActive);
    }

    public List<GameTip> GetTipsByHero(string heroID)
    {
        return assistantData.AllTips.FindAll(t => t.RelatedHeroID == heroID && t.IsActive);
    }

    public List<GameTip> GetFeaturedTips()
    {
        return assistantData.AllTips.FindAll(t => t.Priority >= 3 && t.IsActive);
    }

    public List<GameTip> GetTipsByPriority(int minPriority)
    {
        return assistantData.AllTips.FindAll(t => t.Priority >= minPriority && t.IsActive);
    }

    public TacticalSuggestion GetSuggestion(string suggestionID)
    {
        return assistantData.AllSuggestions.Find(s => s.SuggestionID == suggestionID);
    }

    public List<TacticalSuggestion> GetHeroSuggestions(string heroID)
    {
        return assistantData.AllSuggestions.FindAll(s => s.HeroID == heroID);
    }

    public List<TacticalSuggestion> GetSuggestionsByType(string suggestionType)
    {
        return assistantData.AllSuggestions.FindAll(s => s.SuggestionType == suggestionType);
    }

    public List<TacticalSuggestion> GetSuggestionsByScenario(string scenario)
    {
        return assistantData.AllSuggestions.FindAll(s => s.Scenario == scenario);
    }

    public List<HeroCounter> GetHeroCounters(string heroID)
    {
        return assistantData.AllCounters.FindAll(c => c.HeroID == heroID);
    }

    public List<HeroCounter> GetCountersByEffectiveness(string heroID, int minEffectiveness)
    {
        return assistantData.AllCounters.FindAll(c => c.HeroID == heroID && c.Effectiveness >= minEffectiveness);
    }

    public List<InGameReminder> GetPlayerReminders(string playerID)
    {
        return assistantData.ActiveReminders.FindAll(r => r.PlayerID == playerID && !r.IsTriggered);
    }

    public List<InGameReminder> GetTriggeredReminders(string playerID)
    {
        return assistantData.ActiveReminders.FindAll(r => r.PlayerID == playerID && r.IsTriggered);
    }

    public void TriggerReminder(string reminderID)
    {
        InGameReminder reminder = assistantData.ActiveReminders.Find(r => r.ReminderID == reminderID);
        if (reminder != null)
        {
            reminder.IsTriggered = true;
            dataManager.CreateAssistantEvent("reminder_trigger", reminder.PlayerID, "触发提醒: " + reminder.Title);
            dataManager.SaveAssistantData();
            Debug.Log("触发提醒: " + reminder.Title);
        }
    }

    public void DismissReminder(string reminderID)
    {
        InGameReminder reminder = assistantData.ActiveReminders.Find(r => r.ReminderID == reminderID);
        if (reminder != null)
        {
            assistantData.ActiveReminders.Remove(reminder);
            dataManager.CreateAssistantEvent("reminder_dismiss", reminder.PlayerID, "忽略提醒: " + reminder.Title);
            dataManager.SaveAssistantData();
            Debug.Log("忽略提醒: " + reminder.Title);
        }
    }

    public PlayerAssistantConfig GetPlayerConfig(string playerID)
    {
        if (assistantData.PlayerConfigs.ContainsKey(playerID))
        {
            return assistantData.PlayerConfigs[playerID];
        }
        return null;
    }

    public void UpdatePlayerConfig(string playerID, bool tipsEnabled, bool voiceHintsEnabled, bool autoBuyEnabled, bool mapAwarenessEnabled)
    {
        if (assistantData.PlayerConfigs.ContainsKey(playerID))
        {
            PlayerAssistantConfig config = assistantData.PlayerConfigs[playerID];
            config.TipsEnabled = tipsEnabled;
            config.VoiceHintsEnabled = voiceHintsEnabled;
            config.AutoBuyEnabled = autoBuyEnabled;
            config.MapAwarenessEnabled = mapAwarenessEnabled;
            dataManager.CreateAssistantEvent("config_update", playerID, "更新助手配置");
            dataManager.SaveAssistantData();
            Debug.Log("更新助手配置成功");
        }
    }

    public void SetTipDisplayDuration(string playerID, int duration)
    {
        if (assistantData.PlayerConfigs.ContainsKey(playerID))
        {
            assistantData.PlayerConfigs[playerID].TipDisplayDuration = duration;
            dataManager.SaveAssistantData();
        }
    }

    public void SetSuggestionFrequency(string playerID, int frequency)
    {
        if (assistantData.PlayerConfigs.ContainsKey(playerID))
        {
            assistantData.PlayerConfigs[playerID].SuggestionFrequency = frequency;
            dataManager.SaveAssistantData();
        }
    }

    public void DisableTipCategory(string playerID, string category)
    {
        if (assistantData.PlayerConfigs.ContainsKey(playerID))
        {
            if (!assistantData.PlayerConfigs[playerID].DisabledTipCategories.Contains(category))
            {
                assistantData.PlayerConfigs[playerID].DisabledTipCategories.Add(category);
                dataManager.SaveAssistantData();
            }
        }
    }

    public void EnableTipCategory(string playerID, string category)
    {
        if (assistantData.PlayerConfigs.ContainsKey(playerID))
        {
            assistantData.PlayerConfigs[playerID].DisabledTipCategories.Remove(category);
            dataManager.SaveAssistantData();
        }
    }

    public List<GameTip> GetPersonalizedTips(string playerID, string currentHeroID)
    {
        List<GameTip> tips = new List<GameTip>();
        PlayerAssistantConfig config = GetPlayerConfig(playerID);
        if (config == null)
        {
            InitializePlayerConfig(playerID);
            config = GetPlayerConfig(playerID);
        }

        foreach (GameTip tip in assistantData.AllTips)
        {
            if (!tip.IsActive) continue;
            if (config.DisabledTipCategories.Contains(tip.TipType.ToString())) continue;
            if (tip.RelatedHeroID == currentHeroID || string.IsNullOrEmpty(tip.RelatedHeroID))
            {
                tips.Add(tip);
            }
        }

        tips.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        return tips;
    }

    public void RecordTipView(string playerID, string tipID)
    {
        if (!assistantData.PlayerTipHistory.ContainsKey(playerID))
        {
            assistantData.PlayerTipHistory[playerID] = new List<string>();
        }
        if (!assistantData.PlayerTipHistory[playerID].Contains(tipID))
        {
            assistantData.PlayerTipHistory[playerID].Add(tipID);
            dataManager.SaveAssistantData();
        }
    }

    public List<string> GetPlayerViewedTips(string playerID)
    {
        if (assistantData.PlayerTipHistory.ContainsKey(playerID))
        {
            return assistantData.PlayerTipHistory[playerID];
        }
        return new List<string>();
    }

    public void UpdateSuggestionEffectiveness(string suggestionID, int effectiveness)
    {
        TacticalSuggestion suggestion = assistantData.AllSuggestions.Find(s => s.SuggestionID == suggestionID);
        if (suggestion != null)
        {
            suggestion.Effectiveness = effectiveness;
            dataManager.SaveAssistantData();
        }
    }

    public void ClearExpiredReminders()
    {
        DateTime now = DateTime.Now;
        assistantData.ActiveReminders.RemoveAll(r => r.IsTriggered && (now - r.TriggerTime).TotalDays > 7);
        dataManager.SaveAssistantData();
        Debug.Log("清理过期提醒完成");
    }

    public List<AssistantEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveAssistantData();
    }

    public void LoadData()
    {
        dataManager.LoadAssistantData();
    }
}
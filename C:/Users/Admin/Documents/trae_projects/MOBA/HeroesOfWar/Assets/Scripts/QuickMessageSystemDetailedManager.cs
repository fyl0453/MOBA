using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickMessageSystemDetailedManager
{
    private static QuickMessageSystemDetailedManager _instance;
    public static QuickMessageSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new QuickMessageSystemDetailedManager();
            }
            return _instance;
        }
    }

    private QuickMessageSystemData messageData;
    private QuickMessageSystemDataManager dataManager;

    private QuickMessageSystemDetailedManager()
    {
        dataManager = QuickMessageSystemDataManager.Instance;
        messageData = dataManager.messageData;
    }

    public void InitializePlayerMessageData(string playerID)
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            PlayerMessageData playerMessageData = new PlayerMessageData(playerID);
            messageData.AddPlayerMessageData(playerID, playerMessageData);
            dataManager.SaveMessageData();
            Debug.Log("初始化快捷消息数据成功");
        }
    }

    public void SendQuickMessage(string playerID, string messageID, string matchID, string recipientID = "")
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            InitializePlayerMessageData(playerID);
        }
        
        PlayerMessageData playerMessageData = messageData.PlayerMessageData[playerID];
        if (!playerMessageData.QuickMessageEnabled)
        {
            Debug.LogError("快捷消息功能已禁用");
            return;
        }
        
        QuickMessage message = FindMessageByID(messageID);
        if (message != null)
        {
            message.UsageCount++;
            message.LastUsedTime = DateTime.Now;
            
            playerMessageData.TotalMessagesSent++;
            playerMessageData.LastMessageTime = DateTime.Now;
            
            AddToRecentMessages(playerID, message);
            
            dataManager.CreateMessageEvent("message_send", playerID, messageID, "发送快捷消息: " + message.Content);
            dataManager.SaveMessageData();
            Debug.Log("发送快捷消息成功: " + message.Content);
        }
    }

    public string CreateCustomMessage(string playerID, string content, string category, string language)
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            InitializePlayerMessageData(playerID);
        }
        
        PlayerMessageData playerMessageData = messageData.PlayerMessageData[playerID];
        if (playerMessageData.CustomMessages.Count >= playerMessageData.MaxCustomMessages)
        {
            Debug.LogError("自定义快捷消息数量达到上限");
            return "";
        }
        
        if (content.Length > 50)
        {
            Debug.LogError("快捷消息内容过长");
            return "";
        }
        
        QuickMessage message = new QuickMessage(content, category, language, false);
        playerMessageData.CustomMessages.Add(message);
        
        dataManager.CreateMessageEvent("message_create", playerID, message.MessageID, "创建自定义快捷消息: " + content);
        dataManager.SaveMessageData();
        Debug.Log("创建自定义快捷消息成功: " + content);
        return message.MessageID;
    }

    public void UpdateCustomMessage(string playerID, string messageID, string newContent)
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerMessageData playerMessageData = messageData.PlayerMessageData[playerID];
        QuickMessage message = playerMessageData.CustomMessages.Find(m => m.MessageID == messageID);
        if (message != null && !message.IsPreset)
        {
            if (newContent.Length > 50)
            {
                Debug.LogError("快捷消息内容过长");
                return;
            }
            
            message.Content = newContent;
            
            dataManager.CreateMessageEvent("message_update", playerID, messageID, "更新自定义快捷消息: " + newContent);
            dataManager.SaveMessageData();
            Debug.Log("更新自定义快捷消息成功: " + newContent);
        }
    }

    public void DeleteCustomMessage(string playerID, string messageID)
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerMessageData playerMessageData = messageData.PlayerMessageData[playerID];
        QuickMessage message = playerMessageData.CustomMessages.Find(m => m.MessageID == messageID);
        if (message != null && !message.IsPreset)
        {
            playerMessageData.CustomMessages.Remove(message);
            playerMessageData.FavoriteMessageIDs.Remove(messageID);
            playerMessageData.RecentMessages.RemoveAll(m => m.MessageID == messageID);
            
            dataManager.CreateMessageEvent("message_delete", playerID, messageID, "删除自定义快捷消息: " + message.Content);
            dataManager.SaveMessageData();
            Debug.Log("删除自定义快捷消息成功: " + message.Content);
        }
    }

    public void ToggleFavoriteMessage(string playerID, string messageID)
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            InitializePlayerMessageData(playerID);
        }
        
        PlayerMessageData playerMessageData = messageData.PlayerMessageData[playerID];
        if (playerMessageData.FavoriteMessageIDs.Contains(messageID))
        {
            playerMessageData.FavoriteMessageIDs.Remove(messageID);
            dataManager.CreateMessageEvent("message_unfavorite", playerID, messageID, "取消收藏快捷消息");
            Debug.Log("取消收藏快捷消息成功");
        }
        else
        {
            if (playerMessageData.FavoriteMessageIDs.Count >= messageData.MaxFavoriteMessages)
            {
                Debug.LogError("收藏快捷消息数量达到上限");
                return;
            }
            
            playerMessageData.FavoriteMessageIDs.Add(messageID);
            dataManager.CreateMessageEvent("message_favorite", playerID, messageID, "收藏快捷消息");
            Debug.Log("收藏快捷消息成功");
        }
        
        dataManager.SaveMessageData();
    }

    public void ToggleMessageEnabled(string messageID, bool enabled)
    {
        QuickMessage message = FindMessageByID(messageID);
        if (message != null)
        {
            message.IsEnabled = enabled;
            dataManager.SaveMessageData();
            Debug.Log("快捷消息" + (enabled ? "已启用" : "已禁用") + ": " + message.Content);
        }
    }

    public void ToggleQuickMessageEnabled(string playerID, bool enabled)
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            InitializePlayerMessageData(playerID);
        }
        
        messageData.PlayerMessageData[playerID].QuickMessageEnabled = enabled;
        
        dataManager.CreateMessageEvent("message_system_toggle", playerID, "", "快捷消息系统" + (enabled ? "已开启" : "已关闭"));
        dataManager.SaveMessageData();
        Debug.Log("快捷消息系统" + (enabled ? "已开启" : "已关闭"));
    }

    public List<QuickMessage> GetPresetMessages(string category = "", string language = "zh-CN")
    {
        List<QuickMessage> messages = messageData.PresetMessages.FindAll(m => m.IsEnabled && m.Language == language);
        if (!string.IsNullOrEmpty(category))
        {
            messages = messages.FindAll(m => m.Category == category);
        }
        return messages;
    }

    public List<QuickMessage> GetCustomMessages(string playerID, string category = "")
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            InitializePlayerMessageData(playerID);
        }
        
        List<QuickMessage> messages = messageData.PlayerMessageData[playerID].CustomMessages.FindAll(m => m.IsEnabled);
        if (!string.IsNullOrEmpty(category))
        {
            messages = messages.FindAll(m => m.Category == category);
        }
        return messages;
    }

    public List<QuickMessage> GetFavoriteMessages(string playerID)
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            InitializePlayerMessageData(playerID);
        }
        
        List<QuickMessage> favoriteMessages = new List<QuickMessage>();
        PlayerMessageData playerMessageData = messageData.PlayerMessageData[playerID];
        
        foreach (string messageID in playerMessageData.FavoriteMessageIDs)
        {
            QuickMessage message = FindMessageByID(messageID);
            if (message != null && message.IsEnabled)
            {
                favoriteMessages.Add(message);
            }
        }
        
        return favoriteMessages;
    }

    public List<QuickMessage> GetRecentMessages(string playerID, int count = 10)
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            InitializePlayerMessageData(playerID);
        }
        
        List<QuickMessage> recentMessages = messageData.PlayerMessageData[playerID].RecentMessages;
        if (count < recentMessages.Count)
        {
            recentMessages = recentMessages.GetRange(0, count);
        }
        return recentMessages;
    }

    public List<QuickMessage> GetAllMessages(string playerID, string category = "", string language = "zh-CN")
    {
        List<QuickMessage> allMessages = new List<QuickMessage>();
        
        allMessages.AddRange(GetPresetMessages(category, language));
        allMessages.AddRange(GetCustomMessages(playerID, category));
        
        return allMessages;
    }

    public List<MessageCategory> GetMessageCategories()
    {
        return messageData.MessageCategories.FindAll(c => c.IsEnabled);
    }

    public List<string> GetAvailableLanguages()
    {
        return messageData.AvailableLanguages;
    }

    public QuickMessage GetMessage(string messageID)
    {
        return FindMessageByID(messageID);
    }

    public PlayerMessageData GetPlayerMessageData(string playerID)
    {
        if (!messageData.PlayerMessageData.ContainsKey(playerID))
        {
            InitializePlayerMessageData(playerID);
        }
        return messageData.PlayerMessageData[playerID];
    }

    private QuickMessage FindMessageByID(string messageID)
    {
        QuickMessage message = messageData.PresetMessages.Find(m => m.MessageID == messageID);
        if (message != null)
        {
            return message;
        }
        
        foreach (PlayerMessageData playerMessageData in messageData.PlayerMessageData.Values)
        {
            message = playerMessageData.CustomMessages.Find(m => m.MessageID == messageID);
            if (message != null)
            {
                return message;
            }
        }
        
        return null;
    }

    private void AddToRecentMessages(string playerID, QuickMessage message)
    {
        PlayerMessageData playerMessageData = messageData.PlayerMessageData[playerID];
        
        playerMessageData.RecentMessages.RemoveAll(m => m.MessageID == message.MessageID);
        playerMessageData.RecentMessages.Insert(0, message);
        
        if (playerMessageData.RecentMessages.Count > messageData.MaxRecentMessages)
        {
            playerMessageData.RecentMessages.RemoveAt(playerMessageData.RecentMessages.Count - 1);
        }
    }

    public void AddLanguage(string languageCode)
    {
        if (!messageData.AvailableLanguages.Contains(languageCode))
        {
            messageData.AvailableLanguages.Add(languageCode);
            dataManager.SaveMessageData();
            Debug.Log("添加语言成功: " + languageCode);
        }
    }

    public void RemoveLanguage(string languageCode)
    {
        if (languageCode != "zh-CN" && messageData.AvailableLanguages.Contains(languageCode))
        {
            messageData.AvailableLanguages.Remove(languageCode);
            dataManager.SaveMessageData();
            Debug.Log("删除语言成功: " + languageCode);
        }
    }

    public void AddCategory(string categoryName, string description, int priority)
    {
        MessageCategory category = new MessageCategory(categoryName, description, priority);
        messageData.MessageCategories.Add(category);
        dataManager.SaveMessageData();
        Debug.Log("添加消息分类成功: " + categoryName);
    }

    public void RemoveCategory(string categoryID)
    {
        MessageCategory category = messageData.MessageCategories.Find(c => c.CategoryID == categoryID);
        if (category != null && category.CategoryName != "战斗" && category.CategoryName != "战略" && category.CategoryName != "社交")
        {
            messageData.MessageCategories.Remove(category);
            dataManager.SaveMessageData();
            Debug.Log("删除消息分类成功: " + category.CategoryName);
        }
    }

    public void CleanupOldMessages(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        int totalCleaned = 0;
        
        foreach (PlayerMessageData playerMessageData in messageData.PlayerMessageData.Values)
        {
            List<QuickMessage> oldMessages = playerMessageData.RecentMessages.FindAll(m => m.LastUsedTime < cutoffDate);
            foreach (QuickMessage message in oldMessages)
            {
                playerMessageData.RecentMessages.Remove(message);
                totalCleaned++;
            }
        }
        
        if (totalCleaned > 0)
        {
            dataManager.CreateMessageEvent("message_cleanup", "system", "", "清理旧快捷消息: " + totalCleaned);
            dataManager.SaveMessageData();
            Debug.Log("清理旧快捷消息成功: " + totalCleaned);
        }
    }

    public void SaveData()
    {
        dataManager.SaveMessageData();
    }

    public void LoadData()
    {
        dataManager.LoadMessageData();
    }

    public List<MessageEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class DanmakuSystemDetailedManager
{
    private static DanmakuSystemDetailedManager _instance;
    public static DanmakuSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DanmakuSystemDetailedManager();
            }
            return _instance;
        }
    }

    private DanmakuSystemData danmakuData;
    private DanmakuSystemDataManager dataManager;

    private DanmakuSystemDetailedManager()
    {
        dataManager = DanmakuSystemDataManager.Instance;
        danmakuData = dataManager.danmakuData;
    }

    public void InitializePlayerDanmakuData(string playerID)
    {
        if (!danmakuData.PlayerDanmakuData.ContainsKey(playerID))
        {
            PlayerDanmakuData playerDanmakuData = new PlayerDanmakuData(playerID);
            danmakuData.AddPlayerDanmakuData(playerID, playerDanmakuData);
            dataManager.SaveDanmakuData();
            Debug.Log("初始化弹幕数据成功");
        }
    }

    public void InitializeMatchDanmakuData(string matchID)
    {
        if (!danmakuData.MatchDanmakuData.ContainsKey(matchID))
        {
            MatchDanmakuData matchDanmakuData = new MatchDanmakuData(matchID);
            danmakuData.AddMatchDanmakuData(matchID, matchDanmakuData);
            dataManager.SaveDanmakuData();
            Debug.Log("初始化比赛弹幕数据成功");
        }
    }

    public string SendDanmaku(string playerID, string playerName, string matchID, string content, string color, int fontSize, string danmakuType, float timestamp)
    {
        if (!danmakuData.PlayerDanmakuData.ContainsKey(playerID))
        {
            InitializePlayerDanmakuData(playerID);
        }
        
        if (!danmakuData.MatchDanmakuData.ContainsKey(matchID))
        {
            InitializeMatchDanmakuData(matchID);
        }
        
        PlayerDanmakuData playerDanmakuData = danmakuData.PlayerDanmakuData[playerID];
        MatchDanmakuData matchDanmakuData = danmakuData.MatchDanmakuData[matchID];
        
        if (!danmakuData.SystemEnabled || !playerDanmakuData.DanmakuConfig.DanmakuEnabled || !matchDanmakuData.DanmakuEnabled)
        {
            Debug.LogError("弹幕功能已禁用");
            return "";
        }
        
        if (playerDanmakuData.DanmakuBanned)
        {
            Debug.LogError("您已被禁止发送弹幕");
            return "";
        }
        
        if (content.Length > playerDanmakuData.DanmakuConfig.MaxLength)
        {
            Debug.LogError("弹幕内容过长");
            return "";
        }
        
        if (ContainsBannedWords(content))
        {
            Debug.LogError("弹幕包含敏感词");
            return "";
        }
        
        if (playerDanmakuData.SentDanmakus.FindAll(d => d.MatchID == matchID).Count >= danmakuData.MaxDanmakusPerPlayerPerMatch)
        {
            Debug.LogError("您在本场比赛发送的弹幕数量已达上限");
            return "";
        }
        
        if (matchDanmakuData.Danmakus.Count >= danmakuData.MaxDanmakusPerMatch)
        {
            Debug.LogError("本场比赛弹幕数量已达上限");
            return "";
        }
        
        if ((DateTime.Now - playerDanmakuData.LastDanmakuTime).TotalSeconds < 1)
        {
            Debug.LogError("发送弹幕过于频繁，请稍后再试");
            return "";
        }
        
        Danmaku danmaku = new Danmaku(matchID, playerID, playerName, content, color, fontSize, danmakuType, timestamp);
        matchDanmakuData.Danmakus.Add(danmaku);
        matchDanmakuData.TotalDanmakus++;
        matchDanmakuData.LastDanmakuTime = DateTime.Now;
        
        playerDanmakuData.SentDanmakus.Add(danmaku);
        playerDanmakuData.TotalDanmakusSent++;
        playerDanmakuData.LastDanmakuTime = DateTime.Now;
        
        dataManager.CreateDanmakuEvent("danmaku_send", playerID, matchID, danmaku.DanmakuID, "发送弹幕: " + content);
        dataManager.SaveDanmakuData();
        Debug.Log("发送弹幕成功: " + content);
        return danmaku.DanmakuID;
    }

    public void LikeDanmaku(string playerID, string danmakuID)
    {
        if (!danmakuData.PlayerDanmakuData.ContainsKey(playerID))
        {
            InitializePlayerDanmakuData(playerID);
        }
        
        PlayerDanmakuData playerDanmakuData = danmakuData.PlayerDanmakuData[playerID];
        if (playerDanmakuData.LikedDanmakus.Exists(d => d.DanmakuID == danmakuID))
        {
            Debug.LogError("您已经点赞过这条弹幕");
            return;
        }
        
        Danmaku danmaku = FindDanmakuByID(danmakuID);
        if (danmaku != null)
        {
            danmaku.LikeCount++;
            playerDanmakuData.LikedDanmakus.Add(danmaku);
            playerDanmakuData.TotalDanmakusLiked++;
            
            dataManager.CreateDanmakuEvent("danmaku_like", playerID, danmaku.MatchID, danmakuID, "点赞弹幕: " + danmaku.Content);
            dataManager.SaveDanmakuData();
            Debug.Log("点赞弹幕成功: " + danmaku.Content);
        }
    }

    public void PinDanmaku(string danmakuID, bool pinned)
    {
        Danmaku danmaku = FindDanmakuByID(danmakuID);
        if (danmaku != null)
        {
            danmaku.IsPinned = pinned;
            dataManager.CreateDanmakuEvent("danmaku_pin", "system", danmaku.MatchID, danmakuID, "弹幕" + (pinned ? "已置顶" : "已取消置顶"));
            dataManager.SaveDanmakuData();
            Debug.Log("弹幕" + (pinned ? "已置顶" : "已取消置顶") + ": " + danmaku.Content);
        }
    }

    public void BlockDanmaku(string danmakuID, bool blocked)
    {
        Danmaku danmaku = FindDanmakuByID(danmakuID);
        if (danmaku != null)
        {
            danmaku.IsBlocked = blocked;
            dataManager.CreateDanmakuEvent("danmaku_block", "system", danmaku.MatchID, danmakuID, "弹幕" + (blocked ? "已屏蔽" : "已取消屏蔽"));
            dataManager.SaveDanmakuData();
            Debug.Log("弹幕" + (blocked ? "已屏蔽" : "已取消屏蔽") + ": " + danmaku.Content);
        }
    }

    public void UpdateDanmakuConfig(string playerID, DanmakuConfig config)
    {
        if (!danmakuData.PlayerDanmakuData.ContainsKey(playerID))
        {
            InitializePlayerDanmakuData(playerID);
        }
        
        danmakuData.PlayerDanmakuData[playerID].DanmakuConfig = config;
        
        dataManager.CreateDanmakuEvent("danmaku_config_update", playerID, "", "", "更新弹幕配置");
        dataManager.SaveDanmakuData();
        Debug.Log("更新弹幕配置成功");
    }

    public void ToggleDanmakuEnabled(string playerID, bool enabled)
    {
        if (!danmakuData.PlayerDanmakuData.ContainsKey(playerID))
        {
            InitializePlayerDanmakuData(playerID);
        }
        
        danmakuData.PlayerDanmakuData[playerID].DanmakuConfig.DanmakuEnabled = enabled;
        
        dataManager.CreateDanmakuEvent("danmaku_toggle", playerID, "", "", "弹幕功能" + (enabled ? "已开启" : "已关闭"));
        dataManager.SaveDanmakuData();
        Debug.Log("弹幕功能" + (enabled ? "已开启" : "已关闭"));
    }

    public void ToggleMatchDanmakuEnabled(string matchID, bool enabled)
    {
        if (!danmakuData.MatchDanmakuData.ContainsKey(matchID))
        {
            InitializeMatchDanmakuData(matchID);
        }
        
        danmakuData.MatchDanmakuData[matchID].DanmakuEnabled = enabled;
        
        dataManager.CreateDanmakuEvent("match_danmaku_toggle", "system", matchID, "", "比赛弹幕" + (enabled ? "已开启" : "已关闭"));
        dataManager.SaveDanmakuData();
        Debug.Log("比赛弹幕" + (enabled ? "已开启" : "已关闭"));
    }

    public void BanPlayerDanmaku(string playerID, bool banned)
    {
        if (!danmakuData.PlayerDanmakuData.ContainsKey(playerID))
        {
            InitializePlayerDanmakuData(playerID);
        }
        
        danmakuData.PlayerDanmakuData[playerID].DanmakuBanned = banned;
        
        dataManager.CreateDanmakuEvent("player_danmaku_ban", "system", "", "", "玩家弹幕" + (banned ? "已禁用" : "已解禁") + ": " + playerID);
        dataManager.SaveDanmakuData();
        Debug.Log("玩家弹幕" + (banned ? "已禁用" : "已解禁") + ": " + playerID);
    }

    public List<Danmaku> GetMatchDanmakus(string matchID, float startTime = 0, float endTime = float.MaxValue)
    {
        if (!danmakuData.MatchDanmakuData.ContainsKey(matchID))
        {
            InitializeMatchDanmakuData(matchID);
        }
        
        List<Danmaku> danmakus = danmakuData.MatchDanmakuData[matchID].Danmakus.FindAll(d => !d.IsBlocked && d.Timestamp >= startTime && d.Timestamp <= endTime);
        danmakus.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));
        return danmakus;
    }

    public List<Danmaku> GetPlayerSentDanmakus(string playerID, string matchID = "")
    {
        if (!danmakuData.PlayerDanmakuData.ContainsKey(playerID))
        {
            InitializePlayerDanmakuData(playerID);
        }
        
        List<Danmaku> danmakus = danmakuData.PlayerDanmakuData[playerID].SentDanmakus;
        if (!string.IsNullOrEmpty(matchID))
        {
            danmakus = danmakus.FindAll(d => d.MatchID == matchID);
        }
        return danmakus;
    }

    public List<Danmaku> GetPlayerLikedDanmakus(string playerID)
    {
        if (!danmakuData.PlayerDanmakuData.ContainsKey(playerID))
        {
            InitializePlayerDanmakuData(playerID);
        }
        return danmakuData.PlayerDanmakuData[playerID].LikedDanmakus;
    }

    public DanmakuConfig GetDanmakuConfig(string playerID)
    {
        if (!danmakuData.PlayerDanmakuData.ContainsKey(playerID))
        {
            InitializePlayerDanmakuData(playerID);
        }
        return danmakuData.PlayerDanmakuData[playerID].DanmakuConfig;
    }

    public PlayerDanmakuData GetPlayerDanmakuData(string playerID)
    {
        if (!danmakuData.PlayerDanmakuData.ContainsKey(playerID))
        {
            InitializePlayerDanmakuData(playerID);
        }
        return danmakuData.PlayerDanmakuData[playerID];
    }

    public MatchDanmakuData GetMatchDanmakuData(string matchID)
    {
        if (!danmakuData.MatchDanmakuData.ContainsKey(matchID))
        {
            InitializeMatchDanmakuData(matchID);
        }
        return danmakuData.MatchDanmakuData[matchID];
    }

    public Danmaku GetDanmaku(string danmakuID)
    {
        return FindDanmakuByID(danmakuID);
    }

    public void AddBannedWord(string word)
    {
        if (!danmakuData.BannedWords.Contains(word))
        {
            danmakuData.BannedWords.Add(word);
            dataManager.SaveDanmakuData();
            Debug.Log("添加敏感词成功: " + word);
        }
    }

    public void RemoveBannedWord(string word)
    {
        if (danmakuData.BannedWords.Contains(word))
        {
            danmakuData.BannedWords.Remove(word);
            dataManager.SaveDanmakuData();
            Debug.Log("删除敏感词成功: " + word);
        }
    }

    public List<string> GetBannedWords()
    {
        return danmakuData.BannedWords;
    }

    public void CleanupOldDanmakus(int days = 7)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        int totalCleaned = 0;
        
        List<string> matchesToRemove = new List<string>();
        foreach (KeyValuePair<string, MatchDanmakuData> kvp in danmakuData.MatchDanmakuData)
        {
            if (kvp.Value.LastDanmakuTime < cutoffDate)
            {
                matchesToRemove.Add(kvp.Key);
                totalCleaned += kvp.Value.Danmakus.Count;
            }
        }
        
        foreach (string matchID in matchesToRemove)
        {
            danmakuData.MatchDanmakuData.Remove(matchID);
        }
        
        foreach (PlayerDanmakuData playerDanmakuData in danmakuData.PlayerDanmakuData.Values)
        {
            List<Danmaku> oldDanmakus = playerDanmakuData.SentDanmakus.FindAll(d => d.SendTime < cutoffDate);
            foreach (Danmaku danmaku in oldDanmakus)
            {
                playerDanmakuData.SentDanmakus.Remove(danmaku);
                playerDanmakuData.TotalDanmakusSent = Math.Max(0, playerDanmakuData.TotalDanmakusSent - 1);
                totalCleaned++;
            }
            
            oldDanmakus = playerDanmakuData.LikedDanmakus.FindAll(d => d.SendTime < cutoffDate);
            foreach (Danmaku danmaku in oldDanmakus)
            {
                playerDanmakuData.LikedDanmakus.Remove(danmaku);
                playerDanmakuData.TotalDanmakusLiked = Math.Max(0, playerDanmakuData.TotalDanmakusLiked - 1);
            }
        }
        
        if (totalCleaned > 0)
        {
            dataManager.CreateDanmakuEvent("danmaku_cleanup", "system", "", "", "清理旧弹幕: " + totalCleaned);
            dataManager.SaveDanmakuData();
            Debug.Log("清理旧弹幕成功: " + totalCleaned);
        }
    }

    private Danmaku FindDanmakuByID(string danmakuID)
    {
        foreach (MatchDanmakuData matchDanmakuData in danmakuData.MatchDanmakuData.Values)
        {
            Danmaku danmaku = matchDanmakuData.Danmakus.Find(d => d.DanmakuID == danmakuID);
            if (danmaku != null)
            {
                return danmaku;
            }
        }
        
        foreach (PlayerDanmakuData playerDanmakuData in danmakuData.PlayerDanmakuData.Values)
        {
            Danmaku danmaku = playerDanmakuData.SentDanmakus.Find(d => d.DanmakuID == danmakuID);
            if (danmaku != null)
            {
                return danmaku;
            }
            
            danmaku = playerDanmakuData.LikedDanmakus.Find(d => d.DanmakuID == danmakuID);
            if (danmaku != null)
            {
                return danmaku;
            }
        }
        
        return null;
    }

    private bool ContainsBannedWords(string content)
    {
        foreach (string word in danmakuData.BannedWords)
        {
            if (content.Contains(word))
            {
                return true;
            }
        }
        return false;
    }

    public void SaveData()
    {
        dataManager.SaveDanmakuData();
    }

    public void LoadData()
    {
        dataManager.LoadDanmakuData();
    }

    public List<DanmakuEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
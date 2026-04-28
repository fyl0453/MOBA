using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LeaderboardSystemDetailedManager : MonoBehaviour
{
    public static LeaderboardSystemDetailedManager Instance { get; private set; }
    
    public LeaderboardSystemDetailedManagerData leaderboardData;
    
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
        LoadLeaderboardData();
        
        if (leaderboardData == null)
        {
            leaderboardData = new LeaderboardSystemDetailedManagerData();
            InitializeDefaultLeaderboardSystem();
        }
    }
    
    private void InitializeDefaultLeaderboardSystem()
    {
        // 排行榜
        Leaderboard leaderboard1 = new Leaderboard("leaderboard_001", "王者段位榜", "根据玩家段位排名", "rank", "score", "desc", 100, "daily");
        Leaderboard leaderboard2 = new Leaderboard("leaderboard_002", "击杀榜", "根据玩家击杀数排名", "kill", "score", "desc", 100, "daily");
        Leaderboard leaderboard3 = new Leaderboard("leaderboard_003", "助攻榜", "根据玩家助攻数排名", "assist", "score", "desc", 100, "daily");
        Leaderboard leaderboard4 = new Leaderboard("leaderboard_004", "金币榜", "根据玩家金币数排名", "gold", "score", "desc", 100, "daily");
        Leaderboard leaderboard5 = new Leaderboard("leaderboard_005", "经验榜", "根据玩家经验值排名", "exp", "score", "desc", 100, "daily");
        
        leaderboardData.system.AddLeaderboard(leaderboard1);
        leaderboardData.system.AddLeaderboard(leaderboard2);
        leaderboardData.system.AddLeaderboard(leaderboard3);
        leaderboardData.system.AddLeaderboard(leaderboard4);
        leaderboardData.system.AddLeaderboard(leaderboard5);
        
        // 排行榜奖励
        LeaderboardReward reward1 = new LeaderboardReward("reward_001", "leaderboard_001", "王者段位奖励", "currency", "diamond", "第1-3名奖励", 1000, 1, 3);
        LeaderboardReward reward2 = new LeaderboardReward("reward_002", "leaderboard_001", "钻石段位奖励", "currency", "diamond", "第4-10名奖励", 500, 4, 10);
        LeaderboardReward reward3 = new LeaderboardReward("reward_003", "leaderboard_001", "铂金段位奖励", "currency", "diamond", "第11-50名奖励", 200, 11, 50);
        LeaderboardReward reward4 = new LeaderboardReward("reward_004", "leaderboard_002", "击杀榜奖励", "currency", "gold", "第1-10名奖励", 10000, 1, 10);
        LeaderboardReward reward5 = new LeaderboardReward("reward_005", "leaderboard_003", "助攻榜奖励", "currency", "gold", "第1-10名奖励", 5000, 1, 10);
        
        leaderboardData.system.AddLeaderboardReward(reward1);
        leaderboardData.system.AddLeaderboardReward(reward2);
        leaderboardData.system.AddLeaderboardReward(reward3);
        leaderboardData.system.AddLeaderboardReward(reward4);
        leaderboardData.system.AddLeaderboardReward(reward5);
        
        // 排行榜条目
        LeaderboardEntry entry1 = new LeaderboardEntry("entry_001", "leaderboard_001", "user_001", "张三", "avatar_001", 10000);
        LeaderboardEntry entry2 = new LeaderboardEntry("entry_002", "leaderboard_001", "user_002", "李四", "avatar_002", 9500);
        LeaderboardEntry entry3 = new LeaderboardEntry("entry_003", "leaderboard_001", "user_003", "王五", "avatar_003", 9000);
        LeaderboardEntry entry4 = new LeaderboardEntry("entry_004", "leaderboard_001", "user_004", "赵六", "avatar_004", 8500);
        LeaderboardEntry entry5 = new LeaderboardEntry("entry_005", "leaderboard_001", "user_005", "钱七", "avatar_005", 8000);
        
        LeaderboardEntry entry6 = new LeaderboardEntry("entry_006", "leaderboard_002", "user_001", "张三", "avatar_001", 1000);
        LeaderboardEntry entry7 = new LeaderboardEntry("entry_007", "leaderboard_002", "user_002", "李四", "avatar_002", 950);
        LeaderboardEntry entry8 = new LeaderboardEntry("entry_008", "leaderboard_002", "user_003", "王五", "avatar_003", 900);
        LeaderboardEntry entry9 = new LeaderboardEntry("entry_009", "leaderboard_002", "user_004", "赵六", "avatar_004", 850);
        LeaderboardEntry entry10 = new LeaderboardEntry("entry_010", "leaderboard_002", "user_005", "钱七", "avatar_005", 800);
        
        leaderboardData.system.AddLeaderboardEntry(entry1);
        leaderboardData.system.AddLeaderboardEntry(entry2);
        leaderboardData.system.AddLeaderboardEntry(entry3);
        leaderboardData.system.AddLeaderboardEntry(entry4);
        leaderboardData.system.AddLeaderboardEntry(entry5);
        leaderboardData.system.AddLeaderboardEntry(entry6);
        leaderboardData.system.AddLeaderboardEntry(entry7);
        leaderboardData.system.AddLeaderboardEntry(entry8);
        leaderboardData.system.AddLeaderboardEntry(entry9);
        leaderboardData.system.AddLeaderboardEntry(entry10);
        
        // 排行榜事件
        LeaderboardEvent event1 = new LeaderboardEvent("event_001", "rank_change", "leaderboard_001", "user_001", "排名变化", 9500, 10000, 2, 1);
        LeaderboardEvent event2 = new LeaderboardEvent("event_002", "score_update", "leaderboard_001", "user_002", "分数更新", 9000, 9500, 3, 2);
        LeaderboardEvent event3 = new LeaderboardEvent("event_003", "rank_change", "leaderboard_002", "user_001", "排名变化", 950, 1000, 2, 1);
        
        leaderboardData.system.AddLeaderboardEvent(event1);
        leaderboardData.system.AddLeaderboardEvent(event2);
        leaderboardData.system.AddLeaderboardEvent(event3);
        
        // 更新排名
        UpdateLeaderboardRanks("leaderboard_001");
        UpdateLeaderboardRanks("leaderboard_002");
        
        SaveLeaderboardData();
    }
    
    // 排行榜管理
    public void AddLeaderboard(string name, string description, string leaderboardType, string sortBy, string sortOrder, int maxEntries, string refreshInterval)
    {
        string leaderboardID = "leaderboard_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Leaderboard leaderboard = new Leaderboard(leaderboardID, name, description, leaderboardType, sortBy, sortOrder, maxEntries, refreshInterval);
        leaderboardData.system.AddLeaderboard(leaderboard);
        SaveLeaderboardData();
        Debug.Log("成功添加排行榜: " + name);
    }
    
    public List<Leaderboard> GetLeaderboardsByType(string leaderboardType)
    {
        return leaderboardData.system.GetLeaderboardsByType(leaderboardType);
    }
    
    public List<Leaderboard> GetAllLeaderboards()
    {
        return leaderboardData.system.leaderboards;
    }
    
    public void EnableLeaderboard(string leaderboardID)
    {
        Leaderboard leaderboard = leaderboardData.system.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            leaderboard.Enable();
            SaveLeaderboardData();
            Debug.Log("成功启用排行榜: " + leaderboard.leaderboardName);
        }
        else
        {
            Debug.LogError("排行榜不存在: " + leaderboardID);
        }
    }
    
    public void DisableLeaderboard(string leaderboardID)
    {
        Leaderboard leaderboard = leaderboardData.system.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            leaderboard.Disable();
            SaveLeaderboardData();
            Debug.Log("成功禁用排行榜: " + leaderboard.leaderboardName);
        }
        else
        {
            Debug.LogError("排行榜不存在: " + leaderboardID);
        }
    }
    
    public void SetLeaderboardVisibility(string leaderboardID, bool visible)
    {
        Leaderboard leaderboard = leaderboardData.system.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            leaderboard.SetVisibility(visible);
            SaveLeaderboardData();
            Debug.Log("成功设置排行榜可见性: " + visible);
        }
        else
        {
            Debug.LogError("排行榜不存在: " + leaderboardID);
        }
    }
    
    // 排行榜奖励管理
    public void AddLeaderboardReward(string leaderboardID, string rewardName, string rewardType, string rewardValue, string description, int quantity, int minRank, int maxRank)
    {
        string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        LeaderboardReward reward = new LeaderboardReward(rewardID, leaderboardID, rewardName, rewardType, rewardValue, description, quantity, minRank, maxRank);
        leaderboardData.system.AddLeaderboardReward(reward);
        SaveLeaderboardData();
        Debug.Log("成功添加排行榜奖励: " + rewardName);
    }
    
    public List<LeaderboardReward> GetLeaderboardRewards(string leaderboardID)
    {
        return leaderboardData.system.GetLeaderboardRewardsByLeaderboard(leaderboardID);
    }
    
    public void EnableLeaderboardReward(string rewardID)
    {
        LeaderboardReward reward = leaderboardData.system.GetLeaderboardReward(rewardID);
        if (reward != null)
        {
            reward.Enable();
            SaveLeaderboardData();
            Debug.Log("成功启用排行榜奖励: " + reward.rewardName);
        }
        else
        {
            Debug.LogError("排行榜奖励不存在: " + rewardID);
        }
    }
    
    public void DisableLeaderboardReward(string rewardID)
    {
        LeaderboardReward reward = leaderboardData.system.GetLeaderboardReward(rewardID);
        if (reward != null)
        {
            reward.Disable();
            SaveLeaderboardData();
            Debug.Log("成功禁用排行榜奖励: " + reward.rewardName);
        }
        else
        {
            Debug.LogError("排行榜奖励不存在: " + rewardID);
        }
    }
    
    // 排行榜条目管理
    public void AddLeaderboardEntry(string leaderboardID, string userID, string userName, string userAvatar, float score)
    {
        // 检查是否已存在该用户的条目
        List<LeaderboardEntry> entries = leaderboardData.system.GetLeaderboardEntriesByLeaderboard(leaderboardID);
        LeaderboardEntry existingEntry = entries.Find(e => e.userID == userID);
        
        if (existingEntry != null)
        {
            // 更新现有条目
            float oldScore = existingEntry.score;
            int oldRank = existingEntry.rank;
            existingEntry.UpdateScore(score);
            
            // 更新排名
            UpdateLeaderboardRanks(leaderboardID);
            
            // 记录排行榜事件
            CreateLeaderboardEvent("score_update", leaderboardID, userID, "分数更新", oldScore, score, oldRank, existingEntry.rank);
            
            SaveLeaderboardData();
            Debug.Log("成功更新排行榜条目: " + userName);
        }
        else
        {
            // 创建新条目
            string entryID = "entry_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            LeaderboardEntry entry = new LeaderboardEntry(entryID, leaderboardID, userID, userName, userAvatar, score);
            leaderboardData.system.AddLeaderboardEntry(entry);
            
            // 更新排名
            UpdateLeaderboardRanks(leaderboardID);
            
            // 记录排行榜事件
            CreateLeaderboardEvent("entry_add", leaderboardID, userID, "添加排行榜条目", 0, score, 0, entry.rank);
            
            SaveLeaderboardData();
            Debug.Log("成功添加排行榜条目: " + userName);
        }
    }
    
    public void UpdateLeaderboardEntryScore(string leaderboardID, string userID, float score)
    {
        List<LeaderboardEntry> entries = leaderboardData.system.GetLeaderboardEntriesByLeaderboard(leaderboardID);
        LeaderboardEntry entry = entries.Find(e => e.userID == userID);
        
        if (entry != null)
        {
            float oldScore = entry.score;
            int oldRank = entry.rank;
            entry.UpdateScore(score);
            
            // 更新排名
            UpdateLeaderboardRanks(leaderboardID);
            
            // 记录排行榜事件
            CreateLeaderboardEvent("score_update", leaderboardID, userID, "分数更新", oldScore, score, oldRank, entry.rank);
            
            SaveLeaderboardData();
            Debug.Log("成功更新排行榜条目分数: " + userID);
        }
        else
        {
            // 如果条目不存在，创建新条目
            AddLeaderboardEntry(leaderboardID, userID, "", "", score);
        }
    }
    
    public List<LeaderboardEntry> GetLeaderboardEntries(string leaderboardID, int limit = 100)
    {
        List<LeaderboardEntry> entries = leaderboardData.system.GetLeaderboardEntriesByLeaderboard(leaderboardID);
        entries.Sort((a, b) => b.score.CompareTo(a.score));
        return entries.GetRange(0, Mathf.Min(limit, entries.Count));
    }
    
    public LeaderboardEntry GetLeaderboardEntry(string leaderboardID, string userID)
    {
        List<LeaderboardEntry> entries = leaderboardData.system.GetLeaderboardEntriesByLeaderboard(leaderboardID);
        return entries.Find(e => e.userID == userID);
    }
    
    // 排名更新
    public void UpdateLeaderboardRanks(string leaderboardID)
    {
        List<LeaderboardEntry> entries = leaderboardData.system.GetLeaderboardEntriesByLeaderboard(leaderboardID);
        
        // 根据分数排序
        entries.Sort((a, b) => b.score.CompareTo(a.score));
        
        // 更新排名
        for (int i = 0; i < entries.Count; i++)
        {
            entries[i].UpdateRank(i + 1);
        }
        
        SaveLeaderboardData();
        Debug.Log("成功更新排行榜排名: " + leaderboardID);
    }
    
    // 排行榜事件管理
    public string CreateLeaderboardEvent(string eventType, string leaderboardID, string userID, string description, float oldScore, float newScore, int oldRank, int newRank)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        LeaderboardEvent leaderboardEvent = new LeaderboardEvent(eventID, eventType, leaderboardID, userID, description, oldScore, newScore, oldRank, newRank);
        leaderboardData.system.AddLeaderboardEvent(leaderboardEvent);
        SaveLeaderboardData();
        Debug.Log("成功创建排行榜事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        LeaderboardEvent leaderboardEvent = leaderboardData.system.GetLeaderboardEvent(eventID);
        if (leaderboardEvent != null)
        {
            leaderboardEvent.MarkAsCompleted();
            SaveLeaderboardData();
            Debug.Log("成功标记排行榜事件为完成");
        }
        else
        {
            Debug.LogError("排行榜事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        LeaderboardEvent leaderboardEvent = leaderboardData.system.GetLeaderboardEvent(eventID);
        if (leaderboardEvent != null)
        {
            leaderboardEvent.MarkAsFailed();
            SaveLeaderboardData();
            Debug.Log("成功标记排行榜事件为失败");
        }
        else
        {
            Debug.LogError("排行榜事件不存在: " + eventID);
        }
    }
    
    // 奖励发放
    public void DistributeLeaderboardRewards(string leaderboardID)
    {
        List<LeaderboardEntry> entries = leaderboardData.system.GetLeaderboardEntriesByLeaderboard(leaderboardID);
        List<LeaderboardReward> rewards = leaderboardData.system.GetLeaderboardRewardsByLeaderboard(leaderboardID);
        
        foreach (LeaderboardEntry entry in entries)
        {
            foreach (LeaderboardReward reward in rewards)
            {
                if (reward.isEnabled && entry.rank >= reward.minRank && entry.rank <= reward.maxRank)
                {
                    // 这里可以添加发放奖励的逻辑
                    Debug.Log("发放排行榜奖励: " + reward.rewardName + " 给玩家: " + entry.userName + " 排名: " + entry.rank);
                }
            }
        }
        
        SaveLeaderboardData();
        Debug.Log("成功发放排行榜奖励: " + leaderboardID);
    }
    
    // 数据持久化
    public void SaveLeaderboardData()
    {
        string path = Application.dataPath + "/Data/leaderboard_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, leaderboardData);
        stream.Close();
    }
    
    public void LoadLeaderboardData()
    {
        string path = Application.dataPath + "/Data/leaderboard_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            leaderboardData = (LeaderboardSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            leaderboardData = new LeaderboardSystemDetailedManagerData();
        }
    }
}
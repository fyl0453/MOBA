using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }
    
    public LeaderboardManagerData leaderboardData;
    
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
            leaderboardData = new LeaderboardManagerData();
            InitializeDefaultLeaderboards();
        }
    }
    
    private void InitializeDefaultLeaderboards()
    {
        // 段位排行榜
        Leaderboard rankLeaderboard = new Leaderboard("leaderboard_rank", "段位排行榜", "Rank");
        rankLeaderboard.AddEntry(new LeaderboardEntry("player_001", "玩家1", 1000, 30));
        rankLeaderboard.AddEntry(new LeaderboardEntry("player_002", "关羽", 950, 25));
        rankLeaderboard.AddEntry(new LeaderboardEntry("player_003", "张飞", 900, 22));
        rankLeaderboard.AddEntry(new LeaderboardEntry("player_004", "刘备", 850, 20));
        rankLeaderboard.AddEntry(new LeaderboardEntry("player_005", "赵云", 800, 18));
        rankLeaderboard.AddEntry(new LeaderboardEntry("player_006", "诸葛亮", 750, 15));
        rankLeaderboard.AddEntry(new LeaderboardEntry("player_007", "孙尚香", 700, 12));
        rankLeaderboard.AddEntry(new LeaderboardEntry("player_008", "貂蝉", 650, 10));
        rankLeaderboard.AddEntry(new LeaderboardEntry("player_009", "吕布", 600, 8));
        rankLeaderboard.AddEntry(new LeaderboardEntry("player_010", "周瑜", 550, 5));
        leaderboardData.AddLeaderboard(rankLeaderboard);
        
        // 击杀排行榜
        Leaderboard killLeaderboard = new Leaderboard("leaderboard_kills", "击杀排行榜", "Kills");
        killLeaderboard.AddEntry(new LeaderboardEntry("player_002", "关羽", 500, 25));
        killLeaderboard.AddEntry(new LeaderboardEntry("player_005", "赵云", 450, 18));
        killLeaderboard.AddEntry(new LeaderboardEntry("player_009", "吕布", 400, 8));
        killLeaderboard.AddEntry(new LeaderboardEntry("player_001", "玩家1", 350, 30));
        killLeaderboard.AddEntry(new LeaderboardEntry("player_003", "张飞", 300, 22));
        leaderboardData.AddLeaderboard(killLeaderboard);
        
        // 胜率排行榜
        Leaderboard winrateLeaderboard = new Leaderboard("leaderboard_winrate", "胜率排行榜", "WinRate");
        winrateLeaderboard.AddEntry(new LeaderboardEntry("player_006", "诸葛亮", 85, 15));
        winrateLeaderboard.AddEntry(new LeaderboardEntry("player_004", "刘备", 80, 20));
        winrateLeaderboard.AddEntry(new LeaderboardEntry("player_007", "孙尚香", 75, 12));
        winrateLeaderboard.AddEntry(new LeaderboardEntry("player_001", "玩家1", 70, 30));
        winrateLeaderboard.AddEntry(new LeaderboardEntry("player_002", "关羽", 65, 25));
        leaderboardData.AddLeaderboard(winrateLeaderboard);
        
        // 公会排行榜
        Leaderboard guildLeaderboard = new Leaderboard("leaderboard_guild", "公会排行榜", "Guild");
        guildLeaderboard.AddEntry(new LeaderboardEntry("guild_001", "王者荣耀", 1500, 5));
        guildLeaderboard.AddEntry(new LeaderboardEntry("guild_003", "和平精英", 1200, 3));
        guildLeaderboard.AddEntry(new LeaderboardEntry("guild_002", "英雄联盟", 1000, 2));
        leaderboardData.AddLeaderboard(guildLeaderboard);
        
        SaveLeaderboardData();
    }
    
    public void UpdatePlayerScore(string leaderboardID, string playerID, int newScore)
    {
        Leaderboard leaderboard = leaderboardData.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            leaderboard.UpdateEntry(playerID, newScore);
            SaveLeaderboardData();
        }
    }
    
    public void AddPlayerToLeaderboard(string leaderboardID, string playerID, string playerName, int score, int level = 1)
    {
        Leaderboard leaderboard = leaderboardData.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            LeaderboardEntry entry = new LeaderboardEntry(playerID, playerName, score, level);
            leaderboard.AddEntry(entry);
            SaveLeaderboardData();
        }
    }
    
    public Leaderboard GetLeaderboard(string leaderboardID)
    {
        return leaderboardData.GetLeaderboard(leaderboardID);
    }
    
    public List<LeaderboardEntry> GetTopEntries(string leaderboardID, int count = 10)
    {
        Leaderboard leaderboard = leaderboardData.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            return leaderboard.GetTopEntries(count);
        }
        return new List<LeaderboardEntry>();
    }
    
    public List<LeaderboardEntry> GetEntriesAroundPlayer(string leaderboardID, string playerID, int range = 5)
    {
        Leaderboard leaderboard = leaderboardData.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            LeaderboardEntry playerEntry = leaderboard.GetEntryByPlayerID(playerID);
            if (playerEntry != null)
            {
                return leaderboard.GetEntriesAroundRank(playerEntry.rank, range);
            }
        }
        return new List<LeaderboardEntry>();
    }
    
    public int GetPlayerRank(string leaderboardID, string playerID)
    {
        Leaderboard leaderboard = leaderboardData.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            LeaderboardEntry entry = leaderboard.GetEntryByPlayerID(playerID);
            if (entry != null)
            {
                return entry.rank;
            }
        }
        return 0;
    }
    
    public int GetPlayerScore(string leaderboardID, string playerID)
    {
        Leaderboard leaderboard = leaderboardData.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            LeaderboardEntry entry = leaderboard.GetEntryByPlayerID(playerID);
            if (entry != null)
            {
                return entry.score;
            }
        }
        return 0;
    }
    
    public void ResetLeaderboard(string leaderboardID)
    {
        Leaderboard leaderboard = leaderboardData.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            leaderboard.entries.Clear();
            SaveLeaderboardData();
        }
    }
    
    public void SaveLeaderboardData()
    {
        string path = Application.dataPath + "/Data/leaderboard_data.dat";
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
        string path = Application.dataPath + "/Data/leaderboard_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            leaderboardData = (LeaderboardManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
    }
}
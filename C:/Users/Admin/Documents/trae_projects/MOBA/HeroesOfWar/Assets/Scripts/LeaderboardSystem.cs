using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LeaderboardSystem : MonoBehaviour
{
    public static LeaderboardSystem Instance { get; private set; }
    
    private LeaderboardData leaderboardData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLeaderboardData();
            
            if (leaderboardData == null)
            {
                leaderboardData = new LeaderboardData();
                InitializeDefaultLeaderboards();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeDefaultLeaderboards()
    {
        // 段位排行榜
        leaderboardData.leaderboards.Add(new Leaderboard
        {
            leaderboardID = "rank",
            leaderboardName = "段位排行榜",
            entries = new List<LeaderboardEntry>()
        });
        
        // 击杀排行榜
        leaderboardData.leaderboards.Add(new Leaderboard
        {
            leaderboardID = "kills",
            leaderboardName = "击杀排行榜",
            entries = new List<LeaderboardEntry>()
        });
        
        // 胜率排行榜
        leaderboardData.leaderboards.Add(new Leaderboard
        {
            leaderboardID = "winrate",
            leaderboardName = "胜率排行榜",
            entries = new List<LeaderboardEntry>()
        });
        
        // KDA排行榜
        leaderboardData.leaderboards.Add(new Leaderboard
        {
            leaderboardID = "kda",
            leaderboardName = "KDA排行榜",
            entries = new List<LeaderboardEntry>()
        });
        
        // 金币排行榜
        leaderboardData.leaderboards.Add(new Leaderboard
        {
            leaderboardID = "gold",
            leaderboardName = "金币排行榜",
            entries = new List<LeaderboardEntry>()
        });
        
        // 等级排行榜
        leaderboardData.leaderboards.Add(new Leaderboard
        {
            leaderboardID = "level",
            leaderboardName = "等级排行榜",
            entries = new List<LeaderboardEntry>()
        });
        
        // 添加一些示例数据
        AddSampleData();
        
        SaveLeaderboardData();
    }
    
    private void AddSampleData()
    {
        // 段位排行榜示例数据
        AddLeaderboardEntry("rank", "玩家1", 1000, 1);
        AddLeaderboardEntry("rank", "玩家2", 950, 2);
        AddLeaderboardEntry("rank", "玩家3", 900, 3);
        AddLeaderboardEntry("rank", "玩家4", 850, 4);
        AddLeaderboardEntry("rank", "玩家5", 800, 5);
        
        // 击杀排行榜示例数据
        AddLeaderboardEntry("kills", "玩家A", 150, 1);
        AddLeaderboardEntry("kills", "玩家B", 120, 2);
        AddLeaderboardEntry("kills", "玩家C", 100, 3);
        AddLeaderboardEntry("kills", "玩家D", 80, 4);
        AddLeaderboardEntry("kills", "玩家E", 60, 5);
        
        // 胜率排行榜示例数据
        AddLeaderboardEntry("winrate", "玩家X", 80, 1);
        AddLeaderboardEntry("winrate", "玩家Y", 75, 2);
        AddLeaderboardEntry("winrate", "玩家Z", 70, 3);
        AddLeaderboardEntry("winrate", "玩家W", 65, 4);
        AddLeaderboardEntry("winrate", "玩家V", 60, 5);
        
        // KDA排行榜示例数据
        AddLeaderboardEntry("kda", "玩家P", 5.5f, 1);
        AddLeaderboardEntry("kda", "玩家Q", 4.8f, 2);
        AddLeaderboardEntry("kda", "玩家R", 4.2f, 3);
        AddLeaderboardEntry("kda", "玩家S", 3.8f, 4);
        AddLeaderboardEntry("kda", "玩家T", 3.5f, 5);
        
        // 金币排行榜示例数据
        AddLeaderboardEntry("gold", "玩家M", 50000, 1);
        AddLeaderboardEntry("gold", "玩家N", 45000, 2);
        AddLeaderboardEntry("gold", "玩家O", 40000, 3);
        AddLeaderboardEntry("gold", "玩家L", 35000, 4);
        AddLeaderboardEntry("gold", "玩家K", 30000, 5);
        
        // 等级排行榜示例数据
        AddLeaderboardEntry("level", "玩家U", 30, 1);
        AddLeaderboardEntry("level", "玩家V", 28, 2);
        AddLeaderboardEntry("level", "玩家W", 25, 3);
        AddLeaderboardEntry("level", "玩家X", 22, 4);
        AddLeaderboardEntry("level", "玩家Y", 20, 5);
    }
    
    public void AddLeaderboardEntry(string leaderboardID, string playerName, float value, int rank = 0)
    {
        Leaderboard leaderboard = leaderboardData.leaderboards.Find(l => l.leaderboardID == leaderboardID);
        if (leaderboard != null)
        {
            LeaderboardEntry entry = new LeaderboardEntry
            {
                playerName = playerName,
                value = value,
                rank = rank,
                updateDate = System.DateTime.Now.ToString()
            };
            
            // 添加到排行榜
            leaderboard.entries.Add(entry);
            
            // 按值排序
            leaderboard.entries.Sort((a, b) => b.value.CompareTo(a.value));
            
            // 更新排名
            for (int i = 0; i < leaderboard.entries.Count; i++)
            {
                leaderboard.entries[i].rank = i + 1;
            }
            
            // 只保留前100名
            if (leaderboard.entries.Count > 100)
            {
                leaderboard.entries.RemoveRange(100, leaderboard.entries.Count - 100);
            }
            
            SaveLeaderboardData();
        }
    }
    
    public Leaderboard GetLeaderboard(string leaderboardID)
    {
        return leaderboardData.leaderboards.Find(l => l.leaderboardID == leaderboardID);
    }
    
    public List<Leaderboard> GetAllLeaderboards()
    {
        return leaderboardData.leaderboards;
    }
    
    public List<LeaderboardEntry> GetLeaderboardEntries(string leaderboardID, int count = 50)
    {
        Leaderboard leaderboard = GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            return leaderboard.entries.GetRange(0, Mathf.Min(count, leaderboard.entries.Count));
        }
        return new List<LeaderboardEntry>();
    }
    
    public int GetPlayerRank(string leaderboardID, string playerName)
    {
        Leaderboard leaderboard = GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            LeaderboardEntry entry = leaderboard.entries.Find(e => e.playerName == playerName);
            if (entry != null)
            {
                return entry.rank;
            }
        }
        return 0;
    }
    
    public float GetPlayerValue(string leaderboardID, string playerName)
    {
        Leaderboard leaderboard = GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            LeaderboardEntry entry = leaderboard.entries.Find(e => e.playerName == playerName);
            if (entry != null)
            {
                return entry.value;
            }
        }
        return 0f;
    }
    
    public void UpdatePlayerScore(string leaderboardID, string playerName, float value)
    {
        Leaderboard leaderboard = GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            LeaderboardEntry entry = leaderboard.entries.Find(e => e.playerName == playerName);
            if (entry != null)
            {
                entry.value = value;
                entry.updateDate = System.DateTime.Now.ToString();
                
                // 重新排序
                leaderboard.entries.Sort((a, b) => b.value.CompareTo(a.value));
                
                // 更新排名
                for (int i = 0; i < leaderboard.entries.Count; i++)
                {
                    leaderboard.entries[i].rank = i + 1;
                }
                
                SaveLeaderboardData();
            }
            else
            {
                // 如果玩家不在排行榜中，添加新条目
                AddLeaderboardEntry(leaderboardID, playerName, value);
            }
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
            leaderboardData = (LeaderboardData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            leaderboardData = new LeaderboardData();
        }
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<Leaderboard> leaderboards = new List<Leaderboard>();
}

[System.Serializable]
public class Leaderboard
{
    public string leaderboardID;
    public string leaderboardName;
    public List<LeaderboardEntry> entries;
}

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float value;
    public int rank;
    public string updateDate;
}

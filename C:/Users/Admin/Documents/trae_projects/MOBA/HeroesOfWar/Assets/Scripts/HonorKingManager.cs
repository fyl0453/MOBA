using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HonorKingManager : MonoBehaviour
{
    public static HonorKingManager Instance { get; private set; }
    
    public HonorKingManagerData honorKingData;
    
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
        LoadHonorKingData();
        
        if (honorKingData == null)
        {
            honorKingData = new HonorKingManagerData();
            InitializeDefaultHonorKing();
        }
    }
    
    private void InitializeDefaultHonorKing()
    {
        // 创建默认赛季荣誉
        SeasonHonor season1 = new SeasonHonor("season_1", "S1赛季", "2024-01-01", "2024-03-31");
        SeasonHonor season2 = new SeasonHonor("season_2", "S2赛季", "2024-04-01", "2024-06-30");
        
        honorKingData.system.AddSeasonHonor(season1);
        honorKingData.system.AddSeasonHonor(season2);
        
        // 创建默认荣耀王者
        HonorKing honorKing1 = new HonorKing("user_1", "Player1", 15, "season_1", "S1赛季");
        HonorKing honorKing2 = new HonorKing("user_2", "Player2", 12, "season_1", "S1赛季");
        HonorKing honorKing3 = new HonorKing("user_3", "Player3", 10, "season_1", "S1赛季");
        
        honorKingData.system.AddHonorKing(honorKing1);
        honorKingData.system.AddHonorKing(honorKing2);
        honorKingData.system.AddHonorKing(honorKing3);
        
        // 更新排名
        UpdateHonorKingRanks("season_1");
        
        SaveHonorKingData();
    }
    
    public void UpdateHonorKing(string playerID, string playerName, int stars, string seasonID, string seasonName)
    {
        HonorKing existing = honorKingData.system.GetHonorKing(playerID);
        if (existing != null)
        {
            existing.UpdateStars(stars);
        }
        else
        {
            HonorKing newHonorKing = new HonorKing(playerID, playerName, stars, seasonID, seasonName);
            honorKingData.system.AddHonorKing(newHonorKing);
        }
        
        UpdateHonorKingRanks(seasonID);
        SaveHonorKingData();
    }
    
    private void UpdateHonorKingRanks(string seasonID)
    {
        List<HonorKing> honorKings = honorKingData.system.honorKings.FindAll(hk => hk.seasonID == seasonID);
        honorKings.Sort((a, b) => b.stars.CompareTo(a.stars));
        
        for (int i = 0; i < honorKings.Count; i++)
        {
            honorKings[i].UpdateRank(i + 1);
        }
        
        // 更新赛季荣誉
        SeasonHonor seasonHonor = honorKingData.system.GetSeasonHonor(seasonID);
        if (seasonHonor != null)
        {
            seasonHonor.honorKingIDs.Clear();
            for (int i = 0; i < Mathf.Min(100, honorKings.Count); i++)
            {
                seasonHonor.AddHonorKing(honorKings[i].playerID);
            }
        }
    }
    
    public List<HonorKing> GetTopHonorKings(int limit = 100)
    {
        return honorKingData.system.GetTopHonorKings(limit);
    }
    
    public HonorKing GetHonorKing(string playerID)
    {
        return honorKingData.system.GetHonorKing(playerID);
    }
    
    public bool IsHonorKing(string playerID)
    {
        HonorKing honorKing = honorKingData.system.GetHonorKing(playerID);
        return honorKing != null && honorKing.isHonorKing;
    }
    
    public int GetHonorKingRank(string playerID)
    {
        HonorKing honorKing = honorKingData.system.GetHonorKing(playerID);
        return honorKing != null ? honorKing.rank : 0;
    }
    
    public List<HonorKing> GetSeasonHonorKings(string seasonID)
    {
        return honorKingData.system.honorKings.FindAll(hk => hk.seasonID == seasonID);
    }
    
    public void SaveHonorKingData()
    {
        string path = Application.dataPath + "/Data/honor_king_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, honorKingData);
        stream.Close();
    }
    
    public void LoadHonorKingData()
    {
        string path = Application.dataPath + "/Data/honor_king_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            honorKingData = (HonorKingManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            honorKingData = new HonorKingManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class KingMarkManager : MonoBehaviour
{
    public static KingMarkManager Instance { get; private set; }
    
    public KingMarkManagerData markData;
    
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
        LoadMarkData();
        
        if (markData == null)
        {
            markData = new KingMarkManagerData();
            InitializeDefaultSeasons();
        }
    }
    
    private void InitializeDefaultSeasons()
    {
        // 创建默认赛季
        Season season1 = new Season("season_1", "S1赛季", "第一赛季", "2024-01-01", "2024-03-31", 1);
        Season season2 = new Season("season_2", "S2赛季", "第二赛季", "2024-04-01", "2024-06-30", 2);
        Season season3 = new Season("season_3", "S3赛季", "第三赛季", "2024-07-01", "2024-09-30", 3);
        Season season4 = new Season("season_4", "S4赛季", "第四赛季", "2024-10-01", "2024-12-31", 4);
        
        // 激活当前赛季
        season4.Activate();
        
        markData.system.AddSeason(season1);
        markData.system.AddSeason(season2);
        markData.system.AddSeason(season3);
        markData.system.AddSeason(season4);
        
        SaveMarkData();
    }
    
    public string CreateKingMark(string playerID, string playerName, string seasonID, int rank, string rankName, int stars, bool isKing)
    {
        Season season = markData.system.GetSeason(seasonID);
        if (season != null)
        {
            string markID = System.Guid.NewGuid().ToString();
            KingMark mark = new KingMark(markID, playerID, playerName, seasonID, season.seasonName, rank, rankName, stars, isKing);
            markData.system.AddMark(mark);
            
            PlayerMarkData playerData = GetOrCreatePlayerData(playerID);
            playerData.AddMark(markID, isKing, rankName);
            
            SaveMarkData();
            Debug.Log($"成功创建王者印记: {playerName} 在 {season.seasonName} 达到 {rankName}");
            return markID;
        }
        return "";
    }
    
    public void ActivateSeason(string seasonID)
    {
        // 先停用所有赛季
        foreach (Season season in markData.system.seasons)
        {
            season.Deactivate();
        }
        
        // 激活指定赛季
        Season targetSeason = markData.system.GetSeason(seasonID);
        if (targetSeason != null)
        {
            targetSeason.Activate();
            SaveMarkData();
            Debug.Log($"成功激活赛季: {targetSeason.seasonName}");
        }
    }
    
    public List<KingMark> GetPlayerMarks(string playerID)
    {
        List<KingMark> playerMarks = new List<KingMark>();
        PlayerMarkData playerData = GetOrCreatePlayerData(playerID);
        
        foreach (string markID in playerData.GetMarks())
        {
            KingMark mark = markData.system.GetMark(markID);
            if (mark != null)
            {
                playerMarks.Add(mark);
            }
        }
        
        return playerMarks;
    }
    
    public List<KingMark> GetSeasonMarks(string seasonID)
    {
        return markData.system.GetMarksBySeason(seasonID);
    }
    
    public Season GetCurrentSeason()
    {
        List<Season> activeSeasons = markData.system.GetActiveSeasons();
        return activeSeasons.Count > 0 ? activeSeasons[0] : null;
    }
    
    public List<Season> GetAllSeasons()
    {
        return markData.system.seasons;
    }
    
    public int GetTotalKingSeasons(string playerID)
    {
        PlayerMarkData playerData = GetOrCreatePlayerData(playerID);
        return playerData.GetTotalKingSeasons();
    }
    
    public string GetHighestRank(string playerID)
    {
        PlayerMarkData playerData = GetOrCreatePlayerData(playerID);
        return playerData.GetHighestRank();
    }
    
    public bool HasKingMark(string playerID, string seasonID)
    {
        List<KingMark> playerMarks = markData.system.GetMarksByPlayer(playerID);
        return playerMarks.Exists(m => m.seasonID == seasonID && m.isKing);
    }
    
    public void AddSeason(string name, string description, string start, string end, int number)
    {
        string seasonID = System.Guid.NewGuid().ToString();
        Season season = new Season(seasonID, name, description, start, end, number);
        markData.system.AddSeason(season);
        SaveMarkData();
    }
    
    private PlayerMarkData GetOrCreatePlayerData(string playerID)
    {
        PlayerMarkData playerData = markData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerMarkData(playerID);
            markData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public void SaveMarkData()
    {
        string path = Application.dataPath + "/Data/king_mark_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, markData);
        stream.Close();
    }
    
    public void LoadMarkData()
    {
        string path = Application.dataPath + "/Data/king_mark_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            markData = (KingMarkManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            markData = new KingMarkManagerData();
        }
    }
}
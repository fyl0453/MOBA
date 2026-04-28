using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameStatisticsManagerExtended : MonoBehaviour
{
    public static GameStatisticsManagerExtended Instance { get; private set; }
    
    public GameStatisticsManagerData gameStatisticsData;
    
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
        LoadGameStatisticsData();
        
        if (gameStatisticsData == null)
        {
            gameStatisticsData = new GameStatisticsManagerData();
        }
    }
    
    public void InitializePlayerStatistics(string playerID, string playerName)
    {
        PlayerStatistics existing = gameStatisticsData.system.GetPlayerStatistics(playerID);
        if (existing == null)
        {
            PlayerStatistics newStats = new PlayerStatistics(playerID, playerName);
            gameStatisticsData.system.AddPlayerStatistics(newStats);
            SaveGameStatisticsData();
            Debug.Log($"成功初始化玩家统计: {playerName}");
        }
    }
    
    public void InitializeHeroStatistics(string playerID, string heroID, string heroName)
    {
        HeroStatistics existing = gameStatisticsData.system.GetHeroStatistics(playerID, heroID);
        if (existing == null)
        {
            HeroStatistics newStats = new HeroStatistics(playerID, heroID, heroName);
            gameStatisticsData.system.AddHeroStatistics(newStats);
            SaveGameStatisticsData();
            Debug.Log($"成功初始化英雄统计: {heroName}");
        }
    }
    
    public void InitializeModeStatistics(string playerID, string modeID, string modeName)
    {
        GameModeStatistics existing = gameStatisticsData.system.GetModeStatistics(playerID, modeID);
        if (existing == null)
        {
            GameModeStatistics newStats = new GameModeStatistics(playerID, modeID, modeName);
            gameStatisticsData.system.AddModeStatistics(newStats);
            SaveGameStatisticsData();
            Debug.Log($"成功初始化模式统计: {modeName}");
        }
    }
    
    public void RecordHeroMatch(string playerID, string heroID, string heroName, bool isWin, int kills, int deaths, int assists, int damageDealt, int damageTaken, int goldEarned, bool isMVP)
    {
        HeroStatistics heroStats = gameStatisticsData.system.GetHeroStatistics(playerID, heroID);
        if (heroStats == null)
        {
            InitializeHeroStatistics(playerID, heroID, heroName);
            heroStats = gameStatisticsData.system.GetHeroStatistics(playerID, heroID);
        }
        
        if (heroStats != null)
        {
            heroStats.RecordMatch(isWin, kills, deaths, assists, damageDealt, damageTaken, goldEarned, isMVP);
        }
        
        // 同时更新玩家总体统计
        RecordPlayerMatch(playerID, isWin, kills, deaths, assists, goldEarned, damageDealt, isMVP, heroID, heroName);
        
        SaveGameStatisticsData();
    }
    
    public void RecordPlayerMatch(string playerID, bool isWin, int kills, int deaths, int assists, int gold, int damage, bool isMVP, string heroID, string heroName)
    {
        PlayerStatistics playerStats = gameStatisticsData.system.GetPlayerStatistics(playerID);
        if (playerStats == null)
        {
            InitializePlayerStatistics(playerID, heroName.Split('_')[0]);
            playerStats = gameStatisticsData.system.GetPlayerStatistics(playerID);
        }
        
        if (playerStats != null)
        {
            playerStats.RecordMatch(isWin, kills, deaths, assists, gold, damage, isMVP, heroID, heroName);
        }
        
        SaveGameStatisticsData();
    }
    
    public void RecordModeMatch(string playerID, string modeID, string modeName, bool isWin, bool isMVP)
    {
        GameModeStatistics modeStats = gameStatisticsData.system.GetModeStatistics(playerID, modeID);
        if (modeStats == null)
        {
            InitializeModeStatistics(playerID, modeID, modeName);
            modeStats = gameStatisticsData.system.GetModeStatistics(playerID, modeID);
        }
        
        if (modeStats != null)
        {
            modeStats.RecordMatch(isWin, isMVP);
        }
        
        SaveGameStatisticsData();
    }
    
    public PlayerStatistics GetPlayerStatistics(string playerID)
    {
        return gameStatisticsData.system.GetPlayerStatistics(playerID);
    }
    
    public HeroStatistics GetHeroStatistics(string playerID, string heroID)
    {
        return gameStatisticsData.system.GetHeroStatistics(playerID, heroID);
    }
    
    public GameModeStatistics GetModeStatistics(string playerID, string modeID)
    {
        return gameStatisticsData.system.GetModeStatistics(playerID, modeID);
    }
    
    public List<HeroStatistics> GetPlayerHeroStatistics(string playerID)
    {
        List<HeroStatistics> stats = new List<HeroStatistics>();
        foreach (HeroStatistics heroStats in gameStatisticsData.system.heroStatistics)
        {
            if (heroStats.playerID == playerID)
            {
                stats.Add(heroStats);
            }
        }
        return stats;
    }
    
    public List<HeroStatistics> GetTopPlayedHeroes(string playerID, int limit = 5)
    {
        List<HeroStatistics> stats = GetPlayerHeroStatistics(playerID);
        stats.Sort((a, b) => b.totalMatches.CompareTo(a.totalMatches));
        return stats.GetRange(0, Mathf.Min(limit, stats.Count));
    }
    
    public List<HeroStatistics> GetTopWinRateHeroes(string playerID, int minMatches = 5, int limit = 5)
    {
        List<HeroStatistics> stats = GetPlayerHeroStatistics(playerID);
        stats.RemoveAll(h => h.totalMatches < minMatches);
        stats.Sort((a, b) => b.GetWinRate().CompareTo(a.GetWinRate()));
        return stats.GetRange(0, Mathf.Min(limit, stats.Count));
    }
    
    public List<HeroStatistics> GetTopKDAHeroes(string playerID, int minMatches = 5, int limit = 5)
    {
        List<HeroStatistics> stats = GetPlayerHeroStatistics(playerID);
        stats.RemoveAll(h => h.totalMatches < minMatches);
        stats.Sort((a, b) => b.GetKDA().CompareTo(a.GetKDA()));
        return stats.GetRange(0, Mathf.Min(limit, stats.Count));
    }
    
    public void SaveGameStatisticsData()
    {
        string path = Application.dataPath + "/Data/game_statistics_extended_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, gameStatisticsData);
        stream.Close();
    }
    
    public void LoadGameStatisticsData()
    {
        string path = Application.dataPath + "/Data/game_statistics_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            gameStatisticsData = (GameStatisticsManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            gameStatisticsData = new GameStatisticsManagerData();
        }
    }
}
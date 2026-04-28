using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataStatisticsManager : MonoBehaviour
{
    public static DataStatisticsManager Instance { get; private set; }
    
    public DataStatisticsManagerData statisticsData;
    
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
        LoadStatisticsData();
        
        if (statisticsData == null)
        {
            statisticsData = new DataStatisticsManagerData();
            InitializeDefaultData();
        }
    }
    
    private void InitializeDefaultData()
    {
        // 创建默认游戏统计数据
        GameStatistics gameStats = new GameStatistics("game_main");
        statisticsData.AddGameStats(gameStats);
        
        SaveStatisticsData();
    }
    
    public void RecordEvent(string eventType, string playerID, string eventData, string location = "")
    {
        if (!statisticsData.system.isEnabled)
        {
            return;
        }
        
        string eventID = System.Guid.NewGuid().ToString();
        DataEvent dataEvent = new DataEvent(eventID, eventType, playerID, eventData, location);
        statisticsData.system.AddDataEvent(dataEvent);
        
        // 每100个事件保存一次数据
        if (statisticsData.system.dataEvents.Count % 100 == 0)
        {
            SaveStatisticsData();
        }
    }
    
    public void RecordMatch(string playerID, string playerName, int kills, int deaths, int assists, int gold, int damage, int healing, bool won, int matchTime, string heroID)
    {
        if (!statisticsData.system.isEnabled)
        {
            return;
        }
        
        // 更新玩家统计数据
        PlayerStatistics playerStats = GetOrCreatePlayerStats(playerID, playerName);
        playerStats.UpdateMatchStats(kills, deaths, assists, gold, damage, healing, won, matchTime);
        playerStats.UpdateHeroStats(heroID, kills, deaths, assists, won);
        
        // 更新游戏统计数据
        GameStatistics gameStats = statisticsData.GetGameStats("game_main");
        if (gameStats != null)
        {
            float kda = deaths > 0 ? (float)(kills + assists) / deaths : (float)(kills + assists);
            gameStats.UpdateGameStats(matchTime, won ? 1f : 0f, kda, 0);
        }
        
        // 记录匹配事件
        string eventData = $"kills:{kills},deaths:{deaths},assists:{assists},gold:{gold},damage:{damage},healing:{healing},won:{won},matchTime:{matchTime},heroID:{heroID}";
        RecordEvent("match_complete", playerID, eventData);
        
        SaveStatisticsData();
    }
    
    public void RecordPurchase(string playerID, string playerName, string itemID, int amount, string currencyType)
    {
        if (!statisticsData.system.isEnabled)
        {
            return;
        }
        
        // 更新游戏收入
        GameStatistics gameStats = statisticsData.GetGameStats("game_main");
        if (gameStats != null)
        {
            gameStats.UpdateGameStats(0, 0f, 0f, amount);
        }
        
        // 记录购买事件
        string eventData = $"itemID:{itemID},amount:{amount},currencyType:{currencyType}";
        RecordEvent("purchase", playerID, eventData);
        
        SaveStatisticsData();
    }
    
    public void RecordLogin(string playerID, string playerName)
    {
        if (!statisticsData.system.isEnabled)
        {
            return;
        }
        
        // 确保玩家统计数据存在
        GetOrCreatePlayerStats(playerID, playerName);
        
        // 记录登录事件
        RecordEvent("login", playerID, "");
        
        // 更新在线玩家数
        GameStatistics gameStats = statisticsData.GetGameStats("game_main");
        if (gameStats != null)
        {
            // 这里应该获取实际的在线玩家数，这里简化处理
            gameStats.UpdatePlayerCount(1);
        }
    }
    
    public void RecordLogout(string playerID)
    {
        if (!statisticsData.system.isEnabled)
        {
            return;
        }
        
        // 记录登出事件
        RecordEvent("logout", playerID, "");
    }
    
    public void GenerateReport(string reportName, string reportType, string reportData, string description = "")
    {
        string reportID = System.Guid.NewGuid().ToString();
        DataReport report = new DataReport(reportID, reportName, reportType, reportData, description);
        statisticsData.system.AddDataReport(report);
        SaveStatisticsData();
    }
    
    public PlayerStatistics GetPlayerStatistics(string playerID)
    {
        return statisticsData.GetPlayerStats(playerID);
    }
    
    public GameStatistics GetGameStatistics(string gameID = "game_main")
    {
        return statisticsData.GetGameStats(gameID);
    }
    
    public List<DataEvent> GetEventsByType(string eventType)
    {
        return statisticsData.system.GetEventsByType(eventType);
    }
    
    public List<DataEvent> GetEventsByPlayer(string playerID)
    {
        return statisticsData.system.GetEventsByPlayer(playerID);
    }
    
    public DataReport GetReport(string reportID)
    {
        return statisticsData.system.GetReport(reportID);
    }
    
    public void EnableStatistics(bool enable)
    {
        statisticsData.system.isEnabled = enable;
        SaveStatisticsData();
    }
    
    public bool IsStatisticsEnabled()
    {
        return statisticsData.system.isEnabled;
    }
    
    private PlayerStatistics GetOrCreatePlayerStats(string playerID, string playerName)
    {
        PlayerStatistics playerStats = statisticsData.GetPlayerStats(playerID);
        if (playerStats == null)
        {
            playerStats = new PlayerStatistics(playerID, playerName);
            statisticsData.AddPlayerStats(playerStats);
        }
        return playerStats;
    }
    
    public void SaveStatisticsData()
    {
        string path = Application.dataPath + "/Data/statistics_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, statisticsData);
        stream.Close();
    }
    
    public void LoadStatisticsData()
    {
        string path = Application.dataPath + "/Data/statistics_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            statisticsData = (DataStatisticsManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            statisticsData = new DataStatisticsManagerData();
        }
    }
}
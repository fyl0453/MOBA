using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }
    
    public GameModeManagerData gameModeData;
    
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
        LoadGameModeData();
        
        if (gameModeData == null)
        {
            gameModeData = new GameModeManagerData();
            InitializeDefaultGameModes();
        }
    }
    
    private void InitializeDefaultGameModes()
    {
        // 创建地图
        Map summonerRift = new Map("map_summoner_rift", "召唤师峡谷", "经典5v5地图", 10000);
        summonerRift.AddSpawnPoint(new Vector3(-4000, 0, -4000), "Blue");
        summonerRift.AddSpawnPoint(new Vector3(4000, 0, 4000), "Red");
        summonerRift.AddObjective("Dragon", new Vector3(2000, 0, -2000));
        summonerRift.AddObjective("Baron", new Vector3(-2000, 0, 2000));
        summonerRift.AddTurret(new Vector3(-3000, 0, -3000), "Blue");
        summonerRift.AddTurret(new Vector3(3000, 0, 3000), "Red");
        summonerRift.AddBase(new Vector3(-4500, 0, -4500), "Blue");
        summonerRift.AddBase(new Vector3(4500, 0, 4500), "Red");
        gameModeData.AddMap(summonerRift);
        
        Map howlingAbyss = new Map("map_howling_abyss", "嚎哭深渊", "快节奏3v3地图", 5000);
        howlingAbyss.AddSpawnPoint(new Vector3(-2000, 0, 0), "Blue");
        howlingAbyss.AddSpawnPoint(new Vector3(2000, 0, 0), "Red");
        howlingAbyss.AddTurret(new Vector3(-1500, 0, 0), "Blue");
        howlingAbyss.AddTurret(new Vector3(1500, 0, 0), "Red");
        howlingAbyss.AddBase(new Vector3(-2000, 0, 0), "Blue");
        howlingAbyss.AddBase(new Vector3(2000, 0, 0), "Red");
        gameModeData.AddMap(howlingAbyss);
        
        // 创建游戏模式
        GameMode classic = new GameMode("mode_classic", "经典模式", "5v5经典对战", 10, 10, 1);
        classic.AddRule("5v5对战", "两支队伍各5名玩家进行对战");
        classic.AddRule("推塔", "摧毁敌方基地获得胜利");
        classic.AddRule("野怪", "击杀野怪获得Buff");
        classic.AddReward("Currency", "gold", 100);
        classic.AddReward("Experience", "exp", 100);
        gameModeData.AddGameMode(classic);
        
        GameMode ranked = new GameMode("mode_ranked", "排位赛", "5v5排位对战", 10, 10, 1);
        ranked.AddRule("5v5对战", "两支队伍各5名玩家进行对战");
        ranked.AddRule("推塔", "摧毁敌方基地获得胜利");
        ranked.AddRule("段位", "根据胜负调整段位");
        ranked.AddReward("Currency", "gold", 150);
        ranked.AddReward("Experience", "exp", 150);
        ranked.AddReward("RankPoints", "rank_points", 20);
        gameModeData.AddGameMode(ranked);
        
        GameMode aram = new GameMode("mode_aram", "大乱斗", "3v3快节奏对战", 6, 6, 2);
        aram.AddRule("3v3对战", "两支队伍各3名玩家进行对战");
        aram.AddRule("快节奏", "地图较小，节奏更快");
        aram.AddRule("随机英雄", "随机分配英雄");
        aram.AddReward("Currency", "gold", 80);
        aram.AddReward("Experience", "exp", 80);
        gameModeData.AddGameMode(aram);
        
        GameMode practice = new GameMode("mode_practice", "训练模式", "单人或多人训练", 1, 10, 1);
        practice.AddRule("自由练习", "可以自由练习英雄和技能");
        practice.AddRule("无经验", "不会获得经验和金币");
        practice.AddRule("自定义", "可以调整各种参数");
        gameModeData.AddGameMode(practice);
        
        GameMode tournament = new GameMode("mode_tournament", " tournament", " tournament对战", 10, 10, 1);
        tournament.AddRule("5v5对战", "两支队伍各5名玩家进行对战");
        tournament.AddRule("单淘汰赛", "输掉比赛即淘汰");
        tournament.AddRule("奖励丰厚", "获得专属奖励");
        tournament.AddReward("Currency", "gold", 500);
        tournament.AddReward("Currency", "gems", 100);
        tournament.AddReward("Skin", "skin_tournament", 1);
        tournament.isEnabled = false;
        gameModeData.AddGameMode(tournament);
        
        SaveGameModeData();
    }
    
    public GameMode GetGameMode(string modeID)
    {
        return gameModeData.GetGameMode(modeID);
    }
    
    public List<GameMode> GetEnabledGameModes()
    {
        return gameModeData.GetEnabledGameModes();
    }
    
    public Map GetMap(string mapID)
    {
        return gameModeData.GetMap(mapID);
    }
    
    public List<Map> GetAllMaps()
    {
        return gameModeData.GetAllMaps();
    }
    
    public void EnableGameMode(string modeID, bool enable)
    {
        GameMode mode = gameModeData.GetGameMode(modeID);
        if (mode != null)
        {
            mode.isEnabled = enable;
            SaveGameModeData();
        }
    }
    
    public void StartGame(string modeID, List<string> playerIDs)
    {
        GameMode mode = gameModeData.GetGameMode(modeID);
        if (mode != null && mode.isEnabled && playerIDs.Count >= mode.minPlayers && playerIDs.Count <= mode.maxPlayers)
        {
            // 这里可以添加开始游戏的逻辑
            Debug.Log("Starting game mode: " + mode.modeName + " with " + playerIDs.Count + " players");
        }
    }
    
    public void SaveGameModeData()
    {
        string path = Application.dataPath + "/Data/game_mode_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, gameModeData);
        stream.Close();
    }
    
    public void LoadGameModeData()
    {
        string path = Application.dataPath + "/Data/game_mode_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            gameModeData = (GameModeManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            gameModeData = new GameModeManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameModeSystemDetailedManager : MonoBehaviour
{
    public static GameModeSystemDetailedManager Instance { get; private set; }
    
    public GameModeSystemDetailedManagerData gameModeData;
    
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
            gameModeData = new GameModeSystemDetailedManagerData();
            InitializeDefaultGameModeSystem();
        }
    }
    
    private void InitializeDefaultGameModeSystem()
    {
        // 游戏模式
        GameMode gameMode1 = new GameMode("game_mode_001", "排位赛", "competitive", "5v5竞技模式，影响段位", 10, 10, 300, true, false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31", "icons/game_mode_001", "audio/bgm/ranked");
        GameMode gameMode2 = new GameMode("game_mode_002", "匹配赛", "casual", "5v5休闲模式，不影响段位", 10, 10, 300, false, false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31", "icons/game_mode_002", "audio/bgm/casual");
        GameMode gameMode3 = new GameMode("game_mode_003", "火焰山大战", "entertainment", "5v5娱乐模式，地图为火焰山", 10, 10, 180, false, false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31", "icons/game_mode_003", "audio/bgm/flaming_mountain");
        GameMode gameMode4 = new GameMode("game_mode_004", "无限乱斗", "entertainment", "5v5娱乐模式，技能冷却加快", 10, 10, 240, false, false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31", "icons/game_mode_004", "audio/bgm/infinite_brawl");
        GameMode gameMode5 = new GameMode("game_mode_005", "克隆大作战", "entertainment", "5v5娱乐模式，双方使用相同英雄", 10, 10, 240, false, false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31", "icons/game_mode_005", "audio/bgm/clone_battle");
        GameMode gameMode6 = new GameMode("game_mode_006", "大乱斗", "entertainment", "5v5娱乐模式，随机英雄", 10, 10, 200, false, false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31", "icons/game_mode_006", "audio/bgm/brawl");
        GameMode gameMode7 = new GameMode("game_mode_007", "1v1模式", "solo", "1v1单挑模式", 2, 2, 120, false, false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31", "icons/game_mode_007", "audio/bgm/solo");
        GameMode gameMode8 = new GameMode("game_mode_008", "3v3模式", "team", "3v3团队模式", 6, 6, 180, false, false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31", "icons/game_mode_008", "audio/bgm/team3v3");
        
        gameModeData.system.AddGameMode(gameMode1);
        gameModeData.system.AddGameMode(gameMode2);
        gameModeData.system.AddGameMode(gameMode3);
        gameModeData.system.AddGameMode(gameMode4);
        gameModeData.system.AddGameMode(gameMode5);
        gameModeData.system.AddGameMode(gameMode6);
        gameModeData.system.AddGameMode(gameMode7);
        gameModeData.system.AddGameMode(gameMode8);
        
        // 游戏规则
        GameRule rule1 = new GameRule("rule_001", "胜利条件", "victory", "摧毁敌方基地", "string", "destroy_base");
        GameRule rule2 = new GameRule("rule_002", "失败条件", "defeat", "我方基地被摧毁", "string", "base_destroyed");
        GameRule rule3 = new GameRule("rule_003", "最大游戏时间", "time_limit", "游戏最大时长", "int", "300");
        GameRule rule4 = new GameRule("rule_004", "重生时间", "respawn_time", "英雄死亡后重生时间", "int", "10");
        GameRule rule5 = new GameRule("rule_005", "金币获取", "gold_rate", "金币获取速率", "float", "1.0");
        GameRule rule6 = new GameRule("rule_006", "经验获取", "exp_rate", "经验获取速率", "float", "1.0");
        GameRule rule7 = new GameRule("rule_007", "技能冷却", "cooldown_rate", "技能冷却速率", "float", "1.0");
        GameRule rule8 = new GameRule("rule_008", "防御塔伤害", "tower_damage", "防御塔伤害倍率", "float", "1.0");
        
        gameModeData.system.AddGameRule(rule1);
        gameModeData.system.AddGameRule(rule2);
        gameModeData.system.AddGameRule(rule3);
        gameModeData.system.AddGameRule(rule4);
        gameModeData.system.AddGameRule(rule5);
        gameModeData.system.AddGameRule(rule6);
        gameModeData.system.AddGameRule(rule7);
        gameModeData.system.AddGameRule(rule8);
        
        // 添加规则到游戏模式
        gameMode1.AddRule("rule_001");
        gameMode1.AddRule("rule_002");
        gameMode1.AddRule("rule_003");
        gameMode1.AddRule("rule_004");
        gameMode1.AddRule("rule_005");
        gameMode1.AddRule("rule_006");
        gameMode1.AddRule("rule_007");
        gameMode1.AddRule("rule_008");
        
        gameMode2.AddRule("rule_001");
        gameMode2.AddRule("rule_002");
        gameMode2.AddRule("rule_003");
        gameMode2.AddRule("rule_004");
        gameMode2.AddRule("rule_005");
        gameMode2.AddRule("rule_006");
        gameMode2.AddRule("rule_007");
        gameMode2.AddRule("rule_008");
        
        gameMode3.AddRule("rule_001");
        gameMode3.AddRule("rule_002");
        gameMode3.AddRule("rule_003");
        gameMode3.AddRule("rule_004");
        
        gameMode4.AddRule("rule_001");
        gameMode4.AddRule("rule_002");
        gameMode4.AddRule("rule_003");
        gameMode4.AddRule("rule_004");
        
        // 游戏地图
        GameMap map1 = new GameMap("map_001", "王者峡谷", "game_mode_001", "5v5标准地图", "standard", 10000, 10000, "maps/kings_canyon", "maps/minimaps/kings_canyon");
        map1.AddSpawnPoint("(1000, 1000)");
        map1.AddSpawnPoint("(9000, 9000)");
        map1.AddObjectivePoint("(5000, 5000)");
        
        GameMap map2 = new GameMap("map_002", "王者峡谷", "game_mode_002", "5v5标准地图", "standard", 10000, 10000, "maps/kings_canyon", "maps/minimaps/kings_canyon");
        map2.AddSpawnPoint("(1000, 1000)");
        map2.AddSpawnPoint("(9000, 9000)");
        map2.AddObjectivePoint("(5000, 5000)");
        
        GameMap map3 = new GameMap("map_003", "火焰山", "game_mode_003", "5v5娱乐地图", "entertainment", 8000, 8000, "maps/flaming_mountain", "maps/minimaps/flaming_mountain");
        map3.AddSpawnPoint("(1000, 1000)");
        map3.AddSpawnPoint("(7000, 7000)");
        map3.AddObjectivePoint("(4000, 4000)");
        
        GameMap map4 = new GameMap("map_004", "王者峡谷", "game_mode_004", "5v5标准地图", "standard", 10000, 10000, "maps/kings_canyon", "maps/minimaps/kings_canyon");
        map4.AddSpawnPoint("(1000, 1000)");
        map4.AddSpawnPoint("(9000, 9000)");
        map4.AddObjectivePoint("(5000, 5000)");
        
        GameMap map5 = new GameMap("map_005", "王者峡谷", "game_mode_005", "5v5标准地图", "standard", 10000, 10000, "maps/kings_canyon", "maps/minimaps/kings_canyon");
        map5.AddSpawnPoint("(1000, 1000)");
        map5.AddSpawnPoint("(9000, 9000)");
        map5.AddObjectivePoint("(5000, 5000)");
        
        GameMap map6 = new GameMap("map_006", "大乱斗地图", "game_mode_006", "5v5娱乐地图", "entertainment", 6000, 6000, "maps/brawl", "maps/minimaps/brawl");
        map6.AddSpawnPoint("(1000, 3000)");
        map6.AddSpawnPoint("(5000, 3000)");
        map6.AddObjectivePoint("(3000, 3000)");
        
        GameMap map7 = new GameMap("map_007", "1v1地图", "game_mode_007", "1v1单挑地图", "solo", 4000, 4000, "maps/solo", "maps/minimaps/solo");
        map7.AddSpawnPoint("(1000, 2000)");
        map7.AddSpawnPoint("(3000, 2000)");
        map7.AddObjectivePoint("(2000, 2000)");
        
        GameMap map8 = new GameMap("map_008", "3v3地图", "game_mode_008", "3v3团队地图", "team", 6000, 6000, "maps/team3v3", "maps/minimaps/team3v3");
        map8.AddSpawnPoint("(1000, 1000)");
        map8.AddSpawnPoint("(5000, 5000)");
        map8.AddObjectivePoint("(3000, 3000)");
        
        gameModeData.system.AddGameMap(map1);
        gameModeData.system.AddGameMap(map2);
        gameModeData.system.AddGameMap(map3);
        gameModeData.system.AddGameMap(map4);
        gameModeData.system.AddGameMap(map5);
        gameModeData.system.AddGameMap(map6);
        gameModeData.system.AddGameMap(map7);
        gameModeData.system.AddGameMap(map8);
        
        // 添加地图到游戏模式
        gameMode1.AddMap("map_001");
        gameMode2.AddMap("map_002");
        gameMode3.AddMap("map_003");
        gameMode4.AddMap("map_004");
        gameMode5.AddMap("map_005");
        gameMode6.AddMap("map_006");
        gameMode7.AddMap("map_007");
        gameMode8.AddMap("map_008");
        
        // 游戏事件
        GameEvent event1 = new GameEvent("event_001", "首血", "game_mode_001", "第一次击杀", "kill", "first_kill", "give_bonus", System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        GameEvent event2 = new GameEvent("event_002", "双杀", "game_mode_001", "短时间内击杀两名敌人", "kill", "double_kill", "give_bonus", System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        GameEvent event3 = new GameEvent("event_003", "三杀", "game_mode_001", "短时间内击杀三名敌人", "kill", "triple_kill", "give_bonus", System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        GameEvent event4 = new GameEvent("event_004", "四杀", "game_mode_001", "短时间内击杀四名敌人", "kill", "quadra_kill", "give_bonus", System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        GameEvent event5 = new GameEvent("event_005", "五杀", "game_mode_001", "短时间内击杀五名敌人", "kill", "penta_kill", "give_bonus", System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        
        gameModeData.system.AddGameEvent(event1);
        gameModeData.system.AddGameEvent(event2);
        gameModeData.system.AddGameEvent(event3);
        gameModeData.system.AddGameEvent(event4);
        gameModeData.system.AddGameEvent(event5);
        
        // 玩家游戏模式统计
        PlayerGameModeStats stats1 = new PlayerGameModeStats("stats_001", "user_001", "game_mode_001");
        stats1.AddGame(true, 5, 2, 3);
        stats1.AddGame(false, 3, 4, 2);
        stats1.AddGame(true, 7, 1, 4);
        
        PlayerGameModeStats stats2 = new PlayerGameModeStats("stats_002", "user_001", "game_mode_002");
        stats2.AddGame(true, 4, 3, 5);
        stats2.AddGame(true, 6, 2, 3);
        
        PlayerGameModeStats stats3 = new PlayerGameModeStats("stats_003", "user_002", "game_mode_001");
        stats3.AddGame(false, 2, 5, 1);
        stats3.AddGame(true, 5, 3, 2);
        
        gameModeData.system.AddPlayerGameModeStats(stats1);
        gameModeData.system.AddPlayerGameModeStats(stats2);
        gameModeData.system.AddPlayerGameModeStats(stats3);
        
        SaveGameModeData();
    }
    
    // 游戏模式管理
    public void AddGameMode(string name, string type, string desc, int minPlayers, int maxPlayers, int matchTimeLimit, bool isRanked, bool isLimited, string releaseDate, string expiryDate, string iconPath, string bgmPath)
    {
        string gameModeID = "game_mode_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GameMode gameMode = new GameMode(gameModeID, name, type, desc, minPlayers, maxPlayers, matchTimeLimit, isRanked, isLimited, releaseDate, expiryDate, iconPath, bgmPath);
        gameModeData.system.AddGameMode(gameMode);
        SaveGameModeData();
        Debug.Log("成功添加游戏模式: " + name);
    }
    
    public List<GameMode> GetGameModesByType(string type)
    {
        return gameModeData.system.GetGameModesByType(type);
    }
    
    public List<GameMode> GetActiveGameModes()
    {
        return gameModeData.system.GetActiveGameModes();
    }
    
    public List<GameMode> GetAllGameModes()
    {
        return gameModeData.system.gameModes;
    }
    
    public void EnableGameMode(string gameModeID)
    {
        GameMode gameMode = gameModeData.system.GetGameMode(gameModeID);
        if (gameMode != null)
        {
            gameMode.isEnabled = true;
            SaveGameModeData();
            Debug.Log("成功启用游戏模式: " + gameMode.gameModeName);
        }
        else
        {
            Debug.LogError("游戏模式不存在: " + gameModeID);
        }
    }
    
    public void DisableGameMode(string gameModeID)
    {
        GameMode gameMode = gameModeData.system.GetGameMode(gameModeID);
        if (gameMode != null)
        {
            gameMode.isEnabled = false;
            SaveGameModeData();
            Debug.Log("成功禁用游戏模式: " + gameMode.gameModeName);
        }
        else
        {
            Debug.LogError("游戏模式不存在: " + gameModeID);
        }
    }
    
    // 游戏规则管理
    public void AddGameRule(string name, string type, string desc, string valueType, string value)
    {
        string ruleID = "rule_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GameRule rule = new GameRule(ruleID, name, type, desc, valueType, value);
        gameModeData.system.AddGameRule(rule);
        SaveGameModeData();
        Debug.Log("成功添加游戏规则: " + name);
    }
    
    public void AddRuleToGameMode(string gameModeID, string ruleID)
    {
        GameMode gameMode = gameModeData.system.GetGameMode(gameModeID);
        if (gameMode != null)
        {
            gameMode.AddRule(ruleID);
            SaveGameModeData();
            Debug.Log("成功为游戏模式添加规则: " + ruleID);
        }
        else
        {
            Debug.LogError("游戏模式不存在: " + gameModeID);
        }
    }
    
    public void EnableGameRule(string ruleID)
    {
        GameRule rule = gameModeData.system.GetGameRule(ruleID);
        if (rule != null)
        {
            rule.Enable();
            SaveGameModeData();
            Debug.Log("成功启用游戏规则: " + rule.ruleName);
        }
        else
        {
            Debug.LogError("游戏规则不存在: " + ruleID);
        }
    }
    
    public void DisableGameRule(string ruleID)
    {
        GameRule rule = gameModeData.system.GetGameRule(ruleID);
        if (rule != null)
        {
            rule.Disable();
            SaveGameModeData();
            Debug.Log("成功禁用游戏规则: " + rule.ruleName);
        }
        else
        {
            Debug.LogError("游戏规则不存在: " + ruleID);
        }
    }
    
    // 游戏地图管理
    public void AddGameMap(string name, string gameModeID, string desc, string mapType, int mapSizeX, int mapSizeY, string mapPath, string minimapPath)
    {
        string mapID = "map_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GameMap map = new GameMap(mapID, name, gameModeID, desc, mapType, mapSizeX, mapSizeY, mapPath, minimapPath);
        gameModeData.system.AddGameMap(map);
        
        GameMode gameMode = gameModeData.system.GetGameMode(gameModeID);
        if (gameMode != null)
        {
            gameMode.AddMap(mapID);
        }
        
        SaveGameModeData();
        Debug.Log("成功添加游戏地图: " + name);
    }
    
    public List<GameMap> GetMapsByGameMode(string gameModeID)
    {
        return gameModeData.system.GetMapsByGameMode(gameModeID);
    }
    
    public void EnableGameMap(string mapID)
    {
        GameMap map = gameModeData.system.GetGameMap(mapID);
        if (map != null)
        {
            map.Enable();
            SaveGameModeData();
            Debug.Log("成功启用游戏地图: " + map.mapName);
        }
        else
        {
            Debug.LogError("游戏地图不存在: " + mapID);
        }
    }
    
    public void DisableGameMap(string mapID)
    {
        GameMap map = gameModeData.system.GetGameMap(mapID);
        if (map != null)
        {
            map.Disable();
            SaveGameModeData();
            Debug.Log("成功禁用游戏地图: " + map.mapName);
        }
        else
        {
            Debug.LogError("游戏地图不存在: " + mapID);
        }
    }
    
    // 游戏事件管理
    public void AddGameEvent(string name, string gameModeID, string desc, string eventType, string triggerCondition, string action, string startDate, string endDate)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GameEvent gameEvent = new GameEvent(eventID, name, gameModeID, desc, eventType, triggerCondition, action, startDate, endDate);
        gameModeData.system.AddGameEvent(gameEvent);
        SaveGameModeData();
        Debug.Log("成功添加游戏事件: " + name);
    }
    
    public void EnableGameEvent(string eventID)
    {
        GameEvent gameEvent = gameModeData.system.GetGameEvent(eventID);
        if (gameEvent != null)
        {
            gameEvent.Enable();
            SaveGameModeData();
            Debug.Log("成功启用游戏事件: " + gameEvent.eventName);
        }
        else
        {
            Debug.LogError("游戏事件不存在: " + eventID);
        }
    }
    
    public void DisableGameEvent(string eventID)
    {
        GameEvent gameEvent = gameModeData.system.GetGameEvent(eventID);
        if (gameEvent != null)
        {
            gameEvent.Disable();
            SaveGameModeData();
            Debug.Log("成功禁用游戏事件: " + gameEvent.eventName);
        }
        else
        {
            Debug.LogError("游戏事件不存在: " + eventID);
        }
    }
    
    // 玩家游戏模式统计管理
    public void AddPlayerGameModeStats(string playerID, string gameModeID)
    {
        PlayerGameModeStats existing = gameModeData.system.GetPlayerGameModeStats(playerID, gameModeID);
        if (existing == null)
        {
            string statsID = "stats_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            PlayerGameModeStats stats = new PlayerGameModeStats(statsID, playerID, gameModeID);
            gameModeData.system.AddPlayerGameModeStats(stats);
            SaveGameModeData();
            Debug.Log("成功添加玩家游戏模式统计: " + playerID + " - " + gameModeID);
        }
    }
    
    public void RecordGameResult(string playerID, string gameModeID, bool won, int kills, int deaths, int assists)
    {
        PlayerGameModeStats stats = gameModeData.system.GetPlayerGameModeStats(playerID, gameModeID);
        if (stats != null)
        {
            stats.AddGame(won, kills, deaths, assists);
            SaveGameModeData();
            Debug.Log("成功记录游戏结果: " + (won ? "胜利" : "失败"));
        }
        else
        {
            AddPlayerGameModeStats(playerID, gameModeID);
            RecordGameResult(playerID, gameModeID, won, kills, deaths, assists);
        }
    }
    
    public List<PlayerGameModeStats> GetPlayerStats(string playerID)
    {
        return gameModeData.system.GetPlayerStats(playerID);
    }
    
    public PlayerGameModeStats GetPlayerGameModeStats(string playerID, string gameModeID)
    {
        return gameModeData.system.GetPlayerGameModeStats(playerID, gameModeID);
    }
    
    // 数据持久化
    public void SaveGameModeData()
    {
        string path = Application.dataPath + "/Data/game_mode_system_detailed_data.dat";
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
        string path = Application.dataPath + "/Data/game_mode_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            gameModeData = (GameModeSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            gameModeData = new GameModeSystemDetailedManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EntertainmentModeDetailedManager : MonoBehaviour
{
    public static EntertainmentModeDetailedManager Instance { get; private set; }
    
    public EntertainmentModeDetailedManagerData entertainmentModeData;
    
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
        LoadEntertainmentModeData();
        
        if (entertainmentModeData == null)
        {
            entertainmentModeData = new EntertainmentModeDetailedManagerData();
            InitializeDefaultEntertainmentModes();
        }
    }
    
    private void InitializeDefaultEntertainmentModes()
    {
        // 火焰山大战
        ModeMap fireMountainMap = new ModeMap("map_fire_mountain", "mode_fire_mountain", "火焰山", "火山口地图，狭窄的战斗空间", "battle", 1000);
        fireMountainMap.AddSpawnPoint(new SpawnPoint("spawn_red_1", "red", 100, 0, 100));
        fireMountainMap.AddSpawnPoint(new SpawnPoint("spawn_blue_1", "blue", -100, 0, -100));
        fireMountainMap.AddFeature(new MapFeature("feature_volcano", "火山口", "obstacle", 0, 0, 0, "地图中心的火山口，会喷出岩浆"));
        fireMountainMap.AddFeature(new MapFeature("feature_lava", "岩浆", "hazard", 50, 0, 50, "会造成持续伤害的岩浆区域"));
        entertainmentModeData.system.AddMap(fireMountainMap);
        
        ModeRule fireMountainRule1 = new ModeRule("rule_fire_mountain_1", "mode_fire_mountain", "岩浆伤害", "岩浆区域会对玩家造成持续伤害");
        ModeRule fireMountainRule2 = new ModeRule("rule_fire_mountain_2", "mode_fire_mountain", "技能强化", "所有技能冷却时间缩短");
        ModeRule fireMountainRule3 = new ModeRule("rule_fire_mountain_3", "mode_fire_mountain", "击杀方式", "将敌人推入岩浆即可击杀");
        entertainmentModeData.system.AddRule(fireMountainRule1);
        entertainmentModeData.system.AddRule(fireMountainRule2);
        entertainmentModeData.system.AddRule(fireMountainRule3);
        
        EntertainmentMode fireMountainMode = new EntertainmentMode("mode_fire_mountain", "火焰山大战", "在火山口地图中战斗，将敌人推入岩浆击杀", "battle", 10, 10, 10, "map_fire_mountain");
        fireMountainMode.AddRule("rule_fire_mountain_1");
        fireMountainMode.AddRule("rule_fire_mountain_2");
        fireMountainMode.AddRule("rule_fire_mountain_3");
        fireMountainMode.AddFeature("岩浆区域");
        fireMountainMode.AddFeature("技能强化");
        fireMountainMode.AddFeature("快速战斗");
        entertainmentModeData.system.AddMode(fireMountainMode);
        
        // 无限乱斗
        ModeMap chaosMap = new ModeMap("map_chaos", "mode_chaos", "大乱斗地图", "广阔的战斗空间，随机Buff", "battle", 2000);
        chaosMap.AddSpawnPoint(new SpawnPoint("spawn_red_2", "red", 200, 0, 200));
        chaosMap.AddSpawnPoint(new SpawnPoint("spawn_blue_2", "blue", -200, 0, -200));
        chaosMap.AddFeature(new MapFeature("feature_buff", "随机Buff", "powerup", 0, 0, 0, "地图中会随机出现各种Buff"));
        entertainmentModeData.system.AddMap(chaosMap);
        
        ModeRule chaosRule1 = new ModeRule("rule_chaos_1", "mode_chaos", "随机Buff", "地图中会随机出现各种Buff");
        ModeRule chaosRule2 = new ModeRule("rule_chaos_2", "mode_chaos", "技能强化", "所有技能冷却时间缩短，伤害增加");
        ModeRule chaosRule3 = new ModeRule("rule_chaos_3", "mode_chaos", "快速复活", "死亡后快速复活，回到战场");
        entertainmentModeData.system.AddRule(chaosRule1);
        entertainmentModeData.system.AddRule(chaosRule2);
        entertainmentModeData.system.AddRule(chaosRule3);
        
        EntertainmentMode chaosMode = new EntertainmentMode("mode_chaos", "无限乱斗", "随机Buff，技能强化，快速战斗", "battle", 10, 10, 15, "map_chaos");
        chaosMode.AddRule("rule_chaos_1");
        chaosMode.AddRule("rule_chaos_2");
        chaosMode.AddRule("rule_chaos_3");
        chaosMode.AddFeature("随机Buff");
        chaosMode.AddFeature("技能强化");
        chaosMode.AddFeature("快速复活");
        entertainmentModeData.system.AddMode(chaosMode);
        
        // 克隆大作战
        ModeMap cloneMap = new ModeMap("map_clone", "mode_clone", "克隆地图", "标准地图，所有玩家使用相同英雄", "battle", 1500);
        cloneMap.AddSpawnPoint(new SpawnPoint("spawn_red_3", "red", 150, 0, 150));
        cloneMap.AddSpawnPoint(new SpawnPoint("spawn_blue_3", "blue", -150, 0, -150));
        entertainmentModeData.system.AddMap(cloneMap);
        
        ModeRule cloneRule1 = new ModeRule("rule_clone_1", "mode_clone", "相同英雄", "同一队伍的玩家使用相同英雄");
        ModeRule cloneRule2 = new ModeRule("rule_clone_2", "mode_clone", "英雄选择", "通过投票选择队伍使用的英雄");
        ModeRule cloneRule3 = new ModeRule("rule_clone_3", "mode_clone", "标准规则", "使用标准的MOBA游戏规则");
        entertainmentModeData.system.AddRule(cloneRule1);
        entertainmentModeData.system.AddRule(cloneRule2);
        entertainmentModeData.system.AddRule(cloneRule3);
        
        EntertainmentMode cloneMode = new EntertainmentMode("mode_clone", "克隆大作战", "同一队伍使用相同英雄，通过投票选择", "battle", 10, 10, 20, "map_clone");
        cloneMode.AddRule("rule_clone_1");
        cloneMode.AddRule("rule_clone_2");
        cloneMode.AddRule("rule_clone_3");
        cloneMode.AddFeature("相同英雄");
        cloneMode.AddFeature("英雄投票");
        cloneMode.AddFeature("标准规则");
        entertainmentModeData.system.AddMode(cloneMode);
        
        // 大乱斗
        ModeMap brawlMap = new ModeMap("map_brawl", "mode_brawl", "大乱斗地图", "随机英雄，快速战斗", "battle", 1200);
        brawlMap.AddSpawnPoint(new SpawnPoint("spawn_red_4", "red", 120, 0, 120));
        brawlMap.AddSpawnPoint(new SpawnPoint("spawn_blue_4", "blue", -120, 0, -120));
        entertainmentModeData.system.AddMap(brawlMap);
        
        ModeRule brawlRule1 = new ModeRule("rule_brawl_1", "mode_brawl", "随机英雄", "玩家随机获得英雄");
        ModeRule brawlRule2 = new ModeRule("rule_brawl_2", "mode_brawl", "快速战斗", "战斗节奏快，游戏时间短");
        ModeRule brawlRule3 = new ModeRule("rule_brawl_3", "mode_brawl", "自动出装", "系统自动为玩家选择装备");
        entertainmentModeData.system.AddRule(brawlRule1);
        entertainmentModeData.system.AddRule(brawlRule2);
        entertainmentModeData.system.AddRule(brawlRule3);
        
        EntertainmentMode brawlMode = new EntertainmentMode("mode_brawl", "大乱斗", "随机英雄，快速战斗，自动出装", "battle", 10, 10, 12, "map_brawl");
        brawlMode.AddRule("rule_brawl_1");
        brawlMode.AddRule("rule_brawl_2");
        brawlMode.AddRule("rule_brawl_3");
        brawlMode.AddFeature("随机英雄");
        brawlMode.AddFeature("快速战斗");
        brawlMode.AddFeature("自动出装");
        entertainmentModeData.system.AddMode(brawlMode);
        
        SaveEntertainmentModeData();
    }
    
    // 模式管理
    public void AddMode(string name, string desc, string type, int maxPlayers, int minPlayers, int matchDuration, string mapID)
    {
        string modeID = "mode_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EntertainmentMode mode = new EntertainmentMode(modeID, name, desc, type, maxPlayers, minPlayers, matchDuration, mapID);
        entertainmentModeData.system.AddMode(mode);
        SaveEntertainmentModeData();
        Debug.Log("成功添加娱乐模式: " + name);
    }
    
    public void UpdateMode(string modeID, string name, string desc, int maxPlayers, int minPlayers, int matchDuration)
    {
        EntertainmentMode mode = entertainmentModeData.system.GetMode(modeID);
        if (mode != null)
        {
            mode.UpdateMode(name, desc, maxPlayers, minPlayers, matchDuration);
            SaveEntertainmentModeData();
            Debug.Log("成功更新娱乐模式: " + name);
        }
        else
        {
            Debug.LogError("娱乐模式不存在: " + modeID);
        }
    }
    
    public void DeleteMode(string modeID)
    {
        EntertainmentMode mode = entertainmentModeData.system.GetMode(modeID);
        if (mode != null)
        {
            entertainmentModeData.system.modes.Remove(mode);
            SaveEntertainmentModeData();
            Debug.Log("成功删除娱乐模式: " + modeID);
        }
        else
        {
            Debug.LogError("娱乐模式不存在: " + modeID);
        }
    }
    
    public List<EntertainmentMode> GetAllModes()
    {
        return entertainmentModeData.system.GetAllModes();
    }
    
    public EntertainmentMode GetMode(string modeID)
    {
        return entertainmentModeData.system.GetMode(modeID);
    }
    
    // 匹配管理
    public string CreateMatch(string modeID, string matchName)
    {
        string matchID = "match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ModeMatch match = new ModeMatch(matchID, modeID, matchName);
        entertainmentModeData.system.AddMatch(match);
        SaveEntertainmentModeData();
        Debug.Log("成功创建匹配: " + matchName);
        return matchID;
    }
    
    public void AddPlayerToMatch(string matchID, string playerID, string playerName, string heroID, string team)
    {
        ModeMatch match = entertainmentModeData.system.GetMatch(matchID);
        if (match != null)
        {
            MatchPlayer player = new MatchPlayer(playerID, playerName, heroID, team);
            match.AddPlayer(player);
            SaveEntertainmentModeData();
            Debug.Log("成功添加玩家到匹配: " + playerName);
        }
        else
        {
            Debug.LogError("匹配不存在: " + matchID);
        }
    }
    
    public void RemovePlayerFromMatch(string matchID, string playerID)
    {
        ModeMatch match = entertainmentModeData.system.GetMatch(matchID);
        if (match != null)
        {
            match.RemovePlayer(playerID);
            SaveEntertainmentModeData();
            Debug.Log("成功从匹配中移除玩家: " + playerID);
        }
        else
        {
            Debug.LogError("匹配不存在: " + matchID);
        }
    }
    
    public void StartMatch(string matchID)
    {
        ModeMatch match = entertainmentModeData.system.GetMatch(matchID);
        if (match != null)
        {
            match.StartMatch();
            SaveEntertainmentModeData();
            Debug.Log("成功开始匹配: " + matchID);
        }
        else
        {
            Debug.LogError("匹配不存在: " + matchID);
        }
    }
    
    public void EndMatch(string matchID)
    {
        ModeMatch match = entertainmentModeData.system.GetMatch(matchID);
        if (match != null)
        {
            match.EndMatch();
            SaveEntertainmentModeData();
            Debug.Log("成功结束匹配: " + matchID);
        }
        else
        {
            Debug.LogError("匹配不存在: " + matchID);
        }
    }
    
    public void AddMatchEvent(string matchID, string eventType, string playerID, string targetID, string description)
    {
        ModeMatch match = entertainmentModeData.system.GetMatch(matchID);
        if (match != null)
        {
            string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            MatchEvent ev = new MatchEvent(eventID, eventType, playerID, targetID, description);
            match.AddEvent(ev);
            SaveEntertainmentModeData();
            Debug.Log("成功添加匹配事件: " + eventType);
        }
        else
        {
            Debug.LogError("匹配不存在: " + matchID);
        }
    }
    
    public ModeMatch GetMatch(string matchID)
    {
        return entertainmentModeData.system.GetMatch(matchID);
    }
    
    public List<ModeMatch> GetMatchesByMode(string modeID)
    {
        return entertainmentModeData.system.GetMatchesByMode(modeID);
    }
    
    // 规则管理
    public void AddRule(string modeID, string name, string desc)
    {
        string ruleID = "rule_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ModeRule rule = new ModeRule(ruleID, modeID, name, desc);
        entertainmentModeData.system.AddRule(rule);
        
        EntertainmentMode mode = entertainmentModeData.system.GetMode(modeID);
        if (mode != null)
        {
            mode.AddRule(ruleID);
        }
        
        SaveEntertainmentModeData();
        Debug.Log("成功添加规则: " + name);
    }
    
    public void UpdateRule(string ruleID, string name, string desc, bool enabled)
    {
        ModeRule rule = entertainmentModeData.system.GetRule(ruleID);
        if (rule != null)
        {
            rule.UpdateRule(name, desc, enabled);
            SaveEntertainmentModeData();
            Debug.Log("成功更新规则: " + name);
        }
        else
        {
            Debug.LogError("规则不存在: " + ruleID);
        }
    }
    
    public void DeleteRule(string ruleID)
    {
        ModeRule rule = entertainmentModeData.system.GetRule(ruleID);
        if (rule != null)
        {
            // 从模式中移除规则
            EntertainmentMode mode = entertainmentModeData.system.GetMode(rule.modeID);
            if (mode != null)
            {
                mode.rules.Remove(ruleID);
            }
            
            entertainmentModeData.system.rules.Remove(rule);
            SaveEntertainmentModeData();
            Debug.Log("成功删除规则: " + ruleID);
        }
        else
        {
            Debug.LogError("规则不存在: " + ruleID);
        }
    }
    
    public List<ModeRule> GetRulesByMode(string modeID)
    {
        return entertainmentModeData.system.GetRulesByMode(modeID);
    }
    
    // 地图管理
    public void AddMap(string modeID, string name, string desc, string type, int size)
    {
        string mapID = "map_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ModeMap map = new ModeMap(mapID, modeID, name, desc, type, size);
        entertainmentModeData.system.AddMap(map);
        SaveEntertainmentModeData();
        Debug.Log("成功添加地图: " + name);
    }
    
    public void AddSpawnPointToMap(string mapID, string team, float x, float y, float z)
    {
        ModeMap map = entertainmentModeData.system.GetMap(mapID);
        if (map != null)
        {
            string spawnID = "spawn_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            SpawnPoint spawnPoint = new SpawnPoint(spawnID, team, x, y, z);
            map.AddSpawnPoint(spawnPoint);
            SaveEntertainmentModeData();
            Debug.Log("成功添加出生点到地图: " + mapID);
        }
        else
        {
            Debug.LogError("地图不存在: " + mapID);
        }
    }
    
    public void AddFeatureToMap(string mapID, string name, string type, float x, float y, float z, string desc)
    {
        ModeMap map = entertainmentModeData.system.GetMap(mapID);
        if (map != null)
        {
            string featureID = "feature_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            MapFeature feature = new MapFeature(featureID, name, type, x, y, z, desc);
            map.AddFeature(feature);
            SaveEntertainmentModeData();
            Debug.Log("成功添加地图特性: " + name);
        }
        else
        {
            Debug.LogError("地图不存在: " + mapID);
        }
    }
    
    public ModeMap GetMap(string mapID)
    {
        return entertainmentModeData.system.GetMap(mapID);
    }
    
    public List<ModeMap> GetMapsByMode(string modeID)
    {
        return entertainmentModeData.system.GetMapsByMode(modeID);
    }
    
    // 数据持久化
    public void SaveEntertainmentModeData()
    {
        string path = Application.dataPath + "/Data/entertainment_mode_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, entertainmentModeData);
        stream.Close();
    }
    
    public void LoadEntertainmentModeData()
    {
        string path = Application.dataPath + "/Data/entertainment_mode_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            entertainmentModeData = (EntertainmentModeDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            entertainmentModeData = new EntertainmentModeDetailedManagerData();
        }
    }
}
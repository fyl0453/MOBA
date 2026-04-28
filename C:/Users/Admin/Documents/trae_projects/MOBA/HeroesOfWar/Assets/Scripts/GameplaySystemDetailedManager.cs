using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameplaySystemDetailedManager : MonoBehaviour
{
    public static GameplaySystemDetailedManager Instance { get; private set; }
    
    public GameplaySystemDetailedManagerData gameplayData;
    
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
        LoadGameplayData();
        
        if (gameplayData == null)
        {
            gameplayData = new GameplaySystemDetailedManagerData();
            InitializeDefaultGameplaySystem();
        }
    }
    
    private void InitializeDefaultGameplaySystem()
    {
        // 英雄
        Hero hero1 = new Hero("hero_001", "李白", "assassin", "青莲剑仙，擅长突进和爆发", 3200, 400, 170, 0, 1.0f, 350, 50, 30, 150, "models/hero_001", "icons/hero_001");
        Hero hero2 = new Hero("hero_002", "诸葛亮", "mage", "卧龙先生，擅长法术伤害和控制", 2800, 500, 100, 160, 0.8f, 330, 40, 40, 125, "models/hero_002", "icons/hero_002");
        Hero hero3 = new Hero("hero_003", "关羽", "warrior", "武圣，擅长冲锋和控制", 3800, 350, 180, 0, 0.9f, 340, 80, 25, 175, "models/hero_003", "icons/hero_003");
        Hero hero4 = new Hero("hero_004", "后羿", "marksman", "神箭手，擅长远程输出", 2500, 300, 150, 0, 1.2f, 320, 30, 20, 600, "models/hero_004", "icons/hero_004");
        Hero hero5 = new Hero("hero_005", "张飞", "tank", "燕人张翼德，擅长坦克和控制", 4200, 450, 120, 0, 0.8f, 310, 100, 35, 150, "models/hero_005", "icons/hero_005");
        
        gameplayData.system.AddHero(hero1);
        gameplayData.system.AddHero(hero2);
        gameplayData.system.AddHero(hero3);
        gameplayData.system.AddHero(hero4);
        gameplayData.system.AddHero(hero5);
        
        // 技能
        Skill skill1_1 = new Skill("skill_001", "hero_001", "将进酒", "normal", "向前突进并造成伤害", 10, 50, 100, 500, 5, "targeted", "damage");
        Skill skill1_2 = new Skill("skill_002", "hero_001", "神来之笔", "normal", "画出剑阵并造成伤害", 8, 60, 150, 800, 5, "area", "damage");
        Skill skill1_3 = new Skill("skill_003", "hero_001", "青莲剑歌", "ultimate", "连续攻击并无法被选中", 40, 100, 200, 1000, 3, "channeled", "damage");
        Skill skill2_1 = new Skill("skill_004", "hero_002", "东风破袭", "normal", "发射能量球并造成伤害", 6, 40, 80, 1000, 5, "projectile", "damage");
        Skill skill2_2 = new Skill("skill_005", "hero_002", "时空穿梭", "normal", "闪烁到指定位置", 12, 80, 0, 600, 5, "blink", "movement");
        Skill skill2_3 = new Skill("skill_006", "hero_002", "元气弹", "ultimate", "蓄力后发射强力能量球", 35, 120, 300, 1500, 3, "charged", "damage");
        
        gameplayData.system.AddSkill(skill1_1);
        gameplayData.system.AddSkill(skill1_2);
        gameplayData.system.AddSkill(skill1_3);
        gameplayData.system.AddSkill(skill2_1);
        gameplayData.system.AddSkill(skill2_2);
        gameplayData.system.AddSkill(skill2_3);
        
        // 添加技能到英雄
        hero1.AddSkill("skill_001");
        hero1.AddSkill("skill_002");
        hero1.AddSkill("skill_003");
        hero2.AddSkill("skill_004");
        hero2.AddSkill("skill_005");
        hero2.AddSkill("skill_006");
        
        // 物品
        Item item1 = new Item("item_001", "无尽战刃", "weapon", "增加攻击力和暴击率", 2100, 0, 0, 80, 0, 0, 0, 0, 0, 0.25f, 0, 0, "icons/item_001", false, 1);
        Item item2 = new Item("item_002", "泣血之刃", "weapon", "增加攻击力和生命偷取", 1740, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0.25f, 0, "icons/item_002", false, 1);
        Item item3 = new Item("item_003", "宗师之力", "weapon", "增加攻击力、暴击率和技能伤害", 2100, 0, 0, 80, 0, 0.15f, 0, 0, 0, 0.20f, 0, 0, "icons/item_003", false, 1);
        Item item4 = new Item("item_004", "回响之杖", "magic", "增加法术强度和移速", 2100, 0, 0, 0, 240, 0, 70, 0, 0, 0, 0, 0, "icons/item_004", false, 1);
        Item item5 = new Item("item_005", "痛苦面具", "magic", "增加法术强度和法术穿透", 2040, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, "icons/item_005", false, 1);
        
        gameplayData.system.AddItem(item1);
        gameplayData.system.AddItem(item2);
        gameplayData.system.AddItem(item3);
        gameplayData.system.AddItem(item4);
        gameplayData.system.AddItem(item5);
        
        // 添加推荐装备到英雄
        hero1.AddRecommendedItem("item_001");
        hero1.AddRecommendedItem("item_002");
        hero1.AddRecommendedItem("item_003");
        hero2.AddRecommendedItem("item_004");
        hero2.AddRecommendedItem("item_005");
        
        // 地图
        Map map1 = new Map("map_001", "王者峡谷", "game_mode_001", "5v5标准地图", 10000, 10000, "maps/kings_canyon", "maps/minimaps/kings_canyon");
        map1.AddSpawnPoint(new SpawnPoint("spawn_001", "map_001", "team1", 1000, 0, 1000, "hero"));
        map1.AddSpawnPoint(new SpawnPoint("spawn_002", "map_001", "team2", 9000, 0, 9000, "hero"));
        map1.AddObjective(new Objective("obj_001", "map_001", "红BUFF", "buff", 3000, 0, 7000, "available", 120));
        map1.AddObjective(new Objective("obj_002", "map_001", "蓝BUFF", "buff", 7000, 0, 3000, "available", 120));
        map1.AddObjective(new Objective("obj_003", "map_001", "大龙", "boss", 5000, 0, 5000, "available", 180));
        map1.AddObjective(new Objective("obj_004", "map_001", "小龙", "boss", 5000, 0, 3000, "available", 120));
        
        Path path1 = new Path("path_001", "map_001", "上路", "lane");
        path1.AddWaypoint(new Waypoint("wp_001", "path_001", 2000, 0, 2000, 1));
        path1.AddWaypoint(new Waypoint("wp_002", "path_001", 4000, 0, 4000, 2));
        path1.AddWaypoint(new Waypoint("wp_003", "path_001", 6000, 0, 6000, 3));
        path1.AddWaypoint(new Waypoint("wp_004", "path_001", 8000, 0, 8000, 4));
        map1.AddPath(path1);
        
        Path path2 = new Path("path_002", "map_001", "中路", "lane");
        path2.AddWaypoint(new Waypoint("wp_005", "path_002", 1000, 0, 5000, 1));
        path2.AddWaypoint(new Waypoint("wp_006", "path_002", 3000, 0, 5000, 2));
        path2.AddWaypoint(new Waypoint("wp_007", "path_002", 5000, 0, 5000, 3));
        path2.AddWaypoint(new Waypoint("wp_008", "path_002", 7000, 0, 5000, 4));
        path2.AddWaypoint(new Waypoint("wp_009", "path_002", 9000, 0, 5000, 5));
        map1.AddPath(path2);
        
        Path path3 = new Path("path_003", "map_001", "下路", "lane");
        path3.AddWaypoint(new Waypoint("wp_010", "path_003", 2000, 0, 8000, 1));
        path3.AddWaypoint(new Waypoint("wp_011", "path_003", 4000, 0, 6000, 2));
        path3.AddWaypoint(new Waypoint("wp_012", "path_003", 6000, 0, 4000, 3));
        path3.AddWaypoint(new Waypoint("wp_013", "path_003", 8000, 0, 2000, 4));
        map1.AddPath(path3);
        
        gameplayData.system.AddMap(map1);
        
        // 游戏模式
        GameMode gameMode1 = new GameMode("game_mode_001", "排位赛", "competitive", "5v5竞技模式，影响段位", 10, 10, 300, "摧毁敌方基地", "我方基地被摧毁", true);
        GameMode gameMode2 = new GameMode("game_mode_002", "匹配赛", "casual", "5v5休闲模式，不影响段位", 10, 10, 300, "摧毁敌方基地", "我方基地被摧毁", false);
        GameMode gameMode3 = new GameMode("game_mode_003", "火焰山大战", "entertainment", "5v5娱乐模式，地图为火焰山", 10, 10, 180, "击败敌方英雄", "我方英雄被击败", false);
        
        gameplayData.system.AddGameMode(gameMode1);
        gameplayData.system.AddGameMode(gameMode2);
        gameplayData.system.AddGameMode(gameMode3);
        
        // 比赛
        Match match1 = new Match("match_001", "game_mode_001", "map_001", "in_progress");
        match1.AddPlayer("player_001", "team1");
        match1.AddPlayer("player_002", "team1");
        match1.AddPlayer("player_003", "team1");
        match1.AddPlayer("player_004", "team1");
        match1.AddPlayer("player_005", "team1");
        match1.AddPlayer("player_006", "team2");
        match1.AddPlayer("player_007", "team2");
        match1.AddPlayer("player_008", "team2");
        match1.AddPlayer("player_009", "team2");
        match1.AddPlayer("player_010", "team2");
        
        gameplayData.system.AddMatch(match1);
        
        // 玩家
        Player player1 = new Player("player_001", "match_001", "user_001", "张三", "hero_001", "team1");
        player1.SetHealth(3200);
        player1.SetMana(400);
        player1.SetGold(0);
        
        Player player2 = new Player("player_002", "match_001", "user_002", "李四", "hero_002", "team1");
        player2.SetHealth(2800);
        player2.SetMana(500);
        player2.SetGold(0);
        
        Player player3 = new Player("player_003", "match_001", "user_003", "王五", "hero_003", "team1");
        player3.SetHealth(3800);
        player3.SetMana(350);
        player3.SetGold(0);
        
        Player player4 = new Player("player_004", "match_001", "user_004", "赵六", "hero_004", "team1");
        player4.SetHealth(2500);
        player4.SetMana(300);
        player4.SetGold(0);
        
        Player player5 = new Player("player_005", "match_001", "user_005", "钱七", "hero_005", "team1");
        player5.SetHealth(4200);
        player5.SetMana(450);
        player5.SetGold(0);
        
        Player player6 = new Player("player_006", "match_001", "user_006", "孙八", "hero_001", "team2");
        player6.SetHealth(3200);
        player6.SetMana(400);
        player6.SetGold(0);
        
        Player player7 = new Player("player_007", "match_001", "user_007", "周九", "hero_002", "team2");
        player7.SetHealth(2800);
        player7.SetMana(500);
        player7.SetGold(0);
        
        Player player8 = new Player("player_008", "match_001", "user_008", "吴十", "hero_003", "team2");
        player8.SetHealth(3800);
        player8.SetMana(350);
        player8.SetGold(0);
        
        Player player9 = new Player("player_009", "match_001", "user_009", "郑十一", "hero_004", "team2");
        player9.SetHealth(2500);
        player9.SetMana(300);
        player9.SetGold(0);
        
        Player player10 = new Player("player_010", "match_001", "user_010", "王十二", "hero_005", "team2");
        player10.SetHealth(4200);
        player10.SetMana(450);
        player10.SetGold(0);
        
        gameplayData.system.AddPlayer(player1);
        gameplayData.system.AddPlayer(player2);
        gameplayData.system.AddPlayer(player3);
        gameplayData.system.AddPlayer(player4);
        gameplayData.system.AddPlayer(player5);
        gameplayData.system.AddPlayer(player6);
        gameplayData.system.AddPlayer(player7);
        gameplayData.system.AddPlayer(player8);
        gameplayData.system.AddPlayer(player9);
        gameplayData.system.AddPlayer(player10);
        
        // 游戏状态
        GameState gameState1 = new GameState("game_state_001", "match_001", "游戏开始", "start", "游戏开始阶段");
        GameState gameState2 = new GameState("game_state_002", "match_001", "游戏进行中", "playing", "游戏进行中阶段");
        
        gameplayData.system.AddGameState(gameState1);
        gameplayData.system.AddGameState(gameState2);
        
        SaveGameplayData();
    }
    
    // 英雄管理
    public void AddHero(string name, string type, string desc, float health, float mana, float attackDamage, float abilityPower, float attackSpeed, float movementSpeed, float armor, float magicResist, float attackRange, string modelPath, string iconPath)
    {
        string heroID = "hero_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Hero hero = new Hero(heroID, name, type, desc, health, mana, attackDamage, abilityPower, attackSpeed, movementSpeed, armor, magicResist, attackRange, modelPath, iconPath);
        gameplayData.system.AddHero(hero);
        SaveGameplayData();
        Debug.Log("成功添加英雄: " + name);
    }
    
    public List<Hero> GetHeroesByType(string type)
    {
        return gameplayData.system.GetHeroesByType(type);
    }
    
    public List<Hero> GetAllHeroes()
    {
        return gameplayData.system.heroes;
    }
    
    public void EnableHero(string heroID)
    {
        Hero hero = gameplayData.system.GetHero(heroID);
        if (hero != null)
        {
            hero.Enable();
            SaveGameplayData();
            Debug.Log("成功启用英雄: " + hero.heroName);
        }
        else
        {
            Debug.LogError("英雄不存在: " + heroID);
        }
    }
    
    public void DisableHero(string heroID)
    {
        Hero hero = gameplayData.system.GetHero(heroID);
        if (hero != null)
        {
            hero.Disable();
            SaveGameplayData();
            Debug.Log("成功禁用英雄: " + hero.heroName);
        }
        else
        {
            Debug.LogError("英雄不存在: " + heroID);
        }
    }
    
    // 技能管理
    public void AddSkill(string heroID, string name, string type, string desc, float cooldown, float manaCost, float damage, float range, int maxLevel, string castType, string effectType)
    {
        string skillID = "skill_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Skill skill = new Skill(skillID, heroID, name, type, desc, cooldown, manaCost, damage, range, maxLevel, castType, effectType);
        gameplayData.system.AddSkill(skill);
        
        Hero hero = gameplayData.system.GetHero(heroID);
        if (hero != null)
        {
            hero.AddSkill(skillID);
        }
        
        SaveGameplayData();
        Debug.Log("成功添加技能: " + name);
    }
    
    public List<Skill> GetSkillsByHero(string heroID)
    {
        return gameplayData.system.GetSkillsByHero(heroID);
    }
    
    // 物品管理
    public void AddItem(string name, string type, string desc, float price, float health, float mana, float attackDamage, float abilityPower, float attackSpeed, float movementSpeed, float armor, float magicResist, float criticalChance, float lifeSteal, float spellVamp, string iconPath, bool isStackable, int maxStacks)
    {
        string itemID = "item_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Item item = new Item(itemID, name, type, desc, price, health, mana, attackDamage, abilityPower, attackSpeed, movementSpeed, armor, magicResist, criticalChance, lifeSteal, spellVamp, iconPath, isStackable, maxStacks);
        gameplayData.system.AddItem(item);
        SaveGameplayData();
        Debug.Log("成功添加物品: " + name);
    }
    
    public List<Item> GetItemsByType(string type)
    {
        return gameplayData.system.GetItemsByType(type);
    }
    
    // 地图管理
    public void AddMap(string name, string gameModeID, string desc, int mapSizeX, int mapSizeY, string mapPath, string minimapPath)
    {
        string mapID = "map_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Map map = new Map(mapID, name, gameModeID, desc, mapSizeX, mapSizeY, mapPath, minimapPath);
        gameplayData.system.AddMap(map);
        SaveGameplayData();
        Debug.Log("成功添加地图: " + name);
    }
    
    public void AddSpawnPointToMap(string mapID, string team, float x, float y, float z, string spawnType)
    {
        Map map = gameplayData.system.GetMap(mapID);
        if (map != null)
        {
            string spawnPointID = "spawn_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            SpawnPoint spawnPoint = new SpawnPoint(spawnPointID, mapID, team, x, y, z, spawnType);
            map.AddSpawnPoint(spawnPoint);
            SaveGameplayData();
            Debug.Log("成功添加出生点到地图: " + map.mapName);
        }
        else
        {
            Debug.LogError("地图不存在: " + mapID);
        }
    }
    
    public void AddObjectiveToMap(string mapID, string name, string type, float x, float y, float z, string status, int respawnTime)
    {
        Map map = gameplayData.system.GetMap(mapID);
        if (map != null)
        {
            string objectiveID = "obj_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            Objective objective = new Objective(objectiveID, mapID, name, type, x, y, z, status, respawnTime);
            map.AddObjective(objective);
            SaveGameplayData();
            Debug.Log("成功添加目标点到地图: " + map.mapName);
        }
        else
        {
            Debug.LogError("地图不存在: " + mapID);
        }
    }
    
    public void AddPathToMap(string mapID, string name, string type)
    {
        Map map = gameplayData.system.GetMap(mapID);
        if (map != null)
        {
            string pathID = "path_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            Path path = new Path(pathID, mapID, name, type);
            map.AddPath(path);
            SaveGameplayData();
            Debug.Log("成功添加路径到地图: " + map.mapName);
        }
        else
        {
            Debug.LogError("地图不存在: " + mapID);
        }
    }
    
    public void AddWaypointToPath(string pathID, float x, float y, float z, int order)
    {
        // 查找路径所在的地图
        Map targetMap = null;
        foreach (Map map in gameplayData.system.maps)
        {
            foreach (Path path in map.paths)
            {
                if (path.pathID == pathID)
                {
                    targetMap = map;
                    break;
                }
            }
            if (targetMap != null)
                break;
        }
        
        if (targetMap != null)
        {
            foreach (Path path in targetMap.paths)
            {
                if (path.pathID == pathID)
                {
                    string waypointID = "wp_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                    Waypoint waypoint = new Waypoint(waypointID, pathID, x, y, z, order);
                    path.AddWaypoint(waypoint);
                    SaveGameplayData();
                    Debug.Log("成功添加路点到路径: " + path.pathName);
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("路径不存在: " + pathID);
        }
    }
    
    // 游戏模式管理
    public void AddGameMode(string name, string type, string desc, int minPlayers, int maxPlayers, int matchTimeLimit, string victoryCondition, string defeatCondition, bool isRanked)
    {
        string gameModeID = "game_mode_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GameMode gameMode = new GameMode(gameModeID, name, type, desc, minPlayers, maxPlayers, matchTimeLimit, victoryCondition, defeatCondition, isRanked);
        gameplayData.system.AddGameMode(gameMode);
        SaveGameplayData();
        Debug.Log("成功添加游戏模式: " + name);
    }
    
    // 比赛管理
    public string CreateMatch(string gameModeID, string mapID, string matchStatus)
    {
        string matchID = "match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Match match = new Match(matchID, gameModeID, mapID, matchStatus);
        gameplayData.system.AddMatch(match);
        SaveGameplayData();
        Debug.Log("成功创建比赛: " + matchID);
        return matchID;
    }
    
    public void AddPlayerToMatch(string matchID, string userID, string userName, string heroID, string team)
    {
        Match match = gameplayData.system.GetMatch(matchID);
        if (match != null)
        {
            string playerID = "player_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            Player player = new Player(playerID, matchID, userID, userName, heroID, team);
            gameplayData.system.AddPlayer(player);
            match.AddPlayer(playerID, team);
            
            // 设置玩家初始属性
            Hero hero = gameplayData.system.GetHero(heroID);
            if (hero != null)
            {
                player.SetHealth(hero.health);
                player.SetMana(hero.mana);
                player.SetGold(0);
            }
            
            SaveGameplayData();
            Debug.Log("成功添加玩家到比赛: " + userName);
        }
        else
        {
            Debug.LogError("比赛不存在: " + matchID);
        }
    }
    
    public void EndMatch(string matchID, string winnerTeam)
    {
        Match match = gameplayData.system.GetMatch(matchID);
        if (match != null)
        {
            match.EndMatch(winnerTeam);
            SaveGameplayData();
            Debug.Log("成功结束比赛，获胜队伍: " + winnerTeam);
        }
        else
        {
            Debug.LogError("比赛不存在: " + matchID);
        }
    }
    
    // 玩家管理
    public void UpdatePlayerStats(string playerID, int kills, int deaths, int assists, float gold, float health, float mana, int level)
    {
        Player player = gameplayData.system.GetPlayer(playerID);
        if (player != null)
        {
            player.kills = kills;
            player.deaths = deaths;
            player.assists = assists;
            player.gold = gold;
            player.SetHealth(health);
            player.SetMana(mana);
            player.SetLevel(level);
            SaveGameplayData();
            Debug.Log("成功更新玩家状态: " + player.userName);
        }
        else
        {
            Debug.LogError("玩家不存在: " + playerID);
        }
    }
    
    public void AddItemToPlayer(string playerID, string itemID)
    {
        Player player = gameplayData.system.GetPlayer(playerID);
        if (player != null)
        {
            player.AddItem(itemID);
            SaveGameplayData();
            Debug.Log("成功为玩家添加物品: " + itemID);
        }
        else
        {
            Debug.LogError("玩家不存在: " + playerID);
        }
    }
    
    public void RemoveItemFromPlayer(string playerID, string itemID)
    {
        Player player = gameplayData.system.GetPlayer(playerID);
        if (player != null)
        {
            player.RemoveItem(itemID);
            SaveGameplayData();
            Debug.Log("成功从玩家移除物品: " + itemID);
        }
        else
        {
            Debug.LogError("玩家不存在: " + playerID);
        }
    }
    
    // 游戏状态管理
    public string CreateGameState(string matchID, string name, string type, string desc)
    {
        string gameStateID = "game_state_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GameState gameState = new GameState(gameStateID, matchID, name, type, desc);
        gameplayData.system.AddGameState(gameState);
        SaveGameplayData();
        Debug.Log("成功创建游戏状态: " + name);
        return gameStateID;
    }
    
    public void EndGameState(string gameStateID)
    {
        GameState gameState = gameplayData.system.GetGameState(gameStateID);
        if (gameState != null)
        {
            gameState.EndState();
            SaveGameplayData();
            Debug.Log("成功结束游戏状态: " + gameState.stateName);
        }
        else
        {
            Debug.LogError("游戏状态不存在: " + gameStateID);
        }
    }
    
    // 数据持久化
    public void SaveGameplayData()
    {
        string path = Application.dataPath + "/Data/gameplay_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, gameplayData);
        stream.Close();
    }
    
    public void LoadGameplayData()
    {
        string path = Application.dataPath + "/Data/gameplay_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            gameplayData = (GameplaySystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            gameplayData = new GameplaySystemDetailedManagerData();
        }
    }
}
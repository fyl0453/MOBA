[System.Serializable]
public class GameplaySystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Hero> heroes;
    public List<Skill> skills;
    public List<Item> items;
    public List<Map> maps;
    public List<GameMode> gameModes;
    public List<Match> matches;
    public List<Player> players;
    public List<GameState> gameStates;
    
    public GameplaySystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        heroes = new List<Hero>();
        skills = new List<Skill>();
        items = new List<Item>();
        maps = new List<Map>();
        gameModes = new List<GameMode>();
        matches = new List<Match>();
        players = new List<Player>();
        gameStates = new List<GameState>();
    }
    
    public void AddHero(Hero hero)
    {
        heroes.Add(hero);
    }
    
    public void AddSkill(Skill skill)
    {
        skills.Add(skill);
    }
    
    public void AddItem(Item item)
    {
        items.Add(item);
    }
    
    public void AddMap(Map map)
    {
        maps.Add(map);
    }
    
    public void AddGameMode(GameMode gameMode)
    {
        gameModes.Add(gameMode);
    }
    
    public void AddMatch(Match match)
    {
        matches.Add(match);
    }
    
    public void AddPlayer(Player player)
    {
        players.Add(player);
    }
    
    public void AddGameState(GameState gameState)
    {
        gameStates.Add(gameState);
    }
    
    public Hero GetHero(string heroID)
    {
        return heroes.Find(h => h.heroID == heroID);
    }
    
    public Skill GetSkill(string skillID)
    {
        return skills.Find(s => s.skillID == skillID);
    }
    
    public Item GetItem(string itemID)
    {
        return items.Find(i => i.itemID == itemID);
    }
    
    public Map GetMap(string mapID)
    {
        return maps.Find(m => m.mapID == mapID);
    }
    
    public GameMode GetGameMode(string gameModeID)
    {
        return gameModes.Find(gm => gm.gameModeID == gameModeID);
    }
    
    public Match GetMatch(string matchID)
    {
        return matches.Find(m => m.matchID == matchID);
    }
    
    public Player GetPlayer(string playerID)
    {
        return players.Find(p => p.playerID == playerID);
    }
    
    public GameState GetGameState(string gameStateID)
    {
        return gameStates.Find(gs => gs.gameStateID == gameStateID);
    }
    
    public List<Hero> GetHeroesByType(string type)
    {
        return heroes.FindAll(h => h.heroType == type);
    }
    
    public List<Skill> GetSkillsByHero(string heroID)
    {
        return skills.FindAll(s => s.heroID == heroID);
    }
    
    public List<Item> GetItemsByType(string type)
    {
        return items.FindAll(i => i.itemType == type);
    }
    
    public List<Map> GetMapsByGameMode(string gameModeID)
    {
        return maps.FindAll(m => m.gameModeID == gameModeID);
    }
    
    public List<Match> GetMatchesByGameMode(string gameModeID)
    {
        return matches.FindAll(m => m.gameModeID == gameModeID);
    }
    
    public List<Player> GetPlayersByMatch(string matchID)
    {
        return players.FindAll(p => p.matchID == matchID);
    }
}

[System.Serializable]
public class Hero
{
    public string heroID;
    public string heroName;
    public string heroType;
    public string description;
    public float health;
    public float mana;
    public float attackDamage;
    public float abilityPower;
    public float attackSpeed;
    public float movementSpeed;
    public float armor;
    public float magicResist;
    public float attackRange;
    public string modelPath;
    public string iconPath;
    public List<string> skillIDs;
    public List<string> recommendedItems;
    public bool isEnabled;
    
    public Hero(string id, string name, string type, string desc, float health, float mana, float attackDamage, float abilityPower, float attackSpeed, float movementSpeed, float armor, float magicResist, float attackRange, string modelPath, string iconPath)
    {
        heroID = id;
        heroName = name;
        heroType = type;
        description = desc;
        this.health = health;
        this.mana = mana;
        this.attackDamage = attackDamage;
        this.abilityPower = abilityPower;
        this.attackSpeed = attackSpeed;
        this.movementSpeed = movementSpeed;
        this.armor = armor;
        this.magicResist = magicResist;
        this.attackRange = attackRange;
        this.modelPath = modelPath;
        this.iconPath = iconPath;
        skillIDs = new List<string>();
        recommendedItems = new List<string>();
        isEnabled = true;
    }
    
    public void AddSkill(string skillID)
    {
        skillIDs.Add(skillID);
    }
    
    public void AddRecommendedItem(string itemID)
    {
        recommendedItems.Add(itemID);
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class Skill
{
    public string skillID;
    public string heroID;
    public string skillName;
    public string skillType;
    public string description;
    public float cooldown;
    public float manaCost;
    public float damage;
    public float range;
    public int maxLevel;
    public string castType;
    public string effectType;
    public List<string> effectIDs;
    public bool isEnabled;
    
    public Skill(string id, string heroID, string name, string type, string desc, float cooldown, float manaCost, float damage, float range, int maxLevel, string castType, string effectType)
    {
        skillID = id;
        this.heroID = heroID;
        skillName = name;
        skillType = type;
        description = desc;
        this.cooldown = cooldown;
        this.manaCost = manaCost;
        this.damage = damage;
        this.range = range;
        this.maxLevel = maxLevel;
        this.castType = castType;
        this.effectType = effectType;
        effectIDs = new List<string>();
        isEnabled = true;
    }
    
    public void AddEffect(string effectID)
    {
        effectIDs.Add(effectID);
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class Item
{
    public string itemID;
    public string itemName;
    public string itemType;
    public string description;
    public float price;
    public float health;
    public float mana;
    public float attackDamage;
    public float abilityPower;
    public float attackSpeed;
    public float movementSpeed;
    public float armor;
    public float magicResist;
    public float criticalChance;
    public float lifeSteal;
    public float spellVamp;
    public string iconPath;
    public bool isStackable;
    public int maxStacks;
    public bool isEnabled;
    
    public Item(string id, string name, string type, string desc, float price, float health, float mana, float attackDamage, float abilityPower, float attackSpeed, float movementSpeed, float armor, float magicResist, float criticalChance, float lifeSteal, float spellVamp, string iconPath, bool isStackable, int maxStacks)
    {
        itemID = id;
        itemName = name;
        itemType = type;
        description = desc;
        this.price = price;
        this.health = health;
        this.mana = mana;
        this.attackDamage = attackDamage;
        this.abilityPower = abilityPower;
        this.attackSpeed = attackSpeed;
        this.movementSpeed = movementSpeed;
        this.armor = armor;
        this.magicResist = magicResist;
        this.criticalChance = criticalChance;
        this.lifeSteal = lifeSteal;
        this.spellVamp = spellVamp;
        this.iconPath = iconPath;
        this.isStackable = isStackable;
        this.maxStacks = maxStacks;
        isEnabled = true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class Map
{
    public string mapID;
    public string mapName;
    public string gameModeID;
    public string description;
    public int mapSizeX;
    public int mapSizeY;
    public string mapPath;
    public string minimapPath;
    public List<SpawnPoint> spawnPoints;
    public List<Objective> objectives;
    public List<Path> paths;
    public bool isEnabled;
    
    public Map(string id, string name, string gameModeID, string desc, int mapSizeX, int mapSizeY, string mapPath, string minimapPath)
    {
        mapID = id;
        mapName = name;
        this.gameModeID = gameModeID;
        description = desc;
        this.mapSizeX = mapSizeX;
        this.mapSizeY = mapSizeY;
        this.mapPath = mapPath;
        this.minimapPath = minimapPath;
        spawnPoints = new List<SpawnPoint>();
        objectives = new List<Objective>();
        paths = new List<Path>();
        isEnabled = true;
    }
    
    public void AddSpawnPoint(SpawnPoint spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }
    
    public void AddObjective(Objective objective)
    {
        objectives.Add(objective);
    }
    
    public void AddPath(Path path)
    {
        paths.Add(path);
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class SpawnPoint
{
    public string spawnPointID;
    public string mapID;
    public string team;
    public float x;
    public float y;
    public float z;
    public string spawnType;
    
    public SpawnPoint(string id, string mapID, string team, float x, float y, float z, string spawnType)
    {
        spawnPointID = id;
        this.mapID = mapID;
        this.team = team;
        this.x = x;
        this.y = y;
        this.z = z;
        this.spawnType = spawnType;
    }
}

[System.Serializable]
public class Objective
{
    public string objectiveID;
    public string mapID;
    public string objectiveName;
    public string objectiveType;
    public float x;
    public float y;
    public float z;
    public string status;
    public int respawnTime;
    
    public Objective(string id, string mapID, string name, string type, float x, float y, float z, string status, int respawnTime)
    {
        objectiveID = id;
        this.mapID = mapID;
        objectiveName = name;
        objectiveType = type;
        this.x = x;
        this.y = y;
        this.z = z;
        this.status = status;
        this.respawnTime = respawnTime;
    }
}

[System.Serializable]
public class Path
{
    public string pathID;
    public string mapID;
    public string pathName;
    public string pathType;
    public List<Waypoint> waypoints;
    
    public Path(string id, string mapID, string name, string type)
    {
        pathID = id;
        this.mapID = mapID;
        pathName = name;
        pathType = type;
        waypoints = new List<Waypoint>();
    }
    
    public void AddWaypoint(Waypoint waypoint)
    {
        waypoints.Add(waypoint);
    }
}

[System.Serializable]
public class Waypoint
{
    public string waypointID;
    public string pathID;
    public float x;
    public float y;
    public float z;
    public int order;
    
    public Waypoint(string id, string pathID, float x, float y, float z, int order)
    {
        waypointID = id;
        this.pathID = pathID;
        this.x = x;
        this.y = y;
        this.z = z;
        this.order = order;
    }
}

[System.Serializable]
public class GameMode
{
    public string gameModeID;
    public string gameModeName;
    public string gameModeType;
    public string description;
    int minPlayers;
    int maxPlayers;
    int matchTimeLimit;
    string victoryCondition;
    string defeatCondition;
    bool isRanked;
    bool isEnabled;
    
    public GameMode(string id, string name, string type, string desc, int minPlayers, int maxPlayers, int matchTimeLimit, string victoryCondition, string defeatCondition, bool isRanked)
    {
        gameModeID = id;
        gameModeName = name;
        gameModeType = type;
        description = desc;
        this.minPlayers = minPlayers;
        this.maxPlayers = maxPlayers;
        this.matchTimeLimit = matchTimeLimit;
        this.victoryCondition = victoryCondition;
        this.defeatCondition = defeatCondition;
        this.isRanked = isRanked;
        isEnabled = true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class Match
{
    public string matchID;
    public string gameModeID;
    public string mapID;
    public string matchStatus;
    public string startTime;
    public string endTime;
    public string winnerTeam;
    public int duration;
    public List<string> playerIDs;
    public List<string> team1IDs;
    public List<string> team2IDs;
    
    public Match(string id, string gameModeID, string mapID, string matchStatus)
    {
        matchID = id;
        this.gameModeID = gameModeID;
        this.mapID = mapID;
        this.matchStatus = matchStatus;
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        endTime = "";
        winnerTeam = "";
        duration = 0;
        playerIDs = new List<string>();
        team1IDs = new List<string>();
        team2IDs = new List<string>();
    }
    
    public void AddPlayer(string playerID, string team)
    {
        playerIDs.Add(playerID);
        if (team == "team1")
        {
            team1IDs.Add(playerID);
        }
        else if (team == "team2")
        {
            team2IDs.Add(playerID);
        }
    }
    
    public void EndMatch(string winnerTeam)
    {
        matchStatus = "completed";
        endTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.winnerTeam = winnerTeam;
        System.DateTime start = System.DateTime.Parse(startTime);
        System.DateTime end = System.DateTime.Parse(endTime);
        duration = (int)(end - start).TotalSeconds;
    }
}

[System.Serializable]
public class Player
{
    public string playerID;
    public string matchID;
    public string userID;
    public string userName;
    public string heroID;
    public string team;
    public int level;
    public float health;
    public float mana;
    public float gold;
    public int kills;
    public int deaths;
    public int assists;
    public List<string> itemIDs;
    public string status;
    
    public Player(string id, string matchID, string userID, string userName, string heroID, string team)
    {
        playerID = id;
        this.matchID = matchID;
        this.userID = userID;
        this.userName = userName;
        this.heroID = heroID;
        this.team = team;
        level = 1;
        health = 0;
        mana = 0;
        gold = 0;
        kills = 0;
        deaths = 0;
        assists = 0;
        itemIDs = new List<string>();
        status = "alive";
    }
    
    public void AddItem(string itemID)
    {
        itemIDs.Add(itemID);
    }
    
    public void RemoveItem(string itemID)
    {
        itemIDs.Remove(itemID);
    }
    
    public void AddKill()
    {
        kills++;
    }
    
    public void AddDeath()
    {
        deaths++;
    }
    
    public void AddAssist()
    {
        assists++;
    }
    
    public void AddGold(float amount)
    {
        gold += amount;
    }
    
    public void RemoveGold(float amount)
    {
        gold -= amount;
    }
    
    public void SetHealth(float health)
    {
        this.health = health;
    }
    
    public void SetMana(float mana)
    {
        this.mana = mana;
    }
    
    public void SetLevel(int level)
    {
        this.level = level;
    }
    
    public void SetStatus(string status)
    {
        this.status = status;
    }
}

[System.Serializable]
public class GameState
{
    public string gameStateID;
    public string matchID;
    public string stateName;
    public string stateType;
    public string startTime;
    public string endTime;
    public string description;
    public List<string> eventIDs;
    
    public GameState(string id, string matchID, string name, string type, string desc)
    {
        gameStateID = id;
        this.matchID = matchID;
        stateName = name;
        stateType = type;
        description = desc;
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        endTime = "";
        eventIDs = new List<string>();
    }
    
    public void AddEvent(string eventID)
    {
        eventIDs.Add(eventID);
    }
    
    public void EndState()
    {
        endTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class GameplaySystemDetailedManagerData
{
    public GameplaySystemDetailed system;
    
    public GameplaySystemDetailedManagerData()
    {
        system = new GameplaySystemDetailed("gameplay_system_detailed", "游戏核心玩法系统详细", "管理游戏核心玩法的详细功能，包括英雄技能系统、地图系统和游戏模式管理");
    }
}
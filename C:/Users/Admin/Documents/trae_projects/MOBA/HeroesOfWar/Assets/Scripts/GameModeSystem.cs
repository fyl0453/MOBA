[System.Serializable]
public class GameMode
{
    public string modeID;
    public string modeName;
    public string modeDescription;
    public int minPlayers;
    public int maxPlayers;
    public int mapID;
    public float matchDuration;
    public string victoryCondition;
    public List<ModeRule> rules;
    public List<ModeReward> rewards;
    public bool isEnabled;
    public string modeIcon;
    
    public GameMode(string id, string name, string desc, int min, int max, int map)
    {
        modeID = id;
        modeName = name;
        modeDescription = desc;
        minPlayers = min;
        maxPlayers = max;
        mapID = map;
        matchDuration = 20f;
        victoryCondition = "摧毁敌方基地";
        rules = new List<ModeRule>();
        rewards = new List<ModeReward>();
        isEnabled = true;
        modeIcon = "";
    }
    
    public void AddRule(string ruleName, string ruleDescription)
    {
        ModeRule rule = new ModeRule(ruleName, ruleDescription);
        rules.Add(rule);
    }
    
    public void AddReward(string rewardType, string rewardItemID, int quantity)
    {
        ModeReward reward = new ModeReward(rewardType, rewardItemID, quantity);
        rewards.Add(reward);
    }
}

[System.Serializable]
public class ModeRule
{
    public string ruleName;
    public string ruleDescription;
    
    public ModeRule(string name, string desc)
    {
        ruleName = name;
        ruleDescription = desc;
    }
}

[System.Serializable]
public class ModeReward
{
    public string rewardType;
    public string rewardItemID;
    public int quantity;
    
    public ModeReward(string type, string itemID, int qty)
    {
        rewardType = type;
        rewardItemID = itemID;
        quantity = qty;
    }
}

[System.Serializable]
public class Map
{
    public string mapID;
    public string mapName;
    public string mapDescription;
    public int mapSize;
    public List<SpawnPoint> spawnPoints;
    public List<Objective> objectives;
    public List<Turret> turrets;
    public List<Base> bases;
    public string mapPrefab;
    
    public Map(string id, string name, string desc, int size)
    {
        mapID = id;
        mapName = name;
        mapDescription = desc;
        mapSize = size;
        spawnPoints = new List<SpawnPoint>();
        objectives = new List<Objective>();
        turrets = new List<Turret>();
        bases = new List<Base>();
        mapPrefab = "";
    }
    
    public void AddSpawnPoint(Vector3 position, string team)
    {
        SpawnPoint spawnPoint = new SpawnPoint(position, team);
        spawnPoints.Add(spawnPoint);
    }
    
    public void AddObjective(string objectiveType, Vector3 position)
    {
        Objective objective = new Objective(objectiveType, position);
        objectives.Add(objective);
    }
    
    public void AddTurret(Vector3 position, string team)
    {
        Turret turret = new Turret(position, team);
        turrets.Add(turret);
    }
    
    public void AddBase(Vector3 position, string team)
    {
        Base gameBase = new Base(position, team);
        bases.Add(gameBase);
    }
}

[System.Serializable]
public class SpawnPoint
{
    public Vector3 position;
    public string team;
    
    public SpawnPoint(Vector3 pos, string team)
    {
        position = pos;
        this.team = team;
    }
}

[System.Serializable]
public class Objective
{
    public string objectiveType;
    public Vector3 position;
    public bool isActive;
    
    public Objective(string type, Vector3 pos)
    {
        objectiveType = type;
        position = pos;
        isActive = true;
    }
}

[System.Serializable]
public class Turret
{
    public Vector3 position;
    public string team;
    public int health;
    public int attackDamage;
    
    public Turret(Vector3 pos, string team)
    {
        position = pos;
        this.team = team;
        health = 2000;
        attackDamage = 100;
    }
}

[System.Serializable]
public class Base
{
    public Vector3 position;
    public string team;
    public int health;
    
    public Base(Vector3 pos, string team)
    {
        position = pos;
        this.team = team;
        health = 5000;
    }
}

[System.Serializable]
public class GameModeManagerData
{
    public List<GameMode> gameModes;
    public List<Map> maps;
    
    public GameModeManagerData()
    {
        gameModes = new List<GameMode>();
        maps = new List<Map>();
    }
    
    public void AddGameMode(GameMode mode)
    {
        gameModes.Add(mode);
    }
    
    public void AddMap(Map map)
    {
        maps.Add(map);
    }
    
    public GameMode GetGameMode(string modeID)
    {
        return gameModes.Find(m => m.modeID == modeID);
    }
    
    public Map GetMap(string mapID)
    {
        return maps.Find(m => m.mapID == mapID);
    }
    
    public List<GameMode> GetEnabledGameModes()
    {
        return gameModes.FindAll(m => m.isEnabled);
    }
    
    public List<Map> GetAllMaps()
    {
        return maps;
    }
}
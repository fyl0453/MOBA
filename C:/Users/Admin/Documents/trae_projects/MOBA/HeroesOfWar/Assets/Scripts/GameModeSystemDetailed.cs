[System.Serializable]
public class GameModeSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<GameMode> gameModes;
    public List<GameRule> gameRules;
    public List<GameMap> gameMaps;
    public List<GameEvent> gameEvents;
    public List<PlayerGameModeStats> playerGameModeStats;
    
    public GameModeSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        gameModes = new List<GameMode>();
        gameRules = new List<GameRule>();
        gameMaps = new List<GameMap>();
        gameEvents = new List<GameEvent>();
        playerGameModeStats = new List<PlayerGameModeStats>();
    }
    
    public void AddGameMode(GameMode gameMode)
    {
        gameModes.Add(gameMode);
    }
    
    public void AddGameRule(GameRule gameRule)
    {
        gameRules.Add(gameRule);
    }
    
    public void AddGameMap(GameMap gameMap)
    {
        gameMaps.Add(gameMap);
    }
    
    public void AddGameEvent(GameEvent gameEvent)
    {
        gameEvents.Add(gameEvent);
    }
    
    public void AddPlayerGameModeStats(PlayerGameModeStats stats)
    {
        playerGameModeStats.Add(stats);
    }
    
    public GameMode GetGameMode(string gameModeID)
    {
        return gameModes.Find(gm => gm.gameModeID == gameModeID);
    }
    
    public GameRule GetGameRule(string ruleID)
    {
        return gameRules.Find(gr => gr.ruleID == ruleID);
    }
    
    public GameMap GetGameMap(string mapID)
    {
        return gameMaps.Find(gm => gm.mapID == mapID);
    }
    
    public GameEvent GetGameEvent(string eventID)
    {
        return gameEvents.Find(ge => ge.eventID == eventID);
    }
    
    public PlayerGameModeStats GetPlayerGameModeStats(string playerID, string gameModeID)
    {
        return playerGameModeStats.Find(pgs => pgs.playerID == playerID && pgs.gameModeID == gameModeID);
    }
    
    public List<GameMode> GetGameModesByType(string type)
    {
        return gameModes.FindAll(gm => gm.gameModeType == type);
    }
    
    public List<GameMode> GetActiveGameModes()
    {
        return gameModes.FindAll(gm => gm.isEnabled && gm.IsAvailable());
    }
    
    public List<GameMap> GetMapsByGameMode(string gameModeID)
    {
        return gameMaps.FindAll(gm => gm.gameModeID == gameModeID);
    }
    
    public List<PlayerGameModeStats> GetPlayerStats(string playerID)
    {
        return playerGameModeStats.FindAll(pgs => pgs.playerID == playerID);
    }
}

[System.Serializable]
public class GameMode
{
    public string gameModeID;
    public string gameModeName;
    public string gameModeType;
    public string description;
    public int minPlayers;
    public int maxPlayers;
    public int matchTimeLimit;
    public bool isRanked;
    public bool isLimited;
    public string releaseDate;
    public string expiryDate;
    public string iconPath;
    public string bgmPath;
    public List<string> ruleIDs;
    public List<string> mapIDs;
    public bool isEnabled;
    
    public GameMode(string id, string name, string type, string desc, int minPlayers, int maxPlayers, int matchTimeLimit, bool isRanked, bool isLimited, string releaseDate, string expiryDate, string iconPath, string bgmPath)
    {
        gameModeID = id;
        gameModeName = name;
        gameModeType = type;
        description = desc;
        this.minPlayers = minPlayers;
        this.maxPlayers = maxPlayers;
        this.matchTimeLimit = matchTimeLimit;
        this.isRanked = isRanked;
        this.isLimited = isLimited;
        this.releaseDate = releaseDate;
        this.expiryDate = expiryDate;
        this.iconPath = iconPath;
        this.bgmPath = bgmPath;
        ruleIDs = new List<string>();
        mapIDs = new List<string>();
        isEnabled = true;
    }
    
    public void AddRule(string ruleID)
    {
        ruleIDs.Add(ruleID);
    }
    
    public void AddMap(string mapID)
    {
        mapIDs.Add(mapID);
    }
    
    public bool IsAvailable()
    {
        if (!isEnabled)
            return false;
        
        if (isLimited)
        {
            System.DateTime now = System.DateTime.Now;
            System.DateTime release = System.DateTime.Parse(releaseDate);
            System.DateTime expiry = System.DateTime.Parse(expiryDate);
            return now >= release && now <= expiry;
        }
        
        return true;
    }
}

[System.Serializable]
public class GameRule
{
    public string ruleID;
    public string ruleName;
    public string ruleType;
    public string description;
    public string valueType;
    public string value;
    public bool isEnabled;
    
    public GameRule(string id, string name, string type, string desc, string valueType, string value)
    {
        ruleID = id;
        ruleName = name;
        ruleType = type;
        description = desc;
        this.valueType = valueType;
        this.value = value;
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
public class GameMap
{
    public string mapID;
    public string mapName;
    public string gameModeID;
    public string description;
    public string mapType;
    public int mapSizeX;
    public int mapSizeY;
    public string mapPath;
    public string minimapPath;
    public List<string> spawnPoints;
    public List<string> objectivePoints;
    public bool isEnabled;
    
    public GameMap(string id, string name, string gameModeID, string desc, string mapType, int mapSizeX, int mapSizeY, string mapPath, string minimapPath)
    {
        mapID = id;
        mapName = name;
        this.gameModeID = gameModeID;
        description = desc;
        this.mapType = mapType;
        this.mapSizeX = mapSizeX;
        this.mapSizeY = mapSizeY;
        this.mapPath = mapPath;
        this.minimapPath = minimapPath;
        spawnPoints = new List<string>();
        objectivePoints = new List<string>();
        isEnabled = true;
    }
    
    public void AddSpawnPoint(string spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }
    
    public void AddObjectivePoint(string objectivePoint)
    {
        objectivePoints.Add(objectivePoint);
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
public class GameEvent
{
    public string eventID;
    public string eventName;
    public string gameModeID;
    public string description;
    public string eventType;
    public string triggerCondition;
    public string action;
    public string startDate;
    public string endDate;
    public bool isEnabled;
    
    public GameEvent(string id, string name, string gameModeID, string desc, string eventType, string triggerCondition, string action, string startDate, string endDate)
    {
        eventID = id;
        eventName = name;
        this.gameModeID = gameModeID;
        description = desc;
        this.eventType = eventType;
        this.triggerCondition = triggerCondition;
        this.action = action;
        this.startDate = startDate;
        this.endDate = endDate;
        isEnabled = true;
    }
    
    public bool IsActive()
    {
        if (!isEnabled)
            return false;
        
        System.DateTime now = System.DateTime.Now;
        System.DateTime start = System.DateTime.Parse(startDate);
        System.DateTime end = System.DateTime.Parse(endDate);
        return now >= start && now <= end;
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
public class PlayerGameModeStats
{
    public string statsID;
    public string playerID;
    public string gameModeID;
    public int totalGames;
    public int totalWins;
    public int totalLosses;
    public int totalKills;
    public int totalDeaths;
    public int totalAssists;
    public float winRate;
    public float kda;
    public string lastPlayedDate;
    
    public PlayerGameModeStats(string id, string playerID, string gameModeID)
    {
        statsID = id;
        this.playerID = playerID;
        this.gameModeID = gameModeID;
        totalGames = 0;
        totalWins = 0;
        totalLosses = 0;
        totalKills = 0;
        totalDeaths = 0;
        totalAssists = 0;
        winRate = 0;
        kda = 0;
        lastPlayedDate = "";
    }
    
    public void AddGame(bool won, int kills, int deaths, int assists)
    {
        totalGames++;
        if (won)
        {
            totalWins++;
        }
        else
        {
            totalLosses++;
        }
        totalKills += kills;
        totalDeaths += deaths;
        totalAssists += assists;
        UpdateStats();
        lastPlayedDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    private void UpdateStats()
    {
        if (totalGames > 0)
        {
            winRate = (float)totalWins / totalGames * 100;
        }
        
        if (totalDeaths > 0)
        {
            kda = (float)(totalKills + totalAssists) / totalDeaths;
        }
        else if (totalKills + totalAssists > 0)
        {
            kda = (float)(totalKills + totalAssists);
        }
    }
}

[System.Serializable]
public class GameModeSystemDetailedManagerData
{
    public GameModeSystemDetailed system;
    
    public GameModeSystemDetailedManagerData()
    {
        system = new GameModeSystemDetailed("game_mode_system_detailed", "游戏模式系统详细", "管理游戏模式的详细功能，包括更多游戏模式和规则");
    }
}
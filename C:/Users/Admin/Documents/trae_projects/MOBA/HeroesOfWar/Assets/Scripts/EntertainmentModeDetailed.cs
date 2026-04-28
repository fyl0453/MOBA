[System.Serializable]
public class EntertainmentModeDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<EntertainmentMode> modes;
    public List<ModeMatch> matches;
    public List<ModeRule> rules;
    public List<ModeMap> maps;
    
    public EntertainmentModeDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        modes = new List<EntertainmentMode>();
        matches = new List<ModeMatch>();
        rules = new List<ModeRule>();
        maps = new List<ModeMap>();
    }
    
    public void AddMode(EntertainmentMode mode)
    {
        modes.Add(mode);
    }
    
    public void AddMatch(ModeMatch match)
    {
        matches.Add(match);
    }
    
    public void AddRule(ModeRule rule)
    {
        rules.Add(rule);
    }
    
    public void AddMap(ModeMap map)
    {
        maps.Add(map);
    }
    
    public EntertainmentMode GetMode(string modeID)
    {
        return modes.Find(m => m.modeID == modeID);
    }
    
    public ModeMatch GetMatch(string matchID)
    {
        return matches.Find(m => m.matchID == matchID);
    }
    
    public ModeRule GetRule(string ruleID)
    {
        return rules.Find(r => r.ruleID == ruleID);
    }
    
    public ModeMap GetMap(string mapID)
    {
        return maps.Find(m => m.mapID == mapID);
    }
    
    public List<EntertainmentMode> GetAllModes()
    {
        return modes;
    }
    
    public List<ModeMatch> GetMatchesByMode(string modeID)
    {
        return matches.FindAll(m => m.modeID == modeID);
    }
    
    public List<ModeRule> GetRulesByMode(string modeID)
    {
        return rules.FindAll(r => r.modeID == modeID);
    }
    
    public List<ModeMap> GetMapsByMode(string modeID)
    {
        return maps.FindAll(m => m.modeID == modeID);
    }
}

[System.Serializable]
public class EntertainmentMode
{
    public string modeID;
    public string modeName;
    public string modeDescription;
    public string modeType;
    public int maxPlayers;
    public int minPlayers;
    public int matchDuration;
    public bool isEnabled;
    public string mapID;
    public List<string> rules;
    public List<string> features;
    
    public EntertainmentMode(string id, string name, string desc, string type, int maxPlayers, int minPlayers, int matchDuration, string mapID)
    {
        modeID = id;
        modeName = name;
        modeDescription = desc;
        modeType = type;
        this.maxPlayers = maxPlayers;
        this.minPlayers = minPlayers;
        this.matchDuration = matchDuration;
        isEnabled = true;
        this.mapID = mapID;
        rules = new List<string>();
        features = new List<string>();
    }
    
    public void AddRule(string ruleID)
    {
        rules.Add(ruleID);
    }
    
    public void AddFeature(string feature)
    {
        features.Add(feature);
    }
    
    public void UpdateMode(string name, string desc, int maxPlayers, int minPlayers, int matchDuration)
    {
        modeName = name;
        modeDescription = desc;
        this.maxPlayers = maxPlayers;
        this.minPlayers = minPlayers;
        this.matchDuration = matchDuration;
    }
}

[System.Serializable]
public class ModeMatch
{
    public string matchID;
    public string modeID;
    public string matchName;
    public string matchStatus;
    public string startTime;
    public string endTime;
    public int playerCount;
    public List<MatchPlayer> players;
    public List<MatchEvent> events;
    
    public ModeMatch(string id, string modeID, string matchName)
    {
        matchID = id;
        this.modeID = modeID;
        this.matchName = matchName;
        matchStatus = "Waiting";
        startTime = "";
        endTime = "";
        playerCount = 0;
        players = new List<MatchPlayer>();
        events = new List<MatchEvent>();
    }
    
    public void AddPlayer(MatchPlayer player)
    {
        players.Add(player);
        playerCount++;
    }
    
    public void RemovePlayer(string playerID)
    {
        MatchPlayer player = players.Find(p => p.playerID == playerID);
        if (player != null)
        {
            players.Remove(player);
            playerCount--;
        }
    }
    
    public void AddEvent(MatchEvent ev)
    {
        events.Add(ev);
    }
    
    public void StartMatch()
    {
        matchStatus = "Running";
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void EndMatch()
    {
        matchStatus = "Ended";
        endTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class MatchPlayer
{
    public string playerID;
    public string playerName;
    public string heroID;
    public int level;
    public int kills;
    public int deaths;
    public int assists;
    public int gold;
    public int score;
    public string team;
    
    public MatchPlayer(string playerID, string playerName, string heroID, string team)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        this.heroID = heroID;
        level = 1;
        kills = 0;
        deaths = 0;
        assists = 0;
        gold = 0;
        score = 0;
        this.team = team;
    }
    
    public void UpdateStats(int kills, int deaths, int assists, int gold, int score)
    {
        this.kills = kills;
        this.deaths = deaths;
        this.assists = assists;
        this.gold = gold;
        this.score = score;
    }
    
    public void LevelUp()
    {
        level++;
    }
}

[System.Serializable]
public class MatchEvent
{
    public string eventID;
    public string eventType;
    public string eventTime;
    public string playerID;
    public string targetID;
    public string description;
    
    public MatchEvent(string id, string type, string playerID, string targetID, string description)
    {
        eventID = id;
        eventType = type;
        eventTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.playerID = playerID;
        this.targetID = targetID;
        this.description = description;
    }
}

[System.Serializable]
public class ModeRule
{
    public string ruleID;
    public string modeID;
    public string ruleName;
    public string ruleDescription;
    public bool isEnabled;
    
    public ModeRule(string id, string modeID, string name, string desc)
    {
        ruleID = id;
        this.modeID = modeID;
        ruleName = name;
        ruleDescription = desc;
        isEnabled = true;
    }
    
    public void UpdateRule(string name, string desc, bool enabled)
    {
        ruleName = name;
        ruleDescription = desc;
        isEnabled = enabled;
    }
}

[System.Serializable]
public class ModeMap
{
    public string mapID;
    public string modeID;
    public string mapName;
    public string mapDescription;
    public string mapType;
    public int mapSize;
    public List<SpawnPoint> spawnPoints;
    public List<MapFeature> features;
    
    public ModeMap(string id, string modeID, string name, string desc, string type, int size)
    {
        mapID = id;
        this.modeID = modeID;
        mapName = name;
        mapDescription = desc;
        mapType = type;
        mapSize = size;
        spawnPoints = new List<SpawnPoint>();
        features = new List<MapFeature>();
    }
    
    public void AddSpawnPoint(SpawnPoint point)
    {
        spawnPoints.Add(point);
    }
    
    public void AddFeature(MapFeature feature)
    {
        features.Add(feature);
    }
}

[System.Serializable]
public class SpawnPoint
{
    public string spawnID;
    public string team;
    public float x;
    public float y;
    public float z;
    public bool isActive;
    
    public SpawnPoint(string id, string team, float x, float y, float z)
    {
        spawnID = id;
        this.team = team;
        this.x = x;
        this.y = y;
        this.z = z;
        isActive = true;
    }
}

[System.Serializable]
public class MapFeature
{
    public string featureID;
    public string featureName;
    public string featureType;
    public float x;
    public float y;
    public float z;
    public bool isActive;
    public string description;
    
    public MapFeature(string id, string name, string type, float x, float y, float z, string desc)
    {
        featureID = id;
        featureName = name;
        featureType = type;
        this.x = x;
        this.y = y;
        this.z = z;
        isActive = true;
        description = desc;
    }
}

[System.Serializable]
public class EntertainmentModeDetailedManagerData
{
    public EntertainmentModeDetailed system;
    
    public EntertainmentModeDetailedManagerData()
    {
        system = new EntertainmentModeDetailed("entertainment_mode_detailed", "娱乐模式详细系统", "管理各种娱乐模式的详细规则和玩法");
    }
}
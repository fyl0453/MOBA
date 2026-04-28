[System.Serializable]
public class CrossServerMatchmakingSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Server> servers;
    public List<CrossServerQueue> queues;
    public List<CrossServerMatch> matches;
    
    public CrossServerMatchmakingSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        servers = new List<Server>();
        queues = new List<CrossServerQueue>();
        matches = new List<CrossServerMatch>();
    }
    
    public void AddServer(Server server)
    {
        servers.Add(server);
    }
    
    public void AddQueue(CrossServerQueue queue)
    {
        queues.Add(queue);
    }
    
    public void AddMatch(CrossServerMatch match)
    {
        matches.Add(match);
    }
    
    public Server GetServer(string serverID)
    {
        return servers.Find(s => s.serverID == serverID);
    }
    
    public CrossServerQueue GetQueue(string queueID)
    {
        return queues.Find(q => q.queueID == queueID);
    }
    
    public CrossServerMatch GetMatch(string matchID)
    {
        return matches.Find(m => m.matchID == matchID);
    }
    
    public List<Server> GetActiveServers()
    {
        return servers.FindAll(s => s.isOnline);
    }
    
    public List<CrossServerQueue> GetActiveQueues()
    {
        return queues.FindAll(q => q.isEnabled);
    }
}

[System.Serializable]
public class Server
{
    public string serverID;
    public string serverName;
    public string serverRegion;
    public int playerCount;
    public int maxPlayerCount;
    public bool isOnline;
    public float ping;
    public string serverStatus;
    
    public Server(string id, string name, string region, int maxPlayers)
    {
        serverID = id;
        serverName = name;
        serverRegion = region;
        playerCount = 0;
        maxPlayerCount = maxPlayers;
        isOnline = true;
        ping = 0f;
        serverStatus = "Normal";
    }
    
    public void SetOnline(bool online)
    {
        isOnline = online;
        serverStatus = online ? "Normal" : "Offline";
    }
    
    public void UpdatePlayerCount(int count)
    {
        playerCount = count;
    }
    
    public void UpdatePing(float ping)
    {
        this.ping = ping;
    }
    
    public void SetStatus(string status)
    {
        serverStatus = status;
    }
}

[System.Serializable]
public class CrossServerQueue
{
    public string queueID;
    public string queueName;
    public string queueType;
    public int minPlayers;
    public int maxPlayers;
    public int teamSize;
    public List<CrossServerPlayer> players;
    public bool isEnabled;
    public float averageWaitTime;
    
    public CrossServerQueue(string id, string name, string type, int min, int max, int teamSize)
    {
        queueID = id;
        queueName = name;
        queueType = type;
        minPlayers = min;
        maxPlayers = max;
        this.teamSize = teamSize;
        players = new List<CrossServerPlayer>();
        isEnabled = true;
        averageWaitTime = 0f;
    }
    
    public void AddPlayer(CrossServerPlayer player)
    {
        players.Add(player);
    }
    
    public void RemovePlayer(string playerID)
    {
        players.RemoveAll(p => p.playerID == playerID);
    }
    
    public int GetPlayerCount()
    {
        return players.Count;
    }
    
    public bool HasEnoughPlayers()
    {
        return players.Count >= minPlayers;
    }
    
    public List<CrossServerPlayer> GetPlayers()
    {
        return players;
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
public class CrossServerPlayer
{
    public string playerID;
    public string playerName;
    public string serverID;
    public int rankPoints;
    public int level;
    public string queueID;
    public float joinTime;
    public float waitTime;
    public bool isReady;
    
    public CrossServerPlayer(string id, string name, string server, int rank, int lvl, string queue)
    {
        playerID = id;
        playerName = name;
        serverID = server;
        rankPoints = rank;
        level = lvl;
        queueID = queue;
        joinTime = Time.time;
        waitTime = 0f;
        isReady = false;
    }
    
    public void UpdateWaitTime()
    {
        waitTime = Time.time - joinTime;
    }
    
    public void SetReady(bool ready)
    {
        isReady = ready;
    }
}

[System.Serializable]
public class CrossServerMatch
{
    public string matchID;
    public string matchName;
    public string queueID;
    public List<CrossServerTeam> teams;
    public string serverID;
    public float matchStartTime;
    public float matchEndTime;
    public string matchStatus;
    
    public CrossServerMatch(string id, string name, string queue, string server)
    {
        matchID = id;
        matchName = name;
        queueID = queue;
        teams = new List<CrossServerTeam>();
        serverID = server;
        matchStartTime = Time.time;
        matchEndTime = 0f;
        matchStatus = "Waiting";
    }
    
    public void AddTeam(CrossServerTeam team)
    {
        teams.Add(team);
    }
    
    public void StartMatch()
    {
        matchStatus = "InProgress";
    }
    
    public void EndMatch()
    {
        matchStatus = "Ended";
        matchEndTime = Time.time;
    }
    
    public float GetMatchDuration()
    {
        return matchEndTime > 0 ? matchEndTime - matchStartTime : Time.time - matchStartTime;
    }
}

[System.Serializable]
public class CrossServerTeam
{
    public string teamID;
    public string teamName;
    public List<CrossServerPlayer> players;
    public bool isWinner;
    
    public CrossServerTeam(string id, string name)
    {
        teamID = id;
        teamName = name;
        players = new List<CrossServerPlayer>();
        isWinner = false;
    }
    
    public void AddPlayer(CrossServerPlayer player)
    {
        players.Add(player);
    }
    
    public int GetPlayerCount()
    {
        return players.Count;
    }
    
    public void SetWinner(bool winner)
    {
        isWinner = winner;
    }
}

[System.Serializable]
public class CrossServerMatchmakingManagerData
{
    public CrossServerMatchmakingSystem system;
    
    public CrossServerMatchmakingManagerData()
    {
        system = new CrossServerMatchmakingSystem("cross_server_matchmaking", "跨服匹配系统", "管理不同服务器之间的匹配");
    }
}
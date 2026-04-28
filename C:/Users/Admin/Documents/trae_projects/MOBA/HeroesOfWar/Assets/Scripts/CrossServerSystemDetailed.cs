[System.Serializable]
public class CrossServerSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Server> servers;
    public List<CrossServerMatch> crossServerMatches;
    public List<CrossServerParticipant> crossServerParticipants;
    public List<CrossServerEvent> crossServerEvents;

    public CrossServerSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        servers = new List<Server>();
        crossServerMatches = new List<CrossServerMatch>();
        crossServerParticipants = new List<CrossServerParticipant>();
        crossServerEvents = new List<CrossServerEvent>();
    }

    public void AddServer(Server server)
    {
        servers.Add(server);
    }

    public void AddCrossServerMatch(CrossServerMatch crossServerMatch)
    {
        crossServerMatches.Add(crossServerMatch);
    }

    public void AddCrossServerParticipant(CrossServerParticipant crossServerParticipant)
    {
        crossServerParticipants.Add(crossServerParticipant);
    }

    public void AddCrossServerEvent(CrossServerEvent crossServerEvent)
    {
        crossServerEvents.Add(crossServerEvent);
    }

    public Server GetServer(string serverID)
    {
        return servers.Find(s => s.serverID == serverID);
    }

    public CrossServerMatch GetCrossServerMatch(string matchID)
    {
        return crossServerMatches.Find(csm => csm.matchID == matchID);
    }

    public CrossServerParticipant GetCrossServerParticipant(string participantID)
    {
        return crossServerParticipants.Find(csp => csp.participantID == participantID);
    }

    public CrossServerEvent GetCrossServerEvent(string eventID)
    {
        return crossServerEvents.Find(cse => cse.eventID == eventID);
    }

    public List<CrossServerMatch> GetCrossServerMatchesByStatus(string status)
    {
        return crossServerMatches.FindAll(csm => csm.status == status);
    }

    public List<CrossServerParticipant> GetCrossServerParticipantsByMatch(string matchID)
    {
        return crossServerParticipants.FindAll(csp => csp.matchID == matchID);
    }

    public List<CrossServerEvent> GetCrossServerEventsByUser(string userID)
    {
        return crossServerEvents.FindAll(cse => cse.userID == userID);
    }
}

[System.Serializable]
public class Server
{
    public string serverID;
    public string serverName;
    public string serverRegion;
    public int onlinePlayers;
    public int maxPlayers;
    public string status;

    public Server(string id, string serverName, string serverRegion, int maxPlayers)
    {
        serverID = id;
        this.serverName = serverName;
        this.serverRegion = serverRegion;
        onlinePlayers = 0;
        this.maxPlayers = maxPlayers;
        status = "online";
    }
}

[System.Serializable]
public class CrossServerMatch
{
    public string matchID;
    public string matchType;
    public string status;
    public string server1ID;
    public string server2ID;
    public string winnerServer;
    public int server1Score;
    public int server2Score;
    public string startTime;
    public string endTime;

    public CrossServerMatch(string id, string matchType, string server1ID, string server2ID)
    {
        matchID = id;
        this.matchType = matchType;
        status = "waiting";
        this.server1ID = server1ID;
        this.server2ID = server2ID;
        winnerServer = "";
        server1Score = 0;
        server2Score = 0;
        startTime = "";
        endTime = "";
    }

    public void Start()
    {
        status = "active";
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void End(string winner, int score1, int score2)
    {
        status = "completed";
        winnerServer = winner;
        server1Score = score1;
        server2Score = score2;
        endTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CrossServerParticipant
{
    public string participantID;
    public string matchID;
    public string userID;
    public string userName;
    public string serverID;
    public string heroID;
    public string status;

    public CrossServerParticipant(string id, string matchID, string userID, string userName, string serverID, string heroID)
    {
        participantID = id;
        this.matchID = matchID;
        this.userID = userID;
        this.userName = userName;
        this.serverID = serverID;
        this.heroID = heroID;
        status = "joined";
    }
}

[System.Serializable]
public class CrossServerEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string matchID;
    public string description;
    public string timestamp;

    public CrossServerEvent(string id, string eventType, string userID, string matchID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.matchID = matchID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CrossServerSystemDetailedManagerData
{
    public CrossServerSystemDetailed system;

    public CrossServerSystemDetailedManagerData()
    {
        system = new CrossServerSystemDetailed("cross_server_system_detailed", "跨服匹配系统详细", "管理跨服对战的详细功能");
    }
}
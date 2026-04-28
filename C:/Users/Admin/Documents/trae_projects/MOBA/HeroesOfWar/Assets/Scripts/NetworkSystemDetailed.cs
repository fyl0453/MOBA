[System.Serializable]
public class NetworkSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Server> servers;
    public List<Client> clients;
    public List<Matchmaking> matchmaking;
    public List<Room> rooms;
    public List<NetworkMessage> networkMessages;
    public List<NetworkEvent> networkEvents;
    public List<Connection> connections;
    
    public NetworkSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        servers = new List<Server>();
        clients = new List<Client>();
        matchmaking = new List<Matchmaking>();
        rooms = new List<Room>();
        networkMessages = new List<NetworkMessage>();
        networkEvents = new List<NetworkEvent>();
        connections = new List<Connection>();
    }
    
    public void AddServer(Server server)
    {
        servers.Add(server);
    }
    
    public void AddClient(Client client)
    {
        clients.Add(client);
    }
    
    public void AddMatchmaking(Matchmaking matchmaker)
    {
        matchmaking.Add(matchmaker);
    }
    
    public void AddRoom(Room room)
    {
        rooms.Add(room);
    }
    
    public void AddNetworkMessage(NetworkMessage message)
    {
        networkMessages.Add(message);
    }
    
    public void AddNetworkEvent(NetworkEvent networkEvent)
    {
        networkEvents.Add(networkEvent);
    }
    
    public void AddConnection(Connection connection)
    {
        connections.Add(connection);
    }
    
    public Server GetServer(string serverID)
    {
        return servers.Find(s => s.serverID == serverID);
    }
    
    public Client GetClient(string clientID)
    {
        return clients.Find(c => c.clientID == clientID);
    }
    
    public Matchmaking GetMatchmaking(string matchmakingID)
    {
        return matchmaking.Find(m => m.matchmakingID == matchmakingID);
    }
    
    public Room GetRoom(string roomID)
    {
        return rooms.Find(r => r.roomID == roomID);
    }
    
    public NetworkMessage GetNetworkMessage(string messageID)
    {
        return networkMessages.Find(m => m.messageID == messageID);
    }
    
    public NetworkEvent GetNetworkEvent(string eventID)
    {
        return networkEvents.Find(e => e.eventID == eventID);
    }
    
    public Connection GetConnection(string connectionID)
    {
        return connections.Find(c => c.connectionID == connectionID);
    }
    
    public List<Server> GetServersByRegion(string region)
    {
        return servers.FindAll(s => s.region == region);
    }
    
    public List<Client> GetClientsByServer(string serverID)
    {
        return clients.FindAll(c => c.serverID == serverID);
    }
    
    public List<Room> GetRoomsByServer(string serverID)
    {
        return rooms.FindAll(r => r.serverID == serverID);
    }
    
    public List<Matchmaking> GetMatchmakingByGameMode(string gameModeID)
    {
        return matchmaking.FindAll(m => m.gameModeID == gameModeID);
    }
    
    public List<Connection> GetConnectionsByClient(string clientID)
    {
        return connections.FindAll(c => c.clientID == clientID);
    }
}

[System.Serializable]
public class Server
{
    public string serverID;
    public string serverName;
    public string region;
    public string ipAddress;
    public int port;
    public int maxPlayers;
    public int currentPlayers;
    public string status;
    public string serverType;
    public string version;
    public bool isEnabled;
    
    public Server(string id, string name, string region, string ipAddress, int port, int maxPlayers, string serverType, string version)
    {
        serverID = id;
        serverName = name;
        this.region = region;
        this.ipAddress = ipAddress;
        this.port = port;
        this.maxPlayers = maxPlayers;
        currentPlayers = 0;
        status = "online";
        this.serverType = serverType;
        this.version = version;
        isEnabled = true;
    }
    
    public void IncrementPlayerCount()
    {
        if (currentPlayers < maxPlayers)
        {
            currentPlayers++;
        }
    }
    
    public void DecrementPlayerCount()
    {
        if (currentPlayers > 0)
        {
            currentPlayers--;
        }
    }
    
    public void SetStatus(string status)
    {
        this.status = status;
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
public class Client
{
    public string clientID;
    public string userID;
    public string clientName;
    public string serverID;
    public string ipAddress;
    public int port;
    public string status;
    public string clientType;
    public string version;
    public string lastHeartbeat;
    
    public Client(string id, string userID, string clientName, string serverID, string ipAddress, int port, string clientType, string version)
    {
        clientID = id;
        this.userID = userID;
        this.clientName = clientName;
        this.serverID = serverID;
        this.ipAddress = ipAddress;
        this.port = port;
        status = "connected";
        this.clientType = clientType;
        this.version = version;
        lastHeartbeat = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateHeartbeat()
    {
        lastHeartbeat = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void SetStatus(string status)
    {
        this.status = status;
    }
    
    public void SetServer(string serverID)
    {
        this.serverID = serverID;
    }
}

[System.Serializable]
public class Matchmaking
{
    public string matchmakingID;
    public string gameModeID;
    public string serverID;
    public string status;
    public int queueSize;
    public int minPlayers;
    public int maxPlayers;
    public float matchmakingTime;
    public List<string> playerIDs;
    public string createTime;
    
    public Matchmaking(string id, string gameModeID, string serverID, int minPlayers, int maxPlayers)
    {
        matchmakingID = id;
        this.gameModeID = gameModeID;
        this.serverID = serverID;
        status = "waiting";
        queueSize = 0;
        this.minPlayers = minPlayers;
        this.maxPlayers = maxPlayers;
        matchmakingTime = 0;
        playerIDs = new List<string>();
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddPlayer(string playerID)
    {
        if (!playerIDs.Contains(playerID))
        {
            playerIDs.Add(playerID);
            queueSize++;
        }
    }
    
    public void RemovePlayer(string playerID)
    {
        if (playerIDs.Contains(playerID))
        {
            playerIDs.Remove(playerID);
            queueSize--;
        }
    }
    
    public void SetStatus(string status)
    {
        this.status = status;
    }
    
    public void SetMatchmakingTime(float time)
    {
        matchmakingTime = time;
    }
}

[System.Serializable]
public class Room
{
    public string roomID;
    public string serverID;
    public string gameModeID;
    public string roomName;
    public string status;
    public int maxPlayers;
    public int currentPlayers;
    public string hostID;
    public string password;
    public bool isPrivate;
    public string createTime;
    public string startGameTime;
    public List<string> playerIDs;
    
    public Room(string id, string serverID, string gameModeID, string roomName, int maxPlayers, string hostID, string password, bool isPrivate)
    {
        roomID = id;
        this.serverID = serverID;
        this.gameModeID = gameModeID;
        this.roomName = roomName;
        status = "waiting";
        this.maxPlayers = maxPlayers;
        currentPlayers = 1;
        this.hostID = hostID;
        this.password = password;
        this.isPrivate = isPrivate;
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        startGameTime = "";
        playerIDs = new List<string>();
        playerIDs.Add(hostID);
    }
    
    public void AddPlayer(string playerID)
    {
        if (!playerIDs.Contains(playerID) && currentPlayers < maxPlayers)
        {
            playerIDs.Add(playerID);
            currentPlayers++;
        }
    }
    
    public void RemovePlayer(string playerID)
    {
        if (playerIDs.Contains(playerID))
        {
            playerIDs.Remove(playerID);
            currentPlayers--;
        }
    }
    
    public void SetStatus(string status)
    {
        this.status = status;
    }
    
    public void StartGame()
    {
        status = "playing";
        startGameTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void EndGame()
    {
        status = "completed";
    }
}

[System.Serializable]
public class NetworkMessage
{
    public string messageID;
    public string senderID;
    public string receiverID;
    public string messageType;
    public string content;
    public string status;
    public string sendTime;
    public string receiveTime;
    
    public NetworkMessage(string id, string senderID, string receiverID, string messageType, string content)
    {
        messageID = id;
        this.senderID = senderID;
        this.receiverID = receiverID;
        this.messageType = messageType;
        this.content = content;
        status = "sent";
        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        receiveTime = "";
    }
    
    public void MarkAsReceived()
    {
        status = "received";
        receiveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class NetworkEvent
{
    public string eventID;
    public string eventType;
    public string serverID;
    public string clientID;
    public string description;
    public string timestamp;
    public string status;
    
    public NetworkEvent(string id, string eventType, string serverID, string clientID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.serverID = serverID;
        this.clientID = clientID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
    }
    
    public void MarkAsCompleted()
    {
        status = "completed";
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class Connection
{
    public string connectionID;
    public string clientID;
    public string serverID;
    public string connectionType;
    public string status;
    public string ipAddress;
    public int port;
    public string connectTime;
    public string disconnectTime;
    public float ping;
    
    public Connection(string id, string clientID, string serverID, string connectionType, string ipAddress, int port)
    {
        connectionID = id;
        this.clientID = clientID;
        this.serverID = serverID;
        this.connectionType = connectionType;
        status = "connected";
        this.ipAddress = ipAddress;
        this.port = port;
        connectTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        disconnectTime = "";
        ping = 0;
    }
    
    public void SetStatus(string status)
    {
        this.status = status;
    }
    
    public void SetPing(float ping)
    {
        this.ping = ping;
    }
    
    public void Disconnect()
    {
        status = "disconnected";
        disconnectTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class NetworkSystemDetailedManagerData
{
    public NetworkSystemDetailed system;
    
    public NetworkSystemDetailedManagerData()
    {
        system = new NetworkSystemDetailed("network_system_detailed", "网络系统详细", "管理网络的详细功能，包括多人在线对战和实时匹配");
    }
}
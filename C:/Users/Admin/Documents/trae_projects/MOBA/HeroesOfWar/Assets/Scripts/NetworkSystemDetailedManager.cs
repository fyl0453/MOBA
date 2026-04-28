using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class NetworkSystemDetailedManager : MonoBehaviour
{
    public static NetworkSystemDetailedManager Instance { get; private set; }
    
    public NetworkSystemDetailedManagerData networkData;
    
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
        LoadNetworkData();
        
        if (networkData == null)
        {
            networkData = new NetworkSystemDetailedManagerData();
            InitializeDefaultNetworkSystem();
        }
    }
    
    private void InitializeDefaultNetworkSystem()
    {
        // 服务器
        Server server1 = new Server("server_001", "北京服务器", "China-North", "127.0.0.1", 8080, 10000, "game", "1.0.0");
        Server server2 = new Server("server_002", "上海服务器", "China-East", "127.0.0.1", 8081, 10000, "game", "1.0.0");
        Server server3 = new Server("server_003", "广州服务器", "China-South", "127.0.0.1", 8082, 10000, "game", "1.0.0");
        Server server4 = new Server("server_004", "成都服务器", "China-West", "127.0.0.1", 8083, 10000, "game", "1.0.0");
        
        networkData.system.AddServer(server1);
        networkData.system.AddServer(server2);
        networkData.system.AddServer(server3);
        networkData.system.AddServer(server4);
        
        // 客户端
        Client client1 = new Client("client_001", "user_001", "张三", "server_001", "192.168.1.100", 12345, "mobile", "1.0.0");
        Client client2 = new Client("client_002", "user_002", "李四", "server_001", "192.168.1.101", 12346, "pc", "1.0.0");
        Client client3 = new Client("client_003", "user_003", "王五", "server_002", "192.168.1.102", 12347, "mobile", "1.0.0");
        Client client4 = new Client("client_004", "user_004", "赵六", "server_003", "192.168.1.103", 12348, "pc", "1.0.0");
        
        networkData.system.AddClient(client1);
        networkData.system.AddClient(client2);
        networkData.system.AddClient(client3);
        networkData.system.AddClient(client4);
        
        // 匹配
        Matchmaking matchmaking1 = new Matchmaking("matchmaking_001", "game_mode_001", "server_001", 10, 10);
        matchmaking1.AddPlayer("user_001");
        matchmaking1.AddPlayer("user_002");
        matchmaking1.AddPlayer("user_003");
        matchmaking1.AddPlayer("user_004");
        
        Matchmaking matchmaking2 = new Matchmaking("matchmaking_002", "game_mode_002", "server_001", 10, 10);
        matchmaking2.AddPlayer("user_001");
        matchmaking2.AddPlayer("user_002");
        
        networkData.system.AddMatchmaking(matchmaking1);
        networkData.system.AddMatchmaking(matchmaking2);
        
        // 房间
        Room room1 = new Room("room_001", "server_001", "game_mode_001", "自定义房间1", 10, "user_001", "", false);
        room1.AddPlayer("user_002");
        room1.AddPlayer("user_003");
        room1.AddPlayer("user_004");
        
        Room room2 = new Room("room_002", "server_002", "game_mode_002", "自定义房间2", 10, "user_003", "123456", true);
        room2.AddPlayer("user_004");
        
        networkData.system.AddRoom(room1);
        networkData.system.AddRoom(room2);
        
        // 网络消息
        NetworkMessage message1 = new NetworkMessage("message_001", "client_001", "client_002", "chat", "你好，一起玩游戏吗？");
        NetworkMessage message2 = new NetworkMessage("message_002", "client_002", "client_001", "chat", "好的，马上来");
        NetworkMessage message3 = new NetworkMessage("message_003", "client_001", "server_001", "ping", "ping");
        
        networkData.system.AddNetworkMessage(message1);
        networkData.system.AddNetworkMessage(message2);
        networkData.system.AddNetworkMessage(message3);
        
        // 网络事件
        NetworkEvent event1 = new NetworkEvent("event_001", "connect", "server_001", "client_001", "客户端连接服务器");
        NetworkEvent event2 = new NetworkEvent("event_002", "disconnect", "server_001", "client_002", "客户端断开连接");
        NetworkEvent event3 = new NetworkEvent("event_003", "matchmaking", "server_001", "client_001", "开始匹配");
        
        networkData.system.AddNetworkEvent(event1);
        networkData.system.AddNetworkEvent(event2);
        networkData.system.AddNetworkEvent(event3);
        
        // 连接
        Connection connection1 = new Connection("connection_001", "client_001", "server_001", "tcp", "192.168.1.100", 12345);
        Connection connection2 = new Connection("connection_002", "client_002", "server_001", "tcp", "192.168.1.101", 12346);
        Connection connection3 = new Connection("connection_003", "client_003", "server_002", "tcp", "192.168.1.102", 12347);
        
        networkData.system.AddConnection(connection1);
        networkData.system.AddConnection(connection2);
        networkData.system.AddConnection(connection3);
        
        SaveNetworkData();
    }
    
    // 服务器管理
    public void AddServer(string name, string region, string ipAddress, int port, int maxPlayers, string serverType, string version)
    {
        string serverID = "server_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Server server = new Server(serverID, name, region, ipAddress, port, maxPlayers, serverType, version);
        networkData.system.AddServer(server);
        SaveNetworkData();
        Debug.Log("成功添加服务器: " + name);
    }
    
    public List<Server> GetServersByRegion(string region)
    {
        return networkData.system.GetServersByRegion(region);
    }
    
    public List<Server> GetAllServers()
    {
        return networkData.system.servers;
    }
    
    public void EnableServer(string serverID)
    {
        Server server = networkData.system.GetServer(serverID);
        if (server != null)
        {
            server.Enable();
            SaveNetworkData();
            Debug.Log("成功启用服务器: " + server.serverName);
        }
        else
        {
            Debug.LogError("服务器不存在: " + serverID);
        }
    }
    
    public void DisableServer(string serverID)
    {
        Server server = networkData.system.GetServer(serverID);
        if (server != null)
        {
            server.Disable();
            SaveNetworkData();
            Debug.Log("成功禁用服务器: " + server.serverName);
        }
        else
        {
            Debug.LogError("服务器不存在: " + serverID);
        }
    }
    
    public void SetServerStatus(string serverID, string status)
    {
        Server server = networkData.system.GetServer(serverID);
        if (server != null)
        {
            server.SetStatus(status);
            SaveNetworkData();
            Debug.Log("成功设置服务器状态: " + status);
        }
        else
        {
            Debug.LogError("服务器不存在: " + serverID);
        }
    }
    
    // 客户端管理
    public void AddClient(string userID, string clientName, string serverID, string ipAddress, int port, string clientType, string version)
    {
        string clientID = "client_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Client client = new Client(clientID, userID, clientName, serverID, ipAddress, port, clientType, version);
        networkData.system.AddClient(client);
        
        // 更新服务器玩家数
        Server server = networkData.system.GetServer(serverID);
        if (server != null)
        {
            server.IncrementPlayerCount();
        }
        
        SaveNetworkData();
        Debug.Log("成功添加客户端: " + clientName);
    }
    
    public void UpdateClientHeartbeat(string clientID)
    {
        Client client = networkData.system.GetClient(clientID);
        if (client != null)
        {
            client.UpdateHeartbeat();
            SaveNetworkData();
            Debug.Log("成功更新客户端心跳: " + client.clientName);
        }
        else
        {
            Debug.LogError("客户端不存在: " + clientID);
        }
    }
    
    public void SetClientStatus(string clientID, string status)
    {
        Client client = networkData.system.GetClient(clientID);
        if (client != null)
        {
            client.SetStatus(status);
            SaveNetworkData();
            Debug.Log("成功设置客户端状态: " + status);
        }
        else
        {
            Debug.LogError("客户端不存在: " + clientID);
        }
    }
    
    public void SetClientServer(string clientID, string serverID)
    {
        Client client = networkData.system.GetClient(clientID);
        if (client != null)
        {
            // 减少旧服务器玩家数
            Server oldServer = networkData.system.GetServer(client.serverID);
            if (oldServer != null)
            {
                oldServer.DecrementPlayerCount();
            }
            
            // 设置新服务器
            client.SetServer(serverID);
            
            // 增加新服务器玩家数
            Server newServer = networkData.system.GetServer(serverID);
            if (newServer != null)
            {
                newServer.IncrementPlayerCount();
            }
            
            SaveNetworkData();
            Debug.Log("成功设置客户端服务器: " + serverID);
        }
        else
        {
            Debug.LogError("客户端不存在: " + clientID);
        }
    }
    
    // 匹配管理
    public string CreateMatchmaking(string gameModeID, string serverID, int minPlayers, int maxPlayers)
    {
        string matchmakingID = "matchmaking_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Matchmaking matchmaking = new Matchmaking(matchmakingID, gameModeID, serverID, minPlayers, maxPlayers);
        networkData.system.AddMatchmaking(matchmaking);
        SaveNetworkData();
        Debug.Log("成功创建匹配: " + matchmakingID);
        return matchmakingID;
    }
    
    public void AddPlayerToMatchmaking(string matchmakingID, string playerID)
    {
        Matchmaking matchmaking = networkData.system.GetMatchmaking(matchmakingID);
        if (matchmaking != null)
        {
            matchmaking.AddPlayer(playerID);
            SaveNetworkData();
            Debug.Log("成功添加玩家到匹配: " + playerID);
        }
        else
        {
            Debug.LogError("匹配不存在: " + matchmakingID);
        }
    }
    
    public void RemovePlayerFromMatchmaking(string matchmakingID, string playerID)
    {
        Matchmaking matchmaking = networkData.system.GetMatchmaking(matchmakingID);
        if (matchmaking != null)
        {
            matchmaking.RemovePlayer(playerID);
            SaveNetworkData();
            Debug.Log("成功从匹配移除玩家: " + playerID);
        }
        else
        {
            Debug.LogError("匹配不存在: " + matchmakingID);
        }
    }
    
    public void SetMatchmakingStatus(string matchmakingID, string status)
    {
        Matchmaking matchmaking = networkData.system.GetMatchmaking(matchmakingID);
        if (matchmaking != null)
        {
            matchmaking.SetStatus(status);
            SaveNetworkData();
            Debug.Log("成功设置匹配状态: " + status);
        }
        else
        {
            Debug.LogError("匹配不存在: " + matchmakingID);
        }
    }
    
    // 房间管理
    public string CreateRoom(string serverID, string gameModeID, string roomName, int maxPlayers, string hostID, string password, bool isPrivate)
    {
        string roomID = "room_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Room room = new Room(roomID, serverID, gameModeID, roomName, maxPlayers, hostID, password, isPrivate);
        networkData.system.AddRoom(room);
        SaveNetworkData();
        Debug.Log("成功创建房间: " + roomName);
        return roomID;
    }
    
    public void AddPlayerToRoom(string roomID, string playerID)
    {
        Room room = networkData.system.GetRoom(roomID);
        if (room != null)
        {
            room.AddPlayer(playerID);
            SaveNetworkData();
            Debug.Log("成功添加玩家到房间: " + playerID);
        }
        else
        {
            Debug.LogError("房间不存在: " + roomID);
        }
    }
    
    public void RemovePlayerFromRoom(string roomID, string playerID)
    {
        Room room = networkData.system.GetRoom(roomID);
        if (room != null)
        {
            room.RemovePlayer(playerID);
            SaveNetworkData();
            Debug.Log("成功从房间移除玩家: " + playerID);
        }
        else
        {
            Debug.LogError("房间不存在: " + roomID);
        }
    }
    
    public void StartGameInRoom(string roomID)
    {
        Room room = networkData.system.GetRoom(roomID);
        if (room != null)
        {
            room.StartGame();
            SaveNetworkData();
            Debug.Log("成功开始游戏: " + room.roomName);
        }
        else
        {
            Debug.LogError("房间不存在: " + roomID);
        }
    }
    
    public void EndGameInRoom(string roomID)
    {
        Room room = networkData.system.GetRoom(roomID);
        if (room != null)
        {
            room.EndGame();
            SaveNetworkData();
            Debug.Log("成功结束游戏: " + room.roomName);
        }
        else
        {
            Debug.LogError("房间不存在: " + roomID);
        }
    }
    
    // 网络消息管理
    public string SendNetworkMessage(string senderID, string receiverID, string messageType, string content)
    {
        string messageID = "message_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        NetworkMessage message = new NetworkMessage(messageID, senderID, receiverID, messageType, content);
        networkData.system.AddNetworkMessage(message);
        SaveNetworkData();
        Debug.Log("成功发送网络消息");
        return messageID;
    }
    
    public void MarkMessageAsReceived(string messageID)
    {
        NetworkMessage message = networkData.system.GetNetworkMessage(messageID);
        if (message != null)
        {
            message.MarkAsReceived();
            SaveNetworkData();
            Debug.Log("成功标记消息为已接收");
        }
        else
        {
            Debug.LogError("消息不存在: " + messageID);
        }
    }
    
    public void MarkMessageAsFailed(string messageID)
    {
        NetworkMessage message = networkData.system.GetNetworkMessage(messageID);
        if (message != null)
        {
            message.MarkAsFailed();
            SaveNetworkData();
            Debug.Log("成功标记消息为失败");
        }
        else
        {
            Debug.LogError("消息不存在: " + messageID);
        }
    }
    
    // 网络事件管理
    public string CreateNetworkEvent(string eventType, string serverID, string clientID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        NetworkEvent networkEvent = new NetworkEvent(eventID, eventType, serverID, clientID, description);
        networkData.system.AddNetworkEvent(networkEvent);
        SaveNetworkData();
        Debug.Log("成功创建网络事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        NetworkEvent networkEvent = networkData.system.GetNetworkEvent(eventID);
        if (networkEvent != null)
        {
            networkEvent.MarkAsCompleted();
            SaveNetworkData();
            Debug.Log("成功标记事件为完成");
        }
        else
        {
            Debug.LogError("事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        NetworkEvent networkEvent = networkData.system.GetNetworkEvent(eventID);
        if (networkEvent != null)
        {
            networkEvent.MarkAsFailed();
            SaveNetworkData();
            Debug.Log("成功标记事件为失败");
        }
        else
        {
            Debug.LogError("事件不存在: " + eventID);
        }
    }
    
    // 连接管理
    public string CreateConnection(string clientID, string serverID, string connectionType, string ipAddress, int port)
    {
        string connectionID = "connection_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Connection connection = new Connection(connectionID, clientID, serverID, connectionType, ipAddress, port);
        networkData.system.AddConnection(connection);
        SaveNetworkData();
        Debug.Log("成功创建连接: " + connectionID);
        return connectionID;
    }
    
    public void SetConnectionStatus(string connectionID, string status)
    {
        Connection connection = networkData.system.GetConnection(connectionID);
        if (connection != null)
        {
            connection.SetStatus(status);
            SaveNetworkData();
            Debug.Log("成功设置连接状态: " + status);
        }
        else
        {
            Debug.LogError("连接不存在: " + connectionID);
        }
    }
    
    public void SetConnectionPing(string connectionID, float ping)
    {
        Connection connection = networkData.system.GetConnection(connectionID);
        if (connection != null)
        {
            connection.SetPing(ping);
            SaveNetworkData();
            Debug.Log("成功设置连接延迟: " + ping + "ms");
        }
        else
        {
            Debug.LogError("连接不存在: " + connectionID);
        }
    }
    
    public void DisconnectConnection(string connectionID)
    {
        Connection connection = networkData.system.GetConnection(connectionID);
        if (connection != null)
        {
            connection.Disconnect();
            SaveNetworkData();
            Debug.Log("成功断开连接: " + connectionID);
        }
        else
        {
            Debug.LogError("连接不存在: " + connectionID);
        }
    }
    
    // 数据持久化
    public void SaveNetworkData()
    {
        string path = Application.dataPath + "/Data/network_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, networkData);
        stream.Close();
    }
    
    public void LoadNetworkData()
    {
        string path = Application.dataPath + "/Data/network_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            networkData = (NetworkSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            networkData = new NetworkSystemDetailedManagerData();
        }
    }
}
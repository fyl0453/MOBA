using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance { get; private set; }
    
    private ServerData serverData;
    private string selectedServerID;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadServerData();
            
            if (serverData == null)
            {
                serverData = new ServerData();
                InitializeDefaultServers();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeDefaultServers()
    {
        // 添加默认服务器
        AddServer("server_1", "中国大陆-东部", "127.0.0.1", 8080, ServerStatus.Online, 1200);
        AddServer("server_2", "中国大陆-西部", "127.0.0.2", 8080, ServerStatus.Online, 850);
        AddServer("server_3", "中国大陆-南部", "127.0.0.3", 8080, ServerStatus.Online, 950);
        AddServer("server_4", "中国大陆-北部", "127.0.0.4", 8080, ServerStatus.Online, 750);
        AddServer("server_5", "海外-亚洲", "127.0.0.5", 8080, ServerStatus.Online, 450);
        AddServer("server_6", "海外-欧洲", "127.0.0.6", 8080, ServerStatus.Online, 300);
        AddServer("server_7", "海外-美洲", "127.0.0.7", 8080, ServerStatus.Online, 250);
        
        SaveServerData();
    }
    
    private void AddServer(string serverID, string serverName, string ipAddress, int port, ServerStatus status, int playerCount)
    {
        Server server = new Server
        {
            serverID = serverID,
            serverName = serverName,
            ipAddress = ipAddress,
            port = port,
            status = status,
            playerCount = playerCount,
            maxPlayerCount = 5000,
            ping = Random.Range(50, 200) // 模拟延迟
        };
        
        serverData.servers.Add(server);
    }
    
    public List<Server> GetAllServers()
    {
        return serverData.servers;
    }
    
    public Server GetServer(string serverID)
    {
        return serverData.servers.Find(s => s.serverID == serverID);
    }
    
    public void SelectServer(string serverID)
    {
        selectedServerID = serverID;
        serverData.lastSelectedServerID = serverID;
        SaveServerData();
        Debug.Log($"选择服务器: {GetServer(serverID)?.serverName}");
    }
    
    public string GetSelectedServerID()
    {
        return selectedServerID ?? serverData.lastSelectedServerID;
    }
    
    public Server GetSelectedServer()
    {
        string serverID = GetSelectedServerID();
        return GetServer(serverID);
    }
    
    public void UpdateServerStatus(string serverID, ServerStatus status)
    {
        Server server = GetServer(serverID);
        if (server != null)
        {
            server.status = status;
            SaveServerData();
        }
    }
    
    public void UpdateServerPlayerCount(string serverID, int playerCount)
    {
        Server server = GetServer(serverID);
        if (server != null)
        {
            server.playerCount = playerCount;
            SaveServerData();
        }
    }
    
    public void UpdateServerPing(string serverID, int ping)
    {
        Server server = GetServer(serverID);
        if (server != null)
        {
            server.ping = ping;
            SaveServerData();
        }
    }
    
    public void SaveServerData()
    {
        string path = Application.dataPath + "/Data/server_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, serverData);
        stream.Close();
    }
    
    public void LoadServerData()
    {
        string path = Application.dataPath + "/Data/server_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            serverData = (ServerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            serverData = new ServerData();
        }
    }
}

[System.Serializable]
public class ServerData
{
    public List<Server> servers = new List<Server>();
    public string lastSelectedServerID = "server_1"; // 默认选择第一个服务器
}

[System.Serializable]
public class Server
{
    public string serverID;
    public string serverName;
    public string ipAddress;
    public int port;
    public ServerStatus status;
    public int playerCount;
    public int maxPlayerCount;
    public int ping;
}

public enum ServerStatus
{
    Online,
    Busy,
    Full,
    Maintenance,
    Offline
}

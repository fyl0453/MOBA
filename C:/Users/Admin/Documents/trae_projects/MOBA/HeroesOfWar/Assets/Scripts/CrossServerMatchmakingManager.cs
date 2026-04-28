using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CrossServerMatchmakingManager : MonoBehaviour
{
    public static CrossServerMatchmakingManager Instance { get; private set; }
    
    public CrossServerMatchmakingManagerData crossServerData;
    
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
        LoadCrossServerData();
        
        if (crossServerData == null)
        {
            crossServerData = new CrossServerMatchmakingManagerData();
            InitializeDefaultData();
        }
    }
    
    private void InitializeDefaultData()
    {
        // 创建默认服务器
        Server server1 = new Server("server_1", "服务器1", "华东", 10000);
        Server server2 = new Server("server_2", "服务器2", "华北", 10000);
        Server server3 = new Server("server_3", "服务器3", "华南", 10000);
        Server server4 = new Server("server_4", "服务器4", "西南", 10000);
        
        crossServerData.system.AddServer(server1);
        crossServerData.system.AddServer(server2);
        crossServerData.system.AddServer(server3);
        crossServerData.system.AddServer(server4);
        
        // 创建默认跨服队列
        CrossServerQueue classicQueue = new CrossServerQueue("queue_classic", "经典模式", "Normal", 10, 10, 5);
        CrossServerQueue rankedQueue = new CrossServerQueue("queue_ranked", "排位赛", "Ranked", 10, 10, 5);
        CrossServerQueue aramQueue = new CrossServerQueue("queue_aram", "大乱斗", "Arcade", 6, 6, 3);
        
        crossServerData.system.AddQueue(classicQueue);
        crossServerData.system.AddQueue(rankedQueue);
        crossServerData.system.AddQueue(aramQueue);
        
        SaveCrossServerData();
    }
    
    public void StartCrossServerMatchmaking(string queueID, string playerID, string playerName, string serverID, int rankPoints, int level)
    {
        CrossServerQueue queue = crossServerData.system.GetQueue(queueID);
        if (queue != null && queue.isEnabled)
        {
            CrossServerPlayer player = new CrossServerPlayer(playerID, playerName, serverID, rankPoints, level, queueID);
            queue.AddPlayer(player);
            Debug.Log($"玩家 {playerName} 加入跨服匹配队列: {queue.queueName}");
            
            // 检查是否可以创建匹配
            if (queue.HasEnoughPlayers())
            {
                CreateCrossServerMatch(queue);
            }
            
            SaveCrossServerData();
        }
    }
    
    public void CancelCrossServerMatchmaking(string queueID, string playerID)
    {
        CrossServerQueue queue = crossServerData.system.GetQueue(queueID);
        if (queue != null)
        {
            queue.RemovePlayer(playerID);
            Debug.Log($"玩家 {playerID} 取消跨服匹配");
            SaveCrossServerData();
        }
    }
    
    private void CreateCrossServerMatch(CrossServerQueue queue)
    {
        string matchID = System.Guid.NewGuid().ToString();
        string matchName = $"{queue.queueName} - {matchID.Substring(0, 8)}";
        
        // 选择负载最低的服务器
        Server bestServer = null;
        float lowestLoad = float.MaxValue;
        
        foreach (Server server in crossServerData.system.GetActiveServers())
        {
            float load = (float)server.playerCount / server.maxPlayerCount;
            if (load < lowestLoad)
            {
                lowestLoad = load;
                bestServer = server;
            }
        }
        
        if (bestServer == null)
        {
            bestServer = crossServerData.system.servers[0];
        }
        
        CrossServerMatch match = new CrossServerMatch(matchID, matchName, queue.queueID, bestServer.serverID);
        
        // 分配队伍
        List<CrossServerPlayer> players = queue.GetPlayers();
        List<CrossServerPlayer> team1Players = new List<CrossServerPlayer>();
        List<CrossServerPlayer> team2Players = new List<CrossServerPlayer>();
        
        // 按照段位和等级进行匹配，确保队伍实力平衡
        players.Sort((a, b) => b.rankPoints.CompareTo(a.rankPoints));
        
        for (int i = 0; i < players.Count; i++)
        {
            if (i % 2 == 0)
            {
                team1Players.Add(players[i]);
            }
            else
            {
                team2Players.Add(players[i]);
            }
        }
        
        // 创建队伍
        CrossServerTeam team1 = new CrossServerTeam("team1", "蓝色方");
        CrossServerTeam team2 = new CrossServerTeam("team2", "红色方");
        
        foreach (CrossServerPlayer player in team1Players)
        {
            team1.AddPlayer(player);
        }
        
        foreach (CrossServerPlayer player in team2Players)
        {
            team2.AddPlayer(player);
        }
        
        match.AddTeam(team1);
        match.AddTeam(team2);
        
        // 从队列中移除玩家
        foreach (CrossServerPlayer player in players)
        {
            queue.RemovePlayer(player.playerID);
        }
        
        // 添加到活跃匹配
        crossServerData.system.AddMatch(match);
        
        // 开始匹配
        match.StartMatch();
        Debug.Log($"跨服匹配成功！创建了 {match.matchName} 匹配，使用服务器: {bestServer.serverName}");
        
        // 加载游戏场景
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        
        SaveCrossServerData();
    }
    
    public void EndCrossServerMatch(string matchID, string winningTeamID)
    {
        CrossServerMatch match = crossServerData.system.GetMatch(matchID);
        if (match != null)
        {
            // 设置获胜队伍
            foreach (CrossServerTeam team in match.teams)
            {
                team.SetWinner(team.teamID == winningTeamID);
            }
            
            // 结束匹配
            match.EndMatch();
            Debug.Log($"跨服匹配结束！{winningTeamID} 获胜");
            
            SaveCrossServerData();
        }
    }
    
    public void UpdateServerStatus(string serverID, bool online, int playerCount, float ping)
    {
        Server server = crossServerData.system.GetServer(serverID);
        if (server != null)
        {
            server.SetOnline(online);
            server.UpdatePlayerCount(playerCount);
            server.UpdatePing(ping);
            SaveCrossServerData();
        }
    }
    
    public void EnableQueue(string queueID, bool enable)
    {
        CrossServerQueue queue = crossServerData.system.GetQueue(queueID);
        if (queue != null)
        {
            if (enable)
            {
                queue.Enable();
            }
            else
            {
                queue.Disable();
            }
            SaveCrossServerData();
        }
    }
    
    public List<Server> GetActiveServers()
    {
        return crossServerData.system.GetActiveServers();
    }
    
    public List<CrossServerQueue> GetActiveQueues()
    {
        return crossServerData.system.GetActiveQueues();
    }
    
    public List<CrossServerMatch> GetActiveMatches()
    {
        return crossServerData.system.matches.FindAll(m => m.matchStatus == "InProgress");
    }
    
    public Server GetServer(string serverID)
    {
        return crossServerData.system.GetServer(serverID);
    }
    
    public CrossServerQueue GetQueue(string queueID)
    {
        return crossServerData.system.GetQueue(queueID);
    }
    
    public CrossServerMatch GetMatch(string matchID)
    {
        return crossServerData.system.GetMatch(matchID);
    }
    
    public void SaveCrossServerData()
    {
        string path = Application.dataPath + "/Data/cross_server_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, crossServerData);
        stream.Close();
    }
    
    public void LoadCrossServerData()
    {
        string path = Application.dataPath + "/Data/cross_server_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            crossServerData = (CrossServerMatchmakingManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            crossServerData = new CrossServerMatchmakingManagerData();
        }
    }
}
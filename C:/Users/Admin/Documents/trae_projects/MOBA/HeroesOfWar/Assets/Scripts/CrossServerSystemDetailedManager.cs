using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CrossServerSystemDetailedManager : MonoBehaviour
{
    public static CrossServerSystemDetailedManager Instance { get; private set; }

    public CrossServerSystemDetailedManagerData crossServerData;

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
            crossServerData = new CrossServerSystemDetailedManagerData();
            InitializeDefaultCrossServerSystem();
        }
    }

    private void InitializeDefaultCrossServerSystem()
    {
        // 服务器
        Server server1 = new Server("server_001", "区服1", "华东", 10000);
        Server server2 = new Server("server_002", "区服2", "华北", 10000);
        Server server3 = new Server("server_003", "区服3", "华南", 10000);
        Server server4 = new Server("server_004", "区服4", "华中", 10000);
        Server server5 = new Server("server_005", "巅峰赛服", "全国", 50000);

        crossServerData.system.AddServer(server1);
        crossServerData.system.AddServer(server2);
        crossServerData.system.AddServer(server3);
        crossServerData.system.AddServer(server4);
        crossServerData.system.AddServer(server5);

        // 跨服匹配
        CrossServerMatch match1 = new CrossServerMatch("match_001", "跨服1v1", "server_001", "server_002");
        CrossServerMatch match2 = new CrossServerMatch("match_002", "跨服5v5", "server_001", "server_003");

        match1.Start();

        crossServerData.system.AddCrossServerMatch(match1);
        crossServerData.system.AddCrossServerMatch(match2);

        // 跨服参与者
        CrossServerParticipant participant1 = new CrossServerParticipant("participant_001", "match_001", "user_001", "张三", "server_001", "hero_001");
        CrossServerParticipant participant2 = new CrossServerParticipant("participant_002", "match_001", "user_002", "李四", "server_002", "hero_002");
        CrossServerParticipant participant3 = new CrossServerParticipant("participant_003", "match_002", "user_001", "张三", "server_001", "hero_003");

        crossServerData.system.AddCrossServerParticipant(participant1);
        crossServerData.system.AddCrossServerParticipant(participant2);
        crossServerData.system.AddCrossServerParticipant(participant3);

        // 跨服事件
        CrossServerEvent crossServerEvent1 = new CrossServerEvent("event_001", "match_start", "system", "match_001", "跨服匹配开始");
        CrossServerEvent crossServerEvent2 = new CrossServerEvent("event_002", "player_join", "user_001", "match_001", "玩家加入跨服匹配");
        CrossServerEvent crossServerEvent3 = new CrossServerEvent("event_003", "match_end", "system", "match_001", "跨服匹配结束");

        crossServerData.system.AddCrossServerEvent(crossServerEvent1);
        crossServerData.system.AddCrossServerEvent(crossServerEvent2);
        crossServerData.system.AddCrossServerEvent(crossServerEvent3);

        SaveCrossServerData();
    }

    // 服务器管理
    public void AddServer(string serverName, string serverRegion, int maxPlayers)
    {
        string serverID = "server_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Server server = new Server(serverID, serverName, serverRegion, maxPlayers);
        crossServerData.system.AddServer(server);
        SaveCrossServerData();
        Debug.Log("成功添加服务器: " + serverName);
    }

    public Server GetServer(string serverID)
    {
        return crossServerData.system.GetServer(serverID);
    }

    // 跨服匹配管理
    public void CreateCrossServerMatch(string matchType, string server1ID, string server2ID)
    {
        string matchID = "match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CrossServerMatch crossServerMatch = new CrossServerMatch(matchID, matchType, server1ID, server2ID);
        crossServerData.system.AddCrossServerMatch(crossServerMatch);
        CreateCrossServerEvent("match_create", "system", matchID, "创建跨服匹配");
        SaveCrossServerData();
        Debug.Log("成功创建跨服匹配");
    }

    public void StartCrossServerMatch(string matchID)
    {
        CrossServerMatch crossServerMatch = crossServerData.system.GetCrossServerMatch(matchID);
        if (crossServerMatch != null)
        {
            crossServerMatch.Start();
            CreateCrossServerEvent("match_start", "system", matchID, "跨服匹配开始");
            SaveCrossServerData();
            Debug.Log("成功开始跨服匹配");
        }
    }

    public void EndCrossServerMatch(string matchID, string winnerServer, int score1, int score2)
    {
        CrossServerMatch crossServerMatch = crossServerData.system.GetCrossServerMatch(matchID);
        if (crossServerMatch != null)
        {
            crossServerMatch.End(winnerServer, score1, score2);
            CreateCrossServerEvent("match_end", "system", matchID, "跨服匹配结束，获胜服务器: " + winnerServer);
            SaveCrossServerData();
            Debug.Log("成功结束跨服匹配");
        }
    }

    public List<CrossServerMatch> GetCrossServerMatchesByStatus(string status)
    {
        return crossServerData.system.GetCrossServerMatchesByStatus(status);
    }

    // 跨服参与者管理
    public void JoinCrossServerMatch(string matchID, string userID, string userName, string serverID, string heroID)
    {
        CrossServerMatch crossServerMatch = crossServerData.system.GetCrossServerMatch(matchID);
        if (crossServerMatch != null)
        {
            string participantID = "participant_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            CrossServerParticipant participant = new CrossServerParticipant(participantID, matchID, userID, userName, serverID, heroID);
            crossServerData.system.AddCrossServerParticipant(participant);
            CreateCrossServerEvent("player_join", userID, matchID, "玩家加入跨服匹配");
            SaveCrossServerData();
            Debug.Log("成功加入跨服匹配");
        }
    }

    public void LeaveCrossServerMatch(string participantID)
    {
        CrossServerParticipant participant = crossServerData.system.GetCrossServerParticipant(participantID);
        if (participant != null)
        {
            participant.status = "left";
            CreateCrossServerEvent("player_leave", participant.userID, participant.matchID, "玩家离开跨服匹配");
            SaveCrossServerData();
            Debug.Log("成功离开跨服匹配");
        }
    }

    public List<CrossServerParticipant> GetCrossServerParticipants(string matchID)
    {
        return crossServerData.system.GetCrossServerParticipantsByMatch(matchID);
    }

    // 跨服事件管理
    public string CreateCrossServerEvent(string eventType, string userID, string matchID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CrossServerEvent crossServerEvent = new CrossServerEvent(eventID, eventType, userID, matchID, description);
        crossServerData.system.AddCrossServerEvent(crossServerEvent);
        SaveCrossServerData();
        Debug.Log("成功创建跨服事件: " + eventType);
        return eventID;
    }

    public List<CrossServerEvent> GetUserEvents(string userID)
    {
        return crossServerData.system.GetCrossServerEventsByUser(userID);
    }

    // 数据持久化
    public void SaveCrossServerData()
    {
        string path = Application.dataPath + "/Data/cross_server_system_detailed_data.dat";
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
        string path = Application.dataPath + "/Data/cross_server_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            crossServerData = (CrossServerSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            crossServerData = new CrossServerSystemDetailedManagerData();
        }
    }
}
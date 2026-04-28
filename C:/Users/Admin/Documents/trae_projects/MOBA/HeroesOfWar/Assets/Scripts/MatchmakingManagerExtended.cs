using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MatchmakingManagerExtended : MonoBehaviour
{
    public static MatchmakingManagerExtended Instance { get; private set; }
    
    public MatchmakingManagerData matchmakingData;
    
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
        LoadMatchmakingData();
        
        if (matchmakingData == null)
        {
            matchmakingData = new MatchmakingManagerData();
            InitializeDefaultQueues();
        }
    }
    
    private void InitializeDefaultQueues()
    {
        // 5v5 经典模式
        MatchmakingQueue classicQueue = new MatchmakingQueue("queue_classic", "经典模式", MatchType.Normal, 10, 10, 5);
        matchmakingData.AddQueue(classicQueue);
        
        // 5v5 排位赛
        MatchmakingQueue rankedQueue = new MatchmakingQueue("queue_ranked", "排位赛", MatchType.Ranked, 10, 10, 5);
        matchmakingData.AddQueue(rankedQueue);
        
        // 3v3 大乱斗
        MatchmakingQueue aramQueue = new MatchmakingQueue("queue_aram", "大乱斗", MatchType.Arcade, 6, 6, 3);
        matchmakingData.AddQueue(aramQueue);
        
        SaveMatchmakingData();
    }
    
    public void StartMatchmaking(string queueID)
    {
        MatchmakingQueue queue = matchmakingData.GetQueue(queueID);
        if (queue != null)
        {
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            string playerName = ProfileManager.Instance.currentProfile.playerName;
            int rankPoints = ProfileManager.Instance.currentProfile.rankPoints;
            int level = ProfileManager.Instance.currentProfile.level;
            
            PlayerMatchmaking player = new PlayerMatchmaking(playerID, playerName, rankPoints, level, queue.matchType);
            queue.AddPlayer(player);
            
            Debug.Log($"开始匹配 {queue.queueName} 模式...");
            SaveMatchmakingData();
        }
    }
    
    public void CancelMatchmaking(string queueID)
    {
        MatchmakingQueue queue = matchmakingData.GetQueue(queueID);
        if (queue != null)
        {
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            queue.RemovePlayer(playerID);
            Debug.Log("匹配已取消");
            SaveMatchmakingData();
        }
    }
    
    public void CheckMatchmaking()
    {
        foreach (MatchmakingQueue queue in matchmakingData.queues)
        {
            if (queue.HasEnoughPlayers())
            {
                CreateMatch(queue);
            }
        }
    }
    
    private void CreateMatch(MatchmakingQueue queue)
    {
        string matchID = System.Guid.NewGuid().ToString();
        string matchName = $"{queue.queueName} - {matchID.Substring(0, 8)}";
        string mapID = queue.matchType == MatchType.Arcade ? "map_howling_abyss" : "map_summoner_rift";
        
        Match match = new Match(matchID, matchName, queue.matchType, mapID);
        
        // 分配队伍
        List<PlayerMatchmaking> players = queue.GetPlayers();
        List<PlayerMatchmaking> team1Players = new List<PlayerMatchmaking>();
        List<PlayerMatchmaking> team2Players = new List<PlayerMatchmaking>();
        
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
        Team team1 = new Team("team1", "蓝色方");
        Team team2 = new Team("team2", "红色方");
        
        foreach (PlayerMatchmaking player in team1Players)
        {
            team1.AddPlayer(player);
        }
        
        foreach (PlayerMatchmaking player in team2Players)
        {
            team2.AddPlayer(player);
        }
        
        match.AddTeam(team1);
        match.AddTeam(team2);
        
        // 从队列中移除玩家
        foreach (PlayerMatchmaking player in players)
        {
            queue.RemovePlayer(player.playerID);
        }
        
        // 添加到活跃匹配
        matchmakingData.AddActiveMatch(match);
        
        // 开始匹配
        match.StartMatch();
        Debug.Log($"匹配成功！创建了 {match.matchName} 匹配");
        
        // 加载游戏场景
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        
        SaveMatchmakingData();
    }
    
    public void EndMatch(string matchID, string winningTeamID)
    {
        Match match = matchmakingData.GetActiveMatch(matchID);
        if (match != null)
        {
            // 设置获胜队伍
            foreach (Team team in match.teams)
            {
                team.SetWinner(team.teamID == winningTeamID);
            }
            
            // 结束匹配
            match.EndMatch();
            matchmakingData.AddCompletedMatch(match);
            
            // 更新玩家数据
            UpdatePlayerStats(match);
            
            Debug.Log($"匹配结束！{winningTeamID} 获胜");
            SaveMatchmakingData();
        }
    }
    
    private void UpdatePlayerStats(Match match)
    {
        foreach (Team team in match.teams)
        {
            foreach (PlayerMatchmaking player in team.players)
            {
                // 这里可以添加更新玩家统计数据的逻辑
                // 例如更新胜率、KDA、段位等
                if (team.isWinner)
                {
                    // 获胜玩家增加段位分
                    Debug.Log($"玩家 {player.playerName} 获胜！");
                }
                else
                {
                    // 失败玩家减少段位分
                    Debug.Log($"玩家 {player.playerName} 失败！");
                }
            }
        }
    }
    
    public List<MatchmakingQueue> GetAllQueues()
    {
        return matchmakingData.queues;
    }
    
    public List<Match> GetActiveMatches()
    {
        return matchmakingData.activeMatches;
    }
    
    public List<Match> GetCompletedMatches()
    {
        return matchmakingData.completedMatches;
    }
    
    public float GetAverageMatchmakingTime()
    {
        return matchmakingData.GetAverageMatchmakingTime();
    }
    
    public void SaveMatchmakingData()
    {
        string path = Application.dataPath + "/Data/matchmaking_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, matchmakingData);
        stream.Close();
    }
    
    public void LoadMatchmakingData()
    {
        string path = Application.dataPath + "/Data/matchmaking_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            matchmakingData = (MatchmakingManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            matchmakingData = new MatchmakingManagerData();
        }
    }
}
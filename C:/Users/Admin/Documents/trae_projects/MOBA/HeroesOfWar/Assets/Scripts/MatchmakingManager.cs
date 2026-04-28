using System.Collections.Generic;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    public static MatchmakingManager Instance { get; private set; }
    
    public enum MatchType
    {
        Normal,
        Ranked,
        Arcade,
        Custom
    }
    
    public enum MatchStatus
    {
        Idle,
        Waiting,
        Found,
        Ready,
        InGame
    }
    
    private List<Player> matchmakingPool = new List<Player>();
    private MatchStatus currentStatus = MatchStatus.Idle;
    private MatchType currentMatchType = MatchType.Normal;
    private float matchmakingTimer = 0f;
    private float matchmakingTimeout = 60f;
    
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
    
    private void Update()
    {
        if (currentStatus == MatchStatus.Waiting)
        {
            matchmakingTimer += Time.deltaTime;
            if (matchmakingTimer >= matchmakingTimeout)
            {
                CancelMatchmaking();
            }
            else
            {
                CheckMatchmaking();
            }
        }
    }
    
    public void StartMatchmaking(MatchType type)
    {
        currentMatchType = type;
        currentStatus = MatchStatus.Waiting;
        matchmakingTimer = 0f;
        
        // 添加当前玩家到匹配池
        Player localPlayer = new Player("LocalPlayer", 1000, 10);
        matchmakingPool.Add(localPlayer);
        
        Debug.Log($"开始匹配 {type} 模式...");
    }
    
    public void CancelMatchmaking()
    {
        currentStatus = MatchStatus.Idle;
        matchmakingTimer = 0f;
        matchmakingPool.Clear();
        Debug.Log("匹配已取消");
    }
    
    private void CheckMatchmaking()
    {
        // 模拟匹配过程
        if (matchmakingPool.Count >= 10) // 5v5
        {
            StartMatch();
        }
        else
        {
            // 模拟添加其他玩家
            if (Random.value < 0.1f)
            {
                Player botPlayer = new Player($"Bot{Random.Range(1, 1000)}", Random.Range(800, 1200), Random.Range(1, 20));
                matchmakingPool.Add(botPlayer);
                Debug.Log($"添加机器人玩家: {botPlayer.playerName}");
            }
        }
    }
    
    private void StartMatch()
    {
        currentStatus = MatchStatus.Found;
        Debug.Log($"匹配成功！找到 {matchmakingPool.Count} 名玩家");
        
        // 分配队伍
        List<Player> team1 = new List<Player>();
        List<Player> team2 = new List<Player>();
        
        for (int i = 0; i < matchmakingPool.Count; i++)
        {
            if (i % 2 == 0)
            {
                team1.Add(matchmakingPool[i]);
            }
            else
            {
                team2.Add(matchmakingPool[i]);
            }
        }
        
        // 开始游戏
        currentStatus = MatchStatus.InGame;
        Debug.Log("游戏开始！");
        
        // 加载游戏场景
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    
    public MatchStatus GetMatchStatus()
    {
        return currentStatus;
    }
    
    public MatchType GetCurrentMatchType()
    {
        return currentMatchType;
    }
    
    public float GetMatchmakingProgress()
    {
        return matchmakingTimer / matchmakingTimeout;
    }
}
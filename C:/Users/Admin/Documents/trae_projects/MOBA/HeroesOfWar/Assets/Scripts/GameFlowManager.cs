using UnityEngine;
using System.Collections;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }
    
    // 使用GameManager的游戏状态
    private void UpdateGameState(GameManager.GameState state)
    {
        GameManager.Instance.SetGameState(state);
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentState = GameState.MainMenu;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void StartGame()
    {
        Debug.Log("开始游戏");
        UpdateGameState(GameManager.GameState.HeroSelect);
        MainUIManager.Instance.ShowPanel("HeroSelect");
    }
    
    public void StartMatching(string selectedHeroID)
    {
        Debug.Log("开始匹配，选择的英雄: " + selectedHeroID);
        UpdateGameState(GameManager.GameState.Matchmaking);
        StartCoroutine(MatchingProcess());
    }
    
    private IEnumerator MatchingProcess()
    {
        // 显示匹配界面
        Debug.Log("正在匹配...");
        MainUIManager.Instance.ShowPanel("Matching");
        
        // 模拟匹配过程
        yield return new WaitForSeconds(3f);
        
        // 匹配成功，进入加载界面
        UpdateGameState(GameManager.GameState.LoadingGame);
        Debug.Log("匹配成功，开始加载游戏");
        
        // 模拟加载过程
        yield return new WaitForSeconds(2f);
        
        // 加载完成，进入战斗
        UpdateGameState(GameManager.GameState.InGame);
        
        // 开始战斗
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.StartBattle();
        }
        
        MainUIManager.Instance.ShowPanel("Battle");
        Debug.Log("进入战斗");
    }
    
    public void EndBattle(bool isWin, int kills, int deaths, int assists)
    {
        Debug.Log("战斗结束，结果: " + (isWin ? "胜利" : "失败"));
        
        // 更新职业生涯数据
        var careerManager = KingCareerSystemDetailedManager.Instance;
        careerManager.UpdateMatchResult(isWin, kills, deaths, assists, 10000, 5000, 2000, 8000, 20, 2, 1, 0, kills);
        
        // 更新经济系统
        var economyManager = EconomySystemDetailedManager.Instance;
        int goldEarned = isWin ? 500 : 200;
        economyManager.AddCurrency("player_001", "gold", goldEarned);
        
        // 更新任务进度
        var questManager = QuestSystemDetailedManager.Instance;
        questManager.UpdateQuestProgress("player_001", "daily", 1);
        
        // 更新排行榜
        var leaderboardManager = LeaderboardSystemDetailedManager.Instance;
        leaderboardManager.UpdatePlayerScore("player_001", "玩家1", "ranking", isWin ? 100 : 50, isWin ? 1 : 0, isWin ? 0 : 1, kills + assists - deaths);
        
        // 发送奖励邮件
        var mailManager = MailSystemDetailedManager.Instance;
        string reward = isWin ? "金币:500" : "金币:200";
        mailManager.SendRewardMail("player_001", "战斗奖励", "恭喜完成一场战斗！", new string[] { reward });
        
        // 显示结算界面
        UpdateGameState(GameManager.GameState.Results);
        MainUIManager.Instance.ShowPanel("Results");
        
        // 设置结算面板数据
        var resultsPanel = MainUIManager.Instance.GetPanel("Results") as ResultsPanel;
        if (resultsPanel != null)
        {
            resultsPanel.SetResult(isWin, kills, deaths, assists, goldEarned);
        }
        
        Debug.Log("显示结算界面");
    }
    
    public void ReturnToMainMenu()
    {
        UpdateGameState(GameManager.GameState.MainMenu);
        MainUIManager.Instance.ShowPanel("MainMenu");
        Debug.Log("返回主菜单");
    }
    

}

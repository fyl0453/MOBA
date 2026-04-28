using System.Collections.Generic;
using UnityEngine;

public class MatchHistoryManager : MonoBehaviour
{
    public static MatchHistoryManager Instance { get; private set; }
    
    private List<MatchHistory> matchHistory = new List<MatchHistory>();
    private int matchCounter = 0;
    
    private int totalKills = 0;
    private int totalDeaths = 0;
    private int totalAssists = 0;
    private int totalWins = 0;
    private int totalLosses = 0;
    private int totalGoldEarned = 0;
    
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
    
    public void RecordMatch(bool isVictory, string gameMode, int durationSeconds, string playerName, string heroName, int kills, int deaths, int assists, int gold, int damageD, int damageT)
    {
        matchCounter++;
        string matchId = $"Match_{matchCounter}_{System.DateTime.Now:yyyyMMdd}";
        
        MatchHistory match = new MatchHistory(matchId, isVictory, gameMode, durationSeconds);
        match.SetPlayerStats(playerName, heroName, kills, deaths, assists, gold, damageD, damageT);
        
        matchHistory.Insert(0, match);
        
        // 更新统计数据
        totalKills += kills;
        totalDeaths += deaths;
        totalAssists += assists;
        totalGoldEarned += gold;
        
        if (isVictory)
        {
            totalWins++;
        }
        else
        {
            totalLosses++;
        }
        
        Debug.Log($"比赛记录已保存: {matchId}");
    }
    
    public List<MatchHistory> GetMatchHistory()
    {
        return matchHistory;
    }
    
    public List<MatchHistory> GetRecentMatches(int count)
    {
        if (matchHistory.Count <= count)
        {
            return matchHistory;
        }
        return matchHistory.GetRange(0, count);
    }
    
    public int GetTotalKills()
    {
        return totalKills;
    }
    
    public int GetTotalDeaths()
    {
        return totalDeaths;
    }
    
    public int GetTotalAssists()
    {
        return totalAssists;
    }
    
    public int GetTotalWins()
    {
        return totalWins;
    }
    
    public int GetTotalLosses()
    {
        return totalLosses;
    }
    
    public int GetTotalGames()
    {
        return totalWins + totalLosses;
    }
    
    public float GetWinRate()
    {
        int totalGames = GetTotalGames();
        if (totalGames == 0) return 0f;
        return (float)totalWins / totalGames * 100f;
    }
    
    public float GetOverallKDA()
    {
        if (totalDeaths == 0) return (totalKills + totalAssists) * 1.0f;
        return (totalKills + totalAssists) * 1.0f / totalDeaths;
    }
    
    public int GetTotalGoldEarned()
    {
        return totalGoldEarned;
    }
    
    public MatchHistory GetLastMatch()
    {
        if (matchHistory.Count > 0)
        {
            return matchHistory[0];
        }
        return null;
    }
}
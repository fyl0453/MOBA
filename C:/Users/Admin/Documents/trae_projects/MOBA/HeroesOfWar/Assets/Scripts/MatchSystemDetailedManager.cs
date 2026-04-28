using System;
using System.Collections.Generic;

public class MatchSystemDetailedManager
{
    private static MatchSystemDetailedManager _instance;
    public static MatchSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MatchSystemDetailedManager();
            }
            return _instance;
        }
    }

    private MatchSystemData matchData;
    private MatchSystemDataManager dataManager;

    private MatchSystemDetailedManager()
    {
        dataManager = MatchSystemDataManager.Instance;
        matchData = dataManager.matchData;
    }

    public string EnterMatchQueue(string playerID, string playerName, int matchType, int playerLevel, int rankLevel)
    {
        string queueID = "queue_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchQueueEntry entry = new MatchQueueEntry(queueID, playerID, playerName, matchType, playerLevel, rankLevel);
        matchData.AddQueueEntry(entry);
        dataManager.CreateMatchEvent("queue_enter", playerID, "", "进入匹配队列: " + matchType);
        dataManager.SaveMatchData();
        Debug.Log("进入匹配队列成功: " + playerName);
        return queueID;
    }

    public string EnterTeamMatchQueue(string teamID, List<string> teamMembers, int matchType, int averageLevel, int averageRankLevel)
    {
        string queueID = "queue_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchQueueEntry entry = new MatchQueueEntry(queueID, teamID, teamMembers, matchType, averageLevel, averageRankLevel);
        matchData.AddQueueEntry(entry);
        dataManager.CreateMatchEvent("team_queue_enter", teamMembers[0], "", "队伍进入匹配队列: " + matchType);
        dataManager.SaveMatchData();
        Debug.Log("队伍进入匹配队列成功: " + teamID);
        return queueID;
    }

    public void LeaveMatchQueue(string queueID)
    {
        MatchQueueEntry entry = matchData.MatchQueue.Find(e => e.QueueID == queueID);
        if (entry != null)
        {
            matchData.RemoveQueueEntry(queueID);
            dataManager.CreateMatchEvent("queue_leave", entry.PlayerID, "", "离开匹配队列");
            dataManager.SaveMatchData();
            Debug.Log("离开匹配队列成功: " + entry.PlayerID);
        }
    }

    public List<MatchQueueEntry> GetMatchQueue()
    {
        return matchData.MatchQueue;
    }

    public List<MatchQueueEntry> GetQueueByMatchType(int matchType)
    {
        return matchData.MatchQueue.FindAll(e => e.MatchType == matchType);
    }

    public MatchQueueEntry GetQueueEntry(string queueID)
    {
        return matchData.MatchQueue.Find(e => e.QueueID == queueID);
    }

    public string StartMatchmaking(int matchType)
    {
        List<MatchQueueEntry> queueEntries = GetQueueByMatchType(matchType);
        if (queueEntries.Count < 10)
        {
            Debug.Log("匹配队列人数不足: " + queueEntries.Count);
            return "";
        }

        List<string> blueTeam = new List<string>();
        List<string> redTeam = new List<string>();

        for (int i = 0; i < 5; i++)
        {
            blueTeam.Add(queueEntries[i].PlayerID);
        }
        for (int i = 5; i < 10; i++)
        {
            redTeam.Add(queueEntries[i].PlayerID);
        }

        string gameID = "game_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchGame game = new MatchGame(gameID, GetMatchTypeName(matchType), blueTeam, redTeam);
        matchData.AddActiveGame(game);

        foreach (MatchQueueEntry entry in queueEntries.GetRange(0, 10))
        {
            matchData.RemoveQueueEntry(entry.QueueID);
            dataManager.CreateMatchEvent("match_found", entry.PlayerID, gameID, "匹配成功");
        }

        dataManager.SaveMatchData();
        Debug.Log("开始匹配成功: " + gameID);
        return gameID;
    }

    public string StartAIMatch(string playerID, string playerName, int difficulty = 1)
    {
        List<string> blueTeam = new List<string>();
        blueTeam.Add(playerID);

        List<string> redTeam = new List<string>();
        for (int i = 0; i < 5; i++)
        {
            redTeam.Add("ai_" + i);
        }

        string gameID = "game_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchGame game = new MatchGame(gameID, "人机对战", blueTeam, redTeam);
        matchData.AddActiveGame(game);

        dataManager.CreateMatchEvent("ai_match_start", playerID, gameID, "开始人机对战");
        dataManager.SaveMatchData();
        Debug.Log("开始人机对战成功: " + gameID);
        return gameID;
    }

    public void EndMatch(string gameID, string winnerTeam, Dictionary<string, PlayerMatchStats> playerStats)
    {
        MatchGame game = matchData.ActiveGames.Find(g => g.GameID == gameID);
        if (game != null)
        {
            game.WinnerTeam = winnerTeam;
            game.EndTime = DateTime.Now;
            game.Duration = (int)(game.EndTime - game.StartTime).TotalMinutes;
            game.IsCompleted = true;

            foreach (KeyValuePair<string, PlayerMatchStats> stat in playerStats)
            {
                game.PlayerStats.Add(stat.Value);
                AddMatchHistory(stat.Value.PlayerID, gameID, game.MatchType, stat.Value.IsWinner, stat.Value.Kills, stat.Value.Deaths, stat.Value.Assists, stat.Value.HeroUsed, game.Duration, "");
            }

            matchData.RemoveActiveGame(gameID);
            dataManager.CreateMatchEvent("match_end", "", gameID, "比赛结束，获胜方: " + winnerTeam);
            dataManager.SaveMatchData();
            Debug.Log("结束比赛成功: " + gameID);
        }
    }

    private void AddMatchHistory(string playerID, string gameID, string matchType, bool isWinner, int kills, int deaths, int assists, string heroUsed, int duration, string opponentName)
    {
        string historyID = "history_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchHistory history = new MatchHistory(historyID, playerID, gameID, matchType, isWinner, kills, deaths, assists, heroUsed, duration, opponentName);
        matchData.AddMatchHistory(history);
    }

    public List<MatchGame> GetActiveGames()
    {
        return matchData.ActiveGames;
    }

    public MatchGame GetActiveGame(string gameID)
    {
        return matchData.ActiveGames.Find(g => g.GameID == gameID);
    }

    public List<MatchHistory> GetPlayerMatchHistory(string playerID, int count = 50)
    {
        if (matchData.PlayerMatchHistory.ContainsKey(playerID))
        {
            List<MatchHistory> history = matchData.PlayerMatchHistory[playerID];
            history.Sort((a, b) => b.MatchTime.CompareTo(a.MatchTime));
            if (count < history.Count)
            {
                return history.GetRange(0, count);
            }
            return history;
        }
        return new List<MatchHistory>();
    }

    public List<MatchHistory> GetRecentMatches(int count = 50)
    {
        List<MatchHistory> history = new List<MatchHistory>(matchData.MatchHistories);
        history.Sort((a, b) => b.MatchTime.CompareTo(a.MatchTime));
        if (count < history.Count)
        {
            return history.GetRange(0, count);
        }
        return history;
    }

    public MatchConfig GetMatchConfig(int matchType)
    {
        return matchData.MatchConfigs.Find(c => c.ConfigID == "config_" + GetMatchTypeKey(matchType));
    }

    public void UpdateMatchConfig(int matchType, int minPlayers, int maxPlayers, int teamSize, bool allowTeams, bool isRanked, int minLevel, int matchmakingRange, int maxWaitTime)
    {
        MatchConfig config = GetMatchConfig(matchType);
        if (config != null)
        {
            config.MinPlayers = minPlayers;
            config.MaxPlayers = maxPlayers;
            config.TeamSize = teamSize;
            config.AllowTeams = allowTeams;
            config.IsRanked = isRanked;
            config.MinLevel = minLevel;
            config.MatchmakingRange = matchmakingRange;
            config.MaxWaitTime = maxWaitTime;
            dataManager.SaveMatchData();
            Debug.Log("更新匹配配置成功: " + config.MatchTypeName);
        }
    }

    public void CleanupExpiredGames()
    {
        DateTime now = DateTime.Now;
        List<MatchGame> expiredGames = new List<MatchGame>();
        foreach (MatchGame game in matchData.ActiveGames)
        {
            if ((now - game.StartTime).TotalMinutes > 60)
            {
                expiredGames.Add(game);
            }
        }
        
        foreach (MatchGame game in expiredGames)
        {
            matchData.RemoveActiveGame(game.GameID);
        }
        
        if (expiredGames.Count > 0)
        {
            dataManager.SaveMatchData();
            Debug.Log("清理过期比赛成功: " + expiredGames.Count);
        }
    }

    public void CleanupOldMatchHistory(int daysToKeep = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-daysToKeep);
        List<MatchHistory> oldHistory = new List<MatchHistory>();
        foreach (MatchHistory history in matchData.MatchHistories)
        {
            if (history.MatchTime < cutoffDate)
            {
                oldHistory.Add(history);
            }
        }
        
        foreach (MatchHistory history in oldHistory)
        {
            matchData.MatchHistories.Remove(history);
            if (matchData.PlayerMatchHistory.ContainsKey(history.PlayerID))
            {
                matchData.PlayerMatchHistory[history.PlayerID].Remove(history);
            }
        }
        
        if (oldHistory.Count > 0)
        {
            dataManager.SaveMatchData();
            Debug.Log("清理旧匹配历史成功: " + oldHistory.Count);
        }
    }

    private string GetMatchTypeKey(int matchType)
    {
        switch (matchType)
        {
            case 1: return "normal";
            case 2: return "ranked";
            case 3: return "ai";
            case 4: return "custom";
            default: return "normal";
        }
    }

    private string GetMatchTypeName(int matchType)
    {
        switch (matchType)
        {
            case 1: return "普通匹配";
            case 2: return "排位赛";
            case 3: return "人机对战";
            case 4: return "自定义";
            default: return "普通匹配";
        }
    }

    public void SaveData()
    {
        dataManager.SaveMatchData();
    }

    public void LoadData()
    {
        dataManager.LoadMatchData();
    }

    public List<MatchEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
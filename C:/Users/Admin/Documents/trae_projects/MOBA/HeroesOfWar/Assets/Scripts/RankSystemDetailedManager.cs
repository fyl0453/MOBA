using System;
using System.Collections.Generic;

public class RankSystemDetailedManager
{
    private static RankSystemDetailedManager _instance;
    public static RankSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RankSystemDetailedManager();
            }
            return _instance;
        }
    }

    private RankSystemData rankData;
    private RankSystemDataManager dataManager;

    private RankSystemDetailedManager()
    {
        dataManager = RankSystemDataManager.Instance;
        rankData = dataManager.rankData;
    }

    public void InitializePlayerRankData(string playerID)
    {
        if (!rankData.PlayerRankData.ContainsKey(playerID))
        {
            rankData.AddPlayerRankData(playerID, new PlayerRankData(playerID));
            dataManager.CreateRankEvent("rank_init", playerID, "初始化排位数据");
            dataManager.SaveRankData();
            Debug.Log("初始化玩家排位数据: " + playerID);
        }
    }

    public void RecordRankMatch(string playerID, bool isWin, int opponentLevel)
    {
        if (!rankData.PlayerRankData.ContainsKey(playerID))
        {
            InitializePlayerRankData(playerID);
        }

        PlayerRankData rankData = this.rankData.PlayerRankData[playerID];
        int levelBefore = rankData.CurrentLevel;
        int starsBefore = rankData.CurrentStars;

        if (isWin)
        {
            HandleWin(playerID, rankData);
        }
        else
        {
            HandleLose(playerID, rankData);
        }

        int levelAfter = rankData.CurrentLevel;
        int starsAfter = rankData.CurrentStars;
        int scoreChange = isWin ? 10 : -5;

        string matchID = "rank_match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RankMatch match = new RankMatch(matchID, playerID, 1, isWin, levelBefore, starsBefore, levelAfter, starsAfter, scoreChange);
        match.OpponentLevel = opponentLevel;
        this.rankData.AddRankMatch(match);

        if (!this.rankData.PlayerMatchHistory.ContainsKey(playerID))
        {
            this.rankData.PlayerMatchHistory[playerID] = new List<string>();
        }
        this.rankData.PlayerMatchHistory[playerID].Add(matchID);

        dataManager.CreateRankEvent("rank_match", playerID, "排位赛: " + (isWin ? "胜利" : "失败"));
        dataManager.SaveRankData();
        Debug.Log("记录排位赛: " + (isWin ? "胜利" : "失败"));
    }

    private void HandleWin(string playerID, PlayerRankData rankData)
    {
        rankData.TotalMatches++;
        rankData.WinMatches++;
        rankData.SeasonWins++;
        rankData.CurrentWinStreak++;
        
        if (rankData.CurrentWinStreak > rankData.MaxWinStreak)
        {
            rankData.MaxWinStreak = rankData.CurrentWinStreak;
        }

        if (rankData.IsInPromotionMatch)
        {
            rankData.PromotionMatchWins++;
            if (rankData.PromotionMatchWins >= 2)
            {
                rankData.IsInPromotionMatch = false;
                rankData.PromotionMatchWins = 0;
                rankData.PromotionMatchLoses = 0;
                rankData.CurrentLevel++;
                rankData.CurrentStars = 0;
                dataManager.CreateRankEvent("rank_promote", playerID, "晋级成功: " + rankData.CurrentLevel);
            }
        }
        else
        {
            rankData.CurrentStars++;
            RankLevel currentLevel = GetRankLevel(rankData.CurrentLevel);
            if (currentLevel != null && rankData.CurrentStars > currentLevel.MaxStars)
            {
                if (currentLevel.IsPromotionMatch)
                {
                    rankData.IsInPromotionMatch = true;
                    rankData.CurrentStars = 0;
                    dataManager.CreateRankEvent("rank_promotion_match", playerID, "进入晋级赛");
                }
                else
                {
                    rankData.CurrentLevel++;
                    rankData.CurrentStars = 1;
                    if (rankData.CurrentLevel > rankData.HighestLevel)
                    {
                        rankData.HighestLevel = rankData.CurrentLevel;
                    }
                    dataManager.CreateRankEvent("rank_level_up", playerID, "段位提升: " + rankData.CurrentLevel);
                }
            }
        }

        rankData.CurrentScore += 10;
        rankData.LastMatchTime = DateTime.Now;
        rankData.UpdateStats();
    }

    private void HandleLose(string playerID, PlayerRankData rankData)
    {
        rankData.TotalMatches++;
        rankData.LoseMatches++;
        rankData.SeasonLoses++;
        rankData.CurrentWinStreak = 0;

        if (rankData.IsInPromotionMatch)
        {
            rankData.PromotionMatchLoses++;
            if (rankData.PromotionMatchLoses >= 2)
            {
                rankData.IsInPromotionMatch = false;
                rankData.PromotionMatchWins = 0;
                rankData.PromotionMatchLoses = 0;
                dataManager.CreateRankEvent("rank_promotion_fail", playerID, "晋级失败");
            }
        }
        else
        {
            rankData.CurrentStars--;
            if (rankData.CurrentStars < 0)
            {
                if (rankData.CurrentLevel > 1)
                {
                    rankData.CurrentLevel--;
                    RankLevel previousLevel = GetRankLevel(rankData.CurrentLevel);
                    if (previousLevel != null)
                    {
                        rankData.CurrentStars = previousLevel.MaxStars;
                    }
                    dataManager.CreateRankEvent("rank_level_down", playerID, "段位下降: " + rankData.CurrentLevel);
                }
                else
                {
                    rankData.CurrentStars = 0;
                }
            }
        }

        rankData.CurrentScore = Math.Max(0, rankData.CurrentScore - 5);
        rankData.LastMatchTime = DateTime.Now;
        rankData.UpdateStats();
    }

    public RankLevel GetRankLevel(int levelID)
    {
        return rankData.RankLevels.Find(l => l.LevelID == levelID);
    }

    public PlayerRankData GetPlayerRankData(string playerID)
    {
        if (rankData.PlayerRankData.ContainsKey(playerID))
        {
            return rankData.PlayerRankData[playerID];
        }
        return null;
    }

    public List<RankMatch> GetPlayerMatchHistory(string playerID, int count = 20)
    {
        List<RankMatch> matches = new List<RankMatch>();
        if (rankData.PlayerMatchHistory.ContainsKey(playerID))
        {
            foreach (string matchID in rankData.PlayerMatchHistory[playerID])
            {
                RankMatch match = rankData.MatchHistory.Find(m => m.MatchID == matchID);
                if (match != null)
                {
                    matches.Add(match);
                }
            }
            matches.Sort((a, b) => b.MatchTime.CompareTo(a.MatchTime));
            if (count < matches.Count)
            {
                return matches.GetRange(0, count);
            }
        }
        return matches;
    }

    public List<RankLevel> GetAllRankLevels()
    {
        return rankData.RankLevels;
    }

    public string GetRankName(int level)
    {
        RankLevel rankLevel = GetRankLevel(level);
        return rankLevel != null ? rankLevel.LevelName : "未知段位";
    }

    public string GetRankColor(int level)
    {
        RankLevel rankLevel = GetRankLevel(level);
        return rankLevel != null ? rankLevel.LevelColor : "#888888";
    }

    public int GetCurrentSeason()
    {
        return rankData.CurrentSeason;
    }

    public Season GetCurrentSeasonData()
    {
        return rankData.Seasons.Find(s => s.IsCurrent);
    }

    public void StartNewSeason(int seasonNumber, string seasonName, DateTime endTime)
    {
        string seasonID = "season_" + seasonNumber.ToString("000");
        Season newSeason = new Season(seasonID, seasonName, seasonNumber, DateTime.Now, endTime);
        newSeason.IsCurrent = true;
        
        foreach (Season season in rankData.Seasons)
        {
            season.IsCurrent = false;
        }
        
        rankData.AddSeason(newSeason);
        rankData.CurrentSeason = seasonNumber;
        
        foreach (var kvp in rankData.PlayerRankData)
        {
            PlayerRankData playerData = kvp.Value;
            playerData.CurrentLevel = 1;
            playerData.CurrentStars = 0;
            playerData.SeasonWins = 0;
            playerData.SeasonLoses = 0;
            playerData.IsInPromotionMatch = false;
            playerData.PromotionMatchWins = 0;
            playerData.PromotionMatchLoses = 0;
        }
        
        dataManager.CreateRankEvent("season_start", "", "新赛季开始: " + seasonName);
        dataManager.SaveRankData();
        Debug.Log("新赛季开始: " + seasonName);
    }

    public void EndSeason()
    {
        Season currentSeason = GetCurrentSeasonData();
        if (currentSeason != null)
        {
            currentSeason.IsCurrent = false;
            currentSeason.EndTime = DateTime.Now;
            
            List<PlayerRankData> sortedPlayers = new List<PlayerRankData>(rankData.PlayerRankData.Values);
            sortedPlayers.Sort((a, b) => b.CurrentScore.CompareTo(a.CurrentScore));
            
            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                sortedPlayers[i].SeasonRank = i + 1;
            }
            
            dataManager.CreateRankEvent("season_end", "", "赛季结束: " + currentSeason.SeasonName);
            dataManager.SaveRankData();
            Debug.Log("赛季结束: " + currentSeason.SeasonName);
        }
    }

    public List<PlayerRankData> GetTopPlayers(int count = 10)
    {
        List<PlayerRankData> players = new List<PlayerRankData>(rankData.PlayerRankData.Values);
        players.Sort((a, b) => b.CurrentScore.CompareTo(a.CurrentScore));
        if (count < players.Count)
        {
            return players.GetRange(0, count);
        }
        return players;
    }

    public double GetPlayerWinRate(string playerID)
    {
        PlayerRankData data = GetPlayerRankData(playerID);
        if (data != null)
        {
            return data.WinRate;
        }
        return 0.0;
    }

    public int GetPlayerHighestLevel(string playerID)
    {
        PlayerRankData data = GetPlayerRankData(playerID);
        if (data != null)
        {
            return data.HighestLevel;
        }
        return 1;
    }

    public int GetPlayerSeasonWins(string playerID)
    {
        PlayerRankData data = GetPlayerRankData(playerID);
        if (data != null)
        {
            return data.SeasonWins;
        }
        return 0;
    }

    public bool IsInPromotionMatch(string playerID)
    {
        PlayerRankData data = GetPlayerRankData(playerID);
        if (data != null)
        {
            return data.IsInPromotionMatch;
        }
        return false;
    }

    public int GetPromotionMatchProgress(string playerID)
    {
        PlayerRankData data = GetPlayerRankData(playerID);
        if (data != null && data.IsInPromotionMatch)
        {
            return data.PromotionMatchWins;
        }
        return 0;
    }

    public void ResetPlayerRank(string playerID)
    {
        if (rankData.PlayerRankData.ContainsKey(playerID))
        {
            rankData.PlayerRankData[playerID] = new PlayerRankData(playerID);
            dataManager.SaveRankData();
            Debug.Log("重置玩家排位数据: " + playerID);
        }
    }

    public List<RankEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveRankData();
    }

    public void LoadData()
    {
        dataManager.LoadRankData();
    }
}
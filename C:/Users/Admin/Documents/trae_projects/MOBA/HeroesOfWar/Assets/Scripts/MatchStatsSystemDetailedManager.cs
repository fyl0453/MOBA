using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchStatsSystemDetailedManager
{
    private static MatchStatsSystemDetailedManager _instance;
    public static MatchStatsSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MatchStatsSystemDetailedManager();
            }
            return _instance;
        }
    }

    private MatchStatsSystemData statsData;
    private MatchStatsSystemDataManager dataManager;

    private MatchStatsSystemDetailedManager()
    {
        dataManager = MatchStatsSystemDataManager.Instance;
        statsData = dataManager.statsData;
    }

    public void InitializePlayerMatchHistory(string playerID)
    {
        if (!statsData.PlayerMatchHistories.ContainsKey(playerID))
        {
            PlayerMatchHistory playerMatchHistory = new PlayerMatchHistory(playerID);
            statsData.AddPlayerMatchHistory(playerID, playerMatchHistory);
            dataManager.SaveStatsData();
            Debug.Log("初始化对局历史数据成功");
        }
    }

    public string CreateMatchStats(string matchType, string mapName, bool isRanked, string seasonID)
    {
        string matchID = "match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchStats matchStats = new MatchStats(matchID, matchType, mapName, isRanked, seasonID);
        statsData.AddMatchStats(matchID, matchStats);
        
        dataManager.CreateStatsEvent("match_create", "system", "", matchID, "创建比赛: " + matchType);
        dataManager.SaveStatsData();
        Debug.Log("创建比赛统计数据成功: " + matchID);
        return matchID;
    }

    public void AddPlayerMatchStats(string matchID, string playerID, string playerName, string heroID, string heroName)
    {
        if (!statsData.AllMatches.ContainsKey(matchID))
        {
            Debug.LogError("比赛不存在: " + matchID);
            return;
        }
        
        MatchStats matchStats = statsData.AllMatches[matchID];
        PlayerMatchStats playerStats = new PlayerMatchStats(playerID, playerName, heroID, heroName);
        matchStats.PlayerStats.Add(playerStats);
        
        dataManager.CreateStatsEvent("player_stats_add", playerID, matchID, "添加玩家统计数据: " + playerName);
        dataManager.SaveStatsData();
        Debug.Log("添加玩家统计数据成功: " + playerName);
    }

    public void UpdatePlayerMatchStats(string matchID, string playerID, int kills, int deaths, int assists, int damageDealt, int damageTaken, int healingDone, int goldEarned, int minionsKilled, int turretsDestroyed, int dragonsKilled, int baronsKilled, float visionScore, float timeSpentDead, float timePlayed, string items, bool isVictory, int rankPointsChange, string matchResult)
    {
        if (!statsData.AllMatches.ContainsKey(matchID))
        {
            return;
        }
        
        MatchStats matchStats = statsData.AllMatches[matchID];
        PlayerMatchStats playerStats = matchStats.PlayerStats.Find(p => p.PlayerID == playerID);
        if (playerStats != null)
        {
            playerStats.Kills = kills;
            playerStats.Deaths = deaths;
            playerStats.Assists = assists;
            playerStats.DamageDealt = damageDealt;
            playerStats.DamageTaken = damageTaken;
            playerStats.HealingDone = healingDone;
            playerStats.GoldEarned = goldEarned;
            playerStats.MinionsKilled = minionsKilled;
            playerStats.TurretsDestroyed = turretsDestroyed;
            playerStats.DragonsKilled = dragonsKilled;
            playerStats.BaronsKilled = baronsKilled;
            playerStats.VisionScore = visionScore;
            playerStats.TimeSpentDead = timeSpentDead;
            playerStats.TimePlayed = timePlayed;
            playerStats.Items = items;
            playerStats.IsVictory = isVictory;
            playerStats.RankPointsChange = rankPointsChange;
            playerStats.MatchResult = matchResult;
            
            dataManager.SaveStatsData();
            Debug.Log("更新玩家统计数据成功: " + playerStats.PlayerName);
        }
    }

    public void EndMatch(string matchID, string winningTeam, string losingTeam, float matchDuration)
    {
        if (!statsData.AllMatches.ContainsKey(matchID))
        {
            return;
        }
        
        MatchStats matchStats = statsData.AllMatches[matchID];
        matchStats.MatchEndTime = DateTime.Now;
        matchStats.MatchDuration = matchDuration;
        matchStats.WinningTeam = winningTeam;
        matchStats.LosingTeam = losingTeam;
        
        foreach (PlayerMatchStats playerStats in matchStats.PlayerStats)
        {
            UpdatePlayerMatchHistory(playerStats.PlayerID, matchStats);
            UpdateHeroStats(playerStats.PlayerID, playerStats);
            UpdatePlayerStatsSummary(playerStats.PlayerID);
        }
        
        dataManager.CreateStatsEvent("match_end", "system", "", matchID, "结束比赛: " + matchDuration + "秒");
        dataManager.SaveStatsData();
        Debug.Log("结束比赛成功: " + matchID);
    }

    private void UpdatePlayerMatchHistory(string playerID, MatchStats matchStats)
    {
        if (!statsData.PlayerMatchHistories.ContainsKey(playerID))
        {
            InitializePlayerMatchHistory(playerID);
        }
        
        PlayerMatchHistory playerMatchHistory = statsData.PlayerMatchHistories[playerID];
        playerMatchHistory.MatchHistory.Insert(0, matchStats);
        
        if (playerMatchHistory.MatchHistory.Count > playerMatchHistory.MaxMatchHistory)
        {
            playerMatchHistory.MatchHistory.RemoveAt(playerMatchHistory.MatchHistory.Count - 1);
        }
    }

    private void UpdateHeroStats(string playerID, PlayerMatchStats playerStats)
    {
        if (!statsData.PlayerMatchHistories.ContainsKey(playerID))
        {
            InitializePlayerMatchHistory(playerID);
        }
        
        PlayerMatchHistory playerMatchHistory = statsData.PlayerMatchHistories[playerID];
        if (!playerMatchHistory.HeroStats.ContainsKey(playerStats.HeroID))
        {
            playerMatchHistory.HeroStats[playerStats.HeroID] = new HeroStats(playerStats.HeroID, playerStats.HeroName);
        }
        
        HeroStats heroStats = playerMatchHistory.HeroStats[playerStats.HeroID];
        heroStats.MatchesPlayed++;
        if (playerStats.IsVictory)
        {
            heroStats.Wins++;
        }
        else
        {
            heroStats.Losses++;
        }
        heroStats.WinRate = (float)heroStats.Wins / heroStats.MatchesPlayed;
        heroStats.TotalKills += playerStats.Kills;
        heroStats.TotalDeaths += playerStats.Deaths;
        heroStats.TotalAssists += playerStats.Assists;
        heroStats.KDA = heroStats.TotalDeaths > 0 ? (float)(heroStats.TotalKills + heroStats.TotalAssists) / heroStats.TotalDeaths : (float)(heroStats.TotalKills + heroStats.TotalAssists);
        heroStats.TotalDamageDealt += playerStats.DamageDealt;
        heroStats.TotalDamageTaken += playerStats.DamageTaken;
        heroStats.TotalHealingDone += playerStats.HealingDone;
        heroStats.TotalGoldEarned += playerStats.GoldEarned;
        heroStats.AverageVisionScore = (heroStats.AverageVisionScore * (heroStats.MatchesPlayed - 1) + playerStats.VisionScore) / heroStats.MatchesPlayed;
        heroStats.AverageTimePlayed = (heroStats.AverageTimePlayed * (heroStats.MatchesPlayed - 1) + playerStats.TimePlayed) / heroStats.MatchesPlayed;
        heroStats.LastPlayed = DateTime.Now;
    }

    private void UpdatePlayerStatsSummary(string playerID)
    {
        if (!statsData.PlayerMatchHistories.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerMatchHistory playerMatchHistory = statsData.PlayerMatchHistories[playerID];
        PlayerStatsSummary summary = playerMatchHistory.StatsSummary;
        
        int totalMatches = 0;
        int totalWins = 0;
        int totalKills = 0;
        int totalDeaths = 0;
        int totalAssists = 0;
        int totalDamageDealt = 0;
        int totalDamageTaken = 0;
        int totalHealingDone = 0;
        int totalGoldEarned = 0;
        int totalMinionsKilled = 0;
        int totalTurretsDestroyed = 0;
        float totalVisionScore = 0f;
        float totalTimePlayed = 0f;
        
        foreach (MatchStats matchStats in playerMatchHistory.MatchHistory)
        {
            PlayerMatchStats playerStats = matchStats.PlayerStats.Find(p => p.PlayerID == playerID);
            if (playerStats != null)
            {
                totalMatches++;
                if (playerStats.IsVictory)
                {
                    totalWins++;
                }
                totalKills += playerStats.Kills;
                totalDeaths += playerStats.Deaths;
                totalAssists += playerStats.Assists;
                totalDamageDealt += playerStats.DamageDealt;
                totalDamageTaken += playerStats.DamageTaken;
                totalHealingDone += playerStats.HealingDone;
                totalGoldEarned += playerStats.GoldEarned;
                totalMinionsKilled += playerStats.MinionsKilled;
                totalTurretsDestroyed += playerStats.TurretsDestroyed;
                totalVisionScore += playerStats.VisionScore;
                totalTimePlayed += playerStats.TimePlayed;
            }
        }
        
        summary.TotalMatches = totalMatches;
        summary.TotalWins = totalWins;
        summary.TotalLosses = totalMatches - totalWins;
        summary.WinRate = totalMatches > 0 ? (float)totalWins / totalMatches : 0f;
        summary.TotalKills = totalKills;
        summary.TotalDeaths = totalDeaths;
        summary.TotalAssists = totalAssists;
        summary.KDA = totalDeaths > 0 ? (float)(totalKills + totalAssists) / totalDeaths : (float)(totalKills + totalAssists);
        summary.TotalDamageDealt = totalDamageDealt;
        summary.TotalDamageTaken = totalDamageTaken;
        summary.TotalHealingDone = totalHealingDone;
        summary.TotalGoldEarned = totalGoldEarned;
        summary.TotalMinionsKilled = totalMinionsKilled;
        summary.TotalTurretsDestroyed = totalTurretsDestroyed;
        summary.AverageVisionScore = totalMatches > 0 ? totalVisionScore / totalMatches : 0f;
        summary.AverageTimePlayed = totalMatches > 0 ? totalTimePlayed / totalMatches : 0f;
        summary.LastMatchTime = DateTime.Now;
        
        summary.BestHero = GetBestHero(playerID);
        summary.MostPlayedHero = GetMostPlayedHero(playerID);
    }

    private string GetBestHero(string playerID)
    {
        if (!statsData.PlayerMatchHistories.ContainsKey(playerID))
        {
            return "";
        }
        
        PlayerMatchHistory playerMatchHistory = statsData.PlayerMatchHistories[playerID];
        HeroStats bestHero = null;
        float bestWinRate = 0f;
        int minMatches = 5;
        
        foreach (HeroStats heroStats in playerMatchHistory.HeroStats.Values)
        {
            if (heroStats.MatchesPlayed >= minMatches && heroStats.WinRate > bestWinRate)
            {
                bestWinRate = heroStats.WinRate;
                bestHero = heroStats;
            }
        }
        
        return bestHero != null ? bestHero.HeroName : "";
    }

    private string GetMostPlayedHero(string playerID)
    {
        if (!statsData.PlayerMatchHistories.ContainsKey(playerID))
        {
            return "";
        }
        
        PlayerMatchHistory playerMatchHistory = statsData.PlayerMatchHistories[playerID];
        HeroStats mostPlayedHero = null;
        int maxMatches = 0;
        
        foreach (HeroStats heroStats in playerMatchHistory.HeroStats.Values)
        {
            if (heroStats.MatchesPlayed > maxMatches)
            {
                maxMatches = heroStats.MatchesPlayed;
                mostPlayedHero = heroStats;
            }
        }
        
        return mostPlayedHero != null ? mostPlayedHero.HeroName : "";
    }

    public MatchStats GetMatchStats(string matchID)
    {
        if (statsData.AllMatches.ContainsKey(matchID))
        {
            return statsData.AllMatches[matchID];
        }
        return null;
    }

    public List<MatchStats> GetPlayerMatchHistory(string playerID, int count = 20)
    {
        if (!statsData.PlayerMatchHistories.ContainsKey(playerID))
        {
            InitializePlayerMatchHistory(playerID);
        }
        
        List<MatchStats> matchHistory = statsData.PlayerMatchHistories[playerID].MatchHistory;
        if (count < matchHistory.Count)
        {
            matchHistory = matchHistory.GetRange(0, count);
        }
        return matchHistory;
    }

    public PlayerStatsSummary GetPlayerStatsSummary(string playerID)
    {
        if (!statsData.PlayerMatchHistories.ContainsKey(playerID))
        {
            InitializePlayerMatchHistory(playerID);
        }
        return statsData.PlayerMatchHistories[playerID].StatsSummary;
    }

    public HeroStats GetHeroStats(string playerID, string heroID)
    {
        if (!statsData.PlayerMatchHistories.ContainsKey(playerID))
        {
            return null;
        }
        
        PlayerMatchHistory playerMatchHistory = statsData.PlayerMatchHistories[playerID];
        if (playerMatchHistory.HeroStats.ContainsKey(heroID))
        {
            return playerMatchHistory.HeroStats[heroID];
        }
        return null;
    }

    public List<HeroStats> GetAllHeroStats(string playerID)
    {
        if (!statsData.PlayerMatchHistories.ContainsKey(playerID))
        {
            InitializePlayerMatchHistory(playerID);
        }
        
        return new List<HeroStats>(statsData.PlayerMatchHistories[playerID].HeroStats.Values);
    }

    public List<string> GetMatchTypes()
    {
        return statsData.MatchTypes;
    }

    public List<string> GetMaps()
    {
        return statsData.Maps;
    }

    public void AddMatchType(string matchType)
    {
        if (!statsData.MatchTypes.Contains(matchType))
        {
            statsData.MatchTypes.Add(matchType);
            dataManager.SaveStatsData();
            Debug.Log("添加比赛类型成功: " + matchType);
        }
    }

    public void RemoveMatchType(string matchType)
    {
        if (statsData.MatchTypes.Contains(matchType))
        {
            statsData.MatchTypes.Remove(matchType);
            dataManager.SaveStatsData();
            Debug.Log("删除比赛类型成功: " + matchType);
        }
    }

    public void AddMap(string mapName)
    {
        if (!statsData.Maps.Contains(mapName))
        {
            statsData.Maps.Add(mapName);
            dataManager.SaveStatsData();
            Debug.Log("添加地图成功: " + mapName);
        }
    }

    public void RemoveMap(string mapName)
    {
        if (statsData.Maps.Contains(mapName))
        {
            statsData.Maps.Remove(mapName);
            dataManager.SaveStatsData();
            Debug.Log("删除地图成功: " + mapName);
        }
    }

    public void CleanupOldMatches(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<string> matchesToRemove = new List<string>();
        
        foreach (KeyValuePair<string, MatchStats> kvp in statsData.AllMatches)
        {
            if (kvp.Value.MatchEndTime < cutoffDate)
            {
                matchesToRemove.Add(kvp.Key);
            }
        }
        
        foreach (string matchID in matchesToRemove)
        {
            statsData.AllMatches.Remove(matchID);
            
            foreach (PlayerMatchHistory playerMatchHistory in statsData.PlayerMatchHistories.Values)
            {
                playerMatchHistory.MatchHistory.RemoveAll(m => m.MatchID == matchID);
            }
        }
        
        if (matchesToRemove.Count > 0)
        {
            dataManager.CreateStatsEvent("match_cleanup", "system", "", "", "清理旧比赛: " + matchesToRemove.Count);
            dataManager.SaveStatsData();
            Debug.Log("清理旧比赛成功: " + matchesToRemove.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveStatsData();
    }

    public void LoadData()
    {
        dataManager.LoadStatsData();
    }

    public List<StatsEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
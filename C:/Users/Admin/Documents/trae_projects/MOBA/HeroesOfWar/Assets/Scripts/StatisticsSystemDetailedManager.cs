using System;
using System.Collections.Generic;

public class StatisticsSystemDetailedManager
{
    private static StatisticsSystemDetailedManager _instance;
    public static StatisticsSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new StatisticsSystemDetailedManager();
            }
            return _instance;
        }
    }

    private StatisticsSystemData statisticsData;
    private StatisticsSystemDataManager dataManager;

    private StatisticsSystemDetailedManager()
    {
        dataManager = StatisticsSystemDataManager.Instance;
        statisticsData = dataManager.statisticsData;
    }

    public void RecordMatchStatistics(string matchID, string gameMode, List<PlayerMatchStats> playerStats, string winTeam, int duration, string mapID, int serverID)
    {
        MatchStatistics match = new MatchStatistics(matchID, gameMode);
        match.Duration = duration;
        match.WinTeam = winTeam;
        match.PlayerStats = playerStats;
        match.MapID = mapID;
        match.ServerID = serverID;

        foreach (PlayerMatchStats stats in playerStats)
        {
            stats.CalculateKDA();
            UpdatePlayerOverallStats(stats, winTeam == GetTeamName(stats.Team));
            UpdateHeroStats(stats, winTeam == GetTeamName(stats.Team));
        }

        CalculateTeamStats(match, winTeam);
        CalculateDamagePercentages(match);
        statisticsData.AddMatchStatistics(match);

        if (!statisticsData.PlayerMatchHistory.ContainsKey(stats.PlayerID))
        {
            statisticsData.PlayerMatchHistory[stats.PlayerID] = new List<MatchStatistics>();
        }
        statisticsData.PlayerMatchHistory[stats.PlayerID].Add(match);

        if (!statisticsData.PlayerDetailedHistory.ContainsKey(stats.PlayerID))
        {
            statisticsData.PlayerDetailedHistory[stats.PlayerID] = new List<PlayerMatchStats>();
        }
        statisticsData.PlayerDetailedHistory[stats.PlayerID].Add(stats);

        dataManager.CreateStatisticsEvent("match_record", stats.PlayerID, "记录比赛统计: " + matchID);
        dataManager.SaveStatisticsData();
    }

    private string GetTeamName(int team)
    {
        return team == 1 ? "Blue" : "Red";
    }

    private void UpdatePlayerOverallStats(PlayerMatchStats matchStats, bool isWin)
    {
        string playerID = matchStats.PlayerID;
        if (!statisticsData.PlayerStats.ContainsKey(playerID))
        {
            statisticsData.PlayerStats[playerID] = new PlayerOverallStats(playerID);
        }

        PlayerOverallStats overall = statisticsData.PlayerStats[playerID];
        overall.TotalMatches++;
        if (isWin) overall.WinMatches++;
        else overall.LoseMatches++;

        overall.TotalDamage += matchStats.Damage;
        overall.TotalTank += matchStats.Tank;
        overall.TotalHeal += matchStats.Heal;

        if (!overall.HeroUsageCount.ContainsKey(matchStats.HeroID))
        {
            overall.HeroUsageCount[matchStats.HeroID] = 0;
            overall.HeroWinCount[matchStats.HeroID] = 0;
        }
        overall.HeroUsageCount[matchStats.HeroID]++;
        if (isWin) overall.HeroWinCount[matchStats.HeroID]++;

        overall.LastMatchTime = DateTime.Now;

        if (matchStats.KillCount >= overall.HighestKillMatch)
        {
            overall.HighestKillMatch = matchStats.KillCount;
        }
        if (matchStats.Damage >= overall.HighestDamageMatch)
        {
            overall.HighestDamageMatch = (int)matchStats.Damage;
        }

        overall.UpdateAverages();
    }

    private void UpdateHeroStats(PlayerMatchStats matchStats, bool isWin)
    {
        string heroID = matchStats.HeroID;
        if (!statisticsData.HeroStats.ContainsKey(heroID))
        {
            statisticsData.HeroStats[heroID] = new HeroStatistics(heroID);
        }

        HeroStatistics hero = statisticsData.HeroStats[heroID];
        hero.TotalPicks++;
        if (isWin) hero.TotalWins++;
        if (matchStats.KillCount > hero.HighestKills)
        {
            hero.HighestKills = matchStats.KillCount;
        }
        hero.UpdateStats();
    }

    private void CalculateTeamStats(MatchStatistics match, string winTeam)
    {
        foreach (PlayerMatchStats stats in match.PlayerStats)
        {
            if (stats.Team == 1)
            {
                match.BlueTeamStats.TotalKills += stats.KillCount;
                match.BlueTeamStats.TotalDeaths += stats.DeathCount;
                match.BlueTeamStats.TotalDamage += stats.Damage;
                match.BlueTeamStats.TotalTank += stats.Tank;
            }
            else
            {
                match.RedTeamStats.TotalKills += stats.KillCount;
                match.RedTeamStats.TotalDeaths += stats.DeathCount;
                match.RedTeamStats.TotalDamage += stats.Damage;
                match.RedTeamStats.TotalTank += stats.Tank;
            }
        }
    }

    private void CalculateDamagePercentages(MatchStatistics match)
    {
        double totalDamage = 0;
        double totalTank = 0;

        foreach (PlayerMatchStats stats in match.PlayerStats)
        {
            totalDamage += stats.Damage;
            totalTank += stats.Tank;
        }

        foreach (PlayerMatchStats stats in match.PlayerStats)
        {
            if (totalDamage > 0)
            {
                stats.DamagePercent = stats.Damage / totalDamage * 100.0;
            }
            if (totalTank > 0)
            {
                stats.TankPercent = stats.Tank / totalTank * 100.0;
            }
        }
    }

    public PlayerOverallStats GetPlayerOverallStats(string playerID)
    {
        if (statisticsData.PlayerStats.ContainsKey(playerID))
        {
            return statisticsData.PlayerStats[playerID];
        }
        return null;
    }

    public HeroStatistics GetHeroStats(string heroID)
    {
        if (statisticsData.HeroStats.ContainsKey(heroID))
        {
            return statisticsData.HeroStats[heroID];
        }
        return null;
    }

    public List<MatchStatistics> GetPlayerMatchHistory(string playerID, int count = 20)
    {
        if (statisticsData.PlayerMatchHistory.ContainsKey(playerID))
        {
            List<MatchStatistics> history = new List<MatchStatistics>(statisticsData.PlayerMatchHistory[playerID]);
            history.Sort((a, b) => b.MatchTime.CompareTo(a.MatchTime));
            if (count < history.Count)
            {
                return history.GetRange(0, count);
            }
            return history;
        }
        return new List<MatchStatistics>();
    }

    public List<PlayerMatchStats> GetPlayerDetailedHistory(string playerID, int count = 20)
    {
        if (statisticsData.PlayerDetailedHistory.ContainsKey(playerID))
        {
            List<PlayerMatchStats> history = new List<PlayerMatchStats>(statisticsData.PlayerDetailedHistory[playerID]);
            if (count < history.Count)
            {
                return history.GetRange(history.Count - count, count);
            }
            return history;
        }
        return new List<PlayerMatchStats>();
    }

    public List<PlayerMatchStats> GetRecentMatchesByHero(string playerID, string heroID, int count = 10)
    {
        List<PlayerMatchStats> allStats = GetPlayerDetailedHistory(playerID, 100);
        List<PlayerMatchStats> filtered = allStats.FindAll(s => s.HeroID == heroID);
        if (count < filtered.Count)
        {
            return filtered.GetRange(filtered.Count - count, count);
        }
        return filtered;
    }

    public Dictionary<string, int> GetHeroUsageRanking(int count = 10)
    {
        Dictionary<string, int> ranking = new Dictionary<string, int>();
        foreach (var hero in statisticsData.HeroStats)
        {
            ranking[hero.Key] = hero.Value.TotalPicks;
        }

        List<KeyValuePair<string, int>> sorted = new List<KeyValuePair<string, int>>(ranking);
        sorted.Sort((a, b) => b.Value.CompareTo(a.Value));

        Dictionary<string, int> result = new Dictionary<string, int>();
        for (int i = 0; i < Math.Min(count, sorted.Count); i++)
        {
            result[sorted[i].Key] = sorted[i].Value;
        }
        return result;
    }

    public Dictionary<string, double> GetHeroWinRateRanking(int count = 10)
    {
        Dictionary<string, double> ranking = new Dictionary<string, double>();
        foreach (var hero in statisticsData.HeroStats)
        {
            if (hero.Value.TotalPicks >= 10)
            {
                ranking[hero.Key] = hero.Value.WinRate;
            }
        }

        List<KeyValuePair<string, double>> sorted = new List<KeyValuePair<string, double>>(ranking);
        sorted.Sort((a, b) => b.Value.CompareTo(a.Value));

        Dictionary<string, double> result = new Dictionary<string, double>();
        for (int i = 0; i < Math.Min(count, sorted.Count); i++)
        {
            result[sorted[i].Key] = sorted[i].Value;
        }
        return result;
    }

    public double GetPlayerWinRate(string playerID)
    {
        if (statisticsData.PlayerStats.ContainsKey(playerID))
        {
            PlayerOverallStats stats = statisticsData.PlayerStats[playerID];
            if (stats.TotalMatches > 0)
            {
                return (double)stats.WinMatches / stats.TotalMatches * 100.0;
            }
        }
        return 0.0;
    }

    public double GetPlayerAverageKDA(string playerID)
    {
        List<PlayerMatchStats> history = GetPlayerDetailedHistory(playerID, 20);
        if (history.Count > 0)
        {
            double totalKDA = 0;
            foreach (PlayerMatchStats stats in history)
            {
                totalKDA += stats.KDA;
            }
            return totalKDA / history.Count;
        }
        return 0.0;
    }

    public Dictionary<string, double> GetPlayerHeroWinRates(string playerID)
    {
        Dictionary<string, double> winRates = new Dictionary<string, double>();
        if (statisticsData.PlayerStats.ContainsKey(playerID))
        {
            PlayerOverallStats stats = statisticsData.PlayerStats[playerID];
            foreach (var hero in stats.HeroUsageCount)
            {
                int wins = stats.HeroWinCount.ContainsKey(hero.Key) ? stats.HeroWinCount[hero.Key] : 0;
                winRates[hero.Key] = (double)wins / hero.Value * 100.0;
            }
        }
        return winRates;
    }

    public int GetPlayerTotalDamage(string playerID)
    {
        if (statisticsData.PlayerStats.ContainsKey(playerID))
        {
            return (int)statisticsData.PlayerStats[playerID].TotalDamage;
        }
        return 0;
    }

    public int GetPlayerHighestKills(string playerID)
    {
        if (statisticsData.PlayerStats.ContainsKey(playerID))
        {
            return statisticsData.PlayerStats[playerID].HighestKillMatch;
        }
        return 0;
    }

    public List<MatchStatistics> GetRecentMatches(int count = 20)
    {
        List<MatchStatistics> matches = new List<MatchStatistics>(statisticsData.MatchHistory);
        matches.Sort((a, b) => b.MatchTime.CompareTo(a.MatchTime));
        if (count < matches.Count)
        {
            return matches.GetRange(0, count);
        }
        return matches;
    }

    public Dictionary<string, int> GetGameModeStatistics()
    {
        Dictionary<string, int> stats = new Dictionary<string, int>();
        foreach (MatchStatistics match in statisticsData.MatchHistory)
        {
            if (stats.ContainsKey(match.GameMode))
            {
                stats[match.GameMode]++;
            }
            else
            {
                stats[match.GameMode] = 1;
            }
        }
        return stats;
    }

    public int GetTotalMatchCount()
    {
        return statisticsData.MatchHistory.Count;
    }

    public int GetTodayMatchCount()
    {
        DateTime today = DateTime.Today;
        int count = 0;
        foreach (MatchStatistics match in statisticsData.MatchHistory)
        {
            if (match.MatchTime.Date == today)
            {
                count++;
            }
        }
        return count;
    }

    public Dictionary<string, double> GetPlayerRecentPerformance(string playerID, int matchCount = 5)
    {
        Dictionary<string, double> performance = new Dictionary<string, double>();
        List<PlayerMatchStats> recent = GetPlayerDetailedHistory(playerID, matchCount);

        if (recent.Count > 0)
        {
            double avgKDA = 0, avgDamage = 0, avgTank = 0;
            foreach (PlayerMatchStats stats in recent)
            {
                avgKDA += stats.KDA;
                avgDamage += stats.Damage;
                avgTank += stats.Tank;
            }
            performance["AverageKDA"] = avgKDA / recent.Count;
            performance["AverageDamage"] = avgDamage / recent.Count;
            performance["AverageTank"] = avgTank / recent.Count;
        }

        return performance;
    }

    public List<StatisticsEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveStatisticsData();
    }

    public void LoadData()
    {
        dataManager.LoadStatisticsData();
    }
}
using System;
using System.Collections.Generic;

public class CareerSystemDetailedManager
{
    private static CareerSystemDetailedManager _instance;
    public static CareerSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CareerSystemDetailedManager();
            }
            return _instance;
        }
    }

    private CareerData careerData;
    private CareerSystemDataManager dataManager;

    private CareerSystemDetailedManager()
    {
        dataManager = CareerSystemDataManager.Instance;
        careerData = dataManager.careerData;
    }

    public void InitializePlayerCareer(string playerID)
    {
        if (!careerData.PlayerCareers.ContainsKey(playerID))
        {
            careerData.PlayerCareers[playerID] = new CareerStatistics(playerID);
            dataManager.CreateCareerEvent("career_init", playerID, "初始化生涯数据");
            dataManager.SaveCareerData();
            Debug.Log("初始化玩家生涯: " + playerID);
        }
    }

    public void RecordMatch(string playerID, string heroID, string heroName, string gameMode, bool isWin, int kill, int death, int assist, double damage, double tank, double gold, int duration, int rankBefore, int rankAfter, string serverName)
    {
        if (!careerData.PlayerCareers.ContainsKey(playerID))
        {
            InitializePlayerCareer(playerID);
        }

        string matchID = "match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchRecord record = new MatchRecord(matchID, playerID, heroID, heroName, gameMode);
        record.IsWin = isWin;
        record.KillCount = kill;
        record.DeathCount = death;
        record.AssistCount = assist;
        record.Damage = damage;
        record.Tank = tank;
        record.Gold = gold;
        record.Duration = duration;
        record.RankBefore = rankBefore;
        record.RankAfter = rankAfter;
        record.ServerName = serverName;
        record.CalculateKDA();

        careerData.AddMatchRecord(playerID, record);

        CareerStatistics career = careerData.PlayerCareers[playerID];
        if (isWin)
        {
            career.AddWin();
        }
        else
        {
            career.AddLose();
        }

        if (!career.HeroUsageCount.ContainsKey(heroID))
        {
            career.HeroUsageCount[heroID] = 0;
            career.HeroWinCount[heroID] = 0;
        }
        career.HeroUsageCount[heroID]++;
        if (isWin)
        {
            career.HeroWinCount[heroID]++;
        }

        career.TotalPlayTime += duration;
        career.LastPlayTime = DateTime.Now;

        UpdateFavoriteHeroes(career);
        CheckAndUnlockAchievements(playerID);

        dataManager.CreateCareerEvent("match_record", playerID, "记录比赛: " + matchID + " 英雄:" + heroName);
        dataManager.SaveCareerData();
        Debug.Log("记录比赛完成: " + matchID);
    }

    public void RecordMVP(string playerID)
    {
        if (careerData.PlayerCareers.ContainsKey(playerID))
        {
            careerData.PlayerCareers[playerID].IncrementMVP();
            dataManager.CreateCareerEvent("mvp_record", playerID, "获得MVP");
            dataManager.SaveCareerData();
        }
    }

    public void RecordSVP(string playerID)
    {
        if (careerData.PlayerCareers.ContainsKey(playerID))
        {
            careerData.PlayerCareers[playerID].IncrementSVP();
            dataManager.CreateCareerEvent("svp_record", playerID, "获得SVP");
            dataManager.SaveCareerData();
        }
    }

    private void UpdateFavoriteHeroes(CareerStatistics career)
    {
        career.FavoriteHeroes.Clear();
        var sortedHeroes = new List<KeyValuePair<string, int>>(career.HeroUsageCount);
        sortedHeroes.Sort((a, b) => b.Value.CompareTo(a.Value));
        int count = 0;
        foreach (var kvp in sortedHeroes)
        {
            if (count >= 5) break;
            career.FavoriteHeroes.Add(kvp.Key);
            count++;
        }
    }

    private void CheckAndUnlockAchievements(string playerID)
    {
        CareerStatistics career = careerData.PlayerCareers[playerID];
        List<AchievementRecord> achievements = new List<AchievementRecord>();

        if (career.WinMatches >= 1 && !HasAchievement(playerID, "first_win"))
        {
            achievements.Add(new AchievementRecord("first_win", "首胜"));
        }
        if (career.MaxConsecutiveWins >= 10 && !HasAchievement(playerID, "win_10"))
        {
            achievements.Add(new AchievementRecord("win_10", "十连胜"));
        }
        if (career.WinMatches >= 100 && !HasAchievement(playerID, "win_100"))
        {
            achievements.Add(new AchievementRecord("win_100", "百场胜利"));
        }
        if (career.MVPCount >= 50 && !HasAchievement(playerID, "mvp_50"))
        {
            achievements.Add(new AchievementRecord("mvp_50", "MVP之星"));
        }
        if (career.WinRate >= 60.0 && !HasAchievement(playerID, "win_rate_60"))
        {
            achievements.Add(new AchievementRecord("win_rate_60", "高胜率"));
        }
        if (career.TotalPlayTime >= 360000 && !HasAchievement(playerID, "play_time_100"))
        {
            achievements.Add(new AchievementRecord("play_time_100", "老玩家"));
        }

        foreach (AchievementRecord achievement in achievements)
        {
            careerData.AddAchievement(playerID, achievement);
            dataManager.CreateCareerEvent("achievement_unlock", playerID, "解锁成就: " + achievement.AchievementName);
        }
    }

    private bool HasAchievement(string playerID, string achievementID)
    {
        if (careerData.PlayerAchievements.ContainsKey(playerID))
        {
            return careerData.PlayerAchievements[playerID].Exists(a => a.AchievementID == achievementID);
        }
        return false;
    }

    public CareerStatistics GetPlayerCareer(string playerID)
    {
        if (careerData.PlayerCareers.ContainsKey(playerID))
        {
            return careerData.PlayerCareers[playerID];
        }
        return null;
    }

    public List<MatchRecord> GetPlayerMatchRecords(string playerID, int count = 20)
    {
        if (careerData.PlayerMatchRecords.ContainsKey(playerID))
        {
            List<MatchRecord> records = new List<MatchRecord>(careerData.PlayerMatchRecords[playerID]);
            records.Sort((a, b) => b.MatchTime.CompareTo(a.MatchTime));
            if (count < records.Count)
            {
                return records.GetRange(0, count);
            }
            return records;
        }
        return new List<MatchRecord>();
    }

    public List<MatchRecord> GetPlayerMatchRecordsByHero(string playerID, string heroID)
    {
        if (careerData.PlayerMatchRecords.ContainsKey(playerID))
        {
            return careerData.PlayerMatchRecords[playerID].FindAll(m => m.HeroID == heroID);
        }
        return new List<MatchRecord>();
    }

    public List<MatchRecord> GetRecentMatches(string playerID, int count = 10)
    {
        return GetPlayerMatchRecords(playerID, count);
    }

    public List<AchievementRecord> GetPlayerAchievements(string playerID)
    {
        if (careerData.PlayerAchievements.ContainsKey(playerID))
        {
            return careerData.PlayerAchievements[playerID];
        }
        return new List<AchievementRecord>();
    }

    public List<AchievementRecord> GetNewAchievements(string playerID)
    {
        if (careerData.PlayerAchievements.ContainsKey(playerID))
        {
            return careerData.PlayerAchievements[playerID].FindAll(a => a.IsNew);
        }
        return new List<AchievementRecord>();
    }

    public void MarkAchievementAsRead(string playerID, string achievementID)
    {
        if (careerData.PlayerAchievements.ContainsKey(playerID))
        {
            AchievementRecord achievement = careerData.PlayerAchievements[playerID].Find(a => a.AchievementID == achievementID);
            if (achievement != null)
            {
                achievement.IsNew = false;
                dataManager.SaveCareerData();
            }
        }
    }

    public List<CareerMilestone> GetAllMilestones()
    {
        return careerData.Milestones;
    }

    public CareerMilestone GetMilestone(string milestoneID)
    {
        return careerData.Milestones.Find(m => m.MilestoneID == milestoneID);
    }

    public Dictionary<string, int> GetHeroUsageStatistics(string playerID)
    {
        if (careerData.PlayerCareers.ContainsKey(playerID))
        {
            return careerData.PlayerCareers[playerID].HeroUsageCount;
        }
        return new Dictionary<string, int>();
    }

    public double GetHeroWinRate(string playerID, string heroID)
    {
        if (careerData.PlayerCareers.ContainsKey(playerID))
        {
            CareerStatistics career = careerData.PlayerCareers[playerID];
            if (career.HeroUsageCount.ContainsKey(heroID))
            {
                int total = career.HeroUsageCount[heroID];
                int wins = career.HeroWinCount.ContainsKey(heroID) ? career.HeroWinCount[heroID] : 0;
                if (total > 0)
                {
                    return (double)wins / total * 100.0;
                }
            }
        }
        return 0.0;
    }

    public Dictionary<string, double> GetAverageStats(string playerID)
    {
        Dictionary<string, double> stats = new Dictionary<string, double>();
        if (careerData.PlayerCareers.ContainsKey(playerID))
        {
            CareerStatistics career = careerData.PlayerCareers[playerID];
            if (career.TotalMatches > 0)
            {
                stats["AverageKDA"] = career.AverageKDA;
                stats["AverageDamage"] = career.AverageDamage;
                stats["AverageTank"] = career.AverageTank;
                stats["AverageGold"] = career.AverageGold;
            }
        }
        return stats;
    }

    public List<MatchRecord> GetMatchesByDateRange(string playerID, DateTime startDate, DateTime endDate)
    {
        if (careerData.PlayerMatchRecords.ContainsKey(playerID))
        {
            return careerData.PlayerMatchRecords[playerID].FindAll(m => m.MatchTime >= startDate && m.MatchTime <= endDate);
        }
        return new List<MatchRecord>();
    }

    public int GetTotalPlayTimeHours(string playerID)
    {
        if (careerData.PlayerCareers.ContainsKey(playerID))
        {
            return careerData.PlayerCareers[playerID].TotalPlayTime / 3600;
        }
        return 0;
    }

    public string GetPlayerRankTitle(string playerID)
    {
        if (careerData.PlayerCareers.ContainsKey(playerID))
        {
            int rank = careerData.PlayerCareers[playerID].HighestRank;
            if (rank >= 1 && rank <= 5) return "青铜";
            if (rank >= 6 && rank <= 10) return "白银";
            if (rank >= 11 && rank <= 15) return "黄金";
            if (rank >= 16 && rank <= 20) return "铂金";
            if (rank >= 21 && rank <= 25) return "钻石";
            if (rank >= 26 && rank <= 30) return "星耀";
            if (rank >= 31 && rank <= 40) return "王者";
            if (rank >= 41) return "荣耀王者";
        }
        return "坚韧黑铁";
    }

    public List<CareerEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveCareerData();
    }

    public void LoadData()
    {
        dataManager.LoadCareerData();
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class CareerStatistics
{
    public string PlayerID;
    public int TotalMatches;
    public int WinMatches;
    public int LoseMatches;
    public int MVPCount;
    public int SVPCount;
    public int ConsecutiveWins;
    public int ConsecutiveLosses;
    public int MaxConsecutiveWins;
    public int MaxConsecutiveLosses;
    public double WinRate;
    public double AverageKDA;
    public double AverageDamage;
    public double AverageTank;
    public double AverageGold;
    public int TotalPlayTime;
    public int HighestRank;
    public int CurrentStars;
    public int TotalStars;
    public Dictionary<string, int> HeroUsageCount;
    public Dictionary<string, int> HeroWinCount;
    public List<string> FavoriteHeroes;
    public DateTime FirstPlayTime;
    public DateTime LastPlayTime;

    public CareerStatistics(string playerID)
    {
        PlayerID = playerID;
        TotalMatches = 0;
        WinMatches = 0;
        LoseMatches = 0;
        MVPCount = 0;
        SVPCount = 0;
        ConsecutiveWins = 0;
        ConsecutiveLosses = 0;
        MaxConsecutiveWins = 0;
        MaxConsecutiveLosses = 0;
        WinRate = 0.0;
        AverageKDA = 0.0;
        AverageDamage = 0.0;
        AverageTank = 0.0;
        AverageGold = 0.0;
        TotalPlayTime = 0;
        HighestRank = 1;
        CurrentStars = 0;
        TotalStars = 0;
        HeroUsageCount = new Dictionary<string, int>();
        HeroWinCount = new Dictionary<string, int>();
        FavoriteHeroes = new List<string>();
        FirstPlayTime = DateTime.Now;
        LastPlayTime = DateTime.Now;
    }

    public void UpdateStatistics()
    {
        if (TotalMatches > 0)
        {
            WinRate = (double)WinMatches / TotalMatches * 100.0;
        }
    }

    public void AddWin()
    {
        WinMatches++;
        TotalMatches++;
        ConsecutiveWins++;
        ConsecutiveLosses = 0;
        if (ConsecutiveWins > MaxConsecutiveWins)
        {
            MaxConsecutiveWins = ConsecutiveWins;
        }
        UpdateStatistics();
    }

    public void AddLose()
    {
        LoseMatches++;
        TotalMatches++;
        ConsecutiveLosses++;
        ConsecutiveWins = 0;
        if (ConsecutiveLosses > MaxConsecutiveLosses)
        {
            MaxConsecutiveLosses = ConsecutiveLosses;
        }
        UpdateStatistics();
    }

    public void IncrementMVP()
    {
        MVPCount++;
    }

    public void IncrementSVP()
    {
        SVPCount++;
    }
}

[Serializable]
public class MatchRecord
{
    public string MatchID;
    public string PlayerID;
    public string HeroID;
    public string HeroName;
    public string GameMode;
    public bool IsWin;
    public int KillCount;
    public int DeathCount;
    public int AssistCount;
    public double KDA;
    public double Damage;
    public double Tank;
    public double Gold;
    public int Duration;
    public DateTime MatchTime;
    public int RankBefore;
    public int RankAfter;
    public string ServerName;

    public MatchRecord(string matchID, string playerID, string heroID, string heroName, string gameMode)
    {
        MatchID = matchID;
        PlayerID = playerID;
        HeroID = heroID;
        HeroName = heroName;
        GameMode = gameMode;
        IsWin = false;
        KillCount = 0;
        DeathCount = 0;
        AssistCount = 0;
        KDA = 0.0;
        Damage = 0.0;
        Tank = 0.0;
        Gold = 0.0;
        Duration = 0;
        MatchTime = DateTime.Now;
        RankBefore = 0;
        RankAfter = 0;
        ServerName = "";
    }

    public void CalculateKDA()
    {
        if (DeathCount == 0)
        {
            KDA = (KillCount + AssistCount) * 1.0;
        }
        else
        {
            KDA = (KillCount + AssistCount) * 1.0 / DeathCount;
        }
    }
}

[Serializable]
public class AchievementRecord
{
    public string AchievementID;
    public string AchievementName;
    public DateTime UnlockTime;
    public bool IsNew;

    public AchievementRecord(string achievementID, string achievementName)
    {
        AchievementID = achievementID;
        AchievementName = achievementName;
        UnlockTime = DateTime.Now;
        IsNew = true;
    }
}

[Serializable]
public class CareerData
{
    public Dictionary<string, CareerStatistics> PlayerCareers;
    public Dictionary<string, List<MatchRecord>> PlayerMatchRecords;
    public Dictionary<string, List<AchievementRecord>> PlayerAchievements;
    public Dictionary<string, Dictionary<string, int>> PlayerHeroStats;
    public List<CareerMilestone> Milestones;
    public DateTime LastUpdated;

    public CareerData()
    {
        PlayerCareers = new Dictionary<string, CareerStatistics>();
        PlayerMatchRecords = new Dictionary<string, List<MatchRecord>>();
        PlayerAchievements = new Dictionary<string, List<AchievementRecord>>();
        PlayerHeroStats = new Dictionary<string, Dictionary<string, int>>();
        Milestones = new List<CareerMilestone>();
        LastUpdated = DateTime.Now;
        InitializeMilestones();
    }

    private void InitializeMilestones()
    {
        Milestones.Add(new CareerMilestone("first_win", "首胜", "获得第一场胜利", 1, 1));
        Milestones.Add(new CareerMilestone("win_10", "十连胜", "连续获得10场胜利", 10, 2));
        Milestones.Add(new CareerMilestone("win_100", "百场胜利", "累计获得100场胜利", 100, 5));
        Milestones.Add(new CareerMilestone("mvp_50", "MVP之星", "累计获得50次MVP", 50, 3));
        Milestones.Add(new CareerMilestone("win_rate_60", "高胜率", "胜率达到60%", 60, 2));
        Milestones.Add(new CareerMilestone("play_time_100", "老玩家", "累计游戏100小时", 100, 2));
    }

    public void AddMatchRecord(string playerID, MatchRecord record)
    {
        if (!PlayerMatchRecords.ContainsKey(playerID))
        {
            PlayerMatchRecords[playerID] = new List<MatchRecord>();
        }
        PlayerMatchRecords[playerID].Add(record);
    }

    public void AddAchievement(string playerID, AchievementRecord achievement)
    {
        if (!PlayerAchievements.ContainsKey(playerID))
        {
            PlayerAchievements[playerID] = new List<AchievementRecord>();
        }
        PlayerAchievements[playerID].Add(achievement);
    }
}

[Serializable]
public class CareerMilestone
{
    public string MilestoneID;
    public string MilestoneName;
    public string Description;
    public int TargetValue;
    public int RewardValue;

    public CareerMilestone(string id, string name, string description, int target, int reward)
    {
        MilestoneID = id;
        MilestoneName = name;
        Description = description;
        TargetValue = target;
        RewardValue = reward;
    }
}

[Serializable]
public class CareerEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EventData;

    public CareerEvent(string eventID, string eventType, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class CareerSystemDataManager
{
    private static CareerSystemDataManager _instance;
    public static CareerSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CareerSystemDataManager();
            }
            return _instance;
        }
    }

    public CareerData careerData;
    private List<CareerEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private CareerSystemDataManager()
    {
        careerData = new CareerData();
        recentEvents = new List<CareerEvent>();
        LoadCareerData();
    }

    public void SaveCareerData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "CareerSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, careerData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存生涯系统数据失败: " + e.Message);
        }
    }

    public void LoadCareerData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "CareerSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    careerData = (CareerData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载生涯系统数据失败: " + e.Message);
            careerData = new CareerData();
        }
    }

    public void CreateCareerEvent(string eventType, string playerID, string eventData)
    {
        string eventID = "career_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CareerEvent careerEvent = new CareerEvent(eventID, eventType, playerID, eventData);
        recentEvents.Add(careerEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<CareerEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
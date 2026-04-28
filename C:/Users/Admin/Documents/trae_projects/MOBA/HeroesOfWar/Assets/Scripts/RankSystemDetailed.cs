using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class RankLevel
{
    public int LevelID;
    public string LevelName;
    public string LevelColor;
    public int MinStars;
    public int MaxStars;
    public int MinScore;
    public bool IsPromotionMatch;
    public string IconName;

    public RankLevel(int levelID, string levelName, string levelColor, int minStars, int maxStars, int minScore, bool isPromotionMatch)
    {
        LevelID = levelID;
        LevelName = levelName;
        LevelColor = levelColor;
        MinStars = minStars;
        MaxStars = maxStars;
        MinScore = minScore;
        IsPromotionMatch = isPromotionMatch;
        IconName = "";
    }
}

[Serializable]
public class PlayerRankData
{
    public string PlayerID;
    public int CurrentLevel;
    public int CurrentStars;
    public int CurrentScore;
    public int HighestLevel;
    public int HighestStars;
    public int TotalMatches;
    public int WinMatches;
    public int LoseMatches;
    public double WinRate;
    public int CurrentWinStreak;
    public int MaxWinStreak;
    public int SeasonWins;
    public int SeasonLoses;
    public int SeasonRank;
    public DateTime LastMatchTime;
    public bool IsInPromotionMatch;
    public int PromotionMatchWins;
    public int PromotionMatchLoses;

    public PlayerRankData(string playerID)
    {
        PlayerID = playerID;
        CurrentLevel = 1;
        CurrentStars = 0;
        CurrentScore = 0;
        HighestLevel = 1;
        HighestStars = 0;
        TotalMatches = 0;
        WinMatches = 0;
        LoseMatches = 0;
        WinRate = 0.0;
        CurrentWinStreak = 0;
        MaxWinStreak = 0;
        SeasonWins = 0;
        SeasonLoses = 0;
        SeasonRank = 0;
        LastMatchTime = DateTime.Now;
        IsInPromotionMatch = false;
        PromotionMatchWins = 0;
        PromotionMatchLoses = 0;
    }

    public void UpdateStats()
    {
        if (TotalMatches > 0)
        {
            WinRate = (double)WinMatches / TotalMatches * 100.0;
        }
    }
}

[Serializable]
public class RankMatch
{
    public string MatchID;
    public string PlayerID;
    public int MatchType;
    public bool IsWin;
    public int LevelBefore;
    public int StarsBefore;
    public int LevelAfter;
    public int StarsAfter;
    public int ScoreChange;
    public string OpponentID;
    public int OpponentLevel;
    public DateTime MatchTime;
    public string MatchResult;

    public RankMatch(string matchID, string playerID, int matchType, bool isWin, int levelBefore, int starsBefore, int levelAfter, int starsAfter, int scoreChange)
    {
        MatchID = matchID;
        PlayerID = playerID;
        MatchType = matchType;
        IsWin = isWin;
        LevelBefore = levelBefore;
        StarsBefore = starsBefore;
        LevelAfter = levelAfter;
        StarsAfter = starsAfter;
        ScoreChange = scoreChange;
        OpponentID = "";
        OpponentLevel = 0;
        MatchTime = DateTime.Now;
        MatchResult = isWin ? "胜利" : "失败";
    }
}

[Serializable]
public class Season
{
    public string SeasonID;
    public string SeasonName;
    public DateTime StartTime;
    public DateTime EndTime;
    public int SeasonNumber;
    public bool IsCurrent;
    public string Theme;
    public List<string> Rewards;

    public Season(string seasonID, string seasonName, int seasonNumber, DateTime startTime, DateTime endTime)
    {
        SeasonID = seasonID;
        SeasonName = seasonName;
        SeasonNumber = seasonNumber;
        StartTime = startTime;
        EndTime = endTime;
        IsCurrent = false;
        Theme = "";
        Rewards = new List<string>();
    }
}

[Serializable]
public class RankSystemData
{
    public List<RankLevel> RankLevels;
    public Dictionary<string, PlayerRankData> PlayerRankData;
    public List<RankMatch> MatchHistory;
    public List<Season> Seasons;
    public Dictionary<string, List<string>> PlayerMatchHistory;
    public int CurrentSeason;
    public DateTime LastCleanupTime;

    public RankSystemData()
    {
        RankLevels = new List<RankLevel>();
        PlayerRankData = new Dictionary<string, PlayerRankData>();
        MatchHistory = new List<RankMatch>();
        Seasons = new List<Season>();
        PlayerMatchHistory = new Dictionary<string, List<string>>();
        CurrentSeason = 1;
        LastCleanupTime = DateTime.Now;
        InitializeDefaultRankLevels();
        InitializeDefaultSeasons();
    }

    private void InitializeDefaultRankLevels()
    {
        RankLevels.Add(new RankLevel(1, "坚韧黑铁", "#888888", 0, 3, 0, false));
        RankLevels.Add(new RankLevel(2, "英勇黄铜", "#CD7F32", 0, 3, 100, false));
        RankLevels.Add(new RankLevel(3, "不屈白银", "#C0C0C0", 0, 3, 200, false));
        RankLevels.Add(new RankLevel(4, "荣耀黄金", "#FFD700", 0, 4, 300, true));
        RankLevels.Add(new RankLevel(5, "华贵铂金", "#E5E4E2", 0, 4, 400, true));
        RankLevels.Add(new RankLevel(6, "璀璨钻石", "#B9F2FF", 0, 4, 500, true));
        RankLevels.Add(new RankLevel(7, "星耀", "#FFB6C1", 0, 5, 600, true));
        RankLevels.Add(new RankLevel(8, "最强王者", "#FF4500", 0, 999, 700, false));
        RankLevels.Add(new RankLevel(9, "荣耀王者", "#8A2BE2", 0, 999, 800, false));
    }

    private void InitializeDefaultSeasons()
    {
        Season currentSeason = new Season("season_001", "S1赛季", 1, DateTime.Now, DateTime.Now.AddDays(90));
        currentSeason.IsCurrent = true;
        currentSeason.Theme = "初露锋芒";
        currentSeason.Rewards.Add("赛季皮肤");
        currentSeason.Rewards.Add("段位框");
        Seasons.Add(currentSeason);
    }

    public void AddRankMatch(RankMatch match)
    {
        MatchHistory.Add(match);
    }

    public void AddPlayerRankData(string playerID, PlayerRankData data)
    {
        PlayerRankData[playerID] = data;
    }

    public void AddSeason(Season season)
    {
        Seasons.Add(season);
    }
}

[Serializable]
public class RankEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EventData;

    public RankEvent(string eventID, string eventType, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class RankSystemDataManager
{
    private static RankSystemDataManager _instance;
    public static RankSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RankSystemDataManager();
            }
            return _instance;
        }
    }

    public RankSystemData rankData;
    private List<RankEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private RankSystemDataManager()
    {
        rankData = new RankSystemData();
        recentEvents = new List<RankEvent>();
        LoadRankData();
    }

    public void SaveRankData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "RankSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, rankData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存排位赛系统数据失败: " + e.Message);
        }
    }

    public void LoadRankData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "RankSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    rankData = (RankSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载排位赛系统数据失败: " + e.Message);
            rankData = new RankSystemData();
        }
    }

    public void CreateRankEvent(string eventType, string playerID, string eventData)
    {
        string eventID = "rank_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RankEvent rankEvent = new RankEvent(eventID, eventType, playerID, eventData);
        recentEvents.Add(rankEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<RankEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
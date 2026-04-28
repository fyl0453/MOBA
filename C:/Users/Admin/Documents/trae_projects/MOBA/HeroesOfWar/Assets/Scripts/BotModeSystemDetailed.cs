using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class AIBot
{
    public string BotID;
    public string BotName;
    public string BotLevel;
    public string Difficulty;
    public string PlayStyle;
    public string HeroID;
    public string HeroName;
    public int Aggressiveness;
    public int Defensive;
    public int FarmingPriority;
    public int RoamingPriority;
    public bool IsAvailable;
    public DateTime LastUsedTime;

    public AIBot(string botName, string difficulty, string playStyle, string heroID, string heroName)
    {
        BotID = "bot_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BotName = botName;
        BotLevel = "1";
        Difficulty = difficulty;
        PlayStyle = playStyle;
        HeroID = heroID;
        HeroName = heroName;
        Aggressiveness = 50;
        Defensive = 50;
        FarmingPriority = 50;
        RoamingPriority = 50;
        IsAvailable = true;
        LastUsedTime = DateTime.MinValue;
    }
}

[Serializable]
public class Tutorial
{
    public string TutorialID;
    public string TutorialName;
    public string Description;
    public string Category;
    public int Order;
    public string TargetHeroID;
    public List<TutorialStep> Steps;
    public bool IsCompleted;
    public int RequiredLevel;
    public string IconURL;
    public string VideoURL;
    public bool IsRepeatable;
    public int RewardExperience;
    public int RewardGold;

    public Tutorial(string tutorialName, string description, string category, int order)
    {
        TutorialID = "tutorial_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TutorialName = tutorialName;
        Description = description;
        Category = category;
        Order = order;
        TargetHeroID = "";
        Steps = new List<TutorialStep>();
        IsCompleted = false;
        RequiredLevel = 1;
        IconURL = "";
        VideoURL = "";
        IsRepeatable = false;
        RewardExperience = 0;
        RewardGold = 0;
    }
}

[Serializable]
public class TutorialStep
{
    public int StepOrder;
    public string StepTitle;
    public string StepDescription;
    public string StepType;
    public string TargetObject;
    public Vector3 TargetPosition;
    public float Duration;
    public bool IsOptional;
    public string Hint;

    public TutorialStep(int stepOrder, string stepTitle, string stepDescription, string stepType, string targetObject, float duration)
    {
        StepOrder = stepOrder;
        StepTitle = stepTitle;
        StepDescription = stepDescription;
        StepType = stepType;
        TargetObject = targetObject;
        TargetPosition = Vector3.zero;
        Duration = duration;
        IsOptional = false;
        Hint = "";
    }
}

[Serializable]
public class PlayerTutorialProgress
{
    public string PlayerID;
    public Dictionary<string, bool> CompletedTutorials;
    public Dictionary<string, int> TutorialSteps;
    public int TotalTutorialsCompleted;
    public int TotalTutorialTime;
    public DateTime LastTutorialTime;

    public PlayerTutorialProgress(string playerID)
    {
        PlayerID = playerID;
        CompletedTutorials = new Dictionary<string, bool>();
        TutorialSteps = new Dictionary<string, int>();
        TotalTutorialsCompleted = 0;
        TotalTutorialTime = 0;
        LastTutorialTime = DateTime.MinValue;
    }
}

[Serializable]
public class BotMatch
{
    public string MatchID;
    public string MatchType;
    public string Difficulty;
    public List<string> PlayerTeamBotIDs;
    public List<string> EnemyTeamBotIDs;
    public string MapName;
    public string MatchStatus;
    public DateTime MatchStartTime;
    public DateTime MatchEndTime;
    public float MatchDuration;
    public string WinnerTeam;
    public bool IsTutorial;

    public BotMatch(string matchType, string difficulty, string mapName, bool isTutorial)
    {
        MatchID = "bot_match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchType = matchType;
        Difficulty = difficulty;
        PlayerTeamBotIDs = new List<string>();
        EnemyTeamBotIDs = new List<string>();
        MapName = mapName;
        MatchStatus = "waiting";
        MatchStartTime = DateTime.Now;
        MatchEndTime = DateTime.MinValue;
        MatchDuration = 0f;
        WinnerTeam = "";
        IsTutorial = isTutorial;
    }
}

[Serializable]
public class BotModeSystemData
{
    public List<AIBot> AvailableBots;
    public List<Tutorial> AvailableTutorials;
    public Dictionary<string, PlayerTutorialProgress> PlayerTutorialProgress;
    public List<BotMatch> BotMatches;
    public List<string> Difficulties;
    public List<string> MatchTypes;
    public List<string> Maps;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public BotModeSystemData()
    {
        AvailableBots = new List<AIBot>();
        PlayerTutorialProgress = new Dictionary<string, PlayerTutorialProgress>();
        BotMatches = new List<BotMatch>();
        Difficulties = new List<string> { "easy", "medium", "hard", "extreme" };
        MatchTypes = new List<string> { "practice", "tutorial", "challenge", "arcade" };
        Maps = new List<string> { "Summoner's Rift", "Howling Abyss" };
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class BotModeEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string MatchID;
    public string EventData;

    public BotModeEvent(string eventID, string eventType, string playerID, string matchID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        MatchID = matchID;
        EventData = eventData;
    }
}

public class BotModeSystemDataManager
{
    private static BotModeSystemDataManager _instance;
    public static BotModeSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BotModeSystemDataManager();
            }
            return _instance;
        }
    }

    public BotModeSystemData botModeData;
    private List<BotModeEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private BotModeSystemDataManager()
    {
        botModeData = new BotModeSystemData();
        recentEvents = new List<BotModeEvent>();
        LoadBotModeData();
    }

    public void SaveBotModeData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "BotModeSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, botModeData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存人机模式系统数据失败: " + e.Message);
        }
    }

    public void LoadBotModeData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "BotModeSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    botModeData = (BotModeSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载人机模式系统数据失败: " + e.Message);
            botModeData = new BotModeSystemData();
        }
    }

    public void CreateBotModeEvent(string eventType, string playerID, string matchID, string eventData)
    {
        string eventID = "bot_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BotModeEvent botModeEvent = new BotModeEvent(eventID, eventType, playerID, matchID, eventData);
        recentEvents.Add(botModeEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<BotModeEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class BotModeSystemDetailedManager
{
    private static BotModeSystemDetailedManager _instance;
    public static BotModeSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BotModeSystemDetailedManager();
            }
            return _instance;
        }
    }

    private BotModeSystemData botModeData;
    private BotModeSystemDataManager dataManager;

    private BotModeSystemDetailedManager()
    {
        dataManager = BotModeSystemDataManager.Instance;
        botModeData = dataManager.botModeData;
    }

    public void InitializePlayerTutorialProgress(string playerID)
    {
        if (!botModeData.PlayerTutorialProgress.ContainsKey(playerID))
        {
            PlayerTutorialProgress progress = new PlayerTutorialProgress(playerID);
            botModeData.PlayerTutorialProgress[playerID] = progress;
            dataManager.SaveBotModeData();
            Debug.Log("初始化人机教程进度成功");
        }
    }

    public string CreateAIBot(string botName, string difficulty, string playStyle, string heroID, string heroName)
    {
        AIBot bot = new AIBot(botName, difficulty, playStyle, heroID, heroName);
        botModeData.AvailableBots.Add(bot);
        
        dataManager.CreateBotModeEvent("bot_create", "system", "", "创建AI机器人: " + botName);
        dataManager.SaveBotModeData();
        Debug.Log("创建AI机器人成功: " + botName);
        return bot.BotID;
    }

    public void UpdateAIBot(string botID, string difficulty, int aggressiveness, int defensive, int farmingPriority, int roamingPriority)
    {
        AIBot bot = botModeData.AvailableBots.Find(b => b.BotID == botID);
        if (bot != null)
        {
            bot.Difficulty = difficulty;
            bot.Aggressiveness = aggressiveness;
            bot.Defensive = defensive;
            bot.FarmingPriority = farmingPriority;
            bot.RoamingPriority = roamingPriority;
            
            dataManager.CreateBotModeEvent("bot_update", "system", "", "更新AI机器人: " + bot.BotName);
            dataManager.SaveBotModeData();
            Debug.Log("更新AI机器人成功: " + bot.BotName);
        }
    }

    public string CreateTutorial(string tutorialName, string description, string category, int order, int requiredLevel, int rewardExperience, int rewardGold)
    {
        Tutorial tutorial = new Tutorial(tutorialName, description, category, order);
        tutorial.RequiredLevel = requiredLevel;
        tutorial.RewardExperience = rewardExperience;
        tutorial.RewardGold = rewardGold;
        botModeData.AvailableTutorials.Add(tutorial);
        
        dataManager.CreateBotModeEvent("tutorial_create", "system", "", "创建教程: " + tutorialName);
        dataManager.SaveBotModeData();
        Debug.Log("创建教程成功: " + tutorialName);
        return tutorial.TutorialID;
    }

    public void AddTutorialStep(string tutorialID, int stepOrder, string stepTitle, string stepDescription, string stepType, string targetObject, float duration)
    {
        Tutorial tutorial = botModeData.AvailableTutorials.Find(t => t.TutorialID == tutorialID);
        if (tutorial != null)
        {
            TutorialStep step = new TutorialStep(stepOrder, stepTitle, stepDescription, stepType, targetObject, duration);
            tutorial.Steps.Add(step);
            
            dataManager.CreateBotModeEvent("tutorial_step_add", "system", "", "添加教程步骤: " + stepTitle);
            dataManager.SaveBotModeData();
            Debug.Log("添加教程步骤成功: " + stepTitle);
        }
    }

    public string StartBotMatch(string matchType, string difficulty, List<string> playerTeamBotIDs, List<string> enemyTeamBotIDs, string mapName, bool isTutorial)
    {
        BotMatch match = new BotMatch(matchType, difficulty, mapName, isTutorial);
        match.PlayerTeamBotIDs = playerTeamBotIDs;
        match.EnemyTeamBotIDs = enemyTeamBotIDs;
        match.MatchStatus = "ongoing";
        botModeData.BotMatches.Add(match);
        
        dataManager.CreateBotModeEvent("match_start", "system", match.MatchID, "开始人机对战: " + difficulty);
        dataManager.SaveBotModeData();
        Debug.Log("开始人机对战成功: " + difficulty);
        return match.MatchID;
    }

    public void EndBotMatch(string matchID, string winnerTeam, float matchDuration)
    {
        BotMatch match = botModeData.BotMatches.Find(m => m.MatchID == matchID);
        if (match != null && match.MatchStatus == "ongoing")
        {
            match.MatchStatus = "completed";
            match.WinnerTeam = winnerTeam;
            match.MatchDuration = matchDuration;
            match.MatchEndTime = DateTime.Now;
            
            dataManager.CreateBotModeEvent("match_end", "system", matchID, "结束人机对战: " + winnerTeam);
            dataManager.SaveBotModeData();
            Debug.Log("结束人机对战成功: " + winnerTeam);
        }
    }

    public void StartTutorial(string playerID, string tutorialID)
    {
        if (!botModeData.PlayerTutorialProgress.ContainsKey(playerID))
        {
            InitializePlayerTutorialProgress(playerID);
        }
        
        PlayerTutorialProgress progress = botModeData.PlayerTutorialProgress[playerID];
        string key = tutorialID + "_current";
        progress.TutorialSteps[key] = 0;
        
        dataManager.CreateBotModeEvent("tutorial_start", playerID, "", "开始教程: " + tutorialID);
        dataManager.SaveBotModeData();
        Debug.Log("开始教程成功: " + tutorialID);
    }

    public void UpdateTutorialProgress(string playerID, string tutorialID, int currentStep)
    {
        if (!botModeData.PlayerTutorialProgress.ContainsKey(playerID))
        {
            InitializePlayerTutorialProgress(playerID);
        }
        
        PlayerTutorialProgress progress = botModeData.PlayerTutorialProgress[playerID];
        string key = tutorialID + "_current";
        progress.TutorialSteps[key] = currentStep;
        
        dataManager.SaveBotModeData();
    }

    public void CompleteTutorial(string playerID, string tutorialID)
    {
        if (!botModeData.PlayerTutorialProgress.ContainsKey(playerID))
        {
            InitializePlayerTutorialProgress(playerID);
        }
        
        Tutorial tutorial = botModeData.AvailableTutorials.Find(t => t.TutorialID == tutorialID);
        if (tutorial == null)
        {
            return;
        }
        
        PlayerTutorialProgress progress = botModeData.PlayerTutorialProgress[playerID];
        progress.CompletedTutorials[tutorialID] = true;
        progress.TotalTutorialsCompleted++;
        progress.LastTutorialTime = DateTime.Now;
        
        tutorial.IsCompleted = true;
        
        dataManager.CreateBotModeEvent("tutorial_complete", playerID, "", "完成教程: " + tutorial.TutorialName);
        dataManager.SaveBotModeData();
        Debug.Log("完成教程成功: " + tutorial.TutorialName);
    }

    public List<AIBot> GetAvailableBots()
    {
        return botModeData.AvailableBots.FindAll(b => b.IsAvailable);
    }

    public List<AIBot> GetBotsByDifficulty(string difficulty)
    {
        return botModeData.AvailableBots.FindAll(b => b.Difficulty == difficulty && b.IsAvailable);
    }

    public AIBot GetBot(string botID)
    {
        return botModeData.AvailableBots.Find(b => b.BotID == botID);
    }

    public List<Tutorial> GetAvailableTutorials()
    {
        return botModeData.AvailableTutorials;
    }

    public List<Tutorial> GetTutorialsByCategory(string category)
    {
        return botModeData.AvailableTutorials.FindAll(t => t.Category == category);
    }

    public Tutorial GetTutorial(string tutorialID)
    {
        return botModeData.AvailableTutorials.Find(t => t.TutorialID == tutorialID);
    }

    public bool IsTutorialCompleted(string playerID, string tutorialID)
    {
        if (!botModeData.PlayerTutorialProgress.ContainsKey(playerID))
        {
            return false;
        }
        return botModeData.PlayerTutorialProgress[playerID].CompletedTutorials.ContainsKey(tutorialID) && botModeData.PlayerTutorialProgress[playerID].CompletedTutorials[tutorialID];
    }

    public int GetTutorialProgress(string playerID, string tutorialID)
    {
        if (!botModeData.PlayerTutorialProgress.ContainsKey(playerID))
        {
            return 0;
        }
        
        PlayerTutorialProgress progress = botModeData.PlayerTutorialProgress[playerID];
        string key = tutorialID + "_current";
        if (progress.TutorialSteps.ContainsKey(key))
        {
            return progress.TutorialSteps[key];
        }
        return 0;
    }

    public PlayerTutorialProgress GetPlayerTutorialProgress(string playerID)
    {
        if (!botModeData.PlayerTutorialProgress.ContainsKey(playerID))
        {
            InitializePlayerTutorialProgress(playerID);
        }
        return botModeData.PlayerTutorialProgress[playerID];
    }

    public BotMatch GetBotMatch(string matchID)
    {
        return botModeData.BotMatches.Find(m => m.MatchID == matchID);
    }

    public List<BotMatch> GetCompletedBotMatches()
    {
        return botModeData.BotMatches.FindAll(m => m.MatchStatus == "completed");
    }

    public List<BotMatch> GetPlayerBotMatches(string playerID)
    {
        return botModeData.BotMatches.FindAll(m => m.MatchStatus == "completed");
    }

    public List<string> GetDifficulties()
    {
        return botModeData.Difficulties;
    }

    public List<string> GetMatchTypes()
    {
        return botModeData.MatchTypes;
    }

    public List<string> GetMaps()
    {
        return botModeData.Maps;
    }

    public void AddDifficulty(string difficulty)
    {
        if (!botModeData.Difficulties.Contains(difficulty))
        {
            botModeData.Difficulties.Add(difficulty);
            dataManager.SaveBotModeData();
            Debug.Log("添加难度等级成功: " + difficulty);
        }
    }

    public void RemoveDifficulty(string difficulty)
    {
        if (botModeData.Difficulties.Contains(difficulty))
        {
            botModeData.Difficulties.Remove(difficulty);
            dataManager.SaveBotModeData();
            Debug.Log("删除难度等级成功: " + difficulty);
        }
    }

    public void AddMap(string mapName)
    {
        if (!botModeData.Maps.Contains(mapName))
        {
            botModeData.Maps.Add(mapName);
            dataManager.SaveBotModeData();
            Debug.Log("添加地图成功: " + mapName);
        }
    }

    public void RemoveMap(string mapName)
    {
        if (botModeData.Maps.Contains(mapName))
        {
            botModeData.Maps.Remove(mapName);
            dataManager.SaveBotModeData();
            Debug.Log("删除地图成功: " + mapName);
        }
    }

    public void CleanupOldMatches(int days = 7)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<BotMatch> oldMatches = botModeData.BotMatches.FindAll(m => m.MatchEndTime < cutoffDate && m.MatchStatus == "completed");
        foreach (BotMatch match in oldMatches)
        {
            botModeData.BotMatches.Remove(match);
        }
        
        if (oldMatches.Count > 0)
        {
            dataManager.CreateBotModeEvent("match_cleanup", "system", "", "清理旧人机对战: " + oldMatches.Count);
            dataManager.SaveBotModeData();
            Debug.Log("清理旧人机对战成功: " + oldMatches.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveBotModeData();
    }

    public void LoadData()
    {
        dataManager.LoadBotModeData();
    }

    public List<BotModeEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
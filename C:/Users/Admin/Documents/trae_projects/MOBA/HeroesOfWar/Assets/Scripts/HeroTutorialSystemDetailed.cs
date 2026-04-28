using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class HeroSkill
{
    public string SkillID;
    public string SkillName;
    public string SkillDescription;
    public string SkillType;
    public float Cooldown;
    public float ManaCost;
    public string SkillIcon;
    public string SkillVideoURL;
    public string SkillTips;

    public HeroSkill(string skillID, string skillName, string skillDescription, string skillType, float cooldown, float manaCost, string skillIcon, string skillVideoURL, string skillTips)
    {
        SkillID = skillID;
        SkillName = skillName;
        SkillDescription = skillDescription;
        SkillType = skillType;
        Cooldown = cooldown;
        ManaCost = manaCost;
        SkillIcon = skillIcon;
        SkillVideoURL = skillVideoURL;
        SkillTips = skillTips;
    }
}

[Serializable]
public class HeroSkillTutorial
{
    public string TutorialID;
    public string HeroID;
    public string HeroName;
    public List<HeroSkill> Skills;
    public string SkillComboGuide;
    public string SkillUsageTips;
    public string DifficultyLevel;
    public DateTime LastUpdated;

    public HeroSkillTutorial(string heroID, string heroName, string difficultyLevel)
    {
        TutorialID = "tutorial_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HeroID = heroID;
        HeroName = heroName;
        Skills = new List<HeroSkill>();
        SkillComboGuide = "";
        SkillUsageTips = "";
        DifficultyLevel = difficultyLevel;
        LastUpdated = DateTime.Now;
    }
}

[Serializable]
public class ItemBuild
{
    public string BuildID;
    public string BuildName;
    public List<string> Items;
    public string BuildType;
    public string Situation;
    public int Popularity;
    public float WinRate;

    public ItemBuild(string buildName, string buildType, string situation)
    {
        BuildID = "build_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BuildName = buildName;
        Items = new List<string>();
        BuildType = buildType;
        Situation = situation;
        Popularity = 0;
        WinRate = 0f;
    }
}

[Serializable]
public class RecommendedBuild
{
    public string BuildSetID;
    public string HeroID;
    public string HeroName;
    public List<ItemBuild> ItemBuilds;
    public string StartingItems;
    public string CoreItems;
    public string SituationalItems;
    public DateTime LastUpdated;

    public RecommendedBuild(string heroID, string heroName)
    {
        BuildSetID = "buildset_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HeroID = heroID;
        HeroName = heroName;
        ItemBuilds = new List<ItemBuild>();
        StartingItems = "";
        CoreItems = "";
        SituationalItems = "";
        LastUpdated = DateTime.Now;
    }
}

[Serializable]
public class ComboStep
{
    public int StepOrder;
    public string SkillID;
    public string SkillName;
    public string Description;
    public float Timing;
    public string Notes;

    public ComboStep(int stepOrder, string skillID, string skillName, string description, float timing, string notes)
    {
        StepOrder = stepOrder;
        SkillID = skillID;
        SkillName = skillName;
        Description = description;
        Timing = timing;
        Notes = notes;
    }
}

[Serializable]
public class ComboGuide
{
    public string ComboID;
    public string HeroID;
    public string HeroName;
    public string ComboName;
    public string ComboDescription;
    public List<ComboStep> ComboSteps;
    public string VideoURL;
    public string Difficulty;
    public string Situation;
    public int Popularity;

    public ComboGuide(string heroID, string heroName, string comboName, string comboDescription, string difficulty, string situation)
    {
        ComboID = "combo_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HeroID = heroID;
        HeroName = heroName;
        ComboName = comboName;
        ComboDescription = comboDescription;
        ComboSteps = new List<ComboStep>();
        VideoURL = "";
        Difficulty = difficulty;
        Situation = situation;
        Popularity = 0;
    }
}

[Serializable]
public class HeroTutorialData
{
    public string HeroID;
    public string HeroName;
    public HeroSkillTutorial SkillTutorial;
    public RecommendedBuild RecommendedBuild;
    public List<ComboGuide> ComboGuides;
    public string HeroLore;
    public string PlayStyle;
    public string CounterTips;
    public string SynergyTips;
    public DateTime LastUpdated;

    public HeroTutorialData(string heroID, string heroName)
    {
        HeroID = heroID;
        HeroName = heroName;
        SkillTutorial = new HeroSkillTutorial(heroID, heroName, "Medium");
        RecommendedBuild = new RecommendedBuild(heroID, heroName);
        ComboGuides = new List<ComboGuide>();
        HeroLore = "";
        PlayStyle = "";
        CounterTips = "";
        SynergyTips = "";
        LastUpdated = DateTime.Now;
    }
}

[Serializable]
public class PlayerTutorialProgress
{
    public string PlayerID;
    public Dictionary<string, bool> CompletedTutorials;
    public Dictionary<string, int> SkillMasteryLevels;
    public Dictionary<string, int> ComboUsageCount;
    public Dictionary<string, string> FavoriteBuilds;
    public int TotalTutorialsCompleted;
    public DateTime LastLearningTime;

    public PlayerTutorialProgress(string playerID)
    {
        PlayerID = playerID;
        CompletedTutorials = new Dictionary<string, bool>();
        SkillMasteryLevels = new Dictionary<string, int>();
        ComboUsageCount = new Dictionary<string, int>();
        FavoriteBuilds = new Dictionary<string, string>();
        TotalTutorialsCompleted = 0;
        LastLearningTime = DateTime.MinValue;
    }
}

[Serializable]
public class HeroTutorialSystemData
{
    public Dictionary<string, HeroTutorialData> HeroTutorials;
    public Dictionary<string, PlayerTutorialProgress> PlayerProgress;
    public List<string> HeroCategories;
    public int MaxComboGuidesPerHero;
    public int MaxBuildsPerHero;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public HeroTutorialSystemData()
    {
        HeroTutorials = new Dictionary<string, HeroTutorialData>();
        PlayerProgress = new Dictionary<string, PlayerTutorialProgress>();
        HeroCategories = new List<string> { "战士", "法师", "射手", "刺客", "辅助", "坦克" };
        MaxComboGuidesPerHero = 10;
        MaxBuildsPerHero = 5;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddHeroTutorialData(string heroID, HeroTutorialData tutorialData)
    {
        HeroTutorials[heroID] = tutorialData;
    }

    public void AddPlayerTutorialProgress(string playerID, PlayerTutorialProgress progress)
    {
        PlayerProgress[playerID] = progress;
    }
}

[Serializable]
public class TutorialEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string HeroID;
    public string EventData;

    public TutorialEvent(string eventID, string eventType, string playerID, string heroID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        HeroID = heroID;
        EventData = eventData;
    }
}

public class HeroTutorialSystemDataManager
{
    private static HeroTutorialSystemDataManager _instance;
    public static HeroTutorialSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HeroTutorialSystemDataManager();
            }
            return _instance;
        }
    }

    public HeroTutorialSystemData tutorialData;
    private List<TutorialEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private HeroTutorialSystemDataManager()
    {
        tutorialData = new HeroTutorialSystemData();
        recentEvents = new List<TutorialEvent>();
        LoadTutorialData();
    }

    public void SaveTutorialData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "HeroTutorialSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tutorialData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存英雄教学系统数据失败: " + e.Message);
        }
    }

    public void LoadTutorialData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "HeroTutorialSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    tutorialData = (HeroTutorialSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载英雄教学系统数据失败: " + e.Message);
            tutorialData = new HeroTutorialSystemData();
        }
    }

    public void CreateTutorialEvent(string eventType, string playerID, string heroID, string eventData)
    {
        string eventID = "tutorial_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TutorialEvent tutorialEvent = new TutorialEvent(eventID, eventType, playerID, heroID, eventData);
        recentEvents.Add(tutorialEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<TutorialEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
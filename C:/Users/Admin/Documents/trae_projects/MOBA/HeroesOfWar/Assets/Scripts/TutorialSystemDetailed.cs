using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class TutorialStep
{
    public string StepID;
    public string StepTitle;
    public string StepDescription;
    public int StepType;
    public string TargetUIElement;
    public string TargetPosition;
    public string HighlightArea;
    public string ButtonAction;
    public int Duration;
    public bool RequireAction;
    public string NextStepCondition;
    public string SkipCondition;
    public int DisplayOrder;

    public TutorialStep(string stepID, string stepTitle, string stepDescription, int stepType, int displayOrder)
    {
        StepID = stepID;
        StepTitle = stepTitle;
        StepDescription = stepDescription;
        StepType = stepType;
        TargetUIElement = "";
        TargetPosition = "";
        HighlightArea = "";
        ButtonAction = "";
        Duration = 0;
        RequireAction = false;
        NextStepCondition = "";
        SkipCondition = "";
        DisplayOrder = displayOrder;
    }
}

[Serializable]
public class Tutorial
{
    public string TutorialID;
    public string TutorialName;
    public string Description;
    public int TutorialType;
    public List<TutorialStep> Steps;
    public int StartStepIndex;
    public bool IsCompleted;
    public bool CanSkip;
    public int UnlockCondition;
    public string UnlockParam;
    public int Priority;
    public DateTime? LastCompletedTime;

    public Tutorial(string tutorialID, string tutorialName, string description, int tutorialType)
    {
        TutorialID = tutorialID;
        TutorialName = tutorialName;
        Description = description;
        TutorialType = tutorialType;
        Steps = new List<TutorialStep>();
        StartStepIndex = 0;
        IsCompleted = false;
        CanSkip = false;
        UnlockCondition = 0;
        UnlockParam = "";
        Priority = 0;
        LastCompletedTime = null;
    }

    public void AddStep(TutorialStep step)
    {
        Steps.Add(step);
    }
}

[Serializable]
public class PlayerTutorialProgress
{
    public string PlayerID;
    public string CurrentTutorialID;
    public int CurrentStepIndex;
    public List<string> CompletedTutorialIDs;
    public List<string> SkippedTutorialIDs;
    public Dictionary<string, int> StepProgress;
    public Dictionary<string, bool> TutorialFlags;
    public DateTime TutorialStartTime;
    public DateTime LastStepTime;

    public PlayerTutorialProgress(string playerID)
    {
        PlayerID = playerID;
        CurrentTutorialID = "";
        CurrentStepIndex = 0;
        CompletedTutorialIDs = new List<string>();
        SkippedTutorialIDs = new List<string>();
        StepProgress = new Dictionary<string, int>();
        TutorialFlags = new Dictionary<string, bool>();
        TutorialStartTime = DateTime.Now;
        LastStepTime = DateTime.Now;
    }
}

[Serializable]
public class GuideArrow
{
    public string ArrowID;
    public string ArrowName;
    public string TargetElement;
    public string ArrowPosition;
    public string ArrowAnimation;
    public int ArrowPriority;
    public bool IsActive;

    public GuideArrow(string arrowID, string arrowName, string targetElement, string arrowPosition, int priority)
    {
        ArrowID = arrowID;
        ArrowName = arrowName;
        TargetElement = targetElement;
        ArrowPosition = arrowPosition;
        ArrowAnimation = "pulse";
        ArrowPriority = priority;
        IsActive = false;
    }
}

[Serializable]
public class TutorialSystemData
{
    public List<Tutorial> AllTutorials;
    public Dictionary<string, List<Tutorial>> TutorialsByType;
    public Dictionary<string, PlayerTutorialProgress> PlayerProgress;
    public List<GuideArrow> GuideArrows;
    public Dictionary<string, string> UIElementPositions;
    public List<string> AvailableTutorialQueue;
    public DateTime LastCleanupTime;

    public TutorialSystemData()
    {
        AllTutorials = new List<Tutorial>();
        TutorialsByType = new Dictionary<string, List<Tutorial>>();
        PlayerProgress = new Dictionary<string, PlayerTutorialProgress>();
        GuideArrows = new List<GuideArrow>();
        UIElementPositions = new Dictionary<string, string>();
        AvailableTutorialQueue = new List<string>();
        LastCleanupTime = DateTime.Now;
        InitializeDefaultTutorials();
    }

    private void InitializeDefaultTutorials()
    {
        Tutorial movementTutorial = new Tutorial("tutorial_001", "基础移动", "学习如何移动角色", 1);
        TutorialStep step1 = new TutorialStep("step_001", "虚拟摇杆", "拖动左下角的虚拟摇杆来移动", 1, 1);
        step1.TargetUIElement = "joystick";
        step1.RequireAction = true;
        step1.Duration = 5;
        movementTutorial.AddStep(step1);
        TutorialStep step2 = new TutorialStep("step_002", "确认移动", "现在尝试自己移动到防御塔附近", 1, 2);
        step2.RequireAction = true;
        step2.NextStepCondition = "reach_position";
        movementTutorial.AddStep(step2);
        AllTutorials.Add(movementTutorial);

        Tutorial attackTutorial = new Tutorial("tutorial_002", "基础攻击", "学习如何攻击敌人", 1);
        TutorialStep atkStep1 = new TutorialStep("step_003", "普通攻击", "点击右下角的普通攻击按钮", 2, 1);
        atkStep1.TargetUIElement = "attack_button";
        atkStep1.RequireAction = true;
        atkStep1.Duration = 3;
        attackTutorial.AddStep(atkStep1);
        AllTutorials.Add(attackTutorial);

        Tutorial skillTutorial = new Tutorial("tutorial_003", "技能使用", "学习如何使用技能", 1);
        TutorialStep skillStep1 = new TutorialStep("step_004", "技能1", "点击技能1按钮释放技能", 3, 1);
        skillStep1.TargetUIElement = "skill_1";
        skillStep1.RequireAction = true;
        skillStep1.Duration = 3;
        skillTutorial.AddStep(skillStep1);
        TutorialStep skillStep2 = new TutorialStep("step_005", "技能2", "点击技能2按钮释放技能", 3, 2);
        skillStep2.TargetUIElement = "skill_2";
        skillStep2.RequireAction = true;
        skillStep2.Duration = 3;
        skillTutorial.AddStep(skillStep2);
        AllTutorials.Add(skillTutorial);
    }

    public void AddTutorial(Tutorial tutorial)
    {
        AllTutorials.Add(tutorial);
        string typeKey = tutorial.TutorialType.ToString();
        if (!TutorialsByType.ContainsKey(typeKey))
        {
            TutorialsByType[typeKey] = new List<Tutorial>();
        }
        TutorialsByType[typeKey].Add(tutorial);
    }

    public void AddPlayerProgress(string playerID, PlayerTutorialProgress progress)
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
    public string TutorialID;
    public string StepID;
    public string EventData;

    public TutorialEvent(string eventID, string eventType, string playerID, string tutorialID, string stepID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        TutorialID = tutorialID;
        StepID = stepID;
        EventData = eventData;
    }
}

public class TutorialSystemDataManager
{
    private static TutorialSystemDataManager _instance;
    public static TutorialSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TutorialSystemDataManager();
            }
            return _instance;
        }
    }

    public TutorialSystemData tutorialData;
    private List<TutorialEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private TutorialSystemDataManager()
    {
        tutorialData = new TutorialSystemData();
        recentEvents = new List<TutorialEvent>();
        LoadTutorialData();
    }

    public void SaveTutorialData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TutorialSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tutorialData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存教程系统数据失败: " + e.Message);
        }
    }

    public void LoadTutorialData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TutorialSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    tutorialData = (TutorialSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载教程系统数据失败: " + e.Message);
            tutorialData = new TutorialSystemData();
        }
    }

    public void CreateTutorialEvent(string eventType, string playerID, string tutorialID, string stepID, string eventData)
    {
        string eventID = "tutorial_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TutorialEvent tutorialEvent = new TutorialEvent(eventID, eventType, playerID, tutorialID, stepID, eventData);
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
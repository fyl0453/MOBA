using System;
using System.Collections.Generic;

public class TutorialSystemDetailedManager
{
    private static TutorialSystemDetailedManager _instance;
    public static TutorialSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TutorialSystemDetailedManager();
            }
            return _instance;
        }
    }

    private TutorialSystemData tutorialData;
    private TutorialSystemDataManager dataManager;

    private TutorialSystemDetailedManager()
    {
        dataManager = TutorialSystemDataManager.Instance;
        tutorialData = dataManager.tutorialData;
    }

    public void InitializePlayerProgress(string playerID)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            tutorialData.PlayerProgress[playerID] = new PlayerTutorialProgress(playerID);
            dataManager.CreateTutorialEvent("progress_init", playerID, "", "", "初始化教程进度");
            dataManager.SaveTutorialData();
            Debug.Log("初始化玩家教程进度: " + playerID);
        }
    }

    public void CreateTutorial(string tutorialName, string description, int tutorialType, bool canSkip, int priority)
    {
        string tutorialID = "tutorial_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Tutorial tutorial = new Tutorial(tutorialID, tutorialName, description, tutorialType);
        tutorial.CanSkip = canSkip;
        tutorial.Priority = priority;
        tutorialData.AddTutorial(tutorial);
        dataManager.CreateTutorialEvent("tutorial_create", "", tutorialID, "", "创建教程: " + tutorialName);
        dataManager.SaveTutorialData();
        Debug.Log("创建教程成功: " + tutorialName);
    }

    public void AddTutorialStep(string tutorialID, string stepTitle, string stepDescription, int stepType, int displayOrder, string targetUIElement = "", bool requireAction = false, int duration = 0)
    {
        Tutorial tutorial = tutorialData.AllTutorials.Find(t => t.TutorialID == tutorialID);
        if (tutorial != null)
        {
            TutorialStep step = new TutorialStep("step_" + System.Guid.NewGuid().ToString().Substring(0, 8), stepTitle, stepDescription, stepType, displayOrder);
            step.TargetUIElement = targetUIElement;
            step.RequireAction = requireAction;
            step.Duration = duration;
            tutorial.AddStep(step);
            dataManager.SaveTutorialData();
            Debug.Log("添加教程步骤: " + stepTitle);
        }
    }

    public void StartTutorial(string playerID, string tutorialID)
    {
        Tutorial tutorial = tutorialData.AllTutorials.Find(t => t.TutorialID == tutorialID);
        if (tutorial == null)
        {
            Debug.LogError("教程不存在: " + tutorialID);
            return;
        }

        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            InitializePlayerProgress(playerID);
        }

        PlayerTutorialProgress progress = tutorialData.PlayerProgress[playerID];
        if (progress.CompletedTutorialIDs.Contains(tutorialID) || progress.SkippedTutorialIDs.Contains(tutorialID))
        {
            Debug.LogWarning("教程已完成或已跳过");
            return;
        }

        progress.CurrentTutorialID = tutorialID;
        progress.CurrentStepIndex = tutorial.StartStepIndex;
        progress.TutorialStartTime = DateTime.Now;
        progress.LastStepTime = DateTime.Now;

        if (progress.StepProgress.ContainsKey(tutorialID))
        {
            progress.StepProgress[tutorialID] = progress.CurrentStepIndex;
        }
        else
        {
            progress.StepProgress[tutorialID] = progress.CurrentStepIndex;
        }

        TutorialStep currentStep = tutorial.Steps[progress.CurrentStepIndex];
        dataManager.CreateTutorialEvent("tutorial_start", playerID, tutorialID, currentStep.StepID, "开始教程: " + tutorial.TutorialName);
        dataManager.SaveTutorialData();
        Debug.Log("开始教程: " + tutorial.TutorialName);
    }

    public void CompleteCurrentStep(string playerID)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            Debug.LogError("玩家教程进度不存在");
            return;
        }

        PlayerTutorialProgress progress = tutorialData.PlayerProgress[playerID];
        Tutorial tutorial = tutorialData.AllTutorials.Find(t => t.TutorialID == progress.CurrentTutorialID);
        if (tutorial == null)
        {
            Debug.LogError("当前教程不存在");
            return;
        }

        TutorialStep currentStep = tutorial.Steps[progress.CurrentStepIndex];
        progress.CurrentStepIndex++;
        progress.LastStepTime = DateTime.Now;

        if (progress.StepProgress.ContainsKey(tutorial.TutorialID))
        {
            progress.StepProgress[tutorial.TutorialID] = progress.CurrentStepIndex;
        }

        if (progress.CurrentStepIndex >= tutorial.Steps.Count)
        {
            CompleteTutorial(playerID, tutorial.TutorialID);
        }
        else
        {
            TutorialStep nextStep = tutorial.Steps[progress.CurrentStepIndex];
            dataManager.CreateTutorialEvent("step_complete", playerID, tutorial.TutorialID, currentStep.StepID, "完成步骤: " + currentStep.StepTitle);
            dataManager.SaveTutorialData();
            Debug.Log("完成步骤: " + currentStep.StepTitle);
        }
    }

    public void CompleteTutorial(string playerID, string tutorialID)
    {
        Tutorial tutorial = tutorialData.AllTutorials.Find(t => t.TutorialID == tutorialID);
        if (tutorial == null)
        {
            Debug.LogError("教程不存在: " + tutorialID);
            return;
        }

        tutorial.IsCompleted = true;
        tutorial.LastCompletedTime = DateTime.Now;

        if (tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            PlayerTutorialProgress progress = tutorialData.PlayerProgress[playerID];
            if (!progress.CompletedTutorialIDs.Contains(tutorialID))
            {
                progress.CompletedTutorialIDs.Add(tutorialID);
            }
            progress.CurrentTutorialID = "";
            progress.CurrentStepIndex = 0;
        }

        dataManager.CreateTutorialEvent("tutorial_complete", playerID, tutorialID, "", "完成教程: " + tutorial.TutorialName);
        dataManager.SaveTutorialData();
        Debug.Log("完成教程: " + tutorial.TutorialName);
    }

    public void SkipTutorial(string playerID, string tutorialID)
    {
        Tutorial tutorial = tutorialData.AllTutorials.Find(t => t.TutorialID == tutorialID);
        if (tutorial == null)
        {
            Debug.LogError("教程不存在: " + tutorialID);
            return;
        }

        if (!tutorial.CanSkip)
        {
            Debug.LogWarning("该教程不可跳过");
            return;
        }

        tutorial.IsCompleted = true;
        if (tutorial.LastCompletedTime == null)
        {
            tutorial.LastCompletedTime = DateTime.Now;
        }

        if (tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            PlayerTutorialProgress progress = tutorialData.PlayerProgress[playerID];
            if (!progress.SkippedTutorialIDs.Contains(tutorialID))
            {
                progress.SkippedTutorialIDs.Add(tutorialID);
            }
            progress.CurrentTutorialID = "";
            progress.CurrentStepIndex = 0;
        }

        dataManager.CreateTutorialEvent("tutorial_skip", playerID, tutorialID, "", "跳过教程: " + tutorial.TutorialName);
        dataManager.SaveTutorialData();
        Debug.Log("跳过教程: " + tutorial.TutorialName);
    }

    public Tutorial GetTutorial(string tutorialID)
    {
        return tutorialData.AllTutorials.Find(t => t.TutorialID == tutorialID);
    }

    public TutorialStep GetCurrentStep(string playerID)
    {
        if (tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            PlayerTutorialProgress progress = tutorialData.PlayerProgress[playerID];
            Tutorial tutorial = tutorialData.AllTutorials.Find(t => t.TutorialID == progress.CurrentTutorialID);
            if (tutorial != null && progress.CurrentStepIndex < tutorial.Steps.Count)
            {
                return tutorial.Steps[progress.CurrentStepIndex];
            }
        }
        return null;
    }

    public PlayerTutorialProgress GetPlayerProgress(string playerID)
    {
        if (tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            return tutorialData.PlayerProgress[playerID];
        }
        return null;
    }

    public List<Tutorial> GetAvailableTutorials(string playerID)
    {
        List<Tutorial> available = new List<Tutorial>();
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            InitializePlayerProgress(playerID);
        }

        PlayerTutorialProgress progress = tutorialData.PlayerProgress[playerID];
        foreach (Tutorial tutorial in tutorialData.AllTutorials)
        {
            if (!progress.CompletedTutorialIDs.Contains(tutorial.TutorialID) &&
                !progress.SkippedTutorialIDs.Contains(tutorial.TutorialID) &&
                tutorial.Priority > 0)
            {
                available.Add(tutorial);
            }
        }
        available.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        return available;
    }

    public List<Tutorial> GetCompletedTutorials(string playerID)
    {
        if (tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            List<Tutorial> completed = new List<Tutorial>();
            foreach (string tutorialID in tutorialData.PlayerProgress[playerID].CompletedTutorialIDs)
            {
                Tutorial tutorial = tutorialData.AllTutorials.Find(t => t.TutorialID == tutorialID);
                if (tutorial != null)
                {
                    completed.Add(tutorial);
                }
            }
            return completed;
        }
        return new List<Tutorial>();
    }

    public List<Tutorial> GetTutorialsByType(int tutorialType)
    {
        string typeKey = tutorialType.ToString();
        if (tutorialData.TutorialsByType.ContainsKey(typeKey))
        {
            return tutorialData.TutorialsByType[typeKey];
        }
        return new List<Tutorial>();
    }

    public bool IsTutorialCompleted(string playerID, string tutorialID)
    {
        if (tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            return tutorialData.PlayerProgress[playerID].CompletedTutorialIDs.Contains(tutorialID);
        }
        return false;
    }

    public bool IsTutorialSkipped(string playerID, string tutorialID)
    {
        if (tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            return tutorialData.PlayerProgress[playerID].SkippedTutorialIDs.Contains(tutorialID);
        }
        return false;
    }

    public void SetTutorialFlag(string playerID, string flagName, bool value)
    {
        if (tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            tutorialData.PlayerProgress[playerID].TutorialFlags[flagName] = value;
            dataManager.SaveTutorialData();
        }
    }

    public bool GetTutorialFlag(string playerID, string flagName)
    {
        if (tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            if (tutorialData.PlayerProgress[playerID].TutorialFlags.ContainsKey(flagName))
            {
                return tutorialData.PlayerProgress[playerID].TutorialFlags[flagName];
            }
        }
        return false;
    }

    public void CreateGuideArrow(string arrowName, string targetElement, string arrowPosition, int priority)
    {
        string arrowID = "arrow_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuideArrow arrow = new GuideArrow(arrowID, arrowName, targetElement, arrowPosition, priority);
        tutorialData.GuideArrows.Add(arrow);
        dataManager.SaveTutorialData();
        Debug.Log("创建引导箭头: " + arrowName);
    }

    public void ShowGuideArrow(string arrowID)
    {
        GuideArrow arrow = tutorialData.GuideArrows.Find(a => a.ArrowID == arrowID);
        if (arrow != null)
        {
            arrow.IsActive = true;
            dataManager.SaveTutorialData();
            Debug.Log("显示引导箭头: " + arrow.ArrowName);
        }
    }

    public void HideGuideArrow(string arrowID)
    {
        GuideArrow arrow = tutorialData.GuideArrows.Find(a => a.ArrowID == arrowID);
        if (arrow != null)
        {
            arrow.IsActive = false;
            dataManager.SaveTutorialData();
            Debug.Log("隐藏引导箭头: " + arrow.ArrowName);
        }
    }

    public void HideAllGuideArrows()
    {
        foreach (GuideArrow arrow in tutorialData.GuideArrows)
        {
            arrow.IsActive = false;
        }
        dataManager.SaveTutorialData();
        Debug.Log("隐藏所有引导箭头");
    }

    public GuideArrow GetActiveArrow()
    {
        return tutorialData.GuideArrows.Find(a => a.IsActive);
    }

    public int GetTutorialCompletionPercentage(string playerID)
    {
        if (tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            PlayerTutorialProgress progress = tutorialData.PlayerProgress[playerID];
            int completed = progress.CompletedTutorialIDs.Count;
            int total = tutorialData.AllTutorials.Count;
            if (total > 0)
            {
                return completed * 100 / total;
            }
        }
        return 0;
    }

    public List<TutorialEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveTutorialData();
    }

    public void LoadData()
    {
        dataManager.LoadTutorialData();
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TrainingUIManager : MonoBehaviour
{
    public static TrainingUIManager Instance { get; private set; }
    
    public Canvas trainingCanvas;
    public Text progressText;
    public Text totalStarsText;
    public ScrollRect scenariosScrollRect;
    public Transform scenariosContent;
    public Button closeButton;
    public GameObject scenarioItemPrefab;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        trainingCanvas.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseTrainingUI);
    }
    
    public void OpenTrainingUI()
    {
        trainingCanvas.gameObject.SetActive(true);
        UpdateProgress();
        UpdateScenariosList();
    }
    
    public void CloseTrainingUI()
    {
        trainingCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateProgress()
    {
        float progress = TrainingManager.Instance.GetOverallProgress();
        int stars = TrainingManager.Instance.GetTotalStars();
        
        progressText.text = $"总进度: {progress:F1}%";
        totalStarsText.text = $"星数: {stars}";
    }
    
    public void UpdateScenariosList()
    {
        foreach (Transform child in scenariosContent)
        {
            Destroy(child.gameObject);
        }
        
        List<TrainingScenario> scenarios = TrainingManager.Instance.GetUnlockedScenarios();
        
        foreach (TrainingScenario scenario in scenarios)
        {
            GameObject scenarioItem = Instantiate(scenarioItemPrefab, scenariosContent);
            
            Text[] texts = scenarioItem.GetComponentsInChildren<Text>();
            if (texts.Length >= 4)
            {
                texts[0].text = scenario.scenarioName;
                texts[1].text = scenario.description;
                texts[2].text = $"目标数: {scenario.objectives.Count}";
                texts[3].text = scenario.isUnlocked ? "已解锁" : "未解锁";
            }
            
            Button scenarioButton = scenarioItem.GetComponent<Button>();
            string scenarioID = scenario.scenarioID;
            scenarioButton.onClick.AddListener(() => OnScenarioClicked(scenarioID));
            
            if (!scenario.isUnlocked)
            {
                scenarioItem.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
        }
    }
    
    public void OnScenarioClicked(string scenarioID)
    {
        TrainingScenario scenario = TrainingManager.Instance.GetScenarioByID(scenarioID);
        if (scenario != null && scenario.isUnlocked)
        {
            ShowScenarioDetails(scenario);
        }
    }
    
    public void ShowScenarioDetails(TrainingScenario scenario)
    {
        Debug.Log($"=== {scenario.scenarioName} ===");
        Debug.Log($"描述: {scenario.description}");
        Debug.Log($"可用英雄: {string.Join(", ", scenario.availableHeroes)}");
        Debug.Log("目标:");
        foreach (TrainingObjective obj in scenario.objectives)
        {
            Debug.Log($"  - {obj.description} (要求: {obj.targetValue})");
        }
        
        ShowStartDialog(scenario);
    }
    
    public void ShowStartDialog(TrainingScenario scenario)
    {
        Debug.Log($"是否开始训练: {scenario.scenarioName}?");
    }
    
    public void StartScenario(string scenarioID)
    {
        TrainingManager.Instance.StartTraining(scenarioID);
    }
    
    public void ShowLessonDetails(string lessonID)
    {
        TrainingLesson lesson = TrainingManager.Instance.GetLessonByID(lessonID);
        if (lesson != null)
        {
            Debug.Log($"=== {lesson.lessonName} ===");
            Debug.Log($"描述: {lesson.description}");
            Debug.Log($"难度: {lesson.difficulty}");
            Debug.Log($"已完成: {lesson.isCompleted}");
            Debug.Log($"获得星数: {lesson.starsEarned}/{lesson.maxStars}");
        }
    }
    
    public void ShowTrainingTips()
    {
        Debug.Log("=== 训练小贴士 ===");
        Debug.Log("1. 熟悉英雄的技能组合");
        Debug.Log("2. 注意观察小地图");
        Debug.Log("3. 保持发育的同时支援队友");
        Debug.Log("4. 合理利用草丛隐蔽");
        Debug.Log("5. 推塔是获胜的关键");
    }
    
    public void ExitTraining()
    {
        TrainingManager.Instance.EndTraining(false);
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TrainingManager : MonoBehaviour
{
    public static TrainingManager Instance { get; private set; }
    
    public List<TrainingLesson> allLessons;
    public List<TrainingScenario> allScenarios;
    public TrainingProgress playerProgress;
    public TrainingScenario currentScenario;
    public bool isTrainingActive;
    
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
        LoadTrainingData();
        LoadTrainingProgress();
        
        if (allLessons.Count == 0)
        {
            InitializeDefaultLessons();
        }
        
        if (allScenarios.Count == 0)
        {
            InitializeDefaultScenarios();
        }
        
        if (playerProgress == null)
        {
            CreateNewProgress();
        }
    }
    
    private void InitializeDefaultLessons()
    {
        TrainingLesson lesson1 = new TrainingLesson("lesson_001", "基础移动", "学习如何移动角色", "Basic", 1);
        lesson1.maxStars = 3;
        allLessons.Add(lesson1);
        
        TrainingLesson lesson2 = new TrainingLesson("lesson_002", "技能释放", "学习释放技能", "Basic", 1);
        lesson2.maxStars = 3;
        allLessons.Add(lesson2);
        
        TrainingLesson lesson3 = new TrainingLesson("lesson_003", "攻击目标", "学习攻击敌人", "Combat", 2);
        lesson3.maxStars = 3;
        allLessons.Add(lesson3);
        
        TrainingLesson lesson4 = new TrainingLesson("lesson_004", "躲避技能", "学习躲避敌人技能", "Combat", 2);
        lesson4.maxStars = 3;
        allLessons.Add(lesson4);
        
        TrainingLesson lesson5 = new TrainingLesson("lesson_005", "连招技巧", "学习技能连招", "Advanced", 3);
        lesson5.maxStars = 3;
        allLessons.Add(lesson5);
        
        TrainingLesson lesson6 = new TrainingLesson("lesson_006", "草丛利用", "学习利用草丛隐蔽", "Advanced", 3);
        lesson6.maxStars = 3;
        allLessons.Add(lesson6);
        
        TrainingLesson lesson7 = new TrainingLesson("lesson_007", "推塔技巧", "学习推塔时机", "Objective", 3);
        lesson7.maxStars = 3;
        allLessons.Add(lesson7);
        
        TrainingLesson lesson8 = new TrainingLesson("lesson_008", "团战配合", "学习团战配合", "Objective", 3);
        lesson8.maxStars = 3;
        allLessons.Add(lesson8);
        
        SaveTrainingData();
    }
    
    private void InitializeDefaultScenarios()
    {
        TrainingScenario scenario1 = new TrainingScenario("scenario_001", "新手训练营");
        scenario1.description = "适合新手玩家的入门训练";
        scenario1.sceneName = "TrainingScene1";
        scenario1.isUnlocked = true;
        scenario1.objectives.Add(new TrainingObjective("obj_001", "击败所有敌人", "Kill", 5, 300, 1));
        scenario1.objectives.Add(new TrainingObjective("obj_002", "在60秒内完成", "Time", 1, 60, 2));
        scenario1.availableHeroes.Add("hero_001");
        scenario1.availableHeroes.Add("hero_002");
        scenario1.availableHeroes.Add("hero_003");
        allScenarios.Add(scenario1);
        
        TrainingScenario scenario2 = new TrainingScenario("scenario_002", "技能训练");
        scenario2.description = "专注于技能使用的训练";
        scenario2.sceneName = "TrainingScene2";
        scenario2.isUnlocked = false;
        scenario2.objectives.Add(new TrainingObjective("obj_003", "使用技能击败10个敌人", "SkillKill", 10, 300, 1));
        scenario2.objectives.Add(new TrainingObjective("obj_004", "不受到伤害", "NoDamage", 1, 300, 3));
        scenario2.availableHeroes.Add("hero_001");
        scenario2.availableHeroes.Add("hero_002");
        allScenarios.Add(scenario2);
        
        TrainingScenario scenario3 = new TrainingScenario("scenario_003", "推塔训练");
        scenario3.description = "学习推塔技巧的训练";
        scenario3.sceneName = "TrainingScene3";
        scenario3.isUnlocked = false;
        scenario3.objectives.Add(new TrainingObjective("obj_005", "摧毁3座防御塔", "DestroyTower", 3, 300, 1));
        scenario3.objectives.Add(new TrainingObjective("obj_006", "在90秒内完成", "Time", 1, 90, 2));
        scenario3.availableHeroes.Add("hero_001");
        allScenarios.Add(scenario3);
        
        TrainingScenario scenario4 = new TrainingScenario("scenario_004", "AI对战");
        scenario4.description = "与AI机器人对战";
        scenario4.sceneName = "TrainingScene4";
        scenario4.isUnlocked = false;
        scenario4.objectives.Add(new TrainingObjective("obj_007", "击败AI队伍", "Win", 1, 600, 1));
        scenario4.objectives.Add(new TrainingObjective("obj_008", "不死亡", "NoDeath", 1, 600, 3));
        scenario4.availableHeroes.Add("hero_001");
        scenario4.availableHeroes.Add("hero_002");
        scenario4.availableHeroes.Add("hero_003");
        scenario4.availableHeroes.Add("hero_004");
        allScenarios.Add(scenario4);
        
        SaveTrainingData();
    }
    
    private void CreateNewProgress()
    {
        playerProgress = new TrainingProgress("player_001");
        SaveTrainingProgress();
    }
    
    public void StartTraining(string scenarioID)
    {
        TrainingScenario scenario = GetScenarioByID(scenarioID);
        if (scenario != null && scenario.isUnlocked)
        {
            currentScenario = scenario;
            isTrainingActive = true;
            
            foreach (TrainingObjective obj in currentScenario.objectives)
            {
                obj.currentValue = 0;
                obj.isCompleted = false;
            }
            
            Debug.Log($"开始训练: {scenario.scenarioName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(scenario.sceneName);
        }
    }
    
    public void EndTraining(bool completed)
    {
        if (currentScenario == null) return;
        
        isTrainingActive = false;
        
        if (completed)
        {
            int totalStars = CalculateEarnedStars();
            playerProgress.AddCompletedLesson(currentScenario.scenarioID, totalStars);
            
            UnlockNextScenario();
            SaveTrainingProgress();
            
            Debug.Log($"训练完成! 获得{totalStars}星");
        }
        
        currentScenario = null;
    }
    
    private int CalculateEarnedStars()
    {
        int stars = 0;
        
        foreach (TrainingObjective obj in currentScenario.objectives)
        {
            if (obj.currentValue >= obj.targetValue)
            {
                stars += obj.starsThreshold;
            }
        }
        
        return Mathf.Min(stars, 3);
    }
    
    private void UnlockNextScenario()
    {
        int currentIndex = allScenarios.FindIndex(s => s.scenarioID == currentScenario.scenarioID);
        if (currentIndex >= 0 && currentIndex < allScenarios.Count - 1)
        {
            TrainingScenario nextScenario = allScenarios[currentIndex + 1];
            nextScenario.isUnlocked = true;
            playerProgress.UnlockScenario(nextScenario.scenarioID);
        }
    }
    
    public void UpdateObjectiveProgress(string objectiveID, int value)
    {
        if (currentScenario == null) return;
        
        TrainingObjective obj = currentScenario.objectives.Find(o => o.objectiveID == objectiveID);
        if (obj != null && !obj.isCompleted)
        {
            obj.currentValue += value;
            
            if (obj.currentValue >= obj.targetValue)
            {
                obj.isCompleted = true;
                Debug.Log($"目标完成: {obj.description}");
            }
        }
    }
    
    public List<TrainingLesson> GetAllLessons()
    {
        return allLessons;
    }
    
    public List<TrainingLesson> GetLessonsByCategory(string category)
    {
        return allLessons.FindAll(l => l.category == category);
    }
    
    public List<TrainingScenario> GetUnlockedScenarios()
    {
        return allScenarios.FindAll(s => s.isUnlocked);
    }
    
    public TrainingScenario GetScenarioByID(string scenarioID)
    {
        return allScenarios.Find(s => s.scenarioID == scenarioID);
    }
    
    public TrainingLesson GetLessonByID(string lessonID)
    {
        return allLessons.Find(l => l.lessonID == lessonID);
    }
    
    public int GetTotalStars()
    {
        return playerProgress.totalStars;
    }
    
    public float GetOverallProgress()
    {
        int completedCount = playerProgress.completedLessons.Count;
        int totalCount = allScenarios.Count;
        
        if (totalCount == 0) return 0;
        
        return (float)completedCount / totalCount * 100;
    }
    
    public void SaveTrainingData()
    {
        string path = Application.dataPath + "/Data/training_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        TrainingData data = new TrainingData();
        data.allLessons = allLessons;
        data.allScenarios = allScenarios;
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    
    public void LoadTrainingData()
    {
        string path = Application.dataPath + "/Data/training_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            TrainingData data = (TrainingData)formatter.Deserialize(stream);
            stream.Close();
            
            allLessons = data.allLessons;
            allScenarios = data.allScenarios;
        }
        else
        {
            allLessons = new List<TrainingLesson>();
            allScenarios = new List<TrainingScenario>();
        }
    }
    
    public void SaveTrainingProgress()
    {
        string path = Application.dataPath + "/Data/training_progress.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, playerProgress);
        stream.Close();
    }
    
    public void LoadTrainingProgress()
    {
        string path = Application.dataPath + "/Data/training_progress.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            playerProgress = (TrainingProgress)formatter.Deserialize(stream);
            stream.Close();
        }
    }
}

[System.Serializable]
public class TrainingData
{
    public List<TrainingLesson> allLessons;
    public List<TrainingScenario> allScenarios;
}
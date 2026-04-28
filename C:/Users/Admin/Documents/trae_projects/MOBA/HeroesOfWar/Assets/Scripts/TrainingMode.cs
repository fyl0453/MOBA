[System.Serializable]
public class TrainingLesson
{
    public string lessonID;
    public string lessonName;
    public string description;
    public string category;
    public int difficulty;
    public bool isCompleted;
    public int starsEarned;
    public int maxStars;
    public float bestTime;
    public string targetHeroID;
    
    public TrainingLesson(string id, string name, string desc, string cat, int diff)
    {
        lessonID = id;
        lessonName = name;
        description = desc;
        category = cat;
        difficulty = diff;
        isCompleted = false;
        starsEarned = 0;
        maxStars = 3;
        bestTime = 0;
        targetHeroID = "";
    }
}

[System.Serializable]
public class TrainingScenario
{
    public string scenarioID;
    public string scenarioName;
    public string description;
    public string sceneName;
    public List<TrainingObjective> objectives;
    public List<string> availableHeroes;
    public List<SpawnPoint> allySpawns;
    public List<SpawnPoint> enemySpawns;
    public bool isUnlocked;
    
    public TrainingScenario(string id, string name)
    {
        scenarioID = id;
        scenarioName = name;
        description = "";
        sceneName = "";
        objectives = new List<TrainingObjective>();
        availableHeroes = new List<string>();
        allySpawns = new List<SpawnPoint>();
        enemySpawns = new List<SpawnPoint>();
        isUnlocked = false;
    }
}

[System.Serializable]
public class TrainingObjective
{
    public string objectiveID;
    public string description;
    public string objectiveType;
    public int targetValue;
    public int currentValue;
    public bool isCompleted;
    public float timeLimit;
    public int starsThreshold;
    
    public TrainingObjective(string id, string desc, string type, int target, float time, int stars)
    {
        objectiveID = id;
        description = desc;
        objectiveType = type;
        targetValue = target;
        currentValue = 0;
        isCompleted = false;
        timeLimit = time;
        starsThreshold = stars;
    }
}

[System.Serializable]
public class SpawnPoint
{
    public Vector3 position;
    public float rotation;
    
    public SpawnPoint(Vector3 pos, float rot)
    {
        position = pos;
        rotation = rot;
    }
}

[System.Serializable]
public class AIMovement
{
    public string aiID;
    public string aiName;
    public string behaviorType;
    public List<Vector3> waypoints;
    public int currentWaypointIndex;
    public float speed;
    public bool isPatrolLoop;
    
    public AIMovement(string id, string name, string behavior)
    {
        aiID = id;
        aiName = name;
        behaviorType = behavior;
        waypoints = new List<Vector3>();
        currentWaypointIndex = 0;
        speed = 3f;
        isPatrolLoop = true;
    }
}

[System.Serializable]
public class TrainingProgress
{
    public string playerID;
    public List<string> completedLessons;
    public List<string> unlockedScenarios;
    public int totalStars;
    public Dictionary<string, int> heroMastery;
    
    public TrainingProgress(string pid)
    {
        playerID = pid;
        completedLessons = new List<string>();
        unlockedScenarios = new List<string>();
        totalStars = 0;
        heroMastery = new Dictionary<string, int>();
    }
    
    public void AddCompletedLesson(string lessonID, int stars)
    {
        if (!completedLessons.Contains(lessonID))
        {
            completedLessons.Add(lessonID);
        }
        totalStars += stars;
    }
    
    public void UnlockScenario(string scenarioID)
    {
        if (!unlockedScenarios.Contains(scenarioID))
        {
            unlockedScenarios.Add(scenarioID);
        }
    }
}
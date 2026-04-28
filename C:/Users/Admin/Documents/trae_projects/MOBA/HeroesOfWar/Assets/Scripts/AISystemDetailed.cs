[System.Serializable]
public class AISystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AIController> aiControllers;
    public List<AIController> aiControllers1;
    public List<AIController> aiControllers2;
    public List<AIController> aiControllers3;
    public List<AIBehavior> aiBehaviors;
    public List<AIEvent> aiEvents;

    public AISystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        aiControllers = new List<AIController>();
        aiBehaviors = new List<AIBehavior>();
        aiEvents = new List<AIEvent>();
    }

    public void AddAIController(AIController aiController)
    {
        aiControllers.Add(aiController);
    }

    public void AddAIBehavior(AIBehavior aiBehavior)
    {
        aiBehaviors.Add(aiBehavior);
    }

    public void AddAIEvent(AIEvent aiEvent)
    {
        aiEvents.Add(aiEvent);
    }

    public AIController GetAIController(string controllerID)
    {
        return aiControllers.Find(aic => aic.controllerID == controllerID);
    }

    public AIBehavior GetAIBehavior(string behaviorID)
    {
        return aiBehaviors.Find(aib => aib.behaviorID == behaviorID);
    }

    public AIEvent GetAIEvent(string eventID)
    {
        return aiEvents.Find(aie => aie.eventID == eventID);
    }

    public List<AIController> GetAIControllersByDifficulty(string difficulty)
    {
        return aiControllers.FindAll(aic => aic.difficulty == difficulty);
    }

    public List<AIBehavior> GetAIBehaviorsByType(string behaviorType)
    {
        return aiBehaviors.FindAll(aib => aib.behaviorType == behaviorType);
    }

    public List<AIEvent> GetAIEventsByUser(string userID)
    {
        return aiEvents.FindAll(aie => aie.userID == userID);
    }
}

[System.Serializable]
public class AIController
{
    public string controllerID;
    public string controllerName;
    public string difficulty;
    public int level;
    public float aggression;
    public float defensiveness;
    public float roaming;
    public string skillLevel;
    public bool isEnabled;

    public AIController(string id, string controllerName, string difficulty, int level, float aggression, float defensiveness, float roaming, string skillLevel)
    {
        controllerID = id;
        this.controllerName = controllerName;
        this.difficulty = difficulty;
        this.level = level;
        this.aggression = aggression;
        this.defensiveness = defensiveness;
        this.roaming = roaming;
        this.skillLevel = skillLevel;
        isEnabled = true;
    }
}

[System.Serializable]
public class AIBehavior
{
    public string behaviorID;
    public string behaviorName;
    public string behaviorType;
    public string behaviorDescription;
    public float priority;
    public string targetType;
    public float activationThreshold;
    public bool isEnabled;

    public AIBehavior(string id, string behaviorName, string behaviorType, string behaviorDescription, float priority, string targetType, float activationThreshold)
    {
        behaviorID = id;
        this.behaviorName = behaviorName;
        this.behaviorType = behaviorType;
        this.behaviorDescription = behaviorDescription;
        this.priority = priority;
        this.targetType = targetType;
        this.activationThreshold = activationThreshold;
        isEnabled = true;
    }
}

[System.Serializable]
public class AIEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string description;
    public string timestamp;
    public string status;

    public AIEvent(string id, string eventType, string userID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
    }
}

[System.Serializable]
public class AISystemDetailedManagerData
{
    public AISystemDetailed system;

    public AISystemDetailedManagerData()
    {
        system = new AISystemDetailed("ai_system_detailed", "智能AI系统详细", "管理智能AI的详细功能，包括智能队友和对手AI");
    }
}
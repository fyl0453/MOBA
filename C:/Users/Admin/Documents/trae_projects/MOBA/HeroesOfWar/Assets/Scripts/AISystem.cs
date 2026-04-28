[System.Serializable]
public class AISystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AIBehavior> behaviors;
    public List<AIDifficulty> difficulties;
    public List<AIProfile> profiles;
    
    public AISystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        behaviors = new List<AIBehavior>();
        difficulties = new List<AIDifficulty>();
        profiles = new List<AIProfile>();
    }
    
    public void AddBehavior(AIBehavior behavior)
    {
        behaviors.Add(behavior);
    }
    
    public void AddDifficulty(AIDifficulty difficulty)
    {
        difficulties.Add(difficulty);
    }
    
    public void AddProfile(AIProfile profile)
    {
        profiles.Add(profile);
    }
    
    public AIBehavior GetBehavior(string behaviorID)
    {
        return behaviors.Find(b => b.behaviorID == behaviorID);
    }
    
    public AIDifficulty GetDifficulty(string difficultyID)
    {
        return difficulties.Find(d => d.difficultyID == difficultyID);
    }
    
    public AIProfile GetProfile(string profileID)
    {
        return profiles.Find(p => p.profileID == profileID);
    }
    
    public List<AIBehavior> GetBehaviorsByType(string behaviorType)
    {
        return behaviors.FindAll(b => b.behaviorType == behaviorType);
    }
}

[System.Serializable]
public class AIBehavior
{
    public string behaviorID;
    public string behaviorName;
    public string behaviorDescription;
    public string behaviorType;
    public float priority;
    public float cooldown;
    public float probability;
    
    public AIBehavior(string id, string name, string desc, string type, float priority, float cooldown, float probability)
    {
        behaviorID = id;
        behaviorName = name;
        behaviorDescription = desc;
        behaviorType = type;
        this.priority = priority;
        this.cooldown = cooldown;
        this.probability = probability;
    }
}

[System.Serializable]
public class AIDifficulty
{
    public string difficultyID;
    public string difficultyName;
    public string difficultyDescription;
    public float aggression;
    public float reactionTime;
    public float accuracy;
    public float decisionMaking;
    public float teamwork;
    
    public AIDifficulty(string id, string name, string desc, float aggression, float reactionTime, float accuracy, float decisionMaking, float teamwork)
    {
        difficultyID = id;
        difficultyName = name;
        difficultyDescription = desc;
        this.aggression = aggression;
        this.reactionTime = reactionTime;
        this.accuracy = accuracy;
        this.decisionMaking = decisionMaking;
        this.teamwork = teamwork;
    }
}

[System.Serializable]
public class AIProfile
{
    public string profileID;
    public string profileName;
    public string profileDescription;
    public string heroID;
    public List<string> behaviors;
    public string difficultyID;
    public float winRate;
    public int totalMatches;
    
    public AIProfile(string id, string name, string desc, string heroID, string difficultyID)
    {
        profileID = id;
        profileName = name;
        profileDescription = desc;
        this.heroID = heroID;
        behaviors = new List<string>();
        this.difficultyID = difficultyID;
        winRate = 0f;
        totalMatches = 0;
    }
    
    public void AddBehavior(string behaviorID)
    {
        behaviors.Add(behaviorID);
    }
    
    public void UpdateStats(bool won)
    {
        totalMatches++;
        winRate = (winRate * (totalMatches - 1) + (won ? 1f : 0f)) / totalMatches;
    }
}

[System.Serializable]
public class AIAction
{
    public string actionID;
    public string actionType;
    public string targetID;
    public float priority;
    public float executionTime;
    public bool isCompleted;
    
    public AIAction(string id, string type, string target, float priority)
    {
        actionID = id;
        actionType = type;
        targetID = target;
        this.priority = priority;
        executionTime = Time.time;
        isCompleted = false;
    }
    
    public void Complete()
    {
        isCompleted = true;
    }
}

[System.Serializable]
public class AIState
{
    public string stateID;
    public string stateName;
    public string stateDescription;
    public List<string> possibleActions;
    public float transitionProbability;
    
    public AIState(string id, string name, string desc)
    {
        stateID = id;
        stateName = name;
        stateDescription = desc;
        possibleActions = new List<string>();
        transitionProbability = 0f;
    }
    
    public void AddPossibleAction(string actionID)
    {
        possibleActions.Add(actionID);
    }
}

[System.Serializable]
public class AIManagerData
{
    public AISystem system;
    public List<AIState> states;
    public List<AIAction> actions;
    
    public AIManagerData()
    {
        system = new AISystem("ai_system", "AI系统", "管理游戏中的AI");
        states = new List<AIState>();
        actions = new List<AIAction>();
    }
    
    public void AddState(AIState state)
    {
        states.Add(state);
    }
    
    public void AddAction(AIAction action)
    {
        actions.Add(action);
    }
    
    public AIState GetState(string stateID)
    {
        return states.Find(s => s.stateID == stateID);
    }
    
    public AIAction GetAction(string actionID)
    {
        return actions.Find(a => a.actionID == actionID);
    }
}
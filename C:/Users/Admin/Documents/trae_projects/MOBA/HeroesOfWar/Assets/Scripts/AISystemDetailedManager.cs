using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AISystemDetailedManager : MonoBehaviour
{
    public static AISystemDetailedManager Instance { get; private set; }

    public AISystemDetailedManagerData aiData;

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
        LoadAIData();

        if (aiData == null)
        {
            aiData = new AISystemDetailedManagerData();
            InitializeDefaultAISystem();
        }
    }

    private void InitializeDefaultAISystem()
    {
        // AI控制器 - 不同难度
        AIController ai1 = new AIController("ai_001", "简单AI", "easy", 1, 0.3f, 0.7f, 0.5f, "beginner");
        AIController ai2 = new AIController("ai_002", "普通AI", "normal", 2, 0.5f, 0.5f, 0.5f, "intermediate");
        AIController ai3 = new AIController("ai_003", "困难AI", "hard", 3, 0.7f, 0.3f, 0.5f, "advanced");
        AIController ai4 = new AIController("ai_004", "大师AI", "master", 4, 0.8f, 0.2f, 0.6f, "expert");
        AIController ai5 = new AIController("ai_005", "王者AI", "king", 5, 0.9f, 0.1f, 0.7f, "master");

        aiData.system.AddAIController(ai1);
        aiData.system.AddAIController(ai2);
        aiData.system.AddAIController(ai3);
        aiData.system.AddAIController(ai4);
        aiData.system.AddAIController(ai5);

        // AI行为
        AIBehavior behavior1 = new AIBehavior("behavior_001", "攻击行为", "combat", "主动攻击敌人", 0.8f, "enemy", 0.5f);
        AIBehavior behavior2 = new AIBehavior("behavior_002", "防御行为", "defensive", "在塔下防守", 0.7f, "tower", 0.6f);
        AIBehavior behavior3 = new AIBehavior("behavior_003", "刷野行为", "farming", "清理野怪", 0.5f, "jungle", 0.3f);
        AIBehavior behavior4 = new AIBehavior("behavior_004", "游走行为", "roaming", "支援其他路", 0.6f, "lane", 0.4f);
        AIBehavior behavior5 = new AIBehavior("behavior_005", "团战行为", "teamfight", "参与团战", 0.9f, "team", 0.7f);
        AIBehavior behavior6 = new AIBehavior("behavior_006", "推塔行为", "pushing", "带线推塔", 0.7f, "tower", 0.5f);

        aiData.system.AddAIBehavior(behavior1);
        aiData.system.AddAIBehavior(behavior2);
        aiData.system.AddAIBehavior(behavior3);
        aiData.system.AddAIBehavior(behavior4);
        aiData.system.AddAIBehavior(behavior5);
        aiData.system.AddAIBehavior(behavior6);

        // AI事件
        AIEvent aiEvent1 = new AIEvent("event_001", "ai_spawn", "system", "AI角色生成");
        AIEvent aiEvent2 = new AIEvent("event_002", "ai_behavior", "system", "AI执行行为");
        AIEvent aiEvent3 = new AIEvent("event_003", "ai_death", "system", "AI角色死亡");

        aiData.system.AddAIEvent(aiEvent1);
        aiData.system.AddAIEvent(aiEvent2);
        aiData.system.AddAIEvent(aiEvent3);

        SaveAIData();
    }

    // AI控制器管理
    public void AddAIController(string controllerName, string difficulty, int level, float aggression, float defensiveness, float roaming, string skillLevel)
    {
        string controllerID = "ai_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AIController aiController = new AIController(controllerID, controllerName, difficulty, level, aggression, defensiveness, roaming, skillLevel);
        aiData.system.AddAIController(aiController);
        SaveAIData();
        Debug.Log("成功添加AI控制器: " + controllerName);
    }

    public List<AIController> GetAIControllersByDifficulty(string difficulty)
    {
        return aiData.system.GetAIControllersByDifficulty(difficulty);
    }

    public AIController GetAIController(string controllerID)
    {
        return aiData.system.GetAIController(controllerID);
    }

    // AI行为管理
    public void AddAIBehavior(string behaviorName, string behaviorType, string behaviorDescription, float priority, string targetType, float activationThreshold)
    {
        string behaviorID = "behavior_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AIBehavior aiBehavior = new AIBehavior(behaviorID, behaviorName, behaviorType, behaviorDescription, priority, targetType, activationThreshold);
        aiData.system.AddAIBehavior(aiBehavior);
        SaveAIData();
        Debug.Log("成功添加AI行为: " + behaviorName);
    }

    public List<AIBehavior> GetAIBehaviorsByType(string behaviorType)
    {
        return aiData.system.GetAIBehaviorsByType(behaviorType);
    }

    // AI决策
    public AIBehavior MakeAIDecision(AIController controller)
    {
        List<AIBehavior> behaviors = aiData.system.aiBehaviors;
        AIBehavior bestBehavior = null;
        float bestPriority = 0;

        foreach (AIBehavior behavior in behaviors)
        {
            if (behavior.isEnabled)
            {
                float priority = behavior.priority;
                if (behavior.behaviorType == "combat" && controller.aggression < 0.5f)
                    priority *= 0.5f;
                if (behavior.behaviorType == "defensive" && controller.defensiveness < 0.5f)
                    priority *= 0.5f;

                if (priority > bestPriority)
                {
                    bestPriority = priority;
                    bestBehavior = behavior;
                }
            }
        }

        if (bestBehavior != null)
        {
            CreateAIEvent("decision", "system", "AI做出决策: " + bestBehavior.behaviorName);
        }

        return bestBehavior;
    }

    // AI事件管理
    public string CreateAIEvent(string eventType, string userID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AIEvent aiEvent = new AIEvent(eventID, eventType, userID, description);
        aiData.system.AddAIEvent(aiEvent);
        SaveAIData();
        Debug.Log("成功创建AI事件: " + eventType);
        return eventID;
    }

    public List<AIEvent> GetUserEvents(string userID)
    {
        return aiData.system.GetAIEventsByUser(userID);
    }

    // 数据持久化
    public void SaveAIData()
    {
        string path = Application.dataPath + "/Data/ai_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, aiData);
        stream.Close();
    }

    public void LoadAIData()
    {
        string path = Application.dataPath + "/Data/ai_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            aiData = (AISystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            aiData = new AISystemDetailedManagerData();
        }
    }
}
[System.Serializable]
public class GameAssistantExtended
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AssistantTip> tips;
    public List<AssistantAnalysis> analyses;
    public List<AssistantConfig> configs;
    
    public GameAssistantExtended(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        tips = new List<AssistantTip>();
        analyses = new List<AssistantAnalysis>();
        configs = new List<AssistantConfig>();
    }
    
    public void AddTip(AssistantTip tip)
    {
        tips.Add(tip);
    }
    
    public void AddAnalysis(AssistantAnalysis analysis)
    {
        analyses.Add(analysis);
    }
    
    public void AddConfig(AssistantConfig config)
    {
        configs.Add(config);
    }
    
    public AssistantTip GetTip(string tipID)
    {
        return tips.Find(t => t.tipID == tipID);
    }
    
    public AssistantAnalysis GetAnalysis(string analysisID)
    {
        return analyses.Find(a => a.analysisID == analysisID);
    }
    
    public AssistantConfig GetConfig(string playerID)
    {
        return configs.Find(c => c.playerID == playerID);
    }
    
    public List<AssistantTip> GetTipsByCategory(string category)
    {
        return tips.FindAll(t => t.category == category);
    }
    
    public List<AssistantTip> GetTipsByHero(string heroID)
    {
        return tips.FindAll(t => t.heroID == heroID);
    }
}

[System.Serializable]
public class AssistantTip
{
    public string tipID;
    public string tipText;
    public string category;
    public string heroID;
    public string heroName;
    public string tipType;
    public int priority;
    public bool isActive;
    
    public AssistantTip(string id, string text, string category, string heroID, string heroName, string type, int priority)
    {
        tipID = id;
        tipText = text;
        this.category = category;
        this.heroID = heroID;
        this.heroName = heroName;
        tipType = type;
        this.priority = priority;
        isActive = true;
    }
    
    public void Activate()
    {
        isActive = true;
    }
    
    public void Deactivate()
    {
        isActive = false;
    }
}

[System.Serializable]
public class AssistantAnalysis
{
    public string analysisID;
    public string playerID;
    public string matchID;
    public string analysisType;
    public string analysisContent;
    public float score;
    public List<string> recommendations;
    public string createdAt;
    
    public AssistantAnalysis(string id, string playerID, string matchID, string type, string content, float score, List<string> recommendations)
    {
        analysisID = id;
        this.playerID = playerID;
        this.matchID = matchID;
        analysisType = type;
        analysisContent = content;
        this.score = score;
        this.recommendations = recommendations;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class AssistantConfig
{
    public string configID;
    public string playerID;
    public bool enableTips;
    public bool enableAnalysis;
    public bool enableVoice;
    public bool enableNotifications;
    public string tipFrequency;
    public string analysisLevel;
    
    public AssistantConfig(string id, string playerID)
    {
        configID = id;
        this.playerID = playerID;
        enableTips = true;
        enableAnalysis = true;
        enableVoice = true;
        enableNotifications = true;
        tipFrequency = "Medium";
        analysisLevel = "Basic";
    }
    
    public void UpdateConfig(bool tips, bool analysis, bool voice, bool notifications, string frequency, string level)
    {
        enableTips = tips;
        enableAnalysis = analysis;
        enableVoice = voice;
        enableNotifications = notifications;
        tipFrequency = frequency;
        analysisLevel = level;
    }
}

[System.Serializable]
public class GameAssistantManagerData
{
    public GameAssistantExtended system;
    
    public GameAssistantManagerData()
    {
        system = new GameAssistantExtended("game_assistant_extended", "游戏助手扩展", "提供智能游戏指导");
    }
}
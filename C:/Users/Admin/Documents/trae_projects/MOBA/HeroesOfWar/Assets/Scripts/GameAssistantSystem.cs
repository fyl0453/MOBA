[System.Serializable]
public class GameAssistantSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AssistantFeature> features;
    public List<AssistantTip> tips;
    public List<AssistantRecommendation> recommendations;
    
    public GameAssistantSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        features = new List<AssistantFeature>();
        tips = new List<AssistantTip>();
        recommendations = new List<AssistantRecommendation>();
    }
    
    public void AddFeature(AssistantFeature feature)
    {
        features.Add(feature);
    }
    
    public void AddTip(AssistantTip tip)
    {
        tips.Add(tip);
    }
    
    public void AddRecommendation(AssistantRecommendation recommendation)
    {
        recommendations.Add(recommendation);
    }
    
    public AssistantFeature GetFeature(string featureID)
    {
        return features.Find(f => f.featureID == featureID);
    }
    
    public List<AssistantTip> GetTipsByType(string tipType)
    {
        return tips.FindAll(t => t.tipType == tipType);
    }
    
    public List<AssistantRecommendation> GetRecommendationsByType(string type)
    {
        return recommendations.FindAll(r => r.recommendationType == type);
    }
}

[System.Serializable]
public class AssistantFeature
{
    public string featureID;
    public string featureName;
    public string featureDescription;
    public bool isEnabled;
    public string featureType;
    
    public AssistantFeature(string id, string name, string desc, string type)
    {
        featureID = id;
        featureName = name;
        featureDescription = desc;
        isEnabled = true;
        featureType = type;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class AssistantTip
{
    public string tipID;
    public string tipContent;
    public string tipType;
    public string heroID;
    public string gameMode;
    public int priority;
    public bool isRead;
    
    public AssistantTip(string id, string content, string type, string hero = "", string mode = "")
    {
        tipID = id;
        tipContent = content;
        tipType = type;
        heroID = hero;
        gameMode = mode;
        priority = 0;
        isRead = false;
    }
    
    public void MarkAsRead()
    {
        isRead = true;
    }
}

[System.Serializable]
public class AssistantRecommendation
{
    public string recommendationID;
    public string recommendationType;
    public string targetID;
    public string recommendationContent;
    public float confidence;
    public string heroID;
    public string gameMode;
    
    public AssistantRecommendation(string id, string type, string target, string content, float confidence, string hero = "", string mode = "")
    {
        recommendationID = id;
        recommendationType = type;
        targetID = target;
        recommendationContent = content;
        this.confidence = confidence;
        heroID = hero;
        gameMode = mode;
    }
}

[System.Serializable]
public class SkillCombo
{
    public string comboID;
    public string heroID;
    public string comboName;
    public List<string> skillOrder;
    public string comboDescription;
    public string situation;
    
    public SkillCombo(string id, string hero, string name, List<string> order, string desc, string situation)
    {
        comboID = id;
        heroID = hero;
        comboName = name;
        skillOrder = order;
        comboDescription = desc;
        this.situation = situation;
    }
}

[System.Serializable]
public class GameAnalysis
{
    public string analysisID;
    public string matchID;
    public string playerID;
    public float kda;
    public int kills;
    public int deaths;
    public int assists;
    public int gold;
    public int damage;
    public int healing;
    public List<string> strengths;
    public List<string> weaknesses;
    public List<string> recommendations;
    
    public GameAnalysis(string id, string match, string player, float kda, int kills, int deaths, int assists, int gold, int damage, int healing)
    {
        analysisID = id;
        matchID = match;
        playerID = player;
        this.kda = kda;
        this.kills = kills;
        this.deaths = deaths;
        this.assists = assists;
        this.gold = gold;
        this.damage = damage;
        this.healing = healing;
        strengths = new List<string>();
        weaknesses = new List<string>();
        recommendations = new List<string>();
    }
    
    public void AddStrength(string strength)
    {
        strengths.Add(strength);
    }
    
    public void AddWeakness(string weakness)
    {
        weaknesses.Add(weakness);
    }
    
    public void AddRecommendation(string recommendation)
    {
        recommendations.Add(recommendation);
    }
}

[System.Serializable]
public class PlayerAssistantData
{
    public string playerID;
    public Dictionary<string, bool> featureSettings;
    public List<string> readTips;
    public List<SkillCombo> favoriteCombos;
    public List<GameAnalysis> gameAnalyses;
    
    public PlayerAssistantData(string playerID)
    {
        this.playerID = playerID;
        featureSettings = new Dictionary<string, bool>();
        readTips = new List<string>();
        favoriteCombos = new List<SkillCombo>();
        gameAnalyses = new List<GameAnalysis>();
    }
    
    public void SetFeatureEnabled(string featureID, bool enabled)
    {
        featureSettings[featureID] = enabled;
    }
    
    public bool IsFeatureEnabled(string featureID)
    {
        return featureSettings.ContainsKey(featureID) && featureSettings[featureID];
    }
    
    public void MarkTipAsRead(string tipID)
    {
        if (!readTips.Contains(tipID))
        {
            readTips.Add(tipID);
        }
    }
    
    public bool IsTipRead(string tipID)
    {
        return readTips.Contains(tipID);
    }
    
    public void AddFavoriteCombo(SkillCombo combo)
    {
        if (!favoriteCombos.Exists(c => c.comboID == combo.comboID))
        {
            favoriteCombos.Add(combo);
        }
    }
    
    public void RemoveFavoriteCombo(string comboID)
    {
        favoriteCombos.RemoveAll(c => c.comboID == comboID);
    }
    
    public void AddGameAnalysis(GameAnalysis analysis)
    {
        gameAnalyses.Add(analysis);
    }
    
    public List<GameAnalysis> GetGameAnalyses(int limit = 10)
    {
        List<GameAnalysis> sorted = new List<GameAnalysis>(gameAnalyses);
        sorted.Sort((a, b) => b.analysisID.CompareTo(a.analysisID));
        return sorted.GetRange(0, Mathf.Min(limit, sorted.Count));
    }
}

[System.Serializable]
public class GameAssistantManagerData
{
    public GameAssistantSystem system;
    public List<PlayerAssistantData> playerData;
    public List<SkillCombo> skillCombos;
    
    public GameAssistantManagerData()
    {
        system = new GameAssistantSystem("game_assistant", "游戏助手系统", "提供游戏内指导和提示");
        playerData = new List<PlayerAssistantData>();
        skillCombos = new List<SkillCombo>();
    }
    
    public void AddPlayerData(PlayerAssistantData data)
    {
        playerData.Add(data);
    }
    
    public void AddSkillCombo(SkillCombo combo)
    {
        skillCombos.Add(combo);
    }
    
    public PlayerAssistantData GetPlayerData(string playerID)
    {
        return playerData.Find(pd => pd.playerID == playerID);
    }
    
    public List<SkillCombo> GetSkillCombosByHero(string heroID)
    {
        return skillCombos.FindAll(c => c.heroID == heroID);
    }
}
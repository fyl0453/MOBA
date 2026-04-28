using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameAssistantManagerExtended : MonoBehaviour
{
    public static GameAssistantManagerExtended Instance { get; private set; }
    
    public GameAssistantManagerData gameAssistantData;
    
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
        LoadGameAssistantData();
        
        if (gameAssistantData == null)
        {
            gameAssistantData = new GameAssistantManagerData();
            InitializeDefaultAssistant();
        }
    }
    
    private void InitializeDefaultAssistant()
    {
        // 创建默认提示
        AssistantTip tip1 = new AssistantTip(
            "tip1",
            "关羽的二技能可以解除控制效果，在被控制时使用效果最佳",
            "Skill",
            "guanyu",
            "关羽",
            "General",
            1
        );
        
        AssistantTip tip2 = new AssistantTip(
            "tip2",
            "诸葛亮的大招需要标记目标才能造成最大伤害",
            "Skill",
            "zhugeliang",
            "诸葛亮",
            "General",
            1
        );
        
        AssistantTip tip3 = new AssistantTip(
            "tip3",
            "团战中优先保护己方输出英雄",
            "Strategy",
            "",
            "通用",
            "General",
            2
        );
        
        AssistantTip tip4 = new AssistantTip(
            "tip4",
            "推塔时注意敌方打野的位置",
            "Strategy",
            "",
            "通用",
            "General",
            2
        );
        
        gameAssistantData.system.AddTip(tip1);
        gameAssistantData.system.AddTip(tip2);
        gameAssistantData.system.AddTip(tip3);
        gameAssistantData.system.AddTip(tip4);
        
        // 创建默认配置
        AssistantConfig defaultConfig = new AssistantConfig("config1", "default");
        gameAssistantData.system.AddConfig(defaultConfig);
        
        SaveGameAssistantData();
    }
    
    public string CreateAnalysis(string playerID, string matchID, string type, string content, float score, List<string> recommendations)
    {
        string analysisID = System.Guid.NewGuid().ToString();
        AssistantAnalysis newAnalysis = new AssistantAnalysis(analysisID, playerID, matchID, type, content, score, recommendations);
        gameAssistantData.system.AddAnalysis(newAnalysis);
        SaveGameAssistantData();
        Debug.Log($"成功创建分析: {type}");
        return analysisID;
    }
    
    public void AddTip(string text, string category, string heroID, string heroName, string type, int priority)
    {
        string tipID = System.Guid.NewGuid().ToString();
        AssistantTip newTip = new AssistantTip(tipID, text, category, heroID, heroName, type, priority);
        gameAssistantData.system.AddTip(newTip);
        SaveGameAssistantData();
        Debug.Log($"成功添加提示: {text}");
    }
    
    public void UpdatePlayerConfig(string playerID, bool tips, bool analysis, bool voice, bool notifications, string frequency, string level)
    {
        AssistantConfig config = gameAssistantData.system.GetConfig(playerID);
        if (config == null)
        {
            string configID = System.Guid.NewGuid().ToString();
            config = new AssistantConfig(configID, playerID);
            gameAssistantData.system.AddConfig(config);
        }
        
        config.UpdateConfig(tips, analysis, voice, notifications, frequency, level);
        SaveGameAssistantData();
        Debug.Log("成功更新游戏助手配置");
    }
    
    public List<AssistantTip> GetTipsByHero(string heroID)
    {
        return gameAssistantData.system.GetTipsByHero(heroID);
    }
    
    public List<AssistantTip> GetTipsByCategory(string category)
    {
        return gameAssistantData.system.GetTipsByCategory(category);
    }
    
    public AssistantConfig GetPlayerConfig(string playerID)
    {
        return gameAssistantData.system.GetConfig(playerID);
    }
    
    public List<AssistantAnalysis> GetPlayerAnalyses(string playerID, int limit = 20)
    {
        List<AssistantAnalysis> analyses = new List<AssistantAnalysis>();
        foreach (AssistantAnalysis analysis in gameAssistantData.system.analyses)
        {
            if (analysis.playerID == playerID)
            {
                analyses.Add(analysis);
            }
        }
        analyses.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return analyses.GetRange(0, Mathf.Min(limit, analyses.Count));
    }
    
    public void AnalyzeMatch(string playerID, string matchID, string heroID, string heroName, int kills, int deaths, int assists, int damage, int gold, bool isWin)
    {
        string analysisType = "Match";
        string content = $"你在本局使用{heroName}的表现分析：\n击杀: {kills}\n死亡: {deaths}\n助攻: {assists}\n伤害: {damage}\n经济: {gold}";
        
        float score = CalculateMatchScore(kills, deaths, assists, damage, gold, isWin);
        List<string> recommendations = GenerateRecommendations(heroID, kills, deaths, assists, damage, gold);
        
        CreateAnalysis(playerID, matchID, analysisType, content, score, recommendations);
    }
    
    private float CalculateMatchScore(int kills, int deaths, int assists, int damage, int gold, bool isWin)
    {
        float baseScore = 50;
        
        // 击杀和助攻加分
        baseScore += kills * 3;
        baseScore += assists * 1.5f;
        
        // 死亡减分
        baseScore -= deaths * 2;
        
        // 伤害和经济加分
        baseScore += damage / 1000f;
        baseScore += gold / 500f;
        
        // 胜利加分
        if (isWin)
        {
            baseScore += 20;
        }
        
        // 确保分数在0-100之间
        return Mathf.Clamp(baseScore, 0, 100);
    }
    
    private List<string> GenerateRecommendations(string heroID, int kills, int deaths, int assists, int damage, int gold)
    {
        List<string> recommendations = new List<string>();
        
        if (deaths > 5)
        {
            recommendations.Add("注意走位，减少不必要的死亡");
        }
        
        if (kills + assists < 5)
        {
            recommendations.Add("积极参与团战，提高参团率");
        }
        
        if (damage < 10000)
        {
            recommendations.Add("提高输出伤害，多参与击杀");
        }
        
        if (gold < 8000)
        {
            recommendations.Add("注意经济发育，多清兵线和野怪");
        }
        
        if (recommendations.Count == 0)
        {
            recommendations.Add("表现不错，继续保持！");
        }
        
        return recommendations;
    }
    
    public void SaveGameAssistantData()
    {
        string path = Application.dataPath + "/Data/game_assistant_extended_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, gameAssistantData);
        stream.Close();
    }
    
    public void LoadGameAssistantData()
    {
        string path = Application.dataPath + "/Data/game_assistant_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            gameAssistantData = (GameAssistantManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            gameAssistantData = new GameAssistantManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameAssistantManager : MonoBehaviour
{
    public static GameAssistantManager Instance { get; private set; }
    
    public GameAssistantManagerData assistantData;
    
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
        LoadAssistantData();
        
        if (assistantData == null)
        {
            assistantData = new GameAssistantManagerData();
            InitializeDefaultData();
        }
    }
    
    private void InitializeDefaultData()
    {
        // 创建默认功能
        AssistantFeature feature1 = new AssistantFeature("feature_combo", "技能连招", "提供英雄技能连招推荐", "Combo");
        AssistantFeature feature2 = new AssistantFeature("feature_item", "出装推荐", "根据游戏局势推荐装备", "Item");
        AssistantFeature feature3 = new AssistantFeature("feature_tip", "游戏提示", "提供游戏内提示和技巧", "Tip");
        AssistantFeature feature4 = new AssistantFeature("feature_analysis", "赛后分析", "提供比赛数据分析", "Analysis");
        
        assistantData.system.AddFeature(feature1);
        assistantData.system.AddFeature(feature2);
        assistantData.system.AddFeature(feature3);
        assistantData.system.AddFeature(feature4);
        
        // 创建默认提示
        AssistantTip tip1 = new AssistantTip("tip_1", "关羽需要保持移动来维持冲锋状态", "Gameplay", "hero_guanyu");
        AssistantTip tip2 = new AssistantTip("tip_2", "诸葛亮的被动技能可以增加伤害", "Gameplay", "hero_zhugeliang");
        AssistantTip tip3 = new AssistantTip("tip_3", "团战中要注意位置，避免被敌方集火", "Teamfight");
        AssistantTip tip4 = new AssistantTip("tip_4", "经济是胜利的关键，要注意补兵和野怪", "Economy");
        
        assistantData.system.AddTip(tip1);
        assistantData.system.AddTip(tip2);
        assistantData.system.AddTip(tip3);
        assistantData.system.AddTip(tip4);
        
        // 创建默认技能连招
        List<string> combo1 = new List<string> { "skill_guanyu_2", "skill_guanyu_1", "skill_guanyu_ult" };
        List<string> combo2 = new List<string> { "skill_zhugeliang_2", "skill_zhugeliang_1", "skill_zhugeliang_ult" };
        
        SkillCombo combo关羽 = new SkillCombo("combo_guanyu_1", "hero_guanyu", "冲锋连招", combo1, "利用冲锋状态先手开团", "团战先手");
        SkillCombo combo诸葛亮 = new SkillCombo("combo_zhugeliang_1", "hero_zhugeliang", "爆发连招", combo2, "利用位移接近敌人后爆发", "单体爆发");
        
        assistantData.AddSkillCombo(combo关羽);
        assistantData.AddSkillCombo(combo诸葛亮);
        
        // 创建默认推荐
        AssistantRecommendation recommendation1 = new AssistantRecommendation("rec_1", "Item", "equipment_blade", "当前局势适合出暗影战斧", 0.9f, "hero_guanyu");
        AssistantRecommendation recommendation2 = new AssistantRecommendation("rec_2", "Item", "equipment_book", "当前局势适合出博学者之怒", 0.85f, "hero_zhugeliang");
        
        assistantData.system.AddRecommendation(recommendation1);
        assistantData.system.AddRecommendation(recommendation2);
        
        SaveAssistantData();
    }
    
    public void EnableFeature(string featureID, bool enable)
    {
        AssistantFeature feature = assistantData.system.GetFeature(featureID);
        if (feature != null)
        {
            if (enable)
            {
                feature.Enable();
            }
            else
            {
                feature.Disable();
            }
            SaveAssistantData();
        }
    }
    
    public void SetPlayerFeatureEnabled(string playerID, string featureID, bool enabled)
    {
        PlayerAssistantData playerData = GetOrCreatePlayerData(playerID);
        playerData.SetFeatureEnabled(featureID, enabled);
        SaveAssistantData();
    }
    
    public bool IsFeatureEnabled(string playerID, string featureID)
    {
        PlayerAssistantData playerData = GetOrCreatePlayerData(playerID);
        return playerData.IsFeatureEnabled(featureID);
    }
    
    public List<AssistantTip> GetTips(string playerID, string tipType = "")
    {
        PlayerAssistantData playerData = GetOrCreatePlayerData(playerID);
        List<AssistantTip> tips = tipType == "" ? assistantData.system.tips : assistantData.system.GetTipsByType(tipType);
        
        // 过滤已读提示
        List<AssistantTip> unreadTips = new List<AssistantTip>();
        foreach (AssistantTip tip in tips)
        {
            if (!playerData.IsTipRead(tip.tipID))
            {
                unreadTips.Add(tip);
            }
        }
        
        return unreadTips;
    }
    
    public void MarkTipAsRead(string playerID, string tipID)
    {
        PlayerAssistantData playerData = GetOrCreatePlayerData(playerID);
        playerData.MarkTipAsRead(tipID);
        SaveAssistantData();
    }
    
    public List<SkillCombo> GetSkillCombos(string heroID)
    {
        return assistantData.GetSkillCombosByHero(heroID);
    }
    
    public void AddFavoriteCombo(string playerID, string comboID)
    {
        PlayerAssistantData playerData = GetOrCreatePlayerData(playerID);
        SkillCombo combo = assistantData.skillCombos.Find(c => c.comboID == comboID);
        if (combo != null)
        {
            playerData.AddFavoriteCombo(combo);
            SaveAssistantData();
        }
    }
    
    public void RemoveFavoriteCombo(string playerID, string comboID)
    {
        PlayerAssistantData playerData = GetOrCreatePlayerData(playerID);
        playerData.RemoveFavoriteCombo(comboID);
        SaveAssistantData();
    }
    
    public List<SkillCombo> GetFavoriteCombos(string playerID)
    {
        PlayerAssistantData playerData = GetOrCreatePlayerData(playerID);
        return playerData.favoriteCombos;
    }
    
    public GameAnalysis AnalyzeGame(string matchID, string playerID, float kda, int kills, int deaths, int assists, int gold, int damage, int healing)
    {
        string analysisID = System.Guid.NewGuid().ToString();
        GameAnalysis analysis = new GameAnalysis(analysisID, matchID, playerID, kda, kills, deaths, assists, gold, damage, healing);
        
        // 分析优势
        if (kda > 3)
        {
            analysis.AddStrength("KDA表现优秀");
        }
        if (gold > 10000)
        {
            analysis.AddStrength("经济发育良好");
        }
        if (damage > 20000)
        {
            analysis.AddStrength("输出伤害充足");
        }
        
        // 分析劣势
        if (deaths > 5)
        {
            analysis.AddWeakness("死亡次数过多");
        }
        if (gold < 8000)
        {
            analysis.AddWeakness("经济发育不足");
        }
        if (assists < 3)
        {
            analysis.AddWeakness("团队配合不足");
        }
        
        // 生成建议
        if (deaths > 5)
        {
            analysis.AddRecommendation("注意走位，避免被敌方集火");
        }
        if (gold < 8000)
        {
            analysis.AddRecommendation("注意补兵和野怪，提高经济收入");
        }
        if (assists < 3)
        {
            analysis.AddRecommendation("多与队友配合，参与团队战斗");
        }
        
        // 保存分析结果
        PlayerAssistantData playerData = GetOrCreatePlayerData(playerID);
        playerData.AddGameAnalysis(analysis);
        SaveAssistantData();
        
        return analysis;
    }
    
    public List<GameAnalysis> GetGameAnalyses(string playerID, int limit = 10)
    {
        PlayerAssistantData playerData = GetOrCreatePlayerData(playerID);
        return playerData.GetGameAnalyses(limit);
    }
    
    public List<AssistantRecommendation> GetRecommendations(string heroID, string type = "")
    {
        List<AssistantRecommendation> recommendations = type == "" ? assistantData.system.recommendations : assistantData.system.GetRecommendationsByType(type);
        return recommendations.FindAll(r => r.heroID == heroID || r.heroID == "");
    }
    
    public void AddRecommendation(string type, string targetID, string content, float confidence, string heroID = "", string gameMode = "")
    {
        string recommendationID = System.Guid.NewGuid().ToString();
        AssistantRecommendation recommendation = new AssistantRecommendation(recommendationID, type, targetID, content, confidence, heroID, gameMode);
        assistantData.system.AddRecommendation(recommendation);
        SaveAssistantData();
    }
    
    private PlayerAssistantData GetOrCreatePlayerData(string playerID)
    {
        PlayerAssistantData playerData = assistantData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerAssistantData(playerID);
            assistantData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public void SaveAssistantData()
    {
        string path = Application.dataPath + "/Data/game_assistant_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, assistantData);
        stream.Close();
    }
    
    public void LoadAssistantData()
    {
        string path = Application.dataPath + "/Data/game_assistant_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            assistantData = (GameAssistantManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            assistantData = new GameAssistantManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RecommendManager : MonoBehaviour
{
    public static RecommendManager Instance { get; private set; }
    
    public List<EquipmentBuild> allBuilds;
    public List<ItemRecommendation> currentRecommendations;
    public GameContext currentGameContext;
    
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
        LoadBuilds();
        LoadRecommendations();
        
        if (allBuilds.Count == 0)
        {
            InitializeDefaultBuilds();
        }
        
        currentGameContext = new GameContext();
    }
    
    private void InitializeDefaultBuilds()
    {
        EquipmentBuild attackBuild = new EquipmentBuild("build_001", "物理输出", "hero_001", "Attack");
        attackBuild.itemIDs.Add("item_001");
        attackBuild.itemIDs.Add("item_002");
        attackBuild.itemIDs.Add("item_003");
        attackBuild.itemIDs.Add("item_004");
        attackBuild.itemIDs.Add("item_005");
        attackBuild.itemIDs.Add("item_006");
        attackBuild.popularity = 1000;
        attackBuild.winRate = 52.5f;
        allBuilds.Add(attackBuild);
        
        EquipmentBuild tankBuild = new EquipmentBuild("build_002", "坦克出装", "hero_003", "Tank");
        tankBuild.itemIDs.Add("item_010");
        tankBuild.itemIDs.Add("item_011");
        tankBuild.itemIDs.Add("item_012");
        tankBuild.itemIDs.Add("item_013");
        tankBuild.itemIDs.Add("item_014");
        tankBuild.itemIDs.Add("item_015");
        tankBuild.popularity = 800;
        tankBuild.winRate = 55.2f;
        allBuilds.Add(tankBuild);
        
        EquipmentBuild magicBuild = new EquipmentBuild("build_003", "法术输出", "hero_002", "Magic");
        magicBuild.itemIDs.Add("item_020");
        magicBuild.itemIDs.Add("item_021");
        magicBuild.itemIDs.Add("item_022");
        magicBuild.itemIDs.Add("item_023");
        magicBuild.itemIDs.Add("item_024");
        magicBuild.itemIDs.Add("item_025");
        magicBuild.popularity = 750;
        magicBuild.winRate = 51.8f;
        allBuilds.Add(magicBuild);
        
        EquipmentBuild assassinBuild = new EquipmentBuild("build_004", "刺客出装", "hero_004", "Assassin");
        assassinBuild.itemIDs.Add("item_030");
        assassinBuild.itemIDs.Add("item_031");
        assassinBuild.itemIDs.Add("item_032");
        assassinBuild.itemIDs.Add("item_033");
        assassinBuild.itemIDs.Add("item_034");
        assassinBuild.itemIDs.Add("item_035");
        assassinBuild.popularity = 600;
        assassinBuild.winRate = 49.3f;
        allBuilds.Add(assassinBuild);
        
        SaveBuilds();
    }
    
    public void UpdateGameContext(GameContext context)
    {
        currentGameContext = context;
        GenerateRecommendations();
    }
    
    public void GenerateRecommendations()
    {
        currentRecommendations.Clear();
        
        if (currentGameContext.currentPhase == "early")
        {
            GenerateEarlyGameRecommendations();
        }
        else if (currentGameContext.currentPhase == "mid")
        {
            GenerateMidGameRecommendations();
        }
        else if (currentGameContext.currentPhase == "late")
        {
            GenerateLateGameRecommendations();
        }
        
        ApplyContextBasedRecommendations();
        SortRecommendationsByPriority();
    }
    
    private void GenerateEarlyGameRecommendations()
    {
        currentRecommendations.Add(new ItemRecommendation("item_001", "铁剑", "增加物理攻击", 1));
        currentRecommendations.Add(new ItemRecommendation("item_002", "鞋子", "增加移速", 1));
        currentRecommendations.Add(new ItemRecommendation("item_003", "匕首", "增加攻击速度", 2));
    }
    
    private void GenerateMidGameRecommendations()
    {
        currentRecommendations.Add(new ItemRecommendation("item_004", "风暴巨剑", "增加物理攻击", 1));
        currentRecommendations.Add(new ItemRecommendation("item_005", "破军", "增加物理攻击和暴击", 1));
        currentRecommendations.Add(new ItemRecommendation("item_006", "无尽战刃", "增加暴击伤害", 2));
    }
    
    private void GenerateLateGameRecommendations()
    {
        currentRecommendations.Add(new ItemRecommendation("item_007", "泣血之刃", "增加物理攻击和吸血", 1));
        currentRecommendations.Add(new ItemRecommendation("item_008", "闪电匕首", "增加攻速和移速", 2));
        currentRecommendations.Add(new ItemRecommendation("item_009", "碎星锤", "增加物理穿透", 1));
    }
    
    private void ApplyContextBasedRecommendations()
    {
        if (currentGameContext.enemyHeroIDs.Count > 0)
        {
            AdjustRecommendationsForEnemyTeam();
        }
        
        if (currentGameContext.isWinning)
        {
            AdjustRecommendationsForWinning();
        }
        else
        {
            AdjustRecommendationsForLosing();
        }
    }
    
    private void AdjustRecommendationsForEnemyTeam()
    {
        foreach (ItemRecommendation rec in currentRecommendations)
        {
            if (rec.itemID.Contains("armor") || rec.itemID.Contains("defense"))
            {
                rec.score += 10;
                rec.reason += " (针对敌方阵容)";
            }
        }
    }
    
    private void AdjustRecommendationsForWinning()
    {
        foreach (ItemRecommendation rec in currentRecommendations)
        {
            if (rec.itemID.Contains("attack"))
            {
                rec.score += 5;
            }
        }
    }
    
    private void AdjustRecommendationsForLosing()
    {
        foreach (ItemRecommendation rec in currentRecommendations)
        {
            if (rec.itemID.Contains("defense"))
            {
                rec.score += 8;
            }
        }
    }
    
    private void SortRecommendationsByPriority()
    {
        currentRecommendations.Sort((a, b) => b.score.CompareTo(a.score));
    }
    
    public List<EquipmentBuild> GetBuildsForHero(string heroID)
    {
        return allBuilds.FindAll(b => b.heroID == heroID);
    }
    
    public List<EquipmentBuild> GetBuildsByType(string buildType)
    {
        return allBuilds.FindAll(b => b.buildType == buildType);
    }
    
    public List<EquipmentBuild> GetPopularBuilds(int count = 10)
    {
        List<EquipmentBuild> sorted = new List<EquipmentBuild>(allBuilds);
        sorted.Sort((a, b) => b.popularity.CompareTo(a.popularity));
        
        if (count > sorted.Count)
        {
            count = sorted.Count;
        }
        
        return sorted.GetRange(0, count);
    }
    
    public List<EquipmentBuild> GetHighWinRateBuilds(int count = 10)
    {
        List<EquipmentBuild> sorted = new List<EquipmentBuild>(allBuilds);
        sorted.Sort((a, b) => b.winRate.CompareTo(a.winRate));
        
        if (count > sorted.Count)
        {
            count = sorted.Count;
        }
        
        return sorted.GetRange(0, count);
    }
    
    public EquipmentBuild GetBuildByID(string buildID)
    {
        return allBuilds.Find(b => b.buildID == buildID);
    }
    
    public List<ItemRecommendation> GetCurrentRecommendations()
    {
        return currentRecommendations;
    }
    
    public void RecordBuildUsage(string buildID, bool won)
    {
        EquipmentBuild build = GetBuildByID(buildID);
        if (build != null)
        {
            build.popularity++;
            if (won)
            {
                int totalWins = (int)(build.winRate * (build.popularity - 1) / 100);
                totalWins++;
                build.winRate = (float)totalWins / build.popularity * 100;
            }
            
            SaveBuilds();
        }
    }
    
    public void CreateCustomBuild(EquipmentBuild build)
    {
        build.buildID = $"build_{System.DateTime.Now.Ticks}";
        allBuilds.Add(build);
        SaveBuilds();
    }
    
    public void SaveBuilds()
    {
        string path = Application.dataPath + "/Data/equipment_builds.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, allBuilds);
        stream.Close();
    }
    
    public void LoadBuilds()
    {
        string path = Application.dataPath + "/Data/equipment_builds.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            allBuilds = (List<EquipmentBuild>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            allBuilds = new List<EquipmentBuild>();
        }
    }
    
    public void SaveRecommendations()
    {
        string path = Application.dataPath + "/Data/recommendations.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, currentRecommendations);
        stream.Close();
    }
    
    public void LoadRecommendations()
    {
        string path = Application.dataPath + "/Data/recommendations.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            currentRecommendations = (List<ItemRecommendation>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            currentRecommendations = new List<ItemRecommendation>();
        }
    }
}
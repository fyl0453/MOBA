using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MatchStatsManager : MonoBehaviour
{
    public static MatchStatsManager Instance { get; private set; }
    
    public List<MatchStatsAnalysis> matchHistory;
    public Dictionary<string, HeroPerformanceSummary> heroPerformance;
    
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
        LoadMatchHistory();
        LoadHeroPerformance();
        
        if (matchHistory == null)
        {
            matchHistory = new List<MatchStatsAnalysis>();
        }
        
        if (heroPerformance == null)
        {
            heroPerformance = new Dictionary<string, HeroPerformanceSummary>();
        }
    }
    
    public void RecordMatchStats(MatchStatsAnalysis stats)
    {
        stats.CalculateKDA();
        stats.CalculatePerformanceScore();
        
        matchHistory.Insert(0, stats);
        
        if (matchHistory.Count > 100)
        {
            matchHistory.RemoveAt(matchHistory.Count - 1);
        }
        
        UpdateHeroPerformance(stats);
        SaveMatchHistory();
        SaveHeroPerformance();
    }
    
    private void UpdateHeroPerformance(MatchStatsAnalysis stats)
    {
        if (heroPerformance.ContainsKey(stats.heroID))
        {
            heroPerformance[stats.heroID].UpdateSummary(stats);
        }
        else
        {
            HeroPerformanceSummary summary = new HeroPerformanceSummary(stats.heroID, stats.heroName);
            summary.UpdateSummary(stats);
            heroPerformance[stats.heroID] = summary;
        }
    }
    
    public List<MatchStatsAnalysis> GetMatchHistory(int count = 20)
    {
        if (count > matchHistory.Count)
        {
            count = matchHistory.Count;
        }
        
        return matchHistory.GetRange(0, count);
    }
    
    public List<MatchStatsAnalysis> GetMatchHistoryByHero(string heroID)
    {
        return matchHistory.FindAll(m => m.heroID == heroID);
    }
    
    public List<MatchStatsAnalysis> GetMatchHistoryByMode(string gameMode)
    {
        return matchHistory.FindAll(m => m.gameMode == gameMode);
    }
    
    public HeroPerformanceSummary GetHeroPerformance(string heroID)
    {
        if (heroPerformance.ContainsKey(heroID))
        {
            return heroPerformance[heroID];
        }
        return null;
    }
    
    public Dictionary<string, HeroPerformanceSummary> GetAllHeroPerformance()
    {
        return heroPerformance;
    }
    
    public MatchStatsAnalysis GetMatchDetails(string matchID)
    {
        return matchHistory.Find(m => m.matchID == matchID);
    }
    
    public List<MatchStatsAnalysis> GetRecentImprovements(int count = 5)
    {
        List<MatchStatsAnalysis> recentMatches = GetMatchHistory(count);
        recentMatches.Sort((a, b) => b.performanceScore.CompareTo(a.performanceScore));
        return recentMatches;
    }
    
    public List<MatchStatsAnalysis> GetBestPerformances(int count = 10)
    {
        List<MatchStatsAnalysis> sorted = new List<MatchStatsAnalysis>(matchHistory);
        sorted.Sort((a, b) => b.performanceScore.CompareTo(a.performanceScore));
        
        if (count > sorted.Count)
        {
            count = sorted.Count;
        }
        
        return sorted.GetRange(0, count);
    }
    
    public string GetOverallWinRate()
    {
        if (matchHistory.Count == 0)
        {
            return "0%";
        }
        
        int wins = matchHistory.FindAll(m => m.result == "Victory").Count;
        float winRate = (float)wins / matchHistory.Count * 100;
        return $"{winRate:F1}%";
    }
    
    public string GetAverageKDA()
    {
        if (matchHistory.Count == 0)
        {
            return "0.00";
        }
        
        float totalKDA = 0;
        foreach (MatchStatsAnalysis stats in matchHistory)
        {
            totalKDA += stats.kda;
        }
        
        return $"{(totalKDA / matchHistory.Count):F2}";
    }
    
    public void SaveMatchHistory()
    {
        string path = Application.dataPath + "/Data/match_history.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, matchHistory);
        stream.Close();
    }
    
    public void LoadMatchHistory()
    {
        string path = Application.dataPath + "/Data/match_history.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            matchHistory = (List<MatchStatsAnalysis>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            matchHistory = new List<MatchStatsAnalysis>();
        }
    }
    
    public void SaveHeroPerformance()
    {
        string path = Application.dataPath + "/Data/hero_performance.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, heroPerformance);
        stream.Close();
    }
    
    public void LoadHeroPerformance()
    {
        string path = Application.dataPath + "/Data/hero_performance.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            heroPerformance = (Dictionary<string, HeroPerformanceSummary>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            heroPerformance = new Dictionary<string, HeroPerformanceSummary>();
        }
    }
    
    public void ExportMatchStats(string matchID)
    {
        MatchStatsAnalysis stats = GetMatchDetails(matchID);
        if (stats != null)
        {
            string exportPath = Application.dataPath + $"/Exports/match_{matchID}_stats.txt";
            string dir = Path.GetDirectoryName(exportPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            string content = GenerateStatsReport(stats);
            File.WriteAllText(exportPath, content);
            Debug.Log($"比赛数据已导出到: {exportPath}");
        }
    }
    
    private string GenerateStatsReport(MatchStatsAnalysis stats)
    {
        string report = $"=== 比赛数据分析报告 ===\n";
        report += $"比赛ID: {stats.matchID}\n";
        report += $"英雄: {stats.heroName}\n";
        report += $"模式: {stats.gameMode}\n";
        report += $"结果: {stats.result}\n";
        report += $"时长: {stats.duration}\n\n";
        
        report += $"=== 个人数据 ===\n";
        report += $"击杀: {stats.kills}\n";
        report += $"死亡: {stats.deaths}\n";
        report += $"助攻: {stats.assists}\n";
        report += $"KDA: {stats.kda:F2}\n\n";
        
        report += $"=== 战斗数据 ===\n";
        report += $"总伤害: {stats.totalDamageDealt:F0}\n";
        report += $"承受伤害: {stats.totalDamageTaken:F0}\n";
        report += $"治疗量: {stats.totalHealing:F0}\n";
        report += $"护盾量: {stats.totalShielding:F0}\n\n";
        
        report += $"=== 经济数据 ===\n";
        report += $"获得金币: {stats.goldEarned:F0}\n";
        report += $"消耗金币: {stats.goldSpent:F0}\n\n";
        
        report += $"=== 目标数据 ===\n";
        report += $"推塔数: {stats.towersDestroyed}\n";
        report += $"小龙: {stats.dragonsKilled}\n";
        report += $"大龙: {stats.baronsKilled}\n\n";
        
        report += $"=== 表现评分 ===\n";
        report += $"评分: {stats.performanceScore:F1}\n";
        report += $"等级: {stats.performanceGrade}\n";
        
        return report;
    }
}
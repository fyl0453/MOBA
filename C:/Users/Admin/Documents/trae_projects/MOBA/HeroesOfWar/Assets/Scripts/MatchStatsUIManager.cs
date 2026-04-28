using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MatchStatsUIManager : MonoBehaviour
{
    public static MatchStatsUIManager Instance { get; private set; }
    
    public Canvas matchStatsCanvas;
    public Text overviewText;
    public ScrollRect matchListScrollRect;
    public Transform matchListContent;
    public Text heroPerformanceText;
    public GameObject matchItemPrefab;
    public Button closeButton;
    public Dropdown filterDropdown;
    
    private string currentFilter = "All";
    
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
        matchStatsCanvas.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseMatchStatsUI);
        filterDropdown.onValueChanged.AddListener(OnFilterChanged);
    }
    
    public void OpenMatchStatsUI()
    {
        matchStatsCanvas.gameObject.SetActive(true);
        UpdateOverview();
        UpdateMatchList();
        UpdateHeroPerformance();
    }
    
    public void CloseMatchStatsUI()
    {
        matchStatsCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateOverview()
    {
        string winRate = MatchStatsManager.Instance.GetOverallWinRate();
        string avgKDA = MatchStatsManager.Instance.GetAverageKDA();
        overviewText.text = $"总胜率: {winRate} | 平均KDA: {avgKDA}";
    }
    
    public void UpdateMatchList()
    {
        foreach (Transform child in matchListContent)
        {
            Destroy(child.gameObject);
        }
        
        List<MatchStatsAnalysis> matches;
        if (currentFilter == "All")
        {
            matches = MatchStatsManager.Instance.GetMatchHistory(20);
        }
        else
        {
            matches = MatchStatsManager.Instance.GetMatchHistoryByMode(currentFilter);
        }
        
        foreach (MatchStatsAnalysis match in matches)
        {
            GameObject matchItem = Instantiate(matchItemPrefab, matchListContent);
            
            Text[] texts = matchItem.GetComponentsInChildren<Text>();
            if (texts.Length >= 6)
            {
                texts[0].text = match.heroName;
                texts[1].text = match.gameMode;
                texts[2].text = match.result;
                texts[3].text = $"{match.kills}/{match.deaths}/{match.assists}";
                texts[4].text = match.performanceGrade;
                texts[5].text = match.duration;
            }
            
            Button matchButton = matchItem.GetComponent<Button>();
            string matchID = match.matchID;
            matchButton.onClick.AddListener(() => ShowMatchDetails(matchID));
        }
    }
    
    public void UpdateHeroPerformance()
    {
        Dictionary<string, HeroPerformanceSummary> performance = MatchStatsManager.Instance.GetAllHeroPerformance();
        
        string performanceText = "英雄表现概要\n";
        foreach (KeyValuePair<string, HeroPerformanceSummary> kvp in performance)
        {
            HeroPerformanceSummary summary = kvp.Value;
            performanceText += $"{summary.heroName}: {summary.wins}/{summary.losses} (胜率{summary.winRate:F1}%)\n";
        }
        
        heroPerformanceText.text = performanceText;
    }
    
    public void ShowMatchDetails(string matchID)
    {
        MatchStatsAnalysis match = MatchStatsManager.Instance.GetMatchDetails(matchID);
        if (match != null)
        {
            Debug.Log($"显示比赛详情: {matchID}");
            OpenMatchDetailsPanel(match);
        }
    }
    
    public void OpenMatchDetailsPanel(MatchStatsAnalysis match)
    {
        Debug.Log($"=== 比赛详情 ===");
        Debug.Log($"英雄: {match.heroName}");
        Debug.Log($"模式: {match.gameMode}");
        Debug.Log($"结果: {match.result}");
        Debug.Log($"时长: {match.duration}");
        Debug.Log($"KDA: {match.kills}/{match.deaths}/{match.assists}");
        Debug.Log($"总伤害: {match.totalDamageDealt:F0}");
        Debug.Log($"承受伤害: {match.totalDamageTaken:F0}");
        Debug.Log($"治疗量: {match.totalHealing:F0}");
        Debug.Log($"评分: {match.performanceScore:F1} ({match.performanceGrade})");
    }
    
    public void OnFilterChanged(int index)
    {
        switch (index)
        {
            case 0:
                currentFilter = "All";
                break;
            case 1:
                currentFilter = "Classic";
                break;
            case 2:
                currentFilter = "Ranked";
                break;
            case 3:
                currentFilter = "Arcade";
                break;
        }
        UpdateMatchList();
    }
    
    public void ShowBestPerformances()
    {
        List<MatchStatsAnalysis> bestMatches = MatchStatsManager.Instance.GetBestPerformances(10);
        Debug.Log("=== 最佳表现 ===");
        foreach (MatchStatsAnalysis match in bestMatches)
        {
            Debug.Log($"{match.heroName}: {match.performanceScore:F1} ({match.performanceGrade})");
        }
    }
    
    public void ExportMatchStats(string matchID)
    {
        MatchStatsManager.Instance.ExportMatchStats(matchID);
    }
}
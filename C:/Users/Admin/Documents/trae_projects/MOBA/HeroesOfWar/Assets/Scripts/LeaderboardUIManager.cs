using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardUIManager : MonoBehaviour
{
    public static LeaderboardUIManager Instance { get; private set; }
    
    public Canvas leaderboardCanvas;
    public ScrollRect leaderboardScrollRect;
    public Transform leaderboardContent;
    public Text playerRankText;
    public Text playerPointsText;
    public Text playerWinsText;
    public Text playerKdaText;
    public GameObject leaderboardEntryPrefab;
    
    private LeaderboardManager.LeaderboardType currentType = LeaderboardManager.LeaderboardType.Ranked;
    
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
        leaderboardCanvas.gameObject.SetActive(false);
    }
    
    public void OpenLeaderboardUI()
    {
        leaderboardCanvas.gameObject.SetActive(true);
        UpdateLeaderboardList();
        UpdatePlayerRankInfo();
    }
    
    public void CloseLeaderboardUI()
    {
        leaderboardCanvas.gameObject.SetActive(false);
    }
    
    public void SetLeaderboardType(int typeIndex)
    {
        switch (typeIndex)
        {
            case 0:
                currentType = LeaderboardManager.LeaderboardType.Ranked;
                break;
            case 1:
                currentType = LeaderboardManager.LeaderboardType.Wins;
                break;
            case 2:
                currentType = LeaderboardManager.LeaderboardType.KDA;
                break;
            case 3:
                currentType = LeaderboardManager.LeaderboardType.GamesPlayed;
                break;
        }
        UpdateLeaderboardList();
    }
    
    public void UpdateLeaderboardList()
    {
        // 清空现有内容
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示排行榜数据
        List<LeaderboardEntry> entries = LeaderboardManager.Instance.GetLeaderboard(currentType);
        foreach (LeaderboardEntry entry in entries)
        {
            GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            Text[] texts = entryObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 5)
            {
                texts[0].text = entry.rank.ToString();
                texts[1].text = entry.playerName;
                texts[2].text = entry.rankPoints.ToString();
                texts[3].text = entry.wins.ToString();
                texts[4].text = entry.kda.ToString("F1");
            }
        }
    }
    
    public void UpdatePlayerRankInfo()
    {
        // 显示当前玩家的排名信息
        string playerName = "LocalPlayer";
        int rank = LeaderboardManager.Instance.GetPlayerRank(playerName);
        
        if (rank != -1)
        {
            playerRankText.text = "排名: " + rank;
            playerPointsText.text = "积分: 1000";
            playerWinsText.text = "胜利: 10";
            playerKdaText.text = "KDA: 3.5";
        }
        else
        {
            playerRankText.text = "未上榜";
            playerPointsText.text = "";
            playerWinsText.text = "";
            playerKdaText.text = "";
        }
    }
}
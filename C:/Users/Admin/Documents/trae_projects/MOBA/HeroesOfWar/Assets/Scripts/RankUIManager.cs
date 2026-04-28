using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankUIManager : MonoBehaviour
{
    public static RankUIManager Instance { get; private set; }
    
    public Canvas rankCanvas;
    public Text seasonInfoText;
    public Text currentRankText;
    public Text rankProgressText;
    public ScrollRect seasonHistoryScrollRect;
    public Transform seasonHistoryContent;
    public Button closeButton;
    public Button claimRewardButton;
    public GameObject divisionPrefab;
    public GameObject seasonHistoryItemPrefab;
    
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
        rankCanvas.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseRankUI);
        claimRewardButton.onClick.AddListener(ClaimSeasonReward);
    }
    
    public void OpenRankUI()
    {
        rankCanvas.gameObject.SetActive(true);
        UpdateSeasonInfo();
        UpdateCurrentRank();
        UpdateRankProgress();
        UpdateSeasonHistory();
        CheckUnclaimedRewards();
    }
    
    public void CloseRankUI()
    {
        rankCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateSeasonInfo()
    {
        RankSeason currentSeason = RankManager.Instance.GetCurrentSeason();
        if (currentSeason != null)
        {
            seasonInfoText.text = currentSeason.seasonName;
        }
    }
    
    public void UpdateCurrentRank()
    {
        PlayerRank rank = RankManager.Instance.playerRank;
        RankDivision division = RankManager.Instance.GetCurrentDivision();
        
        if (rank != null && division != null)
        {
            string protectionText = rank.protectedStatus ? " (保护中)" : "";
            currentRankText.text = $"{division.divisionName} | {rank.stars}星 | 胜率{rank.winRate:F1}%{protectionText}";
        }
    }
    
    public void UpdateRankProgress()
    {
        int starsToNext = RankManager.Instance.GetStarsToNextDivision();
        float progress = RankManager.Instance.GetProgressToNextDivision();
        
        rankProgressText.text = $"距离下一段位: {starsToNext}星 ({progress * 100:F0}%)";
    }
    
    public void UpdateSeasonHistory()
    {
        foreach (Transform child in seasonHistoryContent)
        {
            Destroy(child.gameObject);
        }
        
        List<RankSeason> allSeasons = RankManager.Instance.allSeasons;
        allSeasons.Sort((a, b) => b.seasonNumber.CompareTo(a.seasonNumber));
        
        foreach (RankSeason season in allSeasons)
        {
            GameObject historyItem = Instantiate(seasonHistoryItemPrefab, seasonHistoryContent);
            
            Text[] texts = historyItem.GetComponentsInChildren<Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = season.seasonName;
                texts[1].text = season.isEnded ? "已结束" : "进行中";
                texts[2].text = season.isActive ? "当前" : "";
            }
        }
    }
    
    public void CheckUnclaimedRewards()
    {
        List<SeasonReward> unclaimedRewards = RankManager.Instance.GetUnclaimedRewards();
        if (unclaimedRewards.Count > 0)
        {
            claimRewardButton.gameObject.SetActive(true);
        }
        else
        {
            claimRewardButton.gameObject.SetActive(false);
        }
    }
    
    public void ClaimSeasonReward()
    {
        List<SeasonReward> unclaimedRewards = RankManager.Instance.GetUnclaimedRewards();
        if (unclaimedRewards.Count > 0)
        {
            foreach (SeasonReward reward in unclaimedRewards)
            {
                RankManager.Instance.ClaimReward(reward.rewardID);
            }
            
            Debug.Log("已领取赛季奖励");
            CheckUnclaimedRewards();
        }
    }
    
    public void ShowDivisionDetails(string divisionID)
    {
        RankDivision division = RankManager.Instance.allDivisions.Find(d => d.divisionID == divisionID);
        if (division != null)
        {
            Debug.Log($"=== {division.divisionName} ===");
            Debug.Log($"所需星数: {division.minStars}-{division.maxStars}");
        }
    }
    
    public void ShowSeasonRewards(string seasonID)
    {
        Debug.Log($"显示{seasonID}的赛季奖励");
    }
    
    public void StartRankedMatch()
    {
        Debug.Log("开始排位赛匹配");
    }
}
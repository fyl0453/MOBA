using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardPanel : UIPanel
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button rankButton;
    [SerializeField] private Button killsButton;
    [SerializeField] private Button winrateButton;
    [SerializeField] private Button kdaButton;
    [SerializeField] private Button goldButton;
    [SerializeField] private Button levelButton;
    
    [SerializeField] private GameObject leaderboardContent;
    [SerializeField] private GameObject leaderboardItemPrefab;
    [SerializeField] private Text leaderboardTitleText;
    
    private List<LeaderboardItem> leaderboardItems = new List<LeaderboardItem>();
    
    private void Start()
    {
        backButton.onClick.AddListener(OnBack);
        rankButton.onClick.AddListener(() => ShowLeaderboard("rank"));
        killsButton.onClick.AddListener(() => ShowLeaderboard("kills"));
        winrateButton.onClick.AddListener(() => ShowLeaderboard("winrate"));
        kdaButton.onClick.AddListener(() => ShowLeaderboard("kda"));
        goldButton.onClick.AddListener(() => ShowLeaderboard("gold"));
        levelButton.onClick.AddListener(() => ShowLeaderboard("level"));
        
        // 默认显示段位排行榜
        ShowLeaderboard("rank");
    }
    
    private void ShowLeaderboard(string leaderboardID)
    {
        // 清空现有列表
        foreach (var item in leaderboardItems)
        {
            Destroy(item.gameObject);
        }
        leaderboardItems.Clear();
        
        // 获取排行榜数据
        Leaderboard leaderboard = LeaderboardSystem.Instance.GetLeaderboard(leaderboardID);
        if (leaderboard != null)
        {
            leaderboardTitleText.text = leaderboard.leaderboardName;
            
            List<LeaderboardEntry> entries = LeaderboardSystem.Instance.GetLeaderboardEntries(leaderboardID, 50);
            foreach (LeaderboardEntry entry in entries)
            {
                GameObject leaderboardItemGO = Instantiate(leaderboardItemPrefab, leaderboardContent.transform);
                LeaderboardItem leaderboardItem = leaderboardItemGO.GetComponent<LeaderboardItem>();
                if (leaderboardItem != null)
                {
                    leaderboardItem.Setup(entry, leaderboardID);
                    leaderboardItems.Add(leaderboardItem);
                }
            }
        }
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private Text rankText;
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text valueText;
    
    public void Setup(LeaderboardEntry entry, string leaderboardID)
    {
        rankText.text = entry.rank.ToString();
        playerNameText.text = entry.playerName;
        
        // 根据排行榜类型显示不同的单位
        switch (leaderboardID)
        {
            case "rank":
                valueText.text = entry.value.ToString();
                break;
            case "kills":
                valueText.text = entry.value.ToString() + " 击杀";
                break;
            case "winrate":
                valueText.text = entry.value.ToString() + "%";
                break;
            case "kda":
                valueText.text = entry.value.ToString("F1");
                break;
            case "gold":
                valueText.text = entry.value.ToString() + " 金币";
                break;
            case "level":
                valueText.text = entry.value.ToString() + " 级";
                break;
            default:
                valueText.text = entry.value.ToString();
                break;
        }
    }
}

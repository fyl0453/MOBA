using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpectatorPanel : UIPanel
{
    [SerializeField] private GameObject matchItemPrefab;
    [SerializeField] private Transform matchListContent;
    [SerializeField] private Button backButton;
    [SerializeField] private Text noMatchesText;
    
    private List<MatchItem> matchItems = new List<MatchItem>();
    
    private void Start()
    {
        backButton.onClick.AddListener(OnBack);
        RefreshMatchList();
    }
    
    public void RefreshMatchList()
    {
        // 清空现有列表
        foreach (var item in matchItems)
        {
            Destroy(item.gameObject);
        }
        matchItems.Clear();
        
        // 从SpectatorSystem获取正在进行的比赛
        if (SpectatorSystem.Instance != null)
        {
            List<GameMatch> matches = SpectatorSystem.Instance.GetOngoingMatches();
            
            if (matches.Count > 0)
            {
                noMatchesText.gameObject.SetActive(false);
                
                foreach (GameMatch match in matches)
                {
                    GameObject matchItemGO = Instantiate(matchItemPrefab, matchListContent);
                    MatchItem matchItem = matchItemGO.GetComponent<MatchItem>();
                    if (matchItem != null)
                    {
                        matchItem.Setup(match, OnMatchSelect);
                        matchItems.Add(matchItem);
                    }
                }
            }
            else
            {
                noMatchesText.gameObject.SetActive(true);
            }
        }
    }
    
    private void OnMatchSelect(GameMatch match)
    {
        // 开始观战
        if (SpectatorSystem.Instance != null)
        {
            bool success = SpectatorSystem.Instance.StartSpectating(match.matchID);
            if (success)
            {
                Debug.Log("开始观战比赛: " + match.matchName);
                // 这里可以切换到观战视角
            }
        }
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

public class MatchItem : MonoBehaviour
{
    [SerializeField] private Text matchNameText;
    [SerializeField] private Text matchStatusText;
    [SerializeField] private Text matchTimeText;
    [SerializeField] private Button spectateButton;
    
    private GameMatch match;
    private System.Action<GameMatch> onSelectCallback;
    
    public void Setup(GameMatch m, System.Action<GameMatch> callback)
    {
        match = m;
        matchNameText.text = m.matchName;
        matchStatusText.text = m.status.ToString();
        matchTimeText.text = "时间: " + m.matchTime.ToString("F1") + "秒";
        onSelectCallback = callback;
        
        spectateButton.onClick.AddListener(OnSelect);
    }
    
    private void OnSelect()
    {
        onSelectCallback?.Invoke(match);
    }
}

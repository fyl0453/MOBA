using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpectatorUIManager : MonoBehaviour
{
    public static SpectatorUIManager Instance { get; private set; }
    
    public Canvas spectatorCanvas;
    public ScrollRect matchesScrollRect;
    public Transform matchesContent;
    public Button spectateButton;
    public GameObject matchPrefab;
    
    private string selectedMatchID;
    
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
        spectatorCanvas.gameObject.SetActive(false);
        spectateButton.onClick.AddListener(StartSpectating);
    }
    
    public void OpenSpectatorUI()
    {
        spectatorCanvas.gameObject.SetActive(true);
        UpdateMatchesList();
    }
    
    public void CloseSpectatorUI()
    {
        spectatorCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateMatchesList()
    {
        // 清空现有内容
        foreach (Transform child in matchesContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示可用的比赛列表
        List<MatchInfo> matches = SpectatorManager.Instance.GetAvailableMatches();
        foreach (MatchInfo match in matches)
        {
            GameObject matchObj = Instantiate(matchPrefab, matchesContent);
            Text[] texts = matchObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 4)
            {
                texts[0].text = match.matchType;
                texts[1].text = match.status;
                texts[2].text = $"游戏时间: {match.gameTime}秒";
                texts[3].text = $"玩家数: {match.playerCount}/10";
            }
            
            // 添加点击事件
            Button button = matchObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectMatch(match.matchID));
            }
        }
    }
    
    public void SelectMatch(string matchID)
    {
        selectedMatchID = matchID;
        MatchInfo match = SpectatorManager.Instance.GetMatchInfo(matchID);
        if (match != null)
        {
            Debug.Log($"选择观战比赛: {match.matchType}");
        }
    }
    
    public void StartSpectating()
    {
        if (!string.IsNullOrEmpty(selectedMatchID))
        {
            SpectatorManager.Instance.StartSpectating(selectedMatchID);
        }
    }
    
    public void StopSpectating()
    {
        SpectatorManager.Instance.StopSpectating();
    }
    
    public void FollowPlayer(string playerID)
    {
        SpectatorManager.Instance.FollowPlayer(playerID);
    }
    
    public void SetCameraHeight(float height)
    {
        SpectatorManager.Instance.SetCameraHeight(height);
    }
    
    public void SetCameraDistance(float distance)
    {
        SpectatorManager.Instance.SetCameraDistance(distance);
    }
}
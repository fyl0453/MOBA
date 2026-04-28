using UnityEngine;
using UnityEngine.UI;

public class MatchmakingUIManager : MonoBehaviour
{
    public static MatchmakingUIManager Instance { get; private set; }
    
    public Canvas matchmakingCanvas;
    public Text matchStatusText;
    public Slider matchmakingProgressSlider;
    public Button cancelButton;
    public Button normalMatchButton;
    public Button rankedMatchButton;
    public Button arcadeMatchButton;
    public Button customMatchButton;
    
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
        matchmakingCanvas.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        
        // 添加按钮点击事件
        normalMatchButton.onClick.AddListener(() => StartMatchmaking(MatchmakingManager.MatchType.Normal));
        rankedMatchButton.onClick.AddListener(() => StartMatchmaking(MatchmakingManager.MatchType.Ranked));
        arcadeMatchButton.onClick.AddListener(() => StartMatchmaking(MatchmakingManager.MatchType.Arcade));
        customMatchButton.onClick.AddListener(() => StartMatchmaking(MatchmakingManager.MatchType.Custom));
        cancelButton.onClick.AddListener(CancelMatchmaking);
    }
    
    private void Update()
    {
        // 更新匹配状态
        MatchmakingManager.MatchStatus status = MatchmakingManager.Instance.GetMatchStatus();
        switch (status)
        {
            case MatchmakingManager.MatchStatus.Idle:
                matchStatusText.text = "准备开始匹配";
                matchmakingProgressSlider.value = 0;
                cancelButton.gameObject.SetActive(false);
                break;
            case MatchmakingManager.MatchStatus.Waiting:
                matchStatusText.text = "正在寻找匹配...";
                float progress = MatchmakingManager.Instance.GetMatchmakingProgress();
                matchmakingProgressSlider.value = progress;
                cancelButton.gameObject.SetActive(true);
                break;
            case MatchmakingManager.MatchStatus.Found:
                matchStatusText.text = "匹配成功！正在准备游戏...";
                matchmakingProgressSlider.value = 1;
                cancelButton.gameObject.SetActive(false);
                break;
            case MatchmakingManager.MatchStatus.Ready:
                matchStatusText.text = "游戏准备就绪";
                cancelButton.gameObject.SetActive(false);
                break;
            case MatchmakingManager.MatchStatus.InGame:
                matchStatusText.text = "游戏中";
                cancelButton.gameObject.SetActive(false);
                break;
        }
    }
    
    public void OpenMatchmakingUI()
    {
        matchmakingCanvas.gameObject.SetActive(true);
    }
    
    public void CloseMatchmakingUI()
    {
        matchmakingCanvas.gameObject.SetActive(false);
    }
    
    public void StartMatchmaking(MatchmakingManager.MatchType type)
    {
        MatchmakingManager.Instance.StartMatchmaking(type);
    }
    
    public void CancelMatchmaking()
    {
        MatchmakingManager.Instance.CancelMatchmaking();
    }
}
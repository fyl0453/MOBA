using UnityEngine;
using UnityEngine.UI;

public class ResultsPanel : UIPanel
{
    [SerializeField] private Text resultText;
    [SerializeField] private Text killsText;
    [SerializeField] private Text deathsText;
    [SerializeField] private Text assistsText;
    [SerializeField] private Text goldEarnedText;
    [SerializeField] private Text experienceText;
    [SerializeField] private Text rewardsText;
    
    [SerializeField] private Button continueButton;
    [SerializeField] private Button shareButton;
    [SerializeField] private Button reportButton;
    
    private bool isWin;
    private int kills;
    private int deaths;
    private int assists;
    private int goldEarned;
    
    private void Start()
    {
        continueButton.onClick.AddListener(OnContinue);
        shareButton.onClick.AddListener(OnShare);
        reportButton.onClick.AddListener(OnReport);
    }
    
    public void SetResult(bool win, int k, int d, int a, int gold)
    {
        isWin = win;
        kills = k;
        deaths = d;
        assists = a;
        goldEarned = gold;
        
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        resultText.text = isWin ? "胜利！" : "失败！";
        resultText.color = isWin ? Color.green : Color.red;
        
        killsText.text = "击杀: " + kills;
        deathsText.text = "死亡: " + deaths;
        assistsText.text = "助攻: " + assists;
        goldEarnedText.text = "获得金币: " + goldEarned;
        experienceText.text = "获得经验: " + (isWin ? 100 : 50);
        
        string rewards = isWin ? "金币: 500\n经验: 100" : "金币: 200\n经验: 50";
        rewardsText.text = "奖励:\n" + rewards;
    }
    
    private void OnContinue()
    {
        GameFlowManager.Instance.ReturnToMainMenu();
    }
    
    private void OnShare()
    {
        // 分享战斗结果
        Debug.Log("分享战斗结果");
    }
    
    private void OnReport()
    {
        // 举报功能
        Debug.Log("举报功能");
    }
}

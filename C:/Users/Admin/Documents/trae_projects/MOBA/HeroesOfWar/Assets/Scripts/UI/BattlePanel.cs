using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattlePanel : UIPanel
{
    [SerializeField] private Text timerText;
    [SerializeField] private Text killsText;
    [SerializeField] private Text deathsText;
    [SerializeField] private Text assistsText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text levelText;
    
    [SerializeField] private Button skill1Button;
    [SerializeField] private Button skill2Button;
    [SerializeField] private Button skill3Button;
    [SerializeField] private Button ultButton;
    [SerializeField] private Button summonerSpell1Button;
    [SerializeField] private Button summonerSpell2Button;
    
    [SerializeField] private Button retreatButton;
    [SerializeField] private Button surrenderButton;
    [SerializeField] private Button chatButton;
    [SerializeField] private Button settingsButton;
    
    private void Start()
    {
        skill1Button.onClick.AddListener(OnSkill1);
        skill2Button.onClick.AddListener(OnSkill2);
        skill3Button.onClick.AddListener(OnSkill3);
        ultButton.onClick.AddListener(OnUlt);
        summonerSpell1Button.onClick.AddListener(OnSummonerSpell1);
        summonerSpell2Button.onClick.AddListener(OnSummonerSpell2);
        
        retreatButton.onClick.AddListener(OnRetreat);
        surrenderButton.onClick.AddListener(OnSurrender);
        chatButton.onClick.AddListener(OnChat);
        settingsButton.onClick.AddListener(OnSettings);
    }
    
    public override void OnShow()
    {
        base.OnShow();
        StartBattleTimer();
    }
    
    private void StartBattleTimer()
    {
        // 开始战斗计时器
        Debug.Log("开始战斗计时器");
    }
    
    private void OnSkill1()
    {
        // 使用技能1
        Debug.Log("使用技能1");
    }
    
    private void OnSkill2()
    {
        // 使用技能2
        Debug.Log("使用技能2");
    }
    
    private void OnSkill3()
    {
        // 使用技能3
        Debug.Log("使用技能3");
    }
    
    private void OnUlt()
    {
        // 使用终极技能
        Debug.Log("使用终极技能");
    }
    
    private void OnSummonerSpell1()
    {
        // 使用召唤师技能1
        Debug.Log("使用召唤师技能1");
    }
    
    private void OnSummonerSpell2()
    {
        // 使用召唤师技能2
        Debug.Log("使用召唤师技能2");
    }
    
    private void OnRetreat()
    {
        // 撤退
        Debug.Log("撤退");
    }
    
    private void OnSurrender()
    {
        // 投降
        Debug.Log("投降");
    }
    
    private void OnChat()
    {
        // 打开聊天
        Debug.Log("打开聊天");
    }
    
    private void OnSettings()
    {
        // 打开设置
        Debug.Log("打开设置");
    }
    
    public void UpdateBattleInfo(int kills, int deaths, int assists, int gold, int level)
    {
        killsText.text = "击杀: " + kills;
        deathsText.text = "死亡: " + deaths;
        assistsText.text = "助攻: " + assists;
        goldText.text = "金币: " + gold;
        levelText.text = "等级: " + level;
    }
    
    public void UpdateTimer(string time)
    {
        timerText.text = time;
    }
}

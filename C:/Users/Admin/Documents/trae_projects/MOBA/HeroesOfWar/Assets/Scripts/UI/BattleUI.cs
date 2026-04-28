using UnityEngine;

public class BattleUI : MonoBehaviour
{
    public HealthBar healthBar;
    public SkillPanel skillPanel;
    public MiniMap miniMap;
    public Timer timer;
    
    private void Start()
    {
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        // 初始化UI组件
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
        }
        
        if (skillPanel != null)
        {
            skillPanel.gameObject.SetActive(true);
        }
        
        if (miniMap != null)
        {
            miniMap.gameObject.SetActive(true);
        }
        
        if (timer != null)
        {
            timer.gameObject.SetActive(true);
        }
    }
    
    public void ShowGameOverScreen()
    {
        // 显示游戏结束界面
        Debug.Log("Game Over");
    }
}
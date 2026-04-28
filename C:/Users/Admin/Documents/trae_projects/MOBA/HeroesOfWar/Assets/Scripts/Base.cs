using UnityEngine;

public class Base : MonoBehaviour
{
    public float health = 5000f;
    public float armor = 100f;
    public Tower.Team team;
    
    private bool isAlive = true;
    
    public void TakeDamage(float damage)
    {
        if (!isAlive)
            return;
        
        // 计算实际伤害
        float actualDamage = damage * (100f / (100f + armor));
        health -= actualDamage;
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        isAlive = false;
        Debug.Log($"{team}方基地被摧毁，游戏结束！");
        
        // 这里可以添加摧毁特效
        
        // 结束游戏
        EndGame();
    }
    
    private void EndGame()
    {
        // 确定游戏结果
        bool playerWon = team == Tower.Team.Red; // 假设玩家是红方
        
        // 调用GameFlowManager结束战斗
        GameFlowManager.Instance.EndBattle(playerWon, 0, 0, 0);
    }
}

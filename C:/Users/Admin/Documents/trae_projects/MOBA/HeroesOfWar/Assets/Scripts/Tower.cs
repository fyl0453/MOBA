using UnityEngine;

public class Tower : MonoBehaviour
{
    public float attackRange = 15f;
    public float attackDamage = 100f;
    public float attackInterval = 1f;
    public float health = 2000f;
    public float armor = 50f;
    public Team team;
    
    private float attackTimer = 0f;
    private GameObject target;
    private bool isAlive = true;
    
    public enum Team
    {
        Blue,
        Red
    }
    
    private void Update()
    {
        if (!isAlive)
            return;
        
        attackTimer += Time.deltaTime;
        
        if (attackTimer >= attackInterval)
        {
            FindTarget();
            if (target != null)
            {
                AttackTarget();
            }
            attackTimer = 0f;
        }
    }
    
    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);
        float closestDistance = Mathf.Infinity;
        GameObject closestTarget = null;
        
        foreach (Collider collider in colliders)
        {
            // 检查是否是敌方单位
            if (IsEnemy(collider.gameObject))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = collider.gameObject;
                }
            }
        }
        
        target = closestTarget;
    }
    
    private bool IsEnemy(GameObject obj)
    {
        // 检查是否是敌方英雄
        HeroController heroController = obj.GetComponent<HeroController>();
        if (heroController != null)
        {
            // 这里需要根据英雄的队伍来判断
            // 暂时假设所有英雄都是敌方
            return true;
        }
        
        // 检查是否是敌方小兵
        Minion minion = obj.GetComponent<Minion>();
        if (minion != null)
        {
            return minion.team != team;
        }
        
        return false;
    }
    
    private void AttackTarget()
    {
        if (target == null)
            return;
        
        // 计算伤害
        float damage = attackDamage;
        
        // 应用伤害
        HeroController heroController = target.GetComponent<HeroController>();
        if (heroController != null)
        {
            heroController.TakeDamage(damage);
        }
        
        Minion minion = target.GetComponent<Minion>();
        if (minion != null)
        {
            minion.TakeDamage(damage);
        }
        
        // 显示攻击特效
        Debug.Log($"防御塔攻击 {target.name}");
    }
    
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
        Debug.Log($"防御塔被摧毁");
        
        // 这里可以添加摧毁特效
        
        // 检查是否所有防御塔都被摧毁
        CheckBaseSafety();
    }
    
    private void CheckBaseSafety()
    {
        // 检查是否所有防御塔都被摧毁
        // 如果是，攻击基地
    }
}

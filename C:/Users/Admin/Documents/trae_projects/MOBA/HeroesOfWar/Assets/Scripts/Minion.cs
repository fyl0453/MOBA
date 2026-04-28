using UnityEngine;
using System.Collections.Generic;

public class Minion : MonoBehaviour
{
    public float health = 500f;
    public float armor = 10f;
    public float attackDamage = 20f;
    public float attackRange = 2f;
    public float attackInterval = 1f;
    public float moveSpeed = 3f;
    public Tower.Team team;
    
    private float attackTimer = 0f;
    private GameObject target;
    private List<Vector3> waypoints;
    private int currentWaypointIndex = 0;
    private bool isAlive = true;
    
    public void Initialize(List<Vector3> points)
    {
        waypoints = points;
    }
    
    private void Update()
    {
        if (!isAlive)
            return;
        
        if (target != null)
        {
            // 攻击目标
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                AttackTarget();
                attackTimer = 0f;
            }
        }
        else
        {
            // 寻找目标
            FindTarget();
            
            if (target == null)
            {
                // 移动到下一个路点
                MoveToNextWaypoint();
            }
        }
    }
    
    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);
        
        foreach (Collider collider in colliders)
        {
            if (IsEnemy(collider.gameObject))
            {
                target = collider.gameObject;
                return;
            }
        }
    }
    
    private bool IsEnemy(GameObject obj)
    {
        // 检查是否是敌方英雄
        HeroController heroController = obj.GetComponent<HeroController>();
        if (heroController != null)
        {
            return true;
        }
        
        // 检查是否是敌方防御塔
        Tower tower = obj.GetComponent<Tower>();
        if (tower != null)
        {
            return tower.team != team;
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
        
        Tower tower = target.GetComponent<Tower>();
        if (tower != null)
        {
            tower.TakeDamage(damage);
        }
        
        Minion minion = target.GetComponent<Minion>();
        if (minion != null)
        {
            minion.TakeDamage(damage);
        }
    }
    
    private void MoveToNextWaypoint()
    {
        if (waypoints == null || waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count)
            return;
        
        Vector3 targetPosition = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        // 面向目标方向
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // 到达路点
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex++;
        }
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
        Debug.Log($"小兵被击杀");
        
        // 这里可以添加死亡特效
        
        // 销毁游戏对象
        Destroy(gameObject, 1f);
    }
}

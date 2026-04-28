using UnityEngine;

public class Turret : MonoBehaviour
{
    public string turretName;
    public float maxHealth;
    public float currentHealth;
    public float attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float armor;
    public float magicResistance;
    public int teamId;
    
    public enum TurretType
    {
        Inner,
        Outer,
        Inhibitor,
        Nexus
    }
    
    public TurretType turretType;
    
    private float attackTimer = 0f;
    private GameObject currentTarget;
    private bool isAlive = true;
    
    private void Update()
    {
        if (!isAlive) return;
        
        attackTimer += Time.deltaTime;
        
        if (currentTarget == null)
        {
            FindTarget();
        }
        else
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distance > attackRange)
            {
                currentTarget = null;
            }
            else
            {
                if (attackTimer >= 1f / attackSpeed)
                {
                    Attack(currentTarget);
                    attackTimer = 0f;
                }
            }
        }
    }
    
    private void FindTarget()
    {
        // 优先攻击小兵
        Minion[] minions = FindObjectsOfType<Minion>();
        float closestDistance = attackRange;
        GameObject closestTarget = null;
        
        foreach (Minion minion in minions)
        {
            float distance = Vector3.Distance(transform.position, minion.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = minion.gameObject;
            }
        }
        
        if (closestTarget != null)
        {
            currentTarget = closestTarget;
            return;
        }
        
        // 然后攻击英雄
        HeroController[] heroes = FindObjectsOfType<HeroController>();
        foreach (HeroController hero in heroes)
        {
            float distance = Vector3.Distance(transform.position, hero.transform.position);
            if (distance < attackRange)
            {
                currentTarget = hero.gameObject;
                return;
            }
        }
    }
    
    private void Attack(GameObject target)
    {
        HeroController heroController = target.GetComponent<HeroController>();
        if (heroController != null)
        {
            float actualDamage = CalculateDamage(attackDamage, heroController.heroData.baseStats.armor);
            heroController.TakeDamage(actualDamage);
        }
        
        Minion minion = target.GetComponent<Minion>();
        if (minion != null)
        {
            float actualDamage = CalculateDamage(attackDamage, minion.armor);
            minion.TakeDamage(actualDamage);
        }
    }
    
    private float CalculateDamage(float damage, float armor)
    {
        return damage * (100f / (100f + armor));
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && isAlive)
        {
            Die();
        }
    }
    
    private void Die()
    {
        isAlive = false;
        gameObject.SetActive(false);
        Debug.Log($"{turretName} destroyed!");
    }
    
    public void Repair(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
}
[System.Serializable]
public class HeroStats
{
    public float maxHealth;
    public float currentHealth;
    public float attackDamage;
    public float attackSpeed;
    public float movementSpeed;
    public float armor;
    public float magicResistance;
    public float attackRange;
    
    public HeroStats(float health, float damage, float speed, float moveSpeed, float arm, float magicRes, float range)
    {
        maxHealth = health;
        currentHealth = health;
        attackDamage = damage;
        attackSpeed = speed;
        movementSpeed = moveSpeed;
        armor = arm;
        magicResistance = magicRes;
        attackRange = range;
    }
    
    public void TakeDamage(float damage)
    {
        float effectiveDamage = damage * (100 / (100 + armor));
        currentHealth = Mathf.Max(0, currentHealth - effectiveDamage);
    }
    
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
    
    public void ResetStats()
    {
        currentHealth = maxHealth;
    }
}
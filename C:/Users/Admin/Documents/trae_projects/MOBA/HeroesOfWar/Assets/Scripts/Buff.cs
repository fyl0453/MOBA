[System.Serializable]
public class Buff
{
    public string buffName;
    public string buffDescription;
    public float duration;
    public float currentDuration;
    public bool isStackable;
    public int maxStacks;
    public int currentStacks;
    
    public float attackDamageBonus;
    public float attackSpeedBonus;
    public float movementSpeedBonus;
    public float armorBonus;
    public float magicResistanceBonus;
    public float healthRegenBonus;
    public float manaRegenBonus;
    
    public enum BuffType
    {
        Attack,
        Defense,
        Utility,
        Special
    }
    
    public BuffType buffType;
    
    public Buff(string name, string description, float buffDuration, BuffType type)
    {
        buffName = name;
        buffDescription = description;
        duration = buffDuration;
        currentDuration = duration;
        buffType = type;
        
        isStackable = false;
        maxStacks = 1;
        currentStacks = 1;
        
        attackDamageBonus = 0;
        attackSpeedBonus = 0;
        movementSpeedBonus = 0;
        armorBonus = 0;
        magicResistanceBonus = 0;
        healthRegenBonus = 0;
        manaRegenBonus = 0;
    }
    
    public void ApplyEffect(HeroStats stats)
    {
        stats.attackDamage += attackDamageBonus * currentStacks;
        stats.attackSpeed += attackSpeedBonus * currentStacks;
        // 移动速度需要在HeroController中处理
    }
    
    public void RemoveEffect(HeroStats stats)
    {
        stats.attackDamage -= attackDamageBonus * currentStacks;
        stats.attackSpeed -= attackSpeedBonus * currentStacks;
    }
    
    public void Update(float deltaTime)
    {
        currentDuration -= deltaTime;
        if (currentDuration <= 0)
        {
            // Buff结束
        }
    }
    
    public void Stack()
    {
        if (isStackable && currentStacks < maxStacks)
        {
            currentStacks++;
            currentDuration = duration;
        }
        else if (!isStackable)
        {
            currentDuration = duration;
        }
    }
}
using UnityEngine;

[System.Serializable]
public class Rune
{
    public string runeName;
    public string runeDescription;
    public RuneType runeType;
    public RuneTier tier;
    public Sprite icon;
    
    public float attackDamageBonus;
    public float abilityPowerBonus;
    public float armorBonus;
    public float magicResistBonus;
    public float healthBonus;
    public float manaBonus;
    public float attackSpeedBonus;
    public float cooldownReductionBonus;
    public float movementSpeedBonus;
    public float lifeStealBonus;
    public float armorPenetrationBonus;
    public float magicPenetrationBonus;
    
    public enum RuneType
    {
        Mark,
        Seal,
        Glyph,
        Quintessence
    }
    
    public enum RuneTier
    {
        Tier1,
        Tier2,
        Tier3
    }
    
    public Rune(string name, string description, RuneType type, RuneTier runeTier)
    {
        runeName = name;
        runeDescription = description;
        runeType = type;
        tier = runeTier;
    }
    
    public virtual void ApplyEffect(HeroStats stats)
    {
        stats.attackDamage += attackDamageBonus;
        stats.armor += armorBonus;
        stats.magicResistance += magicResistBonus;
        stats.maxHealth += healthBonus;
    }
    
    public virtual void RemoveEffect(HeroStats stats)
    {
        stats.attackDamage -= attackDamageBonus;
        stats.armor -= armorBonus;
        stats.magicResistance -= magicResistBonus;
        stats.maxHealth -= healthBonus;
    }
}

public class AttackDamageRune : Rune
{
    public AttackDamageRune(RuneTier tier) : base("攻击符文", "增加攻击力", RuneType.Mark, tier)
    {
        switch (tier)
        {
            case RuneTier.Tier1:
                attackDamageBonus = 0.5f;
                break;
            case RuneTier.Tier2:
                attackDamageBonus = 1.0f;
                break;
            case RuneTier.Tier3:
                attackDamageBonus = 2.0f;
                break;
        }
    }
}

public class AbilityPowerRune : Rune
{
    public AbilityPowerRune(RuneTier tier) : base("法术强度符文", "增加法术强度", RuneType.Glyph, tier)
    {
        switch (tier)
        {
            case RuneTier.Tier1:
                abilityPowerBonus = 1.0f;
                break;
            case RuneTier.Tier2:
                abilityPowerBonus = 2.0f;
                break;
            case RuneTier.Tier3:
                abilityPowerBonus = 4.0f;
                break;
        }
    }
}

public class ArmorRune : Rune
{
    public ArmorRune(RuneTier tier) : base("护甲符文", "增加护甲", RuneType.Seal, tier)
    {
        switch (tier)
        {
            case RuneTier.Tier1:
                armorBonus = 0.5f;
                break;
            case RuneTier.Tier2:
                armorBonus = 1.0f;
                break;
            case RuneTier.Tier3:
                armorBonus = 2.0f;
                break;
        }
    }
}

public class MagicResistRune : Rune
{
    public MagicResistRune(RuneTier tier) : base("魔抗符文", "增加魔法抗性", RuneType.Glyph, tier)
    {
        switch (tier)
        {
            case RuneTier.Tier1:
                magicResistBonus = 0.5f;
                break;
            case RuneTier.Tier2:
                magicResistBonus = 1.0f;
                break;
            case RuneTier.Tier3:
                magicResistBonus = 2.0f;
                break;
        }
    }
}

public class HealthRune : Rune
{
    public HealthRune(RuneTier tier) : base("生命符文", "增加生命值", RuneType.Seal, tier)
    {
        switch (tier)
        {
            case RuneTier.Tier1:
                healthBonus = 10f;
                break;
            case RuneTier.Tier2:
                healthBonus = 20f;
                break;
            case RuneTier.Tier3:
                healthBonus = 40f;
                break;
        }
    }
}

public class MovementSpeedRune : Rune
{
    public MovementSpeedRune(RuneTier tier) : base("移动速度符文", "增加移动速度", RuneType.Quintessence, tier)
    {
        switch (tier)
        {
            case RuneTier.Tier1:
                movementSpeedBonus = 0.5f;
                break;
            case RuneTier.Tier2:
                movementSpeedBonus = 1.0f;
                break;
            case RuneTier.Tier3:
                movementSpeedBonus = 2.0f;
                break;
        }
    }
}

public class AttackSpeedRune : Rune
{
    public AttackSpeedRune(RuneTier tier) : base("攻击速度符文", "增加攻击速度", RuneType.Mark, tier)
    {
        switch (tier)
        {
            case RuneTier.Tier1:
                attackSpeedBonus = 0.5f;
                break;
            case RuneTier.Tier2:
                attackSpeedBonus = 1.0f;
                break;
            case RuneTier.Tier3:
                attackSpeedBonus = 2.0f;
                break;
        }
    }
}

public class CooldownReductionRune : Rune
{
    public CooldownReductionRune(RuneTier tier) : base("冷却缩减符文", "减少技能冷却", RuneType.Glyph, tier)
    {
        switch (tier)
        {
            case RuneTier.Tier1:
                cooldownReductionBonus = 0.2f;
                break;
            case RuneTier.Tier2:
                cooldownReductionBonus = 0.4f;
                break;
            case RuneTier.Tier3:
                cooldownReductionBonus = 0.8f;
                break;
        }
    }
}

public class LifeStealRune : Rune
{
    public LifeStealRune(RuneTier tier) : base("生命偷取符文", "增加生命偷取", RuneType.Quintessence, tier)
    {
        switch (tier)
        {
            case RuneTier.Tier1:
                lifeStealBonus = 0.5f;
                break;
            case RuneTier.Tier2:
                lifeStealBonus = 1.0f;
                break;
            case RuneTier.Tier3:
                lifeStealBonus = 2.0f;
                break;
        }
    }
}
using UnityEngine;

[System.Serializable]
public class SummonerSpell
{
    public string spellName;
    public string spellDescription;
    public float cooldown;
    public float currentCooldown;
    public Sprite icon;
    
    public enum SpellType
    {
        Flash,
        Heal,
        Ignite,
        Exhaust,
        Barrier,
        Teleport,
        Cleanse,
        Ghost
    }
    
    public SpellType spellType;
    
    public SummonerSpell(string name, string description, SpellType type, float cd)
    {
        spellName = name;
        spellDescription = description;
        spellType = type;
        cooldown = cd;
        currentCooldown = 0f;
    }
    
    public bool CanCast()
    {
        return currentCooldown <= 0f;
    }
    
    public void StartCooldown()
    {
        currentCooldown = cooldown;
    }
    
    public void UpdateCooldown(float deltaTime)
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= deltaTime;
        }
    }
    
    public virtual void Cast(HeroController caster)
    {
        Debug.Log($"Casting {spellName}");
    }
    
    public float GetCooldownPercentage()
    {
        if (cooldown == 0) return 1f;
        return 1f - (currentCooldown / cooldown);
    }
}

public class FlashSpell : SummonerSpell
{
    public float dashDistance = 10f;
    
    public FlashSpell() : base("闪现", "朝指定方向短距离闪烁", SpellType.Flash, 300f)
    {
        dashDistance = 10f;
    }
    
    public override void Cast(HeroController caster)
    {
        base.Cast(caster);
        
        Vector3 dashDirection = caster.transform.forward;
        Vector3 dashTarget = caster.transform.position + dashDirection * dashDistance;
        
        caster.transform.position = dashTarget;
        StartCooldown();
    }
}

public class HealSpell : SummonerSpell
{
    public float healAmount = 300f;
    
    public HealSpell() : base("治疗", "恢复自身和附近友军生命值", SpellType.Heal, 240f)
    {
        healAmount = 300f;
    }
    
    public override void Cast(HeroController caster)
    {
        base.Cast(caster);
        
        caster.heroData.baseStats.Heal(healAmount);
        
        StartCooldown();
    }
}

public class IgniteSpell : SummonerSpell
{
    public float damageAmount = 70f;
    public float dotDuration = 5f;
    
    public IgniteSpell() : base("点燃", "对目标造成持续伤害", SpellType.Ignite, 180f)
    {
        damageAmount = 70f;
        dotDuration = 5f;
    }
    
    public override void Cast(HeroController caster)
    {
        base.Cast(caster);
        
        StartCooldown();
    }
}

public class ExhaustSpell : SummonerSpell
{
    public float slowDuration = 5f;
    public float slowAmount = 0.4f;
    
    public ExhaustSpell() : base("衰竭", "减少目标移动速度和伤害", SpellType.Exhaust, 210f)
    {
        slowDuration = 5f;
        slowAmount = 0.4f;
    }
    
    public override void Cast(HeroController caster)
    {
        base.Cast(caster);
        
        StartCooldown();
    }
}

public class BarrierSpell : SummonerSpell
{
    public float shieldAmount = 400f;
    
    public BarrierSpell() : base("屏障", "为自身提供临时护盾", SpellType.Barrier, 180f)
    {
        shieldAmount = 400f;
    }
    
    public override void Cast(HeroController caster)
    {
        base.Cast(caster);
        
        StartCooldown();
    }
}

public class TeleportSpell : SummonerSpell
{
    public float channelTime = 3f;
    
    public TeleportSpell() : base("传送", "传送到友军建筑或小兵位置", SpellType.Teleport, 300f)
    {
        channelTime = 3f;
    }
    
    public override void Cast(HeroController caster)
    {
        base.Cast(caster);
        
        StartCooldown();
    }
}

public class CleanseSpell : SummonerSpell
{
    public CleanseSpell() : base("净化", "移除自身所有控制效果", SpellType.Cleanse, 210f)
    {
    }
    
    public override void Cast(HeroController caster)
    {
        base.Cast(caster);
        
        StartCooldown();
    }
}

public class GhostSpell : SummonerSpell
{
    public float duration = 10f;
    public float speedBonus = 0.5f;
    
    public GhostSpell() : base("幽灵", "增加移动速度并无视地形碰撞", SpellType.Ghost, 180f)
    {
        duration = 10f;
        speedBonus = 0.5f;
    }
    
    public override void Cast(HeroController caster)
    {
        base.Cast(caster);
        
        StartCooldown();
    }
}
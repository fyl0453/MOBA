[System.Serializable]
public class Skill
{
    public enum SkillType
    {
        Basic,
        Skill1,
        Skill2,
        Skill3,
        Ultimate
    }
    
    public enum DamageType
    {
        Physical,
        Magic,
        True
    }
    
    public enum CrowdControlType
    {
        None,
        Stun,
        Slow,
        Silence,
        Knockup,
        Root,
        Blind
    }
    
    public string skillName;
    public string skillDescription;
    public SkillType skillType;
    public DamageType damageType;
    public CrowdControlType crowdControlType;
    
    public float cooldown;
    public float currentCooldown;
    public float manaCost;
    public float damage;
    public float range;
    public float castTime;
    public float channelTime;
    public bool isAOE;
    public bool isPointAndClick;
    public bool isSkillShot;
    public bool isChanneled;
    
    public float crowdControlDuration;
    public float crowdControlStrength;
    
    public Skill(string name, string description, SkillType type, DamageType dmgType, float cd, float mana, float dmg, float rng, bool aoe)
    {
        skillName = name;
        skillDescription = description;
        skillType = type;
        damageType = dmgType;
        cooldown = cd;
        currentCooldown = 0f;
        manaCost = mana;
        damage = dmg;
        range = rng;
        isAOE = aoe;
        
        castTime = 0f;
        channelTime = 0f;
        isPointAndClick = false;
        isSkillShot = false;
        isChanneled = false;
        
        crowdControlType = CrowdControlType.None;
        crowdControlDuration = 0f;
        crowdControlStrength = 0f;
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
    
    public void ApplyCrowdControl(GameObject target)
    {
        if (crowdControlType != CrowdControlType.None)
        {
            // 应用控制效果
            Debug.Log($"Applying {crowdControlType} to {target.name} for {crowdControlDuration} seconds");
        }
    }
}
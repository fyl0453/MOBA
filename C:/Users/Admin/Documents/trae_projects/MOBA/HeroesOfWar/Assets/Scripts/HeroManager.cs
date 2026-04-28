using UnityEngine;
using System.Collections.Generic;

public class HeroManager : MonoBehaviour
{
    public static HeroManager Instance { get; private set; }
    
    public List<HeroData> heroList = new List<HeroData>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeHeroes();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeHeroes()
    {
        // 战士
        HeroData warrior = new HeroData
        {
            heroID = "warrior",
            heroName = "战士",
            heroType = HeroType.Warrior,
            description = "高生命值和防御力的近战英雄，适合作为团队的前排",
            baseStats = new HeroStats
            {
                maxHealth = 3000,
                currentHealth = 3000,
                mana = 500,
                currentMana = 500,
                attack = 100,
                armor = 50,
                magicResistance = 30,
                attackSpeed = 1.0f,
                moveSpeed = 3.5f,
                attackRange = 1.5f
            },
            skills = new List<Skill>
            {
                new Skill
                {
                    skillID = "warrior_1",
                    skillName = "冲锋",
                    description = "向前冲锋，对路径上的敌人造成伤害并晕眩",
                    damage = 200,
                    range = 8f,
                    cooldown = 10f,
                    manaCost = 50,
                    isAOE = true,
                    damageType = Skill.DamageType.Physical
                },
                new Skill
                {
                    skillID = "warrior_2",
                    skillName = "嘲讽",
                    description = "嘲讽周围的敌人，强制他们攻击自己",
                    damage = 0,
                    range = 5f,
                    cooldown = 15f,
                    manaCost = 70,
                    isAOE = true,
                    damageType = Skill.DamageType.Physical
                },
                new Skill
                {
                    skillID = "warrior_3",
                    skillName = "旋风斩",
                    description = "旋转攻击周围的敌人，造成持续伤害",
                    damage = 300,
                    range = 3f,
                    cooldown = 20f,
                    manaCost = 100,
                    isAOE = true,
                    damageType = Skill.DamageType.Physical
                }
            }
        };
        
        // 法师
        HeroData mage = new HeroData
        {
            heroID = "mage",
            heroName = "法师",
            heroType = HeroType.Mage,
            description = "高魔法伤害的远程英雄，适合在后排输出",
            baseStats = new HeroStats
            {
                maxHealth = 1500,
                currentHealth = 1500,
                mana = 1000,
                currentMana = 1000,
                attack = 50,
                armor = 20,
                magicResistance = 60,
                attackSpeed = 0.8f,
                moveSpeed = 3.0f,
                attackRange = 6.0f
            },
            skills = new List<Skill>
            {
                new Skill
                {
                    skillID = "mage_1",
                    skillName = "火球术",
                    description = "发射一个火球，对目标造成魔法伤害",
                    damage = 250,
                    range = 10f,
                    cooldown = 5f,
                    manaCost = 60,
                    isAOE = false,
                    damageType = Skill.DamageType.Magic
                },
                new Skill
                {
                    skillID = "mage_2",
                    skillName = "冰锥术",
                    description = "发射冰锥，对目标造成伤害并减速",
                    damage = 150,
                    range = 8f,
                    cooldown = 8f,
                    manaCost = 40,
                    isAOE = false,
                    damageType = Skill.DamageType.Magic
                },
                new Skill
                {
                    skillID = "mage_3",
                    skillName = "暴风雪",
                    description = "召唤暴风雪，对区域内的敌人造成持续伤害",
                    damage = 400,
                    range = 12f,
                    cooldown = 25f,
                    manaCost = 150,
                    isAOE = true,
                    damageType = Skill.DamageType.Magic
                }
            }
        };
        
        // 射手
        HeroData archer = new HeroData
        {
            heroID = "archer",
            heroName = "射手",
            heroType = HeroType.Archer,
            description = "高物理伤害的远程英雄，适合在后排输出",
            baseStats = new HeroStats
            {
                maxHealth = 1800,
                currentHealth = 1800,
                mana = 300,
                currentMana = 300,
                attack = 120,
                armor = 25,
                magicResistance = 25,
                attackSpeed = 1.5f,
                moveSpeed = 3.2f,
                attackRange = 7.0f
            },
            skills = new List<Skill>
            {
                new Skill
                {
                    skillID = "archer_1",
                    skillName = "精准射击",
                    description = "瞄准目标，造成高额物理伤害",
                    damage = 300,
                    range = 12f,
                    cooldown = 8f,
                    manaCost = 30,
                    isAOE = false,
                    damageType = Skill.DamageType.Physical
                },
                new Skill
                {
                    skillID = "archer_2",
                    skillName = "多重箭",
                    description = "发射多支箭，攻击多个目标",
                    damage = 100,
                    range = 10f,
                    cooldown = 10f,
                    manaCost = 40,
                    isAOE = true,
                    damageType = Skill.DamageType.Physical
                },
                new Skill
                {
                    skillID = "archer_3",
                    skillName = "陷阱",
                    description = "放置陷阱，对触发的敌人造成伤害并减速",
                    damage = 200,
                    range = 5f,
                    cooldown = 15f,
                    manaCost = 50,
                    isAOE = true,
                    damageType = Skill.DamageType.Physical
                }
            }
        };
        
        // 刺客
        HeroData assassin = new HeroData
        {
            heroID = "assassin",
            heroName = "刺客",
            heroType = HeroType.Assassin,
            description = "高爆发伤害的近战英雄，适合刺杀敌方后排",
            baseStats = new HeroStats
            {
                maxHealth = 1600,
                currentHealth = 1600,
                mana = 400,
                currentMana = 400,
                attack = 150,
                armor = 30,
                magicResistance = 30,
                attackSpeed = 1.2f,
                moveSpeed = 4.0f,
                attackRange = 1.0f
            },
            skills = new List<Skill>
            {
                new Skill
                {
                    skillID = "assassin_1",
                    skillName = "隐身",
                    description = "进入隐身状态，移动速度增加",
                    damage = 0,
                    range = 0f,
                    cooldown = 12f,
                    manaCost = 60,
                    isAOE = false,
                    damageType = Skill.DamageType.Physical
                },
                new Skill
                {
                    skillID = "assassin_2",
                    skillName = "背刺",
                    description = "从背后攻击敌人，造成额外伤害",
                    damage = 350,
                    range = 2f,
                    cooldown = 8f,
                    manaCost = 40,
                    isAOE = false,
                    damageType = Skill.DamageType.Physical
                },
                new Skill
                {
                    skillID = "assassin_3",
                    skillName = "终结",
                    description = "对生命值低于50%的敌人造成额外伤害",
                    damage = 500,
                    range = 3f,
                    cooldown = 20f,
                    manaCost = 100,
                    isAOE = false,
                    damageType = Skill.DamageType.True
                }
            }
        };
        
        // 辅助
        HeroData support = new HeroData
        {
            heroID = "support",
            heroName = "辅助",
            heroType = HeroType.Support,
            description = "提供治疗和增益效果的英雄，适合辅助队友",
            baseStats = new HeroStats
            {
                maxHealth = 2000,
                currentHealth = 2000,
                mana = 800,
                currentMana = 800,
                attack = 40,
                armor = 40,
                magicResistance = 40,
                attackSpeed = 0.7f,
                moveSpeed = 3.3f,
                attackRange = 5.0f
            },
            skills = new List<Skill>
            {
                new Skill
                {
                    skillID = "support_1",
                    skillName = "治疗术",
                    description = "治疗友方单位的生命值",
                    damage = -200, // 负数表示治疗
                    range = 6f,
                    cooldown = 10f,
                    manaCost = 80,
                    isAOE = false,
                    damageType = Skill.DamageType.Magic
                },
                new Skill
                {
                    skillID = "support_2",
                    skillName = "加速术",
                    description = "增加友方单位的移动速度",
                    damage = 0,
                    range = 8f,
                    cooldown = 12f,
                    manaCost = 60,
                    isAOE = true,
                    damageType = Skill.DamageType.Magic
                },
                new Skill
                {
                    skillID = "support_3",
                    skillName = "守护盾",
                    description = "为友方单位提供护盾，吸收伤害",
                    damage = -300, // 负数表示护盾
                    range = 5f,
                    cooldown = 18f,
                    manaCost = 120,
                    isAOE = true,
                    damageType = Skill.DamageType.Magic
                }
            }
        };
        
        // 坦克
        HeroData tank = new HeroData
        {
            heroID = "tank",
            heroName = "坦克",
            heroType = HeroType.Warrior,
            description = "极高生命值和防御力的近战英雄，是团队的坚固防线",
            baseStats = new HeroStats
            {
                maxHealth = 4000,
                currentHealth = 4000,
                mana = 400,
                currentMana = 400,
                attack = 80,
                armor = 80,
                magicResistance = 50,
                attackSpeed = 0.8f,
                moveSpeed = 3.0f,
                attackRange = 1.5f
            },
            skills = new List<Skill>
            {
                new Skill
                {
                    skillID = "tank_1",
                    skillName = "钢铁壁垒",
                    description = "增加自身护甲和魔法抗性",
                    damage = 0,
                    range = 0f,
                    cooldown = 12f,
                    manaCost = 60,
                    isAOE = false,
                    damageType = Skill.DamageType.Physical
                },
                new Skill
                {
                    skillID = "tank_2",
                    skillName = "地震波",
                    description = "敲击地面，对周围敌人造成伤害并减速",
                    damage = 150,
                    range = 6f,
                    cooldown = 10f,
                    manaCost = 50,
                    isAOE = true,
                    damageType = Skill.DamageType.Physical
                },
                new Skill
                {
                    skillID = "tank_3",
                    skillName = "守护领域",
                    description = "创建一个领域，减少友方受到的伤害",
                    damage = -100, // 负数表示减伤
                    range = 8f,
                    cooldown = 25f,
                    manaCost = 120,
                    isAOE = true,
                    damageType = Skill.DamageType.Physical
                }
            }
        };
        
        // 法师（控制型）
        HeroData controller = new HeroData
        {
            heroID = "controller",
            heroName = "控制法师",
            heroType = HeroType.Mage,
            description = "擅长控制敌人的法师，能限制敌方的行动",
            baseStats = new HeroStats
            {
                maxHealth = 1400,
                currentHealth = 1400,
                mana = 1200,
                currentMana = 1200,
                attack = 40,
                armor = 15,
                magicResistance = 70,
                attackSpeed = 0.7f,
                moveSpeed = 2.8f,
                attackRange = 6.5f
            },
            skills = new List<Skill>
            {
                new Skill
                {
                    skillID = "controller_1",
                    skillName = "禁锢",
                    description = "禁锢目标，使其无法移动和攻击",
                    damage = 100,
                    range = 9f,
                    cooldown = 10f,
                    manaCost = 70,
                    isAOE = false,
                    damageType = Skill.DamageType.Magic
                },
                new Skill
                {
                    skillID = "controller_2",
                    skillName = "法术黑洞",
                    description = "吸引周围敌人到中心",
                    damage = 150,
                    range = 8f,
                    cooldown = 15f,
                    manaCost = 80,
                    isAOE = true,
                    damageType = Skill.DamageType.Magic
                },
                new Skill
                {
                    skillID = "controller_3",
                    skillName = "时间静止",
                    description = "使区域内的敌人静止不动",
                    damage = 200,
                    range = 10f,
                    cooldown = 30f,
                    manaCost = 180,
                    isAOE = true,
                    damageType = Skill.DamageType.Magic
                }
            }
        };
        
        // 射手（攻速型）
        HeroData rapidFire = new HeroData
        {
            heroID = "rapid_fire",
            heroName = "速射射手",
            heroType = HeroType.Archer,
            description = "高攻击速度的射手，能在短时间内造成大量伤害",
            baseStats = new HeroStats
            {
                maxHealth = 1700,
                currentHealth = 1700,
                mana = 250,
                currentMana = 250,
                attack = 80,
                armor = 20,
                magicResistance = 20,
                attackSpeed = 2.0f,
                moveSpeed = 3.3f,
                attackRange = 6.5f
            },
            skills = new List<Skill>
            {
                new Skill
                {
                    skillID = "rapid_fire_1",
                    skillName = "急速射击",
                    description = "提高攻击速度",
                    damage = 0,
                    range = 0f,
                    cooldown = 12f,
                    manaCost = 40,
                    isAOE = false,
                    damageType = Skill.DamageType.Physical
                },
                new Skill
                {
                    skillID = "rapid_fire_2",
                    skillName = "散射",
                    description = "向周围发射多支箭",
                    damage = 80,
                    range = 7f,
                    cooldown = 8f,
                    manaCost = 30,
                    isAOE = true,
                    damageType = Skill.DamageType.Physical
                },
                new Skill
                {
                    skillID = "rapid_fire_3",
                    skillName = "弹幕",
                    description = "持续发射大量箭矢",
                    damage = 250,
                    range = 8f,
                    cooldown = 20f,
                    manaCost = 100,
                    isAOE = true,
                    damageType = Skill.DamageType.Physical
                }
            }
        };
        
        // 刺客（法系）
        HeroData mageAssassin = new HeroData
        {
            heroID = "mage_assassin",
            heroName = "法系刺客",
            heroType = HeroType.Assassin,
            description = "使用魔法进行爆发伤害的刺客，能快速消灭敌人",
            baseStats = new HeroStats
            {
                maxHealth = 1500,
                currentHealth = 1500,
                mana = 600,
                currentMana = 600,
                attack = 60,
                armor = 25,
                magicResistance = 40,
                attackSpeed = 1.0f,
                moveSpeed = 3.8f,
                attackRange = 2.0f
            },
            skills = new List<Skill>
            {
                new Skill
                {
                    skillID = "mage_assassin_1",
                    skillName = "闪现",
                    description = "瞬间移动到目标位置",
                    damage = 0,
                    range = 10f,
                    cooldown = 15f,
                    manaCost = 50,
                    isAOE = false,
                    damageType = Skill.DamageType.Magic
                },
                new Skill
                {
                    skillID = "mage_assassin_2",
                    skillName = "能量冲击",
                    description = "释放能量冲击，对目标造成魔法伤害",
                    damage = 250,
                    range = 5f,
                    cooldown = 8f,
                    manaCost = 60,
                    isAOE = false,
                    damageType = Skill.DamageType.Magic
                },
                new Skill
                {
                    skillID = "mage_assassin_3",
                    skillName = "灵魂收割",
                    description = "收割低生命值敌人的灵魂，造成高额魔法伤害",
                    damage = 450,
                    range = 4f,
                    cooldown = 22f,
                    manaCost = 120,
                    isAOE = false,
                    damageType = Skill.DamageType.Magic
                }
            }
        };
        
        // 辅助（增益型）
        HeroData enhancer = new HeroData
        {
            heroID = "enhancer",
            heroName = "增益辅助",
            heroType = HeroType.Support,
            description = "为队友提供强大增益效果的辅助英雄",
            baseStats = new HeroStats
            {
                maxHealth = 1900,
                currentHealth = 1900,
                mana = 1000,
                currentMana = 1000,
                attack = 30,
                armor = 35,
                magicResistance = 35,
                attackSpeed = 0.6f,
                moveSpeed = 3.2f,
                attackRange = 5.5f
            },
            skills = new List<Skill>
            {
                new Skill
                {
                    skillID = "enhancer_1",
                    skillName = "力量祝福",
                    description = "增加友方单位的攻击力",
                    damage = 0,
                    range = 7f,
                    cooldown = 10f,
                    manaCost = 70,
                    isAOE = true,
                    damageType = Skill.DamageType.Magic
                },
                new Skill
                {
                    skillID = "enhancer_2",
                    skillName = "群体治疗",
                    description = "治疗多个友方单位",
                    damage = -150, // 负数表示治疗
                    range = 8f,
                    cooldown = 15f,
                    manaCost = 90,
                    isAOE = true,
                    damageType = Skill.DamageType.Magic
                },
                new Skill
                {
                    skillID = "enhancer_3",
                    skillName = "守护天使",
                    description = "为友方单位提供无敌效果",
                    damage = 0,
                    range = 6f,
                    cooldown = 30f,
                    manaCost = 150,
                    isAOE = false,
                    damageType = Skill.DamageType.Magic
                }
            }
        };
        
        // 添加到英雄列表
        heroList.Add(warrior);
        heroList.Add(mage);
        heroList.Add(archer);
        heroList.Add(assassin);
        heroList.Add(support);
        heroList.Add(tank);
        heroList.Add(controller);
        heroList.Add(rapidFire);
        heroList.Add(mageAssassin);
        heroList.Add(enhancer);
    }
    
    public HeroData GetHeroByID(string heroID)
    {
        return heroList.Find(hero => hero.heroID == heroID);
    }
    
    public List<HeroData> GetAllHeroes()
    {
        return heroList;
    }
    
    public List<HeroData> GetHeroesByType(HeroType type)
    {
        return heroList.FindAll(hero => hero.heroType == type);
    }
}

[System.Serializable]
public class HeroData
{
    public string heroID;
    public string heroName;
    public HeroType heroType;
    public string description;
    public HeroStats baseStats;
    public List<Skill> skills;
}

public enum HeroType
{
    Warrior,
    Mage,
    Archer,
    Assassin,
    Support
}

[System.Serializable]
public class HeroStats
{
    public float maxHealth;
    public float currentHealth;
    public float mana;
    public float currentMana;
    public float attack;
    public float armor;
    public float magicResistance;
    public float attackSpeed;
    public float moveSpeed;
    public float attackRange;
}

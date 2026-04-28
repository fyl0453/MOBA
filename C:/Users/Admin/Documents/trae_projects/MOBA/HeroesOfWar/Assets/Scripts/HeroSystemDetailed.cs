[System.Serializable]
public class HeroSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Hero> heroes;
    public List<HeroSkill> heroSkills;
    public List<HeroAttribute> heroAttributes;
    public List<PlayerHero> playerHeroes;
    public List<HeroMastery> heroMasteries;
    public List<HeroRecommendation> heroRecommendations;
    
    public HeroSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        heroes = new List<Hero>();
        heroSkills = new List<HeroSkill>();
        heroAttributes = new List<HeroAttribute>();
        playerHeroes = new List<PlayerHero>();
        heroMasteries = new List<HeroMastery>();
        heroRecommendations = new List<HeroRecommendation>();
    }
    
    public void AddHero(Hero hero)
    {
        heroes.Add(hero);
    }
    
    public void AddHeroSkill(HeroSkill skill)
    {
        heroSkills.Add(skill);
    }
    
    public void AddHeroAttribute(HeroAttribute attribute)
    {
        heroAttributes.Add(attribute);
    }
    
    public void AddPlayerHero(PlayerHero playerHero)
    {
        playerHeroes.Add(playerHero);
    }
    
    public void AddHeroMastery(HeroMastery mastery)
    {
        heroMasteries.Add(mastery);
    }
    
    public void AddHeroRecommendation(HeroRecommendation recommendation)
    {
        heroRecommendations.Add(recommendation);
    }
    
    public Hero GetHero(string heroID)
    {
        return heroes.Find(h => h.heroID == heroID);
    }
    
    public HeroSkill GetHeroSkill(string skillID)
    {
        return heroSkills.Find(s => s.skillID == skillID);
    }
    
    public HeroAttribute GetHeroAttribute(string attributeID)
    {
        return heroAttributes.Find(a => a.attributeID == attributeID);
    }
    
    public PlayerHero GetPlayerHero(string playerID, string heroID)
    {
        return playerHeroes.Find(ph => ph.playerID == playerID && ph.heroID == heroID);
    }
    
    public HeroMastery GetHeroMastery(string playerID, string heroID)
    {
        return heroMasteries.Find(m => m.playerID == playerID && m.heroID == heroID);
    }
    
    public HeroRecommendation GetHeroRecommendation(string recommendationID)
    {
        return heroRecommendations.Find(r => r.recommendationID == recommendationID);
    }
    
    public List<Hero> GetHeroesByType(string type)
    {
        return heroes.FindAll(h => h.heroType == type);
    }
    
    public List<Hero> GetHeroesByDifficulty(string difficulty)
    {
        return heroes.FindAll(h => h.difficulty == difficulty);
    }
    
    public List<PlayerHero> GetPlayerHeroes(string playerID)
    {
        return playerHeroes.FindAll(ph => ph.playerID == playerID);
    }
    
    public List<HeroMastery> GetHeroMasteries(string playerID)
    {
        return heroMasteries.FindAll(m => m.playerID == playerID);
    }
    
    public List<HeroRecommendation> GetRecommendationsByHero(string heroID)
    {
        return heroRecommendations.FindAll(r => r.heroID == heroID);
    }
}

[System.Serializable]
public class Hero
{
    public string heroID;
    public string heroName;
    public string heroDescription;
    public string heroType;
    public string difficulty;
    public string priceCurrency;
    public float price;
    public int fragmentCost;
    public string releaseDate;
    public string modelPath;
    public string iconPath;
    public List<string> skillIDs;
    public List<string> attributeIDs;
    public bool isEnabled;
    
    public Hero(string id, string name, string desc, string type, string difficulty, string priceCurrency, float price, int fragmentCost, string releaseDate, string modelPath, string iconPath)
    {
        heroID = id;
        heroName = name;
        heroDescription = desc;
        heroType = type;
        this.difficulty = difficulty;
        this.priceCurrency = priceCurrency;
        this.price = price;
        this.fragmentCost = fragmentCost;
        this.releaseDate = releaseDate;
        this.modelPath = modelPath;
        this.iconPath = iconPath;
        skillIDs = new List<string>();
        attributeIDs = new List<string>();
        isEnabled = true;
    }
    
    public void AddSkill(string skillID)
    {
        skillIDs.Add(skillID);
    }
    
    public void AddAttribute(string attributeID)
    {
        attributeIDs.Add(attributeID);
    }
}

[System.Serializable]
public class HeroSkill
{
    public string skillID;
    public string heroID;
    public string skillName;
    public string skillDescription;
    public string skillType;
    public float cooldown;
    public float damage;
    public float range;
    public int maxLevel;
    public List<SkillEffect> effects;
    public List<SkillUpgrade> upgrades;
    
    public HeroSkill(string id, string heroID, string name, string desc, string type, float cooldown, float damage, float range, int maxLevel)
    {
        skillID = id;
        this.heroID = heroID;
        skillName = name;
        skillDescription = desc;
        skillType = type;
        this.cooldown = cooldown;
        this.damage = damage;
        this.range = range;
        this.maxLevel = maxLevel;
        effects = new List<SkillEffect>();
        upgrades = new List<SkillUpgrade>();
    }
    
    public void AddEffect(SkillEffect effect)
    {
        effects.Add(effect);
    }
    
    public void AddUpgrade(SkillUpgrade upgrade)
    {
        upgrades.Add(upgrade);
    }
}

[System.Serializable]
public class SkillEffect
{
    public string effectID;
    public string effectName;
    public string effectType;
    public string description;
    public float value;
    public float duration;
    
    public SkillEffect(string id, string name, string type, string desc, float value, float duration)
    {
        effectID = id;
        effectName = name;
        effectType = type;
        description = desc;
        this.value = value;
        this.duration = duration;
    }
}

[System.Serializable]
public class SkillUpgrade
{
    public string upgradeID;
    public int level;
    public float cooldownReduction;
    public float damageIncrease;
    public float rangeIncrease;
    public string description;
    
    public SkillUpgrade(string id, int level, float cooldownReduction, float damageIncrease, float rangeIncrease, string desc)
    {
        upgradeID = id;
        this.level = level;
        this.cooldownReduction = cooldownReduction;
        this.damageIncrease = damageIncrease;
        this.rangeIncrease = rangeIncrease;
        description = desc;
    }
}

[System.Serializable]
public class HeroAttribute
{
    public string attributeID;
    public string heroID;
    public string attributeName;
    public string attributeType;
    public float baseValue;
    public float growthValue;
    public string description;
    
    public HeroAttribute(string id, string heroID, string name, string type, float baseValue, float growthValue, string desc)
    {
        attributeID = id;
        this.heroID = heroID;
        attributeName = name;
        attributeType = type;
        this.baseValue = baseValue;
        this.growthValue = growthValue;
        description = desc;
    }
    
    public float GetValueAtLevel(int level)
    {
        return baseValue + (growthValue * (level - 1));
    }
}

[System.Serializable]
public class PlayerHero
{
    public string playerID;
    public string heroID;
    public string obtainDate;
    public int level;
    public int experience;
    public bool isOwned;
    
    public PlayerHero(string playerID, string heroID, bool owned = false)
    {
        this.playerID = playerID;
        this.heroID = heroID;
        obtainDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        level = 1;
        experience = 0;
        isOwned = owned;
    }
    
    public void AddExperience(int exp)
    {
        experience += exp;
        CheckLevelUp();
    }
    
    private void CheckLevelUp()
    {
        int[] levelThresholds = { 0, 100, 300, 600, 1000, 1500, 2100, 2800, 3600, 4500 };
        for (int i = levelThresholds.Length - 1; i >= 0; i--)
        {
            if (experience >= levelThresholds[i])
            {
                level = i + 1;
                break;
            }
        }
    }
    
    public void SetOwned(bool owned)
    {
        isOwned = owned;
    }
}

[System.Serializable]
public class HeroMastery
{
    public string playerID;
    public string heroID;
    public int masteryLevel;
    public int masteryPoints;
    public string masteryTitle;
    public int totalGames;
    public int totalWins;
    
    public HeroMastery(string playerID, string heroID)
    {
        this.playerID = playerID;
        this.heroID = heroID;
        masteryLevel = 1;
        masteryPoints = 0;
        masteryTitle = "见习";
        totalGames = 0;
        totalWins = 0;
    }
    
    public void AddMasteryPoints(int points)
    {
        masteryPoints += points;
        CheckMasteryLevel();
    }
    
    private void CheckMasteryLevel()
    {
        int[] levelThresholds = { 0, 100, 300, 600, 1000, 1500, 2100, 2800, 3600, 4500 };
        string[] titles = { "见习", "熟练", "专家", "大师", "宗师", "王者", "传说", "神话", "永恒", "不朽" };
        
        for (int i = levelThresholds.Length - 1; i >= 0; i--)
        {
            if (masteryPoints >= levelThresholds[i])
            {
                masteryLevel = i + 1;
                masteryTitle = titles[i];
                break;
            }
        }
    }
    
    public void AddGame(bool won)
    {
        totalGames++;
        if (won)
        {
            totalWins++;
        }
    }
    
    public float GetWinRate()
    {
        if (totalGames == 0)
            return 0;
        return (float)totalWins / totalGames * 100;
    }
}

[System.Serializable]
public class HeroRecommendation
{
    public string recommendationID;
    public string heroID;
    public string recommendationType;
    public string title;
    public string description;
    public List<string> items;
    public List<string> skills;
    public List<string> tips;
    
    public HeroRecommendation(string id, string heroID, string type, string title, string desc)
    {
        recommendationID = id;
        this.heroID = heroID;
        recommendationType = type;
        this.title = title;
        description = desc;
        items = new List<string>();
        skills = new List<string>();
        tips = new List<string>();
    }
    
    public void AddItem(string item)
    {
        items.Add(item);
    }
    
    public void AddSkill(string skill)
    {
        skills.Add(skill);
    }
    
    public void AddTip(string tip)
    {
        tips.Add(tip);
    }
}

[System.Serializable]
public class HeroSystemDetailedManagerData
{
    public HeroSystemDetailed system;
    
    public HeroSystemDetailedManagerData()
    {
        system = new HeroSystemDetailed("hero_system_detailed", "英雄系统详细", "管理英雄的详细功能，包括英雄类型和技能组合");
    }
}
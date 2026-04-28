[System.Serializable]
public class HeroAttribute
{
    public string attributeID;
    public string attributeName;
    public string attributeDescription;
    public float baseValue;
    public float growthValue;
    
    public HeroAttribute(string id, string name, string desc, float baseVal, float growthVal)
    {
        attributeID = id;
        attributeName = name;
        attributeDescription = desc;
        baseValue = baseVal;
        growthValue = growthVal;
    }
    
    public float GetValueAtLevel(int level)
    {
        return baseValue + growthValue * (level - 1);
    }
}

[System.Serializable]
public class HeroSkill
{
    public string skillID;
    public string skillName;
    public string skillDescription;
    public string skillType;
    public float cooldown;
    public float manaCost;
    public int maxLevel;
    public List<SkillEffect> effects;
    public List<SkillUpgrade> upgrades;
    public string skillIcon;
    
    public HeroSkill(string id, string name, string desc, string type, float cd, float cost)
    {
        skillID = id;
        skillName = name;
        skillDescription = desc;
        skillType = type;
        cooldown = cd;
        manaCost = cost;
        maxLevel = 5;
        effects = new List<SkillEffect>();
        upgrades = new List<SkillUpgrade>();
        skillIcon = "";
    }
    
    public void AddEffect(string effectType, string effectDescription, float value = 0f)
    {
        SkillEffect effect = new SkillEffect(effectType, effectDescription, value);
        effects.Add(effect);
    }
    
    public void AddUpgrade(int level, string upgradeDescription)
    {
        SkillUpgrade upgrade = new SkillUpgrade(level, upgradeDescription);
        upgrades.Add(upgrade);
    }
    
    public float GetCooldownAtLevel(int level)
    {
        return cooldown * (1 - (level - 1) * 0.1f);
    }
    
    public float GetManaCostAtLevel(int level)
    {
        return manaCost * (1 + (level - 1) * 0.1f);
    }
}

[System.Serializable]
public class SkillEffect
{
    public string effectType;
    public string effectDescription;
    public float value;
    
    public SkillEffect(string type, string desc, float val = 0f)
    {
        effectType = type;
        effectDescription = desc;
        value = val;
    }
}

[System.Serializable]
public class SkillUpgrade
{
    public int level;
    public string upgradeDescription;
    
    public SkillUpgrade(int lvl, string desc)
    {
        level = lvl;
        upgradeDescription = desc;
    }
}

[System.Serializable]
public class HeroSkin
{
    public string skinID;
    public string skinName;
    public string skinDescription;
    public int price;
    public string currencyType;
    public string skinModel;
    public string skinTexture;
    public List<SkinEffect> effects;
    public bool isLimited;
    public string limitedTime;
    
    public HeroSkin(string id, string name, string desc, int price, string currency = "Gems")
    {
        skinID = id;
        skinName = name;
        skinDescription = desc;
        this.price = price;
        currencyType = currency;
        skinModel = "";
        skinTexture = "";
        effects = new List<SkinEffect>();
        isLimited = false;
        limitedTime = "";
    }
    
    public void AddEffect(string effectType, string effectDescription)
    {
        SkinEffect effect = new SkinEffect(effectType, effectDescription);
        effects.Add(effect);
    }
}

[System.Serializable]
public class SkinEffect
{
    public string effectType;
    public string effectDescription;
    
    public SkinEffect(string type, string desc)
    {
        effectType = type;
        effectDescription = desc;
    }
}

[System.Serializable]
public class HeroVoice
{
    public string voiceID;
    public string voiceName;
    public string voiceDescription;
    public List<string> voiceLines;
    public string voiceFile;
    
    public HeroVoice(string id, string name, string desc)
    {
        voiceID = id;
        voiceName = name;
        voiceDescription = desc;
        voiceLines = new List<string>();
        voiceFile = "";
    }
    
    public void AddVoiceLine(string line)
    {
        voiceLines.Add(line);
    }
}

[System.Serializable]
public class HeroManagerExtendedData
{
    public List<Hero> heroes;
    public List<HeroSkin> skins;
    public List<HeroVoice> voices;
    
    public HeroManagerExtendedData()
    {
        heroes = new List<Hero>();
        skins = new List<HeroSkin>();
        voices = new List<HeroVoice>();
    }
    
    public void AddHero(Hero hero)
    {
        heroes.Add(hero);
    }
    
    public void AddSkin(HeroSkin skin)
    {
        skins.Add(skin);
    }
    
    public void AddVoice(HeroVoice voice)
    {
        voices.Add(voice);
    }
    
    public Hero GetHero(string heroID)
    {
        return heroes.Find(h => h.heroID == heroID);
    }
    
    public HeroSkin GetSkin(string skinID)
    {
        return skins.Find(s => s.skinID == skinID);
    }
    
    public HeroVoice GetVoice(string voiceID)
    {
        return voices.Find(v => v.voiceID == voiceID);
    }
    
    public List<Hero> GetHeroesByType(string type)
    {
        return heroes.FindAll(h => h.heroType == type);
    }
    
    public List<HeroSkin> GetSkinsByHero(string heroID)
    {
        return skins.FindAll(s => s.skinID.Contains(heroID));
    }
    
    public List<HeroVoice> GetVoicesByHero(string heroID)
    {
        return voices.FindAll(v => v.voiceID.Contains(heroID));
    }
}
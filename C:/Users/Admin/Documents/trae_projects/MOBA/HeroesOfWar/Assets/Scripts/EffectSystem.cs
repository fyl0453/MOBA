[System.Serializable]
public class EffectSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Effect> effects;
    public List<EffectCategory> categories;
    
    public EffectSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        effects = new List<Effect>();
        categories = new List<EffectCategory>();
    }
    
    public void AddEffect(Effect effect)
    {
        effects.Add(effect);
    }
    
    public void AddCategory(EffectCategory category)
    {
        categories.Add(category);
    }
    
    public Effect GetEffect(string effectID)
    {
        return effects.Find(e => e.effectID == effectID);
    }
    
    public EffectCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public List<Effect> GetEffectsByCategory(string categoryID)
    {
        return effects.FindAll(e => e.categoryID == categoryID);
    }
    
    public List<Effect> GetEffectsByType(string effectType)
    {
        return effects.FindAll(e => e.effectType == effectType);
    }
}

[System.Serializable]
public class EffectCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    public string categoryIcon;
    
    public EffectCategory(string id, string name, string desc)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = desc;
        categoryIcon = "";
    }
}

[System.Serializable]
public class Effect
{
    public string effectID;
    public string effectName;
    public string effectDescription;
    public string effectType;
    public string categoryID;
    public int price;
    public string currencyType;
    public string effectAsset;
    public string effectIcon;
    public bool isLimited;
    public string limitedTime;
    public bool isDefault;
    
    public Effect(string id, string name, string desc, string type, string category, int price, string currency = "Gems")
    {
        effectID = id;
        effectName = name;
        effectDescription = desc;
        effectType = type;
        categoryID = category;
        this.price = price;
        currencyType = currency;
        effectAsset = "";
        effectIcon = "";
        isLimited = false;
        limitedTime = "";
        isDefault = false;
    }
    
    public void SetLimited(bool limited, string time = "")
    {
        isLimited = limited;
        limitedTime = time;
    }
    
    public void SetDefault(bool isDefault)
    {
        this.isDefault = isDefault;
    }
}

[System.Serializable]
public class PlayerEffectData
{
    public string playerID;
    public Dictionary<string, string> equippedEffects;
    public List<string> ownedEffects;
    
    public PlayerEffectData(string playerID)
    {
        this.playerID = playerID;
        equippedEffects = new Dictionary<string, string>();
        ownedEffects = new List<string>();
    }
    
    public void EquipEffect(string effectType, string effectID)
    {
        equippedEffects[effectType] = effectID;
    }
    
    public void UnequipEffect(string effectType)
    {
        if (equippedEffects.ContainsKey(effectType))
        {
            equippedEffects.Remove(effectType);
        }
    }
    
    public void AddOwnedEffect(string effectID)
    {
        if (!ownedEffects.Contains(effectID))
        {
            ownedEffects.Add(effectID);
        }
    }
    
    public void RemoveOwnedEffect(string effectID)
    {
        ownedEffects.Remove(effectID);
    }
    
    public string GetEquippedEffect(string effectType)
    {
        return equippedEffects.ContainsKey(effectType) ? equippedEffects[effectType] : "";
    }
    
    public bool OwnsEffect(string effectID)
    {
        return ownedEffects.Contains(effectID);
    }
}

[System.Serializable]
public class EffectManagerData
{
    public EffectSystem system;
    public List<PlayerEffectData> playerData;
    
    public EffectManagerData()
    {
        system = new EffectSystem("effect_system", "特效系统", "管理游戏中的各种特效");
        playerData = new List<PlayerEffectData>();
    }
    
    public void AddPlayerData(PlayerEffectData data)
    {
        playerData.Add(data);
    }
    
    public PlayerEffectData GetPlayerData(string playerID)
    {
        return playerData.Find(pd => pd.playerID == playerID);
    }
}
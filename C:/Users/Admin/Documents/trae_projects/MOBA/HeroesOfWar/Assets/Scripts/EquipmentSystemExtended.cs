[System.Serializable]
public class EquipmentSystemExtended
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Equipment> equipments;
    public List<EquipmentCategory> categories;
    public List<EquipmentBuild> builds;
    
    public EquipmentSystemExtended(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        equipments = new List<Equipment>();
        categories = new List<EquipmentCategory>();
        builds = new List<EquipmentBuild>();
    }
    
    public void AddEquipment(Equipment equipment)
    {
        equipments.Add(equipment);
    }
    
    public void AddCategory(EquipmentCategory category)
    {
        categories.Add(category);
    }
    
    public void AddBuild(EquipmentBuild build)
    {
        builds.Add(build);
    }
    
    public Equipment GetEquipment(string equipmentID)
    {
        return equipments.Find(e => e.equipmentID == equipmentID);
    }
    
    public EquipmentCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public EquipmentBuild GetBuild(string buildID)
    {
        return builds.Find(b => b.buildID == buildID);
    }
    
    public List<Equipment> GetEquipmentsByCategory(string categoryID)
    {
        return equipments.FindAll(e => e.categoryID == categoryID);
    }
    
    public List<Equipment> GetEquipmentsByType(string equipmentType)
    {
        return equipments.FindAll(e => e.equipmentType == equipmentType);
    }
    
    public List<EquipmentBuild> GetBuildsByHero(string heroID)
    {
        return builds.FindAll(b => b.heroID == heroID);
    }
}

[System.Serializable]
public class EquipmentCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    public string categoryIcon;
    
    public EquipmentCategory(string id, string name, string desc)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = desc;
        categoryIcon = "";
    }
}

[System.Serializable]
public class Equipment
{
    public string equipmentID;
    public string equipmentName;
    public string equipmentDescription;
    public string categoryID;
    public string equipmentType;
    public int price;
    public int sellPrice;
    public Dictionary<string, float> attributes;
    public List<EquipmentPassive> passives;
    public List<string> recipe;
    public string equipmentIcon;
    public bool isEpic;
    public bool isLegendary;
    
    public Equipment(string id, string name, string desc, string category, string type, int price)
    {
        equipmentID = id;
        equipmentName = name;
        equipmentDescription = desc;
        categoryID = category;
        equipmentType = type;
        this.price = price;
        sellPrice = Mathf.RoundToInt(price * 0.7f);
        attributes = new Dictionary<string, float>();
        passives = new List<EquipmentPassive>();
        recipe = new List<string>();
        equipmentIcon = "";
        isEpic = false;
        isLegendary = false;
    }
    
    public void AddAttribute(string attribute, float value)
    {
        attributes[attribute] = value;
    }
    
    public void AddPassive(EquipmentPassive passive)
    {
        passives.Add(passive);
    }
    
    public void AddToRecipe(string equipmentID)
    {
        recipe.Add(equipmentID);
    }
    
    public void SetEpic(bool epic)
    {
        isEpic = epic;
    }
    
    public void SetLegendary(bool legendary)
    {
        isLegendary = legendary;
    }
}

[System.Serializable]
public class EquipmentPassive
{
    public string passiveID;
    public string passiveName;
    public string passiveDescription;
    public string triggerCondition;
    public float cooldown;
    
    public EquipmentPassive(string id, string name, string desc, string condition, float cooldown = 0f)
    {
        passiveID = id;
        passiveName = name;
        passiveDescription = desc;
        triggerCondition = condition;
        this.cooldown = cooldown;
    }
}

[System.Serializable]
public class EquipmentBuild
{
    public string buildID;
    public string buildName;
    public string buildDescription;
    public string heroID;
    public string heroName;
    public string playStyle;
    public List<string> coreItems;
    public List<string> situationalItems;
    public float winRate;
    public int usageCount;
    
    public EquipmentBuild(string id, string name, string desc, string hero, string heroName, string style)
    {
        buildID = id;
        buildName = name;
        buildDescription = desc;
        heroID = hero;
        this.heroName = heroName;
        playStyle = style;
        coreItems = new List<string>();
        situationalItems = new List<string>();
        winRate = 0f;
        usageCount = 0;
    }
    
    public void AddCoreItem(string equipmentID)
    {
        coreItems.Add(equipmentID);
    }
    
    public void AddSituationalItem(string equipmentID)
    {
        situationalItems.Add(equipmentID);
    }
    
    public void UpdateStats(float winRate, int usageCount)
    {
        this.winRate = winRate;
        this.usageCount = usageCount;
    }
}

[System.Serializable]
public class PlayerEquipmentData
{
    public string playerID;
    public List<string> ownedEquipments;
    public List<string> savedBuilds;
    
    public PlayerEquipmentData(string playerID)
    {
        this.playerID = playerID;
        ownedEquipments = new List<string>();
        savedBuilds = new List<string>();
    }
    
    public void AddEquipment(string equipmentID)
    {
        if (!ownedEquipments.Contains(equipmentID))
        {
            ownedEquipments.Add(equipmentID);
        }
    }
    
    public void RemoveEquipment(string equipmentID)
    {
        ownedEquipments.Remove(equipmentID);
    }
    
    public void AddSavedBuild(string buildID)
    {
        if (!savedBuilds.Contains(buildID))
        {
            savedBuilds.Add(buildID);
        }
    }
    
    public void RemoveSavedBuild(string buildID)
    {
        savedBuilds.Remove(buildID);
    }
    
    public bool OwnsEquipment(string equipmentID)
    {
        return ownedEquipments.Contains(equipmentID);
    }
    
    public bool HasSavedBuild(string buildID)
    {
        return savedBuilds.Contains(buildID);
    }
}

[System.Serializable]
public class EquipmentSystemExtendedData
{
    public EquipmentSystemExtended system;
    public List<PlayerEquipmentData> playerData;
    
    public EquipmentSystemExtendedData()
    {
        system = new EquipmentSystemExtended("equipment_system_extended", "装备系统扩展", "管理装备合成路径和被动效果");
        playerData = new List<PlayerEquipmentData>();
    }
    
    public void AddPlayerData(PlayerEquipmentData data)
    {
        playerData.Add(data);
    }
    
    public PlayerEquipmentData GetPlayerData(string playerID)
    {
        return playerData.Find(pd => pd.playerID == playerID);
    }
}
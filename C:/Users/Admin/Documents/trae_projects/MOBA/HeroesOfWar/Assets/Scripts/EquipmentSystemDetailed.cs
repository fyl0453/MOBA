[System.Serializable]
public class EquipmentSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Equipment> equipments;
    public List<EquipmentSet> equipmentSets;
    public List<PlayerEquipment> playerEquipments;
    public List<EquipmentEnhance> equipmentEnhances;
    public List<EquipmentEvent> equipmentEvents;

    public EquipmentSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        equipments = new List<Equipment>();
        equipmentSets = new List<EquipmentSet>();
        playerEquipments = new List<PlayerEquipment>();
        equipmentEnhances = new List<EquipmentEnhance>();
        equipmentEvents = new List<EquipmentEvent>();
    }

    public void AddEquipment(Equipment equipment)
    {
        equipments.Add(equipment);
    }

    public void AddEquipmentSet(EquipmentSet equipmentSet)
    {
        equipmentSets.Add(equipmentSet);
    }

    public void AddPlayerEquipment(PlayerEquipment playerEquipment)
    {
        playerEquipments.Add(playerEquipment);
    }

    public void AddEquipmentEnhance(EquipmentEnhance equipmentEnhance)
    {
        equipmentEnhances.Add(equipmentEnhance);
    }

    public void AddEquipmentEvent(EquipmentEvent equipmentEvent)
    {
        equipmentEvents.Add(equipmentEvent);
    }

    public Equipment GetEquipment(string equipmentID)
    {
        return equipments.Find(e => e.equipmentID == equipmentID);
    }

    public EquipmentSet GetEquipmentSet(string setID)
    {
        return equipmentSets.Find(es => es.setID == setID);
    }

    public PlayerEquipment GetPlayerEquipment(string userID, string equipmentID)
    {
        return playerEquipments.Find(pe => pe.userID == userID && pe.equipmentID == equipmentID);
    }

    public EquipmentEnhance GetEquipmentEnhance(string enhanceID)
    {
        return equipmentEnhances.Find(ee => ee.enhanceID == enhanceID);
    }

    public EquipmentEvent GetEquipmentEvent(string eventID)
    {
        return equipmentEvents.Find(ee => ee.eventID == eventID);
    }

    public List<Equipment> GetEquipmentsByType(string equipmentType)
    {
        return equipments.FindAll(e => e.equipmentType == equipmentType);
    }

    public List<Equipment> GetEquipmentsByLevel(int level)
    {
        return equipments.FindAll(e => e.level == level);
    }

    public List<EquipmentSet> GetEquipmentSetsByType(string setType)
    {
        return equipmentSets.FindAll(es => es.setType == setType);
    }

    public List<PlayerEquipment> GetPlayerEquipmentsByUser(string userID)
    {
        return playerEquipments.FindAll(pe => pe.userID == userID);
    }

    public List<EquipmentEnhance> GetEquipmentEnhancesByEquipment(string equipmentID)
    {
        return equipmentEnhances.FindAll(ee => ee.equipmentID == equipmentID);
    }

    public List<EquipmentEvent> GetEquipmentEventsByUser(string userID)
    {
        return equipmentEvents.FindAll(ee => ee.userID == userID);
    }
}

[System.Serializable]
public class Equipment
{
    public string equipmentID;
    public string equipmentName;
    public string equipmentDescription;
    public string equipmentType;
    public int level;
    public int attack;
    public int defense;
    public int health;
    public string icon;
    public bool isEnabled;

    public Equipment(string id, string equipmentName, string equipmentDescription, string equipmentType, int level, int attack, int defense, int health, string icon)
    {
        equipmentID = id;
        this.equipmentName = equipmentName;
        this.equipmentDescription = equipmentDescription;
        this.equipmentType = equipmentType;
        this.level = level;
        this.attack = attack;
        this.defense = defense;
        this.health = health;
        this.icon = icon;
        isEnabled = true;
    }

    public void Enable()
    {
        isEnabled = true;
    }

    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class EquipmentSet
{
    public string setID;
    public string setName;
    public string setDescription;
    public string setType;
    public int requiredCount;
    public string setBonus;
    public string icon;
    public bool isEnabled;

    public EquipmentSet(string id, string setName, string setDescription, string setType, int requiredCount, string setBonus, string icon)
    {
        setID = id;
        this.setName = setName;
        this.setDescription = setDescription;
        this.setType = setType;
        this.requiredCount = requiredCount;
        this.setBonus = setBonus;
        this.icon = icon;
        isEnabled = true;
    }

    public void Enable()
    {
        isEnabled = true;
    }

    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class PlayerEquipment
{
    public string playerEquipmentID;
    public string userID;
    public string userName;
    public string equipmentID;
    public string equipmentName;
    public int enhanceLevel;
    public int exp;
    public bool isEquipped;
    public string equipTime;
    public List<EquipmentAttr> attrs;

    public PlayerEquipment(string id, string userID, string userName, string equipmentID, string equipmentName)
    {
        playerEquipmentID = id;
        this.userID = userID;
        this.userName = userName;
        this.equipmentID = equipmentID;
        this.equipmentName = equipmentName;
        enhanceLevel = 0;
        exp = 0;
        isEquipped = false;
        equipTime = "";
        attrs = new List<EquipmentAttr>();
    }

    public void Enhance()
    {
        enhanceLevel++;
        exp = 0;
    }

    public void AddExp(int amount)
    {
        exp += amount;
    }

    public void Equip()
    {
        isEquipped = true;
        equipTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void Unequip()
    {
        isEquipped = false;
        equipTime = "";
    }
}

[System.Serializable]
public class EquipmentAttr
{
    public string attrName;
    public int attrValue;
    public int enhanceBonus;

    public EquipmentAttr(string attrName, int attrValue, int enhanceBonus = 0)
    {
        this.attrName = attrName;
        this.attrValue = attrValue;
        this.enhanceBonus = enhanceBonus;
    }
}

[System.Serializable]
public class EquipmentEnhance
{
    public string enhanceID;
    public string equipmentID;
    public int level;
    public int requiredExp;
    public int attackBonus;
    public int defenseBonus;
    public int healthBonus;
    public int goldCost;
    public bool isEnabled;

    public EquipmentEnhance(string id, string equipmentID, int level, int requiredExp, int attackBonus, int defenseBonus, int healthBonus, int goldCost)
    {
        enhanceID = id;
        this.equipmentID = equipmentID;
        this.level = level;
        this.requiredExp = requiredExp;
        this.attackBonus = attackBonus;
        this.defenseBonus = defenseBonus;
        this.healthBonus = healthBonus;
        this.goldCost = goldCost;
        isEnabled = true;
    }
}

[System.Serializable]
public class EquipmentEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string equipmentID;
    public string description;
    public string timestamp;
    public string status;

    public EquipmentEvent(string id, string eventType, string userID, string equipmentID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.equipmentID = equipmentID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
    }

    public void MarkAsCompleted()
    {
        status = "completed";
    }

    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class EquipmentSystemDetailedManagerData
{
    public EquipmentSystemDetailed system;

    public EquipmentSystemDetailedManagerData()
    {
        system = new EquipmentSystemDetailed("equipment_system_detailed", "装备系统详细", "管理装备的详细功能，包括装备强化、洗练和继承");
    }
}
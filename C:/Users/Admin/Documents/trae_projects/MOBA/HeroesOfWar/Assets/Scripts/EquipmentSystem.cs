using UnityEngine;
using System.Collections.Generic;

public class EquipmentSystem : MonoBehaviour
{
    public static EquipmentSystem Instance { get; private set; }
    
    public List<Equipment> equipmentList = new List<Equipment>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEquipment();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeEquipment()
    {
        // 物理攻击装备
        equipmentList.Add(new Equipment
        {
            equipmentID = "sword",
            equipmentName = "铁剑",
            equipmentType = EquipmentType.Physical, 
            price = 300,
            stats = new EquipmentStats
            {
                attack = 20,
                armor = 0,
                magicResistance = 0,
                attackSpeed = 0,
                moveSpeed = 0,
                cooldownReduction = 0
            },
            description = "增加20点物理攻击"
        });
        
        equipmentList.Add(new Equipment
        {
            equipmentID = "dagger",
            equipmentName = "匕首",
            equipmentType = EquipmentType.Physical,
            price = 250,
            stats = new EquipmentStats
            {
                attack = 10,
                armor = 0,
                magicResistance = 0,
                attackSpeed = 0.1f,
                moveSpeed = 0,
                cooldownReduction = 0
            },
            description = "增加10点物理攻击和10%攻击速度"
        });
        
        // 魔法攻击装备
        equipmentList.Add(new Equipment
        {
            equipmentID = "wand",
            equipmentName = "魔法棒",
            equipmentType = EquipmentType.Magic,
            price = 300,
            stats = new EquipmentStats
            {
                attack = 0,
                armor = 0,
                magicResistance = 0,
                attackSpeed = 0,
                moveSpeed = 0,
                cooldownReduction = 0.05f
            },
            description = "增加5%冷却缩减"
        });
        
        // 防御装备
        equipmentList.Add(new Equipment
        {
            equipmentID = "chainmail",
            equipmentName = "锁子甲",
            equipmentType = EquipmentType.Defense,
            price = 400,
            stats = new EquipmentStats
            {
                attack = 0,
                armor = 25,
                magicResistance = 0,
                attackSpeed = 0,
                moveSpeed = 0,
                cooldownReduction = 0
            },
            description = "增加25点护甲"
        });
        
        equipmentList.Add(new Equipment
        {
            equipmentID = "cloak",
            equipmentName = "魔法斗篷",
            equipmentType = EquipmentType.Defense,
            price = 400,
            stats = new EquipmentStats
            {
                attack = 0,
                armor = 0,
                magicResistance = 25,
                attackSpeed = 0,
                moveSpeed = 0,
                cooldownReduction = 0
            },
            description = "增加25点魔法抗性"
        });
        
        // 移动装备
        equipmentList.Add(new Equipment
        {
            equipmentID = "boots",
            equipmentName = "速度之靴",
            equipmentType = EquipmentType.Movement,
            price = 300,
            stats = new EquipmentStats
            {
                attack = 0,
                armor = 0,
                magicResistance = 0,
                attackSpeed = 0,
                moveSpeed = 0.2f,
                cooldownReduction = 0
            },
            description = "增加20%移动速度"
        });
        
        // 高级装备
        equipmentList.Add(new Equipment
        {
            equipmentID = "sword_of_fury",
            equipmentName = "狂怒之剑",
            equipmentType = EquipmentType.Physical,
            price = 1500,
            stats = new EquipmentStats
            {
                attack = 50,
                armor = 0,
                magicResistance = 0,
                attackSpeed = 0.2f,
                moveSpeed = 0,
                cooldownReduction = 0.1f
            },
            description = "增加50点物理攻击，20%攻击速度和10%冷却缩减",
            isAdvanced = true,
            recipe = new string[] { "sword", "dagger" }
        });
    }
    
    public Equipment GetEquipmentByID(string equipmentID)
    {
        return equipmentList.Find(equipment => equipment.equipmentID == equipmentID);
    }
    
    public List<Equipment> GetEquipmentByType(EquipmentType type)
    {
        return equipmentList.FindAll(equipment => equipment.equipmentType == type);
    }
    
    public List<Equipment> GetAllEquipment()
    {
        return equipmentList;
    }
    
    public List<Equipment> GetBasicEquipment()
    {
        return equipmentList.FindAll(equipment => !equipment.isAdvanced);
    }
    
    public List<Equipment> GetAdvancedEquipment()
    {
        return equipmentList.FindAll(equipment => equipment.isAdvanced);
    }
    
    public bool CanCraftEquipment(string equipmentID, List<string> ownedEquipment)
    {
        Equipment equipment = GetEquipmentByID(equipmentID);
        if (equipment == null || !equipment.isAdvanced || equipment.recipe == null)
        {
            return false;
        }
        
        foreach (string requiredItem in equipment.recipe)
        {
            if (!ownedEquipment.Contains(requiredItem))
            {
                return false;
            }
        }
        
        return true;
    }
}

[System.Serializable]
public class Equipment
{
    public string equipmentID;
    public string equipmentName;
    public EquipmentType equipmentType;
    public int price;
    public EquipmentStats stats;
    public string description;
    public bool isAdvanced = false;
    public string[] recipe;
}

public enum EquipmentType
{
    Physical,
    Magic,
    Defense,
    Movement,
    Utility
}

[System.Serializable]
public class EquipmentStats
{
    public float attack;
    public float armor;
    public float magicResistance;
    public float attackSpeed;
    public float moveSpeed;
    public float cooldownReduction;
}

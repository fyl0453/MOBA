using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EquipmentSystemDetailedManager : MonoBehaviour
{
    public static EquipmentSystemDetailedManager Instance { get; private set; }

    public EquipmentSystemDetailedManagerData equipmentData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadEquipmentData();

        if (equipmentData == null)
        {
            equipmentData = new EquipmentSystemDetailedManagerData();
            InitializeDefaultEquipmentSystem();
        }
    }

    private void InitializeDefaultEquipmentSystem()
    {
        // 装备
        Equipment equip1 = new Equipment("equip_001", "破军", "物理攻击最高装备", "weapon", 15, 300, 0, 0, "icon_weapon_1");
        Equipment equip2 = new Equipment("equip_002", "泣血之刃", "物理吸血装备", "weapon", 10, 100, 0, 0, "icon_weapon_2");
        Equipment equip3 = new Equipment("equip_003", "无尽战刃", "暴击装备", "weapon", 12, 200, 0, 0, "icon_weapon_3");
        Equipment equip4 = new Equipment("equip_004", "反伤刺甲", "物理防御装备", "armor", 12, 0, 400, 0, "icon_armor_1");
        Equipment equip5 = new Equipment("equip_005", "魔女斗篷", "法术防御装备", "armor", 11, 0, 360, 0, "icon_armor_2");
        Equipment equip6 = new Equipment("equip_006", "霸者重装", "生命值装备", "armor", 10, 0, 200, 2000, "icon_armor_3");
        Equipment equip7 = new Equipment("equip_007", "急速战靴", "攻击速度装备", "boot", 8, 50, 0, 0, "icon_boot_1");
        Equipment equip8 = new Equipment("equip_008", "抵抗之靴", "韧性装备", "boot", 8, 0, 100, 0, "icon_boot_2");

        equipmentData.system.AddEquipment(equip1);
        equipmentData.system.AddEquipment(equip2);
        equipmentData.system.AddEquipment(equip3);
        equipmentData.system.AddEquipment(equip4);
        equipmentData.system.AddEquipment(equip5);
        equipmentData.system.AddEquipment(equip6);
        equipmentData.system.AddEquipment(equip7);
        equipmentData.system.AddEquipment(equip8);

        // 装备套装
        EquipmentSet set1 = new EquipmentSet("set_001", "物理攻击套装", "增加物理攻击", "attack", 4, "物理攻击+200", "icon_set_attack");
        EquipmentSet set2 = new EquipmentSet("set_002", "法术攻击套装", "增加法术攻击", "magic", 4, "法术攻击+200", "icon_set_magic");
        EquipmentSet set3 = new EquipmentSet("set_003", "防御套装", "增加物理和法术防御", "defense", 4, "双防+150", "icon_set_defense");

        equipmentData.system.AddEquipmentSet(set1);
        equipmentData.system.AddEquipmentSet(set2);
        equipmentData.system.AddEquipmentSet(set3);

        // 玩家装备
        PlayerEquipment playerEquip1 = new PlayerEquipment("player_equip_001", "user_001", "张三", "equip_001", "破军");
        PlayerEquipment playerEquip2 = new PlayerEquipment("player_equip_002", "user_001", "张三", "equip_002", "泣血之刃");
        PlayerEquipment playerEquip3 = new PlayerEquipment("player_equip_003", "user_001", "张三", "equip_004", "反伤刺甲");
        PlayerEquipment playerEquip4 = new PlayerEquipment("player_equip_004", "user_002", "李四", "equip_003", "无尽战刃");
        PlayerEquipment playerEquip5 = new PlayerEquipment("player_equip_005", "user_002", "李四", "equip_005", "魔女斗篷");

        playerEquip1.Enhance();
        playerEquip1.Enhance();
        playerEquip2.Enhance();
        playerEquip1.Equip();

        equipmentData.system.AddPlayerEquipment(playerEquip1);
        equipmentData.system.AddPlayerEquipment(playerEquip2);
        equipmentData.system.AddPlayerEquipment(playerEquip3);
        equipmentData.system.AddPlayerEquipment(playerEquip4);
        equipmentData.system.AddPlayerEquipment(playerEquip5);

        // 装备强化
        EquipmentEnhance enhance1 = new EquipmentEnhance("enhance_001", "equip_001", 1, 1000, 10, 0, 0, 500);
        EquipmentEnhance enhance2 = new EquipmentEnhance("enhance_002", "equip_001", 2, 2000, 20, 0, 0, 1000);
        EquipmentEnhance enhance3 = new EquipmentEnhance("enhance_003", "equip_001", 3, 3000, 30, 0, 0, 1500);
        EquipmentEnhance enhance4 = new EquipmentEnhance("enhance_004", "equip_002", 1, 800, 8, 0, 0, 400);
        EquipmentEnhance enhance5 = new EquipmentEnhance("enhance_005", "equip_002", 2, 1600, 16, 0, 0, 800);

        equipmentData.system.AddEquipmentEnhance(enhance1);
        equipmentData.system.AddEquipmentEnhance(enhance2);
        equipmentData.system.AddEquipmentEnhance(enhance3);
        equipmentData.system.AddEquipmentEnhance(enhance4);
        equipmentData.system.AddEquipmentEnhance(enhance5);

        // 装备事件
        EquipmentEvent event1 = new EquipmentEvent("event_001", "enhance", "user_001", "equip_001", "强化装备");
        EquipmentEvent event2 = new EquipmentEvent("event_002", "equip", "user_001", "equip_001", "装备武器");
        EquipmentEvent event3 = new EquipmentEvent("event_003", "obtain", "user_002", "equip_003", "获得装备");

        equipmentData.system.AddEquipmentEvent(event1);
        equipmentData.system.AddEquipmentEvent(event2);
        equipmentData.system.AddEquipmentEvent(event3);

        SaveEquipmentData();
    }

    // 装备管理
    public void AddEquipment(string equipmentName, string equipmentDescription, string equipmentType, int level, int attack, int defense, int health, string icon)
    {
        string equipmentID = "equip_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Equipment equipment = new Equipment(equipmentID, equipmentName, equipmentDescription, equipmentType, level, attack, defense, health, icon);
        equipmentData.system.AddEquipment(equipment);
        SaveEquipmentData();
        Debug.Log("成功添加装备: " + equipmentName);
    }

    public List<Equipment> GetEquipmentsByType(string equipmentType)
    {
        return equipmentData.system.GetEquipmentsByType(equipmentType);
    }

    // 装备套装管理
    public void AddEquipmentSet(string setName, string setDescription, string setType, int requiredCount, string setBonus, string icon)
    {
        string setID = "set_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EquipmentSet equipmentSet = new EquipmentSet(setID, setName, setDescription, setType, requiredCount, setBonus, icon);
        equipmentData.system.AddEquipmentSet(equipmentSet);
        SaveEquipmentData();
        Debug.Log("成功添加装备套装: " + setName);
    }

    // 玩家装备管理
    public void AddPlayerEquipment(string userID, string userName, string equipmentID)
    {
        Equipment equipment = equipmentData.system.GetEquipment(equipmentID);
        if (equipment != null && equipment.IsEnabled)
        {
            PlayerEquipment existingPlayerEquipment = equipmentData.system.GetPlayerEquipment(userID, equipmentID);
            if (existingPlayerEquipment == null)
            {
                string playerEquipmentID = "player_equip_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                PlayerEquipment newPlayerEquipment = new PlayerEquipment(playerEquipmentID, userID, userName, equipmentID, equipment.equipmentName);
                equipmentData.system.AddPlayerEquipment(newPlayerEquipment);

                CreateEquipmentEvent("obtain", userID, equipmentID, "获得装备: " + equipment.equipmentName);
                SaveEquipmentData();
                Debug.Log("成功添加玩家装备: " + equipment.equipmentName);
            }
            else
            {
                Debug.LogError("玩家已经拥有该装备");
            }
        }
        else
        {
            Debug.LogError("装备不存在或已禁用");
        }
    }

    public void EnhanceEquipment(string userID, string equipmentID)
    {
        PlayerEquipment playerEquipment = equipmentData.system.GetPlayerEquipment(userID, equipmentID);
        if (playerEquipment != null)
        {
            playerEquipment.Enhance();
            CreateEquipmentEvent("enhance", userID, equipmentID, "强化装备: " + playerEquipment.equipmentName);
            SaveEquipmentData();
            Debug.Log("成功强化装备: " + playerEquipment.equipmentName + " 强化等级: " + playerEquipment.enhanceLevel);
        }
        else
        {
            Debug.LogError("玩家装备不存在");
        }
    }

    public void EquipEquipment(string userID, string equipmentID)
    {
        PlayerEquipment playerEquipment = equipmentData.system.GetPlayerEquipment(userID, equipmentID);
        if (playerEquipment != null && !playerEquipment.isEquipped)
        {
            playerEquipment.Equip();
            CreateEquipmentEvent("equip", userID, equipmentID, "装备: " + playerEquipment.equipmentName);
            SaveEquipmentData();
            Debug.Log("成功装备: " + playerEquipment.equipmentName);
        }
        else
        {
            Debug.LogError("玩家装备不存在或已装备");
        }
    }

    public void UnequipEquipment(string userID, string equipmentID)
    {
        PlayerEquipment playerEquipment = equipmentData.system.GetPlayerEquipment(userID, equipmentID);
        if (playerEquipment != null && playerEquipment.isEquipped)
        {
            playerEquipment.Unequip();
            CreateEquipmentEvent("unequip", userID, equipmentID, "卸下装备: " + playerEquipment.equipmentName);
            SaveEquipmentData();
            Debug.Log("成功卸下装备: " + playerEquipment.equipmentName);
        }
        else
        {
            Debug.LogError("玩家装备不存在或未装备");
        }
    }

    public List<PlayerEquipment> GetPlayerEquipments(string userID)
    {
        return equipmentData.system.GetPlayerEquipmentsByUser(userID);
    }

    // 装备强化管理
    public void AddEquipmentEnhance(string equipmentID, int level, int requiredExp, int attackBonus, int defenseBonus, int healthBonus, int goldCost)
    {
        string enhanceID = "enhance_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EquipmentEnhance equipmentEnhance = new EquipmentEnhance(enhanceID, equipmentID, level, requiredExp, attackBonus, defenseBonus, healthBonus, goldCost);
        equipmentData.system.AddEquipmentEnhance(equipmentEnhance);
        SaveEquipmentData();
        Debug.Log("成功添加装备强化等级: " + level);
    }

    // 装备事件管理
    public string CreateEquipmentEvent(string eventType, string userID, string equipmentID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EquipmentEvent equipmentEvent = new EquipmentEvent(eventID, eventType, userID, equipmentID, description);
        equipmentData.system.AddEquipmentEvent(equipmentEvent);
        SaveEquipmentData();
        Debug.Log("成功创建装备事件: " + eventType);
        return eventID;
    }

    public List<EquipmentEvent> GetUserEvents(string userID)
    {
        return equipmentData.system.GetEquipmentEventsByUser(userID);
    }

    // 数据持久化
    public void SaveEquipmentData()
    {
        string path = Application.dataPath + "/Data/equipment_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, equipmentData);
        stream.Close();
    }

    public void LoadEquipmentData()
    {
        string path = Application.dataPath + "/Data/equipment_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            equipmentData = (EquipmentSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            equipmentData = new EquipmentSystemDetailedManagerData();
        }
    }
}
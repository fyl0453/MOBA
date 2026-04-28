using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }
    
    public EquipmentManagerData equipmentData;
    
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
            equipmentData = new EquipmentManagerData();
            InitializeDefaultEquipments();
        }
    }
    
    private void InitializeDefaultEquipments()
    {
        // 物理攻击装备
        Equipment blade = new Equipment("equipment_blade", "暗影战斧", "增加物理攻击和冷却缩减", "Physical", 2090);
        blade.AddStat("Attack", 85f);
        blade.AddStat("Health", 500f);
        blade.AddStat("CDR", 15f);
        blade.AddEffect("ArmorPenetration", "增加物理穿透", 50f);
        blade.AddEffect("Damage", "普通攻击有30%几率减少目标20%移动速度", 30f);
        blade.isEpic = true;
        equipmentData.AddEquipment(blade);
        
        Equipment bow = new Equipment("equipment_bow", "破晓", "增加物理攻击和攻速", "Physical", 3400);
        bow.AddStat("Attack", 50f);
        bow.AddStat("AttackSpeed", 35f);
        bow.AddStat("CritChance", 15f);
        bow.AddEffect("ArmorPenetration", "增加40%物理穿透", 40f);
        bow.isLegendary = true;
        equipmentData.AddEquipment(bow);
        
        // 法术攻击装备
        Equipment book = new Equipment("equipment_book", "博学者之怒", "增加法术强度", "Magic", 2300);
        book.AddStat("AP", 240f);
        book.AddEffect("MagicPower", "增加35%法术强度", 35f);
        book.isLegendary = true;
        equipmentData.AddEquipment(book);
        
        Equipment staff = new Equipment("equipment_staff", "回响之杖", "增加法术强度和移速", "Magic", 2100);
        staff.AddStat("AP", 180f);
        staff.AddStat("MovementSpeed", 7f);
        staff.AddEffect("MagicDamage", "技能命中后触发小范围爆炸", 0f);
        staff.isEpic = true;
        equipmentData.AddEquipment(staff);
        
        // 防御装备
        Equipment armor = new Equipment("equipment_armor", "不祥征兆", "增加物理防御和生命值", "Defense", 2180);
        armor.AddStat("Armor", 270f);
        armor.AddStat("Health", 1200f);
        armor.AddEffect("Slow", "受到攻击时减少攻击者30%攻速和移速", 30f);
        armor.isEpic = true;
        equipmentData.AddEquipment(armor);
        
        Equipment magicResist = new Equipment("equipment_magic_resist", "魔女斗篷", "增加法术防御和生命值", "Defense", 2120);
        magicResist.AddStat("MagicResist", 360f);
        magicResist.AddStat("Health", 1000f);
        magicResist.AddEffect("MagicShield", "获得一个吸收法术伤害的护盾", 0f);
        magicResist.isEpic = true;
        equipmentData.AddEquipment(magicResist);
        
        // 移动装备
        Equipment boots = new Equipment("equipment_boots", "影忍之足", "增加移动速度和物理防御", "Movement", 1000);
        boots.AddStat("MovementSpeed", 60f);
        boots.AddStat("Armor", 110f);
        boots.AddEffect("PhysicalReduction", "减少15%受到的普通攻击伤害", 15f);
        equipmentData.AddEquipment(boots);
        
        Equipment magicBoots = new Equipment("equipment_magic_boots", "抵抗之靴", "增加移动速度和法术防御", "Movement", 1100);
        magicBoots.AddStat("MovementSpeed", 60f);
        magicBoots.AddStat("MagicResist", 110f);
        magicBoots.AddEffect("CCReduction", "减少35%控制效果持续时间", 35f);
        equipmentData.AddEquipment(magicBoots);
        
        // 辅助装备
        Equipment support = new Equipment("equipment_support", "救赎之翼", "增加生命值和护盾", "Support", 1800);
        support.AddStat("Health", 1000f);
        support.AddStat("CDR", 10f);
        support.AddEffect("Shield", "为周围友军提供护盾", 0f);
        equipmentData.AddEquipment(support);
        
        // 物理攻击装备
        Equipment axe = new Equipment("equipment_axe", "战斧", "增加物理攻击和生命值", "Physical", 1500);
        axe.AddStat("Attack", 60f);
        axe.AddStat("Health", 300f);
        equipmentData.AddEquipment(axe);
        
        Equipment spear = new Equipment("equipment_spear", "破军", "增加高额物理攻击", "Physical", 3400);
        spear.AddStat("Attack", 180f);
        spear.AddEffect("Execute", "对生命值低于50%的目标造成额外30%伤害", 30f);
        spear.isLegendary = true;
        equipmentData.AddEquipment(spear);
        
        // 法术攻击装备
        Equipment wand = new Equipment("equipment_wand", "巫术法杖", "增加法术强度和生命值", "Magic", 2100);
        wand.AddStat("AP", 140f);
        wand.AddStat("Health", 500f);
        wand.AddEffect("MagicDamage", "技能命中后增加移动速度", 0f);
        equipmentData.AddEquipment(wand);
        
        Equipment orb = new Equipment("equipment_orb", "痛苦面具", "增加法术强度和生命值", "Magic", 2040);
        orb.AddStat("AP", 100f);
        orb.AddStat("Health", 500f);
        orb.AddEffect("MagicDamage", "技能命中后持续造成伤害", 0f);
        equipmentData.AddEquipment(orb);
        
        // 防御装备
        Equipment shield = new Equipment("equipment_shield", "血魔之怒", "增加生命值和攻击力", "Defense", 2120);
        shield.AddStat("Health", 1000f);
        shield.AddStat("Attack", 80f);
        shield.AddEffect("Shield", "生命值低于30%时获得护盾", 0f);
        equipmentData.AddEquipment(shield);
        
        Equipment belt = new Equipment("equipment_belt", "不死鸟之眼", "增加法术防御和生命值", "Defense", 2100);
        belt.AddStat("MagicResist", 240f);
        belt.AddStat("Health", 1200f);
        belt.AddEffect("Healing", "增加20%治疗效果", 20f);
        equipmentData.AddEquipment(belt);
        
        // 移动装备
        Equipment speedBoots = new Equipment("equipment_speed_boots", "疾步之靴", "增加移动速度", "Movement", 910);
        speedBoots.AddStat("MovementSpeed", 80f);
        speedBoots.AddEffect("SpeedBoost", "脱离战斗后移动速度增加", 0f);
        equipmentData.AddEquipment(speedBoots);
        
        // 辅助装备
        Equipment talisman = new Equipment("equipment_talisman", "极影", "增加冷却缩减和攻速", "Support", 1800);
        talisman.AddStat("CDR", 10f);
        talisman.AddEffect("Aura", "为周围友军增加攻速和冷却缩减", 0f);
        equipmentData.AddEquipment(talisman);
        
        // 装备套装
        EquipmentSet warriorSet = new EquipmentSet("set_warrior", "战士套装", "适合战士类英雄的装备套装");
        warriorSet.AddEquipment("equipment_blade");
        warriorSet.AddEquipment("equipment_armor");
        warriorSet.AddEquipment("equipment_boots");
        warriorSet.AddBonus(2, "增加10%生命值");
        warriorSet.AddBonus(3, "增加15%物理攻击");
        equipmentData.AddEquipmentSet(warriorSet);
        
        EquipmentSet mageSet = new EquipmentSet("set_mage", "法师套装", "适合法师类英雄的装备套装");
        mageSet.AddEquipment("equipment_book");
        mageSet.AddEquipment("equipment_staff");
        mageSet.AddEquipment("equipment_magic_resist");
        mageSet.AddBonus(2, "增加15%法术强度");
        mageSet.AddBonus(3, "技能冷却缩减+10%");
        equipmentData.AddEquipmentSet(mageSet);
        
        SaveEquipmentData();
    }
    
    public Equipment GetEquipment(string equipmentID)
    {
        return equipmentData.GetEquipment(equipmentID);
    }
    
    public List<Equipment> GetEquipmentsByType(string type)
    {
        return equipmentData.GetEquipmentsByType(type);
    }
    
    public List<Equipment> GetEpicEquipments()
    {
        return equipmentData.GetEpicEquipments();
    }
    
    public List<Equipment> GetLegendaryEquipments()
    {
        return equipmentData.GetLegendaryEquipments();
    }
    
    public EquipmentSet GetEquipmentSet(string setID)
    {
        return equipmentData.GetEquipmentSet(setID);
    }
    
    public List<EquipmentSet> GetAllEquipmentSets()
    {
        return equipmentData.equipmentSets;
    }
    
    public float CalculateTotalStat(string equipmentID, string statType)
    {
        Equipment equipment = GetEquipment(equipmentID);
        if (equipment != null)
        {
            return equipment.GetStatValue(statType);
        }
        return 0f;
    }
    
    public List<Equipment> GetRecommendedEquipments(string heroType)
    {
        List<Equipment> recommended = new List<Equipment>();
        
        switch (heroType)
        {
            case "Warrior":
                recommended.Add(GetEquipment("equipment_blade"));
                recommended.Add(GetEquipment("equipment_armor"));
                recommended.Add(GetEquipment("equipment_boots"));
                break;
            case "Mage":
                recommended.Add(GetEquipment("equipment_book"));
                recommended.Add(GetEquipment("equipment_staff"));
                recommended.Add(GetEquipment("equipment_magic_resist"));
                break;
            case "Archer":
                recommended.Add(GetEquipment("equipment_bow"));
                recommended.Add(GetEquipment("equipment_boots"));
                recommended.Add(GetEquipment("equipment_armor"));
                break;
            case "Assassin":
                recommended.Add(GetEquipment("equipment_blade"));
                recommended.Add(GetEquipment("equipment_bow"));
                recommended.Add(GetEquipment("equipment_boots"));
                break;
            case "Tank":
                recommended.Add(GetEquipment("equipment_armor"));
                recommended.Add(GetEquipment("equipment_magic_resist"));
                recommended.Add(GetEquipment("equipment_boots"));
                break;
            case "Support":
                recommended.Add(GetEquipment("equipment_support"));
                recommended.Add(GetEquipment("equipment_magic_resist"));
                recommended.Add(GetEquipment("equipment_boots"));
                break;
        }
        
        return recommended;
    }
    
    public void SaveEquipmentData()
    {
        string path = Application.dataPath + "/Data/equipment_data.dat";
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
        string path = Application.dataPath + "/Data/equipment_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            equipmentData = (EquipmentManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            equipmentData = new EquipmentManagerData();
        }
    }
}
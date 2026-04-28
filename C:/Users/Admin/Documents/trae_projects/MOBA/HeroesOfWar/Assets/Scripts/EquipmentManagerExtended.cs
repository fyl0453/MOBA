using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EquipmentManagerExtended : MonoBehaviour
{
    public static EquipmentManagerExtended Instance { get; private set; }
    
    public EquipmentSystemExtendedData equipmentData;
    
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
            equipmentData = new EquipmentSystemExtendedData();
            InitializeDefaultEquipment();
        }
    }
    
    private void InitializeDefaultEquipment()
    {
        // 创建装备分类
        EquipmentCategory物理 = new EquipmentCategory("category_physical", "物理", "物理输出装备");
        EquipmentCategory法术 = new EquipmentCategory("category_magic", "法术", "法术输出装备");
        EquipmentCategory防御 = new EquipmentCategory("category_defense", "防御", "防御装备");
        EquipmentCategory移动 = new EquipmentCategory("category_movement", "移动", "移动装备");
        EquipmentCategory辅助 = new EquipmentCategory("category_support", "辅助", "辅助装备");
        
        equipmentData.system.AddCategory(物理);
        equipmentData.system.AddCategory(法术);
        equipmentData.system.AddCategory(防御);
        equipmentData.system.AddCategory(移动);
        equipmentData.system.AddCategory(辅助);
        
        // 创建基础装备
        Equipment blade = new Equipment("equipment_blade", "铁剑", "增加攻击力", "category_physical", "Basic", 250);
        blade.AddAttribute("AttackDamage", 15f);
        
        Equipment cloak = new Equipment("equipment_cloak", "布甲", "增加物理防御", "category_defense", "Basic", 220);
        cloak.AddAttribute("Armor", 20f);
        
        Equipment book = new Equipment("equipment_book", "咒术典籍", "增加法术强度", "category_magic", "Basic", 300);
        book.AddAttribute("AbilityPower", 20f);
        
        Equipment boots = new Equipment("equipment_boots", "神速之靴", "增加移动速度", "category_movement", "Basic", 250);
        boots.AddAttribute("MovementSpeed", 30f);
        
        // 创建中级装备
        Equipment dagger = new Equipment("equipment_dagger", "匕首", "增加攻击速度", "category_physical", "Advanced", 410);
        dagger.AddAttribute("AttackSpeed", 0.15f);
        
        Equipment chain = new Equipment("equipment_chain", "力量腰带", "增加生命值", "category_defense", "Advanced", 300);
        chain.AddAttribute("Health", 100f);
        
        Equipment staff = new Equipment("equipment_staff", "大棒", "增加法术强度", "category_magic", "Advanced", 820);
        staff.AddAttribute("AbilityPower", 40f);
        
        // 创建高级装备
        Equipment bladeOfRuinedKing = new Equipment("equipment_blade_ruined_king", "破败王者之刃", "物理输出核心装备", "category_physical", "Epic", 3200);
        bladeOfRuinedKing.AddAttribute("AttackDamage", 40f);
        bladeOfRuinedKing.AddAttribute("AttackSpeed", 0.25f);
        bladeOfRuinedKing.AddAttribute("LifeSteal", 0.10f);
        bladeOfRuinedKing.AddPassive(new EquipmentPassive("passive_ruined_king", "破败", "普通攻击造成目标当前生命值8%的物理伤害", "普通攻击", 1.5f));
        bladeOfRuinedKing.AddToRecipe("equipment_blade");
        bladeOfRuinedKing.AddToRecipe("equipment_dagger");
        bladeOfRuinedKing.SetEpic(true);
        
        Equipment guardianAngel = new Equipment("equipment_guardian_angel", "守护天使", "防御核心装备", "category_defense", "Epic", 2800);
        guardianAngel.AddAttribute("Armor", 45f);
        guardianAngel.AddAttribute("MagicResist", 45f);
        guardianAngel.AddPassive(new EquipmentPassive("passive_guardian_angel", "守护", "死亡后2秒原地复活，恢复50%生命值和30%法力值", "死亡", 180f));
        guardianAngel.AddToRecipe("equipment_cloak");
        guardianAngel.AddToRecipe("equipment_chain");
        guardianAngel.SetEpic(true);
        
        Equipment rodOfAges = new Equipment("equipment_rod_of_ages", "时光之杖", "法术核心装备", "category_magic", "Epic", 3000);
        rodOfAges.AddAttribute("Health", 300f);
        rodOfAges.AddAttribute("Mana", 300f);
        rodOfAges.AddAttribute("AbilityPower", 60f);
        rodOfAges.AddPassive(new EquipmentPassive("passive_rod_of_ages", "时光", "每10分钟增加20生命值、20法力值和4法术强度，最多叠加10层", "游戏时间", 0f));
        rodOfAges.AddToRecipe("equipment_book");
        rodOfAges.AddToRecipe("equipment_staff");
        rodOfAges.SetEpic(true);
        
        // 创建传说装备
        Equipment infinityEdge = new Equipment("equipment_infinity_edge", "无尽战刃", "物理输出传说装备", "category_physical", "Legendary", 3600);
        infinityEdge.AddAttribute("AttackDamage", 80f);
        infinityEdge.AddAttribute("CriticalChance", 0.25f);
        infinityEdge.AddPassive(new EquipmentPassive("passive_infinity_edge", "无尽", "暴击伤害提升50%", "暴击", 0f));
        infinityEdge.AddToRecipe("equipment_blade");
        infinityEdge.AddToRecipe("equipment_dagger");
        infinityEdge.SetLegendary(true);
        
        Equipment rabadonsDeathcap = new Equipment("equipment_rabadons_deathcap", "灭世者的死亡之帽", "法术输出传说装备", "category_magic", "Legendary", 3800);
        rabadonsDeathcap.AddAttribute("AbilityPower", 120f);
        rabadonsDeathcap.AddPassive(new EquipmentPassive("passive_rabadons_deathcap", "死亡之帽", "法术强度提升35%", "装备", 0f));
        rabadonsDeathcap.AddToRecipe("equipment_book");
        rabadonsDeathcap.AddToRecipe("equipment_staff");
        rabadonsDeathcap.SetLegendary(true);
        
        equipmentData.system.AddEquipment(blade);
        equipmentData.system.AddEquipment(cloak);
        equipmentData.system.AddEquipment(book);
        equipmentData.system.AddEquipment(boots);
        equipmentData.system.AddEquipment(dagger);
        equipmentData.system.AddEquipment(chain);
        equipmentData.system.AddEquipment(staff);
        equipmentData.system.AddEquipment(bladeOfRuinedKing);
        equipmentData.system.AddEquipment(guardianAngel);
        equipmentData.system.AddEquipment(rodOfAges);
        equipmentData.system.AddEquipment(infinityEdge);
        equipmentData.system.AddEquipment(rabadonsDeathcap);
        
        // 创建装备推荐
        EquipmentBuild guanyuBuild = new EquipmentBuild("build_guanyu_1", "关羽输出装", "适合关羽的物理输出装备", "hero_guanyu", "关羽", "战士");
        guanyuBuild.AddCoreItem("equipment_boots");
        guanyuBuild.AddCoreItem("equipment_blade_ruined_king");
        guanyuBuild.AddCoreItem("equipment_infinity_edge");
        guanyuBuild.AddSituationalItem("equipment_guardian_angel");
        
        EquipmentBuild zhugeliangBuild = new EquipmentBuild("build_zhugeliang_1", "诸葛亮输出装", "适合诸葛亮的法术输出装备", "hero_zhugeliang", "诸葛亮", "法师");
        zhugeliangBuild.AddCoreItem("equipment_boots");
        zhugeliangBuild.AddCoreItem("equipment_rod_of_ages");
        zhugeliangBuild.AddCoreItem("equipment_rabadons_deathcap");
        zhugeliangBuild.AddSituationalItem("equipment_guardian_angel");
        
        equipmentData.system.AddBuild(guanyuBuild);
        equipmentData.system.AddBuild(zhugeliangBuild);
        
        SaveEquipmentData();
    }
    
    public void PurchaseEquipment(string playerID, string equipmentID)
    {
        Equipment equipment = equipmentData.system.GetEquipment(equipmentID);
        if (equipment != null)
        {
            PlayerEquipmentData playerData = GetOrCreatePlayerData(playerID);
            
            if (!playerData.OwnsEquipment(equipmentID))
            {
                if (ProfileManager.Instance.currentProfile.gold >= equipment.price)
                {
                    ProfileManager.Instance.currentProfile.gold -= equipment.price;
                    ProfileManager.Instance.SaveProfile();
                    
                    playerData.AddEquipment(equipmentID);
                    SaveEquipmentData();
                    Debug.Log($"成功购买装备: {equipment.equipmentName}");
                }
                else
                {
                    Debug.Log("金币不足");
                }
            }
            else
            {
                Debug.Log("已经拥有该装备");
            }
        }
    }
    
    public string CreateEquipmentBuild(string playerID, string buildName, string buildDescription, string heroID, string heroName, string playStyle, List<string> coreItems, List<string> situationalItems)
    {
        string buildID = System.Guid.NewGuid().ToString();
        EquipmentBuild build = new EquipmentBuild(buildID, buildName, buildDescription, heroID, heroName, playStyle);
        
        foreach (string item in coreItems)
        {
            build.AddCoreItem(item);
        }
        
        foreach (string item in situationalItems)
        {
            build.AddSituationalItem(item);
        }
        
        equipmentData.system.AddBuild(build);
        
        PlayerEquipmentData playerData = GetOrCreatePlayerData(playerID);
        playerData.AddSavedBuild(buildID);
        
        SaveEquipmentData();
        Debug.Log($"成功创建装备推荐: {buildName}");
        return buildID;
    }
    
    public void SaveBuild(string playerID, string buildID)
    {
        EquipmentBuild build = equipmentData.system.GetBuild(buildID);
        if (build != null)
        {
            PlayerEquipmentData playerData = GetOrCreatePlayerData(playerID);
            playerData.AddSavedBuild(buildID);
            SaveEquipmentData();
            Debug.Log($"成功保存装备推荐: {build.buildName}");
        }
    }
    
    public void RemoveSavedBuild(string playerID, string buildID)
    {
        PlayerEquipmentData playerData = GetOrCreatePlayerData(playerID);
        playerData.RemoveSavedBuild(buildID);
        SaveEquipmentData();
        Debug.Log("成功移除保存的装备推荐");
    }
    
    public List<Equipment> GetEquipmentsByCategory(string categoryID)
    {
        return equipmentData.system.GetEquipmentsByCategory(categoryID);
    }
    
    public List<Equipment> GetEquipmentsByType(string equipmentType)
    {
        return equipmentData.system.GetEquipmentsByType(equipmentType);
    }
    
    public List<EquipmentBuild> GetBuildsByHero(string heroID)
    {
        return equipmentData.system.GetBuildsByHero(heroID);
    }
    
    public List<EquipmentBuild> GetSavedBuilds(string playerID)
    {
        PlayerEquipmentData playerData = GetOrCreatePlayerData(playerID);
        List<EquipmentBuild> savedBuilds = new List<EquipmentBuild>();
        
        foreach (string buildID in playerData.savedBuilds)
        {
            EquipmentBuild build = equipmentData.system.GetBuild(buildID);
            if (build != null)
            {
                savedBuilds.Add(build);
            }
        }
        
        return savedBuilds;
    }
    
    public Equipment GetEquipment(string equipmentID)
    {
        return equipmentData.system.GetEquipment(equipmentID);
    }
    
    public EquipmentBuild GetBuild(string buildID)
    {
        return equipmentData.system.GetBuild(buildID);
    }
    
    public bool OwnsEquipment(string playerID, string equipmentID)
    {
        PlayerEquipmentData playerData = GetOrCreatePlayerData(playerID);
        return playerData.OwnsEquipment(equipmentID);
    }
    
    public List<Equipment> GetEquipmentRecipe(string equipmentID)
    {
        Equipment equipment = equipmentData.system.GetEquipment(equipmentID);
        if (equipment != null && equipment.recipe.Count > 0)
        {
            List<Equipment> recipe = new List<Equipment>();
            foreach (string itemID in equipment.recipe)
            {
                Equipment item = equipmentData.system.GetEquipment(itemID);
                if (item != null)
                {
                    recipe.Add(item);
                }
            }
            return recipe;
        }
        return new List<Equipment>();
    }
    
    private PlayerEquipmentData GetOrCreatePlayerData(string playerID)
    {
        PlayerEquipmentData playerData = equipmentData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerEquipmentData(playerID);
            equipmentData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public void SaveEquipmentData()
    {
        string path = Application.dataPath + "/Data/equipment_system_extended_data.dat";
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
        string path = Application.dataPath + "/Data/equipment_system_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            equipmentData = (EquipmentSystemExtendedData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            equipmentData = new EquipmentSystemExtendedData();
        }
    }
}
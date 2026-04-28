using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TalentManager : MonoBehaviour
{
    public static TalentManager Instance { get; private set; }
    
    public List<Talent> allTalents;
    public List<TalentPage> talentPages;
    public TalentPage currentPage;
    
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
        LoadTalentData();
        LoadTalentPages();
        
        if (allTalents.Count == 0)
        {
            InitializeDefaultTalents();
        }
        
        if (talentPages.Count == 0)
        {
            CreateDefaultTalentPages();
        }
    }
    
    private void InitializeDefaultTalents()
    {
        Talent t1 = new Talent("talent_001", "祸源", "暴击率+1.6%", 1);
        t1.effects.Add(new TalentEffect("crit_rate", 0.016f, "self"));
        allTalents.Add(t1);
        
        Talent t2 = new Talent("talent_002", "鹰眼", "物理攻击+0.9%，法术攻击+0.9%", 1);
        t2.effects.Add(new TalentEffect("physical_attack", 0.009f, "self"));
        t2.effects.Add(new TalentEffect("magic_attack", 0.009f, "self"));
        allTalents.Add(t2);
        
        Talent t3 = new Talent("talent_003", "无双", "暴击率+0.7%，暴击效果+2.3%", 1);
        t3.effects.Add(new TalentEffect("crit_rate", 0.007f, "self"));
        t3.effects.Add(new TalentEffect("crit_damage", 0.023f, "self"));
        allTalents.Add(t3);
        
        Talent t4 = new Talent("talent_004", "隐匿", "物理攻击+1.6%，移速+1%", 2);
        t4.effects.Add(new TalentEffect("physical_attack", 0.016f, "self"));
        t4.effects.Add(new TalentEffect("move_speed", 0.01f, "self"));
        allTalents.Add(t4);
        
        Talent t5 = new Talent("talent_005", "敬畏", "物理防御+2.4%，法术防御+2.4%", 2);
        t5.effects.Add(new TalentEffect("physical_defense", 2.4f, "self"));
        t5.effects.Add(new TalentEffect("magic_defense", 2.4f, "self"));
        allTalents.Add(t5);
        
        Talent t6 = new Talent("talent_006", "长生", "最大生命+34.5", 2);
        t6.effects.Add(new TalentEffect("max_health", 34.5f, "self"));
        allTalents.Add(t6);
        
        Talent t7 = new Talent("talent_007", "异变", "物理攻击+2.0%，物理穿透+3.2", 3);
        t7.effects.Add(new TalentEffect("physical_attack", 0.02f, "self"));
        t7.effects.Add(new TalentEffect("physical_penetration", 3.2f, "self"));
        allTalents.Add(t7);
        
        Talent t8 = new Talent("talent_008", "虚空", "最大生命+3.75%，冷却缩减+0.6%", 3);
        t8.effects.Add(new TalentEffect("max_health_percent", 0.0375f, "self"));
        t8.effects.Add(new TalentEffect("cooldown_reduction", 0.006f, "self"));
        allTalents.Add(t8);
        
        Talent t9 = new Talent("talent_009", "怜悯", "冷却缩减+1.0%，移速+0.5%", 3);
        t9.effects.Add(new TalentEffect("cooldown_reduction", 0.01f, "self"));
        t9.effects.Add(new TalentEffect("move_speed", 0.005f, "self"));
        allTalents.Add(t9);
        
        SaveTalentData();
    }
    
    private void CreateDefaultTalentPages()
    {
        TalentPage page1 = new TalentPage("page_001", "物理攻击");
        page1.slots[0].equippedTalentID = "talent_002";
        page1.slots[3].equippedTalentID = "talent_004";
        page1.slots[6].equippedTalentID = "talent_007";
        page1.recommendedHeroID = "hero_001";
        page1.isDefault = true;
        talentPages.Add(page1);
        
        TalentPage page2 = new TalentPage("page_002", "法术攻击");
        page2.slots[0].equippedTalentID = "talent_002";
        page2.slots[3].equippedTalentID = "talent_005";
        page2.slots[6].equippedTalentID = "talent_008";
        page2.recommendedHeroID = "hero_002";
        talentPages.Add(page2);
        
        TalentPage page3 = new TalentPage("page_003", "坦克生存");
        page3.slots[0].equippedTalentID = "talent_006";
        page3.slots[3].equippedTalentID = "talent_005";
        page3.slots[6].equippedTalentID = "talent_008";
        page3.recommendedHeroID = "hero_003";
        talentPages.Add(page3);
        
        currentPage = page1;
        SaveTalentPages();
    }
    
    public List<Talent> GetTalentsByTier(int tier)
    {
        return allTalents.FindAll(t => t.tier == tier);
    }
    
    public List<Talent> GetTalentsForHero(string heroID)
    {
        List<Talent> recommendedTalents = new List<Talent>();
        
        foreach (TalentPage page in talentPages)
        {
            if (page.recommendedHeroID == heroID)
            {
                foreach (TalentSlot slot in page.slots)
                {
                    Talent talent = GetTalentByID(slot.equippedTalentID);
                    if (talent != null)
                    {
                        recommendedTalents.Add(talent);
                    }
                }
                break;
            }
        }
        
        return recommendedTalents;
    }
    
    public Talent GetTalentByID(string talentID)
    {
        return allTalents.Find(t => t.talentID == talentID);
    }
    
    public void EquipTalent(int slotIndex, string talentID)
    {
        TalentSlot slot = currentPage.slots.Find(s => s.slotIndex == slotIndex);
        if (slot != null)
        {
            Talent talent = GetTalentByID(talentID);
            if (talent != null && talent.tier == slot.tierLevel)
            {
                slot.equippedTalentID = talentID;
                SaveTalentPages();
                ApplyTalentEffects();
            }
        }
    }
    
    public void CreateNewPage(string pageName)
    {
        string newPageID = $"page_{System.DateTime.Now.Ticks}";
        TalentPage newPage = new TalentPage(newPageID, pageName);
        talentPages.Add(newPage);
        SaveTalentPages();
    }
    
    public void DeletePage(string pageID)
    {
        TalentPage page = talentPages.Find(p => p.pageID == pageID);
        if (page != null && !page.isDefault)
        {
            talentPages.Remove(page);
            SaveTalentPages();
        }
    }
    
    public void SelectPage(string pageID)
    {
        TalentPage page = talentPages.Find(p => p.pageID == pageID);
        if (page != null)
        {
            currentPage = page;
            ApplyTalentEffects();
        }
    }
    
    public void ApplyTalentEffects()
    {
        if (currentPage == null) return;
        
        PlayerStats playerStats = GameObject.FindObjectOfType<PlayerStats>();
        if (playerStats == null) return;
        
        playerStats.ResetTalentEffects();
        
        foreach (TalentSlot slot in currentPage.slots)
        {
            if (!string.IsNullOrEmpty(slot.equippedTalentID))
            {
                Talent talent = GetTalentByID(slot.equippedTalentID);
                if (talent != null)
                {
                    ApplyTalentEffect(playerStats, talent);
                }
            }
        }
    }
    
    private void ApplyTalentEffect(PlayerStats stats, Talent talent)
    {
        foreach (TalentEffect effect in talent.effects)
        {
            switch (effect.effectType)
            {
                case "crit_rate":
                    stats.critRate += effect.value;
                    break;
                case "crit_damage":
                    stats.critDamage += effect.value;
                    break;
                case "physical_attack":
                    stats.physicalAttack += effect.value;
                    break;
                case "magic_attack":
                    stats.magicAttack += effect.value;
                    break;
                case "physical_defense":
                    stats.physicalDefense += effect.value;
                    break;
                case "magic_defense":
                    stats.magicDefense += effect.value;
                    break;
                case "max_health":
                    stats.maxHealth += effect.value;
                    break;
                case "max_health_percent":
                    stats.maxHealth *= (1 + effect.value);
                    break;
                case "move_speed":
                    stats.moveSpeed *= (1 + effect.value);
                    break;
                case "cooldown_reduction":
                    stats.cooldownReduction += effect.value;
                    break;
                case "physical_penetration":
                    stats.physicalPenetration += effect.value;
                    break;
            }
        }
    }
    
    public void SaveTalentData()
    {
        string path = Application.dataPath + "/Data/talent_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, allTalents);
        stream.Close();
    }
    
    public void LoadTalentData()
    {
        string path = Application.dataPath + "/Data/talent_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            allTalents = (List<Talent>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            allTalents = new List<Talent>();
        }
    }
    
    public void SaveTalentPages()
    {
        string path = Application.dataPath + "/Data/talent_pages.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, talentPages);
        stream.Close();
    }
    
    public void LoadTalentPages()
    {
        string path = Application.dataPath + "/Data/talent_pages.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            talentPages = (List<TalentPage>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            talentPages = new List<TalentPage>();
        }
    }
}
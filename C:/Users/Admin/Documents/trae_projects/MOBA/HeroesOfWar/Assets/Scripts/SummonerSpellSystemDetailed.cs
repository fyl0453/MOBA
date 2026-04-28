using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SummonerSpell
{
    public string SpellID;
    public string SpellName;
    public string Description;
    public int Cooldown;
    public int ManaCost;
    public int LevelRequired;
    public string SpellType;
    public string IconName;
    public string AnimationName;
    public string EffectDescription;
    public bool IsUnlocked;

    public SummonerSpell(string spellID, string spellName, string description, int cooldown, int manaCost, int levelRequired, string spellType)
    {
        SpellID = spellID;
        SpellName = spellName;
        Description = description;
        Cooldown = cooldown;
        ManaCost = manaCost;
        LevelRequired = levelRequired;
        SpellType = spellType;
        IconName = "";
        AnimationName = "";
        EffectDescription = "";
        IsUnlocked = false;
    }
}

[Serializable]
public class PlayerSummonerSpell
{
    public string PlayerID;
    public string SpellID;
    public int Level;
    public int Experience;
    public int UsageCount;
    public DateTime LastUsed;
    public bool IsEquipped;

    public PlayerSummonerSpell(string playerID, string spellID)
    {
        PlayerID = playerID;
        SpellID = spellID;
        Level = 1;
        Experience = 0;
        UsageCount = 0;
        LastUsed = DateTime.MinValue;
        IsEquipped = false;
    }
}

[Serializable]
public class SummonerSpellSlot
{
    public string SlotID;
    public string PlayerID;
    public string SpellID;
    public int SlotIndex;
    public DateTime EquipTime;

    public SummonerSpellSlot(string slotID, string playerID, int slotIndex)
    {
        SlotID = slotID;
        PlayerID = playerID;
        SpellID = "";
        SlotIndex = slotIndex;
        EquipTime = DateTime.Now;
    }
}

[Serializable]
public class SpellLevel
{
    public int Level;
    public int RequiredExperience;
    public int CooldownReduction;
    public int ManaCostReduction;
    public string BonusEffect;

    public SpellLevel(int level, int requiredExperience, int cooldownReduction, int manaCostReduction, string bonusEffect)
    {
        Level = level;
        RequiredExperience = requiredExperience;
        CooldownReduction = cooldownReduction;
        ManaCostReduction = manaCostReduction;
        BonusEffect = bonusEffect;
    }
}

[Serializable]
public class SummonerSpellSystemData
{
    public List<SummonerSpell> AllSpells;
    public List<SpellLevel> SpellLevels;
    public Dictionary<string, List<PlayerSummonerSpell>> PlayerSpells;
    public Dictionary<string, List<SummonerSpellSlot>> PlayerSpellSlots;
    public DateTime LastUpdateTime;

    public SummonerSpellSystemData()
    {
        AllSpells = new List<SummonerSpell>();
        SpellLevels = new List<SpellLevel>();
        PlayerSpells = new Dictionary<string, List<PlayerSummonerSpell>>();
        PlayerSpellSlots = new Dictionary<string, List<SummonerSpellSlot>>();
        LastUpdateTime = DateTime.Now;
        InitializeDefaultSpells();
        InitializeDefaultSpellLevels();
    }

    private void InitializeDefaultSpells()
    {
        SummonerSpell flash = new SummonerSpell("spell_flash", "闪现", "向目标方向闪现一段距离", 300, 0, 1, "movement");
        AllSpells.Add(flash);

        SummonerSpell heal = new SummonerSpell("spell_heal", "治疗", "为自己和附近队友恢复生命值", 240, 50, 1, "healing");
        AllSpells.Add(heal);

        SummonerSpell ignite = new SummonerSpell("spell_ignite", "点燃", "对目标造成持续伤害", 180, 50, 4, "damage");
        AllSpells.Add(ignite);

        SummonerSpell barrier = new SummonerSpell("spell_barrier", "屏障", "获得一个护盾", 180, 50, 4, "defense");
        AllSpells.Add(barrier);

        SummonerSpell teleport = new SummonerSpell("spell_teleport", "传送", "传送到友方单位或防御塔", 360, 75, 6, "utility");
        AllSpells.Add(teleport);
    }

    private void InitializeDefaultSpellLevels()
    {
        SpellLevel level1 = new SpellLevel(1, 0, 0, 0, "基础效果");
        SpellLevels.Add(level1);

        SpellLevel level2 = new SpellLevel(2, 1000, 10, 5, "冷却时间减少10秒");
        SpellLevels.Add(level2);

        SpellLevel level3 = new SpellLevel(3, 3000, 20, 10, "冷却时间减少20秒，法力消耗减少10点");
        SpellLevels.Add(level3);

        SpellLevel level4 = new SpellLevel(4, 6000, 30, 15, "冷却时间减少30秒，法力消耗减少15点");
        SpellLevels.Add(level4);

        SpellLevel level5 = new SpellLevel(5, 10000, 40, 20, "冷却时间减少40秒，法力消耗减少20点");
        SpellLevels.Add(level5);
    }

    public void AddSpell(SummonerSpell spell)
    {
        AllSpells.Add(spell);
    }

    public void AddSpellLevel(SpellLevel level)
    {
        SpellLevels.Add(level);
    }

    public void AddPlayerSpell(string playerID, PlayerSummonerSpell spell)
    {
        if (!PlayerSpells.ContainsKey(playerID))
        {
            PlayerSpells[playerID] = new List<PlayerSummonerSpell>();
        }
        PlayerSpells[playerID].Add(spell);
    }

    public void AddPlayerSpellSlot(string playerID, SummonerSpellSlot slot)
    {
        if (!PlayerSpellSlots.ContainsKey(playerID))
        {
            PlayerSpellSlots[playerID] = new List<SummonerSpellSlot>();
        }
        PlayerSpellSlots[playerID].Add(slot);
    }
}

[Serializable]
public class SummonerSpellEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string SpellID;
    public string EventData;

    public SummonerSpellEvent(string eventID, string eventType, string playerID, string spellID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        SpellID = spellID;
        EventData = eventData;
    }
}

public class SummonerSpellSystemDataManager
{
    private static SummonerSpellSystemDataManager _instance;
    public static SummonerSpellSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SummonerSpellSystemDataManager();
            }
            return _instance;
        }
    }

    public SummonerSpellSystemData spellData;
    private List<SummonerSpellEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SummonerSpellSystemDataManager()
    {
        spellData = new SummonerSpellSystemData();
        recentEvents = new List<SummonerSpellEvent>();
        LoadSpellData();
    }

    public void SaveSpellData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SummonerSpellSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, spellData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存召唤师技能系统数据失败: " + e.Message);
        }
    }

    public void LoadSpellData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SummonerSpellSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    spellData = (SummonerSpellSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载召唤师技能系统数据失败: " + e.Message);
            spellData = new SummonerSpellSystemData();
        }
    }

    public void CreateSpellEvent(string eventType, string playerID, string spellID, string eventData)
    {
        string eventID = "spell_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SummonerSpellEvent spellEvent = new SummonerSpellEvent(eventID, eventType, playerID, spellID, eventData);
        recentEvents.Add(spellEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<SummonerSpellEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Rune
{
    public string RuneID;
    public string RuneName;
    public string Description;
    public int RuneType;
    public int Level;
    public int Cost;
    public Dictionary<string, int> Attributes;
    public string IconName;
    public int Rarity;
    public string SetID;

    public Rune(string runeID, string runeName, string description, int runeType, int level, int cost, Dictionary<string, int> attributes, string setID = "")
    {
        RuneID = runeID;
        RuneName = runeName;
        Description = description;
        RuneType = runeType;
        Level = level;
        Cost = cost;
        Attributes = attributes;
        IconName = "";
        Rarity = 1;
        SetID = setID;
    }
}

[Serializable]
public class RunePage
{
    public string PageID;
    public string PlayerID;
    public string PageName;
    public List<string> RuneSlots;
    public int TotalAttributes;
    public DateTime CreateTime;
    public DateTime LastUpdateTime;

    public RunePage(string pageID, string playerID, string pageName)
    {
        PageID = pageID;
        PlayerID = playerID;
        PageName = pageName;
        RuneSlots = new List<string>();
        for (int i = 0; i < 30; i++)
        {
            RuneSlots.Add("");
        }
        TotalAttributes = 0;
        CreateTime = DateTime.Now;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class RuneSet
{
    public string SetID;
    public string SetName;
    public string Description;
    public List<string> RuneIDs;
    public Dictionary<string, int> SetBonus;
    public int BonusRequirement;
    public string IconName;

    public RuneSet(string setID, string setName, string description, List<string> runeIDs, Dictionary<string, int> setBonus, int bonusRequirement)
    {
        SetID = setID;
        SetName = setName;
        Description = description;
        RuneIDs = runeIDs;
        SetBonus = setBonus;
        BonusRequirement = bonusRequirement;
        IconName = "";
    }
}

[Serializable]
public class PlayerRuneInventory
{
    public string PlayerID;
    public Dictionary<string, int> RuneCounts;
    public List<RunePage> RunePages;
    public int TotalRunes;
    public int MaxPages;
    public DateTime LastUpdateTime;

    public PlayerRuneInventory(string playerID)
    {
        PlayerID = playerID;
        RuneCounts = new Dictionary<string, int>();
        RunePages = new List<RunePage>();
        TotalRunes = 0;
        MaxPages = 6;
        LastUpdateTime = DateTime.Now;
        CreateDefaultPage();
    }

    private void CreateDefaultPage()
    {
        string pageID = "page_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RunePage defaultPage = new RunePage(pageID, PlayerID, "默认铭文页");
        RunePages.Add(defaultPage);
    }
}

[Serializable]
public class RuneSystemData
{
    public List<Rune> AllRunes;
    public List<RuneSet> RuneSets;
    public Dictionary<string, PlayerRuneInventory> PlayerInventories;
    public DateTime LastUpdateTime;

    public RuneSystemData()
    {
        AllRunes = new List<Rune>();
        RuneSets = new List<RuneSet>();
        PlayerInventories = new Dictionary<string, PlayerRuneInventory>();
        LastUpdateTime = DateTime.Now;
        InitializeDefaultRunes();
        InitializeDefaultRuneSets();
    }

    private void InitializeDefaultRunes()
    {
        
        Dictionary<string, int> attackAttributes = new Dictionary<string, int>();
        attackAttributes["物理攻击"] = 10;
        Rune attackRune = new Rune("rune_attack", "攻击铭文", "增加物理攻击", 1, 5, 1000, attackAttributes);
        AllRunes.Add(attackRune);

        Dictionary<string, int> magicAttributes = new Dictionary<string, int>();
        magicAttributes["法术攻击"] = 10;
        Rune magicRune = new Rune("rune_magic", "法术铭文", "增加法术攻击", 2, 5, 1000, magicAttributes);
        AllRunes.Add(magicRune);

        Dictionary<string, int> defenseAttributes = new Dictionary<string, int>();
        defenseAttributes["物理防御"] = 10;
        Rune defenseRune = new Rune("rune_defense", "防御铭文", "增加物理防御", 3, 5, 1000, defenseAttributes);
        AllRunes.Add(defenseRune);

        Dictionary<string, int> hpAttributes = new Dictionary<string, int>();
        hpAttributes["最大生命"] = 100;
        Rune hpRune = new Rune("rune_hp", "生命铭文", "增加最大生命", 4, 5, 1000, hpAttributes);
        AllRunes.Add(hpRune);
    }

    private void InitializeDefaultRuneSets()
    {
        
        List<string> attackSetRunes = new List<string> { "rune_attack" };
        Dictionary<string, int> attackSetBonus = new Dictionary<string, int>();
        attackSetBonus["物理攻击"] = 5;
        RuneSet attackSet = new RuneSet("set_attack", "攻击套装", "增加物理攻击", attackSetRunes, attackSetBonus, 4);
        RuneSets.Add(attackSet);

        List<string> magicSetRunes = new List<string> { "rune_magic" };
        Dictionary<string, int> magicSetBonus = new Dictionary<string, int>();
        magicSetBonus["法术攻击"] = 5;
        RuneSet magicSet = new RuneSet("set_magic", "法术套装", "增加法术攻击", magicSetRunes, magicSetBonus, 4);
        RuneSets.Add(magicSet);
    }

    public void AddRune(Rune rune)
    {
        AllRunes.Add(rune);
    }

    public void AddRuneSet(RuneSet runeSet)
    {
        RuneSets.Add(runeSet);
    }

    public void AddPlayerInventory(string playerID, PlayerRuneInventory inventory)
    {
        PlayerInventories[playerID] = inventory;
    }
}

[Serializable]
public class RuneEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string RuneID;
    public string EventData;

    public RuneEvent(string eventID, string eventType, string playerID, string runeID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        RuneID = runeID;
        EventData = eventData;
    }
}

public class RuneSystemDataManager
{
    private static RuneSystemDataManager _instance;
    public static RuneSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RuneSystemDataManager();
            }
            return _instance;
        }
    }

    public RuneSystemData runeData;
    private List<RuneEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private RuneSystemDataManager()
    {
        runeData = new RuneSystemData();
        recentEvents = new List<RuneEvent>();
        LoadRuneData();
    }

    public void SaveRuneData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "RuneSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, runeData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存铭文系统数据失败: " + e.Message);
        }
    }

    public void LoadRuneData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "RuneSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    runeData = (RuneSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载铭文系统数据失败: " + e.Message);
            runeData = new RuneSystemData();
        }
    }

    public void CreateRuneEvent(string eventType, string playerID, string runeID, string eventData)
    {
        string eventID = "rune_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RuneEvent runeEvent = new RuneEvent(eventID, eventType, playerID, runeID, eventData);
        recentEvents.Add(runeEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<RuneEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RuneSystem : MonoBehaviour
{
    public static RuneSystem Instance { get; private set; }
    
    private RuneData runeData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadRuneData();
            
            if (runeData == null)
            {
                runeData = new RuneData();
                InitializeDefaultRunes();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeDefaultRunes()
    {
        // 红色符文（物理攻击）
        AddRune("rune_red_1", "红玛瑙", "增加物理攻击", RuneType.Red, RuneGrade.Common, 1, new Dictionary<string, float> { { "Attack", 1.6f } });
        AddRune("rune_red_2", "红晶", "增加物理攻击", RuneType.Red, RuneGrade.Uncommon, 2, new Dictionary<string, float> { { "Attack", 2.4f } });
        AddRune("rune_red_3", "红宝石", "增加物理攻击", RuneType.Red, RuneGrade.Rare, 3, new Dictionary<string, float> { { "Attack", 3.2f } });
        AddRune("rune_red_4", "红曜石", "增加物理攻击", RuneType.Red, RuneGrade.Epic, 4, new Dictionary<string, float> { { "Attack", 4.0f } });
        
        // 蓝色符文（魔法抗性和冷却缩减）
        AddRune("rune_blue_1", "蓝宝石", "增加魔法抗性", RuneType.Blue, RuneGrade.Common, 1, new Dictionary<string, float> { { "MagicResist", 1.1f } });
        AddRune("rune_blue_2", "蓝晶", "增加冷却缩减", RuneType.Blue, RuneGrade.Uncommon, 2, new Dictionary<string, float> { { "CDR", 0.6f } });
        AddRune("rune_blue_3", "蓝玛瑙", "增加魔法抗性", RuneType.Blue, RuneGrade.Rare, 3, new Dictionary<string, float> { { "MagicResist", 1.8f } });
        AddRune("rune_blue_4", "蓝曜石", "增加冷却缩减", RuneType.Blue, RuneGrade.Epic, 4, new Dictionary<string, float> { { "CDR", 1.0f } });
        
        // 绿色符文（物理防御和生命值）
        AddRune("rune_green_1", "绿玛瑙", "增加物理防御", RuneType.Green, RuneGrade.Common, 1, new Dictionary<string, float> { { "Armor", 1.1f } });
        AddRune("rune_green_2", "绿晶", "增加生命值", RuneType.Green, RuneGrade.Uncommon, 2, new Dictionary<string, float> { { "Health", 17.0f } });
        AddRune("rune_green_3", "绿宝石", "增加物理防御", RuneType.Green, RuneGrade.Rare, 3, new Dictionary<string, float> { { "Armor", 1.8f } });
        AddRune("rune_green_4", "绿曜石", "增加生命值", RuneType.Green, RuneGrade.Epic, 4, new Dictionary<string, float> { { "Health", 26.0f } });
        
        // 精华符文（高级属性）
        AddRune("rune_essence_1", "初级精华", "增加物理攻击", RuneType.Essence, RuneGrade.Common, 1, new Dictionary<string, float> { { "Attack", 2.5f } });
        AddRune("rune_essence_2", "中级精华", "增加法术强度", RuneType.Essence, RuneGrade.Uncommon, 2, new Dictionary<string, float> { { "AP", 4.0f } });
        AddRune("rune_essence_3", "高级精华", "增加生命值", RuneType.Essence, RuneGrade.Rare, 3, new Dictionary<string, float> { { "Health", 45.0f } });
        AddRune("rune_essence_4", "顶级精华", "增加攻击速度", RuneType.Essence, RuneGrade.Epic, 4, new Dictionary<string, float> { { "AttackSpeed", 1.7f } });
        
        // 创建默认符文页
        AddRunePage("page_1", "默认符文页");
        
        SaveRuneData();
    }
    
    private void AddRune(string runeID, string runeName, string description, RuneType type, RuneGrade grade, int level, Dictionary<string, float> stats)
    {
        Rune rune = new Rune
        {
            runeID = runeID,
            runeName = runeName,
            description = description,
            type = type,
            grade = grade,
            level = level,
            stats = stats,
            quantity = 0
        };
        
        runeData.runes.Add(rune);
    }
    
    private void AddRunePage(string pageID, string pageName)
    {
        RunePage page = new RunePage
        {
            pageID = pageID,
            pageName = pageName,
            runeSlots = new List<string> { "", "", "", "", "", "", "", "", "" } // 9个符文槽
        };
        
        runeData.runePages.Add(page);
    }
    
    public List<Rune> GetRunesByType(RuneType type)
    {
        return runeData.runes.FindAll(r => r.type == type);
    }
    
    public List<Rune> GetRunesByGrade(RuneGrade grade)
    {
        return runeData.runes.FindAll(r => r.grade == grade);
    }
    
    public Rune GetRune(string runeID)
    {
        return runeData.runes.Find(r => r.runeID == runeID);
    }
    
    public void AddRuneQuantity(string runeID, int quantity)
    {
        Rune rune = GetRune(runeID);
        if (rune != null)
        {
            rune.quantity += quantity;
            Debug.Log($"获得符文: {rune.runeName} x {quantity}");
            SaveRuneData();
        }
    }
    
    public int GetRuneQuantity(string runeID)
    {
        Rune rune = GetRune(runeID);
        if (rune != null)
        {
            return rune.quantity;
        }
        return 0;
    }
    
    public List<RunePage> GetAllRunePages()
    {
        return runeData.runePages;
    }
    
    public RunePage GetRunePage(string pageID)
    {
        return runeData.runePages.Find(p => p.pageID == pageID);
    }
    
    public void CreateRunePage(string pageName)
    {
        string pageID = "page_" + (runeData.runePages.Count + 1);
        AddRunePage(pageID, pageName);
        SaveRuneData();
    }
    
    public void DeleteRunePage(string pageID)
    {
        if (runeData.runePages.Count > 1) // 至少保留一个符文页
        {
            runeData.runePages.RemoveAll(p => p.pageID == pageID);
            SaveRuneData();
        }
    }
    
    public void UpdateRunePage(string pageID, string pageName)
    {
        RunePage page = GetRunePage(pageID);
        if (page != null)
        {
            page.pageName = pageName;
            SaveRuneData();
        }
    }
    
    public void SetRuneInPage(string pageID, int slotIndex, string runeID)
    {
        RunePage page = GetRunePage(pageID);
        if (page != null && slotIndex >= 0 && slotIndex < page.runeSlots.Count)
        {
            // 检查玩家是否有足够的符文
            Rune rune = GetRune(runeID);
            if (rune != null && rune.quantity > 0)
            {
                page.runeSlots[slotIndex] = runeID;
                SaveRuneData();
            }
        }
    }
    
    public Dictionary<string, float> CalculateRuneStats(string pageID)
    {
        RunePage page = GetRunePage(pageID);
        Dictionary<string, float> totalStats = new Dictionary<string, float>();
        
        if (page != null)
        {
            foreach (string runeID in page.runeSlots)
            {
                if (!string.IsNullOrEmpty(runeID))
                {
                    Rune rune = GetRune(runeID);
                    if (rune != null)
                    {
                        foreach (var stat in rune.stats)
                        {
                            if (totalStats.ContainsKey(stat.Key))
                            {
                                totalStats[stat.Key] += stat.Value;
                            }
                            else
                            {
                                totalStats[stat.Key] = stat.Value;
                            }
                        }
                    }
                }
            }
        }
        
        return totalStats;
    }
    
    public void SaveRuneData()
    {
        string path = Application.dataPath + "/Data/rune_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, runeData);
        stream.Close();
    }
    
    public void LoadRuneData()
    {
        string path = Application.dataPath + "/Data/rune_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            runeData = (RuneData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            runeData = new RuneData();
        }
    }
}

[System.Serializable]
public class RuneData
{
    public List<Rune> runes = new List<Rune>();
    public List<RunePage> runePages = new List<RunePage>();
}

[System.Serializable]
public class Rune
{
    public string runeID;
    public string runeName;
    public string description;
    public RuneType type;
    public RuneGrade grade;
    public int level;
    public Dictionary<string, float> stats;
    public int quantity;
}

[System.Serializable]
public class RunePage
{
    public string pageID;
    public string pageName;
    public List<string> runeSlots;
}

public enum RuneType
{
    Red,
    Blue,
    Green,
    Essence
}

public enum RuneGrade
{
    Common,
    Uncommon,
    Rare,
    Epic
}

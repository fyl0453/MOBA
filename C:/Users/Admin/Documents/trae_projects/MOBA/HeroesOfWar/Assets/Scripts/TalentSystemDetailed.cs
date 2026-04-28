using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Talent
{
    public string TalentID;
    public string TalentName;
    public string Description;
    public int TalentType;
    public int RequiredLevel;
    public int RequiredPoints;
    public string IconName;
    public List<string> RequiredTalents;
    public Dictionary<string, int> Attributes;
    public bool IsPassive;
    public string SkillEffect;

    public Talent(string talentID, string talentName, string description, int talentType, int requiredLevel, int requiredPoints, Dictionary<string, int> attributes, bool isPassive = false, string skillEffect = "")
    {
        TalentID = talentID;
        TalentName = talentName;
        Description = description;
        TalentType = talentType;
        RequiredLevel = requiredLevel;
        RequiredPoints = requiredPoints;
        IconName = "";
        RequiredTalents = new List<string>();
        Attributes = attributes;
        IsPassive = isPassive;
        SkillEffect = skillEffect;
    }
}

[Serializable]
public class TalentTree
{
    public string TreeID;
    public string TreeName;
    public string Description;
    public string HeroID;
    public List<Talent> Talents;
    public int MaxPoints;
    public int CurrentPoints;
    public DateTime LastUpdateTime;

    public TalentTree(string treeID, string treeName, string description, string heroID, int maxPoints)
    {
        TreeID = treeID;
        TreeName = treeName;
        Description = description;
        HeroID = heroID;
        Talents = new List<Talent>();
        MaxPoints = maxPoints;
        CurrentPoints = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class PlayerTalentData
{
    public string PlayerID;
    public Dictionary<string, TalentTree> HeroTalentTrees;
    public int TotalTalentPoints;
    public DateTime LastUpdateTime;

    public PlayerTalentData(string playerID)
    {
        PlayerID = playerID;
        HeroTalentTrees = new Dictionary<string, TalentTree>();
        TotalTalentPoints = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class TalentSystemData
{
    public List<Talent> AllTalents;
    public Dictionary<string, PlayerTalentData> PlayerTalentData;
    public DateTime LastUpdateTime;

    public TalentSystemData()
    {
        AllTalents = new List<Talent>();
        PlayerTalentData = new Dictionary<string, PlayerTalentData>();
        LastUpdateTime = DateTime.Now;
        InitializeDefaultTalents();
    }

    private void InitializeDefaultTalents()
    {
        
        Dictionary<string, int> attackAttributes = new Dictionary<string, int>();
        attackAttributes["物理攻击"] = 5;
        Talent attackTalent = new Talent("talent_attack", "强化攻击", "增加物理攻击", 1, 1, 1, attackAttributes);
        AllTalents.Add(attackTalent);

        Dictionary<string, int> magicAttributes = new Dictionary<string, int>();
        magicAttributes["法术攻击"] = 5;
        Talent magicTalent = new Talent("talent_magic", "强化法术", "增加法术攻击", 2, 1, 1, magicAttributes);
        AllTalents.Add(magicTalent);

        Dictionary<string, int> defenseAttributes = new Dictionary<string, int>();
        defenseAttributes["物理防御"] = 5;
        Talent defenseTalent = new Talent("talent_defense", "强化防御", "增加物理防御", 3, 1, 1, defenseAttributes);
        AllTalents.Add(defenseTalent);

        Dictionary<string, int> hpAttributes = new Dictionary<string, int>();
        hpAttributes["最大生命"] = 50;
        Talent hpTalent = new Talent("talent_hp", "强化生命", "增加最大生命", 4, 1, 1, hpAttributes);
        AllTalents.Add(hpTalent);
    }

    public void AddTalent(Talent talent)
    {
        AllTalents.Add(talent);
    }

    public void AddPlayerTalentData(string playerID, PlayerTalentData data)
    {
        PlayerTalentData[playerID] = data;
    }
}

[Serializable]
public class TalentEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string TalentID;
    public string EventData;

    public TalentEvent(string eventID, string eventType, string playerID, string talentID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        TalentID = talentID;
        EventData = eventData;
    }
}

public class TalentSystemDataManager
{
    private static TalentSystemDataManager _instance;
    public static TalentSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TalentSystemDataManager();
            }
            return _instance;
        }
    }

    public TalentSystemData talentData;
    private List<TalentEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private TalentSystemDataManager()
    {
        talentData = new TalentSystemData();
        recentEvents = new List<TalentEvent>();
        LoadTalentData();
    }

    public void SaveTalentData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TalentSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, talentData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存天赋系统数据失败: " + e.Message);
        }
    }

    public void LoadTalentData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TalentSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    talentData = (TalentSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载天赋系统数据失败: " + e.Message);
            talentData = new TalentSystemData();
        }
    }

    public void CreateTalentEvent(string eventType, string playerID, string talentID, string eventData)
    {
        string eventID = "talent_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TalentEvent talentEvent = new TalentEvent(eventID, eventType, playerID, talentID, eventData);
        recentEvents.Add(talentEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<TalentEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
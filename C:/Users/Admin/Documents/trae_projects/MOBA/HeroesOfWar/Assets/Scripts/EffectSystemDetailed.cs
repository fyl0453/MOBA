using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SkillEffect
{
    public string EffectID;
    public string EffectName;
    public string Description;
    public int EffectType;
    public string AssetPath;
    public string IconName;
    public int Rarity;
    public bool IsDefault;
    public bool IsLimited;
    public int Price;
    public DateTime CreateTime;

    public SkillEffect(string effectID, string effectName, string description, int effectType)
    {
        EffectID = effectID;
        EffectName = effectName;
        Description = description;
        EffectType = effectType;
        AssetPath = "";
        IconName = "";
        Rarity = 1;
        IsDefault = false;
        IsLimited = false;
        Price = 0;
        CreateTime = DateTime.Now;
    }
}

[Serializable]
public class PlayerEffect
{
    public string PlayerID;
    public string EffectID;
    public DateTime UnlockTime;
    public bool IsEquipped;
    public int UseCount;

    public PlayerEffect(string playerID, string effectID)
    {
        PlayerID = playerID;
        EffectID = effectID;
        UnlockTime = DateTime.Now;
        IsEquipped = false;
        UseCount = 0;
    }
}

[Serializable]
public class EffectSet
{
    public string SetID;
    public string SetName;
    public string Description;
    public List<string> EffectIDs;
    public int SetPrice;
    public bool IsAvailable;
    public DateTime AvailableStartTime;
    public DateTime AvailableEndTime;

    public EffectSet(string setID, string setName, string description)
    {
        SetID = setID;
        SetName = setName;
        Description = description;
        EffectIDs = new List<string>();
        SetPrice = 0;
        IsAvailable = true;
        AvailableStartTime = DateTime.Now;
        AvailableEndTime = DateTime.MaxValue;
    }
}

[Serializable]
public class EffectQuickSlot
{
    public string PlayerID;
    public List<string> QuickEffectIDs;

    public EffectQuickSlot(string playerID)
    {
        PlayerID = playerID;
        QuickEffectIDs = new List<string>();
    }
}

[Serializable]
public class EffectUsageRecord
{
    public string RecordID;
    public string PlayerID;
    public string EffectID;
    public string MatchID;
    public DateTime UseTime;

    public EffectUsageRecord(string recordID, string playerID, string effectID, string matchID)
    {
        RecordID = recordID;
        PlayerID = playerID;
        EffectID = effectID;
        MatchID = matchID;
        UseTime = DateTime.Now;
    }
}

[Serializable]
public class EffectSystemData
{
    public List<SkillEffect> AllEffects;
    public Dictionary<string, List<PlayerEffect>> PlayerEffects;
    public List<EffectSet> EffectSets;
    public Dictionary<string, EffectQuickSlot> PlayerQuickSlots;
    public List<EffectUsageRecord> UsageRecords;
    public Dictionary<string, List<string>> PlayerFavoriteEffects;
    public List<string> FeaturedEffectIDs;
    public DateTime LastCleanupTime;

    public EffectSystemData()
    {
        AllEffects = new List<SkillEffect>();
        PlayerEffects = new Dictionary<string, List<PlayerEffect>>();
        EffectSets = new List<EffectSet>();
        PlayerQuickSlots = new Dictionary<string, EffectQuickSlot>();
        UsageRecords = new List<EffectUsageRecord>();
        PlayerFavoriteEffects = new Dictionary<string, List<string>>();
        FeaturedEffectIDs = new List<string>();
        LastCleanupTime = DateTime.Now;
        InitializeDefaultEffects();
    }

    private void InitializeDefaultEffects()
    {
        SkillEffect defaultHit = new SkillEffect("effect_001", "默认打击", "默认打击特效", 1);
        defaultHit.IsDefault = true;
        defaultHit.Rarity = 1;
        AllEffects.Add(defaultHit);

        SkillEffect flameHit = new SkillEffect("effect_002", "火焰打击", "火焰系打击特效", 1);
        flameHit.Rarity = 2;
        flameHit.Price = 500;
        AllEffects.Add(flameHit);

        SkillEffect iceHit = new SkillEffect("effect_003", "冰霜打击", "冰霜系打击特效", 1);
        iceHit.Rarity = 2;
        iceHit.Price = 500;
        AllEffects.Add(iceHit);

        SkillEffect lightningHit = new SkillEffect("effect_004", "雷电打击", "雷电系打击特效", 1);
        lightningHit.Rarity = 3;
        lightningHit.Price = 800;
        AllEffects.Add(lightningHit);

        SkillEffect defaultSkill = new SkillEffect("effect_005", "默认技能", "默认技能释放特效", 2);
        defaultSkill.IsDefault = true;
        defaultSkill.Rarity = 1;
        AllEffects.Add(defaultSkill);

        SkillEffect flameSkill = new SkillEffect("effect_006", "火焰技能", "火焰系技能特效", 2);
        flameSkill.Rarity = 2;
        flameSkill.Price = 600;
        AllEffects.Add(flameSkill);

        SkillEffect defaultDeath = new SkillEffect("effect_007", "默认死亡", "默认死亡特效", 3);
        defaultDeath.IsDefault = true;
        defaultDeath.Rarity = 1;
        AllEffects.Add(defaultDeath);

        SkillEffect flameDeath = new SkillEffect("effect_008", "火焰死亡", "火焰系死亡特效", 3);
        flameDeath.Rarity = 3;
        flameDeath.Price = 1000;
        AllEffects.Add(flameDeath);
    }

    public void AddEffect(SkillEffect effect)
    {
        AllEffects.Add(effect);
    }

    public void AddPlayerEffect(string playerID, PlayerEffect playerEffect)
    {
        if (!PlayerEffects.ContainsKey(playerID))
        {
            PlayerEffects[playerID] = new List<PlayerEffect>();
        }
        PlayerEffects[playerID].Add(playerEffect);
    }

    public void AddEffectSet(EffectSet effectSet)
    {
        EffectSets.Add(effectSet);
    }
}

[Serializable]
public class EffectEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EffectID;
    public string EventData;

    public EffectEvent(string eventID, string eventType, string playerID, string effectID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EffectID = effectID;
        EventData = eventData;
    }
}

public class EffectSystemDataManager
{
    private static EffectSystemDataManager _instance;
    public static EffectSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EffectSystemDataManager();
            }
            return _instance;
        }
    }

    public EffectSystemData effectData;
    private List<EffectEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private EffectSystemDataManager()
    {
        effectData = new EffectSystemData();
        recentEvents = new List<EffectEvent>();
        LoadEffectData();
    }

    public void SaveEffectData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EffectSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, effectData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存特效系统数据失败: " + e.Message);
        }
    }

    public void LoadEffectData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EffectSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    effectData = (EffectSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载特效系统数据失败: " + e.Message);
            effectData = new EffectSystemData();
        }
    }

    public void CreateEffectEvent(string eventType, string playerID, string effectID, string eventData)
    {
        string eventID = "effect_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EffectEvent effectEvent = new EffectEvent(eventID, eventType, playerID, effectID, eventData);
        recentEvents.Add(effectEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<EffectEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Skin
{
    public string SkinID;
    public string SkinName;
    public string HeroID;
    public string Description;
    public int SkinType;
    public string ModelPath;
    public string IconPath;
    public string PreviewPath;
    public int Rarity;
    public bool IsLimited;
    public bool IsEpic;
    public bool IsLegendary;
    public int Price;
    public int PriceType;
    public string ObtainMethod;
    public DateTime? LimitedEndTime;
    public DateTime CreateTime;

    public Skin(string skinID, string skinName, string heroID, int skinType, int rarity)
    {
        SkinID = skinID;
        SkinName = skinName;
        HeroID = heroID;
        Description = "";
        SkinType = skinType;
        ModelPath = "";
        IconPath = "";
        PreviewPath = "";
        Rarity = rarity;
        IsLimited = false;
        IsEpic = false;
        IsLegendary = false;
        Price = 0;
        PriceType = 0;
        ObtainMethod = "";
        LimitedEndTime = null;
        CreateTime = DateTime.Now;
    }
}

[Serializable]
public class PlayerSkin
{
    public string PlayerID;
    public string SkinID;
    public DateTime ObtainTime;
    public bool IsEquipped;
    public string ObtainMethod;

    public PlayerSkin(string playerID, string skinID, string obtainMethod)
    {
        PlayerID = playerID;
        SkinID = skinID;
        ObtainTime = DateTime.Now;
        IsEquipped = false;
        ObtainMethod = obtainMethod;
    }
}

[Serializable]
public class SkinEffect
{
    public string EffectID;
    public string SkinID;
    public string EffectType;
    public string AssetPath;
    public string Description;
    public bool IsDefault;

    public SkinEffect(string effectID, string skinID, string effectType, string assetPath)
    {
        EffectID = effectID;
        SkinID = skinID;
        EffectType = effectType;
        AssetPath = assetPath;
        Description = "";
        IsDefault = false;
    }
}

[Serializable]
public class SkinBundle
{
    public string BundleID;
    public string BundleName;
    public string Description;
    public List<string> SkinIDs;
    public int BundlePrice;
    public int OriginalPrice;
    public bool IsAvailable;
    public DateTime AvailableStartTime;
    public DateTime AvailableEndTime;

    public SkinBundle(string bundleID, string bundleName, string description)
    {
        BundleID = bundleID;
        BundleName = bundleName;
        Description = description;
        SkinIDs = new List<string>();
        BundlePrice = 0;
        OriginalPrice = 0;
        IsAvailable = true;
        AvailableStartTime = DateTime.Now;
        AvailableEndTime = DateTime.MaxValue;
    }
}

[Serializable]
public class SkinSystemData
{
    public List<Skin> AllSkins;
    public Dictionary<string, List<PlayerSkin>> PlayerSkins;
    public List<SkinEffect> SkinEffects;
    public List<SkinBundle> SkinBundles;
    public Dictionary<string, List<string>> HeroSkins;
    public List<string> FeaturedSkinIDs;
    public DateTime LastCleanupTime;

    public SkinSystemData()
    {
        AllSkins = new List<Skin>();
        PlayerSkins = new Dictionary<string, List<PlayerSkin>>();
        SkinEffects = new List<SkinEffect>();
        SkinBundles = new List<SkinBundle>();
        HeroSkins = new Dictionary<string, List<string>>();
        FeaturedSkinIDs = new List<string>();
        LastCleanupTime = DateTime.Now;
        InitializeDefaultSkins();
    }

    private void InitializeDefaultSkins()
    {
        Skin defaultSkin = new Skin("skin_001", "默认皮肤", "hero_001", 1, 1);
        defaultSkin.IsEpic = false;
        defaultSkin.IsLegendary = false;
        defaultSkin.Price = 0;
        defaultSkin.ObtainMethod = "系统赠送";
        AllSkins.Add(defaultSkin);

        Skin epicSkin = new Skin("skin_002", "史诗皮肤", "hero_001", 2, 3);
        epicSkin.IsEpic = true;
        epicSkin.Price = 888;
        epicSkin.PriceType = 1;
        epicSkin.ObtainMethod = "商城购买";
        AllSkins.Add(epicSkin);

        Skin legendarySkin = new Skin("skin_003", "传说皮肤", "hero_001", 3, 5);
        legendarySkin.IsLegendary = true;
        legendarySkin.Price = 1688;
        legendarySkin.PriceType = 1;
        legendarySkin.ObtainMethod = "商城购买";
        AllSkins.Add(legendarySkin);

        Skin limitedSkin = new Skin("skin_004", "限定皮肤", "hero_001", 4, 4);
        limitedSkin.IsLimited = true;
        limitedSkin.Price = 1288;
        limitedSkin.PriceType = 1;
        limitedSkin.ObtainMethod = "限时活动";
        limitedSkin.LimitedEndTime = DateTime.Now.AddDays(30);
        AllSkins.Add(limitedSkin);

        AddHeroSkin("hero_001", "skin_001");
        AddHeroSkin("hero_001", "skin_002");
        AddHeroSkin("hero_001", "skin_003");
        AddHeroSkin("hero_001", "skin_004");
    }

    private void AddHeroSkin(string heroID, string skinID)
    {
        if (!HeroSkins.ContainsKey(heroID))
        {
            HeroSkins[heroID] = new List<string>();
        }
        HeroSkins[heroID].Add(skinID);
    }

    public void AddSkin(Skin skin)
    {
        AllSkins.Add(skin);
        AddHeroSkin(skin.HeroID, skin.SkinID);
    }

    public void AddPlayerSkin(string playerID, PlayerSkin playerSkin)
    {
        if (!PlayerSkins.ContainsKey(playerID))
        {
            PlayerSkins[playerID] = new List<PlayerSkin>();
        }
        PlayerSkins[playerID].Add(playerSkin);
    }

    public void AddSkinEffect(SkinEffect effect)
    {
        SkinEffects.Add(effect);
    }

    public void AddSkinBundle(SkinBundle bundle)
    {
        SkinBundles.Add(bundle);
    }
}

[Serializable]
public class SkinEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string SkinID;
    public string EventData;

    public SkinEvent(string eventID, string eventType, string playerID, string skinID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        SkinID = skinID;
        EventData = eventData;
    }
}

public class SkinSystemDataManager
{
    private static SkinSystemDataManager _instance;
    public static SkinSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkinSystemDataManager();
            }
            return _instance;
        }
    }

    public SkinSystemData skinData;
    private List<SkinEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SkinSystemDataManager()
    {
        skinData = new SkinSystemData();
        recentEvents = new List<SkinEvent>();
        LoadSkinData();
    }

    public void SaveSkinData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SkinSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, skinData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存皮肤系统数据失败: " + e.Message);
        }
    }

    public void LoadSkinData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SkinSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    skinData = (SkinSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载皮肤系统数据失败: " + e.Message);
            skinData = new SkinSystemData();
        }
    }

    public void CreateSkinEvent(string eventType, string playerID, string skinID, string eventData)
    {
        string eventID = "skin_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SkinEvent skinEvent = new SkinEvent(eventID, eventType, playerID, skinID, eventData);
        recentEvents.Add(skinEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<SkinEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
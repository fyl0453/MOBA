using System;
using System.Collections.Generic;

public class SkinSystemDetailedManager
{
    private static SkinSystemDetailedManager _instance;
    public static SkinSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkinSystemDetailedManager();
            }
            return _instance;
        }
    }

    private SkinSystemData skinData;
    private SkinSystemDataManager dataManager;

    private SkinSystemDetailedManager()
    {
        dataManager = SkinSystemDataManager.Instance;
        skinData = dataManager.skinData;
    }

    public void CreateSkin(string skinName, string heroID, int skinType, int rarity, int price, int priceType, string obtainMethod, bool isEpic = false, bool isLegendary = false, bool isLimited = false)
    {
        string skinID = "skin_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Skin skin = new Skin(skinID, skinName, heroID, skinType, rarity);
        skin.Price = price;
        skin.PriceType = priceType;
        skin.ObtainMethod = obtainMethod;
        skin.IsEpic = isEpic;
        skin.IsLegendary = isLegendary;
        skin.IsLimited = isLimited;
        if (isLimited)
        {
            skin.LimitedEndTime = DateTime.Now.AddDays(30);
        }
        skinData.AddSkin(skin);
        dataManager.CreateSkinEvent("skin_create", "", skinID, "创建皮肤: " + skinName);
        dataManager.SaveSkinData();
        Debug.Log("创建皮肤成功: " + skinName);
    }

    public void ObtainSkin(string playerID, string skinID, string obtainMethod)
    {
        Skin skin = skinData.AllSkins.Find(s => s.SkinID == skinID);
        if (skin == null)
        {
            Debug.LogError("皮肤不存在: " + skinID);
            return;
        }

        if (IsSkinObtained(playerID, skinID))
        {
            Debug.LogWarning("皮肤已拥有");
            return;
        }

        PlayerSkin playerSkin = new PlayerSkin(playerID, skinID, obtainMethod);
        skinData.AddPlayerSkin(playerID, playerSkin);
        dataManager.CreateSkinEvent("skin_obtain", playerID, skinID, "获得皮肤: " + skin.SkinName);
        dataManager.SaveSkinData();
        Debug.Log("获得皮肤成功: " + skin.SkinName);
    }

    public bool IsSkinObtained(string playerID, string skinID)
    {
        if (skinData.PlayerSkins.ContainsKey(playerID))
        {
            return skinData.PlayerSkins[playerID].Exists(s => s.SkinID == skinID);
        }
        return false;
    }

    public List<PlayerSkin> GetPlayerSkins(string playerID)
    {
        if (skinData.PlayerSkins.ContainsKey(playerID))
        {
            return skinData.PlayerSkins[playerID];
        }
        return new List<PlayerSkin>();
    }

    public List<Skin> GetPlayerSkinDetails(string playerID)
    {
        List<Skin> skins = new List<Skin>();
        if (skinData.PlayerSkins.ContainsKey(playerID))
        {
            foreach (PlayerSkin playerSkin in skinData.PlayerSkins[playerID])
            {
                Skin skin = skinData.AllSkins.Find(s => s.SkinID == playerSkin.SkinID);
                if (skin != null)
                {
                    skins.Add(skin);
                }
            }
        }
        return skins;
    }

    public void EquipSkin(string playerID, string skinID)
    {
        if (!IsSkinObtained(playerID, skinID))
        {
            Debug.LogWarning("皮肤未获得");
            return;
        }

        if (skinData.PlayerSkins.ContainsKey(playerID))
        {
            foreach (PlayerSkin playerSkin in skinData.PlayerSkins[playerID])
            {
                playerSkin.IsEquipped = false;
            }
            PlayerSkin skinToEquip = skinData.PlayerSkins[playerID].Find(s => s.SkinID == skinID);
            if (skinToEquip != null)
            {
                skinToEquip.IsEquipped = true;
            }
        }

        Skin skin = skinData.AllSkins.Find(s => s.SkinID == skinID);
        dataManager.CreateSkinEvent("skin_equip", playerID, skinID, "装备皮肤: " + (skin != null ? skin.SkinName : skinID));
        dataManager.SaveSkinData();
        Debug.Log("装备皮肤成功");
    }

    public void UnequipSkin(string playerID, string skinID)
    {
        if (skinData.PlayerSkins.ContainsKey(playerID))
        {
            PlayerSkin skin = skinData.PlayerSkins[playerID].Find(s => s.SkinID == skinID);
            if (skin != null)
            {
                skin.IsEquipped = false;
                dataManager.SaveSkinData();
                Debug.Log("卸下皮肤成功");
            }
        }
    }

    public PlayerSkin GetEquippedSkin(string playerID)
    {
        if (skinData.PlayerSkins.ContainsKey(playerID))
        {
            return skinData.PlayerSkins[playerID].Find(s => s.IsEquipped);
        }
        return null;
    }

    public Skin GetSkin(string skinID)
    {
        return skinData.AllSkins.Find(s => s.SkinID == skinID);
    }

    public List<Skin> GetAllSkins()
    {
        return skinData.AllSkins;
    }

    public List<Skin> GetHeroSkins(string heroID)
    {
        List<Skin> heroSkins = new List<Skin>();
        if (skinData.HeroSkins.ContainsKey(heroID))
        {
            foreach (string skinID in skinData.HeroSkins[heroID])
            {
                Skin skin = skinData.AllSkins.Find(s => s.SkinID == skinID);
                if (skin != null)
                {
                    heroSkins.Add(skin);
                }
            }
        }
        return heroSkins;
    }

    public List<Skin> GetLimitedSkins()
    {
        return skinData.AllSkins.FindAll(s => s.IsLimited);
    }

    public List<Skin> GetEpicSkins()
    {
        return skinData.AllSkins.FindAll(s => s.IsEpic);
    }

    public List<Skin> GetLegendarySkins()
    {
        return skinData.AllSkins.FindAll(s => s.IsLegendary);
    }

    public List<Skin> GetShopSkins()
    {
        return skinData.AllSkins.FindAll(s => s.Price > 0);
    }

    public void CreateSkinEffect(string skinID, string effectType, string assetPath)
    {
        string effectID = "effect_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SkinEffect effect = new SkinEffect(effectID, skinID, effectType, assetPath);
        skinData.AddSkinEffect(effect);
        dataManager.SaveSkinData();
        Debug.Log("创建皮肤特效成功");
    }

    public List<SkinEffect> GetSkinEffects(string skinID)
    {
        return skinData.SkinEffects.FindAll(e => e.SkinID == skinID);
    }

    public void CreateSkinBundle(string bundleName, string description, List<string> skinIDs, int bundlePrice, int originalPrice)
    {
        string bundleID = "bundle_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SkinBundle bundle = new SkinBundle(bundleID, bundleName, description);
        bundle.SkinIDs = skinIDs;
        bundle.BundlePrice = bundlePrice;
        bundle.OriginalPrice = originalPrice;
        skinData.AddSkinBundle(bundle);
        dataManager.SaveSkinData();
        Debug.Log("创建皮肤包成功: " + bundleName);
    }

    public List<SkinBundle> GetAvailableBundles()
    {
        DateTime now = DateTime.Now;
        return skinData.SkinBundles.FindAll(b => b.IsAvailable && b.AvailableStartTime <= now && b.AvailableEndTime >= now);
    }

    public SkinBundle GetSkinBundle(string bundleID)
    {
        return skinData.SkinBundles.Find(b => b.BundleID == bundleID);
    }

    public void PurchaseSkinBundle(string playerID, string bundleID)
    {
        SkinBundle bundle = GetSkinBundle(bundleID);
        if (bundle == null)
        {
            Debug.LogError("皮肤包不存在");
            return;
        }

        foreach (string skinID in bundle.SkinIDs)
        {
            ObtainSkin(playerID, skinID, "购买皮肤包");
        }

        dataManager.CreateSkinEvent("bundle_purchase", playerID, bundleID, "购买皮肤包: " + bundle.BundleName);
        dataManager.SaveSkinData();
        Debug.Log("购买皮肤包成功: " + bundle.BundleName);
    }

    public List<Skin> SearchSkins(string keyword)
    {
        return skinData.AllSkins.FindAll(s =>
            s.SkinName.Contains(keyword) ||
            s.Description.Contains(keyword));
    }

    public void UpdateSkinPrice(string skinID, int price, int priceType)
    {
        Skin skin = skinData.AllSkins.Find(s => s.SkinID == skinID);
        if (skin != null)
        {
            skin.Price = price;
            skin.PriceType = priceType;
            dataManager.SaveSkinData();
            Debug.Log("更新皮肤价格成功");
        }
    }

    public void SetSkinLimited(string skinID, bool isLimited, DateTime? limitedEndTime = null)
    {
        Skin skin = skinData.AllSkins.Find(s => s.SkinID == skinID);
        if (skin != null)
        {
            skin.IsLimited = isLimited;
            if (isLimited && limitedEndTime != null)
            {
                skin.LimitedEndTime = limitedEndTime;
            }
            dataManager.SaveSkinData();
            Debug.Log("设置皮肤限定状态成功");
        }
    }

    public void RemoveExpiredLimitedSkins()
    {
        DateTime now = DateTime.Now;
        skinData.AllSkins.RemoveAll(s => s.IsLimited && s.LimitedEndTime != null && s.LimitedEndTime < now);
        dataManager.SaveSkinData();
        Debug.Log("清理过期限定皮肤成功");
    }

    public int GetPlayerSkinCount(string playerID)
    {
        if (skinData.PlayerSkins.ContainsKey(playerID))
        {
            return skinData.PlayerSkins[playerID].Count;
        }
        return 0;
    }

    public int GetTotalSkinCount()
    {
        return skinData.AllSkins.Count;
    }

    public List<SkinEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveSkinData();
    }

    public void LoadData()
    {
        dataManager.LoadSkinData();
    }
}
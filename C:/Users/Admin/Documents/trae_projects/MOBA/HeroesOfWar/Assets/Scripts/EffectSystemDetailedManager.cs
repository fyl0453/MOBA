using System;
using System.Collections.Generic;

public class EffectSystemDetailedManager
{
    private static EffectSystemDetailedManager _instance;
    public static EffectSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EffectSystemDetailedManager();
            }
            return _instance;
        }
    }

    private EffectSystemData effectData;
    private EffectSystemDataManager dataManager;

    private EffectSystemDetailedManager()
    {
        dataManager = EffectSystemDataManager.Instance;
        effectData = dataManager.effectData;
    }

    public void CreateEffect(string effectName, string description, int effectType, int rarity, string assetPath, int price, bool isLimited = false)
    {
        string effectID = "effect_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SkillEffect effect = new SkillEffect(effectID, effectName, description, effectType);
        effect.Rarity = rarity;
        effect.AssetPath = assetPath;
        effect.Price = price;
        effect.IsLimited = isLimited;
        effectData.AddEffect(effect);
        dataManager.CreateEffectEvent("effect_create", "", effectID, "创建特效: " + effectName);
        dataManager.SaveEffectData();
        Debug.Log("创建特效成功: " + effectName);
    }

    public void UnlockEffect(string playerID, string effectID)
    {
        SkillEffect effect = effectData.AllEffects.Find(e => e.EffectID == effectID);
        if (effect == null)
        {
            Debug.LogError("特效不存在: " + effectID);
            return;
        }

        if (IsEffectUnlocked(playerID, effectID))
        {
            Debug.LogWarning("特效已解锁");
            return;
        }

        PlayerEffect playerEffect = new PlayerEffect(playerID, effectID);
        effectData.AddPlayerEffect(playerID, playerEffect);
        dataManager.CreateEffectEvent("effect_unlock", playerID, effectID, "解锁特效: " + effect.EffectName);
        dataManager.SaveEffectData();
        Debug.Log("解锁特效成功: " + effect.EffectName);
    }

    public bool IsEffectUnlocked(string playerID, string effectID)
    {
        if (effectData.PlayerEffects.ContainsKey(playerID))
        {
            return effectData.PlayerEffects[playerID].Exists(e => e.EffectID == effectID);
        }
        return false;
    }

    public List<PlayerEffect> GetPlayerEffects(string playerID)
    {
        if (effectData.PlayerEffects.ContainsKey(playerID))
        {
            return effectData.PlayerEffects[playerID];
        }
        return new List<PlayerEffect>();
    }

    public List<SkillEffect> GetUnlockedEffects(string playerID)
    {
        List<SkillEffect> unlockedEffects = new List<SkillEffect>();
        if (effectData.PlayerEffects.ContainsKey(playerID))
        {
            foreach (PlayerEffect playerEffect in effectData.PlayerEffects[playerID])
            {
                SkillEffect effect = effectData.AllEffects.Find(e => e.EffectID == playerEffect.EffectID);
                if (effect != null)
                {
                    unlockedEffects.Add(effect);
                }
            }
        }
        return unlockedEffects;
    }

    public void EquipEffect(string playerID, string effectID, int slotIndex)
    {
        if (!IsEffectUnlocked(playerID, effectID))
        {
            Debug.LogWarning("特效未解锁");
            return;
        }

        if (!effectData.PlayerQuickSlots.ContainsKey(playerID))
        {
            effectData.PlayerQuickSlots[playerID] = new EffectQuickSlot(playerID);
        }

        EffectQuickSlot quickSlot = effectData.PlayerQuickSlots[playerID];
        while (quickSlot.QuickEffectIDs.Count <= slotIndex)
        {
            quickSlot.QuickEffectIDs.Add("");
        }

        foreach (PlayerEffect pe in effectData.PlayerEffects[playerID])
        {
            pe.IsEquipped = false;
        }

        PlayerEffect effectToEquip = effectData.PlayerEffects[playerID].Find(e => e.EffectID == effectID);
        if (effectToEquip != null)
        {
            effectToEquip.IsEquipped = true;
        }

        quickSlot.QuickEffectIDs[slotIndex] = effectID;
        dataManager.CreateEffectEvent("effect_equip", playerID, effectID, "装备特效: " + effectID);
        dataManager.SaveEffectData();
        Debug.Log("装备特效成功");
    }

    public void UnequipEffect(string playerID, string effectID)
    {
        if (effectData.PlayerEffects.ContainsKey(playerID))
        {
            PlayerEffect effect = effectData.PlayerEffects[playerID].Find(e => e.EffectID == effectID);
            if (effect != null)
            {
                effect.IsEquipped = false;
                dataManager.SaveEffectData();
                Debug.Log("卸下特效成功");
            }
        }
    }

    public PlayerEffect GetEquippedEffect(string playerID)
    {
        if (effectData.PlayerEffects.ContainsKey(playerID))
        {
            return effectData.PlayerEffects[playerID].Find(e => e.IsEquipped);
        }
        return null;
    }

    public List<string> GetQuickSlot(string playerID)
    {
        if (effectData.PlayerQuickSlots.ContainsKey(playerID))
        {
            return effectData.PlayerQuickSlots[playerID].QuickEffectIDs;
        }
        return new List<string>();
    }

    public void UseEffect(string playerID, string effectID, string matchID)
    {
        if (!IsEffectUnlocked(playerID, effectID))
        {
            Debug.LogWarning("特效未解锁");
            return;
        }

        if (effectData.PlayerEffects.ContainsKey(playerID))
        {
            PlayerEffect playerEffect = effectData.PlayerEffects[playerID].Find(e => e.EffectID == effectID);
            if (playerEffect != null)
            {
                playerEffect.UseCount++;
            }
        }

        string recordID = "record_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EffectUsageRecord record = new EffectUsageRecord(recordID, playerID, effectID, matchID);
        effectData.UsageRecords.Add(record);

        dataManager.CreateEffectEvent("effect_use", playerID, effectID, "使用特效: " + effectID);
        dataManager.SaveEffectData();
        Debug.Log("使用特效: " + effectID);
    }

    public void CreateEffectSet(string setName, string description, List<string> effectIDs, int price)
    {
        string setID = "eset_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EffectSet effectSet = new EffectSet(setID, setName, description);
        effectSet.EffectIDs = effectIDs;
        effectSet.SetPrice = price;
        effectData.AddEffectSet(effectSet);
        dataManager.CreateEffectEvent("effectset_create", "", "", "创建特效包: " + setName);
        dataManager.SaveEffectData();
        Debug.Log("创建特效包成功: " + setName);
    }

    public List<EffectSet> GetAvailableEffectSets()
    {
        DateTime now = DateTime.Now;
        return effectData.EffectSets.FindAll(s => s.IsAvailable && s.AvailableStartTime <= now && s.AvailableEndTime >= now);
    }

    public EffectSet GetEffectSet(string setID)
    {
        return effectData.EffectSets.Find(s => s.SetID == setID);
    }

    public void PurchaseEffectSet(string playerID, string setID)
    {
        EffectSet effectSet = GetEffectSet(setID);
        if (effectSet == null)
        {
            Debug.LogError("特效包不存在");
            return;
        }

        foreach (string effectID in effectSet.EffectIDs)
        {
            UnlockEffect(playerID, effectID);
        }

        dataManager.CreateEffectEvent("effectset_purchase", playerID, "", "购买特效包: " + effectSet.SetName);
        dataManager.SaveEffectData();
        Debug.Log("购买特效包成功: " + effectSet.SetName);
    }

    public void FavoriteEffect(string playerID, string effectID)
    {
        if (!effectData.PlayerFavoriteEffects.ContainsKey(playerID))
        {
            effectData.PlayerFavoriteEffects[playerID] = new List<string>();
        }
        if (!effectData.PlayerFavoriteEffects[playerID].Contains(effectID))
        {
            effectData.PlayerFavoriteEffects[playerID].Add(effectID);
            dataManager.SaveEffectData();
            Debug.Log("收藏特效成功");
        }
    }

    public void UnfavoriteEffect(string playerID, string effectID)
    {
        if (effectData.PlayerFavoriteEffects.ContainsKey(playerID))
        {
            effectData.PlayerFavoriteEffects[playerID].Remove(effectID);
            dataManager.SaveEffectData();
            Debug.Log("取消收藏特效成功");
        }
    }

    public List<SkillEffect> GetFavoriteEffects(string playerID)
    {
        List<SkillEffect> favorites = new List<SkillEffect>();
        if (effectData.PlayerFavoriteEffects.ContainsKey(playerID))
        {
            foreach (string effectID in effectData.PlayerFavoriteEffects[playerID])
            {
                SkillEffect effect = effectData.AllEffects.Find(e => e.EffectID == effectID);
                if (effect != null)
                {
                    favorites.Add(effect);
                }
            }
        }
        return favorites;
    }

    public List<SkillEffect> GetEffectsByType(int effectType)
    {
        return effectData.AllEffects.FindAll(e => e.EffectType == effectType);
    }

    public List<SkillEffect> GetEffectsByRarity(int rarity)
    {
        return effectData.AllEffects.FindAll(e => e.Rarity == rarity);
    }

    public List<SkillEffect> GetDefaultEffects()
    {
        return effectData.AllEffects.FindAll(e => e.IsDefault);
    }

    public List<SkillEffect> GetLimitedEffects()
    {
        return effectData.AllEffects.FindAll(e => e.IsLimited);
    }

    public List<SkillEffect> GetShopEffects()
    {
        return effectData.AllEffects.FindAll(e => e.Price > 0 && !e.IsDefault);
    }

    public SkillEffect GetEffect(string effectID)
    {
        return effectData.AllEffects.Find(e => e.EffectID == effectID);
    }

    public List<EffectUsageRecord> GetPlayerUsageHistory(string playerID, int count = 20)
    {
        List<EffectUsageRecord> records = effectData.UsageRecords.FindAll(r => r.PlayerID == playerID);
        records.Sort((a, b) => b.UseTime.CompareTo(a.UseTime));
        if (count < records.Count)
        {
            return records.GetRange(0, count);
        }
        return records;
    }

    public int GetEffectUsageCount(string playerID, string effectID)
    {
        return effectData.UsageRecords.FindAll(r => r.PlayerID == playerID && r.EffectID == effectID).Count;
    }

    public List<EffectEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveEffectData();
    }

    public void LoadData()
    {
        dataManager.LoadEffectData();
    }
}
using System;
using System.Collections.Generic;

public class EmoteSystemDetailedManager
{
    private static EmoteSystemDetailedManager _instance;
    public static EmoteSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EmoteSystemDetailedManager();
            }
            return _instance;
        }
    }

    private EmoteSystemData emoteData;
    private EmoteSystemDataManager dataManager;

    private EmoteSystemDetailedManager()
    {
        dataManager = EmoteSystemDataManager.Instance;
        emoteData = dataManager.emoteData;
    }

    public void CreateEmote(string emoteName, string description, int emoteType, string animationName, string soundName, string iconName, string particleEffect, int rarity, int price, bool isLimited = false)
    {
        string emoteID = "emote_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Emote emote = new Emote(emoteID, emoteName, description, emoteType);
        emote.AnimationName = animationName;
        emote.SoundName = soundName;
        emote.IconName = iconName;
        emote.ParticleEffect = particleEffect;
        emote.Rarity = rarity;
        emote.Price = price;
        emote.IsLimited = isLimited;
        emoteData.AllEmotes.Add(emote);
        dataManager.CreateEmoteEvent("emote_create", "", emoteID, "创建表情: " + emoteName);
        dataManager.SaveEmoteData();
        Debug.Log("创建表情成功: " + emoteName);
    }

    public void UnlockEmote(string playerID, string emoteID)
    {
        Emote emote = emoteData.AllEmotes.Find(e => e.EmoteID == emoteID);
        if (emote == null)
        {
            Debug.LogError("表情不存在: " + emoteID);
            return;
        }

        if (IsEmoteUnlocked(playerID, emoteID))
        {
            Debug.LogWarning("表情已解锁");
            return;
        }

        PlayerEmote playerEmote = new PlayerEmote(playerID, emoteID);
        emoteData.AddPlayerEmote(playerID, playerEmote);
        dataManager.CreateEmoteEvent("emote_unlock", playerID, emoteID, "解锁表情: " + emote.EmoteName);
        dataManager.SaveEmoteData();
        Debug.Log("解锁表情成功: " + emote.EmoteName);
    }

    public bool IsEmoteUnlocked(string playerID, string emoteID)
    {
        if (emoteData.PlayerEmotes.ContainsKey(playerID))
        {
            return emoteData.PlayerEmotes[playerID].Exists(e => e.EmoteID == emoteID);
        }
        return false;
    }

    public List<PlayerEmote> GetPlayerEmotes(string playerID)
    {
        if (emoteData.PlayerEmotes.ContainsKey(playerID))
        {
            return emoteData.PlayerEmotes[playerID];
        }
        return new List<PlayerEmote>();
    }

    public List<Emote> GetUnlockedEmotes(string playerID)
    {
        List<Emote> unlockedEmotes = new List<Emote>();
        if (emoteData.PlayerEmotes.ContainsKey(playerID))
        {
            foreach (PlayerEmote playerEmote in emoteData.PlayerEmotes[playerID])
            {
                Emote emote = emoteData.AllEmotes.Find(e => e.EmoteID == playerEmote.EmoteID);
                if (emote != null)
                {
                    unlockedEmotes.Add(emote);
                }
            }
        }
        return unlockedEmotes;
    }

    public void EquipEmote(string playerID, string emoteID)
    {
        if (!IsEmoteUnlocked(playerID, emoteID))
        {
            Debug.LogWarning("表情未解锁");
            return;
        }

        if (emoteData.PlayerEmotes.ContainsKey(playerID))
        {
            foreach (PlayerEmote playerEmote in emoteData.PlayerEmotes[playerID])
            {
                playerEmote.IsEquipped = false;
            }
            PlayerEmote emote = emoteData.PlayerEmotes[playerID].Find(e => e.EmoteID == emoteID);
            if (emote != null)
            {
                emote.IsEquipped = true;
            }
        }

        Emote emoteData2 = emoteData.AllEmotes.Find(e => e.EmoteID == emoteID);
        dataManager.CreateEmoteEvent("emote_equip", playerID, emoteID, "装备表情: " + (emoteData2 != null ? emoteData2.EmoteName : emoteID));
        dataManager.SaveEmoteData();
        Debug.Log("装备表情成功");
    }

    public void UnequipEmote(string playerID, string emoteID)
    {
        if (emoteData.PlayerEmotes.ContainsKey(playerID))
        {
            PlayerEmote emote = emoteData.PlayerEmotes[playerID].Find(e => e.EmoteID == emoteID);
            if (emote != null)
            {
                emote.IsEquipped = false;
                dataManager.SaveEmoteData();
                Debug.Log("卸下表情成功");
            }
        }
    }

    public PlayerEmote GetEquippedEmote(string playerID)
    {
        if (emoteData.PlayerEmotes.ContainsKey(playerID))
        {
            return emoteData.PlayerEmotes[playerID].Find(e => e.IsEquipped);
        }
        return null;
    }

    public void UseEmote(string playerID, string emoteID)
    {
        if (!IsEmoteUnlocked(playerID, emoteID))
        {
            Debug.LogWarning("表情未解锁");
            return;
        }

        if (emoteData.PlayerEmotes.ContainsKey(playerID))
        {
            PlayerEmote playerEmote = emoteData.PlayerEmotes[playerID].Find(e => e.EmoteID == emoteID);
            if (playerEmote != null)
            {
                playerEmote.UseCount++;
            }
        }

        Emote emote = emoteData.AllEmotes.Find(e => e.EmoteID == emoteID);
        dataManager.CreateEmoteEvent("emote_use", playerID, emoteID, "使用表情: " + (emote != null ? emote.EmoteName : emoteID));
        dataManager.SaveEmoteData();
        Debug.Log("使用表情: " + (emote != null ? emote.EmoteName : emoteID));
    }

    public void SetQuickSlot(string playerID, List<string> emoteIDs)
    {
        if (!emoteData.PlayerQuickSlots.ContainsKey(playerID))
        {
            emoteData.PlayerQuickSlots[playerID] = new EmoteQuickSlot(playerID);
        }
        emoteData.PlayerQuickSlots[playerID].QuickEmoteIDs = emoteIDs;
        dataManager.SaveEmoteData();
        Debug.Log("设置快捷表情栏成功");
    }

    public List<string> GetQuickSlot(string playerID)
    {
        if (emoteData.PlayerQuickSlots.ContainsKey(playerID))
        {
            return emoteData.PlayerQuickSlots[playerID].QuickEmoteIDs;
        }
        return new List<string>();
    }

    public void AddEmoteToQuickSlot(string playerID, string emoteID)
    {
        if (!emoteData.PlayerQuickSlots.ContainsKey(playerID))
        {
            emoteData.PlayerQuickSlots[playerID] = new EmoteQuickSlot(playerID);
        }

        EmoteQuickSlot quickSlot = emoteData.PlayerQuickSlots[playerID];
        if (!quickSlot.QuickEmoteIDs.Contains(emoteID) && quickSlot.QuickEmoteIDs.Count < 8)
        {
            quickSlot.QuickEmoteIDs.Add(emoteID);
            dataManager.SaveEmoteData();
            Debug.Log("添加表情到快捷栏成功");
        }
    }

    public void RemoveEmoteFromQuickSlot(string playerID, string emoteID)
    {
        if (emoteData.PlayerQuickSlots.ContainsKey(playerID))
        {
            emoteData.PlayerQuickSlots[playerID].QuickEmoteIDs.Remove(emoteID);
            dataManager.SaveEmoteData();
            Debug.Log("从快捷栏移除表情成功");
        }
    }

    public void CreateEmoteSet(string setName, string description, List<string> emoteIDs, int price)
    {
        string setID = "set_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EmoteSet emoteSet = new EmoteSet(setID, setName, description);
        emoteSet.EmoteIDs = emoteIDs;
        emoteSet.SetPrice = price;
        emoteData.AddEmoteSet(emoteSet);
        dataManager.CreateEmoteEvent("emoteset_create", "", "", "创建表情包: " + setName);
        dataManager.SaveEmoteData();
        Debug.Log("创建表情包成功: " + setName);
    }

    public List<EmoteSet> GetAvailableEmoteSets()
    {
        DateTime now = DateTime.Now;
        return emoteData.EmoteSets.FindAll(s => s.IsAvailable && s.AvailableStartTime <= now && s.AvailableEndTime >= now);
    }

    public EmoteSet GetEmoteSet(string setID)
    {
        return emoteData.EmoteSets.Find(s => s.SetID == setID);
    }

    public void PurchaseEmoteSet(string playerID, string setID)
    {
        EmoteSet emoteSet = GetEmoteSet(setID);
        if (emoteSet == null)
        {
            Debug.LogError("表情包不存在");
            return;
        }

        foreach (string emoteID in emoteSet.EmoteIDs)
        {
            UnlockEmote(playerID, emoteID);
        }

        dataManager.CreateEmoteEvent("emoteset_purchase", playerID, "", "购买表情包: " + emoteSet.SetName);
        dataManager.SaveEmoteData();
        Debug.Log("购买表情包成功: " + emoteSet.SetName);
    }

    public void SendEmoteInteraction(string playerID, string targetPlayerID, string emoteID)
    {
        if (!IsEmoteUnlocked(playerID, emoteID))
        {
            Debug.LogWarning("表情未解锁");
            return;
        }

        string interactionID = "interaction_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EmoteInteraction interaction = new EmoteInteraction(interactionID, playerID, targetPlayerID, emoteID);
        emoteData.RecentInteractions.Add(interaction);
        dataManager.CreateEmoteEvent("emote_interaction", playerID, emoteID, "发送表情互动: " + targetPlayerID);
        dataManager.SaveEmoteData();
        Debug.Log("发送表情互动成功");
    }

    public void AcceptEmoteInteraction(string interactionID)
    {
        EmoteInteraction interaction = emoteData.RecentInteractions.Find(i => i.InteractionID == interactionID);
        if (interaction != null)
        {
            interaction.IsAccepted = true;
            dataManager.CreateEmoteEvent("emote_accept", interaction.TargetPlayerID, interaction.EmoteID, "接受表情互动");
            dataManager.SaveEmoteData();
            Debug.Log("接受表情互动成功");
        }
    }

    public List<EmoteInteraction> GetRecentInteractions(string playerID, int count = 20)
    {
        List<EmoteInteraction> interactions = emoteData.RecentInteractions.FindAll(i => i.PlayerID == playerID || i.TargetPlayerID == playerID);
        interactions.Sort((a, b) => b.InteractionTime.CompareTo(a.InteractionTime));
        if (count < interactions.Count)
        {
            return interactions.GetRange(0, count);
        }
        return interactions;
    }

    public void FavoriteEmote(string playerID, string emoteID)
    {
        if (!emoteData.PlayerFavoriteEmotes.ContainsKey(playerID))
        {
            emoteData.PlayerFavoriteEmotes[playerID] = new List<string>();
        }
        if (!emoteData.PlayerFavoriteEmotes[playerID].Contains(emoteID))
        {
            emoteData.PlayerFavoriteEmotes[playerID].Add(emoteID);
            dataManager.SaveEmoteData();
            Debug.Log("收藏表情成功");
        }
    }

    public void UnfavoriteEmote(string playerID, string emoteID)
    {
        if (emoteData.PlayerFavoriteEmotes.ContainsKey(playerID))
        {
            emoteData.PlayerFavoriteEmotes[playerID].Remove(emoteID);
            dataManager.SaveEmoteData();
            Debug.Log("取消收藏表情成功");
        }
    }

    public List<Emote> GetFavoriteEmotes(string playerID)
    {
        List<Emote> favorites = new List<Emote>();
        if (emoteData.PlayerFavoriteEmotes.ContainsKey(playerID))
        {
            foreach (string emoteID in emoteData.PlayerFavoriteEmotes[playerID])
            {
                Emote emote = emoteData.AllEmotes.Find(e => e.EmoteID == emoteID);
                if (emote != null)
                {
                    favorites.Add(emote);
                }
            }
        }
        return favorites;
    }

    public List<Emote> GetEmotesByType(int emoteType)
    {
        return emoteData.AllEmotes.FindAll(e => e.EmoteType == emoteType);
    }

    public List<Emote> GetEmotesByRarity(int rarity)
    {
        return emoteData.AllEmotes.FindAll(e => e.Rarity == rarity);
    }

    public List<Emote> GetLimitedEmotes()
    {
        return emoteData.AllEmotes.FindAll(e => e.IsLimited);
    }

    public List<Emote> GetShopEmotes()
    {
        return emoteData.AllEmotes.FindAll(e => e.Price > 0);
    }

    public Emote GetEmote(string emoteID)
    {
        return emoteData.AllEmotes.Find(e => e.EmoteID == emoteID);
    }

    public List<EmoteEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveEmoteData();
    }

    public void LoadData()
    {
        dataManager.LoadEmoteData();
    }
}
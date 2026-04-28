[System.Serializable]
public class EmoteActionSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<EmoteAction> emoteActions;
    public List<EmoteCategory> categories;
    
    public EmoteActionSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        emoteActions = new List<EmoteAction>();
        categories = new List<EmoteCategory>();
    }
    
    public void AddEmoteAction(EmoteAction emoteAction)
    {
        emoteActions.Add(emoteAction);
    }
    
    public void AddCategory(EmoteCategory category)
    {
        categories.Add(category);
    }
    
    public EmoteAction GetEmoteAction(string emoteID)
    {
        return emoteActions.Find(e => e.emoteID == emoteID);
    }
    
    public EmoteCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public List<EmoteAction> GetEmoteActionsByCategory(string categoryID)
    {
        return emoteActions.FindAll(e => e.categoryID == categoryID);
    }
    
    public List<EmoteAction> GetEmoteActionsByHero(string heroID)
    {
        return emoteActions.FindAll(e => e.heroID == heroID || e.heroID == "");
    }
}

[System.Serializable]
public class EmoteCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    public string categoryIcon;
    
    public EmoteCategory(string id, string name, string desc)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = desc;
        categoryIcon = "";
    }
}

[System.Serializable]
public class EmoteAction
{
    public string emoteID;
    public string emoteName;
    public string emoteDescription;
    public string categoryID;
    public string heroID;
    public string heroName;
    public int price;
    public string currencyType;
    public string emoteAsset;
    public string emoteIcon;
    public bool isLimited;
    public string limitedTime;
    public bool isDefault;
    
    public EmoteAction(string id, string name, string desc, string category, string heroID = "", string heroName = "", int price = 0, string currency = "Gems")
    {
        emoteID = id;
        emoteName = name;
        emoteDescription = desc;
        categoryID = category;
        this.heroID = heroID;
        this.heroName = heroName;
        this.price = price;
        currencyType = currency;
        emoteAsset = "";
        emoteIcon = "";
        isLimited = false;
        limitedTime = "";
        isDefault = false;
    }
    
    public void SetLimited(bool limited, string time = "")
    {
        isLimited = limited;
        limitedTime = time;
    }
    
    public void SetDefault(bool isDefault)
    {
        this.isDefault = isDefault;
    }
}

[System.Serializable]
public class PlayerEmoteData
{
    public string playerID;
    public List<string> ownedEmotes;
    public List<string> equippedEmotes;
    
    public PlayerEmoteData(string playerID)
    {
        this.playerID = playerID;
        ownedEmotes = new List<string>();
        equippedEmotes = new List<string>();
    }
    
    public void AddOwnedEmote(string emoteID)
    {
        if (!ownedEmotes.Contains(emoteID))
        {
            ownedEmotes.Add(emoteID);
        }
    }
    
    public void RemoveOwnedEmote(string emoteID)
    {
        ownedEmotes.Remove(emoteID);
        equippedEmotes.Remove(emoteID);
    }
    
    public void EquipEmote(string emoteID)
    {
        if (!equippedEmotes.Contains(emoteID) && equippedEmotes.Count < 8)
        {
            equippedEmotes.Add(emoteID);
        }
    }
    
    public void UnequipEmote(string emoteID)
    {
        equippedEmotes.Remove(emoteID);
    }
    
    public bool OwnsEmote(string emoteID)
    {
        return ownedEmotes.Contains(emoteID);
    }
    
    public bool IsEquipped(string emoteID)
    {
        return equippedEmotes.Contains(emoteID);
    }
    
    public List<string> GetEquippedEmotes()
    {
        return equippedEmotes;
    }
}

[System.Serializable]
public class EmoteActionManagerData
{
    public EmoteActionSystem system;
    public List<PlayerEmoteData> playerData;
    
    public EmoteActionManagerData()
    {
        system = new EmoteActionSystem("emote_action_system", "个性动作系统", "管理英雄的个性动作");
        playerData = new List<PlayerEmoteData>();
    }
    
    public void AddPlayerData(PlayerEmoteData data)
    {
        playerData.Add(data);
    }
    
    public PlayerEmoteData GetPlayerData(string playerID)
    {
        return playerData.Find(pd => pd.playerID == playerID);
    }
}
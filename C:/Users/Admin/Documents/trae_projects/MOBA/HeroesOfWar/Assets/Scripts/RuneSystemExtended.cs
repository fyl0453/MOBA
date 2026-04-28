[System.Serializable]
public class RuneSystemExtended
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Rune> runes;
    public List<RunePage> runePages;
    public List<RuneRecommendation> recommendations;
    
    public RuneSystemExtended(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        runes = new List<Rune>();
        runePages = new List<RunePage>();
        recommendations = new List<RuneRecommendation>();
    }
    
    public void AddRune(Rune rune)
    {
        runes.Add(rune);
    }
    
    public void AddRunePage(RunePage runePage)
    {
        runePages.Add(runePage);
    }
    
    public void AddRecommendation(RuneRecommendation recommendation)
    {
        recommendations.Add(recommendation);
    }
    
    public Rune GetRune(string runeID)
    {
        return runes.Find(r => r.runeID == runeID);
    }
    
    public RunePage GetRunePage(string pageID)
    {
        return runePages.Find(p => p.pageID == pageID);
    }
    
    public RuneRecommendation GetRecommendation(string recommendationID)
    {
        return recommendations.Find(r => r.recommendationID == recommendationID);
    }
    
    public List<Rune> GetRunesByType(string runeType)
    {
        return runes.FindAll(r => r.runeType == runeType);
    }
    
    public List<RuneRecommendation> GetRecommendationsByHero(string heroID)
    {
        return recommendations.FindAll(r => r.heroID == heroID);
    }
}

[System.Serializable]
public class Rune
{
    public string runeID;
    public string runeName;
    public string runeDescription;
    public string runeType;
    public string runeCategory;
    public int price;
    public int level;
    public Dictionary<string, float> attributes;
    public string runeIcon;
    
    public Rune(string id, string name, string desc, string type, string category, int price, int level)
    {
        runeID = id;
        runeName = name;
        runeDescription = desc;
        runeType = type;
        runeCategory = category;
        this.price = price;
        this.level = level;
        attributes = new Dictionary<string, float>();
        runeIcon = "";
    }
    
    public void AddAttribute(string attribute, float value)
    {
        attributes[attribute] = value;
    }
}

[System.Serializable]
public class RunePage
{
    public string pageID;
    public string pageName;
    public string pageDescription;
    public string creatorID;
    public string creatorName;
    public List<string> runeIDs;
    public string heroID;
    public bool isPublic;
    public int likes;
    public int uses;
    
    public RunePage(string id, string name, string desc, string creator, string creatorName, string heroID = "")
    {
        pageID = id;
        pageName = name;
        pageDescription = desc;
        creatorID = creator;
        this.creatorName = creatorName;
        runeIDs = new List<string>();
        this.heroID = heroID;
        isPublic = false;
        likes = 0;
        uses = 0;
    }
    
    public void AddRune(string runeID)
    {
        runeIDs.Add(runeID);
    }
    
    public void RemoveRune(string runeID)
    {
        runeIDs.Remove(runeID);
    }
    
    public void IncrementLikes()
    {
        likes++;
    }
    
    public void IncrementUses()
    {
        uses++;
    }
    
    public void SetPublic(bool isPublic)
    {
        this.isPublic = isPublic;
    }
}

[System.Serializable]
public class RuneRecommendation
{
    public string recommendationID;
    public string heroID;
    public string heroName;
    public string pageID;
    public string pageName;
    public string playStyle;
    public string description;
    public float winRate;
    public int usageCount;
    
    public RuneRecommendation(string id, string hero, string heroName, string page, string pageName, string style, string desc)
    {
        recommendationID = id;
        heroID = hero;
        this.heroName = heroName;
        pageID = page;
        this.pageName = pageName;
        playStyle = style;
        description = desc;
        winRate = 0f;
        usageCount = 0;
    }
    
    public void UpdateStats(float winRate, int usageCount)
    {
        this.winRate = winRate;
        this.usageCount = usageCount;
    }
}

[System.Serializable]
public class PlayerRuneData
{
    public string playerID;
    public List<string> ownedRunes;
    public List<string> runePages;
    public List<string> favoritePages;
    
    public PlayerRuneData(string playerID)
    {
        this.playerID = playerID;
        ownedRunes = new List<string>();
        runePages = new List<string>();
        favoritePages = new List<string>();
    }
    
    public void AddRune(string runeID)
    {
        if (!ownedRunes.Contains(runeID))
        {
            ownedRunes.Add(runeID);
        }
    }
    
    public void RemoveRune(string runeID)
    {
        ownedRunes.Remove(runeID);
    }
    
    public void AddRunePage(string pageID)
    {
        if (!runePages.Contains(pageID))
        {
            runePages.Add(pageID);
        }
    }
    
    public void RemoveRunePage(string pageID)
    {
        runePages.Remove(pageID);
        favoritePages.Remove(pageID);
    }
    
    public void AddFavoritePage(string pageID)
    {
        if (!favoritePages.Contains(pageID))
        {
            favoritePages.Add(pageID);
        }
    }
    
    public void RemoveFavoritePage(string pageID)
    {
        favoritePages.Remove(pageID);
    }
    
    public bool OwnsRune(string runeID)
    {
        return ownedRunes.Contains(runeID);
    }
    
    public bool HasRunePage(string pageID)
    {
        return runePages.Contains(pageID);
    }
    
    public bool IsFavoritePage(string pageID)
    {
        return favoritePages.Contains(pageID);
    }
}

[System.Serializable]
public class RuneSystemExtendedData
{
    public RuneSystemExtended system;
    public List<PlayerRuneData> playerData;
    
    public RuneSystemExtendedData()
    {
        system = new RuneSystemExtended("rune_system_extended", "铭文系统扩展", "管理铭文页的更多自定义选项");
        playerData = new List<PlayerRuneData>();
    }
    
    public void AddPlayerData(PlayerRuneData data)
    {
        playerData.Add(data);
    }
    
    public PlayerRuneData GetPlayerData(string playerID)
    {
        return playerData.Find(pd => pd.playerID == playerID);
    }
}
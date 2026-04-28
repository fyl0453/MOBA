[System.Serializable]
public class PersonalizationSystemExtended
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AvatarFrame> avatarFrames;
    public List<BusinessCard> businessCards;
    public List<HomePageDecoration> homePageDecorations;
    public List<PlayerPersonalization> playerPersonalizations;
    
    public PersonalizationSystemExtended(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        avatarFrames = new List<AvatarFrame>();
        businessCards = new List<BusinessCard>();
        homePageDecorations = new List<HomePageDecoration>();
        playerPersonalizations = new List<PlayerPersonalization>();
    }
    
    public void AddAvatarFrame(AvatarFrame frame)
    {
        avatarFrames.Add(frame);
    }
    
    public void AddBusinessCard(BusinessCard card)
    {
        businessCards.Add(card);
    }
    
    public void AddHomePageDecoration(HomePageDecoration decoration)
    {
        homePageDecorations.Add(decoration);
    }
    
    public void AddPlayerPersonalization(PlayerPersonalization personalization)
    {
        playerPersonalizations.Add(personalization);
    }
    
    public AvatarFrame GetAvatarFrame(string frameID)
    {
        return avatarFrames.Find(f => f.frameID == frameID);
    }
    
    public BusinessCard GetBusinessCard(string cardID)
    {
        return businessCards.Find(c => c.cardID == cardID);
    }
    
    public HomePageDecoration GetHomePageDecoration(string decorationID)
    {
        return homePageDecorations.Find(d => d.decorationID == decorationID);
    }
    
    public PlayerPersonalization GetPlayerPersonalization(string playerID)
    {
        return playerPersonalizations.Find(p => p.playerID == playerID);
    }
    
    public List<AvatarFrame> GetAvatarFramesByType(string type)
    {
        return avatarFrames.FindAll(f => f.frameType == type);
    }
    
    public List<BusinessCard> GetBusinessCardsByType(string type)
    {
        return businessCards.FindAll(c => c.cardType == type);
    }
    
    public List<HomePageDecoration> GetHomePageDecorationsByType(string type)
    {
        return homePageDecorations.FindAll(d => d.decorationType == type);
    }
}

[System.Serializable]
public class AvatarFrame
{
    public string frameID;
    public string frameName;
    public string frameDescription;
    public string frameType;
    public string frameURL;
    public bool isDefault;
    public bool isLimited;
    public string rarity;
    public int price;
    public string obtainMethod;
    public string expiryDate;
    
    public AvatarFrame(string id, string name, string desc, string type, string url, bool isDefault, bool isLimited, string rarity, int price, string obtainMethod, string expiryDate = "")
    {
        frameID = id;
        frameName = name;
        frameDescription = desc;
        frameType = type;
        frameURL = url;
        this.isDefault = isDefault;
        this.isLimited = isLimited;
        this.rarity = rarity;
        this.price = price;
        this.obtainMethod = obtainMethod;
        this.expiryDate = expiryDate;
    }
}

[System.Serializable]
public class BusinessCard
{
    public string cardID;
    public string cardName;
    public string cardDescription;
    public string cardType;
    public string cardURL;
    public bool isDefault;
    public bool isLimited;
    public string rarity;
    public int price;
    public string obtainMethod;
    public string expiryDate;
    
    public BusinessCard(string id, string name, string desc, string type, string url, bool isDefault, bool isLimited, string rarity, int price, string obtainMethod, string expiryDate = "")
    {
        cardID = id;
        cardName = name;
        cardDescription = desc;
        cardType = type;
        cardURL = url;
        this.isDefault = isDefault;
        this.isLimited = isLimited;
        this.rarity = rarity;
        this.price = price;
        this.obtainMethod = obtainMethod;
        this.expiryDate = expiryDate;
    }
}

[System.Serializable]
public class HomePageDecoration
{
    public string decorationID;
    public string decorationName;
    public string decorationDescription;
    public string decorationType;
    public string decorationURL;
    public bool isDefault;
    public bool isLimited;
    public string rarity;
    public int price;
    public string obtainMethod;
    public string expiryDate;
    
    public HomePageDecoration(string id, string name, string desc, string type, string url, bool isDefault, bool isLimited, string rarity, int price, string obtainMethod, string expiryDate = "")
    {
        decorationID = id;
        decorationName = name;
        decorationDescription = desc;
        decorationType = type;
        decorationURL = url;
        this.isDefault = isDefault;
        this.isLimited = isLimited;
        this.rarity = rarity;
        this.price = price;
        this.obtainMethod = obtainMethod;
        this.expiryDate = expiryDate;
    }
}

[System.Serializable]
public class PlayerPersonalization
{
    public string playerID;
    public string playerName;
    public string currentAvatarFrameID;
    public string currentBusinessCardID;
    public string currentHomePageDecorationID;
    public List<string> ownedAvatarFrameIDs;
    public List<string> ownedBusinessCardIDs;
    public List<string> ownedHomePageDecorationIDs;
    
    public PlayerPersonalization(string playerID, string playerName)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        currentAvatarFrameID = "";
        currentBusinessCardID = "";
        currentHomePageDecorationID = "";
        ownedAvatarFrameIDs = new List<string>();
        ownedBusinessCardIDs = new List<string>();
        ownedHomePageDecorationIDs = new List<string>();
    }
    
    public void AddAvatarFrame(string frameID)
    {
        if (!ownedAvatarFrameIDs.Contains(frameID))
        {
            ownedAvatarFrameIDs.Add(frameID);
        }
    }
    
    public void AddBusinessCard(string cardID)
    {
        if (!ownedBusinessCardIDs.Contains(cardID))
        {
            ownedBusinessCardIDs.Add(cardID);
        }
    }
    
    public void AddHomePageDecoration(string decorationID)
    {
        if (!ownedHomePageDecorationIDs.Contains(decorationID))
        {
            ownedHomePageDecorationIDs.Add(decorationID);
        }
    }
    
    public void SetAvatarFrame(string frameID)
    {
        if (ownedAvatarFrameIDs.Contains(frameID))
        {
            currentAvatarFrameID = frameID;
        }
    }
    
    public void SetBusinessCard(string cardID)
    {
        if (ownedBusinessCardIDs.Contains(cardID))
        {
            currentBusinessCardID = cardID;
        }
    }
    
    public void SetHomePageDecoration(string decorationID)
    {
        if (ownedHomePageDecorationIDs.Contains(decorationID))
        {
            currentHomePageDecorationID = decorationID;
        }
    }
}

[System.Serializable]
public class PersonalizationManagerData
{
    public PersonalizationSystemExtended system;
    
    public PersonalizationManagerData()
    {
        system = new PersonalizationSystemExtended("personalization_system_extended", "个性化系统扩展", "提供头像框、名片、个人主页装饰的更多选择");
    }
}
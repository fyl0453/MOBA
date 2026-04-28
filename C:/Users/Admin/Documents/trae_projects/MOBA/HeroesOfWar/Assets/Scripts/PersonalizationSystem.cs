[System.Serializable]
public class PersonalizationSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AvatarFrame> avatarFrames;
    public List<BusinessCard> businessCards;
    public List<ProfileDecoration> profileDecorations;
    
    public PersonalizationSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        avatarFrames = new List<AvatarFrame>();
        businessCards = new List<BusinessCard>();
        profileDecorations = new List<ProfileDecoration>();
    }
    
    public void AddAvatarFrame(AvatarFrame frame)
    {
        avatarFrames.Add(frame);
    }
    
    public void AddBusinessCard(BusinessCard card)
    {
        businessCards.Add(card);
    }
    
    public void AddProfileDecoration(ProfileDecoration decoration)
    {
        profileDecorations.Add(decoration);
    }
    
    public AvatarFrame GetAvatarFrame(string frameID)
    {
        return avatarFrames.Find(f => f.frameID == frameID);
    }
    
    public BusinessCard GetBusinessCard(string cardID)
    {
        return businessCards.Find(c => c.cardID == cardID);
    }
    
    public ProfileDecoration GetProfileDecoration(string decorationID)
    {
        return profileDecorations.Find(d => d.decorationID == decorationID);
    }
    
    public List<AvatarFrame> GetAvatarFramesByType(string type)
    {
        return avatarFrames.FindAll(f => f.frameType == type);
    }
    
    public List<BusinessCard> GetBusinessCardsByType(string type)
    {
        return businessCards.FindAll(c => c.cardType == type);
    }
    
    public List<ProfileDecoration> GetProfileDecorationsByType(string type)
    {
        return profileDecorations.FindAll(d => d.decorationType == type);
    }
}

[System.Serializable]
public class AvatarFrame
{
    public string frameID;
    public string frameName;
    public string frameDescription;
    public string frameType;
    public int price;
    public string currencyType;
    public string frameIcon;
    public bool isLimited;
    public string limitedTime;
    public bool isDefault;
    
    public AvatarFrame(string id, string name, string desc, string type, int price, string currency = "Gems")
    {
        frameID = id;
        frameName = name;
        frameDescription = desc;
        frameType = type;
        this.price = price;
        currencyType = currency;
        frameIcon = "";
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
public class BusinessCard
{
    public string cardID;
    public string cardName;
    public string cardDescription;
    public string cardType;
    public int price;
    public string currencyType;
    public string cardBackground;
    public bool isLimited;
    public string limitedTime;
    public bool isDefault;
    
    public BusinessCard(string id, string name, string desc, string type, int price, string currency = "Gems")
    {
        cardID = id;
        cardName = name;
        cardDescription = desc;
        cardType = type;
        this.price = price;
        currencyType = currency;
        cardBackground = "";
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
public class ProfileDecoration
{
    public string decorationID;
    public string decorationName;
    public string decorationDescription;
    public string decorationType;
    public int price;
    public string currencyType;
    public string decorationAsset;
    public bool isLimited;
    public string limitedTime;
    
    public ProfileDecoration(string id, string name, string desc, string type, int price, string currency = "Gems")
    {
        decorationID = id;
        decorationName = name;
        decorationDescription = desc;
        decorationType = type;
        this.price = price;
        currencyType = currency;
        decorationAsset = "";
        isLimited = false;
        limitedTime = "";
    }
    
    public void SetLimited(bool limited, string time = "")
    {
        isLimited = limited;
        limitedTime = time;
    }
}

[System.Serializable]
public class PlayerPersonalizationData
{
    public string playerID;
    public string equippedAvatarFrame;
    public string equippedBusinessCard;
    public List<string> ownedAvatarFrames;
    public List<string> ownedBusinessCards;
    public List<string> ownedDecorations;
    public Dictionary<string, string> decorationPositions;
    
    public PlayerPersonalizationData(string playerID)
    {
        this.playerID = playerID;
        equippedAvatarFrame = "";
        equippedBusinessCard = "";
        ownedAvatarFrames = new List<string>();
        ownedBusinessCards = new List<string>();
        ownedDecorations = new List<string>();
        decorationPositions = new Dictionary<string, string>();
    }
    
    public void EquipAvatarFrame(string frameID)
    {
        equippedAvatarFrame = frameID;
    }
    
    public void EquipBusinessCard(string cardID)
    {
        equippedBusinessCard = cardID;
    }
    
    public void AddAvatarFrame(string frameID)
    {
        if (!ownedAvatarFrames.Contains(frameID))
        {
            ownedAvatarFrames.Add(frameID);
        }
    }
    
    public void AddBusinessCard(string cardID)
    {
        if (!ownedBusinessCards.Contains(cardID))
        {
            ownedBusinessCards.Add(cardID);
        }
    }
    
    public void AddDecoration(string decorationID)
    {
        if (!ownedDecorations.Contains(decorationID))
        {
            ownedDecorations.Add(decorationID);
        }
    }
    
    public void PlaceDecoration(string position, string decorationID)
    {
        decorationPositions[position] = decorationID;
    }
    
    public void RemoveDecoration(string position)
    {
        if (decorationPositions.ContainsKey(position))
        {
            decorationPositions.Remove(position);
        }
    }
    
    public bool OwnsAvatarFrame(string frameID)
    {
        return ownedAvatarFrames.Contains(frameID);
    }
    
    public bool OwnsBusinessCard(string cardID)
    {
        return ownedBusinessCards.Contains(cardID);
    }
    
    public bool OwnsDecoration(string decorationID)
    {
        return ownedDecorations.Contains(decorationID);
    }
    
    public string GetDecorationAtPosition(string position)
    {
        return decorationPositions.ContainsKey(position) ? decorationPositions[position] : "";
    }
}

[System.Serializable]
public class PersonalizationManagerData
{
    public PersonalizationSystem system;
    public List<PlayerPersonalizationData> playerData;
    
    public PersonalizationManagerData()
    {
        system = new PersonalizationSystem("personalization", "个性化系统", "管理玩家个性化设置");
        playerData = new List<PlayerPersonalizationData>();
    }
    
    public void AddPlayerData(PlayerPersonalizationData data)
    {
        playerData.Add(data);
    }
    
    public PlayerPersonalizationData GetPlayerData(string playerID)
    {
        return playerData.Find(pd => pd.playerID == playerID);
    }
}
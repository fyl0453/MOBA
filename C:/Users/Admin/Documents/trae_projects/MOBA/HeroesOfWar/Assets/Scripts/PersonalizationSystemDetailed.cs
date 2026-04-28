[System.Serializable]
public class PersonalizationSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AvatarFrame> avatarFrames;
    public List<BusinessCard> businessCards;
    public List<ProfileDecoration> profileDecorations;
    public List<PlayerPersonalization> playerPersonalizations;
    public List<Theme> themes;
    public List<Border> borders;
    public List<Title> titles;
    
    public PersonalizationSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        avatarFrames = new List<AvatarFrame>();
        businessCards = new List<BusinessCard>();
        profileDecorations = new List<ProfileDecoration>();
        playerPersonalizations = new List<PlayerPersonalization>();
        themes = new List<Theme>();
        borders = new List<Border>();
        titles = new List<Title>();
    }
    
    public void AddAvatarFrame(AvatarFrame avatarFrame)
    {
        avatarFrames.Add(avatarFrame);
    }
    
    public void AddBusinessCard(BusinessCard businessCard)
    {
        businessCards.Add(businessCard);
    }
    
    public void AddProfileDecoration(ProfileDecoration decoration)
    {
        profileDecorations.Add(decoration);
    }
    
    public void AddPlayerPersonalization(PlayerPersonalization personalization)
    {
        playerPersonalizations.Add(personalization);
    }
    
    public void AddTheme(Theme theme)
    {
        themes.Add(theme);
    }
    
    public void AddBorder(Border border)
    {
        borders.Add(border);
    }
    
    public void AddTitle(Title title)
    {
        titles.Add(title);
    }
    
    public AvatarFrame GetAvatarFrame(string frameID)
    {
        return avatarFrames.Find(af => af.frameID == frameID);
    }
    
    public BusinessCard GetBusinessCard(string cardID)
    {
        return businessCards.Find(bc => bc.cardID == cardID);
    }
    
    public ProfileDecoration GetProfileDecoration(string decorationID)
    {
        return profileDecorations.Find(pd => pd.decorationID == decorationID);
    }
    
    public PlayerPersonalization GetPlayerPersonalization(string playerID)
    {
        return playerPersonalizations.Find(pp => pp.playerID == playerID);
    }
    
    public Theme GetTheme(string themeID)
    {
        return themes.Find(t => t.themeID == themeID);
    }
    
    public Border GetBorder(string borderID)
    {
        return borders.Find(b => b.borderID == borderID);
    }
    
    public Title GetTitle(string titleID)
    {
        return titles.Find(t => t.titleID == titleID);
    }
    
    public List<AvatarFrame> GetAvatarFramesByType(string type)
    {
        return avatarFrames.FindAll(af => af.frameType == type);
    }
    
    public List<BusinessCard> GetBusinessCardsByType(string type)
    {
        return businessCards.FindAll(bc => bc.cardType == type);
    }
    
    public List<ProfileDecoration> GetProfileDecorationsByType(string type)
    {
        return profileDecorations.FindAll(pd => pd.decorationType == type);
    }
    
    public List<Theme> GetThemesByType(string type)
    {
        return themes.FindAll(t => t.themeType == type);
    }
    
    public List<Border> GetBordersByType(string type)
    {
        return borders.FindAll(b => b.borderType == type);
    }
    
    public List<Title> GetTitlesByType(string type)
    {
        return titles.FindAll(t => t.titleType == type);
    }
}

[System.Serializable]
public class AvatarFrame
{
    public string frameID;
    public string frameName;
    public string frameType;
    public string description;
    public string priceCurrency;
    public float price;
    public string iconPath;
    public string framePath;
    public bool isLimited;
    public string releaseDate;
    public string expiryDate;
    public bool isEnabled;
    
    public AvatarFrame(string id, string name, string type, string desc, string priceCurrency, float price, string iconPath, string framePath, bool limited, string releaseDate, string expiryDate)
    {
        frameID = id;
        frameName = name;
        frameType = type;
        description = desc;
        this.priceCurrency = priceCurrency;
        this.price = price;
        this.iconPath = iconPath;
        this.framePath = framePath;
        isLimited = limited;
        this.releaseDate = releaseDate;
        this.expiryDate = expiryDate;
        isEnabled = true;
    }
    
    public bool IsAvailable()
    {
        if (!isEnabled)
            return false;
        
        if (isLimited)
        {
            System.DateTime now = System.DateTime.Now;
            System.DateTime release = System.DateTime.Parse(releaseDate);
            System.DateTime expiry = System.DateTime.Parse(expiryDate);
            return now >= release && now <= expiry;
        }
        
        return true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class BusinessCard
{
    public string cardID;
    public string cardName;
    public string cardType;
    public string description;
    public string priceCurrency;
    public float price;
    public string iconPath;
    public string cardPath;
    public bool isLimited;
    public string releaseDate;
    public string expiryDate;
    public bool isEnabled;
    
    public BusinessCard(string id, string name, string type, string desc, string priceCurrency, float price, string iconPath, string cardPath, bool limited, string releaseDate, string expiryDate)
    {
        cardID = id;
        cardName = name;
        cardType = type;
        description = desc;
        this.priceCurrency = priceCurrency;
        this.price = price;
        this.iconPath = iconPath;
        this.cardPath = cardPath;
        isLimited = limited;
        this.releaseDate = releaseDate;
        this.expiryDate = expiryDate;
        isEnabled = true;
    }
    
    public bool IsAvailable()
    {
        if (!isEnabled)
            return false;
        
        if (isLimited)
        {
            System.DateTime now = System.DateTime.Now;
            System.DateTime release = System.DateTime.Parse(releaseDate);
            System.DateTime expiry = System.DateTime.Parse(expiryDate);
            return now >= release && now <= expiry;
        }
        
        return true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class ProfileDecoration
{
    public string decorationID;
    public string decorationName;
    public string decorationType;
    public string description;
    public string priceCurrency;
    public float price;
    public string iconPath;
    public string decorationPath;
    public bool isLimited;
    public string releaseDate;
    public string expiryDate;
    public bool isEnabled;
    
    public ProfileDecoration(string id, string name, string type, string desc, string priceCurrency, float price, string iconPath, string decorationPath, bool limited, string releaseDate, string expiryDate)
    {
        decorationID = id;
        decorationName = name;
        decorationType = type;
        description = desc;
        this.priceCurrency = priceCurrency;
        this.price = price;
        this.iconPath = iconPath;
        this.decorationPath = decorationPath;
        isLimited = limited;
        this.releaseDate = releaseDate;
        this.expiryDate = expiryDate;
        isEnabled = true;
    }
    
    public bool IsAvailable()
    {
        if (!isEnabled)
            return false;
        
        if (isLimited)
        {
            System.DateTime now = System.DateTime.Now;
            System.DateTime release = System.DateTime.Parse(releaseDate);
            System.DateTime expiry = System.DateTime.Parse(expiryDate);
            return now >= release && now <= expiry;
        }
        
        return true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class PlayerPersonalization
{
    public string personalizationID;
    public string playerID;
    public string currentAvatarFrameID;
    public string currentBusinessCardID;
    public string currentThemeID;
    public string currentBorderID;
    public string currentTitleID;
    public List<string> ownedAvatarFrameIDs;
    public List<string> ownedBusinessCardIDs;
    public List<string> ownedProfileDecorationIDs;
    public List<string> ownedThemeIDs;
    public List<string> ownedBorderIDs;
    public List<string> ownedTitleIDs;
    
    public PlayerPersonalization(string id, string playerID)
    {
        personalizationID = id;
        this.playerID = playerID;
        currentAvatarFrameID = "";
        currentBusinessCardID = "";
        currentThemeID = "";
        currentBorderID = "";
        currentTitleID = "";
        ownedAvatarFrameIDs = new List<string>();
        ownedBusinessCardIDs = new List<string>();
        ownedProfileDecorationIDs = new List<string>();
        ownedThemeIDs = new List<string>();
        ownedBorderIDs = new List<string>();
        ownedTitleIDs = new List<string>();
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
    
    public void AddProfileDecoration(string decorationID)
    {
        if (!ownedProfileDecorationIDs.Contains(decorationID))
        {
            ownedProfileDecorationIDs.Add(decorationID);
        }
    }
    
    public void AddTheme(string themeID)
    {
        if (!ownedThemeIDs.Contains(themeID))
        {
            ownedThemeIDs.Add(themeID);
        }
    }
    
    public void AddBorder(string borderID)
    {
        if (!ownedBorderIDs.Contains(borderID))
        {
            ownedBorderIDs.Add(borderID);
        }
    }
    
    public void AddTitle(string titleID)
    {
        if (!ownedTitleIDs.Contains(titleID))
        {
            ownedTitleIDs.Add(titleID);
        }
    }
    
    public void SetCurrentAvatarFrame(string frameID)
    {
        if (ownedAvatarFrameIDs.Contains(frameID))
        {
            currentAvatarFrameID = frameID;
        }
    }
    
    public void SetCurrentBusinessCard(string cardID)
    {
        if (ownedBusinessCardIDs.Contains(cardID))
        {
            currentBusinessCardID = cardID;
        }
    }
    
    public void SetCurrentTheme(string themeID)
    {
        if (ownedThemeIDs.Contains(themeID))
        {
            currentThemeID = themeID;
        }
    }
    
    public void SetCurrentBorder(string borderID)
    {
        if (ownedBorderIDs.Contains(borderID))
        {
            currentBorderID = borderID;
        }
    }
    
    public void SetCurrentTitle(string titleID)
    {
        if (ownedTitleIDs.Contains(titleID))
        {
            currentTitleID = titleID;
        }
    }
}

[System.Serializable]
public class Theme
{
    public string themeID;
    public string themeName;
    public string themeType;
    public string description;
    public string priceCurrency;
    public float price;
    public string iconPath;
    public string themePath;
    public bool isLimited;
    public string releaseDate;
    public string expiryDate;
    public bool isEnabled;
    
    public Theme(string id, string name, string type, string desc, string priceCurrency, float price, string iconPath, string themePath, bool limited, string releaseDate, string expiryDate)
    {
        themeID = id;
        themeName = name;
        themeType = type;
        description = desc;
        this.priceCurrency = priceCurrency;
        this.price = price;
        this.iconPath = iconPath;
        this.themePath = themePath;
        isLimited = limited;
        this.releaseDate = releaseDate;
        this.expiryDate = expiryDate;
        isEnabled = true;
    }
    
    public bool IsAvailable()
    {
        if (!isEnabled)
            return false;
        
        if (isLimited)
        {
            System.DateTime now = System.DateTime.Now;
            System.DateTime release = System.DateTime.Parse(releaseDate);
            System.DateTime expiry = System.DateTime.Parse(expiryDate);
            return now >= release && now <= expiry;
        }
        
        return true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class Border
{
    public string borderID;
    public string borderName;
    public string borderType;
    public string description;
    public string priceCurrency;
    public float price;
    public string iconPath;
    public string borderPath;
    public bool isLimited;
    public string releaseDate;
    public string expiryDate;
    public bool isEnabled;
    
    public Border(string id, string name, string type, string desc, string priceCurrency, float price, string iconPath, string borderPath, bool limited, string releaseDate, string expiryDate)
    {
        borderID = id;
        borderName = name;
        borderType = type;
        description = desc;
        this.priceCurrency = priceCurrency;
        this.price = price;
        this.iconPath = iconPath;
        this.borderPath = borderPath;
        isLimited = limited;
        this.releaseDate = releaseDate;
        this.expiryDate = expiryDate;
        isEnabled = true;
    }
    
    public bool IsAvailable()
    {
        if (!isEnabled)
            return false;
        
        if (isLimited)
        {
            System.DateTime now = System.DateTime.Now;
            System.DateTime release = System.DateTime.Parse(releaseDate);
            System.DateTime expiry = System.DateTime.Parse(expiryDate);
            return now >= release && now <= expiry;
        }
        
        return true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class Title
{
    public string titleID;
    public string titleName;
    public string titleType;
    public string description;
    public string priceCurrency;
    public float price;
    public string iconPath;
    public bool isLimited;
    public string releaseDate;
    public string expiryDate;
    public bool isEnabled;
    
    public Title(string id, string name, string type, string desc, string priceCurrency, float price, string iconPath, bool limited, string releaseDate, string expiryDate)
    {
        titleID = id;
        titleName = name;
        titleType = type;
        description = desc;
        this.priceCurrency = priceCurrency;
        this.price = price;
        this.iconPath = iconPath;
        isLimited = limited;
        this.releaseDate = releaseDate;
        this.expiryDate = expiryDate;
        isEnabled = true;
    }
    
    public bool IsAvailable()
    {
        if (!isEnabled)
            return false;
        
        if (isLimited)
        {
            System.DateTime now = System.DateTime.Now;
            System.DateTime release = System.DateTime.Parse(releaseDate);
            System.DateTime expiry = System.DateTime.Parse(expiryDate);
            return now >= release && now <= expiry;
        }
        
        return true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class PersonalizationSystemDetailedManagerData
{
    public PersonalizationSystemDetailed system;
    
    public PersonalizationSystemDetailedManagerData()
    {
        system = new PersonalizationSystemDetailed("personalization_system_detailed", "个性化系统详细", "管理个性化的详细功能，包括更多头像框、名片和个人主页装饰");
    }
}
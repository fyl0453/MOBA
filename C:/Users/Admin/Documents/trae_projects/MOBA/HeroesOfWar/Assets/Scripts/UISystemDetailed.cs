[System.Serializable]
public class UISystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<UIScreen> uiScreens;
    public List<UIComponent> uiComponents;
    public List<UITheme> uiThemes;
    public List<UIStyle> uiStyles;
    public List<PlayerUISettings> playerUISettings;
    
    public UISystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        uiScreens = new List<UIScreen>();
        uiComponents = new List<UIComponent>();
        uiThemes = new List<UITheme>();
        uiStyles = new List<UIStyle>();
        playerUISettings = new List<PlayerUISettings>();
    }
    
    public void AddUIScreen(UIScreen screen)
    {
        uiScreens.Add(screen);
    }
    
    public void AddUIComponent(UIComponent component)
    {
        uiComponents.Add(component);
    }
    
    public void AddUITheme(UITheme theme)
    {
        uiThemes.Add(theme);
    }
    
    public void AddUIStyle(UIStyle style)
    {
        uiStyles.Add(style);
    }
    
    public void AddPlayerUISettings(PlayerUISettings settings)
    {
        playerUISettings.Add(settings);
    }
    
    public UIScreen GetUIScreen(string screenID)
    {
        return uiScreens.Find(s => s.screenID == screenID);
    }
    
    public UIComponent GetUIComponent(string componentID)
    {
        return uiComponents.Find(c => c.componentID == componentID);
    }
    
    public UITheme GetUITheme(string themeID)
    {
        return uiThemes.Find(t => t.themeID == themeID);
    }
    
    public UIStyle GetUIStyle(string styleID)
    {
        return uiStyles.Find(s => s.styleID == styleID);
    }
    
    public PlayerUISettings GetPlayerUISettings(string playerID)
    {
        return playerUISettings.Find(p => p.playerID == playerID);
    }
    
    public List<UIScreen> GetUIScreensByType(string type)
    {
        return uiScreens.FindAll(s => s.screenType == type);
    }
    
    public List<UIComponent> GetUIComponentsByScreen(string screenID)
    {
        return uiComponents.FindAll(c => c.screenID == screenID);
    }
    
    public List<UIComponent> GetUIComponentsByType(string type)
    {
        return uiComponents.FindAll(c => c.componentType == type);
    }
    
    public List<UITheme> GetUIThemesByType(string type)
    {
        return uiThemes.FindAll(t => t.themeType == type);
    }
}

[System.Serializable]
public class UIScreen
{
    public string screenID;
    public string screenName;
    public string screenType;
    public string description;
    public string prefabPath;
    public string scenePath;
    public bool isEnabled;
    public bool isModal;
    public bool isPersistent;
    public List<string> componentIDs;
    
    public UIScreen(string id, string name, string type, string desc, string prefabPath, string scenePath, bool isModal = false, bool isPersistent = false)
    {
        screenID = id;
        screenName = name;
        screenType = type;
        description = desc;
        this.prefabPath = prefabPath;
        this.scenePath = scenePath;
        isEnabled = true;
        this.isModal = isModal;
        this.isPersistent = isPersistent;
        componentIDs = new List<string>();
    }
    
    public void AddComponent(string componentID)
    {
        componentIDs.Add(componentID);
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
public class UIComponent
{
    public string componentID;
    public string screenID;
    public string componentName;
    public string componentType;
    public string description;
    public string prefabPath;
    public string parentID;
    public string position;
    public string size;
    public bool isEnabled;
    public bool isVisible;
    public int zIndex;
    
    public UIComponent(string id, string screenID, string name, string type, string desc, string prefabPath, string parentID = "", string position = "(0,0)", string size = "(100,100)", int zIndex = 0)
    {
        componentID = id;
        this.screenID = screenID;
        componentName = name;
        componentType = type;
        description = desc;
        this.prefabPath = prefabPath;
        this.parentID = parentID;
        this.position = position;
        this.size = size;
        isEnabled = true;
        isVisible = true;
        this.zIndex = zIndex;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public void Show()
    {
        isVisible = true;
    }
    
    public void Hide()
    {
        isVisible = false;
    }
    
    public void SetPosition(string position)
    {
        this.position = position;
    }
    
    public void SetSize(string size)
    {
        this.size = size;
    }
    
    public void SetZIndex(int zIndex)
    {
        this.zIndex = zIndex;
    }
}

[System.Serializable]
public class UITheme
{
    public string themeID;
    public string themeName;
    public string themeType;
    public string description;
    public string primaryColor;
    public string secondaryColor;
    public string accentColor;
    public string backgroundColor;
    public string textColor;
    public string fontPath;
    public string iconPath;
    public bool isEnabled;
    
    public UITheme(string id, string name, string type, string desc, string primaryColor, string secondaryColor, string accentColor, string backgroundColor, string textColor, string fontPath, string iconPath)
    {
        themeID = id;
        themeName = name;
        themeType = type;
        description = desc;
        this.primaryColor = primaryColor;
        this.secondaryColor = secondaryColor;
        this.accentColor = accentColor;
        this.backgroundColor = backgroundColor;
        this.textColor = textColor;
        this.fontPath = fontPath;
        this.iconPath = iconPath;
        isEnabled = true;
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
public class UIStyle
{
    public string styleID;
    public string styleName;
    public string styleType;
    public string description;
    public string fontSize;
    public string fontWeight;
    public string padding;
    public string margin;
    public string borderRadius;
    public string borderWidth;
    public string borderColor;
    public bool isEnabled;
    
    public UIStyle(string id, string name, string type, string desc, string fontSize, string fontWeight, string padding, string margin, string borderRadius, string borderWidth, string borderColor)
    {
        styleID = id;
        styleName = name;
        styleType = type;
        description = desc;
        this.fontSize = fontSize;
        this.fontWeight = fontWeight;
        this.padding = padding;
        this.margin = margin;
        this.borderRadius = borderRadius;
        this.borderWidth = borderWidth;
        this.borderColor = borderColor;
        isEnabled = true;
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
public class PlayerUISettings
{
    public string settingsID;
    public string playerID;
    public string currentThemeID;
    public string currentStyleID;
    public bool isFullscreen;
    public bool isHUDVisible;
    public bool isMinimapVisible;
    public bool isChatVisible;
    public bool isNotificationsVisible;
    public float uiScale;
    public float soundVolume;
    public float musicVolume;
    public string language;
    public List<string> enabledScreens;
    
    public PlayerUISettings(string id, string playerID)
    {
        settingsID = id;
        this.playerID = playerID;
        currentThemeID = "";
        currentStyleID = "";
        isFullscreen = true;
        isHUDVisible = true;
        isMinimapVisible = true;
        isChatVisible = true;
        isNotificationsVisible = true;
        uiScale = 1.0f;
        soundVolume = 1.0f;
        musicVolume = 1.0f;
        language = "zh-CN";
        enabledScreens = new List<string>();
    }
    
    public void SetTheme(string themeID)
    {
        currentThemeID = themeID;
    }
    
    public void SetStyle(string styleID)
    {
        currentStyleID = styleID;
    }
    
    public void SetFullscreen(bool fullscreen)
    {
        isFullscreen = fullscreen;
    }
    
    public void SetHUDVisible(bool visible)
    {
        isHUDVisible = visible;
    }
    
    public void SetMinimapVisible(bool visible)
    {
        isMinimapVisible = visible;
    }
    
    public void SetChatVisible(bool visible)
    {
        isChatVisible = visible;
    }
    
    public void SetNotificationsVisible(bool visible)
    {
        isNotificationsVisible = visible;
    }
    
    public void SetUIScale(float scale)
    {
        uiScale = scale;
    }
    
    public void SetSoundVolume(float volume)
    {
        soundVolume = volume;
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
    }
    
    public void SetLanguage(string language)
    {
        this.language = language;
    }
    
    public void AddEnabledScreen(string screenID)
    {
        if (!enabledScreens.Contains(screenID))
        {
            enabledScreens.Add(screenID);
        }
    }
    
    public void RemoveEnabledScreen(string screenID)
    {
        enabledScreens.Remove(screenID);
    }
}

[System.Serializable]
public class UISystemDetailedManagerData
{
    public UISystemDetailed system;
    
    public UISystemDetailedManagerData()
    {
        system = new UISystemDetailed("ui_system_detailed", "UI系统详细", "管理UI的详细功能，包括各个系统的完整UI界面");
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class UISystemDetailedManager : MonoBehaviour
{
    public static UISystemDetailedManager Instance { get; private set; }
    
    public UISystemDetailedManagerData uiData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadUIData();
        
        if (uiData == null)
        {
            uiData = new UISystemDetailedManagerData();
            InitializeDefaultUISystem();
        }
    }
    
    private void InitializeDefaultUISystem()
    {
        // UI屏幕
        UIScreen screen1 = new UIScreen("screen_001", "主菜单", "main", "游戏主菜单界面", "ui/screens/main_menu", "scenes/MainMenu", false, true);
        UIScreen screen2 = new UIScreen("screen_002", "游戏界面", "game", "游戏主界面", "ui/screens/game_ui", "scenes/Game", false, true);
        UIScreen screen3 = new UIScreen("screen_003", "英雄选择", "hero_select", "英雄选择界面", "ui/screens/hero_select", "scenes/HeroSelect", true, false);
        UIScreen screen4 = new UIScreen("screen_004", "背包", "inventory", "背包界面", "ui/screens/inventory", "scenes/Inventory", true, false);
        UIScreen screen5 = new UIScreen("screen_005", "商店", "shop", "商店界面", "ui/screens/shop", "scenes/Shop", true, false);
        UIScreen screen6 = new UIScreen("screen_006", "个人主页", "profile", "个人主页界面", "ui/screens/profile", "scenes/Profile", true, false);
        UIScreen screen7 = new UIScreen("screen_007", "设置", "settings", "设置界面", "ui/screens/settings", "scenes/Settings", true, false);
        UIScreen screen8 = new UIScreen("screen_008", "排行榜", "rankings", "排行榜界面", "ui/screens/rankings", "scenes/Rankings", true, false);
        UIScreen screen9 = new UIScreen("screen_009", "社区", "community", "社区界面", "ui/screens/community", "scenes/Community", true, false);
        UIScreen screen10 = new UIScreen("screen_010", "赛事", "tournament", "赛事界面", "ui/screens/tournament", "scenes/Tournament", true, false);
        
        uiData.system.AddUIScreen(screen1);
        uiData.system.AddUIScreen(screen2);
        uiData.system.AddUIScreen(screen3);
        uiData.system.AddUIScreen(screen4);
        uiData.system.AddUIScreen(screen5);
        uiData.system.AddUIScreen(screen6);
        uiData.system.AddUIScreen(screen7);
        uiData.system.AddUIScreen(screen8);
        uiData.system.AddUIScreen(screen9);
        uiData.system.AddUIScreen(screen10);
        
        // UI组件
        UIComponent component1 = new UIComponent("component_001", "screen_001", "开始游戏按钮", "button", "开始游戏按钮", "ui/components/button", "", "(400, 300)", "(200, 50)", 1);
        UIComponent component2 = new UIComponent("component_002", "screen_001", "背包按钮", "button", "背包按钮", "ui/components/button", "", "(400, 200)", "(200, 50)", 1);
        UIComponent component3 = new UIComponent("component_003", "screen_001", "商店按钮", "button", "商店按钮", "ui/components/button", "", "(400, 100)", "(200, 50)", 1);
        UIComponent component4 = new UIComponent("component_004", "screen_001", "设置按钮", "button", "设置按钮", "ui/components/button", "", "(100, 100)", "(100, 50)", 1);
        UIComponent component5 = new UIComponent("component_005", "screen_002", "小地图", "minimap", "游戏小地图", "ui/components/minimap", "", "(100, 100)", "(200, 200)", 2);
        UIComponent component6 = new UIComponent("component_006", "screen_002", "技能栏", "skill_bar", "技能栏", "ui/components/skill_bar", "", "(400, 50)", "(400, 100)", 2);
        UIComponent component7 = new UIComponent("component_007", "screen_002", "血条", "health_bar", "玩家血条", "ui/components/health_bar", "", "(400, 700)", "(300, 20)", 2);
        UIComponent component8 = new UIComponent("component_008", "screen_003", "英雄列表", "list", "英雄列表", "ui/components/list", "", "(200, 300)", "(300, 400)", 1);
        UIComponent component9 = new UIComponent("component_009", "screen_003", "英雄详情", "panel", "英雄详情面板", "ui/components/panel", "", "(600, 300)", "(400, 400)", 1);
        UIComponent component10 = new UIComponent("component_010", "screen_004", "物品列表", "grid", "物品列表", "ui/components/grid", "", "(200, 300)", "(400, 400)", 1);
        
        uiData.system.AddUIComponent(component1);
        uiData.system.AddUIComponent(component2);
        uiData.system.AddUIComponent(component3);
        uiData.system.AddUIComponent(component4);
        uiData.system.AddUIComponent(component5);
        uiData.system.AddUIComponent(component6);
        uiData.system.AddUIComponent(component7);
        uiData.system.AddUIComponent(component8);
        uiData.system.AddUIComponent(component9);
        uiData.system.AddUIComponent(component10);
        
        // 添加组件到屏幕
        screen1.AddComponent("component_001");
        screen1.AddComponent("component_002");
        screen1.AddComponent("component_003");
        screen1.AddComponent("component_004");
        screen2.AddComponent("component_005");
        screen2.AddComponent("component_006");
        screen2.AddComponent("component_007");
        screen3.AddComponent("component_008");
        screen3.AddComponent("component_009");
        screen4.AddComponent("component_010");
        
        // UI主题
        UITheme theme1 = new UITheme("theme_001", "默认主题", "default", "游戏默认主题", "#4A90E2", "#50E3C2", "#F5A623", "#1A1A1A", "#FFFFFF", "fonts/default", "icons/default");
        UITheme theme2 = new UITheme("theme_002", "暗黑主题", "dark", "暗黑风格主题", "#333333", "#555555", "#FF4500", "#0A0A0A", "#CCCCCC", "fonts/dark", "icons/dark");
        UITheme theme3 = new UITheme("theme_003", "光明主题", "light", "光明风格主题", "#FFFFFF", "#F0F0F0", "#4A90E2", "#F5F5F5", "#333333", "fonts/light", "icons/light");
        UITheme theme4 = new UITheme("theme_004", "节日主题", "festival", "节日风格主题", "#FF6B6B", "#4ECDC4", "#45B7D1", "#292F36", "#FFFFFF", "fonts/festival", "icons/festival");
        
        uiData.system.AddUITheme(theme1);
        uiData.system.AddUITheme(theme2);
        uiData.system.AddUITheme(theme3);
        uiData.system.AddUITheme(theme4);
        
        // UI样式
        UIStyle style1 = new UIStyle("style_001", "默认样式", "default", "默认UI样式", "16px", "normal", "10px", "5px", "5px", "1px", "#CCCCCC");
        UIStyle style2 = new UIStyle("style_002", "按钮样式", "button", "按钮样式", "14px", "bold", "15px", "10px", "8px", "2px", "#4A90E2");
        UIStyle style3 = new UIStyle("style_003", "标题样式", "title", "标题样式", "24px", "bold", "20px", "10px", "0px", "0px", "#000000");
        UIStyle style4 = new UIStyle("style_004", "文本样式", "text", "文本样式", "14px", "normal", "5px", "5px", "0px", "0px", "#000000");
        
        uiData.system.AddUIStyle(style1);
        uiData.system.AddUIStyle(style2);
        uiData.system.AddUIStyle(style3);
        uiData.system.AddUIStyle(style4);
        
        // 玩家UI设置
        PlayerUISettings settings1 = new PlayerUISettings("settings_001", "user_001");
        settings1.SetTheme("theme_001");
        settings1.SetStyle("style_001");
        settings1.AddEnabledScreen("screen_001");
        settings1.AddEnabledScreen("screen_002");
        settings1.AddEnabledScreen("screen_003");
        settings1.AddEnabledScreen("screen_004");
        settings1.AddEnabledScreen("screen_005");
        settings1.AddEnabledScreen("screen_006");
        settings1.AddEnabledScreen("screen_007");
        
        PlayerUISettings settings2 = new PlayerUISettings("settings_002", "user_002");
        settings2.SetTheme("theme_002");
        settings2.SetStyle("style_001");
        settings2.AddEnabledScreen("screen_001");
        settings2.AddEnabledScreen("screen_002");
        settings2.AddEnabledScreen("screen_003");
        settings2.AddEnabledScreen("screen_004");
        settings2.AddEnabledScreen("screen_005");
        settings2.AddEnabledScreen("screen_006");
        settings2.AddEnabledScreen("screen_007");
        
        PlayerUISettings settings3 = new PlayerUISettings("settings_003", "user_003");
        settings3.SetTheme("theme_003");
        settings3.SetStyle("style_001");
        settings3.AddEnabledScreen("screen_001");
        settings3.AddEnabledScreen("screen_002");
        settings3.AddEnabledScreen("screen_003");
        settings3.AddEnabledScreen("screen_004");
        settings3.AddEnabledScreen("screen_005");
        settings3.AddEnabledScreen("screen_006");
        settings3.AddEnabledScreen("screen_007");
        
        uiData.system.AddPlayerUISettings(settings1);
        uiData.system.AddPlayerUISettings(settings2);
        uiData.system.AddPlayerUISettings(settings3);
        
        SaveUIData();
    }
    
    // UI屏幕管理
    public void AddUIScreen(string name, string type, string desc, string prefabPath, string scenePath, bool isModal = false, bool isPersistent = false)
    {
        string screenID = "screen_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        UIScreen screen = new UIScreen(screenID, name, type, desc, prefabPath, scenePath, isModal, isPersistent);
        uiData.system.AddUIScreen(screen);
        SaveUIData();
        Debug.Log("成功添加UI屏幕: " + name);
    }
    
    public List<UIScreen> GetUIScreensByType(string type)
    {
        return uiData.system.GetUIScreensByType(type);
    }
    
    public List<UIScreen> GetAllUIScreens()
    {
        return uiData.system.uiScreens;
    }
    
    public void EnableUIScreen(string screenID)
    {
        UIScreen screen = uiData.system.GetUIScreen(screenID);
        if (screen != null)
        {
            screen.Enable();
            SaveUIData();
            Debug.Log("成功启用UI屏幕: " + screen.screenName);
        }
        else
        {
            Debug.LogError("UI屏幕不存在: " + screenID);
        }
    }
    
    public void DisableUIScreen(string screenID)
    {
        UIScreen screen = uiData.system.GetUIScreen(screenID);
        if (screen != null)
        {
            screen.Disable();
            SaveUIData();
            Debug.Log("成功禁用UI屏幕: " + screen.screenName);
        }
        else
        {
            Debug.LogError("UI屏幕不存在: " + screenID);
        }
    }
    
    // UI组件管理
    public void AddUIComponent(string screenID, string name, string type, string desc, string prefabPath, string parentID = "", string position = "(0,0)", string size = "(100,100)", int zIndex = 0)
    {
        string componentID = "component_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        UIComponent component = new UIComponent(componentID, screenID, name, type, desc, prefabPath, parentID, position, size, zIndex);
        uiData.system.AddUIComponent(component);
        
        UIScreen screen = uiData.system.GetUIScreen(screenID);
        if (screen != null)
        {
            screen.AddComponent(componentID);
        }
        
        SaveUIData();
        Debug.Log("成功添加UI组件: " + name);
    }
    
    public List<UIComponent> GetUIComponentsByScreen(string screenID)
    {
        return uiData.system.GetUIComponentsByScreen(screenID);
    }
    
    public List<UIComponent> GetUIComponentsByType(string type)
    {
        return uiData.system.GetUIComponentsByType(type);
    }
    
    public void EnableUIComponent(string componentID)
    {
        UIComponent component = uiData.system.GetUIComponent(componentID);
        if (component != null)
        {
            component.Enable();
            SaveUIData();
            Debug.Log("成功启用UI组件: " + component.componentName);
        }
        else
        {
            Debug.LogError("UI组件不存在: " + componentID);
        }
    }
    
    public void DisableUIComponent(string componentID)
    {
        UIComponent component = uiData.system.GetUIComponent(componentID);
        if (component != null)
        {
            component.Disable();
            SaveUIData();
            Debug.Log("成功禁用UI组件: " + component.componentName);
        }
        else
        {
            Debug.LogError("UI组件不存在: " + componentID);
        }
    }
    
    public void ShowUIComponent(string componentID)
    {
        UIComponent component = uiData.system.GetUIComponent(componentID);
        if (component != null)
        {
            component.Show();
            SaveUIData();
            Debug.Log("成功显示UI组件: " + component.componentName);
        }
        else
        {
            Debug.LogError("UI组件不存在: " + componentID);
        }
    }
    
    public void HideUIComponent(string componentID)
    {
        UIComponent component = uiData.system.GetUIComponent(componentID);
        if (component != null)
        {
            component.Hide();
            SaveUIData();
            Debug.Log("成功隐藏UI组件: " + component.componentName);
        }
        else
        {
            Debug.LogError("UI组件不存在: " + componentID);
        }
    }
    
    // UI主题管理
    public void AddUITheme(string name, string type, string desc, string primaryColor, string secondaryColor, string accentColor, string backgroundColor, string textColor, string fontPath, string iconPath)
    {
        string themeID = "theme_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        UITheme theme = new UITheme(themeID, name, type, desc, primaryColor, secondaryColor, accentColor, backgroundColor, textColor, fontPath, iconPath);
        uiData.system.AddUITheme(theme);
        SaveUIData();
        Debug.Log("成功添加UI主题: " + name);
    }
    
    public List<UITheme> GetUIThemesByType(string type)
    {
        return uiData.system.GetUIThemesByType(type);
    }
    
    public List<UITheme> GetAllUIThemes()
    {
        return uiData.system.uiThemes;
    }
    
    // UI样式管理
    public void AddUIStyle(string name, string type, string desc, string fontSize, string fontWeight, string padding, string margin, string borderRadius, string borderWidth, string borderColor)
    {
        string styleID = "style_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        UIStyle style = new UIStyle(styleID, name, type, desc, fontSize, fontWeight, padding, margin, borderRadius, borderWidth, borderColor);
        uiData.system.AddUIStyle(style);
        SaveUIData();
        Debug.Log("成功添加UI样式: " + name);
    }
    
    public List<UIStyle> GetAllUIStyles()
    {
        return uiData.system.uiStyles;
    }
    
    // 玩家UI设置管理
    public void AddPlayerUISettings(string playerID)
    {
        PlayerUISettings existing = uiData.system.GetPlayerUISettings(playerID);
        if (existing == null)
        {
            string settingsID = "settings_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            PlayerUISettings settings = new PlayerUISettings(settingsID, playerID);
            uiData.system.AddPlayerUISettings(settings);
            SaveUIData();
            Debug.Log("成功创建玩家UI设置: " + playerID);
        }
    }
    
    public void SetPlayerTheme(string playerID, string themeID)
    {
        PlayerUISettings settings = uiData.system.GetPlayerUISettings(playerID);
        if (settings != null)
        {
            settings.SetTheme(themeID);
            SaveUIData();
            Debug.Log("成功设置玩家UI主题: " + themeID);
        }
        else
        {
            AddPlayerUISettings(playerID);
            SetPlayerTheme(playerID, themeID);
        }
    }
    
    public void SetPlayerStyle(string playerID, string styleID)
    {
        PlayerUISettings settings = uiData.system.GetPlayerUISettings(playerID);
        if (settings != null)
        {
            settings.SetStyle(styleID);
            SaveUIData();
            Debug.Log("成功设置玩家UI样式: " + styleID);
        }
        else
        {
            AddPlayerUISettings(playerID);
            SetPlayerStyle(playerID, styleID);
        }
    }
    
    public void SetPlayerUISettings(string playerID, bool fullscreen, bool hudVisible, bool minimapVisible, bool chatVisible, bool notificationsVisible, float uiScale, float soundVolume, float musicVolume, string language)
    {
        PlayerUISettings settings = uiData.system.GetPlayerUISettings(playerID);
        if (settings != null)
        {
            settings.SetFullscreen(fullscreen);
            settings.SetHUDVisible(hudVisible);
            settings.SetMinimapVisible(minimapVisible);
            settings.SetChatVisible(chatVisible);
            settings.SetNotificationsVisible(notificationsVisible);
            settings.SetUIScale(uiScale);
            settings.SetSoundVolume(soundVolume);
            settings.SetMusicVolume(musicVolume);
            settings.SetLanguage(language);
            SaveUIData();
            Debug.Log("成功更新玩家UI设置");
        }
        else
        {
            AddPlayerUISettings(playerID);
            SetPlayerUISettings(playerID, fullscreen, hudVisible, minimapVisible, chatVisible, notificationsVisible, uiScale, soundVolume, musicVolume, language);
        }
    }
    
    public PlayerUISettings GetPlayerUISettings(string playerID)
    {
        return uiData.system.GetPlayerUISettings(playerID);
    }
    
    // 数据持久化
    public void SaveUIData()
    {
        string path = Application.dataPath + "/Data/ui_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, uiData);
        stream.Close();
    }
    
    public void LoadUIData()
    {
        string path = Application.dataPath + "/Data/ui_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            uiData = (UISystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            uiData = new UISystemDetailedManagerData();
        }
    }
}
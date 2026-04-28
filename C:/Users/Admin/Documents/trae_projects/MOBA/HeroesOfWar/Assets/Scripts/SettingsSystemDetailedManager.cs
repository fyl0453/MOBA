using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSystemDetailedManager
{
    private static SettingsSystemDetailedManager _instance;
    public static SettingsSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SettingsSystemDetailedManager();
            }
            return _instance;
        }
    }

    private SettingsSystemData settingsData;
    private SettingsSystemDataManager dataManager;

    private SettingsSystemDetailedManager()
    {
        dataManager = SettingsSystemDataManager.Instance;
        settingsData = dataManager.settingsData;
    }

    public void InitializePlayerSettings(string playerID)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            PlayerSettings playerSettings = new PlayerSettings(playerID);
            settingsData.AddPlayerSettings(playerID, playerSettings);
            dataManager.SaveSettingsData();
            Debug.Log("初始化玩家设置成功");
        }
    }

    public void UpdateControlSettings(string playerID, ControlSettings controlSettings)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        
        settingsData.PlayerSettings[playerID].ControlSettings = controlSettings;
        settingsData.PlayerSettings[playerID].LastUpdateTime = DateTime.Now;
        
        dataManager.CreateSettingsEvent("control_settings_update", playerID, "control", "更新操作设置");
        dataManager.SaveSettingsData();
        Debug.Log("更新操作设置成功");
    }

    public void UpdateUISettings(string playerID, UIConfig uiSettings)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        
        settingsData.PlayerSettings[playerID].UISettings = uiSettings;
        settingsData.PlayerSettings[playerID].LastUpdateTime = DateTime.Now;
        
        dataManager.CreateSettingsEvent("ui_settings_update", playerID, "ui", "更新界面设置");
        dataManager.SaveSettingsData();
        Debug.Log("更新界面设置成功");
    }

    public void UpdateAudioSettings(string playerID, AudioSettings audioSettings)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        
        settingsData.PlayerSettings[playerID].AudioSettings = audioSettings;
        settingsData.PlayerSettings[playerID].LastUpdateTime = DateTime.Now;
        
        dataManager.CreateSettingsEvent("audio_settings_update", playerID, "audio", "更新音效设置");
        dataManager.SaveSettingsData();
        Debug.Log("更新音效设置成功");
    }

    public void UpdateGraphicsSettings(string playerID, GraphicsSettings graphicsSettings)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        
        settingsData.PlayerSettings[playerID].GraphicsSettings = graphicsSettings;
        settingsData.PlayerSettings[playerID].LastUpdateTime = DateTime.Now;
        
        dataManager.CreateSettingsEvent("graphics_settings_update", playerID, "graphics", "更新图形设置");
        dataManager.SaveSettingsData();
        Debug.Log("更新图形设置成功");
    }

    public void UpdateAccessibilitySettings(string playerID, AccessibilitySettings accessibilitySettings)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        
        settingsData.PlayerSettings[playerID].AccessibilitySettings = accessibilitySettings;
        settingsData.PlayerSettings[playerID].LastUpdateTime = DateTime.Now;
        
        dataManager.CreateSettingsEvent("accessibility_settings_update", playerID, "accessibility", "更新无障碍设置");
        dataManager.SaveSettingsData();
        Debug.Log("更新无障碍设置成功");
    }

    public void UpdateGeneralSettings(string playerID, bool notificationsEnabled, bool autoAcceptMatch, bool autoJoinVoice, bool showTutorials, bool privacyMode, string language)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        
        PlayerSettings playerSettings = settingsData.PlayerSettings[playerID];
        playerSettings.NotificationsEnabled = notificationsEnabled;
        playerSettings.AutoAcceptMatch = autoAcceptMatch;
        playerSettings.AutoJoinVoice = autoJoinVoice;
        playerSettings.ShowTutorials = showTutorials;
        playerSettings.PrivacyMode = privacyMode;
        playerSettings.Language = language;
        playerSettings.LastUpdateTime = DateTime.Now;
        
        dataManager.CreateSettingsEvent("general_settings_update", playerID, "general", "更新通用设置");
        dataManager.SaveSettingsData();
        Debug.Log("更新通用设置成功");
    }

    public ControlSettings GetControlSettings(string playerID)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        return settingsData.PlayerSettings[playerID].ControlSettings;
    }

    public UIConfig GetUISettings(string playerID)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        return settingsData.PlayerSettings[playerID].UISettings;
    }

    public AudioSettings GetAudioSettings(string playerID)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        return settingsData.PlayerSettings[playerID].AudioSettings;
    }

    public GraphicsSettings GetGraphicsSettings(string playerID)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        return settingsData.PlayerSettings[playerID].GraphicsSettings;
    }

    public AccessibilitySettings GetAccessibilitySettings(string playerID)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        return settingsData.PlayerSettings[playerID].AccessibilitySettings;
    }

    public PlayerSettings GetPlayerSettings(string playerID)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        return settingsData.PlayerSettings[playerID];
    }

    public void ResetSettings(string playerID, string settingType)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        
        PlayerSettings playerSettings = settingsData.PlayerSettings[playerID];
        
        switch (settingType)
        {
            case "control":
                playerSettings.ControlSettings = new ControlSettings();
                break;
            case "ui":
                playerSettings.UISettings = new UIConfig();
                break;
            case "audio":
                playerSettings.AudioSettings = new AudioSettings();
                break;
            case "graphics":
                playerSettings.GraphicsSettings = new GraphicsSettings();
                break;
            case "accessibility":
                playerSettings.AccessibilitySettings = new AccessibilitySettings();
                break;
            case "all":
                playerSettings.ControlSettings = new ControlSettings();
                playerSettings.UISettings = new UIConfig();
                playerSettings.AudioSettings = new AudioSettings();
                playerSettings.GraphicsSettings = new GraphicsSettings();
                playerSettings.AccessibilitySettings = new AccessibilitySettings();
                break;
        }
        
        playerSettings.LastUpdateTime = DateTime.Now;
        
        dataManager.CreateSettingsEvent("settings_reset", playerID, settingType, "重置设置: " + settingType);
        dataManager.SaveSettingsData();
        Debug.Log("重置设置成功: " + settingType);
    }

    public void ApplySettings(string playerID)
    {
        if (!settingsData.PlayerSettings.ContainsKey(playerID))
        {
            InitializePlayerSettings(playerID);
        }
        
        PlayerSettings playerSettings = settingsData.PlayerSettings[playerID];
        
        ApplyControlSettings(playerSettings.ControlSettings);
        ApplyUISettings(playerSettings.UISettings);
        ApplyAudioSettings(playerSettings.AudioSettings);
        ApplyGraphicsSettings(playerSettings.GraphicsSettings);
        ApplyAccessibilitySettings(playerSettings.AccessibilitySettings);
        
        dataManager.CreateSettingsEvent("settings_apply", playerID, "all", "应用设置");
        Debug.Log("应用设置成功");
    }

    private void ApplyControlSettings(ControlSettings controlSettings)
    {
        
    }

    private void ApplyUISettings(UIConfig uiSettings)
    {
        
    }

    private void ApplyAudioSettings(AudioSettings audioSettings)
    {
        AudioListener.volume = audioSettings.MasterVolume;
    }

    private void ApplyGraphicsSettings(GraphicsSettings graphicsSettings)
    {
        QualitySettings.SetQualityLevel(graphicsSettings.QualityPreset);
    }

    private void ApplyAccessibilitySettings(AccessibilitySettings accessibilitySettings)
    {
        
    }

    public List<string> GetAvailableThemes()
    {
        return settingsData.AvailableThemes;
    }

    public List<string> GetAvailableLanguages()
    {
        return settingsData.AvailableLanguages;
    }

    public List<string> GetControlSchemes()
    {
        return settingsData.ControlSchemes;
    }

    public void AddControlScheme(string schemeName)
    {
        if (!settingsData.ControlSchemes.Contains(schemeName))
        {
            settingsData.ControlSchemes.Add(schemeName);
            dataManager.SaveSettingsData();
            Debug.Log("添加操作方案成功: " + schemeName);
        }
    }

    public void RemoveControlScheme(string schemeName)
    {
        if (schemeName != "默认" && settingsData.ControlSchemes.Contains(schemeName))
        {
            settingsData.ControlSchemes.Remove(schemeName);
            dataManager.SaveSettingsData();
            Debug.Log("删除操作方案成功: " + schemeName);
        }
    }

    public void AddTheme(string themeName)
    {
        if (!settingsData.AvailableThemes.Contains(themeName))
        {
            settingsData.AvailableThemes.Add(themeName);
            dataManager.SaveSettingsData();
            Debug.Log("添加主题成功: " + themeName);
        }
    }

    public void RemoveTheme(string themeName)
    {
        if (themeName != "default" && settingsData.AvailableThemes.Contains(themeName))
        {
            settingsData.AvailableThemes.Remove(themeName);
            dataManager.SaveSettingsData();
            Debug.Log("删除主题成功: " + themeName);
        }
    }

    public void AddLanguage(string languageCode)
    {
        if (!settingsData.AvailableLanguages.Contains(languageCode))
        {
            settingsData.AvailableLanguages.Add(languageCode);
            dataManager.SaveSettingsData();
            Debug.Log("添加语言成功: " + languageCode);
        }
    }

    public void RemoveLanguage(string languageCode)
    {
        if (languageCode != "zh-CN" && settingsData.AvailableLanguages.Contains(languageCode))
        {
            settingsData.AvailableLanguages.Remove(languageCode);
            dataManager.SaveSettingsData();
            Debug.Log("删除语言成功: " + languageCode);
        }
    }

    public void CleanupOldSettings(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<string> playersToRemove = new List<string>();
        
        foreach (KeyValuePair<string, PlayerSettings> kvp in settingsData.PlayerSettings)
        {
            if (kvp.Value.LastUpdateTime < cutoffDate)
            {
                playersToRemove.Add(kvp.Key);
            }
        }
        
        foreach (string playerID in playersToRemove)
        {
            settingsData.PlayerSettings.Remove(playerID);
        }
        
        if (playersToRemove.Count > 0)
        {
            dataManager.CreateSettingsEvent("settings_cleanup", "system", "general", "清理旧设置: " + playersToRemove.Count);
            dataManager.SaveSettingsData();
            Debug.Log("清理旧设置成功: " + playersToRemove.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveSettingsData();
    }

    public void LoadData()
    {
        dataManager.LoadSettingsData();
    }

    public List<SettingsEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
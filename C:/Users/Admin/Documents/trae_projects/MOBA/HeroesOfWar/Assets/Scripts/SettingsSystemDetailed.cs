using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class ControlSettings
{
    public string SchemeName;
    public float MovementSensitivity;
    public float SkillSensitivity;
    public bool InvertYAxis;
    public bool AutoAttackEnabled;
    public bool QuickCastEnabled;
    public bool SmartCastEnabled;
    public int SkillCastMode;
    public int AttackMode;
    public int CameraFollowMode;
    public bool ShowSkillRange;
    public bool ShowAttackRange;

    public ControlSettings()
    {
        SchemeName = "默认设置";
        MovementSensitivity = 1.0f;
        SkillSensitivity = 1.0f;
        InvertYAxis = false;
        AutoAttackEnabled = true;
        QuickCastEnabled = false;
        SmartCastEnabled = true;
        SkillCastMode = 0;
        AttackMode = 0;
        CameraFollowMode = 0;
        ShowSkillRange = true;
        ShowAttackRange = false;
    }
}

[Serializable]
public class UIConfig
{
    public string Theme;
    public int UIQuality;
    public int Resolution;
    public bool Fullscreen;
    public int FrameRateLimit;
    public bool ShowFPS;
    public bool ShowMinimap;
    public bool ShowHealthBars;
    public bool ShowDamageNumbers;
    public bool ShowSkillCooldowns;
    public bool ShowChat;
    public bool ShowPlayerNames;
    public float UIOpacity;

    public UIConfig()
    {
        Theme = "default";
        UIQuality = 2;
        Resolution = 1;
        Fullscreen = true;
        FrameRateLimit = 60;
        ShowFPS = false;
        ShowMinimap = true;
        ShowHealthBars = true;
        ShowDamageNumbers = true;
        ShowSkillCooldowns = true;
        ShowChat = true;
        ShowPlayerNames = true;
        UIOpacity = 1.0f;
    }
}

[Serializable]
public class AudioSettings
{
    public float MasterVolume;
    public float MusicVolume;
    public float SFXVolume;
    public float VoiceVolume;
    public float AmbientVolume;
    public bool MusicEnabled;
    public bool SFXEnabled;
    public bool VoiceEnabled;
    public bool AmbientEnabled;
    public bool SpatialAudio;
    public int AudioQuality;

    public AudioSettings()
    {
        MasterVolume = 0.7f;
        MusicVolume = 0.6f;
        SFXVolume = 0.8f;
        VoiceVolume = 0.7f;
        AmbientVolume = 0.5f;
        MusicEnabled = true;
        SFXEnabled = true;
        VoiceEnabled = true;
        AmbientEnabled = true;
        SpatialAudio = false;
        AudioQuality = 1;
    }
}

[Serializable]
public class GraphicsSettings
{
    public int QualityPreset;
    public int TextureQuality;
    public int ShadowQuality;
    public int EffectQuality;
    public int AntiAliasing;
    public int VSync;
    public bool BloomEnabled;
    public bool MotionBlurEnabled;
    public bool DepthOfFieldEnabled;
    public bool DynamicLighting;
    public float Brightness;
    public float Contrast;
    public float Saturation;

    public GraphicsSettings()
    {
        QualityPreset = 2;
        TextureQuality = 2;
        ShadowQuality = 1;
        EffectQuality = 2;
        AntiAliasing = 2;
        VSync = 0;
        BloomEnabled = true;
        MotionBlurEnabled = false;
        DepthOfFieldEnabled = false;
        DynamicLighting = true;
        Brightness = 1.0f;
        Contrast = 1.0f;
        Saturation = 1.0f;
    }
}

[Serializable]
public class AccessibilitySettings
{
    public bool ColorBlindMode;
    public int ColorBlindType;
    public float TextSize;
    public bool HighContrastUI;
    public bool SubtitlesEnabled;
    public bool AudioCuesEnabled;
    public bool VisualCuesEnabled;
    public bool HapticFeedback;

    public AccessibilitySettings()
    {
        ColorBlindMode = false;
        ColorBlindType = 0;
        TextSize = 1.0f;
        HighContrastUI = false;
        SubtitlesEnabled = false;
        AudioCuesEnabled = true;
        VisualCuesEnabled = true;
        HapticFeedback = true;
    }
}

[Serializable]
public class PlayerSettings
{
    public string PlayerID;
    public ControlSettings ControlSettings;
    public UIConfig UISettings;
    public AudioSettings AudioSettings;
    public GraphicsSettings GraphicsSettings;
    public AccessibilitySettings AccessibilitySettings;
    public bool NotificationsEnabled;
    public bool AutoAcceptMatch;
    public bool AutoJoinVoice;
    public bool ShowTutorials;
    public bool PrivacyMode;
    public string Language;
    public DateTime LastUpdateTime;

    public PlayerSettings(string playerID)
    {
        PlayerID = playerID;
        ControlSettings = new ControlSettings();
        UISettings = new UIConfig();
        AudioSettings = new AudioSettings();
        GraphicsSettings = new GraphicsSettings();
        AccessibilitySettings = new AccessibilitySettings();
        NotificationsEnabled = true;
        AutoAcceptMatch = false;
        AutoJoinVoice = true;
        ShowTutorials = true;
        PrivacyMode = false;
        Language = "zh-CN";
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class SettingsSystemData
{
    public Dictionary<string, PlayerSettings> PlayerSettings;
    public List<string> AvailableThemes;
    public List<string> AvailableLanguages;
    public List<string> ControlSchemes;
    public int MaxSettingsProfiles;
    public DateTime LastSystemUpdate;

    public SettingsSystemData()
    {
        PlayerSettings = new Dictionary<string, PlayerSettings>();
        AvailableThemes = new List<string> { "default", "dark", "light", "neon" };
        AvailableLanguages = new List<string> { "zh-CN", "zh-TW", "en-US", "ja-JP", "ko-KR" };
        ControlSchemes = new List<string> { "默认", "左手", "右手", "自定义" };
        MaxSettingsProfiles = 5;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddPlayerSettings(string playerID, PlayerSettings settings)
    {
        PlayerSettings[playerID] = settings;
    }
}

[Serializable]
public class SettingsEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string SettingType;
    public string EventData;

    public SettingsEvent(string eventID, string eventType, string playerID, string settingType, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        SettingType = settingType;
        EventData = eventData;
    }
}

public class SettingsSystemDataManager
{
    private static SettingsSystemDataManager _instance;
    public static SettingsSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SettingsSystemDataManager();
            }
            return _instance;
        }
    }

    public SettingsSystemData settingsData;
    private List<SettingsEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SettingsSystemDataManager()
    {
        settingsData = new SettingsSystemData();
        recentEvents = new List<SettingsEvent>();
        LoadSettingsData();
    }

    public void SaveSettingsData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SettingsSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, settingsData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存设置系统数据失败: " + e.Message);
        }
    }

    public void LoadSettingsData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SettingsSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    settingsData = (SettingsSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载设置系统数据失败: " + e.Message);
            settingsData = new SettingsSystemData();
        }
    }

    public void CreateSettingsEvent(string eventType, string playerID, string settingType, string eventData)
    {
        string eventID = "settings_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SettingsEvent settingsEvent = new SettingsEvent(eventID, eventType, playerID, settingType, eventData);
        recentEvents.Add(settingsEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<SettingsEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
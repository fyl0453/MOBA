using UnityEngine;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    
    public SettingsData settingsData;
    private string settingsFilePath;
    
    [System.Serializable]
    public class SettingsData
    {
        public float masterVolume = 1.0f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1.0f;
        public int graphicsQuality = 2; // 0: 低, 1: 中, 2: 高
        public int resolutionIndex = 0;
        public bool vsync = true;
        public bool fullscreen = true;
        public float mouseSensitivity = 1.0f;
        public bool invertYAxis = false;
        public bool showFPS = false;
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeSettings()
    {
        settingsFilePath = Path.Combine(Application.dataPath, "SaveData", "Settings.dat");
        LoadSettings();
        ApplySettings();
    }
    
    public void LoadSettings()
    {
        try
        {
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                settingsData = JsonUtility.FromJson<SettingsData>(json);
            }
            else
            {
                settingsData = new SettingsData();
                SaveSettings();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("加载设置失败: " + e.Message);
            settingsData = new SettingsData();
        }
    }
    
    public void SaveSettings()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));
            string json = JsonUtility.ToJson(settingsData, true);
            File.WriteAllText(settingsFilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("保存设置失败: " + e.Message);
        }
    }
    
    public void ApplySettings()
    {
        // 应用音频设置
        AudioListener.volume = settingsData.masterVolume;
        
        // 应用图形设置
        QualitySettings.SetQualityLevel(settingsData.graphicsQuality);
        QualitySettings.vSyncCount = settingsData.vsync ? 1 : 0;
        Screen.fullScreen = settingsData.fullscreen;
        
        // 应用分辨率设置
        // 这里可以根据resolutionIndex设置不同的分辨率
    }
    
    public void SetMasterVolume(float volume)
    {
        settingsData.masterVolume = Mathf.Clamp01(volume);
        SaveSettings();
        ApplySettings();
    }
    
    public void SetMusicVolume(float volume)
    {
        settingsData.musicVolume = Mathf.Clamp01(volume);
        SaveSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        settingsData.sfxVolume = Mathf.Clamp01(volume);
        SaveSettings();
    }
    
    public void SetGraphicsQuality(int quality)
    {
        settingsData.graphicsQuality = Mathf.Clamp(quality, 0, 2);
        SaveSettings();
        ApplySettings();
    }
    
    public void SetResolution(int index)
    {
        settingsData.resolutionIndex = index;
        SaveSettings();
        ApplySettings();
    }
    
    public void SetVSync(bool enable)
    {
        settingsData.vsync = enable;
        SaveSettings();
        ApplySettings();
    }
    
    public void SetFullscreen(bool enable)
    {
        settingsData.fullscreen = enable;
        SaveSettings();
        ApplySettings();
    }
    
    public void SetMouseSensitivity(float sensitivity)
    {
        settingsData.mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 3.0f);
        SaveSettings();
    }
    
    public void SetInvertYAxis(bool invert)
    {
        settingsData.invertYAxis = invert;
        SaveSettings();
    }
    
    public void SetShowFPS(bool show)
    {
        settingsData.showFPS = show;
        SaveSettings();
    }
}

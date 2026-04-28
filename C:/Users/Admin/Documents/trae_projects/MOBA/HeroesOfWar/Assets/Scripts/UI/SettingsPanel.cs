using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsPanel : UIPanel
{
    [SerializeField] private TabButton gameTab;
    [SerializeField] private TabButton audioTab;
    [SerializeField] private TabButton graphicsTab;
    [SerializeField] private TabButton accountTab;
    
    [SerializeField] private GameObject gameContent;
    [SerializeField] private GameObject audioContent;
    [SerializeField] private GameObject graphicsContent;
    [SerializeField] private GameObject accountContent;
    
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton;
    
    // 游戏设置
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Toggle invertYAxisToggle;
    [SerializeField] private Toggle showFPSToggle;
    
    // 音频设置
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    // 图形设置
    [SerializeField] private Dropdown graphicsQualityDropdown;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private Toggle fullscreenToggle;
    
    private void Start()
    {
        gameTab.onClick.AddListener(() => SwitchTab("Game"));
        audioTab.onClick.AddListener(() => SwitchTab("Audio"));
        graphicsTab.onClick.AddListener(() => SwitchTab("Graphics"));
        accountTab.onClick.AddListener(() => SwitchTab("Account"));
        backButton.onClick.AddListener(OnBack);
        saveButton.onClick.AddListener(OnSave);
        
        // 初始化滑块和开关的事件
        if (mouseSensitivitySlider != null) mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
        if (invertYAxisToggle != null) invertYAxisToggle.onValueChanged.AddListener(OnInvertYAxisChanged);
        if (showFPSToggle != null) showFPSToggle.onValueChanged.AddListener(OnShowFPSChanged);
        
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        if (graphicsQualityDropdown != null) graphicsQualityDropdown.onValueChanged.AddListener(OnGraphicsQualityChanged);
        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        if (vsyncToggle != null) vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        if (fullscreenToggle != null) fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        
        SwitchTab("Game");
    }
    
    private void SwitchTab(string tabName)
    {
        gameContent.SetActive(tabName == "Game");
        audioContent.SetActive(tabName == "Audio");
        graphicsContent.SetActive(tabName == "Graphics");
        accountContent.SetActive(tabName == "Account");
        
        gameTab.SetSelected(tabName == "Game");
        audioTab.SetSelected(tabName == "Audio");
        graphicsTab.SetSelected(tabName == "Graphics");
        accountTab.SetSelected(tabName == "Account");
        
        if (tabName == "Game")
        {
            InitializeGameSettings();
        }
        else if (tabName == "Audio")
        {
            InitializeAudioSettings();
        }
        else if (tabName == "Graphics")
        {
            InitializeGraphicsSettings();
        }
        else if (tabName == "Account")
        {
            InitializeAccountSettings();
        }
    }
    
    private void InitializeGameSettings()
    {
        if (SettingsManager.Instance != null)
        {
            if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = SettingsManager.Instance.settingsData.mouseSensitivity;
            if (invertYAxisToggle != null) invertYAxisToggle.isOn = SettingsManager.Instance.settingsData.invertYAxis;
            if (showFPSToggle != null) showFPSToggle.isOn = SettingsManager.Instance.settingsData.showFPS;
        }
    }
    
    private void InitializeAudioSettings()
    {
        if (SettingsManager.Instance != null)
        {
            if (masterVolumeSlider != null) masterVolumeSlider.value = SettingsManager.Instance.settingsData.masterVolume;
            if (musicVolumeSlider != null) musicVolumeSlider.value = SettingsManager.Instance.settingsData.musicVolume;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = SettingsManager.Instance.settingsData.sfxVolume;
        }
    }
    
    private void InitializeGraphicsSettings()
    {
        if (SettingsManager.Instance != null)
        {
            if (graphicsQualityDropdown != null) graphicsQualityDropdown.value = SettingsManager.Instance.settingsData.graphicsQuality;
            if (resolutionDropdown != null) resolutionDropdown.value = SettingsManager.Instance.settingsData.resolutionIndex;
            if (vsyncToggle != null) vsyncToggle.isOn = SettingsManager.Instance.settingsData.vsync;
            if (fullscreenToggle != null) fullscreenToggle.isOn = SettingsManager.Instance.settingsData.fullscreen;
        }
    }
    
    private void InitializeAccountSettings()
    {
        // 初始化账号设置
        Debug.Log("初始化账号设置");
    }
    
    private void OnMouseSensitivityChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMouseSensitivity(value);
        }
    }
    
    private void OnInvertYAxisChanged(bool value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetInvertYAxis(value);
        }
    }
    
    private void OnShowFPSChanged(bool value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetShowFPS(value);
        }
    }
    
    private void OnMasterVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMasterVolume(value);
        }
    }
    
    private void OnMusicVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMusicVolume(value);
        }
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetSFXVolume(value);
        }
    }
    
    private void OnGraphicsQualityChanged(int value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetGraphicsQuality(value);
        }
    }
    
    private void OnResolutionChanged(int value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetResolution(value);
        }
    }
    
    private void OnVSyncChanged(bool value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetVSync(value);
        }
    }
    
    private void OnFullscreenChanged(bool value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetFullscreen(value);
        }
    }
    
    private void OnSave()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveSettings();
            Debug.Log("设置保存成功");
        }
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

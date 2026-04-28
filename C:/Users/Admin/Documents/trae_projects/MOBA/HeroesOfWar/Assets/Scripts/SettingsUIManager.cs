using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    public static SettingsUIManager Instance { get; private set; }
    
    public Canvas settingsCanvas;
    public Toggle fullscreenToggle;
    public Dropdown qualityDropdown;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider voiceVolumeSlider;
    public Slider sensitivitySlider;
    public Toggle invertYAxisToggle;
    public Toggle autoRunToggle;
    public Toggle tapToMoveToggle;
    public Toggle rememberLoginToggle;
    public Toggle autoLoginToggle;
    public Toggle enableCloudSaveToggle;
    public Toggle enablePushNotificationsToggle;
    public Toggle enableFriendRequestsToggle;
    public Toggle enableGameInvitesToggle;
    public Toggle enableAchievementsToggle;
    public Toggle showOnlineStatusToggle;
    public Toggle allowFriendRequestsToggle;
    public Toggle allowGameInvitesToggle;
    public Toggle allowSpectatingToggle;
    public Button applyButton;
    public Button resetButton;
    public Button closeButton;
    
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
        settingsCanvas.gameObject.SetActive(false);
        applyButton.onClick.AddListener(ApplySettings);
        resetButton.onClick.AddListener(ResetSettings);
        closeButton.onClick.AddListener(CloseSettingsUI);
    }
    
    public void OpenSettingsUI()
    {
        settingsCanvas.gameObject.SetActive(true);
        LoadSettingsToUI();
    }
    
    public void CloseSettingsUI()
    {
        settingsCanvas.gameObject.SetActive(false);
    }
    
    public void LoadSettingsToUI()
    {
        GameSettings gameSettings = SettingsManager.Instance.gameSettings;
        AudioSettings audioSettings = SettingsManager.Instance.audioSettings;
        ControlSettings controlSettings = SettingsManager.Instance.controlSettings;
        AccountSettings accountSettings = SettingsManager.Instance.accountSettings;
        NotificationSettings notificationSettings = SettingsManager.Instance.notificationSettings;
        PrivacySettings privacySettings = SettingsManager.Instance.privacySettings;
        
        // 游戏设置
        fullscreenToggle.isOn = gameSettings.fullscreen;
        qualityDropdown.value = gameSettings.qualityLevel;
        
        // 音频设置
        masterVolumeSlider.value = audioSettings.masterVolume;
        musicVolumeSlider.value = audioSettings.musicVolume;
        sfxVolumeSlider.value = audioSettings.sfxVolume;
        voiceVolumeSlider.value = audioSettings.voiceVolume;
        
        // 控制设置
        sensitivitySlider.value = controlSettings.sensitivity;
        invertYAxisToggle.isOn = controlSettings.invertYAxis;
        autoRunToggle.isOn = controlSettings.autoRun;
        tapToMoveToggle.isOn = controlSettings.tapToMove;
        
        // 账户设置
        rememberLoginToggle.isOn = accountSettings.rememberLogin;
        autoLoginToggle.isOn = accountSettings.autoLogin;
        enableCloudSaveToggle.isOn = accountSettings.enableCloudSave;
        
        // 通知设置
        enablePushNotificationsToggle.isOn = notificationSettings.enablePushNotifications;
        enableFriendRequestsToggle.isOn = notificationSettings.enableFriendRequests;
        enableGameInvitesToggle.isOn = notificationSettings.enableGameInvites;
        enableAchievementsToggle.isOn = notificationSettings.enableAchievements;
        
        // 隐私设置
        showOnlineStatusToggle.isOn = privacySettings.showOnlineStatus;
        allowFriendRequestsToggle.isOn = privacySettings.allowFriendRequests;
        allowGameInvitesToggle.isOn = privacySettings.allowGameInvites;
        allowSpectatingToggle.isOn = privacySettings.allowSpectating;
    }
    
    public void ApplySettings()
    {
        GameSettings gameSettings = SettingsManager.Instance.gameSettings;
        AudioSettings audioSettings = SettingsManager.Instance.audioSettings;
        ControlSettings controlSettings = SettingsManager.Instance.controlSettings;
        AccountSettings accountSettings = SettingsManager.Instance.accountSettings;
        NotificationSettings notificationSettings = SettingsManager.Instance.notificationSettings;
        PrivacySettings privacySettings = SettingsManager.Instance.privacySettings;
        
        // 游戏设置
        gameSettings.fullscreen = fullscreenToggle.isOn;
        gameSettings.qualityLevel = qualityDropdown.value;
        
        // 音频设置
        audioSettings.masterVolume = masterVolumeSlider.value;
        audioSettings.musicVolume = musicVolumeSlider.value;
        audioSettings.sfxVolume = sfxVolumeSlider.value;
        audioSettings.voiceVolume = voiceVolumeSlider.value;
        
        // 控制设置
        controlSettings.sensitivity = sensitivitySlider.value;
        controlSettings.invertYAxis = invertYAxisToggle.isOn;
        controlSettings.autoRun = autoRunToggle.isOn;
        controlSettings.tapToMove = tapToMoveToggle.isOn;
        
        // 账户设置
        accountSettings.rememberLogin = rememberLoginToggle.isOn;
        accountSettings.autoLogin = autoLoginToggle.isOn;
        accountSettings.enableCloudSave = enableCloudSaveToggle.isOn;
        
        // 通知设置
        notificationSettings.enablePushNotifications = enablePushNotificationsToggle.isOn;
        notificationSettings.enableFriendRequests = enableFriendRequestsToggle.isOn;
        notificationSettings.enableGameInvites = enableGameInvitesToggle.isOn;
        notificationSettings.enableAchievements = enableAchievementsToggle.isOn;
        
        // 隐私设置
        privacySettings.showOnlineStatus = showOnlineStatusToggle.isOn;
        privacySettings.allowFriendRequests = allowFriendRequestsToggle.isOn;
        privacySettings.allowGameInvites = allowGameInvitesToggle.isOn;
        privacySettings.allowSpectating = allowSpectatingToggle.isOn;
        
        SettingsManager.Instance.ApplySettings();
        Debug.Log("设置已应用");
    }
    
    public void ResetSettings()
    {
        SettingsManager.Instance.ResetSettings();
        LoadSettingsToUI();
        Debug.Log("设置已重置");
    }
}
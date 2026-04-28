using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    
    public string currentLanguage = "zh-CN";
    private Dictionary<string, string> localizationData = new Dictionary<string, string>();
    private string localizationDirectory;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLocalization();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeLocalization()
    {
        localizationDirectory = Path.Combine(Application.dataPath, "Localization");
        Directory.CreateDirectory(localizationDirectory);
        
        // 自动检测系统语言
        DetectSystemLanguage();
        
        // 加载本地化数据
        LoadLocalizationData(currentLanguage);
    }
    
    private void DetectSystemLanguage()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;
        switch (systemLanguage)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.ChineseTraditional:
                currentLanguage = "zh-CN";
                break;
            case SystemLanguage.English:
                currentLanguage = "en-US";
                break;
            case SystemLanguage.Japanese:
                currentLanguage = "ja-JP";
                break;
            case SystemLanguage.Korean:
                currentLanguage = "ko-KR";
                break;
            default:
                currentLanguage = "en-US";
                break;
        }
    }
    
    public void SetLanguage(string languageCode)
    {
        currentLanguage = languageCode;
        LoadLocalizationData(languageCode);
    }
    
    private void LoadLocalizationData(string languageCode)
    {
        localizationData.Clear();
        
        string filePath = Path.Combine(localizationDirectory, $"{languageCode}.txt");
        
        if (File.Exists(filePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line) && line.Contains("="))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length >= 2)
                        {
                            string key = parts[0].Trim();
                            string value = line.Substring(line.IndexOf('=') + 1).Trim();
                            localizationData[key] = value;
                        }
                    }
                }
                Debug.Log($"加载本地化数据成功: {languageCode}");
            }
            catch (System.Exception e)
            {
                Debug.LogError("加载本地化数据失败: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning($"本地化文件不存在: {filePath}");
            // 创建默认的本地化文件
            CreateDefaultLocalizationFile(languageCode);
        }
    }
    
    private void CreateDefaultLocalizationFile(string languageCode)
    {
        string filePath = Path.Combine(localizationDirectory, $"{languageCode}.txt");
        
        List<string> defaultStrings = new List<string>
        {
            "UI_MainMenu_Start=开始游戏",
            "UI_MainMenu_Heroes=英雄",
            "UI_MainMenu_Social=社交",
            "UI_MainMenu_Inventory=背包",
            "UI_MainMenu_Settings=设置",
            "UI_MainMenu_Leaderboard=排行榜",
            "UI_MainMenu_Quest=任务",
            "UI_MainMenu_Shop=商店",
            "UI_MainMenu_Event=活动",
            "UI_MainMenu_Mail=邮件",
            "UI_MainMenu_Profile=个人资料",
            "UI_MainMenu_Spectator=观战",
            "UI_MainMenu_Replay=录像",
            "UI_HeroSelect_Select=选择",
            "UI_HeroSelect_StartMatch=开始匹配",
            "UI_HeroSelect_Back=返回",
            "UI_Battle_Skill1=技能1",
            "UI_Battle_Skill2=技能2",
            "UI_Battle_Skill3=技能3",
            "UI_Battle_Items=物品",
            "UI_Battle_Settings=设置",
            "UI_Results_Win=胜利",
            "UI_Results_Lose=失败",
            "UI_Results_Kills=击杀",
            "UI_Results_Deaths=死亡",
            "UI_Results_Assists=助攻",
            "UI_Results_Gold=金币",
            "UI_Results_Back=返回",
            "UI_Settings_Game=游戏",
            "UI_Settings_Audio=音频",
            "UI_Settings_Graphics=图形",
            "UI_Settings_Account=账号",
            "UI_Settings_Save=保存",
            "UI_Settings_Back=返回",
            "UI_Settings_MouseSensitivity=鼠标灵敏度",
            "UI_Settings_InvertYAxis=反转Y轴",
            "UI_Settings_ShowFPS=显示FPS",
            "UI_Settings_MasterVolume=主音量",
            "UI_Settings_MusicVolume=音乐音量",
            "UI_Settings_SFXVolume=音效音量",
            "UI_Settings_GraphicsQuality=图形质量",
            "UI_Settings_Resolution=分辨率",
            "UI_Settings_VSync=垂直同步",
            "UI_Settings_Fullscreen=全屏"
        };
        
        try
        {
            File.WriteAllLines(filePath, defaultStrings);
            Debug.Log($"创建默认本地化文件: {filePath}");
            LoadLocalizationData(languageCode);
        }
        catch (System.Exception e)
        {
            Debug.LogError("创建本地化文件失败: " + e.Message);
        }
    }
    
    public string GetLocalizedString(string key)
    {
        if (localizationData.ContainsKey(key))
        {
            return localizationData[key];
        }
        else
        {
            Debug.LogWarning($"本地化键不存在: {key}");
            return key;
        }
    }
    
    public string GetLocalizedString(string key, params object[] args)
    {
        string value = GetLocalizedString(key);
        return string.Format(value, args);
    }
    
    public List<string> GetAvailableLanguages()
    {
        List<string> languages = new List<string>();
        
        string[] files = Directory.GetFiles(localizationDirectory, "*.txt");
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            languages.Add(fileName);
        }
        
        return languages;
    }
}

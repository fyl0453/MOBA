using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    public UIManagerData uiData;
    private Dictionary<string, GameObject> windowInstances = new Dictionary<string, GameObject>();
    
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
            uiData = new UIManagerData();
            InitializeDefaultWindows();
        }
    }
    
    private void InitializeDefaultWindows()
    {
        // 主菜单窗口
        UIWindow mainMenu = new UIWindow("window_main_menu", "主菜单", "UI/MainMenu");
        mainMenu.AddElement(new UIElement("btn_play", "开始游戏按钮", "Button"));
        mainMenu.AddElement(new UIElement("btn_shop", "商店按钮", "Button"));
        mainMenu.AddElement(new UIElement("btn_heroes", "英雄按钮", "Button"));
        mainMenu.AddElement(new UIElement("btn_settings", "设置按钮", "Button"));
        mainMenu.AddElement(new UIElement("btn_exit", "退出按钮", "Button"));
        uiData.AddWindow(mainMenu);
        
        // 游戏设置窗口
        UIWindow settings = new UIWindow("window_settings", "游戏设置", "UI/Settings", true);
        settings.AddElement(new UIElement("slider_quality", "画质滑块", "Slider", "2"));
        settings.AddElement(new UIElement("slider_master_volume", "主音量滑块", "Slider", "1"));
        settings.AddElement(new UIElement("slider_music_volume", "音乐音量滑块", "Slider", "0.8"));
        settings.AddElement(new UIElement("slider_sfx_volume", "音效音量滑块", "Slider", "1"));
        settings.AddElement(new UIElement("btn_save", "保存按钮", "Button"));
        settings.AddElement(new UIElement("btn_cancel", "取消按钮", "Button"));
        uiData.AddWindow(settings);
        
        // 英雄选择窗口
        UIWindow heroSelect = new UIWindow("window_hero_select", "英雄选择", "UI/HeroSelect", true);
        heroSelect.AddElement(new UIElement("grid_heroes", "英雄网格", "Grid"));
        heroSelect.AddElement(new UIElement("btn_confirm", "确认按钮", "Button"));
        heroSelect.AddElement(new UIElement("btn_cancel", "取消按钮", "Button"));
        uiData.AddWindow(heroSelect);
        
        // 游戏内HUD
        UIWindow gameHUD = new UIWindow("window_game_hud", "游戏HUD", "UI/GameHUD", false, true);
        gameHUD.AddElement(new UIElement("health_bar", "血条", "Slider"));
        gameHUD.AddElement(new UIElement("mana_bar", "蓝条", "Slider"));
        gameHUD.AddElement(new UIElement("mini_map", "小地图", "Image"));
        gameHUD.AddElement(new UIElement("skill_buttons", "技能按钮", "Button"));
        gameHUD.AddElement(new UIElement("chat_button", "聊天按钮", "Button"));
        uiData.AddWindow(gameHUD);
        
        // 聊天窗口
        UIWindow chat = new UIWindow("window_chat", "聊天", "UI/Chat", true);
        chat.AddElement(new UIElement("input_chat", "聊天输入框", "InputField"));
        chat.AddElement(new UIElement("btn_send", "发送按钮", "Button"));
        chat.AddElement(new UIElement("scroll_chat", "聊天滚动条", "ScrollView"));
        uiData.AddWindow(chat);
        
        // 背包窗口
        UIWindow inventory = new UIWindow("window_inventory", "背包", "UI/Inventory", true);
        inventory.AddElement(new UIElement("grid_items", "物品网格", "Grid"));
        inventory.AddElement(new UIElement("btn_use", "使用按钮", "Button"));
        inventory.AddElement(new UIElement("btn_sell", "出售按钮", "Button"));
        inventory.AddElement(new UIElement("btn_close", "关闭按钮", "Button"));
        uiData.AddWindow(inventory);
        
        // 商店窗口
        UIWindow shop = new UIWindow("window_shop", "商店", "UI/Shop", true);
        shop.AddElement(new UIElement("tab_items", "物品标签", "Button"));
        shop.AddElement(new UIElement("tab_heroes", "英雄标签", "Button"));
        shop.AddElement(new UIElement("tab_skins", "皮肤标签", "Button"));
        shop.AddElement(new UIElement("grid_shop_items", "商店物品网格", "Grid"));
        shop.AddElement(new UIElement("btn_buy", "购买按钮", "Button"));
        shop.AddElement(new UIElement("btn_close", "关闭按钮", "Button"));
        uiData.AddWindow(shop);
        
        SaveUIData();
    }
    
    public void ShowWindow(string windowID)
    {
        UIWindow window = uiData.GetWindow(windowID);
        if (window != null)
        {
            if (!windowInstances.ContainsKey(windowID) || windowInstances[windowID] == null)
            {
                // 加载窗口预制体
                GameObject windowPrefab = Resources.Load<GameObject>(window.windowPrefab);
                if (windowPrefab != null)
                {
                    GameObject windowInstance = Instantiate(windowPrefab, transform);
                    windowInstances[windowID] = windowInstance;
                }
            }
            
            if (windowInstances.ContainsKey(windowID) && windowInstances[windowID] != null)
            {
                windowInstances[windowID].SetActive(true);
            }
            
            uiData.ShowWindow(windowID);
            Debug.Log($"显示窗口: {window.windowName}");
        }
    }
    
    public void HideWindow(string windowID)
    {
        UIWindow window = uiData.GetWindow(windowID);
        if (window != null)
        {
            if (windowInstances.ContainsKey(windowID) && windowInstances[windowID] != null)
            {
                windowInstances[windowID].SetActive(false);
            }
            
            uiData.HideWindow(windowID);
            Debug.Log($"隐藏窗口: {window.windowName}");
        }
    }
    
    public void HideAllWindows()
    {
        foreach (KeyValuePair<string, GameObject> entry in windowInstances)
        {
            if (entry.Value != null)
            {
                UIWindow window = uiData.GetWindow(entry.Key);
                if (window != null && !window.isPersistent)
                {
                    entry.Value.SetActive(false);
                }
            }
        }
        
        uiData.HideAllWindows();
        Debug.Log("隐藏所有窗口");
    }
    
    public UIWindow GetWindow(string windowID)
    {
        return uiData.GetWindow(windowID);
    }
    
    public List<UIWindow> GetActiveWindows()
    {
        return uiData.GetActiveWindows();
    }
    
    public void UpdateUIElement(string windowID, string elementID, string value)
    {
        UIWindow window = uiData.GetWindow(windowID);
        if (window != null)
        {
            UIElement element = window.GetElement(elementID);
            if (element != null)
            {
                element.elementValue = value;
                SaveUIData();
            }
        }
    }
    
    public void EnableUIElement(string windowID, string elementID, bool enable)
    {
        UIWindow window = uiData.GetWindow(windowID);
        if (window != null)
        {
            UIElement element = window.GetElement(elementID);
            if (element != null)
            {
                if (enable)
                {
                    element.Enable();
                }
                else
                {
                    element.Disable();
                }
                SaveUIData();
            }
        }
    }
    
    public void ShowUIElement(string windowID, string elementID, bool show)
    {
        UIWindow window = uiData.GetWindow(windowID);
        if (window != null)
        {
            UIElement element = window.GetElement(elementID);
            if (element != null)
            {
                if (show)
                {
                    element.Show();
                }
                else
                {
                    element.Hide();
                }
                SaveUIData();
            }
        }
    }
    
    public void SaveUIData()
    {
        string path = Application.dataPath + "/Data/ui_data.dat";
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
        string path = Application.dataPath + "/Data/ui_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            uiData = (UIManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            uiData = new UIManagerData();
        }
    }
}
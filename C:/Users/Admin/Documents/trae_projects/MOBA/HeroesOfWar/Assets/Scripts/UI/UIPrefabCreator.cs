using UnityEngine;
using UnityEngine.UI;

public class UIPrefabCreator : MonoBehaviour
{
    public static void CreateUIPrefab()
    {
        // 创建UI根对象
        GameObject uiRoot = new GameObject("UIRoot");
        Canvas canvas = uiRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiRoot.AddComponent<CanvasScaler>();
        uiRoot.AddComponent<GraphicRaycaster>();
        
        // 创建主UI管理器
        GameObject mainUIManagerGO = new GameObject("MainUIManager");
        mainUIManagerGO.transform.SetParent(uiRoot.transform);
        MainUIManager mainUIManager = mainUIManagerGO.AddComponent<MainUIManager>();
        
        // 创建各个面板
        CreateMainMenuPanel(uiRoot, mainUIManager);
        CreateHeroSelectPanel(uiRoot, mainUIManager);
        CreateSocialPanel(uiRoot, mainUIManager);
        CreateInventoryPanel(uiRoot, mainUIManager);
        CreateLeaderboardPanel(uiRoot, mainUIManager);
        CreateQuestPanel(uiRoot, mainUIManager);
        CreateShopPanel(uiRoot, mainUIManager);
        CreateEventPanel(uiRoot, mainUIManager);
        CreateMailPanel(uiRoot, mainUIManager);
        CreateProfilePanel(uiRoot, mainUIManager);
        CreateSettingsPanel(uiRoot, mainUIManager);
        
        // 保存为预制体
        Debug.Log("UI预制体创建完成");
    }
    
    private static void CreateMainMenuPanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("MainMenuPanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        MainMenuPanel mainMenuPanel = panel.AddComponent<MainMenuPanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.mainMenuPanel = panel;
    }
    
    private static void CreateHeroSelectPanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("HeroSelectPanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        HeroSelectPanel heroSelectPanel = panel.AddComponent<HeroSelectPanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.heroSelectPanel = panel;
    }
    
    private static void CreateSocialPanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("SocialPanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        SocialPanel socialPanel = panel.AddComponent<SocialPanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.socialPanel = panel;
    }
    
    private static void CreateInventoryPanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("InventoryPanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        InventoryPanel inventoryPanel = panel.AddComponent<InventoryPanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.inventoryPanel = panel;
    }
    
    private static void CreateLeaderboardPanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("LeaderboardPanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        LeaderboardPanel leaderboardPanel = panel.AddComponent<LeaderboardPanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.leaderboardPanel = panel;
    }
    
    private static void CreateQuestPanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("QuestPanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        QuestPanel questPanel = panel.AddComponent<QuestPanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.questPanel = panel;
    }
    
    private static void CreateShopPanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("ShopPanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        ShopPanel shopPanel = panel.AddComponent<ShopPanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.shopPanel = panel;
    }
    
    private static void CreateEventPanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("EventPanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        EventPanel eventPanel = panel.AddComponent<EventPanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.eventPanel = panel;
    }
    
    private static void CreateMailPanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("MailPanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        MailPanel mailPanel = panel.AddComponent<MailPanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.mailPanel = panel;
    }
    
    private static void CreateProfilePanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("ProfilePanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        ProfilePanel profilePanel = panel.AddComponent<ProfilePanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.profilePanel = panel;
    }
    
    private static void CreateSettingsPanel(GameObject parent, MainUIManager manager)
    {
        GameObject panel = new GameObject("SettingsPanel");
        panel.transform.SetParent(parent.transform);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        SettingsPanel settingsPanel = panel.AddComponent<SettingsPanel>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        manager.settingsPanel = panel;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UISetupManager : MonoBehaviour
{
    public static void SetupUI()
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
        CreatePanel("MainMenu", mainUIManager, typeof(MainMenuPanel));
        CreatePanel("HeroSelect", mainUIManager, typeof(HeroSelectPanel));
        CreatePanel("Battle", mainUIManager, typeof(BattlePanel));
        CreatePanel("Results", mainUIManager, typeof(ResultsPanel));
        CreatePanel("Matching", mainUIManager, typeof(MatchingPanel));
        CreatePanel("Social", mainUIManager, typeof(SocialPanel));
        CreatePanel("Inventory", mainUIManager, typeof(InventoryPanel));
        CreatePanel("Settings", mainUIManager, typeof(SettingsPanel));
        CreatePanel("Leaderboard", mainUIManager, typeof(LeaderboardPanel));
        CreatePanel("Quest", mainUIManager, typeof(QuestPanel));
        CreatePanel("Shop", mainUIManager, typeof(ShopPanel));
        CreatePanel("Event", mainUIManager, typeof(EventPanel));
        CreatePanel("Mail", mainUIManager, typeof(MailPanel));
        CreatePanel("Profile", mainUIManager, typeof(ProfilePanel));
        
        // 创建游戏流程管理器
        GameObject gameFlowManagerGO = new GameObject("GameFlowManager");
        gameFlowManagerGO.AddComponent<GameFlowManager>();
        
        // 创建UI测试管理器
        GameObject uiTestManagerGO = new GameObject("UITestManager");
        uiTestManagerGO.AddComponent<UITestManager>();
        
        Debug.Log("UI设置完成");
    }
    
    private static void CreatePanel(string panelName, MainUIManager manager, System.Type panelType)
    {
        GameObject panel = new GameObject(panelName + "Panel");
        panel.transform.SetParent(manager.transform.parent);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        
        // 设置面板属性
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        // 添加面板脚本
        panel.AddComponent(panelType);
        
        // 设置管理器引用
        switch (panelName)
        {
            case "MainMenu":
                manager.mainMenuPanel = panel;
                break;
            case "HeroSelect":
                manager.heroSelectPanel = panel;
                break;
            case "Battle":
                manager.battlePanel = panel;
                break;
            case "Results":
                manager.resultsPanel = panel;
                break;
            case "Matching":
                manager.matchingPanel = panel;
                break;
            case "Social":
                manager.socialPanel = panel;
                break;
            case "Inventory":
                manager.inventoryPanel = panel;
                break;
            case "Settings":
                manager.settingsPanel = panel;
                break;
            case "Leaderboard":
                manager.leaderboardPanel = panel;
                break;
            case "Quest":
                manager.questPanel = panel;
                break;
            case "Shop":
                manager.shopPanel = panel;
                break;
            case "Event":
                manager.eventPanel = panel;
                break;
            case "Mail":
                manager.mailPanel = panel;
                break;
            case "Profile":
                manager.profilePanel = panel;
                break;
        }
    }
}

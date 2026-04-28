using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager Instance { get; private set; }
    
    private Dictionary<string, UIPanel> panels = new Dictionary<string, UIPanel>();
    
    [SerializeField] public GameObject mainMenuPanel;
    [SerializeField] public GameObject heroSelectPanel;
    [SerializeField] public GameObject battlePanel;
    [SerializeField] public GameObject resultsPanel;
    [SerializeField] public GameObject matchingPanel;
    [SerializeField] public GameObject socialPanel;
    [SerializeField] public GameObject inventoryPanel;
    [SerializeField] public GameObject settingsPanel;
    [SerializeField] public GameObject leaderboardPanel;
    [SerializeField] public GameObject questPanel;
    [SerializeField] public GameObject shopPanel;
    [SerializeField] public GameObject equipmentShopPanel;
    [SerializeField] public GameObject eventPanel;
    [SerializeField] public GameObject mailPanel;
    [SerializeField] public GameObject profilePanel;
    [SerializeField] public GameObject spectatorPanel;
    [SerializeField] public GameObject replayPanel;
    [SerializeField] public GameObject accountPanel;
    [SerializeField] public GameObject friendPanel;
    [SerializeField] public GameObject clanPanel;
    [SerializeField] public GameObject achievementPanel;
    [SerializeField] public GameObject serverSelectPanel;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePanels();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializePanels()
    {
        RegisterPanel("MainMenu", mainMenuPanel);
        RegisterPanel("HeroSelect", heroSelectPanel);
        RegisterPanel("Battle", battlePanel);
        RegisterPanel("Results", resultsPanel);
        RegisterPanel("Matching", matchingPanel);
        RegisterPanel("Social", socialPanel);
        RegisterPanel("Inventory", inventoryPanel);
        RegisterPanel("Settings", settingsPanel);
        RegisterPanel("Leaderboard", leaderboardPanel);
        RegisterPanel("Quest", questPanel);
        RegisterPanel("Shop", shopPanel);
        RegisterPanel("EquipmentShop", equipmentShopPanel);
        RegisterPanel("Event", eventPanel);
        RegisterPanel("Mail", mailPanel);
        RegisterPanel("Profile", profilePanel);
        RegisterPanel("Spectator", spectatorPanel);
        RegisterPanel("Replay", replayPanel);
        RegisterPanel("Account", accountPanel);
        RegisterPanel("Friend", friendPanel);
        RegisterPanel("Clan", clanPanel);
        RegisterPanel("Achievement", achievementPanel);
        RegisterPanel("ServerSelect", serverSelectPanel);
        
        ShowPanel("MainMenu");
    }
    
    private void RegisterPanel(string panelName, GameObject panel)
    {
        if (panel != null)
        {
            UIPanel uiPanel = panel.GetComponent<UIPanel>();
            if (uiPanel == null)
            {
                uiPanel = panel.AddComponent<UIPanel>();
            }
            uiPanel.PanelName = panelName;
            panels[panelName] = uiPanel;
            panel.SetActive(false);
        }
    }
    
    public void ShowPanel(string panelName)
    {
        foreach (var panel in panels.Values)
        {
            panel.gameObject.SetActive(false);
        }
        
        if (panels.ContainsKey(panelName))
        {
            panels[panelName].gameObject.SetActive(true);
            panels[panelName].OnShow();
        }
    }
    
    public void HideAllPanels()
    {
        foreach (var panel in panels.Values)
        {
            panel.gameObject.SetActive(false);
        }
    }
    
    public bool IsPanelActive(string panelName)
    {
        return panels.ContainsKey(panelName) && panels[panelName].gameObject.activeSelf;
    }
    
    public UIPanel GetPanel(string panelName)
    {
        if (panels.ContainsKey(panelName))
        {
            return panels[panelName];
        }
        return null;
    }
}

public class UIPanel : MonoBehaviour
{
    public string PanelName { get; set; }
    
    public virtual void OnShow()
    {
        // 面板显示时的逻辑
    }
    
    public virtual void OnHide()
    {
        // 面板隐藏时的逻辑
    }
}

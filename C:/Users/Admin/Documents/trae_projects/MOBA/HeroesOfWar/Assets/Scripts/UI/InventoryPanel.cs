using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryPanel : UIPanel
{
    [SerializeField] private TabButton itemsTab;
    [SerializeField] private TabButton equipmentTab;
    [SerializeField] private TabButton skinsTab;
    [SerializeField] private TabButton runesTab;
    
    [SerializeField] private GameObject itemsContent;
    [SerializeField] private GameObject equipmentContent;
    [SerializeField] private GameObject skinsContent;
    [SerializeField] private GameObject runesContent;
    
    [SerializeField] private Button backButton;
    
    private void Start()
    {
        itemsTab.onClick.AddListener(() => SwitchTab("Items"));
        equipmentTab.onClick.AddListener(() => SwitchTab("Equipment"));
        skinsTab.onClick.AddListener(() => SwitchTab("Skins"));
        runesTab.onClick.AddListener(() => SwitchTab("Runes"));
        backButton.onClick.AddListener(OnBack);
        
        SwitchTab("Items");
    }
    
    private void SwitchTab(string tabName)
    {
        itemsContent.SetActive(tabName == "Items");
        equipmentContent.SetActive(tabName == "Equipment");
        skinsContent.SetActive(tabName == "Skins");
        runesContent.SetActive(tabName == "Runes");
        
        itemsTab.SetSelected(tabName == "Items");
        equipmentTab.SetSelected(tabName == "Equipment");
        skinsTab.SetSelected(tabName == "Skins");
        runesTab.SetSelected(tabName == "Runes");
        
        if (tabName == "Items")
        {
            InitializeItems();
        }
        else if (tabName == "Equipment")
        {
            InitializeEquipment();
        }
        else if (tabName == "Skins")
        {
            InitializeSkins();
        }
        else if (tabName == "Runes")
        {
            InitializeRunes();
        }
    }
    
    private void InitializeItems()
    {
        // 从物品系统获取物品列表
        var inventoryManager = InventorySystemDetailedManager.Instance;
        var items = inventoryManager.GetItems("player_001");
        Debug.Log("物品列表数据: " + items.Count + " 个物品");
    }
    
    private void InitializeEquipment()
    {
        // 从装备系统获取装备列表
        var equipmentManager = EquipmentSystemDetailedManager.Instance;
        var equipment = equipmentManager.GetEquipmentList("player_001");
        Debug.Log("装备列表数据: " + equipment.Count + " 件装备");
    }
    
    private void InitializeSkins()
    {
        // 从皮肤系统获取皮肤列表
        var skinManager = SkinSystemDetailedManager.Instance;
        var skins = skinManager.GetPlayerSkins("player_001");
        Debug.Log("皮肤列表数据: " + skins.Count + " 个皮肤");
    }
    
    private void InitializeRunes()
    {
        // 从符文系统获取符文列表
        var runeManager = RuneSystemDetailedManager.Instance;
        var runes = runeManager.GetRunes("player_001");
        Debug.Log("符文列表数据: " + runes.Count + " 个符文");
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

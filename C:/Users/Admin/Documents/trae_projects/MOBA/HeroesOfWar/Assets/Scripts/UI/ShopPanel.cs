using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopPanel : UIPanel
{
    [SerializeField] private TabButton heroesTab;
    [SerializeField] private TabButton skinsTab;
    [SerializeField] private TabButton equipmentTab;
    [SerializeField] private TabButton itemsTab;
    
    [SerializeField] private GameObject heroesContent;
    [SerializeField] private GameObject skinsContent;
    [SerializeField] private GameObject equipmentContent;
    [SerializeField] private GameObject itemsContent;
    
    [SerializeField] private Button backButton;
    
    private void Start()
    {
        heroesTab.onClick.AddListener(() => SwitchTab("Heroes"));
        skinsTab.onClick.AddListener(() => SwitchTab("Skins"));
        equipmentTab.onClick.AddListener(() => SwitchTab("Equipment"));
        itemsTab.onClick.AddListener(() => SwitchTab("Items"));
        backButton.onClick.AddListener(OnBack);
        
        SwitchTab("Heroes");
    }
    
    private void SwitchTab(string tabName)
    {
        heroesContent.SetActive(tabName == "Heroes");
        skinsContent.SetActive(tabName == "Skins");
        equipmentContent.SetActive(tabName == "Equipment");
        itemsContent.SetActive(tabName == "Items");
        
        heroesTab.SetSelected(tabName == "Heroes");
        skinsTab.SetSelected(tabName == "Skins");
        equipmentTab.SetSelected(tabName == "Equipment");
        itemsTab.SetSelected(tabName == "Items");
        
        if (tabName == "Heroes")
        {
            InitializeHeroShop();
        }
        else if (tabName == "Skins")
        {
            InitializeSkinShop();
        }
        else if (tabName == "Equipment")
        {
            InitializeEquipmentShop();
        }
        else if (tabName == "Items")
        {
            InitializeItemShop();
        }
    }
    
    private void InitializeHeroShop()
    {
        // 这里应该从ShopSystem获取英雄购买数据
        Debug.Log("初始化英雄商店");
    }
    
    private void InitializeSkinShop()
    {
        // 这里应该从ShopSystem获取皮肤购买数据
        Debug.Log("初始化皮肤商店");
    }
    
    private void InitializeEquipmentShop()
    {
        // 这里应该从ShopSystem获取装备购买数据
        Debug.Log("初始化装备商店");
    }
    
    private void InitializeItemShop()
    {
        // 这里应该从ShopSystem获取物品购买数据
        Debug.Log("初始化物品商店");
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

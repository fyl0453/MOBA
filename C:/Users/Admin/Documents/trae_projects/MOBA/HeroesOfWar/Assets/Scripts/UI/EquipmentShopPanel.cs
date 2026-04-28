using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EquipmentShopPanel : UIPanel
{
    [SerializeField] private GameObject equipmentItemPrefab;
    [SerializeField] private Transform equipmentListContent;
    [SerializeField] private Button backButton;
    [SerializeField] private Text goldText;
    [SerializeField] private Text selectedEquipmentText;
    [SerializeField] private Text equipmentDescriptionText;
    [SerializeField] private Button buyButton;
    
    private List<EquipmentItem> equipmentItems = new List<EquipmentItem>();
    private Equipment selectedEquipment;
    
    private void Start()
    {
        backButton.onClick.AddListener(OnBack);
        buyButton.onClick.AddListener(OnBuy);
        
        InitializeEquipmentList();
        UpdateGoldDisplay();
    }
    
    private void InitializeEquipmentList()
    {
        // 清空现有列表
        foreach (var item in equipmentItems)
        {
            Destroy(item.gameObject);
        }
        equipmentItems.Clear();
        
        // 从EquipmentSystem获取装备列表
        if (EquipmentSystem.Instance != null)
        {
            List<Equipment> equipmentList = EquipmentSystem.Instance.GetAllEquipment();
            
            foreach (Equipment equipment in equipmentList)
            {
                GameObject equipmentItemGO = Instantiate(equipmentItemPrefab, equipmentListContent);
                EquipmentItem equipmentItem = equipmentItemGO.GetComponent<EquipmentItem>();
                if (equipmentItem != null)
                {
                    equipmentItem.Setup(equipment, OnEquipmentSelect);
                    equipmentItems.Add(equipmentItem);
                }
            }
        }
    }
    
    private void OnEquipmentSelect(Equipment equipment)
    {
        selectedEquipment = equipment;
        selectedEquipmentText.text = "选中: " + equipment.equipmentName;
        equipmentDescriptionText.text = equipment.description;
        
        // 检查是否可以购买
        int playerGold = EconomySystemDetailedManager.Instance.GetCurrencyBalance("player_001", "gold");
        buyButton.interactable = playerGold >= equipment.price;
    }
    
    private void OnBuy()
    {
        if (selectedEquipment != null)
        {
            // 检查金币是否足够
            int playerGold = EconomySystemDetailedManager.Instance.GetCurrencyBalance("player_001", "gold");
            if (playerGold >= selectedEquipment.price)
            {
                // 扣除金币
                EconomySystemDetailedManager.Instance.AddCurrency("player_001", "gold", -selectedEquipment.price);
                
                // 添加装备到背包
                // 这里需要实现背包系统的逻辑
                Debug.Log("购买了装备: " + selectedEquipment.equipmentName);
                
                // 更新金币显示
                UpdateGoldDisplay();
                
                // 重置选择
                selectedEquipment = null;
                selectedEquipmentText.text = "请选择装备";
                equipmentDescriptionText.text = "";
                buyButton.interactable = false;
            }
        }
    }
    
    private void UpdateGoldDisplay()
    {
        int playerGold = EconomySystemDetailedManager.Instance.GetCurrencyBalance("player_001", "gold");
        goldText.text = "金币: " + playerGold;
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

public class EquipmentItem : MonoBehaviour
{
    [SerializeField] private Text equipmentNameText;
    [SerializeField] private Text priceText;
    [SerializeField] private Button selectButton;
    
    private Equipment equipment;
    private System.Action<Equipment> onSelectCallback;
    
    public void Setup(Equipment eq, System.Action<Equipment> callback)
    {
        equipment = eq;
        equipmentNameText.text = eq.equipmentName;
        priceText.text = "价格: " + eq.price;
        onSelectCallback = callback;
        
        selectButton.onClick.AddListener(OnSelect);
    }
    
    private void OnSelect()
    {
        onSelectCallback?.Invoke(equipment);
    }
}

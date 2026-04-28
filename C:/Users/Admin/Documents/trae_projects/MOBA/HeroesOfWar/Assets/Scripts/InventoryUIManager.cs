using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    public Transform inventoryGrid;
    public Text itemNameText;
    public Text itemDescriptionText;
    public Text itemQuantityText;
    public Button useButton;
    public Button equipButton;
    public Button dropButton;
    
    private List<GameObject> itemSlots = new List<GameObject>();
    private Item selectedItem;
    
    private void Start()
    {
        if (useButton != null)
        {
            useButton.onClick.AddListener(UseSelectedItem);
        }
        if (equipButton != null)
        {
            equipButton.onClick.AddListener(EquipSelectedItem);
        }
        if (dropButton != null)
        {
            dropButton.onClick.AddListener(DropSelectedItem);
        }
        
        UpdateInventoryUI();
    }
    
    private void Update()
    {
        UpdateInventoryUI();
    }
    
    public void UpdateInventoryUI()
    {
        ClearInventoryGrid();
        List<Item> items = InventoryManager.Instance.GetInventoryItems();
        
        foreach (Item item in items)
        {
            CreateItemSlot(item);
        }
    }
    
    private void ClearInventoryGrid()
    {
        foreach (GameObject slot in itemSlots)
        {
            Destroy(slot);
        }
        itemSlots.Clear();
    }
    
    private void CreateItemSlot(Item item)
    {
        GameObject slot = new GameObject("ItemSlot");
        slot.transform.SetParent(inventoryGrid);
        
        Image slotImage = slot.AddComponent<Image>();
        slotImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        RectTransform rect = slot.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(60, 60);
        rect.localScale = Vector3.one;
        
        GameObject itemObj = new GameObject("Item");
        itemObj.transform.SetParent(slot.transform);
        
        Image itemImage = itemObj.AddComponent<Image>();
        // 这里应该设置物品图标，暂时使用颜色代替
        itemImage.color = GetItemColor(item);
        
        RectTransform itemRect = itemObj.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(50, 50);
        itemRect.localPosition = Vector3.zero;
        
        if (item.quantity > 1)
        {
            GameObject quantityObj = new GameObject("Quantity");
            quantityObj.transform.SetParent(slot.transform);
            
            Text quantityText = quantityObj.AddComponent<Text>();
            quantityText.text = item.quantity.ToString();
            quantityText.fontSize = 12;
            quantityText.alignment = TextAnchor.UpperRight;
            
            RectTransform quantityRect = quantityObj.GetComponent<RectTransform>();
            quantityRect.sizeDelta = new Vector2(20, 20);
            quantityRect.localPosition = new Vector3(25, 25, 0);
        }
        
        Button button = slot.AddComponent<Button>();
        button.onClick.AddListener(() => SelectItem(item));
        
        itemSlots.Add(slot);
    }
    
    private Color GetItemColor(Item item)
    {
        switch (item.itemType)
        {
            case "Consumable":
                return Color.green;
            case "Equipment":
                return Color.blue;
            case "Quest":
                return Color.yellow;
            case "Material":
                return Color.gray;
            default:
                return Color.white;
        }
    }
    
    private void SelectItem(Item item)
    {
        selectedItem = item;
        UpdateItemDetails();
    }
    
    private void UpdateItemDetails()
    {
        if (selectedItem != null)
        {
            if (itemNameText != null)
            {
                itemNameText.text = selectedItem.itemName;
            }
            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = selectedItem.description;
            }
            if (itemQuantityText != null)
            {
                itemQuantityText.text = "数量: " + selectedItem.quantity;
            }
            
            if (useButton != null)
            {
                useButton.gameObject.SetActive(selectedItem.isConsumable);
            }
            if (equipButton != null)
            {
                equipButton.gameObject.SetActive(selectedItem.isEquipable);
                bool isEquipped = InventoryManager.Instance.GetEquippedItems().Exists(i => i.itemID == selectedItem.itemID);
                equipButton.GetComponentInChildren<Text>().text = isEquipped ? "卸下" : "装备";
            }
            if (dropButton != null)
            {
                dropButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (itemNameText != null)
            {
                itemNameText.text = "";
            }
            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = "";
            }
            if (itemQuantityText != null)
            {
                itemQuantityText.text = "";
            }
            
            if (useButton != null)
            {
                useButton.gameObject.SetActive(false);
            }
            if (equipButton != null)
            {
                equipButton.gameObject.SetActive(false);
            }
            if (dropButton != null)
            {
                dropButton.gameObject.SetActive(false);
            }
        }
    }
    
    private void UseSelectedItem()
    {
        if (selectedItem != null && selectedItem.isConsumable)
        {
            InventoryManager.Instance.UseItem(selectedItem.itemID);
            UpdateInventoryUI();
            selectedItem = null;
            UpdateItemDetails();
        }
    }
    
    private void EquipSelectedItem()
    {
        if (selectedItem != null && selectedItem.isEquipable)
        {
            bool isEquipped = InventoryManager.Instance.GetEquippedItems().Exists(i => i.itemID == selectedItem.itemID);
            if (isEquipped)
            {
                InventoryManager.Instance.UnequipItem(selectedItem.itemID);
            }
            else
            {
                InventoryManager.Instance.EquipItem(selectedItem.itemID);
            }
            UpdateInventoryUI();
            UpdateItemDetails();
        }
    }
    
    private void DropSelectedItem()
    {
        if (selectedItem != null)
        {
            InventoryManager.Instance.RemoveItemFromInventory(selectedItem.itemID, 1);
            UpdateInventoryUI();
            if (selectedItem.quantity <= 1)
            {
                selectedItem = null;
            }
            UpdateItemDetails();
        }
    }
}
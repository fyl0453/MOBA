using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    
    public Canvas shopCanvas;
    public Text goldText;
    public ScrollRect itemList;
    public Transform contentPanel;
    public GameObject itemPrefab;
    
    public Text itemNameText;
    public Text itemDescriptionText;
    public Text itemPriceText;
    public Button buyButton;
    
    private List<Item> availableItems;
    private Item selectedItem;
    
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
        shopCanvas.gameObject.SetActive(false);
        availableItems = ItemManager.Instance.GetShopItems();
        InitializeItemList();
    }
    
    private void Update()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + PlayerStats.Instance.GetGold();
        }
    }
    
    private void InitializeItemList()
    {
        if (contentPanel == null || itemPrefab == null)
        {
            Debug.LogError("Content panel or item prefab not set");
            return;
        }
        
        // 清空现有内容
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        
        // 添加装备
        foreach (Item item in availableItems)
        {
            GameObject itemObj = Instantiate(itemPrefab, contentPanel);
            itemObj.name = item.itemName;
            
            // 设置装备信息
            Text[] texts = itemObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = item.itemName;
                texts[1].text = item.price.ToString();
            }
            
            // 添加点击事件
            Button button = itemObj.GetComponent<Button>();
            if (button != null)
            {
                Item currentItem = item;
                button.onClick.AddListener(() => SelectItem(currentItem));
            }
        }
    }
    
    private void SelectItem(Item item)
    {
        selectedItem = item;
        UpdateItemDetails();
    }
    
    private void UpdateItemDetails()
    {
        if (selectedItem == null)
        {
            return;
        }
        
        if (itemNameText != null)
        {
            itemNameText.text = selectedItem.itemName;
        }
        
        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = selectedItem.itemDescription;
        }
        
        if (itemPriceText != null)
        {
            itemPriceText.text = "Price: " + selectedItem.price;
        }
        
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => BuyItem());
        }
    }
    
    private void BuyItem()
    {
        if (selectedItem == null)
        {
            return;
        }
        
        int currentGold = PlayerStats.Instance.GetGold();
        if (currentGold >= selectedItem.price)
        {
            // 购买装备
            PlayerStats.Instance.RemoveGold(selectedItem.price);
            
            // 应用装备效果
            HeroController playerHero = FindObjectOfType<HeroController>();
            if (playerHero != null)
            {
                selectedItem.ApplyEffects(playerHero.heroData.baseStats);
            }
            
            Debug.Log("Purchased: " + selectedItem.itemName);
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }
    
    public void OpenShop()
    {
        shopCanvas.gameObject.SetActive(true);
        InitializeItemList();
    }
    
    public void CloseShop()
    {
        shopCanvas.gameObject.SetActive(false);
    }
    
    public bool IsShopOpen()
    {
        return shopCanvas.gameObject.activeSelf;
    }
}
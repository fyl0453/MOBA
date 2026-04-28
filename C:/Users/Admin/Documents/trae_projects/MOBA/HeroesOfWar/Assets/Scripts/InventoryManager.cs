using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    public Inventory playerInventory;
    public List<Item> allItems;
    public List<ItemShop> allShops;
    
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
        LoadInventoryData();
        LoadItemData();
        LoadShopData();
        
        if (playerInventory == null)
        {
            playerInventory = new Inventory("player_inventory", 30);
        }
        
        if (allItems.Count == 0)
        {
            InitializeDefaultItems();
        }
        
        if (allShops.Count == 0)
        {
            InitializeDefaultShops();
        }
    }
    
    private void InitializeDefaultItems()
    {
        Item healthPotion = new Item("item_health_potion", "生命药水", "恢复100点生命值", "Consumable");
        healthPotion.isConsumable = true;
        healthPotion.usageEffect = "Heal 100 HP";
        healthPotion.cooldown = 3;
        allItems.Add(healthPotion);
        
        Item manaPotion = new Item("item_mana_potion", "法力药水", "恢复80点法力值", "Consumable");
        manaPotion.isConsumable = true;
        manaPotion.usageEffect = "Restore 80 Mana";
        manaPotion.cooldown = 3;
        allItems.Add(manaPotion);
        
        Item attackPotion = new Item("item_attack_potion", "攻击药水", "增加20点攻击力，持续30秒", "Consumable");
        attackPotion.isConsumable = true;
        attackPotion.usageEffect = "+20 Attack for 30s";
        attackPotion.cooldown = 60;
        allItems.Add(attackPotion);
        
        Item defensePotion = new Item("item_defense_potion", "防御药水", "增加30点防御力，持续30秒", "Consumable");
        defensePotion.isConsumable = true;
        defensePotion.usageEffect = "+30 Defense for 30s";
        defensePotion.cooldown = 60;
        allItems.Add(defensePotion);
        
        Item speedPotion = new Item("item_speed_potion", "速度药水", "增加20%移动速度，持续20秒", "Consumable");
        speedPotion.isConsumable = true;
        speedPotion.usageEffect = "+20% Move Speed for 20s";
        speedPotion.cooldown = 45;
        allItems.Add(speedPotion);
        
        Item回城卷轴 = new Item("item_teleport_scroll", "回城卷轴", "立即返回基地", "Consumable");
       回城卷轴.isConsumable = true;
       回城卷轴.usageEffect = "Teleport to base";
       回城卷轴.cooldown = 60;
        allItems.Add(回城卷轴);
        
        Item visionWard = new Item("item_vision_ward", "视野守卫", "放置一个视野守卫，持续60秒", "Consumable");
        visionWard.isConsumable = true;
        visionWard.usageEffect = "Place vision ward for 60s";
        visionWard.cooldown = 90;
        allItems.Add(visionWard);
        
        Item ward = new Item("item_ward", "守卫", "放置一个守卫，持续30秒", "Consumable");
        ward.isConsumable = true;
        ward.usageEffect = "Place ward for 30s";
        ward.cooldown = 60;
        allItems.Add(ward);
        
        SaveItemData();
    }
    
    private void InitializeDefaultShops()
    {
        ItemShop basicShop = new ItemShop("shop_basic", "基础商店", "Basic");
        basicShop.itemsForSale.Add(new ShopItem("shop_health_potion", "item_health_potion", 50, "Gold"));
        basicShop.itemsForSale.Add(new ShopItem("shop_mana_potion", "item_mana_potion", 30, "Gold"));
        basicShop.itemsForSale.Add(new ShopItem("shop_attack_potion", "item_attack_potion", 100, "Gold"));
        basicShop.itemsForSale.Add(new ShopItem("shop_defense_potion", "item_defense_potion", 100, "Gold"));
        allShops.Add(basicShop);
        
        ItemShop utilityShop = new ItemShop("shop_utility", "辅助商店", "Utility");
        utilityShop.itemsForSale.Add(new ShopItem("shop_speed_potion", "item_speed_potion", 150, "Gold"));
        utilityShop.itemsForSale.Add(new ShopItem("shop_teleport_scroll", "item_teleport_scroll", 200, "Gold"));
        utilityShop.itemsForSale.Add(new ShopItem("shop_vision_ward", "item_vision_ward", 75, "Gold"));
        utilityShop.itemsForSale.Add(new ShopItem("shop_ward", "item_ward", 50, "Gold"));
        allShops.Add(utilityShop);
        
        SaveShopData();
    }
    
    public bool AddItemToInventory(string itemID, int quantity = 1)
    {
        Item itemTemplate = allItems.Find(i => i.itemID == itemID);
        if (itemTemplate == null)
        {
            return false;
        }
        
        Item newItem = new Item(itemTemplate.itemID, itemTemplate.itemName, itemTemplate.description, itemTemplate.itemType);
        newItem.quantity = quantity;
        newItem.isStackable = itemTemplate.isStackable;
        newItem.isConsumable = itemTemplate.isConsumable;
        newItem.isEquipable = itemTemplate.isEquipable;
        newItem.maxStack = itemTemplate.maxStack;
        newItem.icon = itemTemplate.icon;
        newItem.usageEffect = itemTemplate.usageEffect;
        newItem.cooldown = itemTemplate.cooldown;
        
        bool success = playerInventory.AddItem(newItem);
        if (success)
        {
            SaveInventoryData();
        }
        return success;
    }
    
    public bool RemoveItemFromInventory(string itemID, int quantity = 1)
    {
        bool success = playerInventory.RemoveItem(itemID, quantity);
        if (success)
        {
            SaveInventoryData();
        }
        return success;
    }
    
    public bool UseItem(string itemID)
    {
        Item item = playerInventory.GetItem(itemID);
        if (item != null && item.isConsumable)
        {
            ApplyItemEffect(item);
            RemoveItemFromInventory(itemID, 1);
            return true;
        }
        return false;
    }
    
    private void ApplyItemEffect(Item item)
    {
        switch (item.usageEffect)
        {
            case "Heal 100 HP":
                PlayerStats playerStats = GameObject.FindObjectOfType<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.health += 100;
                    if (playerStats.health > playerStats.maxHealth)
                    {
                        playerStats.health = playerStats.maxHealth;
                    }
                }
                break;
            case "Restore 80 Mana":
                playerStats = GameObject.FindObjectOfType<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.mana += 80;
                    if (playerStats.mana > playerStats.maxMana)
                    {
                        playerStats.mana = playerStats.maxMana;
                    }
                }
                break;
            case "Teleport to base":
                Player player = GameObject.FindObjectOfType<Player>();
                if (player != null)
                {
                    player.TeleportToBase();
                }
                break;
        }
    }
    
    public bool EquipItem(string itemID)
    {
        bool success = playerInventory.EquipItem(itemID);
        if (success)
        {
            SaveInventoryData();
        }
        return success;
    }
    
    public bool UnequipItem(string itemID)
    {
        bool success = playerInventory.UnequipItem(itemID);
        if (success)
        {
            SaveInventoryData();
        }
        return success;
    }
    
    public bool BuyItemFromShop(string shopID, string itemID, int quantity = 1)
    {
        ItemShop shop = allShops.Find(s => s.shopID == shopID);
        if (shop == null)
        {
            return false;
        }
        
        ShopItem shopItem = shop.itemsForSale.Find(si => si.itemID == itemID);
        if (shopItem == null)
        {
            return false;
        }
        
        int totalPrice = (int)(shopItem.price * shopItem.discount * quantity);
        int playerGold = ProfileManager.Instance.currentProfile.gold;
        if (playerGold < totalPrice)
        {
            return false;
        }
        
        if (shopItem.isLimited && shopItem.stock < quantity)
        {
            return false;
        }
        
        bool added = AddItemToInventory(itemID, quantity);
        if (added)
        {
            ProfileManager.Instance.currentProfile.gold -= totalPrice;
            if (shopItem.isLimited)
            {
                shopItem.stock -= quantity;
            }
            SaveInventoryData();
            ProfileManager.Instance.SaveProfile();
            SaveShopData();
            return true;
        }
        
        return false;
    }
    
    public List<Item> GetInventoryItems()
    {
        return playerInventory.items;
    }
    
    public List<Item> GetEquippedItems()
    {
        return playerInventory.equippedItems;
    }
    
    public Item GetItemByID(string itemID)
    {
        return allItems.Find(i => i.itemID == itemID);
    }
    
    public ItemShop GetShopByID(string shopID)
    {
        return allShops.Find(s => s.shopID == shopID);
    }
    
    public int GetItemCount(string itemID)
    {
        return playerInventory.GetItemCount(itemID);
    }
    
    public bool HasItem(string itemID, int amount = 1)
    {
        return playerInventory.HasItem(itemID, amount);
    }
    
    public void SaveInventoryData()
    {
        string path = Application.dataPath + "/Data/inventory_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, playerInventory);
        stream.Close();
    }
    
    public void LoadInventoryData()
    {
        string path = Application.dataPath + "/Data/inventory_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            playerInventory = (Inventory)formatter.Deserialize(stream);
            stream.Close();
        }
    }
    
    public void SaveItemData()
    {
        string path = Application.dataPath + "/Data/item_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, allItems);
        stream.Close();
    }
    
    public void LoadItemData()
    {
        string path = Application.dataPath + "/Data/item_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            allItems = (List<Item>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            allItems = new List<Item>();
        }
    }
    
    public void SaveShopData()
    {
        string path = Application.dataPath + "/Data/shop_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, allShops);
        stream.Close();
    }
    
    public void LoadShopData()
    {
        string path = Application.dataPath + "/Data/shop_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            allShops = (List<ItemShop>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            allShops = new List<ItemShop>();
        }
    }
}
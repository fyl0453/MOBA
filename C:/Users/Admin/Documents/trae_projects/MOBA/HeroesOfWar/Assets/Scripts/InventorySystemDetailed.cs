[System.Serializable]
public class InventorySystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Item> items;
    public List<PlayerInventory> playerInventories;
    public List<ItemCategory> categories;
    public List<ItemEffect> itemEffects;
    public List<InventoryEvent> inventoryEvents;
    
    public InventorySystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        items = new List<Item>();
        playerInventories = new List<PlayerInventory>();
        categories = new List<ItemCategory>();
        itemEffects = new List<ItemEffect>();
        inventoryEvents = new List<InventoryEvent>();
    }
    
    public void AddItem(Item item)
    {
        items.Add(item);
    }
    
    public void AddPlayerInventory(PlayerInventory playerInventory)
    {
        playerInventories.Add(playerInventory);
    }
    
    public void AddCategory(ItemCategory category)
    {
        categories.Add(category);
    }
    
    public void AddItemEffect(ItemEffect itemEffect)
    {
        itemEffects.Add(itemEffect);
    }
    
    public void AddInventoryEvent(InventoryEvent inventoryEvent)
    {
        inventoryEvents.Add(inventoryEvent);
    }
    
    public Item GetItem(string itemID)
    {
        return items.Find(i => i.itemID == itemID);
    }
    
    public PlayerInventory GetPlayerInventory(string playerInventoryID)
    {
        return playerInventories.Find(pi => pi.playerInventoryID == playerInventoryID);
    }
    
    public ItemCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public ItemEffect GetItemEffect(string effectID)
    {
        return itemEffects.Find(e => e.effectID == effectID);
    }
    
    public InventoryEvent GetInventoryEvent(string eventID)
    {
        return inventoryEvents.Find(e => e.eventID == eventID);
    }
    
    public List<Item> GetItemsByCategory(string categoryID)
    {
        return items.FindAll(i => i.categoryID == categoryID);
    }
    
    public List<PlayerInventory> GetPlayerInventoriesByUser(string userID)
    {
        return playerInventories.FindAll(pi => pi.userID == userID);
    }
    
    public List<Item> GetItemsByRarity(string rarity)
    {
        return items.FindAll(i => i.rarity == rarity);
    }
    
    public List<ItemEffect> GetItemEffectsByType(string effectType)
    {
        return itemEffects.FindAll(e => e.effectType == effectType);
    }
}

[System.Serializable]
public class Item
{
    public string itemID;
    public string itemName;
    public string itemDescription;
    public string categoryID;
    public string rarity;
    public string itemType;
    public string icon;
    public string model;
    public float price;
    public string priceCurrency;
    public int maxStack;
    public bool isConsumable;
    public bool isEquippable;
    public bool isTradeable;
    public bool isSellable;
    public bool isEnabled;
    public List<string> effectIDs;
    public string usageDescription;
    public string cooldown;
    
    public Item(string id, string name, string description, string categoryID, string rarity, string itemType, string icon, float price, string priceCurrency, int maxStack, bool isConsumable, bool isEquippable, bool isTradeable, bool isSellable)
    {
        itemID = id;
        itemName = name;
        itemDescription = description;
        this.categoryID = categoryID;
        this.rarity = rarity;
        this.itemType = itemType;
        this.icon = icon;
        model = "";
        this.price = price;
        this.priceCurrency = priceCurrency;
        this.maxStack = maxStack;
        this.isConsumable = isConsumable;
        this.isEquippable = isEquippable;
        this.isTradeable = isTradeable;
        this.isSellable = isSellable;
        isEnabled = true;
        effectIDs = new List<string>();
        usageDescription = "";
        cooldown = "";
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public void AddEffect(string effectID)
    {
        if (!effectIDs.Contains(effectID))
        {
            effectIDs.Add(effectID);
        }
    }
    
    public void RemoveEffect(string effectID)
    {
        if (effectIDs.Contains(effectID))
        {
            effectIDs.Remove(effectID);
        }
    }
}

[System.Serializable]
public class PlayerInventory
{
    public string playerInventoryID;
    public string userID;
    public string inventoryName;
    public int maxSlots;
    public int currentSlots;
    public List<InventoryItem> inventoryItems;
    public string lastUpdateTime;
    
    public PlayerInventory(string id, string userID, string inventoryName, int maxSlots)
    {
        playerInventoryID = id;
        this.userID = userID;
        this.inventoryName = inventoryName;
        this.maxSlots = maxSlots;
        currentSlots = 0;
        inventoryItems = new List<InventoryItem>();
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddItem(InventoryItem item)
    {
        inventoryItems.Add(item);
        currentSlots++;
        lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void RemoveItem(string inventoryItemID)
    {
        InventoryItem item = inventoryItems.Find(i => i.inventoryItemID == inventoryItemID);
        if (item != null)
        {
            inventoryItems.Remove(item);
            currentSlots--;
            lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
    
    public void UpdateItemQuantity(string inventoryItemID, int quantity)
    {
        InventoryItem item = inventoryItems.Find(i => i.inventoryItemID == inventoryItemID);
        if (item != null)
        {
            item.UpdateQuantity(quantity);
            lastUpdateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
    
    public InventoryItem GetItem(string inventoryItemID)
    {
        return inventoryItems.Find(i => i.inventoryItemID == inventoryItemID);
    }
    
    public InventoryItem GetItemByItemID(string itemID)
    {
        return inventoryItems.Find(i => i.itemID == itemID);
    }
}

[System.Serializable]
public class InventoryItem
{
    public string inventoryItemID;
    public string itemID;
    public int quantity;
    public bool isEquipped;
    public string equipSlot;
    public string obtainTime;
    public string lastUseTime;
    public Dictionary<string, string> metadata;
    
    public InventoryItem(string id, string itemID, int quantity)
    {
        inventoryItemID = id;
        this.itemID = itemID;
        this.quantity = quantity;
        isEquipped = false;
        equipSlot = "";
        obtainTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastUseTime = "";
        metadata = new Dictionary<string, string>();
    }
    
    public void UpdateQuantity(int quantity)
    {
        this.quantity = quantity;
    }
    
    public void Equip(string equipSlot)
    {
        isEquipped = true;
        this.equipSlot = equipSlot;
    }
    
    public void Unequip()
    {
        isEquipped = false;
        equipSlot = "";
    }
    
    public void Use()
    {
        lastUseTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddMetadata(string key, string value)
    {
        if (metadata.ContainsKey(key))
        {
            metadata[key] = value;
        }
        else
        {
            metadata.Add(key, value);
        }
    }
    
    public void RemoveMetadata(string key)
    {
        if (metadata.ContainsKey(key))
        {
            metadata.Remove(key);
        }
    }
}

[System.Serializable]
public class ItemCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    public string icon;
    public int order;
    public bool isEnabled;
    
    public ItemCategory(string id, string name, string description, string icon, int order)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = description;
        this.icon = icon;
        this.order = order;
        isEnabled = true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public void SetOrder(int order)
    {
        this.order = order;
    }
}

[System.Serializable]
public class ItemEffect
{
    public string effectID;
    public string effectName;
    public string effectDescription;
    public string effectType;
    public string targetType;
    public float effectValue;
    public string duration;
    public string cooldown;
    public bool isStackable;
    public bool isEnabled;
    
    public ItemEffect(string id, string name, string description, string effectType, string targetType, float effectValue, string duration, string cooldown, bool isStackable)
    {
        effectID = id;
        effectName = name;
        effectDescription = description;
        this.effectType = effectType;
        this.targetType = targetType;
        this.effectValue = effectValue;
        this.duration = duration;
        this.cooldown = cooldown;
        this.isStackable = isStackable;
        isEnabled = true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public void SetEffectValue(float effectValue)
    {
        this.effectValue = effectValue;
    }
}

[System.Serializable]
public class InventoryEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string itemID;
    public string description;
    public string timestamp;
    public string status;
    public int quantity;
    
    public InventoryEvent(string id, string eventType, string userID, string itemID, string description, int quantity)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.itemID = itemID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
        this.quantity = quantity;
    }
    
    public void MarkAsCompleted()
    {
        status = "completed";
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class InventorySystemDetailedManagerData
{
    public InventorySystemDetailed system;
    
    public InventorySystemDetailedManagerData()
    {
        system = new InventorySystemDetailed("inventory_system_detailed", "背包系统详细", "管理背包的详细功能，包括物品管理、分类和使用");
    }
}
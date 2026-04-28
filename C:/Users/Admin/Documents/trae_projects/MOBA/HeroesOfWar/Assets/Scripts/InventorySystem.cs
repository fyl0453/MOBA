[System.Serializable]
public class Item
{
    public string itemID;
    public string itemName;
    public string description;
    public string itemType;
    public int quantity;
    public bool isStackable;
    public bool isConsumable;
    public bool isEquipable;
    public int maxStack;
    public string icon;
    public string usageEffect;
    public int cooldown;
    
    public Item(string id, string name, string desc, string type)
    {
        itemID = id;
        itemName = name;
        description = desc;
        itemType = type;
        quantity = 1;
        isStackable = true;
        isConsumable = false;
        isEquipable = false;
        maxStack = 99;
        icon = "";
        usageEffect = "";
        cooldown = 0;
    }
}

[System.Serializable]
public class Inventory
{
    public string inventoryID;
    public int capacity;
    public List<Item> items;
    public List<Item> equippedItems;
    
    public Inventory(string id, int cap)
    {
        inventoryID = id;
        capacity = cap;
        items = new List<Item>();
        equippedItems = new List<Item>();
    }
    
    public bool AddItem(Item item)
    {
        if (items.Count >= capacity)
        {
            return false;
        }
        
        Item existingItem = items.Find(i => i.itemID == item.itemID && i.isStackable);
        if (existingItem != null)
        {
            existingItem.quantity += item.quantity;
            if (existingItem.quantity > existingItem.maxStack)
            {
                existingItem.quantity = existingItem.maxStack;
                return false;
            }
        }
        else
        {
            items.Add(item);
        }
        
        return true;
    }
    
    public bool RemoveItem(string itemID, int amount = 1)
    {
        Item item = items.Find(i => i.itemID == itemID);
        if (item != null)
        {
            item.quantity -= amount;
            if (item.quantity <= 0)
            {
                items.Remove(item);
            }
            return true;
        }
        return false;
    }
    
    public bool EquipItem(string itemID)
    {
        Item item = items.Find(i => i.itemID == itemID);
        if (item != null && item.isEquipable)
        {
            equippedItems.Add(item);
            items.Remove(item);
            return true;
        }
        return false;
    }
    
    public bool UnequipItem(string itemID)
    {
        Item item = equippedItems.Find(i => i.itemID == itemID);
        if (item != null)
        {
            if (AddItem(item))
            {
                equippedItems.Remove(item);
                return true;
            }
        }
        return false;
    }
    
    public Item GetItem(string itemID)
    {
        return items.Find(i => i.itemID == itemID);
    }
    
    public int GetItemCount(string itemID)
    {
        Item item = items.Find(i => i.itemID == itemID);
        return item != null ? item.quantity : 0;
    }
    
    public bool HasItem(string itemID, int amount = 1)
    {
        int count = GetItemCount(itemID);
        return count >= amount;
    }
}

[System.Serializable]
public class ItemUsage
{
    public string usageID;
    public string itemID;
    public string playerID;
    public float timestamp;
    public string effectResult;
    
    public ItemUsage(string id, string item, string player, string result)
    {
        usageID = id;
        itemID = item;
        playerID = player;
        timestamp = Time.time;
        effectResult = result;
    }
}

[System.Serializable]
public class ItemShop
{
    public string shopID;
    public string shopName;
    public List<ShopItem> itemsForSale;
    public string shopType;
    public bool isOpen;
    
    public ItemShop(string id, string name, string type)
    {
        shopID = id;
        shopName = name;
        itemsForSale = new List<ShopItem>();
        shopType = type;
        isOpen = true;
    }
}

[System.Serializable]
public class ShopItem
{
    public string shopItemID;
    public string itemID;
    public int price;
    public string currencyType;
    public int stock;
    public bool isLimited;
    public float discount;
    
    public ShopItem(string id, string item, int p, string currency)
    {
        shopItemID = id;
        itemID = item;
        price = p;
        currencyType = currency;
        stock = 999;
        isLimited = false;
        discount = 1.0f;
    }
}
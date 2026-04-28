using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class InventorySystemDetailedManager : MonoBehaviour
{
    public static InventorySystemDetailedManager Instance { get; private set; }
    
    public InventorySystemDetailedManagerData inventoryData;
    
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
        
        if (inventoryData == null)
        {
            inventoryData = new InventorySystemDetailedManagerData();
            InitializeDefaultInventorySystem();
        }
    }
    
    private void InitializeDefaultInventorySystem()
    {
        // 物品分类
        ItemCategory category1 = new ItemCategory("category_001", "消耗品", "可消耗的物品", "icon_consumable", 1);
        ItemCategory category2 = new ItemCategory("category_002", "装备", "可装备的物品", "icon_equipment", 2);
        ItemCategory category3 = new ItemCategory("category_003", "碎片", "英雄和皮肤碎片", "icon_fragment", 3);
        ItemCategory category4 = new ItemCategory("category_004", "货币", "游戏货币", "icon_currency", 4);
        ItemCategory category5 = new ItemCategory("category_005", "其他", "其他类型的物品", "icon_other", 5);
        
        inventoryData.system.AddCategory(category1);
        inventoryData.system.AddCategory(category2);
        inventoryData.system.AddCategory(category3);
        inventoryData.system.AddCategory(category4);
        inventoryData.system.AddCategory(category5);
        
        // 物品效果
        ItemEffect effect1 = new ItemEffect("effect_001", "生命恢复", "恢复生命值", "heal", "player", 100, "0", "0", false);
        ItemEffect effect2 = new ItemEffect("effect_002", "法力恢复", "恢复法力值", "mana", "player", 50, "0", "0", false);
        ItemEffect effect3 = new ItemEffect("effect_003", "攻击力提升", "提升攻击力", "attack", "player", 10, "60s", "0", true);
        ItemEffect effect4 = new ItemEffect("effect_004", "防御力提升", "提升防御力", "defense", "player", 5, "60s", "0", true);
        ItemEffect effect5 = new ItemEffect("effect_005", "经验增加", "增加经验值", "exp", "player", 100, "0", "0", false);
        
        inventoryData.system.AddItemEffect(effect1);
        inventoryData.system.AddItemEffect(effect2);
        inventoryData.system.AddItemEffect(effect3);
        inventoryData.system.AddItemEffect(effect4);
        inventoryData.system.AddItemEffect(effect5);
        
        // 物品
        Item item1 = new Item("item_001", "生命药水", "恢复100点生命值", "category_001", "common", "consumable", "icon_health_potion", 50, "gold", 99, true, false, true, true);
        item1.AddEffect("effect_001");
        
        Item item2 = new Item("item_002", "法力药水", "恢复50点法力值", "category_001", "common", "consumable", "icon_mana_potion", 30, "gold", 99, true, false, true, true);
        item2.AddEffect("effect_002");
        
        Item item3 = new Item("item_003", "攻击药水", "60秒内提升10点攻击力", "category_001", "uncommon", "consumable", "icon_attack_potion", 100, "gold", 99, true, false, true, true);
        item3.AddEffect("effect_003");
        
        Item item4 = new Item("item_004", "防御药水", "60秒内提升5点防御力", "category_001", "uncommon", "consumable", "icon_defense_potion", 100, "gold", 99, true, false, true, true);
        item4.AddEffect("effect_004");
        
        Item item5 = new Item("item_005", "经验药水", "增加100点经验值", "category_001", "rare", "consumable", "icon_exp_potion", 200, "gold", 99, true, false, true, true);
        item5.AddEffect("effect_005");
        
        Item item6 = new Item("item_006", "英雄碎片", "用于兑换英雄", "category_003", "common", "fragment", "icon_hero_fragment", 10, "gold", 99, false, false, true, true);
        
        Item item7 = new Item("item_007", "皮肤碎片", "用于兑换皮肤", "category_003", "uncommon", "fragment", "icon_skin_fragment", 20, "gold", 99, false, false, true, true);
        
        Item item8 = new Item("item_008", "金币", "游戏货币", "category_004", "common", "currency", "icon_gold", 0, "", 999999, false, false, false, false);
        
        Item item9 = new Item("item_009", "钻石", "高级游戏货币", "category_004", "rare", "currency", "icon_diamond", 0, "", 999999, false, false, false, false);
        
        Item item10 = new Item("item_010", "回城特效", "自定义回城特效", "category_005", "epic", "special", "icon_recall_effect", 500, "diamond", 1, false, true, true, true);
        
        inventoryData.system.AddItem(item1);
        inventoryData.system.AddItem(item2);
        inventoryData.system.AddItem(item3);
        inventoryData.system.AddItem(item4);
        inventoryData.system.AddItem(item5);
        inventoryData.system.AddItem(item6);
        inventoryData.system.AddItem(item7);
        inventoryData.system.AddItem(item8);
        inventoryData.system.AddItem(item9);
        inventoryData.system.AddItem(item10);
        
        // 玩家背包
        PlayerInventory playerInventory1 = new PlayerInventory("player_inventory_001", "user_001", "主背包", 50);
        PlayerInventory playerInventory2 = new PlayerInventory("player_inventory_002", "user_002", "主背包", 50);
        
        // 玩家物品
        InventoryItem inventoryItem1 = new InventoryItem("inventory_item_001", "item_001", 10);
        InventoryItem inventoryItem2 = new InventoryItem("inventory_item_002", "item_002", 5);
        InventoryItem inventoryItem3 = new InventoryItem("inventory_item_003", "item_006", 20);
        InventoryItem inventoryItem4 = new InventoryItem("inventory_item_004", "item_007", 10);
        InventoryItem inventoryItem5 = new InventoryItem("inventory_item_005", "item_008", 1000);
        
        playerInventory1.AddItem(inventoryItem1);
        playerInventory1.AddItem(inventoryItem2);
        playerInventory1.AddItem(inventoryItem3);
        playerInventory1.AddItem(inventoryItem4);
        playerInventory1.AddItem(inventoryItem5);
        
        InventoryItem inventoryItem6 = new InventoryItem("inventory_item_006", "item_001", 5);
        InventoryItem inventoryItem7 = new InventoryItem("inventory_item_007", "item_003", 2);
        InventoryItem inventoryItem8 = new InventoryItem("inventory_item_008", "item_006", 15);
        InventoryItem inventoryItem9 = new InventoryItem("inventory_item_009", "item_009", 100);
        
        playerInventory2.AddItem(inventoryItem6);
        playerInventory2.AddItem(inventoryItem7);
        playerInventory2.AddItem(inventoryItem8);
        playerInventory2.AddItem(inventoryItem9);
        
        inventoryData.system.AddPlayerInventory(playerInventory1);
        inventoryData.system.AddPlayerInventory(playerInventory2);
        
        // 背包事件
        InventoryEvent event1 = new InventoryEvent("event_001", "add", "user_001", "item_001", "添加生命药水", 10);
        InventoryEvent event2 = new InventoryEvent("event_002", "use", "user_001", "item_001", "使用生命药水", 1);
        InventoryEvent event3 = new InventoryEvent("event_003", "remove", "user_001", "item_002", "移除法力药水", 1);
        
        inventoryData.system.AddInventoryEvent(event1);
        inventoryData.system.AddInventoryEvent(event2);
        inventoryData.system.AddInventoryEvent(event3);
        
        SaveInventoryData();
    }
    
    // 物品分类管理
    public void AddCategory(string name, string description, string icon, int order)
    {
        string categoryID = "category_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ItemCategory category = new ItemCategory(categoryID, name, description, icon, order);
        inventoryData.system.AddCategory(category);
        SaveInventoryData();
        Debug.Log("成功添加物品分类: " + name);
    }
    
    public List<ItemCategory> GetAllCategories()
    {
        return inventoryData.system.categories;
    }
    
    public void EnableCategory(string categoryID)
    {
        ItemCategory category = inventoryData.system.GetCategory(categoryID);
        if (category != null)
        {
            category.Enable();
            SaveInventoryData();
            Debug.Log("成功启用物品分类: " + category.categoryName);
        }
        else
        {
            Debug.LogError("物品分类不存在: " + categoryID);
        }
    }
    
    public void DisableCategory(string categoryID)
    {
        ItemCategory category = inventoryData.system.GetCategory(categoryID);
        if (category != null)
        {
            category.Disable();
            SaveInventoryData();
            Debug.Log("成功禁用物品分类: " + category.categoryName);
        }
        else
        {
            Debug.LogError("物品分类不存在: " + categoryID);
        }
    }
    
    // 物品效果管理
    public void AddItemEffect(string name, string description, string effectType, string targetType, float effectValue, string duration, string cooldown, bool isStackable)
    {
        string effectID = "effect_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ItemEffect itemEffect = new ItemEffect(effectID, name, description, effectType, targetType, effectValue, duration, cooldown, isStackable);
        inventoryData.system.AddItemEffect(itemEffect);
        SaveInventoryData();
        Debug.Log("成功添加物品效果: " + name);
    }
    
    public List<ItemEffect> GetItemEffectsByType(string effectType)
    {
        return inventoryData.system.GetItemEffectsByType(effectType);
    }
    
    public List<ItemEffect> GetAllItemEffects()
    {
        return inventoryData.system.itemEffects;
    }
    
    // 物品管理
    public void AddItem(string name, string description, string categoryID, string rarity, string itemType, string icon, float price, string priceCurrency, int maxStack, bool isConsumable, bool isEquippable, bool isTradeable, bool isSellable)
    {
        string itemID = "item_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Item item = new Item(itemID, name, description, categoryID, rarity, itemType, icon, price, priceCurrency, maxStack, isConsumable, isEquippable, isTradeable, isSellable);
        inventoryData.system.AddItem(item);
        SaveInventoryData();
        Debug.Log("成功添加物品: " + name);
    }
    
    public void AddEffectToItem(string itemID, string effectID)
    {
        Item item = inventoryData.system.GetItem(itemID);
        if (item != null)
        {
            item.AddEffect(effectID);
            SaveInventoryData();
            Debug.Log("成功添加效果到物品: " + item.itemName);
        }
        else
        {
            Debug.LogError("物品不存在: " + itemID);
        }
    }
    
    public void RemoveEffectFromItem(string itemID, string effectID)
    {
        Item item = inventoryData.system.GetItem(itemID);
        if (item != null)
        {
            item.RemoveEffect(effectID);
            SaveInventoryData();
            Debug.Log("成功从物品移除效果: " + item.itemName);
        }
        else
        {
            Debug.LogError("物品不存在: " + itemID);
        }
    }
    
    public List<Item> GetItemsByCategory(string categoryID)
    {
        return inventoryData.system.GetItemsByCategory(categoryID);
    }
    
    public List<Item> GetAllItems()
    {
        return inventoryData.system.items;
    }
    
    // 玩家背包管理
    public void AddPlayerInventory(string userID, string inventoryName, int maxSlots)
    {
        string playerInventoryID = "player_inventory_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PlayerInventory playerInventory = new PlayerInventory(playerInventoryID, userID, inventoryName, maxSlots);
        inventoryData.system.AddPlayerInventory(playerInventory);
        SaveInventoryData();
        Debug.Log("成功添加玩家背包: " + inventoryName);
    }
    
    public PlayerInventory GetPlayerInventory(string userID, string inventoryName = "主背包")
    {
        List<PlayerInventory> playerInventories = inventoryData.system.GetPlayerInventoriesByUser(userID);
        return playerInventories.Find(pi => pi.inventoryName == inventoryName);
    }
    
    // 物品操作
    public void AddItemToInventory(string userID, string itemID, int quantity)
    {
        PlayerInventory playerInventory = GetPlayerInventory(userID);
        if (playerInventory != null)
        {
            Item item = inventoryData.system.GetItem(itemID);
            if (item != null && item.isEnabled)
            {
                // 检查是否已有该物品
                InventoryItem existingItem = playerInventory.GetItemByItemID(itemID);
                if (existingItem != null)
                {
                    // 检查是否可堆叠
                    if (existingItem.quantity + quantity <= item.maxStack)
                    {
                        playerInventory.UpdateItemQuantity(existingItem.inventoryItemID, existingItem.quantity + quantity);
                        CreateInventoryEvent("add", userID, itemID, "添加物品", quantity);
                        SaveInventoryData();
                        Debug.Log("成功添加物品到背包: " + item.itemName + " x" + quantity);
                    }
                    else
                    {
                        // 部分添加
                        int addQuantity = item.maxStack - existingItem.quantity;
                        playerInventory.UpdateItemQuantity(existingItem.inventoryItemID, item.maxStack);
                        CreateInventoryEvent("add", userID, itemID, "添加物品", addQuantity);
                        
                        // 剩余数量创建新物品
                        int remainingQuantity = quantity - addQuantity;
                        if (remainingQuantity > 0)
                        {
                            AddNewItemToInventory(playerInventory, itemID, remainingQuantity, userID);
                        }
                    }
                }
                else
                {
                    // 创建新物品
                    AddNewItemToInventory(playerInventory, itemID, quantity, userID);
                }
            }
            else
            {
                Debug.LogError("物品不存在或已禁用: " + itemID);
            }
        }
        else
        {
            // 创建新背包
            AddPlayerInventory(userID, "主背包", 50);
            // 再次添加物品
            AddItemToInventory(userID, itemID, quantity);
        }
    }
    
    private void AddNewItemToInventory(PlayerInventory playerInventory, string itemID, int quantity, string userID)
    {
        if (playerInventory.currentSlots < playerInventory.maxSlots)
        {
            string inventoryItemID = "inventory_item_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            InventoryItem inventoryItem = new InventoryItem(inventoryItemID, itemID, quantity);
            playerInventory.AddItem(inventoryItem);
            CreateInventoryEvent("add", userID, itemID, "添加物品", quantity);
            SaveInventoryData();
            Debug.Log("成功添加新物品到背包: " + inventoryData.system.GetItem(itemID).itemName + " x" + quantity);
        }
        else
        {
            Debug.LogError("背包已满");
        }
    }
    
    public void RemoveItemFromInventory(string userID, string itemID, int quantity)
    {
        PlayerInventory playerInventory = GetPlayerInventory(userID);
        if (playerInventory != null)
        {
            InventoryItem inventoryItem = playerInventory.GetItemByItemID(itemID);
            if (inventoryItem != null)
            {
                if (inventoryItem.quantity >= quantity)
                {
                    int newQuantity = inventoryItem.quantity - quantity;
                    if (newQuantity > 0)
                    {
                        playerInventory.UpdateItemQuantity(inventoryItem.inventoryItemID, newQuantity);
                    }
                    else
                    {
                        playerInventory.RemoveItem(inventoryItem.inventoryItemID);
                    }
                    CreateInventoryEvent("remove", userID, itemID, "移除物品", quantity);
                    SaveInventoryData();
                    Debug.Log("成功从背包移除物品: " + inventoryData.system.GetItem(itemID).itemName + " x" + quantity);
                }
                else
                {
                    Debug.LogError("物品数量不足");
                }
            }
            else
            {
                Debug.LogError("背包中没有该物品");
            }
        }
        else
        {
            Debug.LogError("背包不存在");
        }
    }
    
    public void UseItem(string userID, string itemID)
    {
        PlayerInventory playerInventory = GetPlayerInventory(userID);
        if (playerInventory != null)
        {
            InventoryItem inventoryItem = playerInventory.GetItemByItemID(itemID);
            if (inventoryItem != null)
            {
                Item item = inventoryData.system.GetItem(itemID);
                if (item != null && item.isConsumable && item.isEnabled)
                {
                    // 应用物品效果
                    foreach (string effectID in item.effectIDs)
                    {
                        ItemEffect effect = inventoryData.system.GetItemEffect(effectID);
                        if (effect != null && effect.isEnabled)
                        {
                            // 这里可以添加应用效果的逻辑
                            Debug.Log("应用物品效果: " + effect.effectName + " 值: " + effect.effectValue);
                        }
                    }
                    
                    // 使用物品
                    inventoryItem.Use();
                    RemoveItemFromInventory(userID, itemID, 1);
                    CreateInventoryEvent("use", userID, itemID, "使用物品", 1);
                    SaveInventoryData();
                    Debug.Log("成功使用物品: " + item.itemName);
                }
                else
                {
                    Debug.LogError("物品不可使用或已禁用");
                }
            }
            else
            {
                Debug.LogError("背包中没有该物品");
            }
        }
        else
        {
            Debug.LogError("背包不存在");
        }
    }
    
    public void EquipItem(string userID, string itemID, string equipSlot)
    {
        PlayerInventory playerInventory = GetPlayerInventory(userID);
        if (playerInventory != null)
        {
            InventoryItem inventoryItem = playerInventory.GetItemByItemID(itemID);
            if (inventoryItem != null)
            {
                Item item = inventoryData.system.GetItem(itemID);
                if (item != null && item.isEquippable && item.isEnabled)
                {
                    inventoryItem.Equip(equipSlot);
                    CreateInventoryEvent("equip", userID, itemID, "装备物品", 1);
                    SaveInventoryData();
                    Debug.Log("成功装备物品: " + item.itemName);
                }
                else
                {
                    Debug.LogError("物品不可装备或已禁用");
                }
            }
            else
            {
                Debug.LogError("背包中没有该物品");
            }
        }
        else
        {
            Debug.LogError("背包不存在");
        }
    }
    
    public void UnequipItem(string userID, string itemID)
    {
        PlayerInventory playerInventory = GetPlayerInventory(userID);
        if (playerInventory != null)
        {
            InventoryItem inventoryItem = playerInventory.GetItemByItemID(itemID);
            if (inventoryItem != null && inventoryItem.isEquipped)
            {
                inventoryItem.Unequip();
                CreateInventoryEvent("unequip", userID, itemID, "卸下物品", 1);
                SaveInventoryData();
                Debug.Log("成功卸下物品: " + inventoryData.system.GetItem(itemID).itemName);
            }
            else
            {
                Debug.LogError("物品未装备");
            }
        }
        else
        {
            Debug.LogError("背包不存在");
        }
    }
    
    // 背包事件管理
    public string CreateInventoryEvent(string eventType, string userID, string itemID, string description, int quantity)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        InventoryEvent inventoryEvent = new InventoryEvent(eventID, eventType, userID, itemID, description, quantity);
        inventoryData.system.AddInventoryEvent(inventoryEvent);
        SaveInventoryData();
        Debug.Log("成功创建背包事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        InventoryEvent inventoryEvent = inventoryData.system.GetInventoryEvent(eventID);
        if (inventoryEvent != null)
        {
            inventoryEvent.MarkAsCompleted();
            SaveInventoryData();
            Debug.Log("成功标记背包事件为完成");
        }
        else
        {
            Debug.LogError("背包事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        InventoryEvent inventoryEvent = inventoryData.system.GetInventoryEvent(eventID);
        if (inventoryEvent != null)
        {
            inventoryEvent.MarkAsFailed();
            SaveInventoryData();
            Debug.Log("成功标记背包事件为失败");
        }
        else
        {
            Debug.LogError("背包事件不存在: " + eventID);
        }
    }
    
    // 数据持久化
    public void SaveInventoryData()
    {
        string path = Application.dataPath + "/Data/inventory_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, inventoryData);
        stream.Close();
    }
    
    public void LoadInventoryData()
    {
        string path = Application.dataPath + "/Data/inventory_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            inventoryData = (InventorySystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            inventoryData = new InventorySystemDetailedManagerData();
        }
    }
}
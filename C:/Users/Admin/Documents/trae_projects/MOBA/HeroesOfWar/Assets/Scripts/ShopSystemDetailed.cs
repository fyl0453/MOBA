using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class ShopItem
{
    public string ItemID;
    public string ItemName;
    public string ItemType;
    public int Price;
    public int OriginalPrice;
    public string CurrencyType;
    public string Description;
    public string IconName;
    public string Category;
    public bool IsOnSale;
    public float Discount;
    public int Stock;
    public int MaxPurchaseLimit;
    public bool IsLimitedTime;
    public DateTime StartTime;
    public DateTime EndTime;
    public List<string> Tags;

    public ShopItem(string itemID, string itemName, string itemType, int price, string currencyType, string description, string category)
    {
        ItemID = itemID;
        ItemName = itemName;
        ItemType = itemType;
        Price = price;
        OriginalPrice = price;
        CurrencyType = currencyType;
        Description = description;
        IconName = "";
        Category = category;
        IsOnSale = false;
        Discount = 1.0f;
        Stock = -1;
        MaxPurchaseLimit = -1;
        IsLimitedTime = false;
        StartTime = DateTime.Now;
        EndTime = DateTime.MaxValue;
        Tags = new List<string>();
    }
}

[Serializable]
public class CurrencyPackage
{
    public string PackageID;
    public string PackageName;
    public int CurrencyAmount;
    public int RealPrice;
    public string CurrencyType;
    public int BonusAmount;
    public bool IsSpecialOffer;
    public string IconName;
    public string Description;

    public CurrencyPackage(string packageID, string packageName, int currencyAmount, int realPrice, string currencyType, int bonusAmount = 0)
    {
        PackageID = packageID;
        PackageName = packageName;
        CurrencyAmount = currencyAmount;
        RealPrice = realPrice;
        CurrencyType = currencyType;
        BonusAmount = bonusAmount;
        IsSpecialOffer = false;
        IconName = "";
        Description = "";
    }
}

[Serializable]
public class PurchaseRecord
{
    public string RecordID;
    public string PlayerID;
    public string ItemID;
    public string ItemName;
    public int Price;
    public string CurrencyType;
    public DateTime PurchaseTime;
    public string PaymentMethod;
    public string TransactionID;
    public bool IsSuccessful;
    public string OrderID;

    public PurchaseRecord(string recordID, string playerID, string itemID, string itemName, int price, string currencyType, string paymentMethod, string transactionID)
    {
        RecordID = recordID;
        PlayerID = playerID;
        ItemID = itemID;
        ItemName = itemName;
        Price = price;
        CurrencyType = currencyType;
        PurchaseTime = DateTime.Now;
        PaymentMethod = paymentMethod;
        TransactionID = transactionID;
        IsSuccessful = true;
        OrderID = "";
    }
}

[Serializable]
public class PlayerCurrency
{
    public string PlayerID;
    public Dictionary<string, int> Currencies;
    public DateTime LastUpdateTime;

    public PlayerCurrency(string playerID)
    {
        PlayerID = playerID;
        Currencies = new Dictionary<string, int>();
        Currencies["gold"] = 0;
        Currencies["diamond"] = 0;
        Currencies["coupon"] = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class ShopCategory
{
    public string CategoryID;
    public string CategoryName;
    public string IconName;
    public int SortOrder;
    public bool IsVisible;
    public string Description;

    public ShopCategory(string categoryID, string categoryName, int sortOrder)
    {
        CategoryID = categoryID;
        CategoryName = categoryName;
        IconName = "";
        SortOrder = sortOrder;
        IsVisible = true;
        Description = "";
    }
}

[Serializable]
public class HotItem
{
    public string HotItemID;
    public string ItemID;
    public int DisplayOrder;
    public string BannerImage;
    public DateTime StartTime;
    public DateTime EndTime;

    public HotItem(string hotItemID, string itemID, int displayOrder)
    {
        HotItemID = hotItemID;
        ItemID = itemID;
        DisplayOrder = displayOrder;
        BannerImage = "";
        StartTime = DateTime.Now;
        EndTime = DateTime.MaxValue;
    }
}

[Serializable]
public class ShopSystemData
{
    public List<ShopItem> AllItems;
    public List<CurrencyPackage> CurrencyPackages;
    public List<PurchaseRecord> PurchaseRecords;
    public Dictionary<string, PlayerCurrency> PlayerCurrencies;
    public List<ShopCategory> ShopCategories;
    public List<HotItem> HotItems;
    public DateTime LastUpdateTime;

    public ShopSystemData()
    {
        AllItems = new List<ShopItem>();
        CurrencyPackages = new List<CurrencyPackage>();
        PurchaseRecords = new List<PurchaseRecord>();
        PlayerCurrencies = new Dictionary<string, PlayerCurrency>();
        ShopCategories = new List<ShopCategory>();
        HotItems = new List<HotItem>();
        LastUpdateTime = DateTime.Now;
        InitializeDefaultCategories();
        InitializeDefaultItems();
        InitializeDefaultCurrencyPackages();
    }

    private void InitializeDefaultCategories()
    {
        ShopCategory heroCategory = new ShopCategory("category_hero", "英雄", 1);
        ShopCategories.Add(heroCategory);

        ShopCategory skinCategory = new ShopCategory("category_skin", "皮肤", 2);
        ShopCategories.Add(skinCategory);

        ShopCategory itemCategory = new ShopCategory("category_item", "道具", 3);
        ShopCategories.Add(itemCategory);

        ShopCategory currencyCategory = new ShopCategory("category_currency", "点券", 4);
        ShopCategories.Add(currencyCategory);

        ShopCategory giftCategory = new ShopCategory("category_gift", "礼品", 5);
        ShopCategories.Add(giftCategory);
    }

    private void InitializeDefaultItems()
    {
        ShopItem healthPotion = new ShopItem("item_health_potion", "生命药水", "consumable", 100, "gold", "回复生命值", "category_item");
        AllItems.Add(healthPotion);

        ShopItem manaPotion = new ShopItem("item_mana_potion", "法力药水", "consumable", 80, "gold", "回复法力值", "category_item");
        AllItems.Add(manaPotion);

        ShopItem experienceBooster = new ShopItem("item_exp_booster", "经验加成卡", "booster", 200, "diamond", "增加经验获取", "category_item");
        AllItems.Add(experienceBooster);
    }

    private void InitializeDefaultCurrencyPackages()
    {
        CurrencyPackage smallPackage = new CurrencyPackage("package_small", "小礼包", 100, 6, "diamond", 10);
        CurrencyPackages.Add(smallPackage);

        CurrencyPackage mediumPackage = new CurrencyPackage("package_medium", "中礼包", 500, 30, "diamond", 50);
        CurrencyPackages.Add(mediumPackage);

        CurrencyPackage largePackage = new CurrencyPackage("package_large", "大礼包", 1000, 60, "diamond", 150);
        largePackage.IsSpecialOffer = true;
        CurrencyPackages.Add(largePackage);
    }

    public void AddItem(ShopItem item)
    {
        AllItems.Add(item);
    }

    public void AddCurrencyPackage(CurrencyPackage package)
    {
        CurrencyPackages.Add(package);
    }

    public void AddPurchaseRecord(PurchaseRecord record)
    {
        PurchaseRecords.Add(record);
    }

    public void AddPlayerCurrency(string playerID, PlayerCurrency currency)
    {
        PlayerCurrencies[playerID] = currency;
    }

    public void AddShopCategory(ShopCategory category)
    {
        ShopCategories.Add(category);
    }

    public void AddHotItem(HotItem hotItem)
    {
        HotItems.Add(hotItem);
    }
}

[Serializable]
public class ShopEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string ItemID;
    public string EventData;

    public ShopEvent(string eventID, string eventType, string playerID, string itemID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        ItemID = itemID;
        EventData = eventData;
    }
}

public class ShopSystemDataManager
{
    private static ShopSystemDataManager _instance;
    public static ShopSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ShopSystemDataManager();
            }
            return _instance;
        }
    }

    public ShopSystemData shopData;
    private List<ShopEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private ShopSystemDataManager()
    {
        shopData = new ShopSystemData();
        recentEvents = new List<ShopEvent>();
        LoadShopData();
    }

    public void SaveShopData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ShopSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, shopData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存商城系统数据失败: " + e.Message);
        }
    }

    public void LoadShopData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ShopSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    shopData = (ShopSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载商城系统数据失败: " + e.Message);
            shopData = new ShopSystemData();
        }
    }

    public void CreateShopEvent(string eventType, string playerID, string itemID, string eventData)
    {
        string eventID = "shop_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ShopEvent shopEvent = new ShopEvent(eventID, eventType, playerID, itemID, eventData);
        recentEvents.Add(shopEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<ShopEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
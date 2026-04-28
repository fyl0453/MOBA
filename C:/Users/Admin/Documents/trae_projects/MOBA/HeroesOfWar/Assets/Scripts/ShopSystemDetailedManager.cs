using System;
using System.Collections.Generic;

public class ShopSystemDetailedManager
{
    private static ShopSystemDetailedManager _instance;
    public static ShopSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ShopSystemDetailedManager();
            }
            return _instance;
        }
    }

    private ShopSystemData shopData;
    private ShopSystemDataManager dataManager;

    private ShopSystemDetailedManager()
    {
        dataManager = ShopSystemDataManager.Instance;
        shopData = dataManager.shopData;
    }

    public void AddItem(string itemName, string itemType, int price, string currencyType, string description, string category, string iconName = "")
    {
        string itemID = "item_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ShopItem item = new ShopItem(itemID, itemName, itemType, price, currencyType, description, category);
        item.IconName = iconName;
        shopData.AddItem(item);
        dataManager.SaveShopData();
        Debug.Log("添加商城物品成功: " + itemName);
    }

    public void UpdateItem(string itemID, string itemName, string itemType, int price, string currencyType, string description, string category)
    {
        ShopItem item = shopData.AllItems.Find(i => i.ItemID == itemID);
        if (item != null)
        {
            item.ItemName = itemName;
            item.ItemType = itemType;
            item.Price = price;
            item.OriginalPrice = price;
            item.CurrencyType = currencyType;
            item.Description = description;
            item.Category = category;
            dataManager.SaveShopData();
            Debug.Log("更新商城物品成功: " + itemName);
        }
    }

    public void RemoveItem(string itemID)
    {
        ShopItem item = shopData.AllItems.Find(i => i.ItemID == itemID);
        if (item != null)
        {
            shopData.AllItems.Remove(item);
            dataManager.SaveShopData();
            Debug.Log("移除商城物品成功: " + item.ItemName);
        }
    }

    public void SetItemOnSale(string itemID, bool isOnSale, float discount = 1.0f)
    {
        ShopItem item = shopData.AllItems.Find(i => i.ItemID == itemID);
        if (item != null)
        {
            item.IsOnSale = isOnSale;
            item.Discount = discount;
            if (isOnSale)
            {
                item.Price = (int)(item.OriginalPrice * discount);
            }
            else
            {
                item.Price = item.OriginalPrice;
            }
            dataManager.SaveShopData();
            Debug.Log("设置物品折扣成功: " + item.ItemName);
        }
    }

    public void SetItemLimitedTime(string itemID, bool isLimitedTime, DateTime startTime, DateTime endTime)
    {
        ShopItem item = shopData.AllItems.Find(i => i.ItemID == itemID);
        if (item != null)
        {
            item.IsLimitedTime = isLimitedTime;
            item.StartTime = startTime;
            item.EndTime = endTime;
            dataManager.SaveShopData();
            Debug.Log("设置限时物品成功: " + item.ItemName);
        }
    }

    public void AddCurrencyPackage(string packageName, int currencyAmount, int realPrice, string currencyType, int bonusAmount = 0, bool isSpecialOffer = false)
    {
        string packageID = "package_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CurrencyPackage package = new CurrencyPackage(packageID, packageName, currencyAmount, realPrice, currencyType, bonusAmount);
        package.IsSpecialOffer = isSpecialOffer;
        shopData.AddCurrencyPackage(package);
        dataManager.SaveShopData();
        Debug.Log("添加点券包成功: " + packageName);
    }

    public void UpdateCurrencyPackage(string packageID, string packageName, int currencyAmount, int realPrice, int bonusAmount, bool isSpecialOffer)
    {
        CurrencyPackage package = shopData.CurrencyPackages.Find(p => p.PackageID == packageID);
        if (package != null)
        {
            package.PackageName = packageName;
            package.CurrencyAmount = currencyAmount;
            package.RealPrice = realPrice;
            package.BonusAmount = bonusAmount;
            package.IsSpecialOffer = isSpecialOffer;
            dataManager.SaveShopData();
            Debug.Log("更新点券包成功: " + packageName);
        }
    }

    public void RemoveCurrencyPackage(string packageID)
    {
        CurrencyPackage package = shopData.CurrencyPackages.Find(p => p.PackageID == packageID);
        if (package != null)
        {
            shopData.CurrencyPackages.Remove(package);
            dataManager.SaveShopData();
            Debug.Log("移除点券包成功: " + package.PackageName);
        }
    }

    public List<ShopItem> GetItemsByCategory(string category)
    {
        return shopData.AllItems.FindAll(i => i.Category == category && i.Stock != 0);
    }

    public List<ShopItem> GetAllItems()
    {
        return shopData.AllItems.FindAll(i => i.Stock != 0);
    }

    public List<ShopItem> GetOnSaleItems()
    {
        return shopData.AllItems.FindAll(i => i.IsOnSale && i.Stock != 0);
    }

    public List<ShopItem> GetLimitedTimeItems()
    {
        DateTime now = DateTime.Now;
        return shopData.AllItems.FindAll(i => i.IsLimitedTime && now >= i.StartTime && now <= i.EndTime && i.Stock != 0);
    }

    public List<HotItem> GetHotItems()
    {
        DateTime now = DateTime.Now;
        return shopData.HotItems.FindAll(h => now >= h.StartTime && now <= h.EndTime);
    }

    public List<CurrencyPackage> GetCurrencyPackages()
    {
        return shopData.CurrencyPackages;
    }

    public ShopItem GetItem(string itemID)
    {
        return shopData.AllItems.Find(i => i.ItemID == itemID);
    }

    public CurrencyPackage GetCurrencyPackage(string packageID)
    {
        return shopData.CurrencyPackages.Find(p => p.PackageID == packageID);
    }

    public int GetPlayerCurrency(string playerID, string currencyType)
    {
        if (!shopData.PlayerCurrencies.ContainsKey(playerID))
        {
            PlayerCurrency playerCurrency = new PlayerCurrency(playerID);
            shopData.AddPlayerCurrency(playerID, playerCurrency);
        }
        PlayerCurrency currency = shopData.PlayerCurrencies[playerID];
        if (currency.Currencies.ContainsKey(currencyType))
        {
            return currency.Currencies[currencyType];
        }
        return 0;
    }

    public void AddCurrency(string playerID, string currencyType, int amount)
    {
        if (!shopData.PlayerCurrencies.ContainsKey(playerID))
        {
            PlayerCurrency playerCurrency = new PlayerCurrency(playerID);
            shopData.AddPlayerCurrency(playerID, playerCurrency);
        }
        PlayerCurrency currency = shopData.PlayerCurrencies[playerID];
        if (!currency.Currencies.ContainsKey(currencyType))
        {
            currency.Currencies[currencyType] = 0;
        }
        currency.Currencies[currencyType] += amount;
        currency.LastUpdateTime = DateTime.Now;
        dataManager.SaveShopData();
        Debug.Log("添加货币成功: " + amount + " " + currencyType);
    }

    public void RemoveCurrency(string playerID, string currencyType, int amount)
    {
        if (shopData.PlayerCurrencies.ContainsKey(playerID))
        {
            PlayerCurrency currency = shopData.PlayerCurrencies[playerID];
            if (currency.Currencies.ContainsKey(currencyType) && currency.Currencies[currencyType] >= amount)
            {
                currency.Currencies[currencyType] -= amount;
                currency.LastUpdateTime = DateTime.Now;
                dataManager.SaveShopData();
                Debug.Log("扣除货币成功: " + amount + " " + currencyType);
            }
        }
    }

    public bool CanAfford(string playerID, string currencyType, int amount)
    {
        return GetPlayerCurrency(playerID, currencyType) >= amount;
    }

    public bool PurchaseItem(string playerID, string itemID, string paymentMethod = "in_game")
    {
        ShopItem item = GetItem(itemID);
        if (item == null)
        {
            Debug.LogError("物品不存在: " + itemID);
            return false;
        }

        if (item.Stock == 0)
        {
            Debug.LogError("物品已售罄: " + item.ItemName);
            return false;
        }

        if (item.IsLimitedTime && (DateTime.Now < item.StartTime || DateTime.Now > item.EndTime))
        {
            Debug.LogError("物品不在销售时间内: " + item.ItemName);
            return false;
        }

        if (!CanAfford(playerID, item.CurrencyType, item.Price))
        {
            Debug.LogError("货币不足: " + item.CurrencyType);
            return false;
        }

        RemoveCurrency(playerID, item.CurrencyType, item.Price);

        if (item.Stock > 0)
        {
            item.Stock--;
        }

        string recordID = "record_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        string transactionID = "trans_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PurchaseRecord record = new PurchaseRecord(recordID, playerID, itemID, item.ItemName, item.Price, item.CurrencyType, paymentMethod, transactionID);
        shopData.AddPurchaseRecord(record);

        dataManager.CreateShopEvent("purchase_item", playerID, itemID, "购买物品: " + item.ItemName);
        dataManager.SaveShopData();

        Debug.Log("购买物品成功: " + item.ItemName);
        return true;
    }

    public bool PurchaseCurrencyPackage(string playerID, string packageID, string paymentMethod = "real_money")
    {
        CurrencyPackage package = GetCurrencyPackage(packageID);
        if (package == null)
        {
            Debug.LogError("点券包不存在: " + packageID);
            return false;
        }

        int totalCurrency = package.CurrencyAmount + package.BonusAmount;
        AddCurrency(playerID, package.CurrencyType, totalCurrency);

        string recordID = "record_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        string transactionID = "trans_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PurchaseRecord record = new PurchaseRecord(recordID, playerID, packageID, package.PackageName, package.RealPrice, "real_money", paymentMethod, transactionID);
        shopData.AddPurchaseRecord(record);

        dataManager.CreateShopEvent("purchase_currency", playerID, packageID, "购买点券包: " + package.PackageName);
        dataManager.SaveShopData();

        Debug.Log("购买点券包成功: " + package.PackageName);
        return true;
    }

    public List<PurchaseRecord> GetPlayerPurchaseHistory(string playerID, int count = 50)
    {
        List<PurchaseRecord> records = shopData.PurchaseRecords.FindAll(r => r.PlayerID == playerID);
        records.Sort((a, b) => b.PurchaseTime.CompareTo(a.PurchaseTime));
        if (count < records.Count)
        {
            return records.GetRange(0, count);
        }
        return records;
    }

    public void AddHotItem(string itemID, int displayOrder, string bannerImage = "")
    {
        string hotItemID = "hot_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HotItem hotItem = new HotItem(hotItemID, itemID, displayOrder);
        hotItem.BannerImage = bannerImage;
        shopData.AddHotItem(hotItem);
        dataManager.SaveShopData();
        Debug.Log("添加热门物品成功: " + itemID);
    }

    public void RemoveHotItem(string hotItemID)
    {
        HotItem hotItem = shopData.HotItems.Find(h => h.HotItemID == hotItemID);
        if (hotItem != null)
        {
            shopData.HotItems.Remove(hotItem);
            dataManager.SaveShopData();
            Debug.Log("移除热门物品成功: " + hotItemID);
        }
    }

    public void CleanupExpiredItems()
    {
        DateTime now = DateTime.Now;
        List<ShopItem> expiredItems = new List<ShopItem>();
        foreach (ShopItem item in shopData.AllItems)
        {
            if (item.IsLimitedTime && now > item.EndTime)
            {
                expiredItems.Add(item);
            }
        }
        
        foreach (ShopItem item in expiredItems)
        {
            shopData.AllItems.Remove(item);
        }
        
        List<HotItem> expiredHotItems = new List<HotItem>();
        foreach (HotItem hotItem in shopData.HotItems)
        {
            if (now > hotItem.EndTime)
            {
                expiredHotItems.Add(hotItem);
            }
        }
        
        foreach (HotItem hotItem in expiredHotItems)
        {
            shopData.HotItems.Remove(hotItem);
        }
        
        if (expiredItems.Count > 0 || expiredHotItems.Count > 0)
        {
            dataManager.SaveShopData();
            Debug.Log("清理过期物品成功");
        }
    }

    public void SaveData()
    {
        dataManager.SaveShopData();
    }

    public void LoadData()
    {
        dataManager.LoadShopData();
    }

    public List<ShopEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
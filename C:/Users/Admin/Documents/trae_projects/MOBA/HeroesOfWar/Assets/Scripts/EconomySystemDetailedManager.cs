using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EconomySystemDetailedManager : MonoBehaviour
{
    public static EconomySystemDetailedManager Instance { get; private set; }
    
    public EconomySystemDetailedManagerData economyData;
    
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
        LoadEconomyData();
        
        if (economyData == null)
        {
            economyData = new EconomySystemDetailedManagerData();
            InitializeDefaultEconomySystem();
        }
    }
    
    private void InitializeDefaultEconomySystem()
    {
        // 货币类型
        Currency gold = new Currency("currency_gold", "金币", "G", "primary", "游戏内基础货币", true, true);
        Currency diamond = new Currency("currency_diamond", "钻石", "D", "premium", "游戏内高级货币", true, true);
        Currency honor = new Currency("currency_honor", "荣誉值", "H", "special", "通过排位赛获得的货币", false, true);
        Currency fragment = new Currency("currency_fragment", "碎片", "F", "special", "用于兑换英雄和皮肤的货币", true, true);
        Currency ticket = new Currency("currency_ticket", "抽奖券", "T", "special", "用于参与抽奖活动的货币", false, true);
        
        economyData.system.AddCurrency(gold);
        economyData.system.AddCurrency(diamond);
        economyData.system.AddCurrency(honor);
        economyData.system.AddCurrency(fragment);
        economyData.system.AddCurrency(ticket);
        
        // 玩家货币
        PlayerCurrency player1Gold = new PlayerCurrency("user_001", "currency_gold", 10000);
        PlayerCurrency player1Diamond = new PlayerCurrency("user_001", "currency_diamond", 1000);
        PlayerCurrency player1Honor = new PlayerCurrency("user_001", "currency_honor", 500);
        PlayerCurrency player1Fragment = new PlayerCurrency("user_001", "currency_fragment", 50);
        PlayerCurrency player1Ticket = new PlayerCurrency("user_001", "currency_ticket", 10);
        
        PlayerCurrency player2Gold = new PlayerCurrency("user_002", "currency_gold", 5000);
        PlayerCurrency player2Diamond = new PlayerCurrency("user_002", "currency_diamond", 500);
        PlayerCurrency player2Honor = new PlayerCurrency("user_002", "currency_honor", 200);
        PlayerCurrency player2Fragment = new PlayerCurrency("user_002", "currency_fragment", 20);
        PlayerCurrency player2Ticket = new PlayerCurrency("user_002", "currency_ticket", 5);
        
        economyData.system.AddPlayerCurrency(player1Gold);
        economyData.system.AddPlayerCurrency(player1Diamond);
        economyData.system.AddPlayerCurrency(player1Honor);
        economyData.system.AddPlayerCurrency(player1Fragment);
        economyData.system.AddPlayerCurrency(player1Ticket);
        
        economyData.system.AddPlayerCurrency(player2Gold);
        economyData.system.AddPlayerCurrency(player2Diamond);
        economyData.system.AddPlayerCurrency(player2Honor);
        economyData.system.AddPlayerCurrency(player2Fragment);
        economyData.system.AddPlayerCurrency(player2Ticket);
        
        // 商店物品
        ShopItem item1 = new ShopItem("item_001", "英雄体验卡", "使用后可体验指定英雄3天", "hero", "currency_gold", 1000, 100, false, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.AddDays(30).ToString("yyyy-MM-dd HH:mm:ss"));
        ShopItem item2 = new ShopItem("item_002", "皮肤体验卡", "使用后可体验指定皮肤7天", "skin", "currency_diamond", 50, 50, false, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.AddDays(30).ToString("yyyy-MM-dd HH:mm:ss"));
        ShopItem item3 = new ShopItem("item_003", "金币加成卡", "使用后24小时内金币获取增加20%", "boost", "currency_diamond", 30, 30, true, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss"));
        ShopItem item4 = new ShopItem("item_004", "经验加成卡", "使用后24小时内经验获取增加20%", "boost", "currency_diamond", 30, 30, true, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss"));
        ShopItem item5 = new ShopItem("item_005", "英雄碎片", "用于兑换英雄", "fragment", "currency_gold", 200, 200, false, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.AddDays(30).ToString("yyyy-MM-dd HH:mm:ss"));
        ShopItem item6 = new ShopItem("item_006", "皮肤碎片", "用于兑换皮肤", "fragment", "currency_diamond", 10, 100, false, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.AddDays(30).ToString("yyyy-MM-dd HH:mm:ss"));
        
        economyData.system.AddShopItem(item1);
        economyData.system.AddShopItem(item2);
        economyData.system.AddShopItem(item3);
        economyData.system.AddShopItem(item4);
        economyData.system.AddShopItem(item5);
        economyData.system.AddShopItem(item6);
        
        // 交易记录
        Transaction transaction1 = new Transaction("trans_001", "user_001", "purchase", "currency_gold", 1000, "购买英雄体验卡");
        Transaction transaction2 = new Transaction("trans_002", "user_001", "reward", "currency_diamond", 100, "完成每日任务奖励");
        Transaction transaction3 = new Transaction("trans_003", "user_002", "purchase", "currency_diamond", 50, "购买皮肤体验卡");
        
        economyData.system.AddTransaction(transaction1);
        economyData.system.AddTransaction(transaction2);
        economyData.system.AddTransaction(transaction3);
        
        // 市场交易
        MarketTransaction marketTrans1 = new MarketTransaction("market_001", "user_001", "skin_001", "skin", 200, "currency_diamond", "buy");
        MarketTransaction marketTrans2 = new MarketTransaction("market_002", "user_002", "hero_001", "hero", 5000, "currency_gold", "buy");
        
        economyData.system.AddMarketTransaction(marketTrans1);
        economyData.system.AddMarketTransaction(marketTrans2);
        
        // 货币兑换
        CurrencyExchange exchange1 = new CurrencyExchange("exchange_001", "user_001", "currency_diamond", "currency_gold", 100, 10000, 100);
        
        economyData.system.AddCurrencyExchange(exchange1);
        
        SaveEconomyData();
    }
    
    // 货币管理
    public void AddCurrency(string name, string symbol, string type, string desc, bool tradable, bool consumable)
    {
        string currencyID = "currency_" + name.ToLower().Replace(" ", "_");
        Currency currency = new Currency(currencyID, name, symbol, type, desc, tradable, consumable);
        economyData.system.AddCurrency(currency);
        SaveEconomyData();
        Debug.Log("成功添加货币: " + name);
    }
    
    public List<Currency> GetAllCurrencies()
    {
        return economyData.system.currencies;
    }
    
    public Currency GetCurrency(string currencyID)
    {
        return economyData.system.GetCurrency(currencyID);
    }
    
    // 玩家货币管理
    public void InitializePlayerCurrency(string playerID, string currencyID, float initialBalance = 0)
    {
        PlayerCurrency existing = economyData.system.GetPlayerCurrency(playerID, currencyID);
        if (existing == null)
        {
            PlayerCurrency playerCurrency = new PlayerCurrency(playerID, currencyID, initialBalance);
            economyData.system.AddPlayerCurrency(playerCurrency);
            SaveEconomyData();
            Debug.Log("成功初始化玩家货币: " + currencyID);
        }
    }
    
    public bool AddCurrencyToPlayer(string playerID, string currencyID, float amount)
    {
        PlayerCurrency playerCurrency = economyData.system.GetPlayerCurrency(playerID, currencyID);
        if (playerCurrency == null)
        {
            InitializePlayerCurrency(playerID, currencyID, amount);
            return true;
        }
        else
        {
            playerCurrency.AddAmount(amount);
            SaveEconomyData();
            Debug.Log("成功给玩家添加货币: " + currencyID + " " + amount);
            return true;
        }
    }
    
    public bool DeductCurrencyFromPlayer(string playerID, string currencyID, float amount)
    {
        PlayerCurrency playerCurrency = economyData.system.GetPlayerCurrency(playerID, currencyID);
        if (playerCurrency != null)
        {
            bool success = playerCurrency.DeductAmount(amount);
            if (success)
            {
                SaveEconomyData();
                Debug.Log("成功从玩家扣除货币: " + currencyID + " " + amount);
            }
            return success;
        }
        return false;
    }
    
    public float GetPlayerCurrencyBalance(string playerID, string currencyID)
    {
        PlayerCurrency playerCurrency = economyData.system.GetPlayerCurrency(playerID, currencyID);
        if (playerCurrency != null)
        {
            return playerCurrency.balance;
        }
        return 0;
    }
    
    public List<PlayerCurrency> GetPlayerCurrencies(string playerID)
    {
        return economyData.system.GetPlayerCurrencies(playerID);
    }
    
    // 交易管理
    public void AddTransaction(string playerID, string type, string currencyID, float amount, string desc)
    {
        string transactionID = "trans_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Transaction transaction = new Transaction(transactionID, playerID, type, currencyID, amount, desc);
        economyData.system.AddTransaction(transaction);
        SaveEconomyData();
        Debug.Log("成功添加交易记录: " + transactionID);
    }
    
    public List<Transaction> GetTransactionsByPlayer(string playerID)
    {
        return economyData.system.GetTransactionsByPlayer(playerID);
    }
    
    // 商店管理
    public void AddShopItem(string name, string desc, string category, string currencyID, float price, int stock, bool limited, string startTime, string endTime)
    {
        string itemID = "item_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ShopItem item = new ShopItem(itemID, name, desc, category, currencyID, price, stock, limited, startTime, endTime);
        economyData.system.AddShopItem(item);
        SaveEconomyData();
        Debug.Log("成功添加商店物品: " + name);
    }
    
    public bool PurchaseShopItem(string playerID, string itemID, int quantity = 1)
    {
        ShopItem item = economyData.system.GetShopItem(itemID);
        if (item != null && item.IsAvailable())
        {
            float totalPrice = item.price * quantity;
            if (DeductCurrencyFromPlayer(playerID, item.currencyID, totalPrice))
            {
                item.DecreaseStock(quantity);
                AddTransaction(playerID, "purchase", item.currencyID, -totalPrice, "购买" + item.itemName + " x" + quantity);
                SaveEconomyData();
                Debug.Log("成功购买商店物品: " + item.itemName);
                return true;
            }
            else
            {
                Debug.Log("货币不足，无法购买物品: " + item.itemName);
                return false;
            }
        }
        else
        {
            Debug.Log("物品不可用: " + itemID);
            return false;
        }
    }
    
    public List<ShopItem> GetShopItemsByCategory(string category)
    {
        return economyData.system.GetShopItemsByCategory(category);
    }
    
    public List<ShopItem> GetAllShopItems()
    {
        return economyData.system.shopItems;
    }
    
    // 市场交易管理
    public void AddMarketTransaction(string playerID, string itemID, string itemType, float price, string currencyID, string type)
    {
        string transactionID = "market_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MarketTransaction transaction = new MarketTransaction(transactionID, playerID, itemID, itemType, price, currencyID, type);
        economyData.system.AddMarketTransaction(transaction);
        SaveEconomyData();
        Debug.Log("成功添加市场交易记录: " + transactionID);
    }
    
    public List<MarketTransaction> GetMarketTransactionsByPlayer(string playerID)
    {
        return economyData.system.GetMarketTransactionsByPlayer(playerID);
    }
    
    // 货币兑换管理
    public bool ExchangeCurrency(string playerID, string fromCurrencyID, string toCurrencyID, float fromAmount, float exchangeRate)
    {
        if (DeductCurrencyFromPlayer(playerID, fromCurrencyID, fromAmount))
        {
            float toAmount = fromAmount * exchangeRate;
            AddCurrencyToPlayer(playerID, toCurrencyID, toAmount);
            
            string exchangeID = "exchange_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            CurrencyExchange exchange = new CurrencyExchange(exchangeID, playerID, fromCurrencyID, toCurrencyID, fromAmount, toAmount, exchangeRate);
            economyData.system.AddCurrencyExchange(exchange);
            
            AddTransaction(playerID, "exchange_out", fromCurrencyID, -fromAmount, "兑换货币");
            AddTransaction(playerID, "exchange_in", toCurrencyID, toAmount, "兑换货币");
            
            SaveEconomyData();
            Debug.Log("成功兑换货币: " + fromCurrencyID + " -> " + toCurrencyID);
            return true;
        }
        else
        {
            Debug.Log("货币不足，无法兑换");
            return false;
        }
    }
    
    public List<CurrencyExchange> GetCurrencyExchangesByPlayer(string playerID)
    {
        return economyData.system.GetCurrencyExchangesByPlayer(playerID);
    }
    
    // 数据持久化
    public void SaveEconomyData()
    {
        string path = Application.dataPath + "/Data/economy_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, economyData);
        stream.Close();
    }
    
    public void LoadEconomyData()
    {
        string path = Application.dataPath + "/Data/economy_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            economyData = (EconomySystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            economyData = new EconomySystemDetailedManagerData();
        }
    }
}
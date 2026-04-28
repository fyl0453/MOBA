[System.Serializable]
public class EconomySystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Currency> currencies;
    public List<Transaction> transactions;
    public List<ShopItem> shopItems;
    public List<PlayerCurrency> playerCurrencies;
    public List<MarketTransaction> marketTransactions;
    public List<CurrencyExchange> currencyExchanges;
    
    public EconomySystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        currencies = new List<Currency>();
        transactions = new List<Transaction>();
        shopItems = new List<ShopItem>();
        playerCurrencies = new List<PlayerCurrency>();
        marketTransactions = new List<MarketTransaction>();
        currencyExchanges = new List<CurrencyExchange>();
    }
    
    public void AddCurrency(Currency currency)
    {
        currencies.Add(currency);
    }
    
    public void AddTransaction(Transaction transaction)
    {
        transactions.Add(transaction);
    }
    
    public void AddShopItem(ShopItem item)
    {
        shopItems.Add(item);
    }
    
    public void AddPlayerCurrency(PlayerCurrency playerCurrency)
    {
        playerCurrencies.Add(playerCurrency);
    }
    
    public void AddMarketTransaction(MarketTransaction transaction)
    {
        marketTransactions.Add(transaction);
    }
    
    public void AddCurrencyExchange(CurrencyExchange exchange)
    {
        currencyExchanges.Add(exchange);
    }
    
    public Currency GetCurrency(string currencyID)
    {
        return currencies.Find(c => c.currencyID == currencyID);
    }
    
    public Transaction GetTransaction(string transactionID)
    {
        return transactions.Find(t => t.transactionID == transactionID);
    }
    
    public ShopItem GetShopItem(string itemID)
    {
        return shopItems.Find(s => s.itemID == itemID);
    }
    
    public PlayerCurrency GetPlayerCurrency(string playerID, string currencyID)
    {
        return playerCurrencies.Find(pc => pc.playerID == playerID && pc.currencyID == currencyID);
    }
    
    public MarketTransaction GetMarketTransaction(string transactionID)
    {
        return marketTransactions.Find(mt => mt.transactionID == transactionID);
    }
    
    public CurrencyExchange GetCurrencyExchange(string exchangeID)
    {
        return currencyExchanges.Find(ce => ce.exchangeID == exchangeID);
    }
    
    public List<Transaction> GetTransactionsByPlayer(string playerID)
    {
        return transactions.FindAll(t => t.playerID == playerID);
    }
    
    public List<ShopItem> GetShopItemsByCategory(string category)
    {
        return shopItems.FindAll(s => s.category == category);
    }
    
    public List<PlayerCurrency> GetPlayerCurrencies(string playerID)
    {
        return playerCurrencies.FindAll(pc => pc.playerID == playerID);
    }
    
    public List<MarketTransaction> GetMarketTransactionsByPlayer(string playerID)
    {
        return marketTransactions.FindAll(mt => mt.playerID == playerID);
    }
    
    public List<CurrencyExchange> GetCurrencyExchangesByPlayer(string playerID)
    {
        return currencyExchanges.FindAll(ce => ce.playerID == playerID);
    }
}

[System.Serializable]
public class Currency
{
    public string currencyID;
    public string currencyName;
    public string currencySymbol;
    public string currencyType;
    public string description;
    public bool isTradable;
    public bool isConsumable;
    
    public Currency(string id, string name, string symbol, string type, string desc, bool tradable, bool consumable)
    {
        currencyID = id;
        currencyName = name;
        currencySymbol = symbol;
        currencyType = type;
        description = desc;
        isTradable = tradable;
        isConsumable = consumable;
    }
}

[System.Serializable]
public class Transaction
{
    public string transactionID;
    public string playerID;
    public string transactionType;
    public string currencyID;
    public float amount;
    public string description;
    public string transactionDate;
    public string status;
    
    public Transaction(string id, string playerID, string type, string currencyID, float amount, string desc)
    {
        transactionID = id;
        this.playerID = playerID;
        transactionType = type;
        this.currencyID = currencyID;
        this.amount = amount;
        description = desc;
        transactionDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "Completed";
    }
}

[System.Serializable]
public class ShopItem
{
    public string itemID;
    public string itemName;
    public string itemDescription;
    public string category;
    public string currencyID;
    public float price;
    public int stock;
    public bool isLimited;
    public string startTime;
    public string endTime;
    public bool isEnabled;
    
    public ShopItem(string id, string name, string desc, string category, string currencyID, float price, int stock, bool limited, string startTime, string endTime)
    {
        itemID = id;
        itemName = name;
        itemDescription = desc;
        this.category = category;
        this.currencyID = currencyID;
        this.price = price;
        this.stock = stock;
        isLimited = limited;
        this.startTime = startTime;
        this.endTime = endTime;
        isEnabled = true;
    }
    
    public bool IsAvailable()
    {
        if (!isEnabled)
            return false;
        
        if (isLimited)
        {
            System.DateTime now = System.DateTime.Now;
            System.DateTime start = System.DateTime.Parse(startTime);
            System.DateTime end = System.DateTime.Parse(endTime);
            return now >= start && now <= end && stock > 0;
        }
        
        return stock > 0;
    }
    
    public void DecreaseStock(int amount)
    {
        stock -= amount;
        if (stock < 0)
            stock = 0;
    }
}

[System.Serializable]
public class PlayerCurrency
{
    public string playerID;
    public string currencyID;
    public float balance;
    public string lastUpdateDate;
    
    public PlayerCurrency(string playerID, string currencyID, float balance)
    {
        this.playerID = playerID;
        this.currencyID = currencyID;
        this.balance = balance;
        lastUpdateDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddAmount(float amount)
    {
        balance += amount;
        lastUpdateDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public bool DeductAmount(float amount)
    {
        if (balance >= amount)
        {
            balance -= amount;
            lastUpdateDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class MarketTransaction
{
    public string transactionID;
    public string playerID;
    public string itemID;
    public string itemType;
    public float price;
    public string currencyID;
    public string transactionType;
    public string transactionDate;
    public string status;
    
    public MarketTransaction(string id, string playerID, string itemID, string itemType, float price, string currencyID, string type)
    {
        transactionID = id;
        this.playerID = playerID;
        this.itemID = itemID;
        this.itemType = itemType;
        this.price = price;
        this.currencyID = currencyID;
        transactionType = type;
        transactionDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "Completed";
    }
}

[System.Serializable]
public class CurrencyExchange
{
    public string exchangeID;
    public string playerID;
    public string fromCurrencyID;
    public string toCurrencyID;
    public float fromAmount;
    public float toAmount;
    public float exchangeRate;
    public string exchangeDate;
    public string status;
    
    public CurrencyExchange(string id, string playerID, string fromCurrencyID, string toCurrencyID, float fromAmount, float toAmount, float exchangeRate)
    {
        exchangeID = id;
        this.playerID = playerID;
        this.fromCurrencyID = fromCurrencyID;
        this.toCurrencyID = toCurrencyID;
        this.fromAmount = fromAmount;
        this.toAmount = toAmount;
        this.exchangeRate = exchangeRate;
        exchangeDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "Completed";
    }
}

[System.Serializable]
public class EconomySystemDetailedManagerData
{
    public EconomySystemDetailed system;
    
    public EconomySystemDetailedManagerData()
    {
        system = new EconomySystemDetailed("economy_system_detailed", "游戏内经济系统详细", "管理游戏内经济的详细功能，包括货币类型和交易方式");
    }
}
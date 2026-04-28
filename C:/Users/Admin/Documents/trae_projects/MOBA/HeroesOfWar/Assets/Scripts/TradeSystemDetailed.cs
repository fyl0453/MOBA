[System.Serializable]
public class TradeSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Trade> trades;
    public List<TradeItem> tradeItems;
    public List<TradeEvent> tradeEvents;
    
    public TradeSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        trades = new List<Trade>();
        tradeItems = new List<TradeItem>();
        tradeEvents = new List<TradeEvent>();
    }
    
    public void AddTrade(Trade trade)
    {
        trades.Add(trade);
    }
    
    public void AddTradeItem(TradeItem tradeItem)
    {
        tradeItems.Add(tradeItem);
    }
    
    public void AddTradeEvent(TradeEvent tradeEvent)
    {
        tradeEvents.Add(tradeEvent);
    }
    
    public Trade GetTrade(string tradeID)
    {
        return trades.Find(t => t.tradeID == tradeID);
    }
    
    public TradeItem GetTradeItem(string itemID)
    {
        return tradeItems.Find(ti => ti.itemID == itemID);
    }
    
    public TradeEvent GetTradeEvent(string eventID)
    {
        return tradeEvents.Find(te => te.eventID == eventID);
    }
    
    public List<Trade> GetTradesByUser(string userID)
    {
        return trades.FindAll(t => t.sellerID == userID || t.buyerID == userID);
    }
    
    public List<Trade> GetTradesByStatus(string status)
    {
        return trades.FindAll(t => t.status == status);
    }
    
    public List<TradeItem> GetTradeItemsByTrade(string tradeID)
    {
        return tradeItems.FindAll(ti => ti.tradeID == tradeID);
    }
    
    public List<TradeEvent> GetTradeEventsByUser(string userID)
    {
        return tradeEvents.FindAll(te => te.userID == userID);
    }
}

[System.Serializable]
public class Trade
{
    public string tradeID;
    public string sellerID;
    public string sellerName;
    public string buyerID;
    public string buyerName;
    public string tradeType;
    public string status;
    public float totalPrice;
    public string currency;
    public string createTime;
    public string completeTime;
    public string cancelTime;
    public string description;
    public List<string> itemIDs;
    
    public Trade(string id, string sellerID, string sellerName, string buyerID, string buyerName, string tradeType, float totalPrice, string currency, string description)
    {
        tradeID = id;
        this.sellerID = sellerID;
        this.sellerName = sellerName;
        this.buyerID = buyerID;
        this.buyerName = buyerName;
        this.tradeType = tradeType;
        status = "pending";
        this.totalPrice = totalPrice;
        this.currency = currency;
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        completeTime = "";
        cancelTime = "";
        this.description = description;
        itemIDs = new List<string>();
    }
    
    public void Accept()
    {
        status = "completed";
        completeTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Cancel()
    {
        status = "cancelled";
        cancelTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddItem(string itemID)
    {
        if (!itemIDs.Contains(itemID))
        {
            itemIDs.Add(itemID);
        }
    }
    
    public void RemoveItem(string itemID)
    {
        if (itemIDs.Contains(itemID))
        {
            itemIDs.Remove(itemID);
        }
    }
}

[System.Serializable]
public class TradeItem
{
    public string itemID;
    public string tradeID;
    public string itemType;
    public string itemValue;
    public string itemName;
    public int quantity;
    public float price;
    public string currency;
    public string status;
    
    public TradeItem(string id, string tradeID, string itemType, string itemValue, string itemName, int quantity, float price, string currency)
    {
        itemID = id;
        this.tradeID = tradeID;
        this.itemType = itemType;
        this.itemValue = itemValue;
        this.itemName = itemName;
        this.quantity = quantity;
        this.price = price;
        this.currency = currency;
        status = "pending";
    }
    
    public void Complete()
    {
        status = "completed";
    }
    
    public void Cancel()
    {
        status = "cancelled";
    }
}

[System.Serializable]
public class TradeEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string tradeID;
    public string description;
    public string timestamp;
    public string status;
    
    public TradeEvent(string id, string eventType, string userID, string tradeID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.tradeID = tradeID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
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
public class TradeSystemDetailedManagerData
{
    public TradeSystemDetailed system;
    
    public TradeSystemDetailedManagerData()
    {
        system = new TradeSystemDetailed("trade_system_detailed", "交易系统详细", "管理交易的详细功能，包括物品交易和货币交易");
    }
}
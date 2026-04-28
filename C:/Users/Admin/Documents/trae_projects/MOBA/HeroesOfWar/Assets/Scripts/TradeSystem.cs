[System.Serializable]
public class Trade
{
    public string tradeID;
    public string senderID;
    public string senderName;
    public string receiverID;
    public string receiverName;
    public List<TradeItem> senderItems;
    public List<TradeItem> receiverItems;
    public string tradeStatus;
    public string sendTime;
    public string acceptTime;
    
    public Trade(string id, string sender, string senderName, string receiver, string receiverName)
    {
        tradeID = id;
        this.senderID = sender;
        this.senderName = senderName;
        this.receiverID = receiver;
        this.receiverName = receiverName;
        senderItems = new List<TradeItem>();
        receiverItems = new List<TradeItem>();
        tradeStatus = "Pending";
        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        acceptTime = "";
    }
    
    public void AddSenderItem(string itemID, string itemType, int quantity)
    {
        TradeItem item = new TradeItem(itemID, itemType, quantity);
        senderItems.Add(item);
    }
    
    public void AddReceiverItem(string itemID, string itemType, int quantity)
    {
        TradeItem item = new TradeItem(itemID, itemType, quantity);
        receiverItems.Add(item);
    }
    
    public void Accept()
    {
        tradeStatus = "Accepted";
        acceptTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
    
    public void Reject()
    {
        tradeStatus = "Rejected";
    }
    
    public void Cancel()
    {
        tradeStatus = "Cancelled";
    }
}

[System.Serializable]
public class TradeItem
{
    public string itemID;
    public string itemType;
    public int quantity;
    
    public TradeItem(string id, string type, int qty)
    {
        itemID = id;
        itemType = type;
        quantity = qty;
    }
}

[System.Serializable]
public class TradeManagerData
{
    public List<Trade> trades;
    
    public TradeManagerData()
    {
        trades = new List<Trade>();
    }
    
    public void AddTrade(Trade trade)
    {
        trades.Add(trade);
    }
    
    public Trade GetTrade(string tradeID)
    {
        return trades.Find(t => t.tradeID == tradeID);
    }
    
    public List<Trade> GetPendingTrades(string playerID)
    {
        return trades.FindAll(t => (t.senderID == playerID || t.receiverID == playerID) && t.tradeStatus == "Pending");
    }
    
    public List<Trade> GetCompletedTrades(string playerID)
    {
        return trades.FindAll(t => (t.senderID == playerID || t.receiverID == playerID) && (t.tradeStatus == "Accepted" || t.tradeStatus == "Rejected" || t.tradeStatus == "Cancelled"));
    }
}
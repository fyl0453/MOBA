using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TradeSystemDetailedManager : MonoBehaviour
{
    public static TradeSystemDetailedManager Instance { get; private set; }
    
    public TradeSystemDetailedManagerData tradeData;
    
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
        LoadTradeData();
        
        if (tradeData == null)
        {
            tradeData = new TradeSystemDetailedManagerData();
            InitializeDefaultTradeSystem();
        }
    }
    
    private void InitializeDefaultTradeSystem()
    {
        // 交易
        Trade trade1 = new Trade("trade_001", "user_001", "张三", "user_002", "李四", "item", 1000, "gold", "出售英雄碎片");
        Trade trade2 = new Trade("trade_002", "user_002", "李四", "user_001", "张三", "item", 500, "gold", "出售皮肤碎片");
        Trade trade3 = new Trade("trade_003", "user_001", "张三", "user_003", "王五", "currency", 50, "diamond", "出售钻石");
        
        tradeData.system.AddTrade(trade1);
        tradeData.system.AddTrade(trade2);
        tradeData.system.AddTrade(trade3);
        
        // 交易物品
        TradeItem item1 = new TradeItem("item_001", "trade_001", "fragment", "hero_fragment", "英雄碎片", 10, 100, "gold");
        TradeItem item2 = new TradeItem("item_002", "trade_002", "fragment", "skin_fragment", "皮肤碎片", 5, 100, "gold");
        TradeItem item3 = new TradeItem("item_003", "trade_003", "currency", "diamond", "钻石", 50, 1, "gold");
        
        tradeData.system.AddTradeItem(item1);
        tradeData.system.AddTradeItem(item2);
        tradeData.system.AddTradeItem(item3);
        
        // 添加物品到交易
        trade1.AddItem("item_001");
        trade2.AddItem("item_002");
        trade3.AddItem("item_003");
        
        // 交易事件
        TradeEvent event1 = new TradeEvent("event_001", "create", "user_001", "trade_001", "创建交易");
        TradeEvent event2 = new TradeEvent("event_002", "accept", "user_002", "trade_001", "接受交易");
        TradeEvent event3 = new TradeEvent("event_003", "create", "user_002", "trade_002", "创建交易");
        
        tradeData.system.AddTradeEvent(event1);
        tradeData.system.AddTradeEvent(event2);
        tradeData.system.AddTradeEvent(event3);
        
        SaveTradeData();
    }
    
    // 创建交易
    public string CreateTrade(string sellerID, string sellerName, string buyerID, string buyerName, string tradeType, float totalPrice, string currency, string description)
    {
        string tradeID = "trade_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Trade trade = new Trade(tradeID, sellerID, sellerName, buyerID, buyerName, tradeType, totalPrice, currency, description);
        tradeData.system.AddTrade(trade);
        
        // 创建交易事件
        CreateTradeEvent("create", sellerID, tradeID, "创建交易");
        CreateTradeEvent("receive", buyerID, tradeID, "收到交易请求");
        
        SaveTradeData();
        Debug.Log("成功创建交易: " + description);
        return tradeID;
    }
    
    // 添加交易物品
    public void AddTradeItem(string tradeID, string itemType, string itemValue, string itemName, int quantity, float price, string currency)
    {
        Trade trade = tradeData.system.GetTrade(tradeID);
        if (trade != null && trade.status == "pending")
        {
            string itemID = "item_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            TradeItem tradeItem = new TradeItem(itemID, tradeID, itemType, itemValue, itemName, quantity, price, currency);
            tradeData.system.AddTradeItem(tradeItem);
            trade.AddItem(itemID);
            
            // 更新总价格
            trade.totalPrice += price * quantity;
            
            SaveTradeData();
            Debug.Log("成功添加交易物品: " + itemName);
        }
        else
        {
            Debug.LogError("交易不存在或已完成");
        }
    }
    
    // 接受交易
    public void AcceptTrade(string tradeID, string userID)
    {
        Trade trade = tradeData.system.GetTrade(tradeID);
        if (trade != null && trade.status == "pending" && trade.buyerID == userID)
        {
            // 检查买家货币
            // if (!CheckCurrency(userID, trade.currency, trade.totalPrice))
            // {
            //     Debug.LogError("货币不足");
            //     return;
            // }
            
            // 检查卖家物品
            // if (!CheckItems(trade.sellerID, trade.itemIDs))
            // {
            //     Debug.LogError("物品不足");
            //     return;
            // }
            
            // 扣除买家货币
            // DeductCurrency(userID, trade.currency, trade.totalPrice);
            
            // 扣除卖家物品
            // DeductItems(trade.sellerID, trade.itemIDs);
            
            // 发放卖家货币
            // AddCurrency(trade.sellerID, trade.currency, trade.totalPrice);
            
            // 发放买家物品
            // AddItems(userID, trade.itemIDs);
            
            // 完成交易
            trade.Accept();
            
            // 完成交易物品
            List<TradeItem> items = tradeData.system.GetTradeItemsByTrade(tradeID);
            foreach (TradeItem item in items)
            {
                item.Complete();
            }
            
            // 创建交易事件
            CreateTradeEvent("accept", userID, tradeID, "接受交易");
            CreateTradeEvent("complete", trade.sellerID, tradeID, "交易完成");
            
            SaveTradeData();
            Debug.Log("成功接受交易");
        }
        else
        {
            Debug.LogError("交易不存在或已完成或无权操作");
        }
    }
    
    // 取消交易
    public void CancelTrade(string tradeID, string userID)
    {
        Trade trade = tradeData.system.GetTrade(tradeID);
        if (trade != null && trade.status == "pending" && (trade.sellerID == userID || trade.buyerID == userID))
        {
            trade.Cancel();
            
            // 取消交易物品
            List<TradeItem> items = tradeData.system.GetTradeItemsByTrade(tradeID);
            foreach (TradeItem item in items)
            {
                item.Cancel();
            }
            
            // 创建交易事件
            CreateTradeEvent("cancel", userID, tradeID, "取消交易");
            
            SaveTradeData();
            Debug.Log("成功取消交易");
        }
        else
        {
            Debug.LogError("交易不存在或已完成或无权操作");
        }
    }
    
    // 获取用户交易
    public List<Trade> GetUserTrades(string userID, string status = null)
    {
        List<Trade> trades = tradeData.system.GetTradesByUser(userID);
        
        if (!string.IsNullOrEmpty(status))
        {
            trades = trades.FindAll(t => t.status == status);
        }
        
        // 按创建时间排序
        trades.Sort((a, b) => b.createTime.CompareTo(a.createTime));
        
        return trades;
    }
    
    // 获取交易物品
    public List<TradeItem> GetTradeItems(string tradeID)
    {
        return tradeData.system.GetTradeItemsByTrade(tradeID);
    }
    
    // 交易事件管理
    public string CreateTradeEvent(string eventType, string userID, string tradeID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TradeEvent tradeEvent = new TradeEvent(eventID, eventType, userID, tradeID, description);
        tradeData.system.AddTradeEvent(tradeEvent);
        SaveTradeData();
        Debug.Log("成功创建交易事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        TradeEvent tradeEvent = tradeData.system.GetTradeEvent(eventID);
        if (tradeEvent != null)
        {
            tradeEvent.MarkAsCompleted();
            SaveTradeData();
            Debug.Log("成功标记交易事件为完成");
        }
        else
        {
            Debug.LogError("交易事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        TradeEvent tradeEvent = tradeData.system.GetTradeEvent(eventID);
        if (tradeEvent != null)
        {
            tradeEvent.MarkAsFailed();
            SaveTradeData();
            Debug.Log("成功标记交易事件为失败");
        }
        else
        {
            Debug.LogError("交易事件不存在: " + eventID);
        }
    }
    
    public List<TradeEvent> GetTradeEvents(string userID)
    {
        return tradeData.system.GetTradeEventsByUser(userID);
    }
    
    // 数据持久化
    public void SaveTradeData()
    {
        string path = Application.dataPath + "/Data/trade_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, tradeData);
        stream.Close();
    }
    
    public void LoadTradeData()
    {
        string path = Application.dataPath + "/Data/trade_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            tradeData = (TradeSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            tradeData = new TradeSystemDetailedManagerData();
        }
    }
}
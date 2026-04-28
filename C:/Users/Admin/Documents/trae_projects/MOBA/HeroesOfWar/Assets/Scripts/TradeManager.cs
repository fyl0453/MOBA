using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance { get; private set; }
    
    public TradeManagerData tradeData;
    
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
            tradeData = new TradeManagerData();
        }
    }
    
    public Trade CreateTrade(string receiverID, string receiverName)
    {
        string tradeID = "trade_" + System.DateTime.Now.Ticks;
        string senderID = ProfileManager.Instance.currentProfile.playerID;
        string senderName = ProfileManager.Instance.currentProfile.playerName;
        
        Trade trade = new Trade(tradeID, senderID, senderName, receiverID, receiverName);
        tradeData.AddTrade(trade);
        SaveTradeData();
        
        return trade;
    }
    
    public void AddTradeItem(string tradeID, bool isSender, string itemID, string itemType, int quantity)
    {
        Trade trade = tradeData.GetTrade(tradeID);
        if (trade != null && trade.tradeStatus == "Pending")
        {
            if (isSender)
            {
                trade.AddSenderItem(itemID, itemType, quantity);
            }
            else
            {
                trade.AddReceiverItem(itemID, itemType, quantity);
            }
            SaveTradeData();
        }
    }
    
    public void SendTrade(string tradeID)
    {
        Trade trade = tradeData.GetTrade(tradeID);
        if (trade != null && trade.tradeStatus == "Pending")
        {
            // 这里可以添加发送交易请求的逻辑
            SaveTradeData();
        }
    }
    
    public void AcceptTrade(string tradeID)
    {
        Trade trade = tradeData.GetTrade(tradeID);
        if (trade != null && trade.tradeStatus == "Pending" && trade.receiverID == ProfileManager.Instance.currentProfile.playerID)
        {
            // 检查双方是否有足够的物品
            if (CheckTradeValidity(trade))
            {
                trade.Accept();
                ExecuteTrade(trade);
                SaveTradeData();
            }
        }
    }
    
    public void RejectTrade(string tradeID)
    {
        Trade trade = tradeData.GetTrade(tradeID);
        if (trade != null && trade.tradeStatus == "Pending" && trade.receiverID == ProfileManager.Instance.currentProfile.playerID)
        {
            trade.Reject();
            SaveTradeData();
        }
    }
    
    public void CancelTrade(string tradeID)
    {
        Trade trade = tradeData.GetTrade(tradeID);
        if (trade != null && trade.tradeStatus == "Pending" && trade.senderID == ProfileManager.Instance.currentProfile.playerID)
        {
            trade.Cancel();
            SaveTradeData();
        }
    }
    
    private bool CheckTradeValidity(Trade trade)
    {
        // 检查发送方是否有足够的物品
        foreach (TradeItem item in trade.senderItems)
        {
            if (item.itemType == "Item")
            {
                if (!InventoryManager.Instance.HasItem(item.itemID, item.quantity))
                {
                    return false;
                }
            }
            // 可以添加其他类型物品的检查
        }
        
        // 检查接收方是否有足够的物品
        foreach (TradeItem item in trade.receiverItems)
        {
            if (item.itemType == "Item")
            {
                if (!InventoryManager.Instance.HasItem(item.itemID, item.quantity))
                {
                    return false;
                }
            }
            // 可以添加其他类型物品的检查
        }
        
        return true;
    }
    
    private void ExecuteTrade(Trade trade)
    {
        // 执行交易，交换物品
        foreach (TradeItem item in trade.senderItems)
        {
            if (item.itemType == "Item")
            {
                InventoryManager.Instance.RemoveItemFromInventory(item.itemID, item.quantity);
                // 这里需要添加给接收方添加物品的逻辑
            }
            // 可以添加其他类型物品的处理
        }
        
        foreach (TradeItem item in trade.receiverItems)
        {
            if (item.itemType == "Item")
            {
                InventoryManager.Instance.RemoveItemFromInventory(item.itemID, item.quantity);
                // 这里需要添加给发送方添加物品的逻辑
            }
            // 可以添加其他类型物品的处理
        }
    }
    
    public List<Trade> GetPendingTrades()
    {
        return tradeData.GetPendingTrades(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public List<Trade> GetCompletedTrades()
    {
        return tradeData.GetCompletedTrades(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public Trade GetTrade(string tradeID)
    {
        return tradeData.GetTrade(tradeID);
    }
    
    public void SaveTradeData()
    {
        string path = Application.dataPath + "/Data/trade_data.dat";
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
        string path = Application.dataPath + "/Data/trade_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            tradeData = (TradeManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            tradeData = new TradeManagerData();
        }
    }
}
[System.Serializable]
public class RechargeItem
{
    public string itemID;
    public string itemName;
    public string itemDescription;
    public int gemsAmount;
    public int price;
    public string priceType;
    public bool isLimited;
    public string limitedTime;
    public string bonusDescription;
    public int bonusGems;
    
    public RechargeItem(string id, string name, int gems, int price, string currency = "CNY")
    {
        itemID = id;
        itemName = name;
        gemsAmount = gems;
        this.price = price;
        priceType = currency;
        isLimited = false;
        limitedTime = "";
        bonusDescription = "";
        bonusGems = 0;
    }
    
    public void SetLimited(string time, string bonus)
    {
        isLimited = true;
        limitedTime = time;
        bonusDescription = bonus;
    }
    
    public void SetBonus(int bonusGemsAmount)
    {
        bonusGems = bonusGemsAmount;
        if (bonusGems > 0)
        {
            bonusDescription = "赠送" + bonusGems + "钻石";
        }
    }
}

[System.Serializable]
public class RechargeRecord
{
    public string recordID;
    public string playerID;
    public string itemID;
    public string itemName;
    public int gemsAmount;
    public int bonusGems;
    public int actualGems;
    public int price;
    public string priceType;
    public string purchaseTime;
    public string orderID;
    
    public RechargeRecord(string id, string player, string itemID, string itemName, int gems, int bonus, int actual, int price, string currency)
    {
        recordID = id;
        playerID = player;
        this.itemID = itemID;
        this.itemName = itemName;
        gemsAmount = gems;
        bonusGems = bonus;
        actualGems = actual;
        this.price = price;
        priceType = currency;
        purchaseTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        orderID = "order_" + System.DateTime.Now.Ticks;
    }
}

[System.Serializable]
public class RechargeManagerData
{
    public List<RechargeItem> rechargeItems;
    public List<RechargeRecord> rechargeRecords;
    public int totalRechargeAmount;
    
    public RechargeManagerData()
    {
        rechargeItems = new List<RechargeItem>();
        rechargeRecords = new List<RechargeRecord>();
        totalRechargeAmount = 0;
    }
    
    public void AddRechargeItem(RechargeItem item)
    {
        rechargeItems.Add(item);
    }
    
    public void AddRechargeRecord(RechargeRecord record)
    {
        rechargeRecords.Add(record);
    }
    
    public RechargeItem GetRechargeItem(string itemID)
    {
        return rechargeItems.Find(i => i.itemID == itemID);
    }
    
    public List<RechargeRecord> GetRechargeRecordsByPlayer(string playerID)
    {
        return rechargeRecords.FindAll(r => r.playerID == playerID);
    }
}
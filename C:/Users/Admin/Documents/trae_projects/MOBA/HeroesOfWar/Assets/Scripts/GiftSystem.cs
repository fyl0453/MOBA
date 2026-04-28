[System.Serializable]
public class GiftSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Gift> gifts;
    public List<GiftRecord> giftRecords;
    
    public GiftSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        gifts = new List<Gift>();
        giftRecords = new List<GiftRecord>();
    }
    
    public void AddGift(Gift gift)
    {
        gifts.Add(gift);
    }
    
    public void AddGiftRecord(GiftRecord record)
    {
        giftRecords.Add(record);
    }
    
    public Gift GetGift(string giftID)
    {
        return gifts.Find(g => g.giftID == giftID);
    }
    
    public List<GiftRecord> GetGiftRecordsBySender(string senderID)
    {
        return giftRecords.FindAll(r => r.senderID == senderID);
    }
    
    public List<GiftRecord> GetGiftRecordsByReceiver(string receiverID)
    {
        return giftRecords.FindAll(r => r.receiverID == receiverID);
    }
}

[System.Serializable]
public class Gift
{
    public string giftID;
    public string giftName;
    public string giftDescription;
    public int price;
    public string giftType;
    public string giftIcon;
    public string giftEffect;
    
    public Gift(string id, string name, string desc, int price, string type)
    {
        giftID = id;
        giftName = name;
        giftDescription = desc;
        this.price = price;
        giftType = type;
        giftIcon = "";
        giftEffect = "";
    }
}

[System.Serializable]
public class GiftRecord
{
    public string recordID;
    public string senderID;
    public string senderName;
    public string receiverID;
    public string receiverName;
    public string giftID;
    public string giftName;
    public int giftCount;
    public string sendTime;
    public string message;
    
    public GiftRecord(string id, string senderID, string senderName, string receiverID, string receiverName, string giftID, string giftName, int count, string message = "")
    {
        recordID = id;
        this.senderID = senderID;
        this.senderName = senderName;
        this.receiverID = receiverID;
        this.receiverName = receiverName;
        this.giftID = giftID;
        this.giftName = giftName;
        giftCount = count;
        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.message = message;
    }
}

[System.Serializable]
public class GiftManagerData
{
    public GiftSystem system;
    
    public GiftManagerData()
    {
        system = new GiftSystem("gift_system", "礼物系统", "管理玩家之间的礼物赠送");
    }
}
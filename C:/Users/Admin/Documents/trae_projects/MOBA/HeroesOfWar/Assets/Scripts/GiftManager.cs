using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GiftManager : MonoBehaviour
{
    public static GiftManager Instance { get; private set; }
    
    public GiftManagerData giftData;
    
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
        LoadGiftData();
        
        if (giftData == null)
        {
            giftData = new GiftManagerData();
            InitializeDefaultGifts();
        }
    }
    
    private void InitializeDefaultGifts()
    {
        // 创建默认礼物
        Gift gift1 = new Gift("gift_rose", "玫瑰", "表达爱意的玫瑰", 10, "Flower");
        Gift gift2 = new Gift("gift_diamond", "钻石", "珍贵的钻石", 100, "Gem");
        Gift gift3 = new Gift("gift_cake", "蛋糕", "美味的蛋糕", 50, "Food");
        Gift gift4 = new Gift("gift_heart", "爱心", "表达心意的爱心", 20, "Love");
        
        giftData.system.AddGift(gift1);
        giftData.system.AddGift(gift2);
        giftData.system.AddGift(gift3);
        giftData.system.AddGift(gift4);
        
        SaveGiftData();
    }
    
    public bool SendGift(string senderID, string senderName, string receiverID, string receiverName, string giftID, int count, string message = "")
    {
        Gift gift = giftData.system.GetGift(giftID);
        if (gift != null)
        {
            // 检查发送者是否有足够的钻石
            int totalPrice = gift.price * count;
            if (ProfileManager.Instance.currentProfile.gems >= totalPrice)
            {
                // 扣除钻石
                ProfileManager.Instance.currentProfile.gems -= totalPrice;
                
                // 创建礼物记录
                string recordID = System.Guid.NewGuid().ToString();
                GiftRecord record = new GiftRecord(recordID, senderID, senderName, receiverID, receiverName, giftID, gift.giftName, count, message);
                giftData.system.AddGiftRecord(record);
                
                // 保存数据
                ProfileManager.Instance.SaveProfile();
                SaveGiftData();
                
                Debug.Log($"成功发送礼物: {gift.giftName} x{count} 给 {receiverName}");
                return true;
            }
            else
            {
                Debug.Log("钻石不足，无法发送礼物");
                return false;
            }
        }
        else
        {
            Debug.Log("礼物不存在");
            return false;
        }
    }
    
    public List<Gift> GetAllGifts()
    {
        return giftData.system.gifts;
    }
    
    public Gift GetGift(string giftID)
    {
        return giftData.system.GetGift(giftID);
    }
    
    public List<GiftRecord> GetGiftRecordsBySender(string senderID, int limit = 50)
    {
        List<GiftRecord> records = giftData.system.GetGiftRecordsBySender(senderID);
        records.Sort((a, b) => b.sendTime.CompareTo(a.sendTime));
        return records.GetRange(0, Mathf.Min(limit, records.Count));
    }
    
    public List<GiftRecord> GetGiftRecordsByReceiver(string receiverID, int limit = 50)
    {
        List<GiftRecord> records = giftData.system.GetGiftRecordsByReceiver(receiverID);
        records.Sort((a, b) => b.sendTime.CompareTo(a.sendTime));
        return records.GetRange(0, Mathf.Min(limit, records.Count));
    }
    
    public void AddGift(string name, string description, int price, string type)
    {
        string giftID = System.Guid.NewGuid().ToString();
        Gift newGift = new Gift(giftID, name, description, price, type);
        giftData.system.AddGift(newGift);
        SaveGiftData();
        Debug.Log($"成功添加礼物: {name}");
    }
    
    public void SaveGiftData()
    {
        string path = Application.dataPath + "/Data/gift_system_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, giftData);
        stream.Close();
    }
    
    public void LoadGiftData()
    {
        string path = Application.dataPath + "/Data/gift_system_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            giftData = (GiftManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            giftData = new GiftManagerData();
        }
    }
}
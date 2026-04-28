using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RechargeManager : MonoBehaviour
{
    public static RechargeManager Instance { get; private set; }
    
    public RechargeManagerData rechargeData;
    
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
        LoadRechargeData();
        
        if (rechargeData == null)
        {
            rechargeData = new RechargeManagerData();
            InitializeDefaultRechargeItems();
        }
    }
    
    private void InitializeDefaultRechargeItems()
    {
        RechargeItem item1 = new RechargeItem("recharge_60", "60钻石", 60, 6);
        item1.SetBonus(0);
        rechargeData.AddRechargeItem(item1);
        
        RechargeItem item2 = new RechargeItem("recharge_300", "300钻石", 300, 30);
        item2.SetBonus(30);
        rechargeData.AddRechargeItem(item2);
        
        RechargeItem item3 = new RechargeItem("recharge_980", "980钻石", 980, 98);
        item3.SetBonus(98);
        rechargeData.AddRechargeItem(item3);
        
        RechargeItem item4 = new RechargeItem("recharge_1980", "1980钻石", 1980, 198);
        item4.SetBonus(298);
        rechargeData.AddRechargeItem(item4);
        
        RechargeItem item5 = new RechargeItem("recharge_3980", "3980钻石", 3980, 398);
        item5.SetBonus(796);
        rechargeData.AddRechargeItem(item5);
        
        RechargeItem item6 = new RechargeItem("recharge_6480", "6480钻石", 6480, 648);
        item6.SetBonus(1944);
        rechargeData.AddRechargeItem(item6);
        
        SaveRechargeData();
    }
    
    public void PurchaseRechargeItem(string itemID)
    {
        RechargeItem item = rechargeData.GetRechargeItem(itemID);
        if (item != null)
        {
            int totalGems = item.gemsAmount + item.bonusGems;
            
            string recordID = "record_" + System.DateTime.Now.Ticks;
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            
            RechargeRecord record = new RechargeRecord(recordID, playerID, item.itemID, item.itemName, item.gemsAmount, item.bonusGems, totalGems, item.price, item.priceType);
            rechargeData.AddRechargeRecord(record);
            
            rechargeData.totalRechargeAmount += item.price;
            
            ProfileManager.Instance.currentProfile.gems += totalGems;
            ProfileManager.Instance.SaveProfile();
            
            VIPManager.Instance.AddVIPPoints(item.price);
            
            SaveRechargeData();
        }
    }
    
    public bool SimulatePurchase(string itemID)
    {
        RechargeItem item = rechargeData.GetRechargeItem(itemID);
        if (item != null)
        {
            int totalGems = item.gemsAmount + item.bonusGems;
            
            string recordID = "record_" + System.DateTime.Now.Ticks;
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            
            RechargeRecord record = new RechargeRecord(recordID, playerID, item.itemID, item.itemName, item.gemsAmount, item.bonusGems, totalGems, item.price, item.priceType);
            rechargeData.AddRechargeRecord(record);
            
            rechargeData.totalRechargeAmount += item.price;
            
            ProfileManager.Instance.currentProfile.gems += totalGems;
            ProfileManager.Instance.SaveProfile();
            
            VIPManager.Instance.AddVIPPoints(item.price);
            
            SaveRechargeData();
            return true;
        }
        return false;
    }
    
    public List<RechargeItem> GetAvailableRechargeItems()
    {
        return rechargeData.rechargeItems;
    }
    
    public List<RechargeRecord> GetPlayerRechargeRecords()
    {
        return rechargeData.GetRechargeRecordsByPlayer(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public int GetTotalRechargeAmount()
    {
        return rechargeData.totalRechargeAmount;
    }
    
    public void SaveRechargeData()
    {
        string path = Application.dataPath + "/Data/recharge_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, rechargeData);
        stream.Close();
    }
    
    public void LoadRechargeData()
    {
        string path = Application.dataPath + "/Data/recharge_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            rechargeData = (RechargeManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            rechargeData = new RechargeManagerData();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class RechargePackage
{
    public string PackageID;
    public string PackageName;
    public int CurrencyAmount;
    public int RealPrice;
    public string CurrencyType;
    public int BonusAmount;
    public bool IsSpecialOffer;
    public string IconName;
    public string Description;
    public int PurchaseLimit;
    public DateTime StartTime;
    public DateTime EndTime;
    public bool IsActive;

    public RechargePackage(string packageID, string packageName, int currencyAmount, int realPrice, string currencyType, int bonusAmount = 0, int purchaseLimit = -1)
    {
        PackageID = packageID;
        PackageName = packageName;
        CurrencyAmount = currencyAmount;
        RealPrice = realPrice;
        CurrencyType = currencyType;
        BonusAmount = bonusAmount;
        IsSpecialOffer = false;
        IconName = "";
        Description = "";
        PurchaseLimit = purchaseLimit;
        StartTime = DateTime.Now;
        EndTime = DateTime.MaxValue;
        IsActive = true;
    }
}

[Serializable]
public class RechargeRecord
{
    public string RecordID;
    public string PlayerID;
    public string PackageID;
    public string PackageName;
    public int RealPrice;
    public int CurrencyAmount;
    public int BonusAmount;
    public string PaymentMethod;
    public string TransactionID;
    public string OrderID;
    public int Status;
    public DateTime CreateTime;
    public DateTime CompleteTime;
    public string ErrorMessage;

    public RechargeRecord(string recordID, string playerID, string packageID, string packageName, int realPrice, int currencyAmount, int bonusAmount, string paymentMethod, string transactionID)
    {
        RecordID = recordID;
        PlayerID = playerID;
        PackageID = packageID;
        PackageName = packageName;
        RealPrice = realPrice;
        CurrencyAmount = currencyAmount;
        BonusAmount = bonusAmount;
        PaymentMethod = paymentMethod;
        TransactionID = transactionID;
        OrderID = "";
        Status = 0;
        CreateTime = DateTime.Now;
        CompleteTime = DateTime.MinValue;
        ErrorMessage = "";
    }
}

[Serializable]
public class PaymentMethod
{
    public string MethodID;
    public string MethodName;
    public string MethodType;
    public string Description;
    public string IconName;
    public bool IsActive;
    public int Priority;

    public PaymentMethod(string methodID, string methodName, string methodType, string description)
    {
        MethodID = methodID;
        MethodName = methodName;
        MethodType = methodType;
        Description = description;
        IconName = "";
        IsActive = true;
        Priority = 1;
    }
}

[Serializable]
public class PlayerRechargeData
{
    public string PlayerID;
    public List<RechargeRecord> RechargeRecords;
    public int TotalRechargeAmount;
    public int MonthlyRechargeAmount;
    public int DailyRechargeAmount;
    public DateTime LastRechargeTime;

    public PlayerRechargeData(string playerID)
    {
        PlayerID = playerID;
        RechargeRecords = new List<RechargeRecord>();
        TotalRechargeAmount = 0;
        MonthlyRechargeAmount = 0;
        DailyRechargeAmount = 0;
        LastRechargeTime = DateTime.MinValue;
    }
}

[Serializable]
public class RechargeSystemData
{
    public List<RechargePackage> RechargePackages;
    public List<PaymentMethod> PaymentMethods;
    public Dictionary<string, PlayerRechargeData> PlayerRechargeData;
    public int TotalRechargeCount;
    public int TotalRechargeAmount;
    public DateTime LastUpdateTime;

    public RechargeSystemData()
    {
        RechargePackages = new List<RechargePackage>();
        PaymentMethods = new List<PaymentMethod>();
        PlayerRechargeData = new Dictionary<string, PlayerRechargeData>();
        TotalRechargeCount = 0;
        TotalRechargeAmount = 0;
        LastUpdateTime = DateTime.Now;
        InitializeDefaultPackages();
        InitializeDefaultPaymentMethods();
    }

    private void InitializeDefaultPackages()
    {
        RechargePackage smallPackage = new RechargePackage("package_small", "小礼包", 100, 6, "diamond", 10);
        RechargePackages.Add(smallPackage);

        RechargePackage mediumPackage = new RechargePackage("package_medium", "中礼包", 500, 30, "diamond", 50);
        RechargePackages.Add(mediumPackage);

        RechargePackage largePackage = new RechargePackage("package_large", "大礼包", 1000, 60, "diamond", 150, 1);
        largePackage.IsSpecialOffer = true;
        RechargePackages.Add(largePackage);

        RechargePackage xlargePackage = new RechargePackage("package_xlarge", "超大礼包", 2000, 128, "diamond", 300);
        RechargePackages.Add(xlargePackage);

        RechargePackage maxPackage = new RechargePackage("package_max", "豪华礼包", 5000, 328, "diamond", 800);
        RechargePackages.Add(maxPackage);
    }

    private void InitializeDefaultPaymentMethods()
    {
        PaymentMethod wechatPay = new PaymentMethod("pay_wechat", "微信支付", "online", "微信扫码支付");
        PaymentMethods.Add(wechatPay);

        PaymentMethod alipay = new PaymentMethod("pay_alipay", "支付宝", "online", "支付宝扫码支付");
        PaymentMethods.Add(alipay);

        PaymentMethod applePay = new PaymentMethod("pay_apple", "Apple Pay", "online", "苹果支付");
        PaymentMethods.Add(applePay);

        PaymentMethod googlePay = new PaymentMethod("pay_google", "Google Pay", "online", "谷歌支付");
        PaymentMethods.Add(googlePay);
    }

    public void AddRechargePackage(RechargePackage package)
    {
        RechargePackages.Add(package);
    }

    public void AddPaymentMethod(PaymentMethod method)
    {
        PaymentMethods.Add(method);
    }

    public void AddPlayerRechargeData(string playerID, PlayerRechargeData data)
    {
        PlayerRechargeData[playerID] = data;
    }
}

[Serializable]
public class RechargeEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string PackageID;
    public string EventData;

    public RechargeEvent(string eventID, string eventType, string playerID, string packageID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        PackageID = packageID;
        EventData = eventData;
    }
}

public class RechargeSystemDataManager
{
    private static RechargeSystemDataManager _instance;
    public static RechargeSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RechargeSystemDataManager();
            }
            return _instance;
        }
    }

    public RechargeSystemData rechargeData;
    private List<RechargeEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private RechargeSystemDataManager()
    {
        rechargeData = new RechargeSystemData();
        recentEvents = new List<RechargeEvent>();
        LoadRechargeData();
    }

    public void SaveRechargeData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "RechargeSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, rechargeData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存充值系统数据失败: " + e.Message);
        }
    }

    public void LoadRechargeData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "RechargeSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    rechargeData = (RechargeSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载充值系统数据失败: " + e.Message);
            rechargeData = new RechargeSystemData();
        }
    }

    public void CreateRechargeEvent(string eventType, string playerID, string packageID, string eventData)
    {
        string eventID = "recharge_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RechargeEvent rechargeEvent = new RechargeEvent(eventID, eventType, playerID, packageID, eventData);
        recentEvents.Add(rechargeEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<RechargeEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using System;
using System.Collections.Generic;

public class RechargeSystemDetailedManager
{
    private static RechargeSystemDetailedManager _instance;
    public static RechargeSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RechargeSystemDetailedManager();
            }
            return _instance;
        }
    }

    private RechargeSystemData rechargeData;
    private RechargeSystemDataManager dataManager;

    private RechargeSystemDetailedManager()
    {
        dataManager = RechargeSystemDataManager.Instance;
        rechargeData = dataManager.rechargeData;
    }

    public void InitializePlayerRechargeData(string playerID)
    {
        if (!rechargeData.PlayerRechargeData.ContainsKey(playerID))
        {
            PlayerRechargeData playerData = new PlayerRechargeData(playerID);
            rechargeData.AddPlayerRechargeData(playerID, playerData);
            dataManager.SaveRechargeData();
            Debug.Log("初始化充值数据成功");
        }
    }

    public string CreateRechargeOrder(string playerID, string packageID, string paymentMethod)
    {
        RechargePackage package = GetRechargePackage(packageID);
        if (package == null || !package.IsActive)
        {
            Debug.LogError("充值套餐不存在或已下架");
            return "";
        }

        InitializePlayerRechargeData(playerID);
        PlayerRechargeData playerData = rechargeData.PlayerRechargeData[playerID];

        if (package.PurchaseLimit > 0)
        {
            int purchaseCount = playerData.RechargeRecords.FindAll(r => r.PackageID == packageID && r.Status == 1).Count;
            if (purchaseCount >= package.PurchaseLimit)
            {
                Debug.LogError("充值套餐已达到购买上限");
                return "";
            }
        }

        string recordID = "recharge_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        string transactionID = "trans_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RechargeRecord record = new RechargeRecord(recordID, playerID, packageID, package.PackageName, package.RealPrice, package.CurrencyAmount, package.BonusAmount, paymentMethod, transactionID);
        playerData.RechargeRecords.Add(record);
        rechargeData.TotalRechargeCount++;
        
        dataManager.CreateRechargeEvent("recharge_create", playerID, packageID, "创建充值订单");
        dataManager.SaveRechargeData();
        Debug.Log("创建充值订单成功: " + package.PackageName);
        return recordID;
    }

    public void ProcessRechargePayment(string recordID, bool isSuccess, string errorMessage = "")
    {
        RechargeRecord record = GetRechargeRecord(recordID);
        if (record != null && record.Status == 0)
        {
            if (isSuccess)
            {
                record.Status = 1;
                record.CompleteTime = DateTime.Now;
                record.ErrorMessage = "";
                
                PlayerRechargeData playerData = rechargeData.PlayerRechargeData[record.PlayerID];
                playerData.TotalRechargeAmount += record.RealPrice;
                playerData.MonthlyRechargeAmount += record.RealPrice;
                playerData.DailyRechargeAmount += record.RealPrice;
                playerData.LastRechargeTime = DateTime.Now;
                
                rechargeData.TotalRechargeAmount += record.RealPrice;
                
                int totalCurrency = record.CurrencyAmount + record.BonusAmount;
                AddCurrencyToPlayer(record.PlayerID, record.PackageName, totalCurrency);
                
                dataManager.CreateRechargeEvent("recharge_success", record.PlayerID, record.PackageID, "充值成功");
                Debug.Log("充值成功: " + record.PackageName);
            }
            else
            {
                record.Status = 2;
                record.ErrorMessage = errorMessage;
                dataManager.CreateRechargeEvent("recharge_failed", record.PlayerID, record.PackageID, "充值失败: " + errorMessage);
                Debug.Log("充值失败: " + errorMessage);
            }
            dataManager.SaveRechargeData();
        }
    }

    private void AddCurrencyToPlayer(string playerID, string packageName, int amount)
    {
        
        ShopSystemDetailedManager.Instance.AddCurrency(playerID, "diamond", amount);
    }

    public RechargePackage GetRechargePackage(string packageID)
    {
        return rechargeData.RechargePackages.Find(p => p.PackageID == packageID);
    }

    public List<RechargePackage> GetAllRechargePackages()
    {
        return rechargeData.RechargePackages;
    }

    public List<RechargePackage> GetActiveRechargePackages()
    {
        return rechargeData.RechargePackages.FindAll(p => p.IsActive);
    }

    public List<RechargePackage> GetSpecialOfferPackages()
    {
        return rechargeData.RechargePackages.FindAll(p => p.IsSpecialOffer && p.IsActive);
    }

    public RechargeRecord GetRechargeRecord(string recordID)
    {
        foreach (PlayerRechargeData playerData in rechargeData.PlayerRechargeData.Values)
        {
            RechargeRecord record = playerData.RechargeRecords.Find(r => r.RecordID == recordID);
            if (record != null)
            {
                return record;
            }
        }
        return null;
    }

    public List<RechargeRecord> GetPlayerRechargeRecords(string playerID, int count = 50)
    {
        if (rechargeData.PlayerRechargeData.ContainsKey(playerID))
        {
            List<RechargeRecord> records = rechargeData.PlayerRechargeData[playerID].RechargeRecords;
            records.Sort((a, b) => b.CreateTime.CompareTo(a.CreateTime));
            if (count < records.Count)
            {
                return records.GetRange(0, count);
            }
            return records;
        }
        return new List<RechargeRecord>();
    }

    public List<PaymentMethod> GetAllPaymentMethods()
    {
        return rechargeData.PaymentMethods;
    }

    public List<PaymentMethod> GetActivePaymentMethods()
    {
        return rechargeData.PaymentMethods.FindAll(m => m.IsActive);
    }

    public PaymentMethod GetPaymentMethod(string methodID)
    {
        return rechargeData.PaymentMethods.Find(m => m.MethodID == methodID);
    }

    public void AddRechargePackage(string packageName, int currencyAmount, int realPrice, string currencyType, int bonusAmount = 0, int purchaseLimit = -1, bool isSpecialOffer = false)
    {
        string packageID = "package_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RechargePackage package = new RechargePackage(packageID, packageName, currencyAmount, realPrice, currencyType, bonusAmount, purchaseLimit);
        package.IsSpecialOffer = isSpecialOffer;
        rechargeData.AddRechargePackage(package);
        dataManager.SaveRechargeData();
        Debug.Log("添加充值套餐成功: " + packageName);
    }

    public void UpdateRechargePackage(string packageID, string packageName, int currencyAmount, int realPrice, int bonusAmount, int purchaseLimit, bool isSpecialOffer, bool isActive)
    {
        RechargePackage package = GetRechargePackage(packageID);
        if (package != null)
        {
            package.PackageName = packageName;
            package.CurrencyAmount = currencyAmount;
            package.RealPrice = realPrice;
            package.BonusAmount = bonusAmount;
            package.PurchaseLimit = purchaseLimit;
            package.IsSpecialOffer = isSpecialOffer;
            package.IsActive = isActive;
            dataManager.SaveRechargeData();
            Debug.Log("更新充值套餐成功: " + packageName);
        }
    }

    public void AddPaymentMethod(string methodName, string methodType, string description)
    {
        string methodID = "pay_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PaymentMethod method = new PaymentMethod(methodID, methodName, methodType, description);
        rechargeData.AddPaymentMethod(method);
        dataManager.SaveRechargeData();
        Debug.Log("添加支付方式成功: " + methodName);
    }

    public void UpdatePaymentMethod(string methodID, string methodName, string methodType, string description, bool isActive)
    {
        PaymentMethod method = GetPaymentMethod(methodID);
        if (method != null)
        {
            method.MethodName = methodName;
            method.MethodType = methodType;
            method.Description = description;
            method.IsActive = isActive;
            dataManager.SaveRechargeData();
            Debug.Log("更新支付方式成功: " + methodName);
        }
    }

    public int GetPlayerTotalRecharge(string playerID)
    {
        if (rechargeData.PlayerRechargeData.ContainsKey(playerID))
        {
            return rechargeData.PlayerRechargeData[playerID].TotalRechargeAmount;
        }
        return 0;
    }

    public int GetPlayerMonthlyRecharge(string playerID)
    {
        if (rechargeData.PlayerRechargeData.ContainsKey(playerID))
        {
            return rechargeData.PlayerRechargeData[playerID].MonthlyRechargeAmount;
        }
        return 0;
    }

    public int GetPlayerDailyRecharge(string playerID)
    {
        if (rechargeData.PlayerRechargeData.ContainsKey(playerID))
        {
            return rechargeData.PlayerRechargeData[playerID].DailyRechargeAmount;
        }
        return 0;
    }

    public void ResetMonthlyRecharge()
    {
        foreach (PlayerRechargeData playerData in rechargeData.PlayerRechargeData.Values)
        {
            playerData.MonthlyRechargeAmount = 0;
        }
        dataManager.SaveRechargeData();
        Debug.Log("重置月充值成功");
    }

    public void ResetDailyRecharge()
    {
        foreach (PlayerRechargeData playerData in rechargeData.PlayerRechargeData.Values)
        {
            playerData.DailyRechargeAmount = 0;
        }
        dataManager.SaveRechargeData();
        Debug.Log("重置日充值成功");
    }

    public void CleanupExpiredPackages()
    {
        DateTime now = DateTime.Now;
        List<RechargePackage> expiredPackages = new List<RechargePackage>();
        foreach (RechargePackage package in rechargeData.RechargePackages)
        {
            if (package.EndTime < now)
            {
                package.IsActive = false;
                expiredPackages.Add(package);
            }
        }
        
        if (expiredPackages.Count > 0)
        {
            dataManager.CreateRechargeEvent("package_cleanup", "system", "", "清理过期套餐: " + expiredPackages.Count);
            dataManager.SaveRechargeData();
            Debug.Log("清理过期套餐成功: " + expiredPackages.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveRechargeData();
    }

    public void LoadData()
    {
        dataManager.LoadRechargeData();
    }

    public List<RechargeEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
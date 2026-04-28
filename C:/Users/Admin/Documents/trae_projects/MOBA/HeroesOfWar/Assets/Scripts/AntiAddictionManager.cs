using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AntiAddictionManager : MonoBehaviour
{
    public static AntiAddictionManager Instance { get; private set; }
    
    public AntiAddictionManagerData antiAddictionData;
    
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
        LoadAntiAddictionData();
        
        if (antiAddictionData == null)
        {
            antiAddictionData = new AntiAddictionManagerData();
            InitializeDefaultData();
        }
    }
    
    private void InitializeDefaultData()
    {
        // 年龄分组
        AgeGroup under8 = new AgeGroup("age_group_under_8", "8岁以下", 0, 7, "8岁以下儿童");
        AgeGroup age8_15 = new AgeGroup("age_group_8_15", "8-15岁", 8, 15, "8-15岁未成年人");
        AgeGroup age16_17 = new AgeGroup("age_group_16_17", "16-17岁", 16, 17, "16-17岁未成年人");
        AgeGroup adult = new AgeGroup("age_group_adult", "18岁以上", 18, 100, "18岁以上成年人");
        
        antiAddictionData.system.AddAgeGroup(under8);
        antiAddictionData.system.AddAgeGroup(age8_15);
        antiAddictionData.system.AddAgeGroup(age16_17);
        antiAddictionData.system.AddAgeGroup(adult);
        
        // 时间限制
        TimeLimit under8TimeLimit = new TimeLimit("time_limit_under_8", "age_group_under_8", 0, 0, false, "8岁以下儿童禁止游戏");
        TimeLimit age8_15TimeLimit = new TimeLimit("time_limit_8_15", "age_group_8_15", 40, 280, false, "8-15岁未成年人每日游戏时间不超过40分钟，22点后禁止游戏");
        TimeLimit age16_17TimeLimit = new TimeLimit("time_limit_16_17", "age_group_16_17", 120, 840, false, "16-17岁未成年人每日游戏时间不超过2小时，22点后禁止游戏");
        TimeLimit adultTimeLimit = new TimeLimit("time_limit_adult", "age_group_adult", 0, 0, true, "成年人无游戏时间限制");
        
        antiAddictionData.system.AddTimeLimit(under8TimeLimit);
        antiAddictionData.system.AddTimeLimit(age8_15TimeLimit);
        antiAddictionData.system.AddTimeLimit(age16_17TimeLimit);
        antiAddictionData.system.AddTimeLimit(adultTimeLimit);
        
        // 消费限制
        PurchaseLimit under8PurchaseLimit = new PurchaseLimit("purchase_limit_under_8", "age_group_under_8", 0, 0, "8岁以下儿童禁止充值");
        PurchaseLimit age8_15PurchaseLimit = new PurchaseLimit("purchase_limit_8_15", "age_group_8_15", 50, 200, "8-15岁未成年人单次充值不超过50元，每月不超过200元");
        PurchaseLimit age16_17PurchaseLimit = new PurchaseLimit("purchase_limit_16_17", "age_group_16_17", 100, 400, "16-17岁未成年人单次充值不超过100元，每月不超过400元");
        PurchaseLimit adultPurchaseLimit = new PurchaseLimit("purchase_limit_adult", "age_group_adult", 0, 0, "成年人无充值限制");
        
        antiAddictionData.system.AddPurchaseLimit(under8PurchaseLimit);
        antiAddictionData.system.AddPurchaseLimit(age8_15PurchaseLimit);
        antiAddictionData.system.AddPurchaseLimit(age16_17PurchaseLimit);
        antiAddictionData.system.AddPurchaseLimit(adultPurchaseLimit);
        
        SaveAntiAddictionData();
    }
    
    public bool CheckGameAccess(string playerID, int age)
    {
        if (!antiAddictionData.system.isEnabled)
        {
            return true;
        }
        
        PlayerAntiAddictionData playerData = GetOrCreatePlayerData(playerID, age);
        
        // 检查年龄是否允许游戏
        AgeGroup ageGroup = antiAddictionData.system.GetAgeGroup(age);
        if (ageGroup.ageGroupID == "age_group_under_8")
        {
            Debug.Log("8岁以下儿童禁止游戏");
            return false;
        }
        
        // 检查是否在禁止游戏时间
        if (System.DateTime.Now.Hour >= 22 && !antiAddictionData.system.GetTimeLimit(ageGroup.ageGroupID).isAllowedAfter22)
        {
            Debug.Log("22点后禁止未成年人游戏");
            return false;
        }
        
        // 检查游戏时间是否超限
        if (!CheckPlayTimeLimit(playerData))
        {
            return false;
        }
        
        return true;
    }
    
    public bool CheckPlayTimeLimit(PlayerAntiAddictionData playerData)
    {
        TimeLimit timeLimit = antiAddictionData.system.GetTimeLimit(playerData.ageGroupID);
        if (timeLimit.dailyPlayTime > 0 && playerData.dailyPlayTime >= timeLimit.dailyPlayTime)
        {
            Debug.Log("每日游戏时间已达上限");
            return false;
        }
        
        if (timeLimit.weeklyPlayTime > 0 && playerData.weeklyPlayTime >= timeLimit.weeklyPlayTime)
        {
            Debug.Log("每周游戏时间已达上限");
            return false;
        }
        
        return true;
    }
    
    public bool CheckPurchaseLimit(string playerID, int age, int amount)
    {
        if (!antiAddictionData.system.isEnabled)
        {
            return true;
        }
        
        PlayerAntiAddictionData playerData = GetOrCreatePlayerData(playerID, age);
        PurchaseLimit purchaseLimit = antiAddictionData.system.GetPurchaseLimit(playerData.ageGroupID);
        
        if (purchaseLimit.singlePurchaseLimit > 0 && amount > purchaseLimit.singlePurchaseLimit)
        {
            Debug.Log("单次充值金额超过限制");
            return false;
        }
        
        if (purchaseLimit.monthlyPurchaseLimit > 0 && playerData.monthlyPurchases + amount > purchaseLimit.monthlyPurchaseLimit)
        {
            Debug.Log("月度充值金额超过限制");
            return false;
        }
        
        return true;
    }
    
    public void RecordLogin(string playerID, int age)
    {
        PlayerAntiAddictionData playerData = GetOrCreatePlayerData(playerID, age);
        playerData.lastLoginTime = System.DateTime.Now;
        SaveAntiAddictionData();
    }
    
    public void RecordLogout(string playerID)
    {
        PlayerAntiAddictionData playerData = antiAddictionData.GetPlayerData(playerID);
        if (playerData != null)
        {
            playerData.UpdatePlayTime();
            SaveAntiAddictionData();
        }
    }
    
    public void RecordPurchase(string playerID, int amount)
    {
        PlayerAntiAddictionData playerData = antiAddictionData.GetPlayerData(playerID);
        if (playerData != null)
        {
            playerData.AddPurchase(amount);
            SaveAntiAddictionData();
        }
    }
    
    public void VerifyPlayer(string playerID, bool verified)
    {
        PlayerAntiAddictionData playerData = antiAddictionData.GetPlayerData(playerID);
        if (playerData != null)
        {
            playerData.SetVerified(verified);
            SaveAntiAddictionData();
        }
    }
    
    public void ResetDailyLimits()
    {
        foreach (PlayerAntiAddictionData playerData in antiAddictionData.playerData)
        {
            playerData.ResetDailyPlayTime();
        }
        SaveAntiAddictionData();
    }
    
    public void ResetWeeklyLimits()
    {
        foreach (PlayerAntiAddictionData playerData in antiAddictionData.playerData)
        {
            playerData.ResetWeeklyPlayTime();
        }
        SaveAntiAddictionData();
    }
    
    public void ResetMonthlyLimits()
    {
        foreach (PlayerAntiAddictionData playerData in antiAddictionData.playerData)
        {
            playerData.ResetMonthlyPurchases();
        }
        SaveAntiAddictionData();
    }
    
    private PlayerAntiAddictionData GetOrCreatePlayerData(string playerID, int age)
    {
        PlayerAntiAddictionData playerData = antiAddictionData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerAntiAddictionData(playerID, age);
            antiAddictionData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public PlayerAntiAddictionData GetPlayerData(string playerID)
    {
        return antiAddictionData.GetPlayerData(playerID);
    }
    
    public TimeLimit GetTimeLimit(int age)
    {
        AgeGroup ageGroup = antiAddictionData.system.GetAgeGroup(age);
        return antiAddictionData.system.GetTimeLimit(ageGroup.ageGroupID);
    }
    
    public PurchaseLimit GetPurchaseLimit(int age)
    {
        AgeGroup ageGroup = antiAddictionData.system.GetAgeGroup(age);
        return antiAddictionData.system.GetPurchaseLimit(ageGroup.ageGroupID);
    }
    
    public void EnableAntiAddiction(bool enable)
    {
        antiAddictionData.system.isEnabled = enable;
        SaveAntiAddictionData();
    }
    
    public bool IsAntiAddictionEnabled()
    {
        return antiAddictionData.system.isEnabled;
    }
    
    public void SaveAntiAddictionData()
    {
        string path = Application.dataPath + "/Data/anti_addiction_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, antiAddictionData);
        stream.Close();
    }
    
    public void LoadAntiAddictionData()
    {
        string path = Application.dataPath + "/Data/anti_addiction_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            antiAddictionData = (AntiAddictionManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            antiAddictionData = new AntiAddictionManagerData();
        }
    }
}
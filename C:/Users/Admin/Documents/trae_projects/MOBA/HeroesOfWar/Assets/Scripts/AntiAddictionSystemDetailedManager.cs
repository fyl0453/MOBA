using System;
using System.Collections.Generic;

public class AntiAddictionSystemDetailedManager
{
    private static AntiAddictionSystemDetailedManager _instance;
    public static AntiAddictionSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AntiAddictionSystemDetailedManager();
            }
            return _instance;
        }
    }

    private AntiAddictionSystemData antiAddictionData;
    private AntiAddictionSystemDataManager dataManager;

    private AntiAddictionSystemDetailedManager()
    {
        dataManager = AntiAddictionSystemDataManager.Instance;
        antiAddictionData = dataManager.antiAddictionData;
    }

    public void InitializePlayerAntiAddictionData(string playerID)
    {
        if (!antiAddictionData.PlayerAntiAddictionData.ContainsKey(playerID))
        {
            PlayerAntiAddictionData playerData = new PlayerAntiAddictionData(playerID);
            antiAddictionData.AddPlayerAntiAddictionData(playerID, playerData);
            dataManager.SaveAntiAddictionData();
            Debug.Log("初始化防沉迷数据成功");
        }
    }

    public string SubmitRealNameVerification(string playerID, string realName, string idCard, int age)
    {
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        
        if (playerData.RealNameInfo != null && playerData.RealNameInfo.IsVerified)
        {
            Debug.LogError("已完成实名认证");
            return "";
        }
        
        RealNameInfo realNameInfo = new RealNameInfo(realName, idCard, age);
        playerData.RealNameInfo = realNameInfo;
        playerData.IsMinor = age < 18;
        
        realNameInfo.IsVerified = true;
        realNameInfo.VerifyTime = DateTime.Now;
        realNameInfo.VerificationStatus = "verified";
        
        antiAddictionData.TotalVerifiedPlayers++;
        if (playerData.IsMinor)
        {
            antiAddictionData.TotalMinorPlayers++;
        }
        
        dataManager.CreateAntiAddictionEvent("realname_submit", playerID, "提交实名认证");
        dataManager.SaveAntiAddictionData();
        Debug.Log("提交实名认证成功");
        return realNameInfo.VerificationStatus;
    }

    public bool CheckLoginPermission(string playerID)
    {
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        
        if (antiAddictionData.Config.EnableRealNameVerification && playerData.RealNameInfo == null)
        {
            dataManager.CreateAntiAddictionEvent("login_deny", playerID, "未完成实名认证");
            return false;
        }
        
        if (playerData.IsMinor && playerData.IsLoginLimited)
        {
            dataManager.CreateAntiAddictionEvent("login_deny", playerID, "游戏时间已达上限");
            return false;
        }
        
        if (playerData.IsMinor && IsMinorRestrictedTime())
        {
            dataManager.CreateAntiAddictionEvent("login_deny", playerID, "未成年人限制时间");
            return false;
        }
        
        return true;
    }

    public string RecordLogin(string playerID)
    {
        if (!CheckLoginPermission(playerID))
        {
            return "";
        }
        
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        
        string recordID = "playtime_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PlayTimeRecord record = new PlayTimeRecord(recordID, playerID, DateTime.Now);
        playerData.PlayTimeRecords.Add(record);
        
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        if (!playerData.DailyPlayTimes.ContainsKey(today))
        {
            playerData.DailyPlayTimes[today] = new DailyPlayTime(today);
        }
        playerData.DailyPlayTimes[today].LoginCount++;
        playerData.DailyPlayTimes[today].LastLoginTime = DateTime.Now;
        
        playerData.LastLoginTime = DateTime.Now;
        
        dataManager.CreateAntiAddictionEvent("login_record", playerID, "记录登录");
        dataManager.SaveAntiAddictionData();
        Debug.Log("记录登录成功: " + recordID);
        return recordID;
    }

    public void RecordLogout(string playerID, string recordID)
    {
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        
        PlayTimeRecord record = playerData.PlayTimeRecords.Find(r => r.RecordID == recordID && !r.IsCompleted);
        if (record != null)
        {
            record.LogoutTime = DateTime.Now;
            record.PlayDuration = (float)(record.LogoutTime - record.LoginTime).TotalMinutes;
            record.IsCompleted = true;
            
            string today = record.Date.ToString("yyyy-MM-dd");
            if (playerData.DailyPlayTimes.ContainsKey(today))
            {
                playerData.DailyPlayTimes[today].TotalPlayTime += record.PlayDuration;
                playerData.DailyPlayTimes[today].LastLogoutTime = DateTime.Now;
            }
            
            UpdatePlayTimeStats(playerData);
            CheckPlayTimeLimits(playerData);
            
            playerData.LastLogoutTime = DateTime.Now;
            
            dataManager.CreateAntiAddictionEvent("logout_record", playerID, "记录登出");
            dataManager.SaveAntiAddictionData();
            Debug.Log("记录登出成功");
        }
    }

    public bool CheckPlayTimeLimit(string playerID)
    {
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        
        if (!playerData.IsMinor)
        {
            return playerData.TodayPlayTime < antiAddictionData.Config.AdultMaxDailyPlayTime;
        }
        else
        {
            return playerData.TodayPlayTime < antiAddictionData.Config.MinorMaxDailyPlayTime && 
                   playerData.ThisWeekPlayTime < antiAddictionData.Config.MinorMaxWeeklyPlayTime;
        }
    }

    public bool CheckPaymentLimit(string playerID, int amount)
    {
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        
        if (!playerData.IsMinor || !antiAddictionData.Config.EnablePaymentLimit)
        {
            return true;
        }
        
        if (amount > antiAddictionData.Config.MinorMaxPaymentPerTransaction)
        {
            return false;
        }
        
        int monthlyPayment = GetMonthlyPayment(playerID);
        return monthlyPayment + amount <= antiAddictionData.Config.MinorMaxPaymentPerMonth;
    }

    public float GetTodayPlayTime(string playerID)
    {
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        return playerData.TodayPlayTime;
    }

    public float GetWeeklyPlayTime(string playerID)
    {
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        return playerData.ThisWeekPlayTime;
    }

    public bool IsMinor(string playerID)
    {
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        return playerData.IsMinor;
    }

    public bool IsRealNameVerified(string playerID)
    {
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        return playerData.RealNameInfo != null && playerData.RealNameInfo.IsVerified;
    }

    public RealNameInfo GetRealNameInfo(string playerID)
    {
        InitializePlayerAntiAddictionData(playerID);
        PlayerAntiAddictionData playerData = antiAddictionData.PlayerAntiAddictionData[playerID];
        return playerData.RealNameInfo;
    }

    private void UpdatePlayTimeStats(PlayerAntiAddictionData playerData)
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        if (playerData.DailyPlayTimes.ContainsKey(today))
        {
            playerData.TodayPlayTime = playerData.DailyPlayTimes[today].TotalPlayTime;
        }
        else
        {
            playerData.TodayPlayTime = 0;
        }
        
        playerData.ThisWeekPlayTime = CalculateWeeklyPlayTime(playerData);
    }

    private float CalculateWeeklyPlayTime(PlayerAntiAddictionData playerData)
    {
        float weeklyTime = 0;
        DateTime today = DateTime.Now;
        for (int i = 0; i < 7; i++)
        {
            string date = today.AddDays(-i).ToString("yyyy-MM-dd");
            if (playerData.DailyPlayTimes.ContainsKey(date))
            {
                weeklyTime += playerData.DailyPlayTimes[date].TotalPlayTime;
            }
        }
        return weeklyTime;
    }

    private void CheckPlayTimeLimits(PlayerAntiAddictionData playerData)
    {
        if (playerData.IsMinor)
        {
            if (playerData.TodayPlayTime >= antiAddictionData.Config.MinorMaxDailyPlayTime ||
                playerData.ThisWeekPlayTime >= antiAddictionData.Config.MinorMaxWeeklyPlayTime)
            {
                playerData.IsLoginLimited = true;
            }
            else
            {
                playerData.IsLoginLimited = false;
            }
        }
    }

    private bool IsMinorRestrictedTime()
    {
        DateTime now = DateTime.Now;
        return now.Hour >= 22 || now.Hour < 8;
    }

    private int GetMonthlyPayment(string playerID)
    {
        
        return 0;
    }

    public void ResetDailyPlayTime()
    {
        foreach (PlayerAntiAddictionData playerData in antiAddictionData.PlayerAntiAddictionData.Values)
        {
            playerData.TodayPlayTime = 0;
            if (playerData.IsMinor)
            {
                playerData.IsLoginLimited = false;
            }
        }
        dataManager.SaveAntiAddictionData();
        Debug.Log("重置每日游戏时间成功");
    }

    public void ResetWeeklyPlayTime()
    {
        foreach (PlayerAntiAddictionData playerData in antiAddictionData.PlayerAntiAddictionData.Values)
        {
            playerData.ThisWeekPlayTime = 0;
        }
        dataManager.SaveAntiAddictionData();
        Debug.Log("重置每周游戏时间成功");
    }

    public void CleanupOldPlayTimeRecords(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        foreach (PlayerAntiAddictionData playerData in antiAddictionData.PlayerAntiAddictionData.Values)
        {
            List<PlayTimeRecord> oldRecords = playerData.PlayTimeRecords.FindAll(r => r.Date < cutoffDate);
            foreach (PlayTimeRecord record in oldRecords)
            {
                playerData.PlayTimeRecords.Remove(record);
            }
            
            List<string> oldDates = new List<string>();
            foreach (string date in playerData.DailyPlayTimes.Keys)
            {
                DateTime recordDate = DateTime.Parse(date);
                if (recordDate < cutoffDate)
                {
                    oldDates.Add(date);
                }
            }
            foreach (string date in oldDates)
            {
                playerData.DailyPlayTimes.Remove(date);
            }
        }
        
        dataManager.CreateAntiAddictionEvent("cleanup_records", "system", "清理旧游戏时间记录");
        dataManager.SaveAntiAddictionData();
        Debug.Log("清理旧游戏时间记录成功");
    }

    public void UpdateConfig(AntiAddictionConfig config)
    {
        antiAddictionData.Config = config;
        dataManager.SaveAntiAddictionData();
        Debug.Log("更新防沉迷配置成功");
    }

    public AntiAddictionConfig GetConfig()
    {
        return antiAddictionData.Config;
    }

    public void SaveData()
    {
        dataManager.SaveAntiAddictionData();
    }

    public void LoadData()
    {
        dataManager.LoadAntiAddictionData();
    }

    public List<AntiAddictionEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
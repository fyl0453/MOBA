using System;
using System.Collections.Generic;

public class SecuritySystemDetailedManager
{
    private static SecuritySystemDetailedManager _instance;
    public static SecuritySystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SecuritySystemDetailedManager();
            }
            return _instance;
        }
    }

    private SecuritySystemData securityData;
    private SecuritySystemDataManager dataManager;

    private SecuritySystemDetailedManager()
    {
        dataManager = SecuritySystemDataManager.Instance;
        securityData = dataManager.securityData;
    }

    public void InitializePlayerSecurityData(string playerID, string passwordHash, string passwordSalt)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            PlayerSecurityData playerData = new PlayerSecurityData(playerID, passwordHash, passwordSalt);
            securityData.AddPlayerSecurityData(playerID, playerData);
            dataManager.SaveSecurityData();
            Debug.Log("初始化安全数据成功");
        }
    }

    public bool CheckLoginSecurity(string playerID, string deviceID, string ipAddress, string location, string loginMethod)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return false;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        
        if (playerData.AccountSecurity.IsLocked && playerData.AccountSecurity.LockedUntil > DateTime.Now)
        {
            CreateSecurityAlert(playerID, 0, "账号已被锁定", "登录失败次数过多");
            return false;
        }
        
        if (playerData.AccountSecurity.FailedLoginAttempts >= securityData.MaxFailedLoginAttempts)
        {
            LockAccount(playerID);
            CreateSecurityAlert(playerID, 0, "账号已被锁定", "登录失败次数过多");
            return false;
        }
        
        bool isTrustedDevice = IsTrustedDevice(playerID, deviceID);
        if (!isTrustedDevice)
        {
            CreateSecurityAlert(playerID, 1, "未知设备登录", "检测到新设备登录");
        }
        
        if (IsSuspiciousLocation(playerID, location))
        {
            CreateSecurityAlert(playerID, 2, "异地登录", "检测到异地登录");
        }
        
        return true;
    }

    public string RecordLogin(string playerID, string deviceID, string deviceName, string deviceType, string osVersion, string appVersion, string ipAddress, string location, string loginMethod, bool isSuccessful)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return "";
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        
        string recordID = "login_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        LoginRecord record = new LoginRecord(recordID, playerID, deviceID, ipAddress, location, loginMethod);
        record.IsSuccessful = isSuccessful;
        playerData.LoginRecords.Add(record);
        
        if (isSuccessful)
        {
            UpdateDeviceInfo(playerData, deviceID, deviceName, deviceType, osVersion, appVersion);
            playerData.AccountSecurity.FailedLoginAttempts = 0;
        }
        else
        {
            playerData.AccountSecurity.FailedLoginAttempts++;
            if (playerData.AccountSecurity.FailedLoginAttempts >= securityData.MaxFailedLoginAttempts)
            {
                LockAccount(playerID);
            }
        }
        
        dataManager.CreateSecurityEvent("login_record", playerID, "记录登录: " + (isSuccessful ? "成功" : "失败"));
        dataManager.SaveSecurityData();
        Debug.Log("记录登录成功: " + recordID);
        return recordID;
    }

    public void RecordLogout(string playerID, string recordID)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        LoginRecord record = playerData.LoginRecords.Find(r => r.RecordID == recordID && r.Status == "active");
        if (record != null)
        {
            record.LogoutTime = DateTime.Now;
            record.Status = "completed";
            
            dataManager.CreateSecurityEvent("logout_record", playerID, "记录登出");
            dataManager.SaveSecurityData();
            Debug.Log("记录登出成功");
        }
    }

    public void AddTrustedDevice(string playerID, string deviceID)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        DeviceInfo device = playerData.UnknownDevices.Find(d => d.DeviceID == deviceID);
        if (device != null)
        {
            device.IsTrusted = true;
            playerData.TrustedDevices.Add(device);
            playerData.UnknownDevices.Remove(device);
            
            dataManager.CreateSecurityEvent("device_trust", playerID, "添加信任设备: " + device.DeviceName);
            dataManager.SaveSecurityData();
            Debug.Log("添加信任设备成功: " + device.DeviceName);
        }
    }

    public void RemoveTrustedDevice(string playerID, string deviceID)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        DeviceInfo device = playerData.TrustedDevices.Find(d => d.DeviceID == deviceID);
        if (device != null)
        {
            device.IsTrusted = false;
            playerData.UnknownDevices.Add(device);
            playerData.TrustedDevices.Remove(device);
            
            dataManager.CreateSecurityEvent("device_untrust", playerID, "移除信任设备: " + device.DeviceName);
            dataManager.SaveSecurityData();
            Debug.Log("移除信任设备成功: " + device.DeviceName);
        }
    }

    public void ChangePassword(string playerID, string newPasswordHash, string newPasswordSalt)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        playerData.AccountSecurity.PasswordHash = newPasswordHash;
        playerData.AccountSecurity.PasswordSalt = newPasswordSalt;
        playerData.AccountSecurity.LastPasswordChange = DateTime.Now;
        playerData.AccountSecurity.LastSecurityUpdate = DateTime.Now;
        
        dataManager.CreateSecurityEvent("password_change", playerID, "修改密码");
        dataManager.SaveSecurityData();
        Debug.Log("修改密码成功");
    }

    public void EnableTwoFactor(string playerID, string twoFactorSecret)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        playerData.AccountSecurity.TwoFactorEnabled = true;
        playerData.AccountSecurity.TwoFactorSecret = twoFactorSecret;
        playerData.AccountSecurity.LastSecurityUpdate = DateTime.Now;
        
        dataManager.CreateSecurityEvent("twofactor_enable", playerID, "启用两步验证");
        dataManager.SaveSecurityData();
        Debug.Log("启用两步验证成功");
    }

    public void DisableTwoFactor(string playerID)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        playerData.AccountSecurity.TwoFactorEnabled = false;
        playerData.AccountSecurity.TwoFactorSecret = "";
        playerData.AccountSecurity.LastSecurityUpdate = DateTime.Now;
        
        dataManager.CreateSecurityEvent("twofactor_disable", playerID, "禁用两步验证");
        dataManager.SaveSecurityData();
        Debug.Log("禁用两步验证成功");
    }

    public void UpdateRecoveryInfo(string playerID, string recoveryEmail, string recoveryPhone)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        playerData.AccountSecurity.RecoveryEmail = recoveryEmail;
        playerData.AccountSecurity.RecoveryPhone = recoveryPhone;
        playerData.AccountSecurity.LastSecurityUpdate = DateTime.Now;
        
        dataManager.CreateSecurityEvent("recovery_update", playerID, "更新恢复信息");
        dataManager.SaveSecurityData();
        Debug.Log("更新恢复信息成功");
    }

    public void UnlockAccount(string playerID)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        playerData.AccountSecurity.IsLocked = false;
        playerData.AccountSecurity.LockedUntil = DateTime.MinValue;
        playerData.AccountSecurity.FailedLoginAttempts = 0;
        
        dataManager.CreateSecurityEvent("account_unlock", playerID, "解锁账号");
        dataManager.SaveSecurityData();
        Debug.Log("解锁账号成功");
    }

    public void ResolveSecurityAlert(string alertID, string resolution)
    {
        foreach (PlayerSecurityData playerData in securityData.PlayerSecurityData.Values)
        {
            SecurityAlert alert = playerData.SecurityAlerts.Find(a => a.AlertID == alertID);
            if (alert != null)
            {
                alert.IsResolved = true;
                alert.Resolution = resolution;
                alert.ResolutionTime = DateTime.Now;
                
                dataManager.CreateSecurityEvent("alert_resolve", playerData.PlayerID, "解决安全警报: " + alert.AlertMessage);
                dataManager.SaveSecurityData();
                Debug.Log("解决安全警报成功");
                return;
            }
        }
    }

    private bool IsTrustedDevice(string playerID, string deviceID)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return false;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        return playerData.TrustedDevices.Exists(d => d.DeviceID == deviceID);
    }

    private bool IsSuspiciousLocation(string playerID, string location)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return false;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        if (playerData.LoginRecords.Count < 3)
        {
            return false;
        }
        
        List<LoginRecord> recentLogins = playerData.LoginRecords.FindAll(r => r.IsSuccessful).GetRange(Math.Max(0, playerData.LoginRecords.Count - 5), Math.Min(5, playerData.LoginRecords.Count));
        foreach (LoginRecord record in recentLogins)
        {
            if (record.Location == location)
            {
                return false;
            }
        }
        
        return true;
    }

    private void UpdateDeviceInfo(PlayerSecurityData playerData, string deviceID, string deviceName, string deviceType, string osVersion, string appVersion)
    {
        DeviceInfo existingDevice = playerData.TrustedDevices.Find(d => d.DeviceID == deviceID) ?? playerData.UnknownDevices.Find(d => d.DeviceID == deviceID);
        
        if (existingDevice != null)
        {
            existingDevice.LastLoginTime = DateTime.Now;
            existingDevice.LoginCount++;
            existingDevice.DeviceName = deviceName;
            existingDevice.OSVersion = osVersion;
            existingDevice.AppVersion = appVersion;
        }
        else
        {
            DeviceInfo newDevice = new DeviceInfo(deviceID, deviceName, deviceType, osVersion, appVersion);
            playerData.UnknownDevices.Add(newDevice);
        }
    }

    private void LockAccount(string playerID)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        playerData.AccountSecurity.IsLocked = true;
        playerData.AccountSecurity.LockedUntil = DateTime.Now.AddMinutes(securityData.AccountLockDuration);
        
        dataManager.CreateSecurityEvent("account_lock", playerID, "锁定账号");
        Debug.Log("锁定账号成功");
    }

    private void CreateSecurityAlert(string playerID, int alertType, string alertMessage, string alertDetails)
    {
        if (!securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerSecurityData playerData = securityData.PlayerSecurityData[playerID];
        string alertID = "alert_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SecurityAlert alert = new SecurityAlert(alertID, playerID, alertType, alertMessage, alertDetails);
        playerData.SecurityAlerts.Add(alert);
        
        dataManager.CreateSecurityEvent("alert_create", playerID, "创建安全警报: " + alertMessage);
        Debug.Log("创建安全警报成功: " + alertMessage);
    }

    public List<DeviceInfo> GetTrustedDevices(string playerID)
    {
        if (securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return securityData.PlayerSecurityData[playerID].TrustedDevices;
        }
        return new List<DeviceInfo>();
    }

    public List<DeviceInfo> GetUnknownDevices(string playerID)
    {
        if (securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return securityData.PlayerSecurityData[playerID].UnknownDevices;
        }
        return new List<DeviceInfo>();
    }

    public List<LoginRecord> GetLoginRecords(string playerID, int count = 50)
    {
        if (securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            List<LoginRecord> records = securityData.PlayerSecurityData[playerID].LoginRecords;
            records.Sort((a, b) => b.LoginTime.CompareTo(a.LoginTime));
            if (count < records.Count)
            {
                return records.GetRange(0, count);
            }
            return records;
        }
        return new List<LoginRecord>();
    }

    public List<SecurityAlert> GetSecurityAlerts(string playerID)
    {
        if (securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return securityData.PlayerSecurityData[playerID].SecurityAlerts;
        }
        return new List<SecurityAlert>();
    }

    public bool IsAccountLocked(string playerID)
    {
        if (securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            AccountSecurity accountSecurity = securityData.PlayerSecurityData[playerID].AccountSecurity;
            return accountSecurity.IsLocked && accountSecurity.LockedUntil > DateTime.Now;
        }
        return false;
    }

    public bool IsTwoFactorEnabled(string playerID)
    {
        if (securityData.PlayerSecurityData.ContainsKey(playerID))
        {
            return securityData.PlayerSecurityData[playerID].AccountSecurity.TwoFactorEnabled;
        }
        return false;
    }

    public void CleanupOldRecords(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        foreach (PlayerSecurityData playerData in securityData.PlayerSecurityData.Values)
        {
            List<LoginRecord> oldRecords = playerData.LoginRecords.FindAll(r => r.LoginTime < cutoffDate);
            foreach (LoginRecord record in oldRecords)
            {
                playerData.LoginRecords.Remove(record);
            }
            
            List<SecurityAlert> oldAlerts = playerData.SecurityAlerts.FindAll(a => a.AlertTime < cutoffDate && a.IsResolved);
            foreach (SecurityAlert alert in oldAlerts)
            {
                playerData.SecurityAlerts.Remove(alert);
            }
        }
        
        dataManager.CreateSecurityEvent("cleanup_records", "system", "清理旧安全记录");
        dataManager.SaveSecurityData();
        Debug.Log("清理旧安全记录成功");
    }

    public void SaveData()
    {
        dataManager.SaveSecurityData();
    }

    public void LoadData()
    {
        dataManager.LoadSecurityData();
    }

    public List<SecurityEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
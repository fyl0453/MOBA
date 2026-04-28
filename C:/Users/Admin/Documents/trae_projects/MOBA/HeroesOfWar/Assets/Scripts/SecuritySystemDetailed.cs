using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class DeviceInfo
{
    public string DeviceID;
    public string DeviceName;
    public string DeviceType;
    public string OSVersion;
    public string AppVersion;
    public DateTime FirstLoginTime;
    public DateTime LastLoginTime;
    public int LoginCount;
    public bool IsTrusted;

    public DeviceInfo(string deviceID, string deviceName, string deviceType, string osVersion, string appVersion)
    {
        DeviceID = deviceID;
        DeviceName = deviceName;
        DeviceType = deviceType;
        OSVersion = osVersion;
        AppVersion = appVersion;
        FirstLoginTime = DateTime.Now;
        LastLoginTime = DateTime.Now;
        LoginCount = 1;
        IsTrusted = false;
    }
}

[Serializable]
public class LoginRecord
{
    public string RecordID;
    public string PlayerID;
    public string DeviceID;
    public string IPAddress;
    public string Location;
    public DateTime LoginTime;
    public DateTime LogoutTime;
    public bool IsSuccessful;
    public string LoginMethod;
    public string Status;

    public LoginRecord(string recordID, string playerID, string deviceID, string ipAddress, string location, string loginMethod)
    {
        RecordID = recordID;
        PlayerID = playerID;
        DeviceID = deviceID;
        IPAddress = ipAddress;
        Location = location;
        LoginTime = DateTime.Now;
        LogoutTime = DateTime.MinValue;
        IsSuccessful = true;
        LoginMethod = loginMethod;
        Status = "active";
    }
}

[Serializable]
public class SecurityAlert
{
    public string AlertID;
    public string PlayerID;
    public int AlertType;
    public string AlertMessage;
    public string AlertDetails;
    public DateTime AlertTime;
    public bool IsResolved;
    public string Resolution;
    public DateTime ResolutionTime;

    public SecurityAlert(string alertID, string playerID, int alertType, string alertMessage, string alertDetails)
    {
        AlertID = alertID;
        PlayerID = playerID;
        AlertType = alertType;
        AlertMessage = alertMessage;
        AlertDetails = alertDetails;
        AlertTime = DateTime.Now;
        IsResolved = false;
        Resolution = "";
        ResolutionTime = DateTime.MinValue;
    }
}

[Serializable]
public class AccountSecurity
{
    public string PlayerID;
    public string PasswordHash;
    public string PasswordSalt;
    public bool TwoFactorEnabled;
    public string TwoFactorSecret;
    public string RecoveryEmail;
    public string RecoveryPhone;
    public DateTime LastPasswordChange;
    public DateTime LastSecurityUpdate;
    public int FailedLoginAttempts;
    public bool IsLocked;
    public DateTime LockedUntil;

    public AccountSecurity(string playerID, string passwordHash, string passwordSalt)
    {
        PlayerID = playerID;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        TwoFactorEnabled = false;
        TwoFactorSecret = "";
        RecoveryEmail = "";
        RecoveryPhone = "";
        LastPasswordChange = DateTime.Now;
        LastSecurityUpdate = DateTime.Now;
        FailedLoginAttempts = 0;
        IsLocked = false;
        LockedUntil = DateTime.MinValue;
    }
}

[Serializable]
public class PlayerSecurityData
{
    public string PlayerID;
    public AccountSecurity AccountSecurity;
    public List<DeviceInfo> TrustedDevices;
    public List<DeviceInfo> UnknownDevices;
    public List<LoginRecord> LoginRecords;
    public List<SecurityAlert> SecurityAlerts;
    public DateTime LastSecurityCheck;
    public bool IsSecurityEnabled;

    public PlayerSecurityData(string playerID, string passwordHash, string passwordSalt)
    {
        PlayerID = playerID;
        AccountSecurity = new AccountSecurity(playerID, passwordHash, passwordSalt);
        TrustedDevices = new List<DeviceInfo>();
        UnknownDevices = new List<DeviceInfo>();
        LoginRecords = new List<LoginRecord>();
        SecurityAlerts = new List<SecurityAlert>();
        LastSecurityCheck = DateTime.Now;
        IsSecurityEnabled = true;
    }
}

[Serializable]
public class SecuritySystemData
{
    public Dictionary<string, PlayerSecurityData> PlayerSecurityData;
    public int MaxFailedLoginAttempts;
    public int AccountLockDuration;
    public int SessionTimeout;
    public bool EnableIPTracking;
    public bool EnableDeviceManagement;
    public bool EnableTwoFactor;
    public DateTime LastSystemUpdate;

    public SecuritySystemData()
    {
        PlayerSecurityData = new Dictionary<string, PlayerSecurityData>();
        MaxFailedLoginAttempts = 5;
        AccountLockDuration = 30;
        SessionTimeout = 1800;
        EnableIPTracking = true;
        EnableDeviceManagement = true;
        EnableTwoFactor = true;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddPlayerSecurityData(string playerID, PlayerSecurityData data)
    {
        PlayerSecurityData[playerID] = data;
    }
}

[Serializable]
public class SecurityEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EventData;

    public SecurityEvent(string eventID, string eventType, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class SecuritySystemDataManager
{
    private static SecuritySystemDataManager _instance;
    public static SecuritySystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SecuritySystemDataManager();
            }
            return _instance;
        }
    }

    public SecuritySystemData securityData;
    private List<SecurityEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SecuritySystemDataManager()
    {
        securityData = new SecuritySystemData();
        recentEvents = new List<SecurityEvent>();
        LoadSecurityData();
    }

    public void SaveSecurityData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SecuritySystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, securityData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存安全系统数据失败: " + e.Message);
        }
    }

    public void LoadSecurityData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SecuritySystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    securityData = (SecuritySystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载安全系统数据失败: " + e.Message);
            securityData = new SecuritySystemData();
        }
    }

    public void CreateSecurityEvent(string eventType, string playerID, string eventData)
    {
        string eventID = "security_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SecurityEvent securityEvent = new SecurityEvent(eventID, eventType, playerID, eventData);
        recentEvents.Add(securityEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<SecurityEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
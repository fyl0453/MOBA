[System.Serializable]
public class CrossPlatformSyncEnhanced
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AccountBinding> accountBindings;
    public List<DeviceInfo> deviceInfos;
    public List<SyncRecord> syncRecords;
    public List<SyncConflict> syncConflicts;
    public List<CloudSave> cloudSaves;
    
    public CrossPlatformSyncEnhanced(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        accountBindings = new List<AccountBinding>();
        deviceInfos = new List<DeviceInfo>();
        syncRecords = new List<SyncRecord>();
        syncConflicts = new List<SyncConflict>();
        cloudSaves = new List<CloudSave>();
    }
    
    public void AddAccountBinding(AccountBinding binding)
    {
        accountBindings.Add(binding);
    }
    
    public void AddDeviceInfo(DeviceInfo device)
    {
        deviceInfos.Add(device);
    }
    
    public void AddSyncRecord(SyncRecord record)
    {
        syncRecords.Add(record);
    }
    
    public void AddSyncConflict(SyncConflict conflict)
    {
        syncConflicts.Add(conflict);
    }
    
    public void AddCloudSave(CloudSave save)
    {
        cloudSaves.Add(save);
    }
    
    public AccountBinding GetAccountBinding(string playerID, string platform)
    {
        return accountBindings.Find(b => b.playerID == playerID && b.platform == platform);
    }
    
    public DeviceInfo GetDeviceInfo(string deviceID)
    {
        return deviceInfos.Find(d => d.deviceID == deviceID);
    }
    
    public CloudSave GetCloudSave(string playerID)
    {
        return cloudSaves.Find(c => c.playerID == playerID);
    }
    
    public List<DeviceInfo> GetPlayerDevices(string playerID)
    {
        return deviceInfos.FindAll(d => d.playerID == playerID);
    }
    
    public List<SyncRecord> GetPlayerSyncRecords(string playerID, int limit = 50)
    {
        List<SyncRecord> records = syncRecords.FindAll(r => r.playerID == playerID);
        records.Sort((a, b) => b.syncTime.CompareTo(a.syncTime));
        return records.GetRange(0, Mathf.Min(limit, records.Count));
    }
}

[System.Serializable]
public class AccountBinding
{
    public string bindingID;
    public string playerID;
    public string platform;
    public string platformAccountID;
    public string platformAccountName;
    public bool isVerified;
    public string bindTime;
    public string lastSyncTime;
    
    public AccountBinding(string id, string playerID, string platform, string platformID, string platformName)
    {
        bindingID = id;
        this.playerID = playerID;
        this.platform = platform;
        platformAccountID = platformID;
        platformAccountName = platformName;
        isVerified = false;
        bindTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastSyncTime = "";
    }
    
    public void Verify()
    {
        isVerified = true;
    }
    
    public void UpdateLastSyncTime()
    {
        lastSyncTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class DeviceInfo
{
    public string deviceID;
    public string playerID;
    public string deviceName;
    public string deviceType;
    public string deviceOS;
    public string deviceModel;
    public string appVersion;
    public string lastLoginTime;
    public string lastSyncTime;
    public bool isAuthorized;
    
    public DeviceInfo(string id, string playerID, string name, string type, string os, string model, string version)
    {
        deviceID = id;
        this.playerID = playerID;
        deviceName = name;
        deviceType = type;
        deviceOS = os;
        deviceModel = model;
        appVersion = version;
        lastLoginTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastSyncTime = "";
        isAuthorized = true;
    }
    
    public void UpdateLastLoginTime()
    {
        lastLoginTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateLastSyncTime()
    {
        lastSyncTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Authorize()
    {
        isAuthorized = true;
    }
    
    public void RevokeAuthorization()
    {
        isAuthorized = false;
    }
}

[System.Serializable]
public class SyncRecord
{
    public string recordID;
    public string playerID;
    public string deviceID;
    public string syncType;
    public string syncStatus;
    public string syncTime;
    public string syncData;
    public string errorMessage;
    
    public SyncRecord(string id, string playerID, string deviceID, string type, string status, string data, string error = "")
    {
        recordID = id;
        this.playerID = playerID;
        this.deviceID = deviceID;
        syncType = type;
        syncStatus = status;
        syncTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        syncData = data;
        errorMessage = error;
    }
}

[System.Serializable]
public class SyncConflict
{
    public string conflictID;
    public string playerID;
    public string deviceID1;
    public string deviceID2;
    public string conflictType;
    public string conflictData1;
    public string conflictData2;
    public string resolutionStatus;
    public string resolutionTime;
    
    public SyncConflict(string id, string playerID, string device1, string device2, string type, string data1, string data2)
    {
        conflictID = id;
        this.playerID = playerID;
        deviceID1 = device1;
        deviceID2 = device2;
        conflictType = type;
        conflictData1 = data1;
        conflictData2 = data2;
        resolutionStatus = "Pending";
        resolutionTime = "";
    }
    
    public void Resolve(string status)
    {
        resolutionStatus = status;
        resolutionTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CloudSave
{
    public string saveID;
    public string playerID;
    public string saveData;
    public string saveTime;
    public string saveVersion;
    public bool isEncrypted;
    
    public CloudSave(string id, string playerID, string data, string version, bool encrypted)
    {
        saveID = id;
        this.playerID = playerID;
        saveData = data;
        saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        saveVersion = version;
        isEncrypted = encrypted;
    }
    
    public void UpdateSaveData(string data, string version)
    {
        saveData = data;
        saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        saveVersion = version;
    }
}

[System.Serializable]
public class CrossPlatformSyncManagerData
{
    public CrossPlatformSyncEnhanced system;
    
    public CrossPlatformSyncManagerData()
    {
        system = new CrossPlatformSyncEnhanced("cross_platform_sync_enhanced", "跨平台同步增强", "提供更详细的设备管理和数据同步优化");
    }
}
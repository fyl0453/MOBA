[System.Serializable]
public class CrossPlatformSyncSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AccountBinding> accountBindings;
    public List<DeviceSync> deviceSyncs;
    public List<SyncLog> syncLogs;
    
    public CrossPlatformSyncSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        accountBindings = new List<AccountBinding>();
        deviceSyncs = new List<DeviceSync>();
        syncLogs = new List<SyncLog>();
    }
    
    public void AddAccountBinding(AccountBinding binding)
    {
        accountBindings.Add(binding);
    }
    
    public void AddDeviceSync(DeviceSync sync)
    {
        deviceSyncs.Add(sync);
    }
    
    public void AddSyncLog(SyncLog log)
    {
        syncLogs.Add(log);
    }
    
    public AccountBinding GetAccountBinding(string bindingID)
    {
        return accountBindings.Find(b => b.bindingID == bindingID);
    }
    
    public List<AccountBinding> GetAccountBindingsByPlayer(string playerID)
    {
        return accountBindings.FindAll(b => b.playerID == playerID);
    }
    
    public DeviceSync GetDeviceSync(string syncID)
    {
        return deviceSyncs.Find(s => s.syncID == syncID);
    }
    
    public List<DeviceSync> GetDeviceSyncsByPlayer(string playerID)
    {
        return deviceSyncs.FindAll(s => s.playerID == playerID);
    }
    
    public List<SyncLog> GetSyncLogsByPlayer(string playerID)
    {
        return syncLogs.FindAll(l => l.playerID == playerID);
    }
}

[System.Serializable]
public class AccountBinding
{
    public string bindingID;
    public string playerID;
    public string playerName;
    public string platformType;
    public string platformAccountID;
    public string platformAccountName;
    public string bindingTime;
    public bool isPrimary;
    
    public AccountBinding(string id, string playerID, string playerName, string platformType, string platformAccountID, string platformAccountName, bool isPrimary)
    {
        bindingID = id;
        this.playerID = playerID;
        this.playerName = playerName;
        this.platformType = platformType;
        this.platformAccountID = platformAccountID;
        this.platformAccountName = platformAccountName;
        bindingTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.isPrimary = isPrimary;
    }
    
    public void SetAsPrimary(bool primary)
    {
        isPrimary = primary;
    }
}

[System.Serializable]
public class DeviceSync
{
    public string syncID;
    public string playerID;
    public string deviceID;
    public string deviceName;
    public string lastSyncTime;
    public string syncStatus;
    public string syncType;
    
    public DeviceSync(string id, string playerID, string deviceID, string deviceName, string syncType)
    {
        syncID = id;
        this.playerID = playerID;
        this.deviceID = deviceID;
        this.deviceName = deviceName;
        lastSyncTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        syncStatus = "Pending";
        this.syncType = syncType;
    }
    
    public void UpdateSyncStatus(string status)
    {
        syncStatus = status;
        lastSyncTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class SyncLog
{
    public string logID;
    public string playerID;
    public string syncType;
    public string syncStatus;
    public string syncTime;
    public string deviceID;
    public string deviceName;
    
    public SyncLog(string id, string playerID, string syncType, string syncStatus, string deviceID, string deviceName)
    {
        logID = id;
        this.playerID = playerID;
        this.syncType = syncType;
        this.syncStatus = syncStatus;
        syncTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.deviceID = deviceID;
        this.deviceName = deviceName;
    }
}

[System.Serializable]
public class CrossPlatformSyncManagerData
{
    public CrossPlatformSyncSystem system;
    
    public CrossPlatformSyncManagerData()
    {
        system = new CrossPlatformSyncSystem("cross_platform_sync_system", "跨平台同步系统", "管理跨平台数据同步");
    }
}
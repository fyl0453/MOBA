[System.Serializable]
public class CrossPlatformSyncSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<UserDevice> userDevices;
    public List<SyncRecord> syncRecords;
    public List<SyncSetting> syncSettings;
    public List<DataCategory> dataCategories;
    public List<SyncQueue> syncQueues;
    
    public CrossPlatformSyncSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        userDevices = new List<UserDevice>();
        syncRecords = new List<SyncRecord>();
        syncSettings = new List<SyncSetting>();
        dataCategories = new List<DataCategory>();
        syncQueues = new List<SyncQueue>();
    }
    
    public void AddUserDevice(UserDevice device)
    {
        userDevices.Add(device);
    }
    
    public void AddSyncRecord(SyncRecord record)
    {
        syncRecords.Add(record);
    }
    
    public void AddSyncSetting(SyncSetting setting)
    {
        syncSettings.Add(setting);
    }
    
    public void AddDataCategory(DataCategory category)
    {
        dataCategories.Add(category);
    }
    
    public void AddSyncQueue(SyncQueue queue)
    {
        syncQueues.Add(queue);
    }
    
    public UserDevice GetUserDevice(string deviceID)
    {
        return userDevices.Find(d => d.deviceID == deviceID);
    }
    
    public SyncRecord GetSyncRecord(string recordID)
    {
        return syncRecords.Find(r => r.recordID == recordID);
    }
    
    public SyncSetting GetSyncSetting(string userID)
    {
        return syncSettings.Find(s => s.userID == userID);
    }
    
    public DataCategory GetDataCategory(string categoryID)
    {
        return dataCategories.Find(c => c.categoryID == categoryID);
    }
    
    public SyncQueue GetSyncQueue(string queueID)
    {
        return syncQueues.Find(q => q.queueID == queueID);
    }
    
    public List<UserDevice> GetUserDevices(string userID)
    {
        return userDevices.FindAll(d => d.userID == userID);
    }
    
    public List<SyncRecord> GetSyncRecordsByUser(string userID)
    {
        return syncRecords.FindAll(r => r.userID == userID);
    }
    
    public List<SyncRecord> GetSyncRecordsByDevice(string deviceID)
    {
        return syncRecords.FindAll(r => r.deviceID == deviceID);
    }
    
    public List<SyncQueue> GetSyncQueuesByUser(string userID)
    {
        return syncQueues.FindAll(q => q.userID == userID);
    }
}

[System.Serializable]
public class UserDevice
{
    public string deviceID;
    public string userID;
    public string deviceName;
    public string deviceType;
    public string platform;
    public string deviceModel;
    public string osVersion;
    public string appVersion;
    public string lastSyncTime;
    public string firstLoginTime;
    public bool isPrimary;
    
    public UserDevice(string deviceID, string userID, string deviceName, string deviceType, string platform, string deviceModel, string osVersion, string appVersion, bool isPrimary)
    {
        this.deviceID = deviceID;
        this.userID = userID;
        this.deviceName = deviceName;
        this.deviceType = deviceType;
        this.platform = platform;
        this.deviceModel = deviceModel;
        this.osVersion = osVersion;
        this.appVersion = appVersion;
        lastSyncTime = "";
        firstLoginTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.isPrimary = isPrimary;
    }
    
    public void UpdateLastSync()
    {
        lastSyncTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateAppVersion(string version)
    {
        appVersion = version;
    }
    
    public void SetAsPrimary(bool primary)
    {
        isPrimary = primary;
    }
}

[System.Serializable]
public class SyncRecord
{
    public string recordID;
    public string userID;
    public string deviceID;
    public string syncType;
    public string dataCategory;
    public string status;
    public string startTime;
    public string endTime;
    public int dataSize;
    public string errorMessage;
    
    public SyncRecord(string recordID, string userID, string deviceID, string syncType, string dataCategory, string status, int dataSize)
    {
        this.recordID = recordID;
        this.userID = userID;
        this.deviceID = deviceID;
        this.syncType = syncType;
        this.dataCategory = dataCategory;
        this.status = status;
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        endTime = "";
        this.dataSize = dataSize;
        errorMessage = "";
    }
    
    public void CompleteSync(string status, string errorMessage = "")
    {
        this.status = status;
        this.errorMessage = errorMessage;
        endTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class SyncSetting
{
    public string settingID;
    public string userID;
    public bool autoSyncEnabled;
    public bool syncOnWiFiOnly;
    public bool syncOnCellularEnabled;
    public bool syncInBackground;
    public List<string> enabledDataCategories;
    public string lastSyncTime;
    
    public SyncSetting(string settingID, string userID)
    {
        this.settingID = settingID;
        this.userID = userID;
        autoSyncEnabled = true;
        syncOnWiFiOnly = true;
        syncOnCellularEnabled = false;
        syncInBackground = true;
        enabledDataCategories = new List<string>();
        lastSyncTime = "";
    }
    
    public void UpdateSyncSettings(bool autoSync, bool wifiOnly, bool cellularEnabled, bool backgroundSync)
    {
        autoSyncEnabled = autoSync;
        syncOnWiFiOnly = wifiOnly;
        syncOnCellularEnabled = cellularEnabled;
        syncInBackground = backgroundSync;
    }
    
    public void AddDataCategory(string categoryID)
    {
        if (!enabledDataCategories.Contains(categoryID))
        {
            enabledDataCategories.Add(categoryID);
        }
    }
    
    public void RemoveDataCategory(string categoryID)
    {
        enabledDataCategories.Remove(categoryID);
    }
    
    public void UpdateLastSync()
    {
        lastSyncTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class DataCategory
{
    public string categoryID;
    public string categoryName;
    public string description;
    public bool isEnabled;
    public int priority;
    public string syncFrequency;
    
    public DataCategory(string categoryID, string categoryName, string description, int priority, string syncFrequency)
    {
        this.categoryID = categoryID;
        this.categoryName = categoryName;
        this.description = description;
        isEnabled = true;
        this.priority = priority;
        this.syncFrequency = syncFrequency;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class SyncQueue
{
    public string queueID;
    public string userID;
    public string deviceID;
    public List<SyncItem> items;
    public string status;
    public string createTime;
    public string processTime;
    
    public SyncQueue(string queueID, string userID, string deviceID)
    {
        this.queueID = queueID;
        this.userID = userID;
        this.deviceID = deviceID;
        items = new List<SyncItem>();
        status = "pending";
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        processTime = "";
    }
    
    public void AddSyncItem(SyncItem item)
    {
        items.Add(item);
    }
    
    public void StartProcessing()
    {
        status = "processing";
    }
    
    public void CompleteProcessing()
    {
        status = "completed";
        processTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void FailProcessing()
    {
        status = "failed";
        processTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class SyncItem
{
    public string itemID;
    public string dataCategory;
    public string operationType;
    public string dataPath;
    public string dataHash;
    public int dataSize;
    public string status;
    
    public SyncItem(string itemID, string dataCategory, string operationType, string dataPath, string dataHash, int dataSize)
    {
        this.itemID = itemID;
        this.dataCategory = dataCategory;
        this.operationType = operationType;
        this.dataPath = dataPath;
        this.dataHash = dataHash;
        this.dataSize = dataSize;
        status = "pending";
    }
    
    public void MarkAsProcessing()
    {
        status = "processing";
    }
    
    public void MarkAsCompleted()
    {
        status = "completed";
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class CrossPlatformSyncSystemDetailedManagerData
{
    public CrossPlatformSyncSystemDetailed system;
    
    public CrossPlatformSyncSystemDetailedManagerData()
    {
        system = new CrossPlatformSyncSystemDetailed("cross_platform_sync_system_detailed", "跨平台同步系统详细", "管理跨平台同步的详细功能，包括不同设备之间的游戏数据同步优化");
    }
}
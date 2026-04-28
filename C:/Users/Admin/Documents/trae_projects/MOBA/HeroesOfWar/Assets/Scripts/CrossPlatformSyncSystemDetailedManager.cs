using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CrossPlatformSyncSystemDetailedManager : MonoBehaviour
{
    public static CrossPlatformSyncSystemDetailedManager Instance { get; private set; }
    
    public CrossPlatformSyncSystemDetailedManagerData syncData;
    
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
        LoadSyncData();
        
        if (syncData == null)
        {
            syncData = new CrossPlatformSyncSystemDetailedManagerData();
            InitializeDefaultCrossPlatformSyncSystem();
        }
    }
    
    private void InitializeDefaultCrossPlatformSyncSystem()
    {
        // 数据类别
        DataCategory category1 = new DataCategory("category_001", "游戏进度", "游戏关卡、任务完成情况等", 1, "realtime");
        DataCategory category2 = new DataCategory("category_002", "玩家数据", "等级、经验、金币等", 2, "realtime");
        DataCategory category3 = new DataCategory("category_003", "英雄数据", "已拥有英雄、熟练度等", 3, "realtime");
        DataCategory category4 = new DataCategory("category_004", "皮肤数据", "已拥有皮肤、使用情况等", 4, "realtime");
        DataCategory category5 = new DataCategory("category_005", "设置数据", "游戏设置、键位配置等", 5, "daily");
        DataCategory category6 = new DataCategory("category_006", "战绩数据", "游戏记录、胜率等", 6, "daily");
        
        syncData.system.AddDataCategory(category1);
        syncData.system.AddDataCategory(category2);
        syncData.system.AddDataCategory(category3);
        syncData.system.AddDataCategory(category4);
        syncData.system.AddDataCategory(category5);
        syncData.system.AddDataCategory(category6);
        
        // 用户设备
        UserDevice device1 = new UserDevice("device_001", "user_001", "iPhone 13", "mobile", "iOS", "iPhone14,2", "15.4", "1.0.0", true);
        UserDevice device2 = new UserDevice("device_002", "user_001", "MacBook Pro", "desktop", "macOS", "MacBookPro18,1", "12.3", "1.0.0", false);
        UserDevice device3 = new UserDevice("device_003", "user_002", "iPad Pro", "tablet", "iOS", "iPad13,4", "15.4", "1.0.0", true);
        UserDevice device4 = new UserDevice("device_004", "user_002", "Windows PC", "desktop", "Windows", "PC", "10.0.19044", "1.0.0", false);
        
        syncData.system.AddUserDevice(device1);
        syncData.system.AddUserDevice(device2);
        syncData.system.AddUserDevice(device3);
        syncData.system.AddUserDevice(device4);
        
        // 同步设置
        SyncSetting setting1 = new SyncSetting("setting_001", "user_001");
        setting1.AddDataCategory("category_001");
        setting1.AddDataCategory("category_002");
        setting1.AddDataCategory("category_003");
        setting1.AddDataCategory("category_004");
        setting1.AddDataCategory("category_005");
        setting1.AddDataCategory("category_006");
        
        SyncSetting setting2 = new SyncSetting("setting_002", "user_002");
        setting2.AddDataCategory("category_001");
        setting2.AddDataCategory("category_002");
        setting2.AddDataCategory("category_003");
        setting2.AddDataCategory("category_004");
        setting2.AddDataCategory("category_005");
        setting2.AddDataCategory("category_006");
        
        syncData.system.AddSyncSetting(setting1);
        syncData.system.AddSyncSetting(setting2);
        
        // 同步记录
        SyncRecord record1 = new SyncRecord("record_001", "user_001", "device_001", "upload", "category_001", "completed", 1024);
        record1.CompleteSync("completed");
        
        SyncRecord record2 = new SyncRecord("record_002", "user_001", "device_002", "download", "category_001", "completed", 1024);
        record2.CompleteSync("completed");
        
        SyncRecord record3 = new SyncRecord("record_003", "user_002", "device_003", "upload", "category_002", "completed", 2048);
        record3.CompleteSync("completed");
        
        syncData.system.AddSyncRecord(record1);
        syncData.system.AddSyncRecord(record2);
        syncData.system.AddSyncRecord(record3);
        
        // 同步队列
        SyncQueue queue1 = new SyncQueue("queue_001", "user_001", "device_001");
        queue1.AddSyncItem(new SyncItem("item_001", "category_001", "update", "data/game_progress.json", "hash123", 1024));
        queue1.AddSyncItem(new SyncItem("item_002", "category_002", "update", "data/player_data.json", "hash456", 2048));
        queue1.CompleteProcessing();
        
        SyncQueue queue2 = new SyncQueue("queue_002", "user_002", "device_003");
        queue2.AddSyncItem(new SyncItem("item_003", "category_003", "update", "data/hero_data.json", "hash789", 3072));
        queue2.AddSyncItem(new SyncItem("item_004", "category_004", "update", "data/skin_data.json", "hash012", 4096));
        queue2.CompleteProcessing();
        
        syncData.system.AddSyncQueue(queue1);
        syncData.system.AddSyncQueue(queue2);
        
        SaveSyncData();
    }
    
    // 设备管理
    public void AddUserDevice(string userID, string deviceName, string deviceType, string platform, string deviceModel, string osVersion, string appVersion, bool isPrimary)
    {
        string deviceID = "device_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        UserDevice device = new UserDevice(deviceID, userID, deviceName, deviceType, platform, deviceModel, osVersion, appVersion, isPrimary);
        syncData.system.AddUserDevice(device);
        
        // 如果设置为主要设备，将其他设备设置为非主要
        if (isPrimary)
        {
            List<UserDevice> userDevices = syncData.system.GetUserDevices(userID);
            foreach (UserDevice d in userDevices)
            {
                if (d.deviceID != deviceID)
                {
                    d.SetAsPrimary(false);
                }
            }
        }
        
        SaveSyncData();
        Debug.Log("成功添加用户设备: " + deviceName);
    }
    
    public void UpdateDeviceAppVersion(string deviceID, string appVersion)
    {
        UserDevice device = syncData.system.GetUserDevice(deviceID);
        if (device != null)
        {
            device.UpdateAppVersion(appVersion);
            SaveSyncData();
            Debug.Log("成功更新设备应用版本: " + appVersion);
        }
        else
        {
            Debug.LogError("设备不存在: " + deviceID);
        }
    }
    
    public void SetPrimaryDevice(string deviceID, bool isPrimary)
    {
        UserDevice device = syncData.system.GetUserDevice(deviceID);
        if (device != null)
        {
            // 如果设置为主要设备，将其他设备设置为非主要
            if (isPrimary)
            {
                List<UserDevice> userDevices = syncData.system.GetUserDevices(device.userID);
                foreach (UserDevice d in userDevices)
                {
                    d.SetAsPrimary(d.deviceID == deviceID);
                }
            }
            else
            {
                device.SetAsPrimary(false);
            }
            SaveSyncData();
            Debug.Log("成功设置主要设备: " + (isPrimary ? "是" : "否"));
        }
        else
        {
            Debug.LogError("设备不存在: " + deviceID);
        }
    }
    
    public List<UserDevice> GetUserDevices(string userID)
    {
        return syncData.system.GetUserDevices(userID);
    }
    
    // 同步设置管理
    public void AddSyncSetting(string userID)
    {
        SyncSetting existing = syncData.system.GetSyncSetting(userID);
        if (existing == null)
        {
            string settingID = "setting_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            SyncSetting setting = new SyncSetting(settingID, userID);
            syncData.system.AddSyncSetting(setting);
            SaveSyncData();
            Debug.Log("成功添加同步设置: " + userID);
        }
    }
    
    public void UpdateSyncSettings(string userID, bool autoSync, bool wifiOnly, bool cellularEnabled, bool backgroundSync)
    {
        SyncSetting setting = syncData.system.GetSyncSetting(userID);
        if (setting != null)
        {
            setting.UpdateSyncSettings(autoSync, wifiOnly, cellularEnabled, backgroundSync);
            SaveSyncData();
            Debug.Log("成功更新同步设置");
        }
        else
        {
            AddSyncSetting(userID);
            UpdateSyncSettings(userID, autoSync, wifiOnly, cellularEnabled, backgroundSync);
        }
    }
    
    public void AddDataCategoryToSync(string userID, string categoryID)
    {
        SyncSetting setting = syncData.system.GetSyncSetting(userID);
        if (setting != null)
        {
            setting.AddDataCategory(categoryID);
            SaveSyncData();
            Debug.Log("成功添加同步数据类别: " + categoryID);
        }
        else
        {
            AddSyncSetting(userID);
            AddDataCategoryToSync(userID, categoryID);
        }
    }
    
    public void RemoveDataCategoryFromSync(string userID, string categoryID)
    {
        SyncSetting setting = syncData.system.GetSyncSetting(userID);
        if (setting != null)
        {
            setting.RemoveDataCategory(categoryID);
            SaveSyncData();
            Debug.Log("成功移除同步数据类别: " + categoryID);
        }
        else
        {
            Debug.LogError("同步设置不存在: " + userID);
        }
    }
    
    // 同步操作
    public string CreateSyncRecord(string userID, string deviceID, string syncType, string dataCategory, int dataSize)
    {
        string recordID = "record_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SyncRecord record = new SyncRecord(recordID, userID, deviceID, syncType, dataCategory, "processing", dataSize);
        syncData.system.AddSyncRecord(record);
        SaveSyncData();
        Debug.Log("成功创建同步记录: " + recordID);
        return recordID;
    }
    
    public void CompleteSyncRecord(string recordID, string status, string errorMessage = "")
    {
        SyncRecord record = syncData.system.GetSyncRecord(recordID);
        if (record != null)
        {
            record.CompleteSync(status, errorMessage);
            
            // 更新设备的最后同步时间
            UserDevice device = syncData.system.GetUserDevice(record.deviceID);
            if (device != null)
            {
                device.UpdateLastSync();
            }
            
            // 更新用户的最后同步时间
            SyncSetting setting = syncData.system.GetSyncSetting(record.userID);
            if (setting != null)
            {
                setting.UpdateLastSync();
            }
            
            SaveSyncData();
            Debug.Log("成功完成同步记录: " + status);
        }
        else
        {
            Debug.LogError("同步记录不存在: " + recordID);
        }
    }
    
    public string CreateSyncQueue(string userID, string deviceID)
    {
        string queueID = "queue_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SyncQueue queue = new SyncQueue(queueID, userID, deviceID);
        syncData.system.AddSyncQueue(queue);
        SaveSyncData();
        Debug.Log("成功创建同步队列: " + queueID);
        return queueID;
    }
    
    public void AddSyncItem(string queueID, string dataCategory, string operationType, string dataPath, string dataHash, int dataSize)
    {
        SyncQueue queue = syncData.system.GetSyncQueue(queueID);
        if (queue != null)
        {
            string itemID = "item_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            SyncItem item = new SyncItem(itemID, dataCategory, operationType, dataPath, dataHash, dataSize);
            queue.AddSyncItem(item);
            SaveSyncData();
            Debug.Log("成功添加同步项: " + itemID);
        }
        else
        {
            Debug.LogError("同步队列不存在: " + queueID);
        }
    }
    
    public void ProcessSyncQueue(string queueID)
    {
        SyncQueue queue = syncData.system.GetSyncQueue(queueID);
        if (queue != null)
        {
            queue.StartProcessing();
            
            // 模拟同步过程
            foreach (SyncItem item in queue.items)
            {
                item.MarkAsProcessing();
                // 这里可以添加实际的同步逻辑
                item.MarkAsCompleted();
            }
            
            queue.CompleteProcessing();
            SaveSyncData();
            Debug.Log("成功处理同步队列: " + queueID);
        }
        else
        {
            Debug.LogError("同步队列不存在: " + queueID);
        }
    }
    
    // 数据类别管理
    public void AddDataCategory(string categoryName, string description, int priority, string syncFrequency)
    {
        string categoryID = "category_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        DataCategory category = new DataCategory(categoryID, categoryName, description, priority, syncFrequency);
        syncData.system.AddDataCategory(category);
        SaveSyncData();
        Debug.Log("成功添加数据类别: " + categoryName);
    }
    
    public void EnableDataCategory(string categoryID)
    {
        DataCategory category = syncData.system.GetDataCategory(categoryID);
        if (category != null)
        {
            category.Enable();
            SaveSyncData();
            Debug.Log("成功启用数据类别: " + categoryID);
        }
        else
        {
            Debug.LogError("数据类别不存在: " + categoryID);
        }
    }
    
    public void DisableDataCategory(string categoryID)
    {
        DataCategory category = syncData.system.GetDataCategory(categoryID);
        if (category != null)
        {
            category.Disable();
            SaveSyncData();
            Debug.Log("成功禁用数据类别: " + categoryID);
        }
        else
        {
            Debug.LogError("数据类别不存在: " + categoryID);
        }
    }
    
    public List<DataCategory> GetAllDataCategories()
    {
        return syncData.system.dataCategories;
    }
    
    // 数据持久化
    public void SaveSyncData()
    {
        string path = Application.dataPath + "/Data/cross_platform_sync_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, syncData);
        stream.Close();
    }
    
    public void LoadSyncData()
    {
        string path = Application.dataPath + "/Data/cross_platform_sync_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            syncData = (CrossPlatformSyncSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            syncData = new CrossPlatformSyncSystemDetailedManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CrossPlatformSyncManagerEnhanced : MonoBehaviour
{
    public static CrossPlatformSyncManagerEnhanced Instance { get; private set; }
    
    public CrossPlatformSyncManagerData crossPlatformSyncData;
    
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
        LoadCrossPlatformSyncData();
        
        if (crossPlatformSyncData == null)
        {
            crossPlatformSyncData = new CrossPlatformSyncManagerData();
        }
    }
    
    public string BindAccount(string playerID, string platform, string platformID, string platformName)
    {
        AccountBinding existing = crossPlatformSyncData.system.GetAccountBinding(playerID, platform);
        if (existing != null)
        {
            Debug.Log("账号已经绑定");
            return existing.bindingID;
        }
        
        string bindingID = System.Guid.NewGuid().ToString();
        AccountBinding newBinding = new AccountBinding(bindingID, playerID, platform, platformID, platformName);
        crossPlatformSyncData.system.AddAccountBinding(newBinding);
        SaveCrossPlatformSyncData();
        Debug.Log($"成功绑定{platform}账号: {platformName}");
        return bindingID;
    }
    
    public void VerifyAccountBinding(string bindingID)
    {
        AccountBinding binding = crossPlatformSyncData.system.accountBindings.Find(b => b.bindingID == bindingID);
        if (binding != null)
        {
            binding.Verify();
            SaveCrossPlatformSyncData();
            Debug.Log("成功验证账号绑定");
        }
    }
    
    public string RegisterDevice(string playerID, string deviceName, string deviceType, string deviceOS, string deviceModel, string appVersion)
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        DeviceInfo existing = crossPlatformSyncData.system.GetDeviceInfo(deviceID);
        if (existing != null)
        {
            existing.UpdateLastLoginTime();
            existing.deviceName = deviceName;
            existing.appVersion = appVersion;
            SaveCrossPlatformSyncData();
            Debug.Log("设备已更新");
            return existing.deviceID;
        }
        
        DeviceInfo newDevice = new DeviceInfo(deviceID, playerID, deviceName, deviceType, deviceOS, deviceModel, appVersion);
        crossPlatformSyncData.system.AddDeviceInfo(newDevice);
        SaveCrossPlatformSyncData();
        Debug.Log($"成功注册设备: {deviceName}");
        return deviceID;
    }
    
    public void AuthorizeDevice(string deviceID)
    {
        DeviceInfo device = crossPlatformSyncData.system.GetDeviceInfo(deviceID);
        if (device != null)
        {
            device.Authorize();
            SaveCrossPlatformSyncData();
            Debug.Log("设备已授权");
        }
    }
    
    public void RevokeDeviceAuthorization(string deviceID)
    {
        DeviceInfo device = crossPlatformSyncData.system.GetDeviceInfo(deviceID);
        if (device != null)
        {
            device.RevokeAuthorization();
            SaveCrossPlatformSyncData();
            Debug.Log("设备授权已撤销");
        }
    }
    
    public string SyncData(string playerID, string deviceID, string syncType, string syncData)
    {
        DeviceInfo device = crossPlatformSyncData.system.GetDeviceInfo(deviceID);
        if (device == null || !device.isAuthorized)
        {
            Debug.Log("设备未授权");
            return "";
        }
        
        string recordID = System.Guid.NewGuid().ToString();
        SyncRecord newRecord = new SyncRecord(recordID, playerID, deviceID, syncType, "Success", syncData);
        crossPlatformSyncData.system.AddSyncRecord(newRecord);
        
        // 更新设备最后同步时间
        device.UpdateLastSyncTime();
        
        // 更新云存档
        UpdateCloudSave(playerID, syncData);
        
        SaveCrossPlatformSyncData();
        Debug.Log("数据同步成功");
        return recordID;
    }
    
    public void UpdateCloudSave(string playerID, string saveData)
    {
        CloudSave existing = crossPlatformSyncData.system.GetCloudSave(playerID);
        if (existing != null)
        {
            existing.UpdateSaveData(saveData, Application.version);
        }
        else
        {
            string saveID = System.Guid.NewGuid().ToString();
            CloudSave newSave = new CloudSave(saveID, playerID, saveData, Application.version, true);
            crossPlatformSyncData.system.AddCloudSave(newSave);
        }
    }
    
    public string GetCloudSave(string playerID)
    {
        CloudSave save = crossPlatformSyncData.system.GetCloudSave(playerID);
        if (save != null)
        {
            return save.saveData;
        }
        return "";
    }
    
    public List<DeviceInfo> GetPlayerDevices(string playerID)
    {
        return crossPlatformSyncData.system.GetPlayerDevices(playerID);
    }
    
    public List<SyncRecord> GetPlayerSyncRecords(string playerID, int limit = 50)
    {
        return crossPlatformSyncData.system.GetPlayerSyncRecords(playerID, limit);
    }
    
    public List<AccountBinding> GetPlayerAccountBindings(string playerID)
    {
        List<AccountBinding> bindings = new List<AccountBinding>();
        foreach (AccountBinding binding in crossPlatformSyncData.system.accountBindings)
        {
            if (binding.playerID == playerID)
            {
                bindings.Add(binding);
            }
        }
        return bindings;
    }
    
    public void ResolveSyncConflict(string conflictID, string resolution)
    {
        SyncConflict conflict = crossPlatformSyncData.system.syncConflicts.Find(c => c.conflictID == conflictID);
        if (conflict != null)
        {
            conflict.Resolve(resolution);
            SaveCrossPlatformSyncData();
            Debug.Log("同步冲突已解决");
        }
    }
    
    public void SaveCrossPlatformSyncData()
    {
        string path = Application.dataPath + "/Data/cross_platform_sync_enhanced_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, crossPlatformSyncData);
        stream.Close();
    }
    
    public void LoadCrossPlatformSyncData()
    {
        string path = Application.dataPath + "/Data/cross_platform_sync_enhanced_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            crossPlatformSyncData = (CrossPlatformSyncManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            crossPlatformSyncData = new CrossPlatformSyncManagerData();
        }
    }
}
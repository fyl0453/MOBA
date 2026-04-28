using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CrossPlatformSyncManager : MonoBehaviour
{
    public static CrossPlatformSyncManager Instance { get; private set; }
    
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
    
    public string BindAccount(string playerID, string playerName, string platformType, string platformAccountID, string platformAccountName, bool isPrimary)
    {
        // 检查是否已经绑定
        List<AccountBinding> existingBindings = crossPlatformSyncData.system.GetAccountBindingsByPlayer(playerID);
        AccountBinding existingBinding = existingBindings.Find(b => b.platformType == platformType);
        
        if (existingBinding != null)
        {
            Debug.Log($"该平台账号已经绑定: {platformType}");
            return existingBinding.bindingID;
        }
        
        string bindingID = System.Guid.NewGuid().ToString();
        AccountBinding newBinding = new AccountBinding(bindingID, playerID, playerName, platformType, platformAccountID, platformAccountName, isPrimary);
        crossPlatformSyncData.system.AddAccountBinding(newBinding);
        
        // 如果设置为主要账号，将其他绑定设置为非主要
        if (isPrimary)
        {
            foreach (AccountBinding binding in existingBindings)
            {
                binding.SetAsPrimary(false);
            }
        }
        
        SaveCrossPlatformSyncData();
        Debug.Log($"成功绑定{platformType}账号: {platformAccountName}");
        return bindingID;
    }
    
    public void UnbindAccount(string bindingID)
    {
        AccountBinding binding = crossPlatformSyncData.system.GetAccountBinding(bindingID);
        if (binding != null)
        {
            crossPlatformSyncData.system.accountBindings.Remove(binding);
            SaveCrossPlatformSyncData();
            Debug.Log($"成功解绑账号: {binding.platformType}");
        }
    }
    
    public string SyncDeviceData(string playerID, string deviceID, string deviceName, string syncType)
    {
        string syncID = System.Guid.NewGuid().ToString();
        DeviceSync newSync = new DeviceSync(syncID, playerID, deviceID, deviceName, syncType);
        crossPlatformSyncData.system.AddDeviceSync(newSync);
        
        // 模拟同步过程
        newSync.UpdateSyncStatus("InProgress");
        SaveCrossPlatformSyncData();
        
        // 模拟同步完成
        newSync.UpdateSyncStatus("Completed");
        
        // 记录同步日志
        string logID = System.Guid.NewGuid().ToString();
        SyncLog syncLog = new SyncLog(logID, playerID, syncType, "Completed", deviceID, deviceName);
        crossPlatformSyncData.system.AddSyncLog(syncLog);
        
        SaveCrossPlatformSyncData();
        Debug.Log($"成功同步设备数据: {deviceName}");
        return syncID;
    }
    
    public List<AccountBinding> GetPlayerAccountBindings(string playerID)
    {
        return crossPlatformSyncData.system.GetAccountBindingsByPlayer(playerID);
    }
    
    public List<DeviceSync> GetPlayerDeviceSyncs(string playerID)
    {
        return crossPlatformSyncData.system.GetDeviceSyncsByPlayer(playerID);
    }
    
    public List<SyncLog> GetPlayerSyncLogs(string playerID, int limit = 50)
    {
        List<SyncLog> logs = crossPlatformSyncData.system.GetSyncLogsByPlayer(playerID);
        logs.Sort((a, b) => b.syncTime.CompareTo(a.syncTime));
        return logs.GetRange(0, Mathf.Min(limit, logs.Count));
    }
    
    public AccountBinding GetPrimaryAccount(string playerID)
    {
        List<AccountBinding> bindings = crossPlatformSyncData.system.GetAccountBindingsByPlayer(playerID);
        return bindings.Find(b => b.isPrimary);
    }
    
    public void SetPrimaryAccount(string bindingID)
    {
        AccountBinding binding = crossPlatformSyncData.system.GetAccountBinding(bindingID);
        if (binding != null)
        {
            // 将所有绑定设置为非主要
            List<AccountBinding> bindings = crossPlatformSyncData.system.GetAccountBindingsByPlayer(binding.playerID);
            foreach (AccountBinding b in bindings)
            {
                b.SetAsPrimary(false);
            }
            
            // 设置当前绑定为主要
            binding.SetAsPrimary(true);
            SaveCrossPlatformSyncData();
            Debug.Log($"成功设置{binding.platformType}为主要账号");
        }
    }
    
    public void SaveCrossPlatformSyncData()
    {
        string path = Application.dataPath + "/Data/cross_platform_sync_data.dat";
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
        string path = Application.dataPath + "/Data/cross_platform_sync_data.dat";
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
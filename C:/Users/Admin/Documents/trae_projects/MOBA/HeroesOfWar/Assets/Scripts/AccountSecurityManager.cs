using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AccountSecurityManager : MonoBehaviour
{
    public static AccountSecurityManager Instance { get; private set; }
    
    public AccountSecurityManagerData accountSecurityData;
    
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
        LoadAccountSecurityData();
        
        if (accountSecurityData == null)
        {
            accountSecurityData = new AccountSecurityManagerData();
        }
    }
    
    public void InitializeAccountSecurity(string playerID, string playerName)
    {
        AccountSecurity existing = accountSecurityData.system.GetAccountSecurity(playerID);
        if (existing == null)
        {
            AccountSecurity newAccountSecurity = new AccountSecurity(playerID, playerName);
            accountSecurityData.system.AddAccountSecurity(newAccountSecurity);
            SaveAccountSecurityData();
            Debug.Log($"成功初始化账号安全: {playerName}");
        }
    }
    
    public void SetSecondaryPassword(string playerID, string password)
    {
        AccountSecurity accountSecurity = accountSecurityData.system.GetAccountSecurity(playerID);
        if (accountSecurity != null)
        {
            accountSecurity.SetSecondaryPassword(password);
            SaveAccountSecurityData();
            Debug.Log("成功设置二级密码");
        }
    }
    
    public bool VerifySecondaryPassword(string playerID, string password)
    {
        AccountSecurity accountSecurity = accountSecurityData.system.GetAccountSecurity(playerID);
        if (accountSecurity != null && accountSecurity.secondaryPassword == password)
        {
            return true;
        }
        return false;
    }
    
    public void BindPhoneNumber(string playerID, string phoneNumber)
    {
        AccountSecurity accountSecurity = accountSecurityData.system.GetAccountSecurity(playerID);
        if (accountSecurity != null)
        {
            accountSecurity.SetPhoneNumber(phoneNumber);
            SaveAccountSecurityData();
            Debug.Log("成功绑定手机号");
        }
    }
    
    public void BindEmail(string playerID, string email)
    {
        AccountSecurity accountSecurity = accountSecurityData.system.GetAccountSecurity(playerID);
        if (accountSecurity != null)
        {
            accountSecurity.SetEmail(email);
            SaveAccountSecurityData();
            Debug.Log("成功绑定邮箱");
        }
    }
    
    public void VerifyPhoneNumber(string playerID)
    {
        AccountSecurity accountSecurity = accountSecurityData.system.GetAccountSecurity(playerID);
        if (accountSecurity != null)
        {
            accountSecurity.VerifyPhone();
            SaveAccountSecurityData();
            Debug.Log("成功验证手机号");
        }
    }
    
    public void VerifyEmail(string playerID)
    {
        AccountSecurity accountSecurity = accountSecurityData.system.GetAccountSecurity(playerID);
        if (accountSecurity != null)
        {
            accountSecurity.VerifyEmail();
            SaveAccountSecurityData();
            Debug.Log("成功验证邮箱");
        }
    }
    
    public void EnableTwoFactorAuthentication(string playerID, bool enable)
    {
        AccountSecurity accountSecurity = accountSecurityData.system.GetAccountSecurity(playerID);
        if (accountSecurity != null)
        {
            accountSecurity.EnableTwoFactor(enable);
            SaveAccountSecurityData();
            Debug.Log($"成功{(enable ? "启用" : "禁用")}双因素认证");
        }
    }
    
    public void RecordLogin(string playerID, string deviceID, string deviceName, string deviceType, string deviceOS, string ipAddress)
    {
        // 记录登录设备
        List<LoginDevice> devices = accountSecurityData.system.GetLoginDevicesByPlayer(playerID);
        LoginDevice existingDevice = devices.Find(d => d.deviceID == deviceID);
        
        if (existingDevice != null)
        {
            existingDevice.UpdateLastLogin(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ipAddress);
        }
        else
        {
            LoginDevice newDevice = new LoginDevice(deviceID, playerID, deviceName, deviceType, deviceOS);
            newDevice.UpdateLastLogin(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ipAddress);
            accountSecurityData.system.AddLoginDevice(newDevice);
        }
        
        // 更新账号最后登录信息
        AccountSecurity accountSecurity = accountSecurityData.system.GetAccountSecurity(playerID);
        if (accountSecurity != null)
        {
            accountSecurity.UpdateLastLogin(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ipAddress);
        }
        
        // 记录安全日志
        string logID = System.Guid.NewGuid().ToString();
        SecurityLog securityLog = new SecurityLog(logID, playerID, "Login", "账号登录", ipAddress, deviceID);
        accountSecurityData.system.AddSecurityLog(securityLog);
        
        SaveAccountSecurityData();
        Debug.Log("成功记录登录信息");
    }
    
    public void TrustDevice(string playerID, string deviceID, bool trusted)
    {
        List<LoginDevice> devices = accountSecurityData.system.GetLoginDevicesByPlayer(playerID);
        LoginDevice device = devices.Find(d => d.deviceID == deviceID);
        if (device != null)
        {
            device.SetTrusted(trusted);
            SaveAccountSecurityData();
            Debug.Log($"成功{(trusted ? "信任" : "取消信任")}设备: {device.deviceName}");
        }
    }
    
    public List<LoginDevice> GetLoginDevices(string playerID)
    {
        return accountSecurityData.system.GetLoginDevicesByPlayer(playerID);
    }
    
    public List<SecurityLog> GetSecurityLogs(string playerID, int limit = 50)
    {
        List<SecurityLog> logs = accountSecurityData.system.GetSecurityLogsByPlayer(playerID);
        logs.Sort((a, b) => b.logTime.CompareTo(a.logTime));
        return logs.GetRange(0, Mathf.Min(limit, logs.Count));
    }
    
    public AccountSecurity GetAccountSecurity(string playerID)
    {
        return accountSecurityData.system.GetAccountSecurity(playerID);
    }
    
    public void AddSecurityLog(string playerID, string logType, string logMessage, string ipAddress, string deviceID)
    {
        string logID = System.Guid.NewGuid().ToString();
        SecurityLog securityLog = new SecurityLog(logID, playerID, logType, logMessage, ipAddress, deviceID);
        accountSecurityData.system.AddSecurityLog(securityLog);
        SaveAccountSecurityData();
    }
    
    public void SaveAccountSecurityData()
    {
        string path = Application.dataPath + "/Data/account_security_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, accountSecurityData);
        stream.Close();
    }
    
    public void LoadAccountSecurityData()
    {
        string path = Application.dataPath + "/Data/account_security_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            accountSecurityData = (AccountSecurityManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            accountSecurityData = new AccountSecurityManagerData();
        }
    }
}
[System.Serializable]
public class AccountSecuritySystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AccountSecurity> accountSecurities;
    public List<LoginDevice> loginDevices;
    public List<SecurityLog> securityLogs;
    
    public AccountSecuritySystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        accountSecurities = new List<AccountSecurity>();
        loginDevices = new List<LoginDevice>();
        securityLogs = new List<SecurityLog>();
    }
    
    public void AddAccountSecurity(AccountSecurity accountSecurity)
    {
        accountSecurities.Add(accountSecurity);
    }
    
    public void AddLoginDevice(LoginDevice loginDevice)
    {
        loginDevices.Add(loginDevice);
    }
    
    public void AddSecurityLog(SecurityLog securityLog)
    {
        securityLogs.Add(securityLog);
    }
    
    public AccountSecurity GetAccountSecurity(string playerID)
    {
        return accountSecurities.Find(a => a.playerID == playerID);
    }
    
    public List<LoginDevice> GetLoginDevicesByPlayer(string playerID)
    {
        return loginDevices.FindAll(d => d.playerID == playerID);
    }
    
    public List<SecurityLog> GetSecurityLogsByPlayer(string playerID)
    {
        return securityLogs.FindAll(l => l.playerID == playerID);
    }
}

[System.Serializable]
public class AccountSecurity
{
    public string playerID;
    public string playerName;
    public string secondaryPassword;
    public string phoneNumber;
    public string email;
    public bool phoneVerified;
    public bool emailVerified;
    public bool twoFactorEnabled;
    public string lastLoginTime;
    public string lastLoginIP;
    
    public AccountSecurity(string playerID, string playerName)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        secondaryPassword = "";
        phoneNumber = "";
        email = "";
        phoneVerified = false;
        emailVerified = false;
        twoFactorEnabled = false;
        lastLoginTime = "";
        lastLoginIP = "";
    }
    
    public void SetSecondaryPassword(string password)
    {
        secondaryPassword = password;
    }
    
    public void SetPhoneNumber(string phone)
    {
        phoneNumber = phone;
    }
    
    public void SetEmail(string email)
    {
        this.email = email;
    }
    
    public void VerifyPhone()
    {
        phoneVerified = true;
    }
    
    public void VerifyEmail()
    {
        emailVerified = true;
    }
    
    public void EnableTwoFactor(bool enable)
    {
        twoFactorEnabled = enable;
    }
    
    public void UpdateLastLogin(string time, string ip)
    {
        lastLoginTime = time;
        lastLoginIP = ip;
    }
}

[System.Serializable]
public class LoginDevice
{
    public string deviceID;
    public string playerID;
    public string deviceName;
    public string deviceType;
    public string deviceOS;
    public string lastLoginTime;
    public string lastLoginIP;
    public bool isTrusted;
    
    public LoginDevice(string deviceID, string playerID, string deviceName, string deviceType, string deviceOS)
    {
        this.deviceID = deviceID;
        this.playerID = playerID;
        this.deviceName = deviceName;
        this.deviceType = deviceType;
        this.deviceOS = deviceOS;
        lastLoginTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastLoginIP = "";
        isTrusted = false;
    }
    
    public void UpdateLastLogin(string time, string ip)
    {
        lastLoginTime = time;
        lastLoginIP = ip;
    }
    
    public void SetTrusted(bool trusted)
    {
        isTrusted = trusted;
    }
}

[System.Serializable]
public class SecurityLog
{
    public string logID;
    public string playerID;
    public string logType;
    public string logMessage;
    public string logTime;
    public string ipAddress;
    public string deviceID;
    
    public SecurityLog(string id, string playerID, string logType, string logMessage, string ip, string deviceID)
    {
        logID = id;
        this.playerID = playerID;
        this.logType = logType;
        this.logMessage = logMessage;
        logTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        ipAddress = ip;
        this.deviceID = deviceID;
    }
}

[System.Serializable]
public class AccountSecurityManagerData
{
    public AccountSecuritySystem system;
    
    public AccountSecurityManagerData()
    {
        system = new AccountSecuritySystem("account_security_system", "账号安全系统", "管理账号安全");
    }
}
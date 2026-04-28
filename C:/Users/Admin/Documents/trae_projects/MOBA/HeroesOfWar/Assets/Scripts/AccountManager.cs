using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance { get; private set; }
    
    public bool isLoggedIn = false;
    public string currentUserID;
    public string currentUsername;
    
    private string accountDirectory;
    private Dictionary<string, AccountData> accounts = new Dictionary<string, AccountData>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAccountSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeAccountSystem()
    {
        accountDirectory = Path.Combine(Application.dataPath, "Accounts");
        Directory.CreateDirectory(accountDirectory);
        LoadAccounts();
    }
    
    private void LoadAccounts()
    {
        accounts.Clear();
        
        string[] accountFiles = Directory.GetFiles(accountDirectory, "*.json");
        foreach (string file in accountFiles)
        {
            try
            {
                string json = File.ReadAllText(file);
                AccountData account = JsonUtility.FromJson<AccountData>(json);
                if (account != null)
                {
                    accounts[account.userID] = account;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("加载账号数据失败: " + e.Message);
            }
        }
    }
    
    public bool Register(string username, string password, string email)
    {
        // 检查用户名是否已存在
        foreach (var account in accounts.Values)
        {
            if (account.username == username)
            {
                Debug.LogError("用户名已存在");
                return false;
            }
        }
        
        // 创建新账号
        string userID = System.Guid.NewGuid().ToString();
        AccountData newAccount = new AccountData
        {
            userID = userID,
            username = username,
            password = password, // 实际应用中应该加密存储
            email = email,
            registerDate = System.DateTime.Now.ToString(),
            lastLoginDate = System.DateTime.Now.ToString(),
            level = 1,
            experience = 0,
            gold = 1000,
            diamonds = 100
        };
        
        // 保存账号数据
        SaveAccount(newAccount);
        accounts[userID] = newAccount;
        
        Debug.Log("注册成功");
        return true;
    }
    
    public bool Login(string username, string password)
    {
        // 查找账号
        foreach (var account in accounts.Values)
        {
            if (account.username == username && account.password == password)
            {
                // 更新最后登录时间
                account.lastLoginDate = System.DateTime.Now.ToString();
                SaveAccount(account);
                
                // 设置当前登录状态
                isLoggedIn = true;
                currentUserID = account.userID;
                currentUsername = account.username;
                
                Debug.Log("登录成功");
                return true;
            }
        }
        
        Debug.LogError("用户名或密码错误");
        return false;
    }
    
    public void Logout()
    {
        isLoggedIn = false;
        currentUserID = null;
        currentUsername = null;
        Debug.Log("退出登录");
    }
    
    private void SaveAccount(AccountData account)
    {
        string filePath = Path.Combine(accountDirectory, $"{account.userID}.json");
        
        try
        {
            string json = JsonUtility.ToJson(account, true);
            File.WriteAllText(filePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("保存账号数据失败: " + e.Message);
        }
    }
    
    public AccountData GetCurrentAccount()
    {
        if (isLoggedIn && accounts.ContainsKey(currentUserID))
        {
            return accounts[currentUserID];
        }
        return null;
    }
    
    public void UpdateAccountData()
    {
        if (isLoggedIn && accounts.ContainsKey(currentUserID))
        {
            SaveAccount(accounts[currentUserID]);
        }
    }
    
    public void AddGold(int amount)
    {
        if (isLoggedIn && accounts.ContainsKey(currentUserID))
        {
            accounts[currentUserID].gold += amount;
            UpdateAccountData();
        }
    }
    
    public void AddDiamonds(int amount)
    {
        if (isLoggedIn && accounts.ContainsKey(currentUserID))
        {
            accounts[currentUserID].diamonds += amount;
            UpdateAccountData();
        }
    }
    
    public void AddExperience(int amount)
    {
        if (isLoggedIn && accounts.ContainsKey(currentUserID))
        {
            AccountData account = accounts[currentUserID];
            account.experience += amount;
            
            // 检查是否升级
            while (account.experience >= GetRequiredExperience(account.level))
            {
                account.experience -= GetRequiredExperience(account.level);
                account.level++;
                Debug.Log($"升级了！现在是 {account.level} 级");
            }
            
            UpdateAccountData();
        }
    }
    
    private int GetRequiredExperience(int level)
    {
        // 简单的经验计算公式
        return 100 * level;
    }
}

[System.Serializable]
public class AccountData
{
    public string userID;
    public string username;
    public string password;
    public string email;
    public string registerDate;
    public string lastLoginDate;
    public int level;
    public int experience;
    public int gold;
    public int diamonds;
}

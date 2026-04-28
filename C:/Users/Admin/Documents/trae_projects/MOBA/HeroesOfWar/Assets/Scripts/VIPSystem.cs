[System.Serializable]
public class VIP
{
    public string vipID;
    public int vipLevel;
    public int requiredPoints;
    public List<VIPPrivilege> privileges;
    
    public VIP(string id, int level, int points)
    {
        vipID = id;
        vipLevel = level;
        requiredPoints = points;
        privileges = new List<VIPPrivilege>();
    }
    
    public void AddPrivilege(string privilegeName, string privilegeDescription)
    {
        VIPPrivilege privilege = new VIPPrivilege(privilegeName, privilegeDescription);
        privileges.Add(privilege);
    }
}

[System.Serializable]
public class VIPPrivilege
{
    public string privilegeName;
    public string privilegeDescription;
    
    public VIPPrivilege(string name, string description)
    {
        privilegeName = name;
        privilegeDescription = description;
    }
}

[System.Serializable]
public class VIPUser
{
    public string playerID;
    public int currentLevel;
    public int currentPoints;
    public List<VIP> vipLevels;
    
    public VIPUser(string player)
    {
        playerID = player;
        currentLevel = 0;
        currentPoints = 0;
        vipLevels = new List<VIP>();
        InitializeVIPLevels();
    }
    
    private void InitializeVIPLevels()
    {
        VIP vip1 = new VIP("vip_1", 1, 100);
        vip1.AddPrivilege("专属头像框", "获得VIP1专属头像框");
        vip1.AddPrivilege("每日金币", "每日额外获得100金币");
        vipLevels.Add(vip1);
        
        VIP vip2 = new VIP("vip_2", 2, 300);
        vip2.AddPrivilege("专属头像框", "获得VIP2专属头像框");
        vip2.AddPrivilege("每日金币", "每日额外获得200金币");
        vip2.AddPrivilege("专属皮肤", "获得VIP2专属皮肤");
        vipLevels.Add(vip2);
        
        VIP vip3 = new VIP("vip_3", 3, 500);
        vip3.AddPrivilege("专属头像框", "获得VIP3专属头像框");
        vip3.AddPrivilege("每日金币", "每日额外获得300金币");
        vip3.AddPrivilege("专属皮肤", "获得VIP3专属皮肤");
        vip3.AddPrivilege("专属称号", "获得VIP3专属称号");
        vipLevels.Add(vip3);
        
        VIP vip4 = new VIP("vip_4", 4, 1000);
        vip4.AddPrivilege("专属头像框", "获得VIP4专属头像框");
        vip4.AddPrivilege("每日金币", "每日额外获得400金币");
        vip4.AddPrivilege("专属皮肤", "获得VIP4专属皮肤");
        vip4.AddPrivilege("专属称号", "获得VIP4专属称号");
        vip4.AddPrivilege("优先匹配", "匹配时享有优先队列");
        vipLevels.Add(vip4);
        
        VIP vip5 = new VIP("vip_5", 5, 2000);
        vip5.AddPrivilege("专属头像框", "获得VIP5专属头像框");
        vip5.AddPrivilege("每日金币", "每日额外获得500金币");
        vip5.AddPrivilege("专属皮肤", "获得VIP5专属皮肤");
        vip5.AddPrivilege("专属称号", "获得VIP5专属称号");
        vip5.AddPrivilege("优先匹配", "匹配时享有优先队列");
        vip5.AddPrivilege("专属客服", "享受专属客服服务");
        vipLevels.Add(vip5);
    }
    
    public void AddPoints(int points)
    {
        currentPoints += points;
        UpdateVIPLevel();
    }
    
    private void UpdateVIPLevel()
    {
        for (int i = vipLevels.Count - 1; i >= 0; i--)
        {
            if (currentPoints >= vipLevels[i].requiredPoints)
            {
                currentLevel = vipLevels[i].vipLevel;
                break;
            }
        }
    }
    
    public VIP GetCurrentVIP()
    {
        return vipLevels.Find(v => v.vipLevel == currentLevel);
    }
}

[System.Serializable]
public class VIPManagerData
{
    public List<VIPUser> vipUsers;
    
    public VIPManagerData()
    {
        vipUsers = new List<VIPUser>();
    }
    
    public void AddVIPUser(VIPUser user)
    {
        vipUsers.Add(user);
    }
    
    public VIPUser GetVIPUser(string playerID)
    {
        return vipUsers.Find(u => u.playerID == playerID);
    }
}
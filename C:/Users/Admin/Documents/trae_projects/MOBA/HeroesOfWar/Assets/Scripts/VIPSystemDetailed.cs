[System.Serializable]
public class VIPSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<VIPLevel> vipLevels;
    public List<VIPPrivilege> vipPrivileges;
    public List<VIPMember> vipMembers;
    public List<VIPEvent> vipEvents;
    
    public VIPSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        vipLevels = new List<VIPLevel>();
        vipPrivileges = new List<VIPPrivilege>();
        vipMembers = new List<VIPMember>();
        vipEvents = new List<VIPEvent>();
    }
    
    public void AddVIPLevel(VIPLevel vipLevel)
    {
        vipLevels.Add(vipLevel);
    }
    
    public void AddVIPPrivilege(VIPPrivilege vipPrivilege)
    {
        vipPrivileges.Add(vipPrivilege);
    }
    
    public void AddVIPMember(VIPMember vipMember)
    {
        vipMembers.Add(vipMember);
    }
    
    public void AddVIPEvent(VIPEvent vipEvent)
    {
        vipEvents.Add(vipEvent);
    }
    
    public VIPLevel GetVIPLevel(int level)
    {
        return vipLevels.Find(vl => vl.level == level);
    }
    
    public VIPPrivilege GetVIPPrivilege(string privilegeID)
    {
        return vipPrivileges.Find(vp => vp.privilegeID == privilegeID);
    }
    
    public VIPMember GetVIPMember(string userID)
    {
        return vipMembers.Find(vm => vm.userID == userID);
    }
    
    public VIPEvent GetVIPEvent(string eventID)
    {
        return vipEvents.Find(ve => ve.eventID == eventID);
    }
    
    public List<VIPLevel> GetVIPLevels()
    {
        return vipLevels;
    }
    
    public List<VIPPrivilege> GetVIPPrivilegesByLevel(int level)
    {
        return vipPrivileges.FindAll(vp => vp.requiredLevel <= level);
    }
    
    public List<VIPMember> GetVIPMembersByLevel(int level)
    {
        return vipMembers.FindAll(vm => vm.vipLevel == level);
    }
    
    public List<VIPEvent> GetVIPEventsByUser(string userID)
    {
        return vipEvents.FindAll(ve => ve.userID == userID);
    }
}

[System.Serializable]
public class VIPLevel
{
    public int level;
    public string levelName;
    public string levelDescription;
    public int requiredRecharge;
    public int totalRecharge;
    public string icon;
    public List<string> privileges;
    
    public VIPLevel(int level, string levelName, string levelDescription, int requiredRecharge, int totalRecharge, string icon)
    {
        this.level = level;
        this.levelName = levelName;
        this.levelDescription = levelDescription;
        this.requiredRecharge = requiredRecharge;
        this.totalRecharge = totalRecharge;
        this.icon = icon;
        privileges = new List<string>();
    }
    
    public void AddPrivilege(string privilegeID)
    {
        privileges.Add(privilegeID);
    }
    
    public bool IsReached(int recharge)
    {
        return recharge >= requiredRecharge;
    }
}

[System.Serializable]
public class VIPPrivilege
{
    public string privilegeID;
    public string privilegeName;
    public string privilegeDescription;
    public int requiredLevel;
    public string privilegeType;
    public string privilegeValue;
    public string icon;
    public bool isEnabled;
    
    public VIPPrivilege(string id, string privilegeName, string privilegeDescription, int requiredLevel, string privilegeType, string privilegeValue, string icon)
    {
        privilegeID = id;
        this.privilegeName = privilegeName;
        this.privilegeDescription = privilegeDescription;
        this.requiredLevel = requiredLevel;
        this.privilegeType = privilegeType;
        this.privilegeValue = privilegeValue;
        this.icon = icon;
        isEnabled = true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public bool IsAvailable(int level)
    {
        if (!isEnabled)
            return false;
        
        if (level < requiredLevel)
            return false;
        
        return true;
    }
}

[System.Serializable]
public class VIPMember
{
    public string userID;
    public string userName;
    public int vipLevel;
    public int totalRecharge;
    public int currentRecharge;
    public string joinDate;
    public string lastRechargeDate;
    public List<string> claimedPrivileges;
    
    public VIPMember(string userID, string userName)
    {
        this.userID = userID;
        this.userName = userName;
        vipLevel = 0;
        totalRecharge = 0;
        currentRecharge = 0;
        joinDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastRechargeDate = "";
        claimedPrivileges = new List<string>();
    }
    
    public void AddRecharge(int amount)
    {
        totalRecharge += amount;
        currentRecharge += amount;
        lastRechargeDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        UpdateVIPLevel();
    }
    
    public void UpdateVIPLevel()
    {
        // 简单的等级计算，根据总充值金额
        if (totalRecharge >= 10000)
            vipLevel = 10;
        else if (totalRecharge >= 5000)
            vipLevel = 9;
        else if (totalRecharge >= 2000)
            vipLevel = 8;
        else if (totalRecharge >= 1000)
            vipLevel = 7;
        else if (totalRecharge >= 500)
            vipLevel = 6;
        else if (totalRecharge >= 200)
            vipLevel = 5;
        else if (totalRecharge >= 100)
            vipLevel = 4;
        else if (totalRecharge >= 50)
            vipLevel = 3;
        else if (totalRecharge >= 20)
            vipLevel = 2;
        else if (totalRecharge >= 10)
            vipLevel = 1;
        else
            vipLevel = 0;
    }
    
    public void ClaimPrivilege(string privilegeID)
    {
        if (!claimedPrivileges.Contains(privilegeID))
        {
            claimedPrivileges.Add(privilegeID);
        }
    }
    
    public bool HasClaimedPrivilege(string privilegeID)
    {
        return claimedPrivileges.Contains(privilegeID);
    }
}

[System.Serializable]
public class VIPEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string description;
    public int amount;
    public string timestamp;
    public string status;
    
    public VIPEvent(string id, string eventType, string userID, string description, int amount)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.description = description;
        this.amount = amount;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
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
public class VIPSystemDetailedManagerData
{
    public VIPSystemDetailed system;
    
    public VIPSystemDetailedManagerData()
    {
        system = new VIPSystemDetailed("vip_system_detailed", "贵族系统详细", "管理贵族系统的详细功能，包括贵族等级和特权");
    }
}
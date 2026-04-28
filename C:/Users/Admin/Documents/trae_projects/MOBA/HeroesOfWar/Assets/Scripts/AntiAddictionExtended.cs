[System.Serializable]
public class AntiAddictionExtended
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AgeGroup> ageGroups;
    public List<ParentControl> parentControls;
    public List<PlayerSession> playerSessions;
    public List<RealNameAuth> realNameAuths;
    
    public AntiAddictionExtended(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        ageGroups = new List<AgeGroup>();
        parentControls = new List<ParentControl>();
        playerSessions = new List<PlayerSession>();
        realNameAuths = new List<RealNameAuth>();
    }
    
    public void AddAgeGroup(AgeGroup group)
    {
        ageGroups.Add(group);
    }
    
    public void AddParentControl(ParentControl control)
    {
        parentControls.Add(control);
    }
    
    public void AddPlayerSession(PlayerSession session)
    {
        playerSessions.Add(session);
    }
    
    public void AddRealNameAuth(RealNameAuth auth)
    {
        realNameAuths.Add(auth);
    }
    
    public AgeGroup GetAgeGroup(int age)
    {
        return ageGroups.Find(g => g.minAge <= age && age <= g.maxAge);
    }
    
    public ParentControl GetParentControl(string playerID)
    {
        return parentControls.Find(c => c.playerID == playerID);
    }
    
    public RealNameAuth GetRealNameAuth(string playerID)
    {
        return realNameAuths.Find(a => a.playerID == playerID);
    }
    
    public List<PlayerSession> GetPlayerSessions(string playerID, string date)
    {
        return playerSessions.FindAll(s => s.playerID == playerID && s.loginTime.StartsWith(date));
    }
}

[System.Serializable]
public class AgeGroup
{
    public string groupID;
    public string groupName;
    public int minAge;
    public int maxAge;
    public int dailyPlayTimeLimit;
    public int weeklyPlayTimeLimit;
    public bool canPlayAfter9pm;
    public bool canRecharge;
    public int monthlyRechargeLimit;
    
    public AgeGroup(string id, string name, int min, int max, int dailyLimit, int weeklyLimit, bool canPlayLate, bool canRecharge, int rechargeLimit)
    {
        groupID = id;
        groupName = name;
        minAge = min;
        maxAge = max;
        dailyPlayTimeLimit = dailyLimit;
        weeklyPlayTimeLimit = weeklyLimit;
        canPlayAfter9pm = canPlayLate;
        this.canRecharge = canRecharge;
        monthlyRechargeLimit = rechargeLimit;
    }
}

[System.Serializable]
public class ParentControl
{
    public string controlID;
    public string playerID;
    public string parentEmail;
    public string parentPhone;
    public bool enableTimeLimit;
    public int customDailyLimit;
    public int customWeeklyLimit;
    public bool enableRechargeLimit;
    public int customRechargeLimit;
    public bool enablePlayTimeSchedule;
    public List<PlayTimeSlot> playTimeSlots;
    public bool enableGameContentRestriction;
    public string contentRestrictionLevel;
    
    public ParentControl(string id, string playerID, string email, string phone)
    {
        controlID = id;
        this.playerID = playerID;
        parentEmail = email;
        parentPhone = phone;
        enableTimeLimit = false;
        customDailyLimit = 120;
        customWeeklyLimit = 720;
        enableRechargeLimit = false;
        customRechargeLimit = 100;
        enablePlayTimeSchedule = false;
        playTimeSlots = new List<PlayTimeSlot>();
        enableGameContentRestriction = false;
        contentRestrictionLevel = "All";
    }
    
    public void AddPlayTimeSlot(PlayTimeSlot slot)
    {
        playTimeSlots.Add(slot);
    }
    
    public void UpdateControl(bool timeLimit, int dailyLimit, int weeklyLimit, bool rechargeLimit, int rechargeLimitAmount, bool schedule, bool contentRestriction, string contentLevel)
    {
        enableTimeLimit = timeLimit;
        customDailyLimit = dailyLimit;
        customWeeklyLimit = weeklyLimit;
        enableRechargeLimit = rechargeLimit;
        customRechargeLimit = rechargeLimitAmount;
        enablePlayTimeSchedule = schedule;
        enableGameContentRestriction = contentRestriction;
        contentRestrictionLevel = contentLevel;
    }
}

[System.Serializable]
public class PlayTimeSlot
{
    public string slotID;
    public string dayOfWeek;
    public string startTime;
    public string endTime;
    
    public PlayTimeSlot(string id, string day, string start, string end)
    {
        slotID = id;
        dayOfWeek = day;
        startTime = start;
        endTime = end;
    }
}

[System.Serializable]
public class PlayerSession
{
    public string sessionID;
    public string playerID;
    public string loginTime;
    public string logoutTime;
    public int playTime;
    public string sessionStatus;
    
    public PlayerSession(string id, string playerID, string login)
    {
        sessionID = id;
        this.playerID = playerID;
        loginTime = login;
        logoutTime = "";
        playTime = 0;
        sessionStatus = "Active";
    }
    
    public void EndSession(string logout)
    {
        logoutTime = logout;
        sessionStatus = "Ended";
        // 计算游戏时间（分钟）
        System.DateTime loginDate = System.DateTime.Parse(loginTime);
        System.DateTime logoutDate = System.DateTime.Parse(logoutTime);
        playTime = (int)(logoutDate - loginDate).TotalMinutes;
    }
}

[System.Serializable]
public class RealNameAuth
{
    public string authID;
    public string playerID;
    public string realName;
    public string idNumber;
    public int age;
    public bool isVerified;
    public string verificationTime;
    public string verificationStatus;
    
    public RealNameAuth(string id, string playerID, string name, string idNum, int age)
    {
        authID = id;
        this.playerID = playerID;
        realName = name;
        idNumber = idNum;
        this.age = age;
        isVerified = false;
        verificationTime = "";
        verificationStatus = "Pending";
    }
    
    public void Verify()
    {
        isVerified = true;
        verificationTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        verificationStatus = "Verified";
    }
    
    public void Reject()
    {
        isVerified = false;
        verificationTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        verificationStatus = "Rejected";
    }
}

[System.Serializable]
public class AntiAddictionManagerData
{
    public AntiAddictionExtended system;
    
    public AntiAddictionManagerData()
    {
        system = new AntiAddictionExtended("anti_addiction_extended", "防沉迷系统扩展", "提供更严格的未成年人游戏时间限制和家长控制功能");
    }
}
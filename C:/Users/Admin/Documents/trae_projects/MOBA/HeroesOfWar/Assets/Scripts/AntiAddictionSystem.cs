[System.Serializable]
public class AntiAddictionSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<AgeGroup> ageGroups;
    public List<TimeLimit> timeLimits;
    public List<PurchaseLimit> purchaseLimits;
    
    public AntiAddictionSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        ageGroups = new List<AgeGroup>();
        timeLimits = new List<TimeLimit>();
        purchaseLimits = new List<PurchaseLimit>();
    }
    
    public void AddAgeGroup(AgeGroup ageGroup)
    {
        ageGroups.Add(ageGroup);
    }
    
    public void AddTimeLimit(TimeLimit timeLimit)
    {
        timeLimits.Add(timeLimit);
    }
    
    public void AddPurchaseLimit(PurchaseLimit purchaseLimit)
    {
        purchaseLimits.Add(purchaseLimit);
    }
    
    public AgeGroup GetAgeGroup(int age)
    {
        return ageGroups.Find(ag => age >= ag.minAge && age <= ag.maxAge);
    }
    
    public TimeLimit GetTimeLimit(string ageGroupID)
    {
        return timeLimits.Find(tl => tl.ageGroupID == ageGroupID);
    }
    
    public PurchaseLimit GetPurchaseLimit(string ageGroupID)
    {
        return purchaseLimits.Find(pl => pl.ageGroupID == ageGroupID);
    }
}

[System.Serializable]
public class AgeGroup
{
    public string ageGroupID;
    public string ageGroupName;
    public int minAge;
    public int maxAge;
    public string description;
    
    public AgeGroup(string id, string name, int min, int max, string desc)
    {
        ageGroupID = id;
        ageGroupName = name;
        minAge = min;
        maxAge = max;
        description = desc;
    }
}

[System.Serializable]
public class TimeLimit
{
    public string timeLimitID;
    public string ageGroupID;
    public int dailyPlayTime;
    public int weeklyPlayTime;
    public bool isAllowedAfter22;
    public string description;
    
    public TimeLimit(string id, string ageGroup, int daily, int weekly, bool allowedAfter22, string desc)
    {
        timeLimitID = id;
        ageGroupID = ageGroup;
        dailyPlayTime = daily;
        weeklyPlayTime = weekly;
        isAllowedAfter22 = allowedAfter22;
        description = desc;
    }
}

[System.Serializable]
public class PurchaseLimit
{
    public string purchaseLimitID;
    public string ageGroupID;
    public int singlePurchaseLimit;
    public int monthlyPurchaseLimit;
    public string description;
    
    public PurchaseLimit(string id, string ageGroup, int single, int monthly, string desc)
    {
        purchaseLimitID = id;
        ageGroupID = ageGroup;
        singlePurchaseLimit = single;
        monthlyPurchaseLimit = monthly;
        description = desc;
    }
}

[System.Serializable]
public class PlayerAntiAddictionData
{
    public string playerID;
    public int age;
    public string ageGroupID;
    public int dailyPlayTime;
    public int weeklyPlayTime;
    public System.DateTime lastLoginTime;
    public System.DateTime lastLogoutTime;
    public int monthlyPurchases;
    public bool isVerified;
    
    public PlayerAntiAddictionData(string playerID, int age)
    {
        this.playerID = playerID;
        this.age = age;
        ageGroupID = GetAgeGroupID(age);
        dailyPlayTime = 0;
        weeklyPlayTime = 0;
        lastLoginTime = System.DateTime.Now;
        lastLogoutTime = System.DateTime.Now;
        monthlyPurchases = 0;
        isVerified = false;
    }
    
    private string GetAgeGroupID(int age)
    {
        if (age < 8)
        {
            return "age_group_under_8";
        }
        else if (age < 16)
        {
            return "age_group_8_15";
        }
        else if (age < 18)
        {
            return "age_group_16_17";
        }
        else
        {
            return "age_group_adult";
        }
    }
    
    public void UpdatePlayTime()
    {
        if (lastLogoutTime < lastLoginTime)
        {
            System.TimeSpan playTime = System.DateTime.Now - lastLoginTime;
            dailyPlayTime += (int)playTime.TotalMinutes;
            weeklyPlayTime += (int)playTime.TotalMinutes;
            lastLogoutTime = System.DateTime.Now;
        }
    }
    
    public void ResetDailyPlayTime()
    {
        dailyPlayTime = 0;
    }
    
    public void ResetWeeklyPlayTime()
    {
        weeklyPlayTime = 0;
    }
    
    public void ResetMonthlyPurchases()
    {
        monthlyPurchases = 0;
    }
    
    public void AddPurchase(int amount)
    {
        monthlyPurchases += amount;
    }
    
    public void SetVerified(bool verified)
    {
        isVerified = verified;
    }
}

[System.Serializable]
public class AntiAddictionManagerData
{
    public AntiAddictionSystem system;
    public List<PlayerAntiAddictionData> playerData;
    
    public AntiAddictionManagerData()
    {
        system = new AntiAddictionSystem("anti_addiction", "防沉迷系统", "防止未成年人过度游戏");
        playerData = new List<PlayerAntiAddictionData>();
    }
    
    public void AddPlayerData(PlayerAntiAddictionData data)
    {
        playerData.Add(data);
    }
    
    public PlayerAntiAddictionData GetPlayerData(string playerID)
    {
        return playerData.Find(pd => pd.playerID == playerID);
    }
    
    public void UpdatePlayerData(string playerID, PlayerAntiAddictionData data)
    {
        PlayerAntiAddictionData existingData = GetPlayerData(playerID);
        if (existingData != null)
        {
            existingData = data;
        }
        else
        {
            AddPlayerData(data);
        }
    }
}
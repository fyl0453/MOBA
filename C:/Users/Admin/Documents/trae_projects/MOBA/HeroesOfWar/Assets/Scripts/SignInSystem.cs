[System.Serializable]
public class SignInDay
{
    public int dayNumber;
    public string rewardType;
    public string rewardItemID;
    public int rewardQuantity;
    public bool isPremium;
    public bool isSigned;
    
    public SignInDay(int day, string type, string itemID, int qty, bool premium = false)
    {
        dayNumber = day;
        rewardType = type;
        rewardItemID = itemID;
        rewardQuantity = qty;
        isPremium = premium;
        isSigned = false;
    }
    
    public void Sign()
    {
        isSigned = true;
    }
}

[System.Serializable]
public class SignInWeek
{
    public string weekID;
    public int weekNumber;
    public List<SignInDay> days;
    public bool isPremium;
    
    public SignInWeek(string id, int week)
    {
        weekID = id;
        weekNumber = week;
        days = new List<SignInDay>();
        isPremium = false;
    }
    
    public void InitializeDefaultRewards()
    {
        days.Add(new SignInDay(1, "Currency", "gold", 100));
        days.Add(new SignInDay(2, "Currency", "gold", 150));
        days.Add(new SignInDay(3, "Item", "item_health_potion", 2));
        days.Add(new SignInDay(4, "Currency", "gold", 200));
        days.Add(new SignInDay(5, "Item", "item_mana_potion", 2));
        days.Add(new SignInDay(6, "Currency", "gold", 250));
        days.Add(new SignInDay(7, "Skin", "skin_guanyu_spring", 1, true));
    }
}

[System.Serializable]
public class SignInData
{
    public string playerID;
    public int currentStreak;
    public int totalSignDays;
    public string lastSignDate;
    public List<SignInWeek> weeks;
    public int totalSignInCount;
    
    public SignInData(string player)
    {
        playerID = player;
        currentStreak = 0;
        totalSignDays = 0;
        lastSignDate = "";
        weeks = new List<SignInWeek>();
        totalSignInCount = 0;
    }
    
    public void AddWeek(SignInWeek week)
    {
        weeks.Add(week);
    }
    
    public SignInWeek GetCurrentWeek()
    {
        if (weeks.Count > 0)
        {
            return weeks[weeks.Count - 1];
        }
        return null;
    }
    
    public void ResetStreak()
    {
        currentStreak = 0;
    }
}

[System.Serializable]
public class SignInManagerData
{
    public List<SignInData> signInDataList;
    
    public SignInManagerData()
    {
        signInDataList = new List<SignInData>();
    }
    
    public void AddSignInData(SignInData data)
    {
        signInDataList.Add(data);
    }
    
    public SignInData GetSignInData(string playerID)
    {
        return signInDataList.Find(d => d.playerID == playerID);
    }
}
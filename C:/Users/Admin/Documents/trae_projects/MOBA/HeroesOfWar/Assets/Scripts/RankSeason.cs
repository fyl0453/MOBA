[System.Serializable]
public class RankSeason
{
    public string seasonID;
    public string seasonName;
    public string startDate;
    public string endDate;
    public bool isActive;
    public bool isEnded;
    public int seasonNumber;
    
    public RankSeason(string id, string name, int number)
    {
        seasonID = id;
        seasonName = name;
        seasonNumber = number;
        startDate = "";
        endDate = "";
        isActive = false;
        isEnded = false;
    }
}

[System.Serializable]
public class RankDivision
{
    public string divisionID;
    public string divisionName;
    public int tier;
    public int minStars;
    public int maxStars;
    public string icon;
    
    public RankDivision(string id, string name, int tierLevel, int min, int max)
    {
        divisionID = id;
        divisionName = name;
        tier = tierLevel;
        minStars = min;
        maxStars = max;
        icon = "";
    }
}

[System.Serializable]
public class PlayerRank
{
    public string playerID;
    public string currentSeasonID;
    public string currentDivisionID;
    public int stars;
    public int totalStars;
    public int wins;
    public int losses;
    public int consecutiveWins;
    public int consecutiveLosses;
    public int peakStars;
    public string peakDivisionID;
    public float winRate;
    public bool protectedStatus;
    
    public PlayerRank(string pid)
    {
        playerID = pid;
        currentSeasonID = "";
        currentDivisionID = "bronze_4";
        stars = 0;
        totalStars = 0;
        wins = 0;
        losses = 0;
        consecutiveWins = 0;
        consecutiveLosses = 0;
        peakStars = 0;
        peakDivisionID = "bronze_4";
        winRate = 0;
        protectedStatus = false;
    }
    
    public void AddWin()
    {
        wins++;
        consecutiveWins++;
        consecutiveLosses = 0;
        UpdateWinRate();
        ProtectCheck();
    }
    
    public void AddLoss()
    {
        losses++;
        consecutiveLosses++;
        consecutiveWins = 0;
        UpdateWinRate();
    }
    
    private void UpdateWinRate()
    {
        int total = wins + losses;
        if (total > 0)
        {
            winRate = (float)wins / total * 100;
        }
    }
    
    private void ProtectCheck()
    {
        if (consecutiveWins >= 3)
        {
            protectedStatus = true;
        }
    }
}

[System.Serializable]
public class SeasonReward
{
    public string rewardID;
    public string seasonID;
    public string divisionID;
    public string rewardType;
    public string rewardItemID;
    public int quantity;
    public bool isClaimed;
    
    public SeasonReward(string id, string season, string div, string type, string item, int qty)
    {
        rewardID = id;
        seasonID = season;
        divisionID = div;
        rewardType = type;
        rewardItemID = item;
        quantity = qty;
        isClaimed = false;
    }
}
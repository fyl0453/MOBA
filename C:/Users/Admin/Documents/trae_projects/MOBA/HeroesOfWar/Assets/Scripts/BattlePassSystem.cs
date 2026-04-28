[System.Serializable]
public class BattlePass
{
    public string passID;
    public string passName;
    public string seasonID;
    public string startTime;
    public string endTime;
    public int maxLevel;
    public bool isActive;
    public List<PassReward> rewards;
    public int price;
    public string currencyType;
    
    public BattlePass(string id, string name, string season, int maxLvl, int cost)
    {
        passID = id;
        passName = name;
        seasonID = season;
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd");
        endTime = System.DateTime.Now.AddDays(60).ToString("yyyy-MM-dd");
        maxLevel = maxLvl;
        isActive = true;
        rewards = new List<PassReward>();
        price = cost;
        currencyType = "Gems";
    }
    
    public void AddReward(int level, string rewardType, string rewardItemID, int quantity, bool isPremium = false)
    {
        PassReward reward = new PassReward(level, rewardType, rewardItemID, quantity, isPremium);
        rewards.Add(reward);
    }
}

[System.Serializable]
public class PassReward
{
    public int level;
    public string rewardType;
    public string rewardItemID;
    public int quantity;
    public bool isPremium;
    public bool isClaimed;
    
    public PassReward(int lvl, string type, string itemID, int qty, bool premium = false)
    {
        level = lvl;
        rewardType = type;
        rewardItemID = itemID;
        quantity = qty;
        isPremium = premium;
        isClaimed = false;
    }
    
    public void Claim()
    {
        isClaimed = true;
    }
}

[System.Serializable]
public class PassProgress
{
    public string playerID;
    public string passID;
    public int currentLevel;
    public int currentXP;
    public bool hasPurchasedPremium;
    public List<PassReward> claimedRewards;
    
    public PassProgress(string player, string pass)
    {
        playerID = player;
        passID = pass;
        currentLevel = 1;
        currentXP = 0;
        hasPurchasedPremium = false;
        claimedRewards = new List<PassReward>();
    }
    
    public void AddXP(int xp)
    {
        currentXP += xp;
        while (currentXP >= GetXPForLevel(currentLevel + 1))
        {
            currentXP -= GetXPForLevel(currentLevel + 1);
            currentLevel++;
        }
    }
    
    private int GetXPForLevel(int level)
    {
        return 1000 + (level - 1) * 200;
    }
    
    public void ClaimReward(PassReward reward)
    {
        if (!reward.isClaimed)
        {
            reward.Claim();
            claimedRewards.Add(reward);
        }
    }
    
    public void PurchasePremium()
    {
        hasPurchasedPremium = true;
    }
}

[System.Serializable]
public class BattlePassManagerData
{
    public List<BattlePass> battlePasses;
    public List<PassProgress> passProgresses;
    
    public BattlePassManagerData()
    {
        battlePasses = new List<BattlePass>();
        passProgresses = new List<PassProgress>();
    }
    
    public void AddBattlePass(BattlePass pass)
    {
        battlePasses.Add(pass);
    }
    
    public void AddPassProgress(PassProgress progress)
    {
        passProgresses.Add(progress);
    }
    
    public BattlePass GetCurrentPass()
    {
        return battlePasses.Find(p => p.isActive);
    }
    
    public PassProgress GetPassProgress(string playerID, string passID)
    {
        return passProgresses.Find(p => p.playerID == playerID && p.passID == passID);
    }
}
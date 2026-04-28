[System.Serializable]
public class CoupleSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<CoupleRelation> couples;
    public List<CoupleReward> rewards;
    
    public CoupleSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        couples = new List<CoupleRelation>();
        rewards = new List<CoupleReward>();
    }
    
    public void AddCouple(CoupleRelation couple)
    {
        couples.Add(couple);
    }
    
    public void AddReward(CoupleReward reward)
    {
        rewards.Add(reward);
    }
    
    public CoupleRelation GetCouple(string coupleID)
    {
        return couples.Find(c => c.coupleID == coupleID);
    }
    
    public CoupleRelation GetCoupleByPlayer(string playerID)
    {
        return couples.Find(c => c.player1ID == playerID || c.player2ID == playerID);
    }
    
    public CoupleReward GetReward(string rewardID)
    {
        return rewards.Find(r => r.rewardID == rewardID);
    }
    
    public List<CoupleReward> GetRewardsByLevel(int level)
    {
        return rewards.FindAll(r => r.requiredLevel <= level);
    }
}

[System.Serializable]
public class CoupleRelation
{
    public string coupleID;
    public string player1ID;
    public string player1Name;
    public string player2ID;
    public string player2Name;
    public int coupleLevel;
    public int intimacy;
    public string status;
    public string createdAt;
    public string lastInteraction;
    
    public CoupleRelation(string id, string player1, string player1Name, string player2, string player2Name)
    {
        coupleID = id;
        player1ID = player1;
        this.player1Name = player1Name;
        player2ID = player2;
        this.player2Name = player2Name;
        coupleLevel = 1;
        intimacy = 0;
        status = "Active";
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastInteraction = createdAt;
    }
    
    public void AddIntimacy(int amount)
    {
        intimacy += amount;
        UpdateLevel();
        lastInteraction = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateLevel()
    {
        if (intimacy >= 1000)
        {
            coupleLevel = 5;
        }
        else if (intimacy >= 500)
        {
            coupleLevel = 4;
        }
        else if (intimacy >= 300)
        {
            coupleLevel = 3;
        }
        else if (intimacy >= 100)
        {
            coupleLevel = 2;
        }
        else
        {
            coupleLevel = 1;
        }
    }
    
    public void SetStatus(string status)
    {
        this.status = status;
    }
    
    public string GetPartnerID(string playerID)
    {
        return playerID == player1ID ? player2ID : player1ID;
    }
    
    public string GetPartnerName(string playerID)
    {
        return playerID == player1ID ? player2Name : player1Name;
    }
}

[System.Serializable]
public CoupleReward
{
    public string rewardID;
    public string rewardName;
    public string rewardDescription;
    public int requiredLevel;
    public string rewardType;
    public string rewardContent;
    public bool isClaimed;
    
    public CoupleReward(string id, string name, string desc, int level, string type, string content)
    {
        rewardID = id;
        rewardName = name;
        rewardDescription = desc;
        requiredLevel = level;
        rewardType = type;
        rewardContent = content;
        isClaimed = false;
    }
    
    public void Claim()
    {
        isClaimed = true;
    }
}

[System.Serializable]
public class CoupleManagerData
{
    public CoupleSystem system;
    
    public CoupleManagerData()
    {
        system = new CoupleSystem("couple_system", "情侣系统", "管理玩家之间的情侣关系");
    }
}
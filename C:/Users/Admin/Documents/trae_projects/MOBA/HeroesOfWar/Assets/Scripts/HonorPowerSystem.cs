[System.Serializable]
public class HonorPowerSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<HeroHonorPower> heroHonorPowers;
    public List<HonorPowerRank> honorPowerRanks;
    
    public HonorPowerSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        heroHonorPowers = new List<HeroHonorPower>();
        honorPowerRanks = new List<HonorPowerRank>();
    }
    
    public void AddHeroHonorPower(HeroHonorPower heroHonorPower)
    {
        heroHonorPowers.Add(heroHonorPower);
    }
    
    public void AddHonorPowerRank(HonorPowerRank honorPowerRank)
    {
        honorPowerRanks.Add(honorPowerRank);
    }
    
    public HeroHonorPower GetHeroHonorPower(string playerID, string heroID)
    {
        return heroHonorPowers.Find(h => h.playerID == playerID && h.heroID == heroID);
    }
    
    public HonorPowerRank GetHonorPowerRank(string rankID)
    {
        return honorPowerRanks.Find(r => r.rankID == rankID);
    }
    
    public List<HeroHonorPower> GetHeroHonorPowersByPlayer(string playerID)
    {
        return heroHonorPowers.FindAll(h => h.playerID == playerID);
    }
    
    public List<HeroHonorPower> GetHeroHonorPowersByHero(string heroID)
    {
        return heroHonorPowers.FindAll(h => h.heroID == heroID);
    }
    
    public List<HeroHonorPower> GetTopHeroHonorPowers(string heroID, int limit = 100)
    {
        List<HeroHonorPower> sorted = heroHonorPowers.FindAll(h => h.heroID == heroID);
        sorted.Sort((a, b) => b.honorPower.CompareTo(a.honorPower));
        return sorted.GetRange(0, Mathf.Min(limit, sorted.Count));
    }
}

[System.Serializable]
public class HeroHonorPower
{
    public string playerID;
    public string playerName;
    public string heroID;
    public string heroName;
    public int honorPower;
    public int rank;
    public int wins;
    public int matches;
    public string currentSeasonID;
    public List<HonorPowerHistory> honorPowerHistory;
    
    public HeroHonorPower(string playerID, string playerName, string heroID, string heroName, int honorPower, string seasonID)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        this.heroID = heroID;
        this.heroName = heroName;
        this.honorPower = honorPower;
        rank = 0;
        wins = 0;
        matches = 0;
        currentSeasonID = seasonID;
        honorPowerHistory = new List<HonorPowerHistory>();
    }
    
    public void UpdateHonorPower(int power)
    {
        honorPower = power;
    }
    
    public void UpdateRank(int rank)
    {
        this.rank = rank;
    }
    
    public void AddMatch(bool isWin, int powerChange)
    {
        matches++;
        if (isWin)
        {
            wins++;
        }
        
        HonorPowerHistory history = new HonorPowerHistory(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), isWin, powerChange, honorPower);
        honorPowerHistory.Add(history);
    }
    
    public float GetWinRate()
    {
        return matches > 0 ? (float)wins / matches : 0;
    }
}

[System.Serializable]
public class HonorPowerHistory
{
    public string matchTime;
    public bool isWin;
    public int powerChange;
    public int totalPower;
    
    public HonorPowerHistory(string matchTime, bool isWin, int powerChange, int totalPower)
    {
        this.matchTime = matchTime;
        this.isWin = isWin;
        this.powerChange = powerChange;
        this.totalPower = totalPower;
    }
}

[System.Serializable]
public class HonorPowerRank
{
    public string rankID;
    public string rankName;
    public string rankDescription;
    public int minPower;
    public int maxPower;
    public string rankIcon;
    
    public HonorPowerRank(string rankID, string rankName, string rankDescription, int minPower, int maxPower)
    {
        this.rankID = rankID;
        this.rankName = rankName;
        this.rankDescription = rankDescription;
        this.minPower = minPower;
        this.maxPower = maxPower;
        rankIcon = "";
    }
}

[System.Serializable]
public class HonorPowerManagerData
{
    public HonorPowerSystem system;
    
    public HonorPowerManagerData()
    {
        system = new HonorPowerSystem("honor_power_system", "荣耀战力系统", "管理英雄荣耀战力");
    }
}
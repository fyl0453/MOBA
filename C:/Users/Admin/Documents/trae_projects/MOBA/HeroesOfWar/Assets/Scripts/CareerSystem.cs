[System.Serializable]
public class PlayerCareer
{
    public string playerID;
    public string playerName;
    public int totalMatches;
    public int totalWins;
    public int totalLosses;
    public float winRate;
    public int totalKills;
    public int totalDeaths;
    public int totalAssists;
    public float kda;
    public int totalGold;
    public int totalDamageDealt;
    public int totalDamageTaken;
    public int totalHealing;
    public int totalVisionScore;
    public int totalMVP;
    public int highestRank;
    public string highestRankSeason;
    public List<CareerStat> seasonStats;
    
    public PlayerCareer(string player, string name)
    {
        playerID = player;
        playerName = name;
        totalMatches = 0;
        totalWins = 0;
        totalLosses = 0;
        winRate = 0f;
        totalKills = 0;
        totalDeaths = 0;
        totalAssists = 0;
        kda = 0f;
        totalGold = 0;
        totalDamageDealt = 0;
        totalDamageTaken = 0;
        totalHealing = 0;
        totalVisionScore = 0;
        totalMVP = 0;
        highestRank = 0;
        highestRankSeason = "";
        seasonStats = new List<CareerStat>();
    }
    
    public void UpdateMatchStats(int kills, int deaths, int assists, int gold, int damageDealt, int damageTaken, int healing, int visionScore, bool isWin, bool isMVP)
    {
        totalMatches++;
        if (isWin)
        {
            totalWins++;
        }
        else
        {
            totalLosses++;
        }
        
        totalKills += kills;
        totalDeaths += deaths;
        totalAssists += assists;
        totalGold += gold;
        totalDamageDealt += damageDealt;
        totalDamageTaken += damageTaken;
        totalHealing += healing;
        totalVisionScore += visionScore;
        
        if (isMVP)
        {
            totalMVP++;
        }
        
        UpdateDerivedStats();
    }
    
    private void UpdateDerivedStats()
    {
        winRate = totalMatches > 0 ? (float)totalWins / totalMatches * 100f : 0f;
        kda = totalDeaths > 0 ? (float)(totalKills + totalAssists) / totalDeaths : (float)(totalKills + totalAssists);
    }
    
    public void UpdateHighestRank(int rank, string season)
    {
        if (rank > highestRank)
        {
            highestRank = rank;
            highestRankSeason = season;
        }
    }
    
    public void AddSeasonStat(CareerStat stat)
    {
        seasonStats.Add(stat);
    }
}

[System.Serializable]
public class CareerStat
{
    public string seasonID;
    public int matches;
    public int wins;
    public int losses;
    public float winRate;
    public int kills;
    public int deaths;
    public int assists;
    public float kda;
    public int mvpCount;
    public int finalRank;
    
    public CareerStat(string season)
    {
        seasonID = season;
        matches = 0;
        wins = 0;
        losses = 0;
        winRate = 0f;
        kills = 0;
        deaths = 0;
        assists = 0;
        kda = 0f;
        mvpCount = 0;
        finalRank = 0;
    }
    
    public void UpdateStats(int kills, int deaths, int assists, bool isWin, bool isMVP)
    {
        matches++;
        if (isWin)
        {
            wins++;
        }
        else
        {
            losses++;
        }
        
        this.kills += kills;
        this.deaths += deaths;
        this.assists += assists;
        
        if (isMVP)
        {
            mvpCount++;
        }
        
        UpdateDerivedStats();
    }
    
    private void UpdateDerivedStats()
    {
        winRate = matches > 0 ? (float)wins / matches * 100f : 0f;
        kda = deaths > 0 ? (float)(kills + assists) / deaths : (float)(kills + assists);
    }
    
    public void SetFinalRank(int rank)
    {
        finalRank = rank;
    }
}

[System.Serializable]
public class CareerManagerData
{
    public List<PlayerCareer> playerCareers;
    
    public CareerManagerData()
    {
        playerCareers = new List<PlayerCareer>();
    }
    
    public void AddPlayerCareer(PlayerCareer career)
    {
        playerCareers.Add(career);
    }
    
    public PlayerCareer GetPlayerCareer(string playerID)
    {
        return playerCareers.Find(c => c.playerID == playerID);
    }
}
[System.Serializable]
public class GameStatisticsExtended
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<HeroStatistics> heroStatistics;
    public List<PlayerStatistics> playerStatistics;
    public List<GameModeStatistics> modeStatistics;
    
    public GameStatisticsExtended(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        heroStatistics = new List<HeroStatistics>();
        playerStatistics = new List<PlayerStatistics>();
        modeStatistics = new List<GameModeStatistics>();
    }
    
    public void AddHeroStatistics(HeroStatistics stats)
    {
        heroStatistics.Add(stats);
    }
    
    public void AddPlayerStatistics(PlayerStatistics stats)
    {
        playerStatistics.Add(stats);
    }
    
    public void AddModeStatistics(GameModeStatistics stats)
    {
        modeStatistics.Add(stats);
    }
    
    public HeroStatistics GetHeroStatistics(string playerID, string heroID)
    {
        return heroStatistics.Find(h => h.playerID == playerID && h.heroID == heroID);
    }
    
    public PlayerStatistics GetPlayerStatistics(string playerID)
    {
        return playerStatistics.Find(p => p.playerID == playerID);
    }
    
    public GameModeStatistics GetModeStatistics(string playerID, string modeID)
    {
        return modeStatistics.Find(m => m.playerID == playerID && m.modeID == modeID);
    }
}

[System.Serializable]
public class HeroStatistics
{
    public string playerID;
    public string heroID;
    public string heroName;
    public int totalMatches;
    public int wins;
    public int losses;
    public int kills;
    public int deaths;
    public int assists;
    public int damageDealt;
    public int damageTaken;
    public int goldEarned;
    public int mvpCount;
    public int consecutiveWins;
    public int consecutiveLosses;
    public string lastPlayedTime;
    
    public HeroStatistics(string playerID, string heroID, string heroName)
    {
        this.playerID = playerID;
        this.heroID = heroID;
        this.heroName = heroName;
        totalMatches = 0;
        wins = 0;
        losses = 0;
        kills = 0;
        deaths = 0;
        assists = 0;
        damageDealt = 0;
        damageTaken = 0;
        goldEarned = 0;
        mvpCount = 0;
        consecutiveWins = 0;
        consecutiveLosses = 0;
        lastPlayedTime = "";
    }
    
    public float GetWinRate()
    {
        return totalMatches > 0 ? (float)wins / totalMatches * 100 : 0;
    }
    
    public float GetKDA()
    {
        return deaths > 0 ? (float)(kills + assists) / deaths : kills + assists;
    }
    
    public float GetAverageKills()
    {
        return totalMatches > 0 ? (float)kills / totalMatches : 0;
    }
    
    public float GetAverageDeaths()
    {
        return totalMatches > 0 ? (float)deaths / totalMatches : 0;
    }
    
    public float GetAverageAssists()
    {
        return totalMatches > 0 ? (float)assists / totalMatches : 0;
    }
    
    public float GetAverageDamage()
    {
        return totalMatches > 0 ? (float)damageDealt / totalMatches : 0;
    }
    
    public void RecordMatch(bool isWin, int kills, int deaths, int assists, int damageDealt, int damageTaken, int goldEarned, bool isMVP)
    {
        totalMatches++;
        if (isWin)
        {
            wins++;
            consecutiveWins++;
            consecutiveLosses = 0;
        }
        else
        {
            losses++;
            consecutiveLosses++;
            consecutiveWins = 0;
        }
        
        this.kills += kills;
        this.deaths += deaths;
        this.assists += assists;
        this.damageDealt += damageDealt;
        this.damageTaken += damageTaken;
        this.goldEarned += goldEarned;
        
        if (isMVP)
        {
            mvpCount++;
        }
        
        lastPlayedTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class PlayerStatistics
{
    public string playerID;
    public string playerName;
    public int totalMatches;
    public int totalWins;
    public int totalLosses;
    public int totalKills;
    public int totalDeaths;
    public int totalAssists;
    public int totalMVP;
    public int totalGold;
    public int totalDamage;
    public int maxConsecutiveWins;
    public int maxConsecutiveLosses;
    public int highestRank;
    public string mostPlayedHeroID;
    public string mostPlayedHeroName;
    public string lastMatchTime;
    
    public PlayerStatistics(string playerID, string playerName)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        totalMatches = 0;
        totalWins = 0;
        totalLosses = 0;
        totalKills = 0;
        totalDeaths = 0;
        totalAssists = 0;
        totalMVP = 0;
        totalGold = 0;
        totalDamage = 0;
        maxConsecutiveWins = 0;
        maxConsecutiveLosses = 0;
        highestRank = 0;
        mostPlayedHeroID = "";
        mostPlayedHeroName = "";
        lastMatchTime = "";
    }
    
    public float GetOverallWinRate()
    {
        return totalMatches > 0 ? (float)totalWins / totalMatches * 100 : 0;
    }
    
    public void RecordMatch(bool isWin, int kills, int deaths, int assists, int gold, int damage, bool isMVP, string heroID, string heroName)
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
        totalDamage += damage;
        
        if (isMVP)
        {
            totalMVP++;
        }
        
        mostPlayedHeroID = heroID;
        mostPlayedHeroName = heroName;
        lastMatchTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class GameModeStatistics
{
    public string playerID;
    public string modeID;
    public string modeName;
    public int matches;
    public int wins;
    public int losses;
    public int mvpCount;
    
    public GameModeStatistics(string playerID, string modeID, string modeName)
    {
        this.playerID = playerID;
        this.modeID = modeID;
        this.modeName = modeName;
        matches = 0;
        wins = 0;
        losses = 0;
        mvpCount = 0;
    }
    
    public float GetWinRate()
    {
        return matches > 0 ? (float)wins / matches * 100 : 0;
    }
    
    public void RecordMatch(bool isWin, bool isMVP)
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
        
        if (isMVP)
        {
            mvpCount++;
        }
    }
}

[System.Serializable]
public class GameStatisticsManagerData
{
    public GameStatisticsExtended system;
    
    public GameStatisticsManagerData()
    {
        system = new GameStatisticsExtended("game_statistics_extended", "游戏数据统计扩展", "提供详细的个人数据统计");
    }
}
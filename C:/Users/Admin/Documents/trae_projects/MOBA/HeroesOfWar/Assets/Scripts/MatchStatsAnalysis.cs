[System.Serializable]
public class MatchStatsAnalysis
{
    public string matchID;
    public string heroID;
    public string heroName;
    public string gameMode;
    public string result;
    public string duration;
    
    public int kills;
    public int deaths;
    public int assists;
    public float kda;
    
    public float totalDamageDealt;
    public float totalDamageTaken;
    public float totalHealing;
    public float totalShielding;
    
    public float goldEarned;
    public float goldSpent;
    
    public int towersDestroyed;
    public int dragonsKilled;
    public int baronsKilled;
    public int heraldsKilled;
    
    public int cs;
    public float csPerMinute;
    
    public List<DamageDetail> damageDetails;
    public List<KillDetail> killDetails;
    public List<GoldTimeline> goldTimeline;
    
    public float performanceScore;
    public string performanceGrade;
    
    public MatchStatsAnalysis()
    {
        damageDetails = new List<DamageDetail>();
        killDetails = new List<KillDetail>();
        goldTimeline = new List<GoldTimeline>();
    }
    
    public void CalculateKDA()
    {
        if (deaths > 0)
        {
            kda = (float)(kills + assists) / deaths;
        }
        else
        {
            kda = kills + assists;
        }
    }
    
    public void CalculatePerformanceScore()
    {
        float score = 0;
        
        score += kills * 2f;
        score += assists * 1f;
        score += totalDamageDealt * 0.001f;
        score += totalHealing * 0.002f;
        score += towersDestroyed * 5f;
        score += dragonsKilled * 3f;
        score += baronsKilled * 5f;
        
        if (deaths > 0)
        {
            score -= deaths * 1f;
        }
        
        if (result == "Victory")
        {
            score += 20f;
        }
        
        performanceScore = Mathf.Clamp(score, 0, 100);
        CalculatePerformanceGrade();
    }
    
    private void CalculatePerformanceGrade()
    {
        if (performanceScore >= 90)
            performanceGrade = "S+";
        else if (performanceScore >= 80)
            performanceGrade = "S";
        else if (performanceScore >= 70)
            performanceGrade = "A";
        else if (performanceScore >= 60)
            performanceGrade = "B";
        else if (performanceScore >= 50)
            performanceGrade = "C";
        else
            performanceGrade = "D";
    }
}

[System.Serializable]
public class DamageDetail
{
    public string targetType;
    public string targetName;
    public float damage;
    public float percentage;
    
    public DamageDetail(string type, string name, float dmg)
    {
        targetType = type;
        targetName = name;
        damage = dmg;
    }
}

[System.Serializable]
public class KillDetail
{
    public float timestamp;
    public int killerID;
    public int victimID;
    public int assistCount;
    public List<int> assistingPlayerIDs;
    public string killType;
    
    public KillDetail(float time, int killer, int victim)
    {
        timestamp = time;
        killerID = killer;
        victimID = victim;
        assistCount = 0;
        assistingPlayerIDs = new List<int>();
        killType = "Normal";
    }
}

[System.Serializable]
public class GoldTimeline
{
    public float timestamp;
    public float goldAmount;
    public string eventType;
    
    public GoldTimeline(float time, float gold, string evt)
    {
        timestamp = time;
        goldAmount = gold;
        eventType = evt;
    }
}

[System.Serializable]
public class HeroPerformanceSummary
{
    public string heroID;
    public string heroName;
    public int totalGames;
    public int wins;
    public int losses;
    public float winRate;
    public float averageKDA;
    public float averageDamage;
    public float averageGold;
    public float bestScore;
    public string bestScoreMatchID;
    
    public HeroPerformanceSummary(string id, string name)
    {
        heroID = id;
        heroName = name;
        totalGames = 0;
        wins = 0;
        losses = 0;
        winRate = 0;
        averageKDA = 0;
        averageDamage = 0;
        averageGold = 0;
        bestScore = 0;
    }
    
    public void UpdateSummary(MatchStatsAnalysis stats)
    {
        totalGames++;
        if (stats.result == "Victory")
        {
            wins++;
        }
        else
        {
            losses++;
        }
        
        winRate = (float)wins / totalGames * 100;
        averageKDA = (averageKDA * (totalGames - 1) + stats.kda) / totalGames;
        averageDamage = (averageDamage * (totalGames - 1) + stats.totalDamageDealt) / totalGames;
        averageGold = (averageGold * (totalGames - 1) + stats.goldEarned) / totalGames;
        
        if (stats.performanceScore > bestScore)
        {
            bestScore = stats.performanceScore;
            bestScoreMatchID = stats.matchID;
        }
    }
}
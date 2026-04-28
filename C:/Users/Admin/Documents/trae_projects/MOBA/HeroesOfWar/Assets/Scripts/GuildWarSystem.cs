using System.Linq;

[System.Serializable]
public class GuildWar
{
    public string warID;
    public string guild1ID;
    public string guild1Name;
    public string guild2ID;
    public string guild2Name;
    public string startTime;
    public string endTime;
    public string warStatus;
    public int guild1Score;
    public int guild2Score;
    public List<WarEvent> warEvents;
    public string winnerID;
    
    public GuildWar(string id, string guild1, string guild1Name, string guild2, string guild2Name)
    {
        warID = id;
        this.guild1ID = guild1;
        this.guild1Name = guild1Name;
        this.guild2ID = guild2;
        this.guild2Name = guild2Name;
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        endTime = System.DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm");
        warStatus = "Pending";
        guild1Score = 0;
        guild2Score = 0;
        warEvents = new List<WarEvent>();
        winnerID = "";
    }
    
    public void StartWar()
    {
        warStatus = "Active";
    }
    
    public void EndWar()
    {
        warStatus = "Ended";
        DetermineWinner();
    }
    
    private void DetermineWinner()
    {
        if (guild1Score > guild2Score)
        {
            winnerID = guild1ID;
        }
        else if (guild2Score > guild1Score)
        {
            winnerID = guild2ID;
        }
        else
        {
            winnerID = "Draw";
        }
    }
    
    public void AddScore(string guildID, int score)
    {
        if (guildID == guild1ID)
        {
            guild1Score += score;
        }
        else if (guildID == guild2ID)
        {
            guild2Score += score;
        }
    }
    
    public void AddWarEvent(string eventType, string guildID, string playerID, int score)
    {
        WarEvent warEvent = new WarEvent(eventType, guildID, playerID, score);
        warEvents.Add(warEvent);
    }
}

[System.Serializable]
public class WarEvent
{
    public string eventID;
    public string eventType;
    public string guildID;
    public string playerID;
    public int score;
    public string eventTime;
    
    public WarEvent(string type, string guild, string player, int score)
    {
        eventID = "event_" + System.DateTime.Now.Ticks;
        eventType = type;
        guildID = guild;
        playerID = player;
        this.score = score;
        eventTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}

[System.Serializable]
public class GuildWarSeason
{
    public string seasonID;
    public string seasonName;
    public string startTime;
    public string endTime;
    public List<GuildWar> wars;
    public List<GuildWarRank> ranks;
    
    public GuildWarSeason(string id, string name)
    {
        seasonID = id;
        seasonName = name;
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd");
        endTime = System.DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
        wars = new List<GuildWar>();
        ranks = new List<GuildWarRank>();
    }
    
    public void AddWar(GuildWar war)
    {
        wars.Add(war);
    }
    
    public void UpdateRanks()
    {
        Dictionary<string, int> guildScores = new Dictionary<string, int>();
        
        foreach (GuildWar war in wars)
        {
            if (war.winnerID != "" && war.winnerID != "Draw")
            {
                if (guildScores.ContainsKey(war.winnerID))
                {
                    guildScores[war.winnerID] += 1;
                }
                else
                {
                    guildScores[war.winnerID] = 1;
                }
            }
        }
        
        ranks.Clear();
        int rank = 1;
        foreach (var kvp in guildScores.OrderByDescending(x => x.Value))
        {
            GuildWarRank guildRank = new GuildWarRank(kvp.Key, kvp.Value, rank);
            ranks.Add(guildRank);
            rank++;
        }
    }
}

[System.Serializable]
public class GuildWarRank
{
    public string guildID;
    public int wins;
    public int rank;
    
    public GuildWarRank(string guild, int winCount, int r)
    {
        guildID = guild;
        wins = winCount;
        rank = r;
    }
}

[System.Serializable]
public class GuildWarManagerData
{
    public List<GuildWar> activeWars;
    public List<GuildWarSeason> seasons;
    
    public GuildWarManagerData()
    {
        activeWars = new List<GuildWar>();
        seasons = new List<GuildWarSeason>();
    }
    
    public void AddWar(GuildWar war)
    {
        activeWars.Add(war);
    }
    
    public void AddSeason(GuildWarSeason season)
    {
        seasons.Add(season);
    }
    
    public GuildWar GetWar(string warID)
    {
        return activeWars.Find(w => w.warID == warID);
    }
    
    public GuildWarSeason GetCurrentSeason()
    {
        if (seasons.Count > 0)
        {
            return seasons[seasons.Count - 1];
        }
        return null;
    }
}
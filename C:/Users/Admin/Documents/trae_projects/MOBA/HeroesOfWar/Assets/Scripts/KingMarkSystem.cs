[System.Serializable]
public class KingMarkSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<KingMark> marks;
    public List<Season> seasons;
    
    public KingMarkSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        marks = new List<KingMark>();
        seasons = new List<Season>();
    }
    
    public void AddMark(KingMark mark)
    {
        marks.Add(mark);
    }
    
    public void AddSeason(Season season)
    {
        seasons.Add(season);
    }
    
    public KingMark GetMark(string markID)
    {
        return marks.Find(m => m.markID == markID);
    }
    
    public Season GetSeason(string seasonID)
    {
        return seasons.Find(s => s.seasonID == seasonID);
    }
    
    public List<KingMark> GetMarksByPlayer(string playerID)
    {
        return marks.FindAll(m => m.playerID == playerID);
    }
    
    public List<KingMark> GetMarksBySeason(string seasonID)
    {
        return marks.FindAll(m => m.seasonID == seasonID);
    }
    
    public List<Season> GetActiveSeasons()
    {
        return seasons.FindAll(s => s.isActive);
    }
}

[System.Serializable]
public class Season
{
    public string seasonID;
    public string seasonName;
    public string seasonDescription;
    public string startTime;
    public string endTime;
    public bool isActive;
    public int seasonNumber;
    
    public Season(string id, string name, string desc, string start, string end, int number)
    {
        seasonID = id;
        seasonName = name;
        seasonDescription = desc;
        startTime = start;
        endTime = end;
        isActive = false;
        seasonNumber = number;
    }
    
    public void Activate()
    {
        isActive = true;
    }
    
    public void Deactivate()
    {
        isActive = false;
    }
    
    public bool IsActive()
    {
        return isActive;
    }
}

[System.Serializable]
public class KingMark
{
    public string markID;
    public string playerID;
    public string playerName;
    public string seasonID;
    public string seasonName;
    public int rank;
    public string rankName;
    public int stars;
    public bool isKing;
    public string obtainedAt;
    
    public KingMark(string id, string player, string playerName, string season, string seasonName, int rank, string rankName, int stars, bool isKing)
    {
        markID = id;
        playerID = player;
        this.playerName = playerName;
        seasonID = season;
        this.seasonName = seasonName;
        this.rank = rank;
        this.rankName = rankName;
        this.stars = stars;
        this.isKing = isKing;
        obtainedAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class PlayerMarkData
{
    public string playerID;
    public List<string> markIDs;
    public int totalKingSeasons;
    public string highestRank;
    
    public PlayerMarkData(string playerID)
    {
        this.playerID = playerID;
        markIDs = new List<string>();
        totalKingSeasons = 0;
        highestRank = "青铜";
    }
    
    public void AddMark(string markID, bool isKing, string rankName)
    {
        if (!markIDs.Contains(markID))
        {
            markIDs.Add(markID);
            if (isKing)
            {
                totalKingSeasons++;
            }
            if (GetRankPriority(rankName) > GetRankPriority(highestRank))
            {
                highestRank = rankName;
            }
        }
    }
    
    private int GetRankPriority(string rankName)
    {
        switch (rankName)
        {
            case "王者": return 9;
            case "星耀": return 8;
            case "钻石": return 7;
            case "铂金": return 6;
            case "黄金": return 5;
            case "白银": return 4;
            case "青铜": return 3;
            default: return 0;
        }
    }
    
    public List<string> GetMarks()
    {
        return markIDs;
    }
    
    public int GetTotalKingSeasons()
    {
        return totalKingSeasons;
    }
    
    public string GetHighestRank()
    {
        return highestRank;
    }
}

[System.Serializable]
public class KingMarkManagerData
{
    public KingMarkSystem system;
    public List<PlayerMarkData> playerData;
    
    public KingMarkManagerData()
    {
        system = new KingMarkSystem("king_mark_system", "王者印记系统", "管理玩家的王者印记");
        playerData = new List<PlayerMarkData>();
    }
    
    public void AddPlayerData(PlayerMarkData data)
    {
        playerData.Add(data);
    }
    
    public PlayerMarkData GetPlayerData(string playerID)
    {
        return playerData.Find(pd => pd.playerID == playerID);
    }
}
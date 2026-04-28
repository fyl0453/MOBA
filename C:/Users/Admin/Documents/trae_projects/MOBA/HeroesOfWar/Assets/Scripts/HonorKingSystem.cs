[System.Serializable]
public class HonorKingSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<HonorKing> honorKings;
    public List<SeasonHonor> seasonHonors;
    
    public HonorKingSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        honorKings = new List<HonorKing>();
        seasonHonors = new List<SeasonHonor>();
    }
    
    public void AddHonorKing(HonorKing honorKing)
    {
        honorKings.Add(honorKing);
    }
    
    public void AddSeasonHonor(SeasonHonor seasonHonor)
    {
        seasonHonors.Add(seasonHonor);
    }
    
    public HonorKing GetHonorKing(string playerID)
    {
        return honorKings.Find(hk => hk.playerID == playerID);
    }
    
    public SeasonHonor GetSeasonHonor(string seasonID)
    {
        return seasonHonors.Find(sh => sh.seasonID == seasonID);
    }
    
    public List<HonorKing> GetTopHonorKings(int limit = 100)
    {
        List<HonorKing> sorted = new List<HonorKing>(honorKings);
        sorted.Sort((a, b) => b.stars.CompareTo(a.stars));
        return sorted.GetRange(0, Mathf.Min(limit, sorted.Count));
    }
}

[System.Serializable]
public class HonorKing
{
    public string playerID;
    public string playerName;
    public int stars;
    public int rank;
    public string seasonID;
    public string seasonName;
    public bool isHonorKing;
    public string obtainedAt;
    
    public HonorKing(string playerID, string playerName, int stars, string seasonID, string seasonName)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        this.stars = stars;
        rank = 0;
        this.seasonID = seasonID;
        this.seasonName = seasonName;
        isHonorKing = false;
        obtainedAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateStars(int stars)
    {
        this.stars = stars;
    }
    
    public void UpdateRank(int rank)
    {
        this.rank = rank;
        isHonorKing = rank <= 100;
    }
}

[System.Serializable]
public class SeasonHonor
{
    public string seasonID;
    public string seasonName;
    public string startTime;
    public string endTime;
    public List<string> honorKingIDs;
    
    public SeasonHonor(string seasonID, string seasonName, string start, string end)
    {
        this.seasonID = seasonID;
        this.seasonName = seasonName;
        startTime = start;
        endTime = end;
        honorKingIDs = new List<string>();
    }
    
    public void AddHonorKing(string playerID)
    {
        if (!honorKingIDs.Contains(playerID))
        {
            honorKingIDs.Add(playerID);
        }
    }
}

[System.Serializable]
public class HonorKingManagerData
{
    public HonorKingSystem system;
    
    public HonorKingManagerData()
    {
        system = new HonorKingSystem("honor_king_system", "荣耀王者系统", "管理荣耀王者排名");
    }
}
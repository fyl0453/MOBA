[System.Serializable]
public class PeakMatchSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<PeakMatch> matches;
    public List<PeakPlayer> players;
    public List<PeakSeason> seasons;
    
    public PeakMatchSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        matches = new List<PeakMatch>();
        players = new List<PeakPlayer>();
        seasons = new List<PeakSeason>();
    }
    
    public void AddMatch(PeakMatch match)
    {
        matches.Add(match);
    }
    
    public void AddPlayer(PeakPlayer player)
    {
        players.Add(player);
    }
    
    public void AddSeason(PeakSeason season)
    {
        seasons.Add(season);
    }
    
    public PeakMatch GetMatch(string matchID)
    {
        return matches.Find(m => m.matchID == matchID);
    }
    
    public PeakPlayer GetPlayer(string playerID)
    {
        return players.Find(p => p.playerID == playerID);
    }
    
    public PeakSeason GetSeason(string seasonID)
    {
        return seasons.Find(s => s.seasonID == seasonID);
    }
    
    public List<PeakPlayer> GetTopPlayers(int limit = 100)
    {
        List<PeakPlayer> sorted = new List<PeakPlayer>(players);
        sorted.Sort((a, b) => b.peakPoints.CompareTo(a.peakPoints));
        return sorted.GetRange(0, Mathf.Min(limit, sorted.Count));
    }
}

[System.Serializable]
public class PeakPlayer
{
    public string playerID;
    public string playerName;
    public int peakPoints;
    public int peakRank;
    public int wins;
    public int losses;
    public string currentSeasonID;
    public List<PeakMatchHistory> matchHistory;
    
    public PeakPlayer(string playerID, string playerName, int peakPoints, string seasonID)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        this.peakPoints = peakPoints;
        peakRank = 0;
        wins = 0;
        losses = 0;
        currentSeasonID = seasonID;
        matchHistory = new List<PeakMatchHistory>();
    }
    
    public void UpdatePeakPoints(int points)
    {
        peakPoints = points;
    }
    
    public void UpdateRank(int rank)
    {
        peakRank = rank;
    }
    
    public void AddMatchHistory(PeakMatchHistory history)
    {
        matchHistory.Add(history);
    }
    
    public void WinMatch()
    {
        wins++;
    }
    
    public void LoseMatch()
    {
        losses++;
    }
    
    public float GetWinRate()
    {
        int total = wins + losses;
        return total > 0 ? (float)wins / total : 0;
    }
}

[System.Serializable]
public class PeakMatch
{
    public string matchID;
    public string seasonID;
    public List<string> playerIDs;
    public List<int> teamIDs;
    public List<string> heroIDs;
    public string winnerTeamID;
    public string startTime;
    public string endTime;
    public string matchStatus;
    
    public PeakMatch(string id, string seasonID)
    {
        matchID = id;
        this.seasonID = seasonID;
        playerIDs = new List<string>();
        teamIDs = new List<int>();
        heroIDs = new List<string>();
        winnerTeamID = "";
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        endTime = "";
        matchStatus = "Pending";
    }
    
    public void AddPlayer(string playerID, int teamID, string heroID)
    {
        playerIDs.Add(playerID);
        teamIDs.Add(teamID);
        heroIDs.Add(heroID);
    }
    
    public void StartMatch()
    {
        matchStatus = "InProgress";
    }
    
    public void EndMatch(string winnerTeam)
    {
        matchStatus = "Completed";
        endTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        winnerTeamID = winnerTeam;
    }
}

[System.Serializable]
public class PeakMatchHistory
{
    public string matchID;
    public string matchTime;
    public bool isWin;
    public string heroID;
    public int pointsChange;
    
    public PeakMatchHistory(string matchID, string matchTime, bool isWin, string heroID, int pointsChange)
    {
        this.matchID = matchID;
        this.matchTime = matchTime;
        this.isWin = isWin;
        this.heroID = heroID;
        this.pointsChange = pointsChange;
    }
}

[System.Serializable]
public class PeakSeason
{
    public string seasonID;
    public string seasonName;
    public string startTime;
    public string endTime;
    public bool isActive;
    
    public PeakSeason(string seasonID, string seasonName, string start, string end)
    {
        this.seasonID = seasonID;
        this.seasonName = seasonName;
        startTime = start;
        endTime = end;
        isActive = false;
    }
    
    public void Activate()
    {
        isActive = true;
    }
    
    public void Deactivate()
    {
        isActive = false;
    }
}

[System.Serializable]
public class PeakMatchManagerData
{
    public PeakMatchSystem system;
    
    public PeakMatchManagerData()
    {
        system = new PeakMatchSystem("peak_match_system", "巅峰赛系统", "管理高端玩家的专属排位赛");
    }
}
[System.Serializable]
public class EntertainmentModeSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<EntertainmentMode> modes;
    public List<ModeMatch> matches;
    
    public EntertainmentModeSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        modes = new List<EntertainmentMode>();
        matches = new List<ModeMatch>();
    }
    
    public void AddMode(EntertainmentMode mode)
    {
        modes.Add(mode);
    }
    
    public void AddMatch(ModeMatch match)
    {
        matches.Add(match);
    }
    
    public EntertainmentMode GetMode(string modeID)
    {
        return modes.Find(m => m.modeID == modeID);
    }
    
    public ModeMatch GetMatch(string matchID)
    {
        return matches.Find(m => m.matchID == matchID);
    }
    
    public List<EntertainmentMode> GetActiveModes()
    {
        return modes.FindAll(m => m.isActive);
    }
}

[System.Serializable]
public class EntertainmentMode
{
    public string modeID;
    public string modeName;
    public string modeDescription;
    public string modeType;
    public int playerCount;
    public int matchDuration;
    public bool isActive;
    public string modeIcon;
    public string modeBanner;
    
    public EntertainmentMode(string id, string name, string desc, string type, int playerCount, int matchDuration)
    {
        modeID = id;
        modeName = name;
        modeDescription = desc;
        modeType = type;
        this.playerCount = playerCount;
        this.matchDuration = matchDuration;
        isActive = true;
        modeIcon = "";
        modeBanner = "";
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
public class ModeMatch
{
    public string matchID;
    public string modeID;
    public string modeName;
    public List<string> playerIDs;
    public List<int> teamIDs;
    public List<string> heroIDs;
    public string winnerTeamID;
    public string startTime;
    public string endTime;
    public string matchStatus;
    
    public ModeMatch(string id, string modeID, string modeName)
    {
        matchID = id;
        this.modeID = modeID;
        this.modeName = modeName;
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
public class EntertainmentModeManagerData
{
    public EntertainmentModeSystem system;
    
    public EntertainmentModeManagerData()
    {
        system = new EntertainmentModeSystem("entertainment_mode_system", "娱乐模式系统", "管理各种娱乐模式");
    }
}
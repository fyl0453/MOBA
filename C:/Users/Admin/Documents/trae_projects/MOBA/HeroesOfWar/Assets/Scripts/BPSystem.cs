[System.Serializable]
public class BPSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<BPPhase> phases;
    public List<BPSession> sessions;
    
    public BPSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        phases = new List<BPPhase>();
        sessions = new List<BPSession>();
    }
    
    public void AddPhase(BPPhase phase)
    {
        phases.Add(phase);
    }
    
    public void AddSession(BPSession session)
    {
        sessions.Add(session);
    }
    
    public BPPhase GetPhase(string phaseID)
    {
        return phases.Find(p => p.phaseID == phaseID);
    }
    
    public BPSession GetSession(string sessionID)
    {
        return sessions.Find(s => s.sessionID == sessionID);
    }
}

[System.Serializable]
public class BPPhase
{
    public string phaseID;
    public string phaseName;
    public string phaseDescription;
    public int phaseOrder;
    public int duration;
    public string phaseType;
    
    public BPPhase(string id, string name, string desc, int order, int duration, string type)
    {
        phaseID = id;
        phaseName = name;
        phaseDescription = desc;
        phaseOrder = order;
        this.duration = duration;
        phaseType = type;
    }
}

[System.Serializable]
public class BPSession
{
    public string sessionID;
    public string matchID;
    public string sessionStatus;
    public List<BPAction> actions;
    public List<BPPlayer> players;
    public string currentPhaseID;
    public int currentPhaseIndex;
    public int startTime;
    public int endTime;
    
    public BPSession(string id, string matchID)
    {
        sessionID = id;
        this.matchID = matchID;
        sessionStatus = "Pending";
        actions = new List<BPAction>();
        players = new List<BPPlayer>();
        currentPhaseID = "";
        currentPhaseIndex = 0;
        startTime = 0;
        endTime = 0;
    }
    
    public void AddAction(BPAction action)
    {
        actions.Add(action);
    }
    
    public void AddPlayer(BPPlayer player)
    {
        players.Add(player);
    }
    
    public void StartSession()
    {
        sessionStatus = "InProgress";
        startTime = (int)System.DateTime.Now.Ticks;
    }
    
    public void EndSession()
    {
        sessionStatus = "Completed";
        endTime = (int)System.DateTime.Now.Ticks;
    }
    
    public void SetCurrentPhase(string phaseID, int index)
    {
        currentPhaseID = phaseID;
        currentPhaseIndex = index;
    }
}

[System.Serializable]
public class BPAction
{
    public string actionID;
    public string sessionID;
    public string playerID;
    public string actionType;
    public string heroID;
    public string heroName;
    public int timestamp;
    public int phaseIndex;
    
    public BPAction(string id, string sessionID, string playerID, string type, string heroID, string heroName, int phaseIndex)
    {
        actionID = id;
        this.sessionID = sessionID;
        this.playerID = playerID;
        actionType = type;
        this.heroID = heroID;
        this.heroName = heroName;
        timestamp = (int)System.DateTime.Now.Ticks;
        this.phaseIndex = phaseIndex;
    }
}

[System.Serializable]
public class BPPlayer
{
    public string playerID;
    public string playerName;
    public int teamID;
    public int pickOrder;
    public List<string> bannedHeroes;
    public List<string> pickedHeroes;
    
    public BPPlayer(string playerID, string playerName, int teamID, int pickOrder)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        this.teamID = teamID;
        this.pickOrder = pickOrder;
        bannedHeroes = new List<string>();
        pickedHeroes = new List<string>();
    }
    
    public void BanHero(string heroID)
    {
        bannedHeroes.Add(heroID);
    }
    
    public void PickHero(string heroID)
    {
        pickedHeroes.Add(heroID);
    }
}

[System.Serializable]
public class BPManagerData
{
    public BPSystem system;
    
    public BPManagerData()
    {
        system = new BPSystem("bp_system", "BP系统", "管理排位赛中的禁用和选择英雄");
    }
}
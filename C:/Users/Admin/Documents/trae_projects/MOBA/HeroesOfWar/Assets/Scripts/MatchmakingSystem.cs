[System.Serializable]
public class MatchmakingQueue
{
    public string queueID;
    public string queueName;
    public MatchType matchType;
    public int minPlayers;
    public int maxPlayers;
    public int teamSize;
    public List<PlayerMatchmaking> players;
    public float averageWaitTime;
    
    public MatchmakingQueue(string id, string name, MatchType type, int min, int max, int teamSize)
    {
        queueID = id;
        queueName = name;
        matchType = type;
        minPlayers = min;
        maxPlayers = max;
        this.teamSize = teamSize;
        players = new List<PlayerMatchmaking>();
        averageWaitTime = 0f;
    }
    
    public void AddPlayer(PlayerMatchmaking player)
    {
        players.Add(player);
    }
    
    public void RemovePlayer(string playerID)
    {
        players.RemoveAll(p => p.playerID == playerID);
    }
    
    public int GetPlayerCount()
    {
        return players.Count;
    }
    
    public bool HasEnoughPlayers()
    {
        return players.Count >= minPlayers;
    }
    
    public List<PlayerMatchmaking> GetPlayers()
    {
        return players;
    }
}

[System.Serializable]
public class PlayerMatchmaking
{
    public string playerID;
    public string playerName;
    public int rankPoints;
    public int level;
    public MatchType matchType;
    public float joinTime;
    public float waitTime;
    public bool isReady;
    
    public PlayerMatchmaking(string id, string name, int points, int lvl, MatchType type)
    {
        playerID = id;
        playerName = name;
        rankPoints = points;
        level = lvl;
        matchType = type;
        joinTime = Time.time;
        waitTime = 0f;
        isReady = false;
    }
    
    public void UpdateWaitTime()
    {
        waitTime = Time.time - joinTime;
    }
    
    public void SetReady(bool ready)
    {
        isReady = ready;
    }
}

[System.Serializable]
public class Match
{
    public string matchID;
    public string matchName;
    public MatchType matchType;
    public List<Team> teams;
    public string mapID;
    public float matchStartTime;
    public float matchEndTime;
    public MatchStatus matchStatus;
    
    public Match(string id, string name, MatchType type, string map)
    {
        matchID = id;
        matchName = name;
        matchType = type;
        teams = new List<Team>();
        mapID = map;
        matchStartTime = Time.time;
        matchEndTime = 0f;
        matchStatus = MatchStatus.Waiting;
    }
    
    public void AddTeam(Team team)
    {
        teams.Add(team);
    }
    
    public void StartMatch()
    {
        matchStatus = MatchStatus.InProgress;
    }
    
    public void EndMatch()
    {
        matchStatus = MatchStatus.Ended;
        matchEndTime = Time.time;
    }
    
    public float GetMatchDuration()
    {
        return matchEndTime > 0 ? matchEndTime - matchStartTime : Time.time - matchStartTime;
    }
}

[System.Serializable]
public class Team
{
    public string teamID;
    public string teamName;
    public List<PlayerMatchmaking> players;
    public bool isWinner;
    
    public Team(string id, string name)
    {
        teamID = id;
        teamName = name;
        players = new List<PlayerMatchmaking>();
        isWinner = false;
    }
    
    public void AddPlayer(PlayerMatchmaking player)
    {
        players.Add(player);
    }
    
    public int GetPlayerCount()
    {
        return players.Count;
    }
    
    public void SetWinner(bool winner)
    {
        isWinner = winner;
    }
}

[System.Serializable]
public enum MatchType
{
    Normal,
    Ranked,
    Arcade,
    Custom
}

[System.Serializable]
public enum MatchStatus
{
    Waiting,
    InProgress,
    Ended
}

[System.Serializable]
public class MatchmakingManagerData
{
    public List<MatchmakingQueue> queues;
    public List<Match> activeMatches;
    public List<Match> completedMatches;
    public float totalMatchmakingTime;
    public int totalMatches;
    
    public MatchmakingManagerData()
    {
        queues = new List<MatchmakingQueue>();
        activeMatches = new List<Match>();
        completedMatches = new List<Match>();
        totalMatchmakingTime = 0f;
        totalMatches = 0;
    }
    
    public void AddQueue(MatchmakingQueue queue)
    {
        queues.Add(queue);
    }
    
    public void AddActiveMatch(Match match)
    {
        activeMatches.Add(match);
        totalMatches++;
    }
    
    public void AddCompletedMatch(Match match)
    {
        activeMatches.Remove(match);
        completedMatches.Add(match);
    }
    
    public MatchmakingQueue GetQueue(string queueID)
    {
        return queues.Find(q => q.queueID == queueID);
    }
    
    public Match GetActiveMatch(string matchID)
    {
        return activeMatches.Find(m => m.matchID == matchID);
    }
    
    public Match GetCompletedMatch(string matchID)
    {
        return completedMatches.Find(m => m.matchID == matchID);
    }
    
    public void UpdateTotalMatchmakingTime(float time)
    {
        totalMatchmakingTime += time;
    }
    
    public float GetAverageMatchmakingTime()
    {
        return totalMatches > 0 ? totalMatchmakingTime / totalMatches : 0f;
    }
}
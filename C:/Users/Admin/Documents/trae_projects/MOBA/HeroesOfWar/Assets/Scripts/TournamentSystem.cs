[System.Serializable]
public class TournamentSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Tournament> tournaments;
    public List<Match> matches;
    public List<Team> teams;
    public List<Player> players;
    
    public TournamentSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        tournaments = new List<Tournament>();
        matches = new List<Match>();
        teams = new List<Team>();
        players = new List<Player>();
    }
    
    public void AddTournament(Tournament tournament)
    {
        tournaments.Add(tournament);
    }
    
    public void AddMatch(Match match)
    {
        matches.Add(match);
    }
    
    public void AddTeam(Team team)
    {
        teams.Add(team);
    }
    
    public void AddPlayer(Player player)
    {
        players.Add(player);
    }
    
    public Tournament GetTournament(string tournamentID)
    {
        return tournaments.Find(t => t.tournamentID == tournamentID);
    }
    
    public Match GetMatch(string matchID)
    {
        return matches.Find(m => m.matchID == matchID);
    }
    
    public Team GetTeam(string teamID)
    {
        return teams.Find(t => t.teamID == teamID);
    }
    
    public Player GetPlayer(string playerID)
    {
        return players.Find(p => p.playerID == playerID);
    }
}

[System.Serializable]
public class Tournament
{
    public string tournamentID;
    public string tournamentName;
    public string tournamentDescription;
    public string tournamentType;
    public string startTime;
    public string endTime;
    public int totalTeams;
    public int prizePool;
    public List<Match> tournamentMatches;
    public List<Team> participatingTeams;
    public string tournamentStatus;
    
    public Tournament(string id, string name, string desc, string type, string start, string end, int teams, int prize)
    {
        tournamentID = id;
        tournamentName = name;
        tournamentDescription = desc;
        tournamentType = type;
        startTime = start;
        endTime = end;
        totalTeams = teams;
        prizePool = prize;
        tournamentMatches = new List<Match>();
        participatingTeams = new List<Team>();
        tournamentStatus = "Upcoming";
    }
    
    public void AddMatch(Match match)
    {
        tournamentMatches.Add(match);
    }
    
    public void AddTeam(Team team)
    {
        participatingTeams.Add(team);
    }
    
    public void StartTournament()
    {
        tournamentStatus = "Ongoing";
    }
    
    public void EndTournament()
    {
        tournamentStatus = "Ended";
    }
}

[System.Serializable]
public class Match
{
    public string matchID;
    public string matchName;
    public string tournamentID;
    public string team1ID;
    public string team2ID;
    public string startTime;
    public string endTime;
    public string matchStatus;
    public string winnerID;
    public int team1Score;
    public int team2Score;
    public string streamURL;
    
    public Match(string id, string name, string tournament, string team1, string team2, string start)
    {
        matchID = id;
        matchName = name;
        tournamentID = tournament;
        team1ID = team1;
        team2ID = team2;
        startTime = start;
        endTime = "";
        matchStatus = "Scheduled";
        winnerID = "";
        team1Score = 0;
        team2Score = 0;
        streamURL = "";
    }
    
    public void StartMatch()
    {
        matchStatus = "Ongoing";
    }
    
    public void EndMatch(string winner, int score1, int score2)
    {
        matchStatus = "Ended";
        winnerID = winner;
        team1Score = score1;
        team2Score = score2;
        endTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void SetStreamURL(string url)
    {
        streamURL = url;
    }
}

[System.Serializable]
public class Team
{
    public string teamID;
    public string teamName;
    public string teamLogo;
    public string teamTag;
    public List<Player> teamMembers;
    public int totalWins;
    public int totalLosses;
    public float winRate;
    public string teamStatus;
    
    public Team(string id, string name, string logo, string tag)
    {
        teamID = id;
        teamName = name;
        teamLogo = logo;
        teamTag = tag;
        teamMembers = new List<Player>();
        totalWins = 0;
        totalLosses = 0;
        winRate = 0f;
        teamStatus = "Active";
    }
    
    public void AddPlayer(Player player)
    {
        teamMembers.Add(player);
    }
    
    public void UpdateStats(bool won)
    {
        if (won)
        {
            totalWins++;
        }
        else
        {
            totalLosses++;
        }
        winRate = (float)totalWins / (totalWins + totalLosses);
    }
}

[System.Serializable]
public class Player
{
    public string playerID;
    public string playerName;
    public string playerRole;
    public string teamID;
    public int totalWins;
    public int totalLosses;
    public float winRate;
    public int totalKills;
    public int totalDeaths;
    public int totalAssists;
    public float kda;
    public string playerStatus;
    
    public Player(string id, string name, string role, string team)
    {
        playerID = id;
        playerName = name;
        playerRole = role;
        teamID = team;
        totalWins = 0;
        totalLosses = 0;
        winRate = 0f;
        totalKills = 0;
        totalDeaths = 0;
        totalAssists = 0;
        kda = 0f;
        playerStatus = "Active";
    }
    
    public void UpdateStats(bool won, int kills, int deaths, int assists)
    {
        if (won)
        {
            totalWins++;
        }
        else
        {
            totalLosses++;
        }
        winRate = (float)totalWins / (totalWins + totalLosses);
        totalKills += kills;
        totalDeaths += deaths;
        totalAssists += assists;
        kda = totalDeaths > 0 ? (float)(totalKills + totalAssists) / totalDeaths : (float)(totalKills + totalAssists);
    }
}

[System.Serializable]
public class TournamentManagerData
{
    public TournamentSystem system;
    
    public TournamentManagerData()
    {
        system = new TournamentSystem("tournament", "赛事系统", "管理游戏赛事");
    }
}

[System.Serializable]
public class Stream
{
    public string streamID;
    public string streamTitle;
    public string streamerName;
    public string streamURL;
    public string streamStatus;
    public int viewerCount;
    public string startTime;
    public string endTime;
    
    public Stream(string id, string title, string streamer, string url)
    {
        streamID = id;
        streamTitle = title;
        streamerName = streamer;
        streamURL = url;
        streamStatus = "Offline";
        viewerCount = 0;
        startTime = "";
        endTime = "";
    }
    
    public void StartStream()
    {
        streamStatus = "Live";
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void EndStream()
    {
        streamStatus = "Offline";
        endTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateViewerCount(int count)
    {
        viewerCount = count;
    }
}
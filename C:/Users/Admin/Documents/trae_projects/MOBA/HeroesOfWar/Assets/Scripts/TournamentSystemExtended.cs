[System.Serializable]
public class TournamentSystemExtended
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Tournament> tournaments;
    public List<Team> teams;
    public List<Match> matches;
    public List<Player> players;
    public List<Prize> prizes;
    
    public TournamentSystemExtended(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        tournaments = new List<Tournament>();
        teams = new List<Team>();
        matches = new List<Match>();
        players = new List<Player>();
        prizes = new List<Prize>();
    }
    
    public void AddTournament(Tournament tournament)
    {
        tournaments.Add(tournament);
    }
    
    public void AddTeam(Team team)
    {
        teams.Add(team);
    }
    
    public void AddMatch(Match match)
    {
        matches.Add(match);
    }
    
    public void AddPlayer(Player player)
    {
        players.Add(player);
    }
    
    public void AddPrize(Prize prize)
    {
        prizes.Add(prize);
    }
    
    public Tournament GetTournament(string tournamentID)
    {
        return tournaments.Find(t => t.tournamentID == tournamentID);
    }
    
    public Team GetTeam(string teamID)
    {
        return teams.Find(t => t.teamID == teamID);
    }
    
    public Match GetMatch(string matchID)
    {
        return matches.Find(m => m.matchID == matchID);
    }
    
    public Player GetPlayer(string playerID)
    {
        return players.Find(p => p.playerID == playerID);
    }
    
    public Prize GetPrize(string prizeID)
    {
        return prizes.Find(p => p.prizeID == prizeID);
    }
    
    public List<Tournament> GetTournamentsByType(string type)
    {
        return tournaments.FindAll(t => t.tournamentType == type);
    }
    
    public List<Match> GetMatchesByTournament(string tournamentID)
    {
        return matches.FindAll(m => m.tournamentID == tournamentID);
    }
    
    public List<Team> GetTeamsByTournament(string tournamentID)
    {
        List<Team> result = new List<Team>();
        foreach (Team team in teams)
        {
            if (team.tournamentIDs.Contains(tournamentID))
            {
                result.Add(team);
            }
        }
        return result;
    }
}

[System.Serializable]
public class Tournament
{
    public string tournamentID;
    public string tournamentName;
    public string tournamentType;
    public string description;
    public string startDate;
    public string endDate;
    public string status;
    public int maxTeams;
    public int entryFee;
    public List<string> teamIDs;
    public List<string> matchIDs;
    public List<string> prizeIDs;
    
    public Tournament(string id, string name, string type, string desc, string start, string end, int maxTeams, int entryFee)
    {
        tournamentID = id;
        tournamentName = name;
        tournamentType = type;
        description = desc;
        startDate = start;
        endDate = end;
        status = "Upcoming";
        this.maxTeams = maxTeams;
        this.entryFee = entryFee;
        teamIDs = new List<string>();
        matchIDs = new List<string>();
        prizeIDs = new List<string>();
    }
    
    public void AddTeam(string teamID)
    {
        teamIDs.Add(teamID);
    }
    
    public void AddMatch(string matchID)
    {
        matchIDs.Add(matchID);
    }
    
    public void AddPrize(string prizeID)
    {
        prizeIDs.Add(prizeID);
    }
    
    public void Start()
    {
        status = "Ongoing";
    }
    
    public void End()
    {
        status = "Ended";
    }
}

[System.Serializable]
public class Team
{
    public string teamID;
    public string teamName;
    public string captainID;
    public string logoURL;
    public string region;
    public List<string> playerIDs;
    public List<string> tournamentIDs;
    public int totalWins;
    public int totalLosses;
    public float winRate;
    
    public Team(string id, string name, string captain, string logo, string region)
    {
        teamID = id;
        teamName = name;
        captainID = captain;
        logoURL = logo;
        this.region = region;
        playerIDs = new List<string>();
        tournamentIDs = new List<string>();
        totalWins = 0;
        totalLosses = 0;
        winRate = 0;
    }
    
    public void AddPlayer(string playerID)
    {
        playerIDs.Add(playerID);
    }
    
    public void AddTournament(string tournamentID)
    {
        tournamentIDs.Add(tournamentID);
    }
    
    public void UpdateStats(int wins, int losses)
    {
        totalWins += wins;
        totalLosses += losses;
        int totalMatches = totalWins + totalLosses;
        winRate = totalMatches > 0 ? (float)totalWins / totalMatches * 100 : 0;
    }
}

[System.Serializable]
public class Player
{
    public string playerID;
    public string playerName;
    public string teamID;
    public string role;
    public string avatarURL;
    public string country;
    public int age;
    public int totalMatches;
    public int totalWins;
    public float winRate;
    public int totalKills;
    public int totalDeaths;
    public int totalAssists;
    public float kda;
    
    public Player(string id, string name, string team, string role, string avatar, string country, int age)
    {
        playerID = id;
        playerName = name;
        teamID = team;
        this.role = role;
        avatarURL = avatar;
        this.country = country;
        this.age = age;
        totalMatches = 0;
        totalWins = 0;
        winRate = 0;
        totalKills = 0;
        totalDeaths = 0;
        totalAssists = 0;
        kda = 0;
    }
    
    public void UpdateStats(int kills, int deaths, int assists, bool isWin)
    {
        totalMatches++;
        if (isWin)
        {
            totalWins++;
        }
        totalKills += kills;
        totalDeaths += deaths;
        totalAssists += assists;
        
        winRate = totalMatches > 0 ? (float)totalWins / totalMatches * 100 : 0;
        kda = totalDeaths > 0 ? (float)(totalKills + totalAssists) / totalDeaths : totalKills + totalAssists;
    }
}

[System.Serializable]
public class Match
{
    public string matchID;
    public string tournamentID;
    public string team1ID;
    public string team2ID;
    public string team1Name;
    public string team2Name;
    public string matchTime;
    public string matchStatus;
    public int team1Score;
    public int team2Score;
    public string winnerID;
    public string matchType;
    public string matchStage;
    
    public Match(string id, string tournament, string team1, string team2, string team1Name, string team2Name, string time, string type, string stage)
    {
        matchID = id;
        tournamentID = tournament;
        team1ID = team1;
        team2ID = team2;
        this.team1Name = team1Name;
        this.team2Name = team2Name;
        matchTime = time;
        matchStatus = "Scheduled";
        team1Score = 0;
        team2Score = 0;
        winnerID = "";
        matchType = type;
        matchStage = stage;
    }
    
    public void StartMatch()
    {
        matchStatus = "Ongoing";
    }
    
    public void EndMatch(int score1, int score2, string winner)
    {
        matchStatus = "Ended";
        team1Score = score1;
        team2Score = score2;
        winnerID = winner;
    }
}

[System.Serializable]
public class Prize
{
    public string prizeID;
    public string tournamentID;
    public string prizeName;
    public int rank;
    public string prizeType;
    public int prizeAmount;
    public string prizeDescription;
    
    public Prize(string id, string tournament, string name, int rank, string type, int amount, string desc)
    {
        prizeID = id;
        tournamentID = tournament;
        prizeName = name;
        this.rank = rank;
        prizeType = type;
        prizeAmount = amount;
        prizeDescription = desc;
    }
}

[System.Serializable]
public class TournamentManagerData
{
    public TournamentSystemExtended system;
    
    public TournamentManagerData()
    {
        system = new TournamentSystemExtended("tournament_system_extended", "赛事系统扩展", "提供KPL职业联赛、城市赛、高校赛等多种赛事类型的深度实现");
    }
}
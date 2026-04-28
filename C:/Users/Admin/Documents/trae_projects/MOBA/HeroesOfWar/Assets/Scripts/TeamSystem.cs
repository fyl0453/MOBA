[System.Serializable]
public class Team
{
    public string teamID;
    public string teamName;
    public string captainID;
    public string captainName;
    public string teamLogo;
    public string teamDescription;
    public int memberCount;
    public int maxMembers;
    public int teamLevel;
    public int teamPoints;
    public List<TeamMember> members;
    public List<TeamMatch> matches;
    public string createdAt;
    
    public Team(string id, string name, string captain, string captainName, string logo)
    {
        teamID = id;
        teamName = name;
        this.captainID = captain;
        this.captainName = captainName;
        teamLogo = logo;
        teamDescription = "欢迎加入我们的战队！";
        memberCount = 1;
        maxMembers = 5;
        teamLevel = 1;
        teamPoints = 0;
        members = new List<TeamMember>();
        matches = new List<TeamMatch>();
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd");
        
        // 添加队长到战队
        TeamMember captainMember = new TeamMember(captain, captainName, "Captain");
        members.Add(captainMember);
    }
    
    public void AddMember(string playerID, string playerName, string role = "Member")
    {
        if (memberCount < maxMembers)
        {
            TeamMember member = new TeamMember(playerID, playerName, role);
            members.Add(member);
            memberCount++;
        }
    }
    
    public void RemoveMember(string playerID)
    {
        members.RemoveAll(m => m.playerID == playerID);
        memberCount = members.Count;
        
        if (playerID == captainID && members.Count > 0)
        {
            // 转让队长
            captainID = members[0].playerID;
            captainName = members[0].playerName;
            members[0].role = "Captain";
        }
    }
    
    public void PromoteMember(string playerID, string role)
    {
        TeamMember member = members.Find(m => m.playerID == playerID);
        if (member != null)
        {
            member.role = role;
        }
    }
    
    public void AddTeamPoints(int points)
    {
        teamPoints += points;
        UpdateTeamLevel();
    }
    
    private void UpdateTeamLevel()
    {
        if (teamPoints >= 1000)
        {
            teamLevel = 5;
            maxMembers = 7;
        }
        else if (teamPoints >= 800)
        {
            teamLevel = 4;
            maxMembers = 6;
        }
        else if (teamPoints >= 600)
        {
            teamLevel = 3;
            maxMembers = 6;
        }
        else if (teamPoints >= 300)
        {
            teamLevel = 2;
            maxMembers = 5;
        }
        else
        {
            teamLevel = 1;
            maxMembers = 5;
        }
    }
    
    public void AddMatch(TeamMatch match)
    {
        matches.Add(match);
    }
}

[System.Serializable]
public class TeamMember
{
    public string playerID;
    public string playerName;
    public string role;
    public int contribution;
    public string joinDate;
    
    public TeamMember(string id, string name, string role)
    {
        playerID = id;
        playerName = name;
        this.role = role;
        contribution = 0;
        joinDate = System.DateTime.Now.ToString("yyyy-MM-dd");
    }
    
    public void AddContribution(int amount)
    {
        contribution += amount;
    }
}

[System.Serializable]
public class TeamMatch
{
    public string matchID;
    public string opponentTeamID;
    public string opponentTeamName;
    public string matchResult;
    public int teamScore;
    public int opponentScore;
    public string matchDate;
    
    public TeamMatch(string id, string opponent, string opponentName, string result, int score, int opponentScore)
    {
        matchID = id;
        opponentTeamID = opponent;
        opponentTeamName = opponentName;
        matchResult = result;
        this.teamScore = score;
        this.opponentScore = opponentScore;
        matchDate = System.DateTime.Now.ToString("yyyy-MM-dd");
    }
}

[System.Serializable]
public class TeamManagerData
{
    public List<Team> teams;
    
    public TeamManagerData()
    {
        teams = new List<Team>();
    }
    
    public void AddTeam(Team team)
    {
        teams.Add(team);
    }
    
    public Team GetTeam(string teamID)
    {
        return teams.Find(t => t.teamID == teamID);
    }
    
    public List<Team> GetTeamsByPlayer(string playerID)
    {
        return teams.FindAll(t => t.members.Exists(m => m.playerID == playerID));
    }
    
    public Team GetPlayerTeam(string playerID)
    {
        return teams.Find(t => t.members.Exists(m => m.playerID == playerID));
    }
}
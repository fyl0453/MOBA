using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Team
{
    public string TeamID;
    public string TeamName;
    public string TeamDescription;
    public string LeaderID;
    public string LeaderName;
    public List<string> MemberIDs;
    public List<string> OfficerIDs;
    public int TeamLevel;
    public int TeamExp;
    public int TotalMembers;
    public string TeamTag;
    public string TeamIcon;
    public int TeamScore;
    public int WeeklyScore;
    public DateTime CreateTime;
    public DateTime LastActivityTime;
    public bool IsRecruiting;
    public int RequiredLevel;
    public int RequiredScore;

    public Team(string teamID, string teamName, string leaderID, string leaderName)
    {
        TeamID = teamID;
        TeamName = teamName;
        LeaderID = leaderID;
        LeaderName = leaderName;
        TeamDescription = "";
        MemberIDs = new List<string>();
        OfficerIDs = new List<string>();
        TeamLevel = 1;
        TeamExp = 0;
        TotalMembers = 1;
        TeamTag = "";
        TeamIcon = "";
        TeamScore = 0;
        WeeklyScore = 0;
        CreateTime = DateTime.Now;
        LastActivityTime = DateTime.Now;
        IsRecruiting = true;
        RequiredLevel = 1;
        RequiredScore = 0;
    }

    public void AddMember(string memberID)
    {
        if (!MemberIDs.Contains(memberID))
        {
            MemberIDs.Add(memberID);
            TotalMembers = MemberIDs.Count;
        }
    }

    public void RemoveMember(string memberID)
    {
        MemberIDs.Remove(memberID);
        OfficerIDs.Remove(memberID);
        TotalMembers = MemberIDs.Count;
    }

    public bool IsFull()
    {
        int maxMembers = 10 + TeamLevel * 5;
        return TotalMembers >= maxMembers;
    }

    public bool IsLeader(string playerID)
    {
        return LeaderID == playerID;
    }

    public bool IsOfficer(string playerID)
    {
        return OfficerIDs.Contains(playerID);
    }

    public bool IsMember(string playerID)
    {
        return MemberIDs.Contains(playerID);
    }
}

[Serializable]
public class TeamMember
{
    public string PlayerID;
    public string PlayerName;
    public string TeamID;
    public int MemberLevel;
    public int Contribution;
    public int WeeklyContribution;
    public int Title;
    public DateTime JoinTime;
    public DateTime LastActiveTime;
    public int TotalMatches;
    public int WinMatches;
    public double WinRate;

    public TeamMember(string playerID, string playerName, string teamID)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        TeamID = teamID;
        MemberLevel = 1;
        Contribution = 0;
        WeeklyContribution = 0;
        Title = 0;
        JoinTime = DateTime.Now;
        LastActiveTime = DateTime.Now;
        TotalMatches = 0;
        WinMatches = 0;
        WinRate = 0.0;
    }
}

[Serializable]
public class TeamApplication
{
    public string ApplicationID;
    public string PlayerID;
    public string PlayerName;
    public string TeamID;
    public string Message;
    public DateTime ApplyTime;
    public int ApplicationStatus;
    public int PlayerLevel;
    public int PlayerScore;

    public TeamApplication(string applicationID, string playerID, string playerName, string teamID, string message)
    {
        ApplicationID = applicationID;
        PlayerID = playerID;
        PlayerName = playerName;
        TeamID = teamID;
        Message = message;
        ApplyTime = DateTime.Now;
        ApplicationStatus = 0;
        PlayerLevel = 1;
        PlayerScore = 0;
    }
}

[Serializable]
public class TeamInvite
{
    public string InviteID;
    public string TeamID;
    public string TeamName;
    public string InviterID;
    public string InviteeID;
    public DateTime InviteTime;
    public int InviteStatus;

    public TeamInvite(string inviteID, string teamID, string teamName, string inviterID, string inviteeID)
    {
        InviteID = inviteID;
        TeamID = teamID;
        TeamName = teamName;
        InviterID = inviterID;
        InviteeID = inviteeID;
        InviteTime = DateTime.Now;
        InviteStatus = 0;
    }
}

[Serializable]
public class TeamMatch
{
    public string MatchID;
    public string TeamID;
    public string OpponentTeamID;
    public string MatchResult;
    public int TeamScoreChange;
    public DateTime MatchTime;
    public List<string> ParticipantIDs;

    public TeamMatch(string matchID, string teamID, string opponentTeamID, string matchResult, int scoreChange)
    {
        MatchID = matchID;
        TeamID = teamID;
        OpponentTeamID = opponentTeamID;
        MatchResult = matchResult;
        TeamScoreChange = scoreChange;
        MatchTime = DateTime.Now;
        ParticipantIDs = new List<string>();
    }
}

[Serializable]
public class TeamSystemData
{
    public List<Team> AllTeams;
    public Dictionary<string, TeamMember> TeamMembers;
    public List<TeamApplication> Applications;
    public List<TeamInvite> Invites;
    public List<TeamMatch> TeamMatches;
    public Dictionary<string, List<string>> PlayerTeamHistory;
    public DateTime LastCleanupTime;

    public TeamSystemData()
    {
        AllTeams = new List<Team>();
        TeamMembers = new Dictionary<string, TeamMember>();
        Applications = new List<TeamApplication>();
        Invites = new List<TeamInvite>();
        TeamMatches = new List<TeamMatch>();
        PlayerTeamHistory = new Dictionary<string, List<string>>();
        LastCleanupTime = DateTime.Now;
    }

    public void AddTeam(Team team)
    {
        AllTeams.Add(team);
    }

    public void AddTeamMember(TeamMember member)
    {
        TeamMembers[member.PlayerID] = member;
    }

    public void AddApplication(TeamApplication application)
    {
        Applications.Add(application);
    }

    public void AddInvite(TeamInvite invite)
    {
        Invites.Add(invite);
    }

    public void AddTeamMatch(TeamMatch match)
    {
        TeamMatches.Add(match);
    }
}

[Serializable]
public class TeamEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string TeamID;
    public string PlayerID;
    public string EventData;

    public TeamEvent(string eventID, string eventType, string teamID, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        TeamID = teamID;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class TeamSystemDataManager
{
    private static TeamSystemDataManager _instance;
    public static TeamSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TeamSystemDataManager();
            }
            return _instance;
        }
    }

    public TeamSystemData teamData;
    private List<TeamEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private TeamSystemDataManager()
    {
        teamData = new TeamSystemData();
        recentEvents = new List<TeamEvent>();
        LoadTeamData();
    }

    public void SaveTeamData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TeamSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, teamData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存战队系统数据失败: " + e.Message);
        }
    }

    public void LoadTeamData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TeamSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    teamData = (TeamSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载战队系统数据失败: " + e.Message);
            teamData = new TeamSystemData();
        }
    }

    public void CreateTeamEvent(string eventType, string teamID, string playerID, string eventData)
    {
        string eventID = "team_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TeamEvent teamEvent = new TeamEvent(eventID, eventType, teamID, playerID, eventData);
        recentEvents.Add(teamEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<TeamEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
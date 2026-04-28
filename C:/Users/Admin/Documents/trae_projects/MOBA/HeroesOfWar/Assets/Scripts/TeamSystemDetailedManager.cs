using System;
using System.Collections.Generic;

public class TeamSystemDetailedManager
{
    private static TeamSystemDetailedManager _instance;
    public static TeamSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TeamSystemDetailedManager();
            }
            return _instance;
        }
    }

    private TeamSystemData teamData;
    private TeamSystemDataManager dataManager;

    private TeamSystemDetailedManager()
    {
        dataManager = TeamSystemDataManager.Instance;
        teamData = dataManager.teamData;
    }

    public void CreateTeam(string teamName, string leaderID, string leaderName, string description = "", string teamTag = "")
    {
        string teamID = "team_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Team team = new Team(teamID, teamName, leaderID, leaderName);
        team.TeamDescription = description;
        team.TeamTag = teamTag;
        team.AddMember(leaderID);
        teamData.AddTeam(team);

        TeamMember leaderMember = new TeamMember(leaderID, leaderName, teamID);
        leaderMember.Title = 2;
        teamData.AddTeamMember(leaderMember);

        dataManager.CreateTeamEvent("team_create", teamID, leaderID, "创建战队: " + teamName);
        dataManager.SaveTeamData();
        Debug.Log("创建战队成功: " + teamName);
    }

    public void DisbandTeam(string teamID)
    {
        Team team = teamData.AllTeams.Find(t => t.TeamID == teamID);
        if (team == null)
        {
            Debug.LogError("战队不存在: " + teamID);
            return;
        }

        foreach (string memberID in team.MemberIDs)
        {
            if (teamData.TeamMembers.ContainsKey(memberID))
            {
                teamData.TeamMembers.Remove(memberID);
            }
        }

        teamData.Applications.RemoveAll(a => a.TeamID == teamID);
        teamData.Invites.RemoveAll(i => i.TeamID == teamID);
        teamData.AllTeams.Remove(team);

        dataManager.CreateTeamEvent("team_disband", teamID, "", "解散战队: " + team.TeamName);
        dataManager.SaveTeamData();
        Debug.Log("解散战队: " + team.TeamName);
    }

    public Team GetTeam(string teamID)
    {
        return teamData.AllTeams.Find(t => t.TeamID == teamID);
    }

    public TeamMember GetTeamMember(string playerID)
    {
        if (teamData.TeamMembers.ContainsKey(playerID))
        {
            return teamData.TeamMembers[playerID];
        }
        return null;
    }

    public List<Team> GetRecruitingTeams()
    {
        return teamData.AllTeams.FindAll(t => t.IsRecruiting);
    }

    public List<Team> SearchTeams(string keyword)
    {
        return teamData.AllTeams.FindAll(t =>
            t.TeamName.Contains(keyword) ||
            t.TeamTag.Contains(keyword));
    }

    public List<Team> GetTeamsByLevel(int minLevel)
    {
        return teamData.AllTeams.FindAll(t => t.TeamLevel >= minLevel);
    }

    public void ApplyToTeam(string playerID, string playerName, string teamID, string message = "")
    {
        Team team = teamData.AllTeams.Find(t => t.TeamID == teamID);
        if (team == null)
        {
            Debug.LogError("战队不存在: " + teamID);
            return;
        }

        if (team.IsFull())
        {
            Debug.LogWarning("战队已满");
            return;
        }

        if (IsInTeam(playerID))
        {
            Debug.LogWarning("已在战队中");
            return;
        }

        var existingApplication = teamData.Applications.Find(a => a.PlayerID == playerID && a.TeamID == teamID && a.ApplicationStatus == 0);
        if (existingApplication != null)
        {
            Debug.LogWarning("已申请过该战队");
            return;
        }

        string applicationID = "app_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TeamApplication application = new TeamApplication(applicationID, playerID, playerName, teamID, message);
        teamData.AddApplication(application);

        dataManager.CreateTeamEvent("application_create", teamID, playerID, "申请加入: " + team.TeamName);
        dataManager.SaveTeamData();
        Debug.Log("申请加入战队成功");
    }

    public void ProcessApplication(string applicationID, bool accepted)
    {
        TeamApplication application = teamData.Applications.Find(a => a.ApplicationID == applicationID);
        if (application == null)
        {
            Debug.LogError("申请不存在: " + applicationID);
            return;
        }

        application.ApplicationStatus = accepted ? 1 : 2;

        if (accepted)
        {
            Team team = teamData.AllTeams.Find(t => t.TeamID == application.TeamID);
            if (team != null)
            {
                team.AddMember(application.PlayerID);
                TeamMember member = new TeamMember(application.PlayerID, application.PlayerName, application.TeamID);
                teamData.AddTeamMember(member);

                dataManager.CreateTeamEvent("member_join", application.TeamID, application.PlayerID, "成员加入: " + application.PlayerName);
            }
        }

        dataManager.SaveTeamData();
        Debug.Log("处理申请: " + (accepted ? "同意" : "拒绝"));
    }

    public void InvitePlayer(string teamID, string inviterID, string inviteeID)
    {
        Team team = teamData.AllTeams.Find(t => t.TeamID == teamID);
        if (team == null)
        {
            Debug.LogError("战队不存在: " + teamID);
            return;
        }

        if (IsInTeam(inviteeID))
        {
            Debug.LogWarning("玩家已在战队中");
            return;
        }

        var existingInvite = teamData.Invites.Find(i => i.TeamID == teamID && i.InviteeID == inviteeID && i.InviteStatus == 0);
        if (existingInvite != null)
        {
            Debug.LogWarning("已邀请过该玩家");
            return;
        }

        string inviteID = "invite_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TeamInvite invite = new TeamInvite(inviteID, teamID, team.TeamName, inviterID, inviteeID);
        teamData.AddInvite(invite);

        dataManager.CreateTeamEvent("invite_send", teamID, inviterID, "邀请玩家: " + inviteeID);
        dataManager.SaveTeamData();
        Debug.Log("发送邀请成功");
    }

    public void ProcessInvite(string inviteID, bool accepted)
    {
        TeamInvite invite = teamData.Invites.Find(i => i.InviteID == inviteID);
        if (invite == null)
        {
            Debug.LogError("邀请不存在: " + inviteID);
            return;
        }

        invite.InviteStatus = accepted ? 1 : 2;

        if (accepted)
        {
            Team team = teamData.AllTeams.Find(t => t.TeamID == invite.TeamID);
            if (team != null)
            {
                TeamMember member = teamData.TeamMembers.ContainsKey(invite.InviteeID) ?
                    teamData.TeamMembers[invite.InviteeID] : null;
                string playerName = member != null ? member.PlayerName : invite.InviteeID;
                team.AddMember(invite.InviteeID);
                if (teamData.TeamMembers.ContainsKey(invite.InviteeID))
                {
                    teamData.TeamMembers[invite.InviteeID].TeamID = invite.TeamID;
                }
                else
                {
                    TeamMember newMember = new TeamMember(invite.InviteeID, playerName, invite.TeamID);
                    teamData.AddTeamMember(newMember);
                }

                dataManager.CreateTeamEvent("member_join", invite.TeamID, invite.InviteeID, "成员通过邀请加入: " + playerName);
            }
        }

        dataManager.SaveTeamData();
        Debug.Log("处理邀请: " + (accepted ? "接受" : "拒绝"));
    }

    public void LeaveTeam(string playerID)
    {
        TeamMember member = teamData.TeamMembers.ContainsKey(playerID) ? teamData.TeamMembers[playerID] : null;
        if (member == null)
        {
            Debug.LogError("成员不存在");
            return;
        }

        Team team = teamData.AllTeams.Find(t => t.TeamID == member.TeamID);
        if (team == null)
        {
            Debug.LogError("战队不存在");
            return;
        }

        if (team.IsLeader(playerID))
        {
            Debug.LogWarning("队长无法直接离开，请转让队长或解散战队");
            return;
        }

        team.RemoveMember(playerID);
        teamData.TeamMembers.Remove(playerID);

        dataManager.CreateTeamEvent("member_leave", member.TeamID, playerID, "离开战队");
        dataManager.SaveTeamData();
        Debug.Log("离开战队成功");
    }

    public void KickMember(string teamID, string leaderID, string memberID)
    {
        Team team = teamData.AllTeams.Find(t => t.TeamID == teamID);
        if (team == null)
        {
            Debug.LogError("战队不存在: " + teamID);
            return;
        }

        if (!team.IsLeader(leaderID))
        {
            Debug.LogWarning("只有队长可以踢人");
            return;
        }

        if (team.IsLeader(memberID))
        {
            Debug.LogWarning("无法踢出队长");
            return;
        }

        team.RemoveMember(memberID);
        if (teamData.TeamMembers.ContainsKey(memberID))
        {
            teamData.TeamMembers.Remove(memberID);
        }

        dataManager.CreateTeamEvent("member_kick", teamID, memberID, "踢出成员");
        dataManager.SaveTeamData();
        Debug.Log("踢出成员成功");
    }

    public void SetOfficer(string teamID, string leaderID, string memberID)
    {
        Team team = teamData.AllTeams.Find(t => t.TeamID == teamID);
        if (team == null)
        {
            Debug.LogError("战队不存在: " + teamID);
            return;
        }

        if (!team.IsLeader(leaderID))
        {
            Debug.LogWarning("只有队长可以设置官员");
            return;
        }

        if (!team.IsMember(memberID))
        {
            Debug.LogWarning("该玩家不是成员");
            return;
        }

        if (!team.OfficerIDs.Contains(memberID))
        {
            team.OfficerIDs.Add(memberID);
            if (teamData.TeamMembers.ContainsKey(memberID))
            {
                teamData.TeamMembers[memberID].Title = 1;
            }
        }

        dataManager.CreateTeamEvent("officer_set", teamID, memberID, "设置官员");
        dataManager.SaveTeamData();
        Debug.Log("设置官员成功");
    }

    public void RemoveOfficer(string teamID, string leaderID, string memberID)
    {
        Team team = teamData.AllTeams.Find(t => t.TeamID == teamID);
        if (team == null)
        {
            Debug.LogError("战队不存在: " + teamID);
            return;
        }

        if (!team.IsLeader(leaderID))
        {
            Debug.LogWarning("只有队长可以移除官员");
            return;
        }

        team.OfficerIDs.Remove(memberID);
        if (teamData.TeamMembers.ContainsKey(memberID))
        {
            teamData.TeamMembers[memberID].Title = 0;
        }

        dataManager.SaveTeamData();
        Debug.Log("移除官员成功");
    }

    public void TransferLeader(string teamID, string currentLeaderID, string newLeaderID)
    {
        Team team = teamData.AllTeams.Find(t => t.TeamID == teamID);
        if (team == null)
        {
            Debug.LogError("战队不存在: " + teamID);
            return;
        }

        if (!team.IsLeader(currentLeaderID))
        {
            Debug.LogWarning("只有队长可以转让");
            return;
        }

        if (!team.IsMember(newLeaderID))
        {
            Debug.LogWarning("新队长必须是成员");
            return;
        }

        team.LeaderID = newLeaderID;
        if (teamData.TeamMembers.ContainsKey(currentLeaderID))
        {
            teamData.TeamMembers[currentLeaderID].Title = 1;
        }
        if (teamData.TeamMembers.ContainsKey(newLeaderID))
        {
            teamData.TeamMembers[newLeaderID].Title = 2;
        }

        team.OfficerIDs.Remove(newLeaderID);

        dataManager.CreateTeamEvent("leader_transfer", teamID, newLeaderID, "转让队长");
        dataManager.SaveTeamData();
        Debug.Log("转让队长成功");
    }

    public bool IsInTeam(string playerID)
    {
        return teamData.TeamMembers.ContainsKey(playerID);
    }

    public List<TeamApplication> GetTeamApplications(string teamID)
    {
        return teamData.Applications.FindAll(a => a.TeamID == teamID && a.ApplicationStatus == 0);
    }

    public List<TeamInvite> GetPlayerInvites(string playerID)
    {
        return teamData.Invites.FindAll(i => i.InviteeID == playerID && i.InviteStatus == 0);
    }

    public List<TeamMember> GetTeamMembers(string teamID)
    {
        List<TeamMember> members = new List<TeamMember>();
        foreach (var kvp in teamData.TeamMembers)
        {
            if (kvp.Value.TeamID == teamID)
            {
                members.Add(kvp.Value);
            }
        }
        members.Sort((a, b) => b.Title.CompareTo(a.Title));
        return members;
    }

    public void UpdateTeamSettings(string teamID, string leaderID, bool isRecruiting, int requiredLevel, int requiredScore)
    {
        Team team = teamData.AllTeams.Find(t => t.TeamID == teamID);
        if (team == null)
        {
            Debug.LogError("战队不存在: " + teamID);
            return;
        }

        if (!team.IsLeader(leaderID))
        {
            Debug.LogWarning("只有队长可以修改设置");
            return;
        }

        team.IsRecruiting = isRecruiting;
        team.RequiredLevel = requiredLevel;
        team.RequiredScore = requiredScore;
        dataManager.SaveTeamData();
        Debug.Log("更新战队设置成功");
    }

    public void UpdateMemberContribution(string playerID, int contribution)
    {
        if (teamData.TeamMembers.ContainsKey(playerID))
        {
            teamData.TeamMembers[playerID].Contribution += contribution;
            teamData.TeamMembers[playerID].WeeklyContribution += contribution;
            dataManager.SaveTeamData();
        }
    }

    public void UpdateTeamScore(string teamID, int scoreChange)
    {
        Team team = teamData.AllTeams.Find(t => t.TeamID == teamID);
        if (team != null)
        {
            team.TeamScore += scoreChange;
            team.WeeklyScore += scoreChange;
            dataManager.SaveTeamData();
        }
    }

    public void ResetWeeklyScores()
    {
        foreach (Team team in teamData.AllTeams)
        {
            team.WeeklyScore = 0;
        }
        foreach (var kvp in teamData.TeamMembers)
        {
            kvp.Value.WeeklyContribution = 0;
        }
        dataManager.SaveTeamData();
        Debug.Log("重置周分数成功");
    }

    public List<Team> GetTopTeams(int count)
    {
        List<Team> teams = new List<Team>(teamData.AllTeams);
        teams.Sort((a, b) => b.TeamScore.CompareTo(a.TeamScore));
        if (count < teams.Count)
        {
            return teams.GetRange(0, count);
        }
        return teams;
    }

    public List<TeamEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveTeamData();
    }

    public void LoadData()
    {
        dataManager.LoadTeamData();
    }
}
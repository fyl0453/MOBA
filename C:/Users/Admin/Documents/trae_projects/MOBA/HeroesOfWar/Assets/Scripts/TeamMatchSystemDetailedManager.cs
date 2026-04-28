using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamMatchSystemDetailedManager
{
    private static TeamMatchSystemDetailedManager _instance;
    public static TeamMatchSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TeamMatchSystemDetailedManager();
            }
            return _instance;
        }
    }

    private TeamMatchSystemData teamMatchData;
    private TeamMatchSystemDataManager dataManager;

    private TeamMatchSystemDetailedManager()
    {
        dataManager = TeamMatchSystemDataManager.Instance;
        teamMatchData = dataManager.teamMatchData;
    }

    public string CreateTeamMatch(string team1ID, string team1Name, string team2ID, string team2Name, string matchType, string mapName)
    {
        TeamMatch match = new TeamMatch(team1ID, team1Name, team2ID, team2Name, matchType, mapName);
        teamMatchData.AddTeamMatch(match);
        
        dataManager.CreateTeamMatchEvent("match_create", team1ID, match.MatchID, "创建战队赛: " + team1Name + " vs " + team2Name);
        dataManager.CreateTeamMatchEvent("match_create", team2ID, match.MatchID, "创建战队赛: " + team2Name + " vs " + team1Name);
        dataManager.SaveTeamMatchData();
        Debug.Log("创建战队赛成功: " + team1Name + " vs " + team2Name);
        return match.MatchID;
    }

    public string AddMatchParticipant(string matchID, string teamID, string playerID, string playerName, string heroID, string heroName)
    {
        TeamMatch match = teamMatchData.AllTeamMatches.Find(m => m.MatchID == matchID);
        if (match == null)
        {
            Debug.LogError("战队赛不存在: " + matchID);
            return "";
        }
        
        TeamMatchParticipant participant = new TeamMatchParticipant(matchID, teamID, playerID, playerName, heroID, heroName);
        teamMatchData.AddParticipant(participant);
        
        dataManager.CreateTeamMatchEvent("participant_add", teamID, matchID, "添加参赛选手: " + playerName);
        dataManager.SaveTeamMatchData();
        Debug.Log("添加参赛选手成功: " + playerName);
        return participant.ParticipantID;
    }

    public void UpdateMatchParticipantStats(string participantID, int kills, int deaths, int assists, int damageDealt, bool isMVP)
    {
        TeamMatchParticipant participant = teamMatchData.AllParticipants.Find(p => p.ParticipantID == participantID);
        if (participant != null)
        {
            participant.Kills = kills;
            participant.Deaths = deaths;
            participant.Assists = assists;
            participant.DamageDealt = damageDealt;
            participant.IsMVP = isMVP;
            
            dataManager.SaveTeamMatchData();
            Debug.Log("更新参赛选手数据成功: " + participant.PlayerName);
        }
    }

    public void StartMatch(string matchID)
    {
        TeamMatch match = teamMatchData.AllTeamMatches.Find(m => m.MatchID == matchID);
        if (match != null && match.MatchStatus == "scheduled")
        {
            match.MatchStatus = "ongoing";
            
            dataManager.CreateTeamMatchEvent("match_start", match.Team1ID, matchID, "开始战队赛: " + match.Team1Name + " vs " + match.Team2Name);
            dataManager.CreateTeamMatchEvent("match_start", match.Team2ID, matchID, "开始战队赛: " + match.Team2Name + " vs " + match.Team1Name);
            dataManager.SaveTeamMatchData();
            Debug.Log("开始战队赛成功: " + match.Team1Name + " vs " + match.Team2Name);
        }
    }

    public void EndMatch(string matchID, int team1Score, int team2Score, string winnerTeamID, float matchDuration, string matchVideoURL)
    {
        TeamMatch match = teamMatchData.AllTeamMatches.Find(m => m.MatchID == matchID);
        if (match != null && match.MatchStatus == "ongoing")
        {
            match.MatchStatus = "completed";
            match.Team1Score = team1Score;
            match.Team2Score = team2Score;
            match.WinnerTeamID = winnerTeamID;
            match.MatchDuration = matchDuration;
            match.MatchEndTime = DateTime.Now;
            match.MatchVideoURL = matchVideoURL;
            match.MatchResult = winnerTeamID == match.Team1ID ? match.Team1Name + " 获胜" : match.Team2Name + " 获胜";
            
            UpdateTeamRanks(match.Team1ID, match.Team2ID, winnerTeamID);
            CreateMatchRewards(matchID, match.Team1ID, match.Team2ID, winnerTeamID);
            
            dataManager.CreateTeamMatchEvent("match_end", match.Team1ID, matchID, "结束战队赛: " + match.MatchResult);
            dataManager.CreateTeamMatchEvent("match_end", match.Team2ID, matchID, "结束战队赛: " + match.MatchResult);
            dataManager.SaveTeamMatchData();
            Debug.Log("结束战队赛成功: " + match.MatchResult);
        }
    }

    public string CreateTeamMatchSeason(string seasonName, DateTime startTime, DateTime endTime)
    {
        TeamMatchSeason season = new TeamMatchSeason(seasonName, startTime, endTime);
        teamMatchData.AddSeason(season);
        
        dataManager.CreateTeamMatchEvent("season_create", "system", "", "创建战队赛赛季: " + seasonName);
        dataManager.SaveTeamMatchData();
        Debug.Log("创建战队赛赛季成功: " + seasonName);
        return season.SeasonID;
    }

    public void StartSeason(string seasonID)
    {
        TeamMatchSeason season = teamMatchData.AllSeasons.Find(s => s.SeasonID == seasonID);
        if (season != null && season.SeasonStatus == "upcoming")
        {
            season.SeasonStatus = "ongoing";
            
            dataManager.CreateTeamMatchEvent("season_start", "system", "", "开始战队赛赛季: " + season.SeasonName);
            dataManager.SaveTeamMatchData();
            Debug.Log("开始战队赛赛季成功: " + season.SeasonName);
        }
    }

    public void EndSeason(string seasonID)
    {
        TeamMatchSeason season = teamMatchData.AllSeasons.Find(s => s.SeasonID == seasonID);
        if (season != null && season.SeasonStatus == "ongoing")
        {
            season.SeasonStatus = "completed";
            
            dataManager.CreateTeamMatchEvent("season_end", "system", "", "结束战队赛赛季: " + season.SeasonName);
            dataManager.SaveTeamMatchData();
            Debug.Log("结束战队赛赛季成功: " + season.SeasonName);
        }
    }

    public void AddTeamToSeason(string seasonID, string teamID, string teamName)
    {
        TeamMatchSeason season = teamMatchData.AllSeasons.Find(s => s.SeasonID == seasonID);
        if (season != null && !season.ParticipatingTeams.Contains(teamID))
        {
            season.ParticipatingTeams.Add(teamID);
            
            TeamRank rank = new TeamRank(teamID, teamName, seasonID);
            season.TeamRanks.Add(rank);
            teamMatchData.AddTeamRank(rank);
            
            dataManager.CreateTeamMatchEvent("team_add_season", teamID, "", "添加战队到赛季: " + season.SeasonName);
            dataManager.SaveTeamMatchData();
            Debug.Log("添加战队到赛季成功: " + teamName + " -> " + season.SeasonName);
        }
    }

    public bool ClaimMatchReward(string rewardID, string teamID)
    {
        TeamMatchReward reward = teamMatchData.AllRewards.Find(r => r.RewardID == rewardID && r.TeamID == teamID);
        if (reward != null && !reward.IsClaimed)
        {
            reward.IsClaimed = true;
            reward.ClaimTime = DateTime.Now;
            
            dataManager.CreateTeamMatchEvent("reward_claim", teamID, reward.MatchID, "领取战队赛奖励: " + reward.RewardName);
            dataManager.SaveTeamMatchData();
            Debug.Log("领取战队赛奖励成功: " + reward.RewardName);
            return true;
        }
        return false;
    }

    private void UpdateTeamRanks(string team1ID, string team2ID, string winnerTeamID)
    {
        UpdateTeamRank(team1ID, winnerTeamID == team1ID);
        UpdateTeamRank(team2ID, winnerTeamID == team2ID);
    }

    private void UpdateTeamRank(string teamID, bool isWinner)
    {
        if (!teamMatchData.TeamRanks.ContainsKey(teamID))
        {
            TeamRank rank = new TeamRank(teamID, "", "");
            teamMatchData.AddTeamRank(rank);
        }
        
        TeamRank rankData = teamMatchData.TeamRanks[teamID];
        rankData.MatchesPlayed++;
        if (isWinner)
        {
            rankData.Wins++;
            rankData.Points += 3;
        }
        else
        {
            rankData.Losses++;
            rankData.Points += 1;
        }
        rankData.WinRate = (float)rankData.Wins / rankData.MatchesPlayed;
        rankData.LastMatchTime = DateTime.Now;
    }

    private void CreateMatchRewards(string matchID, string team1ID, string team2ID, string winnerTeamID)
    {
        if (winnerTeamID == team1ID)
        {
            TeamMatchReward reward1 = new TeamMatchReward(matchID, team1ID, "gold", "胜利奖励", 1000, "战队赛胜利奖励");
            teamMatchData.AddReward(reward1);
            
            TeamMatchReward reward2 = new TeamMatchReward(matchID, team2ID, "gold", "参与奖励", 500, "战队赛参与奖励");
            teamMatchData.AddReward(reward2);
        }
        else
        {
            TeamMatchReward reward1 = new TeamMatchReward(matchID, team2ID, "gold", "胜利奖励", 1000, "战队赛胜利奖励");
            teamMatchData.AddReward(reward1);
            
            TeamMatchReward reward2 = new TeamMatchReward(matchID, team1ID, "gold", "参与奖励", 500, "战队赛参与奖励");
            teamMatchData.AddReward(reward2);
        }
    }

    public TeamMatch GetTeamMatch(string matchID)
    {
        return teamMatchData.AllTeamMatches.Find(m => m.MatchID == matchID);
    }

    public List<TeamMatch> GetTeamMatches(string teamID)
    {
        if (teamMatchData.TeamMatchHistory.ContainsKey(teamID))
        {
            return teamMatchData.TeamMatchHistory[teamID];
        }
        return new List<TeamMatch>();
    }

    public List<TeamMatch> GetUpcomingMatches()
    {
        return teamMatchData.AllTeamMatches.FindAll(m => m.MatchStatus == "scheduled");
    }

    public List<TeamMatch> GetOngoingMatches()
    {
        return teamMatchData.AllTeamMatches.FindAll(m => m.MatchStatus == "ongoing");
    }

    public List<TeamMatch> GetCompletedMatches()
    {
        return teamMatchData.AllTeamMatches.FindAll(m => m.MatchStatus == "completed");
    }

    public List<TeamMatchParticipant> GetMatchParticipants(string matchID)
    {
        return teamMatchData.AllParticipants.FindAll(p => p.MatchID == matchID);
    }

    public List<TeamMatchReward> GetMatchRewards(string matchID, string teamID)
    {
        return teamMatchData.AllRewards.FindAll(r => r.MatchID == matchID && r.TeamID == teamID);
    }

    public List<TeamMatchSeason> GetAllSeasons()
    {
        return teamMatchData.AllSeasons;
    }

    public TeamMatchSeason GetSeason(string seasonID)
    {
        return teamMatchData.AllSeasons.Find(s => s.SeasonID == seasonID);
    }

    public List<TeamRank> GetTeamRanks(string seasonID)
    {
        TeamMatchSeason season = teamMatchData.AllSeasons.Find(s => s.SeasonID == seasonID);
        if (season != null)
        {
            season.TeamRanks.Sort((a, b) => b.Points.CompareTo(a.Points));
            for (int i = 0; i < season.TeamRanks.Count; i++)
            {
                season.TeamRanks[i].Rank = i + 1;
            }
            return season.TeamRanks;
        }
        return new List<TeamRank>();
    }

    public TeamRank GetTeamRank(string teamID)
    {
        if (teamMatchData.TeamRanks.ContainsKey(teamID))
        {
            return teamMatchData.TeamRanks[teamID];
        }
        return null;
    }

    public List<string> GetMatchTypes()
    {
        return teamMatchData.MatchTypes;
    }

    public List<string> GetMaps()
    {
        return teamMatchData.Maps;
    }

    public void AddMatchType(string matchType)
    {
        if (!teamMatchData.MatchTypes.Contains(matchType))
        {
            teamMatchData.MatchTypes.Add(matchType);
            dataManager.SaveTeamMatchData();
            Debug.Log("添加比赛类型成功: " + matchType);
        }
    }

    public void RemoveMatchType(string matchType)
    {
        if (teamMatchData.MatchTypes.Contains(matchType))
        {
            teamMatchData.MatchTypes.Remove(matchType);
            dataManager.SaveTeamMatchData();
            Debug.Log("删除比赛类型成功: " + matchType);
        }
    }

    public void AddMap(string mapName)
    {
        if (!teamMatchData.Maps.Contains(mapName))
        {
            teamMatchData.Maps.Add(mapName);
            dataManager.SaveTeamMatchData();
            Debug.Log("添加地图成功: " + mapName);
        }
    }

    public void RemoveMap(string mapName)
    {
        if (teamMatchData.Maps.Contains(mapName))
        {
            teamMatchData.Maps.Remove(mapName);
            dataManager.SaveTeamMatchData();
            Debug.Log("删除地图成功: " + mapName);
        }
    }

    public void CleanupOldMatches(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<TeamMatch> oldMatches = teamMatchData.AllTeamMatches.FindAll(m => m.MatchEndTime < cutoffDate);
        foreach (TeamMatch match in oldMatches)
        {
            teamMatchData.AllTeamMatches.Remove(match);
            teamMatchData.AllParticipants.RemoveAll(p => p.MatchID == match.MatchID);
            teamMatchData.AllRewards.RemoveAll(r => r.MatchID == match.MatchID);
        }
        
        if (oldMatches.Count > 0)
        {
            dataManager.CreateTeamMatchEvent("match_cleanup", "system", "", "清理旧战队赛: " + oldMatches.Count);
            dataManager.SaveTeamMatchData();
            Debug.Log("清理旧战队赛成功: " + oldMatches.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveTeamMatchData();
    }

    public void LoadData()
    {
        dataManager.LoadTeamMatchData();
    }

    public List<TeamMatchEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
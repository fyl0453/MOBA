using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class TeamMatch
{
    public string MatchID;
    public string Team1ID;
    public string Team1Name;
    public string Team2ID;
    public string Team2Name;
    public string MatchType;
    public string MatchStatus;
    public DateTime MatchTime;
    public string MapName;
    public int Team1Score;
    public int Team2Score;
    public string WinnerTeamID;
    public string MatchResult;
    public string MatchVideoURL;
    public DateTime MatchEndTime;
    public float MatchDuration;

    public TeamMatch(string team1ID, string team1Name, string team2ID, string team2Name, string matchType, string mapName)
    {
        MatchID = "team_match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Team1ID = team1ID;
        Team1Name = team1Name;
        Team2ID = team2ID;
        Team2Name = team2Name;
        MatchType = matchType;
        MatchStatus = "scheduled";
        MatchTime = DateTime.Now;
        MapName = mapName;
        Team1Score = 0;
        Team2Score = 0;
        WinnerTeamID = "";
        MatchResult = "";
        MatchVideoURL = "";
        MatchEndTime = DateTime.MinValue;
        MatchDuration = 0f;
    }
}

[Serializable]
public class TeamMatchParticipant
{
    public string ParticipantID;
    public string MatchID;
    public string TeamID;
    public string PlayerID;
    public string PlayerName;
    public string HeroID;
    public string HeroName;
    public int Kills;
    public int Deaths;
    public int Assists;
    public int DamageDealt;
    public bool IsMVP;

    public TeamMatchParticipant(string matchID, string teamID, string playerID, string playerName, string heroID, string heroName)
    {
        ParticipantID = "participant_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchID = matchID;
        TeamID = teamID;
        PlayerID = playerID;
        PlayerName = playerName;
        HeroID = heroID;
        HeroName = heroName;
        Kills = 0;
        Deaths = 0;
        Assists = 0;
        DamageDealt = 0;
        IsMVP = false;
    }
}

[Serializable]
public class TeamRank
{
    public string TeamID;
    public string TeamName;
    public int Rank;
    public int Points;
    public int MatchesPlayed;
    public int Wins;
    public int Losses;
    public float WinRate;
    public string TeamLogo;
    public DateTime LastMatchTime;
    public string SeasonID;

    public TeamRank(string teamID, string teamName, string seasonID)
    {
        TeamID = teamID;
        TeamName = teamName;
        Rank = 0;
        Points = 0;
        MatchesPlayed = 0;
        Wins = 0;
        Losses = 0;
        WinRate = 0f;
        TeamLogo = "";
        LastMatchTime = DateTime.MinValue;
        SeasonID = seasonID;
    }
}

[Serializable]
public class TeamMatchReward
{
    public string RewardID;
    public string MatchID;
    public string TeamID;
    public string RewardType;
    public string RewardName;
    public int RewardAmount;
    public string RewardDescription;
    public bool IsClaimed;
    public DateTime ClaimTime;

    public TeamMatchReward(string matchID, string teamID, string rewardType, string rewardName, int rewardAmount, string rewardDescription)
    {
        RewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchID = matchID;
        TeamID = teamID;
        RewardType = rewardType;
        RewardName = rewardName;
        RewardAmount = rewardAmount;
        RewardDescription = rewardDescription;
        IsClaimed = false;
        ClaimTime = DateTime.MinValue;
    }
}

[Serializable]
public class TeamMatchSeason
{
    public string SeasonID;
    public string SeasonName;
    public DateTime StartTime;
    public DateTime EndTime;
    public string SeasonStatus;
    public List<string> ParticipatingTeams;
    public List<TeamRank> TeamRanks;
    public string SeasonRewards;

    public TeamMatchSeason(string seasonName, DateTime startTime, DateTime endTime)
    {
        SeasonID = "season_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SeasonName = seasonName;
        StartTime = startTime;
        EndTime = endTime;
        SeasonStatus = "upcoming";
        ParticipatingTeams = new List<string>();
        TeamRanks = new List<TeamRank>();
        SeasonRewards = "";
    }
}

[Serializable]
public class TeamMatchSystemData
{
    public List<TeamMatch> AllTeamMatches;
    public List<TeamMatchParticipant> AllParticipants;
    public List<TeamMatchReward> AllRewards;
    public List<TeamMatchSeason> AllSeasons;
    public Dictionary<string, List<TeamMatch>> TeamMatchHistory;
    public Dictionary<string, TeamRank> TeamRanks;
    public List<string> MatchTypes;
    public List<string> Maps;
    public int MaxMatchesPerSeason;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public TeamMatchSystemData()
    {
        AllTeamMatches = new List<TeamMatch>();
        AllParticipants = new List<TeamMatchParticipant>();
        AllRewards = new List<TeamMatchReward>();
        AllSeasons = new List<TeamMatchSeason>();
        TeamMatchHistory = new Dictionary<string, List<TeamMatch>>();
        TeamRanks = new Dictionary<string, TeamRank>();
        MatchTypes = new List<string> { "friendly", "ranked", "tournament" };
        Maps = new List<string> { "Summoner's Rift", "Howling Abyss" };
        MaxMatchesPerSeason = 50;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddTeamMatch(TeamMatch match)
    {
        AllTeamMatches.Add(match);
        if (!TeamMatchHistory.ContainsKey(match.Team1ID))
        {
            TeamMatchHistory[match.Team1ID] = new List<TeamMatch>();
        }
        TeamMatchHistory[match.Team1ID].Add(match);
        
        if (!TeamMatchHistory.ContainsKey(match.Team2ID))
        {
            TeamMatchHistory[match.Team2ID] = new List<TeamMatch>();
        }
        TeamMatchHistory[match.Team2ID].Add(match);
    }

    public void AddParticipant(TeamMatchParticipant participant)
    {
        AllParticipants.Add(participant);
    }

    public void AddReward(TeamMatchReward reward)
    {
        AllRewards.Add(reward);
    }

    public void AddSeason(TeamMatchSeason season)
    {
        AllSeasons.Add(season);
    }

    public void AddTeamRank(TeamRank rank)
    {
        TeamRanks[rank.TeamID] = rank;
    }
}

[Serializable]
public class TeamMatchEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string TeamID;
    public string MatchID;
    public string EventData;

    public TeamMatchEvent(string eventID, string eventType, string teamID, string matchID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        TeamID = teamID;
        MatchID = matchID;
        EventData = eventData;
    }
}

public class TeamMatchSystemDataManager
{
    private static TeamMatchSystemDataManager _instance;
    public static TeamMatchSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TeamMatchSystemDataManager();
            }
            return _instance;
        }
    }

    public TeamMatchSystemData teamMatchData;
    private List<TeamMatchEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private TeamMatchSystemDataManager()
    {
        teamMatchData = new TeamMatchSystemData();
        recentEvents = new List<TeamMatchEvent>();
        LoadTeamMatchData();
    }

    public void SaveTeamMatchData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TeamMatchSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, teamMatchData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存战队赛系统数据失败: " + e.Message);
        }
    }

    public void LoadTeamMatchData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TeamMatchSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    teamMatchData = (TeamMatchSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载战队赛系统数据失败: " + e.Message);
            teamMatchData = new TeamMatchSystemData();
        }
    }

    public void CreateTeamMatchEvent(string eventType, string teamID, string matchID, string eventData)
    {
        string eventID = "team_match_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TeamMatchEvent teamMatchEvent = new TeamMatchEvent(eventID, eventType, teamID, matchID, eventData);
        recentEvents.Add(teamMatchEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<TeamMatchEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
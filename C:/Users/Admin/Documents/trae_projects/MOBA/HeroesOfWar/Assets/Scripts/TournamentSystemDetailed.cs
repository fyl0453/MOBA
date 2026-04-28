using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Tournament
{
    public string TournamentID;
    public string TournamentName;
    public string TournamentType;
    public string Description;
    public DateTime StartTime;
    public DateTime EndTime;
    public int MaxParticipants;
    public int CurrentParticipants;
    public string Status;
    public string TournamentMode;
    public string TournamentMap;
    public List<string> RewardIDs;

    public Tournament(string tournamentID, string tournamentName, string tournamentType, string description, DateTime startTime, DateTime endTime, int maxParticipants, string tournamentMode, string tournamentMap)
    {
        TournamentID = tournamentID;
        TournamentName = tournamentName;
        TournamentType = tournamentType;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
        MaxParticipants = maxParticipants;
        CurrentParticipants = 0;
        Status = "upcoming";
        TournamentMode = tournamentMode;
        TournamentMap = tournamentMap;
        RewardIDs = new List<string>();
    }
}

[Serializable]
public class TournamentMatch
{
    public string MatchID;
    public string TournamentID;
    public string MatchName;
    public string Team1ID;
    public string Team1Name;
    public string Team2ID;
    public string Team2Name;
    public int Team1Score;
    public int Team2Score;
    public string WinnerTeamID;
    public DateTime MatchTime;
    public string MatchStatus;
    public string StreamURL;
    public string ReplayURL;

    public TournamentMatch(string matchID, string tournamentID, string matchName, string team1ID, string team1Name, string team2ID, string team2Name, DateTime matchTime)
    {
        MatchID = matchID;
        TournamentID = tournamentID;
        MatchName = matchName;
        Team1ID = team1ID;
        Team1Name = team1Name;
        Team2ID = team2ID;
        Team2Name = team2Name;
        Team1Score = 0;
        Team2Score = 0;
        WinnerTeamID = "";
        MatchTime = matchTime;
        MatchStatus = "scheduled";
        StreamURL = "";
        ReplayURL = "";
    }
}

[Serializable]
public class TournamentParticipant
{
    public string ParticipantID;
    public string TournamentID;
    public string PlayerID;
    public string PlayerName;
    public string TeamID;
    public string TeamName;
    public DateTime JoinTime;
    public int MatchPlayed;
    public int MatchWon;
    public int MatchLost;
    public bool IsEliminated;

    public TournamentParticipant(string participantID, string tournamentID, string playerID, string playerName, string teamID, string teamName)
    {
        ParticipantID = participantID;
        TournamentID = tournamentID;
        PlayerID = playerID;
        PlayerName = playerName;
        TeamID = teamID;
        TeamName = teamName;
        JoinTime = DateTime.Now;
        MatchPlayed = 0;
        MatchWon = 0;
        MatchLost = 0;
        IsEliminated = false;
    }
}

[Serializable]
public class TournamentReward
{
    public string RewardID;
    public string TournamentID;
    public string RewardName;
    public string RewardType;
    public int RewardAmount;
    public string RewardDescription;
    public int RankRequirement;
    public bool IsClaimed;
    public DateTime ClaimTime;

    public TournamentReward(string rewardID, string tournamentID, string rewardName, string rewardType, int rewardAmount, string rewardDescription, int rankRequirement)
    {
        RewardID = rewardID;
        TournamentID = tournamentID;
        RewardName = rewardName;
        RewardType = rewardType;
        RewardAmount = rewardAmount;
        RewardDescription = rewardDescription;
        RankRequirement = rankRequirement;
        IsClaimed = false;
        ClaimTime = DateTime.MinValue;
    }
}

[Serializable]
public class SpectatorInfo
{
    public string SpectatorID;
    public string MatchID;
    public string PlayerID;
    public string PlayerName;
    public DateTime JoinTime;
    public DateTime LeaveTime;
    public int WatchDuration;
    public string WatchStatus;

    public SpectatorInfo(string spectatorID, string matchID, string playerID, string playerName)
    {
        SpectatorID = spectatorID;
        MatchID = matchID;
        PlayerID = playerID;
        PlayerName = playerName;
        JoinTime = DateTime.Now;
        LeaveTime = DateTime.MinValue;
        WatchDuration = 0;
        WatchStatus = "watching";
    }
}

[Serializable]
public class PlayerTournamentData
{
    public string PlayerID;
    public List<TournamentParticipant> Tournaments;
    public List<SpectatorInfo> SpectatorHistory;
    public List<TournamentReward> Rewards;
    public int TotalTournaments;
    public int TotalMatches;
    public int TotalWins;
    public int TotalLosses;
    public DateTime LastTournamentTime;
    public DateTime LastSpectatorTime;

    public PlayerTournamentData(string playerID)
    {
        PlayerID = playerID;
        Tournaments = new List<TournamentParticipant>();
        SpectatorHistory = new List<SpectatorInfo>();
        Rewards = new List<TournamentReward>();
        TotalTournaments = 0;
        TotalMatches = 0;
        TotalWins = 0;
        TotalLosses = 0;
        LastTournamentTime = DateTime.MinValue;
        LastSpectatorTime = DateTime.MinValue;
    }
}

[Serializable]
public class TournamentSystemData
{
    public List<Tournament> AllTournaments;
    public List<TournamentMatch> AllMatches;
    public List<TournamentParticipant> AllParticipants;
    public List<TournamentReward> AllRewards;
    public List<SpectatorInfo> AllSpectators;
    public Dictionary<string, PlayerTournamentData> PlayerTournamentData;
    public int MaxTournaments;
    public int MaxSpectatorsPerMatch;
    public DateTime LastSystemUpdate;

    public TournamentSystemData()
    {
        AllTournaments = new List<Tournament>();
        AllMatches = new List<TournamentMatch>();
        AllParticipants = new List<TournamentParticipant>();
        AllRewards = new List<TournamentReward>();
        AllSpectators = new List<SpectatorInfo>();
        PlayerTournamentData = new Dictionary<string, PlayerTournamentData>();
        MaxTournaments = 10;
        MaxSpectatorsPerMatch = 1000;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddTournament(Tournament tournament)
    {
        AllTournaments.Add(tournament);
    }

    public void AddMatch(TournamentMatch match)
    {
        AllMatches.Add(match);
    }

    public void AddParticipant(TournamentParticipant participant)
    {
        AllParticipants.Add(participant);
    }

    public void AddReward(TournamentReward reward)
    {
        AllRewards.Add(reward);
    }

    public void AddSpectator(SpectatorInfo spectator)
    {
        AllSpectators.Add(spectator);
    }

    public void AddPlayerTournamentData(string playerID, PlayerTournamentData data)
    {
        PlayerTournamentData[playerID] = data;
    }
}

[Serializable]
public class TournamentEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string TournamentID;
    public string EventData;

    public TournamentEvent(string eventID, string eventType, string playerID, string tournamentID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        TournamentID = tournamentID;
        EventData = eventData;
    }
}

public class TournamentSystemDataManager
{
    private static TournamentSystemDataManager _instance;
    public static TournamentSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TournamentSystemDataManager();
            }
            return _instance;
        }
    }

    public TournamentSystemData tournamentData;
    private List<TournamentEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private TournamentSystemDataManager()
    {
        tournamentData = new TournamentSystemData();
        recentEvents = new List<TournamentEvent>();
        LoadTournamentData();
    }

    public void SaveTournamentData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TournamentSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tournamentData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存赛事系统数据失败: " + e.Message);
        }
    }

    public void LoadTournamentData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "TournamentSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    tournamentData = (TournamentSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载赛事系统数据失败: " + e.Message);
            tournamentData = new TournamentSystemData();
        }
    }

    public void CreateTournamentEvent(string eventType, string playerID, string tournamentID, string eventData)
    {
        string eventID = "tournament_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TournamentEvent tournamentEvent = new TournamentEvent(eventID, eventType, playerID, tournamentID, eventData);
        recentEvents.Add(tournamentEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<TournamentEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
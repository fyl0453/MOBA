using System;
using System.Collections.Generic;

public class TournamentSystemDetailedManager
{
    private static TournamentSystemDetailedManager _instance;
    public static TournamentSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TournamentSystemDetailedManager();
            }
            return _instance;
        }
    }

    private TournamentSystemData tournamentData;
    private TournamentSystemDataManager dataManager;

    private TournamentSystemDetailedManager()
    {
        dataManager = TournamentSystemDataManager.Instance;
        tournamentData = dataManager.tournamentData;
    }

    public void InitializePlayerTournamentData(string playerID)
    {
        if (!tournamentData.PlayerTournamentData.ContainsKey(playerID))
        {
            PlayerTournamentData playerData = new PlayerTournamentData(playerID);
            tournamentData.AddPlayerTournamentData(playerID, playerData);
            dataManager.SaveTournamentData();
            Debug.Log("初始化赛事数据成功");
        }
    }

    public string CreateTournament(string tournamentName, string tournamentType, string description, DateTime startTime, DateTime endTime, int maxParticipants, string tournamentMode, string tournamentMap)
    {
        string tournamentID = "tournament_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Tournament tournament = new Tournament(tournamentID, tournamentName, tournamentType, description, startTime, endTime, maxParticipants, tournamentMode, tournamentMap);
        tournamentData.AddTournament(tournament);
        
        dataManager.CreateTournamentEvent("tournament_create", "system", tournamentID, "创建赛事: " + tournamentName);
        dataManager.SaveTournamentData();
        Debug.Log("创建赛事成功: " + tournamentName);
        return tournamentID;
    }

    public string JoinTournament(string playerID, string playerName, string tournamentID, string teamID, string teamName)
    {
        Tournament tournament = tournamentData.AllTournaments.Find(t => t.TournamentID == tournamentID);
        if (tournament == null)
        {
            Debug.LogError("赛事不存在");
            return "";
        }
        
        if (tournament.CurrentParticipants >= tournament.MaxParticipants)
        {
            Debug.LogError("赛事人数已满");
            return "";
        }
        
        if (tournament.Status != "upcoming")
        {
            Debug.LogError("赛事已开始或已结束");
            return "";
        }
        
        InitializePlayerTournamentData(playerID);
        PlayerTournamentData playerData = tournamentData.PlayerTournamentData[playerID];
        
        if (playerData.Tournaments.Exists(t => t.TournamentID == tournamentID))
        {
            Debug.LogError("已报名该赛事");
            return "";
        }
        
        string participantID = "participant_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TournamentParticipant participant = new TournamentParticipant(participantID, tournamentID, playerID, playerName, teamID, teamName);
        tournamentData.AddParticipant(participant);
        playerData.Tournaments.Add(participant);
        playerData.TotalTournaments++;
        playerData.LastTournamentTime = DateTime.Now;
        
        tournament.CurrentParticipants++;
        
        dataManager.CreateTournamentEvent("tournament_join", playerID, tournamentID, "报名赛事: " + tournament.TournamentName);
        dataManager.SaveTournamentData();
        Debug.Log("报名赛事成功: " + tournament.TournamentName);
        return participantID;
    }

    public void LeaveTournament(string playerID, string tournamentID)
    {
        Tournament tournament = tournamentData.AllTournaments.Find(t => t.TournamentID == tournamentID);
        if (tournament == null)
        {
            return;
        }
        
        if (tournament.Status != "upcoming")
        {
            return;
        }
        
        if (!tournamentData.PlayerTournamentData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerTournamentData playerData = tournamentData.PlayerTournamentData[playerID];
        TournamentParticipant participant = playerData.Tournaments.Find(t => t.TournamentID == tournamentID);
        if (participant != null)
        {
            tournamentData.AllParticipants.Remove(participant);
            playerData.Tournaments.Remove(participant);
            playerData.TotalTournaments--;
            
            tournament.CurrentParticipants = Math.Max(0, tournament.CurrentParticipants - 1);
            
            dataManager.CreateTournamentEvent("tournament_leave", playerID, tournamentID, "取消报名: " + tournament.TournamentName);
            dataManager.SaveTournamentData();
            Debug.Log("取消报名成功: " + tournament.TournamentName);
        }
    }

    public string CreateMatch(string tournamentID, string matchName, string team1ID, string team1Name, string team2ID, string team2Name, DateTime matchTime)
    {
        Tournament tournament = tournamentData.AllTournaments.Find(t => t.TournamentID == tournamentID);
        if (tournament == null)
        {
            return "";
        }
        
        string matchID = "match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TournamentMatch match = new TournamentMatch(matchID, tournamentID, matchName, team1ID, team1Name, team2ID, team2Name, matchTime);
        tournamentData.AddMatch(match);
        
        dataManager.CreateTournamentEvent("match_create", "system", tournamentID, "创建比赛: " + matchName);
        dataManager.SaveTournamentData();
        Debug.Log("创建比赛成功: " + matchName);
        return matchID;
    }

    public void UpdateMatchResult(string matchID, int team1Score, int team2Score, string winnerTeamID)
    {
        TournamentMatch match = tournamentData.AllMatches.Find(m => m.MatchID == matchID);
        if (match != null)
        {
            match.Team1Score = team1Score;
            match.Team2Score = team2Score;
            match.WinnerTeamID = winnerTeamID;
            match.MatchStatus = "completed";
            
            UpdateParticipantStats(match.TournamentID, match.Team1ID, team1Score > team2Score);
            UpdateParticipantStats(match.TournamentID, match.Team2ID, team2Score > team1Score);
            
            dataManager.CreateTournamentEvent("match_result", "system", match.TournamentID, "更新比赛结果: " + match.MatchName);
            dataManager.SaveTournamentData();
            Debug.Log("更新比赛结果成功: " + match.MatchName);
        }
    }

    public string JoinSpectator(string playerID, string playerName, string matchID)
    {
        TournamentMatch match = tournamentData.AllMatches.Find(m => m.MatchID == matchID);
        if (match == null)
        {
            return "";
        }
        
        int spectatorCount = tournamentData.AllSpectators.FindAll(s => s.MatchID == matchID && s.WatchStatus == "watching").Count;
        if (spectatorCount >= tournamentData.MaxSpectatorsPerMatch)
        {
            Debug.LogError("观战人数已满");
            return "";
        }
        
        InitializePlayerTournamentData(playerID);
        PlayerTournamentData playerData = tournamentData.PlayerTournamentData[playerID];
        
        string spectatorID = "spectator_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SpectatorInfo spectator = new SpectatorInfo(spectatorID, matchID, playerID, playerName);
        tournamentData.AddSpectator(spectator);
        playerData.SpectatorHistory.Add(spectator);
        playerData.LastSpectatorTime = DateTime.Now;
        
        dataManager.CreateTournamentEvent("spectator_join", playerID, match.TournamentID, "加入观战: " + match.MatchName);
        dataManager.SaveTournamentData();
        Debug.Log("加入观战成功: " + match.MatchName);
        return spectatorID;
    }

    public void LeaveSpectator(string playerID, string spectatorID)
    {
        SpectatorInfo spectator = tournamentData.AllSpectators.Find(s => s.SpectatorID == spectatorID && s.PlayerID == playerID && s.WatchStatus == "watching");
        if (spectator != null)
        {
            spectator.LeaveTime = DateTime.Now;
            spectator.WatchDuration = (int)(spectator.LeaveTime - spectator.JoinTime).TotalMinutes;
            spectator.WatchStatus = "completed";
            
            dataManager.CreateTournamentEvent("spectator_leave", playerID, "", "离开观战");
            dataManager.SaveTournamentData();
            Debug.Log("离开观战成功");
        }
    }

    public string CreateReward(string tournamentID, string rewardName, string rewardType, int rewardAmount, string rewardDescription, int rankRequirement)
    {
        Tournament tournament = tournamentData.AllTournaments.Find(t => t.TournamentID == tournamentID);
        if (tournament == null)
        {
            return "";
        }
        
        string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TournamentReward reward = new TournamentReward(rewardID, tournamentID, rewardName, rewardType, rewardAmount, rewardDescription, rankRequirement);
        tournamentData.AddReward(reward);
        tournament.RewardIDs.Add(rewardID);
        
        dataManager.CreateTournamentEvent("reward_create", "system", tournamentID, "创建奖励: " + rewardName);
        dataManager.SaveTournamentData();
        Debug.Log("创建奖励成功: " + rewardName);
        return rewardID;
    }

    public void ClaimReward(string playerID, string rewardID)
    {
        TournamentReward reward = tournamentData.AllRewards.Find(r => r.RewardID == rewardID);
        if (reward == null || reward.IsClaimed)
        {
            return;
        }
        
        if (!tournamentData.PlayerTournamentData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerTournamentData playerData = tournamentData.PlayerTournamentData[playerID];
        TournamentParticipant participant = playerData.Tournaments.Find(t => t.TournamentID == reward.TournamentID);
        if (participant == null)
        {
            return;
        }
        
        reward.IsClaimed = true;
        reward.ClaimTime = DateTime.Now;
        playerData.Rewards.Add(reward);
        
        dataManager.CreateTournamentEvent("reward_claim", playerID, reward.TournamentID, "领取奖励: " + reward.RewardName);
        dataManager.SaveTournamentData();
        Debug.Log("领取奖励成功: " + reward.RewardName);
    }

    public void StartTournament(string tournamentID)
    {
        Tournament tournament = tournamentData.AllTournaments.Find(t => t.TournamentID == tournamentID);
        if (tournament != null && tournament.Status == "upcoming")
        {
            tournament.Status = "ongoing";
            
            dataManager.CreateTournamentEvent("tournament_start", "system", tournamentID, "开始赛事: " + tournament.TournamentName);
            dataManager.SaveTournamentData();
            Debug.Log("开始赛事成功: " + tournament.TournamentName);
        }
    }

    public void EndTournament(string tournamentID)
    {
        Tournament tournament = tournamentData.AllTournaments.Find(t => t.TournamentID == tournamentID);
        if (tournament != null && tournament.Status == "ongoing")
        {
            tournament.Status = "completed";
            
            dataManager.CreateTournamentEvent("tournament_end", "system", tournamentID, "结束赛事: " + tournament.TournamentName);
            dataManager.SaveTournamentData();
            Debug.Log("结束赛事成功: " + tournament.TournamentName);
        }
    }

    private void UpdateParticipantStats(string tournamentID, string teamID, bool isWinner)
    {
        List<TournamentParticipant> participants = tournamentData.AllParticipants.FindAll(p => p.TournamentID == tournamentID && p.TeamID == teamID);
        foreach (TournamentParticipant participant in participants)
        {
            participant.MatchPlayed++;
            if (isWinner)
            {
                participant.MatchWon++;
                if (tournamentData.PlayerTournamentData.ContainsKey(participant.PlayerID))
                {
                    PlayerTournamentData playerData = tournamentData.PlayerTournamentData[participant.PlayerID];
                    playerData.TotalWins++;
                }
            }
            else
            {
                participant.MatchLost++;
                if (tournamentData.PlayerTournamentData.ContainsKey(participant.PlayerID))
                {
                    PlayerTournamentData playerData = tournamentData.PlayerTournamentData[participant.PlayerID];
                    playerData.TotalLosses++;
                }
            }
        }
    }

    public List<Tournament> GetAllTournaments()
    {
        return tournamentData.AllTournaments;
    }

    public List<Tournament> GetUpcomingTournaments()
    {
        return tournamentData.AllTournaments.FindAll(t => t.Status == "upcoming");
    }

    public List<Tournament> GetOngoingTournaments()
    {
        return tournamentData.AllTournaments.FindAll(t => t.Status == "ongoing");
    }

    public List<Tournament> GetCompletedTournaments()
    {
        return tournamentData.AllTournaments.FindAll(t => t.Status == "completed");
    }

    public List<TournamentMatch> GetTournamentMatches(string tournamentID)
    {
        return tournamentData.AllMatches.FindAll(m => m.TournamentID == tournamentID);
    }

    public List<TournamentParticipant> GetTournamentParticipants(string tournamentID)
    {
        return tournamentData.AllParticipants.FindAll(p => p.TournamentID == tournamentID);
    }

    public List<TournamentReward> GetTournamentRewards(string tournamentID)
    {
        return tournamentData.AllRewards.FindAll(r => r.TournamentID == tournamentID);
    }

    public List<TournamentParticipant> GetPlayerTournaments(string playerID)
    {
        if (tournamentData.PlayerTournamentData.ContainsKey(playerID))
        {
            return tournamentData.PlayerTournamentData[playerID].Tournaments;
        }
        return new List<TournamentParticipant>();
    }

    public List<SpectatorInfo> GetPlayerSpectatorHistory(string playerID)
    {
        if (tournamentData.PlayerTournamentData.ContainsKey(playerID))
        {
            return tournamentData.PlayerTournamentData[playerID].SpectatorHistory;
        }
        return new List<SpectatorInfo>();
    }

    public List<TournamentReward> GetPlayerRewards(string playerID)
    {
        if (tournamentData.PlayerTournamentData.ContainsKey(playerID))
        {
            return tournamentData.PlayerTournamentData[playerID].Rewards;
        }
        return new List<TournamentReward>();
    }

    public Tournament GetTournament(string tournamentID)
    {
        return tournamentData.AllTournaments.Find(t => t.TournamentID == tournamentID);
    }

    public TournamentMatch GetMatch(string matchID)
    {
        return tournamentData.AllMatches.Find(m => m.MatchID == matchID);
    }

    public void CleanupOldTournaments(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<Tournament> oldTournaments = tournamentData.AllTournaments.FindAll(t => t.EndTime < cutoffDate);
        foreach (Tournament tournament in oldTournaments)
        {
            tournamentData.AllTournaments.Remove(tournament);
            
            List<TournamentMatch> tournamentMatches = tournamentData.AllMatches.FindAll(m => m.TournamentID == tournament.TournamentID);
            foreach (TournamentMatch match in tournamentMatches)
            {
                tournamentData.AllMatches.Remove(match);
            }
            
            List<TournamentParticipant> tournamentParticipants = tournamentData.AllParticipants.FindAll(p => p.TournamentID == tournament.TournamentID);
            foreach (TournamentParticipant participant in tournamentParticipants)
            {
                tournamentData.AllParticipants.Remove(participant);
            }
            
            List<TournamentReward> tournamentRewards = tournamentData.AllRewards.FindAll(r => r.TournamentID == tournament.TournamentID);
            foreach (TournamentReward reward in tournamentRewards)
            {
                tournamentData.AllRewards.Remove(reward);
            }
        }
        
        dataManager.CreateTournamentEvent("tournament_cleanup", "system", "", "清理旧赛事: " + oldTournaments.Count);
        dataManager.SaveTournamentData();
        Debug.Log("清理旧赛事成功: " + oldTournaments.Count);
    }

    public void SaveData()
    {
        dataManager.SaveTournamentData();
    }

    public void LoadData()
    {
        dataManager.LoadTournamentData();
    }

    public List<TournamentEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
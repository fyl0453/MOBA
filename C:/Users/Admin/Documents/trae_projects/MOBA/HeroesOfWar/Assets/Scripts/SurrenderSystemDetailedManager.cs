using System;
using System.Collections.Generic;
using UnityEngine;

public class SurrenderSystemDetailedManager
{
    private static SurrenderSystemDetailedManager _instance;
    public static SurrenderSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SurrenderSystemDetailedManager();
            }
            return _instance;
        }
    }

    private SurrenderSystemData surrenderData;
    private SurrenderSystemDataManager dataManager;

    private SurrenderSystemDetailedManager()
    {
        dataManager = SurrenderSystemDataManager.Instance;
        surrenderData = dataManager.surrenderData;
    }

    public void InitializeMatchSurrenderData(string matchID)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            MatchSurrenderData matchSurrenderData = new MatchSurrenderData(matchID);
            surrenderData.MatchSurrenderData[matchID] = matchSurrenderData;
            dataManager.SaveSurrenderData();
            Debug.Log("初始化投降数据成功");
        }
    }

    public string InitiateSurrenderVote(string matchID, string playerID, string playerName, int totalPlayers)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            InitializeMatchSurrenderData(matchID);
        }
        
        MatchSurrenderData matchSurrenderData = surrenderData.MatchSurrenderData[matchID];
        if (!surrenderData.AllowSurrender)
        {
            Debug.LogError("投降功能已禁用");
            return "";
        }
        
        SurrenderVote activeVote = matchSurrenderData.SurrenderVotes.Find(v => v.IsActive);
        if (activeVote != null)
        {
            Debug.LogError("已有进行中的投降投票");
            return "";
        }
        
        SurrenderVote vote = new SurrenderVote(matchID, playerID, playerName, totalPlayers);
        matchSurrenderData.SurrenderVotes.Add(vote);
        matchSurrenderData.SurrenderAttempts++;
        
        dataManager.CreateSurrenderEvent("vote_initiate", matchID, playerID, "发起投降投票: " + playerName);
        dataManager.SaveSurrenderData();
        Debug.Log("发起投降投票成功");
        return vote.VoteID;
    }

    public bool CastVote(string matchID, string playerID, string playerName, string voteID, bool voteChoice)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            return false;
        }
        
        MatchSurrenderData matchSurrenderData = surrenderData.MatchSurrenderData[matchID];
        SurrenderVote vote = matchSurrenderData.SurrenderVotes.Find(v => v.VoteID == voteID && v.IsActive);
        if (vote == null)
        {
            Debug.LogError("投降投票不存在");
            return false;
        }
        
        if (DateTime.Now > vote.VoteEndTime)
        {
            vote.IsActive = false;
            vote.VoteResult = "超时未决";
            Debug.LogError("投降投票已超时");
            return false;
        }
        
        PlayerSurrenderVote existingVote = matchSurrenderData.PlayerVotes.Find(p => p.PlayerID == playerID && p.VoteID == voteID);
        if (existingVote != null && existingVote.HasVoted)
        {
            Debug.LogError("您已投过票");
            return false;
        }
        
        PlayerSurrenderVote playerVote = new PlayerSurrenderVote(playerID, playerName, voteID, voteChoice);
        playerVote.HasVoted = true;
        playerVote.VoteChoice = voteChoice;
        playerVote.VoteTime = DateTime.Now;
        matchSurrenderData.PlayerVotes.Add(playerVote);
        
        if (voteChoice)
        {
            vote.YesVotes++;
        }
        else
        {
            vote.NoVotes++;
        }
        
        dataManager.CreateSurrenderEvent("vote_cast", matchID, playerID, "投票: " + (voteChoice ? "赞成" : "反对"));
        dataManager.SaveSurrenderData();
        Debug.Log("投票成功: " + (voteChoice ? "赞成" : "反对"));
        
        CheckVoteResult(matchID, voteID);
        return true;
    }

    private void CheckVoteResult(string matchID, string voteID)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            return;
        }
        
        MatchSurrenderData matchSurrenderData = surrenderData.MatchSurrenderData[matchID];
        SurrenderVote vote = matchSurrenderData.SurrenderVotes.Find(v => v.VoteID == voteID);
        if (vote == null || !vote.IsActive)
        {
            return;
        }
        
        if (DateTime.Now > vote.VoteEndTime || vote.YesVotes + vote.NoVotes >= vote.TotalPlayers)
        {
            vote.IsActive = false;
            
            if (vote.YesVotes >= vote.RequiredVotes)
            {
                vote.IsPassed = true;
                vote.VoteResult = "投降通过";
                
                dataManager.CreateSurrenderEvent("vote_pass", matchID, "", "投降投票通过: " + vote.YesVotes + "/" + vote.RequiredVotes);
                Debug.Log("投降投票通过: " + vote.YesVotes + "/" + vote.RequiredVotes);
            }
            else
            {
                vote.VoteResult = "投降被否决";
                
                dataManager.CreateSurrenderEvent("vote_fail", matchID, "", "投降投票被否决: " + vote.YesVotes + "/" + vote.RequiredVotes);
                Debug.Log("投降投票被否决: " + vote.YesVotes + "/" + vote.RequiredVotes);
            }
            
            dataManager.SaveSurrenderData();
        }
    }

    public void RecordDisconnect(string matchID, string playerID, string playerName, string reason)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            InitializeMatchSurrenderData(matchID);
        }
        
        MatchSurrenderData matchSurrenderData = surrenderData.MatchSurrenderData[matchID];
        DisconnectRecord record = new DisconnectRecord(matchID, playerID, playerName, reason);
        matchSurrenderData.DisconnectRecords.Add(record);
        matchSurrenderData.LastDisconnectTime = DateTime.Now;
        
        dataManager.CreateSurrenderEvent("disconnect", matchID, playerID, "玩家断线: " + playerName + " 原因: " + reason);
        dataManager.SaveSurrenderData();
        Debug.Log("记录断线成功: " + playerName);
    }

    public bool RecordReconnect(string matchID, string playerID, string playerName)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            return false;
        }
        
        MatchSurrenderData matchSurrenderData = surrenderData.MatchSurrenderData[matchID];
        DisconnectRecord record = matchSurrenderData.DisconnectRecords.Find(d => d.PlayerID == playerID && !d.IsReconnected);
        if (record != null)
        {
            record.IsReconnected = true;
            record.ReconnectTime = DateTime.Now;
            record.DisconnectDuration = (int)(record.ReconnectTime - record.DisconnectTime).TotalSeconds;
            
            if (record.DisconnectDuration > surrenderData.MaxDisconnectPenaltyMinutes * 60)
            {
                record.WasPenalized = true;
            }
            
            dataManager.CreateSurrenderEvent("reconnect", matchID, playerID, "玩家重连: " + playerName + " 断线时长: " + record.DisconnectDuration + "秒");
            dataManager.SaveSurrenderData();
            Debug.Log("记录重连成功: " + playerName + " 断线时长: " + record.DisconnectDuration + "秒");
            return true;
        }
        
        return false;
    }

    public SurrenderVote GetActiveVote(string matchID)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            return null;
        }
        return surrenderData.MatchSurrenderData[matchID].SurrenderVotes.Find(v => v.IsActive);
    }

    public List<SurrenderVote> GetMatchVotes(string matchID)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            return new List<SurrenderVote>();
        }
        return surrenderData.MatchSurrenderData[matchID].SurrenderVotes;
    }

    public List<DisconnectRecord> GetDisconnectRecords(string matchID)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            return new List<DisconnectRecord>();
        }
        return surrenderData.MatchSurrenderData[matchID].DisconnectRecords;
    }

    public MatchSurrenderData GetMatchSurrenderData(string matchID)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            return null;
        }
        return surrenderData.MatchSurrenderData[matchID];
    }

    public int GetDisconnectCount(string matchID, string playerID)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            return 0;
        }
        return surrenderData.MatchSurrenderData[matchID].DisconnectRecords.FindAll(d => d.PlayerID == playerID).Count;
    }

    public int GetTotalDisconnectTime(string matchID, string playerID)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            return 0;
        }
        
        List<DisconnectRecord> records = surrenderData.MatchSurrenderData[matchID].DisconnectRecords.FindAll(d => d.PlayerID == playerID);
        int totalTime = 0;
        foreach (DisconnectRecord record in records)
        {
            if (record.IsReconnected)
            {
                totalTime += record.DisconnectDuration;
            }
        }
        return totalTime;
    }

    public void EnableSurrender(string matchID, bool enabled)
    {
        if (!surrenderData.MatchSurrenderData.ContainsKey(matchID))
        {
            InitializeMatchSurrenderData(matchID);
        }
        surrenderData.MatchSurrenderData[matchID].SurrenderEnabled = enabled;
        dataManager.SaveSurrenderData();
    }

    public void SetMinGameTimeForSurrender(int minutes)
    {
        surrenderData.MinGameTimeForSurrender = minutes;
        dataManager.SaveSurrenderData();
    }

    public void SetVoteDuration(int seconds)
    {
        surrenderData.VoteDuration = seconds;
        dataManager.SaveSurrenderData();
    }

    public void SetAllowSurrender(bool allowed)
    {
        surrenderData.AllowSurrender = allowed;
        dataManager.SaveSurrenderData();
    }

    public void CleanupOldData(int days = 7)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<string> matchesToRemove = new List<string>();
        
        foreach (KeyValuePair<string, MatchSurrenderData> kvp in surrenderData.MatchSurrenderData)
        {
            if (kvp.Value.LastDisconnectTime < cutoffDate)
            {
                matchesToRemove.Add(kvp.Key);
            }
        }
        
        foreach (string matchID in matchesToRemove)
        {
            surrenderData.MatchSurrenderData.Remove(matchID);
        }
        
        if (matchesToRemove.Count > 0)
        {
            dataManager.CreateSurrenderEvent("data_cleanup", "system", "", "清理旧投降数据: " + matchesToRemove.Count);
            dataManager.SaveSurrenderData();
            Debug.Log("清理旧投降数据成功: " + matchesToRemove.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveSurrenderData();
    }

    public void LoadData()
    {
        dataManager.LoadSurrenderData();
    }

    public List<SurrenderEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
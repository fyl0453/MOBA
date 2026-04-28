using System;
using System.Collections.Generic;
using System.IO;

public class ReplaySystemDetailedManager
{
    private static ReplaySystemDetailedManager _instance;
    public static ReplaySystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ReplaySystemDetailedManager();
            }
            return _instance;
        }
    }

    private ReplaySystemData replayData;
    private ReplaySystemDataManager dataManager;

    private ReplaySystemDetailedManager()
    {
        dataManager = ReplaySystemDataManager.Instance;
        replayData = dataManager.replayData;
    }

    public void InitializePlayerReplayData(string playerID)
    {
        if (!replayData.PlayerReplayData.ContainsKey(playerID))
        {
            PlayerReplayData playerReplayData = new PlayerReplayData(playerID);
            replayData.AddPlayerReplayData(playerID, playerReplayData);
            dataManager.SaveReplayData();
            Debug.Log("初始化回放数据成功");
        }
    }

    public string RecordReplay(string playerID, string matchID, string matchType, float duration, List<ReplayParticipant> participants)
    {
        InitializePlayerReplayData(playerID);
        PlayerReplayData playerReplayData = replayData.PlayerReplayData[playerID];
        
        if (playerReplayData.SavedReplays.Count >= replayData.MaxReplaysPerPlayer)
        {
            Debug.LogError("回放数量已达上限");
            return "";
        }
        
        string replayID = "replay_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ReplayData replay = new ReplayData(replayID, matchID, matchType, duration);
        replay.Participants = participants;
        replayData.AddReplay(replay);
        
        playerReplayData.SavedReplays.Add(replay);
        playerReplayData.TotalReplays++;
        playerReplayData.LastReplayTime = DateTime.Now;
        
        dataManager.CreateReplayEvent("replay_record", playerID, replayID, "录制回放开始");
        dataManager.SaveReplayData();
        Debug.Log("开始录制回放: " + replayID);
        return replayID;
    }

    public void AddReplayFrame(string replayID, int frameIndex, float time, Dictionary<string, PlayerState> playerStates, List<GameEvent> events)
    {
        ReplayData replay = replayData.AllReplays.Find(r => r.ReplayID == replayID);
        if (replay != null)
        {
            string frameID = "frame_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            ReplayFrame frame = new ReplayFrame(frameID, frameIndex, time);
            frame.PlayerStates = playerStates;
            frame.Events = events;
            replay.Frames.Add(frame);
            
            dataManager.SaveReplayData();
        }
    }

    public void SaveReplay(string replayID, string winnerTeam, string replayPath)
    {
        ReplayData replay = replayData.AllReplays.Find(r => r.ReplayID == replayID);
        if (replay != null)
        {
            replay.WinnerTeam = winnerTeam;
            replay.ReplayPath = replayPath;
            replay.IsSaved = true;
            
            if (File.Exists(replayPath))
            {
                FileInfo fileInfo = new FileInfo(replayPath);
                replay.FileSize = fileInfo.Length;
            }
            
            dataManager.CreateReplayEvent("replay_save", "", replayID, "保存回放成功");
            dataManager.SaveReplayData();
            Debug.Log("保存回放成功: " + replayID);
        }
    }

    public string CreateHighlightClip(string playerID, string replayID, string clipName, float startTime, float endTime, string clipType)
    {
        ReplayData replay = replayData.AllReplays.Find(r => r.ReplayID == replayID);
        if (replay == null)
        {
            Debug.LogError("回放不存在");
            return "";
        }
        
        InitializePlayerReplayData(playerID);
        PlayerReplayData playerReplayData = replayData.PlayerReplayData[playerID];
        
        if (replay.HighlightClips.Count >= replayData.MaxClipsPerReplay)
        {
            Debug.LogError("精彩时刻数量已达上限");
            return "";
        }
        
        string clipID = "clip_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ReplayClip clip = new ReplayClip(clipID, replayID, clipName, startTime, endTime, clipType);
        replayData.AddClip(clip);
        replay.HighlightClips.Add(clip);
        
        playerReplayData.SavedClips.Add(clip);
        playerReplayData.TotalClips++;
        
        dataManager.CreateReplayEvent("clip_create", playerID, replayID, "创建精彩时刻: " + clipName);
        dataManager.SaveReplayData();
        Debug.Log("创建精彩时刻成功: " + clipName);
        return clipID;
    }

    public void SetClipFavorite(string clipID, bool isFavorite)
    {
        ReplayClip clip = replayData.AllClips.Find(c => c.ClipID == clipID);
        if (clip != null)
        {
            clip.IsFavorite = isFavorite;
            
            ReplayData replay = replayData.AllReplays.Find(r => r.ReplayID == clip.ReplayID);
            if (replay != null)
            {
                ReplayClip replayClip = replay.HighlightClips.Find(c => c.ClipID == clipID);
                if (replayClip != null)
                {
                    replayClip.IsFavorite = isFavorite;
                }
            }
            
            dataManager.SaveReplayData();
            Debug.Log("设置精彩时刻收藏状态: " + isFavorite);
        }
    }

    public void SetReplayFavorite(string playerID, string replayID, bool isFavorite)
    {
        InitializePlayerReplayData(playerID);
        PlayerReplayData playerReplayData = replayData.PlayerReplayData[playerID];
        
        if (isFavorite && !playerReplayData.FavoriteReplays.Contains(replayID))
        {
            playerReplayData.FavoriteReplays.Add(replayID);
        }
        else if (!isFavorite && playerReplayData.FavoriteReplays.Contains(replayID))
        {
            playerReplayData.FavoriteReplays.Remove(replayID);
        }
        
        dataManager.SaveReplayData();
        Debug.Log("设置回放收藏状态: " + isFavorite);
    }

    public void DeleteReplay(string playerID, string replayID)
    {
        ReplayData replay = replayData.AllReplays.Find(r => r.ReplayID == replayID);
        if (replay != null)
        {
            if (File.Exists(replay.ReplayPath))
            {
                try
                {
                    File.Delete(replay.ReplayPath);
                }
                catch (Exception e)
                {
                    Debug.LogError("删除回放文件失败: " + e.Message);
                }
            }
            
            List<ReplayClip> clipsToDelete = replayData.AllClips.FindAll(c => c.ReplayID == replayID);
            foreach (ReplayClip clip in clipsToDelete)
            {
                replayData.AllClips.Remove(clip);
            }
            
            replayData.AllReplays.Remove(replay);
            
            if (replayData.PlayerReplayData.ContainsKey(playerID))
            {
                PlayerReplayData playerReplayData = replayData.PlayerReplayData[playerID];
                playerReplayData.SavedReplays.Remove(replay);
                playerReplayData.TotalReplays--;
                if (playerReplayData.FavoriteReplays.Contains(replayID))
                {
                    playerReplayData.FavoriteReplays.Remove(replayID);
                }
                
                List<ReplayClip> playerClipsToDelete = playerReplayData.SavedClips.FindAll(c => c.ReplayID == replayID);
                foreach (ReplayClip clip in playerClipsToDelete)
                {
                    playerReplayData.SavedClips.Remove(clip);
                    playerReplayData.TotalClips--;
                    if (playerReplayData.FavoriteClips.Contains(clip.ClipID))
                    {
                        playerReplayData.FavoriteClips.Remove(clip.ClipID);
                    }
                }
            }
            
            dataManager.CreateReplayEvent("replay_delete", playerID, replayID, "删除回放");
            dataManager.SaveReplayData();
            Debug.Log("删除回放成功: " + replayID);
        }
    }

    public void DeleteClip(string playerID, string clipID)
    {
        ReplayClip clip = replayData.AllClips.Find(c => c.ClipID == clipID);
        if (clip != null)
        {
            ReplayData replay = replayData.AllReplays.Find(r => r.ReplayID == clip.ReplayID);
            if (replay != null)
            {
                replay.HighlightClips.Remove(clip);
            }
            
            replayData.AllClips.Remove(clip);
            
            if (replayData.PlayerReplayData.ContainsKey(playerID))
            {
                PlayerReplayData playerReplayData = replayData.PlayerReplayData[playerID];
                playerReplayData.SavedClips.Remove(clip);
                playerReplayData.TotalClips--;
                if (playerReplayData.FavoriteClips.Contains(clipID))
                {
                    playerReplayData.FavoriteClips.Remove(clipID);
                }
            }
            
            dataManager.CreateReplayEvent("clip_delete", playerID, clip.ReplayID, "删除精彩时刻");
            dataManager.SaveReplayData();
            Debug.Log("删除精彩时刻成功: " + clipID);
        }
    }

    public List<ReplayData> GetPlayerReplays(string playerID)
    {
        if (replayData.PlayerReplayData.ContainsKey(playerID))
        {
            return replayData.PlayerReplayData[playerID].SavedReplays;
        }
        return new List<ReplayData>();
    }

    public List<ReplayClip> GetPlayerClips(string playerID)
    {
        if (replayData.PlayerReplayData.ContainsKey(playerID))
        {
            return replayData.PlayerReplayData[playerID].SavedClips;
        }
        return new List<ReplayClip>();
    }

    public List<ReplayData> GetPlayerFavoriteReplays(string playerID)
    {
        if (replayData.PlayerReplayData.ContainsKey(playerID))
        {
            PlayerReplayData playerReplayData = replayData.PlayerReplayData[playerID];
            return replayData.AllReplays.FindAll(r => playerReplayData.FavoriteReplays.Contains(r.ReplayID));
        }
        return new List<ReplayData>();
    }

    public List<ReplayClip> GetPlayerFavoriteClips(string playerID)
    {
        if (replayData.PlayerReplayData.ContainsKey(playerID))
        {
            PlayerReplayData playerReplayData = replayData.PlayerReplayData[playerID];
            return replayData.AllClips.FindAll(c => playerReplayData.FavoriteClips.Contains(c.ClipID));
        }
        return new List<ReplayClip>();
    }

    public ReplayData GetReplay(string replayID)
    {
        return replayData.AllReplays.Find(r => r.ReplayID == replayID);
    }

    public ReplayClip GetClip(string clipID)
    {
        return replayData.AllClips.Find(c => c.ClipID == clipID);
    }

    public List<ReplayFrame> GetReplayFrames(string replayID, float startTime, float endTime)
    {
        ReplayData replay = GetReplay(replayID);
        if (replay != null)
        {
            return replay.Frames.FindAll(f => f.Time >= startTime && f.Time <= endTime);
        }
        return new List<ReplayFrame>();
    }

    public void CleanupOldReplays()
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-replayData.ReplayRetentionDays);
        List<ReplayData> oldReplays = new List<ReplayData>();
        
        foreach (ReplayData replay in replayData.AllReplays)
        {
            if (replay.MatchTime < cutoffDate)
            {
                oldReplays.Add(replay);
            }
        }
        
        foreach (ReplayData replay in oldReplays)
        {
            DeleteReplay("system", replay.ReplayID);
        }
        
        if (oldReplays.Count > 0)
        {
            dataManager.CreateReplayEvent("replay_cleanup", "system", "", "清理旧回放: " + oldReplays.Count);
            replayData.LastSystemCleanup = DateTime.Now;
            dataManager.SaveReplayData();
            Debug.Log("清理旧回放成功: " + oldReplays.Count);
        }
    }

    public void CleanupExcessReplays()
    {
        foreach (string playerID in replayData.PlayerReplayData.Keys)
        {
            PlayerReplayData playerReplayData = replayData.PlayerReplayData[playerID];
            if (playerReplayData.SavedReplays.Count > replayData.MaxReplaysPerPlayer)
            {
                playerReplayData.SavedReplays.Sort((a, b) => b.MatchTime.CompareTo(a.MatchTime));
                List<ReplayData> excessReplays = playerReplayData.SavedReplays.GetRange(replayData.MaxReplaysPerPlayer, playerReplayData.SavedReplays.Count - replayData.MaxReplaysPerPlayer);
                foreach (ReplayData replay in excessReplays)
                {
                    DeleteReplay(playerID, replay.ReplayID);
                }
            }
        }
    }

    public long GetTotalReplayStorage()
    {
        long totalSize = 0;
        foreach (ReplayData replay in replayData.AllReplays)
        {
            if (replay.IsSaved)
            {
                totalSize += replay.FileSize;
            }
        }
        return totalSize;
    }

    public void SaveData()
    {
        dataManager.SaveReplayData();
    }

    public void LoadData()
    {
        dataManager.LoadReplayData();
    }

    public List<ReplayEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
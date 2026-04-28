using System;
using System.Collections.Generic;

public class SpectatorSystemDetailedManager
{
    private static SpectatorSystemDetailedManager _instance;
    public static SpectatorSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SpectatorSystemDetailedManager();
            }
            return _instance;
        }
    }

    private SpectatorSystemData spectatorData;
    private SpectatorSystemDataManager dataManager;

    private SpectatorSystemDetailedManager()
    {
        dataManager = SpectatorSystemDataManager.Instance;
        spectatorData = dataManager.spectatorData;
    }

    public string StartSpectating(string playerID, string playerName, string gameID, string spectatorType = "friend")
    {
        string spectatorID = "spectator_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Spectator spectator = new Spectator(spectatorID, playerID, playerName, gameID, spectatorType);
        spectatorData.AddSpectator(spectator);
        dataManager.CreateSpectatorEvent("spectator_join", playerID, gameID, "开始观战");
        dataManager.SaveSpectatorData();
        Debug.Log("开始观战成功");
        return spectatorID;
    }

    public void StopSpectating(string spectatorID)
    {
        Spectator spectator = spectatorData.Spectators.Find(s => s.SpectatorID == spectatorID);
        if (spectator != null && spectator.IsActive)
        {
            spectator.IsActive = false;
            spectator.LeaveTime = DateTime.Now;
            dataManager.CreateSpectatorEvent("spectator_leave", spectator.PlayerID, spectator.GameID, "结束观战");
            dataManager.SaveSpectatorData();
            Debug.Log("结束观战成功");
        }
    }

    public List<Spectator> GetGameSpectators(string gameID)
    {
        if (spectatorData.GameSpectators.ContainsKey(gameID))
        {
            return spectatorData.GameSpectators[gameID].FindAll(s => s.IsActive);
        }
        return new List<Spectator>();
    }

    public int GetGameSpectatorCount(string gameID)
    {
        return GetGameSpectators(gameID).Count;
    }

    public Spectator GetSpectator(string spectatorID)
    {
        return spectatorData.Spectators.Find(s => s.SpectatorID == spectatorID);
    }

    public void RecordGameReplay(string gameID, string matchType, List<string> blueTeamPlayers, List<string> redTeamPlayers, int duration, string winnerTeam, string replayPath)
    {
        string replayID = "replay_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GameReplay replay = new GameReplay(replayID, gameID, matchType, blueTeamPlayers, redTeamPlayers, duration, winnerTeam, replayPath);
        spectatorData.AddGameReplay(replay);
        dataManager.CreateSpectatorEvent("replay_record", "system", gameID, "录制游戏回放");
        dataManager.SaveSpectatorData();
        Debug.Log("录制游戏回放成功");
    }

    public List<GameReplay> GetRecentReplays(int count = 50)
    {
        List<GameReplay> replays = new List<GameReplay>(spectatorData.GameReplays);
        replays.Sort((a, b) => b.GameTime.CompareTo(a.GameTime));
        if (count < replays.Count)
        {
            return replays.GetRange(0, count);
        }
        return replays;
    }

    public List<GameReplay> GetPlayerReplays(string playerID, int count = 50)
    {
        if (spectatorData.PlayerReplays.ContainsKey(playerID))
        {
            List<GameReplay> replays = spectatorData.PlayerReplays[playerID];
            replays.Sort((a, b) => b.GameTime.CompareTo(a.GameTime));
            if (count < replays.Count)
            {
                return replays.GetRange(0, count);
            }
            return replays;
        }
        return new List<GameReplay>();
    }

    public GameReplay GetReplay(string replayID)
    {
        return spectatorData.GameReplays.Find(r => r.ReplayID == replayID);
    }

    public void PlayReplay(string playerID, string replayID)
    {
        GameReplay replay = GetReplay(replayID);
        if (replay != null && replay.IsAvailable)
        {
            replay.ViewCount++;
            dataManager.CreateSpectatorEvent("replay_play", playerID, replayID, "播放游戏回放");
            dataManager.SaveSpectatorData();
            Debug.Log("播放游戏回放成功");
        }
    }

    public void DeleteReplay(string replayID)
    {
        GameReplay replay = GetReplay(replayID);
        if (replay != null)
        {
            spectatorData.GameReplays.Remove(replay);
            foreach (string playerID in replay.BlueTeamPlayers)
            {
                if (spectatorData.PlayerReplays.ContainsKey(playerID))
                {
                    spectatorData.PlayerReplays[playerID].Remove(replay);
                }
            }
            foreach (string playerID in replay.RedTeamPlayers)
            {
                if (spectatorData.PlayerReplays.ContainsKey(playerID))
                {
                    spectatorData.PlayerReplays[playerID].Remove(replay);
                }
            }
            dataManager.SaveSpectatorData();
            Debug.Log("删除游戏回放成功");
        }
    }

    public string StartLiveStream(string streamerID, string streamerName, string streamTitle, string streamType, string streamURL, string thumbnailURL = "")
    {
        string streamID = "stream_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        LiveStream stream = new LiveStream(streamID, streamerID, streamerName, streamTitle, streamType, streamURL);
        stream.ThumbnailURL = thumbnailURL;
        spectatorData.AddLiveStream(stream);
        dataManager.CreateSpectatorEvent("stream_start", streamerID, streamID, "开始直播");
        dataManager.SaveSpectatorData();
        Debug.Log("开始直播成功: " + streamTitle);
        return streamID;
    }

    public void EndLiveStream(string streamID)
    {
        LiveStream stream = spectatorData.LiveStreams.Find(s => s.StreamID == streamID);
        if (stream != null && stream.IsLive)
        {
            stream.IsLive = false;
            stream.EndTime = DateTime.Now;
            dataManager.CreateSpectatorEvent("stream_end", stream.StreamerID, streamID, "结束直播");
            dataManager.SaveSpectatorData();
            Debug.Log("结束直播成功");
        }
    }

    public List<LiveStream> GetLiveStreams()
    {
        return spectatorData.LiveStreams.FindAll(s => s.IsLive);
    }

    public LiveStream GetLiveStream(string streamID)
    {
        return spectatorData.LiveStreams.Find(s => s.StreamID == streamID);
    }

    public void UpdateStreamViewerCount(string streamID, int viewerCount)
    {
        LiveStream stream = GetLiveStream(streamID);
        if (stream != null && stream.IsLive)
        {
            stream.ViewerCount = viewerCount;
            dataManager.SaveSpectatorData();
        }
    }

    public SpectatorSetting GetPlayerSpectatorSettings(string playerID)
    {
        if (!spectatorData.PlayerSettings.ContainsKey(playerID))
        {
            SpectatorSetting setting = new SpectatorSetting(playerID);
            spectatorData.AddPlayerSetting(playerID, setting);
        }
        return spectatorData.PlayerSettings[playerID];
    }

    public void UpdateSpectatorSettings(string playerID, bool enableChat, bool enableSound, bool enableQualityAuto, string videoQuality, bool enableNotifications)
    {
        SpectatorSetting setting = GetPlayerSpectatorSettings(playerID);
        setting.EnableChat = enableChat;
        setting.EnableSound = enableSound;
        setting.EnableQualityAuto = enableQualityAuto;
        setting.VideoQuality = videoQuality;
        setting.EnableNotifications = enableNotifications;
        setting.LastUpdateTime = DateTime.Now;
        dataManager.SaveSpectatorData();
        Debug.Log("更新观战设置成功");
    }

    public void CleanupInactiveSpectators()
    {
        List<Spectator> inactiveSpectators = spectatorData.Spectators.FindAll(s => !s.IsActive);
        foreach (Spectator spectator in inactiveSpectators)
        {
            spectatorData.Spectators.Remove(spectator);
            if (spectatorData.GameSpectators.ContainsKey(spectator.GameID))
            {
                spectatorData.GameSpectators[spectator.GameID].Remove(spectator);
            }
        }
        if (inactiveSpectators.Count > 0)
        {
            dataManager.SaveSpectatorData();
            Debug.Log("清理无效观战者成功: " + inactiveSpectators.Count);
        }
    }

    public void CleanupExpiredReplays(int daysToKeep = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-daysToKeep);
        List<GameReplay> expiredReplays = new List<GameReplay>();
        foreach (GameReplay replay in spectatorData.GameReplays)
        {
            if (replay.GameTime < cutoffDate)
            {
                expiredReplays.Add(replay);
            }
        }
        
        foreach (GameReplay replay in expiredReplays)
        {
            spectatorData.GameReplays.Remove(replay);
            foreach (string playerID in replay.BlueTeamPlayers)
            {
                if (spectatorData.PlayerReplays.ContainsKey(playerID))
                {
                    spectatorData.PlayerReplays[playerID].Remove(replay);
                }
            }
            foreach (string playerID in replay.RedTeamPlayers)
            {
                if (spectatorData.PlayerReplays.ContainsKey(playerID))
                {
                    spectatorData.PlayerReplays[playerID].Remove(replay);
                }
            }
        }
        
        if (expiredReplays.Count > 0)
        {
            dataManager.SaveSpectatorData();
            Debug.Log("清理过期回放成功: " + expiredReplays.Count);
        }
    }

    public void CleanupEndedStreams()
    {
        List<LiveStream> endedStreams = spectatorData.LiveStreams.FindAll(s => !s.IsLive);
        foreach (LiveStream stream in endedStreams)
        {
            spectatorData.LiveStreams.Remove(stream);
        }
        if (endedStreams.Count > 0)
        {
            dataManager.SaveSpectatorData();
            Debug.Log("清理已结束直播成功: " + endedStreams.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveSpectatorData();
    }

    public void LoadData()
    {
        dataManager.LoadSpectatorData();
    }

    public List<SpectatorEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
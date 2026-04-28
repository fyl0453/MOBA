using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Spectator
{
    public string SpectatorID;
    public string PlayerID;
    public string PlayerName;
    public string GameID;
    public DateTime JoinTime;
    public DateTime LeaveTime;
    public string SpectatorType;
    public bool IsActive;

    public Spectator(string spectatorID, string playerID, string playerName, string gameID, string spectatorType)
    {
        SpectatorID = spectatorID;
        PlayerID = playerID;
        PlayerName = playerName;
        GameID = gameID;
        JoinTime = DateTime.Now;
        LeaveTime = DateTime.MinValue;
        SpectatorType = spectatorType;
        IsActive = true;
    }
}

[Serializable]
public class GameReplay
{
    public string ReplayID;
    public string GameID;
    public string MatchType;
    public List<string> BlueTeamPlayers;
    public List<string> RedTeamPlayers;
    public DateTime GameTime;
    public int Duration;
    public string WinnerTeam;
    public string ReplayPath;
    public long FileSize;
    public bool IsAvailable;
    public int ViewCount;

    public GameReplay(string replayID, string gameID, string matchType, List<string> blueTeamPlayers, List<string> redTeamPlayers, int duration, string winnerTeam, string replayPath)
    {
        ReplayID = replayID;
        GameID = gameID;
        MatchType = matchType;
        BlueTeamPlayers = blueTeamPlayers;
        RedTeamPlayers = redTeamPlayers;
        GameTime = DateTime.Now;
        Duration = duration;
        WinnerTeam = winnerTeam;
        ReplayPath = replayPath;
        FileSize = 0;
        IsAvailable = true;
        ViewCount = 0;
    }
}

[Serializable]
public class LiveStream
{
    public string StreamID;
    public string StreamerID;
    public string StreamerName;
    public string StreamTitle;
    public string StreamType;
    public string StreamURL;
    public DateTime StartTime;
    public DateTime EndTime;
    public int ViewerCount;
    public bool IsLive;
    public string ThumbnailURL;

    public LiveStream(string streamID, string streamerID, string streamerName, string streamTitle, string streamType, string streamURL)
    {
        StreamID = streamID;
        StreamerID = streamerID;
        StreamerName = streamerName;
        StreamTitle = streamTitle;
        StreamType = streamType;
        StreamURL = streamURL;
        StartTime = DateTime.Now;
        EndTime = DateTime.MinValue;
        ViewerCount = 0;
        IsLive = true;
        ThumbnailURL = "";
    }
}

[Serializable]
public class SpectatorSetting
{
    public string PlayerID;
    public bool EnableChat;
    public bool EnableSound;
    public bool EnableQualityAuto;
    public string VideoQuality;
    public bool EnableNotifications;
    public DateTime LastUpdateTime;

    public SpectatorSetting(string playerID)
    {
        PlayerID = playerID;
        EnableChat = true;
        EnableSound = true;
        EnableQualityAuto = true;
        VideoQuality = "high";
        EnableNotifications = true;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class SpectatorSystemData
{
    public List<Spectator> Spectators;
    public List<GameReplay> GameReplays;
    public List<LiveStream> LiveStreams;
    public Dictionary<string, SpectatorSetting> PlayerSettings;
    public Dictionary<string, List<Spectator>> GameSpectators;
    public Dictionary<string, List<GameReplay>> PlayerReplays;
    public DateTime LastCleanupTime;

    public SpectatorSystemData()
    {
        Spectators = new List<Spectator>();
        GameReplays = new List<GameReplay>();
        LiveStreams = new List<LiveStream>();
        PlayerSettings = new Dictionary<string, SpectatorSetting>();
        GameSpectators = new Dictionary<string, List<Spectator>>();
        PlayerReplays = new Dictionary<string, List<GameReplay>>();
        LastCleanupTime = DateTime.Now;
    }

    public void AddSpectator(Spectator spectator)
    {
        Spectators.Add(spectator);
        if (!GameSpectators.ContainsKey(spectator.GameID))
        {
            GameSpectators[spectator.GameID] = new List<Spectator>();
        }
        GameSpectators[spectator.GameID].Add(spectator);
    }

    public void AddGameReplay(GameReplay replay)
    {
        GameReplays.Add(replay);
        foreach (string playerID in replay.BlueTeamPlayers)
        {
            if (!PlayerReplays.ContainsKey(playerID))
            {
                PlayerReplays[playerID] = new List<GameReplay>();
            }
            PlayerReplays[playerID].Add(replay);
        }
        foreach (string playerID in replay.RedTeamPlayers)
        {
            if (!PlayerReplays.ContainsKey(playerID))
            {
                PlayerReplays[playerID] = new List<GameReplay>();
            }
            PlayerReplays[playerID].Add(replay);
        }
    }

    public void AddLiveStream(LiveStream stream)
    {
        LiveStreams.Add(stream);
    }

    public void AddPlayerSetting(string playerID, SpectatorSetting setting)
    {
        PlayerSettings[playerID] = setting;
    }
}

[Serializable]
public class SpectatorEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string RelatedID;
    public string EventData;

    public SpectatorEvent(string eventID, string eventType, string playerID, string relatedID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        RelatedID = relatedID;
        EventData = eventData;
    }
}

public class SpectatorSystemDataManager
{
    private static SpectatorSystemDataManager _instance;
    public static SpectatorSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SpectatorSystemDataManager();
            }
            return _instance;
        }
    }

    public SpectatorSystemData spectatorData;
    private List<SpectatorEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SpectatorSystemDataManager()
    {
        spectatorData = new SpectatorSystemData();
        recentEvents = new List<SpectatorEvent>();
        LoadSpectatorData();
    }

    public void SaveSpectatorData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SpectatorSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, spectatorData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存观战系统数据失败: " + e.Message);
        }
    }

    public void LoadSpectatorData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SpectatorSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    spectatorData = (SpectatorSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载观战系统数据失败: " + e.Message);
            spectatorData = new SpectatorSystemData();
        }
    }

    public void CreateSpectatorEvent(string eventType, string playerID, string relatedID, string eventData)
    {
        string eventID = "spectator_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SpectatorEvent spectatorEvent = new SpectatorEvent(eventID, eventType, playerID, relatedID, eventData);
        recentEvents.Add(spectatorEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<SpectatorEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
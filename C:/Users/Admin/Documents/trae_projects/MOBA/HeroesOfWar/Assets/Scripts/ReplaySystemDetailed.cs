using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class ReplayFrame
{
    public string FrameID;
    public int FrameIndex;
    public float Time;
    public Dictionary<string, PlayerState> PlayerStates;
    public List<GameEvent> Events;
    public DateTime RecordTime;

    public ReplayFrame(string frameID, int frameIndex, float time)
    {
        FrameID = frameID;
        FrameIndex = frameIndex;
        Time = time;
        PlayerStates = new Dictionary<string, PlayerState>();
        Events = new List<GameEvent>();
        RecordTime = DateTime.Now;
    }
}

[Serializable]
public class PlayerState
{
    public string PlayerID;
    public string HeroID;
    public float PositionX;
    public float PositionY;
    public float Rotation;
    public int Health;
    public int Mana;
    public int Level;
    public List<string> Buffs;
    public List<string> Debuffs;
    public int Gold;
    public int Kills;
    public int Deaths;
    public int Assists;

    public PlayerState(string playerID, string heroID, float positionX, float positionY, float rotation)
    {
        PlayerID = playerID;
        HeroID = heroID;
        PositionX = positionX;
        PositionY = positionY;
        Rotation = rotation;
        Health = 100;
        Mana = 100;
        Level = 1;
        Buffs = new List<string>();
        Debuffs = new List<string>();
        Gold = 0;
        Kills = 0;
        Deaths = 0;
        Assists = 0;
    }
}

[Serializable]
public class GameEvent
{
    public string EventID;
    public string EventType;
    public string PlayerID;
    public string TargetID;
    public string EventData;
    public float EventTime;
    public string HeroID;

    public GameEvent(string eventID, string eventType, string playerID, float eventTime, string eventData = "")
    {
        EventID = eventID;
        EventType = eventType;
        PlayerID = playerID;
        TargetID = "";
        EventData = eventData;
        EventTime = eventTime;
        HeroID = "";
    }
}

[Serializable]
public class ReplayData
{
    public string ReplayID;
    public string MatchID;
    public string MatchType;
    public string WinnerTeam;
    public float Duration;
    public DateTime MatchTime;
    public List<ReplayParticipant> Participants;
    public List<ReplayFrame> Frames;
    public List<ReplayClip> HighlightClips;
    public string ReplayPath;
    public long FileSize;
    public bool IsSaved;

    public ReplayData(string replayID, string matchID, string matchType, float duration)
    {
        ReplayID = replayID;
        MatchID = matchID;
        MatchType = matchType;
        WinnerTeam = "";
        Duration = duration;
        MatchTime = DateTime.Now;
        Participants = new List<ReplayParticipant>();
        Frames = new List<ReplayFrame>();
        HighlightClips = new List<ReplayClip>();
        ReplayPath = "";
        FileSize = 0;
        IsSaved = false;
    }
}

[Serializable]
public class ReplayParticipant
{
    public string PlayerID;
    public string PlayerName;
    public string HeroID;
    public string HeroName;
    public string Team;
    public int Kills;
    public int Deaths;
    public int Assists;
    public int Gold;
    public int DamageDealt;
    public int DamageTaken;
    public int HealDone;
    public bool IsWinner;

    public ReplayParticipant(string playerID, string playerName, string heroID, string heroName, string team)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        HeroID = heroID;
        HeroName = heroName;
        Team = team;
        Kills = 0;
        Deaths = 0;
        Assists = 0;
        Gold = 0;
        DamageDealt = 0;
        DamageTaken = 0;
        HealDone = 0;
        IsWinner = false;
    }
}

[Serializable]
public class ReplayClip
{
    public string ClipID;
    public string ReplayID;
    public string ClipName;
    public float StartTime;
    public float EndTime;
    public float Duration;
    public string ClipType;
    public string ThumbnailPath;
    public DateTime CreateTime;
    public bool IsFavorite;

    public ReplayClip(string clipID, string replayID, string clipName, float startTime, float endTime, string clipType)
    {
        ClipID = clipID;
        ReplayID = replayID;
        ClipName = clipName;
        StartTime = startTime;
        EndTime = endTime;
        Duration = endTime - startTime;
        ClipType = clipType;
        ThumbnailPath = "";
        CreateTime = DateTime.Now;
        IsFavorite = false;
    }
}

[Serializable]
public class PlayerReplayData
{
    public string PlayerID;
    public List<ReplayData> SavedReplays;
    public List<ReplayClip> SavedClips;
    public List<string> FavoriteReplays;
    public List<string> FavoriteClips;
    public int TotalReplays;
    public int TotalClips;
    public DateTime LastReplayTime;

    public PlayerReplayData(string playerID)
    {
        PlayerID = playerID;
        SavedReplays = new List<ReplayData>();
        SavedClips = new List<ReplayClip>();
        FavoriteReplays = new List<string>();
        FavoriteClips = new List<string>();
        TotalReplays = 0;
        TotalClips = 0;
        LastReplayTime = DateTime.MinValue;
    }
}

[Serializable]
public class ReplaySystemData
{
    public List<ReplayData> AllReplays;
    public List<ReplayClip> AllClips;
    public Dictionary<string, PlayerReplayData> PlayerReplayData;
    public int MaxReplaysPerPlayer;
    public int MaxClipsPerReplay;
    public long MaxReplaySize;
    public int ReplayRetentionDays;
    public DateTime LastSystemCleanup;

    public ReplaySystemData()
    {
        AllReplays = new List<ReplayData>();
        AllClips = new List<ReplayClip>();
        PlayerReplayData = new Dictionary<string, PlayerReplayData>();
        MaxReplaysPerPlayer = 50;
        MaxClipsPerReplay = 10;
        MaxReplaySize = 1024 * 1024 * 100;
        ReplayRetentionDays = 30;
        LastSystemCleanup = DateTime.Now;
    }

    public void AddReplay(ReplayData replay)
    {
        AllReplays.Add(replay);
    }

    public void AddClip(ReplayClip clip)
    {
        AllClips.Add(clip);
    }

    public void AddPlayerReplayData(string playerID, PlayerReplayData data)
    {
        PlayerReplayData[playerID] = data;
    }
}

[Serializable]
public class ReplayEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string ReplayID;
    public string EventData;

    public ReplayEvent(string eventID, string eventType, string playerID, string replayID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        ReplayID = replayID;
        EventData = eventData;
    }
}

public class ReplaySystemDataManager
{
    private static ReplaySystemDataManager _instance;
    public static ReplaySystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ReplaySystemDataManager();
            }
            return _instance;
        }
    }

    public ReplaySystemData replayData;
    private List<ReplayEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private ReplaySystemDataManager()
    {
        replayData = new ReplaySystemData();
        recentEvents = new List<ReplayEvent>();
        LoadReplayData();
    }

    public void SaveReplayData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ReplaySystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, replayData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存回放系统数据失败: " + e.Message);
        }
    }

    public void LoadReplayData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ReplaySystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    replayData = (ReplaySystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载回放系统数据失败: " + e.Message);
            replayData = new ReplaySystemData();
        }
    }

    public void CreateReplayEvent(string eventType, string playerID, string replayID, string eventData)
    {
        string eventID = "replay_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ReplayEvent replayEvent = new ReplayEvent(eventID, eventType, playerID, replayID, eventData);
        recentEvents.Add(replayEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<ReplayEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
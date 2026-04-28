using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public abstract class BaseSystemData
{
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public BaseSystemData()
    {
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public abstract class BaseEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EventData;

    public BaseEvent(string eventID, string eventType, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public abstract class BaseSystemDataManager<TSystemData, TEvent>
    where TSystemData : BaseSystemData, new()
    where TEvent : BaseEvent
{
    private static BaseSystemDataManager<TSystemData, TEvent> _instance;
    public static BaseSystemDataManager<TSystemData, TEvent> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (BaseSystemDataManager<TSystemData, TEvent>)Activator.CreateInstance(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
            return _instance;
        }
    }

    public TSystemData systemData;
    private List<TEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    protected BaseSystemDataManager()
    {
        systemData = new TSystemData();
        recentEvents = new List<TEvent>();
        LoadData();
    }

    public abstract string GetDataFilePath();

    public void SaveData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", GetDataFilePath());
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, systemData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存系统数据失败: " + e.Message);
        }
    }

    public void LoadData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", GetDataFilePath());
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    systemData = (TSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载系统数据失败: " + e.Message);
            systemData = new TSystemData();
        }
    }

    public void CreateEvent(string eventType, string playerID, string eventData)
    {
        string eventID = $"event_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        TEvent ev = (TEvent)Activator.CreateInstance(typeof(TEvent), eventID, eventType, playerID, eventData);
        recentEvents.Add(ev);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<TEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public abstract class BaseSystemDetailedManager<TSystemData, TDataManager>
    where TSystemData : BaseSystemData
    where TDataManager : BaseSystemDataManager<TSystemData, BaseEvent>, new()
{
    private static BaseSystemDetailedManager<TSystemData, TDataManager> _instance;
    public static BaseSystemDetailedManager<TSystemData, TDataManager> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (BaseSystemDetailedManager<TSystemData, TDataManager>)Activator.CreateInstance(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
            return _instance;
        }
    }

    protected TSystemData systemData;
    protected TDataManager dataManager;

    protected BaseSystemDetailedManager()
    {
        dataManager = TDataManager.Instance;
        systemData = dataManager.systemData;
    }

    public void SaveData()
    {
        dataManager.SaveData();
    }

    public void LoadData()
    {
        dataManager.LoadData();
    }

    public List<BaseEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class KingCareerSystemData : BaseSystemData
{
    public string PlayerID;
    public string PlayerName;
    public int TotalGames;
    public int TotalWins;
    public int TotalLosses;
    public int TotalKills;
    public int TotalDeaths;
    public int TotalAssists;
    public float KDA;
    public int HighestKillStreak;
    public int TotalDamageDealt;
    public int TotalDamageTaken;
    public int TotalHealingDone;
    public int TotalGoldEarned;
    public int TotalMinionsKilled;
    public int TotalTowersDestroyed;
    public int TotalDragonsKilled;
    public int TotalBaronsKilled;
    public List<string> Achievements;

    public KingCareerSystemData(string playerID, string playerName)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        TotalGames = 0;
        TotalWins = 0;
        TotalLosses = 0;
        TotalKills = 0;
        TotalDeaths = 0;
        TotalAssists = 0;
        KDA = 0;
        HighestKillStreak = 0;
        TotalDamageDealt = 0;
        TotalDamageTaken = 0;
        TotalHealingDone = 0;
        TotalGoldEarned = 0;
        TotalMinionsKilled = 0;
        TotalTowersDestroyed = 0;
        TotalDragonsKilled = 0;
        TotalBaronsKilled = 0;
        Achievements = new List<string>();
    }
}

[Serializable]
public class CareerEvent : BaseEvent
{
    public CareerEvent(string eventID, string eventType, string playerID, string eventData) : base(eventID, eventType, playerID, eventData)
    {}
}

public class KingCareerSystemDataManager : BaseSystemDataManager<KingCareerSystemData, CareerEvent>
{
    private static KingCareerSystemDataManager _instance;
    public static new KingCareerSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new KingCareerSystemDataManager();
            }
            return _instance;
        }
    }

    private KingCareerSystemDataManager() : base()
    {
        if (systemData.PlayerID == null)
        {
            systemData = new KingCareerSystemData("player_001", "Player");
        }
    }

    public override string GetDataFilePath()
    {
        return "KingCareerSystemData.dat";
    }

    public void CreateCareerEvent(string eventType, string playerID, string eventData)
    {
        CreateEvent(eventType, playerID, eventData);
    }

    public new List<CareerEvent> GetRecentEvents(int count)
    {
        return base.GetRecentEvents(count);
    }
}

public class KingCareerSystemDetailedManager : BaseSystemDetailedManager<KingCareerSystemData, KingCareerSystemDataManager>
{
    private static KingCareerSystemDetailedManager _instance;
    public static new KingCareerSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new KingCareerSystemDetailedManager();
            }
            return _instance;
        }
    }

    private KingCareerSystemDetailedManager() : base()
    {}

    public void UpdateMatchResult(bool isWin, int kills, int deaths, int assists, int damageDealt, int damageTaken, int healingDone, int goldEarned, int minionsKilled, int towersDestroyed, int dragonsKilled, int baronsKilled, int killStreak)
    {
        systemData.TotalGames++;
        if (isWin)
        {
            systemData.TotalWins++;
        }
        else
        {
            systemData.TotalLosses++;
        }
        systemData.TotalKills += kills;
        systemData.TotalDeaths += deaths;
        systemData.TotalAssists += assists;
        systemData.TotalDamageDealt += damageDealt;
        systemData.TotalDamageTaken += damageTaken;
        systemData.TotalHealingDone += healingDone;
        systemData.TotalGoldEarned += goldEarned;
        systemData.TotalMinionsKilled += minionsKilled;
        systemData.TotalTowersDestroyed += towersDestroyed;
        systemData.TotalDragonsKilled += dragonsKilled;
        systemData.TotalBaronsKilled += baronsKilled;

        if (killStreak > systemData.HighestKillStreak)
        {
            systemData.HighestKillStreak = killStreak;
        }

        UpdateKDA();
        CheckAchievements();

        systemData.LastUpdateTime = DateTime.Now;
        dataManager.SaveData();
    }

    private void UpdateKDA()
    {
        if (systemData.TotalDeaths > 0)
        {
            systemData.KDA = (float)(systemData.TotalKills + systemData.TotalAssists) / systemData.TotalDeaths;
        }
        else
        {
            systemData.KDA = systemData.TotalKills + systemData.TotalAssists;
        }
    }

    private void CheckAchievements()
    {
        if (systemData.TotalGames >= 100 && !systemData.Achievements.Contains("百场老将"))
        {
            systemData.Achievements.Add("百场老将");
            dataManager.CreateCareerEvent("achievement", systemData.PlayerID, "获得成就: 百场老将");
        }

        if (systemData.TotalWins >= 50 && !systemData.Achievements.Contains("胜利大师"))
        {
            systemData.Achievements.Add("胜利大师");
            dataManager.CreateCareerEvent("achievement", systemData.PlayerID, "获得成就: 胜利大师");
        }

        if (systemData.TotalKills >= 1000 && !systemData.Achievements.Contains("杀戮之王"))
        {
            systemData.Achievements.Add("杀戮之王");
            dataManager.CreateCareerEvent("achievement", systemData.PlayerID, "获得成就: 杀戮之王");
        }

        if (systemData.KDA >= 3.0f && !systemData.Achievements.Contains("KDA达人"))
        {
            systemData.Achievements.Add("KDA达人");
            dataManager.CreateCareerEvent("achievement", systemData.PlayerID, "获得成就: KDA达人");
        }

        if (systemData.HighestKillStreak >= 10 && !systemData.Achievements.Contains("连杀高手"))
        {
            systemData.Achievements.Add("连杀高手");
            dataManager.CreateCareerEvent("achievement", systemData.PlayerID, "获得成就: 连杀高手");
        }
    }

    public float GetWinRate()
    {
        if (systemData.TotalGames > 0)
        {
            return (float)systemData.TotalWins / systemData.TotalGames * 100;
        }
        return 0;
    }

    public string GetPlayerLevel()
    {
        if (systemData.TotalGames >= 1000)
        {
            return "王者荣耀";
        }
        else if (systemData.TotalGames >= 500)
        {
            return "永恒钻石";
        }
        else if (systemData.TotalGames >= 200)
        {
            return "尊贵铂金";
        }
        else if (systemData.TotalGames >= 100)
        {
            return "荣耀黄金";
        }
        else if (systemData.TotalGames >= 50)
        {
            return "秩序白银";
        }
        else
        {
            return "倔强青铜";
        }
    }

    public KingCareerSystemData GetCareerData()
    {
        return systemData;
    }

    public new List<CareerEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class MythicWorkshopMap
{
    public string MapID;
    public string MapName;
    public string MapDescription;
    public string CreatorID;
    public string CreatorName;
    public int Width;
    public int Height;
    public string MapData;
    public int PlayCount;
    public int LikeCount;
    public DateTime CreateTime;
    public DateTime LastUpdateTime;

    public MythicWorkshopMap(string mapID, string mapName, string mapDescription, string creatorID, string creatorName, int width, int height, string mapData)
    {
        MapID = mapID;
        MapName = mapName;
        MapDescription = mapDescription;
        CreatorID = creatorID;
        CreatorName = creatorName;
        Width = width;
        Height = height;
        MapData = mapData;
        PlayCount = 0;
        LikeCount = 0;
        CreateTime = DateTime.Now;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class MythicWorkshopGameMode
{
    public string ModeID;
    public string ModeName;
    public string ModeDescription;
    public string CreatorID;
    public string CreatorName;
    public string RulesData;
    public int PlayCount;
    public int LikeCount;
    public DateTime CreateTime;
    public DateTime LastUpdateTime;

    public MythicWorkshopGameMode(string modeID, string modeName, string modeDescription, string creatorID, string creatorName, string rulesData)
    {
        ModeID = modeID;
        ModeName = modeName;
        ModeDescription = modeDescription;
        CreatorID = creatorID;
        CreatorName = creatorName;
        RulesData = rulesData;
        PlayCount = 0;
        LikeCount = 0;
        CreateTime = DateTime.Now;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class MythicWorkshopSystemData
{
    public List<MythicWorkshopMap> UserCreatedMaps;
    public List<MythicWorkshopGameMode> UserCreatedModes;
    public Dictionary<string, List<string>> UserCreations;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public MythicWorkshopSystemData()
    {
        UserCreatedMaps = new List<MythicWorkshopMap>();
        UserCreatedModes = new List<MythicWorkshopGameMode>();
        UserCreations = new Dictionary<string, List<string>>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class WorkshopEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string UserID;
    public string ContentID;
    public string EventData;

    public WorkshopEvent(string eventID, string eventType, string userID, string contentID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        UserID = userID;
        ContentID = contentID;
        EventData = eventData;
    }
}

public class MythicWorkshopSystemDataManager
{
    private static MythicWorkshopSystemDataManager _instance;
    public static MythicWorkshopSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MythicWorkshopSystemDataManager();
            }
            return _instance;
        }
    }

    public MythicWorkshopSystemData workshopData;
    private List<WorkshopEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private MythicWorkshopSystemDataManager()
    {
        workshopData = new MythicWorkshopSystemData();
        recentEvents = new List<WorkshopEvent>();
        LoadWorkshopData();
    }

    public void SaveWorkshopData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MythicWorkshopSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, workshopData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存创意工坊系统数据失败: " + e.Message);
        }
    }

    public void LoadWorkshopData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MythicWorkshopSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    workshopData = (MythicWorkshopSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载创意工坊系统数据失败: " + e.Message);
            workshopData = new MythicWorkshopSystemData();
        }
    }

    public void CreateWorkshopEvent(string eventType, string userID, string contentID, string eventData)
    {
        string eventID = "workshop_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        WorkshopEvent workshopEvent = new WorkshopEvent(eventID, eventType, userID, contentID, eventData);
        recentEvents.Add(workshopEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<WorkshopEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class MythicWorkshopSystemDetailedManager
{
    private static MythicWorkshopSystemDetailedManager _instance;
    public static MythicWorkshopSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MythicWorkshopSystemDetailedManager();
            }
            return _instance;
        }
    }

    private MythicWorkshopSystemData workshopData;
    private MythicWorkshopSystemDataManager dataManager;

    private MythicWorkshopSystemDetailedManager()
    {
        dataManager = MythicWorkshopSystemDataManager.Instance;
        workshopData = dataManager.workshopData;
    }

    public string CreateCustomMap(string creatorID, string creatorName, string mapName, string mapDescription, int width, int height, string mapData)
    {
        string mapID = "map_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MythicWorkshopMap map = new MythicWorkshopMap(mapID, mapName, mapDescription, creatorID, creatorName, width, height, mapData);
        workshopData.UserCreatedMaps.Add(map);

        if (!workshopData.UserCreations.ContainsKey(creatorID))
        {
            workshopData.UserCreations[creatorID] = new List<string>();
        }
        workshopData.UserCreations[creatorID].Add(mapID);

        dataManager.CreateWorkshopEvent("map_create", creatorID, mapID, "创建地图: " + mapName);
        dataManager.SaveWorkshopData();
        Debug.Log("创建地图: " + mapName);
        return mapID;
    }

    public string CreateCustomGameMode(string creatorID, string creatorName, string modeName, string modeDescription, string rulesData)
    {
        string modeID = "mode_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MythicWorkshopGameMode mode = new MythicWorkshopGameMode(modeID, modeName, modeDescription, creatorID, creatorName, rulesData);
        workshopData.UserCreatedModes.Add(mode);

        if (!workshopData.UserCreations.ContainsKey(creatorID))
        {
            workshopData.UserCreations[creatorID] = new List<string>();
        }
        workshopData.UserCreations[creatorID].Add(modeID);

        dataManager.CreateWorkshopEvent("mode_create", creatorID, modeID, "创建游戏模式: " + modeName);
        dataManager.SaveWorkshopData();
        Debug.Log("创建游戏模式: " + modeName);
        return modeID;
    }

    public void UpdateMap(string mapID, string mapName, string mapDescription, string mapData)
    {
        MythicWorkshopMap map = workshopData.UserCreatedMaps.Find(m => m.MapID == mapID);
        if (map != null)
        {
            map.MapName = mapName;
            map.MapDescription = mapDescription;
            map.MapData = mapData;
            map.LastUpdateTime = DateTime.Now;
            dataManager.CreateWorkshopEvent("map_update", map.CreatorID, mapID, "更新地图: " + mapName);
            dataManager.SaveWorkshopData();
            Debug.Log("更新地图: " + mapName);
        }
    }

    public void UpdateGameMode(string modeID, string modeName, string modeDescription, string rulesData)
    {
        MythicWorkshopGameMode mode = workshopData.UserCreatedModes.Find(m => m.ModeID == modeID);
        if (mode != null)
        {
            mode.ModeName = modeName;
            mode.ModeDescription = modeDescription;
            mode.RulesData = rulesData;
            mode.LastUpdateTime = DateTime.Now;
            dataManager.CreateWorkshopEvent("mode_update", mode.CreatorID, modeID, "更新游戏模式: " + modeName);
            dataManager.SaveWorkshopData();
            Debug.Log("更新游戏模式: " + modeName);
        }
    }

    public void DeleteMap(string mapID)
    {
        MythicWorkshopMap map = workshopData.UserCreatedMaps.Find(m => m.MapID == mapID);
        if (map != null)
        {
            workshopData.UserCreatedMaps.Remove(map);
            if (workshopData.UserCreations.ContainsKey(map.CreatorID))
            {
                workshopData.UserCreations[map.CreatorID].Remove(mapID);
            }
            dataManager.CreateWorkshopEvent("map_delete", map.CreatorID, mapID, "删除地图: " + map.MapName);
            dataManager.SaveWorkshopData();
            Debug.Log("删除地图: " + map.MapName);
        }
    }

    public void DeleteGameMode(string modeID)
    {
        MythicWorkshopGameMode mode = workshopData.UserCreatedModes.Find(m => m.ModeID == modeID);
        if (mode != null)
        {
            workshopData.UserCreatedModes.Remove(mode);
            if (workshopData.UserCreations.ContainsKey(mode.CreatorID))
            {
                workshopData.UserCreations[mode.CreatorID].Remove(modeID);
            }
            dataManager.CreateWorkshopEvent("mode_delete", mode.CreatorID, modeID, "删除游戏模式: " + mode.ModeName);
            dataManager.SaveWorkshopData();
            Debug.Log("删除游戏模式: " + mode.ModeName);
        }
    }

    public void LikeMap(string mapID)
    {
        MythicWorkshopMap map = workshopData.UserCreatedMaps.Find(m => m.MapID == mapID);
        if (map != null)
        {
            map.LikeCount++;
            dataManager.SaveWorkshopData();
        }
    }

    public void LikeGameMode(string modeID)
    {
        MythicWorkshopGameMode mode = workshopData.UserCreatedModes.Find(m => m.ModeID == modeID);
        if (mode != null)
        {
            mode.LikeCount++;
            dataManager.SaveWorkshopData();
        }
    }

    public void PlayMap(string mapID)
    {
        MythicWorkshopMap map = workshopData.UserCreatedMaps.Find(m => m.MapID == mapID);
        if (map != null)
        {
            map.PlayCount++;
            dataManager.SaveWorkshopData();
        }
    }

    public void PlayGameMode(string modeID)
    {
        MythicWorkshopGameMode mode = workshopData.UserCreatedModes.Find(m => m.ModeID == modeID);
        if (mode != null)
        {
            mode.PlayCount++;
            dataManager.SaveWorkshopData();
        }
    }

    public List<MythicWorkshopMap> GetMaps(int count = 20)
    {
        return workshopData.UserCreatedMaps.OrderByDescending(m => m.LikeCount).Take(count).ToList();
    }

    public List<MythicWorkshopGameMode> GetGameModes(int count = 20)
    {
        return workshopData.UserCreatedModes.OrderByDescending(m => m.LikeCount).Take(count).ToList();
    }

    public List<MythicWorkshopMap> GetUserMaps(string userID)
    {
        return workshopData.UserCreatedMaps.Where(m => m.CreatorID == userID).ToList();
    }

    public List<MythicWorkshopGameMode> GetUserGameModes(string userID)
    {
        return workshopData.UserCreatedModes.Where(m => m.CreatorID == userID).ToList();
    }

    public MythicWorkshopMap GetMap(string mapID)
    {
        return workshopData.UserCreatedMaps.Find(m => m.MapID == mapID);
    }

    public MythicWorkshopGameMode GetGameMode(string modeID)
    {
        return workshopData.UserCreatedModes.Find(m => m.ModeID == modeID);
    }

    public void SaveData()
    {
        dataManager.SaveWorkshopData();
    }

    public void LoadData()
    {
        dataManager.LoadWorkshopData();
    }

    public List<WorkshopEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class VoiceMessage
{
    public string MessageID;
    public string SenderID;
    public string SenderName;
    public string ReceiverID;
    public string VoiceData;
    public string TranscribedText;
    public float Duration;
    public DateTime SendTime;
    public bool IsRead;

    public VoiceMessage(string messageID, string senderID, string senderName, string receiverID, string voiceData, string transcribedText, float duration)
    {
        MessageID = messageID;
        SenderID = senderID;
        SenderName = senderName;
        ReceiverID = receiverID;
        VoiceData = voiceData;
        TranscribedText = transcribedText;
        Duration = duration;
        SendTime = DateTime.Now;
        IsRead = false;
    }
}

[Serializable]
public class VoiceRecognitionSystemData
{
    public List<VoiceMessage> VoiceMessages;
    public Dictionary<string, List<string>> UserMessages;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public VoiceRecognitionSystemData()
    {
        VoiceMessages = new List<VoiceMessage>();
        UserMessages = new Dictionary<string, List<string>>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class VoiceEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string UserID;
    public string MessageID;
    public string EventData;

    public VoiceEvent(string eventID, string eventType, string userID, string messageID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        UserID = userID;
        MessageID = messageID;
        EventData = eventData;
    }
}

public class VoiceRecognitionSystemDataManager
{
    private static VoiceRecognitionSystemDataManager _instance;
    public static VoiceRecognitionSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new VoiceRecognitionSystemDataManager();
            }
            return _instance;
        }
    }

    public VoiceRecognitionSystemData voiceData;
    private List<VoiceEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private VoiceRecognitionSystemDataManager()
    {
        voiceData = new VoiceRecognitionSystemData();
        recentEvents = new List<VoiceEvent>();
        LoadVoiceData();
    }

    public void SaveVoiceData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "VoiceRecognitionSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, voiceData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存语音识别系统数据失败: " + e.Message);
        }
    }

    public void LoadVoiceData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "VoiceRecognitionSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    voiceData = (VoiceRecognitionSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载语音识别系统数据失败: " + e.Message);
            voiceData = new VoiceRecognitionSystemData();
        }
    }

    public void CreateVoiceEvent(string eventType, string userID, string messageID, string eventData)
    {
        string eventID = "voice_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        VoiceEvent voiceEvent = new VoiceEvent(eventID, eventType, userID, messageID, eventData);
        recentEvents.Add(voiceEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<VoiceEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class VoiceRecognitionSystemDetailedManager
{
    private static VoiceRecognitionSystemDetailedManager _instance;
    public static VoiceRecognitionSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new VoiceRecognitionSystemDetailedManager();
            }
            return _instance;
        }
    }

    private VoiceRecognitionSystemData voiceData;
    private VoiceRecognitionSystemDataManager dataManager;

    private VoiceRecognitionSystemDetailedManager()
    {
        dataManager = VoiceRecognitionSystemDataManager.Instance;
        voiceData = dataManager.voiceData;
    }

    public string SendVoiceMessage(string senderID, string senderName, string receiverID, string voiceData, string transcribedText, float duration)
    {
        string messageID = "voice_msg_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        VoiceMessage message = new VoiceMessage(messageID, senderID, senderName, receiverID, voiceData, transcribedText, duration);
        voiceData.VoiceMessages.Add(message);

        if (!voiceData.UserMessages.ContainsKey(receiverID))
        {
            voiceData.UserMessages[receiverID] = new List<string>();
        }
        voiceData.UserMessages[receiverID].Add(messageID);

        dataManager.CreateVoiceEvent("voice_send", senderID, messageID, "发送语音消息");
        dataManager.SaveVoiceData();
        Debug.Log("发送语音消息");
        return messageID;
    }

    public void MarkAsRead(string messageID)
    {
        VoiceMessage message = voiceData.VoiceMessages.Find(m => m.MessageID == messageID);
        if (message != null)
        {
            message.IsRead = true;
            dataManager.CreateVoiceEvent("voice_read", message.ReceiverID, messageID, "标记为已读");
            dataManager.SaveVoiceData();
        }
    }

    public List<VoiceMessage> GetUserMessages(string userID)
    {
        List<VoiceMessage> messages = new List<VoiceMessage>();
        if (voiceData.UserMessages.ContainsKey(userID))
        {
            foreach (string messageID in voiceData.UserMessages[userID])
            {
                VoiceMessage message = voiceData.VoiceMessages.Find(m => m.MessageID == messageID);
                if (message != null)
                {
                    messages.Add(message);
                }
            }
        }
        return messages.OrderByDescending(m => m.SendTime).ToList();
    }

    public List<VoiceMessage> GetUnreadMessages(string userID)
    {
        List<VoiceMessage> messages = GetUserMessages(userID);
        return messages.FindAll(m => !m.IsRead);
    }

    public VoiceMessage GetMessage(string messageID)
    {
        return voiceData.VoiceMessages.Find(m => m.MessageID == messageID);
    }

    public void DeleteMessage(string messageID)
    {
        VoiceMessage message = voiceData.VoiceMessages.Find(m => m.MessageID == messageID);
        if (message != null)
        {
            voiceData.VoiceMessages.Remove(message);
            if (voiceData.UserMessages.ContainsKey(message.ReceiverID))
            {
                voiceData.UserMessages[message.ReceiverID].Remove(messageID);
            }
            dataManager.CreateVoiceEvent("voice_delete", message.ReceiverID, messageID, "删除语音消息");
            dataManager.SaveVoiceData();
            Debug.Log("删除语音消息");
        }
    }

    public string TranscribeVoice(string voiceData)
    {
        return "这是一段语音消息的转录文本";
    }

    public void SaveData()
    {
        dataManager.SaveVoiceData();
    }

    public void LoadData()
    {
        dataManager.LoadVoiceData();
    }

    public List<VoiceEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class MatchmakingPlayer
{
    public string PlayerID;
    public string PlayerName;
    public int Rank;
    public float WinRate;
    public int TotalGames;
    public float KDA;
    public List<string> PreferredHeroes;
    public string RolePreference;
    public DateTime LastMatchTime;

    public MatchmakingPlayer(string playerID, string playerName, int rank, float winRate, int totalGames, float kda, List<string> preferredHeroes, string rolePreference)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        Rank = rank;
        WinRate = winRate;
        TotalGames = totalGames;
        KDA = kda;
        PreferredHeroes = preferredHeroes;
        RolePreference = rolePreference;
        LastMatchTime = DateTime.Now;
    }
}

[Serializable]
public class MatchmakingQueue
{
    public string QueueID;
    public string QueueType;
    public List<MatchmakingPlayer> Players;
    public int MinPlayers;
    public int MaxPlayers;
    public int MatchmakingTime;
    public DateTime CreateTime;

    public MatchmakingQueue(string queueID, string queueType, int minPlayers, int maxPlayers)
    {
        QueueID = queueID;
        QueueType = queueType;
        Players = new List<MatchmakingPlayer>();
        MinPlayers = minPlayers;
        MaxPlayers = maxPlayers;
        MatchmakingTime = 0;
        CreateTime = DateTime.Now;
    }
}

[Serializable]
public class MatchmakingResult
{
    public string MatchID;
    public string MatchType;
    public List<string> Team1Players;
    public List<string> Team2Players;
    public DateTime MatchTime;
    public string MatchStatus;

    public MatchmakingResult(string matchID, string matchType, List<string> team1Players, List<string> team2Players)
    {
        MatchID = matchID;
        MatchType = matchType;
        Team1Players = team1Players;
        Team2Players = team2Players;
        MatchTime = DateTime.Now;
        MatchStatus = "created";
    }
}

[Serializable]
public class SmartMatchmakingSystemData
{
    public List<MatchmakingQueue> Queues;
    public List<MatchmakingResult> MatchResults;
    public Dictionary<string, MatchmakingPlayer> PlayerData;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public SmartMatchmakingSystemData()
    {
        Queues = new List<MatchmakingQueue>();
        MatchResults = new List<MatchmakingResult>();
        PlayerData = new Dictionary<string, MatchmakingPlayer>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultQueues();
    }

    private void InitializeDefaultQueues()
    {
        Queues.Add(new MatchmakingQueue("queue_normal", "normal", 10, 10));
        Queues.Add(new MatchmakingQueue("queue_rank", "ranked", 10, 10));
        Queues.Add(new MatchmakingQueue("queue_arena", "arena", 6, 6));
    }
}

[Serializable]
public class MatchmakingEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string MatchID;
    public string EventData;

    public MatchmakingEvent(string eventID, string eventType, string playerID, string matchID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        MatchID = matchID;
        EventData = eventData;
    }
}

public class SmartMatchmakingSystemDataManager
{
    private static SmartMatchmakingSystemDataManager _instance;
    public static SmartMatchmakingSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SmartMatchmakingSystemDataManager();
            }
            return _instance;
        }
    }

    public SmartMatchmakingSystemData matchmakingData;
    private List<MatchmakingEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SmartMatchmakingSystemDataManager()
    {
        matchmakingData = new SmartMatchmakingSystemData();
        recentEvents = new List<MatchmakingEvent>();
        LoadMatchmakingData();
    }

    public void SaveMatchmakingData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SmartMatchmakingSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, matchmakingData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存智能匹配系统数据失败: " + e.Message);
        }
    }

    public void LoadMatchmakingData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SmartMatchmakingSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    matchmakingData = (SmartMatchmakingSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载智能匹配系统数据失败: " + e.Message);
            matchmakingData = new SmartMatchmakingSystemData();
        }
    }

    public void CreateMatchmakingEvent(string eventType, string playerID, string matchID, string eventData)
    {
        string eventID = "matchmaking_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MatchmakingEvent matchmakingEvent = new MatchmakingEvent(eventID, eventType, playerID, matchID, eventData);
        recentEvents.Add(matchmakingEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<MatchmakingEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class SmartMatchmakingSystemDetailedManager
{
    private static SmartMatchmakingSystemDetailedManager _instance;
    public static SmartMatchmakingSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SmartMatchmakingSystemDetailedManager();
            }
            return _instance;
        }
    }

    private SmartMatchmakingSystemData matchmakingData;
    private SmartMatchmakingSystemDataManager dataManager;

    private SmartMatchmakingSystemDetailedManager()
    {
        dataManager = SmartMatchmakingSystemDataManager.Instance;
        matchmakingData = dataManager.matchmakingData;
    }

    public void RegisterPlayer(string playerID, string playerName, int rank, float winRate, int totalGames, float kda, List<string> preferredHeroes, string rolePreference)
    {
        if (!matchmakingData.PlayerData.ContainsKey(playerID))
        {
            MatchmakingPlayer player = new MatchmakingPlayer(playerID, playerName, rank, winRate, totalGames, kda, preferredHeroes, rolePreference);
            matchmakingData.PlayerData[playerID] = player;
            dataManager.SaveMatchmakingData();
        }
    }

    public string EnterQueue(string playerID, string queueType)
    {
        if (!matchmakingData.PlayerData.ContainsKey(playerID))
        {
            return null;
        }

        MatchmakingQueue queue = matchmakingData.Queues.Find(q => q.QueueType == queueType);
        if (queue != null)
        {
            MatchmakingPlayer player = matchmakingData.PlayerData[playerID];
            queue.Players.Add(player);
            dataManager.CreateMatchmakingEvent("enter_queue", playerID, "", "进入匹配队列: " + queueType);
            dataManager.SaveMatchmakingData();
            Debug.Log("玩家 " + player.PlayerName + " 进入匹配队列: " + queueType);
            
            TryMatchmaking(queue);
            return queue.QueueID;
        }
        return null;
    }

    public void LeaveQueue(string playerID, string queueType)
    {
        MatchmakingQueue queue = matchmakingData.Queues.Find(q => q.QueueType == queueType);
        if (queue != null)
        {
            MatchmakingPlayer player = queue.Players.Find(p => p.PlayerID == playerID);
            if (player != null)
            {
                queue.Players.Remove(player);
                dataManager.CreateMatchmakingEvent("leave_queue", playerID, "", "离开匹配队列: " + queueType);
                dataManager.SaveMatchmakingData();
                Debug.Log("玩家 " + player.PlayerName + " 离开匹配队列: " + queueType);
            }
        }
    }

    private void TryMatchmaking(MatchmakingQueue queue)
    {
        if (queue.Players.Count >= queue.MinPlayers)
        {
            List<MatchmakingPlayer> sortedPlayers = queue.Players.OrderBy(p => p.Rank).ToList();
            List<string> team1 = new List<string>();
            List<string> team2 = new List<string>();

            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                if (i % 2 == 0)
                {
                    team1.Add(sortedPlayers[i].PlayerID);
                }
                else
                {
                    team2.Add(sortedPlayers[i].PlayerID);
                }
            }

            string matchID = "match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            MatchmakingResult result = new MatchmakingResult(matchID, queue.QueueType, team1, team2);
            matchmakingData.MatchResults.Add(result);

            queue.Players.Clear();
            dataManager.CreateMatchmakingEvent("match_found", "system", matchID, "匹配成功: " + queue.QueueType);
            dataManager.SaveMatchmakingData();
            Debug.Log("匹配成功: " + queue.QueueType + " 匹配ID: " + matchID);
        }
    }

    public MatchmakingResult GetMatchResult(string matchID)
    {
        return matchmakingData.MatchResults.Find(m => m.MatchID == matchID);
    }

    public List<MatchmakingResult> GetRecentMatches(int count = 10)
    {
        return matchmakingData.MatchResults.OrderByDescending(m => m.MatchTime).Take(count).ToList();
    }

    public MatchmakingPlayer GetPlayerData(string playerID)
    {
        if (matchmakingData.PlayerData.ContainsKey(playerID))
        {
            return matchmakingData.PlayerData[playerID];
        }
        return null;
    }

    public void UpdatePlayerStats(string playerID, int rank, float winRate, int totalGames, float kda)
    {
        if (matchmakingData.PlayerData.ContainsKey(playerID))
        {
            MatchmakingPlayer player = matchmakingData.PlayerData[playerID];
            player.Rank = rank;
            player.WinRate = winRate;
            player.TotalGames = totalGames;
            player.KDA = kda;
            player.LastMatchTime = DateTime.Now;
            dataManager.SaveMatchmakingData();
        }
    }

    public void SaveData()
    {
        dataManager.SaveMatchmakingData();
    }

    public void LoadData()
    {
        dataManager.LoadMatchmakingData();
    }

    public List<MatchmakingEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class HonorZoneData
{
    public string ZoneID;
    public string ZoneName;
    public string ZoneType;
    public int RankLevel;
    public int HonorPoints;
    public string PlayerID;
    public string PlayerName;
    public string HeroID;
    public int HeroRank;
    public DateTime LastUpdateTime;

    public HonorZoneData(string zoneID, string zoneName, string zoneType, string playerID, string playerName, string heroID)
    {
        ZoneID = zoneID;
        ZoneName = zoneName;
        ZoneType = zoneType;
        RankLevel = 1;
        HonorPoints = 0;
        PlayerID = playerID;
        PlayerName = playerName;
        HeroID = heroID;
        HeroRank = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class HonorZoneSystemData
{
    public Dictionary<string, List<HonorZoneData>> PlayerZones;
    public Dictionary<string, List<HonorZoneData>> HeroRanks;
    public Dictionary<string, List<HonorZoneData>> GlobalZones;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public HonorZoneSystemData()
    {
        PlayerZones = new Dictionary<string, List<HonorZoneData>>();
        HeroRanks = new Dictionary<string, List<HonorZoneData>>();
        GlobalZones = new Dictionary<string, List<HonorZoneData>>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class HonorZoneEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EventData;

    public HonorZoneEvent(string eventID, string eventType, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class HonorZoneSystemDataManager
{
    private static HonorZoneSystemDataManager _instance;
    public static HonorZoneSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HonorZoneSystemDataManager();
            }
            return _instance;
        }
    }

    public HonorZoneSystemData honorData;
    private List<HonorZoneEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private HonorZoneSystemDataManager()
    {
        honorData = new HonorZoneSystemData();
        recentEvents = new List<HonorZoneEvent>();
        LoadHonorData();
    }

    public void SaveHonorData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "HonorZoneSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, honorData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存荣耀战区系统数据失败: " + e.Message);
        }
    }

    public void LoadHonorData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "HonorZoneSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    honorData = (HonorZoneSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载荣耀战区系统数据失败: " + e.Message);
            honorData = new HonorZoneSystemData();
        }
    }

    public void CreateHonorEvent(string eventType, string playerID, string eventData)
    {
        string eventID = "honor_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HonorZoneEvent honorEvent = new HonorZoneEvent(eventID, eventType, playerID, eventData);
        recentEvents.Add(honorEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<HonorZoneEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class HonorZoneSystemDetailedManager
{
    private static HonorZoneSystemDetailedManager _instance;
    public static HonorZoneSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HonorZoneSystemDetailedManager();
            }
            return _instance;
        }
    }

    private HonorZoneSystemData honorData;
    private HonorZoneSystemDataManager dataManager;

    private HonorZoneSystemDetailedManager()
    {
        dataManager = HonorZoneSystemDataManager.Instance;
        honorData = dataManager.honorData;
    }

    public string RegisterZone(string playerID, string playerName, string zoneName, string zoneType, string heroID)
    {
        string zoneID = "zone_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HonorZoneData zoneData = new HonorZoneData(zoneID, zoneName, zoneType, playerID, playerName, heroID);

        if (!honorData.PlayerZones.ContainsKey(playerID))
        {
            honorData.PlayerZones[playerID] = new List<HonorZoneData>();
        }
        honorData.PlayerZones[playerID].Add(zoneData);

        if (!honorData.HeroRanks.ContainsKey(heroID))
        {
            honorData.HeroRanks[heroID] = new List<HonorZoneData>();
        }
        honorData.HeroRanks[heroID].Add(zoneData);

        if (!honorData.GlobalZones.ContainsKey(zoneType))
        {
            honorData.GlobalZones[zoneType] = new List<HonorZoneData>();
        }
        honorData.GlobalZones[zoneType].Add(zoneData);

        dataManager.CreateHonorEvent("zone_register", playerID, "注册战区: " + zoneName);
        dataManager.SaveHonorData();
        Debug.Log("注册战区成功: " + zoneName);
        return zoneID;
    }

    public void UpdateHonorPoints(string playerID, string zoneID, int points)
    {
        foreach (var zoneData in honorData.PlayerZones[playerID])
        {
            if (zoneData.ZoneID == zoneID)
            {
                zoneData.HonorPoints += points;
                zoneData.LastUpdateTime = DateTime.Now;
                UpdateRankLevel(zoneData);
                UpdateHeroRank(zoneData);
                break;
            }
        }

        dataManager.CreateHonorEvent("honor_update", playerID, "更新荣誉值: " + points);
        dataManager.SaveHonorData();
    }

    private void UpdateRankLevel(HonorZoneData zoneData)
    {
        if (zoneData.HonorPoints >= 10000)
            zoneData.RankLevel = 5;
        else if (zoneData.HonorPoints >= 5000)
            zoneData.RankLevel = 4;
        else if (zoneData.HonorPoints >= 2000)
            zoneData.RankLevel = 3;
        else if (zoneData.HonorPoints >= 1000)
            zoneData.RankLevel = 2;
        else
            zoneData.RankLevel = 1;
    }

    private void UpdateHeroRank(HonorZoneData zoneData)
    {
        var heroRanks = honorData.HeroRanks[zoneData.HeroID];
        heroRanks.Sort((a, b) => b.HonorPoints.CompareTo(a.HonorPoints));
        zoneData.HeroRank = heroRanks.IndexOf(zoneData) + 1;
    }

    public List<HonorZoneData> GetPlayerZones(string playerID)
    {
        if (honorData.PlayerZones.ContainsKey(playerID))
        {
            return honorData.PlayerZones[playerID];
        }
        return new List<HonorZoneData>();
    }

    public List<HonorZoneData> GetHeroRanks(string heroID, int count = 10)
    {
        if (honorData.HeroRanks.ContainsKey(heroID))
        {
            var ranks = honorData.HeroRanks[heroID].OrderByDescending(z => z.HonorPoints).Take(count).ToList();
            return ranks;
        }
        return new List<HonorZoneData>();
    }

    public List<HonorZoneData> GetZoneRanks(string zoneType, int count = 10)
    {
        if (honorData.GlobalZones.ContainsKey(zoneType))
        {
            var ranks = honorData.GlobalZones[zoneType].OrderByDescending(z => z.HonorPoints).Take(count).ToList();
            return ranks;
        }
        return new List<HonorZoneData>();
    }

    public string GetHonorTitle(int rankLevel)
    {
        switch (rankLevel)
        {
            case 5: return "荣耀王者";
            case 4: return "最强王者";
            case 3: return "永恒钻石";
            case 2: return "尊贵铂金";
            case 1: return "秩序白银";
            default: return "倔强青铜";
        }
    }

    public void SaveData()
    {
        dataManager.SaveHonorData();
    }

    public void LoadData()
    {
        dataManager.LoadHonorData();
    }

    public List<HonorZoneEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class HeroMasteryData
{
    public string PlayerID;
    public string HeroID;
    public int MasteryLevel;
    public int MasteryPoints;
    public int TotalGamesPlayed;
    public int TotalWins;
    public DateTime LastPlayTime;
    public List<string> UnlockedRewards;

    public HeroMasteryData(string playerID, string heroID)
    {
        PlayerID = playerID;
        HeroID = heroID;
        MasteryLevel = 1;
        MasteryPoints = 0;
        TotalGamesPlayed = 0;
        TotalWins = 0;
        LastPlayTime = DateTime.Now;
        UnlockedRewards = new List<string>();
    }
}

[Serializable]
public class HeroMasterySystemData
{
    public Dictionary<string, Dictionary<string, HeroMasteryData>> PlayerMastery;
    public Dictionary<string, List<HeroMasteryData>> HeroLeaderboards;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public HeroMasterySystemData()
    {
        PlayerMastery = new Dictionary<string, Dictionary<string, HeroMasteryData>>();
        HeroLeaderboards = new Dictionary<string, List<HeroMasteryData>>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class MasteryEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string HeroID;
    public string EventData;

    public MasteryEvent(string eventID, string eventType, string playerID, string heroID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        HeroID = heroID;
        EventData = eventData;
    }
}

public class HeroMasterySystemDataManager
{
    private static HeroMasterySystemDataManager _instance;
    public static HeroMasterySystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HeroMasterySystemDataManager();
            }
            return _instance;
        }
    }

    public HeroMasterySystemData masteryData;
    private List<MasteryEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private HeroMasterySystemDataManager()
    {
        masteryData = new HeroMasterySystemData();
        recentEvents = new List<MasteryEvent>();
        LoadMasteryData();
    }

    public void SaveMasteryData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "HeroMasterySystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, masteryData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存英雄熟练度系统数据失败: " + e.Message);
        }
    }

    public void LoadMasteryData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "HeroMasterySystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    masteryData = (HeroMasterySystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载英雄熟练度系统数据失败: " + e.Message);
            masteryData = new HeroMasterySystemData();
        }
    }

    public void CreateMasteryEvent(string eventType, string playerID, string heroID, string eventData)
    {
        string eventID = "mastery_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MasteryEvent masteryEvent = new MasteryEvent(eventID, eventType, playerID, heroID, eventData);
        recentEvents.Add(masteryEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<MasteryEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class HeroMasterySystemDetailedManager
{
    private static HeroMasterySystemDetailedManager _instance;
    public static HeroMasterySystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HeroMasterySystemDetailedManager();
            }
            return _instance;
        }
    }

    private HeroMasterySystemData masteryData;
    private HeroMasterySystemDataManager dataManager;

    private HeroMasterySystemDetailedManager()
    {
        dataManager = HeroMasterySystemDataManager.Instance;
        masteryData = dataManager.masteryData;
    }

    public HeroMasteryData GetHeroMastery(string playerID, string heroID)
    {
        if (!masteryData.PlayerMastery.ContainsKey(playerID))
        {
            masteryData.PlayerMastery[playerID] = new Dictionary<string, HeroMasteryData>();
        }

        if (!masteryData.PlayerMastery[playerID].ContainsKey(heroID))
        {
            masteryData.PlayerMastery[playerID][heroID] = new HeroMasteryData(playerID, heroID);
        }

        return masteryData.PlayerMastery[playerID][heroID];
    }

    public void UpdateMastery(string playerID, string heroID, bool isWin, int performanceScore)
    {
        HeroMasteryData mastery = GetHeroMastery(playerID, heroID);
        mastery.TotalGamesPlayed++;
        if (isWin)
        {
            mastery.TotalWins++;
        }
        mastery.LastPlayTime = DateTime.Now;

        int pointsEarned = CalculateMasteryPoints(isWin, performanceScore, mastery.MasteryLevel);
        mastery.MasteryPoints += pointsEarned;

        CheckMasteryLevelUp(mastery);

        UpdateHeroLeaderboard(heroID, mastery);

        dataManager.CreateMasteryEvent("mastery_update", playerID, heroID, "获得熟练度: " + pointsEarned);
        dataManager.SaveMasteryData();
    }

    private int CalculateMasteryPoints(bool isWin, int performanceScore, int currentLevel)
    {
        int basePoints = isWin ? 20 : 10;
        int performanceMultiplier = Math.Max(1, performanceScore / 10);
        int levelMultiplier = currentLevel;
        return basePoints * performanceMultiplier * levelMultiplier;
    }

    private void CheckMasteryLevelUp(HeroMasteryData mastery)
    {
        int[] requiredPoints = { 0, 1000, 3000, 6000, 10000, 15000 };
        int newLevel = mastery.MasteryLevel;

        while (newLevel < requiredPoints.Length - 1 && mastery.MasteryPoints >= requiredPoints[newLevel + 1])
        {
            newLevel++;
            mastery.MasteryLevel = newLevel;
            UnlockMasteryReward(mastery);
            dataManager.CreateMasteryEvent("mastery_level_up", mastery.PlayerID, mastery.HeroID, "熟练度等级提升: " + newLevel);
            Debug.Log("英雄 " + mastery.HeroID + " 熟练度等级提升到: " + newLevel);
        }
    }

    private void UnlockMasteryReward(HeroMasteryData mastery)
    {
        string reward = GetMasteryReward(mastery.MasteryLevel);
        if (!mastery.UnlockedRewards.Contains(reward))
        {
            mastery.UnlockedRewards.Add(reward);
        }
    }

    private string GetMasteryReward(int level)
    {
        switch (level)
        {
            case 2: return "英雄称号: 见习";
            case 3: return "英雄称号: 资深";
            case 4: return "英雄称号: 精英";
            case 5: return "英雄称号: 宗师";
            case 6: return "英雄称号: 传说";
            default: return "无";
        }
    }

    private void UpdateHeroLeaderboard(string heroID, HeroMasteryData mastery)
    {
        if (!masteryData.HeroLeaderboards.ContainsKey(heroID))
        {
            masteryData.HeroLeaderboards[heroID] = new List<HeroMasteryData>();
        }

        var leaderboard = masteryData.HeroLeaderboards[heroID];
        var existingEntry = leaderboard.Find(entry => entry.PlayerID == mastery.PlayerID);

        if (existingEntry != null)
        {
            existingEntry.MasteryPoints = mastery.MasteryPoints;
            existingEntry.MasteryLevel = mastery.MasteryLevel;
        }
        else
        {
            leaderboard.Add(mastery);
        }

        leaderboard.Sort((a, b) => b.MasteryPoints.CompareTo(a.MasteryPoints));
    }

    public List<HeroMasteryData> GetPlayerMastery(string playerID)
    {
        if (masteryData.PlayerMastery.ContainsKey(playerID))
        {
            return masteryData.PlayerMastery[playerID].Values.ToList();
        }
        return new List<HeroMasteryData>();
    }

    public List<HeroMasteryData> GetHeroLeaderboard(string heroID, int count = 10)
    {
        if (masteryData.HeroLeaderboards.ContainsKey(heroID))
        {
            var leaderboard = masteryData.HeroLeaderboards[heroID].OrderByDescending(m => m.MasteryPoints).Take(count).ToList();
            return leaderboard;
        }
        return new List<HeroMasteryData>();
    }

    public double GetHeroWinRate(string playerID, string heroID)
    {
        HeroMasteryData mastery = GetHeroMastery(playerID, heroID);
        if (mastery.TotalGamesPlayed > 0)
        {
            return (double)mastery.TotalWins / mastery.TotalGamesPlayed * 100;
        }
        return 0;
    }

    public string GetMasteryTitle(int level)
    {
        switch (level)
        {
            case 6: return "传说";
            case 5: return "宗师";
            case 4: return "精英";
            case 3: return "资深";
            case 2: return "见习";
            default: return "新手";
        }
    }

    public void SaveData()
    {
        dataManager.SaveMasteryData();
    }

    public void LoadData()
    {
        dataManager.LoadMasteryData();
    }

    public List<MasteryEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class BattlePassTask
{
    public string TaskID;
    public string TaskName;
    public string TaskDescription;
    public int RequiredProgress;
    public int CurrentProgress;
    public int ExpReward;
    public int CoinReward;
    public string TaskType;
    public bool IsCompleted;
    public bool IsClaimed;
    public DateTime StartTime;
    public DateTime EndTime;

    public BattlePassTask(string taskID, string taskName, string taskDescription, int requiredProgress, int expReward, int coinReward, string taskType, DateTime startTime, DateTime endTime)
    {
        TaskID = taskID;
        TaskName = taskName;
        TaskDescription = taskDescription;
        RequiredProgress = requiredProgress;
        CurrentProgress = 0;
        ExpReward = expReward;
        CoinReward = coinReward;
        TaskType = taskType;
        IsCompleted = false;
        IsClaimed = false;
        StartTime = startTime;
        EndTime = endTime;
    }
}

[Serializable]
public class BattlePassReward
{
    public string RewardID;
    public string RewardName;
    public string RewardDescription;
    public string RewardType;
    public int RequiredLevel;
    public bool IsPremium;
    public bool IsClaimed;

    public BattlePassReward(string rewardID, string rewardName, string rewardDescription, string rewardType, int requiredLevel, bool isPremium)
    {
        RewardID = rewardID;
        RewardName = rewardName;
        RewardDescription = rewardDescription;
        RewardType = rewardType;
        RequiredLevel = requiredLevel;
        IsPremium = isPremium;
        IsClaimed = false;
    }
}

[Serializable]
public class BattlePassData
{
    public string PlayerID;
    public int CurrentLevel;
    public int CurrentExp;
    public int TotalExp;
    public int BattlePassCoins;
    public bool HasPremium;
    public List<BattlePassTask> Tasks;
    public List<BattlePassReward> Rewards;
    public DateTime LastUpdateTime;

    public BattlePassData(string playerID)
    {
        PlayerID = playerID;
        CurrentLevel = 1;
        CurrentExp = 0;
        TotalExp = 0;
        BattlePassCoins = 0;
        HasPremium = false;
        Tasks = new List<BattlePassTask>();
        Rewards = new List<BattlePassReward>();
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class BattlePassSeason
{
    public string SeasonID;
    public string SeasonName;
    public string SeasonTheme;
    public DateTime StartDate;
    public DateTime EndDate;
    public bool IsActive;

    public BattlePassSeason(string seasonID, string seasonName, string seasonTheme, DateTime startDate, DateTime endDate)
    {
        SeasonID = seasonID;
        SeasonName = seasonName;
        SeasonTheme = seasonTheme;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = false;
    }
}

[Serializable]
public class BattlePassSystemData
{
    public BattlePassSeason CurrentSeason;
    public Dictionary<string, BattlePassData> PlayerData;
    public List<BattlePassTask> GlobalTasks;
    public List<BattlePassReward> GlobalRewards;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public BattlePassSystemData()
    {
        CurrentSeason = new BattlePassSeason("season_1", "S1赛季", "王者归来", DateTime.Now, DateTime.Now.AddDays(90));
        CurrentSeason.IsActive = true;
        PlayerData = new Dictionary<string, BattlePassData>();
        GlobalTasks = new List<BattlePassTask>();
        GlobalRewards = new List<BattlePassReward>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultTasks();
        InitializeDefaultRewards();
    }

    private void InitializeDefaultTasks()
    {
        GlobalTasks.Add(new BattlePassTask("task_1", "每日首胜", "获得一场胜利", 1, 100, 10, "daily", DateTime.Now, DateTime.Now.AddDays(1)));
        GlobalTasks.Add(new BattlePassTask("task_2", "击杀达人", "累计击杀20人", 20, 200, 20, "weekly", DateTime.Now, DateTime.Now.AddDays(7)));
        GlobalTasks.Add(new BattlePassTask("task_3", "助攻能手", "累计助攻30次", 30, 150, 15, "weekly", DateTime.Now, DateTime.Now.AddDays(7)));
        GlobalTasks.Add(new BattlePassTask("task_4", "赛季挑战", "获得50场胜利", 50, 500, 50, "season", DateTime.Now, CurrentSeason.EndDate));
    }

    private void InitializeDefaultRewards()
    {
        GlobalRewards.Add(new BattlePassReward("reward_1", "金币奖励", "1000金币", "coin", 1, false));
        GlobalRewards.Add(new BattlePassReward("reward_2", "经验卡", "双倍经验卡x1", "item", 2, false));
        GlobalRewards.Add(new BattlePassReward("reward_3", "皮肤碎片", "皮肤碎片x5", "item", 3, false));
        GlobalRewards.Add(new BattlePassReward("reward_4", "精英战令", "解锁精英战令", "special", 4, true));
        GlobalRewards.Add(new BattlePassReward("reward_5", "限定皮肤", "赛季限定皮肤", "skin", 10, true));
        GlobalRewards.Add(new BattlePassReward("reward_6", "专属头像框", "赛季专属头像框", "avatar", 15, true));
        GlobalRewards.Add(new BattlePassReward("reward_7", "荣耀播报", "赛季荣耀播报", "effect", 20, true));
        GlobalRewards.Add(new BattlePassReward("reward_8", "终极奖励", "赛季终极奖励", "special", 30, true));
    }
}

[Serializable]
public class BattlePassEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EventData;

    public BattlePassEvent(string eventID, string eventType, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class BattlePassSystemDataManager
{
    private static BattlePassSystemDataManager _instance;
    public static BattlePassSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BattlePassSystemDataManager();
            }
            return _instance;
        }
    }

    public BattlePassSystemData bpData;
    private List<BattlePassEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private BattlePassSystemDataManager()
    {
        bpData = new BattlePassSystemData();
        recentEvents = new List<BattlePassEvent>();
        LoadBattlePassData();
    }

    public void SaveBattlePassData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "BattlePassSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, bpData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存战令系统数据失败: " + e.Message);
        }
    }

    public void LoadBattlePassData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "BattlePassSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bpData = (BattlePassSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载战令系统数据失败: " + e.Message);
            bpData = new BattlePassSystemData();
        }
    }

    public void CreateBattlePassEvent(string eventType, string playerID, string eventData)
    {
        string eventID = "bp_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BattlePassEvent bpEvent = new BattlePassEvent(eventID, eventType, playerID, eventData);
        recentEvents.Add(bpEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<BattlePassEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class BattlePassSystemDetailedManager
{
    private static BattlePassSystemDetailedManager _instance;
    public static BattlePassSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BattlePassSystemDetailedManager();
            }
            return _instance;
        }
    }

    private BattlePassSystemData bpData;
    private BattlePassSystemDataManager dataManager;

    private BattlePassSystemDetailedManager()
    {
        dataManager = BattlePassSystemDataManager.Instance;
        bpData = dataManager.bpData;
    }

    public BattlePassData GetPlayerBattlePass(string playerID)
    {
        if (!bpData.PlayerData.ContainsKey(playerID))
        {
            bpData.PlayerData[playerID] = new BattlePassData(playerID);
            InitializePlayerTasks(playerID);
            InitializePlayerRewards(playerID);
        }
        return bpData.PlayerData[playerID];
    }

    private void InitializePlayerTasks(string playerID)
    {
        BattlePassData data = bpData.PlayerData[playerID];
        foreach (var globalTask in bpData.GlobalTasks)
        {
            data.Tasks.Add(new BattlePassTask(
                globalTask.TaskID,
                globalTask.TaskName,
                globalTask.TaskDescription,
                globalTask.RequiredProgress,
                globalTask.ExpReward,
                globalTask.CoinReward,
                globalTask.TaskType,
                globalTask.StartTime,
                globalTask.EndTime
            ));
        }
    }

    private void InitializePlayerRewards(string playerID)
    {
        BattlePassData data = bpData.PlayerData[playerID];
        foreach (var globalReward in bpData.GlobalRewards)
        {
            data.Rewards.Add(new BattlePassReward(
                globalReward.RewardID,
                globalReward.RewardName,
                globalReward.RewardDescription,
                globalReward.RewardType,
                globalReward.RequiredLevel,
                globalReward.IsPremium
            ));
        }
    }

    public void UpdateTaskProgress(string playerID, string taskID, int progress)
    {
        BattlePassData data = GetPlayerBattlePass(playerID);
        var task = data.Tasks.Find(t => t.TaskID == taskID);
        if (task != null && !task.IsCompleted)
        {
            task.CurrentProgress += progress;
            if (task.CurrentProgress >= task.RequiredProgress)
            {
                task.CurrentProgress = task.RequiredProgress;
                task.IsCompleted = true;
                GrantTaskRewards(playerID, task);
            }
            dataManager.CreateBattlePassEvent("task_update", playerID, "任务进度更新: " + task.TaskName + " " + task.CurrentProgress + "/" + task.RequiredProgress);
            dataManager.SaveBattlePassData();
        }
    }

    private void GrantTaskRewards(string playerID, BattlePassTask task)
    {
        BattlePassData data = bpData.PlayerData[playerID];
        data.CurrentExp += task.ExpReward;
        data.TotalExp += task.ExpReward;
        data.BattlePassCoins += task.CoinReward;
        data.LastUpdateTime = DateTime.Now;
        CheckLevelUp(data);
    }

    private void CheckLevelUp(BattlePassData data)
    {
        int expPerLevel = 1000;
        while (data.CurrentExp >= expPerLevel)
        {
            data.CurrentExp -= expPerLevel;
            data.CurrentLevel++;
            dataManager.CreateBattlePassEvent("level_up", data.PlayerID, "战令等级提升: " + data.CurrentLevel);
            Debug.Log("玩家 " + data.PlayerID + " 战令等级提升到: " + data.CurrentLevel);
        }
    }

    public void ClaimReward(string playerID, string rewardID)
    {
        BattlePassData data = GetPlayerBattlePass(playerID);
        var reward = data.Rewards.Find(r => r.RewardID == rewardID);
        if (reward != null && !reward.IsClaimed && data.CurrentLevel >= reward.RequiredLevel)
        {
            if (reward.IsPremium && !data.HasPremium)
            {
                Debug.Log("需要解锁精英战令才能领取此奖励");
                return;
            }
            reward.IsClaimed = true;
            dataManager.CreateBattlePassEvent("reward_claim", playerID, "领取奖励: " + reward.RewardName);
            dataManager.SaveBattlePassData();
            Debug.Log("领取奖励成功: " + reward.RewardName);
        }
    }

    public void PurchasePremium(string playerID)
    {
        BattlePassData data = GetPlayerBattlePass(playerID);
        if (!data.HasPremium)
        {
            data.HasPremium = true;
            dataManager.CreateBattlePassEvent("premium_purchase", playerID, "购买精英战令");
            dataManager.SaveBattlePassData();
            Debug.Log("购买精英战令成功");
        }
    }

    public List<BattlePassTask> GetPlayerTasks(string playerID)
    {
        BattlePassData data = GetPlayerBattlePass(playerID);
        return data.Tasks;
    }

    public List<BattlePassReward> GetAvailableRewards(string playerID)
    {
        BattlePassData data = GetPlayerBattlePass(playerID);
        return data.Rewards.FindAll(r => data.CurrentLevel >= r.RequiredLevel && !r.IsClaimed && (!r.IsPremium || data.HasPremium));
    }

    public BattlePassSeason GetCurrentSeason()
    {
        return bpData.CurrentSeason;
    }

    public void SaveData()
    {
        dataManager.SaveBattlePassData();
    }

    public void LoadData()
    {
        dataManager.LoadBattlePassData();
    }

    public List<BattlePassEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class ClanWarMatch
{
    public string MatchID;
    public string Clan1ID;
    public string Clan1Name;
    public string Clan2ID;
    public string Clan2Name;
    public int Clan1Score;
    public int Clan2Score;
    public string WinnerID;
    public DateTime MatchTime;
    public string MatchResult;

    public ClanWarMatch(string matchID, string clan1ID, string clan1Name, string clan2ID, string clan2Name)
    {
        MatchID = matchID;
        Clan1ID = clan1ID;
        Clan1Name = clan1Name;
        Clan2ID = clan2ID;
        Clan2Name = clan2Name;
        Clan1Score = 0;
        Clan2Score = 0;
        WinnerID = "";
        MatchTime = DateTime.Now;
        MatchResult = "未开始";
    }
}

[Serializable]
public class ClanWarSeason
{
    public string SeasonID;
    public string SeasonName;
    public DateTime StartDate;
    public DateTime EndDate;
    public bool IsActive;

    public ClanWarSeason(string seasonID, string seasonName, DateTime startDate, DateTime endDate)
    {
        SeasonID = seasonID;
        SeasonName = seasonName;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = false;
    }
}

[Serializable]
public class ClanWarData
{
    public string ClanID;
    public string ClanName;
    public int TotalWins;
    public int TotalLosses;
    public int TotalMatches;
    public int WarPoints;
    public int RankPosition;
    public List<string> MatchHistory;
    public DateTime LastMatchTime;

    public ClanWarData(string clanID, string clanName)
    {
        ClanID = clanID;
        ClanName = clanName;
        TotalWins = 0;
        TotalLosses = 0;
        TotalMatches = 0;
        WarPoints = 0;
        RankPosition = 0;
        MatchHistory = new List<string>();
        LastMatchTime = DateTime.MinValue;
    }
}

[Serializable]
public class ClanWarSystemData
{
    public ClanWarSeason CurrentSeason;
    public Dictionary<string, ClanWarData> ClanWarData;
    public List<ClanWarMatch> MatchHistory;
    public Dictionary<string, List<ClanWarMatch>> ClanMatches;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public ClanWarSystemData()
    {
        CurrentSeason = new ClanWarSeason("war_season_1", "S1战队赛", DateTime.Now, DateTime.Now.AddDays(30));
        CurrentSeason.IsActive = true;
        ClanWarData = new Dictionary<string, ClanWarData>();
        MatchHistory = new List<ClanWarMatch>();
        ClanMatches = new Dictionary<string, List<ClanWarMatch>>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class ClanWarEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string ClanID;
    public string EventData;

    public ClanWarEvent(string eventID, string eventType, string clanID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        ClanID = clanID;
        EventData = eventData;
    }
}

public class ClanWarSystemDataManager
{
    private static ClanWarSystemDataManager _instance;
    public static ClanWarSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ClanWarSystemDataManager();
            }
            return _instance;
        }
    }

    public ClanWarSystemData warData;
    private List<ClanWarEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private ClanWarSystemDataManager()
    {
        warData = new ClanWarSystemData();
        recentEvents = new List<ClanWarEvent>();
        LoadClanWarData();
    }

    public void SaveClanWarData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ClanWarSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, warData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存战队赛系统数据失败: " + e.Message);
        }
    }

    public void LoadClanWarData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ClanWarSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    warData = (ClanWarSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载战队赛系统数据失败: " + e.Message);
            warData = new ClanWarSystemData();
        }
    }

    public void CreateClanWarEvent(string eventType, string clanID, string eventData)
    {
        string eventID = "war_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ClanWarEvent warEvent = new ClanWarEvent(eventID, eventType, clanID, eventData);
        recentEvents.Add(warEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<ClanWarEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class ClanWarSystemDetailedManager
{
    private static ClanWarSystemDetailedManager _instance;
    public static ClanWarSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ClanWarSystemDetailedManager();
            }
            return _instance;
        }
    }

    private ClanWarSystemData warData;
    private ClanWarSystemDataManager dataManager;

    private ClanWarSystemDetailedManager()
    {
        dataManager = ClanWarSystemDataManager.Instance;
        warData = dataManager.warData;
    }

    public ClanWarData GetClanWarData(string clanID, string clanName)
    {
        if (!warData.ClanWarData.ContainsKey(clanID))
        {
            warData.ClanWarData[clanID] = new ClanWarData(clanID, clanName);
        }
        return warData.ClanWarData[clanID];
    }

    public string CreateClanWarMatch(string clan1ID, string clan1Name, string clan2ID, string clan2Name)
    {
        string matchID = "war_match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ClanWarMatch match = new ClanWarMatch(matchID, clan1ID, clan1Name, clan2ID, clan2Name);
        warData.MatchHistory.Add(match);

        if (!warData.ClanMatches.ContainsKey(clan1ID))
        {
            warData.ClanMatches[clan1ID] = new List<ClanWarMatch>();
        }
        warData.ClanMatches[clan1ID].Add(match);

        if (!warData.ClanMatches.ContainsKey(clan2ID))
        {
            warData.ClanMatches[clan2ID] = new List<ClanWarMatch>();
        }
        warData.ClanMatches[clan2ID].Add(match);

        GetClanWarData(clan1ID, clan1Name);
        GetClanWarData(clan2ID, clan2Name);

        dataManager.CreateClanWarEvent("match_create", clan1ID, "创建战队赛: " + clan1Name + " vs " + clan2Name);
        dataManager.CreateClanWarEvent("match_create", clan2ID, "创建战队赛: " + clan2Name + " vs " + clan1Name);
        dataManager.SaveClanWarData();
        Debug.Log("创建战队赛: " + clan1Name + " vs " + clan2Name);
        return matchID;
    }

    public void UpdateMatchResult(string matchID, int clan1Score, int clan2Score, string winnerID)
    {
        ClanWarMatch match = warData.MatchHistory.Find(m => m.MatchID == matchID);
        if (match != null)
        {
            match.Clan1Score = clan1Score;
            match.Clan2Score = clan2Score;
            match.WinnerID = winnerID;
            match.MatchResult = "已结束";

            UpdateClanStats(match.Clan1ID, match.Clan2ID, winnerID);
            UpdateRankings();

            dataManager.CreateClanWarEvent("match_result", match.Clan1ID, "比赛结果: " + match.Clan1Name + " " + clan1Score + " - " + clan2Score + " " + match.Clan2Name);
            dataManager.CreateClanWarEvent("match_result", match.Clan2ID, "比赛结果: " + match.Clan2Name + " " + clan2Score + " - " + clan1Score + " " + match.Clan1Name);
            dataManager.SaveClanWarData();
            Debug.Log("比赛结果: " + match.Clan1Name + " " + clan1Score + " - " + clan2Score + " " + match.Clan2Name);
        }
    }

    private void UpdateClanStats(string clan1ID, string clan2ID, string winnerID)
    {
        ClanWarData clan1Data = warData.ClanWarData[clan1ID];
        ClanWarData clan2Data = warData.ClanWarData[clan2ID];

        clan1Data.TotalMatches++;
        clan2Data.TotalMatches++;

        if (winnerID == clan1ID)
        {
            clan1Data.TotalWins++;
            clan2Data.TotalLosses++;
            clan1Data.WarPoints += 3;
            clan2Data.WarPoints += 1;
        }
        else if (winnerID == clan2ID)
        {
            clan2Data.TotalWins++;
            clan1Data.TotalLosses++;
            clan2Data.WarPoints += 3;
            clan1Data.WarPoints += 1;
        }
        else
        {
            clan1Data.WarPoints += 1;
            clan2Data.WarPoints += 1;
        }

        clan1Data.LastMatchTime = DateTime.Now;
        clan2Data.LastMatchTime = DateTime.Now;
    }

    private void UpdateRankings()
    {
        var sortedClans = warData.ClanWarData.Values.OrderByDescending(c => c.WarPoints).ToList();
        for (int i = 0; i < sortedClans.Count; i++)
        {
            sortedClans[i].RankPosition = i + 1;
        }
    }

    public List<ClanWarData> GetClanRankings(int count = 10)
    {
        var sortedClans = warData.ClanWarData.Values.OrderByDescending(c => c.WarPoints).Take(count).ToList();
        return sortedClans;
    }

    public List<ClanWarMatch> GetClanMatches(string clanID)
    {
        if (warData.ClanMatches.ContainsKey(clanID))
        {
            return warData.ClanMatches[clanID];
        }
        return new List<ClanWarMatch>();
    }

    public List<ClanWarMatch> GetRecentMatches(int count = 10)
    {
        var sortedMatches = warData.MatchHistory.OrderByDescending(m => m.MatchTime).Take(count).ToList();
        return sortedMatches;
    }

    public double GetClanWinRate(string clanID)
    {
        if (warData.ClanWarData.ContainsKey(clanID))
        {
            ClanWarData data = warData.ClanWarData[clanID];
            if (data.TotalMatches > 0)
            {
                return (double)data.TotalWins / data.TotalMatches * 100;
            }
        }
        return 0;
    }

    public ClanWarSeason GetCurrentSeason()
    {
        return warData.CurrentSeason;
    }

    public void SaveData()
    {
        dataManager.SaveClanWarData();
    }

    public void LoadData()
    {
        dataManager.LoadClanWarData();
    }

    public List<ClanWarEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class AchievementBadge
{
    public string BadgeID;
    public string BadgeName;
    public string BadgeDescription;
    public string BadgeType;
    public int RarityLevel;
    public int RequiredProgress;
    public string IconPath;
    public string UnlockCondition;
    public bool IsUnlocked;
    public int CurrentProgress;
    public DateTime UnlockTime;

    public AchievementBadge(string badgeID, string badgeName, string badgeDescription, string badgeType, int rarityLevel, int requiredProgress, string unlockCondition)
    {
        BadgeID = badgeID;
        BadgeName = badgeName;
        BadgeDescription = badgeDescription;
        BadgeType = badgeType;
        RarityLevel = rarityLevel;
        RequiredProgress = requiredProgress;
        IconPath = "Badges/" + badgeID;
        UnlockCondition = unlockCondition;
        IsUnlocked = false;
        CurrentProgress = 0;
        UnlockTime = DateTime.MinValue;
    }
}

[Serializable]
public class PlayerBadgeCollection
{
    public string PlayerID;
    public List<AchievementBadge> CollectedBadges;
    public List<AchievementBadge> InProgressBadges;
    public int TotalBadges;
    public int UnlockedBadges;
    public DateTime LastUpdateTime;

    public PlayerBadgeCollection(string playerID)
    {
        PlayerID = playerID;
        CollectedBadges = new List<AchievementBadge>();
        InProgressBadges = new List<AchievementBadge>();
        TotalBadges = 0;
        UnlockedBadges = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class AchievementBadgeSystemData
{
    public List<AchievementBadge> GlobalBadges;
    public Dictionary<string, PlayerBadgeCollection> PlayerCollections;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public AchievementBadgeSystemData()
    {
        GlobalBadges = new List<AchievementBadge>();
        PlayerCollections = new Dictionary<string, PlayerBadgeCollection>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultBadges();
    }

    private void InitializeDefaultBadges()
    {
        GlobalBadges.Add(new AchievementBadge("badge_first_win", "初露锋芒", "获得第一场胜利", "combat", 1, 1, "获得一场胜利"));
        GlobalBadges.Add(new AchievementBadge("badge_10_wins", "常胜将军", "获得10场胜利", "combat", 2, 10, "获得10场胜利"));
        GlobalBadges.Add(new AchievementBadge("badge_50_wins", "战场精英", "获得50场胜利", "combat", 3, 50, "获得50场胜利"));
        GlobalBadges.Add(new AchievementBadge("badge_100_wins", "战争之王", "获得100场胜利", "combat", 4, 100, "获得100场胜利"));
        GlobalBadges.Add(new AchievementBadge("badge_500_kills", "杀神", "累计击杀500人", "combat", 3, 500, "累计击杀500人"));
        GlobalBadges.Add(new AchievementBadge("badge_1000_kills", "屠戮者", "累计击杀1000人", "combat", 4, 1000, "累计击杀1000人"));
        GlobalBadges.Add(new AchievementBadge("badge_500_assists", "助攻王", "累计助攻500次", "combat", 3, 500, "累计助攻500次"));
        GlobalBadges.Add(new AchievementBadge("badge_mvp_10", "MVP常客", "获得10次MVP", "combat", 3, 10, "获得10次MVP"));
        GlobalBadges.Add(new AchievementBadge("badge_clan_leader", "战队领袖", "创建并领导战队", "social", 4, 1, "创建并领导战队"));
        GlobalBadges.Add(new AchievementBadge("badge_friend_10", "社交达人", "拥有10个好友", "social", 2, 10, "拥有10个好友"));
    }
}

[Serializable]
public class BadgeEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string BadgeID;
    public string EventData;

    public BadgeEvent(string eventID, string eventType, string playerID, string badgeID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        BadgeID = badgeID;
        EventData = eventData;
    }
}

public class AchievementBadgeSystemDataManager
{
    private static AchievementBadgeSystemDataManager _instance;
    public static AchievementBadgeSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AchievementBadgeSystemDataManager();
            }
            return _instance;
        }
    }

    public AchievementBadgeSystemData badgeData;
    private List<BadgeEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private AchievementBadgeSystemDataManager()
    {
        badgeData = new AchievementBadgeSystemData();
        recentEvents = new List<BadgeEvent>();
        LoadBadgeData();
    }

    public void SaveBadgeData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "AchievementBadgeSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, badgeData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存成就徽章系统数据失败: " + e.Message);
        }
    }

    public void LoadBadgeData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "AchievementBadgeSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    badgeData = (AchievementBadgeSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载成就徽章系统数据失败: " + e.Message);
            badgeData = new AchievementBadgeSystemData();
        }
    }

    public void CreateBadgeEvent(string eventType, string playerID, string badgeID, string eventData)
    {
        string eventID = "badge_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BadgeEvent badgeEvent = new BadgeEvent(eventID, eventType, playerID, badgeID, eventData);
        recentEvents.Add(badgeEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<BadgeEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class AchievementBadgeSystemDetailedManager
{
    private static AchievementBadgeSystemDetailedManager _instance;
    public static AchievementBadgeSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AchievementBadgeSystemDetailedManager();
            }
            return _instance;
        }
    }

    private AchievementBadgeSystemData badgeData;
    private AchievementBadgeSystemDataManager dataManager;

    private AchievementBadgeSystemDetailedManager()
    {
        dataManager = AchievementBadgeSystemDataManager.Instance;
        badgeData = dataManager.badgeData;
    }

    public PlayerBadgeCollection GetPlayerBadges(string playerID)
    {
        if (!badgeData.PlayerCollections.ContainsKey(playerID))
        {
            badgeData.PlayerCollections[playerID] = new PlayerBadgeCollection(playerID);
            InitializePlayerBadges(playerID);
        }
        return badgeData.PlayerCollections[playerID];
    }

    private void InitializePlayerBadges(string playerID)
    {
        PlayerBadgeCollection collection = badgeData.PlayerCollections[playerID];
        foreach (var globalBadge in badgeData.GlobalBadges)
        {
            AchievementBadge playerBadge = new AchievementBadge(
                globalBadge.BadgeID,
                globalBadge.BadgeName,
                globalBadge.BadgeDescription,
                globalBadge.BadgeType,
                globalBadge.RarityLevel,
                globalBadge.RequiredProgress,
                globalBadge.UnlockCondition
            );
            collection.InProgressBadges.Add(playerBadge);
        }
        collection.TotalBadges = collection.InProgressBadges.Count;
    }

    public void UpdateBadgeProgress(string playerID, string badgeID, int progress)
    {
        PlayerBadgeCollection collection = GetPlayerBadges(playerID);
        AchievementBadge badge = collection.InProgressBadges.Find(b => b.BadgeID == badgeID);
        if (badge != null && !badge.IsUnlocked)
        {
            badge.CurrentProgress += progress;
            if (badge.CurrentProgress >= badge.RequiredProgress)
            {
                badge.CurrentProgress = badge.RequiredProgress;
                badge.IsUnlocked = true;
                badge.UnlockTime = DateTime.Now;
                collection.CollectedBadges.Add(badge);
                collection.InProgressBadges.Remove(badge);
                collection.UnlockedBadges++;
                collection.LastUpdateTime = DateTime.Now;
                dataManager.CreateBadgeEvent("badge_unlock", playerID, badgeID, "解锁徽章: " + badge.BadgeName);
                Debug.Log("玩家 " + playerID + " 解锁徽章: " + badge.BadgeName);
            }
            dataManager.SaveBadgeData();
        }
    }

    public void UpdateBadgeProgressByType(string playerID, string badgeType, int progress)
    {
        PlayerBadgeCollection collection = GetPlayerBadges(playerID);
        foreach (var badge in collection.InProgressBadges)
        {
            if (badge.BadgeType == badgeType && !badge.IsUnlocked)
            {
                UpdateBadgeProgress(playerID, badge.BadgeID, progress);
            }
        }
    }

    public List<AchievementBadge> GetPlayerCollectedBadges(string playerID)
    {
        PlayerBadgeCollection collection = GetPlayerBadges(playerID);
        return collection.CollectedBadges;
    }

    public List<AchievementBadge> GetPlayerInProgressBadges(string playerID)
    {
        PlayerBadgeCollection collection = GetPlayerBadges(playerID);
        return collection.InProgressBadges;
    }

    public List<AchievementBadge> GetBadgesByType(string playerID, string badgeType)
    {
        PlayerBadgeCollection collection = GetPlayerBadges(playerID);
        var allBadges = new List<AchievementBadge>();
        allBadges.AddRange(collection.CollectedBadges);
        allBadges.AddRange(collection.InProgressBadges);
        return allBadges.FindAll(b => b.BadgeType == badgeType);
    }

    public int GetBadgeCompletionRate(string playerID)
    {
        PlayerBadgeCollection collection = GetPlayerBadges(playerID);
        if (collection.TotalBadges > 0)
        {
            return (int)((double)collection.UnlockedBadges / collection.TotalBadges * 100);
        }
        return 0;
    }

    public string GetRarityColor(int rarityLevel)
    {
        switch (rarityLevel)
        {
            case 1: return "#FFFFFF";
            case 2: return "#4CAF50";
            case 3: return "#2196F3";
            case 4: return "#9C27B0";
            case 5: return "#FF9800";
            default: return "#9E9E9E";
        }
    }

    public string GetRarityName(int rarityLevel)
    {
        switch (rarityLevel)
        {
            case 1: return "普通";
            case 2: return "优秀";
            case 3: return "稀有";
            case 4: return "史诗";
            case 5: return "传说";
            default: return "未知";
        }
    }

    public void SaveData()
    {
        dataManager.SaveBadgeData();
    }

    public void LoadData()
    {
        dataManager.LoadBadgeData();
    }

    public List<BadgeEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class PetData
{
    public string PetID;
    public string PetName;
    public string PetType;
    public string PetDescription;
    public int Level;
    public int Exp;
    public int MaxExp;
    public int Attack;
    public int Defense;
    public int Health;
    public int Speed;
    public string Skill1;
    public string Skill2;
    public string Skill3;
    public string PetIcon;
    public bool IsActive;
    public DateTime ObtainTime;

    public PetData(string petID, string petName, string petType, string petDescription)
    {
        PetID = petID;
        PetName = petName;
        PetType = petType;
        PetDescription = petDescription;
        Level = 1;
        Exp = 0;
        MaxExp = 100;
        Attack = 10;
        Defense = 5;
        Health = 100;
        Speed = 5;
        Skill1 = "基础攻击";
        Skill2 = "防御提升";
        Skill3 = "生命恢复";
        PetIcon = "Pets/" + petID;
        IsActive = false;
        ObtainTime = DateTime.Now;
    }
}

[Serializable]
public class PlayerPetData
{
    public string PlayerID;
    public List<PetData> Pets;
    public string ActivePetID;
    public int TotalPets;
    public DateTime LastUpdateTime;

    public PlayerPetData(string playerID)
    {
        PlayerID = playerID;
        Pets = new List<PetData>();
        ActivePetID = "";
        TotalPets = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class PetSystemData
{
    public List<PetData> AvailablePets;
    public Dictionary<string, PlayerPetData> PlayerPets;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public PetSystemData()
    {
        AvailablePets = new List<PetData>();
        PlayerPets = new Dictionary<string, PlayerPetData>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultPets();
    }

    private void InitializeDefaultPets()
    {
        AvailablePets.Add(new PetData("pet_dragon", "小火龙", "fire", "一只可爱的小火龙，擅长火焰攻击"));
        AvailablePets.Add(new PetData("pet_tiger", "小老虎", "beast", "一只勇猛的小老虎，拥有强大的攻击力"));
        AvailablePets.Add(new PetData("pet_turtle", "小乌龟", "defense", "一只坚硬的小乌龟，拥有出色的防御力"));
        AvailablePets.Add(new PetData("pet_bird", "小鸟", "speed", "一只敏捷的小鸟，拥有极快的速度"));
        AvailablePets.Add(new PetData("pet_bunny", "小兔子", "support", "一只可爱的小兔子，擅长辅助和治疗"));
    }
}

[Serializable]
public class PetEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string PetID;
    public string EventData;

    public PetEvent(string eventID, string eventType, string playerID, string petID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        PetID = petID;
        EventData = eventData;
    }
}

public class PetSystemDataManager
{
    private static PetSystemDataManager _instance;
    public static PetSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PetSystemDataManager();
            }
            return _instance;
        }
    }

    public PetSystemData petData;
    private List<PetEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private PetSystemDataManager()
    {
        petData = new PetSystemData();
        recentEvents = new List<PetEvent>();
        LoadPetData();
    }

    public void SavePetData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "PetSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, petData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存宠物系统数据失败: " + e.Message);
        }
    }

    public void LoadPetData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "PetSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    petData = (PetSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载宠物系统数据失败: " + e.Message);
            petData = new PetSystemData();
        }
    }

    public void CreatePetEvent(string eventType, string playerID, string petID, string eventData)
    {
        string eventID = "pet_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PetEvent petEvent = new PetEvent(eventID, eventType, playerID, petID, eventData);
        recentEvents.Add(petEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<PetEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class PetSystemDetailedManager
{
    private static PetSystemDetailedManager _instance;
    public static PetSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PetSystemDetailedManager();
            }
            return _instance;
        }
    }

    private PetSystemData petData;
    private PetSystemDataManager dataManager;

    private PetSystemDetailedManager()
    {
        dataManager = PetSystemDataManager.Instance;
        petData = dataManager.petData;
    }

    public PlayerPetData GetPlayerPets(string playerID)
    {
        if (!petData.PlayerPets.ContainsKey(playerID))
        {
            petData.PlayerPets[playerID] = new PlayerPetData(playerID);
        }
        return petData.PlayerPets[playerID];
    }

    public string ObtainPet(string playerID, string petID)
    {
        PlayerPetData playerData = GetPlayerPets(playerID);
        PetData petTemplate = petData.AvailablePets.Find(p => p.PetID == petID);
        
        if (petTemplate != null && !playerData.Pets.Exists(p => p.PetID == petID))
        {
            PetData newPet = new PetData(petTemplate.PetID, petTemplate.PetName, petTemplate.PetType, petTemplate.PetDescription);
            playerData.Pets.Add(newPet);
            playerData.TotalPets++;
            playerData.LastUpdateTime = DateTime.Now;
            
            dataManager.CreatePetEvent("pet_obtain", playerID, petID, "获得宠物: " + newPet.PetName);
            dataManager.SavePetData();
            Debug.Log("玩家 " + playerID + " 获得宠物: " + newPet.PetName);
            return petID;
        }
        return null;
    }

    public void LevelUpPet(string playerID, string petID)
    {
        PlayerPetData playerData = GetPlayerPets(playerID);
        PetData pet = playerData.Pets.Find(p => p.PetID == petID);
        
        if (pet != null)
        {
            pet.Exp += 50;
            if (pet.Exp >= pet.MaxExp)
            {
                pet.Level++;
                pet.Exp -= pet.MaxExp;
                pet.MaxExp = (int)(pet.MaxExp * 1.5f);
                pet.Attack += 5;
                pet.Defense += 2;
                pet.Health += 50;
                pet.Speed += 1;
                
                dataManager.CreatePetEvent("pet_level_up", playerID, petID, "宠物升级: " + pet.PetName + " 等级 " + pet.Level);
                Debug.Log("宠物 " + pet.PetName + " 升级到等级 " + pet.Level);
            }
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.SavePetData();
        }
    }

    public void SetActivePet(string playerID, string petID)
    {
        PlayerPetData playerData = GetPlayerPets(playerID);
        if (playerData.Pets.Exists(p => p.PetID == petID))
        {
            playerData.ActivePetID = petID;
            foreach (var pet in playerData.Pets)
            {
                pet.IsActive = (pet.PetID == petID);
            }
            playerData.LastUpdateTime = DateTime.Now;
            
            PetData activePet = playerData.Pets.Find(p => p.PetID == petID);
            dataManager.CreatePetEvent("pet_activate", playerID, petID, "激活宠物: " + activePet.PetName);
            dataManager.SavePetData();
            Debug.Log("激活宠物: " + activePet.PetName);
        }
    }

    public PetData GetActivePet(string playerID)
    {
        PlayerPetData playerData = GetPlayerPets(playerID);
        if (!string.IsNullOrEmpty(playerData.ActivePetID))
        {
            return playerData.Pets.Find(p => p.PetID == playerData.ActivePetID);
        }
        return null;
    }

    public List<PetData> GetPlayerPetList(string playerID)
    {
        PlayerPetData playerData = GetPlayerPets(playerID);
        return playerData.Pets;
    }

    public List<PetData> GetAvailablePets()
    {
        return petData.AvailablePets;
    }

    public void FeedPet(string playerID, string petID, int expAmount)
    {
        PlayerPetData playerData = GetPlayerPets(playerID);
        PetData pet = playerData.Pets.Find(p => p.PetID == petID);
        
        if (pet != null)
        {
            pet.Exp += expAmount;
            while (pet.Exp >= pet.MaxExp)
            {
                pet.Level++;
                pet.Exp -= pet.MaxExp;
                pet.MaxExp = (int)(pet.MaxExp * 1.5f);
                pet.Attack += 5;
                pet.Defense += 2;
                pet.Health += 50;
                pet.Speed += 1;
                
                dataManager.CreatePetEvent("pet_level_up", playerID, petID, "宠物升级: " + pet.PetName + " 等级 " + pet.Level);
                Debug.Log("宠物 " + pet.PetName + " 升级到等级 " + pet.Level);
            }
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.SavePetData();
        }
    }

    public void SaveData()
    {
        dataManager.SavePetData();
    }

    public void LoadData()
    {
        dataManager.LoadPetData();
    }

    public List<PetEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class FriendData
{
    public string FriendID;
    public string FriendName;
    public string FriendAvatar;
    public string FriendStatus;
    public DateTime LastOnlineTime;
    public DateTime AddTime;
    public int FriendshipLevel;
    public int FriendshipPoints;
    public bool IsOnline;

    public FriendData(string friendID, string friendName, string friendAvatar)
    {
        FriendID = friendID;
        FriendName = friendName;
        FriendAvatar = friendAvatar;
        FriendStatus = "离线";
        LastOnlineTime = DateTime.Now;
        AddTime = DateTime.Now;
        FriendshipLevel = 1;
        FriendshipPoints = 0;
        IsOnline = false;
    }
}

[Serializable]
public class ChatMessage
{
    public string MessageID;
    public string SenderID;
    public string SenderName;
    public string ReceiverID;
    public string MessageContent;
    public DateTime SendTime;
    public string MessageType;
    public bool IsRead;

    public ChatMessage(string senderID, string senderName, string receiverID, string messageContent, string messageType)
    {
        MessageID = "msg_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SenderID = senderID;
        SenderName = senderName;
        ReceiverID = receiverID;
        MessageContent = messageContent;
        SendTime = DateTime.Now;
        MessageType = messageType;
        IsRead = false;
    }
}

[Serializable]
public class TeamData
{
    public string TeamID;
    public string TeamName;
    public string TeamLeaderID;
    public List<string> TeamMembers;
    public string TeamStatus;
    public DateTime CreateTime;
    public int MaxMembers;

    public TeamData(string teamID, string teamName, string teamLeaderID)
    {
        TeamID = teamID;
        TeamName = teamName;
        TeamLeaderID = teamLeaderID;
        TeamMembers = new List<string> { teamLeaderID };
        TeamStatus = "待机";
        CreateTime = DateTime.Now;
        MaxMembers = 5;
    }
}

[Serializable]
public class PlayerSocialData
{
    public string PlayerID;
    public List<FriendData> Friends;
    public List<ChatMessage> ChatMessages;
    public List<TeamData> Teams;
    public string CurrentTeamID;
    public int TotalFriends;
    public int OnlineFriends;
    public DateTime LastUpdateTime;

    public PlayerSocialData(string playerID)
    {
        PlayerID = playerID;
        Friends = new List<FriendData>();
        ChatMessages = new List<ChatMessage>();
        Teams = new List<TeamData>();
        CurrentTeamID = "";
        TotalFriends = 0;
        OnlineFriends = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class SocialSystemData
{
    public Dictionary<string, PlayerSocialData> PlayerSocials;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public SocialSystemData()
    {
        PlayerSocials = new Dictionary<string, PlayerSocialData>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class SocialEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string TargetID;
    public string EventData;

    public SocialEvent(string eventID, string eventType, string playerID, string targetID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        TargetID = targetID;
        EventData = eventData;
    }
}

public class SocialSystemDataManager
{
    private static SocialSystemDataManager _instance;
    public static SocialSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SocialSystemDataManager();
            }
            return _instance;
        }
    }

    public SocialSystemData socialData;
    private List<SocialEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SocialSystemDataManager()
    {
        socialData = new SocialSystemData();
        recentEvents = new List<SocialEvent>();
        LoadSocialData();
    }

    public void SaveSocialData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SocialSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, socialData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存社交系统数据失败: " + e.Message);
        }
    }

    public void LoadSocialData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SocialSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    socialData = (SocialSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载社交系统数据失败: " + e.Message);
            socialData = new SocialSystemData();
        }
    }

    public void CreateSocialEvent(string eventType, string playerID, string targetID, string eventData)
    {
        string eventID = "social_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SocialEvent socialEvent = new SocialEvent(eventID, eventType, playerID, targetID, eventData);
        recentEvents.Add(socialEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<SocialEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class SocialSystemDetailedManager
{
    private static SocialSystemDetailedManager _instance;
    public static SocialSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SocialSystemDetailedManager();
            }
            return _instance;
        }
    }

    private SocialSystemData socialData;
    private SocialSystemDataManager dataManager;

    private SocialSystemDetailedManager()
    {
        dataManager = SocialSystemDataManager.Instance;
        socialData = dataManager.socialData;
    }

    public PlayerSocialData GetPlayerSocialData(string playerID)
    {
        if (!socialData.PlayerSocials.ContainsKey(playerID))
        {
            socialData.PlayerSocials[playerID] = new PlayerSocialData(playerID);
        }
        return socialData.PlayerSocials[playerID];
    }

    public void SendFriendRequest(string playerID, string friendID, string friendName, string friendAvatar)
    {
        dataManager.CreateSocialEvent("friend_request", playerID, friendID, "发送好友请求");
        Debug.Log("玩家 " + playerID + " 向 " + friendName + " 发送好友请求");
    }

    public void AcceptFriendRequest(string playerID, string friendID, string friendName, string friendAvatar)
    {
        PlayerSocialData playerData = GetPlayerSocialData(playerID);
        FriendData newFriend = new FriendData(friendID, friendName, friendAvatar);
        playerData.Friends.Add(newFriend);
        playerData.TotalFriends++;
        playerData.LastUpdateTime = DateTime.Now;
        
        dataManager.CreateSocialEvent("friend_accept", playerID, friendID, "接受好友请求");
        dataManager.SaveSocialData();
        Debug.Log("玩家 " + playerID + " 接受了 " + friendName + " 的好友请求");
    }

    public void RemoveFriend(string playerID, string friendID)
    {
        PlayerSocialData playerData = GetPlayerSocialData(playerID);
        FriendData friend = playerData.Friends.Find(f => f.FriendID == friendID);
        if (friend != null)
        {
            playerData.Friends.Remove(friend);
            playerData.TotalFriends--;
            if (friend.IsOnline)
            {
                playerData.OnlineFriends--;
            }
            playerData.LastUpdateTime = DateTime.Now;
            
            dataManager.CreateSocialEvent("friend_remove", playerID, friendID, "移除好友");
            dataManager.SaveSocialData();
            Debug.Log("玩家 " + playerID + " 移除了好友 " + friend.FriendName);
        }
    }

    public void SendChatMessage(string senderID, string senderName, string receiverID, string messageContent, string messageType = "text")
    {
        ChatMessage message = new ChatMessage(senderID, senderName, receiverID, messageContent, messageType);
        
        PlayerSocialData senderData = GetPlayerSocialData(senderID);
        senderData.ChatMessages.Add(message);
        
        PlayerSocialData receiverData = GetPlayerSocialData(receiverID);
        receiverData.ChatMessages.Add(message);
        
        dataManager.CreateSocialEvent("chat_send", senderID, receiverID, "发送聊天消息");
        dataManager.SaveSocialData();
        Debug.Log("玩家 " + senderName + " 向 " + receiverID + " 发送消息: " + messageContent);
    }

    public List<ChatMessage> GetChatMessages(string playerID, string targetID, int count = 50)
    {
        PlayerSocialData playerData = GetPlayerSocialData(playerID);
        var messages = playerData.ChatMessages.FindAll(m => 
            (m.SenderID == playerID && m.ReceiverID == targetID) || 
            (m.SenderID == targetID && m.ReceiverID == playerID)
        );
        messages.Sort((a, b) => a.SendTime.CompareTo(b.SendTime));
        
        if (messages.Count > count)
        {
            return messages.GetRange(messages.Count - count, count);
        }
        return messages;
    }

    public string CreateTeam(string playerID, string teamName)
    {
        string teamID = "team_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TeamData newTeam = new TeamData(teamID, teamName, playerID);
        
        PlayerSocialData playerData = GetPlayerSocialData(playerID);
        playerData.Teams.Add(newTeam);
        playerData.CurrentTeamID = teamID;
        playerData.LastUpdateTime = DateTime.Now;
        
        dataManager.CreateSocialEvent("team_create", playerID, teamID, "创建战队");
        dataManager.SaveSocialData();
        Debug.Log("玩家 " + playerID + " 创建了战队 " + teamName);
        return teamID;
    }

    public void InviteToTeam(string teamID, string playerID, string inviteeID)
    {
        dataManager.CreateSocialEvent("team_invite", playerID, inviteeID, "邀请加入战队");
        Debug.Log("玩家 " + playerID + " 邀请 " + inviteeID + " 加入战队 " + teamID);
    }

    public void JoinTeam(string teamID, string playerID)
    {
        PlayerSocialData playerData = GetPlayerSocialData(playerID);
        TeamData team = playerData.Teams.Find(t => t.TeamID == teamID);
        
        if (team != null && team.TeamMembers.Count < team.MaxMembers)
        {
            team.TeamMembers.Add(playerID);
            playerData.CurrentTeamID = teamID;
            playerData.LastUpdateTime = DateTime.Now;
            
            dataManager.CreateSocialEvent("team_join", playerID, teamID, "加入战队");
            dataManager.SaveSocialData();
            Debug.Log("玩家 " + playerID + " 加入了战队 " + team.TeamName);
        }
    }

    public void LeaveTeam(string teamID, string playerID)
    {
        PlayerSocialData playerData = GetPlayerSocialData(playerID);
        TeamData team = playerData.Teams.Find(t => t.TeamID == teamID);
        
        if (team != null)
        {
            team.TeamMembers.Remove(playerID);
            if (playerData.CurrentTeamID == teamID)
            {
                playerData.CurrentTeamID = "";
            }
            playerData.LastUpdateTime = DateTime.Now;
            
            dataManager.CreateSocialEvent("team_leave", playerID, teamID, "离开战队");
            dataManager.SaveSocialData();
            Debug.Log("玩家 " + playerID + " 离开了战队 " + team.TeamName);
        }
    }

    public void UpdateFriendStatus(string friendID, bool isOnline)
    {
        foreach (var playerData in socialData.PlayerSocials.Values)
        {
            FriendData friend = playerData.Friends.Find(f => f.FriendID == friendID);
            if (friend != null)
            {
                friend.IsOnline = isOnline;
                friend.FriendStatus = isOnline ? "在线" : "离线";
                friend.LastOnlineTime = DateTime.Now;
                
                if (isOnline)
                {
                    playerData.OnlineFriends++;
                }
                else
                {
                    playerData.OnlineFriends--;
                }
                playerData.LastUpdateTime = DateTime.Now;
            }
        }
        dataManager.SaveSocialData();
    }

    public List<FriendData> GetFriends(string playerID)
    {
        PlayerSocialData playerData = GetPlayerSocialData(playerID);
        return playerData.Friends;
    }

    public List<FriendData> GetOnlineFriends(string playerID)
    {
        PlayerSocialData playerData = GetPlayerSocialData(playerID);
        return playerData.Friends.FindAll(f => f.IsOnline);
    }

    public TeamData GetCurrentTeam(string playerID)
    {
        PlayerSocialData playerData = GetPlayerSocialData(playerID);
        if (!string.IsNullOrEmpty(playerData.CurrentTeamID))
        {
            return playerData.Teams.Find(t => t.TeamID == playerData.CurrentTeamID);
        }
        return null;
    }

    public void SaveData()
    {
        dataManager.SaveSocialData();
    }

    public void LoadData()
    {
        dataManager.LoadSocialData();
    }

    public List<SocialEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class CurrencyData
{
    public string CurrencyID;
    public string CurrencyName;
    public string CurrencyType;
    public int Amount;
    public int MaxAmount;
    public string IconPath;
    public bool IsPrimary;

    public CurrencyData(string currencyID, string currencyName, string currencyType, int initialAmount, bool isPrimary = false)
    {
        CurrencyID = currencyID;
        CurrencyName = currencyName;
        CurrencyType = currencyType;
        Amount = initialAmount;
        MaxAmount = int.MaxValue;
        IconPath = "Currencies/" + currencyID;
        IsPrimary = isPrimary;
    }
}

[Serializable]
public class TransactionData
{
    public string TransactionID;
    public string TransactionType;
    public string CurrencyID;
    public int Amount;
    public string Description;
    public DateTime TransactionTime;
    public string Source;
    public string Target;

    public TransactionData(string transactionType, string currencyID, int amount, string description, string source, string target)
    {
        TransactionID = "trans_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        TransactionType = transactionType;
        CurrencyID = currencyID;
        Amount = amount;
        Description = description;
        TransactionTime = DateTime.Now;
        Source = source;
        Target = target;
    }
}

[Serializable]
public class PlayerEconomyData
{
    public string PlayerID;
    public Dictionary<string, CurrencyData> Currencies;
    public List<TransactionData> Transactions;
    public int TotalTransactions;
    public DateTime LastUpdateTime;

    public PlayerEconomyData(string playerID)
    {
        PlayerID = playerID;
        Currencies = new Dictionary<string, CurrencyData>();
        Transactions = new List<TransactionData>();
        TotalTransactions = 0;
        LastUpdateTime = DateTime.Now;
        InitializeDefaultCurrencies();
    }

    private void InitializeDefaultCurrencies()
    {
        Currencies["gold"] = new CurrencyData("gold", "金币", "primary", 1000, true);
        Currencies["diamond"] = new CurrencyData("diamond", "钻石", "premium", 100);
        Currencies["honor"] = new CurrencyData("honor", "荣誉", "special", 0);
        Currencies["experience"] = new CurrencyData("experience", "经验", "special", 0);
    }
}

[Serializable]
public class EconomySystemData
{
    public Dictionary<string, PlayerEconomyData> PlayerEconomies;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public EconomySystemData()
    {
        PlayerEconomies = new Dictionary<string, PlayerEconomyData>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class EconomyEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string CurrencyID;
    public int Amount;
    public string EventData;

    public EconomyEvent(string eventID, string eventType, string playerID, string currencyID, int amount, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        CurrencyID = currencyID;
        Amount = amount;
        EventData = eventData;
    }
}

public class EconomySystemDataManager
{
    private static EconomySystemDataManager _instance;
    public static EconomySystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EconomySystemDataManager();
            }
            return _instance;
        }
    }

    public EconomySystemData economyData;
    private List<EconomyEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private EconomySystemDataManager()
    {
        economyData = new EconomySystemData();
        recentEvents = new List<EconomyEvent>();
        LoadEconomyData();
    }

    public void SaveEconomyData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EconomySystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, economyData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存经济系统数据失败: " + e.Message);
        }
    }

    public void LoadEconomyData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EconomySystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    economyData = (EconomySystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载经济系统数据失败: " + e.Message);
            economyData = new EconomySystemData();
        }
    }

    public void CreateEconomyEvent(string eventType, string playerID, string currencyID, int amount, string eventData)
    {
        string eventID = "economy_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EconomyEvent economyEvent = new EconomyEvent(eventID, eventType, playerID, currencyID, amount, eventData);
        recentEvents.Add(economyEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<EconomyEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class EconomySystemDetailedManager
{
    private static EconomySystemDetailedManager _instance;
    public static EconomySystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EconomySystemDetailedManager();
            }
            return _instance;
        }
    }

    private EconomySystemData economyData;
    private EconomySystemDataManager dataManager;

    private EconomySystemDetailedManager()
    {
        dataManager = EconomySystemDataManager.Instance;
        economyData = dataManager.economyData;
    }

    public PlayerEconomyData GetPlayerEconomyData(string playerID)
    {
        if (!economyData.PlayerEconomies.ContainsKey(playerID))
        {
            economyData.PlayerEconomies[playerID] = new PlayerEconomyData(playerID);
        }
        return economyData.PlayerEconomies[playerID];
    }

    public bool AddCurrency(string playerID, string currencyID, int amount, string source, string description)
    {
        PlayerEconomyData playerData = GetPlayerEconomyData(playerID);
        if (playerData.Currencies.ContainsKey(currencyID))
        {
            CurrencyData currency = playerData.Currencies[currencyID];
            if (currency.Amount + amount <= currency.MaxAmount)
            {
                currency.Amount += amount;
                playerData.LastUpdateTime = DateTime.Now;
                
                TransactionData transaction = new TransactionData("add", currencyID, amount, description, source, playerID);
                playerData.Transactions.Add(transaction);
                playerData.TotalTransactions++;
                
                dataManager.CreateEconomyEvent("currency_add", playerID, currencyID, amount, description);
                dataManager.SaveEconomyData();
                Debug.Log("玩家 " + playerID + " 获得 " + amount + " " + currency.CurrencyName + " - " + description);
                return true;
            }
        }
        return false;
    }

    public bool RemoveCurrency(string playerID, string currencyID, int amount, string target, string description)
    {
        PlayerEconomyData playerData = GetPlayerEconomyData(playerID);
        if (playerData.Currencies.ContainsKey(currencyID))
        {
            CurrencyData currency = playerData.Currencies[currencyID];
            if (currency.Amount >= amount)
            {
                currency.Amount -= amount;
                playerData.LastUpdateTime = DateTime.Now;
                
                TransactionData transaction = new TransactionData("remove", currencyID, amount, description, playerID, target);
                playerData.Transactions.Add(transaction);
                playerData.TotalTransactions++;
                
                dataManager.CreateEconomyEvent("currency_remove", playerID, currencyID, amount, description);
                dataManager.SaveEconomyData();
                Debug.Log("玩家 " + playerID + " 消耗 " + amount + " " + currency.CurrencyName + " - " + description);
                return true;
            }
        }
        return false;
    }

    public int GetCurrencyAmount(string playerID, string currencyID)
    {
        PlayerEconomyData playerData = GetPlayerEconomyData(playerID);
        if (playerData.Currencies.ContainsKey(currencyID))
        {
            return playerData.Currencies[currencyID].Amount;
        }
        return 0;
    }

    public Dictionary<string, CurrencyData> GetAllCurrencies(string playerID)
    {
        PlayerEconomyData playerData = GetPlayerEconomyData(playerID);
        return playerData.Currencies;
    }

    public List<TransactionData> GetRecentTransactions(string playerID, int count = 50)
    {
        PlayerEconomyData playerData = GetPlayerEconomyData(playerID);
        var transactions = playerData.Transactions.OrderByDescending(t => t.TransactionTime).Take(count).ToList();
        return transactions;
    }

    public bool HasEnoughCurrency(string playerID, string currencyID, int amount)
    {
        return GetCurrencyAmount(playerID, currencyID) >= amount;
    }

    public bool TransferCurrency(string fromPlayerID, string toPlayerID, string currencyID, int amount, string description)
    {
        if (RemoveCurrency(fromPlayerID, currencyID, amount, toPlayerID, description))
        {
            return AddCurrency(toPlayerID, currencyID, amount, fromPlayerID, description);
        }
        return false;
    }

    public void UpdateCurrencyMaxAmount(string playerID, string currencyID, int maxAmount)
    {
        PlayerEconomyData playerData = GetPlayerEconomyData(playerID);
        if (playerData.Currencies.ContainsKey(currencyID))
        {
            playerData.Currencies[currencyID].MaxAmount = maxAmount;
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.SaveEconomyData();
        }
    }

    public void AddNewCurrency(string playerID, string currencyID, string currencyName, string currencyType, int initialAmount)
    {
        PlayerEconomyData playerData = GetPlayerEconomyData(playerID);
        if (!playerData.Currencies.ContainsKey(currencyID))
        {
            playerData.Currencies[currencyID] = new CurrencyData(currencyID, currencyName, currencyType, initialAmount);
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.SaveEconomyData();
        }
    }

    public void SaveData()
    {
        dataManager.SaveEconomyData();
    }

    public void LoadData()
    {
        dataManager.LoadEconomyData();
    }

    public List<EconomyEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class SkinData
{
    public string SkinID;
    public string SkinName;
    public string HeroID;
    public string SkinDescription;
    public string SkinType;
    public int RarityLevel;
    public int Price;
    public string CurrencyType;
    public string SkinIcon;
    public string SkinModel;
    public List<string> SkinEffects;
    public bool IsOwned;
    public DateTime PurchaseTime;

    public SkinData(string skinID, string skinName, string heroID, string skinDescription, string skinType, int rarityLevel, int price, string currencyType)
    {
        SkinID = skinID;
        SkinName = skinName;
        HeroID = heroID;
        SkinDescription = skinDescription;
        SkinType = skinType;
        RarityLevel = rarityLevel;
        Price = price;
        CurrencyType = currencyType;
        SkinIcon = "Skins/" + skinID + "_icon";
        SkinModel = "Skins/" + skinID + "_model";
        SkinEffects = new List<string>();
        IsOwned = false;
        PurchaseTime = DateTime.MinValue;
    }
}

[Serializable]
public class SkinFragment
{
    public string FragmentID;
    public string SkinID;
    public string FragmentName;
    public int RequiredFragments;
    public int CurrentFragments;
    public string FragmentIcon;

    public SkinFragment(string fragmentID, string skinID, string fragmentName, int requiredFragments)
    {
        FragmentID = fragmentID;
        SkinID = skinID;
        FragmentName = fragmentName;
        RequiredFragments = requiredFragments;
        CurrentFragments = 0;
        FragmentIcon = "Fragments/" + fragmentID;
    }
}

[Serializable]
public class PlayerSkinData
{
    public string PlayerID;
    public List<SkinData> OwnedSkins;
    public List<SkinFragment> SkinFragments;
    public int TotalSkins;
    public int TotalFragments;
    public DateTime LastUpdateTime;

    public PlayerSkinData(string playerID)
    {
        PlayerID = playerID;
        OwnedSkins = new List<SkinData>();
        SkinFragments = new List<SkinFragment>();
        TotalSkins = 0;
        TotalFragments = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class SkinSystemData
{
    public List<SkinData> AvailableSkins;
    public Dictionary<string, PlayerSkinData> PlayerSkins;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public SkinSystemData()
    {
        AvailableSkins = new List<SkinData>();
        PlayerSkins = new Dictionary<string, PlayerSkinData>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultSkins();
    }

    private void InitializeDefaultSkins()
    {
        AvailableSkins.Add(new SkinData("skin_warrior_legend", "传说战士", "hero_warrior", "战士的传说皮肤，拥有强大的视觉效果", "legendary", 5, 2000, "diamond"));
        AvailableSkins.Add(new SkinData("skin_mage_frost", "冰霜法师", "hero_mage", "法师的冰霜皮肤，释放冰冷的力量", "epic", 4, 1200, "diamond"));
        AvailableSkins.Add(new SkinData("skin_archer_forest", "森林射手", "hero_archer", "射手的森林皮肤，与自然融为一体", "rare", 3, 800, "diamond"));
        AvailableSkins.Add(new SkinData("skin_assassin_shadow", "暗影刺客", "hero_assassin", "刺客的暗影皮肤，隐藏在黑暗中", "epic", 4, 1500, "diamond"));
        AvailableSkins.Add(new SkinData("skin_tank_iron", "钢铁坦克", "hero_tank", "坦克的钢铁皮肤，坚不可摧", "rare", 3, 600, "diamond"));
    }
}

[Serializable]
public class SkinEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string SkinID;
    public string EventData;

    public SkinEvent(string eventID, string eventType, string playerID, string skinID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        SkinID = skinID;
        EventData = eventData;
    }
}

public class SkinSystemDataManager
{
    private static SkinSystemDataManager _instance;
    public static SkinSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkinSystemDataManager();
            }
            return _instance;
        }
    }

    public SkinSystemData skinData;
    private List<SkinEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SkinSystemDataManager()
    {
        skinData = new SkinSystemData();
        recentEvents = new List<SkinEvent>();
        LoadSkinData();
    }

    public void SaveSkinData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SkinSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, skinData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存皮肤系统数据失败: " + e.Message);
        }
    }

    public void LoadSkinData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SkinSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    skinData = (SkinSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载皮肤系统数据失败: " + e.Message);
            skinData = new SkinSystemData();
        }
    }

    public void CreateSkinEvent(string eventType, string playerID, string skinID, string eventData)
    {
        string eventID = "skin_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SkinEvent skinEvent = new SkinEvent(eventID, eventType, playerID, skinID, eventData);
        recentEvents.Add(skinEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<SkinEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class SkinSystemDetailedManager
{
    private static SkinSystemDetailedManager _instance;
    public static SkinSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkinSystemDetailedManager();
            }
            return _instance;
        }
    }

    private SkinSystemData skinData;
    private SkinSystemDataManager dataManager;

    private SkinSystemDetailedManager()
    {
        dataManager = SkinSystemDataManager.Instance;
        skinData = dataManager.skinData;
    }

    public PlayerSkinData GetPlayerSkinData(string playerID)
    {
        if (!skinData.PlayerSkins.ContainsKey(playerID))
        {
            skinData.PlayerSkins[playerID] = new PlayerSkinData(playerID);
        }
        return skinData.PlayerSkins[playerID];
    }

    public bool PurchaseSkin(string playerID, string skinID)
    {
        PlayerSkinData playerData = GetPlayerSkinData(playerID);
        SkinData skin = skinData.AvailableSkins.Find(s => s.SkinID == skinID);
        
        if (skin != null && !playerData.OwnedSkins.Exists(s => s.SkinID == skinID))
        {
            EconomySystemDetailedManager economyManager = EconomySystemDetailedManager.Instance;
            if (economyManager.HasEnoughCurrency(playerID, skin.CurrencyType, skin.Price))
            {
                if (economyManager.RemoveCurrency(playerID, skin.CurrencyType, skin.Price, "skin_shop", "购买皮肤: " + skin.SkinName))
                {
                    SkinData purchasedSkin = new SkinData(
                        skin.SkinID, skin.SkinName, skin.HeroID, skin.SkinDescription, 
                        skin.SkinType, skin.RarityLevel, skin.Price, skin.CurrencyType
                    );
                    purchasedSkin.IsOwned = true;
                    purchasedSkin.PurchaseTime = DateTime.Now;
                    purchasedSkin.SkinEffects = skin.SkinEffects;
                    
                    playerData.OwnedSkins.Add(purchasedSkin);
                    playerData.TotalSkins++;
                    playerData.LastUpdateTime = DateTime.Now;
                    
                    dataManager.CreateSkinEvent("skin_purchase", playerID, skinID, "购买皮肤: " + skin.SkinName);
                    dataManager.SaveSkinData();
                    Debug.Log("玩家 " + playerID + " 购买了皮肤: " + skin.SkinName);
                    return true;
                }
            }
        }
        return false;
    }

    public void AddSkinFragment(string playerID, string skinID, int amount)
    {
        PlayerSkinData playerData = GetPlayerSkinData(playerID);
        SkinFragment fragment = playerData.SkinFragments.Find(f => f.SkinID == skinID);
        
        if (fragment == null)
        {
            SkinData skin = skinData.AvailableSkins.Find(s => s.SkinID == skinID);
            if (skin != null)
            {
                fragment = new SkinFragment("fragment_" + skinID, skinID, skin.SkinName + "碎片", 10);
                playerData.SkinFragments.Add(fragment);
                playerData.TotalFragments++;
            }
        }
        
        if (fragment != null)
        {
            fragment.CurrentFragments += amount;
            playerData.LastUpdateTime = DateTime.Now;
            
            if (fragment.CurrentFragments >= fragment.RequiredFragments)
            {
                UnlockSkinByFragments(playerID, skinID, fragment);
            }
            
            dataManager.CreateSkinEvent("fragment_add", playerID, skinID, "获得皮肤碎片: " + amount + "个");
            dataManager.SaveSkinData();
            Debug.Log("玩家 " + playerID + " 获得 " + amount + " 个 " + fragment.FragmentName);
        }
    }

    private void UnlockSkinByFragments(string playerID, string skinID, SkinFragment fragment)
    {
        PlayerSkinData playerData = GetPlayerSkinData(playerID);
        SkinData skin = skinData.AvailableSkins.Find(s => s.SkinID == skinID);
        
        if (skin != null && !playerData.OwnedSkins.Exists(s => s.SkinID == skinID))
        {
            SkinData unlockedSkin = new SkinData(
                skin.SkinID, skin.SkinName, skin.HeroID, skin.SkinDescription, 
                skin.SkinType, skin.RarityLevel, skin.Price, skin.CurrencyType
            );
            unlockedSkin.IsOwned = true;
            unlockedSkin.PurchaseTime = DateTime.Now;
            unlockedSkin.SkinEffects = skin.SkinEffects;
            
            playerData.OwnedSkins.Add(unlockedSkin);
            playerData.TotalSkins++;
            playerData.SkinFragments.Remove(fragment);
            playerData.TotalFragments--;
            playerData.LastUpdateTime = DateTime.Now;
            
            dataManager.CreateSkinEvent("skin_unlock_fragment", playerID, skinID, "通过碎片解锁皮肤: " + skin.SkinName);
            dataManager.SaveSkinData();
            Debug.Log("玩家 " + playerID + " 通过碎片解锁了皮肤: " + skin.SkinName);
        }
    }

    public List<SkinData> GetOwnedSkins(string playerID)
    {
        PlayerSkinData playerData = GetPlayerSkinData(playerID);
        return playerData.OwnedSkins;
    }

    public List<SkinData> GetAvailableSkins()
    {
        return skinData.AvailableSkins;
    }

    public List<SkinFragment> GetSkinFragments(string playerID)
    {
        PlayerSkinData playerData = GetPlayerSkinData(playerID);
        return playerData.SkinFragments;
    }

    public SkinData GetSkinByID(string skinID)
    {
        return skinData.AvailableSkins.Find(s => s.SkinID == skinID);
    }

    public bool IsSkinOwned(string playerID, string skinID)
    {
        PlayerSkinData playerData = GetPlayerSkinData(playerID);
        return playerData.OwnedSkins.Exists(s => s.SkinID == skinID);
    }

    public int GetSkinFragmentCount(string playerID, string skinID)
    {
        PlayerSkinData playerData = GetPlayerSkinData(playerID);
        SkinFragment fragment = playerData.SkinFragments.Find(f => f.SkinID == skinID);
        return fragment != null ? fragment.CurrentFragments : 0;
    }

    public void SaveData()
    {
        dataManager.SaveSkinData();
    }

    public void LoadData()
    {
        dataManager.LoadSkinData();
    }

    public List<SkinEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class EquipmentData
{
    public string EquipmentID;
    public string EquipmentName;
    public string EquipmentType;
    public string EquipmentDescription;
    public int Level;
    public int Attack;
    public int Defense;
    public int Health;
    public int Mana;
    public int Price;
    public string CurrencyType;
    public string EquipmentIcon;
    public string EquipmentModel;
    public int RequiredLevel;
    public bool IsEquipped;

    public EquipmentData(string equipmentID, string equipmentName, string equipmentType, string equipmentDescription, int attack, int defense, int health, int mana, int price, string currencyType, int requiredLevel)
    {
        EquipmentID = equipmentID;
        EquipmentName = equipmentName;
        EquipmentType = equipmentType;
        EquipmentDescription = equipmentDescription;
        Level = 1;
        Attack = attack;
        Defense = defense;
        Health = health;
        Mana = mana;
        Price = price;
        CurrencyType = currencyType;
        EquipmentIcon = "Equipment/" + equipmentID + "_icon";
        EquipmentModel = "Equipment/" + equipmentID + "_model";
        RequiredLevel = requiredLevel;
        IsEquipped = false;
    }
}

[Serializable]
public class EquipmentRecipe
{
    public string RecipeID;
    public string ResultEquipmentID;
    public Dictionary<string, int> RequiredMaterials;
    public int CraftingCost;
    public string CurrencyType;

    public EquipmentRecipe(string recipeID, string resultEquipmentID, Dictionary<string, int> requiredMaterials, int craftingCost, string currencyType)
    {
        RecipeID = recipeID;
        ResultEquipmentID = resultEquipmentID;
        RequiredMaterials = requiredMaterials;
        CraftingCost = craftingCost;
        CurrencyType = currencyType;
    }
}

[Serializable]
public class PlayerEquipmentData
{
    public string PlayerID;
    public List<EquipmentData> EquipmentInventory;
    public List<EquipmentData> EquippedEquipment;
    public List<EquipmentRecipe> KnownRecipes;
    public int TotalEquipment;
    public int EquippedCount;
    public DateTime LastUpdateTime;

    public PlayerEquipmentData(string playerID)
    {
        PlayerID = playerID;
        EquipmentInventory = new List<EquipmentData>();
        EquippedEquipment = new List<EquipmentData>();
        KnownRecipes = new List<EquipmentRecipe>();
        TotalEquipment = 0;
        EquippedCount = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class EquipmentSystemData
{
    public List<EquipmentData> AvailableEquipment;
    public List<EquipmentRecipe> AvailableRecipes;
    public Dictionary<string, PlayerEquipmentData> PlayerEquipments;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public EquipmentSystemData()
    {
        AvailableEquipment = new List<EquipmentData>();
        AvailableRecipes = new List<EquipmentRecipe>();
        PlayerEquipments = new Dictionary<string, PlayerEquipmentData>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultEquipment();
        InitializeDefaultRecipes();
    }

    private void InitializeDefaultEquipment()
    {
        AvailableEquipment.Add(new EquipmentData("eq_sword_basic", "基础剑", "weapon", "一把基础的剑，提供攻击力", 10, 0, 0, 0, 100, "gold", 1));
        AvailableEquipment.Add(new EquipmentData("eq_shield_basic", "基础盾牌", "defense", "一个基础的盾牌，提供防御力", 0, 10, 0, 0, 100, "gold", 1));
        AvailableEquipment.Add(new EquipmentData("eq_armor_basic", "基础盔甲", "armor", "一件基础的盔甲，提供生命值", 0, 0, 50, 0, 150, "gold", 1));
        AvailableEquipment.Add(new EquipmentData("eq_helmet_basic", "基础头盔", "head", "一个基础的头盔，提供防御力和生命值", 0, 5, 25, 0, 80, "gold", 1));
        AvailableEquipment.Add(new EquipmentData("eq_boots_basic", "基础靴子", "feet", "一双基础的靴子，提供移动速度", 0, 0, 0, 0, 80, "gold", 1));
    }

    private void InitializeDefaultRecipes()
    {
        var materials1 = new Dictionary<string, int> { { "eq_sword_basic", 1 }, { "eq_shield_basic", 1 } };
        AvailableRecipes.Add(new EquipmentRecipe("recipe_knight_gear", "eq_knight_gear", materials1, 200, "gold"));
    }
}

[Serializable]
public class EquipmentEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EquipmentID;
    public string EventData;

    public EquipmentEvent(string eventID, string eventType, string playerID, string equipmentID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EquipmentID = equipmentID;
        EventData = eventData;
    }
}

public class EquipmentSystemDataManager
{
    private static EquipmentSystemDataManager _instance;
    public static EquipmentSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EquipmentSystemDataManager();
            }
            return _instance;
        }
    }

    public EquipmentSystemData equipmentData;
    private List<EquipmentEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private EquipmentSystemDataManager()
    {
        equipmentData = new EquipmentSystemData();
        recentEvents = new List<EquipmentEvent>();
        LoadEquipmentData();
    }

    public void SaveEquipmentData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EquipmentSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, equipmentData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存装备系统数据失败: " + e.Message);
        }
    }

    public void LoadEquipmentData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EquipmentSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    equipmentData = (EquipmentSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载装备系统数据失败: " + e.Message);
            equipmentData = new EquipmentSystemData();
        }
    }

    public void CreateEquipmentEvent(string eventType, string playerID, string equipmentID, string eventData)
    {
        string eventID = "equipment_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EquipmentEvent equipmentEvent = new EquipmentEvent(eventID, eventType, playerID, equipmentID, eventData);
        recentEvents.Add(equipmentEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<EquipmentEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class EquipmentSystemDetailedManager
{
    private static EquipmentSystemDetailedManager _instance;
    public static EquipmentSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EquipmentSystemDetailedManager();
            }
            return _instance;
        }
    }

    private EquipmentSystemData equipmentData;
    private EquipmentSystemDataManager dataManager;

    private EquipmentSystemDetailedManager()
    {
        dataManager = EquipmentSystemDataManager.Instance;
        equipmentData = dataManager.equipmentData;
    }

    public PlayerEquipmentData GetPlayerEquipmentData(string playerID)
    {
        if (!equipmentData.PlayerEquipments.ContainsKey(playerID))
        {
            equipmentData.PlayerEquipments[playerID] = new PlayerEquipmentData(playerID);
        }
        return equipmentData.PlayerEquipments[playerID];
    }

    public bool PurchaseEquipment(string playerID, string equipmentID)
    {
        PlayerEquipmentData playerData = GetPlayerEquipmentData(playerID);
        EquipmentData equipment = equipmentData.AvailableEquipment.Find(e => e.EquipmentID == equipmentID);
        
        if (equipment != null && !playerData.EquipmentInventory.Exists(e => e.EquipmentID == equipmentID))
        {
            EconomySystemDetailedManager economyManager = EconomySystemDetailedManager.Instance;
            if (economyManager.HasEnoughCurrency(playerID, equipment.CurrencyType, equipment.Price))
            {
                if (economyManager.RemoveCurrency(playerID, equipment.CurrencyType, equipment.Price, "equipment_shop", "购买装备: " + equipment.EquipmentName))
                {
                    EquipmentData purchasedEquipment = new EquipmentData(
                        equipment.EquipmentID, equipment.EquipmentName, equipment.EquipmentType, equipment.EquipmentDescription, 
                        equipment.Attack, equipment.Defense, equipment.Health, equipment.Mana, equipment.Price, 
                        equipment.CurrencyType, equipment.RequiredLevel
                    );
                    
                    playerData.EquipmentInventory.Add(purchasedEquipment);
                    playerData.TotalEquipment++;
                    playerData.LastUpdateTime = DateTime.Now;
                    
                    dataManager.CreateEquipmentEvent("equipment_purchase", playerID, equipmentID, "购买装备: " + equipment.EquipmentName);
                    dataManager.SaveEquipmentData();
                    Debug.Log("玩家 " + playerID + " 购买了装备: " + equipment.EquipmentName);
                    return true;
                }
            }
        }
        return false;
    }

    public bool EquipEquipment(string playerID, string equipmentID)
    {
        PlayerEquipmentData playerData = GetPlayerEquipmentData(playerID);
        EquipmentData equipment = playerData.EquipmentInventory.Find(e => e.EquipmentID == equipmentID);
        
        if (equipment != null && !equipment.IsEquipped)
        {
            var existingEquip = playerData.EquippedEquipment.Find(e => e.EquipmentType == equipment.EquipmentType);
            if (existingEquip != null)
            {
                existingEquip.IsEquipped = false;
                playerData.EquipmentInventory.Add(existingEquip);
                playerData.EquippedEquipment.Remove(existingEquip);
                playerData.EquippedCount--;
            }
            
            equipment.IsEquipped = true;
            playerData.EquippedEquipment.Add(equipment);
            playerData.EquipmentInventory.Remove(equipment);
            playerData.EquippedCount++;
            playerData.LastUpdateTime = DateTime.Now;
            
            dataManager.CreateEquipmentEvent("equipment_equip", playerID, equipmentID, "装备: " + equipment.EquipmentName);
            dataManager.SaveEquipmentData();
            Debug.Log("玩家 " + playerID + " 装备了: " + equipment.EquipmentName);
            return true;
        }
        return false;
    }

    public bool UnequipEquipment(string playerID, string equipmentID)
    {
        PlayerEquipmentData playerData = GetPlayerEquipmentData(playerID);
        EquipmentData equipment = playerData.EquippedEquipment.Find(e => e.EquipmentID == equipmentID);
        
        if (equipment != null && equipment.IsEquipped)
        {
            equipment.IsEquipped = false;
            playerData.EquipmentInventory.Add(equipment);
            playerData.EquippedEquipment.Remove(equipment);
            playerData.EquippedCount--;
            playerData.LastUpdateTime = DateTime.Now;
            
            dataManager.CreateEquipmentEvent("equipment_unequip", playerID, equipmentID, "卸下装备: " + equipment.EquipmentName);
            dataManager.SaveEquipmentData();
            Debug.Log("玩家 " + playerID + " 卸下了: " + equipment.EquipmentName);
            return true;
        }
        return false;
    }

    public bool UpgradeEquipment(string playerID, string equipmentID)
    {
        PlayerEquipmentData playerData = GetPlayerEquipmentData(playerID);
        EquipmentData equipment = playerData.EquipmentInventory.Find(e => e.EquipmentID == equipmentID);
        
        if (equipment == null)
        {
            equipment = playerData.EquippedEquipment.Find(e => e.EquipmentID == equipmentID);
        }
        
        if (equipment != null)
        {
            int upgradeCost = equipment.Price * (equipment.Level + 1);
            EconomySystemDetailedManager economyManager = EconomySystemDetailedManager.Instance;
            if (economyManager.HasEnoughCurrency(playerID, "gold", upgradeCost))
            {
                if (economyManager.RemoveCurrency(playerID, "gold", upgradeCost, "equipment_upgrade", "升级装备: " + equipment.EquipmentName))
                {
                    equipment.Level++;
                    equipment.Attack = (int)(equipment.Attack * 1.2f);
                    equipment.Defense = (int)(equipment.Defense * 1.2f);
                    equipment.Health = (int)(equipment.Health * 1.2f);
                    equipment.Mana = (int)(equipment.Mana * 1.2f);
                    playerData.LastUpdateTime = DateTime.Now;
                    
                    dataManager.CreateEquipmentEvent("equipment_upgrade", playerID, equipmentID, "升级装备: " + equipment.EquipmentName + " 到等级 " + equipment.Level);
                    dataManager.SaveEquipmentData();
                    Debug.Log("玩家 " + playerID + " 升级了装备: " + equipment.EquipmentName + " 到等级 " + equipment.Level);
                    return true;
                }
            }
        }
        return false;
    }

    public bool CraftEquipment(string playerID, string recipeID)
    {
        PlayerEquipmentData playerData = GetPlayerEquipmentData(playerID);
        EquipmentRecipe recipe = equipmentData.AvailableRecipes.Find(r => r.RecipeID == recipeID);
        
        if (recipe != null)
        {
            bool hasMaterials = true;
            foreach (var material in recipe.RequiredMaterials)
            {
                int count = playerData.EquipmentInventory.FindAll(e => e.EquipmentID == material.Key).Count;
                if (count < material.Value)
                {
                    hasMaterials = false;
                    break;
                }
            }
            
            if (hasMaterials)
            {
                EconomySystemDetailedManager economyManager = EconomySystemDetailedManager.Instance;
                if (economyManager.HasEnoughCurrency(playerID, recipe.CurrencyType, recipe.CraftingCost))
                {
                    if (economyManager.RemoveCurrency(playerID, recipe.CurrencyType, recipe.CraftingCost, "equipment_craft", "合成装备: " + recipe.ResultEquipmentID))
                    {
                        foreach (var material in recipe.RequiredMaterials)
                        {
                            for (int i = 0; i < material.Value; i++)
                            {
                                var equipment = playerData.EquipmentInventory.Find(e => e.EquipmentID == material.Key);
                                if (equipment != null)
                                {
                                    playerData.EquipmentInventory.Remove(equipment);
                                    playerData.TotalEquipment--;
                                }
                            }
                        }
                        
                        EquipmentData resultEquipment = equipmentData.AvailableEquipment.Find(e => e.EquipmentID == recipe.ResultEquipmentID);
                        if (resultEquipment == null)
                        {
                            resultEquipment = new EquipmentData(recipe.ResultEquipmentID, "合成装备", "special", "通过合成获得的装备", 20, 20, 100, 50, 500, "gold", 5);
                        }
                        
                        playerData.EquipmentInventory.Add(resultEquipment);
                        playerData.TotalEquipment++;
                        playerData.LastUpdateTime = DateTime.Now;
                        
                        dataManager.CreateEquipmentEvent("equipment_craft", playerID, recipe.ResultEquipmentID, "合成装备: " + resultEquipment.EquipmentName);
                        dataManager.SaveEquipmentData();
                        Debug.Log("玩家 " + playerID + " 合成了装备: " + resultEquipment.EquipmentName);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public List<EquipmentData> GetEquipmentInventory(string playerID)
    {
        PlayerEquipmentData playerData = GetPlayerEquipmentData(playerID);
        return playerData.EquipmentInventory;
    }

    public List<EquipmentData> GetEquippedEquipment(string playerID)
    {
        PlayerEquipmentData playerData = GetPlayerEquipmentData(playerID);
        return playerData.EquippedEquipment;
    }

    public List<EquipmentData> GetAvailableEquipment()
    {
        return equipmentData.AvailableEquipment;
    }

    public List<EquipmentRecipe> GetAvailableRecipes()
    {
        return equipmentData.AvailableRecipes;
    }

    public EquipmentData GetEquipmentByID(string equipmentID)
    {
        return equipmentData.AvailableEquipment.Find(e => e.EquipmentID == equipmentID);
    }

    public void SaveData()
    {
        dataManager.SaveEquipmentData();
    }

    public void LoadData()
    {
        dataManager.LoadEquipmentData();
    }

    public List<EquipmentEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class EventData
{
    public string EventID;
    public string EventName;
    public string EventType;
    public string EventDescription;
    public DateTime StartTime;
    public DateTime EndTime;
    public bool IsActive;
    public bool IsCompleted;
    public string EventIcon;
    public string EventBanner;
    public List<string> EventRewards;
    public Dictionary<string, object> EventDataDict;

    public EventData(string eventID, string eventName, string eventType, string eventDescription, DateTime startTime, DateTime endTime)
    {
        EventID = eventID;
        EventName = eventName;
        EventType = eventType;
        EventDescription = eventDescription;
        StartTime = startTime;
        EndTime = endTime;
        IsActive = false;
        IsCompleted = false;
        EventIcon = "Events/" + eventID + "_icon";
        EventBanner = "Events/" + eventID + "_banner";
        EventRewards = new List<string>();
        EventDataDict = new Dictionary<string, object>();
    }
}

[Serializable]
public class PlayerEventData
{
    public string PlayerID;
    public Dictionary<string, EventData> ActiveEvents;
    public Dictionary<string, EventData> CompletedEvents;
    public Dictionary<string, EventData> ExpiredEvents;
    public int TotalEvents;
    public int CompletedEventsCount;
    public DateTime LastUpdateTime;

    public PlayerEventData(string playerID)
    {
        PlayerID = playerID;
        ActiveEvents = new Dictionary<string, EventData>();
        CompletedEvents = new Dictionary<string, EventData>();
        ExpiredEvents = new Dictionary<string, EventData>();
        TotalEvents = 0;
        CompletedEventsCount = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class EventSystemData
{
    public List<EventData> GlobalEvents;
    public Dictionary<string, PlayerEventData> PlayerEvents;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public EventSystemData()
    {
        GlobalEvents = new List<EventData>();
        PlayerEvents = new Dictionary<string, PlayerEventData>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultEvents();
    }

    private void InitializeDefaultEvents()
    {
        GlobalEvents.Add(new EventData("event_newbie", "新手任务", "newbie", "完成新手任务获得奖励", DateTime.Now, DateTime.Now.AddDays(7)));
        GlobalEvents.Add(new EventData("event_daily_login", "每日登录", "daily", "每日登录获得奖励", DateTime.Now, DateTime.Now.AddMonths(1)));
        GlobalEvents.Add(new EventData("event_weekend_bonus", "周末双倍", "weekend", "周末战斗获得双倍经验和金币", DateTime.Now, DateTime.Now.AddDays(2)));
        GlobalEvents.Add(new EventData("event_season_1", "第一赛季", "season", "参与第一赛季获得赛季奖励", DateTime.Now, DateTime.Now.AddMonths(3)));
    }
}

[Serializable]
public class EventEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string TargetEventID;
    public string EventData;

    public EventEvent(string eventID, string eventType, string playerID, string targetEventID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        TargetEventID = targetEventID;
        EventData = eventData;
    }
}

public class EventSystemDataManager
{
    private static EventSystemDataManager _instance;
    public static EventSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EventSystemDataManager();
            }
            return _instance;
        }
    }

    public EventSystemData eventData;
    private List<EventEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private EventSystemDataManager()
    {
        eventData = new EventSystemData();
        recentEvents = new List<EventEvent>();
        LoadEventData();
    }

    public void SaveEventData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EventSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, eventData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存活动系统数据失败: " + e.Message);
        }
    }

    public void LoadEventData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EventSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    eventData = (EventSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载活动系统数据失败: " + e.Message);
            eventData = new EventSystemData();
        }
    }

    public void CreateEventEvent(string eventType, string playerID, string targetEventID, string eventData)
    {
        string eventID = "event_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EventEvent eventEvent = new EventEvent(eventID, eventType, playerID, targetEventID, eventData);
        recentEvents.Add(eventEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<EventEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class EventSystemDetailedManager
{
    private static EventSystemDetailedManager _instance;
    public static EventSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EventSystemDetailedManager();
            }
            return _instance;
        }
    }

    private EventSystemData eventData;
    private EventSystemDataManager dataManager;

    private EventSystemDetailedManager()
    {
        dataManager = EventSystemDataManager.Instance;
        eventData = dataManager.eventData;
    }

    public PlayerEventData GetPlayerEventData(string playerID)
    {
        if (!eventData.PlayerEvents.ContainsKey(playerID))
        {
            eventData.PlayerEvents[playerID] = new PlayerEventData(playerID);
        }
        return eventData.PlayerEvents[playerID];
    }

    public void UpdateEventStatus(string playerID)
    {
        PlayerEventData playerData = GetPlayerEventData(playerID);
        DateTime now = DateTime.Now;
        
        foreach (var globalEvent in eventData.GlobalEvents)
        {
            if (now >= globalEvent.StartTime && now <= globalEvent.EndTime)
            {
                if (!playerData.ActiveEvents.ContainsKey(globalEvent.EventID) && 
                    !playerData.CompletedEvents.ContainsKey(globalEvent.EventID))
                {
                    globalEvent.IsActive = true;
                    playerData.ActiveEvents[globalEvent.EventID] = globalEvent;
                    playerData.TotalEvents++;
                    dataManager.CreateEventEvent("event_activate", playerID, globalEvent.EventID, "活动激活: " + globalEvent.EventName);
                }
            }
            else if (now > globalEvent.EndTime)
            {
                if (playerData.ActiveEvents.ContainsKey(globalEvent.EventID))
                {
                    globalEvent.IsActive = false;
                    playerData.ExpiredEvents[globalEvent.EventID] = globalEvent;
                    playerData.ActiveEvents.Remove(globalEvent.EventID);
                    dataManager.CreateEventEvent("event_expire", playerID, globalEvent.EventID, "活动过期: " + globalEvent.EventName);
                }
            }
        }
        
        playerData.LastUpdateTime = DateTime.Now;
        dataManager.SaveEventData();
    }

    public bool CompleteEvent(string playerID, string eventID, string completionData)
    {
        PlayerEventData playerData = GetPlayerEventData(playerID);
        if (playerData.ActiveEvents.ContainsKey(eventID))
        {
            EventData ev = playerData.ActiveEvents[eventID];
            ev.IsCompleted = true;
            ev.IsActive = false;
            playerData.CompletedEvents[eventID] = ev;
            playerData.ActiveEvents.Remove(eventID);
            playerData.CompletedEventsCount++;
            playerData.LastUpdateTime = DateTime.Now;
            
            dataManager.CreateEventEvent("event_complete", playerID, eventID, "活动完成: " + ev.EventName);
            dataManager.SaveEventData();
            Debug.Log("玩家 " + playerID + " 完成了活动: " + ev.EventName);
            return true;
        }
        return false;
    }

    public List<EventData> GetActiveEvents(string playerID)
    {
        UpdateEventStatus(playerID);
        PlayerEventData playerData = GetPlayerEventData(playerID);
        return playerData.ActiveEvents.Values.ToList();
    }

    public List<EventData> GetCompletedEvents(string playerID)
    {
        PlayerEventData playerData = GetPlayerEventData(playerID);
        return playerData.CompletedEvents.Values.ToList();
    }

    public List<EventData> GetExpiredEvents(string playerID)
    {
        PlayerEventData playerData = GetPlayerEventData(playerID);
        return playerData.ExpiredEvents.Values.ToList();
    }

    public List<EventData> GetAllEvents(string playerID)
    {
        UpdateEventStatus(playerID);
        PlayerEventData playerData = GetPlayerEventData(playerID);
        var allEvents = new List<EventData>();
        allEvents.AddRange(playerData.ActiveEvents.Values);
        allEvents.AddRange(playerData.CompletedEvents.Values);
        allEvents.AddRange(playerData.ExpiredEvents.Values);
        return allEvents;
    }

    public EventData GetEventByID(string eventID)
    {
        return eventData.GlobalEvents.Find(e => e.EventID == eventID);
    }

    public void AddEventProgress(string playerID, string eventID, string progressData)
    {
        PlayerEventData playerData = GetPlayerEventData(playerID);
        if (playerData.ActiveEvents.ContainsKey(eventID))
        {
            EventData ev = playerData.ActiveEvents[eventID];
            if (!ev.EventDataDict.ContainsKey("progress"))
            {
                ev.EventDataDict["progress"] = new List<string>();
            }
            ((List<string>)ev.EventDataDict["progress"]).Add(progressData);
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.SaveEventData();
        }
    }

    public void AddEventReward(string playerID, string eventID, string reward)
    {
        PlayerEventData playerData = GetPlayerEventData(playerID);
        if (playerData.ActiveEvents.ContainsKey(eventID))
        {
            EventData ev = playerData.ActiveEvents[eventID];
            ev.EventRewards.Add(reward);
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.SaveEventData();
        }
    }

    public void CreateCustomEvent(string eventID, string eventName, string eventType, string eventDescription, DateTime startTime, DateTime endTime)
    {
        EventData newEvent = new EventData(eventID, eventName, eventType, eventDescription, startTime, endTime);
        eventData.GlobalEvents.Add(newEvent);
        dataManager.SaveEventData();
        Debug.Log("创建了自定义活动: " + eventName);
    }

    public void SaveData()
    {
        dataManager.SaveEventData();
    }

    public void LoadData()
    {
        dataManager.LoadEventData();
    }

    public List<EventEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class MailData
{
    public string MailID;
    public string MailType;
    public string SenderName;
    public string Subject;
    public string Content;
    public List<string> Attachments;
    public bool IsRead;
    public bool IsClaimed;
    public DateTime SendTime;
    public DateTime ExpireTime;
    public string MailIcon;

    public MailData(string mailID, string mailType, string senderName, string subject, string content, DateTime expireTime)
    {
        MailID = mailID;
        MailType = mailType;
        SenderName = senderName;
        Subject = subject;
        Content = content;
        Attachments = new List<string>();
        IsRead = false;
        IsClaimed = false;
        SendTime = DateTime.Now;
        ExpireTime = expireTime;
        MailIcon = "Mails/" + mailType + "_icon";
    }
}

[Serializable]
public class PlayerMailData
{
    public string PlayerID;
    public List<MailData> Inbox;
    public List<MailData> Sent;
    public List<MailData> Trash;
    public int TotalMails;
    public int UnreadMails;
    public DateTime LastUpdateTime;

    public PlayerMailData(string playerID)
    {
        PlayerID = playerID;
        Inbox = new List<MailData>();
        Sent = new List<MailData>();
        Trash = new List<MailData>();
        TotalMails = 0;
        UnreadMails = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class MailSystemData
{
    public Dictionary<string, PlayerMailData> PlayerMails;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public MailSystemData()
    {
        PlayerMails = new Dictionary<string, PlayerMailData>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class MailEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string MailID;
    public string EventData;

    public MailEvent(string eventID, string eventType, string playerID, string mailID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        MailID = mailID;
        EventData = eventData;
    }
}

public class MailSystemDataManager
{
    private static MailSystemDataManager _instance;
    public static MailSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MailSystemDataManager();
            }
            return _instance;
        }
    }

    public MailSystemData mailData;
    private List<MailEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private MailSystemDataManager()
    {
        mailData = new MailSystemData();
        recentEvents = new List<MailEvent>();
        LoadMailData();
    }

    public void SaveMailData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MailSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, mailData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存邮件系统数据失败: " + e.Message);
        }
    }

    public void LoadMailData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MailSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    mailData = (MailSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载邮件系统数据失败: " + e.Message);
            mailData = new MailSystemData();
        }
    }

    public void CreateMailEvent(string eventType, string playerID, string mailID, string eventData)
    {
        string eventID = "mail_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MailEvent mailEvent = new MailEvent(eventID, eventType, playerID, mailID, eventData);
        recentEvents.Add(mailEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<MailEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class MailSystemDetailedManager
{
    private static MailSystemDetailedManager _instance;
    public static MailSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MailSystemDetailedManager();
            }
            return _instance;
        }
    }

    private MailSystemData mailData;
    private MailSystemDataManager dataManager;

    private MailSystemDetailedManager()
    {
        dataManager = MailSystemDataManager.Instance;
        mailData = dataManager.mailData;
    }

    public PlayerMailData GetPlayerMailData(string playerID)
    {
        if (!mailData.PlayerMails.ContainsKey(playerID))
        {
            mailData.PlayerMails[playerID] = new PlayerMailData(playerID);
        }
        return mailData.PlayerMails[playerID];
    }

    public string SendMail(string senderID, string receiverID, string mailType, string subject, string content, List<string> attachments = null)
    {
        string mailID = "mail_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        DateTime expireTime = DateTime.Now.AddDays(7);
        MailData mail = new MailData(mailID, mailType, "系统", subject, content, expireTime);
        
        if (attachments != null)
        {
            mail.Attachments.AddRange(attachments);
        }
        
        PlayerMailData receiverData = GetPlayerMailData(receiverID);
        receiverData.Inbox.Add(mail);
        receiverData.TotalMails++;
        receiverData.UnreadMails++;
        receiverData.LastUpdateTime = DateTime.Now;
        
        if (!string.IsNullOrEmpty(senderID))
        {
            PlayerMailData senderData = GetPlayerMailData(senderID);
            senderData.Sent.Add(mail);
            senderData.LastUpdateTime = DateTime.Now;
        }
        
        dataManager.CreateMailEvent("mail_send", receiverID, mailID, "发送邮件: " + subject);
        dataManager.SaveMailData();
        Debug.Log("发送邮件给玩家 " + receiverID + ": " + subject);
        return mailID;
    }

    public void SendSystemMail(string receiverID, string subject, string content, List<string> attachments = null)
    {
        SendMail("system", receiverID, "system", subject, content, attachments);
    }

    public void SendRewardMail(string receiverID, string subject, string content, List<string> rewards)
    {
        SendMail("system", receiverID, "reward", subject, content, rewards);
    }

    public void MarkAsRead(string playerID, string mailID)
    {
        PlayerMailData playerData = GetPlayerMailData(playerID);
        MailData mail = playerData.Inbox.Find(m => m.MailID == mailID);
        if (mail != null && !mail.IsRead)
        {
            mail.IsRead = true;
            playerData.UnreadMails--;
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.CreateMailEvent("mail_read", playerID, mailID, "标记邮件为已读");
            dataManager.SaveMailData();
        }
    }

    public void ClaimAttachments(string playerID, string mailID)
    {
        PlayerMailData playerData = GetPlayerMailData(playerID);
        MailData mail = playerData.Inbox.Find(m => m.MailID == mailID);
        if (mail != null && !mail.IsClaimed && mail.Attachments.Count > 0)
        {
            mail.IsClaimed = true;
            playerData.LastUpdateTime = DateTime.Now;
            
            foreach (var attachment in mail.Attachments)
            {
                Debug.Log("玩家 " + playerID + " 领取了附件: " + attachment);
            }
            
            dataManager.CreateMailEvent("mail_claim", playerID, mailID, "领取邮件附件");
            dataManager.SaveMailData();
        }
    }

    public void DeleteMail(string playerID, string mailID)
    {
        PlayerMailData playerData = GetPlayerMailData(playerID);
        MailData mail = playerData.Inbox.Find(m => m.MailID == mailID);
        if (mail != null)
        {
            playerData.Inbox.Remove(mail);
            playerData.Trash.Add(mail);
            playerData.TotalMails--;
            if (!mail.IsRead)
            {
                playerData.UnreadMails--;
            }
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.CreateMailEvent("mail_delete", playerID, mailID, "删除邮件");
            dataManager.SaveMailData();
        }
    }

    public void EmptyTrash(string playerID)
    {
        PlayerMailData playerData = GetPlayerMailData(playerID);
        playerData.Trash.Clear();
        playerData.LastUpdateTime = DateTime.Now;
        dataManager.CreateMailEvent("mail_empty_trash", playerID, "", "清空回收站");
        dataManager.SaveMailData();
    }

    public List<MailData> GetInbox(string playerID)
    {
        PlayerMailData playerData = GetPlayerMailData(playerID);
        return playerData.Inbox;
    }

    public List<MailData> GetSentMails(string playerID)
    {
        PlayerMailData playerData = GetPlayerMailData(playerID);
        return playerData.Sent;
    }

    public List<MailData> GetTrash(string playerID)
    {
        PlayerMailData playerData = GetPlayerMailData(playerID);
        return playerData.Trash;
    }

    public int GetUnreadMailCount(string playerID)
    {
        PlayerMailData playerData = GetPlayerMailData(playerID);
        return playerData.UnreadMails;
    }

    public void CleanExpiredMails(string playerID)
    {
        PlayerMailData playerData = GetPlayerMailData(playerID);
        DateTime now = DateTime.Now;
        var expiredMails = playerData.Inbox.FindAll(m => m.ExpireTime < now);
        
        foreach (var mail in expiredMails)
        {
            playerData.Inbox.Remove(mail);
            playerData.Trash.Add(mail);
            if (!mail.IsRead)
            {
                playerData.UnreadMails--;
            }
        }
        
        playerData.LastUpdateTime = DateTime.Now;
        dataManager.SaveMailData();
    }

    public void SaveData()
    {
        dataManager.SaveMailData();
    }

    public void LoadData()
    {
        dataManager.LoadMailData();
    }

    public List<MailEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class QuestData
{
    public string QuestID;
    public string QuestName;
    public string QuestType;
    public string QuestDescription;
    public int RequiredProgress;
    public int CurrentProgress;
    public List<string> Rewards;
    public bool IsCompleted;
    public bool IsClaimed;
    public DateTime StartTime;
    public DateTime EndTime;
    public string QuestIcon;

    public QuestData(string questID, string questName, string questType, string questDescription, int requiredProgress, DateTime endTime)
    {
        QuestID = questID;
        QuestName = questName;
        QuestType = questType;
        QuestDescription = questDescription;
        RequiredProgress = requiredProgress;
        CurrentProgress = 0;
        Rewards = new List<string>();
        IsCompleted = false;
        IsClaimed = false;
        StartTime = DateTime.Now;
        EndTime = endTime;
        QuestIcon = "Quests/" + questType + "_icon";
    }
}

[Serializable]
public class PlayerQuestData
{
    public string PlayerID;
    public List<QuestData> ActiveQuests;
    public List<QuestData> CompletedQuests;
    public List<QuestData> ExpiredQuests;
    public int TotalQuests;
    public int CompletedQuestsCount;
    public DateTime LastUpdateTime;

    public PlayerQuestData(string playerID)
    {
        PlayerID = playerID;
        ActiveQuests = new List<QuestData>();
        CompletedQuests = new List<QuestData>();
        ExpiredQuests = new List<QuestData>();
        TotalQuests = 0;
        CompletedQuestsCount = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class QuestSystemData
{
    public List<QuestData> GlobalQuests;
    public Dictionary<string, PlayerQuestData> PlayerQuests;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public QuestSystemData()
    {
        GlobalQuests = new List<QuestData>();
        PlayerQuests = new Dictionary<string, PlayerQuestData>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultQuests();
    }

    private void InitializeDefaultQuests()
    {
        GlobalQuests.Add(new QuestData("quest_daily_win", "每日胜利", "daily", "获得一场胜利", 1, DateTime.Now.AddDays(1)));
        GlobalQuests.Add(new QuestData("quest_daily_kill", "每日击杀", "daily", "击杀10个敌人", 10, DateTime.Now.AddDays(1)));
        GlobalQuests.Add(new QuestData("quest_weekly_win", "每周胜利", "weekly", "获得5场胜利", 5, DateTime.Now.AddDays(7)));
        GlobalQuests.Add(new QuestData("quest_first_win", "首胜", "achievement", "获得第一场胜利", 1, DateTime.MaxValue));
        GlobalQuests.Add(new QuestData("quest_100_wins", "百战百胜", "achievement", "获得100场胜利", 100, DateTime.MaxValue));
    }
}

[Serializable]
public class QuestEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string QuestID;
    public string EventData;

    public QuestEvent(string eventID, string eventType, string playerID, string questID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        QuestID = questID;
        EventData = eventData;
    }
}

public class QuestSystemDataManager
{
    private static QuestSystemDataManager _instance;
    public static QuestSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new QuestSystemDataManager();
            }
            return _instance;
        }
    }

    public QuestSystemData questData;
    private List<QuestEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private QuestSystemDataManager()
    {
        questData = new QuestSystemData();
        recentEvents = new List<QuestEvent>();
        LoadQuestData();
    }

    public void SaveQuestData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "QuestSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, questData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存任务系统数据失败: " + e.Message);
        }
    }

    public void LoadQuestData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "QuestSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    questData = (QuestSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载任务系统数据失败: " + e.Message);
            questData = new QuestSystemData();
        }
    }

    public void CreateQuestEvent(string eventType, string playerID, string questID, string eventData)
    {
        string eventID = "quest_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        QuestEvent questEvent = new QuestEvent(eventID, eventType, playerID, questID, eventData);
        recentEvents.Add(questEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<QuestEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class QuestSystemDetailedManager
{
    private static QuestSystemDetailedManager _instance;
    public static QuestSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new QuestSystemDetailedManager();
            }
            return _instance;
        }
    }

    private QuestSystemData questData;
    private QuestSystemDataManager dataManager;

    private QuestSystemDetailedManager()
    {
        dataManager = QuestSystemDataManager.Instance;
        questData = dataManager.questData;
    }

    public PlayerQuestData GetPlayerQuestData(string playerID)
    {
        if (!questData.PlayerQuests.ContainsKey(playerID))
        {
            questData.PlayerQuests[playerID] = new PlayerQuestData(playerID);
        }
        return questData.PlayerQuests[playerID];
    }

    public void UpdateQuestStatus(string playerID)
    {
        PlayerQuestData playerData = GetPlayerQuestData(playerID);
        DateTime now = DateTime.Now;
        
        foreach (var globalQuest in questData.GlobalQuests)
        {
            if (now >= globalQuest.StartTime && now <= globalQuest.EndTime)
            {
                if (!playerData.ActiveQuests.Exists(q => q.QuestID == globalQuest.QuestID) && 
                    !playerData.CompletedQuests.Exists(q => q.QuestID == globalQuest.QuestID))
                {
                    QuestData newQuest = new QuestData(
                        globalQuest.QuestID, globalQuest.QuestName, globalQuest.QuestType, 
                        globalQuest.QuestDescription, globalQuest.RequiredProgress, globalQuest.EndTime
                    );
                    newQuest.Rewards = globalQuest.Rewards;
                    playerData.ActiveQuests.Add(newQuest);
                    playerData.TotalQuests++;
                    dataManager.CreateQuestEvent("quest_activate", playerID, newQuest.QuestID, "任务激活: " + newQuest.QuestName);
                }
            }
            else if (now > globalQuest.EndTime)
            {
                QuestData quest = playerData.ActiveQuests.Find(q => q.QuestID == globalQuest.QuestID);
                if (quest != null)
                {
                    playerData.ExpiredQuests.Add(quest);
                    playerData.ActiveQuests.Remove(quest);
                    dataManager.CreateQuestEvent("quest_expire", playerID, quest.QuestID, "任务过期: " + quest.QuestName);
                }
            }
        }
        
        playerData.LastUpdateTime = DateTime.Now;
        dataManager.SaveQuestData();
    }

    public void UpdateQuestProgress(string playerID, string questType, int progress)
    {
        PlayerQuestData playerData = GetPlayerQuestData(playerID);
        UpdateQuestStatus(playerID);
        
        foreach (var quest in playerData.ActiveQuests)
        {
            if (quest.QuestType == questType && !quest.IsCompleted)
            {
                quest.CurrentProgress += progress;
                if (quest.CurrentProgress >= quest.RequiredProgress)
                {
                    quest.CurrentProgress = quest.RequiredProgress;
                    quest.IsCompleted = true;
                    playerData.CompletedQuests.Add(quest);
                    playerData.ActiveQuests.Remove(quest);
                    playerData.CompletedQuestsCount++;
                    dataManager.CreateQuestEvent("quest_complete", playerID, quest.QuestID, "任务完成: " + quest.QuestName);
                }
            }
        }
        
        playerData.LastUpdateTime = DateTime.Now;
        dataManager.SaveQuestData();
    }

    public void UpdateQuestProgressByID(string playerID, string questID, int progress)
    {
        PlayerQuestData playerData = GetPlayerQuestData(playerID);
        QuestData quest = playerData.ActiveQuests.Find(q => q.QuestID == questID);
        
        if (quest != null && !quest.IsCompleted)
        {
            quest.CurrentProgress += progress;
            if (quest.CurrentProgress >= quest.RequiredProgress)
            {
                quest.CurrentProgress = quest.RequiredProgress;
                quest.IsCompleted = true;
                playerData.CompletedQuests.Add(quest);
                playerData.ActiveQuests.Remove(quest);
                playerData.CompletedQuestsCount++;
                dataManager.CreateQuestEvent("quest_complete", playerID, quest.QuestID, "任务完成: " + quest.QuestName);
            }
            
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.SaveQuestData();
        }
    }

    public bool ClaimQuestRewards(string playerID, string questID)
    {
        PlayerQuestData playerData = GetPlayerQuestData(playerID);
        QuestData quest = playerData.CompletedQuests.Find(q => q.QuestID == questID);
        
        if (quest != null && !quest.IsClaimed)
        {
            quest.IsClaimed = true;
            playerData.LastUpdateTime = DateTime.Now;
            
            foreach (var reward in quest.Rewards)
            {
                Debug.Log("玩家 " + playerID + " 领取了任务奖励: " + reward);
            }
            
            dataManager.CreateQuestEvent("quest_claim", playerID, questID, "领取任务奖励");
            dataManager.SaveQuestData();
            return true;
        }
        return false;
    }

    public List<QuestData> GetActiveQuests(string playerID)
    {
        UpdateQuestStatus(playerID);
        PlayerQuestData playerData = GetPlayerQuestData(playerID);
        return playerData.ActiveQuests;
    }

    public List<QuestData> GetCompletedQuests(string playerID)
    {
        PlayerQuestData playerData = GetPlayerQuestData(playerID);
        return playerData.CompletedQuests;
    }

    public List<QuestData> GetExpiredQuests(string playerID)
    {
        PlayerQuestData playerData = GetPlayerQuestData(playerID);
        return playerData.ExpiredQuests;
    }

    public QuestData GetQuestByID(string questID)
    {
        return questData.GlobalQuests.Find(q => q.QuestID == questID);
    }

    public void AddCustomQuest(string questID, string questName, string questType, string questDescription, int requiredProgress, DateTime endTime, List<string> rewards)
    {
        QuestData newQuest = new QuestData(questID, questName, questType, questDescription, requiredProgress, endTime);
        newQuest.Rewards = rewards;
        questData.GlobalQuests.Add(newQuest);
        dataManager.SaveQuestData();
        Debug.Log("创建了自定义任务: " + questName);
    }

    public void SaveData()
    {
        dataManager.SaveQuestData();
    }

    public void LoadData()
    {
        dataManager.LoadQuestData();
    }

    public List<QuestEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

[Serializable]
public class LeaderboardEntry
{
    public string PlayerID;
    public string PlayerName;
    public int Rank;
    public int Score;
    public int Wins;
    public int Losses;
    public float WinRate;
    public int KDA;
    public DateTime LastUpdateTime;

    public LeaderboardEntry(string playerID, string playerName, int score, int wins, int losses, int kda)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        Rank = 0;
        Score = score;
        Wins = wins;
        Losses = losses;
        WinRate = wins > 0 ? (float)wins / (wins + losses) * 100 : 0;
        KDA = kda;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class LeaderboardData
{
    public string LeaderboardID;
    public string LeaderboardName;
    public string LeaderboardType;
    public List<LeaderboardEntry> Entries;
    public int MaxEntries;
    public DateTime LastUpdateTime;

    public LeaderboardData(string leaderboardID, string leaderboardName, string leaderboardType, int maxEntries = 100)
    {
        LeaderboardID = leaderboardID;
        LeaderboardName = leaderboardName;
        LeaderboardType = leaderboardType;
        Entries = new List<LeaderboardEntry>();
        MaxEntries = maxEntries;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class LeaderboardSystemData
{
    public List<LeaderboardData> Leaderboards;
    public int SystemEnabled;
    public DateTime LastSystemUpdate;

    public LeaderboardSystemData()
    {
        Leaderboards = new List<LeaderboardData>();
        SystemEnabled = 1;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultLeaderboards();
    }

    private void InitializeDefaultLeaderboards()
    {
        Leaderboards.Add(new LeaderboardData("leaderboard_ranking", "段位排行榜", "ranking", 100));
        Leaderboards.Add(new LeaderboardData("leaderboard_kill", "击杀排行榜", "kill", 100));
        Leaderboards.Add(new LeaderboardData("leaderboard_win", "胜率排行榜", "win", 100));
        Leaderboards.Add(new LeaderboardData("leaderboard_kda", "KDA排行榜", "kda", 100));
        Leaderboards.Add(new LeaderboardData("leaderboard_gold", "金币排行榜", "gold", 100));
    }
}

[Serializable]
public class LeaderboardEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string LeaderboardID;
    public string EventData;

    public LeaderboardEvent(string eventID, string eventType, string playerID, string leaderboardID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        LeaderboardID = leaderboardID;
        EventData = eventData;
    }
}

public class LeaderboardSystemDataManager
{
    private static LeaderboardSystemDataManager _instance;
    public static LeaderboardSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LeaderboardSystemDataManager();
            }
            return _instance;
        }
    }

    public LeaderboardSystemData leaderboardData;
    private List<LeaderboardEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private LeaderboardSystemDataManager()
    {
        leaderboardData = new LeaderboardSystemData();
        recentEvents = new List<LeaderboardEvent>();
        LoadLeaderboardData();
    }

    public void SaveLeaderboardData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "LeaderboardSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, leaderboardData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存排行榜系统数据失败: " + e.Message);
        }
    }

    public void LoadLeaderboardData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "LeaderboardSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    leaderboardData = (LeaderboardSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载排行榜系统数据失败: " + e.Message);
            leaderboardData = new LeaderboardSystemData();
        }
    }

    public void CreateLeaderboardEvent(string eventType, string playerID, string leaderboardID, string eventData)
    {
        string eventID = "leaderboard_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        LeaderboardEvent leaderboardEvent = new LeaderboardEvent(eventID, eventType, playerID, leaderboardID, eventData);
        recentEvents.Add(leaderboardEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<LeaderboardEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class LeaderboardSystemDetailedManager
{
    private static LeaderboardSystemDetailedManager _instance;
    public static LeaderboardSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LeaderboardSystemDetailedManager();
            }
            return _instance;
        }
    }

    private LeaderboardSystemData leaderboardData;
    private LeaderboardSystemDataManager dataManager;

    private LeaderboardSystemDetailedManager()
    {
        dataManager = LeaderboardSystemDataManager.Instance;
        leaderboardData = dataManager.leaderboardData;
    }

    public void UpdatePlayerScore(string playerID, string playerName, string leaderboardType, int score, int wins, int losses, int kda)
    {
        LeaderboardData leaderboard = leaderboardData.Leaderboards.Find(lb => lb.LeaderboardType == leaderboardType);
        if (leaderboard != null)
        {
            LeaderboardEntry existingEntry = leaderboard.Entries.Find(entry => entry.PlayerID == playerID);
            if (existingEntry != null)
            {
                existingEntry.Score = score;
                existingEntry.Wins = wins;
                existingEntry.Losses = losses;
                existingEntry.WinRate = wins > 0 ? (float)wins / (wins + losses) * 100 : 0;
                existingEntry.KDA = kda;
                existingEntry.LastUpdateTime = DateTime.Now;
            }
            else
            {
                LeaderboardEntry newEntry = new LeaderboardEntry(playerID, playerName, score, wins, losses, kda);
                leaderboard.Entries.Add(newEntry);
            }
            
            SortLeaderboard(leaderboard);
            TrimLeaderboard(leaderboard);
            UpdateRanks(leaderboard);
            
            leaderboard.LastUpdateTime = DateTime.Now;
            dataManager.CreateLeaderboardEvent("score_update", playerID, leaderboard.LeaderboardID, "更新排行榜分数");
            dataManager.SaveLeaderboardData();
        }
    }

    private void SortLeaderboard(LeaderboardData leaderboard)
    {
        switch (leaderboard.LeaderboardType)
        {
            case "ranking":
            case "kill":
            case "gold":
                leaderboard.Entries.Sort((a, b) => b.Score.CompareTo(a.Score));
                break;
            case "win":
                leaderboard.Entries.Sort((a, b) => b.WinRate.CompareTo(a.WinRate));
                break;
            case "kda":
                leaderboard.Entries.Sort((a, b) => b.KDA.CompareTo(a.KDA));
                break;
        }
    }

    private void TrimLeaderboard(LeaderboardData leaderboard)
    {
        if (leaderboard.Entries.Count > leaderboard.MaxEntries)
        {
            leaderboard.Entries = leaderboard.Entries.GetRange(0, leaderboard.MaxEntries);
        }
    }

    private void UpdateRanks(LeaderboardData leaderboard)
    {
        for (int i = 0; i < leaderboard.Entries.Count; i++)
        {
            leaderboard.Entries[i].Rank = i + 1;
        }
    }

    public List<LeaderboardEntry> GetLeaderboard(string leaderboardType, int count = 50)
    {
        LeaderboardData leaderboard = leaderboardData.Leaderboards.Find(lb => lb.LeaderboardType == leaderboardType);
        if (leaderboard != null)
        {
            if (count > leaderboard.Entries.Count)
            {
                count = leaderboard.Entries.Count;
            }
            return leaderboard.Entries.GetRange(0, count);
        }
        return new List<LeaderboardEntry>();
    }

    public LeaderboardEntry GetPlayerRank(string playerID, string leaderboardType)
    {
        LeaderboardData leaderboard = leaderboardData.Leaderboards.Find(lb => lb.LeaderboardType == leaderboardType);
        if (leaderboard != null)
        {
            return leaderboard.Entries.Find(entry => entry.PlayerID == playerID);
        }
        return null;
    }

    public int GetPlayerGlobalRank(string playerID, string leaderboardType)
    {
        LeaderboardEntry entry = GetPlayerRank(playerID, leaderboardType);
        return entry != null ? entry.Rank : 0;
    }

    public void RefreshAllLeaderboards()
    {
        foreach (var leaderboard in leaderboardData.Leaderboards)
        {
            SortLeaderboard(leaderboard);
            TrimLeaderboard(leaderboard);
            UpdateRanks(leaderboard);
            leaderboard.LastUpdateTime = DateTime.Now;
        }
        dataManager.SaveLeaderboardData();
    }

    public void AddCustomLeaderboard(string leaderboardID, string leaderboardName, string leaderboardType, int maxEntries = 100)
    {
        LeaderboardData newLeaderboard = new LeaderboardData(leaderboardID, leaderboardName, leaderboardType, maxEntries);
        leaderboardData.Leaderboards.Add(newLeaderboard);
        dataManager.SaveLeaderboardData();
        Debug.Log("创建了自定义排行榜: " + leaderboardName);
    }

    public List<LeaderboardData> GetAllLeaderboards()
    {
        return leaderboardData.Leaderboards;
    }

    public void SaveData()
    {
        dataManager.SaveLeaderboardData();
    }

    public void LoadData()
    {
        dataManager.LoadLeaderboardData();
    }

    public List<LeaderboardEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}

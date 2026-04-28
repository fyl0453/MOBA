using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class PlayerProfile
{
    public string PlayerID;
    public string PlayerName;
    public string Avatar;
    public int Level;
    public int Exp;
    public string Title;
    public string Frame;
    public string Background;
    public string Signature;
    public int TotalGames;
    public int TotalWins;
    public int TotalLosses;
    public float WinRate;
    public int MaxRankLevel;
    public string CurrentRank;
    public int HonorLevel;
    public List<string> RecentHeroes;
    public List<string> FavoriteHeroes;
    public List<string> OwnedSkins;
    public List<string> Achievements;
    public DateTime LastLoginTime;
    public DateTime RegisterTime;

    public PlayerProfile(string playerID, string playerName)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        Avatar = "default_avatar";
        Level = 1;
        Exp = 0;
        Title = "";
        Frame = "default_frame";
        Background = "default_background";
        Signature = "";
        TotalGames = 0;
        TotalWins = 0;
        TotalLosses = 0;
        WinRate = 0;
        MaxRankLevel = 1;
        CurrentRank = "坚韧黑铁";
        HonorLevel = 1;
        RecentHeroes = new List<string>();
        FavoriteHeroes = new List<string>();
        OwnedSkins = new List<string>();
        Achievements = new List<string>();
        LastLoginTime = DateTime.Now;
        RegisterTime = DateTime.Now;
    }
}

[Serializable]
public class PlayerStats
{
    public string PlayerID;
    public Dictionary<string, HeroStats> HeroStats;
    public Dictionary<string, ModeStats> ModeStats;
    public int TotalKills;
    public int TotalDeaths;
    public int TotalAssists;
    public float KDA;
    public int TotalGoldEarned;
    public int TotalDamageDealt;
    public int TotalHealingDone;
    public int TotalObjectivesTaken;
    public int TotalMinionsKilled;
    public DateTime LastUpdateTime;

    public PlayerStats(string playerID)
    {
        PlayerID = playerID;
        HeroStats = new Dictionary<string, HeroStats>();
        ModeStats = new Dictionary<string, ModeStats>();
        TotalKills = 0;
        TotalDeaths = 0;
        TotalAssists = 0;
        KDA = 0;
        TotalGoldEarned = 0;
        TotalDamageDealt = 0;
        TotalHealingDone = 0;
        TotalObjectivesTaken = 0;
        TotalMinionsKilled = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class HeroStats
{
    public string HeroID;
    public int PlayCount;
    public int WinCount;
    public float WinRate;
    public int TotalKills;
    public int TotalDeaths;
    public int TotalAssists;
    public float KDA;
    public int HighestKillStreak;
    public int TotalDamageDealt;
    public int TotalHealingDone;
    public DateTime LastPlayed;

    public HeroStats(string heroID)
    {
        HeroID = heroID;
        PlayCount = 0;
        WinCount = 0;
        WinRate = 0;
        TotalKills = 0;
        TotalDeaths = 0;
        TotalAssists = 0;
        KDA = 0;
        HighestKillStreak = 0;
        TotalDamageDealt = 0;
        TotalHealingDone = 0;
        LastPlayed = DateTime.MinValue;
    }
}

[Serializable]
public class ModeStats
{
    public string ModeID;
    public int PlayCount;
    public int WinCount;
    public float WinRate;
    public int TotalKills;
    public int TotalDeaths;
    public int TotalAssists;
    public float KDA;
    public int TotalGames;

    public ModeStats(string modeID)
    {
        ModeID = modeID;
        PlayCount = 0;
        WinCount = 0;
        WinRate = 0;
        TotalKills = 0;
        TotalDeaths = 0;
        TotalAssists = 0;
        KDA = 0;
        TotalGames = 0;
    }
}

[Serializable]
public class PlayerCollection
{
    public string PlayerID;
    public List<string> OwnedHeroes;
    public List<string> OwnedSkins;
    public List<string> OwnedItems;
    public List<string> OwnedTitles;
    public List<string> OwnedFrames;
    public List<string> OwnedBackgrounds;
    public int CollectionCompletion;
    public DateTime LastUpdateTime;

    public PlayerCollection(string playerID)
    {
        PlayerID = playerID;
        OwnedHeroes = new List<string>();
        OwnedSkins = new List<string>();
        OwnedItems = new List<string>();
        OwnedTitles = new List<string>();
        OwnedFrames = new List<string>();
        OwnedBackgrounds = new List<string>();
        CollectionCompletion = 0;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class ProfileSystemData
{
    public Dictionary<string, PlayerProfile> PlayerProfiles;
    public Dictionary<string, PlayerStats> PlayerStats;
    public Dictionary<string, PlayerCollection> PlayerCollections;
    public DateTime LastCleanupTime;

    public ProfileSystemData()
    {
        PlayerProfiles = new Dictionary<string, PlayerProfile>();
        PlayerStats = new Dictionary<string, PlayerStats>();
        PlayerCollections = new Dictionary<string, PlayerCollection>();
        LastCleanupTime = DateTime.Now;
    }

    public void AddPlayerProfile(string playerID, PlayerProfile profile)
    {
        PlayerProfiles[playerID] = profile;
    }

    public void AddPlayerStats(string playerID, PlayerStats stats)
    {
        PlayerStats[playerID] = stats;
    }

    public void AddPlayerCollection(string playerID, PlayerCollection collection)
    {
        PlayerCollections[playerID] = collection;
    }
}

[Serializable]
public class ProfileEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EventData;

    public ProfileEvent(string eventID, string eventType, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class ProfileSystemDataManager
{
    private static ProfileSystemDataManager _instance;
    public static ProfileSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ProfileSystemDataManager();
            }
            return _instance;
        }
    }

    public ProfileSystemData profileData;
    private List<ProfileEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private ProfileSystemDataManager()
    {
        profileData = new ProfileSystemData();
        recentEvents = new List<ProfileEvent>();
        LoadProfileData();
    }

    public void SaveProfileData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ProfileSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, profileData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存个人主页数据失败: " + e.Message);
        }
    }

    public void LoadProfileData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ProfileSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    profileData = (ProfileSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载个人主页数据失败: " + e.Message);
            profileData = new ProfileSystemData();
        }
    }

    public void CreateProfileEvent(string eventType, string playerID, string eventData)
    {
        string eventID = "profile_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ProfileEvent profileEvent = new ProfileEvent(eventID, eventType, playerID, eventData);
        recentEvents.Add(profileEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<ProfileEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
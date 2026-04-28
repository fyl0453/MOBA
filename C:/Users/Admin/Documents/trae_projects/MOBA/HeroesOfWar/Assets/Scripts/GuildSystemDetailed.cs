using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Guild
{
    public string GuildID;
    public string GuildName;
    public string GuildTag;
    public string LeaderID;
    public string LeaderName;
    public int Level;
    public int Exp;
    public int MaxMembers;
    public List<GuildMember> Members;
    public List<GuildRequest> JoinRequests;
    public List<GuildActivity> Activities;
    public string Description;
    public string Announcement;
    public DateTime CreateTime;
    public int TotalContribution;

    public Guild(string guildID, string guildName, string guildTag, string leaderID, string leaderName)
    {
        GuildID = guildID;
        GuildName = guildName;
        GuildTag = guildTag;
        LeaderID = leaderID;
        LeaderName = leaderName;
        Level = 1;
        Exp = 0;
        MaxMembers = 30;
        Members = new List<GuildMember>();
        JoinRequests = new List<GuildRequest>();
        Activities = new List<GuildActivity>();
        Description = "";
        Announcement = "";
        CreateTime = DateTime.Now;
        TotalContribution = 0;
    }
}

[Serializable]
public class GuildMember
{
    public string MemberID;
    public string PlayerID;
    public string PlayerName;
    public string GuildID;
    public int Role;
    public int Contribution;
    public int WeeklyContribution;
    public DateTime JoinTime;
    public DateTime LastOnlineTime;
    public bool IsOnline;

    public GuildMember(string memberID, string playerID, string playerName, string guildID, int role)
    {
        MemberID = memberID;
        PlayerID = playerID;
        PlayerName = playerName;
        GuildID = guildID;
        Role = role;
        Contribution = 0;
        WeeklyContribution = 0;
        JoinTime = DateTime.Now;
        LastOnlineTime = DateTime.Now;
        IsOnline = false;
    }
}

[Serializable]
public class GuildRequest
{
    public string RequestID;
    public string PlayerID;
    public string PlayerName;
    public string GuildID;
    public int PlayerLevel;
    public int PlayerRank;
    public DateTime RequestTime;
    public int Status;

    public GuildRequest(string requestID, string playerID, string playerName, string guildID, int playerLevel, int playerRank)
    {
        RequestID = requestID;
        PlayerID = playerID;
        PlayerName = playerName;
        GuildID = guildID;
        PlayerLevel = playerLevel;
        PlayerRank = playerRank;
        RequestTime = DateTime.Now;
        Status = 0;
    }
}

[Serializable]
public class GuildActivity
{
    public string ActivityID;
    public string GuildID;
    public string ActivityName;
    public string ActivityType;
    public DateTime StartTime;
    public DateTime EndTime;
    public List<string> Participants;
    public int RequiredLevel;
    public bool IsActive;

    public GuildActivity(string activityID, string guildID, string activityName, string activityType, DateTime startTime, DateTime endTime, int requiredLevel)
    {
        ActivityID = activityID;
        GuildID = guildID;
        ActivityName = activityName;
        ActivityType = activityType;
        StartTime = startTime;
        EndTime = endTime;
        Participants = new List<string>();
        RequiredLevel = requiredLevel;
        IsActive = true;
    }
}

[Serializable]
public class GuildSystemData
{
    public List<Guild> Guilds;
    public List<GuildMember> GuildMembers;
    public List<GuildRequest> GuildRequests;
    public List<GuildActivity> GuildActivities;
    public Dictionary<string, string> PlayerGuilds;
    public DateTime LastCleanupTime;

    public GuildSystemData()
    {
        Guilds = new List<Guild>();
        GuildMembers = new List<GuildMember>();
        GuildRequests = new List<GuildRequest>();
        GuildActivities = new List<GuildActivity>();
        PlayerGuilds = new Dictionary<string, string>();
        LastCleanupTime = DateTime.Now;
    }

    public void AddGuild(Guild guild)
    {
        Guilds.Add(guild);
    }

    public void AddGuildMember(GuildMember member)
    {
        GuildMembers.Add(member);
        PlayerGuilds[member.PlayerID] = member.GuildID;
    }

    public void AddGuildRequest(GuildRequest request)
    {
        GuildRequests.Add(request);
    }

    public void AddGuildActivity(GuildActivity activity)
    {
        GuildActivities.Add(activity);
    }
}

[Serializable]
public class GuildEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string GuildID;
    public string EventData;

    public GuildEvent(string eventID, string eventType, string playerID, string guildID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        GuildID = guildID;
        EventData = eventData;
    }
}

public class GuildSystemDataManager
{
    private static GuildSystemDataManager _instance;
    public static GuildSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GuildSystemDataManager();
            }
            return _instance;
        }
    }

    public GuildSystemData guildData;
    private List<GuildEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private GuildSystemDataManager()
    {
        guildData = new GuildSystemData();
        recentEvents = new List<GuildEvent>();
        LoadGuildData();
    }

    public void SaveGuildData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "GuildSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, guildData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存公会系统数据失败: " + e.Message);
        }
    }

    public void LoadGuildData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "GuildSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    guildData = (GuildSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载公会系统数据失败: " + e.Message);
            guildData = new GuildSystemData();
        }
    }

    public void CreateGuildEvent(string eventType, string playerID, string guildID, string eventData)
    {
        string eventID = "guild_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuildEvent guildEvent = new GuildEvent(eventID, eventType, playerID, guildID, eventData);
        recentEvents.Add(guildEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<GuildEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
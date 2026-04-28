using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class GuildWar
{
    public string WarID;
    public string WarName;
    public string Guild1ID;
    public string Guild1Name;
    public string Guild2ID;
    public string Guild2Name;
    public DateTime WarStartTime;
    public DateTime WarEndTime;
    public string WarStatus;
    public string WarType;
    public int Guild1Score;
    public int Guild2Score;
    public string WinnerGuildID;
    public string WarResult;
    public List<string> BattleRecords;

    public GuildWar(string warName, string guild1ID, string guild1Name, string guild2ID, string guild2Name, string warType, DateTime startTime, DateTime endTime)
    {
        WarID = "guild_war_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        WarName = warName;
        Guild1ID = guild1ID;
        Guild1Name = guild1Name;
        Guild2ID = guild2ID;
        Guild2Name = guild2Name;
        WarStartTime = startTime;
        WarEndTime = endTime;
        WarStatus = "preparing";
        WarType = warType;
        Guild1Score = 0;
        Guild2Score = 0;
        WinnerGuildID = "";
        WarResult = "";
        BattleRecords = new List<string>();
    }
}

[Serializable]
public class GuildWarBattle
{
    public string BattleID;
    public string WarID;
    public string MatchID;
    public string AttackingGuildID;
    public string DefendingGuildID;
    public string AttackingTeamPlayer1ID;
    public string AttackingTeamPlayer2ID;
    public string DefendingTeamPlayer1ID;
    public string DefendingTeamPlayer2ID;
    public string AttackingTeamHero1ID;
    public string AttackingTeamHero2ID;
    public string DefendingTeamHero1ID;
    public string DefendingTeamHero2ID;
    public string WinnerTeam;
    public int Duration;
    public DateTime BattleTime;

    public GuildWarBattle(string warID, string matchID, string attackingGuildID, string defendingGuildID)
    {
        BattleID = "war_battle_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        WarID = warID;
        MatchID = matchID;
        AttackingGuildID = attackingGuildID;
        DefendingGuildID = defendingGuildID;
        AttackingTeamPlayer1ID = "";
        AttackingTeamPlayer2ID = "";
        DefendingTeamPlayer1ID = "";
        DefendingTeamPlayer2ID = "";
        AttackingTeamHero1ID = "";
        AttackingTeamHero2ID = "";
        DefendingTeamHero1ID = "";
        DefendingTeamHero2ID = "";
        WinnerTeam = "";
        Duration = 0;
        BattleTime = DateTime.Now;
    }
}

[Serializable]
public class GuildAlliance
{
    public string AllianceID;
    public string AllianceName;
    public List<string> MemberGuildIDs;
    public string LeaderGuildID;
    public DateTime CreatedTime;
    public string AllianceDescription;
    public int TotalMembers;
    public int AllianceLevel;

    public GuildAlliance(string allianceName, string leaderGuildID, string description)
    {
        AllianceID = "alliance_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AllianceName = allianceName;
        MemberGuildIDs = new List<string>();
        MemberGuildIDs.Add(leaderGuildID);
        LeaderGuildID = leaderGuildID;
        CreatedTime = DateTime.Now;
        AllianceDescription = description;
        TotalMembers = 0;
        AllianceLevel = 1;
    }
}

[Serializable]
public class GuildWarSystemData
{
    public List<GuildWar> AllGuildWars;
    public List<GuildWarBattle> AllWarBattles;
    public List<GuildAlliance> Alliances;
    public Dictionary<string, List<string>> GuildWarHistory;
    public int MaxWarsPerGuild;
    public int MaxAllianceMembers;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public GuildWarSystemData()
    {
        AllGuildWars = new List<GuildWar>();
        AllWarBattles = new List<GuildWarBattle>();
        Alliances = new List<GuildAlliance>();
        GuildWarHistory = new Dictionary<string, List<string>>();
        MaxWarsPerGuild = 5;
        MaxAllianceMembers = 10;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class GuildWarEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string GuildID;
    public string WarID;
    public string EventData;

    public GuildWarEvent(string eventID, string eventType, string guildID, string warID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        GuildID = guildID;
        WarID = warID;
        EventData = eventData;
    }
}

public class GuildWarSystemDataManager
{
    private static GuildWarSystemDataManager _instance;
    public static GuildWarSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GuildWarSystemDataManager();
            }
            return _instance;
        }
    }

    public GuildWarSystemData guildWarData;
    private List<GuildWarEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private GuildWarSystemDataManager()
    {
        guildWarData = new GuildWarSystemData();
        recentEvents = new List<GuildWarEvent>();
        LoadGuildWarData();
    }

    public void SaveGuildWarData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "GuildWarSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, guildWarData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存公会战系统数据失败: " + e.Message);
        }
    }

    public void LoadGuildWarData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "GuildWarSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    guildWarData = (GuildWarSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载公会战系统数据失败: " + e.Message);
            guildWarData = new GuildWarSystemData();
        }
    }

    public void CreateGuildWarEvent(string eventType, string guildID, string warID, string eventData)
    {
        string eventID = "guild_war_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuildWarEvent guildWarEvent = new GuildWarEvent(eventID, eventType, guildID, warID, eventData);
        recentEvents.Add(guildWarEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<GuildWarEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}

public class GuildWarSystemDetailedManager
{
    private static GuildWarSystemDetailedManager _instance;
    public static GuildWarSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GuildWarSystemDetailedManager();
            }
            return _instance;
        }
    }

    private GuildWarSystemData guildWarData;
    private GuildWarSystemDataManager dataManager;

    private GuildWarSystemDetailedManager()
    {
        dataManager = GuildWarSystemDataManager.Instance;
        guildWarData = dataManager.guildWarData;
    }

    public string CreateGuildWar(string warName, string guild1ID, string guild1Name, string guild2ID, string guild2Name, string warType, DateTime startTime, DateTime endTime)
    {
        GuildWar war = new GuildWar(warName, guild1ID, guild1Name, guild2ID, guild2Name, warType, startTime, endTime);
        guildWarData.AllGuildWars.Add(war);
        
        if (!guildWarData.GuildWarHistory.ContainsKey(guild1ID))
        {
            guildWarData.GuildWarHistory[guild1ID] = new List<string>();
        }
        guildWarData.GuildWarHistory[guild1ID].Add(war.WarID);
        
        if (!guildWarData.GuildWarHistory.ContainsKey(guild2ID))
        {
            guildWarData.GuildWarHistory[guild2ID] = new List<string>();
        }
        guildWarData.GuildWarHistory[guild2ID].Add(war.WarID);
        
        dataManager.CreateGuildWarEvent("war_create", guild1ID, war.WarID, "创建公会战: " + warName);
        dataManager.CreateGuildWarEvent("war_create", guild2ID, war.WarID, "创建公会战: " + warName);
        dataManager.SaveGuildWarData();
        Debug.Log("创建公会战成功: " + warName);
        return war.WarID;
    }

    public string CreateGuildWarBattle(string warID, string matchID, string attackingGuildID, string defendingGuildID)
    {
        GuildWar war = guildWarData.AllGuildWars.Find(w => w.WarID == warID);
        if (war == null)
        {
            return "";
        }
        
        GuildWarBattle battle = new GuildWarBattle(warID, matchID, attackingGuildID, defendingGuildID);
        guildWarData.AllWarBattles.Add(battle);
        war.BattleRecords.Add(battle.BattleID);
        
        dataManager.CreateGuildWarEvent("battle_create", attackingGuildID, warID, "发起公会战战斗");
        dataManager.SaveGuildWarData();
        Debug.Log("创建公会战战斗成功");
        return battle.BattleID;
    }

    public void EndGuildWarBattle(string battleID, string winnerTeam, int duration)
    {
        GuildWarBattle battle = guildWarData.AllWarBattles.Find(b => b.BattleID == battleID);
        if (battle == null)
        {
            return;
        }
        
        battle.WinnerTeam = winnerTeam;
        battle.Duration = duration;
        
        GuildWar war = guildWarData.AllGuildWars.Find(w => w.WarID == battle.WarID);
        if (war != null)
        {
            if (winnerTeam == "attacking")
            {
                war.Guild1Score++;
            }
            else
            {
                war.Guild2Score++;
            }
        }
        
        dataManager.CreateGuildWarEvent("battle_end", battle.AttackingGuildID, battle.WarID, "战斗结束: " + winnerTeam);
        dataManager.SaveGuildWarData();
        Debug.Log("公会战战斗结束: " + winnerTeam);
    }

    public void StartGuildWar(string warID)
    {
        GuildWar war = guildWarData.AllGuildWars.Find(w => w.WarID == warID);
        if (war != null && war.WarStatus == "preparing")
        {
            war.WarStatus = "ongoing";
            
            dataManager.CreateGuildWarEvent("war_start", war.Guild1ID, warID, "公会战开始: " + war.WarName);
            dataManager.CreateGuildWarEvent("war_start", war.Guild2ID, warID, "公会战开始: " + war.WarName);
            dataManager.SaveGuildWarData();
            Debug.Log("公会战开始: " + war.WarName);
        }
    }

    public void EndGuildWar(string warID)
    {
        GuildWar war = guildWarData.AllGuildWars.Find(w => w.WarID == warID);
        if (war != null && war.WarStatus == "ongoing")
        {
            war.WarStatus = "completed";
            
            if (war.Guild1Score > war.Guild2Score)
            {
                war.WinnerGuildID = war.Guild1ID;
                war.WarResult = war.Guild1Name + " 获胜";
            }
            else if (war.Guild2Score > war.Guild1Score)
            {
                war.WinnerGuildID = war.Guild2ID;
                war.WarResult = war.Guild2Name + " 获胜";
            }
            else
            {
                war.WarResult = "平局";
            }
            
            dataManager.CreateGuildWarEvent("war_end", war.Guild1ID, warID, "公会战结束: " + war.WarResult);
            dataManager.CreateGuildWarEvent("war_end", war.Guild2ID, warID, "公会战结束: " + war.WarResult);
            dataManager.SaveGuildWarData();
            Debug.Log("公会战结束: " + war.WarResult);
        }
    }

    public string CreateAlliance(string allianceName, string leaderGuildID, string description)
    {
        GuildAlliance alliance = new GuildAlliance(allianceName, leaderGuildID, description);
        guildWarData.Alliances.Add(alliance);
        
        dataManager.CreateGuildWarEvent("alliance_create", leaderGuildID, "", "创建联盟: " + allianceName);
        dataManager.SaveGuildWarData();
        Debug.Log("创建联盟成功: " + allianceName);
        return alliance.AllianceID;
    }

    public bool JoinAlliance(string allianceID, string guildID)
    {
        GuildAlliance alliance = guildWarData.Alliances.Find(a => a.AllianceID == allianceID);
        if (alliance == null)
        {
            return false;
        }
        
        if (alliance.MemberGuildIDs.Count >= guildWarData.MaxAllianceMembers)
        {
            Debug.LogError("联盟成员已满");
            return false;
        }
        
        if (alliance.MemberGuildIDs.Contains(guildID))
        {
            Debug.LogError("公会已在联盟中");
            return false;
        }
        
        alliance.MemberGuildIDs.Add(guildID);
        alliance.TotalMembers = alliance.MemberGuildIDs.Count;
        
        dataManager.CreateGuildWarEvent("alliance_join", guildID, "", "加入联盟: " + alliance.AllianceName);
        dataManager.SaveGuildWarData();
        Debug.Log("加入联盟成功: " + alliance.AllianceName);
        return true;
    }

    public bool LeaveAlliance(string allianceID, string guildID)
    {
        GuildAlliance alliance = guildWarData.Alliances.Find(a => a.AllianceID == allianceID);
        if (alliance == null)
        {
            return false;
        }
        
        if (alliance.LeaderGuildID == guildID)
        {
            Debug.LogError("盟主不能退出联盟");
            return false;
        }
        
        alliance.MemberGuildIDs.Remove(guildID);
        alliance.TotalMembers = alliance.MemberGuildIDs.Count;
        
        dataManager.CreateGuildWarEvent("alliance_leave", guildID, "", "离开联盟: " + alliance.AllianceName);
        dataManager.SaveGuildWarData();
        Debug.Log("离开联盟成功: " + alliance.AllianceName);
        return true;
    }

    public List<GuildWar> GetActiveWars()
    {
        return guildWarData.AllGuildWars.FindAll(w => w.WarStatus == "ongoing");
    }

    public List<GuildWar> GetGuildWars(string guildID)
    {
        if (guildWarData.GuildWarHistory.ContainsKey(guildID))
        {
            List<GuildWar> wars = new List<GuildWar>();
            foreach (string warID in guildWarData.GuildWarHistory[guildID])
            {
                GuildWar war = guildWarData.AllGuildWars.Find(w => w.WarID == warID);
                if (war != null)
                {
                    wars.Add(war);
                }
            }
            return wars;
        }
        return new List<GuildWar>();
    }

    public GuildWar GetWar(string warID)
    {
        return guildWarData.AllGuildWars.Find(w => w.WarID == warID);
    }

    public List<GuildWarBattle> GetWarBattles(string warID)
    {
        return guildWarData.AllWarBattles.FindAll(b => b.WarID == warID);
    }

    public GuildAlliance GetAlliance(string allianceID)
    {
        return guildWarData.Alliances.Find(a => a.AllianceID == allianceID);
    }

    public List<GuildAlliance> GetGuildAlliance(string guildID)
    {
        return guildWarData.Alliances.FindAll(a => a.MemberGuildIDs.Contains(guildID));
    }

    public void CleanupOldWars(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<GuildWar> oldWars = guildWarData.AllGuildWars.FindAll(w => w.WarEndTime < cutoffDate && w.WarStatus == "completed");
        foreach (GuildWar war in oldWars)
        {
            guildWarData.AllGuildWars.Remove(war);
        }
        
        if (oldWars.Count > 0)
        {
            dataManager.CreateGuildWarEvent("war_cleanup", "system", "", "清理旧公会战: " + oldWars.Count);
            dataManager.SaveGuildWarData();
            Debug.Log("清理旧公会战成功: " + oldWars.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveGuildWarData();
    }

    public void LoadData()
    {
        dataManager.LoadGuildWarData();
    }

    public List<GuildWarEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
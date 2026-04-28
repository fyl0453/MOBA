using System;
using System.Collections.Generic;

public class GuildSystemDetailedManager
{
    private static GuildSystemDetailedManager _instance;
    public static GuildSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GuildSystemDetailedManager();
            }
            return _instance;
        }
    }

    private GuildSystemData guildData;
    private GuildSystemDataManager dataManager;

    private GuildSystemDetailedManager()
    {
        dataManager = GuildSystemDataManager.Instance;
        guildData = dataManager.guildData;
    }

    public string CreateGuild(string guildName, string guildTag, string leaderID, string leaderName)
    {
        string guildID = "guild_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Guild guild = new Guild(guildID, guildName, guildTag, leaderID, leaderName);
        guildData.AddGuild(guild);
        
        string memberID = "member_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuildMember leaderMember = new GuildMember(memberID, leaderID, leaderName, guildID, 0);
        guild.Members.Add(leaderMember);
        guildData.AddGuildMember(leaderMember);
        
        dataManager.CreateGuildEvent("guild_create", leaderID, guildID, "创建公会: " + guildName);
        dataManager.SaveGuildData();
        Debug.Log("创建公会成功: " + guildName);
        return guildID;
    }

    public void JoinGuild(string playerID, string playerName, string guildID, int playerLevel, int playerRank)
    {
        Guild guild = GetGuild(guildID);
        if (guild == null)
        {
            Debug.LogError("公会不存在: " + guildID);
            return;
        }

        if (guild.Members.Count >= guild.MaxMembers)
        {
            Debug.LogError("公会人数已满");
            return;
        }

        if (guildData.PlayerGuilds.ContainsKey(playerID))
        {
            Debug.LogError("玩家已加入其他公会");
            return;
        }

        string requestID = "request_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuildRequest request = new GuildRequest(requestID, playerID, playerName, guildID, playerLevel, playerRank);
        guild.JoinRequests.Add(request);
        guildData.AddGuildRequest(request);
        
        dataManager.CreateGuildEvent("guild_join_request", playerID, guildID, "申请加入公会");
        dataManager.SaveGuildData();
        Debug.Log("申请加入公会成功");
    }

    public void AcceptJoinRequest(string requestID)
    {
        GuildRequest request = guildData.GuildRequests.Find(r => r.RequestID == requestID);
        if (request != null && request.Status == 0)
        {
            Guild guild = GetGuild(request.GuildID);
            if (guild != null && guild.Members.Count < guild.MaxMembers)
            {
                request.Status = 1;
                string memberID = "member_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                GuildMember member = new GuildMember(memberID, request.PlayerID, request.PlayerName, request.GuildID, 2);
                guild.Members.Add(member);
                guildData.AddGuildMember(member);
                
                dataManager.CreateGuildEvent("guild_join_accept", request.PlayerID, request.GuildID, "接受加入申请");
                dataManager.SaveGuildData();
                Debug.Log("接受加入申请成功");
            }
        }
    }

    public void RejectJoinRequest(string requestID)
    {
        GuildRequest request = guildData.GuildRequests.Find(r => r.RequestID == requestID);
        if (request != null && request.Status == 0)
        {
            request.Status = 2;
            dataManager.CreateGuildEvent("guild_join_reject", request.PlayerID, request.GuildID, "拒绝加入申请");
            dataManager.SaveGuildData();
            Debug.Log("拒绝加入申请成功");
        }
    }

    public void LeaveGuild(string playerID)
    {
        if (guildData.PlayerGuilds.ContainsKey(playerID))
        {
            string guildID = guildData.PlayerGuilds[playerID];
            Guild guild = GetGuild(guildID);
            if (guild != null)
            {
                GuildMember member = guild.Members.Find(m => m.PlayerID == playerID);
                if (member != null)
                {
                    if (member.Role == 0)
                    {
                        if (guild.Members.Count > 1)
                        {
                            GuildMember newLeader = guild.Members.Find(m => m.Role == 1);
                            if (newLeader == null)
                            {
                                newLeader = guild.Members[1];
                            }
                            newLeader.Role = 0;
                            guild.LeaderID = newLeader.PlayerID;
                            guild.LeaderName = newLeader.PlayerName;
                        }
                    }
                    guild.Members.Remove(member);
                    guildData.GuildMembers.Remove(member);
                    guildData.PlayerGuilds.Remove(playerID);
                    
                    dataManager.CreateGuildEvent("guild_leave", playerID, guildID, "离开公会");
                    dataManager.SaveGuildData();
                    Debug.Log("离开公会成功");
                }
            }
        }
    }

    public void KickMember(string guildID, string playerID)
    {
        Guild guild = GetGuild(guildID);
        if (guild != null)
        {
            GuildMember member = guild.Members.Find(m => m.PlayerID == playerID);
            if (member != null && member.Role != 0)
            {
                guild.Members.Remove(member);
                guildData.GuildMembers.Remove(member);
                guildData.PlayerGuilds.Remove(playerID);
                
                dataManager.CreateGuildEvent("guild_kick", playerID, guildID, "踢出公会");
                dataManager.SaveGuildData();
                Debug.Log("踢出公会成功");
            }
        }
    }

    public void PromoteMember(string guildID, string playerID, int newRole)
    {
        Guild guild = GetGuild(guildID);
        if (guild != null)
        {
            GuildMember member = guild.Members.Find(m => m.PlayerID == playerID);
            if (member != null)
            {
                member.Role = newRole;
                dataManager.SaveGuildData();
                Debug.Log("成员职位变更成功");
            }
        }
    }

    public void UpdateGuildInfo(string guildID, string guildName, string guildTag, string description, string announcement)
    {
        Guild guild = GetGuild(guildID);
        if (guild != null)
        {
            guild.GuildName = guildName;
            guild.GuildTag = guildTag;
            guild.Description = description;
            guild.Announcement = announcement;
            dataManager.SaveGuildData();
            Debug.Log("更新公会信息成功");
        }
    }

    public void ContributeToGuild(string playerID, int contribution)
    {
        if (guildData.PlayerGuilds.ContainsKey(playerID))
        {
            string guildID = guildData.PlayerGuilds[playerID];
            Guild guild = GetGuild(guildID);
            if (guild != null)
            {
                GuildMember member = guild.Members.Find(m => m.PlayerID == playerID);
                if (member != null)
                {
                    member.Contribution += contribution;
                    member.WeeklyContribution += contribution;
                    guild.Exp += contribution;
                    guild.TotalContribution += contribution;
                    CheckGuildLevelUp(guild);
                    dataManager.CreateGuildEvent("guild_contribute", playerID, guildID, "贡献: " + contribution);
                    dataManager.SaveGuildData();
                    Debug.Log("公会贡献成功");
                }
            }
        }
    }

    private void CheckGuildLevelUp(Guild guild)
    {
        int[] levelExp = { 0, 1000, 3000, 6000, 10000, 15000, 21000, 28000, 36000, 45000 };
        int currentLevel = guild.Level;
        if (currentLevel < levelExp.Length - 1 && guild.Exp >= levelExp[currentLevel])
        {
            guild.Level++;
            guild.MaxMembers += 5;
            dataManager.CreateGuildEvent("guild_level_up", "system", guild.GuildID, "公会升级到" + guild.Level + "级");
        }
    }

    public void CreateGuildActivity(string guildID, string activityName, string activityType, DateTime startTime, DateTime endTime, int requiredLevel)
    {
        string activityID = "activity_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuildActivity activity = new GuildActivity(activityID, guildID, activityName, activityType, startTime, endTime, requiredLevel);
        Guild guild = GetGuild(guildID);
        if (guild != null)
        {
            guild.Activities.Add(activity);
            guildData.AddGuildActivity(activity);
            dataManager.CreateGuildEvent("guild_activity_create", "system", guildID, "创建公会活动: " + activityName);
            dataManager.SaveGuildData();
            Debug.Log("创建公会活动成功: " + activityName);
        }
    }

    public void JoinGuildActivity(string playerID, string activityID)
    {
        GuildActivity activity = guildData.GuildActivities.Find(a => a.ActivityID == activityID);
        if (activity != null && activity.IsActive)
        {
            if (!activity.Participants.Contains(playerID))
            {
                activity.Participants.Add(playerID);
                dataManager.CreateGuildEvent("guild_activity_join", playerID, activity.GuildID, "参加公会活动");
                dataManager.SaveGuildData();
                Debug.Log("参加公会活动成功");
            }
        }
    }

    public void EndGuildActivity(string activityID)
    {
        GuildActivity activity = guildData.GuildActivities.Find(a => a.ActivityID == activityID);
        if (activity != null && activity.IsActive)
        {
            activity.IsActive = false;
            dataManager.CreateGuildEvent("guild_activity_end", "system", activity.GuildID, "结束公会活动");
            dataManager.SaveGuildData();
            Debug.Log("结束公会活动成功");
        }
    }

    public Guild GetGuild(string guildID)
    {
        return guildData.Guilds.Find(g => g.GuildID == guildID);
    }

    public Guild GetPlayerGuild(string playerID)
    {
        if (guildData.PlayerGuilds.ContainsKey(playerID))
        {
            string guildID = guildData.PlayerGuilds[playerID];
            return GetGuild(guildID);
        }
        return null;
    }

    public List<Guild> GetGuilds(int page = 1, int pageSize = 20)
    {
        List<Guild> guilds = new List<Guild>(guildData.Guilds);
        guilds.Sort((a, b) => b.Level.CompareTo(a.Level));
        int start = (page - 1) * pageSize;
        int end = Math.Min(start + pageSize, guilds.Count);
        if (start < guilds.Count)
        {
            return guilds.GetRange(start, end - start);
        }
        return new List<Guild>();
    }

    public List<GuildMember> GetGuildMembers(string guildID)
    {
        Guild guild = GetGuild(guildID);
        if (guild != null)
        {
            return guild.Members;
        }
        return new List<GuildMember>();
    }

    public List<GuildRequest> GetGuildJoinRequests(string guildID)
    {
        Guild guild = GetGuild(guildID);
        if (guild != null)
        {
            return guild.JoinRequests.FindAll(r => r.Status == 0);
        }
        return new List<GuildRequest>();
    }

    public List<GuildActivity> GetGuildActivities(string guildID)
    {
        Guild guild = GetGuild(guildID);
        if (guild != null)
        {
            return guild.Activities.FindAll(a => a.IsActive);
        }
        return new List<GuildActivity>();
    }

    public void UpdateMemberStatus(string playerID, bool isOnline, string lastOnlineTime)
    {
        if (guildData.PlayerGuilds.ContainsKey(playerID))
        {
            string guildID = guildData.PlayerGuilds[playerID];
            Guild guild = GetGuild(guildID);
            if (guild != null)
            {
                GuildMember member = guild.Members.Find(m => m.PlayerID == playerID);
                if (member != null)
                {
                    member.IsOnline = isOnline;
                    if (!isOnline)
                    {
                        member.LastOnlineTime = DateTime.Now;
                    }
                    dataManager.SaveGuildData();
                }
            }
        }
    }

    public void CleanupInactiveGuilds()
    {
        List<Guild> inactiveGuilds = new List<Guild>();
        foreach (Guild guild in guildData.Guilds)
        {
            if (guild.Members.Count == 0)
            {
                inactiveGuilds.Add(guild);
            }
        }
        
        foreach (Guild guild in inactiveGuilds)
        {
            guildData.Guilds.Remove(guild);
            foreach (GuildMember member in guild.Members)
            {
                guildData.GuildMembers.Remove(member);
                guildData.PlayerGuilds.Remove(member.PlayerID);
            }
        }
        
        if (inactiveGuilds.Count > 0)
        {
            dataManager.SaveGuildData();
            Debug.Log("清理无效公会成功: " + inactiveGuilds.Count);
        }
    }

    public void ResetWeeklyContribution()
    {
        foreach (GuildMember member in guildData.GuildMembers)
        {
            member.WeeklyContribution = 0;
        }
        dataManager.SaveGuildData();
        Debug.Log("重置周贡献成功");
    }

    public void SaveData()
    {
        dataManager.SaveGuildData();
    }

    public void LoadData()
    {
        dataManager.LoadGuildData();
    }

    public List<GuildEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
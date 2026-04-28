[System.Serializable]
public class Guild
{
    public string guildID;
    public string guildName;
    public string guildMasterID;
    public string guildMasterName;
    public int level;
    public int memberCount;
    public int maxMembers;
    public string guildDescription;
    public string creationDate;
    public int guildPoints;
    public List<GuildMember> members;
    public List<GuildRequest> joinRequests;
    
    public Guild(string id, string name, string masterID, string masterName)
    {
        guildID = id;
        guildName = name;
        guildMasterID = masterID;
        guildMasterName = masterName;
        level = 1;
        memberCount = 1;
        maxMembers = 30;
        guildDescription = "欢迎加入我们的公会！";
        creationDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        guildPoints = 0;
        members = new List<GuildMember>();
        joinRequests = new List<GuildRequest>();
        
        GuildMember master = new GuildMember(masterID, masterName, "GuildMaster");
        members.Add(master);
    }
    
    public void AddMember(GuildMember member)
    {
        if (memberCount < maxMembers)
        {
            members.Add(member);
            memberCount++;
        }
    }
    
    public void RemoveMember(string playerID)
    {
        members.RemoveAll(m => m.playerID == playerID);
        memberCount = members.Count;
    }
    
    public void PromoteMember(string playerID, string newRole)
    {
        GuildMember member = members.Find(m => m.playerID == playerID);
        if (member != null)
        {
            member.role = newRole;
        }
    }
    
    public void DemoteMember(string playerID, string newRole)
    {
        GuildMember member = members.Find(m => m.playerID == playerID);
        if (member != null)
        {
            member.role = newRole;
        }
    }
    
    public void AddGuildPoints(int points)
    {
        guildPoints += points;
        CheckLevelUp();
    }
    
    private void CheckLevelUp()
    {
        int pointsNeeded = GetPointsNeededForNextLevel();
        if (guildPoints >= pointsNeeded)
        {
            level++;
            maxMembers += 5;
            guildPoints -= pointsNeeded;
        }
    }
    
    private int GetPointsNeededForNextLevel()
    {
        return 1000 * level;
    }
}

[System.Serializable]
public class GuildMember
{
    public string playerID;
    public string playerName;
    public string role;
    public int contribution;
    public string joinDate;
    public string lastOnline;
    public bool isOnline;
    
    public GuildMember(string id, string name, string r = "Member")
    {
        playerID = id;
        playerName = name;
        role = r;
        contribution = 0;
        joinDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        lastOnline = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        isOnline = true;
    }
    
    public void AddContribution(int amount)
    {
        contribution += amount;
    }
}

[System.Serializable]
public class GuildRequest
{
    public string requestID;
    public string playerID;
    public string playerName;
    public string guildID;
    public string requestStatus;
    public string sendTime;
    
    public GuildRequest(string id, string player, string guild)
    {
        requestID = id;
        playerID = player;
        playerName = player;
        guildID = guild;
        requestStatus = "Pending";
        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}

[System.Serializable]
public class GuildActivity
{
    public string activityID;
    public string activityName;
    public string activityType;
    public string startTime;
    public string endTime;
    public int rewardPoints;
    public bool isActive;
    
    public GuildActivity(string id, string name, string type, int points)
    {
        activityID = id;
        activityName = name;
        activityType = type;
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        endTime = System.DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm");
        rewardPoints = points;
        isActive = true;
    }
}
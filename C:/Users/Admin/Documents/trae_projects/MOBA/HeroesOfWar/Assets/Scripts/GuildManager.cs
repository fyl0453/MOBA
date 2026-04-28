using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GuildManager : MonoBehaviour
{
    public static GuildManager Instance { get; private set; }
    
    public Guild currentGuild;
    public List<Guild> availableGuilds;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadGuildData();
        LoadAvailableGuilds();
        
        if (availableGuilds.Count == 0)
        {
            CreateDefaultGuilds();
        }
    }
    
    private void CreateDefaultGuilds()
    {
        Guild guild1 = new Guild("guild_001", "王者荣耀", "player_002", "关羽");
        guild1.AddMember(new GuildMember("player_003", "张飞"));
        guild1.AddMember(new GuildMember("player_004", "刘备"));
        guild1.guildPoints = 500;
        availableGuilds.Add(guild1);
        
        Guild guild2 = new Guild("guild_002", "英雄联盟", "player_005", "赵云");
        guild2.AddMember(new GuildMember("player_006", "诸葛亮"));
        guild2.guildPoints = 300;
        availableGuilds.Add(guild2);
        
        Guild guild3 = new Guild("guild_003", "和平精英", "player_007", "孙尚香");
        guild3.AddMember(new GuildMember("player_008", "貂蝉"));
        guild3.AddMember(new GuildMember("player_009", "吕布"));
        guild3.AddMember(new GuildMember("player_010", "周瑜"));
        guild3.guildPoints = 800;
        availableGuilds.Add(guild3);
        
        SaveAvailableGuilds();
    }
    
    public void CreateGuild(string guildName)
    {
        string guildID = "guild_" + System.DateTime.Now.Ticks;
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        string playerName = ProfileManager.Instance.currentProfile.playerName;
        
        Guild newGuild = new Guild(guildID, guildName, playerID, playerName);
        availableGuilds.Add(newGuild);
        currentGuild = newGuild;
        
        SaveAvailableGuilds();
        SaveGuildData();
    }
    
    public void JoinGuild(string guildID)
    {
        Guild guild = availableGuilds.Find(g => g.guildID == guildID);
        if (guild != null && guild.memberCount < guild.maxMembers)
        {
            GuildMember newMember = new GuildMember(ProfileManager.Instance.currentProfile.playerID, ProfileManager.Instance.currentProfile.playerName);
            guild.AddMember(newMember);
            currentGuild = guild;
            
            SaveAvailableGuilds();
            SaveGuildData();
        }
    }
    
    public void LeaveGuild()
    {
        if (currentGuild != null)
        {
            if (currentGuild.guildMasterID == ProfileManager.Instance.currentProfile.playerID)
            {
                // 公会会长离开，需要转让会长或解散公会
                if (currentGuild.memberCount > 1)
                {
                    // 转让会长给第一个成员
                    GuildMember newMaster = currentGuild.members.Find(m => m.playerID != currentGuild.guildMasterID);
                    if (newMaster != null)
                    {
                        currentGuild.guildMasterID = newMaster.playerID;
                        currentGuild.guildMasterName = newMaster.playerName;
                        currentGuild.PromoteMember(newMaster.playerID, "GuildMaster");
                    }
                }
                else
                {
                    // 解散公会
                    availableGuilds.Remove(currentGuild);
                }
            }
            else
            {
                currentGuild.RemoveMember(ProfileManager.Instance.currentProfile.playerID);
            }
            
            currentGuild = null;
            SaveAvailableGuilds();
            SaveGuildData();
        }
    }
    
    public void SendJoinRequest(string guildID)
    {
        Guild guild = availableGuilds.Find(g => g.guildID == guildID);
        if (guild != null)
        {
            GuildRequest request = new GuildRequest("request_" + System.DateTime.Now.Ticks, ProfileManager.Instance.currentProfile.playerID, guildID);
            request.playerName = ProfileManager.Instance.currentProfile.playerName;
            guild.joinRequests.Add(request);
            
            SaveAvailableGuilds();
        }
    }
    
    public void AcceptJoinRequest(string guildID, string requestID)
    {
        Guild guild = availableGuilds.Find(g => g.guildID == guildID);
        if (guild != null && guild.guildMasterID == ProfileManager.Instance.currentProfile.playerID)
        {
            GuildRequest request = guild.joinRequests.Find(r => r.requestID == requestID);
            if (request != null)
            {
                GuildMember newMember = new GuildMember(request.playerID, request.playerName);
                guild.AddMember(newMember);
                guild.joinRequests.Remove(request);
                
                SaveAvailableGuilds();
            }
        }
    }
    
    public void DeclineJoinRequest(string guildID, string requestID)
    {
        Guild guild = availableGuilds.Find(g => g.guildID == guildID);
        if (guild != null && guild.guildMasterID == ProfileManager.Instance.currentProfile.playerID)
        {
            guild.joinRequests.RemoveAll(r => r.requestID == requestID);
            SaveAvailableGuilds();
        }
    }
    
    public void PromoteMember(string playerID, string newRole)
    {
        if (currentGuild != null && currentGuild.guildMasterID == ProfileManager.Instance.currentProfile.playerID)
        {
            currentGuild.PromoteMember(playerID, newRole);
            SaveAvailableGuilds();
        }
    }
    
    public void DemoteMember(string playerID, string newRole)
    {
        if (currentGuild != null && currentGuild.guildMasterID == ProfileManager.Instance.currentProfile.playerID)
        {
            currentGuild.DemoteMember(playerID, newRole);
            SaveAvailableGuilds();
        }
    }
    
    public void KickMember(string playerID)
    {
        if (currentGuild != null && currentGuild.guildMasterID == ProfileManager.Instance.currentProfile.playerID)
        {
            currentGuild.RemoveMember(playerID);
            SaveAvailableGuilds();
        }
    }
    
    public void UpdateGuildDescription(string description)
    {
        if (currentGuild != null && currentGuild.guildMasterID == ProfileManager.Instance.currentProfile.playerID)
        {
            currentGuild.guildDescription = description;
            SaveAvailableGuilds();
        }
    }
    
    public void AddGuildPoints(int points)
    {
        if (currentGuild != null)
        {
            currentGuild.AddGuildPoints(points);
            SaveAvailableGuilds();
        }
    }
    
    public List<Guild> GetAvailableGuilds()
    {
        return availableGuilds;
    }
    
    public Guild GetCurrentGuild()
    {
        return currentGuild;
    }
    
    public bool IsInGuild()
    {
        return currentGuild != null;
    }
    
    public void SaveGuildData()
    {
        string path = Application.dataPath + "/Data/guild_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, currentGuild);
        stream.Close();
    }
    
    public void LoadGuildData()
    {
        string path = Application.dataPath + "/Data/guild_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            currentGuild = (Guild)formatter.Deserialize(stream);
            stream.Close();
        }
    }
    
    public void SaveAvailableGuilds()
    {
        string path = Application.dataPath + "/Data/available_guilds.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, availableGuilds);
        stream.Close();
    }
    
    public void LoadAvailableGuilds()
    {
        string path = Application.dataPath + "/Data/available_guilds.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            availableGuilds = (List<Guild>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            availableGuilds = new List<Guild>();
        }
    }
}
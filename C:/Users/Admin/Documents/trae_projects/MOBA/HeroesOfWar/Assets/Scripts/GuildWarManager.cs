using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GuildWarManager : MonoBehaviour
{
    public static GuildWarManager Instance { get; private set; }
    
    public GuildWarManagerData guildWarData;
    
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
        LoadGuildWarData();
        
        if (guildWarData == null)
        {
            guildWarData = new GuildWarManagerData();
            InitializeDefaultGuildWarSeason();
        }
    }
    
    private void InitializeDefaultGuildWarSeason()
    {
        GuildWarSeason season = new GuildWarSeason("season_1", "第一赛季");
        guildWarData.AddSeason(season);
        SaveGuildWarData();
    }
    
    public GuildWar CreateGuildWar(string guild1ID, string guild1Name, string guild2ID, string guild2Name)
    {
        string warID = "war_" + System.DateTime.Now.Ticks;
        GuildWar war = new GuildWar(warID, guild1ID, guild1Name, guild2ID, guild2Name);
        guildWarData.AddWar(war);
        
        GuildWarSeason currentSeason = guildWarData.GetCurrentSeason();
        if (currentSeason != null)
        {
            currentSeason.AddWar(war);
        }
        
        SaveGuildWarData();
        return war;
    }
    
    public void StartGuildWar(string warID)
    {
        GuildWar war = guildWarData.GetWar(warID);
        if (war != null && war.warStatus == "Pending")
        {
            war.StartWar();
            SaveGuildWarData();
        }
    }
    
    public void EndGuildWar(string warID)
    {
        GuildWar war = guildWarData.GetWar(warID);
        if (war != null && war.warStatus == "Active")
        {
            war.EndWar();
            
            GuildWarSeason currentSeason = guildWarData.GetCurrentSeason();
            if (currentSeason != null)
            {
                currentSeason.UpdateRanks();
            }
            
            SaveGuildWarData();
        }
    }
    
    public void AddWarScore(string warID, string guildID, int score, string playerID)
    {
        GuildWar war = guildWarData.GetWar(warID);
        if (war != null && war.warStatus == "Active")
        {
            war.AddScore(guildID, score);
            war.AddWarEvent("Score", guildID, playerID, score);
            SaveGuildWarData();
        }
    }
    
    public List<GuildWar> GetActiveWars()
    {
        return guildWarData.activeWars.FindAll(w => w.warStatus == "Active");
    }
    
    public List<GuildWar> GetPendingWars()
    {
        return guildWarData.activeWars.FindAll(w => w.warStatus == "Pending");
    }
    
    public List<GuildWar> GetEndedWars()
    {
        return guildWarData.activeWars.FindAll(w => w.warStatus == "Ended");
    }
    
    public GuildWar GetWar(string warID)
    {
        return guildWarData.GetWar(warID);
    }
    
    public GuildWarSeason GetCurrentSeason()
    {
        return guildWarData.GetCurrentSeason();
    }
    
    public void CreateNewSeason(string seasonName)
    {
        string seasonID = "season_" + (guildWarData.seasons.Count + 1);
        GuildWarSeason season = new GuildWarSeason(seasonID, seasonName);
        guildWarData.AddSeason(season);
        SaveGuildWarData();
    }
    
    public void SaveGuildWarData()
    {
        string path = Application.dataPath + "/Data/guild_war_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, guildWarData);
        stream.Close();
    }
    
    public void LoadGuildWarData()
    {
        string path = Application.dataPath + "/Data/guild_war_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            guildWarData = (GuildWarManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            guildWarData = new GuildWarManagerData();
        }
    }
}
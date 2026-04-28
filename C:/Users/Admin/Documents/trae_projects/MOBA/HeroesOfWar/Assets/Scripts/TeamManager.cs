using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance { get; private set; }
    
    public TeamManagerData teamData;
    
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
        LoadTeamData();
        
        if (teamData == null)
        {
            teamData = new TeamManagerData();
        }
    }
    
    public Team CreateTeam(string teamName, string teamLogo)
    {
        string teamID = "team_" + System.DateTime.Now.Ticks;
        string captainID = ProfileManager.Instance.currentProfile.playerID;
        string captainName = ProfileManager.Instance.currentProfile.playerName;
        
        Team team = new Team(teamID, teamName, captainID, captainName, teamLogo);
        teamData.AddTeam(team);
        SaveTeamData();
        
        return team;
    }
    
    public bool JoinTeam(string teamID)
    {
        Team team = teamData.GetTeam(teamID);
        if (team != null && !team.IsFull())
        {
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            string playerName = ProfileManager.Instance.currentProfile.playerName;
            team.AddMember(playerID, playerName);
            SaveTeamData();
            return true;
        }
        return false;
    }
    
    public void LeaveTeam(string teamID)
    {
        Team team = teamData.GetTeam(teamID);
        if (team != null)
        {
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            team.RemoveMember(playerID);
            
            if (team.memberCount == 0)
            {
                teamData.teams.Remove(team);
            }
            
            SaveTeamData();
        }
    }
    
    public void PromoteMember(string teamID, string playerID, string role)
    {
        Team team = teamData.GetTeam(teamID);
        if (team != null && team.captainID == ProfileManager.Instance.currentProfile.playerID)
        {
            team.PromoteMember(playerID, role);
            SaveTeamData();
        }
    }
    
    public void AddTeamPoints(string teamID, int points)
    {
        Team team = teamData.GetTeam(teamID);
        if (team != null)
        {
            team.AddTeamPoints(points);
            SaveTeamData();
        }
    }
    
    public void AddTeamMatch(string teamID, string opponentTeamID, string opponentTeamName, string result, int score, int opponentScore)
    {
        Team team = teamData.GetTeam(teamID);
        if (team != null)
        {
            string matchID = "match_" + System.DateTime.Now.Ticks;
            TeamMatch match = new TeamMatch(matchID, opponentTeamID, opponentTeamName, result, score, opponentScore);
            team.AddMatch(match);
            
            if (result == "Win")
            {
                team.AddTeamPoints(10);
            }
            else if (result == "Loss")
            {
                team.AddTeamPoints(2);
            }
            
            SaveTeamData();
        }
    }
    
    public Team GetTeam(string teamID)
    {
        return teamData.GetTeam(teamID);
    }
    
    public Team GetPlayerTeam()
    {
        return teamData.GetPlayerTeam(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public List<Team> GetTeamsByPlayer()
    {
        return teamData.GetTeamsByPlayer(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public void SaveTeamData()
    {
        string path = Application.dataPath + "/Data/team_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, teamData);
        stream.Close();
    }
    
    public void LoadTeamData()
    {
        string path = Application.dataPath + "/Data/team_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            teamData = (TeamManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            teamData = new TeamManagerData();
        }
    }
}
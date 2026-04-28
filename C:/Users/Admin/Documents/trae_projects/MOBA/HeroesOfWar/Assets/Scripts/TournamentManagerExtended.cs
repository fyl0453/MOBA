using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TournamentManagerExtended : MonoBehaviour
{
    public static TournamentManagerExtended Instance { get; private set; }
    
    public TournamentManagerData tournamentData;
    
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
        LoadTournamentData();
        
        if (tournamentData == null)
        {
            tournamentData = new TournamentManagerData();
            InitializeDefaultTournaments();
        }
    }
    
    private void InitializeDefaultTournaments()
    {
        // 创建默认KPL职业联赛
        Tournament kpl = new Tournament(
            "kpl_2024",
            "2024 KPL职业联赛",
            "KPL",
            "王者荣耀职业联赛，最高水平的电竞比赛",
            "2024-01-01",
            "2024-12-31",
            16,
            0
        );
        
        // 创建默认城市赛
        Tournament city = new Tournament(
            "city_2024",
            "2024城市赛",
            "City",
            "面向普通玩家的城市级比赛",
            "2024-03-01",
            "2024-08-31",
            64,
            50
        );
        
        // 创建默认高校赛
        Tournament college = new Tournament(
            "college_2024",
            "2024高校赛",
            "College",
            "面向高校学生的比赛",
            "2024-09-01",
            "2024-11-30",
            32,
            20
        );
        
        tournamentData.system.AddTournament(kpl);
        tournamentData.system.AddTournament(city);
        tournamentData.system.AddTournament(college);
        
        // 创建默认队伍
        Team team1 = new Team(
            "team1",
            "AG超玩会",
            "player1",
            "ag_logo.png",
            "成都"
        );
        
        Team team2 = new Team(
            "team2",
            "QGhappy",
            "player2",
            "qg_logo.png",
            "重庆"
        );
        
        tournamentData.system.AddTeam(team1);
        tournamentData.system.AddTeam(team2);
        
        // 创建默认玩家
        Player player1 = new Player(
            "player1",
            "一诺",
            "team1",
            "射手",
            "player1_avatar.png",
            "中国",
            20
        );
        
        Player player2 = new Player(
            "player2",
            "Fly",
            "team2",
            "对抗路",
            "player2_avatar.png",
            "中国",
            22
        );
        
        tournamentData.system.AddPlayer(player1);
        tournamentData.system.AddPlayer(player2);
        
        // 创建默认比赛
        Match match1 = new Match(
            "match1",
            "kpl_2024",
            "team1",
            "team2",
            "AG超玩会",
            "QGhappy",
            "2024-01-15 19:00",
            "BO5",
            "常规赛"
        );
        
        tournamentData.system.AddMatch(match1);
        
        // 创建默认奖励
        Prize prize1 = new Prize(
            "prize1",
            "kpl_2024",
            "冠军奖金",
            1,
            "Cash",
            1000000,
            "KPL冠军奖金"
        );
        
        Prize prize2 = new Prize(
            "prize2",
            "kpl_2024",
            "亚军奖金",
            2,
            "Cash",
            500000,
            "KPL亚军奖金"
        );
        
        tournamentData.system.AddPrize(prize1);
        tournamentData.system.AddPrize(prize2);
        
        SaveTournamentData();
    }
    
    public string CreateTournament(string name, string type, string description, string startDate, string endDate, int maxTeams, int entryFee)
    {
        string tournamentID = System.Guid.NewGuid().ToString();
        Tournament newTournament = new Tournament(tournamentID, name, type, description, startDate, endDate, maxTeams, entryFee);
        tournamentData.system.AddTournament(newTournament);
        SaveTournamentData();
        Debug.Log($"成功创建赛事: {name}");
        return tournamentID;
    }
    
    public string CreateTeam(string name, string captainID, string logoURL, string region)
    {
        string teamID = System.Guid.NewGuid().ToString();
        Team newTeam = new Team(teamID, name, captainID, logoURL, region);
        tournamentData.system.AddTeam(newTeam);
        SaveTournamentData();
        Debug.Log($"成功创建队伍: {name}");
        return teamID;
    }
    
    public string CreatePlayer(string name, string teamID, string role, string avatarURL, string country, int age)
    {
        string playerID = System.Guid.NewGuid().ToString();
        Player newPlayer = new Player(playerID, name, teamID, role, avatarURL, country, age);
        tournamentData.system.AddPlayer(newPlayer);
        
        // 更新队伍的玩家列表
        Team team = tournamentData.system.GetTeam(teamID);
        if (team != null)
        {
            team.AddPlayer(playerID);
        }
        
        SaveTournamentData();
        Debug.Log($"成功创建玩家: {name}");
        return playerID;
    }
    
    public string CreateMatch(string tournamentID, string team1ID, string team2ID, string team1Name, string team2Name, string matchTime, string matchType, string matchStage)
    {
        string matchID = System.Guid.NewGuid().ToString();
        Match newMatch = new Match(matchID, tournamentID, team1ID, team2ID, team1Name, team2Name, matchTime, matchType, matchStage);
        tournamentData.system.AddMatch(newMatch);
        
        // 更新赛事的比赛列表
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            tournament.AddMatch(matchID);
        }
        
        SaveTournamentData();
        Debug.Log($"成功创建比赛: {team1Name} vs {team2Name}");
        return matchID;
    }
    
    public string CreatePrize(string tournamentID, string name, int rank, string type, int amount, string description)
    {
        string prizeID = System.Guid.NewGuid().ToString();
        Prize newPrize = new Prize(prizeID, tournamentID, name, rank, type, amount, description);
        tournamentData.system.AddPrize(newPrize);
        
        // 更新赛事的奖励列表
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            tournament.AddPrize(prizeID);
        }
        
        SaveTournamentData();
        Debug.Log($"成功创建奖励: {name}");
        return prizeID;
    }
    
    public void RegisterTeamToTournament(string teamID, string tournamentID)
    {
        Team team = tournamentData.system.GetTeam(teamID);
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        
        if (team != null && tournament != null)
        {
            if (!team.tournamentIDs.Contains(tournamentID))
            {
                team.AddTournament(tournamentID);
            }
            
            if (!tournament.teamIDs.Contains(teamID))
            {
                tournament.AddTeam(teamID);
            }
            
            SaveTournamentData();
            Debug.Log($"成功注册队伍到赛事: {team.teamName} -> {tournament.tournamentName}");
        }
    }
    
    public void StartMatch(string matchID)
    {
        Match match = tournamentData.system.GetMatch(matchID);
        if (match != null)
        {
            match.StartMatch();
            SaveTournamentData();
            Debug.Log("比赛开始");
        }
    }
    
    public void EndMatch(string matchID, int score1, int score2, string winnerID)
    {
        Match match = tournamentData.system.GetMatch(matchID);
        if (match != null)
        {
            match.EndMatch(score1, score2, winnerID);
            
            // 更新队伍统计
            Team winner = tournamentData.system.GetTeam(winnerID);
            Team loser = match.team1ID == winnerID ? tournamentData.system.GetTeam(match.team2ID) : tournamentData.system.GetTeam(match.team1ID);
            
            if (winner != null)
            {
                winner.UpdateStats(1, 0);
            }
            
            if (loser != null)
            {
                loser.UpdateStats(0, 1);
            }
            
            SaveTournamentData();
            Debug.Log($"比赛结束，获胜方: {winner?.teamName}");
        }
    }
    
    public void StartTournament(string tournamentID)
    {
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            tournament.Start();
            SaveTournamentData();
            Debug.Log($"赛事开始: {tournament.tournamentName}");
        }
    }
    
    public void EndTournament(string tournamentID)
    {
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            tournament.End();
            SaveTournamentData();
            Debug.Log($"赛事结束: {tournament.tournamentName}");
        }
    }
    
    public Tournament GetTournament(string tournamentID)
    {
        return tournamentData.system.GetTournament(tournamentID);
    }
    
    public Team GetTeam(string teamID)
    {
        return tournamentData.system.GetTeam(teamID);
    }
    
    public Match GetMatch(string matchID)
    {
        return tournamentData.system.GetMatch(matchID);
    }
    
    public Player GetPlayer(string playerID)
    {
        return tournamentData.system.GetPlayer(playerID);
    }
    
    public List<Tournament> GetTournamentsByType(string type)
    {
        return tournamentData.system.GetTournamentsByType(type);
    }
    
    public List<Match> GetMatchesByTournament(string tournamentID)
    {
        return tournamentData.system.GetMatchesByTournament(tournamentID);
    }
    
    public List<Team> GetTeamsByTournament(string tournamentID)
    {
        return tournamentData.system.GetTeamsByTournament(tournamentID);
    }
    
    public void SaveTournamentData()
    {
        string path = Application.dataPath + "/Data/tournament_system_extended_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, tournamentData);
        stream.Close();
    }
    
    public void LoadTournamentData()
    {
        string path = Application.dataPath + "/Data/tournament_system_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            tournamentData = (TournamentManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            tournamentData = new TournamentManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TournamentManagerDetailed : MonoBehaviour
{
    public static TournamentManagerDetailed Instance { get; private set; }
    
    public TournamentSystemDetailedData tournamentData;
    
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
            tournamentData = new TournamentSystemDetailedData();
            InitializeDefaultTournaments();
        }
        
        // 检查赛事状态
        CheckTournamentStatus();
    }
    
    private void CheckTournamentStatus()
    {
        foreach (Tournament tournament in tournamentData.system.tournaments)
        {
            string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (currentTime >= tournament.startTime && currentTime <= tournament.endTime)
            {
                tournament.Activate();
            }
            else
            {
                tournament.Deactivate();
            }
        }
        SaveTournamentData();
    }
    
    private void InitializeDefaultTournaments()
    {
        // 创建赛事分类
        TournamentCategory职业联赛 = new TournamentCategory("category_pro", "职业联赛", "KPL职业联赛");
        TournamentCategory城市赛 = new TournamentCategory("category_city", "城市赛", "城市王者荣耀争霸赛");
        TournamentCategory高校赛 = new TournamentCategory("category_college", "高校赛", "高校王者荣耀联赛");
        TournamentCategory友谊赛 = new TournamentCategory("category_friendly", "友谊赛", "玩家自发组织的友谊赛");
        
        tournamentData.system.AddCategory(职业联赛);
        tournamentData.system.AddCategory(城市赛);
        tournamentData.system.AddCategory(高校赛);
        tournamentData.system.AddCategory(友谊赛);
        
        // 创建赛事奖励
        TournamentReward reward1 = new TournamentReward("reward_champion_gold", "冠军奖金", "100000金币", "Gold", 100000);
        TournamentReward reward2 = new TournamentReward("reward_champion_gems", "冠军钻石", "10000钻石", "Gems", 10000);
        TournamentReward reward3 = new TournamentReward("reward_champion_skin", "冠军皮肤", "限定冠军皮肤", "Skin", 1, "skin_champion");
        TournamentReward reward4 = new TournamentReward("reward_runnerup_gold", "亚军奖金", "50000金币", "Gold", 50000);
        
        tournamentData.AddReward(reward1);
        tournamentData.AddReward(reward2);
        tournamentData.AddReward(reward3);
        tournamentData.AddReward(reward4);
        
        // 创建参赛队伍
        TournamentTeam team1 = new TournamentTeam("team_1", "AG超玩会", "player_1", "选手1");
        team1.AddPlayer("player_1", "选手1");
        team1.AddPlayer("player_2", "选手2");
        team1.AddPlayer("player_3", "选手3");
        team1.AddPlayer("player_4", "选手4");
        team1.AddPlayer("player_5", "选手5");
        
        TournamentTeam team2 = new TournamentTeam("team_2", "QGhappy", "player_6", "选手6");
        team2.AddPlayer("player_6", "选手6");
        team2.AddPlayer("player_7", "选手7");
        team2.AddPlayer("player_8", "选手8");
        team2.AddPlayer("player_9", "选手9");
        team2.AddPlayer("player_10", "选手10");
        
        TournamentTeam team3 = new TournamentTeam("team_3", "WE", "player_11", "选手11");
        team3.AddPlayer("player_11", "选手11");
        team3.AddPlayer("player_12", "选手12");
        team3.AddPlayer("player_13", "选手13");
        team3.AddPlayer("player_14", "选手14");
        team3.AddPlayer("player_15", "选手15");
        
        tournamentData.system.AddTeam(team1);
        tournamentData.system.AddTeam(team2);
        tournamentData.system.AddTeam(team3);
        
        // 创建KPL职业联赛
        Tournament kpl = new Tournament("tournament_kpl", "KPL职业联赛", "王者荣耀职业联赛", "category_pro", "Professional", "2024-01-01 00:00:00", "2024-12-31 23:59:59");
        kpl.AddTeam("team_1");
        kpl.AddTeam("team_2");
        kpl.AddTeam("team_3");
        kpl.AddReward("reward_champion_gold");
        kpl.AddReward("reward_champion_gems");
        kpl.AddReward("reward_champion_skin");
        kpl.AddReward("reward_runnerup_gold");
        kpl.Activate();
        
        // 创建城市赛
        Tournament city = new Tournament("tournament_city", "城市赛", "城市王者荣耀争霸赛", "category_city", "Amateur", "2024-01-01 00:00:00", "2024-12-31 23:59:59");
        city.AddTeam("team_1");
        city.AddTeam("team_2");
        city.Activate();
        
        tournamentData.system.AddTournament(kpl);
        tournamentData.system.AddTournament(city);
        
        // 创建赛事匹配
        TournamentMatch match1 = new TournamentMatch("match_1", "tournament_kpl", "KPL Round 1", "team_1", "team_2", "AG超玩会", "QGhappy", "2024-01-01 19:00:00");
        TournamentMatch match2 = new TournamentMatch("match_2", "tournament_kpl", "KPL Round 2", "team_2", "team_3", "QGhappy", "WE", "2024-01-02 19:00:00");
        TournamentMatch match3 = new TournamentMatch("match_3", "tournament_kpl", "KPL Round 3", "team_3", "team_1", "WE", "AG超玩会", "2024-01-03 19:00:00");
        
        kpl.AddMatch("match_1");
        kpl.AddMatch("match_2");
        kpl.AddMatch("match_3");
        
        tournamentData.system.AddMatch(match1);
        tournamentData.system.AddMatch(match2);
        tournamentData.system.AddMatch(match3);
        
        SaveTournamentData();
    }
    
    public void CreateTournament(string name, string description, string category, string type, string start, string end, List<string> teamIDs, List<string> rewardIDs)
    {
        string tournamentID = System.Guid.NewGuid().ToString();
        Tournament newTournament = new Tournament(tournamentID, name, description, category, type, start, end);
        
        foreach (string teamID in teamIDs)
        {
            newTournament.AddTeam(teamID);
        }
        
        foreach (string rewardID in rewardIDs)
        {
            newTournament.AddReward(rewardID);
        }
        
        tournamentData.system.AddTournament(newTournament);
        SaveTournamentData();
        Debug.Log($"成功创建赛事: {name}");
    }
    
    public void CreateTeam(string name, string captainID, string captainName, List<string> playerIDs, List<string> playerNames)
    {
        string teamID = System.Guid.NewGuid().ToString();
        TournamentTeam newTeam = new TournamentTeam(teamID, name, captainID, captainName);
        
        for (int i = 0; i < playerIDs.Count; i++)
        {
            newTeam.AddPlayer(playerIDs[i], playerNames[i]);
        }
        
        tournamentData.system.AddTeam(newTeam);
        SaveTournamentData();
        Debug.Log($"成功创建队伍: {name}");
    }
    
    public void CreateMatch(string tournamentID, string name, string team1ID, string team2ID, string start)
    {
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            TournamentTeam team1 = tournamentData.system.GetTeam(team1ID);
            TournamentTeam team2 = tournamentData.system.GetTeam(team2ID);
            
            if (team1 != null && team2 != null)
            {
                string matchID = System.Guid.NewGuid().ToString();
                TournamentMatch newMatch = new TournamentMatch(matchID, tournamentID, name, team1ID, team2ID, team1.teamName, team2.teamName, start);
                tournamentData.system.AddMatch(newMatch);
                tournament.AddMatch(matchID);
                SaveTournamentData();
                Debug.Log($"成功创建比赛: {name}");
            }
        }
    }
    
    public void StartMatch(string matchID)
    {
        TournamentMatch match = tournamentData.system.GetMatch(matchID);
        if (match != null)
        {
            match.StartMatch();
            SaveTournamentData();
            Debug.Log($"成功开始比赛: {match.matchName}");
        }
    }
    
    public void EndMatch(string matchID, int score1, int score2, string winnerID)
    {
        TournamentMatch match = tournamentData.system.GetMatch(matchID);
        if (match != null)
        {
            match.EndMatch(score1, score2, winnerID);
            SaveTournamentData();
            Debug.Log($"成功结束比赛: {match.matchName}, 获胜者: {winnerID}");
        }
    }
    
    public void SetMatchVOD(string matchID, string vod)
    {
        TournamentMatch match = tournamentData.system.GetMatch(matchID);
        if (match != null)
        {
            match.SetVOD(vod);
            SaveTournamentData();
            Debug.Log($"成功设置比赛回放: {match.matchName}");
        }
    }
    
    public List<Tournament> GetActiveTournaments()
    {
        return tournamentData.system.GetActiveTournaments();
    }
    
    public List<Tournament> GetTournamentsByCategory(string categoryID)
    {
        return tournamentData.system.GetTournamentsByCategory(categoryID);
    }
    
    public List<TournamentTeam> GetTournamentTeams(string tournamentID)
    {
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            List<TournamentTeam> teams = new List<TournamentTeam>();
            foreach (string teamID in tournament.teamIDs)
            {
                TournamentTeam team = tournamentData.system.GetTeam(teamID);
                if (team != null)
                {
                    teams.Add(team);
                }
            }
            return teams;
        }
        return new List<TournamentTeam>();
    }
    
    public List<TournamentMatch> GetTournamentMatches(string tournamentID)
    {
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            List<TournamentMatch> matches = new List<TournamentMatch>();
            foreach (string matchID in tournament.matchIDs)
            {
                TournamentMatch match = tournamentData.system.GetMatch(matchID);
                if (match != null)
                {
                    matches.Add(match);
                }
            }
            return matches;
        }
        return new List<TournamentMatch>();
    }
    
    public Tournament GetTournament(string tournamentID)
    {
        return tournamentData.system.GetTournament(tournamentID);
    }
    
    public TournamentTeam GetTeam(string teamID)
    {
        return tournamentData.system.GetTeam(teamID);
    }
    
    public TournamentMatch GetMatch(string matchID)
    {
        return tournamentData.system.GetMatch(matchID);
    }
    
    public void ActivateTournament(string tournamentID)
    {
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            tournament.Activate();
            SaveTournamentData();
            Debug.Log($"成功激活赛事: {tournament.tournamentName}");
        }
    }
    
    public void DeactivateTournament(string tournamentID)
    {
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            tournament.Deactivate();
            SaveTournamentData();
            Debug.Log($"成功停用赛事: {tournament.tournamentName}");
        }
    }
    
    public void SaveTournamentData()
    {
        string path = Application.dataPath + "/Data/tournament_system_detailed_data.dat";
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
        string path = Application.dataPath + "/Data/tournament_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            tournamentData = (TournamentSystemDetailedData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            tournamentData = new TournamentSystemDetailedData();
        }
    }
}
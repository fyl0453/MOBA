using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TournamentManager : MonoBehaviour
{
    public static TournamentManager Instance { get; private set; }
    
    public TournamentManagerData tournamentData;
    public List<Stream> liveStreams;
    
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
            liveStreams = new List<Stream>();
            InitializeDefaultData();
        }
    }
    
    private void InitializeDefaultData()
    {
        // 创建默认队伍
        Team team1 = new Team("team_1", "Team Alpha", "team_alpha_logo", "TMA");
        Team team2 = new Team("team_2", "Team Beta", "team_beta_logo", "TMB");
        Team team3 = new Team("team_3", "Team Gamma", "team_gamma_logo", "TMG");
        Team team4 = new Team("team_4", "Team Delta", "team_delta_logo", "TMD");
        
        tournamentData.system.AddTeam(team1);
        tournamentData.system.AddTeam(team2);
        tournamentData.system.AddTeam(team3);
        tournamentData.system.AddTeam(team4);
        
        // 创建默认玩家
        Player player1 = new Player("player_1", "Player1", "Top", "team_1");
        Player player2 = new Player("player_2", "Player2", "Jungle", "team_1");
        Player player3 = new Player("player_3", "Player3", "Mid", "team_1");
        Player player4 = new Player("player_4", "Player4", "ADC", "team_1");
        Player player5 = new Player("player_5", "Player5", "Support", "team_1");
        
        team1.AddPlayer(player1);
        team1.AddPlayer(player2);
        team1.AddPlayer(player3);
        team1.AddPlayer(player4);
        team1.AddPlayer(player5);
        
        // 创建默认赛事
        string startDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string endDate = System.DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss");
        Tournament tournament = new Tournament("tournament_1", "春季赛", "2024年春季赛", "League", startDate, endDate, 4, 100000);
        
        // 创建赛事匹配
        Match match1 = new Match("match_1", "半决赛1", "tournament_1", "team_1", "team_2", System.DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
        Match match2 = new Match("match_2", "半决赛2", "tournament_1", "team_3", "team_4", System.DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"));
        Match match3 = new Match("match_3", "决赛", "tournament_1", "", "", System.DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"));
        
        tournament.AddMatch(match1);
        tournament.AddMatch(match2);
        tournament.AddMatch(match3);
        
        tournament.AddTeam(team1);
        tournament.AddTeam(team2);
        tournament.AddTeam(team3);
        tournament.AddTeam(team4);
        
        tournamentData.system.AddTournament(tournament);
        tournamentData.system.AddMatch(match1);
        tournamentData.system.AddMatch(match2);
        tournamentData.system.AddMatch(match3);
        
        // 创建默认直播
        Stream stream1 = new Stream("stream_1", "春季赛半决赛1", "官方直播", "https://twitch.tv/example");
        Stream stream2 = new Stream("stream_2", "春季赛半决赛2", "官方直播", "https://twitch.tv/example2");
        
        liveStreams.Add(stream1);
        liveStreams.Add(stream2);
        
        SaveTournamentData();
    }
    
    public void CreateTournament(string name, string description, string type, string start, string end, int teams, int prize)
    {
        string tournamentID = System.Guid.NewGuid().ToString();
        Tournament tournament = new Tournament(tournamentID, name, description, type, start, end, teams, prize);
        tournamentData.system.AddTournament(tournament);
        SaveTournamentData();
    }
    
    public void AddTeamToTournament(string tournamentID, string teamID)
    {
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        Team team = tournamentData.system.GetTeam(teamID);
        if (tournament != null && team != null)
        {
            tournament.AddTeam(team);
            SaveTournamentData();
        }
    }
    
    public void StartTournament(string tournamentID)
    {
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            tournament.StartTournament();
            SaveTournamentData();
        }
    }
    
    public void EndTournament(string tournamentID)
    {
        Tournament tournament = tournamentData.system.GetTournament(tournamentID);
        if (tournament != null)
        {
            tournament.EndTournament();
            SaveTournamentData();
        }
    }
    
    public void StartMatch(string matchID)
    {
        Match match = tournamentData.system.GetMatch(matchID);
        if (match != null)
        {
            match.StartMatch();
            SaveTournamentData();
        }
    }
    
    public void EndMatch(string matchID, string winnerID, int score1, int score2)
    {
        Match match = tournamentData.system.GetMatch(matchID);
        if (match != null)
        {
            match.EndMatch(winnerID, score1, score2);
            
            // 更新队伍统计
            Team team1 = tournamentData.system.GetTeam(match.team1ID);
            Team team2 = tournamentData.system.GetTeam(match.team2ID);
            if (team1 != null)
            {
                team1.UpdateStats(team1.teamID == winnerID);
            }
            if (team2 != null)
            {
                team2.UpdateStats(team2.teamID == winnerID);
            }
            
            SaveTournamentData();
        }
    }
    
    public void StartStream(string streamID)
    {
        Stream stream = liveStreams.Find(s => s.streamID == streamID);
        if (stream != null)
        {
            stream.StartStream();
            SaveTournamentData();
        }
    }
    
    public void EndStream(string streamID)
    {
        Stream stream = liveStreams.Find(s => s.streamID == streamID);
        if (stream != null)
        {
            stream.EndStream();
            SaveTournamentData();
        }
    }
    
    public List<Tournament> GetTournaments()
    {
        return tournamentData.system.tournaments;
    }
    
    public List<Match> GetMatches()
    {
        return tournamentData.system.matches;
    }
    
    public List<Team> GetTeams()
    {
        return tournamentData.system.teams;
    }
    
    public List<Stream> GetLiveStreams()
    {
        return liveStreams.FindAll(s => s.streamStatus == "Live");
    }
    
    public Tournament GetTournament(string tournamentID)
    {
        return tournamentData.system.GetTournament(tournamentID);
    }
    
    public Match GetMatch(string matchID)
    {
        return tournamentData.system.GetMatch(matchID);
    }
    
    public Team GetTeam(string teamID)
    {
        return tournamentData.system.GetTeam(teamID);
    }
    
    public Player GetPlayer(string playerID)
    {
        return tournamentData.system.GetPlayer(playerID);
    }
    
    public Stream GetStream(string streamID)
    {
        return liveStreams.Find(s => s.streamID == streamID);
    }
    
    public void SaveTournamentData()
    {
        string path = Application.dataPath + "/Data/tournament_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, tournamentData);
        stream.Close();
        
        // 保存直播数据
        string streamPath = Application.dataPath + "/Data/stream_data.dat";
        FileStream streamStream = new FileStream(streamPath, FileMode.Create);
        formatter.Serialize(streamStream, liveStreams);
        streamStream.Close();
    }
    
    public void LoadTournamentData()
    {
        string path = Application.dataPath + "/Data/tournament_data.dat";
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
        
        // 加载直播数据
        string streamPath = Application.dataPath + "/Data/stream_data.dat";
        if (File.Exists(streamPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream streamStream = new FileStream(streamPath, FileMode.Open);
            liveStreams = (List<Stream>)formatter.Deserialize(streamStream);
            streamStream.Close();
        }
        else
        {
            liveStreams = new List<Stream>();
        }
    }
}
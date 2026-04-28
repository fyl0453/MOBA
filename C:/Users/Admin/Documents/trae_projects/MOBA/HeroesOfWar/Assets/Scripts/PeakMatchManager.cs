using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PeakMatchManager : MonoBehaviour
{
    public static PeakMatchManager Instance { get; private set; }
    
    public PeakMatchManagerData peakMatchData;
    
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
        LoadPeakMatchData();
        
        if (peakMatchData == null)
        {
            peakMatchData = new PeakMatchManagerData();
            InitializeDefaultPeakMatch();
        }
    }
    
    private void InitializeDefaultPeakMatch()
    {
        // 创建巅峰赛赛季
        PeakSeason season1 = new PeakSeason("peak_season_1", "S1巅峰赛", "2024-01-01", "2024-03-31");
        PeakSeason season2 = new PeakSeason("peak_season_2", "S2巅峰赛", "2024-04-01", "2024-06-30");
        
        season1.Activate();
        
        peakMatchData.system.AddSeason(season1);
        peakMatchData.system.AddSeason(season2);
        
        // 创建默认巅峰赛玩家
        PeakPlayer player1 = new PeakPlayer("user_1", "Player1", 1800, "peak_season_1");
        PeakPlayer player2 = new PeakPlayer("user_2", "Player2", 1750, "peak_season_1");
        PeakPlayer player3 = new PeakPlayer("user_3", "Player3", 1700, "peak_season_1");
        
        peakMatchData.system.AddPlayer(player1);
        peakMatchData.system.AddPlayer(player2);
        peakMatchData.system.AddPlayer(player3);
        
        // 更新排名
        UpdatePeakPlayerRanks("peak_season_1");
        
        SavePeakMatchData();
    }
    
    public string CreatePeakMatch(string seasonID, List<string> playerIDs, List<int> teamIDs, List<string> heroIDs)
    {
        string matchID = System.Guid.NewGuid().ToString();
        PeakMatch newMatch = new PeakMatch(matchID, seasonID);
        
        for (int i = 0; i < playerIDs.Count; i++)
        {
            newMatch.AddPlayer(playerIDs[i], teamIDs[i], heroIDs[i]);
        }
        
        newMatch.StartMatch();
        peakMatchData.system.AddMatch(newMatch);
        SavePeakMatchData();
        Debug.Log($"成功创建巅峰赛: {matchID}");
        return matchID;
    }
    
    public void EndPeakMatch(string matchID, string winnerTeamID)
    {
        PeakMatch match = peakMatchData.system.GetMatch(matchID);
        if (match != null)
        {
            match.EndMatch(winnerTeamID);
            
            // 更新玩家数据
            for (int i = 0; i < match.playerIDs.Count; i++)
            {
                string playerID = match.playerIDs[i];
                int teamID = match.teamIDs[i];
                string heroID = match.heroIDs[i];
                
                PeakPlayer player = peakMatchData.system.GetPlayer(playerID);
                if (player != null)
                {
                    bool isWin = (teamID.ToString() == winnerTeamID);
                    int pointsChange = isWin ? 25 : -20;
                    
                    // 更新巅峰积分
                    player.UpdatePeakPoints(player.peakPoints + pointsChange);
                    
                    // 更新胜负记录
                    if (isWin)
                    {
                        player.WinMatch();
                    }
                    else
                    {
                        player.LoseMatch();
                    }
                    
                    // 添加匹配历史
                    PeakMatchHistory history = new PeakMatchHistory(matchID, match.endTime, isWin, heroID, pointsChange);
                    player.AddMatchHistory(history);
                }
            }
            
            // 更新排名
            UpdatePeakPlayerRanks(match.seasonID);
            SavePeakMatchData();
            Debug.Log($"成功结束巅峰赛: {matchID}");
        }
    }
    
    private void UpdatePeakPlayerRanks(string seasonID)
    {
        List<PeakPlayer> players = peakMatchData.system.players.FindAll(p => p.currentSeasonID == seasonID);
        players.Sort((a, b) => b.peakPoints.CompareTo(a.peakPoints));
        
        for (int i = 0; i < players.Count; i++)
        {
            players[i].UpdateRank(i + 1);
        }
    }
    
    public List<PeakPlayer> GetTopPlayers(int limit = 100)
    {
        return peakMatchData.system.GetTopPlayers(limit);
    }
    
    public PeakPlayer GetPeakPlayer(string playerID)
    {
        return peakMatchData.system.GetPlayer(playerID);
    }
    
    public List<PeakMatchHistory> GetPlayerMatchHistory(string playerID, int limit = 20)
    {
        PeakPlayer player = peakMatchData.system.GetPlayer(playerID);
        if (player != null)
        {
            List<PeakMatchHistory> history = new List<PeakMatchHistory>(player.matchHistory);
            history.Reverse();
            return history.GetRange(0, Mathf.Min(limit, history.Count));
        }
        return new List<PeakMatchHistory>();
    }
    
    public PeakSeason GetCurrentSeason()
    {
        return peakMatchData.system.seasons.Find(s => s.isActive);
    }
    
    public void ActivateSeason(string seasonID)
    {
        // 先停用所有赛季
        foreach (PeakSeason season in peakMatchData.system.seasons)
        {
            season.Deactivate();
        }
        
        // 激活指定赛季
        PeakSeason targetSeason = peakMatchData.system.GetSeason(seasonID);
        if (targetSeason != null)
        {
            targetSeason.Activate();
            SavePeakMatchData();
            Debug.Log($"成功激活巅峰赛赛季: {targetSeason.seasonName}");
        }
    }
    
    public int GetPeakPoints(string playerID)
    {
        PeakPlayer player = peakMatchData.system.GetPlayer(playerID);
        return player != null ? player.peakPoints : 0;
    }
    
    public int GetPeakRank(string playerID)
    {
        PeakPlayer player = peakMatchData.system.GetPlayer(playerID);
        return player != null ? player.peakRank : 0;
    }
    
    public bool IsEligibleForPeakMatch(string playerID)
    {
        // 检查玩家是否达到王者段位
        // 这里需要与段位系统集成
        return true; // 简化处理
    }
    
    public void SavePeakMatchData()
    {
        string path = Application.dataPath + "/Data/peak_match_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, peakMatchData);
        stream.Close();
    }
    
    public void LoadPeakMatchData()
    {
        string path = Application.dataPath + "/Data/peak_match_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            peakMatchData = (PeakMatchManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            peakMatchData = new PeakMatchManagerData();
        }
    }
}
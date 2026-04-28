using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RankManager : MonoBehaviour
{
    public static RankManager Instance { get; private set; }
    
    public List<RankSeason> allSeasons;
    public List<RankDivision> allDivisions;
    public PlayerRank playerRank;
    public List<SeasonReward> seasonRewards;
    
    private string currentSeasonID = "season_8";
    
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
        LoadRankData();
        LoadPlayerRank();
        LoadSeasonRewards();
        
        if (allSeasons.Count == 0)
        {
            InitializeDefaultSeasons();
        }
        
        if (allDivisions.Count == 0)
        {
            InitializeDefaultDivisions();
        }
        
        if (playerRank == null)
        {
            CreateNewPlayerRank();
        }
    }
    
    private void InitializeDefaultSeasons()
    {
        allSeasons.Add(new RankSeason("season_1", "S1赛季", 1));
        allSeasons.Add(new RankSeason("season_2", "S2赛季", 2));
        allSeasons.Add(new RankSeason("season_3", "S3赛季", 3));
        allSeasons.Add(new RankSeason("season_4", "S4赛季", 4));
        allSeasons.Add(new RankSeason("season_5", "S5赛季", 5));
        allSeasons.Add(new RankSeason("season_6", "S6赛季", 6));
        allSeasons.Add(new RankSeason("season_7", "S7赛季", 7));
        allSeasons.Add(new RankSeason("season_8", "S8赛季", 8));
        allSeasons[7].isActive = true;
        
        SaveRankData();
    }
    
    private void InitializeDefaultDivisions()
    {
        allDivisions.Add(new RankDivision("bronze_4", "青铜IV", 1, 0, 2));
        allDivisions.Add(new RankDivision("bronze_3", "青铜III", 1, 3, 5));
        allDivisions.Add(new RankDivision("bronze_2", "青铜II", 1, 6, 8));
        allDivisions.Add(new RankDivision("bronze_1", "青铜I", 1, 9, 11));
        
        allDivisions.Add(new RankDivision("silver_4", "白银IV", 2, 0, 2));
        allDivisions.Add(new RankDivision("silver_3", "白银III", 2, 3, 5));
        allDivisions.Add(new RankDivision("silver_2", "白银II", 2, 6, 8));
        allDivisions.Add(new RankDivision("silver_1", "白银I", 2, 9, 11));
        
        allDivisions.Add(new RankDivision("gold_4", "黄金IV", 3, 0, 2));
        allDivisions.Add(new RankDivision("gold_3", "黄金III", 3, 3, 5));
        allDivisions.Add(new RankDivision("gold_2", "黄金II", 3, 6, 8));
        allDivisions.Add(new RankDivision("gold_1", "黄金I", 3, 9, 11));
        
        allDivisions.Add(new RankDivision("platinum_4", "铂金IV", 4, 0, 2));
        allDivisions.Add(new RankDivision("platinum_3", "铂金III", 4, 3, 5));
        allDivisions.Add(new RankDivision("platinum_2", "铂金II", 4, 6, 8));
        allDivisions.Add(new RankDivision("platinum_1", "铂金I", 4, 9, 11));
        
        allDivisions.Add(new RankDivision("diamond_4", "钻石IV", 5, 0, 2));
        allDivisions.Add(new RankDivision("diamond_3", "钻石III", 5, 3, 5));
        allDivisions.Add(new RankDivision("diamond_2", "钻石II", 5, 6, 8));
        allDivisions.Add(new RankDivision("diamond_1", "钻石I", 5, 9, 11));
        
        allDivisions.Add(new RankDivision("master", "宗师", 6, 0, 999));
        allDivisions.Add(new RankDivision("grandmaster", "大师", 7, 0, 999));
        allDivisions.Add(new RankDivision("king", "王者", 8, 0, 999));
        
        SaveRankData();
    }
    
    private void CreateNewPlayerRank()
    {
        playerRank = new PlayerRank("player_001");
        playerRank.currentSeasonID = currentSeasonID;
        playerRank.currentDivisionID = "bronze_4";
        SavePlayerRank();
    }
    
    public void ProcessMatchResult(bool won)
    {
        if (won)
        {
            ProcessWin();
        }
        else
        {
            ProcessLoss();
        }
        
        SavePlayerRank();
    }
    
    private void ProcessWin()
    {
        playerRank.AddWin();
        int starsToAdd = 1;
        
        if (playerRank.consecutiveWins >= 3 && playerRank.currentDivisionID != "king")
        {
            starsToAdd = 2;
        }
        
        for (int i = 0; i < starsToAdd; i++)
        {
            AddStar();
        }
        
        if (playerRank.stars > GetCurrentDivision().maxStars)
        {
            PromoteDivision();
        }
    }
    
    private void ProcessLoss()
    {
        playerRank.AddLoss();
        
        if (!playerRank.protectedStatus && playerRank.stars > 0)
        {
            RemoveStar();
            
            if (playerRank.stars < 0)
            {
                DemoteDivision();
            }
        }
        
        if (playerRank.consecutiveLosses >= 3)
        {
            playerRank.protectedStatus = true;
        }
    }
    
    private void AddStar()
    {
        playerRank.stars++;
        playerRank.totalStars++;
        
        if (playerRank.stars > playerRank.peakStars)
        {
            playerRank.peakStars = playerRank.stars;
            playerRank.peakDivisionID = playerRank.currentDivisionID;
        }
    }
    
    private void RemoveStar()
    {
        playerRank.stars--;
    }
    
    private void PromoteDivision()
    {
        RankDivision current = GetCurrentDivision();
        int nextIndex = allDivisions.FindIndex(d => d.divisionID == current.divisionID) + 1;
        
        if (nextIndex < allDivisions.Count)
        {
            playerRank.currentDivisionID = allDivisions[nextIndex].divisionID;
            playerRank.stars = 0;
            
            GenerateSeasonReward();
            
            Debug.Log($"晋升到: {allDivisions[nextIndex].divisionName}");
        }
    }
    
    private void DemoteDivision()
    {
        RankDivision current = GetCurrentDivision();
        int prevIndex = allDivisions.FindIndex(d => d.divisionID == current.divisionID) - 1;
        
        if (prevIndex >= 0)
        {
            playerRank.currentDivisionID = allDivisions[prevIndex].divisionID;
            playerRank.stars = allDivisions[prevIndex].maxStars - 1;
            playerRank.protectedStatus = false;
            
            Debug.Log($"降级到: {allDivisions[prevIndex].divisionName}");
        }
    }
    
    private void GenerateSeasonReward()
    {
        string rewardID = $"reward_{System.DateTime.Now.Ticks}";
        SeasonReward reward = new SeasonReward(
            rewardID,
            playerRank.currentSeasonID,
            playerRank.currentDivisionID,
            "skin",
            "season_skin",
            1
        );
        
        seasonRewards.Add(reward);
        SaveSeasonRewards();
    }
    
    public RankDivision GetCurrentDivision()
    {
        return allDivisions.Find(d => d.divisionID == playerRank.currentDivisionID);
    }
    
    public RankSeason GetCurrentSeason()
    {
        return allSeasons.Find(s => s.seasonID == currentSeasonID);
    }
    
    public string GetRankIcon()
    {
        RankDivision division = GetCurrentDivision();
        return division.icon;
    }
    
    public int GetStarsToNextDivision()
    {
        RankDivision current = GetCurrentDivision();
        return current.maxStars - playerRank.stars + 1;
    }
    
    public float GetProgressToNextDivision()
    {
        RankDivision current = GetCurrentDivision();
        return (float)playerRank.stars / (current.maxStars + 1);
    }
    
    public List<SeasonReward> GetUnclaimedRewards()
    {
        return seasonRewards.FindAll(r => !r.isClaimed);
    }
    
    public void ClaimReward(string rewardID)
    {
        SeasonReward reward = seasonRewards.Find(r => r.rewardID == rewardID);
        if (reward != null)
        {
            reward.isClaimed = true;
            SaveSeasonRewards();
            
            Debug.Log($"领取奖励: {reward.rewardType} x{reward.quantity}");
        }
    }
    
    public void SaveRankData()
    {
        string path = Application.dataPath + "/Data/rank_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        RankData data = new RankData();
        data.allSeasons = allSeasons;
        data.allDivisions = allDivisions;
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    
    public void LoadRankData()
    {
        string path = Application.dataPath + "/Data/rank_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            RankData data = (RankData)formatter.Deserialize(stream);
            stream.Close();
            
            allSeasons = data.allSeasons;
            allDivisions = data.allDivisions;
        }
        else
        {
            allSeasons = new List<RankSeason>();
            allDivisions = new List<RankDivision>();
        }
    }
    
    public void SavePlayerRank()
    {
        string path = Application.dataPath + "/Data/player_rank.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, playerRank);
        stream.Close();
    }
    
    public void LoadPlayerRank()
    {
        string path = Application.dataPath + "/Data/player_rank.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            playerRank = (PlayerRank)formatter.Deserialize(stream);
            stream.Close();
        }
    }
    
    public void SaveSeasonRewards()
    {
        string path = Application.dataPath + "/Data/season_rewards.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, seasonRewards);
        stream.Close();
    }
    
    public void LoadSeasonRewards()
    {
        string path = Application.dataPath + "/Data/season_rewards.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            seasonRewards = (List<SeasonReward>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            seasonRewards = new List<SeasonReward>();
        }
    }
}

[System.Serializable]
public class RankData
{
    public List<RankSeason> allSeasons;
    public List<RankDivision> allDivisions;
}
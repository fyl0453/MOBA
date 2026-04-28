using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CareerManager : MonoBehaviour
{
    public static CareerManager Instance { get; private set; }
    
    public CareerManagerData careerData;
    
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
        LoadCareerData();
        
        if (careerData == null)
        {
            careerData = new CareerManagerData();
        }
        
        // 确保当前玩家有生涯数据
        EnsurePlayerCareerData();
    }
    
    private void EnsurePlayerCareerData()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        PlayerCareer career = careerData.GetPlayerCareer(playerID);
        if (career == null)
        {
            string playerName = ProfileManager.Instance.currentProfile.playerName;
            career = new PlayerCareer(playerID, playerName);
            careerData.AddPlayerCareer(career);
            SaveCareerData();
        }
    }
    
    public void UpdateMatchStats(int kills, int deaths, int assists, int gold, int damageDealt, int damageTaken, int healing, int visionScore, bool isWin, bool isMVP)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        PlayerCareer career = careerData.GetPlayerCareer(playerID);
        if (career != null)
        {
            career.UpdateMatchStats(kills, deaths, assists, gold, damageDealt, damageTaken, healing, visionScore, isWin, isMVP);
            
            // 更新当前赛季数据
            string currentSeason = RankManager.Instance.GetCurrentSeason().seasonID;
            CareerStat seasonStat = career.seasonStats.Find(s => s.seasonID == currentSeason);
            if (seasonStat == null)
            {
                seasonStat = new CareerStat(currentSeason);
                career.AddSeasonStat(seasonStat);
            }
            seasonStat.UpdateStats(kills, deaths, assists, isWin, isMVP);
            
            SaveCareerData();
        }
    }
    
    public void UpdateHighestRank(int rank, string season)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        PlayerCareer career = careerData.GetPlayerCareer(playerID);
        if (career != null)
        {
            career.UpdateHighestRank(rank, season);
            SaveCareerData();
        }
    }
    
    public void SetSeasonFinalRank(int rank)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        PlayerCareer career = careerData.GetPlayerCareer(playerID);
        if (career != null)
        {
            string currentSeason = RankManager.Instance.GetCurrentSeason().seasonID;
            CareerStat seasonStat = career.seasonStats.Find(s => s.seasonID == currentSeason);
            if (seasonStat != null)
            {
                seasonStat.SetFinalRank(rank);
                SaveCareerData();
            }
        }
    }
    
    public PlayerCareer GetPlayerCareer()
    {
        return careerData.GetPlayerCareer(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public void SaveCareerData()
    {
        string path = Application.dataPath + "/Data/career_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, careerData);
        stream.Close();
    }
    
    public void LoadCareerData()
    {
        string path = Application.dataPath + "/Data/career_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            careerData = (CareerManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            careerData = new CareerManagerData();
        }
    }
}
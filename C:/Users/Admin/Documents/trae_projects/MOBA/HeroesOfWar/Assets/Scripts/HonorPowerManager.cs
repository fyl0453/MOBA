using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HonorPowerManager : MonoBehaviour
{
    public static HonorPowerManager Instance { get; private set; }
    
    public HonorPowerManagerData honorPowerData;
    
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
        LoadHonorPowerData();
        
        if (honorPowerData == null)
        {
            honorPowerData = new HonorPowerManagerData();
            InitializeDefaultHonorPower();
        }
    }
    
    private void InitializeDefaultHonorPower()
    {
        // 创建荣耀战力排名
        HonorPowerRank rank1 = new HonorPowerRank("rank_1", "县服", "县级排名", 1000, 1999);
        HonorPowerRank rank2 = new HonorPowerRank("rank_2", "市服", "市级排名", 2000, 2999);
        HonorPowerRank rank3 = new HonorPowerRank("rank_3", "省服", "省级排名", 3000, 3999);
        HonorPowerRank rank4 = new HonorPowerRank("rank_4", "国服", "国服排名", 4000, int.MaxValue);
        
        honorPowerData.system.AddHonorPowerRank(rank1);
        honorPowerData.system.AddHonorPowerRank(rank2);
        honorPowerData.system.AddHonorPowerRank(rank3);
        honorPowerData.system.AddHonorPowerRank(rank4);
        
        // 创建默认英雄荣耀战力
        HeroHonorPower hero1 = new HeroHonorPower("user_1", "Player1", "hero_guanyu", "关羽", 3500, "season_1");
        HeroHonorPower hero2 = new HeroHonorPower("user_2", "Player2", "hero_guanyu", "关羽", 3200, "season_1");
        HeroHonorPower hero3 = new HeroHonorPower("user_3", "Player3", "hero_zhugeliang", "诸葛亮", 3000, "season_1");
        
        honorPowerData.system.AddHeroHonorPower(hero1);
        honorPowerData.system.AddHeroHonorPower(hero2);
        honorPowerData.system.AddHeroHonorPower(hero3);
        
        // 更新排名
        UpdateHeroHonorPowerRanks("hero_guanyu");
        UpdateHeroHonorPowerRanks("hero_zhugeliang");
        
        SaveHonorPowerData();
    }
    
    public void UpdateHeroHonorPower(string playerID, string playerName, string heroID, string heroName, bool isWin, string seasonID)
    {
        HeroHonorPower existing = honorPowerData.system.GetHeroHonorPower(playerID, heroID);
        if (existing != null)
        {
            // 计算战力变化
            int powerChange = isWin ? 30 : -20;
            existing.UpdateHonorPower(existing.honorPower + powerChange);
            existing.AddMatch(isWin, powerChange);
        }
        else
        {
            // 新英雄初始战力
            int initialPower = 1000;
            HeroHonorPower newHeroHonorPower = new HeroHonorPower(playerID, playerName, heroID, heroName, initialPower, seasonID);
            newHeroHonorPower.AddMatch(isWin, 30);
            honorPowerData.system.AddHeroHonorPower(newHeroHonorPower);
        }
        
        UpdateHeroHonorPowerRanks(heroID);
        SaveHonorPowerData();
    }
    
    private void UpdateHeroHonorPowerRanks(string heroID)
    {
        List<HeroHonorPower> heroHonorPowers = honorPowerData.system.GetHeroHonorPowersByHero(heroID);
        heroHonorPowers.Sort((a, b) => b.honorPower.CompareTo(a.honorPower));
        
        for (int i = 0; i < heroHonorPowers.Count; i++)
        {
            heroHonorPowers[i].UpdateRank(i + 1);
        }
    }
    
    public List<HeroHonorPower> GetTopHeroHonorPowers(string heroID, int limit = 100)
    {
        return honorPowerData.system.GetTopHeroHonorPowers(heroID, limit);
    }
    
    public HeroHonorPower GetHeroHonorPower(string playerID, string heroID)
    {
        return honorPowerData.system.GetHeroHonorPower(playerID, heroID);
    }
    
    public List<HeroHonorPower> GetPlayerHeroHonorPowers(string playerID)
    {
        return honorPowerData.system.GetHeroHonorPowersByPlayer(playerID);
    }
    
    public int GetHeroHonorPower(string playerID, string heroID)
    {
        HeroHonorPower heroHonorPower = honorPowerData.system.GetHeroHonorPower(playerID, heroID);
        return heroHonorPower != null ? heroHonorPower.honorPower : 0;
    }
    
    public int GetHeroHonorRank(string playerID, string heroID)
    {
        HeroHonorPower heroHonorPower = honorPowerData.system.GetHeroHonorPower(playerID, heroID);
        return heroHonorPower != null ? heroHonorPower.rank : 0;
    }
    
    public string GetHonorPowerRankName(int honorPower)
    {
        HonorPowerRank rank = honorPowerData.system.honorPowerRanks.Find(r => honorPower >= r.minPower && honorPower <= r.maxPower);
        return rank != null ? rank.rankName : "无排名";
    }
    
    public float GetHeroWinRate(string playerID, string heroID)
    {
        HeroHonorPower heroHonorPower = honorPowerData.system.GetHeroHonorPower(playerID, heroID);
        return heroHonorPower != null ? heroHonorPower.GetWinRate() : 0;
    }
    
    public int GetHeroMatches(string playerID, string heroID)
    {
        HeroHonorPower heroHonorPower = honorPowerData.system.GetHeroHonorPower(playerID, heroID);
        return heroHonorPower != null ? heroHonorPower.matches : 0;
    }
    
    public int GetHeroWins(string playerID, string heroID)
    {
        HeroHonorPower heroHonorPower = honorPowerData.system.GetHeroHonorPower(playerID, heroID);
        return heroHonorPower != null ? heroHonorPower.wins : 0;
    }
    
    public void SaveHonorPowerData()
    {
        string path = Application.dataPath + "/Data/honor_power_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, honorPowerData);
        stream.Close();
    }
    
    public void LoadHonorPowerData()
    {
        string path = Application.dataPath + "/Data/honor_power_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            honorPowerData = (HonorPowerManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            honorPowerData = new HonorPowerManagerData();
        }
    }
}
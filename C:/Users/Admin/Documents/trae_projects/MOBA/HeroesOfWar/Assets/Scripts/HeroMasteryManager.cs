using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HeroMasteryManager : MonoBehaviour
{
    public static HeroMasteryManager Instance { get; private set; }
    
    public HeroMasteryManagerData heroMasteryData;
    
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
        LoadHeroMasteryData();
        
        if (heroMasteryData == null)
        {
            heroMasteryData = new HeroMasteryManagerData();
            InitializeDefaultHeroMasteries();
        }
    }
    
    private void InitializeDefaultHeroMasteries()
    {
        HeroMastery guanyu = new HeroMastery("hero_guanyu", "关羽");
        guanyu.Unlock();
        heroMasteryData.AddHeroMastery(guanyu);
        
        HeroMastery zhangfei = new HeroMastery("hero_zhangfei", "张飞");
        zhangfei.Unlock();
        heroMasteryData.AddHeroMastery(zhangfei);
        
        HeroMastery liubei = new HeroMastery("hero_liubei", "刘备");
        liubei.Unlock();
        heroMasteryData.AddHeroMastery(liubei);
        
        HeroMastery zhaoyun = new HeroMastery("hero_zhaoyun", "赵云");
        zhaoyun.Unlock();
        heroMasteryData.AddHeroMastery(zhaoyun);
        
        HeroMastery zhugeliang = new HeroMastery("hero_zhugeliang", "诸葛亮");
        zhugeliang.Unlock();
        heroMasteryData.AddHeroMastery(zhugeliang);
        
        SaveHeroMasteryData();
    }
    
    public void AddMasteryPoints(string heroID, int points)
    {
        HeroMastery mastery = heroMasteryData.GetHeroMastery(heroID);
        if (mastery != null && mastery.isUnlocked)
        {
            mastery.AddMasteryPoints(points);
            SaveHeroMasteryData();
        }
    }
    
    public void UnlockHero(string heroID, string heroName)
    {
        HeroMastery mastery = heroMasteryData.GetHeroMastery(heroID);
        if (mastery == null)
        {
            mastery = new HeroMastery(heroID, heroName);
            mastery.Unlock();
            heroMasteryData.AddHeroMastery(mastery);
            SaveHeroMasteryData();
        }
        else if (!mastery.isUnlocked)
        {
            mastery.Unlock();
            SaveHeroMasteryData();
        }
    }
    
    public HeroMastery GetHeroMastery(string heroID)
    {
        return heroMasteryData.GetHeroMastery(heroID);
    }
    
    public List<HeroMastery> GetUnlockedHeroes()
    {
        return heroMasteryData.GetUnlockedHeroes();
    }
    
    public List<HeroMastery> GetHeroesByMasteryLevel(int level)
    {
        return heroMasteryData.GetHeroesByMasteryLevel(level);
    }
    
    public void SaveHeroMasteryData()
    {
        string path = Application.dataPath + "/Data/hero_mastery_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, heroMasteryData);
        stream.Close();
    }
    
    public void LoadHeroMasteryData()
    {
        string path = Application.dataPath + "/Data/hero_mastery_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            heroMasteryData = (HeroMasteryManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            heroMasteryData = new HeroMasteryManagerData();
        }
    }
}
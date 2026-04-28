using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MarkManager : MonoBehaviour
{
    public static MarkManager Instance { get; private set; }
    
    public MarkManagerData markData;
    
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
        LoadMarkData();
        
        if (markData == null)
        {
            markData = new MarkManagerData();
        }
        
        // 确保当前玩家有印记数据
        EnsurePlayerMarkData();
    }
    
    private void EnsurePlayerMarkData()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        MarkCollection collection = markData.GetMarkCollection(playerID);
        if (collection == null)
        {
            collection = new MarkCollection(playerID);
            
            // 添加默认印记
            collection.AddMark(new Mark("mark_king", "王者印记", "达到最强王者段位", "Rank", "icon_king"));
            collection.AddMark(new Mark("mark_mvp", "MVP印记", "获得10次MVP", "Match", "icon_mvp"));
            collection.AddMark(new Mark("mark_kill", "击杀王印记", "单场比赛击杀20人", "Combat", "icon_kill"));
            collection.AddMark(new Mark("mark_assist", "助攻王印记", "单场比赛助攻30次", "Combat", "icon_assist"));
            collection.AddMark(new Mark("mark_defense", "守护印记", "单场比赛承受伤害10000", "Combat", "icon_defense"));
            collection.AddMark(new Mark("mark_ward", "视野印记", "单场比赛放置10个守卫", "Combat", "icon_ward"));
            collection.AddMark(new Mark("mark_gold", "金钱印记", "单场比赛获得10000金币", "Economy", "icon_gold"));
            collection.AddMark(new Mark("mark_win", "胜利印记", "连胜10场", "Match", "icon_win"));
            collection.AddMark(new Mark("mark_hero", "英雄印记", "解锁所有英雄", "Collection", "icon_hero"));
            collection.AddMark(new Mark("mark_skin", "皮肤印记", "拥有50个皮肤", "Collection", "icon_skin"));
            
            markData.AddMarkCollection(collection);
            SaveMarkData();
        }
    }
    
    public void ObtainMark(string markID)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        MarkCollection collection = markData.GetMarkCollection(playerID);
        if (collection != null)
        {
            collection.ObtainMark(markID);
            SaveMarkData();
        }
    }
    
    public List<Mark> GetObtainedMarks()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        MarkCollection collection = markData.GetMarkCollection(playerID);
        if (collection != null)
        {
            return collection.GetObtainedMarks();
        }
        return new List<Mark>();
    }
    
    public List<Mark> GetMarksByType(string type)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        MarkCollection collection = markData.GetMarkCollection(playerID);
        if (collection != null)
        {
            return collection.GetMarksByType(type);
        }
        return new List<Mark>();
    }
    
    public Mark GetMark(string markID)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        MarkCollection collection = markData.GetMarkCollection(playerID);
        if (collection != null)
        {
            return collection.marks.Find(m => m.markID == markID);
        }
        return null;
    }
    
    public void SaveMarkData()
    {
        string path = Application.dataPath + "/Data/mark_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, markData);
        stream.Close();
    }
    
    public void LoadMarkData()
    {
        string path = Application.dataPath + "/Data/mark_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            markData = (MarkManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            markData = new MarkManagerData();
        }
    }
}
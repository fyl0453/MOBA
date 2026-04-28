using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MentorManager : MonoBehaviour
{
    public static MentorManager Instance { get; private set; }
    
    public MentorManagerData mentorData;
    
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
        LoadMentorData();
        
        if (mentorData == null)
        {
            mentorData = new MentorManagerData();
        }
    }
    
    public MentorApprentice EstablishMentorRelationship(string mentorID, string mentorName, string apprenticeID, string apprenticeName)
    {
        // 检查是否已有师徒关系
        MentorApprentice existingRelationship = mentorData.GetRelationshipBetweenPlayers(mentorID, apprenticeID);
        if (existingRelationship != null)
        {
            return existingRelationship;
        }
        
        string relationshipID = "mentor_" + System.DateTime.Now.Ticks;
        MentorApprentice relationship = new MentorApprentice(relationshipID, mentorID, mentorName, apprenticeID, apprenticeName);
        
        // 添加默认师徒任务
        relationship.AddTask("task_mentor_1", "首次胜利", "师徒一起赢得第一场比赛", 1);
        relationship.AddTask("task_mentor_2", "击杀敌人", "师徒一起击杀30个敌人", 30);
        relationship.AddTask("task_mentor_3", "使用技能", "师徒一起使用技能50次", 50);
        relationship.AddTask("task_mentor_4", "升级", "徒弟等级达到10级", 1);
        
        mentorData.AddMentorApprentice(relationship);
        SaveMentorData();
        
        return relationship;
    }
    
    public void UpdateTaskProgress(string relationshipID, string taskID, int progress)
    {
        MentorApprentice relationship = mentorData.GetMentorApprentice(relationshipID);
        if (relationship != null && relationship.status == "Active")
        {
            relationship.UpdateTaskProgress(taskID, progress);
            SaveMentorData();
        }
    }
    
    public void GraduateApprentice(string relationshipID)
    {
        MentorApprentice relationship = mentorData.GetMentorApprentice(relationshipID);
        if (relationship != null && relationship.status == "Active")
        {
            relationship.Graduate();
            SaveMentorData();
        }
    }
    
    public void EndMentorRelationship(string relationshipID)
    {
        MentorApprentice relationship = mentorData.GetMentorApprentice(relationshipID);
        if (relationship != null)
        {
            relationship.EndRelationship();
            SaveMentorData();
        }
    }
    
    public List<MentorApprentice> GetMentorRelationships()
    {
        return mentorData.GetMentorRelationships(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public List<MentorApprentice> GetApprenticeRelationships()
    {
        return mentorData.GetApprenticeRelationships(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public MentorApprentice GetMentorApprentice(string relationshipID)
    {
        return mentorData.GetMentorApprentice(relationshipID);
    }
    
    public void SaveMentorData()
    {
        string path = Application.dataPath + "/Data/mentor_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, mentorData);
        stream.Close();
    }
    
    public void LoadMentorData()
    {
        string path = Application.dataPath + "/Data/mentor_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            mentorData = (MentorManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            mentorData = new MentorManagerData();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class MentorRelationship
{
    public string RelationshipID;
    public string MasterID;
    public string MasterName;
    public string ApprenticeID;
    public string ApprenticeName;
    public int Status;
    public DateTime EstablishTime;
    public DateTime LastInteractionTime;
    public int MasterLevel;
    public int ApprenticeLevel;
    public int Progress;
    public bool IsCompleted;

    public MentorRelationship(string relationshipID, string masterID, string masterName, string apprenticeID, string apprenticeName, int masterLevel, int apprenticeLevel)
    {
        RelationshipID = relationshipID;
        MasterID = masterID;
        MasterName = masterName;
        ApprenticeID = apprenticeID;
        ApprenticeName = apprenticeName;
        Status = 0;
        EstablishTime = DateTime.Now;
        LastInteractionTime = DateTime.Now;
        MasterLevel = masterLevel;
        ApprenticeLevel = apprenticeLevel;
        Progress = 0;
        IsCompleted = false;
    }
}

[Serializable]
public class MentorTask
{
    public string TaskID;
    public string TaskName;
    public string TaskDescription;
    public int TaskType;
    public int RequiredCount;
    public List<string> Rewards;
    public int TaskLevel;
    public bool IsActive;
    public DateTime StartTime;
    public DateTime EndTime;

    public MentorTask(string taskID, string taskName, string taskDescription, int taskType, int requiredCount, List<string> rewards, int taskLevel)
    {
        TaskID = taskID;
        TaskName = taskName;
        TaskDescription = taskDescription;
        TaskType = taskType;
        RequiredCount = requiredCount;
        Rewards = rewards;
        TaskLevel = taskLevel;
        IsActive = true;
        StartTime = DateTime.Now;
        EndTime = DateTime.MaxValue;
    }
}

[Serializable]
public class MentorTaskProgress
{
    public string ProgressID;
    public string RelationshipID;
    public string TaskID;
    public int CurrentCount;
    public bool IsCompleted;
    public DateTime LastUpdateTime;
    public DateTime CompleteTime;

    public MentorTaskProgress(string progressID, string relationshipID, string taskID)
    {
        ProgressID = progressID;
        RelationshipID = relationshipID;
        TaskID = taskID;
        CurrentCount = 0;
        IsCompleted = false;
        LastUpdateTime = DateTime.Now;
        CompleteTime = DateTime.MinValue;
    }
}

[Serializable]
public class MentorRewards
{
    public string RewardID;
    public string RelationshipID;
    public string RewardType;
    public string RewardName;
    public int RewardAmount;
    public bool IsClaimed;
    public DateTime ClaimTime;

    public MentorRewards(string rewardID, string relationshipID, string rewardType, string rewardName, int rewardAmount)
    {
        RewardID = rewardID;
        RelationshipID = relationshipID;
        RewardType = rewardType;
        RewardName = rewardName;
        RewardAmount = rewardAmount;
        IsClaimed = false;
        ClaimTime = DateTime.MinValue;
    }
}

[Serializable]
public class PlayerMentorData
{
    public string PlayerID;
    public List<MentorRelationship> MasterRelationships;
    public List<MentorRelationship> ApprenticeRelationships;
    public int TotalMasters;
    public int TotalApprentices;
    public int CompletedApprentices;
    public int MasterLevel;
    public DateTime LastMentorActivity;

    public PlayerMentorData(string playerID)
    {
        PlayerID = playerID;
        MasterRelationships = new List<MentorRelationship>();
        ApprenticeRelationships = new List<MentorRelationship>();
        TotalMasters = 0;
        TotalApprentices = 0;
        CompletedApprentices = 0;
        MasterLevel = 1;
        LastMentorActivity = DateTime.MinValue;
    }
}

[Serializable]
public class MentorSystemData
{
    public List<MentorRelationship> AllRelationships;
    public List<MentorTask> AllTasks;
    public List<MentorTaskProgress> TaskProgresses;
    public List<MentorRewards> Rewards;
    public Dictionary<string, PlayerMentorData> PlayerMentorData;
    public int MaxApprenticesPerMaster;
    public int MinMasterLevel;
    public int MaxMastersPerApprentice;
    public DateTime LastSystemUpdate;

    public MentorSystemData()
    {
        AllRelationships = new List<MentorRelationship>();
        AllTasks = new List<MentorTask>();
        TaskProgresses = new List<MentorTaskProgress>();
        Rewards = new List<MentorRewards>();
        PlayerMentorData = new Dictionary<string, PlayerMentorData>();
        MaxApprenticesPerMaster = 3;
        MinMasterLevel = 20;
        MaxMastersPerApprentice = 1;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultTasks();
    }

    private void InitializeDefaultTasks()
    {
        MentorTask task1 = new MentorTask("task_001", "一起战斗", "与师父/徒弟一起完成5场对局", 0, 5, new List<string> { "经验值+100", "金币+50" }, 1);
        AllTasks.Add(task1);

        MentorTask task2 = new MentorTask("task_002", "等级提升", "徒弟等级达到15级", 1, 1, new List<string> { "经验值+200", "皮肤碎片+2" }, 2);
        AllTasks.Add(task2);

        MentorTask task3 = new MentorTask("task_003", "段位提升", "徒弟段位达到白银", 2, 1, new List<string> { "经验值+300", "英雄碎片+1" }, 3);
        AllTasks.Add(task3);

        MentorTask task4 = new MentorTask("task_004", "默契配合", "与师父/徒弟获得3次MVP", 3, 3, new List<string> { "经验值+150", "钻石+20" }, 1);
        AllTasks.Add(task4);

        MentorTask task5 = new MentorTask("task_005", "连续在线", "连续7天与师父/徒弟一起游戏", 4, 7, new List<string> { "经验值+500", "专属师徒头像框" }, 4);
        AllTasks.Add(task5);
    }

    public void AddRelationship(MentorRelationship relationship)
    {
        AllRelationships.Add(relationship);
    }

    public void AddTask(MentorTask task)
    {
        AllTasks.Add(task);
    }

    public void AddTaskProgress(MentorTaskProgress progress)
    {
        TaskProgresses.Add(progress);
    }

    public void AddReward(MentorRewards reward)
    {
        Rewards.Add(reward);
    }

    public void AddPlayerMentorData(string playerID, PlayerMentorData data)
    {
        PlayerMentorData[playerID] = data;
    }
}

[Serializable]
public class MentorEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string TargetPlayerID;
    public string EventData;

    public MentorEvent(string eventID, string eventType, string playerID, string targetPlayerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        TargetPlayerID = targetPlayerID;
        EventData = eventData;
    }
}

public class MentorSystemDataManager
{
    private static MentorSystemDataManager _instance;
    public static MentorSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MentorSystemDataManager();
            }
            return _instance;
        }
    }

    public MentorSystemData mentorData;
    private List<MentorEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private MentorSystemDataManager()
    {
        mentorData = new MentorSystemData();
        recentEvents = new List<MentorEvent>();
        LoadMentorData();
    }

    public void SaveMentorData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MentorSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, mentorData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存师徒系统数据失败: " + e.Message);
        }
    }

    public void LoadMentorData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "MentorSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    mentorData = (MentorSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载师徒系统数据失败: " + e.Message);
            mentorData = new MentorSystemData();
        }
    }

    public void CreateMentorEvent(string eventType, string playerID, string targetPlayerID, string eventData)
    {
        string eventID = "mentor_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MentorEvent mentorEvent = new MentorEvent(eventID, eventType, playerID, targetPlayerID, eventData);
        recentEvents.Add(mentorEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<MentorEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
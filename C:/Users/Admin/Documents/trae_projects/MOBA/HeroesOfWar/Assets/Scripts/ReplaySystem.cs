using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ReplaySystem : MonoBehaviour
{
    public static ReplaySystem Instance { get; private set; }
    
    public List<ReplayData> replayList = new List<ReplayData>();
    private string replayDirectory;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            replayDirectory = Path.Combine(Application.dataPath, "Replays");
            Directory.CreateDirectory(replayDirectory);
            LoadReplays();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RecordReplay(int matchID, string matchName, List<Player> blueTeam, List<Player> redTeam)
    {
        // 创建回放数据
        ReplayData replay = new ReplayData
        {
            replayID = Random.Range(10000, 99999),
            matchID = matchID,
            matchName = matchName,
            blueTeam = blueTeam,
            redTeam = redTeam,
            recordTime = System.DateTime.Now,
            events = new List<ReplayEvent>()
        };
        
        // 开始记录事件
        // 这里需要在游戏过程中记录各种事件
        
        replayList.Add(replay);
        Debug.Log($"开始录制回放: {replay.replayID}");
    }
    
    public void AddReplayEvent(ReplayEvent replayEvent)
    {
        if (replayList.Count > 0)
        {
            ReplayData currentReplay = replayList[replayList.Count - 1];
            currentReplay.events.Add(replayEvent);
        }
    }
    
    public void SaveReplay()
    {
        if (replayList.Count > 0)
        {
            ReplayData currentReplay = replayList[replayList.Count - 1];
            string filePath = Path.Combine(replayDirectory, $"replay_{currentReplay.replayID}.dat");
            
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(filePath, FileMode.Create);
                formatter.Serialize(stream, currentReplay);
                stream.Close();
                
                Debug.Log($"回放保存成功: {currentReplay.replayID}");
            }
            catch (System.Exception e)
            {
                Debug.LogError("保存回放失败: " + e.Message);
            }
        }
    }
    
    public void LoadReplays()
    {
        replayList.Clear();
        
        try
        {
            string[] replayFiles = Directory.GetFiles(replayDirectory, "*.dat");
            foreach (string file in replayFiles)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(file, FileMode.Open);
                ReplayData replay = formatter.Deserialize(stream) as ReplayData;
                stream.Close();
                
                if (replay != null)
                {
                    replayList.Add(replay);
                }
            }
            
            Debug.Log($"加载了 {replayList.Count} 个回放");
        }
        catch (System.Exception e)
        {
            Debug.LogError("加载回放失败: " + e.Message);
        }
    }
    
    public List<ReplayData> GetReplayList()
    {
        return replayList;
    }
    
    public ReplayData GetReplayByID(int replayID)
    {
        return replayList.Find(replay => replay.replayID == replayID);
    }
    
    public void PlayReplay(int replayID)
    {
        ReplayData replay = GetReplayByID(replayID);
        if (replay != null)
        {
            Debug.Log($"开始播放回放: {replay.replayID}");
            // 这里需要实现回放播放逻辑
        }
    }
    
    public void DeleteReplay(int replayID)
    {
        ReplayData replay = GetReplayByID(replayID);
        if (replay != null)
        {
            string filePath = Path.Combine(replayDirectory, $"replay_{replay.replayID}.dat");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                replayList.Remove(replay);
                Debug.Log($"回放删除成功: {replay.replayID}");
            }
        }
    }
}

[System.Serializable]
public class ReplayData
{
    public int replayID;
    public int matchID;
    public string matchName;
    public List<Player> blueTeam;
    public List<Player> redTeam;
    public System.DateTime recordTime;
    public List<ReplayEvent> events;
}

[System.Serializable]
public class ReplayEvent
{
    public float time;
    public EventType eventType;
    public string eventData;
    
    public ReplayEvent(float t, EventType type, string data)
    {
        time = t;
        eventType = type;
        eventData = data;
    }
}

public enum EventType
{
    PlayerJoin,
    PlayerLeave,
    HeroSelect,
    GameStart,
    GameEnd,
    Kill,
    Death,
    Assist,
    SkillCast,
    ItemUse,
    TowerDestroy,
    BaseDestroy,
    LevelUp,
    GoldEarned
}

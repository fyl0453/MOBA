using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance { get; private set; }
    
    public ReplayManagerData replayData;
    
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
        LoadReplayData();
        
        if (replayData == null)
        {
            replayData = new ReplayManagerData();
        }
    }
    
    public Replay SaveReplay(string matchID, string matchType, string matchResult, string duration, string replayPath)
    {
        string replayID = "replay_" + System.DateTime.Now.Ticks;
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        string playerName = ProfileManager.Instance.currentProfile.playerName;
        
        Replay replay = new Replay(replayID, matchID, matchType, matchResult, playerID, playerName, duration, replayPath);
        replayData.AddReplay(replay);
        SaveReplayData();
        
        return replay;
    }
    
    public void DeleteReplay(string replayID)
    {
        Replay replay = replayData.GetReplay(replayID);
        if (replay != null)
        {
            replay.Delete();
            SaveReplayData();
        }
    }
    
    public List<Replay> GetPlayerReplays()
    {
        return replayData.GetReplaysByPlayer(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public List<Replay> GetRecentReplays(int count = 10)
    {
        return replayData.GetRecentReplays(count);
    }
    
    public Replay GetReplay(string replayID)
    {
        return replayData.GetReplay(replayID);
    }
    
    public void SaveReplayData()
    {
        string path = Application.dataPath + "/Data/replay_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, replayData);
        stream.Close();
    }
    
    public void LoadReplayData()
    {
        string path = Application.dataPath + "/Data/replay_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            replayData = (ReplayManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            replayData = new ReplayManagerData();
        }
    }
}
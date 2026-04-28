using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SpectatorManager : MonoBehaviour
{
    public static SpectatorManager Instance { get; private set; }
    
    public SpectatorManagerData spectatorData;
    
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
        LoadSpectatorData();
        
        if (spectatorData == null)
        {
            spectatorData = new SpectatorManagerData();
        }
    }
    
    public Spectator JoinMatch(string matchID)
    {
        string spectatorID = "spectator_" + System.DateTime.Now.Ticks;
        string spectatorName = ProfileManager.Instance.currentProfile.playerName;
        
        Spectator spectator = new Spectator(spectatorID, spectatorName, matchID);
        spectatorData.AddSpectator(spectator);
        SaveSpectatorData();
        
        return spectator;
    }
    
    public void LeaveMatch(string spectatorID)
    {
        Spectator spectator = spectatorData.GetSpectator(spectatorID);
        if (spectator != null)
        {
            spectator.Leave();
            SaveSpectatorData();
        }
    }
    
    public List<Spectator> GetSpectatorsByMatch(string matchID)
    {
        return spectatorData.GetSpectatorsByMatch(matchID);
    }
    
    public int GetSpectatorCount(string matchID)
    {
        return spectatorData.GetSpectatorsByMatch(matchID).Count;
    }
    
    public void SaveSpectatorData()
    {
        string path = Application.dataPath + "/Data/spectator_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, spectatorData);
        stream.Close();
    }
    
    public void LoadSpectatorData()
    {
        string path = Application.dataPath + "/Data/spectator_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            spectatorData = (SpectatorManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            spectatorData = new SpectatorManagerData();
        }
    }
}
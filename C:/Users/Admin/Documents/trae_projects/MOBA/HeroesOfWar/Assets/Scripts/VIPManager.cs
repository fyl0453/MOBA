using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VIPManager : MonoBehaviour
{
    public static VIPManager Instance { get; private set; }
    
    public VIPManagerData vipData;
    
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
        LoadVIPData();
        
        if (vipData == null)
        {
            vipData = new VIPManagerData();
        }
        
        // 确保当前玩家有VIP数据
        EnsurePlayerVIPData();
    }
    
    private void EnsurePlayerVIPData()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        VIPUser vipUser = vipData.GetVIPUser(playerID);
        if (vipUser == null)
        {
            vipUser = new VIPUser(playerID);
            vipData.AddVIPUser(vipUser);
            SaveVIPData();
        }
    }
    
    public void AddVIPPoints(int points)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        VIPUser vipUser = vipData.GetVIPUser(playerID);
        if (vipUser != null)
        {
            vipUser.AddPoints(points);
            SaveVIPData();
        }
    }
    
    public VIP GetCurrentVIP()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        VIPUser vipUser = vipData.GetVIPUser(playerID);
        if (vipUser != null)
        {
            return vipUser.GetCurrentVIP();
        }
        return null;
    }
    
    public int GetVIPLevel()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        VIPUser vipUser = vipData.GetVIPUser(playerID);
        if (vipUser != null)
        {
            return vipUser.currentLevel;
        }
        return 0;
    }
    
    public int GetVIPPoints()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        VIPUser vipUser = vipData.GetVIPUser(playerID);
        if (vipUser != null)
        {
            return vipUser.currentPoints;
        }
        return 0;
    }
    
    public List<VIPPrivilege> GetVIPPrivileges()
    {
        VIP currentVIP = GetCurrentVIP();
        if (currentVIP != null)
        {
            return currentVIP.privileges;
        }
        return new List<VIPPrivilege>();
    }
    
    public void SaveVIPData()
    {
        string path = Application.dataPath + "/Data/vip_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, vipData);
        stream.Close();
    }
    
    public void LoadVIPData()
    {
        string path = Application.dataPath + "/Data/vip_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            vipData = (VIPManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            vipData = new VIPManagerData();
        }
    }
}
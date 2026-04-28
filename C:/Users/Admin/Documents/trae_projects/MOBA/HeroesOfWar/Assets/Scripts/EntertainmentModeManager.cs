using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EntertainmentModeManager : MonoBehaviour
{
    public static EntertainmentModeManager Instance { get; private set; }
    
    public EntertainmentModeManagerData entertainmentModeData;
    
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
        LoadEntertainmentModeData();
        
        if (entertainmentModeData == null)
        {
            entertainmentModeData = new EntertainmentModeManagerData();
            InitializeDefaultEntertainmentModes();
        }
    }
    
    private void InitializeDefaultEntertainmentModes()
    {
        // 创建娱乐模式
        EntertainmentMode mode1 = new EntertainmentMode("mode_chess", "王者模拟战", "自走棋模式，8名玩家对战", "Chess", 8, 20);
        EntertainmentMode mode2 = new EntertainmentMode("mode_border", "边境突围", "大逃杀模式，100名玩家对战", "BattleRoyale", 100, 30);
        EntertainmentMode mode3 = new EntertainmentMode("mode_fire", "火焰山大战", "娱乐模式，技能随机", "FireMountain", 10, 10);
        EntertainmentMode mode4 = new EntertainmentMode("mode_chaos", "无限乱斗", "娱乐模式，技能冷却缩短", "Chaos", 10, 15);
        EntertainmentMode mode5 = new EntertainmentMode("mode_clone", "克隆大作战", "娱乐模式，5名玩家使用相同英雄", "Clone", 10, 15);
        
        entertainmentModeData.system.AddMode(mode1);
        entertainmentModeData.system.AddMode(mode2);
        entertainmentModeData.system.AddMode(mode3);
        entertainmentModeData.system.AddMode(mode4);
        entertainmentModeData.system.AddMode(mode5);
        
        SaveEntertainmentModeData();
    }
    
    public string CreateModeMatch(string modeID, List<string> playerIDs, List<int> teamIDs, List<string> heroIDs)
    {
        EntertainmentMode mode = entertainmentModeData.system.GetMode(modeID);
        if (mode != null)
        {
            string matchID = System.Guid.NewGuid().ToString();
            ModeMatch newMatch = new ModeMatch(matchID, modeID, mode.modeName);
            
            for (int i = 0; i < playerIDs.Count; i++)
            {
                newMatch.AddPlayer(playerIDs[i], teamIDs[i], heroIDs[i]);
            }
            
            newMatch.StartMatch();
            entertainmentModeData.system.AddMatch(newMatch);
            SaveEntertainmentModeData();
            Debug.Log($"成功创建{mode.modeName}匹配: {matchID}");
            return matchID;
        }
        return "";
    }
    
    public void EndModeMatch(string matchID, string winnerTeamID)
    {
        ModeMatch match = entertainmentModeData.system.GetMatch(matchID);
        if (match != null)
        {
            match.EndMatch(winnerTeamID);
            SaveEntertainmentModeData();
            Debug.Log($"成功结束{match.modeName}匹配: {matchID}");
        }
    }
    
    public List<EntertainmentMode> GetActiveModes()
    {
        return entertainmentModeData.system.GetActiveModes();
    }
    
    public EntertainmentMode GetMode(string modeID)
    {
        return entertainmentModeData.system.GetMode(modeID);
    }
    
    public ModeMatch GetMatch(string matchID)
    {
        return entertainmentModeData.system.GetMatch(matchID);
    }
    
    public void ActivateMode(string modeID)
    {
        EntertainmentMode mode = entertainmentModeData.system.GetMode(modeID);
        if (mode != null)
        {
            mode.Activate();
            SaveEntertainmentModeData();
            Debug.Log($"成功激活娱乐模式: {mode.modeName}");
        }
    }
    
    public void DeactivateMode(string modeID)
    {
        EntertainmentMode mode = entertainmentModeData.system.GetMode(modeID);
        if (mode != null)
        {
            mode.Deactivate();
            SaveEntertainmentModeData();
            Debug.Log($"成功停用娱乐模式: {mode.modeName}");
        }
    }
    
    public void SaveEntertainmentModeData()
    {
        string path = Application.dataPath + "/Data/entertainment_mode_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, entertainmentModeData);
        stream.Close();
    }
    
    public void LoadEntertainmentModeData()
    {
        string path = Application.dataPath + "/Data/entertainment_mode_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            entertainmentModeData = (EntertainmentModeManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            entertainmentModeData = new EntertainmentModeManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BPManager : MonoBehaviour
{
    public static BPManager Instance { get; private set; }
    
    public BPManagerData bpData;
    
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
        LoadBPData();
        
        if (bpData == null)
        {
            bpData = new BPManagerData();
            InitializeDefaultBP();
        }
    }
    
    private void InitializeDefaultBP()
    {
        // 创建BP阶段
        BPPhase phase1 = new BPPhase("phase_ban_1", "第一轮禁用", "双方各禁用2个英雄", 1, 30, "Ban");
        BPPhase phase2 = new BPPhase("phase_pick_1", "第一轮选择", "蓝色方选择1个英雄，红色方选择2个英雄", 2, 30, "Pick");
        BPPhase phase3 = new BPPhase("phase_ban_2", "第二轮禁用", "双方各禁用2个英雄", 3, 30, "Ban");
        BPPhase phase4 = new BPPhase("phase_pick_2", "第二轮选择", "蓝色方选择2个英雄，红色方选择1个英雄", 4, 30, "Pick");
        
        bpData.system.AddPhase(phase1);
        bpData.system.AddPhase(phase2);
        bpData.system.AddPhase(phase3);
        bpData.system.AddPhase(phase4);
        
        // 创建默认BP会话
        BPSession session = new BPSession("session_1", "match_1");
        
        // 添加玩家
        BPPlayer player1 = new BPPlayer("user_1", "Player1", 1, 1);
        BPPlayer player2 = new BPPlayer("user_2", "Player2", 1, 2);
        BPPlayer player3 = new BPPlayer("user_3", "Player3", 1, 3);
        BPPlayer player4 = new BPPlayer("user_4", "Player4", 2, 1);
        BPPlayer player5 = new BPPlayer("user_5", "Player5", 2, 2);
        BPPlayer player6 = new BPPlayer("user_6", "Player6", 2, 3);
        
        session.AddPlayer(player1);
        session.AddPlayer(player2);
        session.AddPlayer(player3);
        session.AddPlayer(player4);
        session.AddPlayer(player5);
        session.AddPlayer(player6);
        
        bpData.system.AddSession(session);
        
        SaveBPData();
    }
    
    public string CreateBPSession(string matchID, List<BPPlayer> players)
    {
        string sessionID = System.Guid.NewGuid().ToString();
        BPSession newSession = new BPSession(sessionID, matchID);
        
        foreach (BPPlayer player in players)
        {
            newSession.AddPlayer(player);
        }
        
        bpData.system.AddSession(newSession);
        SaveBPData();
        Debug.Log($"成功创建BP会话: {sessionID}");
        return sessionID;
    }
    
    public void StartBPSession(string sessionID)
    {
        BPSession session = bpData.system.GetSession(sessionID);
        if (session != null)
        {
            session.StartSession();
            session.SetCurrentPhase(bpData.system.phases[0].phaseID, 0);
            SaveBPData();
            Debug.Log($"成功开始BP会话: {sessionID}");
        }
    }
    
    public void EndBPSession(string sessionID)
    {
        BPSession session = bpData.system.GetSession(sessionID);
        if (session != null)
        {
            session.EndSession();
            SaveBPData();
            Debug.Log($"成功结束BP会话: {sessionID}");
        }
    }
    
    public void PerformBPAction(string sessionID, string playerID, string actionType, string heroID, string heroName)
    {
        BPSession session = bpData.system.GetSession(sessionID);
        if (session != null)
        {
            string actionID = System.Guid.NewGuid().ToString();
            BPAction action = new BPAction(actionID, sessionID, playerID, actionType, heroID, heroName, session.currentPhaseIndex);
            session.AddAction(action);
            
            // 更新玩家的禁用/选择记录
            BPPlayer player = session.players.Find(p => p.playerID == playerID);
            if (player != null)
            {
                if (actionType == "Ban")
                {
                    player.BanHero(heroID);
                }
                else if (actionType == "Pick")
                {
                    player.PickHero(heroID);
                }
            }
            
            SaveBPData();
            Debug.Log($"成功执行BP动作: {actionType} {heroName}");
        }
    }
    
    public void NextPhase(string sessionID)
    {
        BPSession session = bpData.system.GetSession(sessionID);
        if (session != null)
        {
            session.currentPhaseIndex++;
            if (session.currentPhaseIndex < bpData.system.phases.Count)
            {
                session.SetCurrentPhase(bpData.system.phases[session.currentPhaseIndex].phaseID, session.currentPhaseIndex);
            }
            else
            {
                session.EndSession();
            }
            SaveBPData();
            Debug.Log($"成功进入下一阶段: {session.currentPhaseIndex}");
        }
    }
    
    public BPSession GetBPSession(string sessionID)
    {
        return bpData.system.GetSession(sessionID);
    }
    
    public List<BPAction> GetBPActions(string sessionID)
    {
        BPSession session = bpData.system.GetSession(sessionID);
        return session != null ? session.actions : new List<BPAction>();
    }
    
    public List<BPPlayer> GetBPSessionPlayers(string sessionID)
    {
        BPSession session = bpData.system.GetSession(sessionID);
        return session != null ? session.players : new List<BPPlayer>();
    }
    
    public BPPhase GetCurrentPhase(string sessionID)
    {
        BPSession session = bpData.system.GetSession(sessionID);
        if (session != null && !string.IsNullOrEmpty(session.currentPhaseID))
        {
            return bpData.system.GetPhase(session.currentPhaseID);
        }
        return null;
    }
    
    public List<string> GetBannedHeroes(string sessionID)
    {
        BPSession session = bpData.system.GetSession(sessionID);
        if (session != null)
        {
            List<string> bannedHeroes = new List<string>();
            foreach (BPPlayer player in session.players)
            {
                bannedHeroes.AddRange(player.bannedHeroes);
            }
            return bannedHeroes;
        }
        return new List<string>();
    }
    
    public List<string> GetPickedHeroes(string sessionID)
    {
        BPSession session = bpData.system.GetSession(sessionID);
        if (session != null)
        {
            List<string> pickedHeroes = new List<string>();
            foreach (BPPlayer player in session.players)
            {
                pickedHeroes.AddRange(player.pickedHeroes);
            }
            return pickedHeroes;
        }
        return new List<string>();
    }
    
    public void SaveBPData()
    {
        string path = Application.dataPath + "/Data/bp_system_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, bpData);
        stream.Close();
    }
    
    public void LoadBPData()
    {
        string path = Application.dataPath + "/Data/bp_system_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            bpData = (BPManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            bpData = new BPManagerData();
        }
    }
}
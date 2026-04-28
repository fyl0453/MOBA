using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }
    
    public MiniGameManagerData miniGameData;
    
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
        LoadMiniGameData();
        
        if (miniGameData == null)
        {
            miniGameData = new MiniGameManagerData();
            InitializeDefaultMiniGames();
        }
    }
    
    private void InitializeDefaultMiniGames()
    {
        // 创建默认小游戏
        MiniGame game1 = new MiniGame("mini_1", "猜英雄", "猜测对方选择的英雄", "Quiz", 2, 60);
        MiniGame game2 = new MiniGame("mini_2", "答题挑战", "回答关于游戏的问题", "Quiz", 2, 120);
        MiniGame game3 = new MiniGame("mini_3", "反应测试", "测试反应速度", "Reaction", 1, 30);
        MiniGame game4 = new MiniGame("mini_4", "记忆挑战", "记忆游戏中的物品位置", "Memory", 1, 60);
        
        miniGameData.system.AddMiniGame(game1);
        miniGameData.system.AddMiniGame(game2);
        miniGameData.system.AddMiniGame(game3);
        miniGameData.system.AddMiniGame(game4);
        
        SaveMiniGameData();
    }
    
    public string CreateChallenge(string senderID, string senderName, string receiverID, string receiverName, string gameID, int challengeScore)
    {
        MiniGame miniGame = miniGameData.system.GetMiniGame(gameID);
        if (miniGame != null)
        {
            string challengeID = System.Guid.NewGuid().ToString();
            MiniGameChallenge challenge = new MiniGameChallenge(challengeID, senderID, senderName, receiverID, receiverName, gameID, miniGame.gameName, challengeScore);
            miniGameData.system.AddChallenge(challenge);
            SaveMiniGameData();
            Debug.Log($"成功创建挑战: {miniGame.gameName}");
            return challengeID;
        }
        return "";
    }
    
    public void AcceptChallenge(string challengeID)
    {
        MiniGameChallenge challenge = miniGameData.system.GetChallenge(challengeID);
        if (challenge != null)
        {
            challenge.Accept();
            SaveMiniGameData();
            Debug.Log($"成功接受挑战: {challenge.gameName}");
        }
    }
    
    public void DeclineChallenge(string challengeID)
    {
        MiniGameChallenge challenge = miniGameData.system.GetChallenge(challengeID);
        if (challenge != null)
        {
            challenge.Decline();
            SaveMiniGameData();
            Debug.Log($"成功拒绝挑战: {challenge.gameName}");
        }
    }
    
    public void CompleteChallenge(string challengeID, string result)
    {
        MiniGameChallenge challenge = miniGameData.system.GetChallenge(challengeID);
        if (challenge != null)
        {
            challenge.Complete(result);
            SaveMiniGameData();
            Debug.Log($"成功完成挑战: {challenge.gameName}, 结果: {result}");
        }
    }
    
    public void RecordGame(string playerID, string playerName, string gameID, int score, bool isWin)
    {
        MiniGame miniGame = miniGameData.system.GetMiniGame(gameID);
        if (miniGame != null)
        {
            string recordID = System.Guid.NewGuid().ToString();
            MiniGameRecord record = new MiniGameRecord(recordID, playerID, playerName, gameID, miniGame.gameName, score, isWin);
            miniGameData.system.AddGameRecord(record);
            
            // 更新排名
            UpdateGameRankings(gameID);
            
            SaveMiniGameData();
            Debug.Log($"成功记录游戏成绩: {miniGame.gameName}, 分数: {score}");
        }
    }
    
    private void UpdateGameRankings(string gameID)
    {
        List<MiniGameRecord> records = miniGameData.system.GetGameRecordsByGame(gameID);
        records.Sort((a, b) => b.score.CompareTo(a.score));
        
        for (int i = 0; i < records.Count; i++)
        {
            records[i].rank = i + 1;
        }
    }
    
    public List<MiniGame> GetAllMiniGames()
    {
        return miniGameData.system.miniGames;
    }
    
    public MiniGame GetMiniGame(string gameID)
    {
        return miniGameData.system.GetMiniGame(gameID);
    }
    
    public List<MiniGameRecord> GetPlayerGameRecords(string playerID, int limit = 50)
    {
        List<MiniGameRecord> records = miniGameData.system.GetGameRecordsByPlayer(playerID);
        records.Sort((a, b) => b.playTime.CompareTo(a.playTime));
        return records.GetRange(0, Mathf.Min(limit, records.Count));
    }
    
    public List<MiniGameRecord> GetGameRankings(string gameID, int limit = 100)
    {
        List<MiniGameRecord> records = miniGameData.system.GetGameRecordsByGame(gameID);
        records.Sort((a, b) => b.score.CompareTo(a.score));
        return records.GetRange(0, Mathf.Min(limit, records.Count));
    }
    
    public List<MiniGameChallenge> GetPendingChallenges(string playerID)
    {
        return miniGameData.system.GetPendingChallenges(playerID);
    }
    
    public List<MiniGameChallenge> GetPlayerChallenges(string playerID, int limit = 50)
    {
        List<MiniGameChallenge> challenges = new List<MiniGameChallenge>();
        challenges.AddRange(miniGameData.system.challenges.FindAll(c => c.senderID == playerID || c.receiverID == playerID));
        challenges.Sort((a, b) => b.createTime.CompareTo(a.createTime));
        return challenges.GetRange(0, Mathf.Min(limit, challenges.Count));
    }
    
    public void AddMiniGame(string name, string description, string type, int maxPlayers, int playTime)
    {
        string gameID = System.Guid.NewGuid().ToString();
        MiniGame newGame = new MiniGame(gameID, name, description, type, maxPlayers, playTime);
        miniGameData.system.AddMiniGame(newGame);
        SaveMiniGameData();
        Debug.Log($"成功添加小游戏: {name}");
    }
    
    public void SaveMiniGameData()
    {
        string path = Application.dataPath + "/Data/mini_game_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, miniGameData);
        stream.Close();
    }
    
    public void LoadMiniGameData()
    {
        string path = Application.dataPath + "/Data/mini_game_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            miniGameData = (MiniGameManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            miniGameData = new MiniGameManagerData();
        }
    }
}
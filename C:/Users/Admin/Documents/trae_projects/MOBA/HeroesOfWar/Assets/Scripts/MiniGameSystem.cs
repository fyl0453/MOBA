[System.Serializable]
public class MiniGameSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<MiniGame> miniGames;
    public List<MiniGameRecord> gameRecords;
    public List<MiniGameChallenge> challenges;
    
    public MiniGameSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        miniGames = new List<MiniGame>();
        gameRecords = new List<MiniGameRecord>();
        challenges = new List<MiniGameChallenge>();
    }
    
    public void AddMiniGame(MiniGame miniGame)
    {
        miniGames.Add(miniGame);
    }
    
    public void AddGameRecord(MiniGameRecord record)
    {
        gameRecords.Add(record);
    }
    
    public void AddChallenge(MiniGameChallenge challenge)
    {
        challenges.Add(challenge);
    }
    
    public MiniGame GetMiniGame(string gameID)
    {
        return miniGames.Find(g => g.gameID == gameID);
    }
    
    public MiniGameRecord GetGameRecord(string recordID)
    {
        return gameRecords.Find(r => r.recordID == recordID);
    }
    
    public MiniGameChallenge GetChallenge(string challengeID)
    {
        return challenges.Find(c => c.challengeID == challengeID);
    }
    
    public List<MiniGameRecord> GetGameRecordsByPlayer(string playerID)
    {
        return gameRecords.FindAll(r => r.playerID == playerID);
    }
    
    public List<MiniGameRecord> GetGameRecordsByGame(string gameID)
    {
        return gameRecords.FindAll(r => r.gameID == gameID);
    }
    
    public List<MiniGameChallenge> GetPendingChallenges(string playerID)
    {
        return challenges.FindAll(c => c.receiverID == playerID && c.status == "Pending");
    }
}

[System.Serializable]
public class MiniGame
{
    public string gameID;
    public string gameName;
    public string gameDescription;
    public string gameType;
    public int maxPlayers;
    public int playTime;
    public string gameIcon;
    
    public MiniGame(string id, string name, string desc, string type, int maxPlayers, int playTime)
    {
        gameID = id;
        gameName = name;
        gameDescription = desc;
        gameType = type;
        this.maxPlayers = maxPlayers;
        this.playTime = playTime;
        gameIcon = "";
    }
}

[System.Serializable]
public class MiniGameRecord
{
    public string recordID;
    public string playerID;
    public string playerName;
    public string gameID;
    public string gameName;
    public int score;
    public int rank;
    public string playTime;
    public bool isWin;
    
    public MiniGameRecord(string id, string playerID, string playerName, string gameID, string gameName, int score, bool isWin)
    {
        recordID = id;
        this.playerID = playerID;
        this.playerName = playerName;
        this.gameID = gameID;
        this.gameName = gameName;
        this.score = score;
        rank = 0;
        this.playTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.isWin = isWin;
    }
}

[System.Serializable]
public class MiniGameChallenge
{
    public string challengeID;
    public string senderID;
    public string senderName;
    public string receiverID;
    public string receiverName;
    public string gameID;
    public string gameName;
    public int challengeScore;
    public string status;
    public string createTime;
    public string respondTime;
    public string result;
    
    public MiniGameChallenge(string id, string senderID, string senderName, string receiverID, string receiverName, string gameID, string gameName, int challengeScore)
    {
        challengeID = id;
        this.senderID = senderID;
        this.senderName = senderName;
        this.receiverID = receiverID;
        this.receiverName = receiverName;
        this.gameID = gameID;
        this.gameName = gameName;
        this.challengeScore = challengeScore;
        status = "Pending";
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        respondTime = "";
        result = "";
    }
    
    public void Accept()
    {
        status = "Accepted";
        respondTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Decline()
    {
        status = "Declined";
        respondTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Complete(string result)
    {
        status = "Completed";
        this.result = result;
    }
}

[System.Serializable]
public class MiniGameManagerData
{
    public MiniGameSystem system;
    
    public MiniGameManagerData()
    {
        system = new MiniGameSystem("mini_game_system", "好友互动小游戏系统", "管理好友之间的互动小游戏");
    }
}
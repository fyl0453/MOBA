[System.Serializable]
public class CustomRoom
{
    public string roomID;
    public string roomName;
    public string creatorID;
    public string creatorName;
    public string roomType;
    public int maxPlayers;
    public int currentPlayers;
    public string roomStatus;
    public List<RoomPlayer> players;
    public string password;
    public string createdAt;
    
    public CustomRoom(string id, string name, string creator, string creatorName, string type, int max)
    {
        roomID = id;
        roomName = name;
        this.creatorID = creator;
        this.creatorName = creatorName;
        roomType = type;
        maxPlayers = max;
        currentPlayers = 1;
        roomStatus = "Waiting";
        players = new List<RoomPlayer>();
        password = "";
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        
        // 添加创建者到房间
        RoomPlayer creatorPlayer = new RoomPlayer(creator, creatorName, true);
        players.Add(creatorPlayer);
    }
    
    public void AddPlayer(string playerID, string playerName)
    {
        if (currentPlayers < maxPlayers)
        {
            RoomPlayer player = new RoomPlayer(playerID, playerName, false);
            players.Add(player);
            currentPlayers++;
        }
    }
    
    public void RemovePlayer(string playerID)
    {
        players.RemoveAll(p => p.playerID == playerID);
        currentPlayers = players.Count;
    }
    
    public void SetPassword(string pass)
    {
        password = pass;
    }
    
    public void StartGame()
    {
        roomStatus = "Playing";
    }
    
    public void EndGame()
    {
        roomStatus = "Ended";
    }
    
    public bool IsFull()
    {
        return currentPlayers >= maxPlayers;
    }
}

[System.Serializable]
public class RoomPlayer
{
    public string playerID;
    public string playerName;
    public bool isCreator;
    public bool isReady;
    
    public RoomPlayer(string id, string name, bool creator)
    {
        playerID = id;
        playerName = name;
        isCreator = creator;
        isReady = false;
    }
    
    public void SetReady(bool ready)
    {
        isReady = ready;
    }
}

[System.Serializable]
public class CustomRoomManagerData
{
    public List<CustomRoom> rooms;
    
    public CustomRoomManagerData()
    {
        rooms = new List<CustomRoom>();
    }
    
    public void AddRoom(CustomRoom room)
    {
        rooms.Add(room);
    }
    
    public CustomRoom GetRoom(string roomID)
    {
        return rooms.Find(r => r.roomID == roomID);
    }
    
    public List<CustomRoom> GetActiveRooms()
    {
        return rooms.FindAll(r => r.roomStatus == "Waiting" || r.roomStatus == "Playing");
    }
    
    public List<CustomRoom> GetRoomsByCreator(string creatorID)
    {
        return rooms.FindAll(r => r.creatorID == creatorID);
    }
}
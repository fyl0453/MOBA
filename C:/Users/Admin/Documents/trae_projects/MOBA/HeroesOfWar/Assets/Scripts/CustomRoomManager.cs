using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CustomRoomManager : MonoBehaviour
{
    public static CustomRoomManager Instance { get; private set; }
    
    public CustomRoomManagerData roomData;
    
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
        LoadRoomData();
        
        if (roomData == null)
        {
            roomData = new CustomRoomManagerData();
        }
    }
    
    public CustomRoom CreateRoom(string roomName, string roomType, int maxPlayers, string password = "")
    {
        string roomID = "room_" + System.DateTime.Now.Ticks;
        string creatorID = ProfileManager.Instance.currentProfile.playerID;
        string creatorName = ProfileManager.Instance.currentProfile.playerName;
        
        CustomRoom room = new CustomRoom(roomID, roomName, creatorID, creatorName, roomType, maxPlayers);
        if (!string.IsNullOrEmpty(password))
        {
            room.SetPassword(password);
        }
        
        roomData.AddRoom(room);
        SaveRoomData();
        
        return room;
    }
    
    public bool JoinRoom(string roomID, string password = "")
    {
        CustomRoom room = roomData.GetRoom(roomID);
        if (room != null && room.roomStatus == "Waiting" && !room.IsFull())
        {
            if (!string.IsNullOrEmpty(room.password) && room.password != password)
            {
                return false;
            }
            
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            string playerName = ProfileManager.Instance.currentProfile.playerName;
            room.AddPlayer(playerID, playerName);
            SaveRoomData();
            return true;
        }
        return false;
    }
    
    public void LeaveRoom(string roomID)
    {
        CustomRoom room = roomData.GetRoom(roomID);
        if (room != null)
        {
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            room.RemovePlayer(playerID);
            
            if (room.currentPlayers == 0)
            {
                roomData.rooms.Remove(room);
            }
            else if (room.creatorID == playerID)
            {
                // 转让房主
                if (room.players.Count > 0)
                {
                    room.creatorID = room.players[0].playerID;
                    room.creatorName = room.players[0].playerName;
                    room.players[0].isCreator = true;
                }
            }
            
            SaveRoomData();
        }
    }
    
    public void StartGame(string roomID)
    {
        CustomRoom room = roomData.GetRoom(roomID);
        if (room != null && room.creatorID == ProfileManager.Instance.currentProfile.playerID)
        {
            room.StartGame();
            SaveRoomData();
        }
    }
    
    public void EndGame(string roomID)
    {
        CustomRoom room = roomData.GetRoom(roomID);
        if (room != null && room.creatorID == ProfileManager.Instance.currentProfile.playerID)
        {
            room.EndGame();
            SaveRoomData();
        }
    }
    
    public void SetPlayerReady(string roomID, bool ready)
    {
        CustomRoom room = roomData.GetRoom(roomID);
        if (room != null)
        {
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            RoomPlayer player = room.players.Find(p => p.playerID == playerID);
            if (player != null)
            {
                player.SetReady(ready);
                SaveRoomData();
            }
        }
    }
    
    public List<CustomRoom> GetActiveRooms()
    {
        return roomData.GetActiveRooms();
    }
    
    public CustomRoom GetRoom(string roomID)
    {
        return roomData.GetRoom(roomID);
    }
    
    public List<CustomRoom> GetCreatorRooms()
    {
        return roomData.GetRoomsByCreator(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public void SaveRoomData()
    {
        string path = Application.dataPath + "/Data/custom_room_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, roomData);
        stream.Close();
    }
    
    public void LoadRoomData()
    {
        string path = Application.dataPath + "/Data/custom_room_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            roomData = (CustomRoomManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            roomData = new CustomRoomManagerData();
        }
    }
}
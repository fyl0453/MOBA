using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class CustomRoom
{
    public string RoomID;
    public string RoomName;
    public string HostPlayerID;
    public string HostPlayerName;
    public string RoomPassword;
    public string MapName;
    public string GameMode;
    public int MaxPlayers;
    public int MinPlayers;
    public List<RoomPlayer> Players;
    public List<RoomSlot> Team1Slots;
    public List<RoomSlot> Team2Slots;
    public string RoomStatus;
    public DateTime CreatedTime;
    public DateTime StartTime;
    public bool IsPrivate;
    public string RoomSettings;
    public int PickMode;
    public int BanMode;
    public bool AllowSpectators;
    public int MaxSpectators;

    public CustomRoom(string roomName, string hostPlayerID, string hostPlayerName, string mapName, string gameMode, int maxPlayers, bool isPrivate, string password)
    {
        RoomID = "room_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RoomName = roomName;
        HostPlayerID = hostPlayerID;
        HostPlayerName = hostPlayerName;
        RoomPassword = password;
        MapName = mapName;
        GameMode = gameMode;
        MaxPlayers = maxPlayers;
        MinPlayers = 2;
        Players = new List<RoomPlayer>();
        Team1Slots = new List<RoomSlot>();
        Team2Slots = new List<RoomSlot>();
        RoomStatus = "waiting";
        CreatedTime = DateTime.Now;
        StartTime = DateTime.MinValue;
        IsPrivate = isPrivate;
        RoomSettings = "";
        PickMode = 0;
        BanMode = 0;
        AllowSpectators = true;
        MaxSpectators = 10;
    }
}

[Serializable]
public class RoomPlayer
{
    public string PlayerID;
    public string PlayerName;
    public string HeroID;
    public string HeroName;
    public string Team;
    public int SlotIndex;
    public bool IsReady;
    public bool IsHost;
    public DateTime JoinTime;
    public bool IsConnected;
    public string SkinID;

    public RoomPlayer(string playerID, string playerName, string team, int slotIndex)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        HeroID = "";
        HeroName = "";
        Team = team;
        SlotIndex = slotIndex;
        IsReady = false;
        IsHost = false;
        JoinTime = DateTime.Now;
        IsConnected = true;
        SkinID = "";
    }
}

[Serializable]
public class RoomSlot
{
    public int SlotIndex;
    public string Team;
    public string PlayerID;
    public string PlayerName;
    public bool IsOccupied;
    public bool IsLocked;
    public string SlotStatus;

    public RoomSlot(int slotIndex, string team)
    {
        SlotIndex = slotIndex;
        Team = team;
        PlayerID = "";
        PlayerName = "";
        IsOccupied = false;
        IsLocked = false;
        SlotStatus = "empty";
    }
}

[Serializable]
public class RoomInvite
{
    public string InviteID;
    public string RoomID;
    public string SenderPlayerID;
    public string SenderPlayerName;
    public string ReceiverPlayerID;
    public string ReceiverPlayerName;
    public DateTime InviteTime;
    public string InviteStatus;
    public DateTime ResponseTime;

    public RoomInvite(string roomID, string senderPlayerID, string senderPlayerName, string receiverPlayerID, string receiverPlayerName)
    {
        InviteID = "invite_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RoomID = roomID;
        SenderPlayerID = senderPlayerID;
        SenderPlayerName = senderPlayerName;
        ReceiverPlayerID = receiverPlayerID;
        ReceiverPlayerName = receiverPlayerName;
        InviteTime = DateTime.Now;
        InviteStatus = "pending";
        ResponseTime = DateTime.MinValue;
    }
}

[Serializable]
public class CustomRoomSystemData
{
    public List<CustomRoom> ActiveRooms;
    public List<CustomRoom> RecentRooms;
    public List<RoomInvite> RoomInvites;
    public List<string> AvailableMaps;
    public List<string> GameModes;
    public int MaxRooms;
    public int MaxPlayersPerRoom;
    public int RoomTimeoutMinutes;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public CustomRoomSystemData()
    {
        ActiveRooms = new List<CustomRoom>();
        RecentRooms = new List<CustomRoom>();
        RoomInvites = new List<RoomInvite>();
        AvailableMaps = new List<string> { "Summoner's Rift", "Howling Abyss", "Twisted Treeline" };
        GameModes = new List<string> { "classic", "ranked", "practice", "custom" };
        MaxRooms = 100;
        MaxPlayersPerRoom = 10;
        RoomTimeoutMinutes = 60;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }
}

[Serializable]
public class CustomRoomEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string RoomID;
    public string PlayerID;
    public string EventData;

    public CustomRoomEvent(string eventID, string eventType, string roomID, string playerID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        RoomID = roomID;
        PlayerID = playerID;
        EventData = eventData;
    }
}

public class CustomRoomSystemDataManager
{
    private static CustomRoomSystemDataManager _instance;
    public static CustomRoomSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CustomRoomSystemDataManager();
            }
            return _instance;
        }
    }

    public CustomRoomSystemData roomData;
    private List<CustomRoomEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private CustomRoomSystemDataManager()
    {
        roomData = new CustomRoomSystemData();
        recentEvents = new List<CustomRoomEvent>();
        LoadRoomData();
    }

    public void SaveRoomData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "CustomRoomSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, roomData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存自定义房间系统数据失败: " + e.Message);
        }
    }

    public void LoadRoomData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "CustomRoomSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    roomData = (CustomRoomSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载自定义房间系统数据失败: " + e.Message);
            roomData = new CustomRoomSystemData();
        }
    }

    public void CreateRoomEvent(string eventType, string roomID, string playerID, string eventData)
    {
        string eventID = "room_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CustomRoomEvent roomEvent = new CustomRoomEvent(eventID, eventType, roomID, playerID, eventData);
        recentEvents.Add(roomEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<CustomRoomEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomRoomSystemDetailedManager
{
    private static CustomRoomSystemDetailedManager _instance;
    public static CustomRoomSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CustomRoomSystemDetailedManager();
            }
            return _instance;
        }
    }

    private CustomRoomSystemData roomData;
    private CustomRoomSystemDataManager dataManager;

    private CustomRoomSystemDetailedManager()
    {
        dataManager = CustomRoomSystemDataManager.Instance;
        roomData = dataManager.roomData;
    }

    public string CreateRoom(string roomName, string hostPlayerID, string hostPlayerName, string mapName, string gameMode, int maxPlayers, bool isPrivate, string password)
    {
        if (roomData.ActiveRooms.Count >= roomData.MaxRooms)
        {
            Debug.LogError("房间数量已达上限");
            return "";
        }
        
        CustomRoom room = new CustomRoom(roomName, hostPlayerID, hostPlayerName, mapName, gameMode, maxPlayers, isPrivate, password);
        InitializeRoomSlots(room, maxPlayers);
        roomData.ActiveRooms.Add(room);
        
        dataManager.CreateRoomEvent("room_create", room.RoomID, hostPlayerID, "创建房间: " + roomName);
        dataManager.SaveRoomData();
        Debug.Log("创建房间成功: " + roomName);
        return room.RoomID;
    }

    private void InitializeRoomSlots(CustomRoom room, int maxPlayers)
    {
        int slotsPerTeam = maxPlayers / 2;
        for (int i = 0; i < slotsPerTeam; i++)
        {
            room.Team1Slots.Add(new RoomSlot(i, "team1"));
            room.Team2Slots.Add(new RoomSlot(i, "team2"));
        }
    }

    public bool JoinRoom(string roomID, string playerID, string playerName, string password)
    {
        CustomRoom room = roomData.ActiveRooms.Find(r => r.RoomID == roomID);
        if (room == null)
        {
            Debug.LogError("房间不存在");
            return false;
        }
        
        if (room.RoomStatus != "waiting")
        {
            Debug.LogError("房间已开始游戏");
            return false;
        }
        
        if (room.IsPrivate && room.RoomPassword != password)
        {
            Debug.LogError("密码错误");
            return false;
        }
        
        if (room.Players.Count >= room.MaxPlayers)
        {
            Debug.LogError("房间已满");
            return false;
        }
        
        RoomPlayer player = new RoomPlayer(playerID, playerName, "team1", room.Players.Count);
        room.Players.Add(player);
        
        if (playerID == room.HostPlayerID)
        {
            player.IsHost = true;
        }
        
        dataManager.CreateRoomEvent("player_join", roomID, playerID, "玩家加入: " + playerName);
        dataManager.SaveRoomData();
        Debug.Log("玩家加入成功: " + playerName);
        return true;
    }

    public bool LeaveRoom(string roomID, string playerID)
    {
        CustomRoom room = roomData.ActiveRooms.Find(r => r.RoomID == roomID);
        if (room == null)
        {
            return false;
        }
        
        RoomPlayer player = room.Players.Find(p => p.PlayerID == playerID);
        if (player != null)
        {
            room.Players.Remove(player);
            
            if (playerID == room.HostPlayerID)
            {
                if (room.Players.Count > 0)
                {
                    room.HostPlayerID = room.Players[0].PlayerID;
                    room.HostPlayerName = room.Players[0].PlayerName;
                    room.Players[0].IsHost = true;
                }
                else
                {
                    roomData.ActiveRooms.Remove(room);
                }
            }
            
            dataManager.CreateRoomEvent("player_leave", roomID, playerID, "玩家离开: " + player.PlayerName);
            dataManager.SaveRoomData();
            Debug.Log("玩家离开成功: " + player.PlayerName);
            return true;
        }
        
        return false;
    }

    public bool SetPlayerReady(string roomID, string playerID, bool ready)
    {
        CustomRoom room = roomData.ActiveRooms.Find(r => r.RoomID == roomID);
        if (room == null)
        {
            return false;
        }
        
        RoomPlayer player = room.Players.Find(p => p.PlayerID == playerID);
        if (player != null)
        {
            player.IsReady = ready;
            
            dataManager.CreateRoomEvent("player_ready", roomID, playerID, "玩家准备: " + ready);
            dataManager.SaveRoomData();
            return true;
        }
        
        return false;
    }

    public bool SwitchTeam(string roomID, string playerID)
    {
        CustomRoom room = roomData.ActiveRooms.Find(r => r.RoomID == roomID);
        if (room == null)
        {
            return false;
        }
        
        RoomPlayer player = room.Players.Find(p => p.PlayerID == playerID);
        if (player != null)
        {
            if (player.Team == "team1")
            {
                player.Team = "team2";
            }
            else
            {
                player.Team = "team1";
            }
            
            dataManager.CreateRoomEvent("player_switch_team", roomID, playerID, "切换队伍");
            dataManager.SaveRoomData();
            return true;
        }
        
        return false;
    }

    public bool SelectHero(string roomID, string playerID, string heroID, string heroName)
    {
        CustomRoom room = roomData.ActiveRooms.Find(r => r.RoomID == roomID);
        if (room == null)
        {
            return false;
        }
        
        RoomPlayer player = room.Players.Find(p => p.PlayerID == playerID);
        if (player != null)
        {
            player.HeroID = heroID;
            player.HeroName = heroName;
            
            dataManager.CreateRoomEvent("hero_select", roomID, playerID, "选择英雄: " + heroName);
            dataManager.SaveRoomData();
            return true;
        }
        
        return false;
    }

    public bool StartGame(string roomID, string playerID)
    {
        CustomRoom room = roomData.ActiveRooms.Find(r => r.RoomID == roomID);
        if (room == null)
        {
            return false;
        }
        
        if (playerID != room.HostPlayerID)
        {
            Debug.LogError("只有房主可以开始游戏");
            return false;
        }
        
        if (room.Players.Count < room.MinPlayers)
        {
            Debug.LogError("玩家数量不足");
            return false;
        }
        
        bool allReady = true;
        foreach (RoomPlayer player in room.Players)
        {
            if (!player.IsReady && player.PlayerID != room.HostPlayerID)
            {
                allReady = false;
                break;
            }
        }
        
        if (!allReady)
        {
            Debug.LogError("有玩家未准备");
            return false;
        }
        
        room.RoomStatus = "playing";
        room.StartTime = DateTime.Now;
        
        dataManager.CreateRoomEvent("game_start", roomID, playerID, "游戏开始");
        dataManager.SaveRoomData();
        Debug.Log("游戏开始成功");
        return true;
    }

    public string InvitePlayer(string roomID, string senderPlayerID, string senderPlayerName, string receiverPlayerID, string receiverPlayerName)
    {
        RoomInvite invite = new RoomInvite(roomID, senderPlayerID, senderPlayerName, receiverPlayerID, receiverPlayerName);
        roomData.RoomInvites.Add(invite);
        
        dataManager.CreateRoomEvent("player_invite", roomID, senderPlayerID, "邀请玩家: " + receiverPlayerName);
        dataManager.SaveRoomData();
        Debug.Log("邀请玩家成功: " + receiverPlayerName);
        return invite.InviteID;
    }

    public bool RespondToInvite(string inviteID, string playerID, bool accepted)
    {
        RoomInvite invite = roomData.RoomInvites.Find(i => i.InviteID == inviteID);
        if (invite == null)
        {
            return false;
        }
        
        if (invite.ReceiverPlayerID != playerID)
        {
            return false;
        }
        
        invite.InviteStatus = accepted ? "accepted" : "rejected";
        invite.ResponseTime = DateTime.Now;
        
        if (accepted)
        {
            JoinRoom(invite.RoomID, playerID, invite.ReceiverPlayerName, "");
        }
        
        dataManager.CreateRoomEvent("invite_response", invite.RoomID, playerID, "回应邀请: " + accepted);
        dataManager.SaveRoomData();
        Debug.Log("回应邀请成功: " + accepted);
        return true;
    }

    public List<CustomRoom> GetPublicRooms()
    {
        return roomData.ActiveRooms.FindAll(r => !r.IsPrivate && r.RoomStatus == "waiting");
    }

    public CustomRoom GetRoom(string roomID)
    {
        return roomData.ActiveRooms.Find(r => r.RoomID == roomID);
    }

    public List<RoomPlayer> GetRoomPlayers(string roomID)
    {
        CustomRoom room = roomData.ActiveRooms.Find(r => r.RoomID == roomID);
        if (room != null)
        {
            return room.Players;
        }
        return new List<RoomPlayer>();
    }

    public List<RoomInvite> GetPendingInvites(string playerID)
    {
        return roomData.RoomInvites.FindAll(i => i.ReceiverPlayerID == playerID && i.InviteStatus == "pending");
    }

    public List<string> GetAvailableMaps()
    {
        return roomData.AvailableMaps;
    }

    public List<string> GetGameModes()
    {
        return roomData.GameModes;
    }

    public void AddMap(string mapName)
    {
        if (!roomData.AvailableMaps.Contains(mapName))
        {
            roomData.AvailableMaps.Add(mapName);
            dataManager.SaveRoomData();
            Debug.Log("添加地图成功: " + mapName);
        }
    }

    public void AddGameMode(string gameMode)
    {
        if (!roomData.GameModes.Contains(gameMode))
        {
            roomData.GameModes.Add(gameMode);
            dataManager.SaveRoomData();
            Debug.Log("添加游戏模式成功: " + gameMode);
        }
    }

    public void CloseRoom(string roomID, string playerID)
    {
        CustomRoom room = roomData.ActiveRooms.Find(r => r.RoomID == roomID);
        if (room != null && room.HostPlayerID == playerID)
        {
            room.RoomStatus = "closed";
            roomData.ActiveRooms.Remove(room);
            roomData.RecentRooms.Add(room);
            
            dataManager.CreateRoomEvent("room_close", roomID, playerID, "关闭房间");
            dataManager.SaveRoomData();
            Debug.Log("关闭房间成功");
        }
    }

    public void CleanupInactiveRooms(int minutes = 60)
    {
        DateTime cutoff = DateTime.Now.AddMinutes(-minutes);
        List<CustomRoom> inactiveRooms = roomData.ActiveRooms.FindAll(r => r.RoomStatus == "waiting" && r.CreatedTime < cutoff);
        
        foreach (CustomRoom room in inactiveRooms)
        {
            roomData.ActiveRooms.Remove(room);
            roomData.RecentRooms.Add(room);
        }
        
        if (inactiveRooms.Count > 0)
        {
            dataManager.CreateRoomEvent("room_cleanup", "system", "", "清理不活跃房间: " + inactiveRooms.Count);
            dataManager.SaveRoomData();
            Debug.Log("清理不活跃房间成功: " + inactiveRooms.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveRoomData();
    }

    public void LoadData()
    {
        dataManager.LoadRoomData();
    }

    public List<CustomRoomEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
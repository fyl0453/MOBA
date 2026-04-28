using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FriendManager : MonoBehaviour
{
    public static FriendManager Instance { get; private set; }
    
    public FriendList friendList;
    
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
        LoadFriendData();
        
        if (friendList == null)
        {
            friendList = new FriendList("player_001");
            AddDefaultFriends();
        }
    }
    
    private void AddDefaultFriends()
    {
        friendList.AddFriend(new Friend("friend_001", "关羽", "Online"));
        friendList.AddFriend(new Friend("friend_002", "张飞", "Online"));
        friendList.AddFriend(new Friend("friend_003", "刘备", "Offline"));
        friendList.AddFriend(new Friend("friend_004", "赵云", "Away"));
        friendList.AddFriend(new Friend("friend_005", "诸葛亮", "Online"));
        
        SaveFriendData();
    }
    
    public void SendFriendRequest(string receiverID, string receiverName)
    {
        if (!friendList.IsFriend(receiverID) && !friendList.HasPendingRequest(receiverID))
        {
            FriendRequest request = new FriendRequest("request_" + System.DateTime.Now.Ticks, ProfileManager.Instance.currentProfile.playerID, receiverID);
            request.senderName = ProfileManager.Instance.currentProfile.playerName;
            friendList.AddOutgoingRequest(request);
            SaveFriendData();
        }
    }
    
    public void AcceptFriendRequest(string requestID)
    {
        FriendRequest request = friendList.incomingRequests.Find(r => r.requestID == requestID);
        if (request != null)
        {
            Friend newFriend = new Friend(request.senderID, request.senderName);
            friendList.AddFriend(newFriend);
            friendList.RemoveIncomingRequest(requestID);
            
            // 这里应该通知对方请求已接受
            SaveFriendData();
        }
    }
    
    public void DeclineFriendRequest(string requestID)
    {
        friendList.RemoveIncomingRequest(requestID);
        SaveFriendData();
    }
    
    public void RemoveFriend(string friendID)
    {
        friendList.RemoveFriend(friendID);
        SaveFriendData();
    }
    
    public void BlockFriend(string friendID)
    {
        Friend friend = friendList.friends.Find(f => f.friendID == friendID);
        if (friend != null)
        {
            friend.isBlocked = true;
            SaveFriendData();
        }
    }
    
    public void UnblockFriend(string friendID)
    {
        Friend friend = friendList.friends.Find(f => f.friendID == friendID);
        if (friend != null)
        {
            friend.isBlocked = false;
            SaveFriendData();
        }
    }
    
    public List<Friend> GetOnlineFriends()
    {
        return friendList.friends.FindAll(f => f.onlineStatus == "Online" && !f.isBlocked);
    }
    
    public List<Friend> GetOfflineFriends()
    {
        return friendList.friends.FindAll(f => f.onlineStatus == "Offline" && !f.isBlocked);
    }
    
    public List<Friend> GetAllFriends()
    {
        return friendList.friends.FindAll(f => !f.isBlocked);
    }
    
    public List<FriendRequest> GetIncomingRequests()
    {
        return friendList.incomingRequests;
    }
    
    public List<FriendRequest> GetOutgoingRequests()
    {
        return friendList.outgoingRequests;
    }
    
    public void UpdateFriendStatus(string friendID, string status)
    {
        Friend friend = friendList.friends.Find(f => f.friendID == friendID);
        if (friend != null)
        {
            friend.onlineStatus = status;
            if (status == "Offline")
            {
                friend.lastOnline = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
            SaveFriendData();
        }
    }
    
    public void AddFriendPoints(string friendID, int points)
    {
        Friend friend = friendList.friends.Find(f => f.friendID == friendID);
        if (friend != null)
        {
            friend.friendPoints += points;
            SaveFriendData();
        }
    }
    
    public void SaveFriendData()
    {
        string path = Application.dataPath + "/Data/friend_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, friendList);
        stream.Close();
    }
    
    public void LoadFriendData()
    {
        string path = Application.dataPath + "/Data/friend_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            friendList = (FriendList)formatter.Deserialize(stream);
            stream.Close();
        }
    }
}
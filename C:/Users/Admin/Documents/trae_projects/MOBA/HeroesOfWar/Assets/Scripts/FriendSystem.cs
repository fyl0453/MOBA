using UnityEngine;
using System.Collections.Generic;

public class FriendSystem : MonoBehaviour
{
    public static FriendSystem Instance { get; private set; }
    
    private Dictionary<string, List<FriendData>> friendLists = new Dictionary<string, List<FriendData>>();
    private Dictionary<string, List<FriendRequest>> friendRequests = new Dictionary<string, List<FriendRequest>>();
    
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
    
    public void AddFriend(string userID, string friendID, string friendName)
    {
        // 确保用户的好友列表存在
        if (!friendLists.ContainsKey(userID))
        {
            friendLists[userID] = new List<FriendData>();
        }
        
        // 检查是否已经是好友
        if (!IsFriend(userID, friendID))
        {
            FriendData friendData = new FriendData
            {
                friendID = friendID,
                friendName = friendName,
                friendSince = System.DateTime.Now.ToString(),
                lastOnline = System.DateTime.Now.ToString(),
                isOnline = true
            };
            
            friendLists[userID].Add(friendData);
            Debug.Log($"添加好友成功: {friendName}");
        }
    }
    
    public void RemoveFriend(string userID, string friendID)
    {
        if (friendLists.ContainsKey(userID))
        {
            FriendData friendData = friendLists[userID].Find(f => f.friendID == friendID);
            if (friendData != null)
            {
                friendLists[userID].Remove(friendData);
                Debug.Log($"移除好友成功: {friendData.friendName}");
            }
        }
    }
    
    public bool IsFriend(string userID, string friendID)
    {
        if (friendLists.ContainsKey(userID))
        {
            return friendLists[userID].Exists(f => f.friendID == friendID);
        }
        return false;
    }
    
    public List<FriendData> GetFriends(string userID)
    {
        if (friendLists.ContainsKey(userID))
        {
            return friendLists[userID];
        }
        return new List<FriendData>();
    }
    
    public void SendFriendRequest(string senderID, string senderName, string receiverID)
    {
        // 确保接收者的好友请求列表存在
        if (!friendRequests.ContainsKey(receiverID))
        {
            friendRequests[receiverID] = new List<FriendRequest>();
        }
        
        FriendRequest request = new FriendRequest
        {
            requestID = System.Guid.NewGuid().ToString(),
            senderID = senderID,
            senderName = senderName,
            receiverID = receiverID,
            sendTime = System.DateTime.Now.ToString(),
            status = FriendRequestStatus.Pending
        };
        
        friendRequests[receiverID].Add(request);
        Debug.Log($"发送好友请求成功: {senderName} -> {receiverID}");
    }
    
    public void AcceptFriendRequest(string requestID, string receiverID)
    {
        if (friendRequests.ContainsKey(receiverID))
        {
            FriendRequest request = friendRequests[receiverID].Find(r => r.requestID == requestID);
            if (request != null && request.status == FriendRequestStatus.Pending)
            {
                // 接受好友请求
                request.status = FriendRequestStatus.Accepted;
                
                // 添加好友关系
                AddFriend(receiverID, request.senderID, request.senderName);
                AddFriend(request.senderID, receiverID, AccountManager.Instance.currentUsername);
                
                Debug.Log($"接受好友请求成功: {request.senderName}");
            }
        }
    }
    
    public void RejectFriendRequest(string requestID, string receiverID)
    {
        if (friendRequests.ContainsKey(receiverID))
        {
            FriendRequest request = friendRequests[receiverID].Find(r => r.requestID == requestID);
            if (request != null && request.status == FriendRequestStatus.Pending)
            {
                request.status = FriendRequestStatus.Rejected;
                Debug.Log($"拒绝好友请求成功: {request.senderName}");
            }
        }
    }
    
    public List<FriendRequest> GetFriendRequests(string userID)
    {
        if (friendRequests.ContainsKey(userID))
        {
            return friendRequests[userID].FindAll(r => r.status == FriendRequestStatus.Pending);
        }
        return new List<FriendRequest>();
    }
    
    public void UpdateFriendStatus(string friendID, bool isOnline)
    {
        // 更新所有用户的好友状态
        foreach (var entry in friendLists)
        {
            FriendData friendData = entry.Value.Find(f => f.friendID == friendID);
            if (friendData != null)
            {
                friendData.isOnline = isOnline;
                if (isOnline)
                {
                    friendData.lastOnline = System.DateTime.Now.ToString();
                }
            }
        }
    }
}

[System.Serializable]
public class FriendData
{
    public string friendID;
    public string friendName;
    public string friendSince;
    public string lastOnline;
    public bool isOnline;
}

[System.Serializable]
public class FriendRequest
{
    public string requestID;
    public string senderID;
    public string senderName;
    public string receiverID;
    public string sendTime;
    public FriendRequestStatus status;
}

public enum FriendRequestStatus
{
    Pending,
    Accepted,
    Rejected
}

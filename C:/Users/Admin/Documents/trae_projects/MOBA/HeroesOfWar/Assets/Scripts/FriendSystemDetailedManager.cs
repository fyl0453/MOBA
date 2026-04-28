using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FriendSystemDetailedManager : MonoBehaviour
{
    public static FriendSystemDetailedManager Instance { get; private set; }
    
    public FriendSystemDetailedManagerData friendData;
    
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
        
        if (friendData == null)
        {
            friendData = new FriendSystemDetailedManagerData();
            InitializeDefaultFriendSystem();
        }
    }
    
    private void InitializeDefaultFriendSystem()
    {
        // 好友分组
        FriendGroup group1 = new FriendGroup("group_001", "user_001", "默认分组", "默认好友分组", 1, true);
        FriendGroup group2 = new FriendGroup("group_002", "user_001", "游戏好友", "一起玩游戏的好友", 2, false);
        FriendGroup group3 = new FriendGroup("group_003", "user_002", "默认分组", "默认好友分组", 1, true);
        
        friendData.system.AddFriendGroup(group1);
        friendData.system.AddFriendGroup(group2);
        friendData.system.AddFriendGroup(group3);
        
        // 好友
        Friend friend1 = new Friend("friend_001", "user_001", "user_002", "李四", "avatar_002", "friend");
        friend1.SetGroup("group_001");
        group1.AddFriend(friend1.friendID);
        
        Friend friend2 = new Friend("friend_002", "user_001", "user_003", "王五", "avatar_003", "friend");
        friend2.SetGroup("group_002");
        group2.AddFriend(friend2.friendID);
        
        Friend friend3 = new Friend("friend_003", "user_002", "user_001", "张三", "avatar_001", "friend");
        friend3.SetGroup("group_003");
        group3.AddFriend(friend3.friendID);
        
        friendData.system.AddFriend(friend1);
        friendData.system.AddFriend(friend2);
        friendData.system.AddFriend(friend3);
        
        // 好友请求
        FriendRequest request1 = new FriendRequest("request_001", "user_004", "赵六", "avatar_004", "user_001", "一起玩游戏吧");
        FriendRequest request2 = new FriendRequest("request_002", "user_001", "张三", "avatar_001", "user_005", "加个好友吧");
        
        friendData.system.AddFriendRequest(request1);
        friendData.system.AddFriendRequest(request2);
        
        // 好友互动
        FriendInteraction interaction1 = new FriendInteraction("interaction_001", "user_001", "user_002", "chat", "你好，一起玩游戏吗？");
        FriendInteraction interaction2 = new FriendInteraction("interaction_002", "user_002", "user_001", "chat", "好的，马上来");
        FriendInteraction interaction3 = new FriendInteraction("interaction_003", "user_001", "user_002", "gift", "送你一个皮肤碎片");
        
        friendData.system.AddFriendInteraction(interaction1);
        friendData.system.AddFriendInteraction(interaction2);
        friendData.system.AddFriendInteraction(interaction3);
        
        // 好友事件
        FriendEvent event1 = new FriendEvent("event_001", "add", "user_001", "user_002", "添加好友");
        FriendEvent event2 = new FriendEvent("event_002", "chat", "user_001", "user_002", "聊天");
        FriendEvent event3 = new FriendEvent("event_003", "gift", "user_001", "user_002", "赠送礼物");
        
        friendData.system.AddFriendEvent(event1);
        friendData.system.AddFriendEvent(event2);
        friendData.system.AddFriendEvent(event3);
        
        SaveFriendData();
    }
    
    // 好友分组管理
    public void AddFriendGroup(string userID, string groupName, string description, int order, bool isDefault)
    {
        string groupID = "group_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        FriendGroup friendGroup = new FriendGroup(groupID, userID, groupName, description, order, isDefault);
        friendData.system.AddFriendGroup(friendGroup);
        SaveFriendData();
        Debug.Log("成功添加好友分组: " + groupName);
    }
    
    public List<FriendGroup> GetFriendGroups(string userID)
    {
        return friendData.system.GetFriendGroupsByUser(userID);
    }
    
    public void UpdateFriendGroupOrder(string groupID, int order)
    {
        FriendGroup friendGroup = friendData.system.GetFriendGroup(groupID);
        if (friendGroup != null)
        {
            friendGroup.SetOrder(order);
            SaveFriendData();
            Debug.Log("成功更新好友分组顺序: " + order);
        }
        else
        {
            Debug.LogError("好友分组不存在: " + groupID);
        }
    }
    
    // 好友请求管理
    public string SendFriendRequest(string senderID, string senderName, string senderAvatar, string receiverID, string message)
    {
        // 检查是否已经是好友
        List<Friend> friends = friendData.system.GetFriendsByUser(senderID);
        if (friends.Exists(f => f.friendUserID == receiverID))
        {
            Debug.LogError("已经是好友");
            return "";
        }
        
        // 检查是否已经有未处理的请求
        List<FriendRequest> requests = friendData.system.GetFriendRequestsByUser(receiverID);
        if (requests.Exists(r => r.senderID == senderID && r.status == "pending"))
        {
            Debug.LogError("已经发送过请求");
            return "";
        }
        
        string requestID = "request_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        FriendRequest friendRequest = new FriendRequest(requestID, senderID, senderName, senderAvatar, receiverID, message);
        friendData.system.AddFriendRequest(friendRequest);
        SaveFriendData();
        Debug.Log("成功发送好友请求");
        return requestID;
    }
    
    public void AcceptFriendRequest(string requestID)
    {
        FriendRequest friendRequest = friendData.system.GetFriendRequest(requestID);
        if (friendRequest != null && friendRequest.status == "pending")
        {
            friendRequest.Accept();
            
            // 添加好友关系
            string friendID1 = "friend_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            Friend friend1 = new Friend(friendID1, friendRequest.receiverID, friendRequest.senderID, friendRequest.senderName, friendRequest.senderAvatar, "friend");
            
            string friendID2 = "friend_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            Friend friend2 = new Friend(friendID2, friendRequest.senderID, friendRequest.receiverID, "", "", "friend");
            
            friendData.system.AddFriend(friend1);
            friendData.system.AddFriend(friend2);
            
            // 添加到默认分组
            List<FriendGroup> groups1 = friendData.system.GetFriendGroupsByUser(friendRequest.receiverID);
            FriendGroup defaultGroup1 = groups1.Find(g => g.isDefault);
            if (defaultGroup1 != null)
            {
                friend1.SetGroup(defaultGroup1.groupID);
                defaultGroup1.AddFriend(friend1.friendID);
            }
            
            List<FriendGroup> groups2 = friendData.system.GetFriendGroupsByUser(friendRequest.senderID);
            FriendGroup defaultGroup2 = groups2.Find(g => g.isDefault);
            if (defaultGroup2 != null)
            {
                friend2.SetGroup(defaultGroup2.groupID);
                defaultGroup2.AddFriend(friend2.friendID);
            }
            
            // 创建好友事件
            CreateFriendEvent("add", friendRequest.receiverID, friendRequest.senderID, "添加好友");
            CreateFriendEvent("add", friendRequest.senderID, friendRequest.receiverID, "添加好友");
            
            SaveFriendData();
            Debug.Log("成功接受好友请求");
        }
        else
        {
            Debug.LogError("好友请求不存在或已处理");
        }
    }
    
    public void RejectFriendRequest(string requestID)
    {
        FriendRequest friendRequest = friendData.system.GetFriendRequest(requestID);
        if (friendRequest != null && friendRequest.status == "pending")
        {
            friendRequest.Reject();
            SaveFriendData();
            Debug.Log("成功拒绝好友请求");
        }
        else
        {
            Debug.LogError("好友请求不存在或已处理");
        }
    }
    
    public void CancelFriendRequest(string requestID)
    {
        FriendRequest friendRequest = friendData.system.GetFriendRequest(requestID);
        if (friendRequest != null && friendRequest.status == "pending")
        {
            friendRequest.Cancel();
            SaveFriendData();
            Debug.Log("成功取消好友请求");
        }
        else
        {
            Debug.LogError("好友请求不存在或已处理");
        }
    }
    
    public List<FriendRequest> GetFriendRequests(string userID)
    {
        return friendData.system.GetFriendRequestsByUser(userID);
    }
    
    // 好友管理
    public void RemoveFriend(string userID, string friendUserID)
    {
        List<Friend> friends = friendData.system.GetFriendsByUser(userID);
        Friend friend = friends.Find(f => f.friendUserID == friendUserID);
        if (friend != null)
        {
            // 从分组中移除
            if (!string.IsNullOrEmpty(friend.groupID))
            {
                FriendGroup group = friendData.system.GetFriendGroup(friend.groupID);
                if (group != null)
                {
                    group.RemoveFriend(friend.friendID);
                }
            }
            
            // 移除好友关系
            friendData.system.friends.Remove(friend);
            
            // 移除对方的好友关系
            List<Friend> friendFriends = friendData.system.GetFriendsByUser(friendUserID);
            Friend friend2 = friendFriends.Find(f => f.friendUserID == userID);
            if (friend2 != null)
            {
                if (!string.IsNullOrEmpty(friend2.groupID))
                {
                    FriendGroup group = friendData.system.GetFriendGroup(friend2.groupID);
                    if (group != null)
                    {
                        group.RemoveFriend(friend2.friendID);
                    }
                }
                friendData.system.friends.Remove(friend2);
            }
            
            // 创建好友事件
            CreateFriendEvent("remove", userID, friendUserID, "删除好友");
            CreateFriendEvent("remove", friendUserID, userID, "删除好友");
            
            SaveFriendData();
            Debug.Log("成功删除好友");
        }
        else
        {
            Debug.LogError("好友不存在");
        }
    }
    
    public void UpdateFriendStatus(string userID, string friendUserID, string status)
    {
        List<Friend> friends = friendData.system.GetFriendsByUser(userID);
        Friend friend = friends.Find(f => f.friendUserID == friendUserID);
        if (friend != null)
        {
            friend.UpdateStatus(status);
            SaveFriendData();
            Debug.Log("成功更新好友状态: " + status);
        }
        else
        {
            Debug.LogError("好友不存在");
        }
    }
    
    public void UpdateFriendGroup(string userID, string friendUserID, string groupID)
    {
        List<Friend> friends = friendData.system.GetFriendsByUser(userID);
        Friend friend = friends.Find(f => f.friendUserID == friendUserID);
        if (friend != null)
        {
            // 从旧分组中移除
            if (!string.IsNullOrEmpty(friend.groupID))
            {
                FriendGroup oldGroup = friendData.system.GetFriendGroup(friend.groupID);
                if (oldGroup != null)
                {
                    oldGroup.RemoveFriend(friend.friendID);
                }
            }
            
            // 添加到新分组
            FriendGroup newGroup = friendData.system.GetFriendGroup(groupID);
            if (newGroup != null && newGroup.userID == userID)
            {
                friend.SetGroup(groupID);
                newGroup.AddFriend(friend.friendID);
                SaveFriendData();
                Debug.Log("成功更新好友分组");
            }
            else
            {
                Debug.LogError("分组不存在或不属于该用户");
            }
        }
        else
        {
            Debug.LogError("好友不存在");
        }
    }
    
    public List<Friend> GetFriends(string userID)
    {
        return friendData.system.GetFriendsByUser(userID);
    }
    
    // 好友互动
    public void SendChatMessage(string userID, string friendUserID, string content)
    {
        List<Friend> friends = friendData.system.GetFriendsByUser(userID);
        Friend friend = friends.Find(f => f.friendUserID == friendUserID);
        if (friend != null)
        {
            string interactionID = "interaction_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            FriendInteraction interaction = new FriendInteraction(interactionID, userID, friendUserID, "chat", content);
            friendData.system.AddFriendInteraction(interaction);
            
            // 更新亲密度
            friend.UpdateIntimacy(1);
            
            // 更新最后互动时间
            friend.UpdateLastInteractionTime();
            
            // 创建好友事件
            CreateFriendEvent("chat", userID, friendUserID, "聊天");
            
            SaveFriendData();
            Debug.Log("成功发送聊天消息");
        }
        else
        {
            Debug.LogError("好友不存在");
        }
    }
    
    public void SendGift(string userID, string friendUserID, string giftName, int quantity)
    {
        List<Friend> friends = friendData.system.GetFriendsByUser(userID);
        Friend friend = friends.Find(f => f.friendUserID == friendUserID);
        if (friend != null)
        {
            string interactionID = "interaction_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            FriendInteraction interaction = new FriendInteraction(interactionID, userID, friendUserID, "gift", giftName + " x" + quantity);
            friendData.system.AddFriendInteraction(interaction);
            
            // 更新亲密度
            friend.UpdateIntimacy(5);
            
            // 更新最后互动时间
            friend.UpdateLastInteractionTime();
            
            // 创建好友事件
            CreateFriendEvent("gift", userID, friendUserID, "赠送礼物: " + giftName);
            
            SaveFriendData();
            Debug.Log("成功发送礼物: " + giftName);
        }
        else
        {
            Debug.LogError("好友不存在");
        }
    }
    
    public void InviteToGame(string userID, string friendUserID, string gameMode)
    {
        List<Friend> friends = friendData.system.GetFriendsByUser(userID);
        Friend friend = friends.Find(f => f.friendUserID == friendUserID);
        if (friend != null)
        {
            string interactionID = "interaction_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            FriendInteraction interaction = new FriendInteraction(interactionID, userID, friendUserID, "invite", "邀请你玩" + gameMode);
            friendData.system.AddFriendInteraction(interaction);
            
            // 更新亲密度
            friend.UpdateIntimacy(2);
            
            // 更新最后互动时间
            friend.UpdateLastInteractionTime();
            
            // 创建好友事件
            CreateFriendEvent("invite", userID, friendUserID, "邀请游戏: " + gameMode);
            
            SaveFriendData();
            Debug.Log("成功邀请好友游戏: " + gameMode);
        }
        else
        {
            Debug.LogError("好友不存在");
        }
    }
    
    public List<FriendInteraction> GetFriendInteractions(string userID)
    {
        return friendData.system.GetFriendInteractionsByUser(userID);
    }
    
    // 好友事件管理
    public string CreateFriendEvent(string eventType, string userID, string friendID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        FriendEvent friendEvent = new FriendEvent(eventID, eventType, userID, friendID, description);
        friendData.system.AddFriendEvent(friendEvent);
        SaveFriendData();
        Debug.Log("成功创建好友事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        FriendEvent friendEvent = friendData.system.GetFriendEvent(eventID);
        if (friendEvent != null)
        {
            friendEvent.MarkAsCompleted();
            SaveFriendData();
            Debug.Log("成功标记好友事件为完成");
        }
        else
        {
            Debug.LogError("好友事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        FriendEvent friendEvent = friendData.system.GetFriendEvent(eventID);
        if (friendEvent != null)
        {
            friendEvent.MarkAsFailed();
            SaveFriendData();
            Debug.Log("成功标记好友事件为失败");
        }
        else
        {
            Debug.LogError("好友事件不存在: " + eventID);
        }
    }
    
    public List<FriendEvent> GetFriendEvents(string userID)
    {
        return friendData.system.GetFriendEventsByUser(userID);
    }
    
    // 数据持久化
    public void SaveFriendData()
    {
        string path = Application.dataPath + "/Data/friend_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, friendData);
        stream.Close();
    }
    
    public void LoadFriendData()
    {
        string path = Application.dataPath + "/Data/friend_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            friendData = (FriendSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            friendData = new FriendSystemDetailedManagerData();
        }
    }
}
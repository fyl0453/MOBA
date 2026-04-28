[System.Serializable]
public class FriendSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Friend> friends;
    public List<FriendRequest> friendRequests;
    public List<FriendInteraction> friendInteractions;
    public List<FriendGroup> friendGroups;
    public List<FriendEvent> friendEvents;
    
    public FriendSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        friends = new List<Friend>();
        friendRequests = new List<FriendRequest>();
        friendInteractions = new List<FriendInteraction>();
        friendGroups = new List<FriendGroup>();
        friendEvents = new List<FriendEvent>();
    }
    
    public void AddFriend(Friend friend)
    {
        friends.Add(friend);
    }
    
    public void AddFriendRequest(FriendRequest friendRequest)
    {
        friendRequests.Add(friendRequest);
    }
    
    public void AddFriendInteraction(FriendInteraction friendInteraction)
    {
        friendInteractions.Add(friendInteraction);
    }
    
    public void AddFriendGroup(FriendGroup friendGroup)
    {
        friendGroups.Add(friendGroup);
    }
    
    public void AddFriendEvent(FriendEvent friendEvent)
    {
        friendEvents.Add(friendEvent);
    }
    
    public Friend GetFriend(string friendID)
    {
        return friends.Find(f => f.friendID == friendID);
    }
    
    public FriendRequest GetFriendRequest(string requestID)
    {
        return friendRequests.Find(fr => fr.requestID == requestID);
    }
    
    public FriendInteraction GetFriendInteraction(string interactionID)
    {
        return friendInteractions.Find(fi => fi.interactionID == interactionID);
    }
    
    public FriendGroup GetFriendGroup(string groupID)
    {
        return friendGroups.Find(fg => fg.groupID == groupID);
    }
    
    public FriendEvent GetFriendEvent(string eventID)
    {
        return friendEvents.Find(fe => fe.eventID == eventID);
    }
    
    public List<Friend> GetFriendsByUser(string userID)
    {
        return friends.FindAll(f => f.userID == userID);
    }
    
    public List<FriendRequest> GetFriendRequestsByUser(string userID)
    {
        return friendRequests.FindAll(fr => fr.receiverID == userID);
    }
    
    public List<FriendInteraction> GetFriendInteractionsByUser(string userID)
    {
        return friendInteractions.FindAll(fi => fi.userID == userID || fi.friendID == userID);
    }
    
    public List<FriendGroup> GetFriendGroupsByUser(string userID)
    {
        return friendGroups.FindAll(fg => fg.userID == userID);
    }
    
    public List<FriendEvent> GetFriendEventsByUser(string userID)
    {
        return friendEvents.FindAll(fe => fe.userID == userID || fe.friendID == userID);
    }
}

[System.Serializable]
public class Friend
{
    public string friendID;
    public string userID;
    public string friendUserID;
    public string friendUserName;
    public string friendUserAvatar;
    public string status;
    public string relationType;
    public int intimacyLevel;
    public int intimacyPoints;
    public string addTime;
    public string lastInteractionTime;
    public string groupID;
    
    public Friend(string id, string userID, string friendUserID, string friendUserName, string friendUserAvatar, string relationType)
    {
        friendID = id;
        this.userID = userID;
        this.friendUserID = friendUserID;
        this.friendUserName = friendUserName;
        this.friendUserAvatar = friendUserAvatar;
        status = "online";
        this.relationType = relationType;
        intimacyLevel = 1;
        intimacyPoints = 0;
        addTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastInteractionTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        groupID = "";
    }
    
    public void UpdateStatus(string status)
    {
        this.status = status;
    }
    
    public void UpdateIntimacy(int points)
    {
        intimacyPoints += points;
        if (intimacyPoints >= 100)
        {
            intimacyLevel++;
            intimacyPoints = 0;
        }
        lastInteractionTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void SetGroup(string groupID)
    {
        this.groupID = groupID;
    }
    
    public void UpdateLastInteractionTime()
    {
        lastInteractionTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class FriendRequest
{
    public string requestID;
    public string senderID;
    public string senderName;
    public string senderAvatar;
    public string receiverID;
    public string status;
    public string requestTime;
    public string responseTime;
    public string message;
    
    public FriendRequest(string id, string senderID, string senderName, string senderAvatar, string receiverID, string message)
    {
        requestID = id;
        this.senderID = senderID;
        this.senderName = senderName;
        this.senderAvatar = senderAvatar;
        this.receiverID = receiverID;
        status = "pending";
        requestTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        responseTime = "";
        this.message = message;
    }
    
    public void Accept()
    {
        status = "accepted";
        responseTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Reject()
    {
        status = "rejected";
        responseTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Cancel()
    {
        status = "cancelled";
        responseTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class FriendInteraction
{
    public string interactionID;
    public string userID;
    public string friendID;
    public string interactionType;
    public string content;
    public string timestamp;
    public string status;
    
    public FriendInteraction(string id, string userID, string friendID, string interactionType, string content)
    {
        interactionID = id;
        this.userID = userID;
        this.friendID = friendID;
        this.interactionType = interactionType;
        this.content = content;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
    }
    
    public void MarkAsCompleted()
    {
        status = "completed";
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class FriendGroup
{
    public string groupID;
    public string userID;
    public string groupName;
    public string description;
    public int order;
    public bool isDefault;
    public List<string> friendIDs;
    
    public FriendGroup(string id, string userID, string groupName, string description, int order, bool isDefault)
    {
        groupID = id;
        this.userID = userID;
        this.groupName = groupName;
        this.description = description;
        this.order = order;
        this.isDefault = isDefault;
        friendIDs = new List<string>();
    }
    
    public void AddFriend(string friendID)
    {
        if (!friendIDs.Contains(friendID))
        {
            friendIDs.Add(friendID);
        }
    }
    
    public void RemoveFriend(string friendID)
    {
        if (friendIDs.Contains(friendID))
        {
            friendIDs.Remove(friendID);
        }
    }
    
    public void SetOrder(int order)
    {
        this.order = order;
    }
}

[System.Serializable]
public class FriendEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string friendID;
    public string description;
    public string timestamp;
    public string status;
    
    public FriendEvent(string id, string eventType, string userID, string friendID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.friendID = friendID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
    }
    
    public void MarkAsCompleted()
    {
        status = "completed";
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class FriendSystemDetailedManagerData
{
    public FriendSystemDetailed system;
    
    public FriendSystemDetailedManagerData()
    {
        system = new FriendSystemDetailed("friend_system_detailed", "好友系统详细", "管理好友的详细功能，包括好友添加、删除和互动");
    }
}
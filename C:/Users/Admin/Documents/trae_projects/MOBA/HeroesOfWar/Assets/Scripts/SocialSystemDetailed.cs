using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Friend
{
    public string FriendID;
    public string PlayerID;
    public string FriendPlayerID;
    public string FriendName;
    public int IntimacyLevel;
    public int IntimacyPoints;
    public DateTime AddTime;
    public DateTime LastInteractionTime;
    public bool IsOnline;
    public string LastOnlineTime;
    public int RelationshipType;

    public Friend(string friendID, string playerID, string friendPlayerID, string friendName)
    {
        FriendID = friendID;
        PlayerID = playerID;
        FriendPlayerID = friendPlayerID;
        FriendName = friendName;
        IntimacyLevel = 1;
        IntimacyPoints = 0;
        AddTime = DateTime.Now;
        LastInteractionTime = DateTime.Now;
        IsOnline = false;
        LastOnlineTime = "";
        RelationshipType = 0;
    }
}

[Serializable]
public class ChatMessage
{
    public string MessageID;
    public string SenderID;
    public string SenderName;
    public string ReceiverID;
    public string Content;
    public int MessageType;
    public DateTime SendTime;
    public bool IsRead;
    public string Channel;

    public ChatMessage(string messageID, string senderID, string senderName, string receiverID, string content, int messageType, string channel)
    {
        MessageID = messageID;
        SenderID = senderID;
        SenderName = senderName;
        ReceiverID = receiverID;
        Content = content;
        MessageType = messageType;
        SendTime = DateTime.Now;
        IsRead = false;
        Channel = channel;
    }
}

[Serializable]
public class ChatChannel
{
    public string ChannelID;
    public string ChannelName;
    public string ChannelType;
    public List<string> Members;
    public int MaxMembers;
    public string CreatorID;
    public DateTime CreateTime;
    public bool IsActive;

    public ChatChannel(string channelID, string channelName, string channelType, string creatorID, int maxMembers = 50)
    {
        ChannelID = channelID;
        ChannelName = channelName;
        ChannelType = channelType;
        Members = new List<string>();
        MaxMembers = maxMembers;
        CreatorID = creatorID;
        CreateTime = DateTime.Now;
        IsActive = true;
    }
}

[Serializable]
public class Team
{
    public string TeamID;
    public string LeaderID;
    public List<string> MemberIDs;
    public string TeamName;
    public int TeamSize;
    public bool IsReady;
    public bool IsInMatch;
    public DateTime CreateTime;

    public Team(string teamID, string leaderID, string teamName)
    {
        TeamID = teamID;
        LeaderID = leaderID;
        MemberIDs = new List<string>();
        MemberIDs.Add(leaderID);
        TeamName = teamName;
        TeamSize = 1;
        IsReady = false;
        IsInMatch = false;
        CreateTime = DateTime.Now;
    }
}

[Serializable]
public class FriendRequest
{
    public string RequestID;
    public string SenderID;
    public string SenderName;
    public string ReceiverID;
    public string Message;
    public int RequestStatus;
    public DateTime SendTime;

    public FriendRequest(string requestID, string senderID, string senderName, string receiverID, string message)
    {
        RequestID = requestID;
        SenderID = senderID;
        SenderName = senderName;
        ReceiverID = receiverID;
        Message = message;
        RequestStatus = 0;
        SendTime = DateTime.Now;
    }
}

[Serializable]
public class SocialSystemData
{
    public List<Friend> Friends;
    public List<ChatMessage> Messages;
    public List<ChatChannel> Channels;
    public List<Team> Teams;
    public List<FriendRequest> FriendRequests;
    public Dictionary<string, List<string>> PlayerFriends;
    public Dictionary<string, List<string>> PlayerMessages;
    public Dictionary<string, List<string>> PlayerChannels;
    public List<string> Blacklist;
    public DateTime LastCleanupTime;

    public SocialSystemData()
    {
        Friends = new List<Friend>();
        Messages = new List<ChatMessage>();
        Channels = new List<ChatChannel>();
        Teams = new List<Team>();
        FriendRequests = new List<FriendRequest>();
        PlayerFriends = new Dictionary<string, List<string>>();
        PlayerMessages = new Dictionary<string, List<string>>();
        PlayerChannels = new Dictionary<string, List<string>>();
        Blacklist = new List<string>();
        LastCleanupTime = DateTime.Now;
        InitializeDefaultChannels();
    }

    private void InitializeDefaultChannels()
    {
        ChatChannel worldChannel = new ChatChannel("channel_world", "世界频道", "world", "system", 1000);
        worldChannel.IsActive = true;
        Channels.Add(worldChannel);

        ChatChannel teamChannel = new ChatChannel("channel_team", "队伍频道", "team", "system", 5);
        teamChannel.IsActive = true;
        Channels.Add(teamChannel);

        ChatChannel friendChannel = new ChatChannel("channel_friend", "好友频道", "friend", "system", 200);
        friendChannel.IsActive = true;
        Channels.Add(friendChannel);
    }

    public void AddFriend(Friend friend)
    {
        Friends.Add(friend);
        if (!PlayerFriends.ContainsKey(friend.PlayerID))
        {
            PlayerFriends[friend.PlayerID] = new List<string>();
        }
        PlayerFriends[friend.PlayerID].Add(friend.FriendID);
    }

    public void AddMessage(ChatMessage message)
    {
        Messages.Add(message);
        if (!PlayerMessages.ContainsKey(message.ReceiverID))
        {
            PlayerMessages[message.ReceiverID] = new List<string>();
        }
        PlayerMessages[message.ReceiverID].Add(message.MessageID);
    }

    public void AddChannel(ChatChannel channel)
    {
        Channels.Add(channel);
        foreach (string memberID in channel.Members)
        {
            if (!PlayerChannels.ContainsKey(memberID))
            {
                PlayerChannels[memberID] = new List<string>();
            }
            PlayerChannels[memberID].Add(channel.ChannelID);
        }
    }

    public void AddTeam(Team team)
    {
        Teams.Add(team);
    }

    public void AddFriendRequest(FriendRequest request)
    {
        FriendRequests.Add(request);
    }
}

[Serializable]
public class SocialEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string RelatedID;
    public string EventData;

    public SocialEvent(string eventID, string eventType, string playerID, string relatedID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        RelatedID = relatedID;
        EventData = eventData;
    }
}

public class SocialSystemDataManager
{
    private static SocialSystemDataManager _instance;
    public static SocialSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SocialSystemDataManager();
            }
            return _instance;
        }
    }

    public SocialSystemData socialData;
    private List<SocialEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SocialSystemDataManager()
    {
        socialData = new SocialSystemData();
        recentEvents = new List<SocialEvent>();
        LoadSocialData();
    }

    public void SaveSocialData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SocialSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, socialData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存社交系统数据失败: " + e.Message);
        }
    }

    public void LoadSocialData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SocialSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    socialData = (SocialSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载社交系统数据失败: " + e.Message);
            socialData = new SocialSystemData();
        }
    }

    public void CreateSocialEvent(string eventType, string playerID, string relatedID, string eventData)
    {
        string eventID = "social_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SocialEvent socialEvent = new SocialEvent(eventID, eventType, playerID, relatedID, eventData);
        recentEvents.Add(socialEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<SocialEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
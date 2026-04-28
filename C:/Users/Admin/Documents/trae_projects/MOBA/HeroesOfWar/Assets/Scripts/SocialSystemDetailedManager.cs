using System;
using System.Collections.Generic;

public class SocialSystemDetailedManager
{
    private static SocialSystemDetailedManager _instance;
    public static SocialSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SocialSystemDetailedManager();
            }
            return _instance;
        }
    }

    private SocialSystemData socialData;
    private SocialSystemDataManager dataManager;

    private SocialSystemDetailedManager()
    {
        dataManager = SocialSystemDataManager.Instance;
        socialData = dataManager.socialData;
    }

    public void SendFriendRequest(string senderID, string senderName, string receiverID, string message)
    {
        string requestID = "request_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        FriendRequest request = new FriendRequest(requestID, senderID, senderName, receiverID, message);
        socialData.AddFriendRequest(request);
        dataManager.CreateSocialEvent("friend_request", senderID, receiverID, "发送好友请求");
        dataManager.SaveSocialData();
        Debug.Log("发送好友请求成功");
    }

    public void AcceptFriendRequest(string requestID)
    {
        FriendRequest request = socialData.FriendRequests.Find(r => r.RequestID == requestID);
        if (request == null)
        {
            Debug.LogError("好友请求不存在: " + requestID);
            return;
        }

        request.RequestStatus = 1;
        
        string friendID1 = "friend_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Friend friend1 = new Friend(friendID1, request.ReceiverID, request.SenderID, request.SenderName);
        socialData.AddFriend(friend1);
        
        string friendID2 = "friend_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Friend friend2 = new Friend(friendID2, request.SenderID, request.ReceiverID, "" /* receiver name */);
        socialData.AddFriend(friend2);
        
        dataManager.CreateSocialEvent("friend_accept", request.ReceiverID, request.SenderID, "接受好友请求");
        dataManager.SaveSocialData();
        Debug.Log("接受好友请求成功");
    }

    public void RejectFriendRequest(string requestID)
    {
        FriendRequest request = socialData.FriendRequests.Find(r => r.RequestID == requestID);
        if (request != null)
        {
            request.RequestStatus = 2;
            dataManager.CreateSocialEvent("friend_reject", request.ReceiverID, request.SenderID, "拒绝好友请求");
            dataManager.SaveSocialData();
            Debug.Log("拒绝好友请求成功");
        }
    }

    public List<FriendRequest> GetReceivedRequests(string playerID)
    {
        return socialData.FriendRequests.FindAll(r => r.ReceiverID == playerID && r.RequestStatus == 0);
    }

    public List<FriendRequest> GetSentRequests(string playerID)
    {
        return socialData.FriendRequests.FindAll(r => r.SenderID == playerID && r.RequestStatus == 0);
    }

    public void RemoveFriend(string playerID, string friendPlayerID)
    {
        Friend friend = socialData.Friends.Find(f => f.PlayerID == playerID && f.FriendPlayerID == friendPlayerID);
        if (friend != null)
        {
            socialData.Friends.Remove(friend);
            if (socialData.PlayerFriends.ContainsKey(playerID))
            {
                socialData.PlayerFriends[playerID].Remove(friend.FriendID);
            }
            dataManager.CreateSocialEvent("friend_remove", playerID, friendPlayerID, "删除好友");
            dataManager.SaveSocialData();
            Debug.Log("删除好友成功");
        }
    }

    public List<Friend> GetFriends(string playerID)
    {
        return socialData.Friends.FindAll(f => f.PlayerID == playerID);
    }

    public Friend GetFriend(string playerID, string friendPlayerID)
    {
        return socialData.Friends.Find(f => f.PlayerID == playerID && f.FriendPlayerID == friendPlayerID);
    }

    public void UpdateFriendStatus(string playerID, string friendPlayerID, bool isOnline, string lastOnlineTime)
    {
        Friend friend = GetFriend(playerID, friendPlayerID);
        if (friend != null)
        {
            friend.IsOnline = isOnline;
            friend.LastOnlineTime = lastOnlineTime;
            dataManager.SaveSocialData();
        }
    }

    public void SendChatMessage(string senderID, string senderName, string receiverID, string content, int messageType, string channel = "private")
    {
        string messageID = "message_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ChatMessage message = new ChatMessage(messageID, senderID, senderName, receiverID, content, messageType, channel);
        socialData.AddMessage(message);
        dataManager.CreateSocialEvent("chat_send", senderID, receiverID, "发送消息: " + content.Substring(0, Math.Min(20, content.Length)));
        dataManager.SaveSocialData();
        Debug.Log("发送消息成功");
    }

    public List<ChatMessage> GetPlayerMessages(string playerID, int count = 50)
    {
        List<ChatMessage> messages = new List<ChatMessage>();
        if (socialData.PlayerMessages.ContainsKey(playerID))
        {
            foreach (string messageID in socialData.PlayerMessages[playerID])
            {
                ChatMessage message = socialData.Messages.Find(m => m.MessageID == messageID);
                if (message != null)
                {
                    messages.Add(message);
                }
            }
            messages.Sort((a, b) => b.SendTime.CompareTo(a.SendTime));
            if (count < messages.Count)
            {
                return messages.GetRange(0, count);
            }
        }
        return messages;
    }

    public List<ChatMessage> GetChannelMessages(string channelID, int count = 50)
    {
        List<ChatMessage> messages = socialData.Messages.FindAll(m => m.Channel == channelID);
        messages.Sort((a, b) => b.SendTime.CompareTo(a.SendTime));
        if (count < messages.Count)
        {
            return messages.GetRange(0, count);
        }
        return messages;
    }

    public void MarkMessageAsRead(string messageID)
    {
        ChatMessage message = socialData.Messages.Find(m => m.MessageID == messageID);
        if (message != null)
        {
            message.IsRead = true;
            dataManager.SaveSocialData();
        }
    }

    public int GetUnreadMessageCount(string playerID)
    {
        int count = 0;
        if (socialData.PlayerMessages.ContainsKey(playerID))
        {
            foreach (string messageID in socialData.PlayerMessages[playerID])
            {
                ChatMessage message = socialData.Messages.Find(m => m.MessageID == messageID);
                if (message != null && !message.IsRead)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public void CreateChatChannel(string channelName, string channelType, string creatorID, int maxMembers = 50)
    {
        string channelID = "channel_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ChatChannel channel = new ChatChannel(channelID, channelName, channelType, creatorID, maxMembers);
        channel.Members.Add(creatorID);
        socialData.AddChannel(channel);
        dataManager.CreateSocialEvent("channel_create", creatorID, channelID, "创建聊天频道: " + channelName);
        dataManager.SaveSocialData();
        Debug.Log("创建聊天频道成功: " + channelName);
    }

    public void JoinChatChannel(string playerID, string channelID)
    {
        ChatChannel channel = socialData.Channels.Find(c => c.ChannelID == channelID);
        if (channel != null && !channel.Members.Contains(playerID) && channel.Members.Count < channel.MaxMembers)
        {
            channel.Members.Add(playerID);
            if (!socialData.PlayerChannels.ContainsKey(playerID))
            {
                socialData.PlayerChannels[playerID] = new List<string>();
            }
            socialData.PlayerChannels[playerID].Add(channelID);
            dataManager.CreateSocialEvent("channel_join", playerID, channelID, "加入聊天频道");
            dataManager.SaveSocialData();
            Debug.Log("加入聊天频道成功");
        }
    }

    public void LeaveChatChannel(string playerID, string channelID)
    {
        ChatChannel channel = socialData.Channels.Find(c => c.ChannelID == channelID);
        if (channel != null && channel.Members.Contains(playerID))
        {
            channel.Members.Remove(playerID);
            if (socialData.PlayerChannels.ContainsKey(playerID))
            {
                socialData.PlayerChannels[playerID].Remove(channelID);
            }
            dataManager.SaveSocialData();
            Debug.Log("离开聊天频道成功");
        }
    }

    public List<ChatChannel> GetPlayerChannels(string playerID)
    {
        List<ChatChannel> channels = new List<ChatChannel>();
        if (socialData.PlayerChannels.ContainsKey(playerID))
        {
            foreach (string channelID in socialData.PlayerChannels[playerID])
            {
                ChatChannel channel = socialData.Channels.Find(c => c.ChannelID == channelID);
                if (channel != null)
                {
                    channels.Add(channel);
                }
            }
        }
        return channels;
    }

    public void CreateTeam(string leaderID, string teamName)
    {
        string teamID = "team_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Team team = new Team(teamID, leaderID, teamName);
        socialData.AddTeam(team);
        dataManager.CreateSocialEvent("team_create", leaderID, teamID, "创建队伍: " + teamName);
        dataManager.SaveSocialData();
        Debug.Log("创建队伍成功: " + teamName);
    }

    public void InviteToTeam(string teamID, string inviterID, string inviteeID)
    {
        Team team = socialData.Teams.Find(t => t.TeamID == teamID);
        if (team != null && team.LeaderID == inviterID && team.TeamSize < 5)
        {
            if (!team.MemberIDs.Contains(inviteeID))
            {
                team.MemberIDs.Add(inviteeID);
                team.TeamSize++;
                dataManager.CreateSocialEvent("team_invite", inviterID, inviteeID, "邀请加入队伍");
                dataManager.SaveSocialData();
                Debug.Log("邀请加入队伍成功");
            }
        }
    }

    public void LeaveTeam(string playerID, string teamID)
    {
        Team team = socialData.Teams.Find(t => t.TeamID == teamID);
        if (team != null && team.MemberIDs.Contains(playerID))
        {
            if (team.LeaderID == playerID)
            {
                if (team.TeamSize > 1)
                {
                    team.LeaderID = team.MemberIDs[1];
                }
                else
                {
                    socialData.Teams.Remove(team);
                }
            }
            team.MemberIDs.Remove(playerID);
            team.TeamSize--;
            dataManager.CreateSocialEvent("team_leave", playerID, teamID, "离开队伍");
            dataManager.SaveSocialData();
            Debug.Log("离开队伍成功");
        }
    }

    public Team GetTeam(string teamID)
    {
        return socialData.Teams.Find(t => t.TeamID == teamID);
    }

    public Team GetPlayerTeam(string playerID)
    {
        return socialData.Teams.Find(t => t.MemberIDs.Contains(playerID));
    }

    public void SetTeamReady(string teamID, bool isReady)
    {
        Team team = GetTeam(teamID);
        if (team != null)
        {
            team.IsReady = isReady;
            dataManager.SaveSocialData();
            Debug.Log("设置队伍准备状态: " + isReady);
        }
    }

    public void UpdateIntimacy(string playerID, string friendPlayerID, int points)
    {
        Friend friend = GetFriend(playerID, friendPlayerID);
        if (friend != null)
        {
            friend.IntimacyPoints += points;
            if (friend.IntimacyPoints >= 100)
            {
                friend.IntimacyLevel++;
                friend.IntimacyPoints = friend.IntimacyPoints % 100;
            }
            friend.LastInteractionTime = DateTime.Now;
            dataManager.SaveSocialData();
            Debug.Log("更新亲密度: " + points);
        }
    }

    public int GetIntimacyLevel(string playerID, string friendPlayerID)
    {
        Friend friend = GetFriend(playerID, friendPlayerID);
        if (friend != null)
        {
            return friend.IntimacyLevel;
        }
        return 1;
    }

    public void AddToBlacklist(string playerID, string targetPlayerID)
    {
        if (!socialData.Blacklist.Contains(targetPlayerID))
        {
            socialData.Blacklist.Add(targetPlayerID);
            dataManager.CreateSocialEvent("blacklist_add", playerID, targetPlayerID, "加入黑名单");
            dataManager.SaveSocialData();
            Debug.Log("加入黑名单成功");
        }
    }

    public void RemoveFromBlacklist(string playerID, string targetPlayerID)
    {
        if (socialData.Blacklist.Contains(targetPlayerID))
        {
            socialData.Blacklist.Remove(targetPlayerID);
            dataManager.CreateSocialEvent("blacklist_remove", playerID, targetPlayerID, "移除黑名单");
            dataManager.SaveSocialData();
            Debug.Log("移除黑名单成功");
        }
    }

    public bool IsInBlacklist(string targetPlayerID)
    {
        return socialData.Blacklist.Contains(targetPlayerID);
    }

    public List<SocialEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveSocialData();
    }

    public void LoadData()
    {
        dataManager.LoadSocialData();
    }
}
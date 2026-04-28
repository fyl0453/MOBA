using System.Collections.Generic;
using UnityEngine;

public class SocialManager : MonoBehaviour
{
    public static SocialManager Instance { get; private set; }
    
    private List<Friend> friends = new List<Friend>();
    private List<FriendRequest> friendRequests = new List<FriendRequest>();
    private List<ChatMessage> chatMessages = new List<ChatMessage>();
    private List<Team> teams = new List<Team>();
    private Team currentTeam;
    
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
        // 模拟数据
        AddFriend(new Friend("Player1", "Online", 1));
        AddFriend(new Friend("Player2", "Offline", 2));
        AddFriend(new Friend("Player3", "Online", 3));
        
        // 创建好友请求
        AddFriendRequest(new FriendRequest("Player4", "Online", 4));
        AddFriendRequest(new FriendRequest("Player5", "Online", 5));
        
        // 创建一个默认队伍
        CreateTeam("My Team");
    }
    
    public void AddFriend(Friend friend)
    {
        if (!friends.Contains(friend))
        {
            friends.Add(friend);
        }
    }
    
    public void RemoveFriend(Friend friend)
    {
        if (friends.Contains(friend))
        {
            friends.Remove(friend);
        }
    }
    
    public List<Friend> GetFriends()
    {
        return friends;
    }
    
    public void AddFriendRequest(FriendRequest request)
    {
        if (!friendRequests.Contains(request))
        {
            friendRequests.Add(request);
        }
    }
    
    public void RemoveFriendRequest(FriendRequest request)
    {
        if (friendRequests.Contains(request))
        {
            friendRequests.Remove(request);
        }
    }
    
    public List<FriendRequest> GetFriendRequests()
    {
        return friendRequests;
    }
    
    public void AcceptFriendRequest(FriendRequest request)
    {
        // 接受好友请求
        AddFriend(new Friend(request.playerName, request.status, request.level));
        RemoveFriendRequest(request);
    }
    
    public void RejectFriendRequest(FriendRequest request)
    {
        // 拒绝好友请求
        RemoveFriendRequest(request);
    }
    
    public void SendChatMessage(string message, string sender)
    {
        ChatMessage chatMessage = new ChatMessage(message, sender, System.DateTime.Now);
        chatMessages.Add(chatMessage);
        Debug.Log($"[{sender}]: {message}");
    }
    
    public List<ChatMessage> GetChatMessages()
    {
        return chatMessages;
    }
    
    public Team CreateTeam(string teamName)
    {
        Team team = new Team(teamName);
        teams.Add(team);
        currentTeam = team;
        return team;
    }
    
    public void JoinTeam(Team team)
    {
        currentTeam = team;
    }
    
    public void LeaveTeam()
    {
        currentTeam = null;
    }
    
    public Team GetCurrentTeam()
    {
        return currentTeam;
    }
    
    public List<Team> GetTeams()
    {
        return teams;
    }
}
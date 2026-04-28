using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance { get; private set; }
    
    public enum ChatChannel
    {
        All,
        Team,
        Friend
    }
    
    private List<ChatMessage> chatMessages = new List<ChatMessage>();
    private int maxMessages = 100;
    
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
    
    public void SendMessage(string message, string sender, ChatChannel channel)
    {
        ChatMessage chatMessage = new ChatMessage(message, sender, System.DateTime.Now);
        chatMessage.channel = channel;
        chatMessages.Add(chatMessage);
        
        // 限制消息数量
        if (chatMessages.Count > maxMessages)
        {
            chatMessages.RemoveAt(0);
        }
        
        Debug.Log($"[{channel}] {sender}: {message}");
    }
    
    public List<ChatMessage> GetMessages(ChatChannel channel = ChatChannel.All)
    {
        if (channel == ChatChannel.All)
        {
            return chatMessages;
        }
        else
        {
            return chatMessages.FindAll(msg => msg.channel == channel);
        }
    }
    
    public List<ChatMessage> GetRecentMessages(int count, ChatChannel channel = ChatChannel.All)
    {
        List<ChatMessage> filteredMessages = GetMessages(channel);
        if (filteredMessages.Count <= count)
        {
            return filteredMessages;
        }
        return filteredMessages.GetRange(filteredMessages.Count - count, count);
    }
    
    public void ClearMessages()
    {
        chatMessages.Clear();
    }
}
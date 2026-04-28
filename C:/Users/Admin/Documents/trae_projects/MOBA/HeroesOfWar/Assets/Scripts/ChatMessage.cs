[System.Serializable]
public class ChatMessage
{
    public string message;
    public string sender;
    public System.DateTime timestamp;
    public ChatManager.ChatChannel channel;
    
    public ChatMessage(string msg, string sender, System.DateTime time)
    {
        message = msg;
        this.sender = sender;
        timestamp = time;
        channel = ChatManager.ChatChannel.All;
    }
}
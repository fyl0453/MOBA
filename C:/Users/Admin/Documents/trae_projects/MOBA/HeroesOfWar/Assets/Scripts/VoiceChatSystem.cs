[System.Serializable]
public class VoiceChatRoom
{
    public string roomID;
    public string roomName;
    public string roomType;
    public List<string> participants;
    public bool isActive;
    public string creatorID;
    public string createdAt;
    
    public VoiceChatRoom(string id, string name, string type, string creator)
    {
        roomID = id;
        roomName = name;
        roomType = type;
        participants = new List<string>();
        isActive = true;
        creatorID = creator;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
    
    public void AddParticipant(string playerID)
    {
        if (!participants.Contains(playerID))
        {
            participants.Add(playerID);
        }
    }
    
    public void RemoveParticipant(string playerID)
    {
        participants.Remove(playerID);
    }
    
    public void Deactivate()
    {
        isActive = false;
    }
}

[System.Serializable]
public class VoiceMessage
{
    public string messageID;
    public string senderID;
    public string senderName;
    public string roomID;
    public byte[] audioData;
    public float duration;
    public string sentAt;
    public bool isRead;
    
    public VoiceMessage(string id, string sender, string senderName, string room, byte[] audio, float dur)
    {
        messageID = id;
        this.senderID = sender;
        this.senderName = senderName;
        roomID = room;
        audioData = audio;
        duration = dur;
        sentAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        isRead = false;
    }
    
    public void MarkAsRead()
    {
        isRead = true;
    }
}

[System.Serializable]
public class VoiceChatManagerData
{
    public List<VoiceChatRoom> rooms;
    public List<VoiceMessage> messages;
    
    public VoiceChatManagerData()
    {
        rooms = new List<VoiceChatRoom>();
        messages = new List<VoiceMessage>();
    }
    
    public void AddRoom(VoiceChatRoom room)
    {
        rooms.Add(room);
    }
    
    public void AddMessage(VoiceMessage message)
    {
        messages.Add(message);
    }
    
    public VoiceChatRoom GetRoom(string roomID)
    {
        return rooms.Find(r => r.roomID == roomID);
    }
    
    public List<VoiceMessage> GetMessagesByRoom(string roomID)
    {
        return messages.FindAll(m => m.roomID == roomID);
    }
    
    public List<VoiceChatRoom> GetActiveRooms()
    {
        return rooms.FindAll(r => r.isActive);
    }
}
[System.Serializable]
public class VoiceChatSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<VoiceChatRoom> voiceChatRooms;
    public List<VoiceMessage> voiceMessages;
    public List<VoiceChatParticipant> voiceChatParticipants;
    public List<VoiceChatEvent> voiceChatEvents;
    
    public VoiceChatSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        voiceChatRooms = new List<VoiceChatRoom>();
        voiceMessages = new List<VoiceMessage>();
        voiceChatParticipants = new List<VoiceChatParticipant>();
        voiceChatEvents = new List<VoiceChatEvent>();
    }
    
    public void AddVoiceChatRoom(VoiceChatRoom voiceChatRoom)
    {
        voiceChatRooms.Add(voiceChatRoom);
    }
    
    public void AddVoiceMessage(VoiceMessage voiceMessage)
    {
        voiceMessages.Add(voiceMessage);
    }
    
    public void AddVoiceChatParticipant(VoiceChatParticipant voiceChatParticipant)
    {
        voiceChatParticipants.Add(voiceChatParticipant);
    }
    
    public void AddVoiceChatEvent(VoiceChatEvent voiceChatEvent)
    {
        voiceChatEvents.Add(voiceChatEvent);
    }
    
    public VoiceChatRoom GetVoiceChatRoom(string roomID)
    {
        return voiceChatRooms.Find(vcr => vcr.roomID == roomID);
    }
    
    public VoiceMessage GetVoiceMessage(string messageID)
    {
        return voiceMessages.Find(vm => vm.messageID == messageID);
    }
    
    public VoiceChatParticipant GetVoiceChatParticipant(string participantID)
    {
        return voiceChatParticipants.Find(vcp => vcp.participantID == participantID);
    }
    
    public VoiceChatEvent GetVoiceChatEvent(string eventID)
    {
        return voiceChatEvents.Find(vce => vce.eventID == eventID);
    }
    
    public List<VoiceChatRoom> GetVoiceChatRoomsByType(string roomType)
    {
        return voiceChatRooms.FindAll(vcr => vcr.roomType == roomType);
    }
    
    public List<VoiceChatRoom> GetVoiceChatRoomsByStatus(string status)
    {
        return voiceChatRooms.FindAll(vcr => vcr.status == status);
    }
    
    public List<VoiceMessage> GetVoiceMessagesByRoom(string roomID)
    {
        return voiceMessages.FindAll(vm => vm.roomID == roomID);
    }
    
    public List<VoiceChatParticipant> GetVoiceChatParticipantsByRoom(string roomID)
    {
        return voiceChatParticipants.FindAll(vcp => vcp.roomID == roomID);
    }
    
    public List<VoiceChatEvent> GetVoiceChatEventsByUser(string userID)
    {
        return voiceChatEvents.FindAll(vce => vce.userID == userID);
    }
}

[System.Serializable]
public class VoiceChatRoom
{
    public string roomID;
    public string roomName;
    public string roomDescription;
    public string roomType;
    public string status;
    public int maxParticipants;
    public int currentParticipants;
    public string creatorID;
    public string password;
    public bool isPrivate;
    public string createTime;
    public string closeTime;
    
    public VoiceChatRoom(string id, string roomName, string roomDescription, string roomType, int maxParticipants, string creatorID, string password, bool isPrivate)
    {
        roomID = id;
        this.roomName = roomName;
        this.roomDescription = roomDescription;
        this.roomType = roomType;
        status = "active";
        this.maxParticipants = maxParticipants;
        currentParticipants = 0;
        this.creatorID = creatorID;
        this.password = password;
        this.isPrivate = isPrivate;
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        closeTime = "";
    }
    
    public void Close()
    {
        status = "closed";
        closeTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddParticipant()
    {
        currentParticipants++;
    }
    
    public void RemoveParticipant()
    {
        currentParticipants--;
    }
    
    public bool CanJoin()
    {
        if (status != "active")
            return false;
        
        if (currentParticipants >= maxParticipants)
            return false;
        
        return true;
    }
}

[System.Serializable]
public class VoiceMessage
{
    public string messageID;
    public string roomID;
    public string userID;
    public string userName;
    public string voiceData;
    public string textData;
    public float duration;
    public string status;
    public string sendTime;
    public string receiveTime;
    
    public VoiceMessage(string id, string roomID, string userID, string userName, string voiceData, string textData, float duration)
    {
        messageID = id;
        this.roomID = roomID;
        this.userID = userID;
        this.userName = userName;
        this.voiceData = voiceData;
        this.textData = textData;
        this.duration = duration;
        status = "sent";
        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        receiveTime = "";
    }
    
    public void MarkAsReceived()
    {
        status = "received";
        receiveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class VoiceChatParticipant
{
    public string participantID;
    public string roomID;
    public string userID;
    public string userName;
    public string status;
    public string joinTime;
    public string leaveTime;
    public bool isSpeaking;
    public float volume;
    
    public VoiceChatParticipant(string id, string roomID, string userID, string userName)
    {
        participantID = id;
        this.roomID = roomID;
        this.userID = userID;
        this.userName = userName;
        status = "active";
        joinTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        leaveTime = "";
        isSpeaking = false;
        volume = 1.0f;
    }
    
    public void Leave()
    {
        status = "left";
        leaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void SetSpeaking(bool isSpeaking)
    {
        this.isSpeaking = isSpeaking;
    }
    
    public void SetVolume(float volume)
    {
        this.volume = volume;
    }
}

[System.Serializable]
public class VoiceChatEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string roomID;
    public string description;
    public string timestamp;
    public string status;
    
    public VoiceChatEvent(string id, string eventType, string userID, string roomID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.roomID = roomID;
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
public class VoiceChatSystemDetailedManagerData
{
    public VoiceChatSystemDetailed system;
    
    public VoiceChatSystemDetailedManagerData()
    {
        system = new VoiceChatSystemDetailed("voice_chat_system_detailed", "语音聊天系统详细", "管理语音聊天的详细功能，包括语音聊天和语音转文字");
    }
}
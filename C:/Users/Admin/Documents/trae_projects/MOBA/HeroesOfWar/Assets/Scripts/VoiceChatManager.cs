using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VoiceChatManager : MonoBehaviour
{
    public static VoiceChatManager Instance { get; private set; }
    
    public VoiceChatManagerData voiceChatData;
    
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
        LoadVoiceChatData();
        
        if (voiceChatData == null)
        {
            voiceChatData = new VoiceChatManagerData();
        }
    }
    
    public VoiceChatRoom CreateRoom(string roomName, string roomType)
    {
        string roomID = "room_" + System.DateTime.Now.Ticks;
        string creatorID = ProfileManager.Instance.currentProfile.playerID;
        
        VoiceChatRoom room = new VoiceChatRoom(roomID, roomName, roomType, creatorID);
        room.AddParticipant(creatorID);
        voiceChatData.AddRoom(room);
        SaveVoiceChatData();
        
        return room;
    }
    
    public void JoinRoom(string roomID)
    {
        VoiceChatRoom room = voiceChatData.GetRoom(roomID);
        if (room != null && room.isActive)
        {
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            room.AddParticipant(playerID);
            SaveVoiceChatData();
        }
    }
    
    public void LeaveRoom(string roomID)
    {
        VoiceChatRoom room = voiceChatData.GetRoom(roomID);
        if (room != null)
        {
            string playerID = ProfileManager.Instance.currentProfile.playerID;
            room.RemoveParticipant(playerID);
            
            if (room.participants.Count == 0)
            {
                room.Deactivate();
            }
            
            SaveVoiceChatData();
        }
    }
    
    public void SendVoiceMessage(string roomID, byte[] audioData, float duration)
    {
        VoiceChatRoom room = voiceChatData.GetRoom(roomID);
        if (room != null && room.isActive && room.participants.Contains(ProfileManager.Instance.currentProfile.playerID))
        {
            string messageID = "message_" + System.DateTime.Now.Ticks;
            string senderID = ProfileManager.Instance.currentProfile.playerID;
            string senderName = ProfileManager.Instance.currentProfile.playerName;
            
            VoiceMessage message = new VoiceMessage(messageID, senderID, senderName, roomID, audioData, duration);
            voiceChatData.AddMessage(message);
            SaveVoiceChatData();
        }
    }
    
    public void MarkMessageAsRead(string messageID)
    {
        VoiceMessage message = voiceChatData.messages.Find(m => m.messageID == messageID);
        if (message != null)
        {
            message.MarkAsRead();
            SaveVoiceChatData();
        }
    }
    
    public List<VoiceChatRoom> GetActiveRooms()
    {
        return voiceChatData.GetActiveRooms();
    }
    
    public VoiceChatRoom GetRoom(string roomID)
    {
        return voiceChatData.GetRoom(roomID);
    }
    
    public List<VoiceMessage> GetMessagesByRoom(string roomID)
    {
        return voiceChatData.GetMessagesByRoom(roomID);
    }
    
    public void SaveVoiceChatData()
    {
        string path = Application.dataPath + "/Data/voice_chat_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, voiceChatData);
        stream.Close();
    }
    
    public void LoadVoiceChatData()
    {
        string path = Application.dataPath + "/Data/voice_chat_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            voiceChatData = (VoiceChatManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            voiceChatData = new VoiceChatManagerData();
        }
    }
}
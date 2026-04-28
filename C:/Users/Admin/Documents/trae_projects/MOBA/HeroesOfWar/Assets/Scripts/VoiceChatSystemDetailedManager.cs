using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VoiceChatSystemDetailedManager : MonoBehaviour
{
    public static VoiceChatSystemDetailedManager Instance { get; private set; }
    
    public VoiceChatSystemDetailedManagerData voiceChatData;
    
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
            voiceChatData = new VoiceChatSystemDetailedManagerData();
            InitializeDefaultVoiceChatSystem();
        }
    }
    
    private void InitializeDefaultVoiceChatSystem()
    {
        // 语音聊天室
        VoiceChatRoom room1 = new VoiceChatRoom("room_001", "好友聊天室", "好友语音聊天", "friend", 10, "user_001", "", false);
        VoiceChatRoom room2 = new VoiceChatRoom("room_002", "公会聊天室", "公会语音聊天", "guild", 50, "user_001", "", false);
        VoiceChatRoom room3 = new VoiceChatRoom("room_003", "组队聊天室", "组队语音聊天", "team", 5, "user_002", "", false);
        
        voiceChatData.system.AddVoiceChatRoom(room1);
        voiceChatData.system.AddVoiceChatRoom(room2);
        voiceChatData.system.AddVoiceChatRoom(room3);
        
        // 语音聊天参与者
        VoiceChatParticipant participant1 = new VoiceChatParticipant("participant_001", "room_001", "user_001", "张三");
        VoiceChatParticipant participant2 = new VoiceChatParticipant("participant_002", "room_001", "user_002", "李四");
        VoiceChatParticipant participant3 = new VoiceChatParticipant("participant_003", "room_002", "user_001", "张三");
        VoiceChatParticipant participant4 = new VoiceChatParticipant("participant_004", "room_002", "user_002", "李四");
        VoiceChatParticipant participant5 = new VoiceChatParticipant("participant_005", "room_003", "user_002", "李四");
        VoiceChatParticipant participant6 = new VoiceChatParticipant("participant_006", "room_003", "user_003", "王五");
        
        voiceChatData.system.AddVoiceChatParticipant(participant1);
        voiceChatData.system.AddVoiceChatParticipant(participant2);
        voiceChatData.system.AddVoiceChatParticipant(participant3);
        voiceChatData.system.AddVoiceChatParticipant(participant4);
        voiceChatData.system.AddVoiceChatParticipant(participant5);
        voiceChatData.system.AddVoiceChatParticipant(participant6);
        
        // 更新聊天室参与者数量
        room1.AddParticipant();
        room1.AddParticipant();
        room2.AddParticipant();
        room2.AddParticipant();
        room3.AddParticipant();
        room3.AddParticipant();
        
        // 语音消息
        VoiceMessage message1 = new VoiceMessage("message_001", "room_001", "user_001", "张三", "voice_data_001", "你好，一起玩游戏吗？", 3.5f);
        VoiceMessage message2 = new VoiceMessage("message_002", "room_001", "user_002", "李四", "voice_data_002", "好的，马上来", 2.0f);
        VoiceMessage message3 = new VoiceMessage("message_003", "room_003", "user_002", "李四", "voice_data_003", "准备就绪，开始游戏", 1.5f);
        
        voiceChatData.system.AddVoiceMessage(message1);
        voiceChatData.system.AddVoiceMessage(message2);
        voiceChatData.system.AddVoiceMessage(message3);
        
        // 语音聊天事件
        VoiceChatEvent event1 = new VoiceChatEvent("event_001", "join", "user_001", "room_001", "加入语音聊天室");
        VoiceChatEvent event2 = new VoiceChatEvent("event_002", "leave", "user_002", "room_001", "离开语音聊天室");
        VoiceChatEvent event3 = new VoiceChatEvent("event_003", "speak", "user_001", "room_001", "开始说话");
        
        voiceChatData.system.AddVoiceChatEvent(event1);
        voiceChatData.system.AddVoiceChatEvent(event2);
        voiceChatData.system.AddVoiceChatEvent(event3);
        
        SaveVoiceChatData();
    }
    
    // 语音聊天室管理
    public void CreateVoiceChatRoom(string roomName, string roomDescription, string roomType, int maxParticipants, string creatorID, string userName, string password, bool isPrivate)
    {
        string roomID = "room_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        VoiceChatRoom voiceChatRoom = new VoiceChatRoom(roomID, roomName, roomDescription, roomType, maxParticipants, creatorID, password, isPrivate);
        voiceChatData.system.AddVoiceChatRoom(voiceChatRoom);
        
        // 加入创建者
        JoinVoiceChatRoom(creatorID, userName, roomID, password);
        
        SaveVoiceChatData();
        Debug.Log("成功创建语音聊天室: " + roomName);
    }
    
    public void CloseVoiceChatRoom(string roomID, string userID)
    {
        VoiceChatRoom voiceChatRoom = voiceChatData.system.GetVoiceChatRoom(roomID);
        if (voiceChatRoom != null && voiceChatRoom.creatorID == userID)
        {
            // 移除所有参与者
            List<VoiceChatParticipant> participants = voiceChatData.system.GetVoiceChatParticipantsByRoom(roomID);
            foreach (VoiceChatParticipant participant in participants)
            {
                participant.Leave();
            }
            
            voiceChatRoom.Close();
            SaveVoiceChatData();
            Debug.Log("成功关闭语音聊天室: " + voiceChatRoom.roomName);
        }
        else
        {
            Debug.LogError("语音聊天室不存在或无权限");
        }
    }
    
    public List<VoiceChatRoom> GetVoiceChatRoomsByType(string roomType)
    {
        return voiceChatData.system.GetVoiceChatRoomsByType(roomType);
    }
    
    public List<VoiceChatRoom> GetActiveVoiceChatRooms()
    {
        return voiceChatData.system.GetVoiceChatRoomsByStatus("active");
    }
    
    // 语音聊天参与管理
    public void JoinVoiceChatRoom(string userID, string userName, string roomID, string password)
    {
        VoiceChatRoom voiceChatRoom = voiceChatData.system.GetVoiceChatRoom(roomID);
        if (voiceChatRoom != null && voiceChatRoom.CanJoin())
        {
            // 检查密码
            if (voiceChatRoom.isPrivate && voiceChatRoom.password != password)
            {
                Debug.LogError("密码错误");
                return;
            }
            
            // 检查是否已经在聊天室中
            List<VoiceChatParticipant> participants = voiceChatData.system.GetVoiceChatParticipantsByRoom(roomID);
            VoiceChatParticipant existingParticipant = participants.Find(p => p.userID == userID);
            
            if (existingParticipant == null)
            {
                string participantID = "participant_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                VoiceChatParticipant voiceChatParticipant = new VoiceChatParticipant(participantID, roomID, userID, userName);
                voiceChatData.system.AddVoiceChatParticipant(voiceChatParticipant);
                voiceChatRoom.AddParticipant();
                
                // 创建语音聊天事件
                CreateVoiceChatEvent("join", userID, roomID, "加入语音聊天室");
                
                SaveVoiceChatData();
                Debug.Log("成功加入语音聊天室: " + voiceChatRoom.roomName);
            }
            else
            {
                Debug.LogError("已经在语音聊天室中");
            }
        }
        else
        {
            Debug.LogError("语音聊天室不存在或已满");
        }
    }
    
    public void LeaveVoiceChatRoom(string userID, string roomID)
    {
        List<VoiceChatParticipant> participants = voiceChatData.system.GetVoiceChatParticipantsByRoom(roomID);
        VoiceChatParticipant voiceChatParticipant = participants.Find(p => p.userID == userID);
        
        if (voiceChatParticipant != null)
        {
            voiceChatParticipant.Leave();
            
            VoiceChatRoom voiceChatRoom = voiceChatData.system.GetVoiceChatRoom(roomID);
            if (voiceChatRoom != null)
            {
                voiceChatRoom.RemoveParticipant();
            }
            
            // 创建语音聊天事件
            CreateVoiceChatEvent("leave", userID, roomID, "离开语音聊天室");
            
            SaveVoiceChatData();
            Debug.Log("成功离开语音聊天室");
        }
        else
        {
            Debug.LogError("不在语音聊天室中");
        }
    }
    
    public void SetSpeakingStatus(string userID, string roomID, bool isSpeaking)
    {
        List<VoiceChatParticipant> participants = voiceChatData.system.GetVoiceChatParticipantsByRoom(roomID);
        VoiceChatParticipant voiceChatParticipant = participants.Find(p => p.userID == userID);
        
        if (voiceChatParticipant != null)
        {
            voiceChatParticipant.SetSpeaking(isSpeaking);
            
            // 创建语音聊天事件
            if (isSpeaking)
            {
                CreateVoiceChatEvent("speak", userID, roomID, "开始说话");
            }
            else
            {
                CreateVoiceChatEvent("stop_speak", userID, roomID, "停止说话");
            }
            
            SaveVoiceChatData();
            Debug.Log("成功设置说话状态: " + isSpeaking);
        }
        else
        {
            Debug.LogError("不在语音聊天室中");
        }
    }
    
    public void SetVolume(string userID, string roomID, float volume)
    {
        List<VoiceChatParticipant> participants = voiceChatData.system.GetVoiceChatParticipantsByRoom(roomID);
        VoiceChatParticipant voiceChatParticipant = participants.Find(p => p.userID == userID);
        
        if (voiceChatParticipant != null)
        {
            voiceChatParticipant.SetVolume(volume);
            SaveVoiceChatData();
            Debug.Log("成功设置音量: " + volume);
        }
        else
        {
            Debug.LogError("不在语音聊天室中");
        }
    }
    
    public List<VoiceChatParticipant> GetVoiceChatParticipants(string roomID)
    {
        return voiceChatData.system.GetVoiceChatParticipantsByRoom(roomID);
    }
    
    // 语音消息管理
    public void SendVoiceMessage(string roomID, string userID, string userName, string voiceData, string textData, float duration)
    {
        List<VoiceChatParticipant> participants = voiceChatData.system.GetVoiceChatParticipantsByRoom(roomID);
        VoiceChatParticipant voiceChatParticipant = participants.Find(p => p.userID == userID);
        
        if (voiceChatParticipant != null)
        {
            string messageID = "message_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            VoiceMessage voiceMessage = new VoiceMessage(messageID, roomID, userID, userName, voiceData, textData, duration);
            voiceChatData.system.AddVoiceMessage(voiceMessage);
            
            // 创建语音聊天事件
            CreateVoiceChatEvent("send_message", userID, roomID, "发送语音消息");
            
            SaveVoiceChatData();
            Debug.Log("成功发送语音消息");
        }
        else
        {
            Debug.LogError("不在语音聊天室中");
        }
    }
    
    public void MarkVoiceMessageAsReceived(string messageID)
    {
        VoiceMessage voiceMessage = voiceChatData.system.GetVoiceMessage(messageID);
        if (voiceMessage != null)
        {
            voiceMessage.MarkAsReceived();
            SaveVoiceChatData();
            Debug.Log("成功标记语音消息为已接收");
        }
        else
        {
            Debug.LogError("语音消息不存在: " + messageID);
        }
    }
    
    public List<VoiceMessage> GetVoiceMessages(string roomID)
    {
        return voiceChatData.system.GetVoiceMessagesByRoom(roomID);
    }
    
    // 语音聊天事件管理
    public string CreateVoiceChatEvent(string eventType, string userID, string roomID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        VoiceChatEvent voiceChatEvent = new VoiceChatEvent(eventID, eventType, userID, roomID, description);
        voiceChatData.system.AddVoiceChatEvent(voiceChatEvent);
        SaveVoiceChatData();
        Debug.Log("成功创建语音聊天事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        VoiceChatEvent voiceChatEvent = voiceChatData.system.GetVoiceChatEvent(eventID);
        if (voiceChatEvent != null)
        {
            voiceChatEvent.MarkAsCompleted();
            SaveVoiceChatData();
            Debug.Log("成功标记语音聊天事件为完成");
        }
        else
        {
            Debug.LogError("语音聊天事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        VoiceChatEvent voiceChatEvent = voiceChatData.system.GetVoiceChatEvent(eventID);
        if (voiceChatEvent != null)
        {
            voiceChatEvent.MarkAsFailed();
            SaveVoiceChatData();
            Debug.Log("成功标记语音聊天事件为失败");
        }
        else
        {
            Debug.LogError("语音聊天事件不存在: " + eventID);
        }
    }
    
    public List<VoiceChatEvent> GetVoiceChatEvents(string userID)
    {
        return voiceChatData.system.GetVoiceChatEventsByUser(userID);
    }
    
    // 数据持久化
    public void SaveVoiceChatData()
    {
        string path = Application.dataPath + "/Data/voice_chat_system_detailed_data.dat";
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
        string path = Application.dataPath + "/Data/voice_chat_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            voiceChatData = (VoiceChatSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            voiceChatData = new VoiceChatSystemDetailedManagerData();
        }
    }
}
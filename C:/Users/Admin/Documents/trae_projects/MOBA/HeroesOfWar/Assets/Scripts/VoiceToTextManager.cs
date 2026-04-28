using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VoiceToTextManager : MonoBehaviour
{
    public static VoiceToTextManager Instance { get; private set; }
    
    public VoiceToTextManagerData voiceToTextData;
    
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
        LoadVoiceToTextData();
        
        if (voiceToTextData == null)
        {
            voiceToTextData = new VoiceToTextManagerData();
        }
    }
    
    public VoiceToText RecordVoiceMessage(string voicePath, float duration)
    {
        string messageID = "voice_" + System.DateTime.Now.Ticks;
        string senderID = ProfileManager.Instance.currentProfile.playerID;
        string senderName = ProfileManager.Instance.currentProfile.playerName;
        
        VoiceToText voiceMessage = new VoiceToText(messageID, senderID, senderName, voicePath, duration);
        voiceToTextData.AddVoiceMessage(voiceMessage);
        SaveVoiceToTextData();
        
        // 模拟语音转文字过程
        StartCoroutine(ConvertVoiceToText(voiceMessage));
        
        return voiceMessage;
    }
    
    private System.Collections.IEnumerator ConvertVoiceToText(VoiceToText voiceMessage)
    {
        // 模拟语音转文字的延迟
        yield return new WaitForSeconds(1f);
        
        // 模拟转换结果
        string convertedText = "这是一段语音消息的转换结果";
        voiceMessage.ConvertToText(convertedText);
        SaveVoiceToTextData();
    }
    
    public VoiceToText GetVoiceMessage(string messageID)
    {
        return voiceToTextData.GetVoiceMessage(messageID);
    }
    
    public List<VoiceToText> GetUnconvertedMessages()
    {
        return voiceToTextData.GetUnconvertedMessages();
    }
    
    public void SaveVoiceToTextData()
    {
        string path = Application.dataPath + "/Data/voice_to_text_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, voiceToTextData);
        stream.Close();
    }
    
    public void LoadVoiceToTextData()
    {
        string path = Application.dataPath + "/Data/voice_to_text_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            voiceToTextData = (VoiceToTextManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            voiceToTextData = new VoiceToTextManagerData();
        }
    }
}
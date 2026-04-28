[System.Serializable]
public class VoiceToText
{
    public string messageID;
    public string senderID;
    public string senderName;
    public string voicePath;
    public string textContent;
    public float duration;
    public string sentTime;
    public bool isConverted;
    
    public VoiceToText(string id, string sender, string senderName, string voice, float dur)
    {
        messageID = id;
        this.senderID = sender;
        this.senderName = senderName;
        voicePath = voice;
        textContent = "";
        duration = dur;
        sentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        isConverted = false;
    }
    
    public void ConvertToText(string text)
    {
        textContent = text;
        isConverted = true;
    }
}

[System.Serializable]
public class VoiceToTextManagerData
{
    public List<VoiceToText> voiceMessages;
    
    public VoiceToTextManagerData()
    {
        voiceMessages = new List<VoiceToText>();
    }
    
    public void AddVoiceMessage(VoiceToText message)
    {
        voiceMessages.Add(message);
    }
    
    public VoiceToText GetVoiceMessage(string messageID)
    {
        return voiceMessages.Find(m => m.messageID == messageID);
    }
    
    public List<VoiceToText> GetUnconvertedMessages()
    {
        return voiceMessages.FindAll(m => !m.isConverted);
    }
}
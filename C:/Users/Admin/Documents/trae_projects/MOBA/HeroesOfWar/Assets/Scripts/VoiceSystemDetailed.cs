using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class VoiceRecording
{
    public string RecordingID;
    public string PlayerID;
    public string PlayerName;
    public string MatchID;
    public string Content;
    public string TranscribedText;
    public float Duration;
    public DateTime RecordingTime;
    public DateTime TranscriptionTime;
    public string AudioPath;
    public string Language;
    public float Confidence;
    public bool IsTranscribed;
    public bool IsPublic;

    public VoiceRecording(string playerID, string playerName, string matchID, float duration, string language)
    {
        RecordingID = "recording_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PlayerID = playerID;
        PlayerName = playerName;
        MatchID = matchID;
        Content = "";
        TranscribedText = "";
        Duration = duration;
        RecordingTime = DateTime.Now;
        TranscriptionTime = DateTime.MinValue;
        AudioPath = "";
        Language = language;
        Confidence = 0f;
        IsTranscribed = false;
        IsPublic = true;
    }
}

[Serializable]
public class TranscriptionConfig
{
    public string Language;
    public bool RealTimeTranscription;
    public bool AutoTranscribe;
    public bool ShowTranscriptions;
    public bool SaveRecordings;
    public float SilenceThreshold;
    public int MaxRecordingDuration;
    public bool EnableProfanityFilter;
    public bool EnableTranslation;
    public string TargetLanguage;

    public TranscriptionConfig()
    {
        Language = "zh-CN";
        RealTimeTranscription = true;
        AutoTranscribe = true;
        ShowTranscriptions = true;
        SaveRecordings = false;
        SilenceThreshold = 0.1f;
        MaxRecordingDuration = 60;
        EnableProfanityFilter = true;
        EnableTranslation = false;
        TargetLanguage = "zh-CN";
    }
}

[Serializable]
public class PlayerVoiceData
{
    public string PlayerID;
    public List<VoiceRecording> Recordings;
    public TranscriptionConfig VoiceConfig;
    public int TotalRecordings;
    public int TotalTranscriptions;
    public DateTime LastRecordingTime;
    public bool VoiceEnabled;
    public bool TranscriptionEnabled;

    public PlayerVoiceData(string playerID)
    {
        PlayerID = playerID;
        Recordings = new List<VoiceRecording>();
        VoiceConfig = new TranscriptionConfig();
        TotalRecordings = 0;
        TotalTranscriptions = 0;
        LastRecordingTime = DateTime.MinValue;
        VoiceEnabled = true;
        TranscriptionEnabled = true;
    }
}

[Serializable]
public class VoiceSystemData
{
    public Dictionary<string, PlayerVoiceData> PlayerVoiceData;
    public List<string> AvailableLanguages;
    public int MaxRecordingsPerPlayer;
    public int MaxRecordingDuration;
    public bool SystemEnabled;
    public DateTime LastSystemUpdate;

    public VoiceSystemData()
    {
        PlayerVoiceData = new Dictionary<string, PlayerVoiceData>();
        AvailableLanguages = new List<string> { "zh-CN", "en-US", "ja-JP", "ko-KR" };
        MaxRecordingsPerPlayer = 100;
        MaxRecordingDuration = 60;
        SystemEnabled = true;
        LastSystemUpdate = DateTime.Now;
    }

    public void AddPlayerVoiceData(string playerID, PlayerVoiceData voiceData)
    {
        PlayerVoiceData[playerID] = voiceData;
    }
}

[Serializable]
public class VoiceEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string RecordingID;
    public string EventData;

    public VoiceEvent(string eventID, string eventType, string playerID, string recordingID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        RecordingID = recordingID;
        EventData = eventData;
    }
}

public class VoiceSystemDataManager
{
    private static VoiceSystemDataManager _instance;
    public static VoiceSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new VoiceSystemDataManager();
            }
            return _instance;
        }
    }

    public VoiceSystemData voiceData;
    private List<VoiceEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private VoiceSystemDataManager()
    {
        voiceData = new VoiceSystemData();
        recentEvents = new List<VoiceEvent>();
        LoadVoiceData();
    }

    public void SaveVoiceData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "VoiceSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, voiceData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存语音系统数据失败: " + e.Message);
        }
    }

    public void LoadVoiceData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "VoiceSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    voiceData = (VoiceSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载语音系统数据失败: " + e.Message);
            voiceData = new VoiceSystemData();
        }
    }

    public void CreateVoiceEvent(string eventType, string playerID, string recordingID, string eventData)
    {
        string eventID = "voice_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        VoiceEvent voiceEvent = new VoiceEvent(eventID, eventType, playerID, recordingID, eventData);
        recentEvents.Add(voiceEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<VoiceEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
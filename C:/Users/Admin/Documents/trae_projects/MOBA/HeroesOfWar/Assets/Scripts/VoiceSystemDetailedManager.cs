using System;
using System.Collections.Generic;
using UnityEngine;

public class VoiceSystemDetailedManager
{
    private static VoiceSystemDetailedManager _instance;
    public static VoiceSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new VoiceSystemDetailedManager();
            }
            return _instance;
        }
    }

    private VoiceSystemData voiceData;
    private VoiceSystemDataManager dataManager;

    private VoiceSystemDetailedManager()
    {
        dataManager = VoiceSystemDataManager.Instance;
        voiceData = dataManager.voiceData;
    }

    public void InitializePlayerVoiceData(string playerID)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            PlayerVoiceData playerVoiceData = new PlayerVoiceData(playerID);
            voiceData.AddPlayerVoiceData(playerID, playerVoiceData);
            dataManager.SaveVoiceData();
            Debug.Log("初始化语音数据成功");
        }
    }

    public string StartRecording(string playerID, string playerName, string matchID, string language)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            InitializePlayerVoiceData(playerID);
        }
        
        PlayerVoiceData playerVoiceData = voiceData.PlayerVoiceData[playerID];
        if (!playerVoiceData.VoiceEnabled)
        {
            Debug.LogError("语音功能已禁用");
            return "";
        }
        
        if (playerVoiceData.Recordings.Count >= voiceData.MaxRecordingsPerPlayer)
        {
            Debug.LogError("录音数量达到上限");
            return "";
        }
        
        VoiceRecording recording = new VoiceRecording(playerID, playerName, matchID, 0, language);
        playerVoiceData.Recordings.Add(recording);
        playerVoiceData.TotalRecordings++;
        playerVoiceData.LastRecordingTime = DateTime.Now;
        
        dataManager.CreateVoiceEvent("recording_start", playerID, recording.RecordingID, "开始录音");
        dataManager.SaveVoiceData();
        Debug.Log("开始录音成功");
        return recording.RecordingID;
    }

    public void StopRecording(string playerID, string recordingID, float duration, string audioPath)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerVoiceData playerVoiceData = voiceData.PlayerVoiceData[playerID];
        VoiceRecording recording = playerVoiceData.Recordings.Find(r => r.RecordingID == recordingID);
        if (recording != null)
        {
            recording.Duration = duration;
            recording.AudioPath = audioPath;
            
            dataManager.CreateVoiceEvent("recording_stop", playerID, recordingID, "停止录音，时长: " + duration + "秒");
            dataManager.SaveVoiceData();
            Debug.Log("停止录音成功");
        }
    }

    public string TranscribeVoice(string playerID, string recordingID, string content)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            return "";
        }
        
        PlayerVoiceData playerVoiceData = voiceData.PlayerVoiceData[playerID];
        VoiceRecording recording = playerVoiceData.Recordings.Find(r => r.RecordingID == recordingID);
        if (recording == null)
        {
            return "";
        }
        
        if (!playerVoiceData.TranscriptionEnabled)
        {
            Debug.LogError("语音转文字功能已禁用");
            return "";
        }
        
        string transcribedText = ProcessTranscription(content, playerVoiceData.VoiceConfig);
        recording.TranscribedText = transcribedText;
        recording.IsTranscribed = true;
        recording.TranscriptionTime = DateTime.Now;
        recording.Confidence = CalculateConfidence(content, transcribedText);
        
        playerVoiceData.TotalTranscriptions++;
        
        dataManager.CreateVoiceEvent("transcription_complete", playerID, recordingID, "语音转文字完成");
        dataManager.SaveVoiceData();
        Debug.Log("语音转文字成功");
        return transcribedText;
    }

    public void UpdateTranscriptionConfig(string playerID, TranscriptionConfig config)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            InitializePlayerVoiceData(playerID);
        }
        
        voiceData.PlayerVoiceData[playerID].VoiceConfig = config;
        
        dataManager.CreateVoiceEvent("config_update", playerID, "", "更新语音转文字配置");
        dataManager.SaveVoiceData();
        Debug.Log("更新语音转文字配置成功");
    }

    public void ToggleVoiceEnabled(string playerID, bool enabled)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            InitializePlayerVoiceData(playerID);
        }
        
        voiceData.PlayerVoiceData[playerID].VoiceEnabled = enabled;
        
        dataManager.CreateVoiceEvent("voice_toggle", playerID, "", "语音功能" + (enabled ? "已开启" : "已关闭"));
        dataManager.SaveVoiceData();
        Debug.Log("语音功能" + (enabled ? "已开启" : "已关闭"));
    }

    public void ToggleTranscriptionEnabled(string playerID, bool enabled)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            InitializePlayerVoiceData(playerID);
        }
        
        voiceData.PlayerVoiceData[playerID].TranscriptionEnabled = enabled;
        
        dataManager.CreateVoiceEvent("transcription_toggle", playerID, "", "语音转文字功能" + (enabled ? "已开启" : "已关闭"));
        dataManager.SaveVoiceData();
        Debug.Log("语音转文字功能" + (enabled ? "已开启" : "已关闭"));
    }

    public void DeleteRecording(string playerID, string recordingID)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerVoiceData playerVoiceData = voiceData.PlayerVoiceData[playerID];
        VoiceRecording recording = playerVoiceData.Recordings.Find(r => r.RecordingID == recordingID);
        if (recording != null)
        {
            playerVoiceData.Recordings.Remove(recording);
            playerVoiceData.TotalRecordings = Math.Max(0, playerVoiceData.TotalRecordings - 1);
            if (recording.IsTranscribed)
            {
                playerVoiceData.TotalTranscriptions = Math.Max(0, playerVoiceData.TotalTranscriptions - 1);
            }
            
            dataManager.CreateVoiceEvent("recording_delete", playerID, recordingID, "删除录音");
            dataManager.SaveVoiceData();
            Debug.Log("删除录音成功");
        }
    }

    public void DeleteAllRecordings(string playerID)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerVoiceData playerVoiceData = voiceData.PlayerVoiceData[playerID];
        int count = playerVoiceData.Recordings.Count;
        playerVoiceData.Recordings.Clear();
        playerVoiceData.TotalRecordings = 0;
        playerVoiceData.TotalTranscriptions = 0;
        
        dataManager.CreateVoiceEvent("recording_delete_all", playerID, "", "删除所有录音: " + count);
        dataManager.SaveVoiceData();
        Debug.Log("删除所有录音成功: " + count);
    }

    public List<VoiceRecording> GetPlayerRecordings(string playerID)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            InitializePlayerVoiceData(playerID);
        }
        return voiceData.PlayerVoiceData[playerID].Recordings;
    }

    public List<VoiceRecording> GetPlayerTranscriptions(string playerID)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            InitializePlayerVoiceData(playerID);
        }
        return voiceData.PlayerVoiceData[playerID].Recordings.FindAll(r => r.IsTranscribed);
    }

    public TranscriptionConfig GetTranscriptionConfig(string playerID)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            InitializePlayerVoiceData(playerID);
        }
        return voiceData.PlayerVoiceData[playerID].VoiceConfig;
    }

    public PlayerVoiceData GetPlayerVoiceData(string playerID)
    {
        if (!voiceData.PlayerVoiceData.ContainsKey(playerID))
        {
            InitializePlayerVoiceData(playerID);
        }
        return voiceData.PlayerVoiceData[playerID];
    }

    public VoiceRecording GetRecording(string recordingID)
    {
        foreach (PlayerVoiceData playerVoiceData in voiceData.PlayerVoiceData.Values)
        {
            VoiceRecording recording = playerVoiceData.Recordings.Find(r => r.RecordingID == recordingID);
            if (recording != null)
            {
                return recording;
            }
        }
        return null;
    }

    public List<string> GetAvailableLanguages()
    {
        return voiceData.AvailableLanguages;
    }

    public void AddLanguage(string languageCode)
    {
        if (!voiceData.AvailableLanguages.Contains(languageCode))
        {
            voiceData.AvailableLanguages.Add(languageCode);
            dataManager.SaveVoiceData();
            Debug.Log("添加语言成功: " + languageCode);
        }
    }

    public void RemoveLanguage(string languageCode)
    {
        if (languageCode != "zh-CN" && voiceData.AvailableLanguages.Contains(languageCode))
        {
            voiceData.AvailableLanguages.Remove(languageCode);
            dataManager.SaveVoiceData();
            Debug.Log("删除语言成功: " + languageCode);
        }
    }

    public void CleanupOldRecordings(int days = 7)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        int totalDeleted = 0;
        
        foreach (PlayerVoiceData playerVoiceData in voiceData.PlayerVoiceData.Values)
        {
            List<VoiceRecording> oldRecordings = playerVoiceData.Recordings.FindAll(r => r.RecordingTime < cutoffDate);
            foreach (VoiceRecording recording in oldRecordings)
            {
                playerVoiceData.Recordings.Remove(recording);
                playerVoiceData.TotalRecordings = Math.Max(0, playerVoiceData.TotalRecordings - 1);
                if (recording.IsTranscribed)
                {
                    playerVoiceData.TotalTranscriptions = Math.Max(0, playerVoiceData.TotalTranscriptions - 1);
                }
                totalDeleted++;
            }
        }
        
        if (totalDeleted > 0)
        {
            dataManager.CreateVoiceEvent("recording_cleanup", "system", "", "清理旧录音: " + totalDeleted);
            dataManager.SaveVoiceData();
            Debug.Log("清理旧录音成功: " + totalDeleted);
        }
    }

    private string ProcessTranscription(string content, TranscriptionConfig config)
    {
        string result = content;
        
        if (config.EnableProfanityFilter)
        {
            result = FilterProfanity(result);
        }
        
        if (config.EnableTranslation && config.TargetLanguage != config.Language)
        {
            result = TranslateText(result, config.Language, config.TargetLanguage);
        }
        
        return result;
    }

    private string FilterProfanity(string text)
    {
        
        return text;
    }

    private string TranslateText(string text, string sourceLanguage, string targetLanguage)
    {
        
        return text;
    }

    private float CalculateConfidence(string original, string transcribed)
    {
        
        return 0.8f;
    }

    public void SaveData()
    {
        dataManager.SaveVoiceData();
    }

    public void LoadData()
    {
        dataManager.LoadVoiceData();
    }

    public List<VoiceEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
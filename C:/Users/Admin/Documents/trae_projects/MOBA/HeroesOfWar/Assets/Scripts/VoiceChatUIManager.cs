using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VoiceChatUIManager : MonoBehaviour
{
    public static VoiceChatUIManager Instance { get; private set; }
    
    public Canvas voiceChatCanvas;
    public Text voiceChatStatusText;
    public Text pushToTalkText;
    public Toggle voiceChatToggle;
    public Button pushToTalkButton;
    public Transform playerVoiceStatusContent;
    public GameObject playerVoiceStatusPrefab;
    
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
        voiceChatCanvas.gameObject.SetActive(false);
        voiceChatToggle.isOn = VoiceChatManager.Instance.isVoiceChatEnabled;
        
        // 添加按钮事件
        voiceChatToggle.onValueChanged.AddListener(OnVoiceChatToggle);
        pushToTalkButton.onClick.AddListener(OnPushToTalkButton);
        
        // 模拟按住说话
        pushToTalkButton.onPointerDown.AddListener((eventData) => VoiceChatManager.Instance.StartRecording());
        pushToTalkButton.onPointerUp.AddListener((eventData) => VoiceChatManager.Instance.StopRecording());
    }
    
    private void Update()
    {
        UpdateVoiceChatStatus();
        UpdatePlayerVoiceStatus();
    }
    
    public void OpenVoiceChatUI()
    {
        voiceChatCanvas.gameObject.SetActive(true);
    }
    
    public void CloseVoiceChatUI()
    {
        voiceChatCanvas.gameObject.SetActive(false);
    }
    
    public void OnVoiceChatToggle(bool value)
    {
        VoiceChatManager.Instance.EnableVoiceChat(value);
    }
    
    public void OnPushToTalkButton()
    {
        // 这里可以添加额外的逻辑
    }
    
    public void StartRecording()
    {
        VoiceChatManager.Instance.StartRecording();
    }
    
    public void StopRecording()
    {
        VoiceChatManager.Instance.StopRecording();
    }
    
    private void UpdateVoiceChatStatus()
    {
        if (VoiceChatManager.Instance.isVoiceChatEnabled)
        {
            voiceChatStatusText.text = "语音聊天: 已开启";
            pushToTalkButton.gameObject.SetActive(true);
        }
        else
        {
            voiceChatStatusText.text = "语音聊天: 已关闭";
            pushToTalkButton.gameObject.SetActive(false);
        }
        
        if (VoiceChatManager.Instance.isVoiceRecording)
        {
            pushToTalkText.text = "正在录音...";
            pushToTalkButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            pushToTalkText.text = "按住说话";
            pushToTalkButton.GetComponent<Image>().color = Color.white;
        }
    }
    
    private void UpdatePlayerVoiceStatus()
    {
        // 清空现有内容
        foreach (Transform child in playerVoiceStatusContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示玩家语音状态
        Dictionary<string, bool> playerStatus = VoiceChatManager.Instance.GetAllPlayerVoiceStatus();
        foreach (KeyValuePair<string, bool> status in playerStatus)
        {
            GameObject statusObj = Instantiate(playerVoiceStatusPrefab, playerVoiceStatusContent);
            Text[] texts = statusObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 1)
            {
                texts[0].text = status.Key;
            }
            
            Image[] images = statusObj.GetComponentsInChildren<Image>();
            if (images.Length >= 1)
            {
                if (status.Value)
                {
                    images[0].color = Color.green;
                }
                else
                {
                    images[0].color = Color.gray;
                }
            }
        }
    }
}
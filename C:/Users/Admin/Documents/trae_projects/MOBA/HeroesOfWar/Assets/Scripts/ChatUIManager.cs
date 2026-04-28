using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChatUIManager : MonoBehaviour
{
    public static ChatUIManager Instance { get; private set; }
    
    public ScrollRect chatScrollRect;
    public InputField chatInputField;
    public Transform chatContent;
    public Text channelText;
    
    public GameObject messagePrefab;
    
    private ChatManager.ChatChannel currentChannel = ChatManager.ChatChannel.All;
    private List<ChatMessage> displayedMessages = new List<ChatMessage>();
    
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
        // 设置输入字段的提交事件
        if (chatInputField != null)
        {
            chatInputField.onSubmit.AddListener(OnSubmitMessage);
        }
        
        // 初始显示最近的消息
        UpdateChatDisplay();
    }
    
    private void Update()
    {
        // 每帧更新聊天显示
        UpdateChatDisplay();
    }
    
    private void OnSubmitMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            // 发送消息
            ChatManager.Instance.SendMessage(message, "Player", currentChannel);
            
            // 清空输入字段
            chatInputField.text = "";
            
            // 更新显示
            UpdateChatDisplay();
        }
    }
    
    private void UpdateChatDisplay()
    {
        List<ChatMessage> messages = ChatManager.Instance.GetRecentMessages(20, currentChannel);
        
        // 清除旧消息
        foreach (Transform child in chatContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示新消息
        foreach (ChatMessage message in messages)
        {
            GameObject messageObj = Instantiate(messagePrefab, chatContent);
            Text messageText = messageObj.GetComponent<Text>();
            if (messageText != null)
            {
                string channelPrefix = "";
                switch (message.channel)
                {
                    case ChatManager.ChatChannel.All:
                        channelPrefix = "[全部] ";
                        break;
                    case ChatManager.ChatChannel.Team:
                        channelPrefix = "[队伍] ";
                        break;
                    case ChatManager.ChatChannel.Friend:
                        channelPrefix = "[好友] ";
                        break;
                }
                
                messageText.text = $"{channelPrefix}<color=yellow>{message.sender}</color>: {message.message}";
            }
        }
        
        // 滚动到底部
        if (chatScrollRect != null)
        {
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }
    
    public void SwitchChannel(ChatManager.ChatChannel channel)
    {
        currentChannel = channel;
        if (channelText != null)
        {
            switch (channel)
            {
                case ChatManager.ChatChannel.All:
                    channelText.text = "全部频道";
                    break;
                case ChatManager.ChatChannel.Team:
                    channelText.text = "队伍频道";
                    break;
                case ChatManager.ChatChannel.Friend:
                    channelText.text = "好友频道";
                    break;
            }
        }
        UpdateChatDisplay();
    }
    
    public void SwitchToAllChannel()
    {
        SwitchChannel(ChatManager.ChatChannel.All);
    }
    
    public void SwitchToTeamChannel()
    {
        SwitchChannel(ChatManager.ChatChannel.Team);
    }
    
    public void SwitchToFriendChannel()
    {
        SwitchChannel(ChatManager.ChatChannel.Friend);
    }
}
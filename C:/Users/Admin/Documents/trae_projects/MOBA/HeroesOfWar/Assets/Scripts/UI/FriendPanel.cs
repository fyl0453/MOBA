using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FriendPanel : UIPanel
{
    [SerializeField] private GameObject friendListPanel;
    [SerializeField] private GameObject friendRequestPanel;
    [SerializeField] private GameObject addFriendPanel;
    
    [SerializeField] private Button friendListButton;
    [SerializeField] private Button friendRequestButton;
    [SerializeField] private Button addFriendButton;
    [SerializeField] private Button backButton;
    
    // 好友列表
    [SerializeField] private GameObject friendItemPrefab;
    [SerializeField] private Transform friendListContent;
    
    // 好友请求
    [SerializeField] private GameObject requestItemPrefab;
    [SerializeField] private Transform requestListContent;
    
    // 添加好友
    [SerializeField] private InputField friendNameInput;
    [SerializeField] private Button sendRequestButton;
    
    private List<FriendItem> friendItems = new List<FriendItem>();
    private List<RequestItem> requestItems = new List<RequestItem>();
    
    private void Start()
    {
        friendListButton.onClick.AddListener(() => SwitchPanel("FriendList"));
        friendRequestButton.onClick.AddListener(() => SwitchPanel("FriendRequest"));
        addFriendButton.onClick.AddListener(() => SwitchPanel("AddFriend"));
        backButton.onClick.AddListener(OnBack);
        sendRequestButton.onClick.AddListener(OnSendRequest);
        
        SwitchPanel("FriendList");
    }
    
    private void SwitchPanel(string panelName)
    {
        friendListPanel.SetActive(panelName == "FriendList");
        friendRequestPanel.SetActive(panelName == "FriendRequest");
        addFriendPanel.SetActive(panelName == "AddFriend");
        
        if (panelName == "FriendList")
        {
            RefreshFriendList();
        }
        else if (panelName == "FriendRequest")
        {
            RefreshFriendRequests();
        }
    }
    
    private void RefreshFriendList()
    {
        // 清空现有列表
        foreach (var item in friendItems)
        {
            Destroy(item.gameObject);
        }
        friendItems.Clear();
        
        // 获取好友列表
        if (AccountManager.Instance.isLoggedIn)
        {
            List<FriendData> friends = FriendSystem.Instance.GetFriends(AccountManager.Instance.currentUserID);
            
            foreach (FriendData friend in friends)
            {
                GameObject friendItemGO = Instantiate(friendItemPrefab, friendListContent);
                FriendItem friendItem = friendItemGO.GetComponent<FriendItem>();
                if (friendItem != null)
                {
                    friendItem.Setup(friend, OnRemoveFriend);
                    friendItems.Add(friendItem);
                }
            }
        }
    }
    
    private void RefreshFriendRequests()
    {
        // 清空现有列表
        foreach (var item in requestItems)
        {
            Destroy(item.gameObject);
        }
        requestItems.Clear();
        
        // 获取好友请求
        if (AccountManager.Instance.isLoggedIn)
        {
            List<FriendRequest> requests = FriendSystem.Instance.GetFriendRequests(AccountManager.Instance.currentUserID);
            
            foreach (FriendRequest request in requests)
            {
                GameObject requestItemGO = Instantiate(requestItemPrefab, requestListContent);
                RequestItem requestItem = requestItemGO.GetComponent<RequestItem>();
                if (requestItem != null)
                {
                    requestItem.Setup(request, OnAcceptRequest, OnRejectRequest);
                    requestItems.Add(requestItem);
                }
            }
        }
    }
    
    private void OnRemoveFriend(FriendData friend)
    {
        if (AccountManager.Instance.isLoggedIn)
        {
            FriendSystem.Instance.RemoveFriend(AccountManager.Instance.currentUserID, friend.friendID);
            RefreshFriendList();
        }
    }
    
    private void OnAcceptRequest(FriendRequest request)
    {
        if (AccountManager.Instance.isLoggedIn)
        {
            FriendSystem.Instance.AcceptFriendRequest(request.requestID, AccountManager.Instance.currentUserID);
            RefreshFriendRequests();
            RefreshFriendList();
        }
    }
    
    private void OnRejectRequest(FriendRequest request)
    {
        if (AccountManager.Instance.isLoggedIn)
        {
            FriendSystem.Instance.RejectFriendRequest(request.requestID, AccountManager.Instance.currentUserID);
            RefreshFriendRequests();
        }
    }
    
    private void OnSendRequest()
    {
        string friendName = friendNameInput.text;
        if (!string.IsNullOrEmpty(friendName) && AccountManager.Instance.isLoggedIn)
        {
            // 这里应该根据好友名称查找用户ID
            // 暂时使用模拟的用户ID
            string friendID = "friend_" + friendName;
            FriendSystem.Instance.SendFriendRequest(AccountManager.Instance.currentUserID, AccountManager.Instance.currentUsername, friendID);
            friendNameInput.text = "";
        }
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("Social");
    }
}

public class FriendItem : MonoBehaviour
{
    [SerializeField] private Text friendNameText;
    [SerializeField] private Text statusText;
    [SerializeField] private Button removeButton;
    
    private FriendData friend;
    private System.Action<FriendData> onRemoveCallback;
    
    public void Setup(FriendData f, System.Action<FriendData> removeCallback)
    {
        friend = f;
        friendNameText.text = f.friendName;
        statusText.text = f.isOnline ? "在线" : "离线";
        onRemoveCallback = removeCallback;
        
        removeButton.onClick.AddListener(() => onRemoveCallback?.Invoke(friend));
    }
}

public class RequestItem : MonoBehaviour
{
    [SerializeField] private Text senderNameText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;
    
    private FriendRequest request;
    private System.Action<FriendRequest> onAcceptCallback;
    private System.Action<FriendRequest> onRejectCallback;
    
    public void Setup(FriendRequest r, System.Action<FriendRequest> acceptCallback, System.Action<FriendRequest> rejectCallback)
    {
        request = r;
        senderNameText.text = r.senderName;
        onAcceptCallback = acceptCallback;
        onRejectCallback = rejectCallback;
        
        acceptButton.onClick.AddListener(() => onAcceptCallback?.Invoke(request));
        rejectButton.onClick.AddListener(() => onRejectCallback?.Invoke(request));
    }
}

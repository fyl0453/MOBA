using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FriendsUIManager : MonoBehaviour
{
    public static FriendsUIManager Instance { get; private set; }
    
    public Canvas friendsCanvas;
    public ScrollRect friendsListScrollRect;
    public ScrollRect friendRequestsScrollRect;
    public Transform friendsContent;
    public Transform friendRequestsContent;
    public GameObject friendPrefab;
    public GameObject friendRequestPrefab;
    
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
        friendsCanvas.gameObject.SetActive(false);
        UpdateFriendsList();
        UpdateFriendRequests();
    }
    
    public void OpenFriendsUI()
    {
        friendsCanvas.gameObject.SetActive(true);
        UpdateFriendsList();
        UpdateFriendRequests();
    }
    
    public void CloseFriendsUI()
    {
        friendsCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateFriendsList()
    {
        // 清空现有内容
        foreach (Transform child in friendsContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示好友列表
        List<Friend> friends = SocialManager.Instance.GetFriends();
        foreach (Friend friend in friends)
        {
            GameObject friendObj = Instantiate(friendPrefab, friendsContent);
            Text[] texts = friendObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = friend.playerName;
                texts[1].text = friend.status;
            }
        }
    }
    
    public void UpdateFriendRequests()
    {
        // 清空现有内容
        foreach (Transform child in friendRequestsContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示好友请求
        List<FriendRequest> requests = SocialManager.Instance.GetFriendRequests();
        foreach (FriendRequest request in requests)
        {
            GameObject requestObj = Instantiate(friendRequestPrefab, friendRequestsContent);
            Text[] texts = requestObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 1)
            {
                texts[0].text = request.playerName;
            }
            
            // 添加接受和拒绝按钮
            Button[] buttons = requestObj.GetComponentsInChildren<Button>();
            if (buttons.Length >= 2)
            {
                buttons[0].onClick.AddListener(() => AcceptFriendRequest(request));
                buttons[1].onClick.AddListener(() => RejectFriendRequest(request));
            }
        }
    }
    
    public void AcceptFriendRequest(FriendRequest request)
    {
        SocialManager.Instance.AcceptFriendRequest(request);
        UpdateFriendRequests();
        UpdateFriendsList();
    }
    
    public void RejectFriendRequest(FriendRequest request)
    {
        SocialManager.Instance.RejectFriendRequest(request);
        UpdateFriendRequests();
    }
    
    public void OpenAddFriendUI()
    {
        // 打开添加好友的UI
        Debug.Log("打开添加好友界面");
    }
}
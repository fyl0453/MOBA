using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProfileUIManager : MonoBehaviour
{
    public static ProfileUIManager Instance { get; private set; }
    
    public Canvas profileCanvas;
    public Image avatarImage;
    public Text playerNameText;
    public Text levelText;
    public Text titleText;
    public Text signatureText;
    public Text totalGamesText;
    public Text winRateText;
    public Text kdaText;
    public ScrollRect inventoryScrollRect;
    public Transform inventoryContent;
    public GameObject heroPrefab;
    public GameObject skinPrefab;
    
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
        profileCanvas.gameObject.SetActive(false);
    }
    
    public void OpenProfileUI()
    {
        profileCanvas.gameObject.SetActive(true);
        UpdateProfileInfo();
    }
    
    public void CloseProfileUI()
    {
        profileCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateProfileInfo()
    {
        PlayerProfile profile = ProfileManager.Instance.GetCurrentProfile();
        if (profile != null)
        {
            playerNameText.text = profile.playerName;
            levelText.text = $"等级: {profile.level}";
            titleText.text = profile.title;
            signatureText.text = profile.signature;
            totalGamesText.text = $"总场次: {profile.totalGames}";
            winRateText.text = $"胜率: {profile.winRate:F1}%";
            kdaText.text = $"KDA: {profile.kda:F2}";
            
            // 更新头像
            // 这里应该根据avatar ID加载对应的头像图片
            
            // 更新背景
            // 这里应该根据background ID加载对应的背景图片
            
            // 更新英雄和皮肤列表
            UpdateInventory();
        }
    }
    
    public void UpdateInventory()
    {
        // 清空现有内容
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }
        
        PlayerProfile profile = ProfileManager.Instance.GetCurrentProfile();
        if (profile != null)
        {
            // 显示拥有的英雄
            foreach (string heroID in profile.ownedHeroes)
            {
                GameObject heroObj = Instantiate(heroPrefab, inventoryContent);
                Text[] texts = heroObj.GetComponentsInChildren<Text>();
                if (texts.Length >= 1)
                {
                    texts[0].text = heroID;
                }
            }
            
            // 显示拥有的皮肤
            foreach (string skinID in profile.ownedSkins)
            {
                GameObject skinObj = Instantiate(skinPrefab, inventoryContent);
                Text[] texts = skinObj.GetComponentsInChildren<Text>();
                if (texts.Length >= 1)
                {
                    texts[0].text = skinID;
                }
            }
        }
    }
    
    public void EditProfile()
    {
        // 打开编辑个人资料界面
        Debug.Log("打开编辑个人资料界面");
    }
    
    public void ChangeAvatar()
    {
        // 打开更换头像界面
        Debug.Log("打开更换头像界面");
    }
    
    public void ChangeBackground()
    {
        // 打开更换背景界面
        Debug.Log("打开更换背景界面");
    }
    
    public void ChangeTitle()
    {
        // 打开更换头衔界面
        Debug.Log("打开更换头衔界面");
    }
    
    public void ChangeSignature()
    {
        // 打开更换签名界面
        Debug.Log("打开更换签名界面");
    }
}
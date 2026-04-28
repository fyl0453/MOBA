using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PersonalizationManagerExtended : MonoBehaviour
{
    public static PersonalizationManagerExtended Instance { get; private set; }
    
    public PersonalizationManagerData personalizationData;
    
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
        LoadPersonalizationData();
        
        if (personalizationData == null)
        {
            personalizationData = new PersonalizationManagerData();
            InitializeDefaultPersonalization();
        }
    }
    
    private void InitializeDefaultPersonalization()
    {
        // 创建默认头像框
        AvatarFrame defaultFrame = new AvatarFrame(
            "frame_default",
            "默认头像框",
            "默认头像框",
            "Default",
            "default_frame.png",
            true,
            false,
            "Common",
            0,
            "系统默认"
        );
        
        AvatarFrame premiumFrame = new AvatarFrame(
            "frame_premium",
            "高级头像框",
            "高级会员专属头像框",
            "Premium",
            "premium_frame.png",
            false,
            false,
            "Rare",
            1000,
            "购买"
        );
        
        AvatarFrame limitedFrame = new AvatarFrame(
            "frame_limited",
            "限定头像框",
            "限时活动限定头像框",
            "Limited",
            "limited_frame.png",
            false,
            true,
            "Epic",
            0,
            "活动获取",
            "2026-12-31"
        );
        
        personalizationData.system.AddAvatarFrame(defaultFrame);
        personalizationData.system.AddAvatarFrame(premiumFrame);
        personalizationData.system.AddAvatarFrame(limitedFrame);
        
        // 创建默认名片
        BusinessCard defaultCard = new BusinessCard(
            "card_default",
            "默认名片",
            "默认名片",
            "Default",
            "default_card.png",
            true,
            false,
            "Common",
            0,
            "系统默认"
        );
        
        BusinessCard premiumCard = new BusinessCard(
            "card_premium",
            "高级名片",
            "高级会员专属名片",
            "Premium",
            "premium_card.png",
            false,
            false,
            "Rare",
            1500,
            "购买"
        );
        
        personalizationData.system.AddBusinessCard(defaultCard);
        personalizationData.system.AddBusinessCard(premiumCard);
        
        // 创建默认个人主页装饰
        HomePageDecoration defaultDecoration = new HomePageDecoration(
            "decoration_default",
            "默认背景",
            "默认个人主页背景",
            "Background",
            "default_bg.png",
            true,
            false,
            "Common",
            0,
            "系统默认"
        );
        
        HomePageDecoration premiumDecoration = new HomePageDecoration(
            "decoration_premium",
            "高级背景",
            "高级会员专属背景",
            "Background",
            "premium_bg.png",
            false,
            false,
            "Rare",
            2000,
            "购买"
        );
        
        personalizationData.system.AddHomePageDecoration(defaultDecoration);
        personalizationData.system.AddHomePageDecoration(premiumDecoration);
        
        SavePersonalizationData();
    }
    
    public void InitializePlayerPersonalization(string playerID, string playerName)
    {
        PlayerPersonalization existing = personalizationData.system.GetPlayerPersonalization(playerID);
        if (existing == null)
        {
            PlayerPersonalization newPersonalization = new PlayerPersonalization(playerID, playerName);
            
            // 给新玩家添加默认物品
            newPersonalization.AddAvatarFrame("frame_default");
            newPersonalization.AddBusinessCard("card_default");
            newPersonalization.AddHomePageDecoration("decoration_default");
            
            // 设置默认物品
            newPersonalization.SetAvatarFrame("frame_default");
            newPersonalization.SetBusinessCard("card_default");
            newPersonalization.SetHomePageDecoration("decoration_default");
            
            personalizationData.system.AddPlayerPersonalization(newPersonalization);
            SavePersonalizationData();
            Debug.Log($"成功初始化玩家个性化: {playerName}");
        }
    }
    
    public string CreateAvatarFrame(string name, string description, string type, string url, bool isDefault, bool isLimited, string rarity, int price, string obtainMethod, string expiryDate = "")
    {
        string frameID = System.Guid.NewGuid().ToString();
        AvatarFrame newFrame = new AvatarFrame(frameID, name, description, type, url, isDefault, isLimited, rarity, price, obtainMethod, expiryDate);
        personalizationData.system.AddAvatarFrame(newFrame);
        SavePersonalizationData();
        Debug.Log($"成功创建头像框: {name}");
        return frameID;
    }
    
    public string CreateBusinessCard(string name, string description, string type, string url, bool isDefault, bool isLimited, string rarity, int price, string obtainMethod, string expiryDate = "")
    {
        string cardID = System.Guid.NewGuid().ToString();
        BusinessCard newCard = new BusinessCard(cardID, name, description, type, url, isDefault, isLimited, rarity, price, obtainMethod, expiryDate);
        personalizationData.system.AddBusinessCard(newCard);
        SavePersonalizationData();
        Debug.Log($"成功创建名片: {name}");
        return cardID;
    }
    
    public string CreateHomePageDecoration(string name, string description, string type, string url, bool isDefault, bool isLimited, string rarity, int price, string obtainMethod, string expiryDate = "")
    {
        string decorationID = System.Guid.NewGuid().ToString();
        HomePageDecoration newDecoration = new HomePageDecoration(decorationID, name, description, type, url, isDefault, isLimited, rarity, price, obtainMethod, expiryDate);
        personalizationData.system.AddHomePageDecoration(newDecoration);
        SavePersonalizationData();
        Debug.Log($"成功创建个人主页装饰: {name}");
        return decorationID;
    }
    
    public bool PurchaseAvatarFrame(string playerID, string frameID)
    {
        AvatarFrame frame = personalizationData.system.GetAvatarFrame(frameID);
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        
        if (frame != null && personalization != null)
        {
            // 检查是否已拥有
            if (personalization.ownedAvatarFrameIDs.Contains(frameID))
            {
                Debug.Log("已经拥有该头像框");
                return false;
            }
            
            // 检查价格
            if (ProfileManager.Instance.currentProfile.gems < frame.price)
            {
                Debug.Log("钻石不足");
                return false;
            }
            
            // 扣除钻石
            ProfileManager.Instance.currentProfile.gems -= frame.price;
            ProfileManager.Instance.SaveProfile();
            
            // 添加到拥有列表
            personalization.AddAvatarFrame(frameID);
            SavePersonalizationData();
            Debug.Log($"成功购买头像框: {frame.frameName}");
            return true;
        }
        
        return false;
    }
    
    public bool PurchaseBusinessCard(string playerID, string cardID)
    {
        BusinessCard card = personalizationData.system.GetBusinessCard(cardID);
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        
        if (card != null && personalization != null)
        {
            // 检查是否已拥有
            if (personalization.ownedBusinessCardIDs.Contains(cardID))
            {
                Debug.Log("已经拥有该名片");
                return false;
            }
            
            // 检查价格
            if (ProfileManager.Instance.currentProfile.gems < card.price)
            {
                Debug.Log("钻石不足");
                return false;
            }
            
            // 扣除钻石
            ProfileManager.Instance.currentProfile.gems -= card.price;
            ProfileManager.Instance.SaveProfile();
            
            // 添加到拥有列表
            personalization.AddBusinessCard(cardID);
            SavePersonalizationData();
            Debug.Log($"成功购买名片: {card.cardName}");
            return true;
        }
        
        return false;
    }
    
    public bool PurchaseHomePageDecoration(string playerID, string decorationID)
    {
        HomePageDecoration decoration = personalizationData.system.GetHomePageDecoration(decorationID);
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        
        if (decoration != null && personalization != null)
        {
            // 检查是否已拥有
            if (personalization.ownedHomePageDecorationIDs.Contains(decorationID))
            {
                Debug.Log("已经拥有该个人主页装饰");
                return false;
            }
            
            // 检查价格
            if (ProfileManager.Instance.currentProfile.gems < decoration.price)
            {
                Debug.Log("钻石不足");
                return false;
            }
            
            // 扣除钻石
            ProfileManager.Instance.currentProfile.gems -= decoration.price;
            ProfileManager.Instance.SaveProfile();
            
            // 添加到拥有列表
            personalization.AddHomePageDecoration(decorationID);
            SavePersonalizationData();
            Debug.Log($"成功购买个人主页装饰: {decoration.decorationName}");
            return true;
        }
        
        return false;
    }
    
    public void EquipAvatarFrame(string playerID, string frameID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.SetAvatarFrame(frameID);
            SavePersonalizationData();
            Debug.Log("成功装备头像框");
        }
    }
    
    public void EquipBusinessCard(string playerID, string cardID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.SetBusinessCard(cardID);
            SavePersonalizationData();
            Debug.Log("成功装备名片");
        }
    }
    
    public void EquipHomePageDecoration(string playerID, string decorationID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.SetHomePageDecoration(decorationID);
            SavePersonalizationData();
            Debug.Log("成功装备个人主页装饰");
        }
    }
    
    public List<AvatarFrame> GetAvailableAvatarFrames(string playerID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization == null)
        {
            InitializePlayerPersonalization(playerID, "Player");
            personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        }
        
        List<AvatarFrame> availableFrames = new List<AvatarFrame>();
        foreach (string frameID in personalization.ownedAvatarFrameIDs)
        {
            AvatarFrame frame = personalizationData.system.GetAvatarFrame(frameID);
            if (frame != null)
            {
                availableFrames.Add(frame);
            }
        }
        return availableFrames;
    }
    
    public List<BusinessCard> GetAvailableBusinessCards(string playerID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization == null)
        {
            InitializePlayerPersonalization(playerID, "Player");
            personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        }
        
        List<BusinessCard> availableCards = new List<BusinessCard>();
        foreach (string cardID in personalization.ownedBusinessCardIDs)
        {
            BusinessCard card = personalizationData.system.GetBusinessCard(cardID);
            if (card != null)
            {
                availableCards.Add(card);
            }
        }
        return availableCards;
    }
    
    public List<HomePageDecoration> GetAvailableHomePageDecorations(string playerID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization == null)
        {
            InitializePlayerPersonalization(playerID, "Player");
            personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        }
        
        List<HomePageDecoration> availableDecorations = new List<HomePageDecoration>();
        foreach (string decorationID in personalization.ownedHomePageDecorationIDs)
        {
            HomePageDecoration decoration = personalizationData.system.GetHomePageDecoration(decorationID);
            if (decoration != null)
            {
                availableDecorations.Add(decoration);
            }
        }
        return availableDecorations;
    }
    
    public PlayerPersonalization GetPlayerPersonalization(string playerID)
    {
        return personalizationData.system.GetPlayerPersonalization(playerID);
    }
    
    public AvatarFrame GetAvatarFrame(string frameID)
    {
        return personalizationData.system.GetAvatarFrame(frameID);
    }
    
    public BusinessCard GetBusinessCard(string cardID)
    {
        return personalizationData.system.GetBusinessCard(cardID);
    }
    
    public HomePageDecoration GetHomePageDecoration(string decorationID)
    {
        return personalizationData.system.GetHomePageDecoration(decorationID);
    }
    
    public List<AvatarFrame> GetAllAvatarFrames()
    {
        return personalizationData.system.avatarFrames;
    }
    
    public List<BusinessCard> GetAllBusinessCards()
    {
        return personalizationData.system.businessCards;
    }
    
    public List<HomePageDecoration> GetAllHomePageDecorations()
    {
        return personalizationData.system.homePageDecorations;
    }
    
    public void SavePersonalizationData()
    {
        string path = Application.dataPath + "/Data/personalization_system_extended_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, personalizationData);
        stream.Close();
    }
    
    public void LoadPersonalizationData()
    {
        string path = Application.dataPath + "/Data/personalization_system_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            personalizationData = (PersonalizationManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            personalizationData = new PersonalizationManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PersonalizationManager : MonoBehaviour
{
    public static PersonalizationManager Instance { get; private set; }
    
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
        AvatarFrame defaultFrame = new AvatarFrame("frame_default", "默认头像框", "默认头像框", "Default", 0);
        defaultFrame.SetDefault(true);
        
        AvatarFrame springFrame = new AvatarFrame("frame_spring", "新春头像框", "2024新春限定头像框", "Limited", 188, "Gems");
        springFrame.SetLimited(true, "2024-01-01 to 2024-02-01");
        
        AvatarFrame tournamentFrame = new AvatarFrame("frame_tournament", "赛事头像框", "赛事限定头像框", "Event", 288, "Gems");
        
        personalizationData.system.AddAvatarFrame(defaultFrame);
        personalizationData.system.AddAvatarFrame(springFrame);
        personalizationData.system.AddAvatarFrame(tournamentFrame);
        
        // 创建默认名片
        BusinessCard defaultCard = new BusinessCard("card_default", "默认名片", "默认名片", "Default", 0);
        defaultCard.SetDefault(true);
        
        BusinessCard springCard = new BusinessCard("card_spring", "新春名片", "2024新春限定名片", "Limited", 288, "Gems");
        springCard.SetLimited(true, "2024-01-01 to 2024-02-01");
        
        BusinessCard tournamentCard = new BusinessCard("card_tournament", "赛事名片", "赛事限定名片", "Event", 388, "Gems");
        
        personalizationData.system.AddBusinessCard(defaultCard);
        personalizationData.system.AddBusinessCard(springCard);
        personalizationData.system.AddBusinessCard(tournamentCard);
        
        // 创建默认个人主页装饰
        ProfileDecoration decoration1 = new ProfileDecoration("decoration_spring_tree", "新春树", "新春主题装饰", "Decoration", 188, "Gems");
        ProfileDecoration decoration2 = new ProfileDecoration("decoration_trophy", "奖杯", "赛事奖杯装饰", "Decoration", 288, "Gems");
        ProfileDecoration decoration3 = new ProfileDecoration("decoration_hero", "英雄雕像", "英雄主题装饰", "Decoration", 388, "Gems");
        
        personalizationData.system.AddProfileDecoration(decoration1);
        personalizationData.system.AddProfileDecoration(decoration2);
        personalizationData.system.AddProfileDecoration(decoration3);
        
        SavePersonalizationData();
    }
    
    public void PurchaseAvatarFrame(string playerID, string frameID)
    {
        AvatarFrame frame = personalizationData.system.GetAvatarFrame(frameID);
        if (frame != null && !frame.isDefault)
        {
            PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否已经拥有
            if (!playerData.OwnsAvatarFrame(frameID))
            {
                // 检查货币是否足够
                if (frame.currencyType == "Gems")
                {
                    if (ProfileManager.Instance.currentProfile.gems >= frame.price)
                    {
                        ProfileManager.Instance.currentProfile.gems -= frame.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddAvatarFrame(frameID);
                        SavePersonalizationData();
                        
                        Debug.Log($"成功购买头像框: {frame.frameName}");
                    }
                    else
                    {
                        Debug.Log("钻石不足");
                    }
                }
                else if (frame.currencyType == "Gold")
                {
                    if (ProfileManager.Instance.currentProfile.gold >= frame.price)
                    {
                        ProfileManager.Instance.currentProfile.gold -= frame.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddAvatarFrame(frameID);
                        SavePersonalizationData();
                        
                        Debug.Log($"成功购买头像框: {frame.frameName}");
                    }
                    else
                    {
                        Debug.Log("金币不足");
                    }
                }
            }
            else
            {
                Debug.Log("已经拥有该头像框");
            }
        }
    }
    
    public void PurchaseBusinessCard(string playerID, string cardID)
    {
        BusinessCard card = personalizationData.system.GetBusinessCard(cardID);
        if (card != null && !card.isDefault)
        {
            PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否已经拥有
            if (!playerData.OwnsBusinessCard(cardID))
            {
                // 检查货币是否足够
                if (card.currencyType == "Gems")
                {
                    if (ProfileManager.Instance.currentProfile.gems >= card.price)
                    {
                        ProfileManager.Instance.currentProfile.gems -= card.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddBusinessCard(cardID);
                        SavePersonalizationData();
                        
                        Debug.Log($"成功购买名片: {card.cardName}");
                    }
                    else
                    {
                        Debug.Log("钻石不足");
                    }
                }
                else if (card.currencyType == "Gold")
                {
                    if (ProfileManager.Instance.currentProfile.gold >= card.price)
                    {
                        ProfileManager.Instance.currentProfile.gold -= card.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddBusinessCard(cardID);
                        SavePersonalizationData();
                        
                        Debug.Log($"成功购买名片: {card.cardName}");
                    }
                    else
                    {
                        Debug.Log("金币不足");
                    }
                }
            }
            else
            {
                Debug.Log("已经拥有该名片");
            }
        }
    }
    
    public void PurchaseDecoration(string playerID, string decorationID)
    {
        ProfileDecoration decoration = personalizationData.system.GetProfileDecoration(decorationID);
        if (decoration != null)
        {
            PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否已经拥有
            if (!playerData.OwnsDecoration(decorationID))
            {
                // 检查货币是否足够
                if (decoration.currencyType == "Gems")
                {
                    if (ProfileManager.Instance.currentProfile.gems >= decoration.price)
                    {
                        ProfileManager.Instance.currentProfile.gems -= decoration.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddDecoration(decorationID);
                        SavePersonalizationData();
                        
                        Debug.Log($"成功购买装饰: {decoration.decorationName}");
                    }
                    else
                    {
                        Debug.Log("钻石不足");
                    }
                }
                else if (decoration.currencyType == "Gold")
                {
                    if (ProfileManager.Instance.currentProfile.gold >= decoration.price)
                    {
                        ProfileManager.Instance.currentProfile.gold -= decoration.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddDecoration(decorationID);
                        SavePersonalizationData();
                        
                        Debug.Log($"成功购买装饰: {decoration.decorationName}");
                    }
                    else
                    {
                        Debug.Log("金币不足");
                    }
                }
            }
            else
            {
                Debug.Log("已经拥有该装饰");
            }
        }
    }
    
    public void EquipAvatarFrame(string playerID, string frameID)
    {
        AvatarFrame frame = personalizationData.system.GetAvatarFrame(frameID);
        if (frame != null)
        {
            PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否拥有该头像框
            if (playerData.OwnsAvatarFrame(frameID) || frame.isDefault)
            {
                playerData.EquipAvatarFrame(frameID);
                SavePersonalizationData();
                Debug.Log($"成功装备头像框: {frame.frameName}");
            }
            else
            {
                Debug.Log("未拥有该头像框");
            }
        }
    }
    
    public void EquipBusinessCard(string playerID, string cardID)
    {
        BusinessCard card = personalizationData.system.GetBusinessCard(cardID);
        if (card != null)
        {
            PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否拥有该名片
            if (playerData.OwnsBusinessCard(cardID) || card.isDefault)
            {
                playerData.EquipBusinessCard(cardID);
                SavePersonalizationData();
                Debug.Log($"成功装备名片: {card.cardName}");
            }
            else
            {
                Debug.Log("未拥有该名片");
            }
        }
    }
    
    public void PlaceDecoration(string playerID, string position, string decorationID)
    {
        ProfileDecoration decoration = personalizationData.system.GetProfileDecoration(decorationID);
        if (decoration != null)
        {
            PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否拥有该装饰
            if (playerData.OwnsDecoration(decorationID))
            {
                playerData.PlaceDecoration(position, decorationID);
                SavePersonalizationData();
                Debug.Log($"成功放置装饰: {decoration.decorationName} 到位置: {position}");
            }
            else
            {
                Debug.Log("未拥有该装饰");
            }
        }
    }
    
    public void RemoveDecoration(string playerID, string position)
    {
        PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
        playerData.RemoveDecoration(position);
        SavePersonalizationData();
        Debug.Log($"成功移除位置: {position} 的装饰");
    }
    
    public List<AvatarFrame> GetAvatarFrames()
    {
        return personalizationData.system.avatarFrames;
    }
    
    public List<BusinessCard> GetBusinessCards()
    {
        return personalizationData.system.businessCards;
    }
    
    public List<ProfileDecoration> GetProfileDecorations()
    {
        return personalizationData.system.profileDecorations;
    }
    
    public List<AvatarFrame> GetOwnedAvatarFrames(string playerID)
    {
        PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
        List<AvatarFrame> ownedFrames = new List<AvatarFrame>();
        
        foreach (AvatarFrame frame in personalizationData.system.avatarFrames)
        {
            if (playerData.OwnsAvatarFrame(frame.frameID) || frame.isDefault)
            {
                ownedFrames.Add(frame);
            }
        }
        
        return ownedFrames;
    }
    
    public List<BusinessCard> GetOwnedBusinessCards(string playerID)
    {
        PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
        List<BusinessCard> ownedCards = new List<BusinessCard>();
        
        foreach (BusinessCard card in personalizationData.system.businessCards)
        {
            if (playerData.OwnsBusinessCard(card.cardID) || card.isDefault)
            {
                ownedCards.Add(card);
            }
        }
        
        return ownedCards;
    }
    
    public List<ProfileDecoration> GetOwnedDecorations(string playerID)
    {
        PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
        List<ProfileDecoration> ownedDecorations = new List<ProfileDecoration>();
        
        foreach (ProfileDecoration decoration in personalizationData.system.profileDecorations)
        {
            if (playerData.OwnsDecoration(decoration.decorationID))
            {
                ownedDecorations.Add(decoration);
            }
        }
        
        return ownedDecorations;
    }
    
    public string GetEquippedAvatarFrame(string playerID)
    {
        PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
        return playerData.equippedAvatarFrame;
    }
    
    public string GetEquippedBusinessCard(string playerID)
    {
        PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
        return playerData.equippedBusinessCard;
    }
    
    public string GetDecorationAtPosition(string playerID, string position)
    {
        PlayerPersonalizationData playerData = GetOrCreatePlayerData(playerID);
        return playerData.GetDecorationAtPosition(position);
    }
    
    private PlayerPersonalizationData GetOrCreatePlayerData(string playerID)
    {
        PlayerPersonalizationData playerData = personalizationData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerPersonalizationData(playerID);
            
            // 自动添加默认头像框和名片
            foreach (AvatarFrame frame in personalizationData.system.avatarFrames)
            {
                if (frame.isDefault)
                {
                    playerData.AddAvatarFrame(frame.frameID);
                    if (string.IsNullOrEmpty(playerData.equippedAvatarFrame))
                    {
                        playerData.EquipAvatarFrame(frame.frameID);
                    }
                }
            }
            
            foreach (BusinessCard card in personalizationData.system.businessCards)
            {
                if (card.isDefault)
                {
                    playerData.AddBusinessCard(card.cardID);
                    if (string.IsNullOrEmpty(playerData.equippedBusinessCard))
                    {
                        playerData.EquipBusinessCard(card.cardID);
                    }
                }
            }
            
            personalizationData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public void SavePersonalizationData()
    {
        string path = Application.dataPath + "/Data/personalization_data.dat";
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
        string path = Application.dataPath + "/Data/personalization_data.dat";
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
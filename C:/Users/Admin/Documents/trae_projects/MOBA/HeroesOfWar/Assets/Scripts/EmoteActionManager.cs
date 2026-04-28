using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EmoteActionManager : MonoBehaviour
{
    public static EmoteActionManager Instance { get; private set; }
    
    public EmoteActionManagerData emoteData;
    
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
        LoadEmoteData();
        
        if (emoteData == null)
        {
            emoteData = new EmoteActionManagerData();
            InitializeDefaultEmotes();
        }
    }
    
    private void InitializeDefaultEmotes()
    {
        // 创建动作分类
        EmoteCategory庆祝 = new EmoteCategory("category_celebration", "庆祝", "庆祝胜利或精彩操作");
        EmoteCategory嘲讽 = new EmoteCategory("category_taunt", "嘲讽", "嘲讽敌方英雄");
        EmoteCategory感谢 = new EmoteCategory("category_thanks", "感谢", "感谢队友的帮助");
        EmoteCategory互动 = new EmoteCategory("category_interaction", "互动", "与队友或敌方互动");
        
        emoteData.system.AddCategory(庆祝);
        emoteData.system.AddCategory(嘲讽);
        emoteData.system.AddCategory(感谢);
        emoteData.system.AddCategory(互动);
        
        // 创建默认动作
        EmoteAction defaultCelebration = new EmoteAction("emote_celebration_default", "默认庆祝", "默认的庆祝动作", "category_celebration");
        defaultCelebration.SetDefault(true);
        
        EmoteAction defaultTaunt = new EmoteAction("emote_taunt_default", "默认嘲讽", "默认的嘲讽动作", "category_taunt");
        defaultTaunt.SetDefault(true);
        
        EmoteAction defaultThanks = new EmoteAction("emote_thanks_default", "默认感谢", "默认的感谢动作", "category_thanks");
        defaultThanks.SetDefault(true);
        
        EmoteAction defaultInteraction = new EmoteAction("emote_interaction_default", "默认互动", "默认的互动动作", "category_interaction");
        defaultInteraction.SetDefault(true);
        
        // 创建关羽专属动作
        EmoteAction guanyuVictory = new EmoteAction("emote_guanyu_victory", "关羽胜利", "关羽的胜利庆祝动作", "category_celebration", "hero_guanyu", "关羽", 188, "Gems");
        EmoteAction guanyuTaunt = new EmoteAction("emote_guanyu_taunt", "关羽嘲讽", "关羽的嘲讽动作", "category_taunt", "hero_guanyu", "关羽", 188, "Gems");
        
        // 创建诸葛亮专属动作
        EmoteAction zhugeliangVictory = new EmoteAction("emote_zhugeliang_victory", "诸葛亮胜利", "诸葛亮的胜利庆祝动作", "category_celebration", "hero_zhugeliang", "诸葛亮", 188, "Gems");
        EmoteAction zhugeliangTaunt = new EmoteAction("emote_zhugeliang_taunt", "诸葛亮嘲讽", "诸葛亮的嘲讽动作", "category_taunt", "hero_zhugeliang", "诸葛亮", 188, "Gems");
        
        // 创建限定动作
        EmoteAction springCelebration = new EmoteAction("emote_spring_celebration", "新春庆祝", "2024新春限定庆祝动作", "category_celebration", "", "", 288, "Gems");
        springCelebration.SetLimited(true, "2024-01-01 to 2024-02-01");
        
        EmoteAction tournamentCelebration = new EmoteAction("emote_tournament_celebration", "赛事庆祝", "赛事限定庆祝动作", "category_celebration", "", "", 388, "Gems");
        
        emoteData.system.AddEmoteAction(defaultCelebration);
        emoteData.system.AddEmoteAction(defaultTaunt);
        emoteData.system.AddEmoteAction(defaultThanks);
        emoteData.system.AddEmoteAction(defaultInteraction);
        emoteData.system.AddEmoteAction(guanyuVictory);
        emoteData.system.AddEmoteAction(guanyuTaunt);
        emoteData.system.AddEmoteAction(zhugeliangVictory);
        emoteData.system.AddEmoteAction(zhugeliangTaunt);
        emoteData.system.AddEmoteAction(springCelebration);
        emoteData.system.AddEmoteAction(tournamentCelebration);
        
        SaveEmoteData();
    }
    
    public void PurchaseEmote(string playerID, string emoteID)
    {
        EmoteAction emote = emoteData.system.GetEmoteAction(emoteID);
        if (emote != null && !emote.isDefault)
        {
            PlayerEmoteData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否已经拥有
            if (!playerData.OwnsEmote(emoteID))
            {
                // 检查货币是否足够
                if (emote.currencyType == "Gems")
                {
                    if (ProfileManager.Instance.currentProfile.gems >= emote.price)
                    {
                        ProfileManager.Instance.currentProfile.gems -= emote.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddOwnedEmote(emoteID);
                        SaveEmoteData();
                        
                        Debug.Log($"成功购买动作: {emote.emoteName}");
                    }
                    else
                    {
                        Debug.Log("钻石不足");
                    }
                }
                else if (emote.currencyType == "Gold")
                {
                    if (ProfileManager.Instance.currentProfile.gold >= emote.price)
                    {
                        ProfileManager.Instance.currentProfile.gold -= emote.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddOwnedEmote(emoteID);
                        SaveEmoteData();
                        
                        Debug.Log($"成功购买动作: {emote.emoteName}");
                    }
                    else
                    {
                        Debug.Log("金币不足");
                    }
                }
            }
            else
            {
                Debug.Log("已经拥有该动作");
            }
        }
    }
    
    public void EquipEmote(string playerID, string emoteID)
    {
        EmoteAction emote = emoteData.system.GetEmoteAction(emoteID);
        if (emote != null)
        {
            PlayerEmoteData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否拥有该动作
            if (playerData.OwnsEmote(emoteID) || emote.isDefault)
            {
                playerData.EquipEmote(emoteID);
                SaveEmoteData();
                Debug.Log($"成功装备动作: {emote.emoteName}");
            }
            else
            {
                Debug.Log("未拥有该动作");
            }
        }
    }
    
    public void UnequipEmote(string playerID, string emoteID)
    {
        PlayerEmoteData playerData = GetOrCreatePlayerData(playerID);
        playerData.UnequipEmote(emoteID);
        SaveEmoteData();
        Debug.Log($"成功卸下动作: {emoteID}");
    }
    
    public List<EmoteAction> GetEmotesByCategory(string categoryID)
    {
        return emoteData.system.GetEmoteActionsByCategory(categoryID);
    }
    
    public List<EmoteAction> GetEmotesByHero(string heroID)
    {
        return emoteData.system.GetEmoteActionsByHero(heroID);
    }
    
    public List<EmoteAction> GetOwnedEmotes(string playerID, string heroID = "")
    {
        PlayerEmoteData playerData = GetOrCreatePlayerData(playerID);
        List<EmoteAction> emotes = heroID == "" ? emoteData.system.emoteActions : emoteData.system.GetEmoteActionsByHero(heroID);
        List<EmoteAction> ownedEmotes = new List<EmoteAction>();
        
        foreach (EmoteAction emote in emotes)
        {
            if (playerData.OwnsEmote(emote.emoteID) || emote.isDefault)
            {
                ownedEmotes.Add(emote);
            }
        }
        
        return ownedEmotes;
    }
    
    public List<EmoteAction> GetEquippedEmotes(string playerID)
    {
        PlayerEmoteData playerData = GetOrCreatePlayerData(playerID);
        List<EmoteAction> equippedEmotes = new List<EmoteAction>();
        
        foreach (string emoteID in playerData.GetEquippedEmotes())
        {
            EmoteAction emote = emoteData.system.GetEmoteAction(emoteID);
            if (emote != null)
            {
                equippedEmotes.Add(emote);
            }
        }
        
        return equippedEmotes;
    }
    
    public EmoteAction GetEmoteAction(string emoteID)
    {
        return emoteData.system.GetEmoteAction(emoteID);
    }
    
    public List<EmoteCategory> GetEmoteCategories()
    {
        return emoteData.system.categories;
    }
    
    public void AddEmoteAction(string name, string description, string category, string heroID = "", string heroName = "", int price = 0, string currency = "Gems")
    {
        string emoteID = System.Guid.NewGuid().ToString();
        EmoteAction emote = new EmoteAction(emoteID, name, description, category, heroID, heroName, price, currency);
        emoteData.system.AddEmoteAction(emote);
        SaveEmoteData();
    }
    
    public void AddEmoteCategory(string name, string description)
    {
        string categoryID = System.Guid.NewGuid().ToString();
        EmoteCategory category = new EmoteCategory(categoryID, name, description);
        emoteData.system.AddCategory(category);
        SaveEmoteData();
    }
    
    private PlayerEmoteData GetOrCreatePlayerData(string playerID)
    {
        PlayerEmoteData playerData = emoteData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerEmoteData(playerID);
            
            // 自动添加默认动作
            foreach (EmoteAction emote in emoteData.system.emoteActions)
            {
                if (emote.isDefault)
                {
                    playerData.AddOwnedEmote(emote.emoteID);
                    if (playerData.GetEquippedEmotes().Count < 8)
                    {
                        playerData.EquipEmote(emote.emoteID);
                    }
                }
            }
            
            emoteData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public void SaveEmoteData()
    {
        string path = Application.dataPath + "/Data/emote_action_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, emoteData);
        stream.Close();
    }
    
    public void LoadEmoteData()
    {
        string path = Application.dataPath + "/Data/emote_action_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            emoteData = (EmoteActionManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            emoteData = new EmoteActionManagerData();
        }
    }
}
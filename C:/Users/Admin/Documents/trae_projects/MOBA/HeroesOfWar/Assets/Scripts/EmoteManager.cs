using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EmoteManager : MonoBehaviour
{
    public static EmoteManager Instance { get; private set; }
    
    public List<Emote> allEmotes;
    public List<EmoteSlot> quickEmoteSlots;
    public List<Emote> ownedEmotes;
    
    private List<PlayerEmote> activeEmotes = new List<PlayerEmote>();
    private float emoteCooldown = 5f;
    private float lastEmoteTime = -999f;
    
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
        LoadOwnedEmotes();
        
        if (allEmotes.Count == 0)
        {
            InitializeDefaultEmotes();
        }
        
        if (quickEmoteSlots.Count == 0)
        {
            InitializeQuickEmoteSlots();
        }
        
        if (ownedEmotes.Count == 0)
        {
            InitializeOwnedEmotes();
        }
    }
    
    private void InitializeDefaultEmotes()
    {
        allEmotes.Add(new Emote("emote_001", "点赞", "给队友点赞", "ThumbsUp", 2f, 0, "Quick"));
        allEmotes.Add(new Emote("emote_002", "加油", "鼓励队友", "Fighting", 2f, 0, "Quick"));
        allEmotes.Add(new Emote("emote_003", "撤退", "示意队友撤退", "Retreat", 2f, 0, "Quick"));
        allEmotes.Add(new Emote("emote_004", "集合", "召集队友", "Gather", 2f, 0, "Quick"));
        allEmotes.Add(new Emote("emote_005", "我来抓人", "表示即将Gank", "Gank", 2f, 0, "Quick"));
        allEmotes.Add(new Emote("emote_006", "打龙", "示意打龙", "Objective", 2f, 0, "Quick"));
        allEmotes.Add(new Emote("emote_007", "等我", "让队友等待", "Wait", 2f, 0, "Quick"));
        allEmotes.Add(new Emote("emote_008", "抱歉", "表示歉意", "Sorry", 2f, 0, "Quick"));
        
        allEmotes.Add(new Emote("emote_009", "大笑", "开心大笑", "Laugh", 3f, 100, "Expression"));
        allEmotes.Add(new Emote("emote_010", "哭泣", "伤心哭泣", "Cry", 3f, 100, "Expression"));
        allEmotes.Add(new Emote("emote_011", "愤怒", "愤怒表情", "Angry", 3f, 100, "Expression"));
        allEmotes.Add(new Emote("emote_012", "得意", "得意洋洋", "Proud", 3f, 150, "Expression"));
        allEmotes.Add(new Emote("emote_013", "惊讶", "惊讶表情", "Surprise", 3f, 150, "Expression"));
        
        allEmotes.Add(new Emote("emote_014", "跳舞", "欢快舞蹈", "Dance", 5f, 300, "Action"));
        allEmotes.Add(new Emote("emote_015", "鼓掌", "热烈鼓掌", "Clap", 3f, 200, "Action"));
        allEmotes.Add(new Emote("emote_016", "敬礼", "军礼", "Salute", 3f, 200, "Action"));
        
        SaveEmoteData();
    }
    
    private void InitializeQuickEmoteSlots()
    {
        for (int i = 0; i < 8; i++)
        {
            quickEmoteSlots.Add(new EmoteSlot(i));
        }
        
        if (allEmotes.Count >= 4)
        {
            quickEmoteSlots[0].equippedEmoteID = allEmotes[0].emoteID;
            quickEmoteSlots[1].equippedEmoteID = allEmotes[1].emoteID;
            quickEmoteSlots[2].equippedEmoteID = allEmotes[2].emoteID;
            quickEmoteSlots[3].equippedEmoteID = allEmotes[3].emoteID;
        }
        
        SaveQuickEmoteSlots();
    }
    
    private void InitializeOwnedEmotes()
    {
        foreach (Emote emote in allEmotes)
        {
            if (emote.cost == 0)
            {
                emote.isOwned = true;
                ownedEmotes.Add(emote);
            }
        }
        SaveOwnedEmotes();
    }
    
    public void UseEmote(string emoteID, Vector3 position)
    {
        if (Time.time - lastEmoteTime < emoteCooldown)
        {
            Debug.Log("表情冷却中...");
            return;
        }
        
        Emote emote = GetEmoteByID(emoteID);
        if (emote == null) return;
        
        if (!emote.isOwned)
        {
            Debug.Log("未拥有该表情");
            return;
        }
        
        PlayerEmote playerEmote = new PlayerEmote("local_player", emoteID, position, emote.category == "Quick");
        activeEmotes.Add(playerEmote);
        lastEmoteTime = Time.time;
        
        BroadcastEmote(emoteID, position);
        PlayEmoteEffect(emote);
    }
    
    public void UseQuickEmote(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= quickEmoteSlots.Count)
        {
            return;
        }
        
        EmoteSlot slot = quickEmoteSlots[slotIndex];
        if (!string.IsNullOrEmpty(slot.equippedEmoteID))
        {
            Vector3 playerPos = Vector3.zero;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerPos = player.transform.position;
            }
            
            UseEmote(slot.equippedEmoteID, playerPos);
        }
    }
    
    public void EquipEmoteToSlot(int slotIndex, string emoteID)
    {
        Emote emote = GetEmoteByID(emoteID);
        if (emote == null || !emote.isOwned) return;
        
        if (slotIndex < 0 || slotIndex >= quickEmoteSlots.Count) return;
        
        quickEmoteSlots[slotIndex].equippedEmoteID = emoteID;
        SaveQuickEmoteSlots();
    }
    
    public void PurchaseEmote(string emoteID)
    {
        Emote emote = GetEmoteByID(emoteID);
        if (emote == null) return;
        
        if (emote.isOwned)
        {
            Debug.Log("已拥有该表情");
            return;
        }
        
        int playerGold = ProfileManager.Instance.currentProfile.gold;
        if (playerGold >= emote.cost)
        {
            ProfileManager.Instance.currentProfile.gold -= emote.cost;
            emote.isOwned = true;
            ownedEmotes.Add(emote);
            SaveOwnedEmotes();
            ProfileManager.Instance.SaveProfile();
            
            Debug.Log($"购买表情成功: {emote.emoteName}");
        }
        else
        {
            Debug.Log("金币不足");
        }
    }
    
    public void BroadcastEmote(string emoteID, Vector3 position)
    {
        Debug.Log($"广播表情: {emoteID} at {position}");
    }
    
    private void PlayEmoteEffect(Emote emote)
    {
        Debug.Log($"播放表情动画: {emote.animation}, 时长: {emote.duration}秒");
    }
    
    public List<Emote> GetOwnedEmotes()
    {
        return ownedEmotes;
    }
    
    public List<Emote> GetEmotesByCategory(string category)
    {
        return allEmotes.FindAll(e => e.category == category);
    }
    
    public Emote GetEmoteByID(string emoteID)
    {
        return allEmotes.Find(e => e.emoteID == emoteID);
    }
    
    public List<Emote> GetQuickEmotes()
    {
        List<Emote> quickEmotes = new List<Emote>();
        foreach (EmoteSlot slot in quickEmoteSlots)
        {
            if (!string.IsNullOrEmpty(slot.equippedEmoteID))
            {
                Emote emote = GetEmoteByID(slot.equippedEmoteID);
                if (emote != null)
                {
                    quickEmotes.Add(emote);
                }
            }
        }
        return quickEmotes;
    }
    
    public void Update()
    {
        float currentTime = Time.time;
        activeEmotes.RemoveAll(e => currentTime - e.timestamp > 10f);
    }
    
    public void SaveEmoteData()
    {
        string path = Application.dataPath + "/Data/emote_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, allEmotes);
        stream.Close();
    }
    
    public void LoadEmoteData()
    {
        string path = Application.dataPath + "/Data/emote_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            allEmotes = (List<Emote>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            allEmotes = new List<Emote>();
        }
    }
    
    public void SaveQuickEmoteSlots()
    {
        string path = Application.dataPath + "/Data/quick_emote_slots.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, quickEmoteSlots);
        stream.Close();
    }
    
    public void LoadQuickEmoteSlots()
    {
        string path = Application.dataPath + "/Data/quick_emote_slots.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            quickEmoteSlots = (List<EmoteSlot>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            quickEmoteSlots = new List<EmoteSlot>();
        }
    }
    
    public void SaveOwnedEmotes()
    {
        string path = Application.dataPath + "/Data/owned_emotes.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, ownedEmotes);
        stream.Close();
    }
    
    public void LoadOwnedEmotes()
    {
        string path = Application.dataPath + "/Data/owned_emotes.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            ownedEmotes = (List<Emote>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            ownedEmotes = new List<Emote>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Emote
{
    public string EmoteID;
    public string EmoteName;
    public string Description;
    public int EmoteType;
    public string AnimationName;
    public string SoundName;
    public string IconName;
    public string ParticleEffect;
    public int UnlockCondition;
    public string UnlockParam;
    public bool IsLimited;
    public DateTime LimitedEndTime;
    public int Price;
    public int Rarity;
    public DateTime CreateTime;

    public Emote(string emoteID, string emoteName, string description, int emoteType)
    {
        EmoteID = emoteID;
        EmoteName = emoteName;
        Description = description;
        EmoteType = emoteType;
        AnimationName = "";
        SoundName = "";
        IconName = "";
        ParticleEffect = "";
        UnlockCondition = 0;
        UnlockParam = "";
        IsLimited = false;
        LimitedEndTime = DateTime.MaxValue;
        Price = 0;
        Rarity = 1;
        CreateTime = DateTime.Now;
    }
}

[Serializable]
public class PlayerEmote
{
    public string PlayerID;
    public string EmoteID;
    public DateTime UnlockTime;
    public bool IsEquipped;
    public int UseCount;

    public PlayerEmote(string playerID, string emoteID)
    {
        PlayerID = playerID;
        EmoteID = emoteID;
        UnlockTime = DateTime.Now;
        IsEquipped = false;
        UseCount = 0;
    }
}

[Serializable]
public class EmoteSet
{
    public string SetID;
    public string SetName;
    public string Description;
    public List<string> EmoteIDs;
    public int SetPrice;
    public bool IsAvailable;
    public DateTime AvailableStartTime;
    public DateTime AvailableEndTime;

    public EmoteSet(string setID, string setName, string description)
    {
        SetID = setID;
        SetName = setName;
        Description = description;
        EmoteIDs = new List<string>();
        SetPrice = 0;
        IsAvailable = true;
        AvailableStartTime = DateTime.Now;
        AvailableEndTime = DateTime.MaxValue;
    }
}

[Serializable]
public class EmoteQuickSlot
{
    public string PlayerID;
    public List<string> QuickEmoteIDs;
    public string LastUsedEmoteID;

    public EmoteQuickSlot(string playerID)
    {
        PlayerID = playerID;
        QuickEmoteIDs = new List<string>();
        LastUsedEmoteID = "";
    }
}

[Serializable]
public class EmoteInteraction
{
    public string InteractionID;
    public string PlayerID;
    public string TargetPlayerID;
    public string EmoteID;
    public DateTime InteractionTime;
    public bool IsAccepted;

    public EmoteInteraction(string interactionID, string playerID, string targetPlayerID, string emoteID)
    {
        InteractionID = interactionID;
        PlayerID = playerID;
        TargetPlayerID = targetPlayerID;
        EmoteID = emoteID;
        InteractionTime = DateTime.Now;
        IsAccepted = false;
    }
}

[Serializable]
public class EmoteSystemData
{
    public List<Emote> AllEmotes;
    public Dictionary<string, List<PlayerEmote>> PlayerEmotes;
    public List<EmoteSet> EmoteSets;
    public Dictionary<string, EmoteQuickSlot> PlayerQuickSlots;
    public List<EmoteInteraction> RecentInteractions;
    public Dictionary<string, List<string>> PlayerFavoriteEmotes;
    public List<string> FeaturedEmoteIDs;
    public DateTime LastCleanupTime;

    public EmoteSystemData()
    {
        AllEmotes = new List<Emote>();
        PlayerEmotes = new Dictionary<string, List<PlayerEmote>>();
        EmoteSets = new List<EmoteSet>();
        PlayerQuickSlots = new Dictionary<string, EmoteQuickSlot>();
        RecentInteractions = new List<EmoteInteraction>();
        PlayerFavoriteEmotes = new Dictionary<string, List<string>>();
        FeaturedEmoteIDs = new List<string>();
        LastCleanupTime = DateTime.Now;
        InitializeDefaultEmotes();
    }

    private void InitializeDefaultEmotes()
    {
        Emote greeting = new Emote("emote_001", "你好", "向敌人打招呼", 1);
        greeting.Rarity = 1;
        greeting.Price = 100;
        AllEmotes.Add(greeting);

        Emote celebrate = new Emote("emote_002", "胜利", "庆祝胜利", 2);
        celebrate.Rarity = 2;
        celebrate.Price = 200;
        AllEmotes.Add(celebrate);

        Emote taunt = new Emote("emote_003", "嘲讽", "嘲讽敌人", 3);
        taunt.Rarity = 3;
        taunt.Price = 300;
        AllEmotes.Add(taunt);

        Emote dance = new Emote("emote_004", "跳舞", "快乐舞蹈", 4);
        dance.Rarity = 2;
        dance.Price = 250;
        AllEmotes.Add(dance);

        Emote bow = new Emote("emote_005", "鞠躬", "礼貌问候", 1);
        bow.Rarity = 1;
        bow.Price = 100;
        AllEmotes.Add(bow);

        EmoteSet defaultSet = new EmoteSet("set_001", "基础表情包", "包含所有基础表情");
        defaultSet.EmoteIDs.Add("emote_001");
        defaultSet.EmoteIDs.Add("emote_002");
        defaultSet.EmoteIDs.Add("emote_005");
        defaultSet.SetPrice = 0;
        EmoteSets.Add(defaultSet);
    }

    public void AddPlayerEmote(string playerID, PlayerEmote playerEmote)
    {
        if (!PlayerEmotes.ContainsKey(playerID))
        {
            PlayerEmotes[playerID] = new List<PlayerEmote>();
        }
        PlayerEmotes[playerID].Add(playerEmote);
    }

    public void AddEmoteSet(EmoteSet emoteSet)
    {
        EmoteSets.Add(emoteSet);
    }
}

[Serializable]
public class EmoteEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EmoteID;
    public string EventData;

    public EmoteEvent(string eventID, string eventType, string playerID, string emoteID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EmoteID = emoteID;
        EventData = eventData;
    }
}

public class EmoteSystemDataManager
{
    private static EmoteSystemDataManager _instance;
    public static EmoteSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EmoteSystemDataManager();
            }
            return _instance;
        }
    }

    public EmoteSystemData emoteData;
    private List<EmoteEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private EmoteSystemDataManager()
    {
        emoteData = new EmoteSystemData();
        recentEvents = new List<EmoteEvent>();
        LoadEmoteData();
    }

    public void SaveEmoteData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EmoteSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, emoteData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存表情系统数据失败: " + e.Message);
        }
    }

    public void LoadEmoteData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EmoteSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    emoteData = (EmoteSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载表情系统数据失败: " + e.Message);
            emoteData = new EmoteSystemData();
        }
    }

    public void CreateEmoteEvent(string eventType, string playerID, string emoteID, string eventData)
    {
        string eventID = "emote_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EmoteEvent emoteEvent = new EmoteEvent(eventID, eventType, playerID, emoteID, eventData);
        recentEvents.Add(emoteEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<EmoteEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
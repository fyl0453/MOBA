[System.Serializable]
public class VoicePackSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<VoicePack> voicePacks;
    public List<VoiceLine> voiceLines;
    
    public VoicePackSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        voicePacks = new List<VoicePack>();
        voiceLines = new List<VoiceLine>();
    }
    
    public void AddVoicePack(VoicePack voicePack)
    {
        voicePacks.Add(voicePack);
    }
    
    public void AddVoiceLine(VoiceLine voiceLine)
    {
        voiceLines.Add(voiceLine);
    }
    
    public VoicePack GetVoicePack(string voicePackID)
    {
        return voicePacks.Find(vp => vp.voicePackID == voicePackID);
    }
    
    public List<VoicePack> GetVoicePacksByHero(string heroID)
    {
        return voicePacks.FindAll(vp => vp.heroID == heroID);
    }
    
    public List<VoiceLine> GetVoiceLinesByPack(string voicePackID)
    {
        return voiceLines.FindAll(vl => vl.voicePackID == voicePackID);
    }
    
    public List<VoicePack> GetVoicePacksByType(string type)
    {
        return voicePacks.FindAll(vp => vp.voicePackType == type);
    }
}

[System.Serializable]
public class VoicePack
{
    public string voicePackID;
    public string heroID;
    public string heroName;
    public string voicePackName;
    public string voicePackDescription;
    public string voicePackType;
    public string voiceActor;
    public int price;
    public string currencyType;
    public string voicePackIcon;
    public bool isLimited;
    public string limitedTime;
    public bool isDefault;
    
    public VoicePack(string id, string heroID, string heroName, string name, string desc, string type, int price, string currency = "Gems")
    {
        voicePackID = id;
        this.heroID = heroID;
        this.heroName = heroName;
        voicePackName = name;
        voicePackDescription = desc;
        voicePackType = type;
        voiceActor = "";
        this.price = price;
        currencyType = currency;
        voicePackIcon = "";
        isLimited = false;
        limitedTime = "";
        isDefault = false;
    }
    
    public void SetVoiceActor(string actor)
    {
        voiceActor = actor;
    }
    
    public void SetLimited(bool limited, string time = "")
    {
        isLimited = limited;
        limitedTime = time;
    }
    
    public void SetDefault(bool isDefault)
    {
        this.isDefault = isDefault;
    }
}

[System.Serializable]
public class VoiceLine
{
    public string voiceLineID;
    public string voicePackID;
    public string lineType;
    public string content;
    public string audioFile;
    public float duration;
    
    public VoiceLine(string id, string voicePackID, string type, string content, string audio, float duration)
    {
        voiceLineID = id;
        this.voicePackID = voicePackID;
        lineType = type;
        this.content = content;
        audioFile = audio;
        this.duration = duration;
    }
}

[System.Serializable]
public class PlayerVoiceData
{
    public string playerID;
    public Dictionary<string, string> equippedVoicePacks;
    public List<string> ownedVoicePacks;
    
    public PlayerVoiceData(string playerID)
    {
        this.playerID = playerID;
        equippedVoicePacks = new Dictionary<string, string>();
        ownedVoicePacks = new List<string>();
    }
    
    public void EquipVoicePack(string heroID, string voicePackID)
    {
        equippedVoicePacks[heroID] = voicePackID;
    }
    
    public void UnequipVoicePack(string heroID)
    {
        if (equippedVoicePacks.ContainsKey(heroID))
        {
            equippedVoicePacks.Remove(heroID);
        }
    }
    
    public void AddOwnedVoicePack(string voicePackID)
    {
        if (!ownedVoicePacks.Contains(voicePackID))
        {
            ownedVoicePacks.Add(voicePackID);
        }
    }
    
    public void RemoveOwnedVoicePack(string voicePackID)
    {
        ownedVoicePacks.Remove(voicePackID);
    }
    
    public string GetEquippedVoicePack(string heroID)
    {
        return equippedVoicePacks.ContainsKey(heroID) ? equippedVoicePacks[heroID] : "";
    }
    
    public bool OwnsVoicePack(string voicePackID)
    {
        return ownedVoicePacks.Contains(voicePackID);
    }
}

[System.Serializable]
public class VoicePackManagerData
{
    public VoicePackSystem system;
    public List<PlayerVoiceData> playerData;
    
    public VoicePackManagerData()
    {
        system = new VoicePackSystem("voice_pack", "语音包系统", "管理英雄语音包");
        playerData = new List<PlayerVoiceData>();
    }
    
    public void AddPlayerData(PlayerVoiceData data)
    {
        playerData.Add(data);
    }
    
    public PlayerVoiceData GetPlayerData(string playerID)
    {
        return playerData.Find(pd => pd.playerID == playerID);
    }
}
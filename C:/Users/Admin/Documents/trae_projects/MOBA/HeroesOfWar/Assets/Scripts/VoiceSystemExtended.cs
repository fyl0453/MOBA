[System.Serializable]
public class VoiceSystemExtended
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<VoicePack> voicePacks;
    public List<VoiceInteraction> voiceInteractions;
    public List<VoiceSetting> voiceSettings;
    
    public VoiceSystemExtended(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        voicePacks = new List<VoicePack>();
        voiceInteractions = new List<VoiceInteraction>();
        voiceSettings = new List<VoiceSetting>();
    }
    
    public void AddVoicePack(VoicePack pack)
    {
        voicePacks.Add(pack);
    }
    
    public void AddVoiceInteraction(VoiceInteraction interaction)
    {
        voiceInteractions.Add(interaction);
    }
    
    public void AddVoiceSetting(VoiceSetting setting)
    {
        voiceSettings.Add(setting);
    }
    
    public VoicePack GetVoicePack(string packID)
    {
        return voicePacks.Find(p => p.packID == packID);
    }
    
    public VoiceSetting GetVoiceSetting(string playerID)
    {
        return voiceSettings.Find(s => s.playerID == playerID);
    }
    
    public List<VoicePack> GetVoicePacksByType(string type)
    {
        return voicePacks.FindAll(p => p.packType == type);
    }
    
    public List<VoicePack> GetVoicePacksByHero(string heroID)
    {
        return voicePacks.FindAll(p => p.heroID == heroID);
    }
}

[System.Serializable]
public class VoicePack
{
    public string packID;
    public string packName;
    public string packDescription;
    public string packType;
    public string heroID;
    public string heroName;
    public string voiceActor;
    public List<string> voiceLines;
    public bool isDefault;
    public bool isLimited;
    public string releaseDate;
    public string expiryDate;
    
    public VoicePack(string id, string name, string desc, string type, string heroID, string heroName, string voiceActor)
    {
        packID = id;
        packName = name;
        packDescription = desc;
        packType = type;
        this.heroID = heroID;
        this.heroName = heroName;
        this.voiceActor = voiceActor;
        voiceLines = new List<string>();
        isDefault = false;
        isLimited = false;
        releaseDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        expiryDate = "";
    }
    
    public void AddVoiceLine(string line)
    {
        voiceLines.Add(line);
    }
    
    public void SetAsDefault()
    {
        isDefault = true;
    }
    
    public void SetAsLimited(string expiry)
    {
        isLimited = true;
        expiryDate = expiry;
    }
}

[System.Serializable]
public class VoiceInteraction
{
    public string interactionID;
    public string triggerType;
    public string triggerCondition;
    public string voicePackID;
    public string voiceLine;
    public string targetType;
    public string targetID;
    public float cooldown;
    
    public VoiceInteraction(string id, string triggerType, string triggerCondition, string voicePackID, string voiceLine, string targetType, string targetID, float cooldown)
    {
        interactionID = id;
        this.triggerType = triggerType;
        this.triggerCondition = triggerCondition;
        this.voicePackID = voicePackID;
        this.voiceLine = voiceLine;
        this.targetType = targetType;
        this.targetID = targetID;
        this.cooldown = cooldown;
    }
}

[System.Serializable]
public class VoiceSetting
{
    public string settingID;
    public string playerID;
    public bool enableVoice;
    public bool enableVoiceChat;
    public bool enableVoiceEffects;
    public float voiceVolume;
    public float voiceChatVolume;
    public string voiceLanguage;
    public string defaultVoicePackID;
    
    public VoiceSetting(string id, string playerID)
    {
        settingID = id;
        this.playerID = playerID;
        enableVoice = true;
        enableVoiceChat = true;
        enableVoiceEffects = true;
        voiceVolume = 1.0f;
        voiceChatVolume = 1.0f;
        voiceLanguage = "Chinese";
        defaultVoicePackID = "";
    }
    
    public void UpdateSetting(bool voice, bool voiceChat, bool voiceEffects, float volume, float chatVolume, string language, string defaultPack)
    {
        enableVoice = voice;
        enableVoiceChat = voiceChat;
        enableVoiceEffects = voiceEffects;
        voiceVolume = volume;
        voiceChatVolume = chatVolume;
        voiceLanguage = language;
        defaultVoicePackID = defaultPack;
    }
}

[System.Serializable]
public class VoiceSystemManagerData
{
    public VoiceSystemExtended system;
    
    public VoiceSystemManagerData()
    {
        system = new VoiceSystemExtended("voice_system_extended", "语音系统扩展", "提供更多语音包和语音互动功能");
    }
}
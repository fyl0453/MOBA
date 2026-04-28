using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VoiceSystemManagerExtended : MonoBehaviour
{
    public static VoiceSystemManagerExtended Instance { get; private set; }
    
    public VoiceSystemManagerData voiceSystemData;
    
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
        LoadVoiceSystemData();
        
        if (voiceSystemData == null)
        {
            voiceSystemData = new VoiceSystemManagerData();
            InitializeDefaultVoiceSystem();
        }
    }
    
    private void InitializeDefaultVoiceSystem()
    {
        // 创建默认语音包
        VoicePack guanyuDefault = new VoicePack(
            "guanyu_default",
            "关羽默认语音",
            "关羽的默认语音包",
            "Hero",
            "guanyu",
            "关羽",
            "张三"
        );
        guanyuDefault.AddVoiceLine("刀锋所划之地，便是疆土");
        guanyuDefault.AddVoiceLine("义薄云天，刀镇山河");
        guanyuDefault.AddVoiceLine("一骑当千");
        guanyuDefault.SetAsDefault();
        
        VoicePack zhugeliangDefault = new VoicePack(
            "zhugeliang_default",
            "诸葛亮默认语音",
            "诸葛亮的默认语音包",
            "Hero",
            "zhugeliang",
            "诸葛亮",
            "李四"
        );
        zhugeliangDefault.AddVoiceLine("运筹帷幄之中，决胜千里之外");
        zhugeliangDefault.AddVoiceLine("鞠躬尽瘁，死而后已");
        zhugeliangDefault.AddVoiceLine("智者，当运筹帷幄");
        zhugeliangDefault.SetAsDefault();
        
        // 创建限定语音包
        VoicePack guanyuLimited = new VoicePack(
            "guanyu_limited",
            "关羽限定语音",
            "关羽的限定语音包",
            "Hero",
            "guanyu",
            "关羽",
            "王五"
        );
        guanyuLimited.AddVoiceLine("武圣在此，谁敢放肆");
        guanyuLimited.AddVoiceLine("青龙偃月刀，所向披靡");
        guanyuLimited.SetAsLimited("2026-12-31");
        
        voiceSystemData.system.AddVoicePack(guanyuDefault);
        voiceSystemData.system.AddVoicePack(zhugeliangDefault);
        voiceSystemData.system.AddVoicePack(guanyuLimited);
        
        // 创建默认语音互动
        VoiceInteraction interaction1 = new VoiceInteraction(
            "interaction1",
            "Kill",
            "FirstBlood",
            "guanyu_default",
            "刀锋所划之地，便是疆土",
            "Player",
            "",
            5.0f
        );
        
        VoiceInteraction interaction2 = new VoiceInteraction(
            "interaction2",
            "Death",
            "FirstDeath",
            "zhugeliang_default",
            "智者千虑，必有一失",
            "Player",
            "",
            10.0f
        );
        
        voiceSystemData.system.AddVoiceInteraction(interaction1);
        voiceSystemData.system.AddVoiceInteraction(interaction2);
        
        // 创建默认设置
        VoiceSetting defaultSetting = new VoiceSetting("setting1", "default");
        voiceSystemData.system.AddVoiceSetting(defaultSetting);
        
        SaveVoiceSystemData();
    }
    
    public string CreateVoicePack(string name, string desc, string type, string heroID, string heroName, string voiceActor, bool isDefault, bool isLimited, string expiryDate = "")
    {
        string packID = System.Guid.NewGuid().ToString();
        VoicePack newPack = new VoicePack(packID, name, desc, type, heroID, heroName, voiceActor);
        
        if (isDefault)
        {
            newPack.SetAsDefault();
        }
        
        if (isLimited)
        {
            newPack.SetAsLimited(expiryDate);
        }
        
        voiceSystemData.system.AddVoicePack(newPack);
        SaveVoiceSystemData();
        Debug.Log($"成功创建语音包: {name}");
        return packID;
    }
    
    public void AddVoiceLine(string packID, string line)
    {
        VoicePack pack = voiceSystemData.system.GetVoicePack(packID);
        if (pack != null)
        {
            pack.AddVoiceLine(line);
            SaveVoiceSystemData();
            Debug.Log($"成功添加语音台词: {line}");
        }
    }
    
    public string CreateVoiceInteraction(string triggerType, string triggerCondition, string voicePackID, string voiceLine, string targetType, string targetID, float cooldown)
    {
        string interactionID = System.Guid.NewGuid().ToString();
        VoiceInteraction newInteraction = new VoiceInteraction(interactionID, triggerType, triggerCondition, voicePackID, voiceLine, targetType, targetID, cooldown);
        voiceSystemData.system.AddVoiceInteraction(newInteraction);
        SaveVoiceSystemData();
        Debug.Log("成功创建语音互动");
        return interactionID;
    }
    
    public void UpdateVoiceSetting(string playerID, bool voice, bool voiceChat, bool voiceEffects, float volume, float chatVolume, string language, string defaultPack)
    {
        VoiceSetting setting = voiceSystemData.system.GetVoiceSetting(playerID);
        if (setting == null)
        {
            string settingID = System.Guid.NewGuid().ToString();
            setting = new VoiceSetting(settingID, playerID);
            voiceSystemData.system.AddVoiceSetting(setting);
        }
        
        setting.UpdateSetting(voice, voiceChat, voiceEffects, volume, chatVolume, language, defaultPack);
        SaveVoiceSystemData();
        Debug.Log("成功更新语音设置");
    }
    
    public VoicePack GetVoicePack(string packID)
    {
        return voiceSystemData.system.GetVoicePack(packID);
    }
    
    public List<VoicePack> GetVoicePacksByType(string type)
    {
        return voiceSystemData.system.GetVoicePacksByType(type);
    }
    
    public List<VoicePack> GetVoicePacksByHero(string heroID)
    {
        return voiceSystemData.system.GetVoicePacksByHero(heroID);
    }
    
    public VoiceSetting GetVoiceSetting(string playerID)
    {
        return voiceSystemData.system.GetVoiceSetting(playerID);
    }
    
    public void PlayVoiceLine(string packID, int lineIndex)
    {
        VoicePack pack = voiceSystemData.system.GetVoicePack(packID);
        if (pack != null && lineIndex < pack.voiceLines.Count)
        {
            string line = pack.voiceLines[lineIndex];
            Debug.Log($"播放语音: {line}");
            // 实际播放语音的逻辑
        }
    }
    
    public void PlayVoiceByTrigger(string playerID, string triggerType, string triggerCondition)
    {
        foreach (VoiceInteraction interaction in voiceSystemData.system.voiceInteractions)
        {
            if (interaction.triggerType == triggerType && interaction.triggerCondition == triggerCondition)
            {
                VoicePack pack = voiceSystemData.system.GetVoicePack(interaction.voicePackID);
                if (pack != null)
                {
                    Debug.Log($"播放语音: {interaction.voiceLine}");
                    // 实际播放语音的逻辑
                }
                break;
            }
        }
    }
    
    public void SaveVoiceSystemData()
    {
        string path = Application.dataPath + "/Data/voice_system_extended_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, voiceSystemData);
        stream.Close();
    }
    
    public void LoadVoiceSystemData()
    {
        string path = Application.dataPath + "/Data/voice_system_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            voiceSystemData = (VoiceSystemManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            voiceSystemData = new VoiceSystemManagerData();
        }
    }
}
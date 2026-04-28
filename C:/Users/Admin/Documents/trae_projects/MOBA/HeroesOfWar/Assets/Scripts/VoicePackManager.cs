using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VoicePackManager : MonoBehaviour
{
    public static VoicePackManager Instance { get; private set; }
    
    public VoicePackManagerData voicePackData;
    
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
        LoadVoicePackData();
        
        if (voicePackData == null)
        {
            voicePackData = new VoicePackManagerData();
            InitializeDefaultVoicePacks();
        }
    }
    
    private void InitializeDefaultVoicePacks()
    {
        // 创建关羽的语音包
        VoicePack guanyuDefault = new VoicePack("voice_pack_guanyu_default", "hero_guanyu", "关羽", "默认语音", "关羽的默认语音包", "Default", 0);
        guanyuDefault.SetDefault(true);
        
        VoicePack guanyuSpring = new VoicePack("voice_pack_guanyu_spring", "hero_guanyu", "关羽", "新春语音", "关羽的新春限定语音包", "Limited", 288, "Gems");
        guanyuSpring.SetVoiceActor("张三");
        guanyuSpring.SetLimited(true, "2024-01-01 to 2024-02-01");
        
        voicePackData.system.AddVoicePack(guanyuDefault);
        voicePackData.system.AddVoicePack(guanyuSpring);
        
        // 创建关羽的语音台词
        List<VoiceLine> guanyuLines = new List<VoiceLine>
        {
            new VoiceLine("line_guanyu_1", "voice_pack_guanyu_default", "Battle", "关羽在此！", "guanyu_1", 1.5f),
            new VoiceLine("line_guanyu_2", "voice_pack_guanyu_default", "Battle", "一夫当关，万夫莫开！", "guanyu_2", 2.0f),
            new VoiceLine("line_guanyu_3", "voice_pack_guanyu_default", "Skill", "青龙偃月刀！", "guanyu_3", 1.0f),
            new VoiceLine("line_guanyu_4", "voice_pack_guanyu_default", "Skill", "威震华夏！", "guanyu_4", 1.2f),
            new VoiceLine("line_guanyu_spring_1", "voice_pack_guanyu_spring", "Battle", "新春快乐！", "guanyu_spring_1", 1.5f),
            new VoiceLine("line_guanyu_spring_2", "voice_pack_guanyu_spring", "Battle", "恭喜发财！", "guanyu_spring_2", 1.5f)
        };
        
        foreach (VoiceLine line in guanyuLines)
        {
            voicePackData.system.AddVoiceLine(line);
        }
        
        // 创建诸葛亮的语音包
        VoicePack zhugeliangDefault = new VoicePack("voice_pack_zhugeliang_default", "hero_zhugeliang", "诸葛亮", "默认语音", "诸葛亮的默认语音包", "Default", 0);
        zhugeliangDefault.SetDefault(true);
        
        VoicePack zhugeliangDragon = new VoicePack("voice_pack_zhugeliang_dragon", "hero_zhugeliang", "诸葛亮", "龙胆亮银", "诸葛亮的龙胆亮银语音包", "Legendary", 888, "Gems");
        zhugeliangDragon.SetVoiceActor("李四");
        
        voicePackData.system.AddVoicePack(zhugeliangDefault);
        voicePackData.system.AddVoicePack(zhugeliangDragon);
        
        // 创建诸葛亮的语音台词
        List<VoiceLine> zhugeliangLines = new List<VoiceLine>
        {
            new VoiceLine("line_zhugeliang_1", "voice_pack_zhugeliang_default", "Battle", "运筹帷幄之中，决胜千里之外！", "zhugeliang_1", 2.5f),
            new VoiceLine("line_zhugeliang_2", "voice_pack_zhugeliang_default", "Skill", "东风破袭！", "zhugeliang_2", 1.0f),
            new VoiceLine("line_zhugeliang_3", "voice_pack_zhugeliang_default", "Skill", "时空穿梭！", "zhugeliang_3", 1.0f),
            new VoiceLine("line_zhugeliang_4", "voice_pack_zhugeliang_default", "Skill", "元气弹！", "zhugeliang_4", 1.0f),
            new VoiceLine("line_zhugeliang_dragon_1", "voice_pack_zhugeliang_dragon", "Battle", "龙胆亮银枪！", "zhugeliang_dragon_1", 1.5f),
            new VoiceLine("line_zhugeliang_dragon_2", "voice_pack_zhugeliang_dragon", "Battle", "龙战于野！", "zhugeliang_dragon_2", 1.5f)
        };
        
        foreach (VoiceLine line in zhugeliangLines)
        {
            voicePackData.system.AddVoiceLine(line);
        }
        
        SaveVoicePackData();
    }
    
    public void PurchaseVoicePack(string playerID, string voicePackID)
    {
        VoicePack voicePack = voicePackData.system.GetVoicePack(voicePackID);
        if (voicePack != null && !voicePack.isDefault)
        {
            PlayerVoiceData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否已经拥有
            if (!playerData.OwnsVoicePack(voicePackID))
            {
                // 检查货币是否足够
                if (voicePack.currencyType == "Gems")
                {
                    if (ProfileManager.Instance.currentProfile.gems >= voicePack.price)
                    {
                        ProfileManager.Instance.currentProfile.gems -= voicePack.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddOwnedVoicePack(voicePackID);
                        SaveVoicePackData();
                        
                        Debug.Log($"成功购买语音包: {voicePack.voicePackName}");
                    }
                    else
                    {
                        Debug.Log("钻石不足");
                    }
                }
                else if (voicePack.currencyType == "Gold")
                {
                    if (ProfileManager.Instance.currentProfile.gold >= voicePack.price)
                    {
                        ProfileManager.Instance.currentProfile.gold -= voicePack.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddOwnedVoicePack(voicePackID);
                        SaveVoicePackData();
                        
                        Debug.Log($"成功购买语音包: {voicePack.voicePackName}");
                    }
                    else
                    {
                        Debug.Log("金币不足");
                    }
                }
            }
            else
            {
                Debug.Log("已经拥有该语音包");
            }
        }
    }
    
    public void EquipVoicePack(string playerID, string heroID, string voicePackID)
    {
        VoicePack voicePack = voicePackData.system.GetVoicePack(voicePackID);
        if (voicePack != null)
        {
            PlayerVoiceData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否拥有该语音包
            if (playerData.OwnsVoicePack(voicePackID) || voicePack.isDefault)
            {
                playerData.EquipVoicePack(heroID, voicePackID);
                SaveVoicePackData();
                Debug.Log($"成功装备语音包: {voicePack.voicePackName} 到英雄: {heroID}");
            }
            else
            {
                Debug.Log("未拥有该语音包");
            }
        }
    }
    
    public void UnequipVoicePack(string playerID, string heroID)
    {
        PlayerVoiceData playerData = GetOrCreatePlayerData(playerID);
        playerData.UnequipVoicePack(heroID);
        SaveVoicePackData();
        Debug.Log($"成功卸下英雄: {heroID} 的语音包");
    }
    
    public List<VoicePack> GetVoicePacksByHero(string heroID)
    {
        return voicePackData.system.GetVoicePacksByHero(heroID);
    }
    
    public List<VoicePack> GetOwnedVoicePacks(string playerID, string heroID)
    {
        PlayerVoiceData playerData = GetOrCreatePlayerData(playerID);
        List<VoicePack> heroVoicePacks = voicePackData.system.GetVoicePacksByHero(heroID);
        List<VoicePack> ownedVoicePacks = new List<VoicePack>();
        
        foreach (VoicePack voicePack in heroVoicePacks)
        {
            if (playerData.OwnsVoicePack(voicePack.voicePackID) || voicePack.isDefault)
            {
                ownedVoicePacks.Add(voicePack);
            }
        }
        
        return ownedVoicePacks;
    }
    
    public string GetEquippedVoicePack(string playerID, string heroID)
    {
        PlayerVoiceData playerData = GetOrCreatePlayerData(playerID);
        return playerData.GetEquippedVoicePack(heroID);
    }
    
    public List<VoiceLine> GetVoiceLinesByPack(string voicePackID)
    {
        return voicePackData.system.GetVoiceLinesByPack(voicePackID);
    }
    
    public VoicePack GetVoicePack(string voicePackID)
    {
        return voicePackData.system.GetVoicePack(voicePackID);
    }
    
    public void AddVoicePack(string heroID, string heroName, string name, string description, string type, int price, string currency = "Gems")
    {
        string voicePackID = System.Guid.NewGuid().ToString();
        VoicePack voicePack = new VoicePack(voicePackID, heroID, heroName, name, description, type, price, currency);
        voicePackData.system.AddVoicePack(voicePack);
        SaveVoicePackData();
    }
    
    public void AddVoiceLine(string voicePackID, string lineType, string content, string audioFile, float duration)
    {
        string voiceLineID = System.Guid.NewGuid().ToString();
        VoiceLine voiceLine = new VoiceLine(voiceLineID, voicePackID, lineType, content, audioFile, duration);
        voicePackData.system.AddVoiceLine(voiceLine);
        SaveVoicePackData();
    }
    
    private PlayerVoiceData GetOrCreatePlayerData(string playerID)
    {
        PlayerVoiceData playerData = voicePackData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerVoiceData(playerID);
            
            // 自动添加默认语音包
            foreach (VoicePack voicePack in voicePackData.system.voicePacks)
            {
                if (voicePack.isDefault)
                {
                    playerData.AddOwnedVoicePack(voicePack.voicePackID);
                }
            }
            
            voicePackData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public void SaveVoicePackData()
    {
        string path = Application.dataPath + "/Data/voice_pack_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, voicePackData);
        stream.Close();
    }
    
    public void LoadVoicePackData()
    {
        string path = Application.dataPath + "/Data/voice_pack_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            voicePackData = (VoicePackManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            voicePackData = new VoicePackManagerData();
        }
    }
}
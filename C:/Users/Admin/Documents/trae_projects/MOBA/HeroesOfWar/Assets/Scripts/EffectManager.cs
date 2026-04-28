using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }
    
    public EffectManagerData effectData;
    
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
        LoadEffectData();
        
        if (effectData == null)
        {
            effectData = new EffectManagerData();
            InitializeDefaultEffects();
        }
    }
    
    private void InitializeDefaultEffects()
    {
        // 创建特效分类
        EffectCategory回城特效 = new EffectCategory("category_recall", "回城特效", "回城时的动画效果");
        EffectCategory击败特效 = new EffectCategory("category_defeat", "击败特效", "击败敌方英雄时的动画效果");
        EffectCategory荣耀播报 = new EffectCategory("category_broadcast", "荣耀播报", "游戏中特殊事件的播报效果");
        EffectCategory技能特效 = new EffectCategory("category_skill", "技能特效", "技能释放时的动画效果");
        
        effectData.system.AddCategory(回城特效);
        effectData.system.AddCategory(击败特效);
        effectData.system.AddCategory(荣耀播报);
        effectData.system.AddCategory(技能特效);
        
        // 创建默认特效
        Effect defaultRecall = new Effect("effect_recall_default", "默认回城", "默认的回城特效", "Recall", "category_recall", 0);
        defaultRecall.SetDefault(true);
        
        Effect springRecall = new Effect("effect_recall_spring", "新春回城", "2024新春限定回城特效", "Recall", "category_recall", 288, "Gems");
        springRecall.SetLimited(true, "2024-01-01 to 2024-02-01");
        
        Effect tournamentRecall = new Effect("effect_recall_tournament", "赛事回城", "赛事限定回城特效", "Recall", "category_recall", 388, "Gems");
        
        Effect defaultDefeat = new Effect("effect_defeat_default", "默认击败", "默认的击败特效", "Defeat", "category_defeat", 0);
        defaultDefeat.SetDefault(true);
        
        Effect springDefeat = new Effect("effect_defeat_spring", "新春击败", "2024新春限定击败特效", "Defeat", "category_defeat", 188, "Gems");
        springDefeat.SetLimited(true, "2024-01-01 to 2024-02-01");
        
        Effect defaultBroadcast = new Effect("effect_broadcast_default", "默认播报", "默认的荣耀播报特效", "Broadcast", "category_broadcast", 0);
        defaultBroadcast.SetDefault(true);
        
        Effect tournamentBroadcast = new Effect("effect_broadcast_tournament", "赛事播报", "赛事限定荣耀播报特效", "Broadcast", "category_broadcast", 288, "Gems");
        
        Effect defaultSkill = new Effect("effect_skill_default", "默认技能", "默认的技能特效", "Skill", "category_skill", 0);
        defaultSkill.SetDefault(true);
        
        Effect legendarySkill = new Effect("effect_skill_legendary", "传说技能", "传说级技能特效", "Skill", "category_skill", 488, "Gems");
        
        effectData.system.AddEffect(defaultRecall);
        effectData.system.AddEffect(springRecall);
        effectData.system.AddEffect(tournamentRecall);
        effectData.system.AddEffect(defaultDefeat);
        effectData.system.AddEffect(springDefeat);
        effectData.system.AddEffect(defaultBroadcast);
        effectData.system.AddEffect(tournamentBroadcast);
        effectData.system.AddEffect(defaultSkill);
        effectData.system.AddEffect(legendarySkill);
        
        SaveEffectData();
    }
    
    public void PurchaseEffect(string playerID, string effectID)
    {
        Effect effect = effectData.system.GetEffect(effectID);
        if (effect != null && !effect.isDefault)
        {
            PlayerEffectData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否已经拥有
            if (!playerData.OwnsEffect(effectID))
            {
                // 检查货币是否足够
                if (effect.currencyType == "Gems")
                {
                    if (ProfileManager.Instance.currentProfile.gems >= effect.price)
                    {
                        ProfileManager.Instance.currentProfile.gems -= effect.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddOwnedEffect(effectID);
                        SaveEffectData();
                        
                        Debug.Log($"成功购买特效: {effect.effectName}");
                    }
                    else
                    {
                        Debug.Log("钻石不足");
                    }
                }
                else if (effect.currencyType == "Gold")
                {
                    if (ProfileManager.Instance.currentProfile.gold >= effect.price)
                    {
                        ProfileManager.Instance.currentProfile.gold -= effect.price;
                        ProfileManager.Instance.SaveProfile();
                        
                        playerData.AddOwnedEffect(effectID);
                        SaveEffectData();
                        
                        Debug.Log($"成功购买特效: {effect.effectName}");
                    }
                    else
                    {
                        Debug.Log("金币不足");
                    }
                }
            }
            else
            {
                Debug.Log("已经拥有该特效");
            }
        }
    }
    
    public void EquipEffect(string playerID, string effectType, string effectID)
    {
        Effect effect = effectData.system.GetEffect(effectID);
        if (effect != null)
        {
            PlayerEffectData playerData = GetOrCreatePlayerData(playerID);
            
            // 检查是否拥有该特效
            if (playerData.OwnsEffect(effectID) || effect.isDefault)
            {
                playerData.EquipEffect(effectType, effectID);
                SaveEffectData();
                Debug.Log($"成功装备特效: {effect.effectName} 类型: {effectType}");
            }
            else
            {
                Debug.Log("未拥有该特效");
            }
        }
    }
    
    public void UnequipEffect(string playerID, string effectType)
    {
        PlayerEffectData playerData = GetOrCreatePlayerData(playerID);
        playerData.UnequipEffect(effectType);
        SaveEffectData();
        Debug.Log($"成功卸下特效类型: {effectType}");
    }
    
    public List<Effect> GetEffectsByType(string effectType)
    {
        return effectData.system.GetEffectsByType(effectType);
    }
    
    public List<Effect> GetEffectsByCategory(string categoryID)
    {
        return effectData.system.GetEffectsByCategory(categoryID);
    }
    
    public List<Effect> GetOwnedEffects(string playerID, string effectType = "")
    {
        PlayerEffectData playerData = GetOrCreatePlayerData(playerID);
        List<Effect> effects = effectType == "" ? effectData.system.effects : effectData.system.GetEffectsByType(effectType);
        List<Effect> ownedEffects = new List<Effect>();
        
        foreach (Effect effect in effects)
        {
            if (playerData.OwnsEffect(effect.effectID) || effect.isDefault)
            {
                ownedEffects.Add(effect);
            }
        }
        
        return ownedEffects;
    }
    
    public string GetEquippedEffect(string playerID, string effectType)
    {
        PlayerEffectData playerData = GetOrCreatePlayerData(playerID);
        return playerData.GetEquippedEffect(effectType);
    }
    
    public Effect GetEffect(string effectID)
    {
        return effectData.system.GetEffect(effectID);
    }
    
    public List<EffectCategory> GetEffectCategories()
    {
        return effectData.system.categories;
    }
    
    public void AddEffect(string name, string description, string type, string category, int price, string currency = "Gems")
    {
        string effectID = System.Guid.NewGuid().ToString();
        Effect effect = new Effect(effectID, name, description, type, category, price, currency);
        effectData.system.AddEffect(effect);
        SaveEffectData();
    }
    
    public void AddEffectCategory(string name, string description)
    {
        string categoryID = System.Guid.NewGuid().ToString();
        EffectCategory category = new EffectCategory(categoryID, name, description);
        effectData.system.AddCategory(category);
        SaveEffectData();
    }
    
    private PlayerEffectData GetOrCreatePlayerData(string playerID)
    {
        PlayerEffectData playerData = effectData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerEffectData(playerID);
            
            // 自动添加默认特效
            foreach (Effect effect in effectData.system.effects)
            {
                if (effect.isDefault)
                {
                    playerData.AddOwnedEffect(effect.effectID);
                    if (string.IsNullOrEmpty(playerData.GetEquippedEffect(effect.effectType)))
                    {
                        playerData.EquipEffect(effect.effectType, effect.effectID);
                    }
                }
            }
            
            effectData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public void SaveEffectData()
    {
        string path = Application.dataPath + "/Data/effect_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, effectData);
        stream.Close();
    }
    
    public void LoadEffectData()
    {
        string path = Application.dataPath + "/Data/effect_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            effectData = (EffectManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            effectData = new EffectManagerData();
        }
    }
}
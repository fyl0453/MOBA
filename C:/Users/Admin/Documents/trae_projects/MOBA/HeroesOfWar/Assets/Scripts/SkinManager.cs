using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance { get; private set; }
    
    public SkinCollection skinCollection;
    
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
        LoadSkinData();
        
        if (skinCollection == null)
        {
            skinCollection = new SkinCollection(ProfileManager.Instance.currentProfile.playerID);
            InitializeDefaultSkins();
        }
    }
    
    private void InitializeDefaultSkins()
    {
        // 关羽皮肤
        Skin关羽默认 = new Skin("skin_guanyu_default", "hero_guanyu", "默认皮肤", "Common", 0, "Gold");
        关羽默认.isOwned = true;
        关羽默认.Equip();
        skinCollection.AddSkin(关羽默认);
        
        Skin关羽新春 = new Skin("skin_guanyu_spring", "hero_guanyu", "新春限定", "Epic", 888, "Gems");
        关羽新春.AddEffect("Particle", "Fireworks");
        关羽新春.AddEffect("Sound", "SpringTheme");
        skinCollection.AddSkin(关羽新春);
        
        // 张飞皮肤
        Skin张飞默认 = new Skin("skin_zhangfei_default", "hero_zhangfei", "默认皮肤", "Common", 0, "Gold");
        张飞默认.isOwned = true;
        张飞默认.Equip();
        skinCollection.AddSkin(张飞默认);
        
        Skin张飞战魂 = new Skin("skin_zhangfei_war", "hero_zhangfei", "战魂", "Rare", 488, "Gems");
        张飞战魂.AddEffect("Particle", "Berserker");
        skinCollection.AddSkin(张飞战魂);
        
        // 刘备皮肤
        Skin刘备默认 = new Skin("skin_liubei_default", "hero_liubei", "默认皮肤", "Common", 0, "Gold");
        刘备默认.isOwned = true;
        刘备默认.Equip();
        skinCollection.AddSkin(刘备默认);
        
        Skin刘备仁德 = new Skin("skin_liubei_benevolence", "hero_liubei", "仁德天下", "Epic", 888, "Gems");
        刘备仁德.AddEffect("Particle", "Healing");
        刘备仁德.AddEffect("Sound", "BenevolenceTheme");
        skinCollection.AddSkin(刘备仁德);
        
        // 赵云皮肤
        Skin赵云默认 = new Skin("skin_zhaoyun_default", "hero_zhaoyun", "默认皮肤", "Common", 0, "Gold");
        赵云默认.isOwned = true;
        赵云默认.Equip();
        skinCollection.AddSkin(赵云默认);
        
        Skin赵云龙胆 = new Skin("skin_zhaoyun_dragon", "hero_zhaoyun", "龙胆亮银", "Legendary", 1688, "Gems");
        赵云龙胆.AddEffect("Particle", "Dragon");
        赵云龙胆.AddEffect("Sound", "DragonRoar");
        赵云龙胆.AddEffect("Model", "DragonArmor");
        skinCollection.AddSkin(赵云龙胆);
        
        // 诸葛亮皮肤
        Skin诸葛亮默认 = new Skin("skin_zhugeliang_default", "hero_zhugeliang", "默认皮肤", "Common", 0, "Gold");
        诸葛亮默认.isOwned = true;
        诸葛亮默认.Equip();
        skinCollection.AddSkin(诸葛亮默认);
        
        Skin诸葛亮卧龙 = new Skin("skin_zhugeliang_dragon", "hero_zhugeliang", "卧龙出山", "Epic", 888, "Gems");
        诸葛亮卧龙.AddEffect("Particle", "Wisdom");
        诸葛亮卧龙.AddEffect("Sound", "StrategyTheme");
        skinCollection.AddSkin(诸葛亮卧龙);
        
        SaveSkinData();
    }
    
    public void PurchaseSkin(string skinID)
    {
        Skin skin = skinCollection.skins.Find(s => s.skinID == skinID);
        if (skin != null && !skin.isOwned)
        {
            int playerCurrency = 0;
            if (skin.currencyType == "Gems")
            {
                playerCurrency = ProfileManager.Instance.currentProfile.gems;
            }
            else if (skin.currencyType == "Gold")
            {
                playerCurrency = ProfileManager.Instance.currentProfile.gold;
            }
            
            if (playerCurrency >= skin.price)
            {
                if (skin.currencyType == "Gems")
                {
                    ProfileManager.Instance.currentProfile.gems -= skin.price;
                }
                else if (skin.currencyType == "Gold")
                {
                    ProfileManager.Instance.currentProfile.gold -= skin.price;
                }
                
                skinCollection.PurchaseSkin(skinID);
                SaveSkinData();
                ProfileManager.Instance.SaveProfile();
            }
        }
    }
    
    public void EquipSkin(string skinID)
    {
        skinCollection.EquipSkin(skinID);
        SaveSkinData();
    }
    
    public Skin GetEquippedSkin(string heroID)
    {
        return skinCollection.GetEquippedSkin(heroID);
    }
    
    public List<Skin> GetAllSkins()
    {
        return skinCollection.skins;
    }
    
    public List<Skin> GetOwnedSkins()
    {
        return skinCollection.GetOwnedSkins();
    }
    
    public List<Skin> GetSkinsByHero(string heroID)
    {
        return skinCollection.GetSkinsByHero(heroID);
    }
    
    public List<Skin> GetSkinsByRarity(string rarity)
    {
        return skinCollection.GetSkinsByRarity(rarity);
    }
    
    public Skin GetSkinByID(string skinID)
    {
        return skinCollection.skins.Find(s => s.skinID == skinID);
    }
    
    public int GetOwnedSkinCount()
    {
        return skinCollection.ownedSkins;
    }
    
    public int GetTotalSkinCount()
    {
        return skinCollection.totalSkins;
    }
    
    public void SaveSkinData()
    {
        string path = Application.dataPath + "/Data/skin_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, skinCollection);
        stream.Close();
    }
    
    public void LoadSkinData()
    {
        string path = Application.dataPath + "/Data/skin_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            skinCollection = (SkinCollection)formatter.Deserialize(stream);
            stream.Close();
        }
    }
}
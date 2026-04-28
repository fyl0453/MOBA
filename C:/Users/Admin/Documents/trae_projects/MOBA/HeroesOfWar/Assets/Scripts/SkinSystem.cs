using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SkinSystem : MonoBehaviour
{
    public static SkinSystem Instance { get; private set; }
    
    private SkinData skinData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSkinData();
            
            if (skinData == null)
            {
                skinData = new SkinData();
                InitializeDefaultSkins();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeDefaultSkins()
    {
        // 为每个英雄添加默认皮肤
        string[] heroIDs = { "warrior", "mage", "archer", "assassin", "support", "tank", "controller", "rapid_fire", "mage_assassin", "enhancer" };
        string[] heroNames = { "战士", "法师", "射手", "刺客", "辅助", "坦克", "控制法师", "速射射手", "法系刺客", "增益辅助" };
        
        for (int i = 0; i < heroIDs.Length; i++)
        {
            // 默认皮肤
            AddSkin(heroIDs[i], heroIDs[i] + "_default", heroNames[i] + "-默认", "默认皮肤", 0, SkinRarity.Common, false);
            
            // 稀有皮肤
            AddSkin(heroIDs[i], heroIDs[i] + "_rare", heroNames[i] + "-稀有", "稀有皮肤，拥有独特的外观", 288, SkinRarity.Rare, false);
            
            // 史诗皮肤
            AddSkin(heroIDs[i], heroIDs[i] + "_epic", heroNames[i] + "-史诗", "史诗皮肤，拥有独特的外观和技能特效", 888, SkinRarity.Epic, true);
            
            // 传说皮肤
            AddSkin(heroIDs[i], heroIDs[i] + "_legendary", heroNames[i] + "-传说", "传说皮肤，拥有独特的外观、技能特效和回城动画", 1688, SkinRarity.Legendary, true);
        }
        
        // 添加皮肤碎片
        AddSkinFragment("skin_fragment_common", "普通皮肤碎片", "用于兑换普通皮肤", 10);
        AddSkinFragment("skin_fragment_rare", "稀有皮肤碎片", "用于兑换稀有皮肤", 50);
        AddSkinFragment("skin_fragment_epic", "史诗皮肤碎片", "用于兑换史诗皮肤", 100);
        AddSkinFragment("skin_fragment_legendary", "传说皮肤碎片", "用于兑换传说皮肤", 200);
        
        SaveSkinData();
    }
    
    private void AddSkin(string heroID, string skinID, string skinName, string description, int price, SkinRarity rarity, bool hasSpecialEffects)
    {
        Skin skin = new Skin
        {
            skinID = skinID,
            heroID = heroID,
            skinName = skinName,
            description = description,
            price = price,
            rarity = rarity,
            hasSpecialEffects = hasSpecialEffects,
            isOwned = skinID.EndsWith("_default") // 默认皮肤默认拥有
        };
        
        skinData.skins.Add(skin);
    }
    
    private void AddSkinFragment(string fragmentID, string fragmentName, string description, int exchangeValue)
    {
        SkinFragment fragment = new SkinFragment
        {
            fragmentID = fragmentID,
            fragmentName = fragmentName,
            description = description,
            exchangeValue = exchangeValue,
            quantity = 0
        };
        
        skinData.skinFragments.Add(fragment);
    }
    
    public List<Skin> GetSkinsByHero(string heroID)
    {
        return skinData.skins.FindAll(s => s.heroID == heroID);
    }
    
    public List<Skin> GetOwnedSkins()
    {
        return skinData.skins.FindAll(s => s.isOwned);
    }
    
    public List<Skin> GetSkinsByRarity(SkinRarity rarity)
    {
        return skinData.skins.FindAll(s => s.rarity == rarity);
    }
    
    public Skin GetSkin(string skinID)
    {
        return skinData.skins.Find(s => s.skinID == skinID);
    }
    
    public bool PurchaseSkin(string skinID, int price)
    {
        Skin skin = GetSkin(skinID);
        if (skin != null && !skin.isOwned)
        {
            // 这里应该检查玩家是否有足够的货币
            // 暂时直接设置为已拥有
            skin.isOwned = true;
            Debug.Log($"购买皮肤成功: {skin.skinName}");
            SaveSkinData();
            return true;
        }
        return false;
    }
    
    public bool ExchangeSkinWithFragments(string skinID)
    {
        Skin skin = GetSkin(skinID);
        if (skin != null && !skin.isOwned)
        {
            // 根据皮肤稀有度获取所需的碎片类型
            string fragmentID = "";
            int requiredFragments = 0;
            
            switch (skin.rarity)
            {
                case SkinRarity.Common:
                    fragmentID = "skin_fragment_common";
                    requiredFragments = 10;
                    break;
                case SkinRarity.Rare:
                    fragmentID = "skin_fragment_rare";
                    requiredFragments = 50;
                    break;
                case SkinRarity.Epic:
                    fragmentID = "skin_fragment_epic";
                    requiredFragments = 100;
                    break;
                case SkinRarity.Legendary:
                    fragmentID = "skin_fragment_legendary";
                    requiredFragments = 200;
                    break;
            }
            
            SkinFragment fragment = skinData.skinFragments.Find(f => f.fragmentID == fragmentID);
            if (fragment != null && fragment.quantity >= requiredFragments)
            {
                fragment.quantity -= requiredFragments;
                skin.isOwned = true;
                Debug.Log($"使用碎片兑换皮肤成功: {skin.skinName}");
                SaveSkinData();
                return true;
            }
        }
        return false;
    }
    
    public void AddSkinFragment(string fragmentID, int quantity)
    {
        SkinFragment fragment = skinData.skinFragments.Find(f => f.fragmentID == fragmentID);
        if (fragment != null)
        {
            fragment.quantity += quantity;
            Debug.Log($"获得皮肤碎片: {fragment.fragmentName} x {quantity}");
            SaveSkinData();
        }
    }
    
    public int GetSkinFragmentQuantity(string fragmentID)
    {
        SkinFragment fragment = skinData.skinFragments.Find(f => f.fragmentID == fragmentID);
        if (fragment != null)
        {
            return fragment.quantity;
        }
        return 0;
    }
    
    public List<SkinFragment> GetAllSkinFragments()
    {
        return skinData.skinFragments;
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
        formatter.Serialize(stream, skinData);
        stream.Close();
    }
    
    public void LoadSkinData()
    {
        string path = Application.dataPath + "/Data/skin_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            skinData = (SkinData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            skinData = new SkinData();
        }
    }
}

[System.Serializable]
public class SkinData
{
    public List<Skin> skins = new List<Skin>();
    public List<SkinFragment> skinFragments = new List<SkinFragment>();
}

[System.Serializable]
public class Skin
{
    public string skinID;
    public string heroID;
    public string skinName;
    public string description;
    public int price;
    public SkinRarity rarity;
    public bool hasSpecialEffects;
    public bool isOwned;
}

[System.Serializable]
public class SkinFragment
{
    public string fragmentID;
    public string fragmentName;
    public string description;
    public int exchangeValue;
    public int quantity;
}

public enum SkinRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

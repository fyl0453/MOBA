using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SummonManager : MonoBehaviour
{
    public static SummonManager Instance { get; private set; }
    
    public SummonManagerData summonData;
    
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
        LoadSummonData();
        
        if (summonData == null)
        {
            summonData = new SummonManagerData();
            InitializeDefaultSummonPools();
        }
    }
    
    private void InitializeDefaultSummonPools()
    {
        // 英雄召唤池
        SummonPool heroPool = new SummonPool("pool_hero", "英雄召唤池", "Hero", 200, "Gems");
        heroPool.AddSummonItem("hero_guanyu", "Hero", 0.1f);
        heroPool.AddSummonItem("hero_zhangfei", "Hero", 0.1f);
        heroPool.AddSummonItem("hero_liubei", "Hero", 0.1f);
        heroPool.AddSummonItem("hero_zhaoyun", "Hero", 0.1f);
        heroPool.AddSummonItem("hero_zhugeliang", "Hero", 0.1f);
        heroPool.AddSummonItem("item_health_potion", "Item", 0.5f, 5);
        summonData.AddSummonPool(heroPool);
        
        // 皮肤召唤池
        SummonPool skinPool = new SummonPool("pool_skin", "皮肤召唤池", "Skin", 300, "Gems");
        skinPool.AddSummonItem("skin_guanyu_spring", "Skin", 0.05f);
        skinPool.AddSummonItem("skin_zhangfei_war", "Skin", 0.05f);
        skinPool.AddSummonItem("skin_liubei_benevolence", "Skin", 0.05f);
        skinPool.AddSummonItem("skin_zhaoyun_dragon", "Skin", 0.05f);
        skinPool.AddSummonItem("skin_zhugeliang_dragon", "Skin", 0.05f);
        skinPool.AddSummonItem("item_mana_potion", "Item", 0.75f, 5);
        summonData.AddSummonPool(skinPool);
        
        // 宠物召唤池
        SummonPool petPool = new SummonPool("pool_pet", "宠物召唤池", "Pet", 150, "Gems");
        petPool.AddSummonItem("pet_dog", "Pet", 0.3f);
        petPool.AddSummonItem("pet_cat", "Pet", 0.3f);
        petPool.AddSummonItem("pet_dragon", "Pet", 0.1f);
        petPool.AddSummonItem("item_attack_potion", "Item", 0.3f, 3);
        summonData.AddSummonPool(petPool);
        
        // 限时活动召唤池
        SummonPool eventPool = new SummonPool("pool_event", "活动召唤池", "Event", 250, "Gems");
        eventPool.AddSummonItem("skin_guanyu_spring", "Skin", 0.2f);
        eventPool.AddSummonItem("pet_dragon", "Pet", 0.1f);
        eventPool.AddSummonItem("item_speed_potion", "Item", 0.7f, 2);
        summonData.AddSummonPool(eventPool);
        
        SaveSummonData();
    }
    
    public SummonItem Summon(string poolID)
    {
        SummonPool pool = summonData.GetSummonPool(poolID);
        if (pool != null && pool.isActive && !pool.IsExpired())
        {
            int playerCurrency = 0;
            if (pool.currencyType == "Gems")
            {
                playerCurrency = ProfileManager.Instance.currentProfile.gems;
            }
            else if (pool.currencyType == "Gold")
            {
                playerCurrency = ProfileManager.Instance.currentProfile.gold;
            }
            
            if (playerCurrency >= pool.summonCost)
            {
                if (pool.currencyType == "Gems")
                {
                    ProfileManager.Instance.currentProfile.gems -= pool.summonCost;
                }
                else if (pool.currencyType == "Gold")
                {
                    ProfileManager.Instance.currentProfile.gold -= pool.summonCost;
                }
                
                SummonItem item = pool.Summon();
                GrantSummonItem(item);
                
                SummonHistory history = new SummonHistory("history_" + System.DateTime.Now.Ticks, poolID, ProfileManager.Instance.currentProfile.playerID);
                history.AddObtainedItem(item);
                summonData.AddSummonHistory(history);
                
                SaveSummonData();
                ProfileManager.Instance.SaveProfile();
                
                return item;
            }
        }
        return null;
    }
    
    public List<SummonItem> SummonMultiple(string poolID, int count)
    {
        List<SummonItem> obtainedItems = new List<SummonItem>();
        SummonPool pool = summonData.GetSummonPool(poolID);
        
        if (pool != null && pool.isActive && !pool.IsExpired())
        {
            int totalCost = pool.summonCost * count;
            int playerCurrency = 0;
            
            if (pool.currencyType == "Gems")
            {
                playerCurrency = ProfileManager.Instance.currentProfile.gems;
            }
            else if (pool.currencyType == "Gold")
            {
                playerCurrency = ProfileManager.Instance.currentProfile.gold;
            }
            
            if (playerCurrency >= totalCost)
            {
                if (pool.currencyType == "Gems")
                {
                    ProfileManager.Instance.currentProfile.gems -= totalCost;
                }
                else if (pool.currencyType == "Gold")
                {
                    ProfileManager.Instance.currentProfile.gold -= totalCost;
                }
                
                SummonHistory history = new SummonHistory("history_" + System.DateTime.Now.Ticks, poolID, ProfileManager.Instance.currentProfile.playerID);
                
                for (int i = 0; i < count; i++)
                {
                    SummonItem item = pool.Summon();
                    obtainedItems.Add(item);
                    GrantSummonItem(item);
                    history.AddObtainedItem(item);
                }
                
                summonData.AddSummonHistory(history);
                SaveSummonData();
                ProfileManager.Instance.SaveProfile();
            }
        }
        
        return obtainedItems;
    }
    
    private void GrantSummonItem(SummonItem item)
    {
        switch (item.itemType)
        {
            case "Hero":
                // 这里需要添加英雄解锁逻辑
                break;
            case "Skin":
                SkinManager.Instance.PurchaseSkin(item.itemID);
                break;
            case "Pet":
                // 这里需要添加宠物获取逻辑
                break;
            case "Item":
                InventoryManager.Instance.AddItemToInventory(item.itemID, item.quantity);
                break;
        }
    }
    
    public List<SummonPool> GetActiveSummonPools()
    {
        return summonData.summonPools.FindAll(p => p.isActive && !p.IsExpired());
    }
    
    public SummonPool GetSummonPool(string poolID)
    {
        return summonData.GetSummonPool(poolID);
    }
    
    public List<SummonHistory> GetSummonHistories()
    {
        return summonData.GetSummonHistoriesByPlayer(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public void AddSummonPool(SummonPool pool)
    {
        summonData.AddSummonPool(pool);
        SaveSummonData();
    }
    
    public void RemoveSummonPool(string poolID)
    {
        summonData.summonPools.RemoveAll(p => p.poolID == poolID);
        SaveSummonData();
    }
    
    public void SaveSummonData()
    {
        string path = Application.dataPath + "/Data/summon_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, summonData);
        stream.Close();
    }
    
    public void LoadSummonData()
    {
        string path = Application.dataPath + "/Data/summon_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            summonData = (SummonManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            summonData = new SummonManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Random;

public class SummonSystemDetailedManager : MonoBehaviour
{
    public static SummonSystemDetailedManager Instance { get; private set; }
    
    public SummonSystemDetailedManagerData summonData;
    private Random random;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            random = new Random();
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
            summonData = new SummonSystemDetailedManagerData();
            InitializeDefaultSummonSystem();
        }
    }
    
    private void InitializeDefaultSummonSystem()
    {
        // 召唤池
        SummonPool pool1 = new SummonPool("pool_001", "英雄召唤池", "召唤英雄", "hero", 200, "diamond", 100, false);
        SummonPool pool2 = new SummonPool("pool_002", "皮肤召唤池", "召唤皮肤", "skin", 300, "diamond", 100, false);
        SummonPool pool3 = new SummonPool("pool_003", "限定召唤池", "限定英雄和皮肤召唤", "limited", 500, "diamond", 50, true);
        
        summonData.system.AddSummonPool(pool1);
        summonData.system.AddSummonPool(pool2);
        summonData.system.AddSummonPool(pool3);
        
        // 召唤池物品
        // 英雄召唤池
        SummonItem item1 = new SummonItem("item_001", "hero", "hero_001", "李白", "epic", 0.05f, 1);
        SummonItem item2 = new SummonItem("item_002", "hero", "hero_002", "韩信", "epic", 0.05f, 1);
        SummonItem item3 = new SummonItem("item_003", "hero", "hero_003", "赵云", "rare", 0.1f, 1);
        SummonItem item4 = new SummonItem("item_004", "hero", "hero_004", "貂蝉", "rare", 0.1f, 1);
        SummonItem item5 = new SummonItem("item_005", "fragment", "hero_fragment", "英雄碎片", "common", 0.7f, 5);
        
        pool1.AddItem(item1);
        pool1.AddItem(item2);
        pool1.AddItem(item3);
        pool1.AddItem(item4);
        pool1.AddItem(item5);
        
        // 皮肤召唤池
        SummonItem item6 = new SummonItem("item_006", "skin", "skin_001", "李白-凤求凰", "epic", 0.05f, 1);
        SummonItem item7 = new SummonItem("item_007", "skin", "skin_002", "韩信-白龙吟", "epic", 0.05f, 1);
        SummonItem item8 = new SummonItem("item_008", "skin", "skin_003", "赵云-嘻哈天王", "rare", 0.1f, 1);
        SummonItem item9 = new SummonItem("item_009", "skin", "skin_004", "貂蝉-仲夏夜之梦", "rare", 0.1f, 1);
        SummonItem item10 = new SummonItem("item_010", "fragment", "skin_fragment", "皮肤碎片", "common", 0.7f, 5);
        
        pool2.AddItem(item6);
        pool2.AddItem(item7);
        pool2.AddItem(item8);
        pool2.AddItem(item9);
        pool2.AddItem(item10);
        
        // 限定召唤池
        SummonItem item11 = new SummonItem("item_011", "hero", "hero_005", "孙悟空", "legendary", 0.02f, 1);
        SummonItem item12 = new SummonItem("item_012", "skin", "skin_005", "孙悟空-至尊宝", "legendary", 0.03f, 1);
        SummonItem item13 = new SummonItem("item_013", "hero", "hero_006", "花木兰", "epic", 0.05f, 1);
        SummonItem item14 = new SummonItem("item_014", "skin", "skin_006", "花木兰-水晶猎龙者", "epic", 0.05f, 1);
        SummonItem item15 = new SummonItem("item_015", "fragment", "hero_fragment", "英雄碎片", "common", 0.45f, 10);
        SummonItem item16 = new SummonItem("item_016", "fragment", "skin_fragment", "皮肤碎片", "common", 0.4f, 10);
        
        pool3.AddItem(item11);
        pool3.AddItem(item12);
        pool3.AddItem(item13);
        pool3.AddItem(item14);
        pool3.AddItem(item15);
        pool3.AddItem(item16);
        
        // 召唤记录
        SummonRecord record1 = new SummonRecord("record_001", "pool_001", "user_001", 1, 200, "diamond");
        SummonRecord record2 = new SummonRecord("record_002", "pool_002", "user_001", 1, 300, "diamond");
        SummonRecord record3 = new SummonRecord("record_003", "pool_001", "user_002", 1, 200, "diamond");
        
        summonData.system.AddSummonRecord(record1);
        summonData.system.AddSummonRecord(record2);
        summonData.system.AddSummonRecord(record3);
        
        // 召唤结果
        SummonResult result1 = new SummonResult("result_001", "pool_001", "user_001", "item_003", "hero", "hero_003", "赵云", "rare", 1);
        SummonResult result2 = new SummonResult("result_002", "pool_002", "user_001", "item_010", "fragment", "skin_fragment", "皮肤碎片", "common", 5);
        SummonResult result3 = new SummonResult("result_003", "pool_001", "user_002", "item_005", "fragment", "hero_fragment", "英雄碎片", "common", 5);
        
        summonData.system.AddSummonResult(result1);
        summonData.system.AddSummonResult(result2);
        summonData.system.AddSummonResult(result3);
        
        // 添加结果到记录
        record1.AddResult("result_001");
        record2.AddResult("result_002");
        record3.AddResult("result_003");
        
        // 召唤事件
        SummonEvent event1 = new SummonEvent("event_001", "summon", "user_001", "pool_001", "召唤英雄");
        SummonEvent event2 = new SummonEvent("event_002", "summon", "user_001", "pool_002", "召唤皮肤");
        SummonEvent event3 = new SummonEvent("event_003", "summon", "user_002", "pool_001", "召唤英雄");
        
        summonData.system.AddSummonEvent(event1);
        summonData.system.AddSummonEvent(event2);
        summonData.system.AddSummonEvent(event3);
        
        SaveSummonData();
    }
    
    // 召唤池管理
    public void AddSummonPool(string poolName, string poolDescription, string poolType, int cost, string currency, int maxSummonCount, bool isLimited)
    {
        string poolID = "pool_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SummonPool summonPool = new SummonPool(poolID, poolName, poolDescription, poolType, cost, currency, maxSummonCount, isLimited);
        summonData.system.AddSummonPool(summonPool);
        SaveSummonData();
        Debug.Log("成功添加召唤池: " + poolName);
    }
    
    public void AddSummonItem(string poolID, string itemType, string itemValue, string itemName, string rarity, float probability, int quantity)
    {
        SummonPool pool = summonData.system.GetSummonPool(poolID);
        if (pool != null)
        {
            string itemID = "item_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            SummonItem item = new SummonItem(itemID, itemType, itemValue, itemName, rarity, probability, quantity);
            pool.AddItem(item);
            SaveSummonData();
            Debug.Log("成功添加召唤物品: " + itemName);
        }
        else
        {
            Debug.LogError("召唤池不存在: " + poolID);
        }
    }
    
    public void ActivateSummonPool(string poolID)
    {
        SummonPool pool = summonData.system.GetSummonPool(poolID);
        if (pool != null)
        {
            pool.Activate();
            SaveSummonData();
            Debug.Log("成功激活召唤池: " + pool.poolName);
        }
        else
        {
            Debug.LogError("召唤池不存在: " + poolID);
        }
    }
    
    public void DisableSummonPool(string poolID)
    {
        SummonPool pool = summonData.system.GetSummonPool(poolID);
        if (pool != null)
        {
            pool.Disable();
            SaveSummonData();
            Debug.Log("成功禁用召唤池: " + pool.poolName);
        }
        else
        {
            Debug.LogError("召唤池不存在: " + poolID);
        }
    }
    
    public List<SummonPool> GetSummonPoolsByType(string poolType)
    {
        return summonData.system.GetSummonPoolsByType(poolType);
    }
    
    public List<SummonPool> GetAllSummonPools()
    {
        return summonData.system.summonPools;
    }
    
    // 召唤功能
    public List<SummonResult> Summon(string poolID, string userID, int count = 1)
    {
        SummonPool pool = summonData.system.GetSummonPool(poolID);
        if (pool != null && pool.IsAvailable())
        {
            // 检查召唤次数
            if (pool.isLimited && pool.summonCount + count > pool.maxSummonCount)
            {
                count = pool.maxSummonCount - pool.summonCount;
                if (count <= 0)
                {
                    Debug.LogError("召唤次数已达上限");
                    return new List<SummonResult>();
                }
            }
            
            // 计算总消耗
            int totalCost = pool.cost * count;
            
            // 这里可以添加检查货币的逻辑
            // if (!CheckCurrency(userID, pool.currency, totalCost))
            // {
            //     Debug.LogError("货币不足");
            //     return new List<SummonResult>();
            // }
            
            // 扣除货币
            // DeductCurrency(userID, pool.currency, totalCost);
            
            // 创建召唤记录
            string recordID = "record_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            SummonRecord record = new SummonRecord(recordID, poolID, userID, count, totalCost, pool.currency);
            summonData.system.AddSummonRecord(record);
            
            // 执行召唤
            List<SummonResult> results = new List<SummonResult>();
            for (int i = 0; i < count; i++)
            {
                SummonResult result = PerformSummon(pool, userID);
                if (result != null)
                {
                    results.Add(result);
                    record.AddResult(result.resultID);
                    pool.AddSummonCount();
                }
            }
            
            // 创建召唤事件
            CreateSummonEvent("summon", userID, poolID, "召唤" + count + "次");
            
            SaveSummonData();
            Debug.Log("成功召唤" + count + "次");
            return results;
        }
        else
        {
            Debug.LogError("召唤池不存在或不可用");
            return new List<SummonResult>();
        }
    }
    
    private SummonResult PerformSummon(SummonPool pool, string userID)
    {
        // 计算概率
        float randomValue = (float)random.NextDouble();
        float cumulativeProbability = 0;
        
        foreach (SummonItem item in pool.items)
        {
            if (item.isEnabled)
            {
                cumulativeProbability += item.probability;
                if (randomValue <= cumulativeProbability)
                {
                    // 创建召唤结果
                    string resultID = "result_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                    SummonResult result = new SummonResult(resultID, pool.poolID, userID, item.itemID, item.itemType, item.itemValue, item.itemName, item.rarity, item.quantity);
                    summonData.system.AddSummonResult(result);
                    
                    // 这里可以添加发放物品的逻辑
                    Debug.Log("获得物品: " + item.itemName + " 数量: " + item.quantity);
                    
                    return result;
                }
            }
        }
        
        return null;
    }
    
    // 召唤记录管理
    public List<SummonRecord> GetSummonRecords(string userID)
    {
        return summonData.system.GetSummonRecordsByUser(userID);
    }
    
    public List<SummonResult> GetSummonResults(string recordID)
    {
        SummonRecord record = summonData.system.GetSummonRecord(recordID);
        if (record != null)
        {
            List<SummonResult> results = new List<SummonResult>();
            foreach (string resultID in record.resultIDs)
            {
                SummonResult result = summonData.system.GetSummonResult(resultID);
                if (result != null)
                {
                    results.Add(result);
                }
            }
            return results;
        }
        else
        {
            return new List<SummonResult>();
        }
    }
    
    // 召唤事件管理
    public string CreateSummonEvent(string eventType, string userID, string poolID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SummonEvent summonEvent = new SummonEvent(eventID, eventType, userID, poolID, description);
        summonData.system.AddSummonEvent(summonEvent);
        SaveSummonData();
        Debug.Log("成功创建召唤事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        SummonEvent summonEvent = summonData.system.GetSummonEvent(eventID);
        if (summonEvent != null)
        {
            summonEvent.MarkAsCompleted();
            SaveSummonData();
            Debug.Log("成功标记召唤事件为完成");
        }
        else
        {
            Debug.LogError("召唤事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        SummonEvent summonEvent = summonData.system.GetSummonEvent(eventID);
        if (summonEvent != null)
        {
            summonEvent.MarkAsFailed();
            SaveSummonData();
            Debug.Log("成功标记召唤事件为失败");
        }
        else
        {
            Debug.LogError("召唤事件不存在: " + eventID);
        }
    }
    
    public List<SummonEvent> GetSummonEvents(string userID)
    {
        return summonData.system.GetSummonEventsByUser(userID);
    }
    
    // 数据持久化
    public void SaveSummonData()
    {
        string path = Application.dataPath + "/Data/summon_system_detailed_data.dat";
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
        string path = Application.dataPath + "/Data/summon_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            summonData = (SummonSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            summonData = new SummonSystemDetailedManagerData();
        }
    }
}
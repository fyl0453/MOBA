[System.Serializable]
public class SummonPool
{
    public string poolID;
    public string poolName;
    public string poolType;
    public string poolDescription;
    public List<SummonItem> items;
    public int summonCost;
    public string currencyType;
    public int summonCount;
    public string startTime;
    public string endTime;
    public bool isActive;
    
    public SummonPool(string id, string name, string type, int cost, string currency = "Gems")
    {
        poolID = id;
        poolName = name;
        poolType = type;
        poolDescription = "一个召唤池";
        items = new List<SummonItem>();
        summonCost = cost;
        currencyType = currency;
        summonCount = 0;
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd");
        endTime = System.DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
        isActive = true;
    }
    
    public void AddSummonItem(string itemID, string itemType, float probability, int quantity = 1)
    {
        SummonItem item = new SummonItem(itemID, itemType, probability, quantity);
        items.Add(item);
    }
    
    public SummonItem Summon()
    {
        summonCount++;
        float randomValue = Random.Range(0f, 1f);
        float cumulativeProbability = 0f;
        
        foreach (SummonItem item in items)
        {
            cumulativeProbability += item.probability;
            if (randomValue <= cumulativeProbability)
            {
                return item;
            }
        }
        
        return items[0]; // 保底返回第一个物品
    }
    
    public bool IsExpired()
    {
        System.DateTime endDateValue;
        if (System.DateTime.TryParse(endTime, out endDateValue))
        {
            return System.DateTime.Now > endDateValue;
        }
        return false;
    }
    
    public void Deactivate()
    {
        isActive = false;
    }
}

[System.Serializable]
public class SummonItem
{
    public string itemID;
    public string itemType;
    public float probability;
    public int quantity;
    
    public SummonItem(string id, string type, float prob, int qty = 1)
    {
        itemID = id;
        itemType = type;
        probability = prob;
        quantity = qty;
    }
}

[System.Serializable]
public class SummonHistory
{
    public string historyID;
    public string poolID;
    public string playerID;
    public string summonTime;
    public List<SummonItem> obtainedItems;
    
    public SummonHistory(string id, string pool, string player)
    {
        historyID = id;
        poolID = pool;
        playerID = player;
        summonTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        obtainedItems = new List<SummonItem>();
    }
    
    public void AddObtainedItem(SummonItem item)
    {
        obtainedItems.Add(item);
    }
}

[System.Serializable]
public class SummonManagerData
{
    public List<SummonPool> summonPools;
    public List<SummonHistory> summonHistories;
    
    public SummonManagerData()
    {
        summonPools = new List<SummonPool>();
        summonHistories = new List<SummonHistory>();
    }
    
    public void AddSummonPool(SummonPool pool)
    {
        summonPools.Add(pool);
    }
    
    public void AddSummonHistory(SummonHistory history)
    {
        summonHistories.Add(history);
    }
    
    public SummonPool GetSummonPool(string poolID)
    {
        return summonPools.Find(p => p.poolID == poolID);
    }
    
    public List<SummonHistory> GetSummonHistoriesByPlayer(string playerID)
    {
        return summonHistories.FindAll(h => h.playerID == playerID);
    }
}
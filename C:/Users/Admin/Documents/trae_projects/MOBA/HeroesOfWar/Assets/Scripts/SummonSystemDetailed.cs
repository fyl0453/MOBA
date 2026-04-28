[System.Serializable]
public class SummonSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<SummonPool> summonPools;
    public List<SummonResult> summonResults;
    public List<SummonRecord> summonRecords;
    public List<SummonEvent> summonEvents;
    
    public SummonSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        summonPools = new List<SummonPool>();
        summonResults = new List<SummonResult>();
        summonRecords = new List<SummonRecord>();
        summonEvents = new List<SummonEvent>();
    }
    
    public void AddSummonPool(SummonPool summonPool)
    {
        summonPools.Add(summonPool);
    }
    
    public void AddSummonResult(SummonResult summonResult)
    {
        summonResults.Add(summonResult);
    }
    
    public void AddSummonRecord(SummonRecord summonRecord)
    {
        summonRecords.Add(summonRecord);
    }
    
    public void AddSummonEvent(SummonEvent summonEvent)
    {
        summonEvents.Add(summonEvent);
    }
    
    public SummonPool GetSummonPool(string poolID)
    {
        return summonPools.Find(sp => sp.poolID == poolID);
    }
    
    public SummonResult GetSummonResult(string resultID)
    {
        return summonResults.Find(sr => sr.resultID == resultID);
    }
    
    public SummonRecord GetSummonRecord(string recordID)
    {
        return summonRecords.Find(sr => sr.recordID == recordID);
    }
    
    public SummonEvent GetSummonEvent(string eventID)
    {
        return summonEvents.Find(se => se.eventID == eventID);
    }
    
    public List<SummonPool> GetSummonPoolsByType(string poolType)
    {
        return summonPools.FindAll(sp => sp.poolType == poolType);
    }
    
    public List<SummonRecord> GetSummonRecordsByUser(string userID)
    {
        return summonRecords.FindAll(sr => sr.userID == userID);
    }
    
    public List<SummonEvent> GetSummonEventsByUser(string userID)
    {
        return summonEvents.FindAll(se => se.userID == userID);
    }
}

[System.Serializable]
public class SummonPool
{
    public string poolID;
    public string poolName;
    public string poolDescription;
    public string poolType;
    public string status;
    public int cost;
    public string currency;
    public int summonCount;
    public int maxSummonCount;
    public string startDate;
    public string endDate;
    public bool isLimited;
    public List<SummonItem> items;
    
    public SummonPool(string id, string name, string description, string poolType, int cost, string currency, int maxSummonCount, bool isLimited)
    {
        poolID = id;
        poolName = name;
        poolDescription = description;
        this.poolType = poolType;
        status = "active";
        this.cost = cost;
        this.currency = currency;
        summonCount = 0;
        this.maxSummonCount = maxSummonCount;
        startDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        endDate = "";
        this.isLimited = isLimited;
        items = new List<SummonItem>();
    }
    
    public void Activate()
    {
        status = "active";
    }
    
    public void Disable()
    {
        status = "disabled";
    }
    
    public void End()
    {
        status = "ended";
        endDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddSummonCount()
    {
        summonCount++;
    }
    
    public void AddItem(SummonItem item)
    {
        items.Add(item);
    }
    
    public void RemoveItem(string itemID)
    {
        items.RemoveAll(i => i.itemID == itemID);
    }
    
    public bool IsAvailable()
    {
        if (status != "active")
            return false;
        
        if (isLimited && summonCount >= maxSummonCount)
            return false;
        
        if (!string.IsNullOrEmpty(endDate))
        {
            System.DateTime now = System.DateTime.Now;
            System.DateTime end = System.DateTime.Parse(endDate);
            return now <= end;
        }
        
        return true;
    }
}

[System.Serializable]
public class SummonItem
{
    public string itemID;
    public string itemType;
    public string itemValue;
    public string itemName;
    public string rarity;
    public float probability;
    public int quantity;
    public bool isEnabled;
    
    public SummonItem(string id, string itemType, string itemValue, string itemName, string rarity, float probability, int quantity)
    {
        itemID = id;
        this.itemType = itemType;
        this.itemValue = itemValue;
        this.itemName = itemName;
        this.rarity = rarity;
        this.probability = probability;
        this.quantity = quantity;
        isEnabled = true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public void SetProbability(float probability)
    {
        this.probability = probability;
    }
    
    public void SetQuantity(int quantity)
    {
        this.quantity = quantity;
    }
}

[System.Serializable]
public class SummonResult
{
    public string resultID;
    public string poolID;
    public string userID;
    public string itemID;
    public string itemType;
    public string itemValue;
    public string itemName;
    public string rarity;
    public int quantity;
    public string summonTime;
    public string status;
    
    public SummonResult(string id, string poolID, string userID, string itemID, string itemType, string itemValue, string itemName, string rarity, int quantity)
    {
        resultID = id;
        this.poolID = poolID;
        this.userID = userID;
        this.itemID = itemID;
        this.itemType = itemType;
        this.itemValue = itemValue;
        this.itemName = itemName;
        this.rarity = rarity;
        this.quantity = quantity;
        summonTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "success";
    }
    
    public void SetStatus(string status)
    {
        this.status = status;
    }
}

[System.Serializable]
public class SummonRecord
{
    public string recordID;
    public string poolID;
    public string userID;
    public int summonCount;
    public int totalCost;
    public string currency;
    public string summonTime;
    public string status;
    public List<string> resultIDs;
    
    public SummonRecord(string id, string poolID, string userID, int summonCount, int totalCost, string currency)
    {
        recordID = id;
        this.poolID = poolID;
        this.userID = userID;
        this.summonCount = summonCount;
        this.totalCost = totalCost;
        this.currency = currency;
        summonTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "success";
        resultIDs = new List<string>();
    }
    
    public void AddResult(string resultID)
    {
        resultIDs.Add(resultID);
    }
    
    public void SetStatus(string status)
    {
        this.status = status;
    }
}

[System.Serializable]
public class SummonEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string poolID;
    public string description;
    public string timestamp;
    public string status;
    
    public SummonEvent(string id, string eventType, string userID, string poolID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.poolID = poolID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
    }
    
    public void MarkAsCompleted()
    {
        status = "completed";
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class SummonSystemDetailedManagerData
{
    public SummonSystemDetailed system;
    
    public SummonSystemDetailedManagerData()
    {
        system = new SummonSystemDetailed("summon_system_detailed", "召唤系统详细", "管理召唤的详细功能，包括英雄和皮肤召唤");
    }
}
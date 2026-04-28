[System.Serializable]
public class MarkSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Mark> marks;
    public List<PlayerMark> playerMarks;
    public List<MarkEvent> markEvents;
    
    public MarkSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        marks = new List<Mark>();
        playerMarks = new List<PlayerMark>();
        markEvents = new List<MarkEvent>();
    }
    
    public void AddMark(Mark mark)
    {
        marks.Add(mark);
    }
    
    public void AddPlayerMark(PlayerMark playerMark)
    {
        playerMarks.Add(playerMark);
    }
    
    public void AddMarkEvent(MarkEvent markEvent)
    {
        markEvents.Add(markEvent);
    }
    
    public Mark GetMark(string markID)
    {
        return marks.Find(m => m.markID == markID);
    }
    
    public PlayerMark GetPlayerMark(string userID, string markID)
    {
        return playerMarks.Find(pm => pm.userID == userID && pm.markID == markID);
    }
    
    public MarkEvent GetMarkEvent(string eventID)
    {
        return markEvents.Find(me => me.eventID == eventID);
    }
    
    public List<Mark> GetMarksByType(string markType)
    {
        return marks.FindAll(m => m.markType == markType);
    }
    
    public List<PlayerMark> GetPlayerMarksByUser(string userID)
    {
        return playerMarks.FindAll(pm => pm.userID == userID);
    }
    
    public List<MarkEvent> GetMarkEventsByUser(string userID)
    {
        return markEvents.FindAll(me => me.userID == userID);
    }
}

[System.Serializable]
public class Mark
{
    public string markID;
    public string markName;
    public string markDescription;
    public string markType;
    public string rarity;
    public string获取条件;
    public string icon;
    public string effect;
    public bool isLimited;
    public bool isEnabled;
    
    public Mark(string id, string markName, string markDescription, string markType, string rarity, string获取条件, string icon, string effect, bool isLimited)
    {
        markID = id;
        this.markName = markName;
        this.markDescription = markDescription;
        this.markType = markType;
        this.rarity = rarity;
        this.获取条件 = 获取条件;
        this.icon = icon;
        this.effect = effect;
        this.isLimited = isLimited;
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
    
    public bool IsAvailable()
    {
        return isEnabled;
    }
}

[System.Serializable]
public class PlayerMark
{
    public string playerMarkID;
    public string userID;
    public string userName;
    public string markID;
    public string markName;
    public bool isOwned;
    public bool isEquipped;
    public string obtainTime;
    public int usageCount;
    
    public PlayerMark(string id, string userID, string userName, string markID, string markName)
    {
        playerMarkID = id;
        this.userID = userID;
        this.userName = userName;
        this.markID = markID;
        this.markName = markName;
        isOwned = true;
        isEquipped = false;
        obtainTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        usageCount = 0;
    }
    
    public void Equip()
    {
        isEquipped = true;
        usageCount++;
    }
    
    public void Unequip()
    {
        isEquipped = false;
    }
    
    public bool IsOwned()
    {
        return isOwned;
    }
    
    public bool IsEquipped()
    {
        return isEquipped;
    }
}

[System.Serializable]
public class MarkEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string markID;
    public string description;
    public string timestamp;
    public string status;
    
    public MarkEvent(string id, string eventType, string userID, string markID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.markID = markID;
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
public class MarkSystemDetailedManagerData
{
    public MarkSystemDetailed system;
    
    public MarkSystemDetailedManagerData()
    {
        system = new MarkSystemDetailed("mark_system_detailed", "印记系统详细", "管理印记的详细功能，包括印记收集和展示");
    }
}
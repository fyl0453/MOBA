[System.Serializable]
public class IntimacySystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<IntimacyRelationship> intimacyRelationships;
    public List<IntimacyLevel> intimacyLevels;
    public List<IntimacyInteraction> intimacyInteractions;
    public List<IntimacyEvent> intimacyEvents;
    
    public IntimacySystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        intimacyRelationships = new List<IntimacyRelationship>();
        intimacyLevels = new List<IntimacyLevel>();
        intimacyInteractions = new List<IntimacyInteraction>();
        intimacyEvents = new List<IntimacyEvent>();
    }
    
    public void AddIntimacyRelationship(IntimacyRelationship intimacyRelationship)
    {
        intimacyRelationships.Add(intimacyRelationship);
    }
    
    public void AddIntimacyLevel(IntimacyLevel intimacyLevel)
    {
        intimacyLevels.Add(intimacyLevel);
    }
    
    public void AddIntimacyInteraction(IntimacyInteraction intimacyInteraction)
    {
        intimacyInteractions.Add(intimacyInteraction);
    }
    
    public void AddIntimacyEvent(IntimacyEvent intimacyEvent)
    {
        intimacyEvents.Add(intimacyEvent);
    }
    
    public IntimacyRelationship GetIntimacyRelationship(string relationshipID)
    {
        return intimacyRelationships.Find(ir => ir.relationshipID == relationshipID);
    }
    
    public IntimacyLevel GetIntimacyLevel(int level)
    {
        return intimacyLevels.Find(il => il.level == level);
    }
    
    public IntimacyInteraction GetIntimacyInteraction(string interactionID)
    {
        return intimacyInteractions.Find(ii => ii.interactionID == interactionID);
    }
    
    public IntimacyEvent GetIntimacyEvent(string eventID)
    {
        return intimacyEvents.Find(ie => ie.eventID == eventID);
    }
    
    public List<IntimacyRelationship> GetIntimacyRelationshipsByUser(string userID)
    {
        return intimacyRelationships.FindAll(ir => ir.user1ID == userID || ir.user2ID == userID);
    }
    
    public List<IntimacyRelationship> GetIntimacyRelationshipsByType(string relationshipType)
    {
        return intimacyRelationships.FindAll(ir => ir.relationshipType == relationshipType);
    }
    
    public List<IntimacyInteraction> GetIntimacyInteractionsByRelationship(string relationshipID)
    {
        return intimacyInteractions.FindAll(ii => ii.relationshipID == relationshipID);
    }
    
    public List<IntimacyEvent> GetIntimacyEventsByUser(string userID)
    {
        return intimacyEvents.FindAll(ie => ie.userID == userID);
    }
}

[System.Serializable]
public class IntimacyRelationship
{
    public string relationshipID;
    public string user1ID;
    public string user1Name;
    public string user2ID;
    public string user2Name;
    public string relationshipType;
    public string status;
    public int intimacyPoints;
    public int intimacyLevel;
    public string establishTime;
    public string breakTime;
    public bool isCouple;
    public string coupleName;
    
    public IntimacyRelationship(string id, string user1ID, string user1Name, string user2ID, string user2Name, string relationshipType, bool isCouple, string coupleName = "")
    {
        relationshipID = id;
        this.user1ID = user1ID;
        this.user1Name = user1Name;
        this.user2ID = user2ID;
        this.user2Name = user2Name;
        this.relationshipType = relationshipType;
        status = "active";
        intimacyPoints = 0;
        intimacyLevel = 1;
        establishTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        breakTime = "";
        this.isCouple = isCouple;
        this.coupleName = coupleName;
    }
    
    public void AddIntimacyPoints(int points)
    {
        intimacyPoints += points;
        UpdateIntimacyLevel();
    }
    
    public void UpdateIntimacyLevel()
    {
        // 简单的等级计算，每100点提升一级
        intimacyLevel = (intimacyPoints / 100) + 1;
    }
    
    public void BreakRelationship()
    {
        status = "broken";
        breakTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public bool IsActive()
    {
        return status == "active";
    }
    
    public bool IsCouple()
    {
        return isCouple;
    }
}

[System.Serializable]
public class IntimacyLevel
{
    public int level;
    public string levelName;
    public string levelDescription;
    public int requiredPoints;
    public string icon;
    public List<string> rewards;
    
    public IntimacyLevel(int level, string levelName, string levelDescription, int requiredPoints, string icon)
    {
        this.level = level;
        this.levelName = levelName;
        this.levelDescription = levelDescription;
        this.requiredPoints = requiredPoints;
        this.icon = icon;
        rewards = new List<string>();
    }
    
    public void AddReward(string reward)
    {
        rewards.Add(reward);
    }
    
    public bool IsReached(int points)
    {
        return points >= requiredPoints;
    }
}

[System.Serializable]
public class IntimacyInteraction
{
    public string interactionID;
    public string relationshipID;
    public string initiatorID;
    public string initiatorName;
    public string targetID;
    public string targetName;
    public string interactionType;
    public int intimacyPoints;
    public string timestamp;
    public string status;
    
    public IntimacyInteraction(string id, string relationshipID, string initiatorID, string initiatorName, string targetID, string targetName, string interactionType, int intimacyPoints)
    {
        interactionID = id;
        this.relationshipID = relationshipID;
        this.initiatorID = initiatorID;
        this.initiatorName = initiatorName;
        this.targetID = targetID;
        this.targetName = targetName;
        this.interactionType = interactionType;
        this.intimacyPoints = intimacyPoints;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "completed";
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class IntimacyEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string targetID;
    public string relationshipID;
    public string description;
    public string timestamp;
    public string status;
    
    public IntimacyEvent(string id, string eventType, string userID, string targetID, string relationshipID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.targetID = targetID;
        this.relationshipID = relationshipID;
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
public class IntimacySystemDetailedManagerData
{
    public IntimacySystemDetailed system;
    
    public IntimacySystemDetailedManagerData()
    {
        system = new IntimacySystemDetailed("intimacy_system_detailed", "亲密关系系统详细", "管理亲密关系的详细功能，包括情侣关系和好友亲密度");
    }
}
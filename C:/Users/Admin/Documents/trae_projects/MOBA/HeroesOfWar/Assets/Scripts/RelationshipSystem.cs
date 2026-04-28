[System.Serializable]
public class Relationship
{
    public string relationshipID;
    public string player1ID;
    public string player1Name;
    public string player2ID;
    public string player2Name;
    public string relationshipType;
    public int intimacyValue;
    public string establishDate;
    public bool isActive;
    
    public Relationship(string id, string p1, string p1Name, string p2, string p2Name, string type)
    {
        relationshipID = id;
        player1ID = p1;
        player1Name = p1Name;
        player2ID = p2;
        player2Name = p2Name;
        relationshipType = type;
        intimacyValue = 0;
        establishDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        isActive = true;
    }
    
    public void AddIntimacy(int value)
    {
        intimacyValue += value;
    }
    
    public void RemoveIntimacy(int value)
    {
        intimacyValue = Mathf.Max(0, intimacyValue - value);
    }
    
    public void Deactivate()
    {
        isActive = false;
    }
    
    public string GetIntimacyLevel()
    {
        if (intimacyValue < 100)
            return "陌生人";
        else if (intimacyValue < 300)
            return "好友";
        else if (intimacyValue < 600)
            return "死党";
        else if (intimacyValue < 1000)
            return "闺蜜";
        else
            return "恋人";
    }
}

[System.Serializable]
public class RelationshipManagerData
{
    public List<Relationship> relationships;
    
    public RelationshipManagerData()
    {
        relationships = new List<Relationship>();
    }
    
    public void AddRelationship(Relationship relationship)
    {
        relationships.Add(relationship);
    }
    
    public Relationship GetRelationship(string id)
    {
        return relationships.Find(r => r.relationshipID == id);
    }
    
    public List<Relationship> GetRelationshipsByPlayer(string playerID)
    {
        return relationships.FindAll(r => (r.player1ID == playerID || r.player2ID == playerID) && r.isActive);
    }
    
    public Relationship GetRelationshipBetweenPlayers(string player1ID, string player2ID)
    {
        return relationships.Find(r => 
            (r.player1ID == player1ID && r.player2ID == player2ID) || 
            (r.player1ID == player2ID && r.player2ID == player1ID));
    }
}
[System.Serializable]
public class Mark
{
    public string markID;
    public string markName;
    public string markDescription;
    public string markType;
    public string markIcon;
    public bool isObtained;
    public string obtainedDate;
    
    public Mark(string id, string name, string desc, string type, string icon)
    {
        markID = id;
        markName = name;
        markDescription = desc;
        markType = type;
        markIcon = icon;
        isObtained = false;
        obtainedDate = "";
    }
    
    public void Obtain()
    {
        isObtained = true;
        obtainedDate = System.DateTime.Now.ToString("yyyy-MM-dd");
    }
}

[System.Serializable]
public class MarkCollection
{
    public string playerID;
    public List<Mark> marks;
    
    public MarkCollection(string player)
    {
        playerID = player;
        marks = new List<Mark>();
    }
    
    public void AddMark(Mark mark)
    {
        marks.Add(mark);
    }
    
    public void ObtainMark(string markID)
    {
        Mark mark = marks.Find(m => m.markID == markID);
        if (mark != null && !mark.isObtained)
        {
            mark.Obtain();
        }
    }
    
    public List<Mark> GetObtainedMarks()
    {
        return marks.FindAll(m => m.isObtained);
    }
    
    public List<Mark> GetMarksByType(string type)
    {
        return marks.FindAll(m => m.markType == type);
    }
}

[System.Serializable]
public class MarkManagerData
{
    public List<MarkCollection> markCollections;
    
    public MarkManagerData()
    {
        markCollections = new List<MarkCollection>();
    }
    
    public void AddMarkCollection(MarkCollection collection)
    {
        markCollections.Add(collection);
    }
    
    public MarkCollection GetMarkCollection(string playerID)
    {
        return markCollections.Find(c => c.playerID == playerID);
    }
}
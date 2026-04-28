[System.Serializable]
public class ShareContent
{
    public string contentID;
    public string contentType;
    public string title;
    public string description;
    public string imagePath;
    public string shareUrl;
    public string templateText;
    
    public ShareContent(string id, string type, string title, string desc, string template)
    {
        contentID = id;
        contentType = type;
        this.title = title;
        description = desc;
        imagePath = "";
        shareUrl = "";
        templateText = template;
    }
}

[System.Serializable]
public class ShareRecord
{
    public string recordID;
    public string playerID;
    public string contentID;
    public string contentType;
    public string sharePlatform;
    public string shareTime;
    public bool isSuccessful;
    
    public ShareRecord(string id, string player, string content, string type, string platform)
    {
        recordID = id;
        playerID = player;
        contentID = content;
        contentType = type;
        sharePlatform = platform;
        shareTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        isSuccessful = false;
    }
    
    public void MarkAsSuccessful()
    {
        isSuccessful = true;
    }
}

[System.Serializable]
public class ShareManagerData
{
    public List<ShareContent> shareContents;
    public List<ShareRecord> shareRecords;
    public int totalShareCount;
    
    public ShareManagerData()
    {
        shareContents = new List<ShareContent>();
        shareRecords = new List<ShareRecord>();
        totalShareCount = 0;
    }
    
    public void AddShareContent(ShareContent content)
    {
        shareContents.Add(content);
    }
    
    public void AddShareRecord(ShareRecord record)
    {
        shareRecords.Add(record);
    }
    
    public ShareContent GetShareContent(string contentID)
    {
        return shareContents.Find(c => c.contentID == contentID);
    }
    
    public List<ShareContent> GetShareContentsByType(string type)
    {
        return shareContents.FindAll(c => c.contentType == type);
    }
    
    public List<ShareRecord> GetShareRecordsByPlayer(string playerID)
    {
        return shareRecords.FindAll(r => r.playerID == playerID);
    }
}
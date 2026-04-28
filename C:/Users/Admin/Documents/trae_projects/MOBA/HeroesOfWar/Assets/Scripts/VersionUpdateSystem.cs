[System.Serializable]
public class VersionUpdateSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<VersionInfo> versions;
    public List<UpdateNotice> updateNotices;
    
    public VersionUpdateSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        versions = new List<VersionInfo>();
        updateNotices = new List<UpdateNotice>();
    }
    
    public void AddVersion(VersionInfo version)
    {
        versions.Add(version);
    }
    
    public void AddUpdateNotice(UpdateNotice notice)
    {
        updateNotices.Add(notice);
    }
    
    public VersionInfo GetVersion(string versionID)
    {
        return versions.Find(v => v.versionID == versionID);
    }
    
    public VersionInfo GetLatestVersion()
    {
        if (versions.Count == 0)
        {
            return null;
        }
        versions.Sort((a, b) => b.versionNumber.CompareTo(a.versionNumber));
        return versions[0];
    }
    
    public UpdateNotice GetUpdateNotice(string noticeID)
    {
        return updateNotices.Find(n => n.noticeID == noticeID);
    }
}

[System.Serializable]
public class VersionInfo
{
    public string versionID;
    public string versionNumber;
    public string versionName;
    public string releaseDate;
    public string updateType;
    public bool isRequired;
    public long fileSize;
    public string downloadURL;
    public string patchNotes;
    
    public VersionInfo(string id, string versionNumber, string versionName, string updateType, bool isRequired, long fileSize, string downloadURL, string patchNotes)
    {
        versionID = id;
        this.versionNumber = versionNumber;
        this.versionName = versionName;
        releaseDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.updateType = updateType;
        this.isRequired = isRequired;
        this.fileSize = fileSize;
        this.downloadURL = downloadURL;
        this.patchNotes = patchNotes;
    }
}

[System.Serializable]
public class UpdateNotice
{
    public string noticeID;
    public string title;
    public string content;
    public string noticeType;
    public string startTime;
    public string endTime;
    public bool isActive;
    
    public UpdateNotice(string id, string title, string content, string noticeType, string start, string end)
    {
        noticeID = id;
        this.title = title;
        this.content = content;
        this.noticeType = noticeType;
        startTime = start;
        endTime = end;
        isActive = false;
    }
    
    public void Activate()
    {
        isActive = true;
    }
    
    public void Deactivate()
    {
        isActive = false;
    }
}

[System.Serializable]
public class VersionUpdateManagerData
{
    public VersionUpdateSystem system;
    
    public VersionUpdateManagerData()
    {
        system = new VersionUpdateSystem("version_update_system", "版本更新系统", "管理游戏版本更新");
    }
}
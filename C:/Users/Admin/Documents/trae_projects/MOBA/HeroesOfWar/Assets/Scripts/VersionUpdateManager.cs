using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VersionUpdateManager : MonoBehaviour
{
    public static VersionUpdateManager Instance { get; private set; }
    
    public VersionUpdateManagerData versionUpdateData;
    
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
        LoadVersionUpdateData();
        
        if (versionUpdateData == null)
        {
            versionUpdateData = new VersionUpdateManagerData();
            InitializeDefaultVersions();
        }
        
        // 检查更新公告状态
        CheckUpdateNoticeStatus();
    }
    
    private void InitializeDefaultVersions()
    {
        // 创建默认版本
        VersionInfo version1 = new VersionInfo(
            "version_1",
            "1.0.0",
            "正式版",
            "Major",
            true,
            1024 * 1024 * 100, // 100MB
            "https://example.com/download/1.0.0",
            "1. 游戏正式上线\n2. 开放5v5对战模式\n3. 初始英雄10个"
        );
        
        VersionInfo version2 = new VersionInfo(
            "version_2",
            "1.1.0",
            "更新版",
            "Minor",
            false,
            1024 * 1024 * 50, // 50MB
            "https://example.com/download/1.1.0",
            "1. 新增3个英雄\n2. 优化游戏性能\n3. 修复已知bug"
        );
        
        versionUpdateData.system.AddVersion(version1);
        versionUpdateData.system.AddVersion(version2);
        
        // 创建默认更新公告
        UpdateNotice notice1 = new UpdateNotice(
            "notice_1",
            "游戏正式上线",
            "亲爱的玩家，《Heroes of War》正式上线啦！快来体验全新的MOBA游戏吧！",
            "Launch",
            "2024-01-01 00:00:00",
            "2024-01-31 23:59:59"
        );
        
        UpdateNotice notice2 = new UpdateNotice(
            "notice_2",
            "版本更新公告",
            "游戏将于2024年2月1日进行版本更新，新增3个英雄，优化游戏性能。",
            "Update",
            "2024-01-25 00:00:00",
            "2024-02-10 23:59:59"
        );
        
        versionUpdateData.system.AddUpdateNotice(notice1);
        versionUpdateData.system.AddUpdateNotice(notice2);
        
        SaveVersionUpdateData();
    }
    
    private void CheckUpdateNoticeStatus()
    {
        string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        foreach (UpdateNotice notice in versionUpdateData.system.updateNotices)
        {
            if (currentTime >= notice.startTime && currentTime <= notice.endTime)
            {
                notice.Activate();
            }
            else
            {
                notice.Deactivate();
            }
        }
        SaveVersionUpdateData();
    }
    
    public void AddVersion(string versionNumber, string versionName, string updateType, bool isRequired, long fileSize, string downloadURL, string patchNotes)
    {
        string versionID = System.Guid.NewGuid().ToString();
        VersionInfo newVersion = new VersionInfo(versionID, versionNumber, versionName, updateType, isRequired, fileSize, downloadURL, patchNotes);
        versionUpdateData.system.AddVersion(newVersion);
        SaveVersionUpdateData();
        Debug.Log($"成功添加版本: {versionNumber} - {versionName}");
    }
    
    public void AddUpdateNotice(string title, string content, string noticeType, string start, string end)
    {
        string noticeID = System.Guid.NewGuid().ToString();
        UpdateNotice newNotice = new UpdateNotice(noticeID, title, content, noticeType, start, end);
        versionUpdateData.system.AddUpdateNotice(newNotice);
        SaveVersionUpdateData();
        Debug.Log($"成功添加更新公告: {title}");
    }
    
    public VersionInfo CheckForUpdates(string currentVersion)
    {
        VersionInfo latestVersion = versionUpdateData.system.GetLatestVersion();
        if (latestVersion != null && IsNewerVersion(latestVersion.versionNumber, currentVersion))
        {
            return latestVersion;
        }
        return null;
    }
    
    private bool IsNewerVersion(string version1, string version2)
    {
        string[] v1 = version1.Split('.');
        string[] v2 = version2.Split('.');
        
        for (int i = 0; i < Mathf.Min(v1.Length, v2.Length); i++)
        {
            int num1 = int.Parse(v1[i]);
            int num2 = int.Parse(v2[i]);
            if (num1 > num2)
            {
                return true;
            }
            else if (num1 < num2)
            {
                return false;
            }
        }
        
        return v1.Length > v2.Length;
    }
    
    public List<UpdateNotice> GetActiveUpdateNotices()
    {
        CheckUpdateNoticeStatus();
        return versionUpdateData.system.updateNotices.FindAll(n => n.isActive);
    }
    
    public VersionInfo GetLatestVersion()
    {
        return versionUpdateData.system.GetLatestVersion();
    }
    
    public List<VersionInfo> GetVersionHistory()
    {
        List<VersionInfo> versions = new List<VersionInfo>(versionUpdateData.system.versions);
        versions.Sort((a, b) => b.versionNumber.CompareTo(a.versionNumber));
        return versions;
    }
    
    public void SaveVersionUpdateData()
    {
        string path = Application.dataPath + "/Data/version_update_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, versionUpdateData);
        stream.Close();
    }
    
    public void LoadVersionUpdateData()
    {
        string path = Application.dataPath + "/Data/version_update_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            versionUpdateData = (VersionUpdateManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            versionUpdateData = new VersionUpdateManagerData();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Report
{
    public string ReportID;
    public string ReporterID;
    public string ReporterName;
    public string ReportedID;
    public string ReportedName;
    public int ReportType;
    public string ReportReason;
    public string EvidenceURL;
    public string MatchID;
    public DateTime ReportTime;
    public int Status;
    public string HandlerID;
    public DateTime HandleTime;
    public string HandleResult;
    public int PunishmentLevel;

    public Report(string reportID, string reporterID, string reporterName, string reportedID, string reportedName, int reportType, string reportReason)
    {
        ReportID = reportID;
        ReporterID = reporterID;
        ReporterName = reporterName;
        ReportedID = reportedID;
        ReportedName = reportedName;
        ReportType = reportType;
        ReportReason = reportReason;
        EvidenceURL = "";
        MatchID = "";
        ReportTime = DateTime.Now;
        Status = 0;
        HandlerID = "";
        HandleTime = DateTime.MinValue;
        HandleResult = "";
        PunishmentLevel = 0;
    }
}

[Serializable]
public class ReportType
{
    public int TypeID;
    public string TypeName;
    public string Description;
    public int DefaultPunishmentLevel;
    public bool IsActive;

    public ReportType(int typeID, string typeName, string description, int defaultPunishmentLevel)
    {
        TypeID = typeID;
        TypeName = typeName;
        Description = description;
        DefaultPunishmentLevel = defaultPunishmentLevel;
        IsActive = true;
    }
}

[Serializable]
public class Punishment
{
    public string PunishmentID;
    public string PlayerID;
    public string PlayerName;
    public int PunishmentType;
    public string PunishmentReason;
    public DateTime StartTime;
    public DateTime EndTime;
    public int Duration;
    public string PunisherID;
    public string PunisherName;
    public bool IsActive;
    public bool IsAppealed;
    public string AppealResult;

    public Punishment(string punishmentID, string playerID, string playerName, int punishmentType, string punishmentReason, int duration, string punisherID, string punisherName)
    {
        PunishmentID = punishmentID;
        PlayerID = playerID;
        PlayerName = playerName;
        PunishmentType = punishmentType;
        PunishmentReason = punishmentReason;
        StartTime = DateTime.Now;
        EndTime = DateTime.Now.AddHours(duration);
        Duration = duration;
        PunisherID = punisherID;
        PunisherName = punisherName;
        IsActive = true;
        IsAppealed = false;
        AppealResult = "";
    }
}

[Serializable]
public class PunishmentType
{
    public int TypeID;
    public string TypeName;
    public string Description;
    public bool IsActive;

    public PunishmentType(int typeID, string typeName, string description)
    {
        TypeID = typeID;
        TypeName = typeName;
        Description = description;
        IsActive = true;
    }
}

[Serializable]
public class PlayerReportData
{
    public string PlayerID;
    public List<Report> ReportsMade;
    public List<Report> ReportsReceived;
    public List<Punishment> Punishments;
    public int TotalReportsMade;
    public int TotalReportsReceived;
    public int TotalPunishments;
    public int ReputationScore;
    public DateTime LastReportTime;
    public DateTime LastPunishmentTime;

    public PlayerReportData(string playerID)
    {
        PlayerID = playerID;
        ReportsMade = new List<Report>();
        ReportsReceived = new List<Report>();
        Punishments = new List<Punishment>();
        TotalReportsMade = 0;
        TotalReportsReceived = 0;
        TotalPunishments = 0;
        ReputationScore = 100;
        LastReportTime = DateTime.MinValue;
        LastPunishmentTime = DateTime.MinValue;
    }
}

[Serializable]
public class ReportSystemData
{
    public List<Report> AllReports;
    public List<ReportType> ReportTypes;
    public List<Punishment> AllPunishments;
    public List<PunishmentType> PunishmentTypes;
    public Dictionary<string, PlayerReportData> PlayerReportData;
    public int MaxReportsPerDay;
    public int AutoPunishmentThreshold;
    public int AppealReviewDays;
    public DateTime LastSystemUpdate;

    public ReportSystemData()
    {
        AllReports = new List<Report>();
        ReportTypes = new List<ReportType>();
        AllPunishments = new List<Punishment>();
        PunishmentTypes = new List<PunishmentType>();
        PlayerReportData = new Dictionary<string, PlayerReportData>();
        MaxReportsPerDay = 5;
        AutoPunishmentThreshold = 3;
        AppealReviewDays = 3;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultReportTypes();
        InitializeDefaultPunishmentTypes();
    }

    private void InitializeDefaultReportTypes()
    {
        ReportTypes.Add(new ReportType(0, "恶意辱骂", "使用侮辱性语言攻击其他玩家", 2));
        ReportTypes.Add(new ReportType(1, "消极游戏", "故意送人头、挂机、拒绝合作", 2));
        ReportTypes.Add(new ReportType(2, "使用外挂", "使用第三方作弊软件", 5));
        ReportTypes.Add(new ReportType(3, "广告骚扰", "发送广告、诈骗信息", 3));
        ReportTypes.Add(new ReportType(4, "账号交易", "买卖账号、代练", 4));
        ReportTypes.Add(new ReportType(5, "其他违规", "其他违反游戏规则的行为", 1));
    }

    private void InitializeDefaultPunishmentTypes()
    {
        PunishmentTypes.Add(new PunishmentType(0, "警告", "口头警告，不影响游戏"));
        PunishmentTypes.Add(new PunishmentType(1, "禁言", "禁止在游戏内聊天"));
        PunishmentTypes.Add(new PunishmentType(2, "禁赛", "禁止参与排位赛"));
        PunishmentTypes.Add(new PunishmentType(3, "封号", "暂时封禁账号"));
        PunishmentTypes.Add(new PunishmentType(4, "永久封号", "永久封禁账号"));
    }

    public void AddReport(Report report)
    {
        AllReports.Add(report);
    }

    public void AddReportType(ReportType reportType)
    {
        ReportTypes.Add(reportType);
    }

    public void AddPunishment(Punishment punishment)
    {
        AllPunishments.Add(punishment);
    }

    public void AddPunishmentType(PunishmentType punishmentType)
    {
        PunishmentTypes.Add(punishmentType);
    }

    public void AddPlayerReportData(string playerID, PlayerReportData data)
    {
        PlayerReportData[playerID] = data;
    }
}

[Serializable]
public class ReportEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string ReportID;
    public string EventData;

    public ReportEvent(string eventID, string eventType, string playerID, string reportID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        ReportID = reportID;
        EventData = eventData;
    }
}

public class ReportSystemDataManager
{
    private static ReportSystemDataManager _instance;
    public static ReportSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ReportSystemDataManager();
            }
            return _instance;
        }
    }

    public ReportSystemData reportData;
    private List<ReportEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private ReportSystemDataManager()
    {
        reportData = new ReportSystemData();
        recentEvents = new List<ReportEvent>();
        LoadReportData();
    }

    public void SaveReportData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ReportSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, reportData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存举报系统数据失败: " + e.Message);
        }
    }

    public void LoadReportData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "ReportSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    reportData = (ReportSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载举报系统数据失败: " + e.Message);
            reportData = new ReportSystemData();
        }
    }

    public void CreateReportEvent(string eventType, string playerID, string reportID, string eventData)
    {
        string eventID = "report_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ReportEvent reportEvent = new ReportEvent(eventID, eventType, playerID, reportID, eventData);
        recentEvents.Add(reportEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<ReportEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
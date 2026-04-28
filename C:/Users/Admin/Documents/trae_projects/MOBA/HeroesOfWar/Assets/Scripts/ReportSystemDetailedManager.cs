using System;
using System.Collections.Generic;

public class ReportSystemDetailedManager
{
    private static ReportSystemDetailedManager _instance;
    public static ReportSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ReportSystemDetailedManager();
            }
            return _instance;
        }
    }

    private ReportSystemData reportData;
    private ReportSystemDataManager dataManager;

    private ReportSystemDetailedManager()
    {
        dataManager = ReportSystemDataManager.Instance;
        reportData = dataManager.reportData;
    }

    public void InitializePlayerReportData(string playerID)
    {
        if (!reportData.PlayerReportData.ContainsKey(playerID))
        {
            PlayerReportData playerReportData = new PlayerReportData(playerID);
            reportData.AddPlayerReportData(playerID, playerReportData);
            dataManager.SaveReportData();
            Debug.Log("初始化举报数据成功");
        }
    }

    public string SubmitReport(string reporterID, string reporterName, string reportedID, string reportedName, int reportType, string reportReason, string matchID = "", string evidenceURL = "")
    {
        InitializePlayerReportData(reporterID);
        InitializePlayerReportData(reportedID);
        
        PlayerReportData reporterData = reportData.PlayerReportData[reporterID];
        
        int todayReports = reporterData.ReportsMade.FindAll(r => r.ReportTime.Date == DateTime.Now.Date).Count;
        if (todayReports >= reportData.MaxReportsPerDay)
        {
            Debug.LogError("今日举报次数已达上限");
            return "";
        }
        
        string reportID = "report_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Report report = new Report(reportID, reporterID, reporterName, reportedID, reportedName, reportType, reportReason);
        report.MatchID = matchID;
        report.EvidenceURL = evidenceURL;
        reportData.AddReport(report);
        
        reporterData.ReportsMade.Add(report);
        reporterData.TotalReportsMade++;
        reporterData.LastReportTime = DateTime.Now;
        
        PlayerReportData reportedData = reportData.PlayerReportData[reportedID];
        reportedData.ReportsReceived.Add(report);
        reportedData.TotalReportsReceived++;
        
        CheckAutoPunishment(reportedID);
        
        dataManager.CreateReportEvent("report_submit", reporterID, reportID, "提交举报: " + reportReason);
        dataManager.SaveReportData();
        Debug.Log("提交举报成功: " + reportID);
        return reportID;
    }

    public void HandleReport(string reportID, string handlerID, string handlerName, bool isGuilty, string handleResult, int punishmentLevel)
    {
        Report report = reportData.AllReports.Find(r => r.ReportID == reportID);
        if (report != null && report.Status == 0)
        {
            report.Status = isGuilty ? 1 : 2;
            report.HandlerID = handlerID;
            report.HandleTime = DateTime.Now;
            report.HandleResult = handleResult;
            report.PunishmentLevel = punishmentLevel;
            
            if (isGuilty)
            {
                IssuePunishment(report.ReportedID, report.ReportedName, punishmentLevel, handleResult, handlerID, handlerName);
            }
            
            dataManager.CreateReportEvent("report_handle", handlerID, reportID, "处理举报: " + (isGuilty ? "有罪" : "无罪"));
            dataManager.SaveReportData();
            Debug.Log("处理举报成功: " + reportID);
        }
    }

    public void IssuePunishment(string playerID, string playerName, int punishmentLevel, string punishmentReason, string punisherID, string punisherName)
    {
        InitializePlayerReportData(playerID);
        PlayerReportData playerData = reportData.PlayerReportData[playerID];
        
        int punishmentType = GetPunishmentTypeByLevel(punishmentLevel);
        int duration = GetPunishmentDuration(punishmentLevel);
        
        string punishmentID = "punishment_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Punishment punishment = new Punishment(punishmentID, playerID, playerName, punishmentType, punishmentReason, duration, punisherID, punisherName);
        reportData.AddPunishment(punishment);
        
        playerData.Punishments.Add(punishment);
        playerData.TotalPunishments++;
        playerData.ReputationScore = Math.Max(0, playerData.ReputationScore - punishmentLevel * 10);
        playerData.LastPunishmentTime = DateTime.Now;
        
        dataManager.CreateReportEvent("punishment_issue", punisherID, "", "下发惩罚: " + punishmentReason);
        dataManager.SaveReportData();
        Debug.Log("下发惩罚成功: " + punishmentID);
    }

    public void AppealPunishment(string punishmentID, string playerID, string appealReason)
    {
        Punishment punishment = reportData.AllPunishments.Find(p => p.PunishmentID == punishmentID && p.PlayerID == playerID && p.IsActive);
        if (punishment != null && !punishment.IsAppealed)
        {
            punishment.IsAppealed = true;
            
            dataManager.CreateReportEvent("punishment_appeal", playerID, "", "申诉惩罚: " + appealReason);
            dataManager.SaveReportData();
            Debug.Log("申诉惩罚成功: " + punishmentID);
        }
    }

    public void ReviewAppeal(string punishmentID, bool isApproved, string reviewResult)
    {
        Punishment punishment = reportData.AllPunishments.Find(p => p.PunishmentID == punishmentID && p.IsAppealed);
        if (punishment != null)
        {
            punishment.AppealResult = reviewResult;
            if (isApproved)
            {
                punishment.IsActive = false;
                
                if (reportData.PlayerReportData.ContainsKey(punishment.PlayerID))
                {
                    PlayerReportData playerData = reportData.PlayerReportData[punishment.PlayerID];
                    playerData.ReputationScore = Math.Min(100, playerData.ReputationScore + 20);
                }
            }
            
            dataManager.CreateReportEvent("appeal_review", "system", "", "审核申诉: " + (isApproved ? "通过" : "拒绝"));
            dataManager.SaveReportData();
            Debug.Log("审核申诉成功: " + punishmentID);
        }
    }

    public void LiftPunishment(string punishmentID)
    {
        Punishment punishment = reportData.AllPunishments.Find(p => p.PunishmentID == punishmentID && p.IsActive);
        if (punishment != null)
        {
            punishment.IsActive = false;
            
            dataManager.CreateReportEvent("punishment_lift", "system", "", "解除惩罚: " + punishment.PunishmentID);
            dataManager.SaveReportData();
            Debug.Log("解除惩罚成功: " + punishmentID);
        }
    }

    private void CheckAutoPunishment(string playerID)
    {
        InitializePlayerReportData(playerID);
        PlayerReportData playerData = reportData.PlayerReportData[playerID];
        
        int recentReports = playerData.ReportsReceived.FindAll(r => (DateTime.Now - r.ReportTime).TotalDays <= 7).Count;
        if (recentReports >= reportData.AutoPunishmentThreshold)
        {
            string punishmentReason = "近期收到多次举报，系统自动处罚";
            IssuePunishment(playerID, "", 2, punishmentReason, "system", "系统");
        }
    }

    private int GetPunishmentTypeByLevel(int level)
    {
        switch (level)
        {
            case 1: return 0;
            case 2: return 1;
            case 3: return 2;
            case 4: return 3;
            case 5: return 4;
            default: return 0;
        }
    }

    private int GetPunishmentDuration(int level)
    {
        switch (level)
        {
            case 1: return 0;
            case 2: return 24;
            case 3: return 72;
            case 4: return 168;
            case 5: return 8760;
            default: return 0;
        }
    }

    public List<Report> GetPendingReports()
    {
        return reportData.AllReports.FindAll(r => r.Status == 0);
    }

    public List<Report> GetPlayerReportsMade(string playerID)
    {
        if (reportData.PlayerReportData.ContainsKey(playerID))
        {
            return reportData.PlayerReportData[playerID].ReportsMade;
        }
        return new List<Report>();
    }

    public List<Report> GetPlayerReportsReceived(string playerID)
    {
        if (reportData.PlayerReportData.ContainsKey(playerID))
        {
            return reportData.PlayerReportData[playerID].ReportsReceived;
        }
        return new List<Report>();
    }

    public List<Punishment> GetPlayerPunishments(string playerID)
    {
        if (reportData.PlayerReportData.ContainsKey(playerID))
        {
            return reportData.PlayerReportData[playerID].Punishments;
        }
        return new List<Punishment>();
    }

    public List<Punishment> GetActivePunishments(string playerID)
    {
        if (reportData.PlayerReportData.ContainsKey(playerID))
        {
            return reportData.PlayerReportData[playerID].Punishments.FindAll(p => p.IsActive && p.EndTime > DateTime.Now);
        }
        return new List<Punishment>();
    }

    public List<ReportType> GetAllReportTypes()
    {
        return reportData.ReportTypes.FindAll(t => t.IsActive);
    }

    public List<PunishmentType> GetAllPunishmentTypes()
    {
        return reportData.PunishmentTypes.FindAll(t => t.IsActive);
    }

    public int GetPlayerReputationScore(string playerID)
    {
        if (reportData.PlayerReportData.ContainsKey(playerID))
        {
            return reportData.PlayerReportData[playerID].ReputationScore;
        }
        return 100;
    }

    public void AddReportType(int typeID, string typeName, string description, int defaultPunishmentLevel)
    {
        ReportType reportType = new ReportType(typeID, typeName, description, defaultPunishmentLevel);
        reportData.AddReportType(reportType);
        dataManager.SaveReportData();
        Debug.Log("添加举报类型成功: " + typeName);
    }

    public void AddPunishmentType(int typeID, string typeName, string description)
    {
        PunishmentType punishmentType = new PunishmentType(typeID, typeName, description);
        reportData.AddPunishmentType(punishmentType);
        dataManager.SaveReportData();
        Debug.Log("添加惩罚类型成功: " + typeName);
    }

    public void CleanupExpiredPunishments()
    {
        List<Punishment> expiredPunishments = new List<Punishment>();
        foreach (Punishment punishment in reportData.AllPunishments)
        {
            if (punishment.IsActive && punishment.EndTime < DateTime.Now)
            {
                punishment.IsActive = false;
                expiredPunishments.Add(punishment);
            }
        }
        
        if (expiredPunishments.Count > 0)
        {
            dataManager.CreateReportEvent("punishment_cleanup", "system", "", "清理过期惩罚: " + expiredPunishments.Count);
            dataManager.SaveReportData();
            Debug.Log("清理过期惩罚成功: " + expiredPunishments.Count);
        }
    }

    public void CleanupOldReports(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<Report> oldReports = new List<Report>();
        foreach (Report report in reportData.AllReports)
        {
            if (report.ReportTime < cutoffDate && report.Status != 0)
            {
                oldReports.Add(report);
            }
        }
        
        foreach (Report report in oldReports)
        {
            reportData.AllReports.Remove(report);
            
            if (reportData.PlayerReportData.ContainsKey(report.ReporterID))
            {
                PlayerReportData reporterData = reportData.PlayerReportData[report.ReporterID];
                reporterData.ReportsMade.Remove(report);
            }
            
            if (reportData.PlayerReportData.ContainsKey(report.ReportedID))
            {
                PlayerReportData reportedData = reportData.PlayerReportData[report.ReportedID];
                reportedData.ReportsReceived.Remove(report);
            }
        }
        
        if (oldReports.Count > 0)
        {
            dataManager.CreateReportEvent("report_cleanup", "system", "", "清理旧举报: " + oldReports.Count);
            dataManager.SaveReportData();
            Debug.Log("清理旧举报成功: " + oldReports.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveReportData();
    }

    public void LoadData()
    {
        dataManager.LoadReportData();
    }

    public List<ReportEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
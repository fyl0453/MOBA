[System.Serializable]
public class Report
{
    public string reportID;
    public string reporterID;
    public string reporterName;
    public string reportedID;
    public string reportedName;
    public string reportType;
    public string reportReason;
    public string reportDescription;
    public string matchID;
    public string reportTime;
    public string status;
    public string handlerID;
    public string handleTime;
    public string handleResult;
    
    public Report(string id, string reporter, string reporterName, string reported, string reportedName, string type, string reason, string desc, string match)
    {
        reportID = id;
        this.reporterID = reporter;
        this.reporterName = reporterName;
        this.reportedID = reported;
        this.reportedName = reportedName;
        reportType = type;
        reportReason = reason;
        reportDescription = desc;
        matchID = match;
        reportTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        status = "Pending";
        handlerID = "";
        handleTime = "";
        handleResult = "";
    }
    
    public void HandleReport(string handler, string result)
    {
        status = "Handled";
        handlerID = handler;
        handleTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        handleResult = result;
    }
    
    public void RejectReport(string handler, string reason)
    {
        status = "Rejected";
        handlerID = handler;
        handleTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        handleResult = reason;
    }
}

[System.Serializable]
public class ReportManagerData
{
    public List<Report> reports;
    
    public ReportManagerData()
    {
        reports = new List<Report>();
    }
    
    public void AddReport(Report report)
    {
        reports.Add(report);
    }
    
    public Report GetReport(string reportID)
    {
        return reports.Find(r => r.reportID == reportID);
    }
    
    public List<Report> GetPendingReports()
    {
        return reports.FindAll(r => r.status == "Pending");
    }
    
    public List<Report> GetReportsByReporter(string reporterID)
    {
        return reports.FindAll(r => r.reporterID == reporterID);
    }
    
    public List<Report> GetReportsByReported(string reportedID)
    {
        return reports.FindAll(r => r.reportedID == reportedID);
    }
}
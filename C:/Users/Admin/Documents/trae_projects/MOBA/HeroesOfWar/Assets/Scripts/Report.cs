[System.Serializable]
public class Report
{
    public string reportedPlayer;
    public ReportManager.ReportReason reason;
    public string description;
    public System.DateTime timestamp;
    
    public Report(string player, ReportManager.ReportReason reportReason, string desc)
    {
        reportedPlayer = player;
        reason = reportReason;
        description = desc;
        timestamp = System.DateTime.Now;
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ReportManager : MonoBehaviour
{
    public static ReportManager Instance { get; private set; }
    
    public ReportManagerData reportData;
    
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
        LoadReportData();
        
        if (reportData == null)
        {
            reportData = new ReportManagerData();
        }
    }
    
    public Report SubmitReport(string reportedID, string reportedName, string reportType, string reportReason, string reportDescription, string matchID)
    {
        string reportID = "report_" + System.DateTime.Now.Ticks;
        string reporterID = ProfileManager.Instance.currentProfile.playerID;
        string reporterName = ProfileManager.Instance.currentProfile.playerName;
        
        Report report = new Report(reportID, reporterID, reporterName, reportedID, reportedName, reportType, reportReason, reportDescription, matchID);
        reportData.AddReport(report);
        SaveReportData();
        
        return report;
    }
    
    public void HandleReport(string reportID, string handlerID, string result)
    {
        Report report = reportData.GetReport(reportID);
        if (report != null && report.status == "Pending")
        {
            report.HandleReport(handlerID, result);
            SaveReportData();
        }
    }
    
    public void RejectReport(string reportID, string handlerID, string reason)
    {
        Report report = reportData.GetReport(reportID);
        if (report != null && report.status == "Pending")
        {
            report.RejectReport(handlerID, reason);
            SaveReportData();
        }
    }
    
    public List<Report> GetPendingReports()
    {
        return reportData.GetPendingReports();
    }
    
    public List<Report> GetUserReports()
    {
        return reportData.GetReportsByReporter(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public Report GetReport(string reportID)
    {
        return reportData.GetReport(reportID);
    }
    
    public void SaveReportData()
    {
        string path = Application.dataPath + "/Data/report_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, reportData);
        stream.Close();
    }
    
    public void LoadReportData()
    {
        string path = Application.dataPath + "/Data/report_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            reportData = (ReportManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            reportData = new ReportManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }
    
    public FeedbackManagerData feedbackData;
    
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
        LoadFeedbackData();
        
        if (feedbackData == null)
        {
            feedbackData = new FeedbackManagerData();
            InitializeDefaultFeedbackCategories();
        }
    }
    
    private void InitializeDefaultFeedbackCategories()
    {
        // 创建默认反馈分类
        FeedbackCategory category1 = new FeedbackCategory("category_bug", "游戏 bug", "游戏中遇到的各种 bug");
        FeedbackCategory category2 = new FeedbackCategory("category_suggestion", "游戏建议", "对游戏的改进建议");
        FeedbackCategory category3 = new FeedbackCategory("category_complaint", "投诉举报", "投诉其他玩家或举报外挂");
        FeedbackCategory category4 = new FeedbackCategory("category_other", "其他问题", "其他类型的问题");
        
        feedbackData.system.AddCategory(category1);
        feedbackData.system.AddCategory(category2);
        feedbackData.system.AddCategory(category3);
        feedbackData.system.AddCategory(category4);
        
        SaveFeedbackData();
    }
    
    public string SubmitFeedback(string playerID, string playerName, string categoryID, string title, string content, List<string> attachmentURLs = null)
    {
        string feedbackID = System.Guid.NewGuid().ToString();
        Feedback newFeedback = new Feedback(feedbackID, playerID, playerName, categoryID, title, content);
        
        if (attachmentURLs != null)
        {
            foreach (string url in attachmentURLs)
            {
                newFeedback.AddAttachment(url);
            }
        }
        
        feedbackData.system.AddFeedback(newFeedback);
        SaveFeedbackData();
        Debug.Log($"成功提交反馈: {title}");
        return feedbackID;
    }
    
    public void HandleFeedback(string feedbackID, string handlerID, string handlerName, string reply)
    {
        Feedback feedback = feedbackData.system.GetFeedback(feedbackID);
        if (feedback != null)
        {
            feedback.Handle(handlerID, handlerName, reply);
            SaveFeedbackData();
            Debug.Log($"成功处理反馈: {feedback.title}");
        }
    }
    
    public void UpdateFeedbackStatus(string feedbackID, string status)
    {
        Feedback feedback = feedbackData.system.GetFeedback(feedbackID);
        if (feedback != null)
        {
            feedback.UpdateStatus(status);
            SaveFeedbackData();
            Debug.Log($"成功更新反馈状态: {feedback.title} -> {status}");
        }
    }
    
    public List<Feedback> GetPlayerFeedbacks(string playerID, int limit = 50)
    {
        List<Feedback> feedbacks = feedbackData.system.GetFeedbacksByPlayer(playerID);
        feedbacks.Sort((a, b) => b.submitTime.CompareTo(a.submitTime));
        return feedbacks.GetRange(0, Mathf.Min(limit, feedbacks.Count));
    }
    
    public List<Feedback> GetFeedbacksByStatus(string status, int limit = 100)
    {
        List<Feedback> feedbacks = feedbackData.system.GetFeedbacksByStatus(status);
        feedbacks.Sort((a, b) => b.submitTime.CompareTo(a.submitTime));
        return feedbacks.GetRange(0, Mathf.Min(limit, feedbacks.Count));
    }
    
    public List<Feedback> GetFeedbacksByCategory(string categoryID, int limit = 100)
    {
        List<Feedback> feedbacks = feedbackData.system.GetFeedbacksByCategory(categoryID);
        feedbacks.Sort((a, b) => b.submitTime.CompareTo(a.submitTime));
        return feedbacks.GetRange(0, Mathf.Min(limit, feedbacks.Count));
    }
    
    public Feedback GetFeedback(string feedbackID)
    {
        return feedbackData.system.GetFeedback(feedbackID);
    }
    
    public List<FeedbackCategory> GetAllCategories()
    {
        return feedbackData.system.categories;
    }
    
    public void AddFeedbackCategory(string name, string description)
    {
        string categoryID = System.Guid.NewGuid().ToString();
        FeedbackCategory newCategory = new FeedbackCategory(categoryID, name, description);
        feedbackData.system.AddCategory(newCategory);
        SaveFeedbackData();
        Debug.Log($"成功添加反馈分类: {name}");
    }
    
    public void SaveFeedbackData()
    {
        string path = Application.dataPath + "/Data/feedback_system_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, feedbackData);
        stream.Close();
    }
    
    public void LoadFeedbackData()
    {
        string path = Application.dataPath + "/Data/feedback_system_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            feedbackData = (FeedbackManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            feedbackData = new FeedbackManagerData();
        }
    }
}
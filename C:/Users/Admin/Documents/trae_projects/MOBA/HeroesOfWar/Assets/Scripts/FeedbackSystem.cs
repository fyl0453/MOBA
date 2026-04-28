[System.Serializable]
public class FeedbackSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Feedback> feedbacks;
    public List<FeedbackCategory> categories;
    
    public FeedbackSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        feedbacks = new List<Feedback>();
        categories = new List<FeedbackCategory>();
    }
    
    public void AddFeedback(Feedback feedback)
    {
        feedbacks.Add(feedback);
    }
    
    public void AddCategory(FeedbackCategory category)
    {
        categories.Add(category);
    }
    
    public Feedback GetFeedback(string feedbackID)
    {
        return feedbacks.Find(f => f.feedbackID == feedbackID);
    }
    
    public FeedbackCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public List<Feedback> GetFeedbacksByPlayer(string playerID)
    {
        return feedbacks.FindAll(f => f.playerID == playerID);
    }
    
    public List<Feedback> GetFeedbacksByCategory(string categoryID)
    {
        return feedbacks.FindAll(f => f.categoryID == categoryID);
    }
    
    public List<Feedback> GetFeedbacksByStatus(string status)
    {
        return feedbacks.FindAll(f => f.status == status);
    }
}

[System.Serializable]
public class FeedbackCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    
    public FeedbackCategory(string id, string name, string desc)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = desc;
    }
}

[System.Serializable]
public class Feedback
{
    public string feedbackID;
    public string playerID;
    public string playerName;
    public string categoryID;
    public string title;
    public string content;
    public string status;
    public string submitTime;
    public string handleTime;
    public string handlerID;
    public string handlerName;
    public string reply;
    public List<string> attachmentURLs;
    
    public Feedback(string id, string playerID, string playerName, string categoryID, string title, string content)
    {
        feedbackID = id;
        this.playerID = playerID;
        this.playerName = playerName;
        this.categoryID = categoryID;
        this.title = title;
        this.content = content;
        status = "Pending";
        submitTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        handleTime = "";
        handlerID = "";
        handlerName = "";
        reply = "";
        attachmentURLs = new List<string>();
    }
    
    public void AddAttachment(string url)
    {
        attachmentURLs.Add(url);
    }
    
    public void UpdateStatus(string status)
    {
        this.status = status;
    }
    
    public void Handle(string handlerID, string handlerName, string reply)
    {
        status = "Handled";
        handleTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.handlerID = handlerID;
        this.handlerName = handlerName;
        this.reply = reply;
    }
}

[System.Serializable]
public class FeedbackManagerData
{
    public FeedbackSystem system;
    
    public FeedbackManagerData()
    {
        system = new FeedbackSystem("feedback_system", "反馈系统", "管理玩家反馈");
    }
}
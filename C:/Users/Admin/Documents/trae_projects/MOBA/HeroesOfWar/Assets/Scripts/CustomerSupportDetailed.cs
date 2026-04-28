[System.Serializable]
public class CustomerSupportDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<SupportTicket> supportTickets;
    public List<ChatSession> chatSessions;
    public List<Feedback> feedbacks;
    public List<AccountRecovery> accountRecoveries;
    public List<FAQ> faqs;
    public List<SupportAgent> supportAgents;
    
    public CustomerSupportDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        supportTickets = new List<SupportTicket>();
        chatSessions = new List<ChatSession>();
        feedbacks = new List<Feedback>();
        accountRecoveries = new List<AccountRecovery>();
        faqs = new List<FAQ>();
        supportAgents = new List<SupportAgent>();
    }
    
    public void AddSupportTicket(SupportTicket ticket)
    {
        supportTickets.Add(ticket);
    }
    
    public void AddChatSession(ChatSession session)
    {
        chatSessions.Add(session);
    }
    
    public void AddFeedback(Feedback feedback)
    {
        feedbacks.Add(feedback);
    }
    
    public void AddAccountRecovery(AccountRecovery recovery)
    {
        accountRecoveries.Add(recovery);
    }
    
    public void AddFAQ(FAQ faq)
    {
        faqs.Add(faq);
    }
    
    public void AddSupportAgent(SupportAgent agent)
    {
        supportAgents.Add(agent);
    }
    
    public SupportTicket GetSupportTicket(string ticketID)
    {
        return supportTickets.Find(t => t.ticketID == ticketID);
    }
    
    public ChatSession GetChatSession(string sessionID)
    {
        return chatSessions.Find(s => s.sessionID == sessionID);
    }
    
    public Feedback GetFeedback(string feedbackID)
    {
        return feedbacks.Find(f => f.feedbackID == feedbackID);
    }
    
    public AccountRecovery GetAccountRecovery(string recoveryID)
    {
        return accountRecoveries.Find(r => r.recoveryID == recoveryID);
    }
    
    public FAQ GetFAQ(string faqID)
    {
        return faqs.Find(f => f.faqID == faqID);
    }
    
    public SupportAgent GetSupportAgent(string agentID)
    {
        return supportAgents.Find(a => a.agentID == agentID);
    }
    
    public List<SupportTicket> GetTicketsByUser(string userID)
    {
        return supportTickets.FindAll(t => t.userID == userID);
    }
    
    public List<ChatSession> GetChatSessionsByUser(string userID)
    {
        return chatSessions.FindAll(s => s.userID == userID);
    }
    
    public List<Feedback> GetFeedbacksByUser(string userID)
    {
        return feedbacks.FindAll(f => f.userID == userID);
    }
    
    public List<AccountRecovery> GetAccountRecoveriesByUser(string userID)
    {
        return accountRecoveries.FindAll(r => r.userID == userID);
    }
    
    public List<SupportTicket> GetTicketsByAgent(string agentID)
    {
        return supportTickets.FindAll(t => t.assignedAgentID == agentID);
    }
    
    public List<ChatSession> GetChatSessionsByAgent(string agentID)
    {
        return chatSessions.FindAll(s => s.agentID == agentID);
    }
}

[System.Serializable]
public class SupportTicket
{
    public string ticketID;
    public string userID;
    public string userEmail;
    public string subject;
    public string description;
    public string category;
    public string priority;
    public string status;
    public string assignedAgentID;
    public string creationDate;
    public string lastUpdateDate;
    public string resolutionDate;
    public List<TicketMessage> messages;
    
    public SupportTicket(string id, string userID, string userEmail, string subject, string description, string category, string priority)
    {
        ticketID = id;
        this.userID = userID;
        this.userEmail = userEmail;
        this.subject = subject;
        this.description = description;
        this.category = category;
        this.priority = priority;
        status = "Open";
        assignedAgentID = "";
        creationDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastUpdateDate = creationDate;
        resolutionDate = "";
        messages = new List<TicketMessage>();
    }
    
    public void AddMessage(TicketMessage message)
    {
        messages.Add(message);
        lastUpdateDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AssignAgent(string agentID)
    {
        assignedAgentID = agentID;
        status = "Assigned";
        lastUpdateDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Resolve(string resolution)
    {
        status = "Resolved";
        resolutionDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lastUpdateDate = resolutionDate;
    }
    
    public void Close()
    {
        status = "Closed";
        lastUpdateDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class TicketMessage
{
    public string messageID;
    public string ticketID;
    public string senderID;
    public string senderType;
    public string content;
    public string timestamp;
    
    public TicketMessage(string id, string ticketID, string senderID, string senderType, string content)
    {
        messageID = id;
        this.ticketID = ticketID;
        this.senderID = senderID;
        this.senderType = senderType;
        this.content = content;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class ChatSession
{
    public string sessionID;
    public string userID;
    public string agentID;
    public string status;
    public string startTime;
    public string endTime;
    public List<ChatMessage> messages;
    
    public ChatSession(string id, string userID, string agentID)
    {
        sessionID = id;
        this.userID = userID;
        this.agentID = agentID;
        status = "Active";
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        endTime = "";
        messages = new List<ChatMessage>();
    }
    
    public void AddMessage(ChatMessage message)
    {
        messages.Add(message);
    }
    
    public void EndSession()
    {
        status = "Ended";
        endTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class ChatMessage
{
    public string messageID;
    public string sessionID;
    public string senderID;
    public string senderType;
    public string content;
    public string timestamp;
    
    public ChatMessage(string id, string sessionID, string senderID, string senderType, string content)
    {
        messageID = id;
        this.sessionID = sessionID;
        this.senderID = senderID;
        this.senderType = senderType;
        this.content = content;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class Feedback
{
    public string feedbackID;
    public string userID;
    public string feedbackType;
    public string content;
    public string severity;
    public string status;
    public string submissionDate;
    public string processingDate;
    public string resolution;
    
    public Feedback(string id, string userID, string feedbackType, string content, string severity)
    {
        feedbackID = id;
        this.userID = userID;
        this.feedbackType = feedbackType;
        this.content = content;
        this.severity = severity;
        status = "Submitted";
        submissionDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        processingDate = "";
        resolution = "";
    }
    
    public void Process()
    {
        status = "Processing";
        processingDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Resolve(string resolution)
    {
        status = "Resolved";
        this.resolution = resolution;
    }
}

[System.Serializable]
public class AccountRecovery
{
    public string recoveryID;
    public string userID;
    public string accountName;
    public string email;
    public string phoneNumber;
    public string verificationMethod;
    public string status;
    public string submissionDate;
    public string verificationDate;
    public string recoveryDate;
    
    public AccountRecovery(string id, string userID, string accountName, string email, string phoneNumber, string verificationMethod)
    {
        recoveryID = id;
        this.userID = userID;
        this.accountName = accountName;
        this.email = email;
        this.phoneNumber = phoneNumber;
        this.verificationMethod = verificationMethod;
        status = "Submitted";
        submissionDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        verificationDate = "";
        recoveryDate = "";
    }
    
    public void Verify()
    {
        status = "Verified";
        verificationDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void Recover()
    {
        status = "Recovered";
        recoveryDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class FAQ
{
    public string faqID;
    public string question;
    public string answer;
    public string category;
    public int viewCount;
    public int helpfulCount;
    public bool isEnabled;
    
    public FAQ(string id, string question, string answer, string category)
    {
        faqID = id;
        this.question = question;
        this.answer = answer;
        this.category = category;
        viewCount = 0;
        helpfulCount = 0;
        isEnabled = true;
    }
    
    public void IncrementViewCount()
    {
        viewCount++;
    }
    
    public void IncrementHelpfulCount()
    {
        helpfulCount++;
    }
}

[System.Serializable]
public class SupportAgent
{
    public string agentID;
    public string agentName;
    public string email;
    public string status;
    public string role;
    public int activeChats;
    public int resolvedTickets;
    public string lastLoginDate;
    
    public SupportAgent(string id, string name, string email, string role)
    {
        agentID = id;
        agentName = name;
        this.email = email;
        status = "Offline";
        this.role = role;
        activeChats = 0;
        resolvedTickets = 0;
        lastLoginDate = "";
    }
    
    public void SetOnline()
    {
        status = "Online";
        lastLoginDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void SetOffline()
    {
        status = "Offline";
    }
    
    public void IncrementActiveChats()
    {
        activeChats++;
    }
    
    public void DecrementActiveChats()
    {
        if (activeChats > 0)
        {
            activeChats--;
        }
    }
    
    public void IncrementResolvedTickets()
    {
        resolvedTickets++;
    }
}

[System.Serializable]
public class CustomerSupportDetailedManagerData
{
    public CustomerSupportDetailed system;
    
    public CustomerSupportDetailedManagerData()
    {
        system = new CustomerSupportDetailed("customer_support_detailed", "客服支持详细系统", "管理客服支持的详细功能，包括在线客服、问题反馈和账号找回");
    }
}
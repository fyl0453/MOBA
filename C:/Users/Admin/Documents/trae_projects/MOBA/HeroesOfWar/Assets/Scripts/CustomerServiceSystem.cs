[System.Serializable]
public class CustomerServiceSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<CustomerServiceTicket> tickets;
    public List<CustomerServiceAgent> agents;
    public List<FAQ> faqs;
    
    public CustomerServiceSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        tickets = new List<CustomerServiceTicket>();
        agents = new List<CustomerServiceAgent>();
        faqs = new List<FAQ>();
    }
    
    public void AddTicket(CustomerServiceTicket ticket)
    {
        tickets.Add(ticket);
    }
    
    public void AddAgent(CustomerServiceAgent agent)
    {
        agents.Add(agent);
    }
    
    public void AddFAQ(FAQ faq)
    {
        faqs.Add(faq);
    }
    
    public CustomerServiceTicket GetTicket(string ticketID)
    {
        return tickets.Find(t => t.ticketID == ticketID);
    }
    
    public CustomerServiceAgent GetAgent(string agentID)
    {
        return agents.Find(a => a.agentID == agentID);
    }
    
    public FAQ GetFAQ(string faqID)
    {
        return faqs.Find(f => f.faqID == faqID);
    }
    
    public List<CustomerServiceTicket> GetTicketsByPlayer(string playerID)
    {
        return tickets.FindAll(t => t.playerID == playerID);
    }
    
    public List<CustomerServiceTicket> GetTicketsByAgent(string agentID)
    {
        return tickets.FindAll(t => t.agentID == agentID);
    }
    
    public List<CustomerServiceTicket> GetTicketsByStatus(string status)
    {
        return tickets.FindAll(t => t.status == status);
    }
}

[System.Serializable]
public class CustomerServiceTicket
{
    public string ticketID;
    public string playerID;
    public string playerName;
    public string subject;
    public string description;
    public string status;
    public string priority;
    public string category;
    public string agentID;
    public string agentName;
    public string submitTime;
    public string handleTime;
    public string closeTime;
    public List<TicketMessage> messages;
    
    public CustomerServiceTicket(string id, string playerID, string playerName, string subject, string description, string category)
    {
        ticketID = id;
        this.playerID = playerID;
        this.playerName = playerName;
        this.subject = subject;
        this.description = description;
        status = "Pending";
        priority = "Medium";
        this.category = category;
        agentID = "";
        agentName = "";
        submitTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        handleTime = "";
        closeTime = "";
        messages = new List<TicketMessage>();
    }
    
    public void AddMessage(TicketMessage message)
    {
        messages.Add(message);
    }
    
    public void AssignToAgent(string agentID, string agentName)
    {
        this.agentID = agentID;
        this.agentName = agentName;
        status = "Assigned";
        handleTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void CloseTicket()
    {
        status = "Closed";
        closeTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateStatus(string status)
    {
        this.status = status;
    }
    
    public void UpdatePriority(string priority)
    {
        this.priority = priority;
    }
}

[System.Serializable]
public class TicketMessage
{
    public string messageID;
    public string ticketID;
    public string senderID;
    public string senderName;
    public string senderType;
    public string content;
    public string sendTime;
    
    public TicketMessage(string id, string ticketID, string senderID, string senderName, string senderType, string content)
    {
        messageID = id;
        this.ticketID = ticketID;
        this.senderID = senderID;
        this.senderName = senderName;
        this.senderType = senderType;
        this.content = content;
        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CustomerServiceAgent
{
    public string agentID;
    public string agentName;
    public string agentEmail;
    public string agentPhone;
    public bool isOnline;
    public int handledTickets;
    
    public CustomerServiceAgent(string id, string name, string email, string phone)
    {
        agentID = id;
        agentName = name;
        agentEmail = email;
        agentPhone = phone;
        isOnline = false;
        handledTickets = 0;
    }
    
    public void SetOnline(bool online)
    {
        isOnline = online;
    }
    
    public void IncrementHandledTickets()
    {
        handledTickets++;
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
    public string createdAt;
    
    public FAQ(string id, string question, string answer, string category)
    {
        faqID = id;
        this.question = question;
        this.answer = answer;
        this.category = category;
        viewCount = 0;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void IncrementViewCount()
    {
        viewCount++;
    }
}

[System.Serializable]
public class CustomerServiceManagerData
{
    public CustomerServiceSystem system;
    
    public CustomerServiceManagerData()
    {
        system = new CustomerServiceSystem("customer_service_system", "客服系统", "管理游戏客服支持");
    }
}
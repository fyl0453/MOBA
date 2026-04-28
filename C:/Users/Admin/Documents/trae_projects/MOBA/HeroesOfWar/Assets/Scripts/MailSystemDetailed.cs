[System.Serializable]
public class MailSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Mail> mails;
    public List<MailAttachment> mailAttachments;
    public List<MailEvent> mailEvents;
    
    public MailSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        mails = new List<Mail>();
        mailAttachments = new List<MailAttachment>();
        mailEvents = new List<MailEvent>();
    }
    
    public void AddMail(Mail mail)
    {
        mails.Add(mail);
    }
    
    public void AddMailAttachment(MailAttachment attachment)
    {
        mailAttachments.Add(attachment);
    }
    
    public void AddMailEvent(MailEvent mailEvent)
    {
        mailEvents.Add(mailEvent);
    }
    
    public Mail GetMail(string mailID)
    {
        return mails.Find(m => m.mailID == mailID);
    }
    
    public MailAttachment GetMailAttachment(string attachmentID)
    {
        return mailAttachments.Find(ma => ma.attachmentID == attachmentID);
    }
    
    public MailEvent GetMailEvent(string eventID)
    {
        return mailEvents.Find(me => me.eventID == eventID);
    }
    
    public List<Mail> GetMailsByUser(string userID)
    {
        return mails.FindAll(m => m.receiverID == userID);
    }
    
    public List<Mail> GetMailsByStatus(string status)
    {
        return mails.FindAll(m => m.status == status);
    }
    
    public List<Mail> GetMailsByType(string mailType)
    {
        return mails.FindAll(m => m.mailType == mailType);
    }
    
    public List<MailEvent> GetMailEventsByUser(string userID)
    {
        return mailEvents.FindAll(me => me.userID == userID);
    }
}

[System.Serializable]
public class Mail
{
    public string mailID;
    public string senderID;
    public string senderName;
    public string receiverID;
    public string receiverName;
    public string subject;
    public string content;
    public string mailType;
    public string status;
    public string sendTime;
    public string readTime;
    public string expireTime;
    public bool hasAttachment;
    public List<string> attachmentIDs;
    
    public Mail(string id, string senderID, string senderName, string receiverID, string receiverName, string subject, string content, string mailType, bool hasAttachment)
    {
        mailID = id;
        this.senderID = senderID;
        this.senderName = senderName;
        this.receiverID = receiverID;
        this.receiverName = receiverName;
        this.subject = subject;
        this.content = content;
        this.mailType = mailType;
        status = "unread";
        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        readTime = "";
        expireTime = System.DateTime.Now.AddDays(30).ToString("yyyy-MM-dd HH:mm:ss");
        this.hasAttachment = hasAttachment;
        attachmentIDs = new List<string>();
    }
    
    public void Read()
    {
        status = "read";
        readTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void ClaimAttachment()
    {
        status = "claimed";
    }
    
    public void Delete()
    {
        status = "deleted";
    }
    
    public void AddAttachment(string attachmentID)
    {
        if (!attachmentIDs.Contains(attachmentID))
        {
            attachmentIDs.Add(attachmentID);
        }
    }
    
    public void RemoveAttachment(string attachmentID)
    {
        if (attachmentIDs.Contains(attachmentID))
        {
            attachmentIDs.Remove(attachmentID);
        }
    }
    
    public bool IsExpired()
    {
        System.DateTime now = System.DateTime.Now;
        System.DateTime expire = System.DateTime.Parse(expireTime);
        return now > expire;
    }
}

[System.Serializable]
public class MailAttachment
{
    public string attachmentID;
    public string mailID;
    public string attachmentType;
    public string attachmentValue;
    public int quantity;
    public string icon;
    public bool isClaimed;
    public string claimTime;
    
    public MailAttachment(string id, string mailID, string attachmentType, string attachmentValue, int quantity, string icon)
    {
        attachmentID = id;
        this.mailID = mailID;
        this.attachmentType = attachmentType;
        this.attachmentValue = attachmentValue;
        this.quantity = quantity;
        this.icon = icon;
        isClaimed = false;
        claimTime = "";
    }
    
    public void Claim()
    {
        isClaimed = true;
        claimTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class MailEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string mailID;
    public string description;
    public string timestamp;
    public string status;
    
    public MailEvent(string id, string eventType, string userID, string mailID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.mailID = mailID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
    }
    
    public void MarkAsCompleted()
    {
        status = "completed";
    }
    
    public void MarkAsFailed()
    {
        status = "failed";
    }
}

[System.Serializable]
public class MailSystemDetailedManagerData
{
    public MailSystemDetailed system;
    
    public MailSystemDetailedManagerData()
    {
        system = new MailSystemDetailed("mail_system_detailed", "邮件系统详细", "管理邮件的详细功能，包括邮件发送、接收和管理");
    }
}
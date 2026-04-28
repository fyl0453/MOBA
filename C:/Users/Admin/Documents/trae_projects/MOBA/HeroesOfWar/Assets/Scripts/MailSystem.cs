[System.Serializable]
public class Mail
{
    public string mailID;
    public string sender;
    public string subject;
    public string content;
    public string sendTime;
    public string expireTime;
    public bool isRead;
    public bool hasAttachment;
    public List<MailAttachment> attachments;
    
    public Mail(string id, string from, string subj, string cont, bool hasAttach = false)
    {
        mailID = id;
        sender = from;
        subject = subj;
        content = cont;
        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        expireTime = System.DateTime.Now.AddDays(30).ToString("yyyy-MM-dd HH:mm");
        isRead = false;
        hasAttachment = hasAttach;
        attachments = new List<MailAttachment>();
    }
    
    public void AddAttachment(string itemID, int quantity, string attachmentType = "Item")
    {
        MailAttachment attachment = new MailAttachment(itemID, quantity, attachmentType);
        attachments.Add(attachment);
        hasAttachment = true;
    }
    
    public void MarkAsRead()
    {
        isRead = true;
    }
    
    public bool IsExpired()
    {
        System.DateTime expireDate;
        if (System.DateTime.TryParse(expireTime, out expireDate))
        {
            return System.DateTime.Now > expireDate;
        }
        return false;
    }
}

[System.Serializable]
public class MailAttachment
{
    public string attachmentID;
    public string itemID;
    public int quantity;
    public string attachmentType;
    public bool isClaimed;
    
    public MailAttachment(string item, int qty, string type = "Item")
    {
        attachmentID = "attachment_" + System.DateTime.Now.Ticks;
        itemID = item;
        quantity = qty;
        attachmentType = type;
        isClaimed = false;
    }
    
    public void ClaimAttachment()
    {
        isClaimed = true;
    }
}

[System.Serializable]
public class MailBox
{
    public string playerID;
    public List<Mail> mails;
    public int maxMails;
    
    public MailBox(string id, int max = 100)
    {
        playerID = id;
        mails = new List<Mail>();
        maxMails = max;
    }
    
    public void AddMail(Mail mail)
    {
        if (mails.Count >= maxMails)
        {
            // 删除最旧的邮件
            Mail oldestMail = mails[0];
            mails.Remove(oldestMail);
        }
        mails.Add(mail);
    }
    
    public void RemoveMail(string mailID)
    {
        mails.RemoveAll(m => m.mailID == mailID);
    }
    
    public void RemoveExpiredMails()
    {
        mails.RemoveAll(m => m.IsExpired());
    }
    
    public List<Mail> GetUnreadMails()
    {
        return mails.FindAll(m => !m.isRead);
    }
    
    public List<Mail> GetMailsWithAttachments()
    {
        return mails.FindAll(m => m.hasAttachment);
    }
    
    public Mail GetMailByID(string mailID)
    {
        return mails.Find(m => m.mailID == mailID);
    }
}
using System;
using System.Collections.Generic;

public class EmailSystemDetailedManager
{
    private static EmailSystemDetailedManager _instance;
    public static EmailSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EmailSystemDetailedManager();
            }
            return _instance;
        }
    }

    private EmailSystemData emailData;
    private EmailSystemDataManager dataManager;

    private EmailSystemDetailedManager()
    {
        dataManager = EmailSystemDataManager.Instance;
        emailData = dataManager.emailData;
    }

    public string SendEmail(string sender, string receiverID, string subject, string content, int emailType, List<EmailAttachment> attachments = null)
    {
        string emailID = "email_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Email email = new Email(emailID, sender, receiverID, subject, content, emailType);
        if (attachments != null && attachments.Count > 0)
        {
            email.Attachments = attachments;
        }
        emailData.AddEmail(email);
        dataManager.CreateEmailEvent("email_send", receiverID, emailID, "发送邮件: " + subject);
        dataManager.SaveEmailData();
        Debug.Log("发送邮件成功: " + subject);
        return emailID;
    }

    public string SendSystemEmail(string receiverID, string subject, string content, int emailType, List<EmailAttachment> attachments = null)
    {
        return SendEmail("系统", receiverID, subject, content, emailType, attachments);
    }

    public string SendBatchEmail(List<string> receiverIDs, string subject, string content, int emailType, List<EmailAttachment> attachments = null)
    {
        string batchID = "batch_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        foreach (string receiverID in receiverIDs)
        {
            SendSystemEmail(receiverID, subject, content, emailType, attachments);
        }
        dataManager.CreateEmailEvent("email_batch_send", "system", batchID, "批量发送邮件: " + subject);
        Debug.Log("批量发送邮件成功: " + subject);
        return batchID;
    }

    public void MarkEmailAsRead(string emailID)
    {
        Email email = GetEmail(emailID);
        if (email != null && !email.IsRead)
        {
            email.IsRead = true;
            dataManager.CreateEmailEvent("email_read", email.ReceiverID, emailID, "标记邮件为已读");
            dataManager.SaveEmailData();
            Debug.Log("标记邮件为已读成功");
        }
    }

    public void MarkEmailAsUnread(string emailID)
    {
        Email email = GetEmail(emailID);
        if (email != null && email.IsRead)
        {
            email.IsRead = false;
            dataManager.SaveEmailData();
            Debug.Log("标记邮件为未读成功");
        }
    }

    public void ClaimEmailAttachment(string emailID)
    {
        Email email = GetEmail(emailID);
        if (email != null && !email.IsClaimed && email.Attachments.Count > 0)
        {
            foreach (EmailAttachment attachment in email.Attachments)
            {
                attachment.IsClaimed = true;
            }
            email.IsClaimed = true;
            dataManager.CreateEmailEvent("email_claim", email.ReceiverID, emailID, "领取邮件附件");
            dataManager.SaveEmailData();
            Debug.Log("领取邮件附件成功");
        }
    }

    public void DeleteEmail(string emailID, string receiverID)
    {
        Email email = GetEmail(emailID);
        if (email != null && email.ReceiverID == receiverID)
        {
            emailData.RemoveEmail(emailID, receiverID);
            dataManager.CreateEmailEvent("email_delete", receiverID, emailID, "删除邮件");
            dataManager.SaveEmailData();
            Debug.Log("删除邮件成功");
        }
    }

    public void DeleteAllEmails(string receiverID)
    {
        if (emailData.PlayerEmails.ContainsKey(receiverID))
        {
            List<string> emailIDs = new List<string>(emailData.PlayerEmails[receiverID]);
            foreach (string emailID in emailIDs)
            {
                emailData.RemoveEmail(emailID, receiverID);
            }
            dataManager.CreateEmailEvent("email_delete_all", receiverID, "", "删除所有邮件");
            dataManager.SaveEmailData();
            Debug.Log("删除所有邮件成功");
        }
    }

    public void DeleteReadEmails(string receiverID)
    {
        if (emailData.PlayerEmails.ContainsKey(receiverID))
        {
            List<string> emailIDs = new List<string>(emailData.PlayerEmails[receiverID]);
            foreach (string emailID in emailIDs)
            {
                Email email = GetEmail(emailID);
                if (email != null && email.IsRead)
                {
                    emailData.RemoveEmail(emailID, receiverID);
                }
            }
            dataManager.CreateEmailEvent("email_delete_read", receiverID, "", "删除已读邮件");
            dataManager.SaveEmailData();
            Debug.Log("删除已读邮件成功");
        }
    }

    public Email GetEmail(string emailID)
    {
        return emailData.Emails.Find(e => e.EmailID == emailID);
    }

    public List<Email> GetPlayerEmails(string playerID, int count = 50)
    {
        if (emailData.PlayerEmails.ContainsKey(playerID))
        {
            List<Email> emails = new List<Email>();
            foreach (string emailID in emailData.PlayerEmails[playerID])
            {
                Email email = GetEmail(emailID);
                if (email != null && email.ExpireTime > DateTime.Now)
                {
                    emails.Add(email);
                }
            }
            emails.Sort((a, b) => b.SendTime.CompareTo(a.SendTime));
            if (count < emails.Count)
            {
                return emails.GetRange(0, count);
            }
            return emails;
        }
        return new List<Email>();
    }

    public List<Email> GetUnreadEmails(string playerID)
    {
        if (emailData.PlayerEmails.ContainsKey(playerID))
        {
            List<Email> unreadEmails = new List<Email>();
            foreach (string emailID in emailData.PlayerEmails[playerID])
            {
                Email email = GetEmail(emailID);
                if (email != null && !email.IsRead && email.ExpireTime > DateTime.Now)
                {
                    unreadEmails.Add(email);
                }
            }
            unreadEmails.Sort((a, b) => b.SendTime.CompareTo(a.SendTime));
            return unreadEmails;
        }
        return new List<Email>();
    }

    public List<Email> GetEmailsWithAttachments(string playerID)
    {
        if (emailData.PlayerEmails.ContainsKey(playerID))
        {
            List<Email> emailsWithAttachments = new List<Email>();
            foreach (string emailID in emailData.PlayerEmails[playerID])
            {
                Email email = GetEmail(emailID);
                if (email != null && email.Attachments.Count > 0 && !email.IsClaimed && email.ExpireTime > DateTime.Now)
                {
                    emailsWithAttachments.Add(email);
                }
            }
            emailsWithAttachments.Sort((a, b) => b.SendTime.CompareTo(a.SendTime));
            return emailsWithAttachments;
        }
        return new List<Email>();
    }

    public int GetUnreadEmailCount(string playerID)
    {
        return GetUnreadEmails(playerID).Count;
    }

    public int GetEmailCount(string playerID)
    {
        if (emailData.PlayerEmails.ContainsKey(playerID))
        {
            int count = 0;
            foreach (string emailID in emailData.PlayerEmails[playerID])
            {
                Email email = GetEmail(emailID);
                if (email != null && email.ExpireTime > DateTime.Now)
                {
                    count++;
                }
            }
            return count;
        }
        return 0;
    }

    public void CleanupExpiredEmails()
    {
        DateTime now = DateTime.Now;
        List<Email> expiredEmails = new List<Email>();
        foreach (Email email in emailData.Emails)
        {
            if (email.ExpireTime < now)
            {
                expiredEmails.Add(email);
            }
        }
        
        foreach (Email email in expiredEmails)
        {
            emailData.RemoveEmail(email.EmailID, email.ReceiverID);
        }
        
        if (expiredEmails.Count > 0)
        {
            dataManager.CreateEmailEvent("email_cleanup", "system", "", "清理过期邮件: " + expiredEmails.Count);
            dataManager.SaveEmailData();
            Debug.Log("清理过期邮件成功: " + expiredEmails.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveEmailData();
    }

    public void LoadData()
    {
        dataManager.LoadEmailData();
    }

    public List<EmailEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
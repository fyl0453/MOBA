using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Email
{
    public string EmailID;
    public string Sender;
    public string ReceiverID;
    public string Subject;
    public string Content;
    public int EmailType;
    public List<EmailAttachment> Attachments;
    public bool IsRead;
    public bool IsClaimed;
    public DateTime SendTime;
    public DateTime ExpireTime;
    public bool IsSystemEmail;

    public Email(string emailID, string sender, string receiverID, string subject, string content, int emailType, bool isSystemEmail = true)
    {
        EmailID = emailID;
        Sender = sender;
        ReceiverID = receiverID;
        Subject = subject;
        Content = content;
        EmailType = emailType;
        Attachments = new List<EmailAttachment>();
        IsRead = false;
        IsClaimed = false;
        SendTime = DateTime.Now;
        ExpireTime = DateTime.Now.AddDays(7);
        IsSystemEmail = isSystemEmail;
    }
}

[Serializable]
public class EmailAttachment
{
    public string AttachmentID;
    public string ItemID;
    public string ItemName;
    public string ItemType;
    public int Quantity;
    public bool IsClaimed;

    public EmailAttachment(string attachmentID, string itemID, string itemName, string itemType, int quantity)
    {
        AttachmentID = attachmentID;
        ItemID = itemID;
        ItemName = itemName;
        ItemType = itemType;
        Quantity = quantity;
        IsClaimed = false;
    }
}

[Serializable]
public class EmailSystemData
{
    public List<Email> Emails;
    public Dictionary<string, List<string>> PlayerEmails;
    public int TotalEmails;
    public DateTime LastCleanupTime;

    public EmailSystemData()
    {
        Emails = new List<Email>();
        PlayerEmails = new Dictionary<string, List<string>>();
        TotalEmails = 0;
        LastCleanupTime = DateTime.Now;
    }

    public void AddEmail(Email email)
    {
        Emails.Add(email);
        if (!PlayerEmails.ContainsKey(email.ReceiverID))
        {
            PlayerEmails[email.ReceiverID] = new List<string>();
        }
        PlayerEmails[email.ReceiverID].Add(email.EmailID);
        TotalEmails++;
    }

    public void RemoveEmail(string emailID, string receiverID)
    {
        Email email = Emails.Find(e => e.EmailID == emailID);
        if (email != null)
        {
            Emails.Remove(email);
            if (PlayerEmails.ContainsKey(receiverID))
            {
                PlayerEmails[receiverID].Remove(emailID);
            }
            TotalEmails--;
        }
    }
}

[Serializable]
public class EmailEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string EmailID;
    public string EventData;

    public EmailEvent(string eventID, string eventType, string playerID, string emailID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        EmailID = emailID;
        EventData = eventData;
    }
}

public class EmailSystemDataManager
{
    private static EmailSystemDataManager _instance;
    public static EmailSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EmailSystemDataManager();
            }
            return _instance;
        }
    }

    public EmailSystemData emailData;
    private List<EmailEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private EmailSystemDataManager()
    {
        emailData = new EmailSystemData();
        recentEvents = new List<EmailEvent>();
        LoadEmailData();
    }

    public void SaveEmailData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EmailSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, emailData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存邮件系统数据失败: " + e.Message);
        }
    }

    public void LoadEmailData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "EmailSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    emailData = (EmailSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载邮件系统数据失败: " + e.Message);
            emailData = new EmailSystemData();
        }
    }

    public void CreateEmailEvent(string eventType, string playerID, string emailID, string eventData)
    {
        string eventID = "email_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EmailEvent emailEvent = new EmailEvent(eventID, eventType, playerID, emailID, eventData);
        recentEvents.Add(emailEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<EmailEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
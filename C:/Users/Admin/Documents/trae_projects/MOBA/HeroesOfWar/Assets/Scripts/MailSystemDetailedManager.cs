using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MailSystemDetailedManager : MonoBehaviour
{
    public static MailSystemDetailedManager Instance { get; private set; }
    
    public MailSystemDetailedManagerData mailData;
    
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
        LoadMailData();
        
        if (mailData == null)
        {
            mailData = new MailSystemDetailedManagerData();
            InitializeDefaultMailSystem();
        }
    }
    
    private void InitializeDefaultMailSystem()
    {
        // 邮件
        Mail mail1 = new Mail("mail_001", "system", "系统", "user_001", "张三", "欢迎加入游戏", "欢迎来到王者荣耀，这是您的新手奖励", "system", true);
        Mail mail2 = new Mail("mail_002", "system", "系统", "user_001", "张三", "活动奖励", "恭喜您完成新手任务，获得以下奖励", "activity", true);
        Mail mail3 = new Mail("mail_003", "user_002", "李四", "user_001", "张三", "好友消息", "一起玩游戏吧", "friend", false);
        Mail mail4 = new Mail("mail_004", "system", "系统", "user_002", "李四", "欢迎加入游戏", "欢迎来到王者荣耀，这是您的新手奖励", "system", true);
        
        mailData.system.AddMail(mail1);
        mailData.system.AddMail(mail2);
        mailData.system.AddMail(mail3);
        mailData.system.AddMail(mail4);
        
        // 邮件附件
        MailAttachment attachment1 = new MailAttachment("attachment_001", "mail_001", "currency", "gold", 1000, "icon_gold");
        MailAttachment attachment2 = new MailAttachment("attachment_002", "mail_001", "currency", "diamond", 100, "icon_diamond");
        MailAttachment attachment3 = new MailAttachment("attachment_003", "mail_002", "fragment", "hero_fragment", 5, "icon_hero_fragment");
        MailAttachment attachment4 = new MailAttachment("attachment_004", "mail_004", "currency", "gold", 1000, "icon_gold");
        
        mailData.system.AddMailAttachment(attachment1);
        mailData.system.AddMailAttachment(attachment2);
        mailData.system.AddMailAttachment(attachment3);
        mailData.system.AddMailAttachment(attachment4);
        
        // 添加附件到邮件
        mail1.AddAttachment("attachment_001");
        mail1.AddAttachment("attachment_002");
        mail2.AddAttachment("attachment_003");
        mail4.AddAttachment("attachment_004");
        
        // 邮件事件
        MailEvent event1 = new MailEvent("event_001", "send", "system", "mail_001", "发送系统邮件");
        MailEvent event2 = new MailEvent("event_002", "receive", "user_001", "mail_001", "接收系统邮件");
        MailEvent event3 = new MailEvent("event_003", "read", "user_001", "mail_001", "阅读邮件");
        
        mailData.system.AddMailEvent(event1);
        mailData.system.AddMailEvent(event2);
        mailData.system.AddMailEvent(event3);
        
        SaveMailData();
    }
    
    // 邮件发送
    public void SendMail(string senderID, string senderName, string receiverID, string receiverName, string subject, string content, string mailType, bool hasAttachment)
    {
        string mailID = "mail_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Mail mail = new Mail(mailID, senderID, senderName, receiverID, receiverName, subject, content, mailType, hasAttachment);
        mailData.system.AddMail(mail);
        
        // 创建邮件事件
        CreateMailEvent("send", senderID, mailID, "发送邮件");
        CreateMailEvent("receive", receiverID, mailID, "接收邮件");
        
        SaveMailData();
        Debug.Log("成功发送邮件: " + subject);
    }
    
    // 添加邮件附件
    public void AddMailAttachment(string mailID, string attachmentType, string attachmentValue, int quantity, string icon)
    {
        Mail mail = mailData.system.GetMail(mailID);
        if (mail != null)
        {
            string attachmentID = "attachment_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            MailAttachment attachment = new MailAttachment(attachmentID, mailID, attachmentType, attachmentValue, quantity, icon);
            mailData.system.AddMailAttachment(attachment);
            mail.AddAttachment(attachmentID);
            mail.hasAttachment = true;
            
            SaveMailData();
            Debug.Log("成功添加邮件附件");
        }
        else
        {
            Debug.LogError("邮件不存在: " + mailID);
        }
    }
    
    // 邮件读取
    public void ReadMail(string mailID, string userID)
    {
        Mail mail = mailData.system.GetMail(mailID);
        if (mail != null && mail.receiverID == userID)
        {
            if (mail.status == "unread")
            {
                mail.Read();
                CreateMailEvent("read", userID, mailID, "阅读邮件");
                SaveMailData();
                Debug.Log("成功读取邮件: " + mail.subject);
            }
            else
            {
                Debug.LogError("邮件已读取");
            }
        }
        else
        {
            Debug.LogError("邮件不存在或无权读取");
        }
    }
    
    // 领取邮件附件
    public void ClaimMailAttachment(string mailID, string userID)
    {
        Mail mail = mailData.system.GetMail(mailID);
        if (mail != null && mail.receiverID == userID)
        {
            if (mail.hasAttachment && (mail.status == "unread" || mail.status == "read"))
            {
                foreach (string attachmentID in mail.attachmentIDs)
                {
                    MailAttachment attachment = mailData.system.GetMailAttachment(attachmentID);
                    if (attachment != null && !attachment.isClaimed)
                    {
                        attachment.Claim();
                        // 这里可以添加发放附件的逻辑
                        Debug.Log("发放附件: " + attachment.attachmentType + " x" + attachment.quantity);
                    }
                }
                
                mail.ClaimAttachment();
                CreateMailEvent("claim", userID, mailID, "领取邮件附件");
                SaveMailData();
                Debug.Log("成功领取邮件附件");
            }
            else
            {
                Debug.LogError("邮件无附件或已领取");
            }
        }
        else
        {
            Debug.LogError("邮件不存在或无权领取");
        }
    }
    
    // 删除邮件
    public void DeleteMail(string mailID, string userID)
    {
        Mail mail = mailData.system.GetMail(mailID);
        if (mail != null && mail.receiverID == userID)
        {
            mail.Delete();
            CreateMailEvent("delete", userID, mailID, "删除邮件");
            SaveMailData();
            Debug.Log("成功删除邮件: " + mail.subject);
        }
        else
        {
            Debug.LogError("邮件不存在或无权删除");
        }
    }
    
    // 批量删除邮件
    public void DeleteMails(List<string> mailIDs, string userID)
    {
        foreach (string mailID in mailIDs)
        {
            DeleteMail(mailID, userID);
        }
    }
    
    // 领取所有邮件附件
    public void ClaimAllMailAttachments(string userID)
    {
        List<Mail> mails = mailData.system.GetMailsByUser(userID);
        foreach (Mail mail in mails)
        {
            if (mail.hasAttachment && (mail.status == "unread" || mail.status == "read"))
            {
                ClaimMailAttachment(mail.mailID, userID);
            }
        }
    }
    
    // 获取用户邮件
    public List<Mail> GetUserMails(string userID, string status = null, string mailType = null)
    {
        List<Mail> mails = mailData.system.GetMailsByUser(userID);
        
        if (!string.IsNullOrEmpty(status))
        {
            mails = mails.FindAll(m => m.status == status);
        }
        
        if (!string.IsNullOrEmpty(mailType))
        {
            mails = mails.FindAll(m => m.mailType == mailType);
        }
        
        // 过滤过期邮件
        mails = mails.FindAll(m => !m.IsExpired());
        
        // 按发送时间排序
        mails.Sort((a, b) => b.sendTime.CompareTo(a.sendTime));
        
        return mails;
    }
    
    // 获取邮件附件
    public List<MailAttachment> GetMailAttachments(string mailID)
    {
        Mail mail = mailData.system.GetMail(mailID);
        if (mail != null)
        {
            List<MailAttachment> attachments = new List<MailAttachment>();
            foreach (string attachmentID in mail.attachmentIDs)
            {
                MailAttachment attachment = mailData.system.GetMailAttachment(attachmentID);
                if (attachment != null)
                {
                    attachments.Add(attachment);
                }
            }
            return attachments;
        }
        else
        {
            return new List<MailAttachment>();
        }
    }
    
    // 清理过期邮件
    public void CleanExpiredMails()
    {
        List<Mail> expiredMails = mailData.system.mails.FindAll(m => m.IsExpired());
        foreach (Mail mail in expiredMails)
        {
            mail.Delete();
        }
        SaveMailData();
        Debug.Log("成功清理过期邮件: " + expiredMails.Count + " 封");
    }
    
    // 邮件事件管理
    public string CreateMailEvent(string eventType, string userID, string mailID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MailEvent mailEvent = new MailEvent(eventID, eventType, userID, mailID, description);
        mailData.system.AddMailEvent(mailEvent);
        SaveMailData();
        Debug.Log("成功创建邮件事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        MailEvent mailEvent = mailData.system.GetMailEvent(eventID);
        if (mailEvent != null)
        {
            mailEvent.MarkAsCompleted();
            SaveMailData();
            Debug.Log("成功标记邮件事件为完成");
        }
        else
        {
            Debug.LogError("邮件事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        MailEvent mailEvent = mailData.system.GetMailEvent(eventID);
        if (mailEvent != null)
        {
            mailEvent.MarkAsFailed();
            SaveMailData();
            Debug.Log("成功标记邮件事件为失败");
        }
        else
        {
            Debug.LogError("邮件事件不存在: " + eventID);
        }
    }
    
    public List<MailEvent> GetMailEvents(string userID)
    {
        return mailData.system.GetMailEventsByUser(userID);
    }
    
    // 数据持久化
    public void SaveMailData()
    {
        string path = Application.dataPath + "/Data/mail_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, mailData);
        stream.Close();
    }
    
    public void LoadMailData()
    {
        string path = Application.dataPath + "/Data/mail_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            mailData = (MailSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            mailData = new MailSystemDetailedManagerData();
        }
    }
}
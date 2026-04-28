using System;

[System.Serializable]
public class Mail
{
    public string mailID;
    public string sender;
    public string subject;
    public string content;
    public MailType mailType;
    public DateTime sendTime;
    public bool isRead;
    public bool hasAttachment;
    public MailAttachment attachment;
    
    public enum MailType
    {
        System,
        Activity,
        Friend,
        Reward,
        Notice
    }
    
    public Mail(string id, string senderName, string mailSubject, string mailContent, MailType type, bool attachment = false, MailAttachment mailAttachment = null)
    {
        mailID = id;
        sender = senderName;
        subject = mailSubject;
        content = mailContent;
        mailType = type;
        sendTime = DateTime.Now;
        isRead = false;
        hasAttachment = attachment;
        this.attachment = mailAttachment;
    }
    
    public void MarkAsRead()
    {
        isRead = true;
    }
    
    public void ClaimAttachment()
    {
        if (hasAttachment && attachment != null)
        {
            // 领取附件的逻辑
            hasAttachment = false;
            attachment = null;
        }
    }
}
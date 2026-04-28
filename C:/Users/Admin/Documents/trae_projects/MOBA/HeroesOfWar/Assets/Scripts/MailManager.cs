using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MailManager : MonoBehaviour
{
    public static MailManager Instance { get; private set; }
    
    public MailBox mailBox;
    
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
        
        if (mailBox == null)
        {
            mailBox = new MailBox(ProfileManager.Instance.currentProfile.playerID);
            AddWelcomeMail();
        }
    }
    
    private void AddWelcomeMail()
    {
        Mail welcomeMail = new Mail("mail_welcome", "系统", "欢迎来到王者荣耀", "欢迎加入王者荣耀！作为新玩家，我们为您准备了一些新手礼包，希望您在游戏中玩得愉快！");
        welcomeMail.AddAttachment("item_health_potion", 10);
        welcomeMail.AddAttachment("item_mana_potion", 10);
        welcomeMail.AddAttachment("gold", 1000, "Currency");
        mailBox.AddMail(welcomeMail);
        SaveMailData();
    }
    
    public void SendMail(string receiverID, string sender, string subject, string content, bool hasAttachment = false)
    {
        Mail mail = new Mail("mail_" + System.DateTime.Now.Ticks, sender, subject, content, hasAttachment);
        mailBox.AddMail(mail);
        SaveMailData();
    }
    
    public void SendMailWithAttachment(string receiverID, string sender, string subject, string content, string itemID, int quantity, string attachmentType = "Item")
    {
        Mail mail = new Mail("mail_" + System.DateTime.Now.Ticks, sender, subject, content, true);
        mail.AddAttachment(itemID, quantity, attachmentType);
        mailBox.AddMail(mail);
        SaveMailData();
    }
    
    public void MarkMailAsRead(string mailID)
    {
        Mail mail = mailBox.GetMailByID(mailID);
        if (mail != null)
        {
            mail.MarkAsRead();
            SaveMailData();
        }
    }
    
    public void ClaimMailAttachment(string mailID)
    {
        Mail mail = mailBox.GetMailByID(mailID);
        if (mail != null && mail.hasAttachment)
        {
            foreach (MailAttachment attachment in mail.attachments)
            {
                if (!attachment.isClaimed)
                {
                    GrantAttachment(attachment);
                    attachment.ClaimAttachment();
                }
            }
            SaveMailData();
        }
    }
    
    private void GrantAttachment(MailAttachment attachment)
    {
        switch (attachment.attachmentType)
        {
            case "Item":
                InventoryManager.Instance.AddItemToInventory(attachment.itemID, attachment.quantity);
                break;
            case "Currency":
                if (attachment.itemID == "gold")
                {
                    ProfileManager.Instance.currentProfile.gold += attachment.quantity;
                    ProfileManager.Instance.SaveProfile();
                }
                else if (attachment.itemID == "gems")
                {
                    ProfileManager.Instance.currentProfile.gems += attachment.quantity;
                    ProfileManager.Instance.SaveProfile();
                }
                break;
        }
    }
    
    public void DeleteMail(string mailID)
    {
        mailBox.RemoveMail(mailID);
        SaveMailData();
    }
    
    public void DeleteAllReadMails()
    {
        mailBox.mails.RemoveAll(m => m.isRead);
        SaveMailData();
    }
    
    public void DeleteExpiredMails()
    {
        mailBox.RemoveExpiredMails();
        SaveMailData();
    }
    
    public List<Mail> GetAllMails()
    {
        return mailBox.mails;
    }
    
    public List<Mail> GetUnreadMails()
    {
        return mailBox.GetUnreadMails();
    }
    
    public List<Mail> GetMailsWithAttachments()
    {
        return mailBox.GetMailsWithAttachments();
    }
    
    public int GetUnreadMailCount()
    {
        return mailBox.GetUnreadMails().Count;
    }
    
    public void SaveMailData()
    {
        string path = Application.dataPath + "/Data/mail_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, mailBox);
        stream.Close();
    }
    
    public void LoadMailData()
    {
        string path = Application.dataPath + "/Data/mail_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            mailBox = (MailBox)formatter.Deserialize(stream);
            stream.Close();
        }
    }
}
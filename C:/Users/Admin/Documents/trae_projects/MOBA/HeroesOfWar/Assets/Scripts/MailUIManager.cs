using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MailUIManager : MonoBehaviour
{
    public static MailUIManager Instance { get; private set; }
    
    public Canvas mailCanvas;
    public ScrollRect mailsScrollRect;
    public Transform mailsContent;
    public Text mailSenderText;
    public Text mailSubjectText;
    public Text mailContentText;
    public Text mailTimeText;
    public Text mailAttachmentText;
    public Button claimAttachmentButton;
    public GameObject mailPrefab;
    
    private Mail selectedMail;
    
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
        mailCanvas.gameObject.SetActive(false);
        claimAttachmentButton.onClick.AddListener(ClaimAttachment);
    }
    
    public void OpenMailUI()
    {
        mailCanvas.gameObject.SetActive(true);
        UpdateMailsList();
        ClearMailDetails();
    }
    
    public void CloseMailUI()
    {
        mailCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateMailsList()
    {
        // 清空现有内容
        foreach (Transform child in mailsContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示邮件列表
        List<Mail> mails = MailManager.Instance.GetAllMails();
        foreach (Mail mail in mails)
        {
            GameObject mailObj = Instantiate(mailPrefab, mailsContent);
            Text[] texts = mailObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = mail.subject;
                texts[1].text = mail.sender;
                texts[2].text = mail.sendTime.ToString("yyyy-MM-dd HH:mm");
            }
            
            // 设置邮件状态
            Image[] images = mailObj.GetComponentsInChildren<Image>();
            if (images.Length >= 1)
            {
                // 未读邮件标记
                if (!mail.isRead)
                {
                    images[0].color = Color.red;
                }
                else
                {
                    images[0].color = Color.gray;
                }
            }
            
            // 添加点击事件
            Button button = mailObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectMail(mail));
            }
        }
    }
    
    public void SelectMail(Mail mail)
    {
        selectedMail = mail;
        mailSenderText.text = "发件人: " + mail.sender;
        mailSubjectText.text = "主题: " + mail.subject;
        mailContentText.text = mail.content;
        mailTimeText.text = "时间: " + mail.sendTime.ToString("yyyy-MM-dd HH:mm");
        
        // 显示附件
        if (mail.hasAttachment && mail.attachment != null)
        {
            string attachmentText = "附件: ";
            if (mail.attachment.gold > 0) attachmentText += $"{mail.attachment.gold} 金币, ";
            if (mail.attachment.experience > 0) attachmentText += $"{mail.attachment.experience} 经验, ";
            if (mail.attachment.gems > 0) attachmentText += $"{mail.attachment.gems} 钻石, ";
            if (!string.IsNullOrEmpty(mail.attachment.itemID)) attachmentText += $"{mail.attachment.itemCount}x {mail.attachment.itemID}, ";
            if (!string.IsNullOrEmpty(mail.attachment.skinID)) attachmentText += $"{mail.attachment.skinID}, ";
            if (!string.IsNullOrEmpty(mail.attachment.heroID)) attachmentText += $"{mail.attachment.heroID}, ";
            
            // 移除末尾的逗号和空格
            if (attachmentText.EndsWith(", "))
            {
                attachmentText = attachmentText.Substring(0, attachmentText.Length - 2);
            }
            
            mailAttachmentText.text = attachmentText;
            claimAttachmentButton.gameObject.SetActive(true);
        }
        else
        {
            mailAttachmentText.text = "无附件";
            claimAttachmentButton.gameObject.SetActive(false);
        }
        
        // 标记为已读
        if (!mail.isRead)
        {
            MailManager.Instance.MarkMailAsRead(mail.mailID);
            UpdateMailsList();
        }
    }
    
    public void ClearMailDetails()
    {
        selectedMail = null;
        mailSenderText.text = "";
        mailSubjectText.text = "";
        mailContentText.text = "";
        mailTimeText.text = "";
        mailAttachmentText.text = "";
        claimAttachmentButton.gameObject.SetActive(false);
    }
    
    public void ClaimAttachment()
    {
        if (selectedMail != null && selectedMail.hasAttachment)
        {
            MailManager.Instance.ClaimMailAttachment(selectedMail.mailID);
            UpdateMailsList();
            SelectMail(selectedMail);
        }
    }
    
    public void DeleteMail()
    {
        if (selectedMail != null)
        {
            MailManager.Instance.RemoveMail(selectedMail.mailID);
            UpdateMailsList();
            ClearMailDetails();
        }
    }
}
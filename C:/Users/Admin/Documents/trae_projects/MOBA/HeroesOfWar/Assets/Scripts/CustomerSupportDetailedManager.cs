using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CustomerSupportDetailedManager : MonoBehaviour
{
    public static CustomerSupportDetailedManager Instance { get; private set; }
    
    public CustomerSupportDetailedManagerData customerSupportData;
    
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
        LoadCustomerSupportData();
        
        if (customerSupportData == null)
        {
            customerSupportData = new CustomerSupportDetailedManagerData();
            InitializeDefaultCustomerSupport();
        }
    }
    
    private void InitializeDefaultCustomerSupport()
    {
        // 客服人员
        SupportAgent agent1 = new SupportAgent("agent_001", "客服1号", "agent1@example.com", "Senior");
        SupportAgent agent2 = new SupportAgent("agent_002", "客服2号", "agent2@example.com", "Junior");
        customerSupportData.system.AddSupportAgent(agent1);
        customerSupportData.system.AddSupportAgent(agent2);
        
        // FAQ
        FAQ faq1 = new FAQ("faq_001", "如何找回账号密码？", "您可以通过绑定的邮箱或手机号进行密码重置，也可以联系客服进行账号找回。", "账号问题");
        FAQ faq2 = new FAQ("faq_002", "游戏卡顿怎么办？", "建议您清理手机缓存，关闭后台应用，或降低游戏画质设置。", "技术问题");
        FAQ faq3 = new FAQ("faq_003", "如何举报不良行为？", "在游戏内点击玩家头像，选择举报选项，填写举报原因即可。", "游戏规则");
        FAQ faq4 = new FAQ("faq_004", "充值未到账怎么办？", "请提供充值凭证联系客服，我们会尽快为您处理。", "充值问题");
        customerSupportData.system.AddFAQ(faq1);
        customerSupportData.system.AddFAQ(faq2);
        customerSupportData.system.AddFAQ(faq3);
        customerSupportData.system.AddFAQ(faq4);
        
        // 支持工单
        SupportTicket ticket1 = new SupportTicket("ticket_001", "user_001", "user1@example.com", "账号无法登录", "我的账号无法登录，提示密码错误，但我确定密码是正确的。", "账号问题", "High");
        ticket1.AssignAgent("agent_001");
        TicketMessage message1 = new TicketMessage("msg_001", "ticket_001", "agent_001", "agent", "您好，请问您是否尝试过密码重置？");
        ticket1.AddMessage(message1);
        TicketMessage message2 = new TicketMessage("msg_002", "ticket_001", "user_001", "user", "是的，我已经尝试过重置密码，但仍然无法登录。");
        ticket1.AddMessage(message2);
        customerSupportData.system.AddSupportTicket(ticket1);
        
        // 聊天会话
        ChatSession session1 = new ChatSession("session_001", "user_002", "agent_002");
        ChatMessage chatMsg1 = new ChatMessage("chat_msg_001", "session_001", "user_002", "user", "您好，我想咨询一下关于皮肤购买的问题。");
        session1.AddMessage(chatMsg1);
        ChatMessage chatMsg2 = new ChatMessage("chat_msg_002", "session_001", "agent_002", "agent", "您好，请问有什么可以帮助您的？");
        session1.AddMessage(chatMsg2);
        customerSupportData.system.AddChatSession(session1);
        
        // 反馈
        Feedback feedback1 = new Feedback("feedback_001", "user_003", "Bug", "游戏在使用技能时会崩溃。", "High");
        feedback1.Process();
        customerSupportData.system.AddFeedback(feedback1);
        
        // 账号找回
        AccountRecovery recovery1 = new AccountRecovery("recovery_001", "user_004", "Player4", "user4@example.com", "13800138000", "email");
        customerSupportData.system.AddAccountRecovery(recovery1);
        
        SaveCustomerSupportData();
    }
    
    // 支持工单管理
    public void CreateSupportTicket(string userID, string userEmail, string subject, string description, string category, string priority)
    {
        string ticketID = "ticket_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SupportTicket ticket = new SupportTicket(ticketID, userID, userEmail, subject, description, category, priority);
        customerSupportData.system.AddSupportTicket(ticket);
        SaveCustomerSupportData();
        Debug.Log("成功创建支持工单: " + subject);
    }
    
    public void AddTicketMessage(string ticketID, string senderID, string senderType, string content)
    {
        SupportTicket ticket = customerSupportData.system.GetSupportTicket(ticketID);
        if (ticket != null)
        {
            string messageID = "msg_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            TicketMessage message = new TicketMessage(messageID, ticketID, senderID, senderType, content);
            ticket.AddMessage(message);
            SaveCustomerSupportData();
            Debug.Log("成功添加工单消息: " + messageID);
        }
        else
        {
            Debug.LogError("工单不存在: " + ticketID);
        }
    }
    
    public void AssignTicketToAgent(string ticketID, string agentID)
    {
        SupportTicket ticket = customerSupportData.system.GetSupportTicket(ticketID);
        if (ticket != null)
        {
            ticket.AssignAgent(agentID);
            SaveCustomerSupportData();
            Debug.Log("成功分配工单给客服: " + agentID);
        }
        else
        {
            Debug.LogError("工单不存在: " + ticketID);
        }
    }
    
    public void ResolveTicket(string ticketID, string resolution)
    {
        SupportTicket ticket = customerSupportData.system.GetSupportTicket(ticketID);
        if (ticket != null)
        {
            ticket.Resolve(resolution);
            
            // 更新客服的已解决工单数
            if (!string.IsNullOrEmpty(ticket.assignedAgentID))
            {
                SupportAgent agent = customerSupportData.system.GetSupportAgent(ticket.assignedAgentID);
                if (agent != null)
                {
                    agent.IncrementResolvedTickets();
                }
            }
            
            SaveCustomerSupportData();
            Debug.Log("成功解决工单: " + ticketID);
        }
        else
        {
            Debug.LogError("工单不存在: " + ticketID);
        }
    }
    
    public void CloseTicket(string ticketID)
    {
        SupportTicket ticket = customerSupportData.system.GetSupportTicket(ticketID);
        if (ticket != null)
        {
            ticket.Close();
            SaveCustomerSupportData();
            Debug.Log("成功关闭工单: " + ticketID);
        }
        else
        {
            Debug.LogError("工单不存在: " + ticketID);
        }
    }
    
    public List<SupportTicket> GetTicketsByUser(string userID)
    {
        return customerSupportData.system.GetTicketsByUser(userID);
    }
    
    public List<SupportTicket> GetTicketsByAgent(string agentID)
    {
        return customerSupportData.system.GetTicketsByAgent(agentID);
    }
    
    // 聊天会话管理
    public string CreateChatSession(string userID, string agentID)
    {
        string sessionID = "session_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ChatSession session = new ChatSession(sessionID, userID, agentID);
        customerSupportData.system.AddChatSession(session);
        
        // 更新客服的活跃聊天数
        SupportAgent agent = customerSupportData.system.GetSupportAgent(agentID);
        if (agent != null)
        {
            agent.IncrementActiveChats();
        }
        
        SaveCustomerSupportData();
        Debug.Log("成功创建聊天会话: " + sessionID);
        return sessionID;
    }
    
    public void AddChatMessage(string sessionID, string senderID, string senderType, string content)
    {
        ChatSession session = customerSupportData.system.GetChatSession(sessionID);
        if (session != null)
        {
            string messageID = "chat_msg_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            ChatMessage message = new ChatMessage(messageID, sessionID, senderID, senderType, content);
            session.AddMessage(message);
            SaveCustomerSupportData();
            Debug.Log("成功添加聊天消息: " + messageID);
        }
        else
        {
            Debug.LogError("聊天会话不存在: " + sessionID);
        }
    }
    
    public void EndChatSession(string sessionID)
    {
        ChatSession session = customerSupportData.system.GetChatSession(sessionID);
        if (session != null)
        {
            session.EndSession();
            
            // 更新客服的活跃聊天数
            if (!string.IsNullOrEmpty(session.agentID))
            {
                SupportAgent agent = customerSupportData.system.GetSupportAgent(session.agentID);
                if (agent != null)
                {
                    agent.DecrementActiveChats();
                }
            }
            
            SaveCustomerSupportData();
            Debug.Log("成功结束聊天会话: " + sessionID);
        }
        else
        {
            Debug.LogError("聊天会话不存在: " + sessionID);
        }
    }
    
    public List<ChatSession> GetChatSessionsByUser(string userID)
    {
        return customerSupportData.system.GetChatSessionsByUser(userID);
    }
    
    public List<ChatSession> GetChatSessionsByAgent(string agentID)
    {
        return customerSupportData.system.GetChatSessionsByAgent(agentID);
    }
    
    // 反馈管理
    public void SubmitFeedback(string userID, string feedbackType, string content, string severity)
    {
        string feedbackID = "feedback_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Feedback feedback = new Feedback(feedbackID, userID, feedbackType, content, severity);
        customerSupportData.system.AddFeedback(feedback);
        SaveCustomerSupportData();
        Debug.Log("成功提交反馈: " + feedbackID);
    }
    
    public void ProcessFeedback(string feedbackID)
    {
        Feedback feedback = customerSupportData.system.GetFeedback(feedbackID);
        if (feedback != null)
        {
            feedback.Process();
            SaveCustomerSupportData();
            Debug.Log("成功处理反馈: " + feedbackID);
        }
        else
        {
            Debug.LogError("反馈不存在: " + feedbackID);
        }
    }
    
    public void ResolveFeedback(string feedbackID, string resolution)
    {
        Feedback feedback = customerSupportData.system.GetFeedback(feedbackID);
        if (feedback != null)
        {
            feedback.Resolve(resolution);
            SaveCustomerSupportData();
            Debug.Log("成功解决反馈: " + feedbackID);
        }
        else
        {
            Debug.LogError("反馈不存在: " + feedbackID);
        }
    }
    
    public List<Feedback> GetFeedbacksByUser(string userID)
    {
        return customerSupportData.system.GetFeedbacksByUser(userID);
    }
    
    // 账号找回管理
    public void SubmitAccountRecovery(string userID, string accountName, string email, string phoneNumber, string verificationMethod)
    {
        string recoveryID = "recovery_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AccountRecovery recovery = new AccountRecovery(recoveryID, userID, accountName, email, phoneNumber, verificationMethod);
        customerSupportData.system.AddAccountRecovery(recovery);
        SaveCustomerSupportData();
        Debug.Log("成功提交账号找回申请: " + recoveryID);
    }
    
    public void VerifyAccountRecovery(string recoveryID)
    {
        AccountRecovery recovery = customerSupportData.system.GetAccountRecovery(recoveryID);
        if (recovery != null)
        {
            recovery.Verify();
            SaveCustomerSupportData();
            Debug.Log("成功验证账号找回申请: " + recoveryID);
        }
        else
        {
            Debug.LogError("账号找回申请不存在: " + recoveryID);
        }
    }
    
    public void RecoverAccount(string recoveryID)
    {
        AccountRecovery recovery = customerSupportData.system.GetAccountRecovery(recoveryID);
        if (recovery != null)
        {
            recovery.Recover();
            SaveCustomerSupportData();
            Debug.Log("成功找回账号: " + recoveryID);
        }
        else
        {
            Debug.LogError("账号找回申请不存在: " + recoveryID);
        }
    }
    
    public List<AccountRecovery> GetAccountRecoveriesByUser(string userID)
    {
        return customerSupportData.system.GetAccountRecoveriesByUser(userID);
    }
    
    // FAQ管理
    public void AddFAQ(string question, string answer, string category)
    {
        string faqID = "faq_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        FAQ faq = new FAQ(faqID, question, answer, category);
        customerSupportData.system.AddFAQ(faq);
        SaveCustomerSupportData();
        Debug.Log("成功添加FAQ: " + question);
    }
    
    public void UpdateFAQ(string faqID, string question, string answer, string category)
    {
        FAQ faq = customerSupportData.system.GetFAQ(faqID);
        if (faq != null)
        {
            faq.question = question;
            faq.answer = answer;
            faq.category = category;
            SaveCustomerSupportData();
            Debug.Log("成功更新FAQ: " + question);
        }
        else
        {
            Debug.LogError("FAQ不存在: " + faqID);
        }
    }
    
    public void DeleteFAQ(string faqID)
    {
        FAQ faq = customerSupportData.system.GetFAQ(faqID);
        if (faq != null)
        {
            customerSupportData.system.faqs.Remove(faq);
            SaveCustomerSupportData();
            Debug.Log("成功删除FAQ: " + faqID);
        }
        else
        {
            Debug.LogError("FAQ不存在: " + faqID);
        }
    }
    
    public void IncrementFAQViewCount(string faqID)
    {
        FAQ faq = customerSupportData.system.GetFAQ(faqID);
        if (faq != null)
        {
            faq.IncrementViewCount();
            SaveCustomerSupportData();
            Debug.Log("成功增加FAQ浏览次数: " + faqID);
        }
        else
        {
            Debug.LogError("FAQ不存在: " + faqID);
        }
    }
    
    public void IncrementFAQHelpfulCount(string faqID)
    {
        FAQ faq = customerSupportData.system.GetFAQ(faqID);
        if (faq != null)
        {
            faq.IncrementHelpfulCount();
            SaveCustomerSupportData();
            Debug.Log("成功增加FAQ有用次数: " + faqID);
        }
        else
        {
            Debug.LogError("FAQ不存在: " + faqID);
        }
    }
    
    public List<FAQ> GetAllFAQs()
    {
        return customerSupportData.system.faqs;
    }
    
    // 客服人员管理
    public void AddSupportAgent(string name, string email, string role)
    {
        string agentID = "agent_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SupportAgent agent = new SupportAgent(agentID, name, email, role);
        customerSupportData.system.AddSupportAgent(agent);
        SaveCustomerSupportData();
        Debug.Log("成功添加客服人员: " + name);
    }
    
    public void SetAgentOnline(string agentID)
    {
        SupportAgent agent = customerSupportData.system.GetSupportAgent(agentID);
        if (agent != null)
        {
            agent.SetOnline();
            SaveCustomerSupportData();
            Debug.Log("成功设置客服在线: " + agent.agentName);
        }
        else
        {
            Debug.LogError("客服人员不存在: " + agentID);
        }
    }
    
    public void SetAgentOffline(string agentID)
    {
        SupportAgent agent = customerSupportData.system.GetSupportAgent(agentID);
        if (agent != null)
        {
            agent.SetOffline();
            SaveCustomerSupportData();
            Debug.Log("成功设置客服离线: " + agent.agentName);
        }
        else
        {
            Debug.LogError("客服人员不存在: " + agentID);
        }
    }
    
    public List<SupportAgent> GetAllSupportAgents()
    {
        return customerSupportData.system.supportAgents;
    }
    
    // 数据持久化
    public void SaveCustomerSupportData()
    {
        string path = Application.dataPath + "/Data/customer_support_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, customerSupportData);
        stream.Close();
    }
    
    public void LoadCustomerSupportData()
    {
        string path = Application.dataPath + "/Data/customer_support_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            customerSupportData = (CustomerSupportDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            customerSupportData = new CustomerSupportDetailedManagerData();
        }
    }
}
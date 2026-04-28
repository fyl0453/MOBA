using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CustomerServiceManager : MonoBehaviour
{
    public static CustomerServiceManager Instance { get; private set; }
    
    public CustomerServiceManagerData customerServiceData;
    
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
        LoadCustomerServiceData();
        
        if (customerServiceData == null)
        {
            customerServiceData = new CustomerServiceManagerData();
            InitializeDefaultCustomerService();
        }
    }
    
    private void InitializeDefaultCustomerService()
    {
        // 创建默认客服人员
        CustomerServiceAgent agent1 = new CustomerServiceAgent("agent_1", "客服1", "agent1@example.com", "12345678901");
        CustomerServiceAgent agent2 = new CustomerServiceAgent("agent_2", "客服2", "agent2@example.com", "12345678902");
        
        customerServiceData.system.AddAgent(agent1);
        customerServiceData.system.AddAgent(agent2);
        
        // 创建默认FAQ
        FAQ faq1 = new FAQ("faq_1", "如何找回密码？", "请点击登录界面的忘记密码按钮，按照提示操作即可找回密码。", "账号问题");
        FAQ faq2 = new FAQ("faq_2", "如何举报外挂？", "在游戏结束界面，点击举报按钮，选择举报类型为外挂，填写相关信息即可。", "游戏问题");
        FAQ faq3 = new FAQ("faq_3", "如何充值？", "在游戏内点击充值按钮，选择充值金额，按照提示完成支付即可。", "充值问题");
        
        customerServiceData.system.AddFAQ(faq1);
        customerServiceData.system.AddFAQ(faq2);
        customerServiceData.system.AddFAQ(faq3);
        
        SaveCustomerServiceData();
    }
    
    public string CreateTicket(string playerID, string playerName, string subject, string description, string category)
    {
        string ticketID = System.Guid.NewGuid().ToString();
        CustomerServiceTicket newTicket = new CustomerServiceTicket(ticketID, playerID, playerName, subject, description, category);
        customerServiceData.system.AddTicket(newTicket);
        
        // 自动分配客服
        AssignTicketToAgent(ticketID);
        
        SaveCustomerServiceData();
        Debug.Log($"成功创建客服工单: {subject}");
        return ticketID;
    }
    
    public void AddTicketMessage(string ticketID, string senderID, string senderName, string senderType, string content)
    {
        CustomerServiceTicket ticket = customerServiceData.system.GetTicket(ticketID);
        if (ticket != null)
        {
            string messageID = System.Guid.NewGuid().ToString();
            TicketMessage message = new TicketMessage(messageID, ticketID, senderID, senderName, senderType, content);
            ticket.AddMessage(message);
            SaveCustomerServiceData();
            Debug.Log("成功添加工单消息");
        }
    }
    
    public void AssignTicketToAgent(string ticketID)
    {
        CustomerServiceTicket ticket = customerServiceData.system.GetTicket(ticketID);
        if (ticket != null && string.IsNullOrEmpty(ticket.agentID))
        {
            // 找到在线的客服
            List<CustomerServiceAgent> onlineAgents = customerServiceData.system.agents.FindAll(a => a.isOnline);
            if (onlineAgents.Count > 0)
            {
                // 分配给处理工单最少的客服
                onlineAgents.Sort((a, b) => a.handledTickets.CompareTo(b.handledTickets));
                CustomerServiceAgent agent = onlineAgents[0];
                ticket.AssignToAgent(agent.agentID, agent.agentName);
                agent.IncrementHandledTickets();
                SaveCustomerServiceData();
                Debug.Log($"成功将工单分配给客服: {agent.agentName}");
            }
        }
    }
    
    public void CloseTicket(string ticketID)
    {
        CustomerServiceTicket ticket = customerServiceData.system.GetTicket(ticketID);
        if (ticket != null)
        {
            ticket.CloseTicket();
            SaveCustomerServiceData();
            Debug.Log($"成功关闭工单: {ticket.subject}");
        }
    }
    
    public void UpdateTicketStatus(string ticketID, string status)
    {
        CustomerServiceTicket ticket = customerServiceData.system.GetTicket(ticketID);
        if (ticket != null)
        {
            ticket.UpdateStatus(status);
            SaveCustomerServiceData();
            Debug.Log($"成功更新工单状态: {ticket.subject} -> {status}");
        }
    }
    
    public void UpdateTicketPriority(string ticketID, string priority)
    {
        CustomerServiceTicket ticket = customerServiceData.system.GetTicket(ticketID);
        if (ticket != null)
        {
            ticket.UpdatePriority(priority);
            SaveCustomerServiceData();
            Debug.Log($"成功更新工单优先级: {ticket.subject} -> {priority}");
        }
    }
    
    public List<CustomerServiceTicket> GetPlayerTickets(string playerID, int limit = 50)
    {
        List<CustomerServiceTicket> tickets = customerServiceData.system.GetTicketsByPlayer(playerID);
        tickets.Sort((a, b) => b.submitTime.CompareTo(a.submitTime));
        return tickets.GetRange(0, Mathf.Min(limit, tickets.Count));
    }
    
    public List<CustomerServiceTicket> GetAgentTickets(string agentID, int limit = 100)
    {
        List<CustomerServiceTicket> tickets = customerServiceData.system.GetTicketsByAgent(agentID);
        tickets.Sort((a, b) => b.submitTime.CompareTo(a.submitTime));
        return tickets.GetRange(0, Mathf.Min(limit, tickets.Count));
    }
    
    public List<CustomerServiceTicket> GetTicketsByStatus(string status, int limit = 100)
    {
        List<CustomerServiceTicket> tickets = customerServiceData.system.GetTicketsByStatus(status);
        tickets.Sort((a, b) => b.submitTime.CompareTo(a.submitTime));
        return tickets.GetRange(0, Mathf.Min(limit, tickets.Count));
    }
    
    public CustomerServiceTicket GetTicket(string ticketID)
    {
        return customerServiceData.system.GetTicket(ticketID);
    }
    
    public List<FAQ> GetFAQs(string category = "", int limit = 50)
    {
        List<FAQ> faqs = category == "" ? customerServiceData.system.faqs : customerServiceData.system.faqs.FindAll(f => f.category == category);
        faqs.Sort((a, b) => b.viewCount.CompareTo(a.viewCount));
        return faqs.GetRange(0, Mathf.Min(limit, faqs.Count));
    }
    
    public FAQ GetFAQ(string faqID)
    {
        FAQ faq = customerServiceData.system.GetFAQ(faqID);
        if (faq != null)
        {
            faq.IncrementViewCount();
            SaveCustomerServiceData();
        }
        return faq;
    }
    
    public void AddFAQ(string question, string answer, string category)
    {
        string faqID = System.Guid.NewGuid().ToString();
        FAQ newFAQ = new FAQ(faqID, question, answer, category);
        customerServiceData.system.AddFAQ(newFAQ);
        SaveCustomerServiceData();
        Debug.Log($"成功添加FAQ: {question}");
    }
    
    public void SetAgentOnline(string agentID, bool online)
    {
        CustomerServiceAgent agent = customerServiceData.system.GetAgent(agentID);
        if (agent != null)
        {
            agent.SetOnline(online);
            SaveCustomerServiceData();
            Debug.Log($"成功设置客服 {agent.agentName} 状态为: {(online ? "在线" : "离线")}");
        }
    }
    
    public void SaveCustomerServiceData()
    {
        string path = Application.dataPath + "/Data/customer_service_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, customerServiceData);
        stream.Close();
    }
    
    public void LoadCustomerServiceData()
    {
        string path = Application.dataPath + "/Data/customer_service_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            customerServiceData = (CustomerServiceManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            customerServiceData = new CustomerServiceManagerData();
        }
    }
}
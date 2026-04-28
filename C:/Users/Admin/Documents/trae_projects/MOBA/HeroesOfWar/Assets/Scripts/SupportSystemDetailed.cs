using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SupportTicket
{
    public string TicketID;
    public string PlayerID;
    public string PlayerName;
    public string Title;
    public string Content;
    public int TicketType;
    public int Status;
    public string Priority;
    public DateTime CreateTime;
    public DateTime UpdateTime;
    public List<SupportTicketMessage> Messages;
    public List<string> Attachments;
    public string AssignedTo;
    public string Resolution;

    public SupportTicket(string ticketID, string playerID, string playerName, string title, string content, int ticketType, string priority = "medium")
    {
        TicketID = ticketID;
        PlayerID = playerID;
        PlayerName = playerName;
        Title = title;
        Content = content;
        TicketType = ticketType;
        Status = 0;
        Priority = priority;
        CreateTime = DateTime.Now;
        UpdateTime = DateTime.Now;
        Messages = new List<SupportTicketMessage>();
        Attachments = new List<string>();
        AssignedTo = "";
        Resolution = "";
    }
}

[Serializable]
public class SupportTicketMessage
{
    public string MessageID;
    public string TicketID;
    public string SenderID;
    public string SenderName;
    public string Content;
    public DateTime SendTime;
    public bool IsSystem;
    public string MessageType;

    public SupportTicketMessage(string messageID, string ticketID, string senderID, string senderName, string content, bool isSystem = false, string messageType = "text")
    {
        MessageID = messageID;
        TicketID = ticketID;
        SenderID = senderID;
        SenderName = senderName;
        Content = content;
        SendTime = DateTime.Now;
        IsSystem = isSystem;
        MessageType = messageType;
    }
}

[Serializable]
public class FAQ
{
    public string FAQID;
    public string Question;
    public string Answer;
    public string Category;
    public int ViewCount;
    public int Priority;
    public bool IsActive;
    public DateTime CreateTime;
    public DateTime LastUpdateTime;

    public FAQ(string faqID, string question, string answer, string category, int priority = 1)
    {
        FAQID = faqID;
        Question = question;
        Answer = answer;
        Category = category;
        ViewCount = 0;
        Priority = priority;
        IsActive = true;
        CreateTime = DateTime.Now;
        LastUpdateTime = DateTime.Now;
    }
}

[Serializable]
public class SupportAgent
{
    public string AgentID;
    public string AgentName;
    public string AgentRole;
    public bool IsOnline;
    public int AssignedTickets;
    public DateTime LastLoginTime;
    public string Status;

    public SupportAgent(string agentID, string agentName, string agentRole)
    {
        AgentID = agentID;
        AgentName = agentName;
        AgentRole = agentRole;
        IsOnline = false;
        AssignedTickets = 0;
        LastLoginTime = DateTime.MinValue;
        Status = "available";
    }
}

[Serializable]
public class PlayerSupportData
{
    public string PlayerID;
    public List<SupportTicket> PlayerTickets;
    public int TotalTickets;
    public int OpenTickets;
    public DateTime LastTicketTime;

    public PlayerSupportData(string playerID)
    {
        PlayerID = playerID;
        PlayerTickets = new List<SupportTicket>();
        TotalTickets = 0;
        OpenTickets = 0;
        LastTicketTime = DateTime.MinValue;
    }
}

[Serializable]
public class SupportSystemData
{
    public List<SupportTicket> AllTickets;
    public List<FAQ> FAQs;
    public List<SupportAgent> SupportAgents;
    public Dictionary<string, PlayerSupportData> PlayerSupportData;
    public int TotalTickets;
    public int OpenTickets;
    public DateTime LastUpdateTime;

    public SupportSystemData()
    {
        AllTickets = new List<SupportTicket>();
        FAQs = new List<FAQ>();
        SupportAgents = new List<SupportAgent>();
        PlayerSupportData = new Dictionary<string, PlayerSupportData>();
        TotalTickets = 0;
        OpenTickets = 0;
        LastUpdateTime = DateTime.Now;
        InitializeDefaultFAQs();
        InitializeDefaultAgents();
    }

    private void InitializeDefaultFAQs()
    {
        FAQ faq1 = new FAQ("faq_001", "如何修改密码？", "在设置页面点击修改密码，按照提示操作即可。", "账号", 1);
        FAQs.Add(faq1);

        FAQ faq2 = new FAQ("faq_002", "如何获取英雄？", "可以通过商城购买、活动奖励或碎片兑换获得英雄。", "游戏玩法", 2);
        FAQs.Add(faq2);

        FAQ faq3 = new FAQ("faq_003", "充值未到账怎么办？", "请提供充值订单号和时间，联系客服处理。", "充值", 3);
        FAQs.Add(faq3);

        FAQ faq4 = new FAQ("faq_004", "游戏卡顿怎么办？", "建议清理缓存、降低画质或检查网络连接。", "技术问题", 4);
        FAQs.Add(faq4);

        FAQ faq5 = new FAQ("faq_005", "如何举报玩家？", "在游戏结束界面点击举报按钮，选择举报原因。", "社交", 5);
        FAQs.Add(faq5);
    }

    private void InitializeDefaultAgents()
    {
        SupportAgent agent1 = new SupportAgent("agent_001", "客服小A", "senior");
        SupportAgents.Add(agent1);

        SupportAgent agent2 = new SupportAgent("agent_002", "客服小B", "junior");
        SupportAgents.Add(agent2);

        SupportAgent agent3 = new SupportAgent("agent_003", "客服小C", "junior");
        SupportAgents.Add(agent3);
    }

    public void AddTicket(SupportTicket ticket)
    {
        AllTickets.Add(ticket);
        TotalTickets++;
        OpenTickets++;
    }

    public void AddFAQ(FAQ faq)
    {
        FAQs.Add(faq);
    }

    public void AddSupportAgent(SupportAgent agent)
    {
        SupportAgents.Add(agent);
    }

    public void AddPlayerSupportData(string playerID, PlayerSupportData data)
    {
        PlayerSupportData[playerID] = data;
    }
}

[Serializable]
public class SupportEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string TicketID;
    public string EventData;

    public SupportEvent(string eventID, string eventType, string playerID, string ticketID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        TicketID = ticketID;
        EventData = eventData;
    }
}

public class SupportSystemDataManager
{
    private static SupportSystemDataManager _instance;
    public static SupportSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SupportSystemDataManager();
            }
            return _instance;
        }
    }

    public SupportSystemData supportData;
    private List<SupportEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private SupportSystemDataManager()
    {
        supportData = new SupportSystemData();
        recentEvents = new List<SupportEvent>();
        LoadSupportData();
    }

    public void SaveSupportData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SupportSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, supportData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存客服系统数据失败: " + e.Message);
        }
    }

    public void LoadSupportData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "SupportSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    supportData = (SupportSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载客服系统数据失败: " + e.Message);
            supportData = new SupportSystemData();
        }
    }

    public void CreateSupportEvent(string eventType, string playerID, string ticketID, string eventData)
    {
        string eventID = "support_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SupportEvent supportEvent = new SupportEvent(eventID, eventType, playerID, ticketID, eventData);
        recentEvents.Add(supportEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<SupportEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
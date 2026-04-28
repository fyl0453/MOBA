using System;
using System.Collections.Generic;

public class SupportSystemDetailedManager
{
    private static SupportSystemDetailedManager _instance;
    public static SupportSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SupportSystemDetailedManager();
            }
            return _instance;
        }
    }

    private SupportSystemData supportData;
    private SupportSystemDataManager dataManager;

    private SupportSystemDetailedManager()
    {
        dataManager = SupportSystemDataManager.Instance;
        supportData = dataManager.supportData;
    }

    public string CreateSupportTicket(string playerID, string playerName, string title, string content, int ticketType, string priority = "medium")
    {
        if (!supportData.PlayerSupportData.ContainsKey(playerID))
        {
            PlayerSupportData playerData = new PlayerSupportData(playerID);
            supportData.AddPlayerSupportData(playerID, playerData);
        }
        
        string ticketID = "ticket_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SupportTicket ticket = new SupportTicket(ticketID, playerID, playerName, title, content, ticketType, priority);
        supportData.AddTicket(ticket);
        
        PlayerSupportData playerSupportData = supportData.PlayerSupportData[playerID];
        playerSupportData.PlayerTickets.Add(ticket);
        playerSupportData.TotalTickets++;
        playerSupportData.OpenTickets++;
        playerSupportData.LastTicketTime = DateTime.Now;
        
        dataManager.CreateSupportEvent("ticket_create", playerID, ticketID, "创建工单: " + title);
        dataManager.SaveSupportData();
        Debug.Log("创建工单成功: " + title);
        return ticketID;
    }

    public void AddTicketMessage(string ticketID, string senderID, string senderName, string content, bool isSystem = false, string messageType = "text")
    {
        SupportTicket ticket = GetTicket(ticketID);
        if (ticket != null)
        {
            string messageID = "message_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            SupportTicketMessage message = new SupportTicketMessage(messageID, ticketID, senderID, senderName, content, isSystem, messageType);
            ticket.Messages.Add(message);
            ticket.UpdateTime = DateTime.Now;
            dataManager.SaveSupportData();
            Debug.Log("添加工单消息成功");
        }
    }

    public void AssignTicket(string ticketID, string agentID)
    {
        SupportTicket ticket = GetTicket(ticketID);
        SupportAgent agent = GetSupportAgent(agentID);
        if (ticket != null && agent != null)
        {
            ticket.AssignedTo = agentID;
            agent.AssignedTickets++;
            agent.Status = "busy";
            ticket.Status = 1;
            ticket.UpdateTime = DateTime.Now;
            dataManager.CreateSupportEvent("ticket_assign", ticket.PlayerID, ticketID, "分配工单给: " + agent.AgentName);
            dataManager.SaveSupportData();
            Debug.Log("分配工单成功给: " + agent.AgentName);
        }
    }

    public void ResolveTicket(string ticketID, string resolution)
    {
        SupportTicket ticket = GetTicket(ticketID);
        if (ticket != null)
        {
            ticket.Status = 2;
            ticket.Resolution = resolution;
            ticket.UpdateTime = DateTime.Now;
            
            if (!string.IsNullOrEmpty(ticket.AssignedTo))
            {
                SupportAgent agent = GetSupportAgent(ticket.AssignedTo);
                if (agent != null)
                {
                    agent.AssignedTickets--;
                    if (agent.AssignedTickets == 0)
                    {
                        agent.Status = "available";
                    }
                }
            }
            
            PlayerSupportData playerData = supportData.PlayerSupportData[ticket.PlayerID];
            playerData.OpenTickets--;
            
            supportData.OpenTickets--;
            dataManager.CreateSupportEvent("ticket_resolve", ticket.PlayerID, ticketID, "解决工单");
            dataManager.SaveSupportData();
            Debug.Log("解决工单成功");
        }
    }

    public void CloseTicket(string ticketID, string reason)
    {
        SupportTicket ticket = GetTicket(ticketID);
        if (ticket != null)
        {
            ticket.Status = 3;
            ticket.Resolution = reason;
            ticket.UpdateTime = DateTime.Now;
            
            if (!string.IsNullOrEmpty(ticket.AssignedTo))
            {
                SupportAgent agent = GetSupportAgent(ticket.AssignedTo);
                if (agent != null)
                {
                    agent.AssignedTickets--;
                    if (agent.AssignedTickets == 0)
                    {
                        agent.Status = "available";
                    }
                }
            }
            
            PlayerSupportData playerData = supportData.PlayerSupportData[ticket.PlayerID];
            playerData.OpenTickets--;
            
            supportData.OpenTickets--;
            dataManager.CreateSupportEvent("ticket_close", ticket.PlayerID, ticketID, "关闭工单: " + reason);
            dataManager.SaveSupportData();
            Debug.Log("关闭工单成功");
        }
    }

    public SupportTicket GetTicket(string ticketID)
    {
        return supportData.AllTickets.Find(t => t.TicketID == ticketID);
    }

    public List<SupportTicket> GetAllTickets()
    {
        return supportData.AllTickets;
    }

    public List<SupportTicket> GetOpenTickets()
    {
        return supportData.AllTickets.FindAll(t => t.Status == 0 || t.Status == 1);
    }

    public List<SupportTicket> GetPlayerTickets(string playerID)
    {
        if (supportData.PlayerSupportData.ContainsKey(playerID))
        {
            return supportData.PlayerSupportData[playerID].PlayerTickets;
        }
        return new List<SupportTicket>();
    }

    public List<SupportTicket> GetAgentTickets(string agentID)
    {
        return supportData.AllTickets.FindAll(t => t.AssignedTo == agentID);
    }

    public List<FAQ> GetAllFAQs()
    {
        return supportData.FAQs;
    }

    public List<FAQ> GetFAQsByCategory(string category)
    {
        return supportData.FAQs.FindAll(f => f.Category == category && f.IsActive);
    }

    public FAQ GetFAQ(string faqID)
    {
        return supportData.FAQs.Find(f => f.FAQID == faqID);
    }

    public void AddFAQ(string question, string answer, string category, int priority = 1)
    {
        string faqID = "faq_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        FAQ faq = new FAQ(faqID, question, answer, category, priority);
        supportData.AddFAQ(faq);
        dataManager.SaveSupportData();
        Debug.Log("添加FAQ成功: " + question);
    }

    public void UpdateFAQ(string faqID, string question, string answer, string category, int priority, bool isActive)
    {
        FAQ faq = GetFAQ(faqID);
        if (faq != null)
        {
            faq.Question = question;
            faq.Answer = answer;
            faq.Category = category;
            faq.Priority = priority;
            faq.IsActive = isActive;
            faq.LastUpdateTime = DateTime.Now;
            dataManager.SaveSupportData();
            Debug.Log("更新FAQ成功: " + question);
        }
    }

    public void IncrementFAQViewCount(string faqID)
    {
        FAQ faq = GetFAQ(faqID);
        if (faq != null)
        {
            faq.ViewCount++;
            dataManager.SaveSupportData();
        }
    }

    public SupportAgent GetSupportAgent(string agentID)
    {
        return supportData.SupportAgents.Find(a => a.AgentID == agentID);
    }

    public List<SupportAgent> GetAllSupportAgents()
    {
        return supportData.SupportAgents;
    }

    public List<SupportAgent> GetOnlineSupportAgents()
    {
        return supportData.SupportAgents.FindAll(a => a.IsOnline);
    }

    public void AddSupportAgent(string agentName, string agentRole)
    {
        string agentID = "agent_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SupportAgent agent = new SupportAgent(agentID, agentName, agentRole);
        supportData.AddSupportAgent(agent);
        dataManager.SaveSupportData();
        Debug.Log("添加客服人员成功: " + agentName);
    }

    public void UpdateSupportAgentStatus(string agentID, bool isOnline, string status)
    {
        SupportAgent agent = GetSupportAgent(agentID);
        if (agent != null)
        {
            agent.IsOnline = isOnline;
            agent.Status = status;
            if (isOnline)
            {
                agent.LastLoginTime = DateTime.Now;
            }
            dataManager.SaveSupportData();
            Debug.Log("更新客服状态成功: " + agent.AgentName);
        }
    }

    public int GetPlayerOpenTicketCount(string playerID)
    {
        if (supportData.PlayerSupportData.ContainsKey(playerID))
        {
            return supportData.PlayerSupportData[playerID].OpenTickets;
        }
        return 0;
    }

    public int GetPlayerTotalTicketCount(string playerID)
    {
        if (supportData.PlayerSupportData.ContainsKey(playerID))
        {
            return supportData.PlayerSupportData[playerID].TotalTickets;
        }
        return 0;
    }

    public void CleanupOldTickets(int days = 90)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<SupportTicket> oldTickets = new List<SupportTicket>();
        foreach (SupportTicket ticket in supportData.AllTickets)
        {
            if (ticket.CreateTime < cutoffDate && (ticket.Status == 2 || ticket.Status == 3))
            {
                oldTickets.Add(ticket);
            }
        }
        
        foreach (SupportTicket ticket in oldTickets)
        {
            supportData.AllTickets.Remove(ticket);
            if (supportData.PlayerSupportData.ContainsKey(ticket.PlayerID))
            {
                PlayerSupportData playerData = supportData.PlayerSupportData[ticket.PlayerID];
                playerData.PlayerTickets.Remove(ticket);
            }
        }
        
        if (oldTickets.Count > 0)
        {
            dataManager.CreateSupportEvent("ticket_cleanup", "system", "", "清理旧工单: " + oldTickets.Count);
            dataManager.SaveSupportData();
            Debug.Log("清理旧工单成功: " + oldTickets.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveSupportData();
    }

    public void LoadData()
    {
        dataManager.LoadSupportData();
    }

    public List<SupportEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
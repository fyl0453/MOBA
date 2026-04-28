using System;
using System.Collections.Generic;

public class CalendarSystemDetailedManager
{
    private static CalendarSystemDetailedManager _instance;
    public static CalendarSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CalendarSystemDetailedManager();
            }
            return _instance;
        }
    }

    private CalendarSystemData calendarData;
    private CalendarSystemDataManager dataManager;

    private CalendarSystemDetailedManager()
    {
        dataManager = CalendarSystemDataManager.Instance;
        calendarData = dataManager.calendarData;
    }

    public string CreateCalendarEvent(string eventName, string description, int eventType, DateTime startTime, DateTime endTime, string eventCategory, List<CalendarEventReward> rewards = null)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CalendarEvent calendarEvent = new CalendarEvent(eventID, eventName, description, eventType, startTime, endTime, eventCategory);
        if (rewards != null)
        {
            calendarEvent.Rewards = rewards;
        }
        calendarData.AddEvent(calendarEvent);
        dataManager.CreateCalendarEvent("event_create", "创建活动: " + eventName);
        dataManager.SaveCalendarData();
        Debug.Log("创建活动成功: " + eventName);
        return eventID;
    }

    public void UpdateCalendarEvent(string eventID, string eventName, string description, int eventType, DateTime startTime, DateTime endTime, string eventCategory)
    {
        CalendarEvent calendarEvent = GetEvent(eventID);
        if (calendarEvent != null)
        {
            calendarEvent.EventName = eventName;
            calendarEvent.Description = description;
            calendarEvent.EventType = eventType;
            calendarEvent.StartTime = startTime;
            calendarEvent.EndTime = endTime;
            calendarEvent.EventCategory = eventCategory;
            dataManager.SaveCalendarData();
            Debug.Log("更新活动成功: " + eventName);
        }
    }

    public void DeleteCalendarEvent(string eventID)
    {
        CalendarEvent calendarEvent = GetEvent(eventID);
        if (calendarEvent != null)
        {
            calendarData.AllEvents.Remove(calendarEvent);
            RemoveEventFromDate(calendarEvent);
            RemoveEventFromCategory(calendarEvent);
            dataManager.CreateCalendarEvent("event_delete", "删除活动: " + calendarEvent.EventName);
            dataManager.SaveCalendarData();
            Debug.Log("删除活动成功: " + calendarEvent.EventName);
        }
    }

    public void ActivateEvent(string eventID)
    {
        CalendarEvent calendarEvent = GetEvent(eventID);
        if (calendarEvent != null && !calendarEvent.IsActive)
        {
            calendarEvent.IsActive = true;
            dataManager.CreateCalendarEvent("event_activate", "激活活动: " + calendarEvent.EventName);
            dataManager.SaveCalendarData();
            Debug.Log("激活活动成功: " + calendarEvent.EventName);
        }
    }

    public void DeactivateEvent(string eventID)
    {
        CalendarEvent calendarEvent = GetEvent(eventID);
        if (calendarEvent != null && calendarEvent.IsActive)
        {
            calendarEvent.IsActive = false;
            dataManager.CreateCalendarEvent("event_deactivate", "停用活动: " + calendarEvent.EventName);
            dataManager.SaveCalendarData();
            Debug.Log("停用活动成功: " + calendarEvent.EventName);
        }
    }

    public void AddEventReward(string eventID, string rewardName, string rewardType, int rewardValue, int requiredProgress = 0)
    {
        CalendarEvent calendarEvent = GetEvent(eventID);
        if (calendarEvent != null)
        {
            string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            CalendarEventReward reward = new CalendarEventReward(rewardID, rewardName, rewardType, rewardValue, requiredProgress);
            calendarEvent.Rewards.Add(reward);
            dataManager.SaveCalendarData();
            Debug.Log("添加活动奖励成功: " + rewardName);
        }
    }

    public CalendarEvent GetEvent(string eventID)
    {
        return calendarData.AllEvents.Find(e => e.EventID == eventID);
    }

    public List<CalendarEvent> GetAllEvents()
    {
        return calendarData.AllEvents;
    }

    public List<CalendarEvent> GetActiveEvents()
    {
        return calendarData.AllEvents.FindAll(e => e.IsActive);
    }

    public List<CalendarEvent> GetEventsByDate(DateTime date)
    {
        string dateKey = date.ToString("yyyy-MM-dd");
        if (calendarData.EventsByDate.ContainsKey(dateKey))
        {
            List<CalendarEvent> events = new List<CalendarEvent>();
            foreach (string eventID in calendarData.EventsByDate[dateKey])
            {
                CalendarEvent calendarEvent = GetEvent(eventID);
                if (calendarEvent != null && calendarEvent.IsActive)
                {
                    events.Add(calendarEvent);
                }
            }
            return events;
        }
        return new List<CalendarEvent>();
    }

    public List<CalendarEvent> GetEventsByCategory(string category)
    {
        if (calendarData.EventsByCategory.ContainsKey(category))
        {
            List<CalendarEvent> events = new List<CalendarEvent>();
            foreach (string eventID in calendarData.EventsByCategory[category])
            {
                CalendarEvent calendarEvent = GetEvent(eventID);
                if (calendarEvent != null && calendarEvent.IsActive)
                {
                    events.Add(calendarEvent);
                }
            }
            return events;
        }
        return new List<CalendarEvent>();
    }

    public List<CalendarEvent> GetUpcomingEvents(int days = 7)
    {
        DateTime now = DateTime.Now;
        DateTime future = now.AddDays(days);
        List<CalendarEvent> upcomingEvents = new List<CalendarEvent>();
        foreach (CalendarEvent calendarEvent in calendarData.AllEvents)
        {
            if (calendarEvent.IsActive && calendarEvent.StartTime >= now && calendarEvent.StartTime <= future)
            {
                upcomingEvents.Add(calendarEvent);
            }
        }
        upcomingEvents.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
        return upcomingEvents;
    }

    public List<CalendarEvent> GetOngoingEvents()
    {
        DateTime now = DateTime.Now;
        List<CalendarEvent> ongoingEvents = new List<CalendarEvent>();
        foreach (CalendarEvent calendarEvent in calendarData.AllEvents)
        {
            if (calendarEvent.IsActive && calendarEvent.StartTime <= now && calendarEvent.EndTime >= now)
            {
                ongoingEvents.Add(calendarEvent);
            }
        }
        return ongoingEvents;
    }

    public CalendarMonth GetMonth(int year, int month)
    {
        return calendarData.CalendarMonths.Find(m => m.Year == year && m.Month == month);
    }

    public void GenerateMonth(int year, int month)
    {
        string monthID = $"month_{year}_{month}";
        if (!calendarData.CalendarMonths.Exists(m => m.MonthID == monthID))
        {
            CalendarMonth newMonth = new CalendarMonth(monthID, year, month);
            calendarData.AddMonth(newMonth);
            dataManager.SaveCalendarData();
            Debug.Log("生成月份成功: " + newMonth.MonthName);
        }
    }

    public void GenerateNextMonths(int count = 3)
    {
        DateTime now = DateTime.Now;
        for (int i = 1; i <= count; i++)
        {
            DateTime nextMonth = now.AddMonths(i);
            GenerateMonth(nextMonth.Year, nextMonth.Month);
        }
    }

    private void RemoveEventFromDate(CalendarEvent calendarEvent)
    {
        DateTime current = calendarEvent.StartTime.Date;
        while (current <= calendarEvent.EndTime.Date)
        {
            string dateKey = current.ToString("yyyy-MM-dd");
            if (calendarData.EventsByDate.ContainsKey(dateKey))
            {
                calendarData.EventsByDate[dateKey].Remove(calendarEvent.EventID);
            }
            current = current.AddDays(1);
        }
    }

    private void RemoveEventFromCategory(CalendarEvent calendarEvent)
    {
        if (calendarData.EventsByCategory.ContainsKey(calendarEvent.EventCategory))
        {
            calendarData.EventsByCategory[calendarEvent.EventCategory].Remove(calendarEvent.EventID);
        }
    }

    public void CleanupExpiredEvents()
    {
        DateTime now = DateTime.Now;
        List<CalendarEvent> expiredEvents = new List<CalendarEvent>();
        foreach (CalendarEvent calendarEvent in calendarData.AllEvents)
        {
            if (calendarEvent.EndTime < now)
            {
                expiredEvents.Add(calendarEvent);
            }
        }
        
        foreach (CalendarEvent calendarEvent in expiredEvents)
        {
            calendarData.AllEvents.Remove(calendarEvent);
            RemoveEventFromDate(calendarEvent);
            RemoveEventFromCategory(calendarEvent);
        }
        
        if (expiredEvents.Count > 0)
        {
            dataManager.CreateCalendarEvent("event_cleanup", "清理过期活动: " + expiredEvents.Count);
            dataManager.SaveCalendarData();
            Debug.Log("清理过期活动成功: " + expiredEvents.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveCalendarData();
    }

    public void LoadData()
    {
        dataManager.LoadCalendarData();
    }

    public List<CalendarEventEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
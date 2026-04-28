using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class CalendarEvent
{
    public string EventID;
    public string EventName;
    public string Description;
    public int EventType;
    public DateTime StartTime;
    public DateTime EndTime;
    public string BannerImage;
    public string BackgroundImage;
    public string EventURL;
    public string EventCategory;
    public bool IsActive;
    public bool IsFeatured;
    public int Priority;
    public List<CalendarEventReward> Rewards;
    public List<string> Tags;

    public CalendarEvent(string eventID, string eventName, string description, int eventType, DateTime startTime, DateTime endTime, string eventCategory)
    {
        EventID = eventID;
        EventName = eventName;
        Description = description;
        EventType = eventType;
        StartTime = startTime;
        EndTime = endTime;
        BannerImage = "";
        BackgroundImage = "";
        EventURL = "";
        EventCategory = eventCategory;
        IsActive = false;
        IsFeatured = false;
        Priority = 1;
        Rewards = new List<CalendarEventReward>();
        Tags = new List<string>();
    }
}

[Serializable]
public class CalendarEventReward
{
    public string RewardID;
    public string RewardName;
    public string RewardType;
    public int RewardValue;
    public string IconName;
    public int RequiredProgress;

    public CalendarEventReward(string rewardID, string rewardName, string rewardType, int rewardValue, int requiredProgress = 0)
    {
        RewardID = rewardID;
        RewardName = rewardName;
        RewardType = rewardType;
        RewardValue = rewardValue;
        IconName = "";
        RequiredProgress = requiredProgress;
    }
}

[Serializable]
public class CalendarDay
{
    public string DayID;
    public DateTime Date;
    public List<string> EventIDs;
    public bool HasEvents;
    public string DayType;
    public string Note;

    public CalendarDay(string dayID, DateTime date)
    {
        DayID = dayID;
        Date = date;
        EventIDs = new List<string>();
        HasEvents = false;
        DayType = "normal";
        Note = "";
    }
}

[Serializable]
public class CalendarMonth
{
    public string MonthID;
    public int Year;
    public int Month;
    public List<CalendarDay> Days;
    public string MonthName;
    public DateTime StartDate;
    public DateTime EndDate;

    public CalendarMonth(string monthID, int year, int month)
    {
        MonthID = monthID;
        Year = year;
        Month = month;
        Days = new List<CalendarDay>();
        MonthName = $"{year}年{month}月";
        StartDate = new DateTime(year, month, 1);
        EndDate = StartDate.AddMonths(1).AddDays(-1);
        InitializeDays();
    }

    private void InitializeDays()
    {
        int daysInMonth = DateTime.DaysInMonth(Year, Month);
        for (int day = 1; day <= daysInMonth; day++)
        {
            string dayID = $"day_{Year}_{Month}_{day}";
            CalendarDay calendarDay = new CalendarDay(dayID, new DateTime(Year, Month, day));
            Days.Add(calendarDay);
        }
    }
}

[Serializable]
public class CalendarSystemData
{
    public List<CalendarEvent> AllEvents;
    public List<CalendarMonth> CalendarMonths;
    public Dictionary<string, List<string>> EventsByDate;
    public Dictionary<string, List<string>> EventsByCategory;
    public DateTime LastUpdateTime;

    public CalendarSystemData()
    {
        AllEvents = new List<CalendarEvent>();
        CalendarMonths = new List<CalendarMonth>();
        EventsByDate = new Dictionary<string, List<string>>();
        EventsByCategory = new Dictionary<string, List<string>>();
        LastUpdateTime = DateTime.Now;
        InitializeCurrentMonth();
    }

    private void InitializeCurrentMonth()
    {
        DateTime now = DateTime.Now;
        string monthID = $"month_{now.Year}_{now.Month}";
        CalendarMonth currentMonth = new CalendarMonth(monthID, now.Year, now.Month);
        CalendarMonths.Add(currentMonth);
    }

    public void AddEvent(CalendarEvent calendarEvent)
    {
        AllEvents.Add(calendarEvent);
        AddEventToDate(calendarEvent);
        AddEventToCategory(calendarEvent);
    }

    private void AddEventToDate(CalendarEvent calendarEvent)
    {
        DateTime current = calendarEvent.StartTime.Date;
        while (current <= calendarEvent.EndTime.Date)
        {
            string dateKey = current.ToString("yyyy-MM-dd");
            if (!EventsByDate.ContainsKey(dateKey))
            {
                EventsByDate[dateKey] = new List<string>();
            }
            if (!EventsByDate[dateKey].Contains(calendarEvent.EventID))
            {
                EventsByDate[dateKey].Add(calendarEvent.EventID);
            }
            current = current.AddDays(1);
        }
    }

    private void AddEventToCategory(CalendarEvent calendarEvent)
    {
        if (!EventsByCategory.ContainsKey(calendarEvent.EventCategory))
        {
            EventsByCategory[calendarEvent.EventCategory] = new List<string>();
        }
        if (!EventsByCategory[calendarEvent.EventCategory].Contains(calendarEvent.EventID))
        {
            EventsByCategory[calendarEvent.EventCategory].Add(calendarEvent.EventID);
        }
    }

    public void AddMonth(CalendarMonth month)
    {
        CalendarMonths.Add(month);
    }
}

[Serializable]
public class CalendarEventEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string EventData;

    public CalendarEventEvent(string eventID, string eventType, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        EventData = eventData;
    }
}

public class CalendarSystemDataManager
{
    private static CalendarSystemDataManager _instance;
    public static CalendarSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CalendarSystemDataManager();
            }
            return _instance;
        }
    }

    public CalendarSystemData calendarData;
    private List<CalendarEventEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private CalendarSystemDataManager()
    {
        calendarData = new CalendarSystemData();
        recentEvents = new List<CalendarEventEvent>();
        LoadCalendarData();
    }

    public void SaveCalendarData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "CalendarSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, calendarData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存活动日历数据失败: " + e.Message);
        }
    }

    public void LoadCalendarData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "CalendarSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    calendarData = (CalendarSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载活动日历数据失败: " + e.Message);
            calendarData = new CalendarSystemData();
        }
    }

    public void CreateCalendarEvent(string eventType, string eventData)
    {
        string eventID = "calendar_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CalendarEventEvent calendarEvent = new CalendarEventEvent(eventID, eventType, eventData);
        recentEvents.Add(calendarEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<CalendarEventEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance { get; private set; }
    
    private EventData eventData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadEventData();
            
            if (eventData == null)
            {
                eventData = new EventData();
                InitializeDefaultEvents();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeDefaultEvents()
    {
        // 新手活动
        AddEvent("event_newbie", "新手福利", "完成新手任务，获得丰厚奖励", System.DateTime.Now, System.DateTime.Now.AddDays(7), EventType.Newbie);
        
        // 限时活动
        AddEvent("event_limited", "限时挑战", "参与限时挑战，赢取稀有奖励", System.DateTime.Now, System.DateTime.Now.AddDays(3), EventType.Limited);
        
        // 节日活动
        AddEvent("event_holiday", "节日庆典", "庆祝节日，参与活动赢取奖励", System.DateTime.Now, System.DateTime.Now.AddDays(10), EventType.Holiday);
        
        // 周末活动
        AddEvent("event_weekend", "周末狂欢", "周末双倍经验，快来参与", System.DateTime.Now, System.DateTime.Now.AddDays(2), EventType.Weekend);
        
        // 累计登录活动
        AddEvent("event_login", "累计登录", "累计登录7天，领取豪华奖励", System.DateTime.Now, System.DateTime.Now.AddDays(14), EventType.Login);
        
        // 添加活动任务
        AddEventTask("event_newbie", "task_newbie_1", "完成一场游戏", 1, 100, "金币");
        AddEventTask("event_newbie", "task_newbie_2", "获得一场胜利", 1, 200, "金币");
        AddEventTask("event_newbie", "task_newbie_3", "击杀10个敌人", 10, 300, "金币");
        
        AddEventTask("event_limited", "task_limited_1", "完成3场游戏", 3, 1, "稀有皮肤碎片");
        AddEventTask("event_limited", "task_limited_2", "获得2场胜利", 2, 1, "英雄碎片");
        
        AddEventTask("event_holiday", "task_holiday_1", "完成5场游戏", 5, 500, "金币");
        AddEventTask("event_holiday", "task_holiday_2", "击杀50个敌人", 50, 1, "节日限定头像框");
        
        AddEventTask("event_weekend", "task_weekend_1", "完成2场游戏", 2, 2, "双倍经验卡");
        
        AddEventTask("event_login", "task_login_1", "登录1天", 1, 100, "金币");
        AddEventTask("event_login", "task_login_2", "登录3天", 3, 200, "金币");
        AddEventTask("event_login", "task_login_3", "登录7天", 7, 1, "稀有英雄");
        
        SaveEventData();
    }
    
    private void AddEvent(string eventID, string eventName, string description, System.DateTime startTime, System.DateTime endTime, EventType eventType)
    {
        Event newEvent = new Event
        {
            eventID = eventID,
            eventName = eventName,
            description = description,
            startTime = startTime.ToString(),
            endTime = endTime.ToString(),
            eventType = eventType,
            isActive = true,
            tasks = new List<EventTask>()
        };
        
        eventData.events.Add(newEvent);
    }
    
    private void AddEventTask(string eventID, string taskID, string taskName, int targetValue, int rewardValue, string rewardType)
    {
        Event targetEvent = eventData.events.Find(e => e.eventID == eventID);
        if (targetEvent != null)
        {
            EventTask task = new EventTask
            {
                taskID = taskID,
                taskName = taskName,
                targetValue = targetValue,
                currentValue = 0,
                isCompleted = false,
                rewardValue = rewardValue,
                rewardType = rewardType,
                isClaimed = false
            };
            
            targetEvent.tasks.Add(task);
        }
    }
    
    public List<Event> GetAllEvents()
    {
        return eventData.events;
    }
    
    public List<Event> GetActiveEvents()
    {
        List<Event> activeEvents = new List<Event>();
        System.DateTime now = System.DateTime.Now;
        
        foreach (Event e in eventData.events)
        {
            System.DateTime startTime = System.DateTime.Parse(e.startTime);
            System.DateTime endTime = System.DateTime.Parse(e.endTime);
            
            if (e.isActive && now >= startTime && now <= endTime)
            {
                activeEvents.Add(e);
            }
        }
        
        return activeEvents;
    }
    
    public List<Event> GetEventsByType(EventType eventType)
    {
        return eventData.events.FindAll(e => e.eventType == eventType);
    }
    
    public Event GetEvent(string eventID)
    {
        return eventData.events.Find(e => e.eventID == eventID);
    }
    
    public void UpdateEventTaskProgress(string eventID, string taskID, int value)
    {
        Event targetEvent = eventData.events.Find(e => e.eventID == eventID);
        if (targetEvent != null)
        {
            EventTask task = targetEvent.tasks.Find(t => t.taskID == taskID);
            if (task != null && !task.isCompleted)
            {
                task.currentValue += value;
                if (task.currentValue >= task.targetValue)
                {
                    task.currentValue = task.targetValue;
                    task.isCompleted = true;
                    Debug.Log($"活动任务完成: {task.taskName}");
                }
                SaveEventData();
            }
        }
    }
    
    public bool ClaimEventTaskReward(string eventID, string taskID)
    {
        Event targetEvent = eventData.events.Find(e => e.eventID == eventID);
        if (targetEvent != null)
        {
            EventTask task = targetEvent.tasks.Find(t => t.taskID == taskID);
            if (task != null && task.isCompleted && !task.isClaimed)
            {
                task.isClaimed = true;
                Debug.Log($"领取活动奖励: {task.rewardValue} {task.rewardType}");
                // 这里可以添加奖励发放逻辑
                SaveEventData();
                return true;
            }
        }
        return false;
    }
    
    public void SaveEventData()
    {
        string path = Application.dataPath + "/Data/event_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, eventData);
        stream.Close();
    }
    
    public void LoadEventData()
    {
        string path = Application.dataPath + "/Data/event_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            eventData = (EventData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            eventData = new EventData();
        }
    }
}

[System.Serializable]
public class EventData
{
    public List<Event> events = new List<Event>();
}

[System.Serializable]
public class Event
{
    public string eventID;
    public string eventName;
    public string description;
    public string startTime;
    public string endTime;
    public EventType eventType;
    public bool isActive;
    public List<EventTask> tasks;
}

[System.Serializable]
public class EventTask
{
    public string taskID;
    public string taskName;
    public int targetValue;
    public int currentValue;
    public bool isCompleted;
    public int rewardValue;
    public string rewardType;
    public bool isClaimed;
}

public enum EventType
{
    Newbie,
    Limited,
    Holiday,
    Weekend,
    Login,
    Other
}

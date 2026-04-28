using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EventManagerExtended : MonoBehaviour
{
    public static EventManagerExtended Instance { get; private set; }
    
    public EventManagerData eventData;
    
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
        LoadEventData();
        
        if (eventData == null)
        {
            eventData = new EventManagerData();
            InitializeDefaultEvents();
        }
        
        CheckEventStatus();
    }
    
    private void Update()
    {
        CheckEventStatus();
    }
    
    private void InitializeDefaultEvents()
    {
        // 创建春节活动
        string startDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string endDate = System.DateTime.Now.AddDays(14).ToString("yyyy-MM-dd HH:mm:ss");
        
        Event springEvent = new Event("event_spring", "春节活动", "2024年春节限时活动", "Holiday", startDate, endDate, "spring_banner", "spring_icon");
        
        // 创建活动任务
        EventTask task1 = new EventTask("task_spring_1", "event_spring", "参与比赛", "参与5场比赛", "Match", 5, startDate, endDate);
        EventTask task2 = new EventTask("task_spring_2", "event_spring", "赢得比赛", "赢得3场比赛", "Win", 3, startDate, endDate);
        EventTask task3 = new EventTask("task_spring_3", "event_spring", "使用特定英雄", "使用关羽参与2场比赛", "HeroUse", 2, startDate, endDate);
        
        // 创建活动奖励
        EventReward reward1 = new EventReward("reward_spring_1", "春节红包", "Currency", "gems", 100, "100钻石", false, 0);
        EventReward reward2 = new EventReward("reward_spring_2", "春节皮肤", "Skin", "skin_guanyu_spring", 1, "关羽新春皮肤", true, 1);
        EventReward reward3 = new EventReward("reward_spring_3", "春节头像框", "AvatarFrame", "frame_spring", 1, "春节限定头像框", true, 1);
        
        // 添加奖励到任务
        task1.AddReward("reward_spring_1");
        task2.AddReward("reward_spring_2");
        task3.AddReward("reward_spring_3");
        
        // 添加任务和奖励到活动
        springEvent.AddTask(task1);
        springEvent.AddTask(task2);
        springEvent.AddTask(task3);
        springEvent.AddReward(reward1);
        springEvent.AddReward(reward2);
        springEvent.AddReward(reward3);
        
        // 添加到系统
        eventData.system.AddEvent(springEvent);
        eventData.system.AddEventTask(task1);
        eventData.system.AddEventTask(task2);
        eventData.system.AddEventTask(task3);
        eventData.system.AddEventReward(reward1);
        eventData.system.AddEventReward(reward2);
        eventData.system.AddEventReward(reward3);
        
        // 创建情人节活动（即将开始）
        string valentineStart = System.DateTime.Now.AddDays(15).ToString("yyyy-MM-dd HH:mm:ss");
        string valentineEnd = System.DateTime.Now.AddDays(22).ToString("yyyy-MM-dd HH:mm:ss");
        
        Event valentineEvent = new Event("event_valentine", "情人节活动", "2024年情人节限时活动", "Holiday", valentineStart, valentineEnd, "valentine_banner", "valentine_icon");
        
        // 创建活动任务
        EventTask valentineTask1 = new EventTask("task_valentine_1", "event_valentine", "组队比赛", "与好友组队参与3场比赛", "TeamMatch", 3, valentineStart, valentineEnd);
        EventTask valentineTask2 = new EventTask("task_valentine_2", "event_valentine", "送礼物", "给好友送5个礼物", "Gift", 5, valentineStart, valentineEnd);
        
        // 创建活动奖励
        EventReward valentineReward1 = new EventReward("reward_valentine_1", "情人节头像框", "AvatarFrame", "frame_valentine", 1, "情人节限定头像框", true, 1);
        EventReward valentineReward2 = new EventReward("reward_valentine_2", "情人节皮肤", "Skin", "skin_valentine", 1, "情人节限定皮肤", true, 1);
        
        // 添加奖励到任务
        valentineTask1.AddReward("reward_valentine_1");
        valentineTask2.AddReward("reward_valentine_2");
        
        // 添加任务和奖励到活动
        valentineEvent.AddTask(valentineTask1);
        valentineEvent.AddTask(valentineTask2);
        valentineEvent.AddReward(valentineReward1);
        valentineEvent.AddReward(valentineReward2);
        
        // 添加到系统
        eventData.system.AddEvent(valentineEvent);
        eventData.system.AddEventTask(valentineTask1);
        eventData.system.AddEventTask(valentineTask2);
        eventData.system.AddEventReward(valentineReward1);
        eventData.system.AddEventReward(valentineReward2);
        
        SaveEventData();
    }
    
    private void CheckEventStatus()
    {
        foreach (Event gameEvent in eventData.system.events)
        {
            if (gameEvent.IsActive() && !gameEvent.isEnabled)
            {
                gameEvent.EnableEvent();
                Debug.Log($"活动 {gameEvent.eventName} 已开始");
            }
            else if (gameEvent.IsExpired() && gameEvent.isEnabled)
            {
                gameEvent.DisableEvent();
                Debug.Log($"活动 {gameEvent.eventName} 已结束");
            }
        }
        
        SaveEventData();
    }
    
    public void CreateEvent(string name, string description, string type, string start, string end, string banner, string icon)
    {
        string eventID = System.Guid.NewGuid().ToString();
        Event gameEvent = new Event(eventID, name, description, type, start, end, banner, icon);
        eventData.system.AddEvent(gameEvent);
        SaveEventData();
    }
    
    public void AddEventTask(string eventID, string name, string description, string type, int required, string start, string end)
    {
        Event gameEvent = eventData.system.GetEvent(eventID);
        if (gameEvent != null)
        {
            string taskID = System.Guid.NewGuid().ToString();
            EventTask task = new EventTask(taskID, eventID, name, description, type, required, start, end);
            gameEvent.AddTask(task);
            eventData.system.AddEventTask(task);
            SaveEventData();
        }
    }
    
    public void AddEventReward(string eventID, string name, string type, string itemID, int quantity, string description, bool limited = false, int limit = 0)
    {
        Event gameEvent = eventData.system.GetEvent(eventID);
        if (gameEvent != null)
        {
            string rewardID = System.Guid.NewGuid().ToString();
            EventReward reward = new EventReward(rewardID, name, type, itemID, quantity, description, limited, limit);
            gameEvent.AddReward(reward);
            eventData.system.AddEventReward(reward);
            SaveEventData();
        }
    }
    
    public void AddTaskReward(string taskID, string rewardID)
    {
        EventTask task = eventData.system.eventTasks.Find(t => t.taskID == taskID);
        if (task != null)
        {
            task.AddReward(rewardID);
            SaveEventData();
        }
    }
    
    public void UpdateTaskProgress(string playerID, string eventID, string taskID, int progress)
    {
        PlayerEventData playerData = GetOrCreatePlayerEventData(playerID);
        EventTask task = eventData.system.eventTasks.Find(t => t.taskID == taskID && t.eventID == eventID);
        
        if (task != null && task.IsActive())
        {
            playerData.UpdateTaskProgress(eventID, taskID, progress);
            task.AddProgress(progress);
            
            if (task.isCompleted && !playerData.IsTaskCompleted(eventID, taskID))
            {
                playerData.UpdateTaskProgress(eventID, taskID, task.requiredProgress);
                // 这里可以添加任务完成的通知
            }
            
            SaveEventData();
        }
    }
    
    public void ClaimTaskReward(string playerID, string eventID, string taskID)
    {
        PlayerEventData playerData = GetOrCreatePlayerEventData(playerID);
        EventTask task = eventData.system.eventTasks.Find(t => t.taskID == taskID && t.eventID == eventID);
        
        if (task != null && task.isCompleted && !playerData.IsTaskClaimed(eventID, taskID))
        {
            playerData.ClaimTaskReward(eventID, taskID);
            task.Claim();
            
            // 发放奖励
            foreach (string rewardID in task.rewardIDs)
            {
                EventReward reward = eventData.system.GetReward(rewardID);
                if (reward != null && reward.CanClaim())
                {
                    GrantReward(playerID, reward);
                    reward.IncrementCount();
                }
            }
            
            SaveEventData();
        }
    }
    
    private void GrantReward(string playerID, EventReward reward)
    {
        switch (reward.rewardType)
        {
            case "Currency":
                if (reward.rewardItemID == "gold")
                {
                    ProfileManager.Instance.currentProfile.gold += reward.quantity;
                }
                else if (reward.rewardItemID == "gems")
                {
                    ProfileManager.Instance.currentProfile.gems += reward.quantity;
                }
                ProfileManager.Instance.SaveProfile();
                break;
            case "Skin":
                SkinManager.Instance.PurchaseSkin(reward.rewardItemID);
                break;
            case "AvatarFrame":
                // 这里需要添加头像框系统的逻辑
                break;
            case "Item":
                InventoryManager.Instance.AddItemToInventory(reward.rewardItemID, reward.quantity);
                break;
        }
    }
    
    public List<Event> GetActiveEvents()
    {
        return eventData.system.GetActiveEvents();
    }
    
    public List<Event> GetUpcomingEvents()
    {
        return eventData.system.GetUpcomingEvents();
    }
    
    public List<Event> GetExpiredEvents()
    {
        return eventData.system.GetExpiredEvents();
    }
    
    public Event GetEvent(string eventID)
    {
        return eventData.system.GetEvent(eventID);
    }
    
    public List<EventTask> GetTasksByEvent(string eventID)
    {
        return eventData.system.GetTasksByEvent(eventID);
    }
    
    public EventTask GetTask(string taskID)
    {
        return eventData.system.eventTasks.Find(t => t.taskID == taskID);
    }
    
    public PlayerEventData GetPlayerEventData(string playerID)
    {
        return eventData.GetPlayerEventData(playerID);
    }
    
    public int GetTaskProgress(string playerID, string eventID, string taskID)
    {
        PlayerEventData playerData = GetOrCreatePlayerEventData(playerID);
        return playerData.GetTaskProgress(eventID, taskID);
    }
    
    public bool IsTaskCompleted(string playerID, string eventID, string taskID)
    {
        PlayerEventData playerData = GetOrCreatePlayerEventData(playerID);
        return playerData.IsTaskCompleted(eventID, taskID);
    }
    
    public bool IsTaskClaimed(string playerID, string eventID, string taskID)
    {
        PlayerEventData playerData = GetOrCreatePlayerEventData(playerID);
        return playerData.IsTaskClaimed(eventID, taskID);
    }
    
    private PlayerEventData GetOrCreatePlayerEventData(string playerID)
    {
        PlayerEventData playerData = eventData.GetPlayerEventData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerEventData(playerID);
            eventData.AddPlayerEventData(playerData);
        }
        return playerData;
    }
    
    public void SaveEventData()
    {
        string path = Application.dataPath + "/Data/event_extended_data.dat";
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
        string path = Application.dataPath + "/Data/event_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            eventData = (EventManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            eventData = new EventManagerData();
        }
    }
}
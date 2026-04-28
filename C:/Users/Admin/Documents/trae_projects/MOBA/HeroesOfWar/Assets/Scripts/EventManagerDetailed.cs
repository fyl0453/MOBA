using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EventManagerDetailed : MonoBehaviour
{
    public static EventManagerDetailed Instance { get; private set; }
    
    public EventSystemDetailedData eventData;
    
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
            eventData = new EventSystemDetailedData();
            InitializeDefaultEvents();
        }
        
        // 检查活动状态
        CheckEventStatus();
    }
    
    private void CheckEventStatus()
    {
        foreach (Event eventObj in eventData.system.events)
        {
            string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (currentTime >= eventObj.startTime && currentTime <= eventObj.endTime)
            {
                eventObj.Activate();
            }
            else
            {
                eventObj.Deactivate();
            }
        }
        SaveEventData();
    }
    
    private void InitializeDefaultEvents()
    {
        // 创建活动分类
        EventCategory限时活动 = new EventCategory("category_time_limited", "限时活动", "限时开放的活动");
        EventCategory节日活动 = new EventCategory("category_festival", "节日活动", "节日相关的活动");
        EventCategory联动活动 = new EventCategory("category_crossover", "联动活动", "与其他IP联动的活动");
        EventCategory日常活动 = new EventCategory("category_daily", "日常活动", "每日可参与的活动");
        
        eventData.system.AddCategory(限时活动);
        eventData.system.AddCategory(节日活动);
        eventData.system.AddCategory(联动活动);
        eventData.system.AddCategory(日常活动);
        
        // 创建活动奖励
        EventReward reward1 = new EventReward("reward_gold_1000", "金币奖励", "1000金币", "Gold", 1000);
        EventReward reward2 = new EventReward("reward_gems_50", "钻石奖励", "50钻石", "Gems", 50);
        EventReward reward3 = new EventReward("reward_skin_guanyu", "关羽皮肤", "关羽新春皮肤", "Skin", 1, "skin_guanyu_spring");
        EventReward reward4 = new EventReward("reward_avatar_frame_spring", "新春头像框", "2024新春头像框", "AvatarFrame", 1, "frame_spring");
        
        eventData.system.AddReward(reward1);
        eventData.system.AddReward(reward2);
        eventData.system.AddReward(reward3);
        eventData.system.AddReward(reward4);
        
        // 创建活动任务
        EventTask task1 = new EventTask("task_play_5", "参与对局", "参与5场对局", "PlayMatch", 5, "reward_gold_1000");
        EventTask task2 = new EventTask("task_win_3", "获得胜利", "获得3场胜利", "WinMatch", 3, "reward_gems_50");
        EventTask task3 = new EventTask("task_guanyu_play_3", "使用关羽", "使用关羽参与3场对局", "PlayHero", 3, "reward_skin_guanyu");
        EventTask task4 = new EventTask("task_collect_10", "收集道具", "收集10个新春道具", "CollectItem", 10, "reward_avatar_frame_spring");
        
        eventData.system.AddTask(task1);
        eventData.system.AddTask(task2);
        eventData.system.AddTask(task3);
        eventData.system.AddTask(task4);
        
        // 创建日常活动
        Event dailyEvent = new Event("event_daily", "每日任务", "完成每日任务获得奖励", "category_daily", "Daily", "2024-01-01 00:00:00", "2024-12-31 23:59:59");
        dailyEvent.AddTask("task_play_5");
        dailyEvent.AddTask("task_win_3");
        dailyEvent.Activate();
        
        // 创建新春活动
        Event springEvent = new Event("event_spring", "新春活动", "2024新春限定活动", "category_festival", "Festival", "2024-01-01 00:00:00", "2024-02-01 23:59:59");
        springEvent.AddTask("task_guanyu_play_3");
        springEvent.AddTask("task_collect_10");
        springEvent.Activate();
        
        eventData.system.AddEvent(dailyEvent);
        eventData.system.AddEvent(springEvent);
        
        SaveEventData();
    }
    
    public void CreateEvent(string name, string description, string category, string type, string start, string end, List<string> taskIDs, List<string> rewardIDs)
    {
        string eventID = System.Guid.NewGuid().ToString();
        Event newEvent = new Event(eventID, name, description, category, type, start, end);
        
        foreach (string taskID in taskIDs)
        {
            newEvent.AddTask(taskID);
        }
        
        foreach (string rewardID in rewardIDs)
        {
            newEvent.AddReward(rewardID);
        }
        
        eventData.system.AddEvent(newEvent);
        SaveEventData();
        Debug.Log($"成功创建活动: {name}");
    }
    
    public void CreateTask(string name, string description, string type, int required, string rewardID)
    {
        string taskID = System.Guid.NewGuid().ToString();
        EventTask newTask = new EventTask(taskID, name, description, type, required, rewardID);
        eventData.system.AddTask(newTask);
        SaveEventData();
        Debug.Log($"成功创建任务: {name}");
    }
    
    public void CreateReward(string name, string description, string type, int amount, string itemID = "")
    {
        string rewardID = System.Guid.NewGuid().ToString();
        EventReward newReward = new EventReward(rewardID, name, description, type, amount, itemID);
        eventData.system.AddReward(newReward);
        SaveEventData();
        Debug.Log($"成功创建奖励: {name}");
    }
    
    public void UpdateTaskProgress(string playerID, string eventID, string taskID, int count)
    {
        PlayerEventData playerData = GetOrCreatePlayerData(playerID);
        
        // 检查玩家是否有该活动的任务数据
        if (!playerData.eventTasks.ContainsKey(eventID))
        {
            // 初始化活动任务数据
            Event eventObj = eventData.system.GetEvent(eventID);
            if (eventObj != null)
            {
                foreach (string taskId in eventObj.taskIDs)
                {
                    EventTask task = eventData.system.GetTask(taskId);
                    if (task != null)
                    {
                        // 创建任务副本
                        EventTask taskCopy = new EventTask(task.taskID, task.taskName, task.taskDescription, task.taskType, task.requiredCount, task.rewardID);
                        playerData.AddEventTask(eventID, taskCopy);
                    }
                }
            }
        }
        
        // 更新任务进度
        playerData.UpdateTaskProgress(eventID, taskID, count);
        SaveEventData();
        
        // 检查任务是否完成
        List<EventTask> tasks = playerData.GetEventTasks(eventID);
        EventTask updatedTask = tasks.Find(t => t.taskID == taskID);
        if (updatedTask != null && updatedTask.isCompleted)
        {
            Debug.Log($"任务完成: {updatedTask.taskName}");
        }
    }
    
    public void ClaimTaskReward(string playerID, string eventID, string taskID)
    {
        PlayerEventData playerData = GetOrCreatePlayerData(playerID);
        playerData.ClaimTaskReward(eventID, taskID);
        
        // 发放奖励
        List<EventTask> tasks = playerData.GetEventTasks(eventID);
        EventTask task = tasks.Find(t => t.taskID == taskID);
        if (task != null && task.isClaimed)
        {
            EventReward reward = eventData.system.GetReward(task.rewardID);
            if (reward != null)
            {
                if (reward.rewardType == "Gold")
                {
                    ProfileManager.Instance.currentProfile.gold += reward.rewardAmount;
                }
                else if (reward.rewardType == "Gems")
                {
                    ProfileManager.Instance.currentProfile.gems += reward.rewardAmount;
                }
                else if (reward.rewardType == "Skin")
                {
                    // 发放皮肤
                }
                else if (reward.rewardType == "AvatarFrame")
                {
                    // 发放头像框
                }
                
                ProfileManager.Instance.SaveProfile();
                Debug.Log($"成功领取奖励: {reward.rewardName}");
            }
        }
        
        SaveEventData();
    }
    
    public List<Event> GetActiveEvents()
    {
        return eventData.system.GetActiveEvents();
    }
    
    public List<EventTask> GetEventTasks(string playerID, string eventID)
    {
        PlayerEventData playerData = GetOrCreatePlayerData(playerID);
        return playerData.GetEventTasks(eventID);
    }
    
    public Event GetEvent(string eventID)
    {
        return eventData.system.GetEvent(eventID);
    }
    
    public EventTask GetTask(string taskID)
    {
        return eventData.system.GetTask(taskID);
    }
    
    public EventReward GetReward(string rewardID)
    {
        return eventData.system.GetReward(rewardID);
    }
    
    public void ActivateEvent(string eventID)
    {
        Event eventObj = eventData.system.GetEvent(eventID);
        if (eventObj != null)
        {
            eventObj.Activate();
            SaveEventData();
            Debug.Log($"成功激活活动: {eventObj.eventName}");
        }
    }
    
    public void DeactivateEvent(string eventID)
    {
        Event eventObj = eventData.system.GetEvent(eventID);
        if (eventObj != null)
        {
            eventObj.Deactivate();
            SaveEventData();
            Debug.Log($"成功停用活动: {eventObj.eventName}");
        }
    }
    
    private PlayerEventData GetOrCreatePlayerData(string playerID)
    {
        PlayerEventData playerData = eventData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerEventData(playerID);
            eventData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public void SaveEventData()
    {
        string path = Application.dataPath + "/Data/event_system_detailed_data.dat";
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
        string path = Application.dataPath + "/Data/event_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            eventData = (EventSystemDetailedData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            eventData = new EventSystemDetailedData();
        }
    }
}
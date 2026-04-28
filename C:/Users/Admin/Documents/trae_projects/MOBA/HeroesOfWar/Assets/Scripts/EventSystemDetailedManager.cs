using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EventSystemDetailedManager : MonoBehaviour
{
    public static EventSystemDetailedManager Instance { get; private set; }
    
    public EventSystemDetailedManagerData eventData;
    
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
            eventData = new EventSystemDetailedManagerData();
            InitializeDefaultEventSystem();
        }
    }
    
    private void InitializeDefaultEventSystem()
    {
        // 活动
        string startDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string endDate = System.DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss");
        
        Event event1 = new Event("event_001", "春节活动", "春节特别活动", "festival", startDate, endDate, false);
        Event event2 = new Event("event_002", "新手活动", "新手专属活动", "newbie", startDate, endDate, true);
        Event event3 = new Event("event_003", "周末活动", "周末限时活动", "weekly", startDate, endDate, false);
        
        eventData.system.AddEvent(event1);
        eventData.system.AddEvent(event2);
        eventData.system.AddEvent(event3);
        
        // 活动任务
        EventTask task1 = new EventTask("task_001", "event_001", "登录游戏", "每天登录游戏", "login", 1, 10, 5, true);
        EventTask task2 = new EventTask("task_002", "event_001", "胜利一场", "完成一场胜利", "win", 1, 20, 10, true);
        EventTask task3 = new EventTask("task_003", "event_001", "击杀10人", "累计击杀10人", "kill", 10, 30, 15, true);
        EventTask task4 = new EventTask("task_004", "event_002", "完成新手引导", "完成新手引导", "guide", 1, 50, 25, false);
        EventTask task5 = new EventTask("task_005", "event_002", "首充", "首次充值", "recharge", 1, 100, 50, false);
        
        eventData.system.AddEventTask(task1);
        eventData.system.AddEventTask(task2);
        eventData.system.AddEventTask(task3);
        eventData.system.AddEventTask(task4);
        eventData.system.AddEventTask(task5);
        
        // 添加任务到活动
        event1.AddTask("task_001");
        event1.AddTask("task_002");
        event1.AddTask("task_003");
        event2.AddTask("task_004");
        event2.AddTask("task_005");
        
        // 活动奖励
        EventReward reward1 = new EventReward("reward_001", "event_001", "金币奖励", "currency", "gold", 1000, 10, 1, "icon_gold");
        EventReward reward2 = new EventReward("reward_002", "event_001", "钻石奖励", "currency", "diamond", 100, 20, 2, "icon_diamond");
        EventReward reward3 = new EventReward("reward_003", "event_001", "英雄碎片", "fragment", "hero_fragment", 5, 30, 3, "icon_hero_fragment");
        EventReward reward4 = new EventReward("reward_004", "event_002", "新手礼包", "package", "newbie_package", 1, 0, 1, "icon_package");
        EventReward reward5 = new EventReward("reward_005", "event_002", "限定皮肤", "skin", "skin_001", 1, 50, 2, "icon_skin");
        
        eventData.system.AddEventReward(reward1);
        eventData.system.AddEventReward(reward2);
        eventData.system.AddEventReward(reward3);
        eventData.system.AddEventReward(reward4);
        eventData.system.AddEventReward(reward5);
        
        // 添加奖励到活动
        event1.AddReward("reward_001");
        event1.AddReward("reward_002");
        event1.AddReward("reward_003");
        event2.AddReward("reward_004");
        event2.AddReward("reward_005");
        
        // 玩家活动
        PlayerEvent playerEvent1 = new PlayerEvent("player_event_001", "user_001", "event_001");
        PlayerEvent playerEvent2 = new PlayerEvent("player_event_002", "user_001", "event_002");
        PlayerEvent playerEvent3 = new PlayerEvent("player_event_003", "user_002", "event_001");
        
        eventData.system.AddPlayerEvent(playerEvent1);
        eventData.system.AddPlayerEvent(playerEvent2);
        eventData.system.AddPlayerEvent(playerEvent3);
        
        // 玩家活动任务
        PlayerEventTask playerTask1 = new PlayerEventTask("player_task_001", "task_001");
        PlayerEventTask playerTask2 = new PlayerEventTask("player_task_002", "task_002");
        PlayerEventTask playerTask3 = new PlayerEventTask("player_task_003", "task_004");
        
        playerEvent1.AddTask(playerTask1);
        playerEvent1.AddTask(playerTask2);
        playerEvent2.AddTask(playerTask3);
        
        // 玩家活动奖励
        PlayerEventReward playerReward1 = new PlayerEventReward("player_reward_001", "reward_001");
        PlayerEventReward playerReward2 = new PlayerEventReward("player_reward_002", "reward_004");
        
        playerEvent1.AddReward(playerReward1);
        playerEvent2.AddReward(playerReward2);
        
        // 活动参与
        EventParticipation participation1 = new EventParticipation("participation_001", "user_001", "event_001");
        EventParticipation participation2 = new EventParticipation("participation_002", "user_001", "event_002");
        EventParticipation participation3 = new EventParticipation("participation_003", "user_002", "event_001");
        
        eventData.system.AddEventParticipation(participation1);
        eventData.system.AddEventParticipation(participation2);
        eventData.system.AddEventParticipation(participation3);
        
        // 启动活动
        event1.Start();
        event2.Start();
        event3.Start();
        
        SaveEventData();
    }
    
    // 活动管理
    public void CreateEvent(string eventName, string eventDescription, string eventType, string startDate, string endDate, bool isLimited)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Event eventObj = new Event(eventID, eventName, eventDescription, eventType, startDate, endDate, isLimited);
        eventData.system.AddEvent(eventObj);
        SaveEventData();
        Debug.Log("成功创建活动: " + eventName);
    }
    
    public void StartEvent(string eventID)
    {
        Event eventObj = eventData.system.GetEvent(eventID);
        if (eventObj != null)
        {
            eventObj.Start();
            SaveEventData();
            Debug.Log("成功启动活动: " + eventObj.eventName);
        }
        else
        {
            Debug.LogError("活动不存在: " + eventID);
        }
    }
    
    public void EndEvent(string eventID)
    {
        Event eventObj = eventData.system.GetEvent(eventID);
        if (eventObj != null)
        {
            eventObj.End();
            SaveEventData();
            Debug.Log("成功结束活动: " + eventObj.eventName);
        }
        else
        {
            Debug.LogError("活动不存在: " + eventID);
        }
    }
    
    public List<Event> GetActiveEvents()
    {
        List<Event> activeEvents = new List<Event>();
        foreach (Event eventObj in eventData.system.events)
        {
            if (eventObj.IsActive())
            {
                activeEvents.Add(eventObj);
            }
        }
        return activeEvents;
    }
    
    public List<Event> GetEventsByType(string eventType)
    {
        return eventData.system.GetEventsByType(eventType);
    }
    
    // 活动任务管理
    public void AddEventTask(string eventID, string taskName, string taskDescription, string taskType, int requiredProgress, int rewardExp, int rewardPoints, bool isRepeatable)
    {
        string taskID = "task_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EventTask eventTask = new EventTask(taskID, eventID, taskName, taskDescription, taskType, requiredProgress, rewardExp, rewardPoints, isRepeatable);
        eventData.system.AddEventTask(eventTask);
        
        // 添加任务到活动
        Event eventObj = eventData.system.GetEvent(eventID);
        if (eventObj != null)
        {
            eventObj.AddTask(taskID);
        }
        
        SaveEventData();
        Debug.Log("成功添加活动任务: " + taskName);
    }
    
    public List<EventTask> GetEventTasks(string eventID)
    {
        return eventData.system.GetEventTasksByEvent(eventID);
    }
    
    // 活动奖励管理
    public void AddEventReward(string eventID, string rewardName, string rewardType, string rewardValue, int quantity, int requiredPoints, int requiredLevel, string icon)
    {
        string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EventReward eventReward = new EventReward(rewardID, eventID, rewardName, rewardType, rewardValue, quantity, requiredPoints, requiredLevel, icon);
        eventData.system.AddEventReward(eventReward);
        
        // 添加奖励到活动
        Event eventObj = eventData.system.GetEvent(eventID);
        if (eventObj != null)
        {
            eventObj.AddReward(rewardID);
        }
        
        SaveEventData();
        Debug.Log("成功添加活动奖励: " + rewardName);
    }
    
    public List<EventReward> GetEventRewards(string eventID)
    {
        return eventData.system.GetEventRewardsByEvent(eventID);
    }
    
    // 玩家活动管理
    public void JoinEvent(string userID, string eventID)
    {
        Event eventObj = eventData.system.GetEvent(eventID);
        if (eventObj != null && eventObj.CanParticipate())
        {
            // 检查是否已经参与
            List<PlayerEvent> playerEvents = eventData.system.GetPlayerEventsByUser(userID);
            PlayerEvent existingEvent = playerEvents.Find(pe => pe.eventID == eventID);
            
            if (existingEvent == null)
            {
                // 创建玩家活动
                string playerEventID = "player_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                PlayerEvent playerEvent = new PlayerEvent(playerEventID, userID, eventID);
                eventData.system.AddPlayerEvent(playerEvent);
                
                // 添加任务
                List<EventTask> tasks = eventData.system.GetEventTasksByEvent(eventID);
                foreach (EventTask task in tasks)
                {
                    string playerTaskID = "player_task_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                    PlayerEventTask playerTask = new PlayerEventTask(playerTaskID, task.taskID);
                    playerEvent.AddTask(playerTask);
                }
                
                // 添加奖励
                List<EventReward> rewards = eventData.system.GetEventRewardsByEvent(eventID);
                foreach (EventReward reward in rewards)
                {
                    string playerRewardID = "player_reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                    PlayerEventReward playerReward = new PlayerEventReward(playerRewardID, reward.rewardID);
                    playerEvent.AddReward(playerReward);
                }
                
                // 创建参与记录
                string participationID = "participation_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                EventParticipation participation = new EventParticipation(participationID, userID, eventID);
                eventData.system.AddEventParticipation(participation);
                
                // 更新活动参与人数
                eventObj.AddParticipant();
                
                SaveEventData();
                Debug.Log("成功参与活动: " + eventObj.eventName);
            }
            else
            {
                Debug.LogError("已经参与该活动");
            }
        }
        else
        {
            Debug.LogError("活动不存在或不可参与");
        }
    }
    
    public void UpdateEventTaskProgress(string userID, string eventID, string taskID, int progress)
    {
        List<PlayerEvent> playerEvents = eventData.system.GetPlayerEventsByUser(userID);
        PlayerEvent playerEvent = playerEvents.Find(pe => pe.eventID == eventID);
        
        if (playerEvent != null)
        {
            PlayerEventTask playerTask = playerEvent.tasks.Find(pt => pt.taskID == taskID);
            if (playerTask != null && playerTask.status == "in_progress")
            {
                EventTask task = eventData.system.GetEventTask(taskID);
                if (task != null && task.status == "active")
                {
                    playerTask.UpdateProgress(progress);
                    
                    // 检查是否完成任务
                    if (playerTask.currentProgress >= task.requiredProgress)
                    {
                        playerTask.Complete();
                        playerEvent.AddExp(task.rewardExp);
                        playerEvent.AddPoints(task.rewardPoints);
                        
                        // 如果任务可重复，重置任务
                        if (task.isRepeatable)
                        {
                            playerTask.Reset();
                        }
                    }
                    
                    SaveEventData();
                    Debug.Log("成功更新活动任务进度: " + task.taskName);
                }
                else
                {
                    Debug.LogError("任务不存在或已禁用");
                }
            }
            else
            {
                Debug.LogError("玩家任务不存在或已完成");
            }
        }
        else
        {
            Debug.LogError("玩家未参与该活动");
        }
    }
    
    public void ClaimEventReward(string userID, string eventID, string rewardID)
    {
        List<PlayerEvent> playerEvents = eventData.system.GetPlayerEventsByUser(userID);
        PlayerEvent playerEvent = playerEvents.Find(pe => pe.eventID == eventID);
        
        if (playerEvent != null)
        {
            PlayerEventReward playerReward = playerEvent.rewards.Find(pr => pr.rewardID == rewardID);
            if (playerReward != null && !playerReward.isClaimed)
            {
                EventReward reward = eventData.system.GetEventReward(rewardID);
                if (reward != null && reward.isEnabled)
                {
                    // 检查是否满足条件
                    if (playerEvent.points >= reward.requiredPoints && playerEvent.level >= reward.requiredLevel)
                    {
                        playerReward.Claim();
                        
                        // 这里可以添加发放奖励的逻辑
                        Debug.Log("发放活动奖励: " + reward.rewardName + " 数量: " + reward.quantity);
                        
                        SaveEventData();
                        Debug.Log("成功领取活动奖励: " + reward.rewardName);
                    }
                    else
                    {
                        Debug.LogError("条件不足，无法领取奖励");
                    }
                }
                else
                {
                    Debug.LogError("奖励不存在或已禁用");
                }
            }
            else
            {
                Debug.LogError("玩家奖励不存在或已领取");
            }
        }
        else
        {
            Debug.LogError("玩家未参与该活动");
        }
    }
    
    public List<PlayerEvent> GetPlayerEvents(string userID)
    {
        return eventData.system.GetPlayerEventsByUser(userID);
    }
    
    public List<EventParticipation> GetEventParticipations(string userID)
    {
        return eventData.system.GetEventParticipationsByUser(userID);
    }
    
    // 数据持久化
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
            eventData = (EventSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            eventData = new EventSystemDetailedManagerData();
        }
    }
}
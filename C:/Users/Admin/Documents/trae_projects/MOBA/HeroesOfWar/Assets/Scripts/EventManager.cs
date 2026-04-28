using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    
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
    }
    
    private void InitializeDefaultEvents()
    {
        // 新春活动
        Event springEvent = new Event("event_spring", "新春活动", "庆祝新春，参与活动获得丰厚奖励", "Seasonal");
        springEvent.AddTask("task_spring_1", "击杀敌人", "击杀20个敌人", 20, 10);
        springEvent.AddTask("task_spring_2", "赢得比赛", "赢得5场比赛", 5, 20);
        springEvent.AddTask("task_spring_3", "使用技能", "使用技能50次", 50, 15);
        springEvent.AddReward("reward_spring_1", "新春红包", "获得1000金币", 20, "Currency", "gold", 1000);
        springEvent.AddReward("reward_spring_2", "新春礼包", "获得5个生命药水", 40, "Item", "item_health_potion", 5);
        springEvent.AddReward("reward_spring_3", "新春皮肤", "获得关羽新春限定皮肤", 60, "Skin", "skin_guanyu_spring", 1);
        eventData.AddEvent(springEvent);
        
        // 周年庆活动
        Event anniversaryEvent = new Event("event_anniversary", "周年庆活动", "庆祝游戏周年，参与活动获得限定奖励", "Anniversary");
        anniversaryEvent.AddTask("task_anniversary_1", "登录游戏", "连续登录7天", 7, 30);
        anniversaryEvent.AddTask("task_anniversary_2", "参与比赛", "参与10场比赛", 10, 25);
        anniversaryEvent.AddTask("task_anniversary_3", "获得助攻", "获得30次助攻", 30, 20);
        anniversaryEvent.AddReward("reward_anniversary_1", "周年金币", "获得2000金币", 30, "Currency", "gold", 2000);
        anniversaryEvent.AddReward("reward_anniversary_2", "周年礼包", "获得10个法力药水", 60, "Item", "item_mana_potion", 10);
        anniversaryEvent.AddReward("reward_anniversary_3", "周年皮肤", "获得赵云龙胆亮银皮肤", 90, "Skin", "skin_zhaoyun_dragon", 1);
        eventData.AddEvent(anniversaryEvent);
        
        // 周末活动
        Event weekendEvent = new Event("event_weekend", "周末狂欢", "周末双倍经验和金币", "Weekly");
        weekendEvent.AddTask("task_weekend_1", "周末登录", "周末登录游戏", 1, 10);
        weekendEvent.AddTask("task_weekend_2", "周末比赛", "周末参与5场比赛", 5, 15);
        weekendEvent.AddReward("reward_weekend_1", "周末金币", "获得500金币", 10, "Currency", "gold", 500);
        weekendEvent.AddReward("reward_weekend_2", "周末经验", "获得双倍经验", 25, "Buff", "double_exp", 1);
        eventData.AddEvent(weekendEvent);
        
        SaveEventData();
    }
    
    public void UpdateEventTaskProgress(string eventID, string taskID, int progress)
    {
        Event eventData = this.eventData.GetEvent(eventID);
        if (eventData != null && eventData.isActive && !eventData.IsExpired())
        {
            EventTask task = eventData.tasks.Find(t => t.taskID == taskID);
            if (task != null && !task.isCompleted)
            {
                task.AddProgress(progress);
                if (task.isCompleted)
                {
                    EventParticipation participation = GetOrCreateParticipation(eventID);
                    participation.AddPoints(task.rewardPoints);
                    participation.AddCompletedTask(task);
                }
                SaveEventData();
            }
        }
    }
    
    public void ClaimEventReward(string eventID, string rewardID)
    {
        Event eventData = this.eventData.GetEvent(eventID);
        if (eventData != null && eventData.isActive && !eventData.IsExpired())
        {
            EventReward reward = eventData.rewards.Find(r => r.rewardID == rewardID);
            if (reward != null && !reward.isClaimed)
            {
                EventParticipation participation = GetOrCreateParticipation(eventID);
                if (participation.totalPoints >= reward.requiredPoints)
                {
                    reward.Claim();
                    GrantEventReward(reward);
                    participation.AddClaimedReward(reward);
                    SaveEventData();
                }
            }
        }
    }
    
    private void GrantEventReward(EventReward reward)
    {
        switch (reward.rewardType)
        {
            case "Item":
                InventoryManager.Instance.AddItemToInventory(reward.rewardItemID, reward.quantity);
                break;
            case "Currency":
                if (reward.rewardItemID == "gold")
                {
                    ProfileManager.Instance.currentProfile.gold += reward.quantity;
                    ProfileManager.Instance.SaveProfile();
                }
                else if (reward.rewardItemID == "gems")
                {
                    ProfileManager.Instance.currentProfile.gems += reward.quantity;
                    ProfileManager.Instance.SaveProfile();
                }
                break;
            case "Skin":
                SkinManager.Instance.PurchaseSkin(reward.rewardItemID);
                break;
        }
    }
    
    private EventParticipation GetOrCreateParticipation(string eventID)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        EventParticipation participation = eventData.GetParticipation(playerID, eventID);
        
        if (participation == null)
        {
            participation = new EventParticipation("participation_" + System.DateTime.Now.Ticks, playerID, eventID);
            eventData.AddParticipation(participation);
        }
        
        return participation;
    }
    
    public List<Event> GetActiveEvents()
    {
        return eventData.GetActiveEvents();
    }
    
    public Event GetEvent(string eventID)
    {
        return eventData.GetEvent(eventID);
    }
    
    public EventParticipation GetEventParticipation(string eventID)
    {
        return eventData.GetParticipation(ProfileManager.Instance.currentProfile.playerID, eventID);
    }
    
    public void AddEvent(Event eventData)
    {
        this.eventData.AddEvent(eventData);
        SaveEventData();
    }
    
    public void RemoveEvent(string eventID)
    {
        this.eventData.events.RemoveAll(e => e.eventID == eventID);
        SaveEventData();
    }
    
    public void DeactivateExpiredEvents()
    {
        foreach (Event eventData in this.eventData.events)
        {
            if (eventData.IsExpired() && eventData.isActive)
            {
                eventData.Deactivate();
            }
        }
        SaveEventData();
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
            eventData = (EventManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            eventData = new EventManagerData();
        }
    }
}
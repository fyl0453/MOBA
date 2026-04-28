using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HonorBattleOrderSystemDetailedManager : MonoBehaviour
{
    public static HonorBattleOrderSystemDetailedManager Instance { get; private set; }
    
    public HonorBattleOrderSystemDetailedManagerData honorBattleOrderData;
    
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
        LoadHonorBattleOrderData();
        
        if (honorBattleOrderData == null)
        {
            honorBattleOrderData = new HonorBattleOrderSystemDetailedManagerData();
            InitializeDefaultHonorBattleOrderSystem();
        }
    }
    
    private void InitializeDefaultHonorBattleOrderSystem()
    {
        // 荣耀战令
        string startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string endTime = System.DateTime.Now.AddDays(60).ToString("yyyy-MM-dd HH:mm:ss");
        
        HonorBattleOrder order1 = new HonorBattleOrder("order_001", "S1赛季战令", "S1赛季专属战令", "S1", startTime, endTime, 80, 388, 688, "icon_order_s1");
        
        honorBattleOrderData.system.AddHonorBattleOrder(order1);
        
        // 战令等级
        for (int i = 1; i <= 80; i++)
        {
            string levelName = "等级 " + i;
            string levelDescription = "战令等级 " + i;
            int requiredExp = i * 100;
            int totalExp = (i * (i + 1)) / 2 * 100;
            string icon = "icon_level_" + i;
            
            HonorBattleOrderLevel level = new HonorBattleOrderLevel(i, levelName, levelDescription, requiredExp, totalExp, icon);
            honorBattleOrderData.system.AddHonorBattleOrderLevel(level);
        }
        
        // 战令任务
        HonorBattleOrderTask task1 = new HonorBattleOrderTask("task_001", "order_001", "每日登录", "每天登录游戏", "daily", 1, 10, 5, true, startTime, endTime);
        HonorBattleOrderTask task2 = new HonorBattleOrderTask("task_002", "order_001", "完成一场游戏", "完成任意一场游戏", "daily", 1, 15, 8, true, startTime, endTime);
        HonorBattleOrderTask task3 = new HonorBattleOrderTask("task_003", "order_001", "获得一场胜利", "获得任意一场胜利", "daily", 1, 20, 10, true, startTime, endTime);
        HonorBattleOrderTask task4 = new HonorBattleOrderTask("task_004", "order_001", "累计击杀10人", "累计击杀10名敌人", "weekly", 10, 30, 15, true, startTime, endTime);
        HonorBattleOrderTask task5 = new HonorBattleOrderTask("task_005", "order_001", "使用指定英雄", "使用指定英雄完成3场游戏", "weekly", 3, 40, 20, true, startTime, endTime);
        
        honorBattleOrderData.system.AddHonorBattleOrderTask(task1);
        honorBattleOrderData.system.AddHonorBattleOrderTask(task2);
        honorBattleOrderData.system.AddHonorBattleOrderTask(task3);
        honorBattleOrderData.system.AddHonorBattleOrderTask(task4);
        honorBattleOrderData.system.AddHonorBattleOrderTask(task5);
        
        // 战令奖励
        for (int i = 1; i <= 80; i++)
        {
            string rewardID = "reward_" + i.ToString("000");
            string rewardName = "战令奖励 " + i;
            string rewardType = i % 10 == 0 ? "skin" : "currency";
            string rewardValue = i % 10 == 0 ? "skin_" + i : "gold";
            int quantity = i % 10 == 0 ? 1 : 1000 * i;
            string tier = i % 5 == 0 ? "premium" : "both";
            string icon = i % 10 == 0 ? "icon_skin" : "icon_gold";
            
            HonorBattleOrderReward reward = new HonorBattleOrderReward(rewardID, "order_001", rewardName, rewardType, rewardValue, quantity, i, tier, icon);
            honorBattleOrderData.system.AddHonorBattleOrderReward(reward);
            
            // 添加奖励到等级
            HonorBattleOrderLevel level = honorBattleOrderData.system.GetHonorBattleOrderLevel(i);
            if (level != null)
            {
                level.AddReward(rewardID);
            }
        }
        
        // 战令事件
        HonorBattleOrderEvent event1 = new HonorBattleOrderEvent("event_001", "start", "user_001", "order_001", "战令赛季开始");
        HonorBattleOrderEvent event2 = new HonorBattleOrderEvent("event_002", "purchase", "user_001", "order_001", "购买高级战令");
        HonorBattleOrderEvent event3 = new HonorBattleOrderEvent("event_003", "level_up", "user_001", "order_001", "战令等级提升");
        
        honorBattleOrderData.system.AddHonorBattleOrderEvent(event1);
        honorBattleOrderData.system.AddHonorBattleOrderEvent(event2);
        honorBattleOrderData.system.AddHonorBattleOrderEvent(event3);
        
        // 启动战令
        order1.Start();
        
        SaveHonorBattleOrderData();
    }
    
    // 战令管理
    public void CreateHonorBattleOrder(string orderName, string orderDescription, string season, string startTime, string endTime, int maxLevel, int basePrice, int premiumPrice, string icon)
    {
        string orderID = "order_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HonorBattleOrder honorBattleOrder = new HonorBattleOrder(orderID, orderName, orderDescription, season, startTime, endTime, maxLevel, basePrice, premiumPrice, icon);
        honorBattleOrderData.system.AddHonorBattleOrder(honorBattleOrder);
        
        // 创建战令等级
        for (int i = 1; i <= maxLevel; i++)
        {
            string levelName = "等级 " + i;
            string levelDescription = "战令等级 " + i;
            int requiredExp = i * 100;
            int totalExp = (i * (i + 1)) / 2 * 100;
            string levelIcon = "icon_level_" + i;
            
            HonorBattleOrderLevel level = new HonorBattleOrderLevel(i, levelName, levelDescription, requiredExp, totalExp, levelIcon);
            honorBattleOrderData.system.AddHonorBattleOrderLevel(level);
        }
        
        SaveHonorBattleOrderData();
        Debug.Log("成功创建荣耀战令: " + orderName);
    }
    
    public void StartHonorBattleOrder(string orderID)
    {
        HonorBattleOrder honorBattleOrder = honorBattleOrderData.system.GetHonorBattleOrder(orderID);
        if (honorBattleOrder != null && honorBattleOrder.status == "upcoming")
        {
            honorBattleOrder.Start();
            
            // 创建战令事件
            CreateHonorBattleOrderEvent("start", "system", orderID, "战令赛季开始");
            
            SaveHonorBattleOrderData();
            Debug.Log("成功启动荣耀战令: " + honorBattleOrder.orderName);
        }
        else
        {
            Debug.LogError("战令不存在或状态错误");
        }
    }
    
    public void EndHonorBattleOrder(string orderID)
    {
        HonorBattleOrder honorBattleOrder = honorBattleOrderData.system.GetHonorBattleOrder(orderID);
        if (honorBattleOrder != null && honorBattleOrder.status == "active")
        {
            honorBattleOrder.End();
            
            // 创建战令事件
            CreateHonorBattleOrderEvent("end", "system", orderID, "战令赛季结束");
            
            SaveHonorBattleOrderData();
            Debug.Log("成功结束荣耀战令: " + honorBattleOrder.orderName);
        }
        else
        {
            Debug.LogError("战令不存在或状态错误");
        }
    }
    
    public List<HonorBattleOrder> GetActiveHonorBattleOrders()
    {
        return honorBattleOrderData.system.GetHonorBattleOrdersByStatus("active");
    }
    
    // 战令等级管理
    public HonorBattleOrderLevel GetHonorBattleOrderLevel(int level)
    {
        return honorBattleOrderData.system.GetHonorBattleOrderLevel(level);
    }
    
    public int GetMaxHonorBattleOrderLevel()
    {
        return honorBattleOrderData.system.honorBattleOrderLevels.Count;
    }
    
    // 战令任务管理
    public void AddHonorBattleOrderTask(string orderID, string taskName, string taskDescription, string taskType, int requiredProgress, int rewardExp, int rewardPoints, bool isRepeatable, string startTime, string endTime)
    {
        string taskID = "task_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HonorBattleOrderTask honorBattleOrderTask = new HonorBattleOrderTask(taskID, orderID, taskName, taskDescription, taskType, requiredProgress, rewardExp, rewardPoints, isRepeatable, startTime, endTime);
        honorBattleOrderData.system.AddHonorBattleOrderTask(honorBattleOrderTask);
        
        SaveHonorBattleOrderData();
        Debug.Log("成功添加战令任务: " + taskName);
    }
    
    public void UpdateTaskProgress(string taskID, string userID, int progress)
    {
        HonorBattleOrderTask task = honorBattleOrderData.system.GetHonorBattleOrderTask(taskID);
        if (task != null && task.IsActive())
        {
            // 这里可以添加任务进度管理逻辑
            // 例如，创建用户任务进度记录
            
            // 检查是否完成任务
            if (progress >= task.requiredProgress)
            {
                // 发放奖励
                // 这里可以添加发放经验和点数的逻辑
                Debug.Log("任务完成: " + task.taskName + "，获得经验: " + task.rewardExp + "，点数: " + task.rewardPoints);
                
                // 创建战令事件
                CreateHonorBattleOrderEvent("task_complete", userID, task.orderID, "完成任务: " + task.taskName);
                
                // 如果任务可重复，重置进度
                if (task.isRepeatable)
                {
                    // 重置进度逻辑
                }
            }
            
            SaveHonorBattleOrderData();
            Debug.Log("成功更新任务进度: " + task.taskName);
        }
        else
        {
            Debug.LogError("任务不存在或已禁用");
        }
    }
    
    public List<HonorBattleOrderTask> GetHonorBattleOrderTasks(string orderID)
    {
        return honorBattleOrderData.system.GetHonorBattleOrderTasksByOrder(orderID);
    }
    
    public List<HonorBattleOrderTask> GetHonorBattleOrderTasksByType(string taskType)
    {
        return honorBattleOrderData.system.GetHonorBattleOrderTasksByType(taskType);
    }
    
    // 战令奖励管理
    public void AddHonorBattleOrderReward(string orderID, string rewardName, string rewardType, string rewardValue, int quantity, int level, string tier, string icon)
    {
        string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HonorBattleOrderReward honorBattleOrderReward = new HonorBattleOrderReward(rewardID, orderID, rewardName, rewardType, rewardValue, quantity, level, tier, icon);
        honorBattleOrderData.system.AddHonorBattleOrderReward(honorBattleOrderReward);
        
        // 添加奖励到等级
        HonorBattleOrderLevel honorBattleOrderLevel = honorBattleOrderData.system.GetHonorBattleOrderLevel(level);
        if (honorBattleOrderLevel != null)
        {
            honorBattleOrderLevel.AddReward(rewardID);
        }
        
        SaveHonorBattleOrderData();
        Debug.Log("成功添加战令奖励: " + rewardName);
    }
    
    public void ClaimHonorBattleOrderReward(string rewardID, string userID, string tier)
    {
        HonorBattleOrderReward reward = honorBattleOrderData.system.GetHonorBattleOrderReward(rewardID);
        if (reward != null && reward.isEnabled)
        {
            // 这里可以添加检查用户等级和战令等级的逻辑
            int userLevel = 1; // 假设用户等级
            
            if (reward.IsAvailable(userLevel, tier))
            {
                // 这里可以添加发放奖励的逻辑
                Debug.Log("发放战令奖励: " + reward.rewardName + " 数量: " + reward.quantity);
                
                // 创建战令事件
                CreateHonorBattleOrderEvent("reward_claim", userID, reward.orderID, "领取奖励: " + reward.rewardName);
                
                SaveHonorBattleOrderData();
                Debug.Log("成功领取战令奖励: " + reward.rewardName);
            }
            else
            {
                Debug.LogError("奖励不可领取");
            }
        }
        else
        {
            Debug.LogError("奖励不存在或已禁用");
        }
    }
    
    public List<HonorBattleOrderReward> GetHonorBattleOrderRewardsByLevel(int level)
    {
        return honorBattleOrderData.system.GetHonorBattleOrderRewardsByLevel(level);
    }
    
    // 战令事件管理
    public string CreateHonorBattleOrderEvent(string eventType, string userID, string orderID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HonorBattleOrderEvent honorBattleOrderEvent = new HonorBattleOrderEvent(eventID, eventType, userID, orderID, description);
        honorBattleOrderData.system.AddHonorBattleOrderEvent(honorBattleOrderEvent);
        SaveHonorBattleOrderData();
        Debug.Log("成功创建战令事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        HonorBattleOrderEvent honorBattleOrderEvent = honorBattleOrderData.system.GetHonorBattleOrderEvent(eventID);
        if (honorBattleOrderEvent != null)
        {
            honorBattleOrderEvent.MarkAsCompleted();
            SaveHonorBattleOrderData();
            Debug.Log("成功标记战令事件为完成");
        }
        else
        {
            Debug.LogError("战令事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        HonorBattleOrderEvent honorBattleOrderEvent = honorBattleOrderData.system.GetHonorBattleOrderEvent(eventID);
        if (honorBattleOrderEvent != null)
        {
            honorBattleOrderEvent.MarkAsFailed();
            SaveHonorBattleOrderData();
            Debug.Log("成功标记战令事件为失败");
        }
        else
        {
            Debug.LogError("战令事件不存在: " + eventID);
        }
    }
    
    public List<HonorBattleOrderEvent> GetUserEvents(string userID)
    {
        return honorBattleOrderData.system.GetHonorBattleOrderEventsByUser(userID);
    }
    
    // 数据持久化
    public void SaveHonorBattleOrderData()
    {
        string path = Application.dataPath + "/Data/honor_battle_order_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, honorBattleOrderData);
        stream.Close();
    }
    
    public void LoadHonorBattleOrderData()
    {
        string path = Application.dataPath + "/Data/honor_battle_order_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            honorBattleOrderData = (HonorBattleOrderSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            honorBattleOrderData = new HonorBattleOrderSystemDetailedManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }
    
    public TaskList taskList;
    public List<TaskCategory> taskCategories;
    
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
        LoadTaskData();
        LoadTaskCategories();
        
        if (taskList == null)
        {
            taskList = new TaskList(ProfileManager.Instance.currentProfile.playerID);
            InitializeDefaultTasks();
        }
        
        if (taskCategories.Count == 0)
        {
            InitializeDefaultCategories();
        }
    }
    
    private void InitializeDefaultTasks()
    {
        // 日常任务
        Task dailyKill = new Task("task_daily_kill", "日常击杀", "击杀10个敌人", "Daily", 10);
        dailyKill.AddReward("Currency", "gold", 100);
        dailyKill.AddReward("Item", "item_health_potion", 2);
        taskList.AddTask(dailyKill);
        
        Task dailyWin = new Task("task_daily_win", "日常胜利", "赢得2场比赛", "Daily", 2);
        dailyWin.AddReward("Currency", "gold", 200);
        dailyWin.AddReward("Item", "item_mana_potion", 2);
        taskList.AddTask(dailyWin);
        
        Task dailyAssist = new Task("task_daily_assist", "日常助攻", "获得5次助攻", "Daily", 5);
        dailyAssist.AddReward("Currency", "gold", 150);
        dailyAssist.AddReward("Item", "item_attack_potion", 1);
        taskList.AddTask(dailyAssist);
        
        // 新手任务
        Task newPlayerKill = new Task("task_new_player_kill", "初露锋芒", "击杀5个敌人", "NewPlayer", 5);
        newPlayerKill.AddReward("Currency", "gold", 500);
        newPlayerKill.AddReward("Item", "item_health_potion", 5);
        taskList.AddTask(newPlayerKill);
        
        Task newPlayerWin = new Task("task_new_player_win", "首战告捷", "赢得1场比赛", "NewPlayer", 1);
        newPlayerWin.AddReward("Currency", "gold", 800);
        newPlayerWin.AddReward("Item", "item_mana_potion", 5);
        taskList.AddTask(newPlayerWin);
        
        Task newPlayerHero = new Task("task_new_player_hero", "英雄解锁", "解锁3个英雄", "NewPlayer", 3);
        newPlayerHero.AddReward("Currency", "gems", 100);
        newPlayerHero.AddReward("Item", "item_defense_potion", 3);
        taskList.AddTask(newPlayerHero);
        
        // 周常任务
        Task weeklyKill = new Task("task_weekly_kill", "周常击杀", "击杀50个敌人", "Weekly", 50);
        weeklyKill.AddReward("Currency", "gold", 1000);
        weeklyKill.AddReward("Item", "item_speed_potion", 3);
        taskList.AddTask(weeklyKill);
        
        Task weeklyWin = new Task("task_weekly_win", "周常胜利", "赢得10场比赛", "Weekly", 10);
        weeklyWin.AddReward("Currency", "gold", 2000);
        weeklyWin.AddReward("Item", "item_teleport_scroll", 5);
        taskList.AddTask(weeklyWin);
        
        // 活动任务
        Task eventTask = new Task("task_event_spring", "新春活动", "完成5场比赛", "Event", 5);
        eventTask.AddReward("Currency", "gems", 200);
        eventTask.AddReward("Item", "item_vision_ward", 10);
        taskList.AddTask(eventTask);
        
        SaveTaskData();
    }
    
    private void InitializeDefaultCategories()
    {
        TaskCategory dailyCategory = new TaskCategory("category_daily", "日常任务", "每天刷新的任务");
        dailyCategory.AddTask("task_daily_kill");
        dailyCategory.AddTask("task_daily_win");
        dailyCategory.AddTask("task_daily_assist");
        taskCategories.Add(dailyCategory);
        
        TaskCategory newPlayerCategory = new TaskCategory("category_new_player", "新手任务", "新玩家专属任务");
        newPlayerCategory.AddTask("task_new_player_kill");
        newPlayerCategory.AddTask("task_new_player_win");
        newPlayerCategory.AddTask("task_new_player_hero");
        taskCategories.Add(newPlayerCategory);
        
        TaskCategory weeklyCategory = new TaskCategory("category_weekly", "周常任务", "每周刷新的任务");
        weeklyCategory.AddTask("task_weekly_kill");
        weeklyCategory.AddTask("task_weekly_win");
        taskCategories.Add(weeklyCategory);
        
        TaskCategory eventCategory = new TaskCategory("category_event", "活动任务", "限时活动任务");
        eventCategory.AddTask("task_event_spring");
        taskCategories.Add(eventCategory);
        
        SaveTaskCategories();
    }
    
    public void UpdateTaskProgress(string taskID, int progress)
    {
        taskList.UpdateTaskProgress(taskID, progress);
        SaveTaskData();
    }
    
    public void ClaimTaskRewards(string taskID)
    {
        Task task = taskList.GetTask(taskID);
        if (task != null && task.isCompleted && !task.isClaimed)
        {
            foreach (TaskReward reward in task.rewards)
            {
                GrantReward(reward);
            }
            task.ClaimRewards();
            SaveTaskData();
        }
    }
    
    private void GrantReward(TaskReward reward)
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
        }
    }
    
    public void AddTask(Task task)
    {
        taskList.AddTask(task);
        SaveTaskData();
    }
    
    public void RemoveTask(string taskID)
    {
        taskList.RemoveTask(taskID);
        SaveTaskData();
    }
    
    public List<Task> GetActiveTasks()
    {
        return taskList.GetActiveTasks();
    }
    
    public List<Task> GetCompletedTasks()
    {
        return taskList.GetCompletedTasks();
    }
    
    public List<Task> GetTasksByType(string type)
    {
        return taskList.GetTasksByType(type);
    }
    
    public Task GetTask(string taskID)
    {
        return taskList.GetTask(taskID);
    }
    
    public List<TaskCategory> GetTaskCategories()
    {
        return taskCategories;
    }
    
    public TaskCategory GetTaskCategory(string categoryID)
    {
        return taskCategories.Find(c => c.categoryID == categoryID);
    }
    
    public void RefreshDailyTasks()
    {
        List<Task> dailyTasks = taskList.GetTasksByType("Daily");
        foreach (Task task in dailyTasks)
        {
            task.currentProgress = 0;
            task.isCompleted = false;
            task.isClaimed = false;
        }
        SaveTaskData();
    }
    
    public void RefreshWeeklyTasks()
    {
        List<Task> weeklyTasks = taskList.GetTasksByType("Weekly");
        foreach (Task task in weeklyTasks)
        {
            task.currentProgress = 0;
            task.isCompleted = false;
            task.isClaimed = false;
        }
        SaveTaskData();
    }
    
    public void RemoveExpiredTasks()
    {
        taskList.RemoveExpiredTasks();
        SaveTaskData();
    }
    
    public void SaveTaskData()
    {
        string path = Application.dataPath + "/Data/task_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, taskList);
        stream.Close();
    }
    
    public void LoadTaskData()
    {
        string path = Application.dataPath + "/Data/task_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            taskList = (TaskList)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            taskList = new TaskList(ProfileManager.Instance.currentProfile.playerID);
        }
    }
    
    public void SaveTaskCategories()
    {
        string path = Application.dataPath + "/Data/task_categories.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, taskCategories);
        stream.Close();
    }
    
    public void LoadTaskCategories()
    {
        string path = Application.dataPath + "/Data/task_categories.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            taskCategories = (List<TaskCategory>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            taskCategories = new List<TaskCategory>();
        }
    }
}
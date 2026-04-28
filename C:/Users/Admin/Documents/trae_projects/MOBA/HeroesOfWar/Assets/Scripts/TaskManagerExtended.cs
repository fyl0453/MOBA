using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TaskManagerExtended : MonoBehaviour
{
    public static TaskManagerExtended Instance { get; private set; }
    
    public TaskManagerExtendedData taskData;
    public TaskProgress playerProgress;
    
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
        
        if (taskData == null)
        {
            taskData = new TaskManagerExtendedData();
            InitializeDefaultTasks();
        }
        
        EnsurePlayerProgress();
        CheckTaskExpiry();
    }
    
    private void EnsurePlayerProgress()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        playerProgress = new TaskProgress(playerID);
    }
    
    private void InitializeDefaultTasks()
    {
        // 每日任务
        TaskCategory dailyCategory = new TaskCategory("daily", "每日任务", "每天刷新的任务");
        
        ExtendedTask dailyLogin = new ExtendedTask("task_daily_login", "每日登录", "每天登录游戏", "login", 1);
        dailyLogin.AddReward(new TaskReward("reward_gold", "Currency", "gold", 100, "100金币"));
        dailyLogin.isTimeLimited = true;
        dailyLogin.endTime = System.DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        dailyCategory.AddTask(dailyLogin);
        taskData.AddTask(dailyLogin);
        
        ExtendedTask dailyMatch = new ExtendedTask("task_daily_match", "参与比赛", "参与1场比赛", "match", 1);
        dailyMatch.AddReward(new TaskReward("reward_gold", "Currency", "gold", 150, "150金币"));
        dailyMatch.isTimeLimited = true;
        dailyMatch.endTime = System.DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        dailyCategory.AddTask(dailyMatch);
        taskData.AddTask(dailyMatch);
        
        ExtendedTask dailyWin = new ExtendedTask("task_daily_win", "赢得比赛", "赢得1场比赛", "win", 1);
        dailyWin.AddReward(new TaskReward("reward_gold", "Currency", "gold", 200, "200金币"));
        dailyWin.isTimeLimited = true;
        dailyWin.endTime = System.DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        dailyCategory.AddTask(dailyWin);
        taskData.AddTask(dailyWin);
        
        taskData.AddCategory(dailyCategory);
        
        // 周常任务
        TaskCategory weeklyCategory = new TaskCategory("weekly", "周常任务", "每周刷新的任务");
        
        ExtendedTask weeklyMatches = new ExtendedTask("task_weekly_matches", "参与比赛", "参与5场比赛", "match", 5);
        weeklyMatches.AddReward(new TaskReward("reward_gems", "Currency", "gems", 50, "50钻石"));
        weeklyMatches.isTimeLimited = true;
        weeklyMatches.endTime = System.DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
        weeklyCategory.AddTask(weeklyMatches);
        taskData.AddTask(weeklyMatches);
        
        ExtendedTask weeklyWins = new ExtendedTask("task_weekly_wins", "赢得比赛", "赢得3场比赛", "win", 3);
        weeklyWins.AddReward(new TaskReward("reward_gems", "Currency", "gems", 100, "100钻石"));
        weeklyWins.isTimeLimited = true;
        weeklyWins.endTime = System.DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
        weeklyCategory.AddTask(weeklyWins);
        taskData.AddTask(weeklyWins);
        
        taskData.AddCategory(weeklyCategory);
        
        // 新手任务
        TaskCategory newbieCategory = new TaskCategory("newbie", "新手任务", "帮助新手熟悉游戏");
        
        ExtendedTask newbieFirstMatch = new ExtendedTask("task_newbie_first_match", "第一场比赛", "参与第一场比赛", "match", 1);
        newbieFirstMatch.AddReward(new TaskReward("reward_gold", "Currency", "gold", 500, "500金币"));
        newbieFirstMatch.AddReward(new TaskReward("reward_hero", "Hero", "hero_guanyu", 1, "关羽"));
        newbieCategory.AddTask(newbieFirstMatch);
        taskData.AddTask(newbieFirstMatch);
        
        ExtendedTask newbieFirstWin = new ExtendedTask("task_newbie_first_win", "第一场胜利", "赢得第一场比赛", "win", 1);
        newbieFirstWin.AddReward(new TaskReward("reward_gold", "Currency", "gold", 1000, "1000金币"));
        newbieFirstWin.AddReward(new TaskReward("reward_skin", "Skin", "skin_guanyu_spring", 1, "关羽新春皮肤"));
        newbieCategory.AddTask(newbieFirstWin);
        taskData.AddTask(newbieFirstWin);
        
        taskData.AddCategory(newbieCategory);
        
        // 活动任务
        TaskCategory eventCategory = new TaskCategory("event", "活动任务", "限时活动任务");
        
        ExtendedTask eventTask = new ExtendedTask("task_event_spring", "新春活动", "参与1场新春活动比赛", "event", 1);
        eventTask.AddReward(new TaskReward("reward_gold", "Currency", "gold", 2000, "2000金币"));
        eventTask.AddReward(new TaskReward("reward_skin", "Skin", "skin_guanyu_spring", 1, "关羽新春皮肤"));
        eventTask.isTimeLimited = true;
        eventTask.endTime = System.DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");
        eventCategory.AddTask(eventTask);
        taskData.AddTask(eventTask);
        
        taskData.AddCategory(eventCategory);
        
        SaveTaskData();
    }
    
    public void UpdateTaskProgress(string taskID, int progress)
    {
        ExtendedTask task = taskData.GetTask(taskID);
        if (task != null && !task.isCompleted && !task.IsExpired())
        {
            task.AddProgress(progress);
            playerProgress.UpdateProgress(taskID, task.currentProgress);
            
            if (task.isCompleted)
            {
                taskData.MoveToCompleted(task);
                playerProgress.MarkAsCompleted(taskID);
            }
            
            SaveTaskData();
        }
    }
    
    public void ClaimTaskReward(string taskID)
    {
        ExtendedTask task = taskData.GetTask(taskID);
        if (task != null && task.isCompleted && !task.isClaimed)
        {
            task.Claim();
            playerProgress.MarkAsClaimed(taskID);
            
            foreach (TaskReward reward in task.rewards)
            {
                GrantTaskReward(reward);
            }
            
            SaveTaskData();
        }
    }
    
    private void GrantTaskReward(TaskReward reward)
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
            case "Hero":
                // 这里需要添加解锁英雄的逻辑
                break;
        }
    }
    
    private void CheckTaskExpiry()
    {
        List<ExtendedTask> expiredTasks = new List<ExtendedTask>();
        
        foreach (ExtendedTask task in taskData.activeTasks)
        {
            if (task.IsExpired())
            {
                expiredTasks.Add(task);
            }
        }
        
        foreach (ExtendedTask task in expiredTasks)
        {
            taskData.MoveToExpired(task);
        }
        
        if (expiredTasks.Count > 0)
        {
            SaveTaskData();
        }
    }
    
    public void RefreshDailyTasks()
    {
        TaskCategory dailyCategory = taskData.GetCategory("daily");
        if (dailyCategory != null)
        {
            foreach (ExtendedTask task in dailyCategory.tasks)
            {
                if (task.IsExpired())
                {
                    task.currentProgress = 0;
                    task.isCompleted = false;
                    task.isClaimed = false;
                    task.startTime = System.DateTime.Now.ToString("yyyy-MM-dd");
                    task.endTime = System.DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                    
                    if (!taskData.activeTasks.Contains(task))
                    {
                        taskData.activeTasks.Add(task);
                    }
                }
            }
            SaveTaskData();
        }
    }
    
    public void RefreshWeeklyTasks()
    {
        TaskCategory weeklyCategory = taskData.GetCategory("weekly");
        if (weeklyCategory != null)
        {
            foreach (ExtendedTask task in weeklyCategory.tasks)
            {
                if (task.IsExpired())
                {
                    task.currentProgress = 0;
                    task.isCompleted = false;
                    task.isClaimed = false;
                    task.startTime = System.DateTime.Now.ToString("yyyy-MM-dd");
                    task.endTime = System.DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
                    
                    if (!taskData.activeTasks.Contains(task))
                    {
                        taskData.activeTasks.Add(task);
                    }
                }
            }
            SaveTaskData();
        }
    }
    
    public List<ExtendedTask> GetActiveTasks()
    {
        CheckTaskExpiry();
        return taskData.activeTasks;
    }
    
    public List<ExtendedTask> GetCompletedTasks()
    {
        return taskData.completedTasks;
    }
    
    public List<ExtendedTask> GetExpiredTasks()
    {
        return taskData.expiredTasks;
    }
    
    public List<ExtendedTask> GetTasksByCategory(string categoryID)
    {
        return taskData.GetTasksByCategory(categoryID);
    }
    
    public List<ExtendedTask> GetTasksByType(string type)
    {
        return taskData.GetTasksByType(type);
    }
    
    public ExtendedTask GetTask(string taskID)
    {
        return taskData.GetTask(taskID);
    }
    
    public TaskProgress GetPlayerProgress()
    {
        return playerProgress;
    }
    
    public void SaveTaskData()
    {
        string path = Application.dataPath + "/Data/task_extended_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, taskData);
        stream.Close();
        
        // 保存玩家进度
        string progressPath = Application.dataPath + "/Data/task_progress_data.dat";
        FileStream progressStream = new FileStream(progressPath, FileMode.Create);
        formatter.Serialize(progressStream, playerProgress);
        progressStream.Close();
    }
    
    public void LoadTaskData()
    {
        string path = Application.dataPath + "/Data/task_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            taskData = (TaskManagerExtendedData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            taskData = new TaskManagerExtendedData();
        }
        
        // 加载玩家进度
        string progressPath = Application.dataPath + "/Data/task_progress_data.dat";
        if (File.Exists(progressPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream progressStream = new FileStream(progressPath, FileMode.Open);
            playerProgress = (TaskProgress)formatter.Deserialize(progressStream);
            progressStream.Close();
        }
        else
        {
            playerProgress = new TaskProgress(ProfileManager.Instance.currentProfile.playerID);
        }
    }
}
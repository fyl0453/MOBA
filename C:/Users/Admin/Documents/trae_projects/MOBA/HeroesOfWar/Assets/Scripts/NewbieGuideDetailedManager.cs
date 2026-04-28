using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class NewbieGuideDetailedManager : MonoBehaviour
{
    public static NewbieGuideDetailedManager Instance { get; private set; }
    
    public NewbieGuideDetailedManagerData newbieGuideData;
    
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
        LoadNewbieGuideData();
        
        if (newbieGuideData == null)
        {
            newbieGuideData = new NewbieGuideDetailedManagerData();
            InitializeDefaultNewbieGuide();
        }
    }
    
    private void InitializeDefaultNewbieGuide()
    {
        // 引导模块
        GuideModule module1 = new GuideModule("module_001", "基础操作", "学习游戏的基本操作", 1);
        GuideModule module2 = new GuideModule("module_002", "英雄选择", "学习如何选择和使用英雄", 2, "module_001");
        GuideModule module3 = new GuideModule("module_003", "战斗技巧", "学习基本的战斗技巧", 3, "module_002");
        GuideModule module4 = new GuideModule("module_004", "装备系统", "学习装备系统的使用", 4, "module_003");
        GuideModule module5 = new GuideModule("module_005", "团队合作", "学习团队合作技巧", 5, "module_004");
        
        newbieGuideData.system.AddGuideModule(module1);
        newbieGuideData.system.AddGuideModule(module2);
        newbieGuideData.system.AddGuideModule(module3);
        newbieGuideData.system.AddGuideModule(module4);
        newbieGuideData.system.AddGuideModule(module5);
        
        // 引导任务
        GuideTask task1_1 = new GuideTask("task_001", "module_001", "移动英雄", "使用虚拟摇杆移动英雄", "move", 1);
        GuideTask task1_2 = new GuideTask("task_002", "module_001", "释放技能", "学习释放英雄技能", "skill", 1);
        GuideTask task1_3 = new GuideTask("task_003", "module_001", "普通攻击", "学习使用普通攻击", "attack", 1);
        
        GuideTask task2_1 = new GuideTask("task_004", "module_002", "选择英雄", "在英雄列表中选择一个英雄", "select_hero", 1);
        GuideTask task2_2 = new GuideTask("task_005", "module_002", "了解英雄技能", "查看英雄的技能介绍", "hero_skills", 1);
        
        GuideTask task3_1 = new GuideTask("task_006", "module_003", "击杀小兵", "击杀10个小兵", "kill_minion", 10);
        GuideTask task3_2 = new GuideTask("task_007", "module_003", "击杀野怪", "击杀5个野怪", "kill_monster", 5);
        GuideTask task3_3 = new GuideTask("task_008", "module_003", "击杀英雄", "击杀1个敌方英雄", "kill_hero", 1);
        
        GuideTask task4_1 = new GuideTask("task_009", "module_004", "购买装备", "在商店购买一件装备", "buy_equipment", 1);
        GuideTask task4_2 = new GuideTask("task_010", "module_004", "升级装备", "升级一件装备", "upgrade_equipment", 1);
        
        GuideTask task5_1 = new GuideTask("task_011", "module_005", "组队游戏", "和队友一起完成一局游戏", "team_game", 1);
        GuideTask task5_2 = new GuideTask("task_012", "module_005", "推塔", "推倒敌方防御塔", "destroy_turret", 1);
        
        newbieGuideData.system.AddGuideTask(task1_1);
        newbieGuideData.system.AddGuideTask(task1_2);
        newbieGuideData.system.AddGuideTask(task1_3);
        newbieGuideData.system.AddGuideTask(task2_1);
        newbieGuideData.system.AddGuideTask(task2_2);
        newbieGuideData.system.AddGuideTask(task3_1);
        newbieGuideData.system.AddGuideTask(task3_2);
        newbieGuideData.system.AddGuideTask(task3_3);
        newbieGuideData.system.AddGuideTask(task4_1);
        newbieGuideData.system.AddGuideTask(task4_2);
        newbieGuideData.system.AddGuideTask(task5_1);
        newbieGuideData.system.AddGuideTask(task5_2);
        
        // 添加任务到模块
        module1.AddTask("task_001");
        module1.AddTask("task_002");
        module1.AddTask("task_003");
        module2.AddTask("task_004");
        module2.AddTask("task_005");
        module3.AddTask("task_006");
        module3.AddTask("task_007");
        module3.AddTask("task_008");
        module4.AddTask("task_009");
        module4.AddTask("task_010");
        module5.AddTask("task_011");
        module5.AddTask("task_012");
        
        // 引导奖励
        GuideReward reward1 = new GuideReward("reward_001", "task_001", "金币", "gold", "基础操作奖励", 100);
        GuideReward reward2 = new GuideReward("reward_002", "task_002", "经验值", "exp", "技能释放奖励", 50);
        GuideReward reward3 = new GuideReward("reward_003", "task_003", "英雄碎片", "hero_fragment", "普通攻击奖励", 1);
        GuideReward reward4 = new GuideReward("reward_004", "task_004", "皮肤体验卡", "skin_trial", "英雄选择奖励", 1);
        GuideReward reward5 = new GuideReward("reward_005", "task_005", "钻石", "diamond", "英雄技能奖励", 10);
        GuideReward reward6 = new GuideReward("reward_006", "task_006", "金币", "gold", "击杀小兵奖励", 200);
        GuideReward reward7 = new GuideReward("reward_007", "task_007", "经验值", "exp", "击杀野怪奖励", 100);
        GuideReward reward8 = new GuideReward("reward_008", "task_008", "英雄碎片", "hero_fragment", "击杀英雄奖励", 2);
        GuideReward reward9 = new GuideReward("reward_009", "task_009", "装备碎片", "equipment_fragment", "购买装备奖励", 1);
        GuideReward reward10 = new GuideReward("reward_010", "task_010", "钻石", "diamond", "升级装备奖励", 15);
        GuideReward reward11 = new GuideReward("reward_011", "task_011", "金币", "gold", "组队游戏奖励", 300);
        GuideReward reward12 = new GuideReward("reward_012", "task_012", "皮肤碎片", "skin_fragment", "推塔奖励", 1);
        
        newbieGuideData.system.AddGuideReward(reward1);
        newbieGuideData.system.AddGuideReward(reward2);
        newbieGuideData.system.AddGuideReward(reward3);
        newbieGuideData.system.AddGuideReward(reward4);
        newbieGuideData.system.AddGuideReward(reward5);
        newbieGuideData.system.AddGuideReward(reward6);
        newbieGuideData.system.AddGuideReward(reward7);
        newbieGuideData.system.AddGuideReward(reward8);
        newbieGuideData.system.AddGuideReward(reward9);
        newbieGuideData.system.AddGuideReward(reward10);
        newbieGuideData.system.AddGuideReward(reward11);
        newbieGuideData.system.AddGuideReward(reward12);
        
        // 添加奖励到任务
        task1_1.AddReward("reward_001");
        task1_2.AddReward("reward_002");
        task1_3.AddReward("reward_003");
        task2_1.AddReward("reward_004");
        task2_2.AddReward("reward_005");
        task3_1.AddReward("reward_006");
        task3_2.AddReward("reward_007");
        task3_3.AddReward("reward_008");
        task4_1.AddReward("reward_009");
        task4_2.AddReward("reward_010");
        task5_1.AddReward("reward_011");
        task5_2.AddReward("reward_012");
        
        SaveNewbieGuideData();
    }
    
    // 模块管理
    public void AddGuideModule(string name, string desc, int order, string prerequisiteID = "")
    {
        string moduleID = "module_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuideModule module = new GuideModule(moduleID, name, desc, order, prerequisiteID);
        newbieGuideData.system.AddGuideModule(module);
        SaveNewbieGuideData();
        Debug.Log("成功添加引导模块: " + name);
    }
    
    public void UpdateGuideModule(string moduleID, string name, string desc, int order)
    {
        GuideModule module = newbieGuideData.system.GetGuideModule(moduleID);
        if (module != null)
        {
            module.UpdateModule(name, desc, order);
            SaveNewbieGuideData();
            Debug.Log("成功更新引导模块: " + name);
        }
        else
        {
            Debug.LogError("引导模块不存在: " + moduleID);
        }
    }
    
    public void DeleteGuideModule(string moduleID)
    {
        GuideModule module = newbieGuideData.system.GetGuideModule(moduleID);
        if (module != null)
        {
            // 删除相关任务
            List<GuideTask> tasksToDelete = newbieGuideData.system.GetTasksByModule(moduleID);
            foreach (GuideTask task in tasksToDelete)
            {
                // 删除相关奖励
                List<GuideReward> rewardsToDelete = newbieGuideData.system.GetRewardsByTask(task.taskID);
                foreach (GuideReward reward in rewardsToDelete)
                {
                    newbieGuideData.system.guideRewards.Remove(reward);
                }
                newbieGuideData.system.guideTasks.Remove(task);
            }
            
            newbieGuideData.system.guideModules.Remove(module);
            SaveNewbieGuideData();
            Debug.Log("成功删除引导模块: " + moduleID);
        }
        else
        {
            Debug.LogError("引导模块不存在: " + moduleID);
        }
    }
    
    public List<GuideModule> GetAllGuideModules()
    {
        return newbieGuideData.system.GetAllGuideModules();
    }
    
    // 任务管理
    public void AddGuideTask(string moduleID, string name, string desc, string type, int targetProgress, string prerequisiteTaskID = "")
    {
        string taskID = "task_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuideTask task = new GuideTask(taskID, moduleID, name, desc, type, targetProgress, prerequisiteTaskID);
        newbieGuideData.system.AddGuideTask(task);
        
        GuideModule module = newbieGuideData.system.GetGuideModule(moduleID);
        if (module != null)
        {
            module.AddTask(taskID);
        }
        
        SaveNewbieGuideData();
        Debug.Log("成功添加引导任务: " + name);
    }
    
    public void UpdateGuideTask(string taskID, string name, string desc, int targetProgress)
    {
        GuideTask task = newbieGuideData.system.GetGuideTask(taskID);
        if (task != null)
        {
            task.UpdateTask(name, desc, targetProgress);
            SaveNewbieGuideData();
            Debug.Log("成功更新引导任务: " + name);
        }
        else
        {
            Debug.LogError("引导任务不存在: " + taskID);
        }
    }
    
    public void DeleteGuideTask(string taskID)
    {
        GuideTask task = newbieGuideData.system.GetGuideTask(taskID);
        if (task != null)
        {
            // 删除相关奖励
            List<GuideReward> rewardsToDelete = newbieGuideData.system.GetRewardsByTask(taskID);
            foreach (GuideReward reward in rewardsToDelete)
            {
                newbieGuideData.system.guideRewards.Remove(reward);
            }
            
            // 从模块中移除任务
            GuideModule module = newbieGuideData.system.GetGuideModule(task.moduleID);
            if (module != null)
            {
                module.taskIDs.Remove(taskID);
            }
            
            newbieGuideData.system.guideTasks.Remove(task);
            SaveNewbieGuideData();
            Debug.Log("成功删除引导任务: " + taskID);
        }
        else
        {
            Debug.LogError("引导任务不存在: " + taskID);
        }
    }
    
    public List<GuideTask> GetTasksByModule(string moduleID)
    {
        return newbieGuideData.system.GetTasksByModule(moduleID);
    }
    
    // 奖励管理
    public void AddGuideReward(string taskID, string name, string type, string desc, int quantity)
    {
        string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuideReward reward = new GuideReward(rewardID, taskID, name, type, desc, quantity);
        newbieGuideData.system.AddGuideReward(reward);
        
        GuideTask task = newbieGuideData.system.GetGuideTask(taskID);
        if (task != null)
        {
            task.AddReward(rewardID);
        }
        
        SaveNewbieGuideData();
        Debug.Log("成功添加引导奖励: " + name);
    }
    
    public void UpdateGuideReward(string rewardID, string name, string desc, int quantity)
    {
        GuideReward reward = newbieGuideData.system.GetGuideReward(rewardID);
        if (reward != null)
        {
            reward.UpdateReward(name, desc, quantity);
            SaveNewbieGuideData();
            Debug.Log("成功更新引导奖励: " + name);
        }
        else
        {
            Debug.LogError("引导奖励不存在: " + rewardID);
        }
    }
    
    public void DeleteGuideReward(string rewardID)
    {
        GuideReward reward = newbieGuideData.system.GetGuideReward(rewardID);
        if (reward != null)
        {
            // 从任务中移除奖励
            GuideTask task = newbieGuideData.system.GetGuideTask(reward.taskID);
            if (task != null)
            {
                task.rewardIDs.Remove(rewardID);
            }
            
            newbieGuideData.system.guideRewards.Remove(reward);
            SaveNewbieGuideData();
            Debug.Log("成功删除引导奖励: " + rewardID);
        }
        else
        {
            Debug.LogError("引导奖励不存在: " + rewardID);
        }
    }
    
    // 玩家进度管理
    public void InitializePlayerProgress(string playerID)
    {
        PlayerGuideProgress existingProgress = newbieGuideData.system.GetPlayerProgress(playerID);
        if (existingProgress == null)
        {
            PlayerGuideProgress progress = new PlayerGuideProgress(playerID);
            
            // 初始化模块进度
            foreach (GuideModule module in newbieGuideData.system.GetAllGuideModules())
            {
                ModuleProgress moduleProgress = new ModuleProgress(module.moduleID, module.moduleName, module.taskIDs.Count);
                progress.AddModuleProgress(moduleProgress);
            }
            
            // 初始化任务进度
            foreach (GuideTask task in newbieGuideData.system.guideTasks)
            {
                TaskProgress taskProgress = new TaskProgress(task.taskID, task.taskName, task.targetProgress);
                progress.AddTaskProgress(taskProgress);
            }
            
            // 初始化奖励进度
            foreach (GuideReward reward in newbieGuideData.system.guideRewards)
            {
                RewardProgress rewardProgress = new RewardProgress(reward.rewardID, reward.rewardName);
                progress.AddRewardProgress(rewardProgress);
            }
            
            newbieGuideData.system.AddPlayerProgress(progress);
            SaveNewbieGuideData();
            Debug.Log("成功初始化玩家引导进度: " + playerID);
        }
        else
        {
            Debug.Log("玩家引导进度已存在: " + playerID);
        }
    }
    
    public void UpdateTaskProgress(string playerID, string taskID, int progress)
    {
        PlayerGuideProgress playerProgress = newbieGuideData.system.GetPlayerProgress(playerID);
        if (playerProgress != null)
        {
            TaskProgress taskProgress = playerProgress.taskProgresses.Find(t => t.taskID == taskID);
            if (taskProgress != null)
            {
                string oldStatus = taskProgress.status;
                taskProgress.UpdateProgress(progress);
                
                if (oldStatus != "Completed" && taskProgress.status == "Completed")
                {
                    playerProgress.IncrementCompletedTasks();
                    
                    // 更新模块进度
                    GuideTask task = newbieGuideData.system.GetGuideTask(taskID);
                    if (task != null)
                    {
                        ModuleProgress moduleProgress = playerProgress.moduleProgresses.Find(m => m.moduleID == task.moduleID);
                        if (moduleProgress != null)
                        {
                            moduleProgress.CompleteTask();
                        }
                    }
                }
                
                playerProgress.UpdateLastUpdateDate();
                SaveNewbieGuideData();
                Debug.Log("成功更新任务进度: " + taskID);
            }
            else
            {
                Debug.LogError("任务进度不存在: " + taskID);
            }
        }
        else
        {
            Debug.LogError("玩家进度不存在: " + playerID);
        }
    }
    
    public void ClaimReward(string playerID, string rewardID)
    {
        PlayerGuideProgress playerProgress = newbieGuideData.system.GetPlayerProgress(playerID);
        if (playerProgress != null)
        {
            RewardProgress rewardProgress = playerProgress.rewardProgresses.Find(r => r.rewardID == rewardID);
            if (rewardProgress != null)
            {
                if (rewardProgress.status == "Available")
                {
                    rewardProgress.Claim();
                    playerProgress.UpdateLastUpdateDate();
                    SaveNewbieGuideData();
                    Debug.Log("成功领取奖励: " + rewardID);
                }
                else
                {
                    Debug.Log("奖励已领取: " + rewardID);
                }
            }
            else
            {
                Debug.LogError("奖励进度不存在: " + rewardID);
            }
        }
        else
        {
            Debug.LogError("玩家进度不存在: " + playerID);
        }
    }
    
    public PlayerGuideProgress GetPlayerProgress(string playerID)
    {
        return newbieGuideData.system.GetPlayerProgress(playerID);
    }
    
    // 数据持久化
    public void SaveNewbieGuideData()
    {
        string path = Application.dataPath + "/Data/newbie_guide_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, newbieGuideData);
        stream.Close();
    }
    
    public void LoadNewbieGuideData()
    {
        string path = Application.dataPath + "/Data/newbie_guide_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            newbieGuideData = (NewbieGuideDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            newbieGuideData = new NewbieGuideDetailedManagerData();
        }
    }
}
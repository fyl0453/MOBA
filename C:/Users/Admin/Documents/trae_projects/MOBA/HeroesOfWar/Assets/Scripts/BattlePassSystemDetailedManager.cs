using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BattlePassSystemDetailedManager : MonoBehaviour
{
    public static BattlePassSystemDetailedManager Instance { get; private set; }
    
    public BattlePassSystemDetailedManagerData battlePassData;
    
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
        LoadBattlePassData();
        
        if (battlePassData == null)
        {
            battlePassData = new BattlePassSystemDetailedManagerData();
            InitializeDefaultBattlePassSystem();
        }
    }
    
    private void InitializeDefaultBattlePassSystem()
    {
        // 通行证
        BattlePass battlePass1 = new BattlePass("battle_pass_001", "王者战令S1", "第一赛季王者战令", 50, 388, 888, "diamond");
        battlePass1.Activate();
        
        BattlePass battlePass2 = new BattlePass("battle_pass_002", "王者战令S2", "第二赛季王者战令", 50, 388, 888, "diamond");
        
        battlePassData.system.AddBattlePass(battlePass1);
        battlePassData.system.AddBattlePass(battlePass2);
        
        // 通行证等级
        for (int i = 1; i <= 50; i++)
        {
            string levelID = "level_" + battlePass1.battlePassID + "_" + i;
            int requiredExp = i * 100;
            BattlePassLevel level = new BattlePassLevel(levelID, battlePass1.battlePassID, i, requiredExp, i % 5 == 0);
            battlePassData.system.AddBattlePassLevel(level);
        }
        
        // 通行证任务
        BattlePassTask task1 = new BattlePassTask("task_001", battlePass1.battlePassID, "每日登录", "每日登录游戏", "login", 1, 50, true, false);
        BattlePassTask task2 = new BattlePassTask("task_002", battlePass1.battlePassID, "胜利一场", "完成一场胜利", "win", 1, 100, true, false);
        BattlePassTask task3 = new BattlePassTask("task_003", battlePass1.battlePassID, "击杀10人", "累计击杀10人", "kill", 10, 150, true, false);
        BattlePassTask task4 = new BattlePassTask("task_004", battlePass1.battlePassID, "助攻20次", "累计助攻20次", "assist", 20, 150, true, false);
        BattlePassTask task5 = new BattlePassTask("task_005", battlePass1.battlePassID, "使用特定英雄", "使用指定英雄3次", "hero", 3, 200, false, true);
        
        battlePassData.system.AddBattlePassTask(task1);
        battlePassData.system.AddBattlePassTask(task2);
        battlePassData.system.AddBattlePassTask(task3);
        battlePassData.system.AddBattlePassTask(task4);
        battlePassData.system.AddBattlePassTask(task5);
        
        // 通行证奖励
        for (int i = 1; i <= 50; i++)
        {
            string levelID = "level_" + battlePass1.battlePassID + "_" + i;
            
            // 普通奖励
            BattlePassReward reward1 = new BattlePassReward("reward_" + levelID + "_1", levelID, "金币奖励", "currency", "gold", 100 * i, false);
            battlePassData.system.AddBattlePassReward(reward1);
            
            // 高级奖励
            if (i % 5 == 0)
            {
                BattlePassReward reward2 = new BattlePassReward("reward_" + levelID + "_2", levelID, "皮肤碎片", "fragment", "skin_fragment", 5, true);
                battlePassData.system.AddBattlePassReward(reward2);
            }
        }
        
        // 玩家通行证
        PlayerBattlePass playerBattlePass1 = new PlayerBattlePass("player_battle_pass_001", "user_001", battlePass1.battlePassID, true);
        PlayerBattlePass playerBattlePass2 = new PlayerBattlePass("player_battle_pass_002", "user_002", battlePass1.battlePassID, false);
        
        // 玩家任务
        PlayerBattlePassTask playerTask1 = new PlayerBattlePassTask("player_task_001", task1.taskID);
        PlayerBattlePassTask playerTask2 = new PlayerBattlePassTask("player_task_002", task2.taskID);
        PlayerBattlePassTask playerTask3 = new PlayerBattlePassTask("player_task_003", task3.taskID);
        
        playerBattlePass1.AddTask(playerTask1);
        playerBattlePass1.AddTask(playerTask2);
        playerBattlePass1.AddTask(playerTask3);
        
        PlayerBattlePassTask playerTask4 = new PlayerBattlePassTask("player_task_004", task1.taskID);
        PlayerBattlePassTask playerTask5 = new PlayerBattlePassTask("player_task_005", task2.taskID);
        
        playerBattlePass2.AddTask(playerTask4);
        playerBattlePass2.AddTask(playerTask5);
        
        // 玩家奖励
        for (int i = 1; i <= 10; i++)
        {
            string levelID = "level_" + battlePass1.battlePassID + "_" + i;
            List<BattlePassReward> rewards = battlePassData.system.GetBattlePassRewardsByLevel(levelID);
            foreach (BattlePassReward reward in rewards)
            {
                if (!reward.isPremium || playerBattlePass1.hasPremium)
                {
                    PlayerBattlePassReward playerReward = new PlayerBattlePassReward("player_reward_" + reward.rewardID + "_1", reward.rewardID);
                    playerBattlePass1.AddReward(playerReward);
                }
                
                if (!reward.isPremium || playerBattlePass2.hasPremium)
                {
                    PlayerBattlePassReward playerReward = new PlayerBattlePassReward("player_reward_" + reward.rewardID + "_2", reward.rewardID);
                    playerBattlePass2.AddReward(playerReward);
                }
            }
        }
        
        battlePassData.system.AddPlayerBattlePass(playerBattlePass1);
        battlePassData.system.AddPlayerBattlePass(playerBattlePass2);
        
        // 通行证事件
        BattlePassEvent event1 = new BattlePassEvent("event_001", "join", "user_001", battlePass1.battlePassID, "加入通行证");
        BattlePassEvent event2 = new BattlePassEvent("event_002", "level_up", "user_001", battlePass1.battlePassID, "升级到2级");
        BattlePassEvent event3 = new BattlePassEvent("event_003", "claim_reward", "user_001", battlePass1.battlePassID, "领取奖励");
        
        battlePassData.system.AddBattlePassEvent(event1);
        battlePassData.system.AddBattlePassEvent(event2);
        battlePassData.system.AddBattlePassEvent(event3);
        
        SaveBattlePassData();
    }
    
    // 通行证管理
    public void AddBattlePass(string name, string description, int maxLevel, int basePrice, int premiumPrice, string currency)
    {
        string battlePassID = "battle_pass_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BattlePass battlePass = new BattlePass(battlePassID, name, description, maxLevel, basePrice, premiumPrice, currency);
        battlePassData.system.AddBattlePass(battlePass);
        SaveBattlePassData();
        Debug.Log("成功添加通行证: " + name);
    }
    
    public void ActivateBattlePass(string battlePassID)
    {
        BattlePass battlePass = battlePassData.system.GetBattlePass(battlePassID);
        if (battlePass != null)
        {
            battlePass.Activate();
            SaveBattlePassData();
            Debug.Log("成功激活通行证: " + battlePass.battlePassName);
        }
        else
        {
            Debug.LogError("通行证不存在: " + battlePassID);
        }
    }
    
    public void DeactivateBattlePass(string battlePassID)
    {
        BattlePass battlePass = battlePassData.system.GetBattlePass(battlePassID);
        if (battlePass != null)
        {
            battlePass.Deactivate();
            SaveBattlePassData();
            Debug.Log("成功停用通行证: " + battlePass.battlePassName);
        }
        else
        {
            Debug.LogError("通行证不存在: " + battlePassID);
        }
    }
    
    public void EndBattlePass(string battlePassID)
    {
        BattlePass battlePass = battlePassData.system.GetBattlePass(battlePassID);
        if (battlePass != null)
        {
            battlePass.End();
            SaveBattlePassData();
            Debug.Log("成功结束通行证: " + battlePass.battlePassName);
        }
        else
        {
            Debug.LogError("通行证不存在: " + battlePassID);
        }
    }
    
    public List<BattlePass> GetActiveBattlePasses()
    {
        return battlePassData.system.GetActiveBattlePasses();
    }
    
    // 通行证等级管理
    public void AddBattlePassLevel(string battlePassID, int level, int requiredExp, bool isPremium)
    {
        string levelID = "level_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BattlePassLevel battlePassLevel = new BattlePassLevel(levelID, battlePassID, level, requiredExp, isPremium);
        battlePassData.system.AddBattlePassLevel(battlePassLevel);
        SaveBattlePassData();
        Debug.Log("成功添加通行证等级: " + level);
    }
    
    public List<BattlePassLevel> GetBattlePassLevels(string battlePassID)
    {
        return battlePassData.system.GetBattlePassLevelsByBattlePass(battlePassID);
    }
    
    // 通行证任务管理
    public void AddBattlePassTask(string battlePassID, string taskName, string taskDescription, string taskType, int requiredProgress, int expReward, bool isRepeatable, bool isPremium)
    {
        string taskID = "task_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BattlePassTask battlePassTask = new BattlePassTask(taskID, battlePassID, taskName, taskDescription, taskType, requiredProgress, expReward, isRepeatable, isPremium);
        battlePassData.system.AddBattlePassTask(battlePassTask);
        SaveBattlePassData();
        Debug.Log("成功添加通行证任务: " + taskName);
    }
    
    public List<BattlePassTask> GetBattlePassTasks(string battlePassID)
    {
        return battlePassData.system.GetBattlePassTasksByBattlePass(battlePassID);
    }
    
    // 通行证奖励管理
    public void AddBattlePassReward(string levelID, string rewardName, string rewardType, string rewardValue, int quantity, bool isPremium)
    {
        string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BattlePassReward battlePassReward = new BattlePassReward(rewardID, levelID, rewardName, rewardType, rewardValue, quantity, isPremium);
        battlePassData.system.AddBattlePassReward(battlePassReward);
        
        // 添加到玩家通行证
        List<PlayerBattlePass> playerBattlePasses = battlePassData.system.playerBattlePasses;
        foreach (PlayerBattlePass playerBattlePass in playerBattlePasses)
        {
            BattlePassLevel level = battlePassData.system.GetBattlePassLevel(levelID);
            if (level != null && playerBattlePass.battlePassID == level.battlePassID)
            {
                if (!battlePassReward.isPremium || playerBattlePass.hasPremium)
                {
                    string playerRewardID = "player_reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                    PlayerBattlePassReward playerReward = new PlayerBattlePassReward(playerRewardID, rewardID);
                    playerBattlePass.AddReward(playerReward);
                }
            }
        }
        
        SaveBattlePassData();
        Debug.Log("成功添加通行证奖励: " + rewardName);
    }
    
    public List<BattlePassReward> GetBattlePassRewards(string levelID)
    {
        return battlePassData.system.GetBattlePassRewardsByLevel(levelID);
    }
    
    // 玩家通行证管理
    public void AddPlayerBattlePass(string userID, string battlePassID, bool hasPremium)
    {
        string playerBattlePassID = "player_battle_pass_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        PlayerBattlePass playerBattlePass = new PlayerBattlePass(playerBattlePassID, userID, battlePassID, hasPremium);
        
        // 添加任务
        List<BattlePassTask> tasks = battlePassData.system.GetBattlePassTasksByBattlePass(battlePassID);
        foreach (BattlePassTask task in tasks)
        {
            if (!task.isPremium || hasPremium)
            {
                string playerTaskID = "player_task_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                PlayerBattlePassTask playerTask = new PlayerBattlePassTask(playerTaskID, task.taskID);
                playerBattlePass.AddTask(playerTask);
            }
        }
        
        // 添加奖励
        List<BattlePassLevel> levels = battlePassData.system.GetBattlePassLevelsByBattlePass(battlePassID);
        foreach (BattlePassLevel level in levels)
        {
            List<BattlePassReward> rewards = battlePassData.system.GetBattlePassRewardsByLevel(level.levelID);
            foreach (BattlePassReward reward in rewards)
            {
                if (!reward.isPremium || hasPremium)
                {
                    string playerRewardID = "player_reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                    PlayerBattlePassReward playerReward = new PlayerBattlePassReward(playerRewardID, reward.rewardID);
                    playerBattlePass.AddReward(playerReward);
                }
            }
        }
        
        battlePassData.system.AddPlayerBattlePass(playerBattlePass);
        CreateBattlePassEvent("join", userID, battlePassID, "加入通行证");
        SaveBattlePassData();
        Debug.Log("成功添加玩家通行证: " + userID);
    }
    
    public PlayerBattlePass GetPlayerBattlePass(string userID, string battlePassID)
    {
        List<PlayerBattlePass> playerBattlePasses = battlePassData.system.GetPlayerBattlePassesByUser(userID);
        return playerBattlePasses.Find(pbp => pbp.battlePassID == battlePassID);
    }
    
    public void UpgradeToPremium(string userID, string battlePassID)
    {
        PlayerBattlePass playerBattlePass = GetPlayerBattlePass(userID, battlePassID);
        if (playerBattlePass != null)
        {
            playerBattlePass.SetPremium(true);
            
            // 添加高级任务和奖励
            List<BattlePassTask> tasks = battlePassData.system.GetBattlePassTasksByBattlePass(battlePassID);
            foreach (BattlePassTask task in tasks)
            {
                if (task.isPremium && !playerBattlePass.tasks.Exists(pt => pt.taskID == task.taskID))
                {
                    string playerTaskID = "player_task_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                    PlayerBattlePassTask playerTask = new PlayerBattlePassTask(playerTaskID, task.taskID);
                    playerBattlePass.AddTask(playerTask);
                }
            }
            
            List<BattlePassLevel> levels = battlePassData.system.GetBattlePassLevelsByBattlePass(battlePassID);
            foreach (BattlePassLevel level in levels)
            {
                List<BattlePassReward> rewards = battlePassData.system.GetBattlePassRewardsByLevel(level.levelID);
                foreach (BattlePassReward reward in rewards)
                {
                    if (reward.isPremium && !playerBattlePass.rewards.Exists(pr => pr.rewardID == reward.rewardID))
                    {
                        string playerRewardID = "player_reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                        PlayerBattlePassReward playerReward = new PlayerBattlePassReward(playerRewardID, reward.rewardID);
                        playerBattlePass.AddReward(playerReward);
                    }
                }
            }
            
            CreateBattlePassEvent("upgrade", userID, battlePassID, "升级到高级通行证");
            SaveBattlePassData();
            Debug.Log("成功升级到高级通行证: " + userID);
        }
        else
        {
            Debug.LogError("玩家通行证不存在");
        }
    }
    
    // 任务进度管理
    public void UpdateTaskProgress(string userID, string battlePassID, string taskID, int progress)
    {
        PlayerBattlePass playerBattlePass = GetPlayerBattlePass(userID, battlePassID);
        if (playerBattlePass != null)
        {
            PlayerBattlePassTask playerTask = playerBattlePass.tasks.Find(pt => pt.taskID == taskID);
            if (playerTask != null && playerTask.status == "in_progress")
            {
                BattlePassTask task = battlePassData.system.GetBattlePassTask(taskID);
                if (task != null && task.status == "active")
                {
                    playerTask.UpdateProgress(progress);
                    
                    if (playerTask.currentProgress >= task.requiredProgress)
                    {
                        playerTask.Complete();
                        playerBattlePass.AddExp(task.expReward);
                        
                        // 检查是否升级
                        CheckLevelUp(playerBattlePass);
                        
                        // 如果任务可重复，重置任务
                        if (task.isRepeatable)
                        {
                            playerTask.Reset();
                        }
                        
                        CreateBattlePassEvent("task_complete", userID, battlePassID, "完成任务: " + task.taskName);
                    }
                    
                    SaveBattlePassData();
                    Debug.Log("成功更新任务进度: " + task.taskName);
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
            Debug.LogError("玩家通行证不存在");
        }
    }
    
    // 等级升级检查
    private void CheckLevelUp(PlayerBattlePass playerBattlePass)
    {
        BattlePass battlePass = battlePassData.system.GetBattlePass(playerBattlePass.battlePassID);
        if (battlePass != null)
        {
            List<BattlePassLevel> levels = battlePassData.system.GetBattlePassLevelsByBattlePass(battlePass.battlePassID);
            levels.Sort((a, b) => a.level.CompareTo(b.level));
            
            foreach (BattlePassLevel level in levels)
            {
                if (playerBattlePass.currentLevel < level.level && playerBattlePass.currentExp >= level.requiredExp)
                {
                    playerBattlePass.LevelUp();
                    CreateBattlePassEvent("level_up", playerBattlePass.userID, battlePass.battlePassID, "升级到" + playerBattlePass.currentLevel + "级");
                    Debug.Log("玩家升级到: " + playerBattlePass.currentLevel + "级");
                }
            }
        }
    }
    
    // 奖励领取
    public void ClaimReward(string userID, string battlePassID, string rewardID)
    {
        PlayerBattlePass playerBattlePass = GetPlayerBattlePass(userID, battlePassID);
        if (playerBattlePass != null)
        {
            PlayerBattlePassReward playerReward = playerBattlePass.rewards.Find(pr => pr.rewardID == rewardID);
            if (playerReward != null && !playerReward.isClaimed)
            {
                BattlePassReward reward = battlePassData.system.GetBattlePassReward(rewardID);
                if (reward != null)
                {
                    // 检查等级是否达到
                    BattlePassLevel level = battlePassData.system.GetBattlePassLevel(reward.levelID);
                    if (level != null && playerBattlePass.currentLevel >= level.level)
                    {
                        playerReward.Claim();
                        reward.Claim();
                        
                        // 这里可以添加发放奖励的逻辑
                        Debug.Log("发放奖励: " + reward.rewardName + " 数量: " + reward.quantity);
                        
                        CreateBattlePassEvent("claim_reward", userID, battlePassID, "领取奖励: " + reward.rewardName);
                        SaveBattlePassData();
                        Debug.Log("成功领取奖励: " + reward.rewardName);
                    }
                    else
                    {
                        Debug.LogError("等级不足");
                    }
                }
                else
                {
                    Debug.LogError("奖励不存在");
                }
            }
            else
            {
                Debug.LogError("玩家奖励不存在或已领取");
            }
        }
        else
        {
            Debug.LogError("玩家通行证不存在");
        }
    }
    
    // 通行证事件管理
    public string CreateBattlePassEvent(string eventType, string userID, string battlePassID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BattlePassEvent battlePassEvent = new BattlePassEvent(eventID, eventType, userID, battlePassID, description);
        battlePassData.system.AddBattlePassEvent(battlePassEvent);
        SaveBattlePassData();
        Debug.Log("成功创建通行证事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        BattlePassEvent battlePassEvent = battlePassData.system.GetBattlePassEvent(eventID);
        if (battlePassEvent != null)
        {
            battlePassEvent.MarkAsCompleted();
            SaveBattlePassData();
            Debug.Log("成功标记通行证事件为完成");
        }
        else
        {
            Debug.LogError("通行证事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        BattlePassEvent battlePassEvent = battlePassData.system.GetBattlePassEvent(eventID);
        if (battlePassEvent != null)
        {
            battlePassEvent.MarkAsFailed();
            SaveBattlePassData();
            Debug.Log("成功标记通行证事件为失败");
        }
        else
        {
            Debug.LogError("通行证事件不存在: " + eventID);
        }
    }
    
    // 数据持久化
    public void SaveBattlePassData()
    {
        string path = Application.dataPath + "/Data/battle_pass_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, battlePassData);
        stream.Close();
    }
    
    public void LoadBattlePassData()
    {
        string path = Application.dataPath + "/Data/battle_pass_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            battlePassData = (BattlePassSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            battlePassData = new BattlePassSystemDetailedManagerData();
        }
    }
}
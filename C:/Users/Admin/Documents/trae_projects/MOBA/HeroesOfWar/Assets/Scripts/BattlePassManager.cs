using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BattlePassManager : MonoBehaviour
{
    public static BattlePassManager Instance { get; private set; }
    
    public BattlePass currentPass;
    public PassProgress playerProgress;
    public List<DailyMission> dailyMissions;
    
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
        LoadPassProgress();
        LoadDailyMissions();
        
        if (currentPass == null)
        {
            currentPass = new BattlePass("pass_001", "S8赛季通行证", "season_8", 50, 680);
            InitializePassRewards();
        }
        
        if (playerProgress == null)
        {
            playerProgress = new PassProgress("player_001", currentPass.passID);
        }
        
        if (dailyMissions.Count == 0)
        {
            GenerateDailyMissions();
        }
    }
    
    private void InitializePassRewards()
    {
        currentPass.rewards.Add(new PassReward("reward_001", 1, "Gold", "gold", 100, false));
        currentPass.rewards.Add(new PassReward("reward_002", 2, "Item", "item_health_potion", 5, false));
        currentPass.rewards.Add(new PassReward("reward_003", 3, "Skin", "skin_001", 1, true));
        currentPass.rewards.Add(new PassReward("reward_004", 4, "Gold", "gold", 200, false));
        currentPass.rewards.Add(new PassReward("reward_005", 5, "Item", "item_mana_potion", 5, false));
        currentPass.rewards.Add(new PassReward("reward_006", 6, "Avatar", "avatar_001", 1, true));
        currentPass.rewards.Add(new PassReward("reward_007", 7, "Gold", "gold", 300, false));
        currentPass.rewards.Add(new PassReward("reward_008", 8, "Item", "item_attack_potion", 3, false));
        currentPass.rewards.Add(new PassReward("reward_009", 9, "Skin", "skin_002", 1, true));
        currentPass.rewards.Add(new PassReward("reward_010", 10, "Gold", "gold", 400, false));
        
        for (int i = 11; i <= 50; i++)
        {
            if (i % 5 == 0)
            {
                currentPass.rewards.Add(new PassReward($"reward_{i:D3}", i, "Skin", $"skin_{i}", 1, true));
            }
            else if (i % 3 == 0)
            {
                currentPass.rewards.Add(new PassReward($"reward_{i:D3}", i, "Avatar", $"avatar_{i}", 1, true));
            }
            else
            {
                currentPass.rewards.Add(new PassReward($"reward_{i:D3}", i, "Gold", "gold", i * 10, false));
            }
        }
        
        SaveBattlePassData();
    }
    
    private void GenerateDailyMissions()
    {
        dailyMissions.Add(new DailyMission("mission_001", "击败10个敌人", "在任何模式中击败10个敌人", 100, 50, 10, "Kill"));
        dailyMissions.Add(new DailyMission("mission_002", "完成2场比赛", "完成2场任何模式的比赛", 150, 100, 2, "Match"));
        dailyMissions.Add(new DailyMission("mission_003", "获得3次助攻", "在任何模式中获得3次助攻", 100, 50, 3, "Assist"));
        dailyMissions.Add(new DailyMission("mission_004", "摧毁1座防御塔", "在任何模式中摧毁1座防御塔", 120, 80, 1, "Tower"));
        dailyMissions.Add(new DailyMission("mission_005", "使用技能10次", "在任何模式中使用技能10次", 80, 30, 10, "Skill"));
        
        SaveDailyMissions();
    }
    
    public void AddPassXP(int xp)
    {
        playerProgress.AddXP(xp);
        SavePassProgress();
    }
    
    public void CompleteMission(string missionID)
    {
        DailyMission mission = dailyMissions.Find(m => m.missionID == missionID);
        if (mission != null)
        {
            mission.isCompleted = true;
            SaveDailyMissions();
        }
    }
    
    public void ClaimMissionRewards(string missionID)
    {
        DailyMission mission = dailyMissions.Find(m => m.missionID == missionID);
        if (mission != null && mission.isCompleted && !mission.isClaimed)
        {
            mission.ClaimRewards();
            ProfileManager.Instance.currentProfile.gold += mission.goldReward;
            AddPassXP(mission.xpReward);
            SaveDailyMissions();
            ProfileManager.Instance.SaveProfile();
        }
    }
    
    public void ClaimPassReward(string rewardID)
    {
        PassReward reward = currentPass.rewards.Find(r => r.rewardID == rewardID);
        if (reward != null && !reward.isClaimed)
        {
            if (reward.level <= playerProgress.currentLevel && (!reward.isPremium || playerProgress.hasPurchasedPremium))
            {
                reward.isClaimed = true;
                playerProgress.ClaimReward(rewardID);
                GrantReward(reward);
                SaveBattlePassData();
                SavePassProgress();
            }
        }
    }
    
    private void GrantReward(PassReward reward)
    {
        switch (reward.rewardType)
        {
            case "Gold":
                ProfileManager.Instance.currentProfile.gold += reward.quantity;
                break;
            case "Item":
                InventoryManager.Instance.AddItemToInventory(reward.rewardItemID, reward.quantity);
                break;
            case "Skin":
                ProfileManager.Instance.currentProfile.AddSkin(reward.rewardItemID);
                break;
            case "Avatar":
                // 这里需要添加头像系统的逻辑
                break;
        }
        
        ProfileManager.Instance.SaveProfile();
    }
    
    public void PurchasePremiumPass()
    {
        if (!playerProgress.hasPurchasedPremium)
        {
            int playerGems = ProfileManager.Instance.currentProfile.gems;
            if (playerGems >= currentPass.price)
            {
                ProfileManager.Instance.currentProfile.gems -= currentPass.price;
                playerProgress.hasPurchasedPremium = true;
                SavePassProgress();
                ProfileManager.Instance.SaveProfile();
            }
        }
    }
    
    public void ResetDailyMissions()
    {
        dailyMissions.Clear();
        GenerateDailyMissions();
    }
    
    public List<PassReward> GetAvailableRewards()
    {
        List<PassReward> available = new List<PassReward>();
        foreach (PassReward reward in currentPass.rewards)
        {
            if (reward.level <= playerProgress.currentLevel && !reward.isClaimed && (!reward.isPremium || playerProgress.hasPurchasedPremium))
            {
                available.Add(reward);
            }
        }
        return available;
    }
    
    public List<DailyMission> GetDailyMissions()
    {
        return dailyMissions;
    }
    
    public PassProgress GetPassProgress()
    {
        return playerProgress;
    }
    
    public BattlePass GetCurrentPass()
    {
        return currentPass;
    }
    
    public void SaveBattlePassData()
    {
        string path = Application.dataPath + "/Data/battle_pass_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, currentPass);
        stream.Close();
    }
    
    public void LoadBattlePassData()
    {
        string path = Application.dataPath + "/Data/battle_pass_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            currentPass = (BattlePass)formatter.Deserialize(stream);
            stream.Close();
        }
    }
    
    public void SavePassProgress()
    {
        string path = Application.dataPath + "/Data/pass_progress.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, playerProgress);
        stream.Close();
    }
    
    public void LoadPassProgress()
    {
        string path = Application.dataPath + "/Data/pass_progress.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            playerProgress = (PassProgress)formatter.Deserialize(stream);
            stream.Close();
        }
    }
    
    public void SaveDailyMissions()
    {
        string path = Application.dataPath + "/Data/daily_missions.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, dailyMissions);
        stream.Close();
    }
    
    public void LoadDailyMissions()
    {
        string path = Application.dataPath + "/Data/daily_missions.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            dailyMissions = (List<DailyMission>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            dailyMissions = new List<DailyMission>();
        }
    }
}
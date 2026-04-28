using System;
using System.Collections.Generic;

public class MasterySystemDetailedManager
{
    private static MasterySystemDetailedManager _instance;
    public static MasterySystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MasterySystemDetailedManager();
            }
            return _instance;
        }
    }

    private MasterySystemData masteryData;
    private MasterySystemDataManager dataManager;

    private MasterySystemDetailedManager()
    {
        dataManager = MasterySystemDataManager.Instance;
        masteryData = dataManager.masteryData;
    }

    public void InitializePlayerMastery(string playerID)
    {
        if (!masteryData.PlayerMasteryData.ContainsKey(playerID))
        {
            PlayerMasteryData playerData = new PlayerMasteryData(playerID);
            masteryData.AddPlayerMasteryData(playerID, playerData);
            dataManager.SaveMasteryData();
            Debug.Log("初始化熟练度数据成功");
        }
    }

    public void UpdateHeroMastery(string playerID, string heroID, bool isWin, int kills, int assists, int damage, int gameDuration)
    {
        InitializePlayerMastery(playerID);
        PlayerMasteryData playerData = masteryData.PlayerMasteryData[playerID];
        
        if (!playerData.HeroMasteries.ContainsKey(heroID))
        {
            string masteryID = "mastery_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            HeroMastery mastery = new HeroMastery(masteryID, playerID, heroID);
            playerData.HeroMasteries[heroID] = mastery;
            playerData.UnlockedHeroes.Add(heroID);
        }
        
        HeroMastery heroMastery = playerData.HeroMasteries[heroID];
        heroMastery.TotalGames++;
        if (isWin)
        {
            heroMastery.WinGames++;
        }
        heroMastery.WinRate = (float)heroMastery.WinGames / heroMastery.TotalGames * 100;
        heroMastery.TotalKills += kills;
        heroMastery.TotalAssists += assists;
        heroMastery.TotalDamage += damage;
        heroMastery.LastPlayed = DateTime.Now;
        
        int expGained = CalculateExpGain(isWin, kills, assists, damage, gameDuration);
        heroMastery.Exp += expGained;
        playerData.TotalMasteryExp += expGained;
        
        CheckMasteryLevelUp(heroMastery, playerData);
        
        playerData.LastUpdateTime = DateTime.Now;
        dataManager.CreateMasteryEvent("mastery_update", playerID, heroID, "更新熟练度: " + expGained);
        dataManager.SaveMasteryData();
        Debug.Log("更新熟练度成功: " + expGained);
    }

    private int CalculateExpGain(bool isWin, int kills, int assists, int damage, int gameDuration)
    {
        int baseExp = 100;
        if (isWin)
        {
            baseExp += 50;
        }
        baseExp += kills * 5;
        baseExp += assists * 3;
        baseExp += damage / 100;
        baseExp += gameDuration / 60;
        return Math.Max(baseExp, 50);
    }

    private void CheckMasteryLevelUp(HeroMastery heroMastery, PlayerMasteryData playerData)
    {
        for (int i = heroMastery.Level; i < masteryData.MasteryLevels.Count; i++)
        {
            MasteryLevel nextLevel = masteryData.MasteryLevels[i];
            if (heroMastery.Exp >= nextLevel.RequiredExp)
            {
                heroMastery.Level = nextLevel.Level;
                heroMastery.MasteryTitle = nextLevel.Title;
                heroMastery.IsUnlocked = true;
                playerData.TotalMasteryLevel++;
                dataManager.CreateMasteryEvent("mastery_level_up", playerData.PlayerID, heroMastery.HeroID, "熟练度升级到" + nextLevel.Title);
                Debug.Log("熟练度升级成功: " + nextLevel.Title);
            }
            else
            {
                break;
            }
        }
    }

    public HeroMastery GetHeroMastery(string playerID, string heroID)
    {
        if (masteryData.PlayerMasteryData.ContainsKey(playerID))
        {
            PlayerMasteryData playerData = masteryData.PlayerMasteryData[playerID];
            if (playerData.HeroMasteries.ContainsKey(heroID))
            {
                return playerData.HeroMasteries[heroID];
            }
        }
        return null;
    }

    public List<HeroMastery> GetPlayerHeroMasteries(string playerID)
    {
        if (masteryData.PlayerMasteryData.ContainsKey(playerID))
        {
            PlayerMasteryData playerData = masteryData.PlayerMasteryData[playerID];
            List<HeroMastery> masteries = new List<HeroMastery>(playerData.HeroMasteries.Values);
            masteries.Sort((a, b) => b.Level.CompareTo(a.Level));
            return masteries;
        }
        return new List<HeroMastery>();
    }

    public List<HeroMastery> GetPlayerUnlockedHeroes(string playerID)
    {
        if (masteryData.PlayerMasteryData.ContainsKey(playerID))
        {
            PlayerMasteryData playerData = masteryData.PlayerMasteryData[playerID];
            List<HeroMastery> unlockedHeroes = new List<HeroMastery>();
            foreach (HeroMastery mastery in playerData.HeroMasteries.Values)
            {
                if (mastery.IsUnlocked)
                {
                    unlockedHeroes.Add(mastery);
                }
            }
            unlockedHeroes.Sort((a, b) => b.Level.CompareTo(a.Level));
            return unlockedHeroes;
        }
        return new List<HeroMastery>();
    }

    public int GetPlayerTotalMasteryLevel(string playerID)
    {
        if (masteryData.PlayerMasteryData.ContainsKey(playerID))
        {
            return masteryData.PlayerMasteryData[playerID].TotalMasteryLevel;
        }
        return 0;
    }

    public int GetPlayerTotalMasteryExp(string playerID)
    {
        if (masteryData.PlayerMasteryData.ContainsKey(playerID))
        {
            return masteryData.PlayerMasteryData[playerID].TotalMasteryExp;
        }
        return 0;
    }

    public MasteryLevel GetMasteryLevel(int level)
    {
        return masteryData.MasteryLevels.Find(l => l.Level == level);
    }

    public List<MasteryLevel> GetAllMasteryLevels()
    {
        return masteryData.MasteryLevels;
    }

    public void AddMasteryLevel(int level, string title, int requiredExp, string description)
    {
        MasteryLevel newLevel = new MasteryLevel(level, title, requiredExp, description);
        masteryData.AddMasteryLevel(newLevel);
        dataManager.SaveMasteryData();
        Debug.Log("添加熟练度等级成功: " + title);
    }

    public void AddMasteryReward(int level, string rewardName, string rewardType, int rewardValue)
    {
        MasteryLevel masteryLevel = GetMasteryLevel(level);
        if (masteryLevel != null)
        {
            string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            MasteryReward reward = new MasteryReward(rewardID, rewardName, rewardType, rewardValue);
            masteryLevel.Rewards.Add(reward);
            dataManager.SaveMasteryData();
            Debug.Log("添加熟练度奖励成功: " + rewardName);
        }
    }

    public List<MasteryReward> GetMasteryRewards(int level)
    {
        MasteryLevel masteryLevel = GetMasteryLevel(level);
        if (masteryLevel != null)
        {
            return masteryLevel.Rewards;
        }
        return new List<MasteryReward>();
    }

    public void ResetHeroMastery(string playerID, string heroID)
    {
        if (masteryData.PlayerMasteryData.ContainsKey(playerID))
        {
            PlayerMasteryData playerData = masteryData.PlayerMasteryData[playerID];
            if (playerData.HeroMasteries.ContainsKey(heroID))
            {
                HeroMastery mastery = playerData.HeroMasteries[heroID];
                playerData.TotalMasteryLevel -= (mastery.Level - 1);
                playerData.HeroMasteries.Remove(heroID);
                playerData.UnlockedHeroes.Remove(heroID);
                playerData.LastUpdateTime = DateTime.Now;
                dataManager.CreateMasteryEvent("mastery_reset", playerID, heroID, "重置熟练度");
                dataManager.SaveMasteryData();
                Debug.Log("重置熟练度成功");
            }
        }
    }

    public void SaveData()
    {
        dataManager.SaveMasteryData();
    }

    public void LoadData()
    {
        dataManager.LoadMasteryData();
    }

    public List<MasteryEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
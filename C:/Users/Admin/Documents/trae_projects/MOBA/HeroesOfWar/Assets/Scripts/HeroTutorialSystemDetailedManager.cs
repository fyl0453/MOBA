using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroTutorialSystemDetailedManager
{
    private static HeroTutorialSystemDetailedManager _instance;
    public static HeroTutorialSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HeroTutorialSystemDetailedManager();
            }
            return _instance;
        }
    }

    private HeroTutorialSystemData tutorialData;
    private HeroTutorialSystemDataManager dataManager;

    private HeroTutorialSystemDetailedManager()
    {
        dataManager = HeroTutorialSystemDataManager.Instance;
        tutorialData = dataManager.tutorialData;
    }

    public void InitializeHeroTutorialData(string heroID, string heroName)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            HeroTutorialData heroTutorialData = new HeroTutorialData(heroID, heroName);
            tutorialData.AddHeroTutorialData(heroID, heroTutorialData);
            dataManager.SaveTutorialData();
            Debug.Log("初始化英雄教学数据成功: " + heroName);
        }
    }

    public void InitializePlayerTutorialProgress(string playerID)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            PlayerTutorialProgress playerProgress = new PlayerTutorialProgress(playerID);
            tutorialData.AddPlayerTutorialProgress(playerID, playerProgress);
            dataManager.SaveTutorialData();
            Debug.Log("初始化玩家学习进度成功");
        }
    }

    public void AddHeroSkill(string heroID, string skillID, string skillName, string skillDescription, string skillType, float cooldown, float manaCost, string skillIcon, string skillVideoURL, string skillTips)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            Debug.LogError("英雄不存在: " + heroID);
            return;
        }
        
        HeroSkill skill = new HeroSkill(skillID, skillName, skillDescription, skillType, cooldown, manaCost, skillIcon, skillVideoURL, skillTips);
        tutorialData.HeroTutorials[heroID].SkillTutorial.Skills.Add(skill);
        tutorialData.HeroTutorials[heroID].LastUpdated = DateTime.Now;
        
        dataManager.CreateTutorialEvent("skill_add", "system", heroID, "添加英雄技能: " + skillName);
        dataManager.SaveTutorialData();
        Debug.Log("添加英雄技能成功: " + skillName);
    }

    public void UpdateHeroSkill(string heroID, string skillID, string skillDescription, float cooldown, float manaCost, string skillTips)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            return;
        }
        
        HeroSkill skill = tutorialData.HeroTutorials[heroID].SkillTutorial.Skills.Find(s => s.SkillID == skillID);
        if (skill != null)
        {
            skill.SkillDescription = skillDescription;
            skill.Cooldown = cooldown;
            skill.ManaCost = manaCost;
            skill.SkillTips = skillTips;
            tutorialData.HeroTutorials[heroID].LastUpdated = DateTime.Now;
            
            dataManager.CreateTutorialEvent("skill_update", "system", heroID, "更新英雄技能: " + skill.SkillName);
            dataManager.SaveTutorialData();
            Debug.Log("更新英雄技能成功: " + skill.SkillName);
        }
    }

    public void AddItemBuild(string heroID, string buildName, string buildType, string situation, List<string> items)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            Debug.LogError("英雄不存在: " + heroID);
            return;
        }
        
        HeroTutorialData heroTutorialData = tutorialData.HeroTutorials[heroID];
        if (heroTutorialData.RecommendedBuild.ItemBuilds.Count >= tutorialData.MaxBuildsPerHero)
        {
            Debug.LogError("推荐出装数量达到上限");
            return;
        }
        
        ItemBuild itemBuild = new ItemBuild(buildName, buildType, situation);
        itemBuild.Items = items;
        heroTutorialData.RecommendedBuild.ItemBuilds.Add(itemBuild);
        heroTutorialData.LastUpdated = DateTime.Now;
        
        dataManager.CreateTutorialEvent("build_add", "system", heroID, "添加推荐出装: " + buildName);
        dataManager.SaveTutorialData();
        Debug.Log("添加推荐出装成功: " + buildName);
    }

    public void UpdateItemBuild(string heroID, string buildID, List<string> items)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            return;
        }
        
        ItemBuild itemBuild = tutorialData.HeroTutorials[heroID].RecommendedBuild.ItemBuilds.Find(b => b.BuildID == buildID);
        if (itemBuild != null)
        {
            itemBuild.Items = items;
            tutorialData.HeroTutorials[heroID].LastUpdated = DateTime.Now;
            
            dataManager.CreateTutorialEvent("build_update", "system", heroID, "更新推荐出装: " + itemBuild.BuildName);
            dataManager.SaveTutorialData();
            Debug.Log("更新推荐出装成功: " + itemBuild.BuildName);
        }
    }

    public void AddComboGuide(string heroID, string comboName, string comboDescription, string difficulty, string situation, List<ComboStep> comboSteps, string videoURL)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            Debug.LogError("英雄不存在: " + heroID);
            return;
        }
        
        HeroTutorialData heroTutorialData = tutorialData.HeroTutorials[heroID];
        if (heroTutorialData.ComboGuides.Count >= tutorialData.MaxComboGuidesPerHero)
        {
            Debug.LogError("连招指南数量达到上限");
            return;
        }
        
        ComboGuide comboGuide = new ComboGuide(heroID, heroTutorialData.HeroName, comboName, comboDescription, difficulty, situation);
        comboGuide.ComboSteps = comboSteps;
        comboGuide.VideoURL = videoURL;
        heroTutorialData.ComboGuides.Add(comboGuide);
        heroTutorialData.LastUpdated = DateTime.Now;
        
        dataManager.CreateTutorialEvent("combo_add", "system", heroID, "添加连招指南: " + comboName);
        dataManager.SaveTutorialData();
        Debug.Log("添加连招指南成功: " + comboName);
    }

    public void UpdateComboGuide(string heroID, string comboID, List<ComboStep> comboSteps, string videoURL)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            return;
        }
        
        ComboGuide comboGuide = tutorialData.HeroTutorials[heroID].ComboGuides.Find(c => c.ComboID == comboID);
        if (comboGuide != null)
        {
            comboGuide.ComboSteps = comboSteps;
            comboGuide.VideoURL = videoURL;
            tutorialData.HeroTutorials[heroID].LastUpdated = DateTime.Now;
            
            dataManager.CreateTutorialEvent("combo_update", "system", heroID, "更新连招指南: " + comboGuide.ComboName);
            dataManager.SaveTutorialData();
            Debug.Log("更新连招指南成功: " + comboGuide.ComboName);
        }
    }

    public void UpdateHeroInfo(string heroID, string heroLore, string playStyle, string counterTips, string synergyTips)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            return;
        }
        
        HeroTutorialData heroTutorialData = tutorialData.HeroTutorials[heroID];
        heroTutorialData.HeroLore = heroLore;
        heroTutorialData.PlayStyle = playStyle;
        heroTutorialData.CounterTips = counterTips;
        heroTutorialData.SynergyTips = synergyTips;
        heroTutorialData.LastUpdated = DateTime.Now;
        
        dataManager.CreateTutorialEvent("hero_info_update", "system", heroID, "更新英雄信息: " + heroTutorialData.HeroName);
        dataManager.SaveTutorialData();
        Debug.Log("更新英雄信息成功: " + heroTutorialData.HeroName);
    }

    public void MarkTutorialComplete(string playerID, string heroID, string tutorialType)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            InitializePlayerTutorialProgress(playerID);
        }
        
        PlayerTutorialProgress playerProgress = tutorialData.PlayerProgress[playerID];
        string tutorialKey = heroID + "_" + tutorialType;
        if (!playerProgress.CompletedTutorials.ContainsKey(tutorialKey) || !playerProgress.CompletedTutorials[tutorialKey])
        {
            playerProgress.CompletedTutorials[tutorialKey] = true;
            playerProgress.TotalTutorialsCompleted++;
            playerProgress.LastLearningTime = DateTime.Now;
            
            dataManager.CreateTutorialEvent("tutorial_complete", playerID, heroID, "完成教学: " + tutorialType);
            dataManager.SaveTutorialData();
            Debug.Log("标记教学完成成功: " + tutorialType);
        }
    }

    public void RecordComboUsage(string playerID, string comboID)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            InitializePlayerTutorialProgress(playerID);
        }
        
        PlayerTutorialProgress playerProgress = tutorialData.PlayerProgress[playerID];
        if (playerProgress.ComboUsageCount.ContainsKey(comboID))
        {
            playerProgress.ComboUsageCount[comboID]++;
        }
        else
        {
            playerProgress.ComboUsageCount[comboID] = 1;
        }
        
        playerProgress.LastLearningTime = DateTime.Now;
        dataManager.SaveTutorialData();
    }

    public void SetFavoriteBuild(string playerID, string heroID, string buildID)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            InitializePlayerTutorialProgress(playerID);
        }
        
        PlayerTutorialProgress playerProgress = tutorialData.PlayerProgress[playerID];
        playerProgress.FavoriteBuilds[heroID] = buildID;
        
        dataManager.CreateTutorialEvent("build_favorite", playerID, heroID, "设置 favorite 出装");
        dataManager.SaveTutorialData();
        Debug.Log("设置 favorite 出装成功");
    }

    public void UpdateSkillMastery(string playerID, string skillID, int masteryLevel)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            InitializePlayerTutorialProgress(playerID);
        }
        
        PlayerTutorialProgress playerProgress = tutorialData.PlayerProgress[playerID];
        playerProgress.SkillMasteryLevels[skillID] = masteryLevel;
        
        dataManager.SaveTutorialData();
    }

    public HeroTutorialData GetHeroTutorialData(string heroID)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            Debug.LogError("英雄不存在: " + heroID);
            return null;
        }
        return tutorialData.HeroTutorials[heroID];
    }

    public List<HeroSkill> GetHeroSkills(string heroID)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            return new List<HeroSkill>();
        }
        return tutorialData.HeroTutorials[heroID].SkillTutorial.Skills;
    }

    public List<ItemBuild> GetHeroItemBuilds(string heroID)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            return new List<ItemBuild>();
        }
        return tutorialData.HeroTutorials[heroID].RecommendedBuild.ItemBuilds;
    }

    public List<ComboGuide> GetHeroComboGuides(string heroID)
    {
        if (!tutorialData.HeroTutorials.ContainsKey(heroID))
        {
            return new List<ComboGuide>();
        }
        return tutorialData.HeroTutorials[heroID].ComboGuides;
    }

    public PlayerTutorialProgress GetPlayerTutorialProgress(string playerID)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            InitializePlayerTutorialProgress(playerID);
        }
        return tutorialData.PlayerProgress[playerID];
    }

    public bool IsTutorialCompleted(string playerID, string heroID, string tutorialType)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            return false;
        }
        
        PlayerTutorialProgress playerProgress = tutorialData.PlayerProgress[playerID];
        string tutorialKey = heroID + "_" + tutorialType;
        return playerProgress.CompletedTutorials.ContainsKey(tutorialKey) && playerProgress.CompletedTutorials[tutorialKey];
    }

    public string GetFavoriteBuild(string playerID, string heroID)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            return "";
        }
        
        PlayerTutorialProgress playerProgress = tutorialData.PlayerProgress[playerID];
        if (playerProgress.FavoriteBuilds.ContainsKey(heroID))
        {
            return playerProgress.FavoriteBuilds[heroID];
        }
        return "";
    }

    public int GetSkillMasteryLevel(string playerID, string skillID)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            return 0;
        }
        
        PlayerTutorialProgress playerProgress = tutorialData.PlayerProgress[playerID];
        if (playerProgress.SkillMasteryLevels.ContainsKey(skillID))
        {
            return playerProgress.SkillMasteryLevels[skillID];
        }
        return 0;
    }

    public int GetComboUsageCount(string playerID, string comboID)
    {
        if (!tutorialData.PlayerProgress.ContainsKey(playerID))
        {
            return 0;
        }
        
        PlayerTutorialProgress playerProgress = tutorialData.PlayerProgress[playerID];
        if (playerProgress.ComboUsageCount.ContainsKey(comboID))
        {
            return playerProgress.ComboUsageCount[comboID];
        }
        return 0;
    }

    public List<string> GetHeroCategories()
    {
        return tutorialData.HeroCategories;
    }

    public void AddHeroCategory(string categoryName)
    {
        if (!tutorialData.HeroCategories.Contains(categoryName))
        {
            tutorialData.HeroCategories.Add(categoryName);
            dataManager.SaveTutorialData();
            Debug.Log("添加英雄分类成功: " + categoryName);
        }
    }

    public void RemoveHeroCategory(string categoryName)
    {
        if (tutorialData.HeroCategories.Contains(categoryName))
        {
            tutorialData.HeroCategories.Remove(categoryName);
            dataManager.SaveTutorialData();
            Debug.Log("删除英雄分类成功: " + categoryName);
        }
    }

    public void CleanupOldData(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<string> playersToRemove = new List<string>();
        
        foreach (KeyValuePair<string, PlayerTutorialProgress> kvp in tutorialData.PlayerProgress)
        {
            if (kvp.Value.LastLearningTime < cutoffDate)
            {
                playersToRemove.Add(kvp.Key);
            }
        }
        
        foreach (string playerID in playersToRemove)
        {
            tutorialData.PlayerProgress.Remove(playerID);
        }
        
        if (playersToRemove.Count > 0)
        {
            dataManager.CreateTutorialEvent("data_cleanup", "system", "", "清理旧数据: " + playersToRemove.Count);
            dataManager.SaveTutorialData();
            Debug.Log("清理旧数据成功: " + playersToRemove.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveTutorialData();
    }

    public void LoadData()
    {
        dataManager.LoadTutorialData();
    }

    public List<TutorialEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
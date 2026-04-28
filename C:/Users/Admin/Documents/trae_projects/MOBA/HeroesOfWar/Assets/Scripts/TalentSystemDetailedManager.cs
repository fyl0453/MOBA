using System;
using System.Collections.Generic;

public class TalentSystemDetailedManager
{
    private static TalentSystemDetailedManager _instance;
    public static TalentSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TalentSystemDetailedManager();
            }
            return _instance;
        }
    }

    private TalentSystemData talentData;
    private TalentSystemDataManager dataManager;

    private TalentSystemDetailedManager()
    {
        dataManager = TalentSystemDataManager.Instance;
        talentData = dataManager.talentData;
    }

    public void InitializePlayerTalents(string playerID)
    {
        if (!talentData.PlayerTalentData.ContainsKey(playerID))
        {
            PlayerTalentData playerData = new PlayerTalentData(playerID);
            talentData.AddPlayerTalentData(playerID, playerData);
            dataManager.SaveTalentData();
            Debug.Log("初始化天赋数据成功");
        }
    }

    public string CreateTalentTree(string playerID, string heroID, string treeName, string description, int maxPoints)
    {
        InitializePlayerTalents(playerID);
        PlayerTalentData playerData = talentData.PlayerTalentData[playerID];
        
        if (!playerData.HeroTalentTrees.ContainsKey(heroID))
        {
            string treeID = "tree_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            TalentTree talentTree = new TalentTree(treeID, treeName, description, heroID, maxPoints);
            playerData.HeroTalentTrees[heroID] = talentTree;
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.CreateTalentEvent("talent_tree_create", playerID, "", "创建天赋树: " + treeName);
            dataManager.SaveTalentData();
            Debug.Log("创建天赋树成功: " + treeName);
            return treeID;
        }
        return "";
    }

    public void AddTalentToTree(string playerID, string heroID, string talentID)
    {
        InitializePlayerTalents(playerID);
        PlayerTalentData playerData = talentData.PlayerTalentData[playerID];
        if (playerData.HeroTalentTrees.ContainsKey(heroID))
        {
            TalentTree talentTree = playerData.HeroTalentTrees[heroID];
            Talent talent = GetTalent(talentID);
            if (talent != null && !talentTree.Talents.Exists(t => t.TalentID == talentID))
            {
                talentTree.Talents.Add(talent);
                talentTree.LastUpdateTime = DateTime.Now;
                playerData.LastUpdateTime = DateTime.Now;
                dataManager.SaveTalentData();
                Debug.Log("添加天赋到天赋树成功: " + talent.TalentName);
            }
        }
    }

    public void AllocateTalentPoint(string playerID, string heroID, string talentID)
    {
        InitializePlayerTalents(playerID);
        PlayerTalentData playerData = talentData.PlayerTalentData[playerID];
        if (playerData.HeroTalentTrees.ContainsKey(heroID))
        {
            TalentTree talentTree = playerData.HeroTalentTrees[heroID];
            Talent talent = talentTree.Talents.Find(t => t.TalentID == talentID);
            if (talent != null && talentTree.CurrentPoints < talentTree.MaxPoints)
            {
                if (CheckTalentRequirements(talentTree, talent))
                {
                    talentTree.CurrentPoints++;
                    playerData.TotalTalentPoints++;
                    talentTree.LastUpdateTime = DateTime.Now;
                    playerData.LastUpdateTime = DateTime.Now;
                    dataManager.CreateTalentEvent("talent_point_allocate", playerID, talentID, "分配天赋点到: " + talent.TalentName);
                    dataManager.SaveTalentData();
                    Debug.Log("分配天赋点成功: " + talent.TalentName);
                }
            }
        }
    }

    public void RemoveTalentPoint(string playerID, string heroID, string talentID)
    {
        InitializePlayerTalents(playerID);
        PlayerTalentData playerData = talentData.PlayerTalentData[playerID];
        if (playerData.HeroTalentTrees.ContainsKey(heroID))
        {
            TalentTree talentTree = playerData.HeroTalentTrees[heroID];
            if (talentTree.CurrentPoints > 0)
            {
                talentTree.CurrentPoints--;
                playerData.TotalTalentPoints--;
                talentTree.LastUpdateTime = DateTime.Now;
                playerData.LastUpdateTime = DateTime.Now;
                dataManager.CreateTalentEvent("talent_point_remove", playerID, talentID, "移除天赋点");
                dataManager.SaveTalentData();
                Debug.Log("移除天赋点成功");
            }
        }
    }

    public void ResetTalentTree(string playerID, string heroID)
    {
        InitializePlayerTalents(playerID);
        PlayerTalentData playerData = talentData.PlayerTalentData[playerID];
        if (playerData.HeroTalentTrees.ContainsKey(heroID))
        {
            TalentTree talentTree = playerData.HeroTalentTrees[heroID];
            playerData.TotalTalentPoints -= talentTree.CurrentPoints;
            talentTree.CurrentPoints = 0;
            talentTree.LastUpdateTime = DateTime.Now;
            playerData.LastUpdateTime = DateTime.Now;
            dataManager.CreateTalentEvent("talent_tree_reset", playerID, "", "重置天赋树");
            dataManager.SaveTalentData();
            Debug.Log("重置天赋树成功");
        }
    }

    private bool CheckTalentRequirements(TalentTree talentTree, Talent talent)
    {
        foreach (string requiredTalentID in talent.RequiredTalents)
        {
            if (!talentTree.Talents.Exists(t => t.TalentID == requiredTalentID))
            {
                return false;
            }
        }
        return true;
    }

    public Talent GetTalent(string talentID)
    {
        return talentData.AllTalents.Find(t => t.TalentID == talentID);
    }

    public List<Talent> GetAllTalents()
    {
        return talentData.AllTalents;
    }

    public List<Talent> GetTalentsByType(int talentType)
    {
        return talentData.AllTalents.FindAll(t => t.TalentType == talentType);
    }

    public TalentTree GetHeroTalentTree(string playerID, string heroID)
    {
        if (talentData.PlayerTalentData.ContainsKey(playerID))
        {
            PlayerTalentData playerData = talentData.PlayerTalentData[playerID];
            if (playerData.HeroTalentTrees.ContainsKey(heroID))
            {
                return playerData.HeroTalentTrees[heroID];
            }
        }
        return null;
    }

    public List<TalentTree> GetPlayerTalentTrees(string playerID)
    {
        if (talentData.PlayerTalentData.ContainsKey(playerID))
        {
            PlayerTalentData playerData = talentData.PlayerTalentData[playerID];
            return new List<TalentTree>(playerData.HeroTalentTrees.Values);
        }
        return new List<TalentTree>();
    }

    public int GetPlayerTotalTalentPoints(string playerID)
    {
        if (talentData.PlayerTalentData.ContainsKey(playerID))
        {
            return talentData.PlayerTalentData[playerID].TotalTalentPoints;
        }
        return 0;
    }

    public void AddTalent(string talentName, string description, int talentType, int requiredLevel, int requiredPoints, Dictionary<string, int> attributes, bool isPassive = false, string skillEffect = "")
    {
        string talentID = "talent_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Talent talent = new Talent(talentID, talentName, description, talentType, requiredLevel, requiredPoints, attributes, isPassive, skillEffect);
        talentData.AddTalent(talent);
        dataManager.SaveTalentData();
        Debug.Log("创建天赋成功: " + talentName);
    }

    public void AddTalentRequirement(string talentID, string requiredTalentID)
    {
        Talent talent = GetTalent(talentID);
        if (talent != null)
        {
            if (!talent.RequiredTalents.Contains(requiredTalentID))
            {
                talent.RequiredTalents.Add(requiredTalentID);
                dataManager.SaveTalentData();
                Debug.Log("添加天赋需求成功");
            }
        }
    }

    public void SaveData()
    {
        dataManager.SaveTalentData();
    }

    public void LoadData()
    {
        dataManager.LoadTalentData();
    }

    public List<TalentEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
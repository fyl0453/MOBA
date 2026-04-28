using System;
using System.Collections.Generic;

public class IntimateSystemDetailedManager
{
    private static IntimateSystemDetailedManager _instance;
    public static IntimateSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new IntimateSystemDetailedManager();
            }
            return _instance;
        }
    }

    private IntimateSystemData intimateData;
    private IntimateSystemDataManager dataManager;

    private IntimateSystemDetailedManager()
    {
        dataManager = IntimateSystemDataManager.Instance;
        intimateData = dataManager.intimateData;
    }

    public void InitializePlayerIntimacyData(string playerID)
    {
        if (!intimateData.PlayerIntimacyData.ContainsKey(playerID))
        {
            PlayerIntimacyData playerData = new PlayerIntimacyData(playerID);
            intimateData.AddPlayerIntimacyData(playerID, playerData);
            dataManager.SaveIntimateData();
            Debug.Log("初始化亲密关系数据成功");
        }
    }

    public string EstablishRelationship(string playerID1, string playerID2, string playerName1, string playerName2, int relationshipType)
    {
        if (playerID1 == playerID2)
        {
            Debug.LogError("不能与自己建立亲密关系");
            return "";
        }

        InitializePlayerIntimacyData(playerID1);
        InitializePlayerIntimacyData(playerID2);

        PlayerIntimacyData playerData1 = intimateData.PlayerIntimacyData[playerID1];
        PlayerIntimacyData playerData2 = intimateData.PlayerIntimacyData[playerID2];

        if (playerData1.Relationships.Count >= intimateData.MaxRelationshipsPerPlayer)
        {
            Debug.LogError("玩家1的亲密关系数量已达上限");
            return "";
        }

        if (playerData2.Relationships.Count >= intimateData.MaxRelationshipsPerPlayer)
        {
            Debug.LogError("玩家2的亲密关系数量已达上限");
            return "";
        }

        if (HasExistingRelationship(playerID1, playerID2))
        {
            Debug.LogError("两位玩家已经存在亲密关系");
            return "";
        }

        string relationshipID = "relationship_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        IntimateRelationship relationship = new IntimateRelationship(relationshipID, playerID1, playerID2, playerName1, playerName2, relationshipType);
        intimateData.AddRelationship(relationship);

        playerData1.Relationships.Add(relationship);
        playerData1.ActiveRelationshipsCount++;
        playerData1.LastInteractionTime = DateTime.Now;

        playerData2.Relationships.Add(relationship);
        playerData2.ActiveRelationshipsCount++;
        playerData2.LastInteractionTime = DateTime.Now;

        dataManager.CreateIntimateEvent("relationship_establish", playerID1, playerID2, "建立亲密关系: " + relationship.RelationshipName);
        dataManager.SaveIntimateData();
        Debug.Log("建立亲密关系成功: " + relationship.RelationshipName);
        return relationshipID;
    }

    public void AddIntimacyPoints(string playerID, string targetPlayerID, int points, string interactionDescription)
    {
        IntimateRelationship relationship = GetRelationship(playerID, targetPlayerID);
        if (relationship != null && relationship.IsActive)
        {
            relationship.IntimacyPoints += points;
            relationship.LastInteractionTime = DateTime.Now;
            
            UpdateIntimacyLevel(relationship);
            
            string interactionID = "interaction_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            IntimacyInteraction interaction = new IntimacyInteraction(interactionID, playerID, targetPlayerID, 0, points, interactionDescription);
            intimateData.AddInteraction(interaction);
            
            PlayerIntimacyData playerData = intimateData.PlayerIntimacyData[playerID];
            playerData.TotalIntimacyPoints += points;
            playerData.LastInteractionTime = DateTime.Now;
            
            dataManager.CreateIntimateEvent("intimacy_add", playerID, targetPlayerID, "增加亲密度: " + points + "点");
            dataManager.SaveIntimateData();
            Debug.Log("增加亲密度成功: " + points + "点");
        }
    }

    public void RemoveRelationship(string playerID, string targetPlayerID)
    {
        IntimateRelationship relationship = GetRelationship(playerID, targetPlayerID);
        if (relationship != null)
        {
            relationship.IsActive = false;
            
            PlayerIntimacyData playerData1 = intimateData.PlayerIntimacyData[playerID];
            playerData1.ActiveRelationshipsCount--;
            
            PlayerIntimacyData playerData2 = intimateData.PlayerIntimacyData[targetPlayerID];
            playerData2.ActiveRelationshipsCount--;
            
            dataManager.CreateIntimateEvent("relationship_remove", playerID, targetPlayerID, "解除亲密关系");
            dataManager.SaveIntimateData();
            Debug.Log("解除亲密关系成功");
        }
    }

    public void UpdateIntimacyLevel(IntimateRelationship relationship)
    {
        int currentPoints = relationship.IntimacyPoints;
        IntimacyLevelConfig nextLevel = null;
        
        foreach (IntimacyLevelConfig config in intimateData.LevelConfigs)
        {
            if (currentPoints >= config.RequiredPoints && config.Level > relationship.IntimacyLevel)
            {
                nextLevel = config;
            }
        }
        
        if (nextLevel != null)
        {
            relationship.IntimacyLevel = nextLevel.Level;
            dataManager.CreateIntimateEvent("intimacy_level_up", relationship.PlayerID1, relationship.PlayerID2, "亲密度升级到: " + nextLevel.LevelName);
            Debug.Log("亲密度升级成功: " + nextLevel.LevelName);
        }
    }

    public IntimateRelationship GetRelationship(string playerID1, string playerID2)
    {
        foreach (IntimateRelationship relationship in intimateData.AllRelationships)
        {
            if (relationship.IsActive && 
                ((relationship.PlayerID1 == playerID1 && relationship.PlayerID2 == playerID2) ||
                 (relationship.PlayerID1 == playerID2 && relationship.PlayerID2 == playerID1)))
            {
                return relationship;
            }
        }
        return null;
    }

    public bool HasExistingRelationship(string playerID1, string playerID2)
    {
        return GetRelationship(playerID1, playerID2) != null;
    }

    public List<IntimateRelationship> GetPlayerRelationships(string playerID)
    {
        if (intimateData.PlayerIntimacyData.ContainsKey(playerID))
        {
            return intimateData.PlayerIntimacyData[playerID].Relationships.FindAll(r => r.IsActive);
        }
        return new List<IntimateRelationship>();
    }

    public List<IntimateRelationship> GetAllRelationships()
    {
        return intimateData.AllRelationships.FindAll(r => r.IsActive);
    }

    public IntimacyLevelConfig GetIntimacyLevelConfig(int level)
    {
        return intimateData.LevelConfigs.Find(c => c.Level == level);
    }

    public List<IntimacyLevelConfig> GetAllLevelConfigs()
    {
        return intimateData.LevelConfigs;
    }

    public int GetPlayerIntimacyPoints(string playerID, string targetPlayerID)
    {
        IntimateRelationship relationship = GetRelationship(playerID, targetPlayerID);
        if (relationship != null)
        {
            return relationship.IntimacyPoints;
        }
        return 0;
    }

    public int GetPlayerIntimacyLevel(string playerID, string targetPlayerID)
    {
        IntimateRelationship relationship = GetRelationship(playerID, targetPlayerID);
        if (relationship != null)
        {
            return relationship.IntimacyLevel;
        }
        return 0;
    }

    public string GetPlayerIntimacyLevelName(string playerID, string targetPlayerID)
    {
        IntimateRelationship relationship = GetRelationship(playerID, targetPlayerID);
        if (relationship != null)
        {
            IntimacyLevelConfig config = GetIntimacyLevelConfig(relationship.IntimacyLevel);
            if (config != null)
            {
                return config.LevelName;
            }
        }
        return "";
    }

    public List<string> GetIntimacyRewards(int level)
    {
        IntimacyLevelConfig config = GetIntimacyLevelConfig(level);
        if (config != null)
        {
            return config.UnlockRewards;
        }
        return new List<string>();
    }

    public void SetRelationshipType(string relationshipID, int newType)
    {
        IntimateRelationship relationship = intimateData.AllRelationships.Find(r => r.RelationshipID == relationshipID);
        if (relationship != null && relationship.IsActive)
        {
            relationship.RelationshipType = newType;
            relationship.RelationshipName = GetRelationshipTypeName(newType);
            dataManager.CreateIntimateEvent("relationship_type_change", relationship.PlayerID1, relationship.PlayerID2, "变更关系类型为: " + relationship.RelationshipName);
            dataManager.SaveIntimateData();
            Debug.Log("变更关系类型成功: " + relationship.RelationshipName);
        }
    }

    private string GetRelationshipTypeName(int type)
    {
        switch (type)
        {
            case 0: return "好友";
            case 1: return "死党";
            case 2: return "恋人";
            case 3: return "基友";
            case 4: return "闺蜜";
            default: return "好友";
        }
    }

    public void CleanupInactiveRelationships()
    {
        List<IntimateRelationship> inactiveRelationships = new List<IntimateRelationship>();
        foreach (IntimateRelationship relationship in intimateData.AllRelationships)
        {
            if (!relationship.IsActive)
            {
                inactiveRelationships.Add(relationship);
            }
        }
        
        foreach (IntimateRelationship relationship in inactiveRelationships)
        {
            intimateData.AllRelationships.Remove(relationship);
            if (intimateData.PlayerIntimacyData.ContainsKey(relationship.PlayerID1))
            {
                PlayerIntimacyData playerData1 = intimateData.PlayerIntimacyData[relationship.PlayerID1];
                playerData1.Relationships.Remove(relationship);
            }
            if (intimateData.PlayerIntimacyData.ContainsKey(relationship.PlayerID2))
            {
                PlayerIntimacyData playerData2 = intimateData.PlayerIntimacyData[relationship.PlayerID2];
                playerData2.Relationships.Remove(relationship);
            }
        }
        
        if (inactiveRelationships.Count > 0)
        {
            dataManager.CreateIntimateEvent("relationship_cleanup", "system", "", "清理无效关系: " + inactiveRelationships.Count);
            dataManager.SaveIntimateData();
            Debug.Log("清理无效关系成功: " + inactiveRelationships.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveIntimateData();
    }

    public void LoadData()
    {
        dataManager.LoadIntimateData();
    }

    public List<IntimateEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
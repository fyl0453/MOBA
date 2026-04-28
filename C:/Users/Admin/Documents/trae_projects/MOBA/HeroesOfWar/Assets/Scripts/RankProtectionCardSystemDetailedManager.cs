using System;
using System.Collections.Generic;
using UnityEngine;

public class RankProtectionCardSystemDetailedManager
{
    private static RankProtectionCardSystemDetailedManager _instance;
    public static RankProtectionCardSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RankProtectionCardSystemDetailedManager();
            }
            return _instance;
        }
    }

    private RankProtectionCardSystemData protectionData;
    private RankProtectionCardSystemDataManager dataManager;

    private RankProtectionCardSystemDetailedManager()
    {
        dataManager = RankProtectionCardSystemDataManager.Instance;
        protectionData = dataManager.protectionData;
    }

    public void InitializePlayerRankProtectionData(string playerID)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            PlayerRankProtectionData playerProtectionData = new PlayerRankProtectionData(playerID);
            protectionData.AddPlayerRankProtectionData(playerID, playerProtectionData);
            dataManager.SaveProtectionData();
            Debug.Log("初始化段位保护卡数据成功");
        }
    }

    public string CreateRankProtectionCard(string cardType, string rankTier, string rankDivision, int protectionCount, string rarity, string description, string iconURL, int expiryDays = 30)
    {
        RankProtectionCard card = new RankProtectionCard(cardType, rankTier, rankDivision, protectionCount, rarity, description, iconURL, expiryDays);
        protectionData.AddProtectionCard(card);
        
        dataManager.CreateProtectionEvent("card_create", "system", card.CardID, "创建段位保护卡: " + rankTier + " " + rankDivision);
        dataManager.SaveProtectionData();
        Debug.Log("创建段位保护卡成功: " + rankTier + " " + rankDivision);
        return card.CardID;
    }

    public void AddRankProtectionCard(string playerID, string cardID)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            InitializePlayerRankProtectionData(playerID);
        }
        
        RankProtectionCard card = protectionData.AllProtectionCards.Find(c => c.CardID == cardID);
        if (card == null)
        {
            Debug.LogError("段位保护卡不存在: " + cardID);
            return;
        }
        
        PlayerRankProtectionData playerProtectionData = protectionData.PlayerRankProtectionData[playerID];
        if (playerProtectionData.ProtectionCards.Count >= protectionData.MaxCardsPerPlayer)
        {
            Debug.LogError("段位保护卡数量达到上限");
            return;
        }
        
        playerProtectionData.ProtectionCards.Add(card);
        playerProtectionData.TotalCardsAcquired++;
        playerProtectionData.LastCardAcquisitionTime = DateTime.Now;
        
        dataManager.CreateProtectionEvent("card_add", playerID, cardID, "添加段位保护卡: " + card.RankTier + " " + card.RankDivision);
        dataManager.SaveProtectionData();
        Debug.Log("添加段位保护卡成功: " + card.RankTier + " " + card.RankDivision);
    }

    public bool UseRankProtectionCard(string playerID, string cardID, string matchID, string rankTier, string rankDivision, int rankPointsBefore, int rankPointsAfter, bool preventedDemotion, string protectionResult)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            return false;
        }
        
        PlayerRankProtectionData playerProtectionData = protectionData.PlayerRankProtectionData[playerID];
        RankProtectionCard card = playerProtectionData.ProtectionCards.Find(c => c.CardID == cardID);
        if (card == null)
        {
            Debug.LogError("段位保护卡不存在");
            return false;
        }
        
        if (card.IsUsed)
        {
            Debug.LogError("段位保护卡已使用");
            return false;
        }
        
        if (card.ExpiryDate < DateTime.Now)
        {
            Debug.LogError("段位保护卡已过期");
            return false;
        }
        
        if (card.RankTier != rankTier || card.RankDivision != rankDivision)
        {
            Debug.LogError("段位保护卡与当前段位不匹配");
            return false;
        }
        
        card.IsUsed = true;
        card.UsedDate = DateTime.Now;
        
        ProtectionRecord record = new ProtectionRecord(playerID, cardID, matchID, rankTier, rankDivision, rankPointsBefore, rankPointsAfter, preventedDemotion, protectionResult);
        protectionData.AddProtectionRecord(record);
        playerProtectionData.ProtectionRecords.Add(record);
        playerProtectionData.TotalCardsUsed++;
        if (preventedDemotion)
        {
            playerProtectionData.TotalDemotionsPrevented++;
            playerProtectionData.LastDemotionPreventionTime = DateTime.Now;
        }
        playerProtectionData.LastCardUseTime = DateTime.Now;
        
        dataManager.CreateProtectionEvent("card_use", playerID, cardID, "使用段位保护卡: " + protectionResult);
        dataManager.SaveProtectionData();
        Debug.Log("使用段位保护卡成功: " + protectionResult);
        return true;
    }

    public void RemoveRankProtectionCard(string playerID, string cardID)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerRankProtectionData playerProtectionData = protectionData.PlayerRankProtectionData[playerID];
        RankProtectionCard card = playerProtectionData.ProtectionCards.Find(c => c.CardID == cardID);
        if (card != null)
        {
            playerProtectionData.ProtectionCards.Remove(card);
            
            dataManager.CreateProtectionEvent("card_remove", playerID, cardID, "移除段位保护卡: " + card.RankTier + " " + card.RankDivision);
            dataManager.SaveProtectionData();
            Debug.Log("移除段位保护卡成功: " + card.RankTier + " " + card.RankDivision);
        }
    }

    public List<RankProtectionCard> GetPlayerRankProtectionCards(string playerID)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            InitializePlayerRankProtectionData(playerID);
        }
        return protectionData.PlayerRankProtectionData[playerID].ProtectionCards;
    }

    public List<RankProtectionCard> GetPlayerActiveRankProtectionCards(string playerID)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            InitializePlayerRankProtectionData(playerID);
        }
        
        DateTime now = DateTime.Now;
        return protectionData.PlayerRankProtectionData[playerID].ProtectionCards.FindAll(c => !c.IsUsed && c.ExpiryDate > now);
    }

    public List<RankProtectionCard> GetPlayerExpiredRankProtectionCards(string playerID)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            InitializePlayerRankProtectionData(playerID);
        }
        
        DateTime now = DateTime.Now;
        return protectionData.PlayerRankProtectionData[playerID].ProtectionCards.FindAll(c => !c.IsUsed && c.ExpiryDate <= now);
    }

    public List<RankProtectionCard> GetPlayerUsedRankProtectionCards(string playerID)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            InitializePlayerRankProtectionData(playerID);
        }
        return protectionData.PlayerRankProtectionData[playerID].ProtectionCards.FindAll(c => c.IsUsed);
    }

    public List<ProtectionRecord> GetPlayerProtectionRecords(string playerID)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            InitializePlayerRankProtectionData(playerID);
        }
        return protectionData.PlayerRankProtectionData[playerID].ProtectionRecords;
    }

    public RankProtectionCard GetRankProtectionCard(string cardID)
    {
        return protectionData.AllProtectionCards.Find(c => c.CardID == cardID);
    }

    public PlayerRankProtectionData GetPlayerRankProtectionData(string playerID)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            InitializePlayerRankProtectionData(playerID);
        }
        return protectionData.PlayerRankProtectionData[playerID];
    }

    public bool IsCardExpired(string cardID)
    {
        RankProtectionCard card = protectionData.AllProtectionCards.Find(c => c.CardID == cardID);
        return card != null && card.ExpiryDate < DateTime.Now && !card.IsUsed;
    }

    public bool IsCardUsed(string cardID)
    {
        RankProtectionCard card = protectionData.AllProtectionCards.Find(c => c.CardID == cardID);
        return card != null && card.IsUsed;
    }

    public bool IsCardActive(string cardID)
    {
        RankProtectionCard card = protectionData.AllProtectionCards.Find(c => c.CardID == cardID);
        return card != null && !card.IsUsed && card.ExpiryDate > DateTime.Now;
    }

    public void CleanupExpiredCards(string playerID)
    {
        if (!protectionData.PlayerRankProtectionData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerRankProtectionData playerProtectionData = protectionData.PlayerRankProtectionData[playerID];
        DateTime now = DateTime.Now;
        List<RankProtectionCard> expiredCards = playerProtectionData.ProtectionCards.FindAll(c => !c.IsUsed && c.ExpiryDate <= now);
        
        foreach (RankProtectionCard card in expiredCards)
        {
            playerProtectionData.ProtectionCards.Remove(card);
        }
        
        if (expiredCards.Count > 0)
        {
            dataManager.CreateProtectionEvent("card_cleanup", playerID, "", "清理过期段位保护卡: " + expiredCards.Count);
            dataManager.SaveProtectionData();
            Debug.Log("清理过期段位保护卡成功: " + expiredCards.Count);
        }
    }

    public void CleanupAllExpiredCards()
    {
        int totalCleaned = 0;
        DateTime now = DateTime.Now;
        
        foreach (PlayerRankProtectionData playerProtectionData in protectionData.PlayerRankProtectionData.Values)
        {
            List<RankProtectionCard> expiredCards = playerProtectionData.ProtectionCards.FindAll(c => !c.IsUsed && c.ExpiryDate <= now);
            foreach (RankProtectionCard card in expiredCards)
            {
                playerProtectionData.ProtectionCards.Remove(card);
                totalCleaned++;
            }
        }
        
        if (totalCleaned > 0)
        {
            dataManager.CreateProtectionEvent("card_cleanup_all", "system", "", "清理所有过期段位保护卡: " + totalCleaned);
            dataManager.SaveProtectionData();
            Debug.Log("清理所有过期段位保护卡成功: " + totalCleaned);
        }
    }

    public List<string> GetCardTypes()
    {
        return protectionData.CardTypes;
    }

    public List<string> GetRankTiers()
    {
        return protectionData.RankTiers;
    }

    public void AddCardType(string cardType)
    {
        if (!protectionData.CardTypes.Contains(cardType))
        {
            protectionData.CardTypes.Add(cardType);
            dataManager.SaveProtectionData();
            Debug.Log("添加段位保护卡类型成功: " + cardType);
        }
    }

    public void RemoveCardType(string cardType)
    {
        if (cardType != "normal" && protectionData.CardTypes.Contains(cardType))
        {
            protectionData.CardTypes.Remove(cardType);
            dataManager.SaveProtectionData();
            Debug.Log("删除段位保护卡类型成功: " + cardType);
        }
    }

    public void AddRankTier(string rankTier)
    {
        if (!protectionData.RankTiers.Contains(rankTier))
        {
            protectionData.RankTiers.Add(rankTier);
            dataManager.SaveProtectionData();
            Debug.Log("添加段位等级成功: " + rankTier);
        }
    }

    public void RemoveRankTier(string rankTier)
    {
        if (rankTier != "Bronze" && protectionData.RankTiers.Contains(rankTier))
        {
            protectionData.RankTiers.Remove(rankTier);
            dataManager.SaveProtectionData();
            Debug.Log("删除段位等级成功: " + rankTier);
        }
    }

    public void SaveData()
    {
        dataManager.SaveProtectionData();
    }

    public void LoadData()
    {
        dataManager.LoadProtectionData();
    }

    public List<RankProtectionEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
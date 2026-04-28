using System;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceCardSystemDetailedManager
{
    private static ExperienceCardSystemDetailedManager _instance;
    public static ExperienceCardSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ExperienceCardSystemDetailedManager();
            }
            return _instance;
        }
    }

    private ExperienceCardSystemData cardData;
    private ExperienceCardSystemDataManager dataManager;

    private ExperienceCardSystemDetailedManager()
    {
        dataManager = ExperienceCardSystemDataManager.Instance;
        cardData = dataManager.cardData;
    }

    public void InitializePlayerExperienceCardData(string playerID)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            PlayerExperienceCardData playerCardData = new PlayerExperienceCardData(playerID);
            cardData.AddPlayerExperienceCardData(playerID, playerCardData);
            dataManager.SaveCardData();
            Debug.Log("初始化体验卡数据成功");
        }
    }

    public string CreateHeroExperienceCard(string heroID, string heroName, int durationHours, string rarity, string description, string iconURL, string heroType, int heroLevel, bool unlockedSkills)
    {
        HeroExperienceCard card = new HeroExperienceCard(heroID, heroName, durationHours, rarity, description, iconURL, heroType, heroLevel, unlockedSkills);
        cardData.AddExperienceCard(card);
        
        dataManager.CreateCardEvent("card_create", "system", card.CardID, "创建英雄体验卡: " + heroName);
        dataManager.SaveCardData();
        Debug.Log("创建英雄体验卡成功: " + heroName);
        return card.CardID;
    }

    public string CreateSkinExperienceCard(string skinID, string skinName, int durationHours, string rarity, string description, string iconURL, string skinRarity, string heroID, string heroName, bool hasSpecialEffects)
    {
        SkinExperienceCard card = new SkinExperienceCard(skinID, skinName, durationHours, rarity, description, iconURL, skinRarity, heroID, heroName, hasSpecialEffects);
        cardData.AddExperienceCard(card);
        
        dataManager.CreateCardEvent("card_create", "system", card.CardID, "创建皮肤体验卡: " + skinName);
        dataManager.SaveCardData();
        Debug.Log("创建皮肤体验卡成功: " + skinName);
        return card.CardID;
    }

    public void AddExperienceCard(string playerID, string cardID)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            InitializePlayerExperienceCardData(playerID);
        }
        
        ExperienceCard card = cardData.AllExperienceCards.Find(c => c.CardID == cardID);
        if (card == null)
        {
            Debug.LogError("体验卡不存在: " + cardID);
            return;
        }
        
        PlayerExperienceCardData playerCardData = cardData.PlayerExperienceCardData[playerID];
        if (playerCardData.ExperienceCards.Count >= cardData.MaxCardsPerPlayer)
        {
            Debug.LogError("体验卡数量达到上限");
            return;
        }
        
        playerCardData.ExperienceCards.Add(card);
        playerCardData.TotalCardsAcquired++;
        playerCardData.LastCardAcquisitionTime = DateTime.Now;
        
        dataManager.CreateCardEvent("card_add", playerID, cardID, "添加体验卡: " + card.AssociatedName);
        dataManager.SaveCardData();
        Debug.Log("添加体验卡成功: " + card.AssociatedName);
    }

    public bool UseExperienceCard(string playerID, string cardID)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            return false;
        }
        
        PlayerExperienceCardData playerCardData = cardData.PlayerExperienceCardData[playerID];
        ExperienceCard card = playerCardData.ExperienceCards.Find(c => c.CardID == cardID);
        if (card == null)
        {
            Debug.LogError("体验卡不存在");
            return false;
        }
        
        if (card.IsUsed)
        {
            Debug.LogError("体验卡已使用");
            return false;
        }
        
        if (card.ExpiryDate < DateTime.Now)
        {
            Debug.LogError("体验卡已过期");
            return false;
        }
        
        card.IsUsed = true;
        card.UsedDate = DateTime.Now;
        playerCardData.UsedCardHistory.Add(cardID);
        playerCardData.TotalCardsUsed++;
        playerCardData.LastCardUseTime = DateTime.Now;
        
        dataManager.CreateCardEvent("card_use", playerID, cardID, "使用体验卡: " + card.AssociatedName);
        dataManager.SaveCardData();
        Debug.Log("使用体验卡成功: " + card.AssociatedName);
        return true;
    }

    public void RemoveExperienceCard(string playerID, string cardID)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerExperienceCardData playerCardData = cardData.PlayerExperienceCardData[playerID];
        ExperienceCard card = playerCardData.ExperienceCards.Find(c => c.CardID == cardID);
        if (card != null)
        {
            playerCardData.ExperienceCards.Remove(card);
            
            dataManager.CreateCardEvent("card_remove", playerID, cardID, "移除体验卡: " + card.AssociatedName);
            dataManager.SaveCardData();
            Debug.Log("移除体验卡成功: " + card.AssociatedName);
        }
    }

    public List<ExperienceCard> GetPlayerExperienceCards(string playerID)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            InitializePlayerExperienceCardData(playerID);
        }
        return cardData.PlayerExperienceCardData[playerID].ExperienceCards;
    }

    public List<ExperienceCard> GetPlayerActiveExperienceCards(string playerID)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            InitializePlayerExperienceCardData(playerID);
        }
        
        DateTime now = DateTime.Now;
        return cardData.PlayerExperienceCardData[playerID].ExperienceCards.FindAll(c => !c.IsUsed && c.ExpiryDate > now);
    }

    public List<ExperienceCard> GetPlayerExpiredExperienceCards(string playerID)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            InitializePlayerExperienceCardData(playerID);
        }
        
        DateTime now = DateTime.Now;
        return cardData.PlayerExperienceCardData[playerID].ExperienceCards.FindAll(c => !c.IsUsed && c.ExpiryDate <= now);
    }

    public List<ExperienceCard> GetPlayerUsedExperienceCards(string playerID)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            InitializePlayerExperienceCardData(playerID);
        }
        return cardData.PlayerExperienceCardData[playerID].ExperienceCards.FindAll(c => c.IsUsed);
    }

    public List<ExperienceCard> GetCardsByType(string playerID, string cardType)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            InitializePlayerExperienceCardData(playerID);
        }
        return cardData.PlayerExperienceCardData[playerID].ExperienceCards.FindAll(c => c.CardType == cardType);
    }

    public ExperienceCard GetExperienceCard(string cardID)
    {
        return cardData.AllExperienceCards.Find(c => c.CardID == cardID);
    }

    public PlayerExperienceCardData GetPlayerExperienceCardData(string playerID)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            InitializePlayerExperienceCardData(playerID);
        }
        return cardData.PlayerExperienceCardData[playerID];
    }

    public bool IsCardExpired(string cardID)
    {
        ExperienceCard card = cardData.AllExperienceCards.Find(c => c.CardID == cardID);
        return card != null && card.ExpiryDate < DateTime.Now && !card.IsUsed;
    }

    public bool IsCardUsed(string cardID)
    {
        ExperienceCard card = cardData.AllExperienceCards.Find(c => c.CardID == cardID);
        return card != null && card.IsUsed;
    }

    public bool IsCardActive(string cardID)
    {
        ExperienceCard card = cardData.AllExperienceCards.Find(c => c.CardID == cardID);
        return card != null && !card.IsUsed && card.ExpiryDate > DateTime.Now;
    }

    public void CleanupExpiredCards(string playerID)
    {
        if (!cardData.PlayerExperienceCardData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerExperienceCardData playerCardData = cardData.PlayerExperienceCardData[playerID];
        DateTime now = DateTime.Now;
        List<ExperienceCard> expiredCards = playerCardData.ExperienceCards.FindAll(c => !c.IsUsed && c.ExpiryDate <= now);
        
        foreach (ExperienceCard card in expiredCards)
        {
            playerCardData.ExperienceCards.Remove(card);
            playerCardData.TotalCardsExpired++;
        }
        
        if (expiredCards.Count > 0)
        {
            dataManager.CreateCardEvent("card_cleanup", playerID, "", "清理过期体验卡: " + expiredCards.Count);
            dataManager.SaveCardData();
            Debug.Log("清理过期体验卡成功: " + expiredCards.Count);
        }
    }

    public void CleanupAllExpiredCards()
    {
        int totalCleaned = 0;
        DateTime now = DateTime.Now;
        
        foreach (PlayerExperienceCardData playerCardData in cardData.PlayerExperienceCardData.Values)
        {
            List<ExperienceCard> expiredCards = playerCardData.ExperienceCards.FindAll(c => !c.IsUsed && c.ExpiryDate <= now);
            foreach (ExperienceCard card in expiredCards)
            {
                playerCardData.ExperienceCards.Remove(card);
                playerCardData.TotalCardsExpired++;
                totalCleaned++;
            }
        }
        
        if (totalCleaned > 0)
        {
            dataManager.CreateCardEvent("card_cleanup_all", "system", "", "清理所有过期体验卡: " + totalCleaned);
            dataManager.SaveCardData();
            Debug.Log("清理所有过期体验卡成功: " + totalCleaned);
        }
    }

    public List<string> GetCardTypes()
    {
        return cardData.CardTypes;
    }

    public void AddCardType(string cardType)
    {
        if (!cardData.CardTypes.Contains(cardType))
        {
            cardData.CardTypes.Add(cardType);
            dataManager.SaveCardData();
            Debug.Log("添加体验卡类型成功: " + cardType);
        }
    }

    public void RemoveCardType(string cardType)
    {
        if (cardType != "hero" && cardType != "skin" && cardData.CardTypes.Contains(cardType))
        {
            cardData.CardTypes.Remove(cardType);
            dataManager.SaveCardData();
            Debug.Log("删除体验卡类型成功: " + cardType);
        }
    }

    public void SaveData()
    {
        dataManager.SaveCardData();
    }

    public void LoadData()
    {
        dataManager.LoadCardData();
    }

    public List<ExperienceCardEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
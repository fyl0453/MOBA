using System;
using System.Collections.Generic;
using UnityEngine;

public class NameChangeCardSystemDetailedManager
{
    private static NameChangeCardSystemDetailedManager _instance;
    public static NameChangeCardSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NameChangeCardSystemDetailedManager();
            }
            return _instance;
        }
    }

    private NameChangeCardSystemData nameChangeData;
    private NameChangeCardSystemDataManager dataManager;

    private NameChangeCardSystemDetailedManager()
    {
        dataManager = NameChangeCardSystemDataManager.Instance;
        nameChangeData = dataManager.nameChangeData;
    }

    public void InitializePlayerNameChangeData(string playerID, string currentName)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            PlayerNameChangeData playerNameChangeData = new PlayerNameChangeData(playerID, currentName);
            nameChangeData.AddPlayerNameChangeData(playerID, playerNameChangeData);
            dataManager.SaveNameChangeData();
            Debug.Log("初始化改名卡数据成功");
        }
    }

    public string CreateNameChangeCard(string cardType, string rarity, string description, string iconURL, int expiryDays = 30)
    {
        NameChangeCard card = new NameChangeCard(cardType, rarity, description, iconURL, expiryDays);
        nameChangeData.AddNameChangeCard(card);
        
        dataManager.CreateNameChangeEvent("card_create", "system", card.CardID, "创建改名卡: " + cardType);
        dataManager.SaveNameChangeData();
        Debug.Log("创建改名卡成功: " + cardType);
        return card.CardID;
    }

    public void AddNameChangeCard(string playerID, string cardID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            Debug.LogError("玩家数据不存在");
            return;
        }
        
        NameChangeCard card = nameChangeData.AllNameChangeCards.Find(c => c.CardID == cardID);
        if (card == null)
        {
            Debug.LogError("改名卡不存在: " + cardID);
            return;
        }
        
        PlayerNameChangeData playerNameChangeData = nameChangeData.PlayerNameChangeData[playerID];
        if (playerNameChangeData.NameChangeCards.Count >= nameChangeData.MaxCardsPerPlayer)
        {
            Debug.LogError("改名卡数量达到上限");
            return;
        }
        
        playerNameChangeData.NameChangeCards.Add(card);
        playerNameChangeData.TotalCardsAcquired++;
        playerNameChangeData.LastCardAcquisitionTime = DateTime.Now;
        
        dataManager.CreateNameChangeEvent("card_add", playerID, cardID, "添加改名卡: " + card.CardType);
        dataManager.SaveNameChangeData();
        Debug.Log("添加改名卡成功: " + card.CardType);
    }

    public bool ChangeName(string playerID, string cardID, string newName, string changeReason)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            Debug.LogError("玩家数据不存在");
            return false;
        }
        
        PlayerNameChangeData playerNameChangeData = nameChangeData.PlayerNameChangeData[playerID];
        NameChangeCard card = playerNameChangeData.NameChangeCards.Find(c => c.CardID == cardID);
        if (card == null)
        {
            Debug.LogError("改名卡不存在");
            return false;
        }
        
        if (card.IsUsed)
        {
            Debug.LogError("改名卡已使用");
            return false;
        }
        
        if (card.ExpiryDate < DateTime.Now)
        {
            Debug.LogError("改名卡已过期");
            return false;
        }
        
        if (playerNameChangeData.NextAvailableChange > DateTime.Now)
        {
            Debug.LogError("改名冷却中，剩余时间: " + (playerNameChangeData.NextAvailableChange - DateTime.Now).Days + "天");
            return false;
        }
        
        if (!IsValidName(newName))
        {
            Debug.LogError("昵称不符合要求");
            return false;
        }
        
        if (IsNameBanned(newName))
        {
            Debug.LogError("昵称已被禁止");
            return false;
        }
        
        if (IsNameInUse(newName))
        {
            Debug.LogError("昵称已被使用");
            return false;
        }
        
        string oldName = playerNameChangeData.CurrentName;
        playerNameChangeData.CurrentName = newName;
        card.IsUsed = true;
        card.UsedDate = DateTime.Now;
        
        playerNameChangeData.NextAvailableChange = DateTime.Now.AddDays(playerNameChangeData.NameChangeCooldownDays);
        playerNameChangeData.TotalCardsUsed++;
        playerNameChangeData.TotalNameChanges++;
        playerNameChangeData.LastCardUseTime = DateTime.Now;
        playerNameChangeData.LastNameChangeTime = DateTime.Now;
        
        NameChangeRecord record = new NameChangeRecord(playerID, cardID, oldName, newName, changeReason, true);
        nameChangeData.AddNameChangeRecord(record);
        playerNameChangeData.NameChangeRecords.Add(record);
        
        dataManager.CreateNameChangeEvent("name_change", playerID, cardID, "修改昵称: " + oldName + " -> " + newName);
        dataManager.SaveNameChangeData();
        Debug.Log("修改昵称成功: " + oldName + " -> " + newName);
        return true;
    }

    public void RemoveNameChangeCard(string playerID, string cardID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerNameChangeData playerNameChangeData = nameChangeData.PlayerNameChangeData[playerID];
        NameChangeCard card = playerNameChangeData.NameChangeCards.Find(c => c.CardID == cardID);
        if (card != null)
        {
            playerNameChangeData.NameChangeCards.Remove(card);
            
            dataManager.CreateNameChangeEvent("card_remove", playerID, cardID, "移除改名卡: " + card.CardType);
            dataManager.SaveNameChangeData();
            Debug.Log("移除改名卡成功: " + card.CardType);
        }
    }

    public List<NameChangeCard> GetPlayerNameChangeCards(string playerID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return new List<NameChangeCard>();
        }
        return nameChangeData.PlayerNameChangeData[playerID].NameChangeCards;
    }

    public List<NameChangeCard> GetPlayerActiveNameChangeCards(string playerID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return new List<NameChangeCard>();
        }
        
        DateTime now = DateTime.Now;
        return nameChangeData.PlayerNameChangeData[playerID].NameChangeCards.FindAll(c => !c.IsUsed && c.ExpiryDate > now);
    }

    public List<NameChangeCard> GetPlayerExpiredNameChangeCards(string playerID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return new List<NameChangeCard>();
        }
        
        DateTime now = DateTime.Now;
        return nameChangeData.PlayerNameChangeData[playerID].NameChangeCards.FindAll(c => !c.IsUsed && c.ExpiryDate <= now);
    }

    public List<NameChangeCard> GetPlayerUsedNameChangeCards(string playerID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return new List<NameChangeCard>();
        }
        return nameChangeData.PlayerNameChangeData[playerID].NameChangeCards.FindAll(c => c.IsUsed);
    }

    public List<NameChangeRecord> GetPlayerNameChangeRecords(string playerID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return new List<NameChangeRecord>();
        }
        return nameChangeData.PlayerNameChangeData[playerID].NameChangeRecords;
    }

    public NameChangeCard GetNameChangeCard(string cardID)
    {
        return nameChangeData.AllNameChangeCards.Find(c => c.CardID == cardID);
    }

    public PlayerNameChangeData GetPlayerNameChangeData(string playerID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return null;
        }
        return nameChangeData.PlayerNameChangeData[playerID];
    }

    public string GetPlayerCurrentName(string playerID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return "";
        }
        return nameChangeData.PlayerNameChangeData[playerID].CurrentName;
    }

    public bool IsNameChangeAvailable(string playerID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return false;
        }
        
        PlayerNameChangeData playerNameChangeData = nameChangeData.PlayerNameChangeData[playerID];
        return playerNameChangeData.NextAvailableChange <= DateTime.Now;
    }

    public DateTime GetNextAvailableChange(string playerID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return DateTime.MinValue;
        }
        return nameChangeData.PlayerNameChangeData[playerID].NextAvailableChange;
    }

    public bool IsValidName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }
        
        if (name.Length < nameChangeData.MinNameLength || name.Length > nameChangeData.MaxNameLength)
        {
            return false;
        }
        
        foreach (char c in name)
        {
            if (!char.IsLetterOrDigit(c) && c != '_' && c != ' ')
            {
                return false;
            }
        }
        
        return true;
    }

    public bool IsNameBanned(string name)
    {
        return nameChangeData.BannedNames.Contains(name.ToLower());
    }

    public bool IsNameInUse(string name)
    {
        foreach (PlayerNameChangeData playerData in nameChangeData.PlayerNameChangeData.Values)
        {
            if (playerData.CurrentName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public void CleanupExpiredCards(string playerID)
    {
        if (!nameChangeData.PlayerNameChangeData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerNameChangeData playerNameChangeData = nameChangeData.PlayerNameChangeData[playerID];
        DateTime now = DateTime.Now;
        List<NameChangeCard> expiredCards = playerNameChangeData.NameChangeCards.FindAll(c => !c.IsUsed && c.ExpiryDate <= now);
        
        foreach (NameChangeCard card in expiredCards)
        {
            playerNameChangeData.NameChangeCards.Remove(card);
        }
        
        if (expiredCards.Count > 0)
        {
            dataManager.CreateNameChangeEvent("card_cleanup", playerID, "", "清理过期改名卡: " + expiredCards.Count);
            dataManager.SaveNameChangeData();
            Debug.Log("清理过期改名卡成功: " + expiredCards.Count);
        }
    }

    public void CleanupAllExpiredCards()
    {
        int totalCleaned = 0;
        DateTime now = DateTime.Now;
        
        foreach (PlayerNameChangeData playerNameChangeData in nameChangeData.PlayerNameChangeData.Values)
        {
            List<NameChangeCard> expiredCards = playerNameChangeData.NameChangeCards.FindAll(c => !c.IsUsed && c.ExpiryDate <= now);
            foreach (NameChangeCard card in expiredCards)
            {
                playerNameChangeData.NameChangeCards.Remove(card);
                totalCleaned++;
            }
        }
        
        if (totalCleaned > 0)
        {
            dataManager.CreateNameChangeEvent("card_cleanup_all", "system", "", "清理所有过期改名卡: " + totalCleaned);
            dataManager.SaveNameChangeData();
            Debug.Log("清理所有过期改名卡成功: " + totalCleaned);
        }
    }

    public List<string> GetCardTypes()
    {
        return nameChangeData.CardTypes;
    }

    public void AddCardType(string cardType)
    {
        if (!nameChangeData.CardTypes.Contains(cardType))
        {
            nameChangeData.CardTypes.Add(cardType);
            dataManager.SaveNameChangeData();
            Debug.Log("添加改名卡类型成功: " + cardType);
        }
    }

    public void RemoveCardType(string cardType)
    {
        if (cardType != "normal" && nameChangeData.CardTypes.Contains(cardType))
        {
            nameChangeData.CardTypes.Remove(cardType);
            dataManager.SaveNameChangeData();
            Debug.Log("删除改名卡类型成功: " + cardType);
        }
    }

    public void AddBannedName(string name)
    {
        if (!nameChangeData.BannedNames.Contains(name.ToLower()))
        {
            nameChangeData.BannedNames.Add(name.ToLower());
            dataManager.SaveNameChangeData();
            Debug.Log("添加禁止昵称成功: " + name);
        }
    }

    public void RemoveBannedName(string name)
    {
        if (nameChangeData.BannedNames.Contains(name.ToLower()))
        {
            nameChangeData.BannedNames.Remove(name.ToLower());
            dataManager.SaveNameChangeData();
            Debug.Log("删除禁止昵称成功: " + name);
        }
    }

    public List<string> GetBannedNames()
    {
        return nameChangeData.BannedNames;
    }

    public void SaveData()
    {
        dataManager.SaveNameChangeData();
    }

    public void LoadData()
    {
        dataManager.LoadNameChangeData();
    }

    public List<NameChangeEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
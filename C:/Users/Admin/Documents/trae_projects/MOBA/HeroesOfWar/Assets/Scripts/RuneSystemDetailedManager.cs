using System;
using System.Collections.Generic;

public class RuneSystemDetailedManager
{
    private static RuneSystemDetailedManager _instance;
    public static RuneSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RuneSystemDetailedManager();
            }
            return _instance;
        }
    }

    private RuneSystemData runeData;
    private RuneSystemDataManager dataManager;

    private RuneSystemDetailedManager()
    {
        dataManager = RuneSystemDataManager.Instance;
        runeData = dataManager.runeData;
    }

    public void InitializePlayerInventory(string playerID)
    {
        if (!runeData.PlayerInventories.ContainsKey(playerID))
        {
            PlayerRuneInventory inventory = new PlayerRuneInventory(playerID);
            runeData.AddPlayerInventory(playerID, inventory);
            dataManager.SaveRuneData();
            Debug.Log("初始化铭文背包成功");
        }
    }

    public void AddRuneToInventory(string playerID, string runeID, int quantity = 1)
    {
        InitializePlayerInventory(playerID);
        PlayerRuneInventory inventory = runeData.PlayerInventories[playerID];
        
        if (inventory.RuneCounts.ContainsKey(runeID))
        {
            inventory.RuneCounts[runeID] += quantity;
        }
        else
        {
            inventory.RuneCounts[runeID] = quantity;
        }
        inventory.TotalRunes += quantity;
        inventory.LastUpdateTime = DateTime.Now;
        
        dataManager.CreateRuneEvent("rune_add", playerID, runeID, "添加铭文: " + quantity);
        dataManager.SaveRuneData();
        Debug.Log("添加铭文成功: " + quantity);
    }

    public void RemoveRuneFromInventory(string playerID, string runeID, int quantity = 1)
    {
        if (runeData.PlayerInventories.ContainsKey(playerID))
        {
            PlayerRuneInventory inventory = runeData.PlayerInventories[playerID];
            if (inventory.RuneCounts.ContainsKey(runeID) && inventory.RuneCounts[runeID] >= quantity)
            {
                inventory.RuneCounts[runeID] -= quantity;
                if (inventory.RuneCounts[runeID] == 0)
                {
                    inventory.RuneCounts.Remove(runeID);
                }
                inventory.TotalRunes -= quantity;
                inventory.LastUpdateTime = DateTime.Now;
                
                dataManager.CreateRuneEvent("rune_remove", playerID, runeID, "移除铭文: " + quantity);
                dataManager.SaveRuneData();
                Debug.Log("移除铭文成功: " + quantity);
            }
        }
    }

    public string CreateRunePage(string playerID, string pageName)
    {
        InitializePlayerInventory(playerID);
        PlayerRuneInventory inventory = runeData.PlayerInventories[playerID];
        
        if (inventory.RunePages.Count >= inventory.MaxPages)
        {
            Debug.LogError("铭文页数量已达上限");
            return "";
        }
        
        string pageID = "page_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RunePage page = new RunePage(pageID, playerID, pageName);
        inventory.RunePages.Add(page);
        inventory.LastUpdateTime = DateTime.Now;
        
        dataManager.CreateRuneEvent("rune_page_create", playerID, pageID, "创建铭文页: " + pageName);
        dataManager.SaveRuneData();
        Debug.Log("创建铭文页成功: " + pageName);
        return pageID;
    }

    public void DeleteRunePage(string playerID, string pageID)
    {
        if (runeData.PlayerInventories.ContainsKey(playerID))
        {
            PlayerRuneInventory inventory = runeData.PlayerInventories[playerID];
            RunePage page = inventory.RunePages.Find(p => p.PageID == pageID);
            if (page != null && inventory.RunePages.Count > 1)
            {
                inventory.RunePages.Remove(page);
                inventory.LastUpdateTime = DateTime.Now;
                
                dataManager.CreateRuneEvent("rune_page_delete", playerID, pageID, "删除铭文页");
                dataManager.SaveRuneData();
                Debug.Log("删除铭文页成功");
            }
        }
    }

    public void UpdateRunePageName(string playerID, string pageID, string newName)
    {
        if (runeData.PlayerInventories.ContainsKey(playerID))
        {
            PlayerRuneInventory inventory = runeData.PlayerInventories[playerID];
            RunePage page = inventory.RunePages.Find(p => p.PageID == pageID);
            if (page != null)
            {
                page.PageName = newName;
                page.LastUpdateTime = DateTime.Now;
                inventory.LastUpdateTime = DateTime.Now;
                
                dataManager.SaveRuneData();
                Debug.Log("更新铭文页名称成功: " + newName);
            }
        }
    }

    public void SetRuneInPage(string playerID, string pageID, int slotIndex, string runeID)
    {
        if (runeData.PlayerInventories.ContainsKey(playerID))
        {
            PlayerRuneInventory inventory = runeData.PlayerInventories[playerID];
            RunePage page = inventory.RunePages.Find(p => p.PageID == pageID);
            if (page != null && slotIndex >= 0 && slotIndex < page.RuneSlots.Count)
            {
                if (!string.IsNullOrEmpty(runeID))
                {
                    if (inventory.RuneCounts.ContainsKey(runeID) && inventory.RuneCounts[runeID] > 0)
                    {
                        page.RuneSlots[slotIndex] = runeID;
                        page.LastUpdateTime = DateTime.Now;
                        inventory.LastUpdateTime = DateTime.Now;
                        UpdatePageAttributes(page);
                        
                        dataManager.SaveRuneData();
                        Debug.Log("设置铭文成功");
                    }
                    else
                    {
                        Debug.LogError("铭文不足");
                    }
                }
                else
                {
                    page.RuneSlots[slotIndex] = "";
                    page.LastUpdateTime = DateTime.Now;
                    inventory.LastUpdateTime = DateTime.Now;
                    UpdatePageAttributes(page);
                    
                    dataManager.SaveRuneData();
                    Debug.Log("移除铭文成功");
                }
            }
        }
    }

    private void UpdatePageAttributes(RunePage page)
    {
        int totalAttributes = 0;
        foreach (string runeID in page.RuneSlots)
        {
            if (!string.IsNullOrEmpty(runeID))
            {
                Rune rune = GetRune(runeID);
                if (rune != null)
                {
                    foreach (KeyValuePair<string, int> attribute in rune.Attributes)
                    {
                        totalAttributes += attribute.Value;
                    }
                }
            }
        }
        page.TotalAttributes = totalAttributes;
    }

    public Rune GetRune(string runeID)
    {
        return runeData.AllRunes.Find(r => r.RuneID == runeID);
    }

    public List<Rune> GetAllRunes()
    {
        return runeData.AllRunes;
    }

    public List<Rune> GetRunesByType(int runeType)
    {
        return runeData.AllRunes.FindAll(r => r.RuneType == runeType);
    }

    public List<RuneSet> GetAllRuneSets()
    {
        return runeData.RuneSets;
    }

    public RuneSet GetRuneSet(string setID)
    {
        return runeData.RuneSets.Find(s => s.SetID == setID);
    }

    public PlayerRuneInventory GetPlayerInventory(string playerID)
    {
        if (runeData.PlayerInventories.ContainsKey(playerID))
        {
            return runeData.PlayerInventories[playerID];
        }
        return null;
    }

    public List<RunePage> GetPlayerRunePages(string playerID)
    {
        PlayerRuneInventory inventory = GetPlayerInventory(playerID);
        if (inventory != null)
        {
            return inventory.RunePages;
        }
        return new List<RunePage>();
    }

    public RunePage GetRunePage(string playerID, string pageID)
    {
        PlayerRuneInventory inventory = GetPlayerInventory(playerID);
        if (inventory != null)
        {
            return inventory.RunePages.Find(p => p.PageID == pageID);
        }
        return null;
    }

    public int GetRuneCount(string playerID, string runeID)
    {
        PlayerRuneInventory inventory = GetPlayerInventory(playerID);
        if (inventory != null && inventory.RuneCounts.ContainsKey(runeID))
        {
            return inventory.RuneCounts[runeID];
        }
        return 0;
    }

    public Dictionary<string, int> GetPlayerRunes(string playerID)
    {
        PlayerRuneInventory inventory = GetPlayerInventory(playerID);
        if (inventory != null)
        {
            return inventory.RuneCounts;
        }
        return new Dictionary<string, int>();
    }

    public void UpgradeRune(string playerID, string runeID)
    {
        Rune rune = GetRune(runeID);
        if (rune != null && rune.Level < 5)
        {
            PlayerRuneInventory inventory = GetPlayerInventory(playerID);
            if (inventory != null && inventory.RuneCounts.ContainsKey(runeID) && inventory.RuneCounts[runeID] >= 3)
            {
                RemoveRuneFromInventory(playerID, runeID, 3);
                
                string newRuneID = runeID + "_" + (rune.Level + 1);
                Rune upgradedRune = GetRune(newRuneID);
                if (upgradedRune == null)
                {
                    
                    Dictionary<string, int> newAttributes = new Dictionary<string, int>();
                    foreach (KeyValuePair<string, int> attribute in rune.Attributes)
                    {
                        newAttributes[attribute.Key] = attribute.Value + 2;
                    }
                    upgradedRune = new Rune(newRuneID, rune.RuneName + "+", rune.Description, rune.RuneType, rune.Level + 1, rune.Cost + 500, newAttributes, rune.SetID);
                    runeData.AddRune(upgradedRune);
                }
                
                AddRuneToInventory(playerID, newRuneID, 1);
                
                dataManager.CreateRuneEvent("rune_upgrade", playerID, runeID, "升级铭文");
                dataManager.SaveRuneData();
                Debug.Log("升级铭文成功");
            }
        }
    }

    public void CreateRune(string runeName, string description, int runeType, int level, int cost, Dictionary<string, int> attributes, string setID = "")
    {
        string runeID = "rune_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Rune rune = new Rune(runeID, runeName, description, runeType, level, cost, attributes, setID);
        runeData.AddRune(rune);
        dataManager.SaveRuneData();
        Debug.Log("创建铭文成功: " + runeName);
    }

    public void CreateRuneSet(string setName, string description, List<string> runeIDs, Dictionary<string, int> setBonus, int bonusRequirement)
    {
        string setID = "set_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        RuneSet runeSet = new RuneSet(setID, setName, description, runeIDs, setBonus, bonusRequirement);
        runeData.AddRuneSet(runeSet);
        dataManager.SaveRuneData();
        Debug.Log("创建铭文套装成功: " + setName);
    }

    public void SaveData()
    {
        dataManager.SaveRuneData();
    }

    public void LoadData()
    {
        dataManager.LoadRuneData();
    }

    public List<RuneEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
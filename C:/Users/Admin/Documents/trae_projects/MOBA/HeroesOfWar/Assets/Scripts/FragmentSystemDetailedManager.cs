using System;
using System.Collections.Generic;
using UnityEngine;

public class FragmentSystemDetailedManager
{
    private static FragmentSystemDetailedManager _instance;
    public static FragmentSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FragmentSystemDetailedManager();
            }
            return _instance;
        }
    }

    private FragmentSystemData fragmentData;
    private FragmentSystemDataManager dataManager;

    private FragmentSystemDetailedManager()
    {
        dataManager = FragmentSystemDataManager.Instance;
        fragmentData = dataManager.fragmentData;
    }

    public void InitializePlayerFragmentData(string playerID)
    {
        if (!fragmentData.PlayerFragmentData.ContainsKey(playerID))
        {
            PlayerFragmentData playerFragmentData = new PlayerFragmentData(playerID);
            fragmentData.AddPlayerFragmentData(playerID, playerFragmentData);
            dataManager.SaveFragmentData();
            Debug.Log("初始化碎片数据成功");
        }
    }

    public string CreateFragment(string fragmentType, string associatedID, string associatedName, int requiredCount, string rarity, string description, string iconURL)
    {
        Fragment fragment = new Fragment(fragmentType, associatedID, associatedName, requiredCount, rarity, description, iconURL);
        fragmentData.AddFragment(fragment);
        
        dataManager.CreateFragmentEvent("fragment_create", "system", fragment.FragmentID, "创建碎片: " + fragment.AssociatedName);
        dataManager.SaveFragmentData();
        Debug.Log("创建碎片成功: " + fragment.AssociatedName);
        return fragment.FragmentID;
    }

    public void AddFragments(string playerID, string fragmentID, int quantity)
    {
        if (!fragmentData.PlayerFragmentData.ContainsKey(playerID))
        {
            InitializePlayerFragmentData(playerID);
        }
        
        if (!fragmentData.AllFragments.Exists(f => f.FragmentID == fragmentID))
        {
            Debug.LogError("碎片不存在: " + fragmentID);
            return;
        }
        
        PlayerFragmentData playerFragmentData = fragmentData.PlayerFragmentData[playerID];
        if (playerFragmentData.FragmentInventory.ContainsKey(fragmentID))
        {
            playerFragmentData.FragmentInventory[fragmentID] += quantity;
        }
        else
        {
            playerFragmentData.FragmentInventory[fragmentID] = quantity;
        }
        
        playerFragmentData.TotalFragmentsCollected += quantity;
        playerFragmentData.LastFragmentAcquisitionTime = DateTime.Now;
        
        dataManager.CreateFragmentEvent("fragment_add", playerID, fragmentID, "添加碎片: " + quantity + "个");
        dataManager.SaveFragmentData();
        Debug.Log("添加碎片成功: " + quantity + "个");
    }

    public void RemoveFragments(string playerID, string fragmentID, int quantity)
    {
        if (!fragmentData.PlayerFragmentData.ContainsKey(playerID))
        {
            return;
        }
        
        PlayerFragmentData playerFragmentData = fragmentData.PlayerFragmentData[playerID];
        if (playerFragmentData.FragmentInventory.ContainsKey(fragmentID))
        {
            if (playerFragmentData.FragmentInventory[fragmentID] >= quantity)
            {
                playerFragmentData.FragmentInventory[fragmentID] -= quantity;
                if (playerFragmentData.FragmentInventory[fragmentID] <= 0)
                {
                    playerFragmentData.FragmentInventory.Remove(fragmentID);
                }
                
                dataManager.CreateFragmentEvent("fragment_remove", playerID, fragmentID, "移除碎片: " + quantity + "个");
                dataManager.SaveFragmentData();
                Debug.Log("移除碎片成功: " + quantity + "个");
            }
            else
            {
                Debug.LogError("碎片数量不足");
            }
        }
        else
        {
            Debug.LogError("碎片不存在");
        }
    }

    public string CreateExchangeOffer(string fragmentID, int requiredFragments, string rewardType, string rewardID, string rewardName, int rewardQuantity, string offerType, DateTime startTime, DateTime endTime, int maxExchanges)
    {
        if (!fragmentData.AllFragments.Exists(f => f.FragmentID == fragmentID))
        {
            Debug.LogError("碎片不存在: " + fragmentID);
            return "";
        }
        
        ExchangeOffer offer = new ExchangeOffer(fragmentID, requiredFragments, rewardType, rewardID, rewardName, rewardQuantity, offerType, startTime, endTime, maxExchanges);
        fragmentData.AddExchangeOffer(offer);
        
        dataManager.CreateFragmentEvent("offer_create", "system", fragmentID, "创建兑换活动: " + rewardName);
        dataManager.SaveFragmentData();
        Debug.Log("创建兑换活动成功: " + rewardName);
        return offer.OfferID;
    }

    public bool ExchangeFragments(string playerID, string offerID)
    {
        if (!fragmentData.PlayerFragmentData.ContainsKey(playerID))
        {
            InitializePlayerFragmentData(playerID);
        }
        
        ExchangeOffer offer = fragmentData.ExchangeOffers.Find(o => o.OfferID == offerID);
        if (offer == null)
        {
            Debug.LogError("兑换活动不存在");
            return false;
        }
        
        if (!offer.IsActive)
        {
            Debug.LogError("兑换活动已结束");
            return false;
        }
        
        if (offer.CurrentExchanges >= offer.MaxExchanges)
        {
            Debug.LogError("兑换活动已达上限");
            return false;
        }
        
        if (DateTime.Now < offer.StartTime || DateTime.Now > offer.EndTime)
        {
            Debug.LogError("兑换活动不在有效期内");
            return false;
        }
        
        PlayerFragmentData playerFragmentData = fragmentData.PlayerFragmentData[playerID];
        if (!playerFragmentData.FragmentInventory.ContainsKey(offer.FragmentID) || playerFragmentData.FragmentInventory[offer.FragmentID] < offer.RequiredFragments)
        {
            Debug.LogError("碎片数量不足");
            return false;
        }
        
        RemoveFragments(playerID, offer.FragmentID, offer.RequiredFragments);
        offer.CurrentExchanges++;
        playerFragmentData.ExchangeHistory.Add(offer.OfferID);
        playerFragmentData.TotalExchanges++;
        playerFragmentData.LastExchangeTime = DateTime.Now;
        
        dataManager.CreateFragmentEvent("fragment_exchange", playerID, offer.FragmentID, "兑换奖励: " + offer.RewardName);
        dataManager.SaveFragmentData();
        Debug.Log("兑换奖励成功: " + offer.RewardName);
        return true;
    }

    public int GetFragmentCount(string playerID, string fragmentID)
    {
        if (!fragmentData.PlayerFragmentData.ContainsKey(playerID))
        {
            return 0;
        }
        
        PlayerFragmentData playerFragmentData = fragmentData.PlayerFragmentData[playerID];
        if (playerFragmentData.FragmentInventory.ContainsKey(fragmentID))
        {
            return playerFragmentData.FragmentInventory[fragmentID];
        }
        return 0;
    }

    public Dictionary<string, int> GetPlayerFragments(string playerID)
    {
        if (!fragmentData.PlayerFragmentData.ContainsKey(playerID))
        {
            InitializePlayerFragmentData(playerID);
        }
        return fragmentData.PlayerFragmentData[playerID].FragmentInventory;
    }

    public List<Fragment> GetAllFragments()
    {
        return fragmentData.AllFragments;
    }

    public List<Fragment> GetFragmentsByType(string fragmentType)
    {
        return fragmentData.AllFragments.FindAll(f => f.FragmentType == fragmentType);
    }

    public List<ExchangeOffer> GetActiveExchangeOffers()
    {
        DateTime now = DateTime.Now;
        return fragmentData.ExchangeOffers.FindAll(o => o.IsActive && now >= o.StartTime && now <= o.EndTime && o.CurrentExchanges < o.MaxExchanges);
    }

    public List<ExchangeOffer> GetAllExchangeOffers()
    {
        return fragmentData.ExchangeOffers;
    }

    public PlayerFragmentData GetPlayerFragmentData(string playerID)
    {
        if (!fragmentData.PlayerFragmentData.ContainsKey(playerID))
        {
            InitializePlayerFragmentData(playerID);
        }
        return fragmentData.PlayerFragmentData[playerID];
    }

    public Fragment GetFragment(string fragmentID)
    {
        return fragmentData.AllFragments.Find(f => f.FragmentID == fragmentID);
    }

    public ExchangeOffer GetExchangeOffer(string offerID)
    {
        return fragmentData.ExchangeOffers.Find(o => o.OfferID == offerID);
    }

    public void DeactivateExchangeOffer(string offerID)
    {
        ExchangeOffer offer = fragmentData.ExchangeOffers.Find(o => o.OfferID == offerID);
        if (offer != null)
        {
            offer.IsActive = false;
            dataManager.CreateFragmentEvent("offer_deactivate", "system", "", "停用兑换活动: " + offer.RewardName);
            dataManager.SaveFragmentData();
            Debug.Log("停用兑换活动成功: " + offer.RewardName);
        }
    }

    public void UpdateExchangeOffer(string offerID, int maxExchanges, DateTime endTime)
    {
        ExchangeOffer offer = fragmentData.ExchangeOffers.Find(o => o.OfferID == offerID);
        if (offer != null)
        {
            offer.MaxExchanges = maxExchanges;
            offer.EndTime = endTime;
            dataManager.CreateFragmentEvent("offer_update", "system", "", "更新兑换活动: " + offer.RewardName);
            dataManager.SaveFragmentData();
            Debug.Log("更新兑换活动成功: " + offer.RewardName);
        }
    }

    public List<string> GetFragmentTypes()
    {
        return fragmentData.FragmentTypes;
    }

    public void AddFragmentType(string fragmentType)
    {
        if (!fragmentData.FragmentTypes.Contains(fragmentType))
        {
            fragmentData.FragmentTypes.Add(fragmentType);
            dataManager.SaveFragmentData();
            Debug.Log("添加碎片类型成功: " + fragmentType);
        }
    }

    public void RemoveFragmentType(string fragmentType)
    {
        if (fragmentType != "hero" && fragmentType != "skin" && fragmentData.FragmentTypes.Contains(fragmentType))
        {
            fragmentData.FragmentTypes.Remove(fragmentType);
            dataManager.SaveFragmentData();
            Debug.Log("删除碎片类型成功: " + fragmentType);
        }
    }

    public void CleanupExpiredOffers()
    {
        DateTime now = DateTime.Now;
        List<ExchangeOffer> expiredOffers = fragmentData.ExchangeOffers.FindAll(o => o.IsActive && now > o.EndTime);
        foreach (ExchangeOffer offer in expiredOffers)
        {
            offer.IsActive = false;
        }
        
        if (expiredOffers.Count > 0)
        {
            dataManager.CreateFragmentEvent("offer_cleanup", "system", "", "清理过期兑换活动: " + expiredOffers.Count);
            dataManager.SaveFragmentData();
            Debug.Log("清理过期兑换活动成功: " + expiredOffers.Count);
        }
    }

    public void CleanupOldFragments(int days = 30)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-days);
        List<Fragment> oldFragments = fragmentData.AllFragments.FindAll(f => f.AddedDate < cutoffDate);
        foreach (Fragment fragment in oldFragments)
        {
            fragmentData.AllFragments.Remove(fragment);
            
            foreach (PlayerFragmentData playerFragmentData in fragmentData.PlayerFragmentData.Values)
            {
                if (playerFragmentData.FragmentInventory.ContainsKey(fragment.FragmentID))
                {
                    playerFragmentData.FragmentInventory.Remove(fragment.FragmentID);
                }
            }
        }
        
        if (oldFragments.Count > 0)
        {
            dataManager.CreateFragmentEvent("fragment_cleanup", "system", "", "清理旧碎片: " + oldFragments.Count);
            dataManager.SaveFragmentData();
            Debug.Log("清理旧碎片成功: " + oldFragments.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveFragmentData();
    }

    public void LoadData()
    {
        dataManager.LoadFragmentData();
    }

    public List<FragmentEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
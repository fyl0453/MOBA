using System;
using System.Collections.Generic;
using System.Linq;

public class HonorZoneSystemDetailedManager
{
    private static HonorZoneSystemDetailedManager _instance;
    public static HonorZoneSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HonorZoneSystemDetailedManager();
            }
            return _instance;
        }
    }

    private HonorZoneData honorData;
    private HonorZoneDataManager dataManager;

    private HonorZoneSystemDetailedManager()
    {
        dataManager = HonorZoneDataManager.Instance;
        honorData = dataManager.honorData;
    }

    public void InitializePlayerHonorData(string playerID)
    {
        if (!honorData.PlayerHonorData.ContainsKey(playerID))
        {
            PlayerHonorData playerData = new PlayerHonorData(playerID);
            honorData.AddPlayerHonorData(playerID, playerData);
            dataManager.SaveHonorData();
            Debug.Log("初始化荣耀战区数据成功");
        }
    }

    public string SetPlayerZone(string playerID, string zoneID)
    {
        HonorZone zone = honorData.AllZones.Find(z => z.ZoneID == zoneID && z.IsActive);
        if (zone == null)
        {
            Debug.LogError("战区不存在或已停用");
            return "";
        }

        InitializePlayerHonorData(playerID);
        PlayerHonorData playerData = honorData.PlayerHonorData[playerID];
        
        string oldZoneID = playerData.CurrentZoneID;
        playerData.CurrentZoneID = zoneID;
        playerData.LastZoneUpdate = DateTime.Now;
        
        zone.Population++;
        if (!string.IsNullOrEmpty(oldZoneID))
        {
            HonorZone oldZone = honorData.AllZones.Find(z => z.ZoneID == oldZoneID);
            if (oldZone != null && oldZone.Population > 0)
            {
                oldZone.Population--;
            }
        }
        
        dataManager.CreateHonorEvent("zone_set", playerID, zoneID, "设置战区: " + zone.ZoneName);
        dataManager.SaveHonorData();
        Debug.Log("设置战区成功: " + zone.ZoneName);
        return zoneID;
    }

    public void UpdateHonorPoints(string playerID, string heroID, string heroName, bool isWin, int performanceScore = 0)
    {
        InitializePlayerHonorData(playerID);
        PlayerHonorData playerData = honorData.PlayerHonorData[playerID];
        
        if (string.IsNullOrEmpty(playerData.CurrentZoneID))
        {
            Debug.LogError("玩家未设置战区");
            return;
        }
        
        int pointsChange = isWin ? honorData.HonorPointsPerWin : honorData.HonorPointsPerLose;
        if (performanceScore > 80)
        {
            pointsChange += 2;
        }
        else if (performanceScore < 30)
        {
            pointsChange -= 1;
        }
        
        HonorRank existingRank = playerData.HeroRanks.Find(r => r.HeroID == heroID);
        if (existingRank != null)
        {
            existingRank.HonorPoints += pointsChange;
            existingRank.UpdateTime = DateTime.Now;
        }
        else
        {
            string rankID = "rank_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            HonorRank newRank = new HonorRank(rankID, playerID, "", playerData.CurrentZoneID, heroID, heroName, pointsChange);
            playerData.HeroRanks.Add(newRank);
            honorData.AddRank(newRank);
        }
        
        playerData.TotalHonorPoints += pointsChange;
        playerData.LastRankUpdate = DateTime.Now;
        
        UpdateHeroRankings(playerData.CurrentZoneID, heroID);
        UpdatePlayerTitles(playerID);
        
        dataManager.CreateHonorEvent("honor_update", playerID, playerData.CurrentZoneID, "更新荣耀积分: " + pointsChange);
        dataManager.SaveHonorData();
        Debug.Log("更新荣耀积分成功: " + pointsChange);
    }

    public void UpdateHeroRankings(string zoneID, string heroID)
    {
        List<HonorRank> heroRanks = honorData.AllRanks.FindAll(r => r.ZoneID == zoneID && r.HeroID == heroID);
        heroRanks.Sort((a, b) => b.HonorPoints.CompareTo(a.HonorPoints));
        
        for (int i = 0; i < heroRanks.Count; i++)
        {
            int oldRank = heroRanks[i].Rank;
            int newRank = i + 1;
            heroRanks[i].Rank = newRank;
            heroRanks[i].Change = oldRank > 0 ? oldRank - newRank : 0;
        }
    }

    public void UpdatePlayerTitles(string playerID)
    {
        InitializePlayerHonorData(playerID);
        PlayerHonorData playerData = honorData.PlayerHonorData[playerID];
        
        if (string.IsNullOrEmpty(playerData.CurrentZoneID))
        {
            return;
        }
        
        List<string> newTitles = new List<string>();
        foreach (HonorRank rank in playerData.HeroRanks)
        {
            HonorTitle title = GetTitleByRank(rank.Rank, "district");
            if (title != null && !newTitles.Contains(title.TitleName))
            {
                newTitles.Add(title.TitleName);
            }
        }
        
        playerData.CurrentTitles = newTitles;
    }

    public HonorTitle GetTitleByRank(int rank, string titleType)
    {
        return honorData.AllTitles.Find(t => t.TitleType == titleType && t.MinRank <= rank && t.MaxRank >= rank && t.IsActive);
    }

    public List<HonorRank> GetZoneHeroRanking(string zoneID, string heroID, int limit = 50)
    {
        List<HonorRank> ranks = honorData.AllRanks.FindAll(r => r.ZoneID == zoneID && r.HeroID == heroID);
        ranks.Sort((a, b) => a.Rank.CompareTo(b.Rank));
        if (limit < ranks.Count)
        {
            return ranks.GetRange(0, limit);
        }
        return ranks;
    }

    public List<HonorRank> GetPlayerHeroRanks(string playerID)
    {
        if (honorData.PlayerHonorData.ContainsKey(playerID))
        {
            return honorData.PlayerHonorData[playerID].HeroRanks;
        }
        return new List<HonorRank>();
    }

    public HonorRank GetPlayerHeroRank(string playerID, string heroID)
    {
        if (honorData.PlayerHonorData.ContainsKey(playerID))
        {
            return honorData.PlayerHonorData[playerID].HeroRanks.Find(r => r.HeroID == heroID);
        }
        return null;
    }

    public List<HonorZone> GetAllZones()
    {
        return honorData.AllZones.FindAll(z => z.IsActive);
    }

    public List<HonorZone> GetZonesByCity(string city)
    {
        return honorData.AllZones.FindAll(z => z.City == city && z.IsActive);
    }

    public List<HonorZone> GetZonesByProvince(string province)
    {
        return honorData.AllZones.FindAll(z => z.Province == province && z.IsActive);
    }

    public HonorZone GetZone(string zoneID)
    {
        return honorData.AllZones.Find(z => z.ZoneID == zoneID && z.IsActive);
    }

    public List<string> GetPlayerTitles(string playerID)
    {
        if (honorData.PlayerHonorData.ContainsKey(playerID))
        {
            return honorData.PlayerHonorData[playerID].CurrentTitles;
        }
        return new List<string>();
    }

    public int GetPlayerTotalHonorPoints(string playerID)
    {
        if (honorData.PlayerHonorData.ContainsKey(playerID))
        {
            return honorData.PlayerHonorData[playerID].TotalHonorPoints;
        }
        return 0;
    }

    public int GetPlayerHighestRank(string playerID)
    {
        if (honorData.PlayerHonorData.ContainsKey(playerID))
        {
            PlayerHonorData playerData = honorData.PlayerHonorData[playerID];
            if (playerData.HeroRanks.Count > 0)
            {
                return playerData.HeroRanks.Min(r => r.Rank);
            }
        }
        return 0;
    }

    public void AddZone(string zoneName, string province, string city, string district)
    {
        string zoneID = "zone_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HonorZone zone = new HonorZone(zoneID, zoneName, province, city, district);
        honorData.AddZone(zone);
        dataManager.SaveHonorData();
        Debug.Log("添加战区成功: " + zoneName);
    }

    public void UpdateZone(string zoneID, string zoneName, string province, string city, string district, bool isActive)
    {
        HonorZone zone = GetZone(zoneID);
        if (zone != null)
        {
            zone.ZoneName = zoneName;
            zone.Province = province;
            zone.City = city;
            zone.District = district;
            zone.IsActive = isActive;
            zone.LastUpdateTime = DateTime.Now;
            dataManager.SaveHonorData();
            Debug.Log("更新战区成功: " + zoneName);
        }
    }

    public void AddTitle(string titleName, string titleType, int minRank, int maxRank, string description)
    {
        string titleID = "title_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HonorTitle title = new HonorTitle(titleID, titleName, titleType, minRank, maxRank, description);
        honorData.AddTitle(title);
        dataManager.SaveHonorData();
        Debug.Log("添加称号成功: " + titleName);
    }

    public void UpdateTitle(string titleID, string titleName, string titleType, int minRank, int maxRank, string description, bool isActive)
    {
        HonorTitle title = honorData.AllTitles.Find(t => t.TitleID == titleID);
        if (title != null)
        {
            title.TitleName = titleName;
            title.TitleType = titleType;
            title.MinRank = minRank;
            title.MaxRank = maxRank;
            title.Description = description;
            title.IsActive = isActive;
            dataManager.SaveHonorData();
            Debug.Log("更新称号成功: " + titleName);
        }
    }

    public void RecalculateAllRankings()
    {
        foreach (HonorZone zone in honorData.AllZones)
        {
            if (zone.IsActive)
            {
                List<string> heroIDs = honorData.AllRanks.Where(r => r.ZoneID == zone.ZoneID).Select(r => r.HeroID).Distinct().ToList();
                foreach (string heroID in heroIDs)
                {
                    UpdateHeroRankings(zone.ZoneID, heroID);
                }
            }
        }
        
        foreach (string playerID in honorData.PlayerHonorData.Keys)
        {
            UpdatePlayerTitles(playerID);
        }
        
        dataManager.CreateHonorEvent("ranking_recalculate", "system", "", "重新计算所有排名");
        dataManager.SaveHonorData();
        Debug.Log("重新计算所有排名成功");
    }

    public void CleanupInactiveZones()
    {
        List<HonorZone> inactiveZones = honorData.AllZones.FindAll(z => !z.IsActive);
        foreach (HonorZone zone in inactiveZones)
        {
            honorData.AllZones.Remove(zone);
            
            List<HonorRank> zoneRanks = honorData.AllRanks.FindAll(r => r.ZoneID == zone.ZoneID);
            foreach (HonorRank rank in zoneRanks)
            {
                honorData.AllRanks.Remove(rank);
                if (honorData.PlayerHonorData.ContainsKey(rank.PlayerID))
                {
                    PlayerHonorData playerData = honorData.PlayerHonorData[rank.PlayerID];
                    playerData.HeroRanks.Remove(rank);
                }
            }
        }
        
        if (inactiveZones.Count > 0)
        {
            dataManager.CreateHonorEvent("zone_cleanup", "system", "", "清理无效战区: " + inactiveZones.Count);
            dataManager.SaveHonorData();
            Debug.Log("清理无效战区成功: " + inactiveZones.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveHonorData();
    }

    public void LoadData()
    {
        dataManager.LoadHonorData();
    }

    public List<HonorEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
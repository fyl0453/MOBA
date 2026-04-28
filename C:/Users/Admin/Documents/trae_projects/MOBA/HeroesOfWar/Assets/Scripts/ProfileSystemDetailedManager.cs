using System;
using System.Collections.Generic;

public class ProfileSystemDetailedManager
{
    private static ProfileSystemDetailedManager _instance;
    public static ProfileSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ProfileSystemDetailedManager();
            }
            return _instance;
        }
    }

    private ProfileSystemData profileData;
    private ProfileSystemDataManager dataManager;

    private ProfileSystemDetailedManager()
    {
        dataManager = ProfileSystemDataManager.Instance;
        profileData = dataManager.profileData;
    }

    public void CreatePlayerProfile(string playerID, string playerName)
    {
        PlayerProfile profile = new PlayerProfile(playerID, playerName);
        profileData.AddPlayerProfile(playerID, profile);
        
        PlayerStats stats = new PlayerStats(playerID);
        profileData.AddPlayerStats(playerID, stats);
        
        PlayerCollection collection = new PlayerCollection(playerID);
        profileData.AddPlayerCollection(playerID, collection);
        
        dataManager.CreateProfileEvent("profile_create", playerID, "创建个人主页");
        dataManager.SaveProfileData();
        Debug.Log("创建个人主页成功: " + playerName);
    }

    public void UpdatePlayerInfo(string playerID, string playerName, string avatar, string signature, string title, string frame, string background)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        if (profile != null)
        {
            if (!string.IsNullOrEmpty(playerName))
                profile.PlayerName = playerName;
            if (!string.IsNullOrEmpty(avatar))
                profile.Avatar = avatar;
            if (!string.IsNullOrEmpty(signature))
                profile.Signature = signature;
            if (!string.IsNullOrEmpty(title))
                profile.Title = title;
            if (!string.IsNullOrEmpty(frame))
                profile.Frame = frame;
            if (!string.IsNullOrEmpty(background))
                profile.Background = background;
            
            dataManager.CreateProfileEvent("profile_update", playerID, "更新个人信息");
            dataManager.SaveProfileData();
            Debug.Log("更新个人信息成功");
        }
    }

    public void UpdatePlayerLevel(string playerID, int level, int exp)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        if (profile != null)
        {
            profile.Level = level;
            profile.Exp = exp;
            dataManager.SaveProfileData();
        }
    }

    public void UpdatePlayerRank(string playerID, string currentRank, int maxRankLevel)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        if (profile != null)
        {
            profile.CurrentRank = currentRank;
            if (maxRankLevel > profile.MaxRankLevel)
            {
                profile.MaxRankLevel = maxRankLevel;
            }
            dataManager.SaveProfileData();
        }
    }

    public void UpdateMatchStats(string playerID, bool isWin, int kills, int deaths, int assists, string heroID, string modeID, int goldEarned, int damageDealt, int healingDone, int objectivesTaken, int minionsKilled)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        PlayerStats stats = GetPlayerStats(playerID);
        if (profile != null && stats != null)
        {
            profile.TotalGames++;
            if (isWin)
            {
                profile.TotalWins++;
            }
            else
            {
                profile.TotalLosses++;
            }
            profile.WinRate = (float)profile.TotalWins / profile.TotalGames * 100;
            
            stats.TotalKills += kills;
            stats.TotalDeaths += deaths;
            stats.TotalAssists += assists;
            if (stats.TotalDeaths > 0)
            {
                stats.KDA = (float)(stats.TotalKills + stats.TotalAssists) / stats.TotalDeaths;
            }
            stats.TotalGoldEarned += goldEarned;
            stats.TotalDamageDealt += damageDealt;
            stats.TotalHealingDone += healingDone;
            stats.TotalObjectivesTaken += objectivesTaken;
            stats.TotalMinionsKilled += minionsKilled;
            
            UpdateHeroStats(playerID, heroID, isWin, kills, deaths, assists, damageDealt, healingDone);
            UpdateModeStats(playerID, modeID, isWin, kills, deaths, assists);
            UpdateRecentHeroes(playerID, heroID);
            
            stats.LastUpdateTime = DateTime.Now;
            dataManager.SaveProfileData();
        }
    }

    private void UpdateHeroStats(string playerID, string heroID, bool isWin, int kills, int deaths, int assists, int damageDealt, int healingDone)
    {
        PlayerStats stats = GetPlayerStats(playerID);
        if (stats != null)
        {
            if (!stats.HeroStats.ContainsKey(heroID))
            {
                stats.HeroStats[heroID] = new HeroStats(heroID);
            }
            HeroStats heroStats = stats.HeroStats[heroID];
            heroStats.PlayCount++;
            if (isWin)
            {
                heroStats.WinCount++;
            }
            heroStats.WinRate = (float)heroStats.WinCount / heroStats.PlayCount * 100;
            heroStats.TotalKills += kills;
            heroStats.TotalDeaths += deaths;
            heroStats.TotalAssists += assists;
            if (heroStats.TotalDeaths > 0)
            {
                heroStats.KDA = (float)(heroStats.TotalKills + heroStats.TotalAssists) / heroStats.TotalDeaths;
            }
            heroStats.TotalDamageDealt += damageDealt;
            heroStats.TotalHealingDone += healingDone;
            heroStats.LastPlayed = DateTime.Now;
        }
    }

    private void UpdateModeStats(string playerID, string modeID, bool isWin, int kills, int deaths, int assists)
    {
        PlayerStats stats = GetPlayerStats(playerID);
        if (stats != null)
        {
            if (!stats.ModeStats.ContainsKey(modeID))
            {
                stats.ModeStats[modeID] = new ModeStats(modeID);
            }
            ModeStats modeStats = stats.ModeStats[modeID];
            modeStats.PlayCount++;
            modeStats.TotalGames++;
            if (isWin)
            {
                modeStats.WinCount++;
            }
            modeStats.WinRate = (float)modeStats.WinCount / modeStats.PlayCount * 100;
            modeStats.TotalKills += kills;
            modeStats.TotalDeaths += deaths;
            modeStats.TotalAssists += assists;
            if (modeStats.TotalDeaths > 0)
            {
                modeStats.KDA = (float)(modeStats.TotalKills + modeStats.TotalAssists) / modeStats.TotalDeaths;
            }
        }
    }

    private void UpdateRecentHeroes(string playerID, string heroID)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        if (profile != null)
        {
            if (profile.RecentHeroes.Contains(heroID))
            {
                profile.RecentHeroes.Remove(heroID);
            }
            profile.RecentHeroes.Insert(0, heroID);
            if (profile.RecentHeroes.Count > 5)
            {
                profile.RecentHeroes.RemoveAt(5);
            }
        }
    }

    public void AddFavoriteHero(string playerID, string heroID)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        if (profile != null && !profile.FavoriteHeroes.Contains(heroID))
        {
            profile.FavoriteHeroes.Add(heroID);
            if (profile.FavoriteHeroes.Count > 3)
            {
                profile.FavoriteHeroes.RemoveAt(3);
            }
            dataManager.SaveProfileData();
            Debug.Log("添加常用英雄成功");
        }
    }

    public void RemoveFavoriteHero(string playerID, string heroID)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        if (profile != null && profile.FavoriteHeroes.Contains(heroID))
        {
            profile.FavoriteHeroes.Remove(heroID);
            dataManager.SaveProfileData();
            Debug.Log("移除常用英雄成功");
        }
    }

    public void AddOwnedSkin(string playerID, string skinID)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        PlayerCollection collection = GetPlayerCollection(playerID);
        if (profile != null && collection != null && !profile.OwnedSkins.Contains(skinID))
        {
            profile.OwnedSkins.Add(skinID);
            collection.OwnedSkins.Add(skinID);
            UpdateCollectionCompletion(playerID);
            dataManager.SaveProfileData();
            Debug.Log("添加皮肤到收藏成功");
        }
    }

    public void AddOwnedHero(string playerID, string heroID)
    {
        PlayerCollection collection = GetPlayerCollection(playerID);
        if (collection != null && !collection.OwnedHeroes.Contains(heroID))
        {
            collection.OwnedHeroes.Add(heroID);
            UpdateCollectionCompletion(playerID);
            dataManager.SaveProfileData();
            Debug.Log("添加英雄到收藏成功");
        }
    }

    public void AddOwnedItem(string playerID, string itemID)
    {
        PlayerCollection collection = GetPlayerCollection(playerID);
        if (collection != null && !collection.OwnedItems.Contains(itemID))
        {
            collection.OwnedItems.Add(itemID);
            UpdateCollectionCompletion(playerID);
            dataManager.SaveProfileData();
            Debug.Log("添加道具到收藏成功");
        }
    }

    public void AddOwnedTitle(string playerID, string titleID)
    {
        PlayerCollection collection = GetPlayerCollection(playerID);
        if (collection != null && !collection.OwnedTitles.Contains(titleID))
        {
            collection.OwnedTitles.Add(titleID);
            dataManager.SaveProfileData();
            Debug.Log("添加称号到收藏成功");
        }
    }

    public void AddOwnedFrame(string playerID, string frameID)
    {
        PlayerCollection collection = GetPlayerCollection(playerID);
        if (collection != null && !collection.OwnedFrames.Contains(frameID))
        {
            collection.OwnedFrames.Add(frameID);
            dataManager.SaveProfileData();
            Debug.Log("添加头像框到收藏成功");
        }
    }

    public void AddOwnedBackground(string playerID, string backgroundID)
    {
        PlayerCollection collection = GetPlayerCollection(playerID);
        if (collection != null && !collection.OwnedBackgrounds.Contains(backgroundID))
        {
            collection.OwnedBackgrounds.Add(backgroundID);
            dataManager.SaveProfileData();
            Debug.Log("添加背景到收藏成功");
        }
    }

    private void UpdateCollectionCompletion(string playerID)
    {
        PlayerCollection collection = GetPlayerCollection(playerID);
        if (collection != null)
        {
            int total = collection.OwnedHeroes.Count + collection.OwnedSkins.Count + collection.OwnedItems.Count;
            
            collection.CollectionCompletion = total;
            collection.LastUpdateTime = DateTime.Now;
        }
    }

    public PlayerProfile GetPlayerProfile(string playerID)
    {
        if (profileData.PlayerProfiles.ContainsKey(playerID))
        {
            return profileData.PlayerProfiles[playerID];
        }
        return null;
    }

    public PlayerStats GetPlayerStats(string playerID)
    {
        if (profileData.PlayerStats.ContainsKey(playerID))
        {
            return profileData.PlayerStats[playerID];
        }
        return null;
    }

    public PlayerCollection GetPlayerCollection(string playerID)
    {
        if (profileData.PlayerCollections.ContainsKey(playerID))
        {
            return profileData.PlayerCollections[playerID];
        }
        return null;
    }

    public HeroStats GetHeroStats(string playerID, string heroID)
    {
        PlayerStats stats = GetPlayerStats(playerID);
        if (stats != null && stats.HeroStats.ContainsKey(heroID))
        {
            return stats.HeroStats[heroID];
        }
        return null;
    }

    public ModeStats GetModeStats(string playerID, string modeID)
    {
        PlayerStats stats = GetPlayerStats(playerID);
        if (stats != null && stats.ModeStats.ContainsKey(modeID))
        {
            return stats.ModeStats[modeID];
        }
        return null;
    }

    public List<string> GetRecentHeroes(string playerID)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        if (profile != null)
        {
            return profile.RecentHeroes;
        }
        return new List<string>();
    }

    public List<string> GetFavoriteHeroes(string playerID)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        if (profile != null)
        {
            return profile.FavoriteHeroes;
        }
        return new List<string>();
    }

    public List<string> GetOwnedSkins(string playerID)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        if (profile != null)
        {
            return profile.OwnedSkins;
        }
        return new List<string>();
    }

    public List<string> GetOwnedHeroes(string playerID)
    {
        PlayerCollection collection = GetPlayerCollection(playerID);
        if (collection != null)
        {
            return collection.OwnedHeroes;
        }
        return new List<string>();
    }

    public void UpdateLastLoginTime(string playerID)
    {
        PlayerProfile profile = GetPlayerProfile(playerID);
        if (profile != null)
        {
            profile.LastLoginTime = DateTime.Now;
            dataManager.SaveProfileData();
        }
    }

    public void CleanupInactiveProfiles(int daysInactive = 90)
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-daysInactive);
        List<string> inactivePlayers = new List<string>();
        foreach (KeyValuePair<string, PlayerProfile> pair in profileData.PlayerProfiles)
        {
            if (pair.Value.LastLoginTime < cutoffDate)
            {
                inactivePlayers.Add(pair.Key);
            }
        }
        
        foreach (string playerID in inactivePlayers)
        {
            profileData.PlayerProfiles.Remove(playerID);
            profileData.PlayerStats.Remove(playerID);
            profileData.PlayerCollections.Remove(playerID);
        }
        
        if (inactivePlayers.Count > 0)
        {
            dataManager.SaveProfileData();
            Debug.Log("清理 inactive profiles 成功: " + inactivePlayers.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveProfileData();
    }

    public void LoadData()
    {
        dataManager.LoadProfileData();
    }

    public List<ProfileEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
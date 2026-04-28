[System.Serializable]
public class PlayerProfile
{
    public string playerID;
    public string playerName;
    public int level;
    public int experience;
    public string avatar;
    public string background;
    public string title;
    public string signature;
    
    public int totalGames;
    public int totalWins;
    public int totalLosses;
    public float winRate;
    public int totalKills;
    public int totalDeaths;
    public int totalAssists;
    public float kda;
    
    public List<string> ownedHeroes;
    public List<string> ownedSkins;
    public List<string> achievements;
    public List<string> titles;
    
    public int gold;
    public int gems;
    
    public PlayerProfile(string id, string name)
    {
        playerID = id;
        playerName = name;
        level = 1;
        experience = 0;
        avatar = "default_avatar";
        background = "default_background";
        title = "初出茅庐";
        signature = "这个人很懒，什么都没写";
        
        totalGames = 0;
        totalWins = 0;
        totalLosses = 0;
        winRate = 0;
        totalKills = 0;
        totalDeaths = 0;
        totalAssists = 0;
        kda = 0;
        
        ownedHeroes = new System.Collections.Generic.List<string>();
        ownedSkins = new System.Collections.Generic.List<string>();
        achievements = new System.Collections.Generic.List<string>();
        titles = new System.Collections.Generic.List<string>();
        
        gold = 10000;
        gems = 1000;
    }
    
    public void UpdateStats(int kills, int deaths, int assists, bool won)
    {
        totalGames++;
        totalKills += kills;
        totalDeaths += deaths;
        totalAssists += assists;
        
        if (won)
        {
            totalWins++;
        }
        else
        {
            totalLosses++;
        }
        
        // 计算胜率
        winRate = (float)totalWins / totalGames * 100;
        
        // 计算KDA
        if (totalDeaths > 0)
        {
            kda = (float)(totalKills + totalAssists) / totalDeaths;
        }
        else
        {
            kda = totalKills + totalAssists;
        }
    }
    
    public void AddExperience(int exp)
    {
        experience += exp;
        CheckLevelUp();
    }
    
    private void CheckLevelUp()
    {
        int requiredExp = level * 1000;
        while (experience >= requiredExp)
        {
            level++;
            experience -= requiredExp;
            requiredExp = level * 1000;
        }
    }
    
    public void AddHero(string heroID)
    {
        if (!ownedHeroes.Contains(heroID))
        {
            ownedHeroes.Add(heroID);
        }
    }
    
    public void AddSkin(string skinID)
    {
        if (!ownedSkins.Contains(skinID))
        {
            ownedSkins.Add(skinID);
        }
    }
    
    public void AddAchievement(string achievementID)
    {
        if (!achievements.Contains(achievementID))
        {
            achievements.Add(achievementID);
        }
    }
    
    public void AddTitle(string titleID)
    {
        if (!titles.Contains(titleID))
        {
            titles.Add(titleID);
        }
    }
    
    public void SetAvatar(string avatarID)
    {
        avatar = avatarID;
    }
    
    public void SetBackground(string backgroundID)
    {
        background = backgroundID;
    }
    
    public void SetTitle(string titleID)
    {
        title = titleID;
    }
    
    public void SetSignature(string sig)
    {
        signature = sig;
    }
}
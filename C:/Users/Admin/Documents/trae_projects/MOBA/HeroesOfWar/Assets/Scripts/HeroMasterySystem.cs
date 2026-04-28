[System.Serializable]
public class HeroMastery
{
    public string heroID;
    public string heroName;
    public int masteryLevel;
    public int masteryPoints;
    public string masteryTitle;
    public bool isUnlocked;
    
    public HeroMastery(string id, string name)
    {
        heroID = id;
        heroName = name;
        masteryLevel = 0;
        masteryPoints = 0;
        masteryTitle = "见习";
        isUnlocked = false;
    }
    
    public void AddMasteryPoints(int points)
    {
        masteryPoints += points;
        UpdateMasteryLevel();
    }
    
    private void UpdateMasteryLevel()
    {
        if (masteryPoints >= 1000)
        {
            masteryLevel = 5;
            masteryTitle = "宗师";
        }
        else if (masteryPoints >= 800)
        {
            masteryLevel = 4;
            masteryTitle = "专家";
        }
        else if (masteryPoints >= 600)
        {
            masteryLevel = 3;
            masteryTitle = "精英";
        }
        else if (masteryPoints >= 300)
        {
            masteryLevel = 2;
            masteryTitle = "熟练";
        }
        else if (masteryPoints >= 100)
        {
            masteryLevel = 1;
            masteryTitle = "资深";
        }
        else
        {
            masteryLevel = 0;
            masteryTitle = "见习";
        }
    }
    
    public void Unlock()
    {
        isUnlocked = true;
    }
}

[System.Serializable]
public class HeroMasteryManagerData
{
    public List<HeroMastery> heroMasteries;
    
    public HeroMasteryManagerData()
    {
        heroMasteries = new List<HeroMastery>();
    }
    
    public void AddHeroMastery(HeroMastery mastery)
    {
        heroMasteries.Add(mastery);
    }
    
    public HeroMastery GetHeroMastery(string heroID)
    {
        return heroMasteries.Find(m => m.heroID == heroID);
    }
    
    public List<HeroMastery> GetUnlockedHeroes()
    {
        return heroMasteries.FindAll(m => m.isUnlocked);
    }
    
    public List<HeroMastery> GetHeroesByMasteryLevel(int level)
    {
        return heroMasteries.FindAll(m => m.masteryLevel == level && m.isUnlocked);
    }
}
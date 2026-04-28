[System.Serializable]
public class EquipmentBuild
{
    public string buildID;
    public string buildName;
    public string heroID;
    public string description;
    public List<string> itemIDs;
    public int popularity;
    public float winRate;
    public string buildType;
    
    public EquipmentBuild(string id, string name, string hero, string type)
    {
        buildID = id;
        buildName = name;
        heroID = hero;
        description = "";
        itemIDs = new List<string>();
        popularity = 0;
        winRate = 0;
        buildType = type;
    }
}

[System.Serializable]
public class GameContext
{
    public string gameMode;
    public string mapType;
    public List<string> allyHeroIDs;
    public List<string> enemyHeroIDs;
    public int gameTime;
    public string currentPhase;
    public int playerGold;
    public bool isWinning;
    
    public GameContext()
    {
        allyHeroIDs = new List<string>();
        enemyHeroIDs = new List<string>();
        gameTime = 0;
        currentPhase = "early";
        playerGold = 0;
        isWinning = true;
    }
}

[System.Serializable]
public class ItemRecommendation
{
    public string itemID;
    public string itemName;
    public string reason;
    public int priority;
    public float score;
    public string category;
    
    public ItemRecommendation(string id, string name, string reasonText, int prio)
    {
        itemID = id;
        itemName = name;
        reason = reasonText;
        priority = prio;
        score = 0;
        category = "";
    }
}

[System.Serializable]
public class HeroRecommendation
{
    public string heroID;
    public string heroName;
    public string role;
    public List<string> counterHeroes;
    public List<string> counteredByHeroes;
    public float effectivenessScore;
    
    public HeroRecommendation(string id, string name, string roleType)
    {
        heroID = id;
        heroName = name;
        role = roleType;
        counterHeroes = new List<string>();
        counteredByHeroes = new List<string>();
        effectivenessScore = 0;
    }
}
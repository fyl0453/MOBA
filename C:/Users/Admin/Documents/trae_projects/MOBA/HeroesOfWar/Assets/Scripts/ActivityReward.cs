[System.Serializable]
public class ActivityReward
{
    public int gold;
    public int experience;
    public string itemID;
    public int itemCount;
    public string skinID;
    public string heroID;
    public int gems;
    
    public ActivityReward(int goldAmount = 0, int expAmount = 0, string item = null, int count = 0, string skin = null, string hero = null, int gemAmount = 0)
    {
        gold = goldAmount;
        experience = expAmount;
        itemID = item;
        itemCount = count;
        skinID = skin;
        heroID = hero;
        gems = gemAmount;
    }
}
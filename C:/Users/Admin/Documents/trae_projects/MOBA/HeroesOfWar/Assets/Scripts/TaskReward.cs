[System.Serializable]
public class TaskReward
{
    public int gold;
    public int experience;
    public string itemID;
    public int itemCount;
    public string skinID;
    
    public TaskReward(int goldAmount, int expAmount, string item = null, int count = 0, string skin = null)
    {
        gold = goldAmount;
        experience = expAmount;
        itemID = item;
        itemCount = count;
        skinID = skin;
    }
}
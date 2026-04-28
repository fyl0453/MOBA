[System.Serializable]
public class Friend
{
    public string playerName;
    public string status;
    public int level;
    
    public Friend(string name, string status, int lvl)
    {
        playerName = name;
        this.status = status;
        level = lvl;
    }
}
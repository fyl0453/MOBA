[System.Serializable]
public class FriendRequest
{
    public string playerName;
    public string status;
    public int level;
    
    public FriendRequest(string name, string status, int lvl)
    {
        playerName = name;
        this.status = status;
        level = lvl;
    }
}
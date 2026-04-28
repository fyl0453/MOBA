[System.Serializable]
public class Player
{
    public string playerName;
    public int rankPoints;
    public int level;
    
    public Player(string name, int points, int lvl)
    {
        playerName = name;
        rankPoints = points;
        level = lvl;
    }
}
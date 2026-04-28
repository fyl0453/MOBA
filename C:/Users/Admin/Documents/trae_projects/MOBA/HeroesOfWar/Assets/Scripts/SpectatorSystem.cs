using UnityEngine;
using System.Collections.Generic;

public class SpectatorSystem : MonoBehaviour
{
    public static SpectatorSystem Instance { get; private set; }
    
    public List<GameMatch> ongoingMatches = new List<GameMatch>();
    private bool isSpectating = false;
    private GameMatch currentMatch;
    private int currentSpectatorID;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddOngoingMatch(GameMatch match)
    {
        if (!ongoingMatches.Contains(match))
        {
            ongoingMatches.Add(match);
        }
    }
    
    public void RemoveOngoingMatch(GameMatch match)
    {
        ongoingMatches.Remove(match);
    }
    
    public List<GameMatch> GetOngoingMatches()
    {
        return ongoingMatches;
    }
    
    public bool StartSpectating(int matchID)
    {
        GameMatch match = ongoingMatches.Find(m => m.matchID == matchID);
        if (match != null)
        {
            currentMatch = match;
            currentSpectatorID = Random.Range(1000, 9999);
            isSpectating = true;
            Debug.Log($"开始观战比赛: {match.matchID}");
            return true;
        }
        return false;
    }
    
    public void StopSpectating()
    {
        isSpectating = false;
        currentMatch = null;
        Debug.Log("停止观战");
    }
    
    public bool IsSpectating()
    {
        return isSpectating;
    }
    
    public GameMatch GetCurrentMatch()
    {
        return currentMatch;
    }
    
    public int GetSpectatorID()
    {
        return currentSpectatorID;
    }
}

[System.Serializable]
public class GameMatch
{
    public int matchID;
    public string matchName;
    public List<Player> blueTeam;
    public List<Player> redTeam;
    public float matchTime;
    public MatchStatus status;
    
    public GameMatch(int id, string name)
    {
        matchID = id;
        matchName = name;
        blueTeam = new List<Player>();
        redTeam = new List<Player>();
        matchTime = 0f;
        status = MatchStatus.Waiting;
    }
}

[System.Serializable]
public class Player
{
    public string playerID;
    public string playerName;
    public string heroID;
    public int level;
    public int kills;
    public int deaths;
    public int assists;
    
    public Player(string id, string name, string hero)
    {
        playerID = id;
        playerName = name;
        heroID = hero;
        level = 1;
        kills = 0;
        deaths = 0;
        assists = 0;
    }
}

public enum MatchStatus
{
    Waiting,
    InProgress,
    Ended
}

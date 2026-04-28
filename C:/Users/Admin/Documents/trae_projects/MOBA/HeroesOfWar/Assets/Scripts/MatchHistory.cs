using System;
using System.Collections.Generic;

[Serializable]
public class MatchHistory
{
    public string matchId;
    public DateTime matchDate;
    public bool isVictory;
    public string gameMode;
    public int duration;
    
    public string playerName;
    public string heroName;
    public int kills;
    public int deaths;
    public int assists;
    public int goldEarned;
    public int damageDealt;
    public int damageTaken;
    
    public List<PlayerMatchStats> playerStats = new List<PlayerMatchStats>();
    
    public MatchHistory(string id, bool victory, string mode, int durationMinutes)
    {
        matchId = id;
        matchDate = DateTime.Now;
        isVictory = victory;
        gameMode = mode;
        duration = durationMinutes;
    }
    
    public void SetPlayerStats(string name, string hero, int kill, int death, int assist, int gold, int damageD, int damageT)
    {
        playerName = name;
        heroName = hero;
        kills = kill;
        deaths = death;
        assists = assist;
        goldEarned = gold;
        damageDealt = damageD;
        damageTaken = damageT;
    }
    
    public float GetKDA()
    {
        if (deaths == 0) return (kills + assists) * 1.0f;
        return (kills + assists) * 1.0f / deaths;
    }
    
    public string GetMatchDuration()
    {
        int minutes = duration / 60;
        int seconds = duration % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }
}

[Serializable]
public class PlayerMatchStats
{
    public string playerName;
    public string heroName;
    public int kills;
    public int deaths;
    public int assists;
    public int teamId;
    
    public PlayerMatchStats(string name, string hero, int kill, int death, int assist, int team)
    {
        playerName = name;
        heroName = hero;
        kills = kill;
        deaths = death;
        assists = assist;
        teamId = team;
    }
}
[System.Serializable]
public class SocialInteractionDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<CoupleRelationship> coupleRelationships;
    public List<FriendInteraction> friendInteractions;
    public List<MiniGame> miniGames;
    public List<SocialEvent> socialEvents;
    public List<SocialAchievement> socialAchievements;
    
    public SocialInteractionDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        coupleRelationships = new List<CoupleRelationship>();
        friendInteractions = new List<FriendInteraction>();
        miniGames = new List<MiniGame>();
        socialEvents = new List<SocialEvent>();
        socialAchievements = new List<SocialAchievement>();
    }
    
    public void AddCoupleRelationship(CoupleRelationship relationship)
    {
        coupleRelationships.Add(relationship);
    }
    
    public void AddFriendInteraction(FriendInteraction interaction)
    {
        friendInteractions.Add(interaction);
    }
    
    public void AddMiniGame(MiniGame game)
    {
        miniGames.Add(game);
    }
    
    public void AddSocialEvent(SocialEvent ev)
    {
        socialEvents.Add(ev);
    }
    
    public void AddSocialAchievement(SocialAchievement achievement)
    {
        socialAchievements.Add(achievement);
    }
    
    public CoupleRelationship GetCoupleRelationship(string relationshipID)
    {
        return coupleRelationships.Find(r => r.relationshipID == relationshipID);
    }
    
    public FriendInteraction GetFriendInteraction(string interactionID)
    {
        return friendInteractions.Find(i => i.interactionID == interactionID);
    }
    
    public MiniGame GetMiniGame(string gameID)
    {
        return miniGames.Find(g => g.gameID == gameID);
    }
    
    public SocialEvent GetSocialEvent(string eventID)
    {
        return socialEvents.Find(e => e.eventID == eventID);
    }
    
    public SocialAchievement GetSocialAchievement(string achievementID)
    {
        return socialAchievements.Find(a => a.achievementID == achievementID);
    }
    
    public List<CoupleRelationship> GetCoupleRelationshipsByUser(string userID)
    {
        return coupleRelationships.FindAll(r => r.user1ID == userID || r.user2ID == userID);
    }
    
    public List<FriendInteraction> GetFriendInteractionsByUser(string userID)
    {
        return friendInteractions.FindAll(i => i.user1ID == userID || i.user2ID == userID);
    }
    
    public List<MiniGame> GetAllMiniGames()
    {
        return miniGames;
    }
    
    public List<SocialEvent> GetSocialEventsByUser(string userID)
    {
        return socialEvents.FindAll(e => e.participants.Contains(userID));
    }
}

[System.Serializable]
public class CoupleRelationship
{
    public string relationshipID;
    public string user1ID;
    public string user2ID;
    public string status;
    public int intimacyLevel;
    public int intimacyPoints;
    public string startDate;
    public string endDate;
    public List<CoupleActivity> activities;
    public List<CoupleReward> rewards;
    
    public CoupleRelationship(string id, string user1ID, string user2ID)
    {
        relationshipID = id;
        this.user1ID = user1ID;
        this.user2ID = user2ID;
        status = "Active";
        intimacyLevel = 1;
        intimacyPoints = 0;
        startDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        endDate = "";
        activities = new List<CoupleActivity>();
        rewards = new List<CoupleReward>();
    }
    
    public void AddActivity(CoupleActivity activity)
    {
        activities.Add(activity);
    }
    
    public void AddReward(CoupleReward reward)
    {
        rewards.Add(reward);
    }
    
    public void AddIntimacyPoints(int points)
    {
        intimacyPoints += points;
        CheckIntimacyLevel();
    }
    
    private void CheckIntimacyLevel()
    {
        int[] levelThresholds = { 0, 100, 300, 600, 1000, 1500, 2100, 2800, 3600, 4500 };
        for (int i = levelThresholds.Length - 1; i >= 0; i--)
        {
            if (intimacyPoints >= levelThresholds[i])
            {
                intimacyLevel = i + 1;
                break;
            }
        }
    }
    
    public void EndRelationship()
    {
        status = "Ended";
        endDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CoupleActivity
{
    public string activityID;
    public string activityType;
    public string activityName;
    public string description;
    public int intimacyPoints;
    public string date;
    public bool isCompleted;
    
    public CoupleActivity(string id, string type, string name, string desc, int points)
    {
        activityID = id;
        activityType = type;
        activityName = name;
        description = desc;
        intimacyPoints = points;
        date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        isCompleted = false;
    }
    
    public void Complete()
    {
        isCompleted = true;
    }
}

[System.Serializable]
public class CoupleReward
{
    public string rewardID;
    public string rewardName;
    public string rewardType;
    public string description;
    public int requiredLevel;
    public bool isClaimed;
    public string claimDate;
    
    public CoupleReward(string id, string name, string type, string desc, int level)
    {
        rewardID = id;
        rewardName = name;
        rewardType = type;
        description = desc;
        requiredLevel = level;
        isClaimed = false;
        claimDate = "";
    }
    
    public void Claim()
    {
        isClaimed = true;
        claimDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class FriendInteraction
{
    public string interactionID;
    public string user1ID;
    public string user2ID;
    public string interactionType;
    public string interactionContent;
    public string date;
    public bool isRead;
    
    public FriendInteraction(string id, string user1ID, string user2ID, string type, string content)
    {
        interactionID = id;
        this.user1ID = user1ID;
        this.user2ID = user2ID;
        interactionType = type;
        interactionContent = content;
        date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        isRead = false;
    }
    
    public void MarkAsRead()
    {
        isRead = true;
    }
}

[System.Serializable]
public class MiniGame
{
    public string gameID;
    public string gameName;
    public string gameDescription;
    public string gameType;
    public int minPlayers;
    public int maxPlayers;
    public int duration;
    public bool isEnabled;
    public List<GameRule> rules;
    public List<GameReward> rewards;
    
    public MiniGame(string id, string name, string desc, string type, int minPlayers, int maxPlayers, int duration)
    {
        gameID = id;
        gameName = name;
        gameDescription = desc;
        gameType = type;
        this.minPlayers = minPlayers;
        this.maxPlayers = maxPlayers;
        this.duration = duration;
        isEnabled = true;
        rules = new List<GameRule>();
        rewards = new List<GameReward>();
    }
    
    public void AddRule(GameRule rule)
    {
        rules.Add(rule);
    }
    
    public void AddReward(GameReward reward)
    {
        rewards.Add(reward);
    }
}

[System.Serializable]
public class GameRule
{
    public string ruleID;
    public string ruleName;
    public string ruleDescription;
    public bool isEnabled;
    
    public GameRule(string id, string name, string desc)
    {
        ruleID = id;
        ruleName = name;
        ruleDescription = desc;
        isEnabled = true;
    }
}

[System.Serializable]
public class GameReward
{
    public string rewardID;
    public string rewardName;
    public string rewardType;
    public string description;
    public int requiredScore;
    
    public GameReward(string id, string name, string type, string desc, int score)
    {
        rewardID = id;
        rewardName = name;
        rewardType = type;
        description = desc;
        requiredScore = score;
    }
}

[System.Serializable]
public class SocialEvent
{
    public string eventID;
    public string eventName;
    public string eventDescription;
    public string eventType;
    public string startTime;
    public string endTime;
    public List<string> participants;
    public List<EventActivity> activities;
    
    public SocialEvent(string id, string name, string desc, string type, string startTime, string endTime)
    {
        eventID = id;
        eventName = name;
        eventDescription = desc;
        eventType = type;
        this.startTime = startTime;
        this.endTime = endTime;
        participants = new List<string>();
        activities = new List<EventActivity>();
    }
    
    public void AddParticipant(string userID)
    {
        if (!participants.Contains(userID))
        {
            participants.Add(userID);
        }
    }
    
    public void RemoveParticipant(string userID)
    {
        participants.Remove(userID);
    }
    
    public void AddActivity(EventActivity activity)
    {
        activities.Add(activity);
    }
}

[System.Serializable]
public class EventActivity
{
    public string activityID;
    public string activityName;
    public string activityDescription;
    public string activityType;
    public string startTime;
    public string endTime;
    public bool isCompleted;
    
    public EventActivity(string id, string name, string desc, string type, string startTime, string endTime)
    {
        activityID = id;
        activityName = name;
        activityDescription = desc;
        activityType = type;
        this.startTime = startTime;
        this.endTime = endTime;
        isCompleted = false;
    }
    
    public void Complete()
    {
        isCompleted = true;
    }
}

[System.Serializable]
public class SocialAchievement
{
    public string achievementID;
    public string achievementName;
    public string achievementDescription;
    public string achievementType;
    public int requiredValue;
    public string reward;
    
    public SocialAchievement(string id, string name, string desc, string type, int value, string reward)
    {
        achievementID = id;
        achievementName = name;
        achievementDescription = desc;
        achievementType = type;
        requiredValue = value;
        this.reward = reward;
    }
}

[System.Serializable]
public class SocialInteractionDetailedManagerData
{
    public SocialInteractionDetailed system;
    
    public SocialInteractionDetailedManagerData()
    {
        system = new SocialInteractionDetailed("social_interaction_detailed", "社交互动详细系统", "管理社交互动的详细功能，包括情侣关系和好友互动");
    }
}
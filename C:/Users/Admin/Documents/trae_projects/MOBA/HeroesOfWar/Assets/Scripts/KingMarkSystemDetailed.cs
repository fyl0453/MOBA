[System.Serializable]
public class KingMarkSystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<KingMark> kingMarks;
    public List<SeasonMark> seasonMarks;
    public List<PlayerKingMark> playerKingMarks;
    public List<KingMarkEvent> kingMarkEvents;

    public KingMarkSystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        kingMarks = new List<KingMark>();
        seasonMarks = new List<SeasonMark>();
        playerKingMarks = new List<PlayerKingMark>();
        kingMarkEvents = new List<KingMarkEvent>();
    }

    public void AddKingMark(KingMark kingMark)
    {
        kingMarks.Add(kingMark);
    }

    public void AddSeasonMark(SeasonMark seasonMark)
    {
        seasonMarks.Add(seasonMark);
    }

    public void AddPlayerKingMark(PlayerKingMark playerKingMark)
    {
        playerKingMarks.Add(playerKingMark);
    }

    public void AddKingMarkEvent(KingMarkEvent kingMarkEvent)
    {
        kingMarkEvents.Add(kingMarkEvent);
    }

    public KingMark GetKingMark(string markID)
    {
        return kingMarks.Find(km => km.markID == markID);
    }

    public SeasonMark GetSeasonMark(string seasonID)
    {
        return seasonMarks.Find(sm => sm.seasonID == seasonID);
    }

    public PlayerKingMark GetPlayerKingMark(string userID, string markID)
    {
        return playerKingMarks.Find(pkm => pkm.userID == userID && pkm.markID == markID);
    }

    public KingMarkEvent GetKingMarkEvent(string eventID)
    {
        return kingMarkEvents.Find(kme => kme.eventID == eventID);
    }

    public List<KingMark> GetKingMarksByRarity(string rarity)
    {
        return kingMarks.FindAll(km => km.rarity == rarity);
    }

    public List<SeasonMark> GetSeasonMarksByStatus(string status)
    {
        return seasonMarks.FindAll(sm => sm.status == status);
    }

    public List<PlayerKingMark> GetPlayerKingMarksByUser(string userID)
    {
        return playerKingMarks.FindAll(pkm => pkm.userID == userID);
    }

    public List<KingMarkEvent> GetKingMarkEventsByUser(string userID)
    {
        return kingMarkEvents.FindAll(kme => kme.userID == userID);
    }
}

[System.Serializable]
public class KingMark
{
    public string markID;
    public string markName;
    public string markDescription;
    public string rarity;
    public string obtainCondition;
    public string icon;
    public string effect;
    public bool isLimited;
    public bool isEnabled;

    public KingMark(string id, string markName, string markDescription, string rarity, string obtainCondition, string icon, string effect, bool isLimited)
    {
        markID = id;
        this.markName = markName;
        this.markDescription = markDescription;
        this.rarity = rarity;
        this.obtainCondition = obtainCondition;
        this.icon = icon;
        this.effect = effect;
        this.isLimited = isLimited;
        isEnabled = true;
    }
}

[System.Serializable]
public class SeasonMark
{
    public string seasonID;
    public string seasonName;
    public string startTime;
    public string endTime;
    public string status;
    public int maxRank;
    public bool isEnabled;

    public SeasonMark(string id, string seasonName, string startTime, string endTime, int maxRank)
    {
        seasonID = id;
        this.seasonName = seasonName;
        this.startTime = startTime;
        this.endTime = endTime;
        status = "active";
        this.maxRank = maxRank;
        isEnabled = true;
    }

    public void Start()
    {
        status = "active";
    }

    public void End()
    {
        status = "ended";
    }
}

[System.Serializable]
public class PlayerKingMark
{
    public string playerMarkID;
    public string userID;
    public string userName;
    public string markID;
    public string markName;
    public string seasonID;
    public bool isEquipped;
    public string obtainTime;

    public PlayerKingMark(string id, string userID, string userName, string markID, string markName, string seasonID)
    {
        playerMarkID = id;
        this.userID = userID;
        this.userName = userName;
        this.markID = markID;
        this.markName = markName;
        this.seasonID = seasonID;
        isEquipped = false;
        obtainTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void Equip()
    {
        isEquipped = true;
    }

    public void Unequip()
    {
        isEquipped = false;
    }
}

[System.Serializable]
public class KingMarkEvent
{
    public string eventID;
    public string eventType;
    public string userID;
    public string markID;
    public string description;
    public string timestamp;
    public string status;

    public KingMarkEvent(string id, string eventType, string userID, string markID, string description)
    {
        eventID = id;
        this.eventType = eventType;
        this.userID = userID;
        this.markID = markID;
        this.description = description;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        status = "active";
    }

    public void MarkAsCompleted()
    {
        status = "completed";
    }
}

[System.Serializable]
public class KingMarkSystemDetailedManagerData
{
    public KingMarkSystemDetailed system;

    public KingMarkSystemDetailedManagerData()
    {
        system = new KingMarkSystemDetailed("king_mark_system_detailed", "王者印记系统详细", "管理王者印记的详细功能，包括赛季印记获取和展示");
    }
}
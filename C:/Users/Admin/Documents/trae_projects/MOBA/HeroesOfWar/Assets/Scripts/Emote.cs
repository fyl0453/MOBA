[System.Serializable]
public class Emote
{
    public string emoteID;
    public string emoteName;
    public string description;
    public string icon;
    public string animation;
    public float duration;
    public int cost;
    public bool isOwned;
    public string category;
    
    public Emote(string id, string name, string desc, string anim, float dur, int price, string cat)
    {
        emoteID = id;
        emoteName = name;
        description = desc;
        icon = "";
        animation = anim;
        duration = dur;
        cost = price;
        isOwned = false;
        category = cat;
    }
}

[System.Serializable]
public class EmoteSlot
{
    public int slotIndex;
    public string equippedEmoteID;
    
    public EmoteSlot(int index)
    {
        slotIndex = index;
        equippedEmoteID = "";
    }
}

[System.Serializable]
public class QuickEmote
{
    public string quickEmoteID;
    public string emoteName;
    public string icon;
    public int slotPosition;
    
    public QuickEmote(string id, string name, int slot)
    {
        quickEmoteID = id;
        emoteName = name;
        icon = "";
        slotPosition = slot;
    }
}

[System.Serializable]
public class PlayerEmote
{
    public string playerID;
    public string emoteID;
    public Vector3 position;
    public float timestamp;
    public bool isQuickEmote;
    
    public PlayerEmote(string pid, string eid, Vector3 pos, bool quick)
    {
        playerID = pid;
        emoteID = eid;
        position = pos;
        timestamp = Time.time;
        isQuickEmote = quick;
    }
}
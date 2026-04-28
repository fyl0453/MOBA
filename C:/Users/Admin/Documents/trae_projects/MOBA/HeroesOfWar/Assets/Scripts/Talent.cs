[System.Serializable]
public class Talent
{
    public string talentID;
    public string talentName;
    public string description;
    public int tier;
    public string icon;
    public List<TalentEffect> effects;
    
    public Talent(string id, string name, string desc, int tierLevel)
    {
        talentID = id;
        talentName = name;
        description = desc;
        tier = tierLevel;
        icon = "";
        effects = new List<TalentEffect>();
    }
}

[System.Serializable]
public class TalentEffect
{
    public string effectType;
    public float value;
    public string target;
    
    public TalentEffect(string type, float val, string tgt)
    {
        effectType = type;
        value = val;
        target = tgt;
    }
}

[System.Serializable]
public class TalentPage
{
    public string pageID;
    public string pageName;
    public List<TalentSlot> slots;
    public string recommendedHeroID;
    public bool isDefault;
    
    public TalentPage(string id, string name)
    {
        pageID = id;
        pageName = name;
        slots = new List<TalentSlot>();
        recommendedHeroID = "";
        isDefault = false;
        
        InitializeSlots();
    }
    
    private void InitializeSlots()
    {
        slots.Add(new TalentSlot(1, 1));
        slots.Add(new TalentSlot(2, 1));
        slots.Add(new TalentSlot(3, 1));
        slots.Add(new TalentSlot(4, 2));
        slots.Add(new TalentSlot(5, 2));
        slots.Add(new TalentSlot(6, 2));
        slots.Add(new TalentSlot(7, 3));
        slots.Add(new TalentSlot(8, 3));
        slots.Add(new TalentSlot(9, 3));
    }
}

[System.Serializable]
public class TalentSlot
{
    public int slotIndex;
    public int tierLevel;
    public string equippedTalentID;
    
    public TalentSlot(int index, int tier)
    {
        slotIndex = index;
        tierLevel = tier;
        equippedTalentID = "";
    }
}
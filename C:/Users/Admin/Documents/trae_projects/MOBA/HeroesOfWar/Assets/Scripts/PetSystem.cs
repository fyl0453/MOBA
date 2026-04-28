[System.Serializable]
public class Pet
{
    public string petID;
    public string petName;
    public string petType;
    public int level;
    public int exp;
    public int maxExp;
    public string rarity;
    public string quality;
    public List<PetAttribute> attributes;
    public List<PetSkill> skills;
    public string description;
    public string modelID;
    public bool isActive;
    
    public Pet(string id, string name, string type, string r = "Common", string q = "Normal")
    {
        petID = id;
        petName = name;
        petType = type;
        level = 1;
        exp = 0;
        maxExp = 100;
        rarity = r;
        quality = q;
        attributes = new List<PetAttribute>();
        skills = new List<PetSkill>();
        description = "一只可爱的宠物";
        modelID = "default_pet";
        isActive = false;
    }
    
    public void AddExp(int amount)
    {
        exp += amount;
        while (exp >= maxExp)
        {
            exp -= maxExp;
            LevelUp();
        }
    }
    
    private void LevelUp()
    {
        level++;
        maxExp = (int)(maxExp * 1.5f);
        foreach (PetAttribute attr in attributes)
        {
            attr.value += attr.growth;
        }
    }
    
    public void AddAttribute(string name, float value, float growth)
    {
        PetAttribute attr = new PetAttribute(name, value, growth);
        attributes.Add(attr);
    }
    
    public void AddSkill(string skillID, string skillName, string skillDescription, float cooldown)
    {
        PetSkill skill = new PetSkill(skillID, skillName, skillDescription, cooldown);
        skills.Add(skill);
    }
    
    public void Activate()
    {
        isActive = true;
    }
    
    public void Deactivate()
    {
        isActive = false;
    }
}

[System.Serializable]
public class PetAttribute
{
    public string name;
    public float value;
    public float growth;
    
    public PetAttribute(string n, float v, float g)
    {
        name = n;
        value = v;
        growth = g;
    }
}

[System.Serializable]
public class PetSkill
{
    public string skillID;
    public string skillName;
    public string skillDescription;
    public float cooldown;
    public float currentCooldown;
    public bool isReady;
    
    public PetSkill(string id, string name, string desc, float cd)
    {
        skillID = id;
        skillName = name;
        skillDescription = desc;
        cooldown = cd;
        currentCooldown = 0;
        isReady = true;
    }
    
    public void UseSkill()
    {
        if (isReady)
        {
            isReady = false;
            currentCooldown = cooldown;
        }
    }
    
    public void UpdateCooldown(float deltaTime)
    {
        if (!isReady)
        {
            currentCooldown -= deltaTime;
            if (currentCooldown <= 0)
            {
                currentCooldown = 0;
                isReady = true;
            }
        }
    }
}

[System.Serializable]
public class PetInventory
{
    public string playerID;
    public List<Pet> pets;
    public int maxPets;
    public string activePetID;
    
    public PetInventory(string id, int max = 10)
    {
        playerID = id;
        pets = new List<Pet>();
        maxPets = max;
        activePetID = "";
    }
    
    public void AddPet(Pet pet)
    {
        if (pets.Count < maxPets)
        {
            pets.Add(pet);
        }
    }
    
    public void RemovePet(string petID)
    {
        pets.RemoveAll(p => p.petID == petID);
        if (activePetID == petID)
        {
            activePetID = "";
        }
    }
    
    public void SetActivePet(string petID)
    {
        Pet pet = pets.Find(p => p.petID == petID);
        if (pet != null)
        {
            foreach (Pet p in pets)
            {
                p.Deactivate();
            }
            pet.Activate();
            activePetID = petID;
        }
    }
    
    public Pet GetActivePet()
    {
        return pets.Find(p => p.petID == activePetID);
    }
    
    public Pet GetPetByID(string petID)
    {
        return pets.Find(p => p.petID == petID);
    }
    
    public List<Pet> GetPetsByType(string type)
    {
        return pets.FindAll(p => p.petType == type);
    }
}
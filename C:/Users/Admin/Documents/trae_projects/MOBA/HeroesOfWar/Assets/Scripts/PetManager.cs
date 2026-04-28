using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PetManager : MonoBehaviour
{
    public static PetManager Instance { get; private set; }
    
    public PetInventory petInventory;
    
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
    
    private void Start()
    {
        LoadPetData();
        
        if (petInventory == null)
        {
            petInventory = new PetInventory(ProfileManager.Instance.currentProfile.playerID);
            AddDefaultPets();
        }
    }
    
    private void AddDefaultPets()
    {
        Pet dog = new Pet("pet_dog", "小狗", "Animal", "Common");
        dog.AddAttribute("攻击", 10, 2);
        dog.AddAttribute("防御", 5, 1);
        dog.AddSkill("skill_bark", " bark", "发出 bark 声，吓退敌人", 10);
        petInventory.AddPet(dog);
        
        Pet cat = new Pet("pet_cat", "小猫", "Animal", "Common");
        cat.AddAttribute("速度", 15, 3);
        cat.AddAttribute("攻击", 8, 1.5f);
        cat.AddSkill("skill_scratch", "抓挠", "用爪子抓挠敌人", 8);
        petInventory.AddPet(cat);
        
        Pet dragon = new Pet("pet_dragon", "小龙", "Mythical", "Rare");
        dragon.AddAttribute("攻击", 20, 4);
        dragon.AddAttribute("防御", 10, 2);
        dragon.AddAttribute("速度", 12, 2.5f);
        dragon.AddSkill("skill_fire_breath", "火焰呼吸", "喷出火焰攻击敌人", 15);
        petInventory.AddPet(dragon);
        
        SavePetData();
    }
    
    public void AddPet(Pet pet)
    {
        petInventory.AddPet(pet);
        SavePetData();
    }
    
    public void RemovePet(string petID)
    {
        petInventory.RemovePet(petID);
        SavePetData();
    }
    
    public void SetActivePet(string petID)
    {
        petInventory.SetActivePet(petID);
        SavePetData();
    }
    
    public Pet GetActivePet()
    {
        return petInventory.GetActivePet();
    }
    
    public List<Pet> GetAllPets()
    {
        return petInventory.pets;
    }
    
    public Pet GetPetByID(string petID)
    {
        return petInventory.GetPetByID(petID);
    }
    
    public void AddPetExp(string petID, int exp)
    {
        Pet pet = petInventory.GetPetByID(petID);
        if (pet != null)
        {
            pet.AddExp(exp);
            SavePetData();
        }
    }
    
    public void UsePetSkill(string petID, string skillID)
    {
        Pet pet = petInventory.GetPetByID(petID);
        if (pet != null)
        {
            PetSkill skill = pet.skills.Find(s => s.skillID == skillID);
            if (skill != null && skill.isReady)
            {
                skill.UseSkill();
                SavePetData();
            }
        }
    }
    
    public void UpdatePetCooldowns(float deltaTime)
    {
        foreach (Pet pet in petInventory.pets)
        {
            foreach (PetSkill skill in pet.skills)
            {
                skill.UpdateCooldown(deltaTime);
            }
        }
    }
    
    public List<Pet> GetPetsByType(string type)
    {
        return petInventory.GetPetsByType(type);
    }
    
    public void SavePetData()
    {
        string path = Application.dataPath + "/Data/pet_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, petInventory);
        stream.Close();
    }
    
    public void LoadPetData()
    {
        string path = Application.dataPath + "/Data/pet_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            petInventory = (PetInventory)formatter.Deserialize(stream);
            stream.Close();
        }
    }
}
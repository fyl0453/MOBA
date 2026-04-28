using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }
    
    public PlayerProfile currentProfile;
    
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
        LoadProfile();
        if (currentProfile == null)
        {
            CreateDefaultProfile();
        }
    }
    
    public void CreateDefaultProfile()
    {
        currentProfile = new PlayerProfile("player_001", "Player1");
        
        // 添加默认英雄
        currentProfile.AddHero("hero_001");
        currentProfile.AddHero("hero_002");
        currentProfile.AddHero("hero_003");
        
        // 添加默认皮肤
        currentProfile.AddSkin("skin_001");
        
        // 添加默认成就
        currentProfile.AddAchievement("achievement_1");
        
        // 添加默认头衔
        currentProfile.AddTitle("title_1");
        
        SaveProfile();
    }
    
    public void LoadProfile()
    {
        string profilePath = Application.dataPath + "/Profiles";
        if (!Directory.Exists(profilePath))
        {
            Directory.CreateDirectory(profilePath);
        }
        
        string fileName = "player_profile.dat";
        string fullPath = Path.Combine(profilePath, fileName);
        
        if (File.Exists(fullPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(fullPath, FileMode.Open);
            currentProfile = (PlayerProfile)formatter.Deserialize(stream);
            stream.Close();
        }
    }
    
    public void SaveProfile()
    {
        string profilePath = Application.dataPath + "/Profiles";
        if (!Directory.Exists(profilePath))
        {
            Directory.CreateDirectory(profilePath);
        }
        
        string fileName = "player_profile.dat";
        string fullPath = Path.Combine(profilePath, fileName);
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(fullPath, FileMode.Create);
        formatter.Serialize(stream, currentProfile);
        stream.Close();
    }
    
    public void UpdatePlayerStats(int kills, int deaths, int assists, bool won)
    {
        if (currentProfile != null)
        {
            currentProfile.UpdateStats(kills, deaths, assists, won);
            SaveProfile();
        }
    }
    
    public void AddExperience(int exp)
    {
        if (currentProfile != null)
        {
            currentProfile.AddExperience(exp);
            SaveProfile();
        }
    }
    
    public void AddHero(string heroID)
    {
        if (currentProfile != null)
        {
            currentProfile.AddHero(heroID);
            SaveProfile();
        }
    }
    
    public void AddSkin(string skinID)
    {
        if (currentProfile != null)
        {
            currentProfile.AddSkin(skinID);
            SaveProfile();
        }
    }
    
    public void AddAchievement(string achievementID)
    {
        if (currentProfile != null)
        {
            currentProfile.AddAchievement(achievementID);
            SaveProfile();
        }
    }
    
    public void AddTitle(string titleID)
    {
        if (currentProfile != null)
        {
            currentProfile.AddTitle(titleID);
            SaveProfile();
        }
    }
    
    public void SetAvatar(string avatarID)
    {
        if (currentProfile != null)
        {
            currentProfile.SetAvatar(avatarID);
            SaveProfile();
        }
    }
    
    public void SetBackground(string backgroundID)
    {
        if (currentProfile != null)
        {
            currentProfile.SetBackground(backgroundID);
            SaveProfile();
        }
    }
    
    public void SetTitle(string titleID)
    {
        if (currentProfile != null)
        {
            currentProfile.SetTitle(titleID);
            SaveProfile();
        }
    }
    
    public void SetSignature(string signature)
    {
        if (currentProfile != null)
        {
            currentProfile.SetSignature(signature);
            SaveProfile();
        }
    }
    
    public PlayerProfile GetCurrentProfile()
    {
        return currentProfile;
    }
}
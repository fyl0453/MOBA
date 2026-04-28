using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SignInManager : MonoBehaviour
{
    public static SignInManager Instance { get; private set; }
    
    public SignInManagerData signInData;
    
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
        LoadSignInData();
        
        if (signInData == null)
        {
            signInData = new SignInManagerData();
        }
        
        EnsurePlayerSignInData();
    }
    
    private void EnsurePlayerSignInData()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        SignInData data = signInData.GetSignInData(playerID);
        if (data == null)
        {
            data = new SignInData(playerID);
            SignInWeek week = new SignInWeek("week_1", 1);
            week.InitializeDefaultRewards();
            data.AddWeek(week);
            signInData.AddSignInData(data);
            SaveSignInData();
        }
    }
    
    public bool CanSignIn()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        SignInData data = signInData.GetSignInData(playerID);
        if (data != null)
        {
            string today = System.DateTime.Now.ToString("yyyy-MM-dd");
            if (data.lastSignDate == today)
            {
                return false;
            }
            return true;
        }
        return false;
    }
    
    public bool SignIn()
    {
        if (!CanSignIn())
        {
            return false;
        }
        
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        SignInData data = signInData.GetSignInData(playerID);
        if (data != null)
        {
            SignInWeek currentWeek = data.GetCurrentWeek();
            if (currentWeek != null)
            {
                SignInDay todaySignIn = currentWeek.days.Find(d => !d.isSigned);
                if (todaySignIn != null)
                {
                    todaySignIn.Sign();
                    data.totalSignDays++;
                    data.totalSignInCount++;
                    data.lastSignDate = System.DateTime.Now.ToString("yyyy-MM-dd");
                    
                    if (data.currentStreak > 0)
                    {
                        data.currentStreak++;
                    }
                    else
                    {
                        data.currentStreak = 1;
                    }
                    
                    GrantReward(todaySignIn);
                    SaveSignInData();
                    return true;
                }
            }
        }
        return false;
    }
    
    private void GrantReward(SignInDay signInDay)
    {
        switch (signInDay.rewardType)
        {
            case "Item":
                InventoryManager.Instance.AddItemToInventory(signInDay.rewardItemID, signInDay.rewardQuantity);
                break;
            case "Currency":
                if (signInDay.rewardItemID == "gold")
                {
                    ProfileManager.Instance.currentProfile.gold += signInDay.rewardQuantity;
                    ProfileManager.Instance.SaveProfile();
                }
                else if (signInDay.rewardItemID == "gems")
                {
                    ProfileManager.Instance.currentProfile.gems += signInDay.rewardQuantity;
                    ProfileManager.Instance.SaveProfile();
                }
                break;
            case "Skin":
                SkinManager.Instance.PurchaseSkin(signInDay.rewardItemID);
                break;
        }
    }
    
    public bool PurchasePremiumWeek()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        SignInData data = signInData.GetSignInData(playerID);
        if (data != null)
        {
            SignInWeek currentWeek = data.GetCurrentWeek();
            if (currentWeek != null && !currentWeek.isPremium)
            {
                if (ProfileManager.Instance.currentProfile.gems >= 100)
                {
                    ProfileManager.Instance.currentProfile.gems -= 100;
                    currentWeek.isPremium = true;
                    ProfileManager.Instance.SaveProfile();
                    SaveSignInData();
                    return true;
                }
            }
        }
        return false;
    }
    
    public void StartNewWeek()
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        SignInData data = signInData.GetSignInData(playerID);
        if (data != null)
        {
            SignInWeek newWeek = new SignInWeek("week_" + (data.weeks.Count + 1), data.weeks.Count + 1);
            newWeek.InitializeDefaultRewards();
            data.AddWeek(newWeek);
            data.ResetStreak();
            SaveSignInData();
        }
    }
    
    public SignInData GetPlayerSignInData()
    {
        return signInData.GetSignInData(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public SignInWeek GetCurrentWeek()
    {
        SignInData data = GetPlayerSignInData();
        if (data != null)
        {
            return data.GetCurrentWeek();
        }
        return null;
    }
    
    public int GetCurrentStreak()
    {
        SignInData data = GetPlayerSignInData();
        if (data != null)
        {
            return data.currentStreak;
        }
        return 0;
    }
    
    public int GetTotalSignDays()
    {
        SignInData data = GetPlayerSignInData();
        if (data != null)
        {
            return data.totalSignDays;
        }
        return 0;
    }
    
    public void SaveSignInData()
    {
        string path = Application.dataPath + "/Data/sign_in_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, signInData);
        stream.Close();
    }
    
    public void LoadSignInData()
    {
        string path = Application.dataPath + "/Data/sign_in_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            signInData = (SignInManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            signInData = new SignInManagerData();
        }
    }
}
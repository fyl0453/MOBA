using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AntiAddictionManagerExtended : MonoBehaviour
{
    public static AntiAddictionManagerExtended Instance { get; private set; }
    
    public AntiAddictionManagerData antiAddictionData;
    
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
        LoadAntiAddictionData();
        
        if (antiAddictionData == null)
        {
            antiAddictionData = new AntiAddictionManagerData();
            InitializeDefaultAntiAddiction();
        }
    }
    
    private void InitializeDefaultAntiAddiction()
    {
        // 创建默认年龄组
        AgeGroup group1 = new AgeGroup(
            "group1",
            "8岁以下",
            0,
            8,
            30,
            180,
            false,
            false,
            0
        );
        
        AgeGroup group2 = new AgeGroup(
            "group2",
            "8-16岁",
            8,
            16,
            60,
            360,
            false,
            true,
            100
        );
        
        AgeGroup group3 = new AgeGroup(
            "group3",
            "16-18岁",
            16,
            18,
            120,
            720,
            true,
            true,
            200
        );
        
        AgeGroup group4 = new AgeGroup(
            "group4",
            "18岁以上",
            18,
            100,
            0,
            0,
            true,
            true,
            0
        );
        
        antiAddictionData.system.AddAgeGroup(group1);
        antiAddictionData.system.AddAgeGroup(group2);
        antiAddictionData.system.AddAgeGroup(group3);
        antiAddictionData.system.AddAgeGroup(group4);
        
        SaveAntiAddictionData();
    }
    
    public string RegisterRealName(string playerID, string realName, string idNumber, int age)
    {
        string authID = System.Guid.NewGuid().ToString();
        RealNameAuth newAuth = new RealNameAuth(authID, playerID, realName, idNumber, age);
        antiAddictionData.system.AddRealNameAuth(newAuth);
        SaveAntiAddictionData();
        Debug.Log($"成功注册实名信息: {realName}");
        return authID;
    }
    
    public void VerifyRealName(string authID)
    {
        RealNameAuth auth = antiAddictionData.system.realNameAuths.Find(a => a.authID == authID);
        if (auth != null)
        {
            auth.Verify();
            SaveAntiAddictionData();
            Debug.Log("成功验证实名信息");
        }
    }
    
    public void RejectRealName(string authID)
    {
        RealNameAuth auth = antiAddictionData.system.realNameAuths.Find(a => a.authID == authID);
        if (auth != null)
        {
            auth.Reject();
            SaveAntiAddictionData();
            Debug.Log("拒绝验证实名信息");
        }
    }
    
    public string CreateParentControl(string playerID, string email, string phone)
    {
        string controlID = System.Guid.NewGuid().ToString();
        ParentControl newControl = new ParentControl(controlID, playerID, email, phone);
        antiAddictionData.system.AddParentControl(newControl);
        SaveAntiAddictionData();
        Debug.Log("成功创建家长控制");
        return controlID;
    }
    
    public void UpdateParentControl(string controlID, bool timeLimit, int dailyLimit, int weeklyLimit, bool rechargeLimit, int rechargeLimitAmount, bool schedule, bool contentRestriction, string contentLevel)
    {
        ParentControl control = antiAddictionData.system.parentControls.Find(c => c.controlID == controlID);
        if (control != null)
        {
            control.UpdateControl(timeLimit, dailyLimit, weeklyLimit, rechargeLimit, rechargeLimitAmount, schedule, contentRestriction, contentLevel);
            SaveAntiAddictionData();
            Debug.Log("成功更新家长控制");
        }
    }
    
    public void AddPlayTimeSlot(string controlID, string day, string start, string end)
    {
        ParentControl control = antiAddictionData.system.parentControls.Find(c => c.controlID == controlID);
        if (control != null)
        {
            string slotID = System.Guid.NewGuid().ToString();
            PlayTimeSlot slot = new PlayTimeSlot(slotID, day, start, end);
            control.AddPlayTimeSlot(slot);
            SaveAntiAddictionData();
            Debug.Log("成功添加游戏时间槽");
        }
    }
    
    public string StartPlayerSession(string playerID)
    {
        string sessionID = System.Guid.NewGuid().ToString();
        PlayerSession newSession = new PlayerSession(sessionID, playerID, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        antiAddictionData.system.AddPlayerSession(newSession);
        SaveAntiAddictionData();
        Debug.Log($"开始游戏会话: {playerID}");
        return sessionID;
    }
    
    public void EndPlayerSession(string sessionID)
    {
        PlayerSession session = antiAddictionData.system.playerSessions.Find(s => s.sessionID == sessionID);
        if (session != null)
        {
            session.EndSession(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SaveAntiAddictionData();
            Debug.Log($"结束游戏会话，游戏时间: {session.playTime}分钟");
        }
    }
    
    public bool CheckPlayTimeLimit(string playerID)
    {
        RealNameAuth auth = antiAddictionData.system.GetRealNameAuth(playerID);
        if (auth == null || !auth.isVerified)
        {
            // 未实名认证，限制游戏时间
            return false;
        }
        
        AgeGroup ageGroup = antiAddictionData.system.GetAgeGroup(auth.age);
        if (ageGroup == null)
        {
            return true; // 无年龄限制
        }
        
        // 检查当日游戏时间
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        List<PlayerSession> todaySessions = antiAddictionData.system.GetPlayerSessions(playerID, today);
        int todayPlayTime = 0;
        foreach (PlayerSession session in todaySessions)
        {
            todayPlayTime += session.playTime;
        }
        
        // 检查家长控制
        ParentControl parentControl = antiAddictionData.system.GetParentControl(playerID);
        if (parentControl != null && parentControl.enableTimeLimit)
        {
            if (todayPlayTime >= parentControl.customDailyLimit)
            {
                return false;
            }
        }
        else
        {
            if (ageGroup.dailyPlayTimeLimit > 0 && todayPlayTime >= ageGroup.dailyPlayTimeLimit)
            {
                return false;
            }
        }
        
        // 检查是否超过晚上9点
        if (!ageGroup.canPlayAfter9pm && System.DateTime.Now.Hour >= 21)
        {
            return false;
        }
        
        // 检查游戏时间槽
        if (parentControl != null && parentControl.enablePlayTimeSchedule)
        {
            string dayOfWeek = System.DateTime.Now.DayOfWeek.ToString();
            string currentTime = System.DateTime.Now.ToString("HH:mm");
            bool inSlot = false;
            
            foreach (PlayTimeSlot slot in parentControl.playTimeSlots)
            {
                if (slot.dayOfWeek == dayOfWeek && 
                    currentTime >= slot.startTime && 
                    currentTime <= slot.endTime)
                {
                    inSlot = true;
                    break;
                }
            }
            
            if (!inSlot)
            {
                return false;
            }
        }
        
        return true;
    }
    
    public bool CheckRechargeLimit(string playerID, int amount)
    {
        RealNameAuth auth = antiAddictionData.system.GetRealNameAuth(playerID);
        if (auth == null || !auth.isVerified)
        {
            // 未实名认证，限制充值
            return false;
        }
        
        AgeGroup ageGroup = antiAddictionData.system.GetAgeGroup(auth.age);
        if (ageGroup == null)
        {
            return true; // 无充值限制
        }
        
        if (!ageGroup.canRecharge)
        {
            return false;
        }
        
        // 检查家长控制
        ParentControl parentControl = antiAddictionData.system.GetParentControl(playerID);
        if (parentControl != null && parentControl.enableRechargeLimit)
        {
            if (amount > parentControl.customRechargeLimit)
            {
                return false;
            }
        }
        else
        {
            if (ageGroup.monthlyRechargeLimit > 0 && amount > ageGroup.monthlyRechargeLimit)
            {
                return false;
            }
        }
        
        return true;
    }
    
    public RealNameAuth GetRealNameAuth(string playerID)
    {
        return antiAddictionData.system.GetRealNameAuth(playerID);
    }
    
    public ParentControl GetParentControl(string playerID)
    {
        return antiAddictionData.system.GetParentControl(playerID);
    }
    
    public int GetTodayPlayTime(string playerID)
    {
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        List<PlayerSession> todaySessions = antiAddictionData.system.GetPlayerSessions(playerID, today);
        int todayPlayTime = 0;
        foreach (PlayerSession session in todaySessions)
        {
            todayPlayTime += session.playTime;
        }
        return todayPlayTime;
    }
    
    public void SaveAntiAddictionData()
    {
        string path = Application.dataPath + "/Data/anti_addiction_extended_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, antiAddictionData);
        stream.Close();
    }
    
    public void LoadAntiAddictionData()
    {
        string path = Application.dataPath + "/Data/anti_addiction_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            antiAddictionData = (AntiAddictionManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            antiAddictionData = new AntiAddictionManagerData();
        }
    }
}
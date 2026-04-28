using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VIPSystemDetailedManager : MonoBehaviour
{
    public static VIPSystemDetailedManager Instance { get; private set; }
    
    public VIPSystemDetailedManagerData vipData;
    
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
        LoadVIPData();
        
        if (vipData == null)
        {
            vipData = new VIPSystemDetailedManagerData();
            InitializeDefaultVIPSystem();
        }
    }
    
    private void InitializeDefaultVIPSystem()
    {
        // 贵族等级
        VIPLevel level0 = new VIPLevel(0, "平民", "普通玩家", 0, 0, "icon_vip_0");
        VIPLevel level1 = new VIPLevel(1, "贵族1", "贵族等级1", 10, 10, "icon_vip_1");
        VIPLevel level2 = new VIPLevel(2, "贵族2", "贵族等级2", 20, 30, "icon_vip_2");
        VIPLevel level3 = new VIPLevel(3, "贵族3", "贵族等级3", 50, 80, "icon_vip_3");
        VIPLevel level4 = new VIPLevel(4, "贵族4", "贵族等级4", 100, 180, "icon_vip_4");
        VIPLevel level5 = new VIPLevel(5, "贵族5", "贵族等级5", 200, 380, "icon_vip_5");
        VIPLevel level6 = new VIPLevel(6, "贵族6", "贵族等级6", 500, 880, "icon_vip_6");
        VIPLevel level7 = new VIPLevel(7, "贵族7", "贵族等级7", 1000, 1880, "icon_vip_7");
        VIPLevel level8 = new VIPLevel(8, "贵族8", "贵族等级8", 2000, 3880, "icon_vip_8");
        VIPLevel level9 = new VIPLevel(9, "贵族9", "贵族等级9", 5000, 8880, "icon_vip_9");
        VIPLevel level10 = new VIPLevel(10, "贵族10", "贵族等级10", 10000, 18880, "icon_vip_10");
        
        vipData.system.AddVIPLevel(level0);
        vipData.system.AddVIPLevel(level1);
        vipData.system.AddVIPLevel(level2);
        vipData.system.AddVIPLevel(level3);
        vipData.system.AddVIPLevel(level4);
        vipData.system.AddVIPLevel(level5);
        vipData.system.AddVIPLevel(level6);
        vipData.system.AddVIPLevel(level7);
        vipData.system.AddVIPLevel(level8);
        vipData.system.AddVIPLevel(level9);
        vipData.system.AddVIPLevel(level10);
        
        // 贵族特权
        VIPPrivilege privilege1 = new VIPPrivilege("privilege_001", "专属头像框", "贵族专属头像框", 1, "decoration", "avatar_frame_vip1", "icon_privilege_1");
        VIPPrivilege privilege2 = new VIPPrivilege("privilege_002", "专属称号", "贵族专属称号", 2, "decoration", "title_vip2", "icon_privilege_2");
        VIPPrivilege privilege3 = new VIPPrivilege("privilege_003", "每日福利", "每日领取额外福利", 3, "daily_reward", "100_gold", "icon_privilege_3");
        VIPPrivilege privilege4 = new VIPPrivilege("privilege_004", "背包扩容", "背包容量增加", 4, "system", "backpack_expand", "icon_privilege_4");
        VIPPrivilege privilege5 = new VIPPrivilege("privilege_005", "专属皮肤", "贵族专属皮肤", 5, "skin", "skin_vip5", "icon_privilege_5");
        VIPPrivilege privilege6 = new VIPPrivilege("privilege_006", "专属英雄", "贵族专属英雄", 6, "hero", "hero_vip6", "icon_privilege_6");
        VIPPrivilege privilege7 = new VIPPrivilege("privilege_007", "专属客服", "专属客服服务", 7, "service", "vip_service", "icon_privilege_7");
        VIPPrivilege privilege8 = new VIPPrivilege("privilege_008", "专属活动", "专属活动参与权", 8, "event", "vip_event", "icon_privilege_8");
        VIPPrivilege privilege9 = new VIPPrivilege("privilege_009", "专属坐骑", "贵族专属坐骑", 9, "mount", "mount_vip9", "icon_privilege_9");
        VIPPrivilege privilege10 = new VIPPrivilege("privilege_010", "专属特效", "贵族专属特效", 10, "effect", "effect_vip10", "icon_privilege_10");
        
        vipData.system.AddVIPPrivilege(privilege1);
        vipData.system.AddVIPPrivilege(privilege2);
        vipData.system.AddVIPPrivilege(privilege3);
        vipData.system.AddVIPPrivilege(privilege4);
        vipData.system.AddVIPPrivilege(privilege5);
        vipData.system.AddVIPPrivilege(privilege6);
        vipData.system.AddVIPPrivilege(privilege7);
        vipData.system.AddVIPPrivilege(privilege8);
        vipData.system.AddVIPPrivilege(privilege9);
        vipData.system.AddVIPPrivilege(privilege10);
        
        // 添加特权到等级
        level1.AddPrivilege("privilege_001");
        level2.AddPrivilege("privilege_002");
        level3.AddPrivilege("privilege_003");
        level4.AddPrivilege("privilege_004");
        level5.AddPrivilege("privilege_005");
        level6.AddPrivilege("privilege_006");
        level7.AddPrivilege("privilege_007");
        level8.AddPrivilege("privilege_008");
        level9.AddPrivilege("privilege_009");
        level10.AddPrivilege("privilege_010");
        
        // 贵族成员
        VIPMember member1 = new VIPMember("user_001", "张三");
        VIPMember member2 = new VIPMember("user_002", "李四");
        VIPMember member3 = new VIPMember("user_003", "王五");
        
        // 添加充值
        member1.AddRecharge(1000);
        member2.AddRecharge(5000);
        member3.AddRecharge(100);
        
        // 领取特权
        member1.ClaimPrivilege("privilege_001");
        member1.ClaimPrivilege("privilege_002");
        member1.ClaimPrivilege("privilege_003");
        member1.ClaimPrivilege("privilege_004");
        member1.ClaimPrivilege("privilege_005");
        member1.ClaimPrivilege("privilege_006");
        member1.ClaimPrivilege("privilege_007");
        
        member2.ClaimPrivilege("privilege_001");
        member2.ClaimPrivilege("privilege_002");
        member2.ClaimPrivilege("privilege_003");
        member2.ClaimPrivilege("privilege_004");
        member2.ClaimPrivilege("privilege_005");
        member2.ClaimPrivilege("privilege_006");
        member2.ClaimPrivilege("privilege_007");
        member2.ClaimPrivilege("privilege_008");
        member2.ClaimPrivilege("privilege_009");
        
        member3.ClaimPrivilege("privilege_001");
        member3.ClaimPrivilege("privilege_002");
        member3.ClaimPrivilege("privilege_003");
        
        vipData.system.AddVIPMember(member1);
        vipData.system.AddVIPMember(member2);
        vipData.system.AddVIPMember(member3);
        
        // 贵族事件
        VIPEvent event1 = new VIPEvent("event_001", "recharge", "user_001", "充值", 1000);
        VIPEvent event2 = new VIPEvent("event_002", "level_up", "user_001", "贵族等级提升", 7);
        VIPEvent event3 = new VIPEvent("event_003", "claim_privilege", "user_001", "领取特权", 0);
        
        vipData.system.AddVIPEvent(event1);
        vipData.system.AddVIPEvent(event2);
        vipData.system.AddVIPEvent(event3);
        
        SaveVIPData();
    }
    
    // 贵族等级管理
    public VIPLevel GetVIPLevel(int level)
    {
        return vipData.system.GetVIPLevel(level);
    }
    
    public List<VIPLevel> GetVIPLevels()
    {
        return vipData.system.GetVIPLevels();
    }
    
    // 贵族成员管理
    public void AddVIPMember(string userID, string userName)
    {
        VIPMember existingMember = vipData.system.GetVIPMember(userID);
        if (existingMember == null)
        {
            VIPMember newMember = new VIPMember(userID, userName);
            vipData.system.AddVIPMember(newMember);
            
            // 创建贵族事件
            CreateVIPEvent("join", userID, "加入贵族系统", 0);
            
            SaveVIPData();
            Debug.Log("成功添加贵族成员: " + userName);
        }
        else
        {
            Debug.LogError("用户已经是贵族成员");
        }
    }
    
    public void AddRecharge(string userID, int amount)
    {
        VIPMember member = vipData.system.GetVIPMember(userID);
        if (member != null)
        {
            int oldLevel = member.vipLevel;
            member.AddRecharge(amount);
            
            // 创建贵族事件
            CreateVIPEvent("recharge", userID, "充值", amount);
            
            // 检查是否升级
            if (member.vipLevel > oldLevel)
            {
                CreateVIPEvent("level_up", userID, "贵族等级提升", member.vipLevel);
            }
            
            SaveVIPData();
            Debug.Log("成功添加充值: " + amount + "，用户: " + member.userName);
        }
        else
        {
            // 如果用户不是贵族成员，创建新成员
            AddVIPMember(userID, "未知用户");
            AddRecharge(userID, amount);
        }
    }
    
    public VIPMember GetVIPMember(string userID)
    {
        return vipData.system.GetVIPMember(userID);
    }
    
    public List<VIPMember> GetVIPMembersByLevel(int level)
    {
        return vipData.system.GetVIPMembersByLevel(level);
    }
    
    // 贵族特权管理
    public void AddVIPPrivilege(string privilegeName, string privilegeDescription, int requiredLevel, string privilegeType, string privilegeValue, string icon)
    {
        string privilegeID = "privilege_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        VIPPrivilege vipPrivilege = new VIPPrivilege(privilegeID, privilegeName, privilegeDescription, requiredLevel, privilegeType, privilegeValue, icon);
        vipData.system.AddVIPPrivilege(vipPrivilege);
        
        // 添加特权到等级
        VIPLevel vipLevel = vipData.system.GetVIPLevel(requiredLevel);
        if (vipLevel != null)
        {
            vipLevel.AddPrivilege(privilegeID);
        }
        
        SaveVIPData();
        Debug.Log("成功添加贵族特权: " + privilegeName);
    }
    
    public void ClaimVIPPrivilege(string userID, string privilegeID)
    {
        VIPMember member = vipData.system.GetVIPMember(userID);
        VIPPrivilege privilege = vipData.system.GetVIPPrivilege(privilegeID);
        
        if (member != null && privilege != null && privilege.IsAvailable(member.vipLevel) && !member.HasClaimedPrivilege(privilegeID))
        {
            member.ClaimPrivilege(privilegeID);
            
            // 创建贵族事件
            CreateVIPEvent("claim_privilege", userID, "领取特权: " + privilege.privilegeName, 0);
            
            // 这里可以添加发放特权的逻辑
            Debug.Log("发放贵族特权: " + privilege.privilegeName + " 给用户: " + member.userName);
            
            SaveVIPData();
            Debug.Log("成功领取贵族特权: " + privilege.privilegeName);
        }
        else
        {
            Debug.LogError("特权不存在或不可领取");
        }
    }
    
    public List<VIPPrivilege> GetVIPPrivilegesByLevel(int level)
    {
        return vipData.system.GetVIPPrivilegesByLevel(level);
    }
    
    public List<VIPPrivilege> GetAvailablePrivileges(string userID)
    {
        VIPMember member = vipData.system.GetVIPMember(userID);
        if (member != null)
        {
            List<VIPPrivilege> allPrivileges = vipData.system.GetVIPPrivilegesByLevel(member.vipLevel);
            return allPrivileges.FindAll(p => !member.HasClaimedPrivilege(p.privilegeID));
        }
        else
        {
            return new List<VIPPrivilege>();
        }
    }
    
    // 贵族事件管理
    public string CreateVIPEvent(string eventType, string userID, string description, int amount)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        VIPEvent vipEvent = new VIPEvent(eventID, eventType, userID, description, amount);
        vipData.system.AddVIPEvent(vipEvent);
        SaveVIPData();
        Debug.Log("成功创建贵族事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        VIPEvent vipEvent = vipData.system.GetVIPEvent(eventID);
        if (vipEvent != null)
        {
            vipEvent.MarkAsCompleted();
            SaveVIPData();
            Debug.Log("成功标记贵族事件为完成");
        }
        else
        {
            Debug.LogError("贵族事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        VIPEvent vipEvent = vipData.system.GetVIPEvent(eventID);
        if (vipEvent != null)
        {
            vipEvent.MarkAsFailed();
            SaveVIPData();
            Debug.Log("成功标记贵族事件为失败");
        }
        else
        {
            Debug.LogError("贵族事件不存在: " + eventID);
        }
    }
    
    public List<VIPEvent> GetUserEvents(string userID)
    {
        return vipData.system.GetVIPEventsByUser(userID);
    }
    
    // 数据持久化
    public void SaveVIPData()
    {
        string path = Application.dataPath + "/Data/vip_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, vipData);
        stream.Close();
    }
    
    public void LoadVIPData()
    {
        string path = Application.dataPath + "/Data/vip_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            vipData = (VIPSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            vipData = new VIPSystemDetailedManagerData();
        }
    }
}
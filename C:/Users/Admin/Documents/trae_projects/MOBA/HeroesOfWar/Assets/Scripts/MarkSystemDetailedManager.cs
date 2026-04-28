using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MarkSystemDetailedManager : MonoBehaviour
{
    public static MarkSystemDetailedManager Instance { get; private set; }
    
    public MarkSystemDetailedManagerData markData;
    
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
        LoadMarkData();
        
        if (markData == null)
        {
            markData = new MarkSystemDetailedManagerData();
            InitializeDefaultMarkSystem();
        }
    }
    
    private void InitializeDefaultMarkSystem()
    {
        // 印记
        Mark mark1 = new Mark("mark_001", "王者印记", "达到王者段位获得", "rank", "legendary", "达到王者段位", "icon_mark_king", "王者专属特效", true);
        Mark mark2 = new Mark("mark_002", "钻石印记", "达到钻石段位获得", "rank", "epic", "达到钻石段位", "icon_mark_diamond", "钻石专属特效", false);
        Mark mark3 = new Mark("mark_003", "铂金印记", "达到铂金段位获得", "rank", "rare", "达到铂金段位", "icon_mark_platinum", "铂金专属特效", false);
        Mark mark4 = new Mark("mark_004", "黄金印记", "达到黄金段位获得", "rank", "uncommon", "达到黄金段位", "icon_mark_gold", "黄金专属特效", false);
        Mark mark5 = new Mark("mark_005", "白银印记", "达到白银段位获得", "rank", "common", "达到白银段位", "icon_mark_silver", "白银专属特效", false);
        Mark mark6 = new Mark("mark_006", "青铜印记", "达到青铜段位获得", "rank", "common", "达到青铜段位", "icon_mark_bronze", "青铜专属特效", false);
        Mark mark7 = new Mark("mark_007", "MVP印记", "获得10次MVP获得", "achievement", "rare", "获得10次MVP", "icon_mark_mvp", "MVP专属特效", false);
        Mark mark8 = new Mark("mark_008", "连胜印记", "获得5连胜获得", "achievement", "epic", "获得5连胜", "icon_mark_win_streak", "连胜专属特效", false);
        Mark mark9 = new Mark("mark_009", "击杀王印记", "单场击杀20人获得", "achievement", "legendary", "单场击杀20人", "icon_mark_kill_king", "击杀王专属特效", true);
        Mark mark10 = new Mark("mark_010", "助攻王印记", "单场助攻30次获得", "achievement", "legendary", "单场助攻30次", "icon_mark_assist_king", "助攻王专属特效", true);
        
        markData.system.AddMark(mark1);
        markData.system.AddMark(mark2);
        markData.system.AddMark(mark3);
        markData.system.AddMark(mark4);
        markData.system.AddMark(mark5);
        markData.system.AddMark(mark6);
        markData.system.AddMark(mark7);
        markData.system.AddMark(mark8);
        markData.system.AddMark(mark9);
        markData.system.AddMark(mark10);
        
        // 玩家印记
        PlayerMark playerMark1 = new PlayerMark("player_mark_001", "user_001", "张三", "mark_001", "王者印记");
        PlayerMark playerMark2 = new PlayerMark("player_mark_002", "user_001", "张三", "mark_007", "MVP印记");
        PlayerMark playerMark3 = new PlayerMark("player_mark_003", "user_001", "张三", "mark_008", "连胜印记");
        PlayerMark playerMark4 = new PlayerMark("player_mark_004", "user_002", "李四", "mark_002", "钻石印记");
        PlayerMark playerMark5 = new PlayerMark("player_mark_005", "user_002", "李四", "mark_007", "MVP印记");
        PlayerMark playerMark6 = new PlayerMark("player_mark_006", "user_003", "王五", "mark_003", "铂金印记");
        
        // 装备印记
        playerMark1.Equip();
        playerMark4.Equip();
        playerMark6.Equip();
        
        markData.system.AddPlayerMark(playerMark1);
        markData.system.AddPlayerMark(playerMark2);
        markData.system.AddPlayerMark(playerMark3);
        markData.system.AddPlayerMark(playerMark4);
        markData.system.AddPlayerMark(playerMark5);
        markData.system.AddPlayerMark(playerMark6);
        
        // 印记事件
        MarkEvent event1 = new MarkEvent("event_001", "obtain", "user_001", "mark_001", "获得王者印记");
        MarkEvent event2 = new MarkEvent("event_002", "equip", "user_001", "mark_001", "装备王者印记");
        MarkEvent event3 = new MarkEvent("event_003", "obtain", "user_002", "mark_002", "获得钻石印记");
        
        markData.system.AddMarkEvent(event1);
        markData.system.AddMarkEvent(event2);
        markData.system.AddMarkEvent(event3);
        
        SaveMarkData();
    }
    
    // 印记管理
    public void AddMark(string markName, string markDescription, string markType, string rarity, string获取条件, string icon, string effect, bool isLimited)
    {
        string markID = "mark_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Mark mark = new Mark(markID, markName, markDescription, markType, rarity,获取条件, icon, effect, isLimited);
        markData.system.AddMark(mark);
        
        SaveMarkData();
        Debug.Log("成功添加印记: " + markName);
    }
    
    public List<Mark> GetMarksByType(string markType)
    {
        return markData.system.GetMarksByType(markType);
    }
    
    public List<Mark> GetAllMarks()
    {
        return markData.system.marks;
    }
    
    // 玩家印记管理
    public void GrantMarkToPlayer(string userID, string userName, string markID)
    {
        Mark mark = markData.system.GetMark(markID);
        if (mark != null && mark.IsAvailable())
        {
            PlayerMark existingPlayerMark = markData.system.GetPlayerMark(userID, markID);
            if (existingPlayerMark == null)
            {
                string playerMarkID = "player_mark_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                PlayerMark newPlayerMark = new PlayerMark(playerMarkID, userID, userName, markID, mark.markName);
                markData.system.AddPlayerMark(newPlayerMark);
                
                // 创建印记事件
                CreateMarkEvent("obtain", userID, markID, "获得印记: " + mark.markName);
                
                SaveMarkData();
                Debug.Log("成功授予玩家印记: " + mark.markName);
            }
            else
            {
                Debug.LogError("玩家已经拥有该印记");
            }
        }
        else
        {
            Debug.LogError("印记不存在或已禁用");
        }
    }
    
    public void EquipMark(string userID, string markID)
    {
        PlayerMark playerMark = markData.system.GetPlayerMark(userID, markID);
        if (playerMark != null && playerMark.IsOwned())
        {
            // 先取消所有其他印记的装备状态
            List<PlayerMark> playerMarks = markData.system.GetPlayerMarksByUser(userID);
            foreach (PlayerMark pm in playerMarks)
            {
                pm.Unequip();
            }
            
            // 装备当前印记
            playerMark.Equip();
            
            // 创建印记事件
            Mark mark = markData.system.GetMark(markID);
            if (mark != null)
            {
                CreateMarkEvent("equip", userID, markID, "装备印记: " + mark.markName);
            }
            
            SaveMarkData();
            Debug.Log("成功装备印记: " + playerMark.markName);
        }
        else
        {
            Debug.LogError("玩家印记不存在或未拥有");
        }
    }
    
    public void UnequipMark(string userID, string markID)
    {
        PlayerMark playerMark = markData.system.GetPlayerMark(userID, markID);
        if (playerMark != null && playerMark.IsOwned() && playerMark.IsEquipped())
        {
            playerMark.Unequip();
            
            // 创建印记事件
            Mark mark = markData.system.GetMark(markID);
            if (mark != null)
            {
                CreateMarkEvent("unequip", userID, markID, "卸下印记: " + mark.markName);
            }
            
            SaveMarkData();
            Debug.Log("成功卸下印记: " + playerMark.markName);
        }
        else
        {
            Debug.LogError("玩家印记不存在或未装备");
        }
    }
    
    public List<PlayerMark> GetPlayerMarks(string userID)
    {
        return markData.system.GetPlayerMarksByUser(userID);
    }
    
    public PlayerMark GetPlayerMark(string userID, string markID)
    {
        return markData.system.GetPlayerMark(userID, markID);
    }
    
    // 印记事件管理
    public string CreateMarkEvent(string eventType, string userID, string markID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MarkEvent markEvent = new MarkEvent(eventID, eventType, userID, markID, description);
        markData.system.AddMarkEvent(markEvent);
        SaveMarkData();
        Debug.Log("成功创建印记事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        MarkEvent markEvent = markData.system.GetMarkEvent(eventID);
        if (markEvent != null)
        {
            markEvent.MarkAsCompleted();
            SaveMarkData();
            Debug.Log("成功标记印记事件为完成");
        }
        else
        {
            Debug.LogError("印记事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        MarkEvent markEvent = markData.system.GetMarkEvent(eventID);
        if (markEvent != null)
        {
            markEvent.MarkAsFailed();
            SaveMarkData();
            Debug.Log("成功标记印记事件为失败");
        }
        else
        {
            Debug.LogError("印记事件不存在: " + eventID);
        }
    }
    
    public List<MarkEvent> GetUserEvents(string userID)
    {
        return markData.system.GetMarkEventsByUser(userID);
    }
    
    // 数据持久化
    public void SaveMarkData()
    {
        string path = Application.dataPath + "/Data/mark_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, markData);
        stream.Close();
    }
    
    public void LoadMarkData()
    {
        string path = Application.dataPath + "/Data/mark_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            markData = (MarkSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            markData = new MarkSystemDetailedManagerData();
        }
    }
}
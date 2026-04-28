using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class KingMarkSystemDetailedManager : MonoBehaviour
{
    public static KingMarkSystemDetailedManager Instance { get; private set; }

    public KingMarkSystemDetailedManagerData kingMarkData;

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
        LoadKingMarkData();

        if (kingMarkData == null)
        {
            kingMarkData = new KingMarkSystemDetailedManagerData();
            InitializeDefaultKingMarkSystem();
        }
    }

    private void InitializeDefaultKingMarkSystem()
    {
        // 王者印记
        KingMark mark1 = new KingMark("mark_001", "S1王者印记", "S1赛季王者专属印记", "legendary", "S1赛季达到王者段位", "icon_mark_s1", "王者专属特效", true);
        KingMark mark2 = new KingMark("mark_002", "S2王者印记", "S2赛季王者专属印记", "legendary", "S2赛季达到王者段位", "icon_mark_s2", "王者专属特效", true);
        KingMark mark3 = new KingMark("mark_003", "S3星耀印记", "S3赛季星耀专属印记", "epic", "S3赛季达到星耀段位", "icon_mark_s3", "星耀专属特效", false);
        KingMark mark4 = new KingMark("mark_004", "S4钻石印记", "S4赛季钻石专属印记", "rare", "S4赛季达到钻石段位", "icon_mark_s4", "钻石专属特效", false);
        KingMark mark5 = new KingMark("mark_005", "S5钻石印记", "S5赛季钻石专属印记", "rare", "S5赛季达到钻石段位", "icon_mark_s5", "钻石专属特效", false);

        kingMarkData.system.AddKingMark(mark1);
        kingMarkData.system.AddKingMark(mark2);
        kingMarkData.system.AddKingMark(mark3);
        kingMarkData.system.AddKingMark(mark4);
        kingMarkData.system.AddKingMark(mark5);

        // 赛季
        SeasonMark season1 = new SeasonMark("season_001", "S1赛季", "2024-01-01 00:00:00", "2024-03-31 23:59:59", 1);
        SeasonMark season2 = new SeasonMark("season_002", "S2赛季", "2024-04-01 00:00:00", "2024-06-30 23:59:59", 1);
        SeasonMark season3 = new SeasonMark("season_003", "S3赛季", "2024-07-01 00:00:00", "2024-09-30 23:59:59", 1);
        season1.End();
        season2.End();

        kingMarkData.system.AddSeasonMark(season1);
        kingMarkData.system.AddSeasonMark(season2);
        kingMarkData.system.AddSeasonMark(season3);

        // 玩家王者印记
        PlayerKingMark playerMark1 = new PlayerKingMark("player_mark_001", "user_001", "张三", "mark_001", "S1王者印记", "season_001");
        PlayerKingMark playerMark2 = new PlayerKingMark("player_mark_002", "user_001", "张三", "mark_002", "S2王者印记", "season_002");
        PlayerKingMark playerMark3 = new PlayerKingMark("player_mark_003", "user_002", "李四", "mark_003", "S3星耀印记", "season_003");
        PlayerKingMark playerMark4 = new PlayerKingMark("player_mark_004", "user_002", "李四", "mark_004", "S4钻石印记", "season_004");

        playerMark1.Equip();

        kingMarkData.system.AddPlayerKingMark(playerMark1);
        kingMarkData.system.AddPlayerKingMark(playerMark2);
        kingMarkData.system.AddPlayerKingMark(playerMark3);
        kingMarkData.system.AddPlayerKingMark(playerMark4);

        // 王者印记事件
        KingMarkEvent event1 = new KingMarkEvent("event_001", "obtain", "user_001", "mark_001", "获得S1王者印记");
        KingMarkEvent event2 = new KingMarkEvent("event_002", "equip", "user_001", "mark_001", "装备S1王者印记");
        KingMarkEvent event3 = new KingMarkEvent("event_003", "obtain", "user_002", "mark_003", "获得S3星耀印记");

        kingMarkData.system.AddKingMarkEvent(event1);
        kingMarkData.system.AddKingMarkEvent(event2);
        kingMarkData.system.AddKingMarkEvent(event3);

        SaveKingMarkData();
    }

    // 王者印记管理
    public void AddKingMark(string markName, string markDescription, string rarity, string obtainCondition, string icon, string effect, bool isLimited)
    {
        string markID = "mark_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        KingMark kingMark = new KingMark(markID, markName, markDescription, rarity, obtainCondition, icon, effect, isLimited);
        kingMarkData.system.AddKingMark(kingMark);
        SaveKingMarkData();
        Debug.Log("成功添加王者印记: " + markName);
    }

    public List<KingMark> GetKingMarksByRarity(string rarity)
    {
        return kingMarkData.system.GetKingMarksByRarity(rarity);
    }

    // 赛季管理
    public void CreateSeason(string seasonName, string startTime, string endTime, int maxRank)
    {
        string seasonID = "season_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SeasonMark seasonMark = new SeasonMark(seasonID, seasonName, startTime, endTime, maxRank);
        kingMarkData.system.AddSeasonMark(seasonMark);
        SaveKingMarkData();
        Debug.Log("成功创建赛季: " + seasonName);
    }

    public void StartSeason(string seasonID)
    {
        SeasonMark seasonMark = kingMarkData.system.GetSeasonMark(seasonID);
        if (seasonMark != null)
        {
            seasonMark.Start();
            SaveKingMarkData();
            Debug.Log("成功开始赛季: " + seasonMark.seasonName);
        }
    }

    public void EndSeason(string seasonID)
    {
        SeasonMark seasonMark = kingMarkData.system.GetSeasonMark(seasonID);
        if (seasonMark != null)
        {
            seasonMark.End();
            SaveKingMarkData();
            Debug.Log("成功结束赛季: " + seasonMark.seasonName);
        }
    }

    public List<SeasonMark> GetActiveSeasons()
    {
        return kingMarkData.system.GetSeasonMarksByStatus("active");
    }

    // 玩家王者印记管理
    public void GrantKingMarkToPlayer(string userID, string userName, string markID, string seasonID)
    {
        KingMark kingMark = kingMarkData.system.GetKingMark(markID);
        if (kingMark != null)
        {
            PlayerKingMark existingPlayerMark = kingMarkData.system.GetPlayerKingMark(userID, markID);
            if (existingPlayerMark == null)
            {
                string playerMarkID = "player_mark_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                PlayerKingMark newPlayerMark = new PlayerKingMark(playerMarkID, userID, userName, markID, kingMark.markName, seasonID);
                kingMarkData.system.AddPlayerKingMark(newPlayerMark);

                CreateKingMarkEvent("obtain", userID, markID, "获得王者印记: " + kingMark.markName);
                SaveKingMarkData();
                Debug.Log("成功授予玩家王者印记: " + kingMark.markName);
            }
            else
            {
                Debug.LogError("玩家已经拥有该王者印记");
            }
        }
        else
        {
            Debug.LogError("王者印记不存在");
        }
    }

    public void EquipKingMark(string userID, string markID)
    {
        PlayerKingMark playerMark = kingMarkData.system.GetPlayerKingMark(userID, markID);
        if (playerMark != null)
        {
            // 先取消所有其他印记的装备状态
            List<PlayerKingMark> playerMarks = kingMarkData.system.GetPlayerKingMarksByUser(userID);
            foreach (PlayerKingMark pm in playerMarks)
            {
                pm.Unequip();
            }

            playerMark.Equip();
            CreateKingMarkEvent("equip", userID, markID, "装备王者印记: " + playerMark.markName);
            SaveKingMarkData();
            Debug.Log("成功装备王者印记: " + playerMark.markName);
        }
        else
        {
            Debug.LogError("玩家印记不存在");
        }
    }

    public void UnequipKingMark(string userID, string markID)
    {
        PlayerKingMark playerMark = kingMarkData.system.GetPlayerKingMark(userID, markID);
        if (playerMark != null)
        {
            playerMark.Unequip();
            CreateKingMarkEvent("unequip", userID, markID, "卸下王者印记: " + playerMark.markName);
            SaveKingMarkData();
            Debug.Log("成功卸下王者印记: " + playerMark.markName);
        }
        else
        {
            Debug.LogError("玩家印记不存在");
        }
    }

    public List<PlayerKingMark> GetPlayerKingMarks(string userID)
    {
        return kingMarkData.system.GetPlayerKingMarksByUser(userID);
    }

    // 王者印记事件管理
    public string CreateKingMarkEvent(string eventType, string userID, string markID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        KingMarkEvent kingMarkEvent = new KingMarkEvent(eventID, eventType, userID, markID, description);
        kingMarkData.system.AddKingMarkEvent(kingMarkEvent);
        SaveKingMarkData();
        Debug.Log("成功创建王者印记事件: " + eventType);
        return eventID;
    }

    public List<KingMarkEvent> GetUserEvents(string userID)
    {
        return kingMarkData.system.GetKingMarkEventsByUser(userID);
    }

    // 数据持久化
    public void SaveKingMarkData()
    {
        string path = Application.dataPath + "/Data/king_mark_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, kingMarkData);
        stream.Close();
    }

    public void LoadKingMarkData()
    {
        string path = Application.dataPath + "/Data/king_mark_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            kingMarkData = (KingMarkSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            kingMarkData = new KingMarkSystemDetailedManagerData();
        }
    }
}
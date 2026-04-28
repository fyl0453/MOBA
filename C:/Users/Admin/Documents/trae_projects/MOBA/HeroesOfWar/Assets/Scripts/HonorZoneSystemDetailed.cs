using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class HonorZone
{
    public string ZoneID;
    public string ZoneName;
    public string Province;
    public string City;
    public string District;
    public int Population;
    public DateTime LastUpdateTime;
    public bool IsActive;

    public HonorZone(string zoneID, string zoneName, string province, string city, string district)
    {
        ZoneID = zoneID;
        ZoneName = zoneName;
        Province = province;
        City = city;
        District = district;
        Population = 0;
        LastUpdateTime = DateTime.Now;
        IsActive = true;
    }
}

[Serializable]
public class HonorRank
{
    public string RankID;
    public string PlayerID;
    public string PlayerName;
    public string ZoneID;
    public string HeroID;
    public string HeroName;
    public int HonorPoints;
    public int Rank;
    public int Change;
    public DateTime UpdateTime;
    public string Title;

    public HonorRank(string rankID, string playerID, string playerName, string zoneID, string heroID, string heroName, int honorPoints)
    {
        RankID = rankID;
        PlayerID = playerID;
        PlayerName = playerName;
        ZoneID = zoneID;
        HeroID = heroID;
        HeroName = heroName;
        HonorPoints = honorPoints;
        Rank = 0;
        Change = 0;
        UpdateTime = DateTime.Now;
        Title = "";
    }
}

[Serializable]
public class HonorTitle
{
    public string TitleID;
    public string TitleName;
    public string TitleType;
    public int MinRank;
    public int MaxRank;
    public string Description;
    public string IconName;
    public bool IsActive;

    public HonorTitle(string titleID, string titleName, string titleType, int minRank, int maxRank, string description)
    {
        TitleID = titleID;
        TitleName = titleName;
        TitleType = titleType;
        MinRank = minRank;
        MaxRank = maxRank;
        Description = description;
        IconName = "";
        IsActive = true;
    }
}

[Serializable]
public class PlayerHonorData
{
    public string PlayerID;
    public string CurrentZoneID;
    public List<HonorRank> HeroRanks;
    public int TotalHonorPoints;
    public int HighestRank;
    public List<string> CurrentTitles;
    public DateTime LastZoneUpdate;
    public DateTime LastRankUpdate;

    public PlayerHonorData(string playerID)
    {
        PlayerID = playerID;
        CurrentZoneID = "";
        HeroRanks = new List<HonorRank>();
        TotalHonorPoints = 0;
        HighestRank = 0;
        CurrentTitles = new List<string>();
        LastZoneUpdate = DateTime.MinValue;
        LastRankUpdate = DateTime.MinValue;
    }
}

[Serializable]
public class HonorZoneData
{
    public List<HonorZone> AllZones;
    public List<HonorRank> AllRanks;
    public List<HonorTitle> AllTitles;
    public Dictionary<string, PlayerHonorData> PlayerHonorData;
    public int HonorPointsPerWin;
    public int HonorPointsPerLose;
    public int TitleUpdateInterval;
    public DateTime LastSystemUpdate;

    public HonorZoneData()
    {
        AllZones = new List<HonorZone>();
        AllRanks = new List<HonorRank>();
        AllTitles = new List<HonorTitle>();
        PlayerHonorData = new Dictionary<string, PlayerHonorData>();
        HonorPointsPerWin = 10;
        HonorPointsPerLose = -3;
        TitleUpdateInterval = 24;
        LastSystemUpdate = DateTime.Now;
        InitializeDefaultZones();
        InitializeDefaultTitles();
    }

    private void InitializeDefaultZones()
    {
        HonorZone zone1 = new HonorZone("zone_001", "北京市海淀区", "北京市", "北京市", "海淀区");
        AllZones.Add(zone1);

        HonorZone zone2 = new HonorZone("zone_002", "上海市浦东新区", "上海市", "上海市", "浦东新区");
        AllZones.Add(zone2);

        HonorZone zone3 = new HonorZone("zone_003", "广州市天河区", "广东省", "广州市", "天河区");
        AllZones.Add(zone3);

        HonorZone zone4 = new HonorZone("zone_004", "深圳市南山区", "广东省", "深圳市", "南山区");
        AllZones.Add(zone4);

        HonorZone zone5 = new HonorZone("zone_005", "杭州市西湖区", "浙江省", "杭州市", "西湖区");
        AllZones.Add(zone5);
    }

    private void InitializeDefaultTitles()
    {
        AllTitles.Add(new HonorTitle("title_001", "区服第1", "district", 1, 1, "本地区域排名第一"));
        AllTitles.Add(new HonorTitle("title_002", "区服第2", "district", 2, 2, "本地区域排名第二"));
        AllTitles.Add(new HonorTitle("title_003", "区服第3", "district", 3, 3, "本地区域排名第三"));
        AllTitles.Add(new HonorTitle("title_004", "区服前10", "district", 4, 10, "本地区域排名前10"));
        AllTitles.Add(new HonorTitle("title_005", "区服前50", "district", 11, 50, "本地区域排名前50"));
        AllTitles.Add(new HonorTitle("title_006", "城市第1", "city", 1, 1, "本市排名第一"));
        AllTitles.Add(new HonorTitle("title_007", "城市第2", "city", 2, 2, "本市排名第二"));
        AllTitles.Add(new HonorTitle("title_008", "城市第3", "city", 3, 3, "本市排名第三"));
        AllTitles.Add(new HonorTitle("title_009", "城市前10", "city", 4, 10, "本市排名前10"));
        AllTitles.Add(new HonorTitle("title_010", "城市前50", "city", 11, 50, "本市排名前50"));
        AllTitles.Add(new HonorTitle("title_011", "省份第1", "province", 1, 1, "本省排名第一"));
        AllTitles.Add(new HonorTitle("title_012", "省份第2", "province", 2, 2, "本省排名第二"));
        AllTitles.Add(new HonorTitle("title_013", "省份第3", "province", 3, 3, "本省排名第三"));
        AllTitles.Add(new HonorTitle("title_014", "省份前10", "province", 4, 10, "本省排名前10"));
        AllTitles.Add(new HonorTitle("title_015", "省份前50", "province", 11, 50, "本省排名前50"));
    }

    public void AddZone(HonorZone zone)
    {
        AllZones.Add(zone);
    }

    public void AddRank(HonorRank rank)
    {
        AllRanks.Add(rank);
    }

    public void AddTitle(HonorTitle title)
    {
        AllTitles.Add(title);
    }

    public void AddPlayerHonorData(string playerID, PlayerHonorData data)
    {
        PlayerHonorData[playerID] = data;
    }
}

[Serializable]
public class HonorEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string ZoneID;
    public string EventData;

    public HonorEvent(string eventID, string eventType, string playerID, string zoneID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        ZoneID = zoneID;
        EventData = eventData;
    }
}

public class HonorZoneDataManager
{
    private static HonorZoneDataManager _instance;
    public static HonorZoneDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HonorZoneDataManager();
            }
            return _instance;
        }
    }

    public HonorZoneData honorData;
    private List<HonorEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private HonorZoneDataManager()
    {
        honorData = new HonorZoneData();
        recentEvents = new List<HonorEvent>();
        LoadHonorData();
    }

    public void SaveHonorData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "HonorZoneData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, honorData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存荣耀战区数据失败: " + e.Message);
        }
    }

    public void LoadHonorData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "HonorZoneData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    honorData = (HonorZoneData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载荣耀战区数据失败: " + e.Message);
            honorData = new HonorZoneData();
        }
    }

    public void CreateHonorEvent(string eventType, string playerID, string zoneID, string eventData)
    {
        string eventID = "honor_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HonorEvent honorEvent = new HonorEvent(eventID, eventType, playerID, zoneID, eventData);
        recentEvents.Add(honorEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<HonorEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
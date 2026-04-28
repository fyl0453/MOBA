using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class HeroGuide
{
    public string GuideID;
    public string HeroID;
    public string GuideTitle;
    public string AuthorID;
    public string AuthorName;
    public int GuideType;
    public string Content;
    public List<string> RecommendedItems;
    public List<string> SkillOrder;
    public List<string> TalentConfig;
    public int UpvoteCount;
    public int DownvoteCount;
    public int ViewCount;
    public double Rating;
    public int UsageCount;
    public DateTime CreateTime;
    public DateTime LastUpdateTime;
    public bool IsOfficial;
    public bool IsFeatured;
    public string Tags;

    public HeroGuide(string guideID, string heroID, string guideTitle, string authorID, string authorName, int guideType, string content)
    {
        GuideID = guideID;
        HeroID = heroID;
        GuideTitle = guideTitle;
        AuthorID = authorID;
        AuthorName = authorName;
        GuideType = guideType;
        Content = content;
        RecommendedItems = new List<string>();
        SkillOrder = new List<string>();
        TalentConfig = new List<string>();
        UpvoteCount = 0;
        DownvoteCount = 0;
        ViewCount = 0;
        Rating = 0.0;
        UsageCount = 0;
        CreateTime = DateTime.Now;
        LastUpdateTime = DateTime.Now;
        IsOfficial = false;
        IsFeatured = false;
        Tags = "";
    }
}

[Serializable]
public class ComboSkill
{
    public string ComboID;
    public string HeroID;
    public string ComboName;
    public List<string> SkillSequence;
    public string Timing;
    public string Description;
    public string VideoURL;
    public int Difficulty;
    public double SuccessRate;

    public ComboSkill(string comboID, string heroID, string comboName, List<string> skillSequence, string timing, string description, int difficulty)
    {
        ComboID = comboID;
        HeroID = heroID;
        ComboName = comboName;
        SkillSequence = skillSequence;
        Timing = timing;
        Description = description;
        VideoURL = "";
        Difficulty = difficulty;
        SuccessRate = 0.0;
    }
}

[Serializable]
public class EquipmentBuild
{
    public string BuildID;
    public string HeroID;
    public string BuildName;
    public List<string> CoreItems;
    public List<string> OptionalItems;
    public List<string> EarlyGameItems;
    public List<string> LateGameItems;
    public string Strategy;
    public int WinRate;
    public int UsageCount;
    public string CreatorID;
    public DateTime CreateTime;

    public EquipmentBuild(string buildID, string heroID, string buildName, string creatorID)
    {
        BuildID = buildID;
        HeroID = heroID;
        BuildName = buildName;
        CoreItems = new List<string>();
        OptionalItems = new List<string>();
        EarlyGameItems = new List<string>();
        LateGameItems = new List<string>();
        Strategy = "";
        WinRate = 0;
        UsageCount = 0;
        CreatorID = creatorID;
        CreateTime = DateTime.Now;
    }
}

[Serializable]
public class GuideComment
{
    public string CommentID;
    public string GuideID;
    public string PlayerID;
    public string PlayerName;
    public string Content;
    public int LikeCount;
    public DateTime CreateTime;
    public string ParentCommentID;

    public GuideComment(string commentID, string guideID, string playerID, string playerName, string content, string parentCommentID = "")
    {
        CommentID = commentID;
        GuideID = guideID;
        PlayerID = playerID;
        PlayerName = playerName;
        Content = content;
        LikeCount = 0;
        CreateTime = DateTime.Now;
        ParentCommentID = parentCommentID;
    }
}

[Serializable]
public class GuideSystemData
{
    public List<HeroGuide> AllGuides;
    public Dictionary<string, List<HeroGuide>> HeroGuides;
    public List<ComboSkill> AllCombos;
    public List<EquipmentBuild> AllBuilds;
    public List<GuideComment> AllComments;
    public Dictionary<string, List<string>> PlayerUpvotes;
    public Dictionary<string, List<string>> PlayerDownvotes;
    public Dictionary<string, List<string>> PlayerFavoriteGuides;
    public List<string> FeaturedGuideIDs;
    public DateTime LastCleanupTime;

    public GuideSystemData()
    {
        AllGuides = new List<HeroGuide>();
        HeroGuides = new Dictionary<string, List<HeroGuide>>();
        AllCombos = new List<ComboSkill>();
        AllBuilds = new List<EquipmentBuild>();
        AllComments = new List<GuideComment>();
        PlayerUpvotes = new Dictionary<string, List<string>>();
        PlayerDownvotes = new Dictionary<string, List<string>>();
        PlayerFavoriteGuides = new Dictionary<string, List<string>>();
        FeaturedGuideIDs = new List<string>();
        LastCleanupTime = DateTime.Now;
    }

    public void AddGuide(HeroGuide guide)
    {
        AllGuides.Add(guide);
        if (!HeroGuides.ContainsKey(guide.HeroID))
        {
            HeroGuides[guide.HeroID] = new List<HeroGuide>();
        }
        HeroGuides[guide.HeroID].Add(guide);
    }

    public void AddCombo(ComboSkill combo)
    {
        AllCombos.Add(combo);
    }

    public void AddBuild(EquipmentBuild build)
    {
        AllBuilds.Add(build);
    }

    public void AddComment(GuideComment comment)
    {
        AllComments.Add(comment);
    }
}

[Serializable]
public class GuideEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string RelatedGuideID;
    public string EventData;

    public GuideEvent(string eventID, string eventType, string playerID, string relatedGuideID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        RelatedGuideID = relatedGuideID;
        EventData = eventData;
    }
}

public class GuideSystemDataManager
{
    private static GuideSystemDataManager _instance;
    public static GuideSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GuideSystemDataManager();
            }
            return _instance;
        }
    }

    public GuideSystemData guideData;
    private List<GuideEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private GuideSystemDataManager()
    {
        guideData = new GuideSystemData();
        recentEvents = new List<GuideEvent>();
        LoadGuideData();
    }

    public void SaveGuideData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "GuideSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, guideData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存攻略系统数据失败: " + e.Message);
        }
    }

    public void LoadGuideData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "GuideSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    guideData = (GuideSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载攻略系统数据失败: " + e.Message);
            guideData = new GuideSystemData();
        }
    }

    public void CreateGuideEvent(string eventType, string playerID, string relatedGuideID, string eventData)
    {
        string eventID = "guide_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuideEvent guideEvent = new GuideEvent(eventID, eventType, playerID, relatedGuideID, eventData);
        recentEvents.Add(guideEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<GuideEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
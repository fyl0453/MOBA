using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class CampArticle
{
    public string ArticleID;
    public string Title;
    public string Content;
    public string AuthorID;
    public string AuthorName;
    public int ArticleType;
    public List<string> Tags;
    public int ViewCount;
    public int LikeCount;
    public int CommentCount;
    public DateTime CreateTime;
    public DateTime LastUpdateTime;
    public bool IsPinned;
    public bool IsFeatured;
    public string CoverImageURL;

    public CampArticle(string articleID, string title, string content, string authorID, string authorName, int articleType)
    {
        ArticleID = articleID;
        Title = title;
        Content = content;
        AuthorID = authorID;
        AuthorName = authorName;
        ArticleType = articleType;
        Tags = new List<string>();
        ViewCount = 0;
        LikeCount = 0;
        CommentCount = 0;
        CreateTime = DateTime.Now;
        LastUpdateTime = DateTime.Now;
        IsPinned = false;
        IsFeatured = false;
        CoverImageURL = "";
    }
}

[Serializable]
public class ArticleComment
{
    public string CommentID;
    public string ArticleID;
    public string PlayerID;
    public string PlayerName;
    public string Content;
    public int LikeCount;
    public DateTime CreateTime;
    public string ParentCommentID;

    public ArticleComment(string commentID, string articleID, string playerID, string playerName, string content, string parentCommentID = "")
    {
        CommentID = commentID;
        ArticleID = articleID;
        PlayerID = playerID;
        PlayerName = playerName;
        Content = content;
        LikeCount = 0;
        CreateTime = DateTime.Now;
        ParentCommentID = parentCommentID;
    }
}

[Serializable]
public class CampVideo
{
    public string VideoID;
    public string Title;
    public string Description;
    public string AuthorID;
    public string AuthorName;
    public string VideoURL;
    public string ThumbnailURL;
    public int Duration;
    public int ViewCount;
    public int LikeCount;
    public int CommentCount;
    public int Category;
    public List<string> Tags;
    public DateTime UploadTime;
    public bool IsFeatured;

    public CampVideo(string videoID, string title, string description, string authorID, string authorName, string videoURL, string thumbnailURL, int category)
    {
        VideoID = videoID;
        Title = title;
        Description = description;
        AuthorID = authorID;
        AuthorName = authorName;
        VideoURL = videoURL;
        ThumbnailURL = thumbnailURL;
        Duration = 0;
        ViewCount = 0;
        LikeCount = 0;
        CommentCount = 0;
        Category = category;
        Tags = new List<string>();
        UploadTime = DateTime.Now;
        IsFeatured = false;
    }
}

[Serializable]
public class CampStreamer
{
    public string StreamerID;
    public string StreamerName;
    public string AvatarURL;
    public int FollowerCount;
    public int TotalViews;
    public double Rating;
    public bool IsLive;
    public string CurrentStreamTitle;
    public List<string> SpecialtyHeroes;
    public DateTime LastStreamTime;

    public CampStreamer(string streamerID, string streamerName)
    {
        StreamerID = streamerID;
        StreamerName = streamerName;
        AvatarURL = "";
        FollowerCount = 0;
        TotalViews = 0;
        Rating = 0.0;
        IsLive = false;
        CurrentStreamTitle = "";
        SpecialtyHeroes = new List<string>();
        LastStreamTime = DateTime.MinValue;
    }
}

[Serializable]
public class PlayerCampActivity
{
    public string PlayerID;
    public int TotalArticles;
    public int TotalComments;
    public int TotalLikes;
    public int TotalVideos;
    public int ActivityScore;
    public DateTime LastActivityTime;
    public List<string> FavoriteArticles;
    public List<string> FavoriteVideos;
    public List<string> FollowedStreamers;

    public PlayerCampActivity(string playerID)
    {
        PlayerID = playerID;
        TotalArticles = 0;
        TotalComments = 0;
        TotalLikes = 0;
        TotalVideos = 0;
        ActivityScore = 0;
        LastActivityTime = DateTime.Now;
        FavoriteArticles = new List<string>();
        FavoriteVideos = new List<string>();
        FollowedStreamers = new List<string>();
    }
}

[Serializable]
public class CampSystemData
{
    public List<CampArticle> Articles;
    public List<ArticleComment> Comments;
    public List<CampVideo> Videos;
    public List<CampStreamer> Streamers;
    public Dictionary<string, PlayerCampActivity> PlayerActivities;
    public Dictionary<string, List<string>> PlayerFavoriteArticles;
    public Dictionary<string, List<string>> PlayerFavoriteVideos;
    public Dictionary<string, List<string>> PlayerFollowedStreamers;
    public List<string> FeaturedArticleIDs;
    public List<string> FeaturedVideoIDs;
    public DateTime LastCleanupTime;

    public CampSystemData()
    {
        Articles = new List<CampArticle>();
        Comments = new List<ArticleComment>();
        Videos = new List<CampVideo>();
        Streamers = new List<CampStreamer>();
        PlayerActivities = new Dictionary<string, PlayerCampActivity>();
        PlayerFavoriteArticles = new Dictionary<string, List<string>>();
        PlayerFavoriteVideos = new Dictionary<string, List<string>>();
        PlayerFollowedStreamers = new Dictionary<string, List<string>>();
        FeaturedArticleIDs = new List<string>();
        FeaturedVideoIDs = new List<string>();
        LastCleanupTime = DateTime.Now;
    }

    public void AddArticle(CampArticle article)
    {
        Articles.Add(article);
    }

    public void AddComment(ArticleComment comment)
    {
        Comments.Add(comment);
    }

    public void AddVideo(CampVideo video)
    {
        Videos.Add(video);
    }

    public void AddStreamer(CampStreamer streamer)
    {
        Streamers.Add(streamer);
    }

    public void AddPlayerActivity(string playerID, PlayerCampActivity activity)
    {
        PlayerActivities[playerID] = activity;
    }
}

[Serializable]
public class CampEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string RelatedID;
    public string EventData;

    public CampEvent(string eventID, string eventType, string playerID, string relatedID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        RelatedID = relatedID;
        EventData = eventData;
    }
}

public class CampSystemDataManager
{
    private static CampSystemDataManager _instance;
    public static CampSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CampSystemDataManager();
            }
            return _instance;
        }
    }

    public CampSystemData campData;
    private List<CampEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private CampSystemDataManager()
    {
        campData = new CampSystemData();
        recentEvents = new List<CampEvent>();
        LoadCampData();
    }

    public void SaveCampData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "CampSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, campData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存营地系统数据失败: " + e.Message);
        }
    }

    public void LoadCampData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "CampSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    campData = (CampSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载营地系统数据失败: " + e.Message);
            campData = new CampSystemData();
        }
    }

    public void CreateCampEvent(string eventType, string playerID, string relatedID, string eventData)
    {
        string eventID = "camp_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CampEvent campEvent = new CampEvent(eventID, eventType, playerID, relatedID, eventData);
        recentEvents.Add(campEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<CampEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
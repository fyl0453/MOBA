using System;
using System.Collections.Generic;

public class CampSystemDetailedManager
{
    private static CampSystemDetailedManager _instance;
    public static CampSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CampSystemDetailedManager();
            }
            return _instance;
        }
    }

    private CampSystemData campData;
    private CampSystemDataManager dataManager;

    private CampSystemDetailedManager()
    {
        dataManager = CampSystemDataManager.Instance;
        campData = dataManager.campData;
    }

    public void InitializePlayerActivity(string playerID)
    {
        if (!campData.PlayerActivities.ContainsKey(playerID))
        {
            campData.PlayerActivities[playerID] = new PlayerCampActivity(playerID);
            dataManager.CreateCampEvent("activity_init", playerID, "", "初始化营地活动");
            dataManager.SaveCampData();
            Debug.Log("初始化玩家营地活动: " + playerID);
        }
    }

    public void CreateArticle(string title, string content, string authorID, string authorName, int articleType, List<string> tags)
    {
        string articleID = "article_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CampArticle article = new CampArticle(articleID, title, content, authorID, authorName, articleType);
        article.Tags = tags;
        campData.AddArticle(article);

        if (campData.PlayerActivities.ContainsKey(authorID))
        {
            campData.PlayerActivities[authorID].TotalArticles++;
            campData.PlayerActivities[authorID].ActivityScore += 10;
            campData.PlayerActivities[authorID].LastActivityTime = DateTime.Now;
        }

        dataManager.CreateCampEvent("article_create", authorID, articleID, "发布文章: " + title);
        dataManager.SaveCampData();
        Debug.Log("发布文章成功: " + title);
    }

    public void UpdateArticle(string articleID, string title, string content, List<string> tags)
    {
        CampArticle article = campData.Articles.Find(a => a.ArticleID == articleID);
        if (article != null)
        {
            article.Title = title;
            article.Content = content;
            article.Tags = tags;
            article.LastUpdateTime = DateTime.Now;
            dataManager.CreateCampEvent("article_update", article.AuthorID, articleID, "更新文章: " + title);
            dataManager.SaveCampData();
            Debug.Log("更新文章成功");
        }
    }

    public void DeleteArticle(string articleID)
    {
        CampArticle article = campData.Articles.Find(a => a.ArticleID == articleID);
        if (article != null)
        {
            dataManager.CreateCampEvent("article_delete", article.AuthorID, articleID, "删除文章: " + article.Title);
            campData.Articles.Remove(article);
            campData.Comments.RemoveAll(c => c.ArticleID == articleID);
            dataManager.SaveCampData();
            Debug.Log("删除文章成功");
        }
    }

    public CampArticle GetArticle(string articleID)
    {
        CampArticle article = campData.Articles.Find(a => a.ArticleID == articleID);
        if (article != null)
        {
            article.ViewCount++;
            dataManager.SaveCampData();
        }
        return article;
    }

    public List<CampArticle> GetArticles(int count = 20)
    {
        List<CampArticle> articles = new List<CampArticle>(campData.Articles);
        articles.Sort((a, b) => b.CreateTime.CompareTo(a.CreateTime));
        if (count < articles.Count)
        {
            return articles.GetRange(0, count);
        }
        return articles;
    }

    public List<CampArticle> GetFeaturedArticles()
    {
        return campData.Articles.FindAll(a => a.IsFeatured);
    }

    public List<CampArticle> GetPinnedArticles()
    {
        return campData.Articles.FindAll(a => a.IsPinned);
    }

    public List<CampArticle> GetArticlesByType(int articleType)
    {
        return campData.Articles.FindAll(a => a.ArticleType == articleType);
    }

    public List<CampArticle> GetArticlesByAuthor(string authorID)
    {
        return campData.Articles.FindAll(a => a.AuthorID == authorID);
    }

    public List<CampArticle> SearchArticles(string keyword)
    {
        return campData.Articles.FindAll(a =>
            a.Title.Contains(keyword) ||
            a.Content.Contains(keyword) ||
            a.Tags.Exists(t => t.Contains(keyword)));
    }

    public void LikeArticle(string playerID, string articleID)
    {
        CampArticle article = campData.Articles.Find(a => a.ArticleID == articleID);
        if (article != null)
        {
            article.LikeCount++;
            if (campData.PlayerActivities.ContainsKey(playerID))
            {
                campData.PlayerActivities[playerID].TotalLikes++;
                campData.PlayerActivities[playerID].ActivityScore += 1;
            }
            dataManager.CreateCampEvent("article_like", playerID, articleID, "点赞文章: " + article.Title);
            dataManager.SaveCampData();
            Debug.Log("点赞文章成功");
        }
    }

    public void AddComment(string articleID, string playerID, string playerName, string content, string parentCommentID = "")
    {
        CampArticle article = campData.Articles.Find(a => a.ArticleID == articleID);
        if (article == null)
        {
            Debug.LogError("文章不存在");
            return;
        }

        string commentID = "comment_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ArticleComment comment = new ArticleComment(commentID, articleID, playerID, playerName, content, parentCommentID);
        campData.AddComment(comment);
        article.CommentCount++;

        if (campData.PlayerActivities.ContainsKey(playerID))
        {
            campData.PlayerActivities[playerID].TotalComments++;
            campData.PlayerActivities[playerID].ActivityScore += 5;
            campData.PlayerActivities[playerID].LastActivityTime = DateTime.Now;
        }

        dataManager.CreateCampEvent("comment_add", playerID, articleID, "添加评论");
        dataManager.SaveCampData();
        Debug.Log("添加评论成功");
    }

    public List<ArticleComment> GetArticleComments(string articleID)
    {
        return campData.Comments.FindAll(c => c.ArticleID == articleID);
    }

    public List<ArticleComment> GetRootComments(string articleID)
    {
        return campData.Comments.FindAll(c => c.ArticleID == articleID && string.IsNullOrEmpty(c.ParentCommentID));
    }

    public List<ArticleComment> GetReplies(string parentCommentID)
    {
        return campData.Comments.FindAll(c => c.ParentCommentID == parentCommentID);
    }

    public void LikeComment(string commentID)
    {
        ArticleComment comment = campData.Comments.Find(c => c.CommentID == commentID);
        if (comment != null)
        {
            comment.LikeCount++;
            dataManager.SaveCampData();
            Debug.Log("点赞评论成功");
        }
    }

    public void UploadVideo(string title, string description, string authorID, string authorName, string videoURL, string thumbnailURL, int category, List<string> tags)
    {
        string videoID = "video_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CampVideo video = new CampVideo(videoID, title, description, authorID, authorName, videoURL, thumbnailURL, category);
        video.Tags = tags;
        campData.AddVideo(video);

        if (campData.PlayerActivities.ContainsKey(authorID))
        {
            campData.PlayerActivities[authorID].TotalVideos++;
            campData.PlayerActivities[authorID].ActivityScore += 20;
        }

        dataManager.CreateCampEvent("video_upload", authorID, videoID, "上传视频: " + title);
        dataManager.SaveCampData();
        Debug.Log("上传视频成功: " + title);
    }

    public CampVideo GetVideo(string videoID)
    {
        CampVideo video = campData.Videos.Find(v => v.VideoID == videoID);
        if (video != null)
        {
            video.ViewCount++;
            dataManager.SaveCampData();
        }
        return video;
    }

    public List<CampVideo> GetVideos(int count = 20)
    {
        List<CampVideo> videos = new List<CampVideo>(campData.Videos);
        videos.Sort((a, b) => b.UploadTime.CompareTo(a.UploadTime));
        if (count < videos.Count)
        {
            return videos.GetRange(0, count);
        }
        return videos;
    }

    public List<CampVideo> GetFeaturedVideos()
    {
        return campData.Videos.FindAll(v => v.IsFeatured);
    }

    public List<CampVideo> GetVideosByCategory(int category)
    {
        return campData.Videos.FindAll(v => v.Category == category);
    }

    public List<CampVideo> GetVideosByAuthor(string authorID)
    {
        return campData.Videos.FindAll(v => v.AuthorID == authorID);
    }

    public List<CampVideo> SearchVideos(string keyword)
    {
        return campData.Videos.FindAll(v =>
            v.Title.Contains(keyword) ||
            v.Description.Contains(keyword) ||
            v.Tags.Exists(t => t.Contains(keyword)));
    }

    public void LikeVideo(string playerID, string videoID)
    {
        CampVideo video = campData.Videos.Find(v => v.VideoID == videoID);
        if (video != null)
        {
            video.LikeCount++;
            if (campData.PlayerActivities.ContainsKey(playerID))
            {
                campData.PlayerActivities[playerID].TotalLikes++;
                campData.PlayerActivities[playerID].ActivityScore += 1;
            }
            dataManager.CreateCampEvent("video_like", playerID, videoID, "点赞视频: " + video.Title);
            dataManager.SaveCampData();
            Debug.Log("点赞视频成功");
        }
    }

    public void FollowStreamer(string playerID, string streamerID)
    {
        CampStreamer streamer = campData.Streamers.Find(s => s.StreamerID == streamerID);
        if (streamer == null)
        {
            Debug.LogError("主播不存在");
            return;
        }

        if (!campData.PlayerFollowedStreamers.ContainsKey(playerID))
        {
            campData.PlayerFollowedStreamers[playerID] = new List<string>();
        }

        if (!campData.PlayerFollowedStreamers[playerID].Contains(streamerID))
        {
            campData.PlayerFollowedStreamers[playerID].Add(streamerID);
            streamer.FollowerCount++;
            dataManager.CreateCampEvent("streamer_follow", playerID, streamerID, "关注主播: " + streamer.StreamerName);
            dataManager.SaveCampData();
            Debug.Log("关注主播成功");
        }
    }

    public void UnfollowStreamer(string playerID, string streamerID)
    {
        CampStreamer streamer = campData.Streamers.Find(s => s.StreamerID == streamerID);
        if (streamer != null && campData.PlayerFollowedStreamers.ContainsKey(playerID))
        {
            if (campData.PlayerFollowedStreamers[playerID].Contains(streamerID))
            {
                campData.PlayerFollowedStreamers[playerID].Remove(streamerID);
                streamer.FollowerCount--;
                dataManager.SaveCampData();
                Debug.Log("取消关注主播成功");
            }
        }
    }

    public List<CampStreamer> GetFollowedStreamers(string playerID)
    {
        List<CampStreamer> followed = new List<CampStreamer>();
        if (campData.PlayerFollowedStreamers.ContainsKey(playerID))
        {
            foreach (string streamerID in campData.PlayerFollowedStreamers[playerID])
            {
                CampStreamer streamer = campData.Streamers.Find(s => s.StreamerID == streamerID);
                if (streamer != null)
                {
                    followed.Add(streamer);
                }
            }
        }
        return followed;
    }

    public CampStreamer GetStreamer(string streamerID)
    {
        return campData.Streamers.Find(s => s.StreamerID == streamerID);
    }

    public List<CampStreamer> GetLiveStreamers()
    {
        return campData.Streamers.FindAll(s => s.IsLive);
    }

    public List<CampStreamer> GetAllStreamers()
    {
        return campData.Streamers;
    }

    public void FavoriteArticle(string playerID, string articleID)
    {
        if (!campData.PlayerFavoriteArticles.ContainsKey(playerID))
        {
            campData.PlayerFavoriteArticles[playerID] = new List<string>();
        }
        if (!campData.PlayerFavoriteArticles[playerID].Contains(articleID))
        {
            campData.PlayerFavoriteArticles[playerID].Add(articleID);
            dataManager.SaveCampData();
            Debug.Log("收藏文章成功");
        }
    }

    public void UnfavoriteArticle(string playerID, string articleID)
    {
        if (campData.PlayerFavoriteArticles.ContainsKey(playerID))
        {
            campData.PlayerFavoriteArticles[playerID].Remove(articleID);
            dataManager.SaveCampData();
            Debug.Log("取消收藏文章成功");
        }
    }

    public List<CampArticle> GetFavoriteArticles(string playerID)
    {
        List<CampArticle> favorites = new List<CampArticle>();
        if (campData.PlayerFavoriteArticles.ContainsKey(playerID))
        {
            foreach (string articleID in campData.PlayerFavoriteArticles[playerID])
            {
                CampArticle article = campData.Articles.Find(a => a.ArticleID == articleID);
                if (article != null)
                {
                    favorites.Add(article);
                }
            }
        }
        return favorites;
    }

    public void FavoriteVideo(string playerID, string videoID)
    {
        if (!campData.PlayerFavoriteVideos.ContainsKey(playerID))
        {
            campData.PlayerFavoriteVideos[playerID] = new List<string>();
        }
        if (!campData.PlayerFavoriteVideos[playerID].Contains(videoID))
        {
            campData.PlayerFavoriteVideos[playerID].Add(videoID);
            dataManager.SaveCampData();
            Debug.Log("收藏视频成功");
        }
    }

    public void UnfavoriteVideo(string playerID, string videoID)
    {
        if (campData.PlayerFavoriteVideos.ContainsKey(playerID))
        {
            campData.PlayerFavoriteVideos[playerID].Remove(videoID);
            dataManager.SaveCampData();
            Debug.Log("取消收藏视频成功");
        }
    }

    public List<CampVideo> GetFavoriteVideos(string playerID)
    {
        List<CampVideo> favorites = new List<CampVideo>();
        if (campData.PlayerFavoriteVideos.ContainsKey(playerID))
        {
            foreach (string videoID in campData.PlayerFavoriteVideos[playerID])
            {
                CampVideo video = campData.Videos.Find(v => v.VideoID == videoID);
                if (video != null)
                {
                    favorites.Add(video);
                }
            }
        }
        return favorites;
    }

    public PlayerCampActivity GetPlayerActivity(string playerID)
    {
        if (campData.PlayerActivities.ContainsKey(playerID))
        {
            return campData.PlayerActivities[playerID];
        }
        return null;
    }

    public List<CampEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveCampData();
    }

    public void LoadData()
    {
        dataManager.LoadCampData();
    }
}
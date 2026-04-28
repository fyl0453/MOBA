[System.Serializable]
public class KingCampApp
{
    public string appID;
    public string appName;
    public string appDescription;
    public bool isEnabled;
    public List<CampNews> news;
    public List<CampUserProfile> userProfiles;
    public List<CampPost> posts;
    public List<CampComment> comments;
    public List<CampLike> likes;
    public List<CampFollower> followers;
    public List<CampNotification> notifications;
    
    public KingCampApp(string id, string name, string desc)
    {
        appID = id;
        appName = name;
        appDescription = desc;
        isEnabled = true;
        news = new List<CampNews>();
        userProfiles = new List<CampUserProfile>();
        posts = new List<CampPost>();
        comments = new List<CampComment>();
        likes = new List<CampLike>();
        followers = new List<CampFollower>();
        notifications = new List<CampNotification>();
    }
    
    public void AddNews(CampNews newsItem)
    {
        news.Add(newsItem);
    }
    
    public void AddUserProfile(CampUserProfile profile)
    {
        userProfiles.Add(profile);
    }
    
    public void AddPost(CampPost post)
    {
        posts.Add(post);
    }
    
    public void AddComment(CampComment comment)
    {
        comments.Add(comment);
    }
    
    public void AddLike(CampLike like)
    {
        likes.Add(like);
    }
    
    public void AddFollower(CampFollower follower)
    {
        followers.Add(follower);
    }
    
    public void AddNotification(CampNotification notification)
    {
        notifications.Add(notification);
    }
    
    public CampNews GetNews(string newsID)
    {
        return news.Find(n => n.newsID == newsID);
    }
    
    public CampUserProfile GetUserProfile(string userID)
    {
        return userProfiles.Find(p => p.userID == userID);
    }
    
    public CampPost GetPost(string postID)
    {
        return posts.Find(p => p.postID == postID);
    }
    
    public List<CampPost> GetPostsByUser(string userID)
    {
        return posts.FindAll(p => p.authorID == userID);
    }
    
    public List<CampComment> GetCommentsByPost(string postID)
    {
        return comments.FindAll(c => c.targetID == postID && c.targetType == "post");
    }
    
    public List<CampLike> GetLikesByPost(string postID)
    {
        return likes.FindAll(l => l.targetID == postID && l.targetType == "post");
    }
    
    public List<CampFollower> GetFollowersByUser(string userID)
    {
        return followers.FindAll(f => f.targetID == userID);
    }
    
    public List<CampFollower> GetFollowingByUser(string userID)
    {
        return followers.FindAll(f => f.followerID == userID);
    }
    
    public List<CampNotification> GetNotificationsByUser(string userID)
    {
        return notifications.FindAll(n => n.userID == userID);
    }
}

[System.Serializable]
public class CampNews
{
    public string newsID;
    public string title;
    public string content;
    public string category;
    public string coverImageURL;
    public string author;
    public string publishDate;
    public int viewCount;
    public int likeCount;
    public bool isTop;
    public bool isRecommended;
    
    public CampNews(string id, string title, string content, string category, string coverImageURL, string author)
    {
        newsID = id;
        this.title = title;
        this.content = content;
        this.category = category;
        this.coverImageURL = coverImageURL;
        this.author = author;
        publishDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        viewCount = 0;
        likeCount = 0;
        isTop = false;
        isRecommended = false;
    }
    
    public void IncrementViewCount()
    {
        viewCount++;
    }
    
    public void IncrementLikeCount()
    {
        likeCount++;
    }
    
    public void DecrementLikeCount()
    {
        if (likeCount > 0)
        {
            likeCount--;
        }
    }
}

[System.Serializable]
public class CampUserProfile
{
    public string userID;
    public string username;
    public string avatarURL;
    public string bio;
    public string level;
    public string rank;
    public int followersCount;
    public int followingCount;
    public int postsCount;
    public List<string> favoriteHeroes;
    public List<string> achievements;
    public string lastLoginDate;
    
    public CampUserProfile(string id, string username, string avatarURL)
    {
        userID = id;
        this.username = username;
        this.avatarURL = avatarURL;
        bio = "";
        level = "1";
        rank = "青铜";
        followersCount = 0;
        followingCount = 0;
        postsCount = 0;
        favoriteHeroes = new List<string>();
        achievements = new List<string>();
        lastLoginDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateProfile(string bio, string level, string rank)
    {
        this.bio = bio;
        this.level = level;
        this.rank = rank;
    }
    
    public void AddFavoriteHero(string heroID)
    {
        if (!favoriteHeroes.Contains(heroID))
        {
            favoriteHeroes.Add(heroID);
        }
    }
    
    public void AddAchievement(string achievementID)
    {
        if (!achievements.Contains(achievementID))
        {
            achievements.Add(achievementID);
        }
    }
    
    public void IncrementFollowersCount()
    {
        followersCount++;
    }
    
    public void DecrementFollowersCount()
    {
        if (followersCount > 0)
        {
            followersCount--;
        }
    }
    
    public void IncrementFollowingCount()
    {
        followingCount++;
    }
    
    public void DecrementFollowingCount()
    {
        if (followingCount > 0)
        {
            followingCount--;
        }
    }
    
    public void IncrementPostsCount()
    {
        postsCount++;
    }
}

[System.Serializable]
public class CampPost
{
    public string postID;
    public string authorID;
    public string content;
    public string type;
    public List<string> imageURLs;
    public string publishDate;
    public int likeCount;
    public int commentCount;
    public int shareCount;
    public bool isPublic;
    
    public CampPost(string id, string authorID, string content, string type)
    {
        postID = id;
        this.authorID = authorID;
        this.content = content;
        this.type = type;
        imageURLs = new List<string>();
        publishDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        likeCount = 0;
        commentCount = 0;
        shareCount = 0;
        isPublic = true;
    }
    
    public void AddImage(string imageURL)
    {
        imageURLs.Add(imageURL);
    }
    
    public void IncrementLikeCount()
    {
        likeCount++;
    }
    
    public void DecrementLikeCount()
    {
        if (likeCount > 0)
        {
            likeCount--;
        }
    }
    
    public void IncrementCommentCount()
    {
        commentCount++;
    }
    
    public void DecrementCommentCount()
    {
        if (commentCount > 0)
        {
            commentCount--;
        }
    }
    
    public void IncrementShareCount()
    {
        shareCount++;
    }
}

[System.Serializable]
public class CampComment
{
    public string commentID;
    public string authorID;
    public string targetID;
    public string targetType;
    public string content;
    public string commentDate;
    public int likeCount;
    
    public CampComment(string id, string authorID, string targetID, string targetType, string content)
    {
        commentID = id;
        this.authorID = authorID;
        this.targetID = targetID;
        this.targetType = targetType;
        this.content = content;
        commentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        likeCount = 0;
    }
    
    public void IncrementLikeCount()
    {
        likeCount++;
    }
    
    public void DecrementLikeCount()
    {
        if (likeCount > 0)
        {
            likeCount--;
        }
    }
}

[System.Serializable]
public class CampLike
{
    public string likeID;
    public string userID;
    public string targetID;
    public string targetType;
    public string likeDate;
    
    public CampLike(string id, string userID, string targetID, string targetType)
    {
        likeID = id;
        this.userID = userID;
        this.targetID = targetID;
        this.targetType = targetType;
        likeDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CampFollower
{
    public string followerID;
    public string targetID;
    public string followDate;
    
    public CampFollower(string followerID, string targetID)
    {
        this.followerID = followerID;
        this.targetID = targetID;
        followDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CampNotification
{
    public string notificationID;
    public string userID;
    public string type;
    public string content;
    public string sendDate;
    public bool isRead;
    
    public CampNotification(string id, string userID, string type, string content)
    {
        notificationID = id;
        this.userID = userID;
        this.type = type;
        this.content = content;
        sendDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        isRead = false;
    }
    
    public void MarkAsRead()
    {
        isRead = true;
    }
}

[System.Serializable]
public class KingCampAppManagerData
{
    public KingCampApp app;
    
    public KingCampAppManagerData()
    {
        app = new KingCampApp("king_camp_app", "王者营地", "官方社区应用");
    }
}
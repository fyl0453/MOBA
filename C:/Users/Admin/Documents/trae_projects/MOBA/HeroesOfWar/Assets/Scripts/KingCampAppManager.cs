using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class KingCampAppManager : MonoBehaviour
{
    public static KingCampAppManager Instance { get; private set; }
    
    public KingCampAppManagerData kingCampAppData;
    
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
        LoadKingCampAppData();
        
        if (kingCampAppData == null)
        {
            kingCampAppData = new KingCampAppManagerData();
            InitializeDefaultKingCampApp();
        }
    }
    
    private void InitializeDefaultKingCampApp()
    {
        // 添加默认新闻
        CampNews news1 = new CampNews("news_001", "新版本更新公告", "全新版本上线，新增英雄和皮肤！", "公告", "https://example.com/news1.jpg", "官方");
        news1.isTop = true;
        news1.isRecommended = true;
        kingCampAppData.app.AddNews(news1);
        
        CampNews news2 = new CampNews("news_002", "KPL春季赛即将开始", "KPL春季赛报名通道已开启，欢迎各位玩家参与！", "赛事", "https://example.com/news2.jpg", "官方");
        kingCampAppData.app.AddNews(news2);
        
        CampNews news3 = new CampNews("news_003", "新英雄上线", "全新英雄'暗影刺客'正式上线，快来体验！", "英雄", "https://example.com/news3.jpg", "官方");
        kingCampAppData.app.AddNews(news3);
        
        // 添加默认用户
        CampUserProfile user1 = new CampUserProfile("user_001", "玩家1", "https://example.com/avatar1.jpg");
        user1.UpdateProfile("热爱MOBA游戏的玩家", "30", "钻石");
        user1.AddFavoriteHero("hero_001");
        user1.AddFavoriteHero("hero_002");
        user1.AddAchievement("achievement_001");
        kingCampAppData.app.AddUserProfile(user1);
        
        CampUserProfile user2 = new CampUserProfile("user_002", "玩家2", "https://example.com/avatar2.jpg");
        user2.UpdateProfile("MOBA游戏爱好者", "25", "铂金");
        user2.AddFavoriteHero("hero_003");
        user2.AddAchievement("achievement_002");
        kingCampAppData.app.AddUserProfile(user2);
        
        // 添加默认帖子
        CampPost post1 = new CampPost("post_001", "user_001", "今天排位赛遇到了一个很厉害的队友，配合得非常默契！", "text");
        post1.AddImage("https://example.com/post1.jpg");
        kingCampAppData.app.AddPost(post1);
        user1.IncrementPostsCount();
        
        CampPost post2 = new CampPost("post_002", "user_002", "新英雄'暗影刺客'的技能连招分享，大家可以试试！", "guide");
        post2.AddImage("https://example.com/post2.jpg");
        post2.AddImage("https://example.com/post3.jpg");
        kingCampAppData.app.AddPost(post2);
        user2.IncrementPostsCount();
        
        // 添加默认评论
        CampComment comment1 = new CampComment("comment_001", "user_002", "post_001", "post", "确实，好的队友很重要！");
        kingCampAppData.app.AddComment(comment1);
        post1.IncrementCommentCount();
        
        CampComment comment2 = new CampComment("comment_002", "user_001", "post_002", "post", "感谢分享，我会试试的！");
        kingCampAppData.app.AddComment(comment2);
        post2.IncrementCommentCount();
        
        // 添加默认点赞
        CampLike like1 = new CampLike("like_001", "user_002", "post_001", "post");
        kingCampAppData.app.AddLike(like1);
        post1.IncrementLikeCount();
        
        CampLike like2 = new CampLike("like_002", "user_001", "post_002", "post");
        kingCampAppData.app.AddLike(like2);
        post2.IncrementLikeCount();
        
        // 添加默认关注
        CampFollower follower1 = new CampFollower("user_001", "user_002");
        kingCampAppData.app.AddFollower(follower1);
        user2.IncrementFollowersCount();
        user1.IncrementFollowingCount();
        
        // 添加默认通知
        CampNotification notification1 = new CampNotification("notification_001", "user_001", "system", "您的帖子获得了新的点赞！");
        kingCampAppData.app.AddNotification(notification1);
        
        CampNotification notification2 = new CampNotification("notification_002", "user_002", "system", "您收到了新的关注！");
        kingCampAppData.app.AddNotification(notification2);
        
        SaveKingCampAppData();
    }
    
    // 新闻相关方法
    public void AddNews(string title, string content, string category, string coverImageURL, string author)
    {
        string newsID = "news_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CampNews news = new CampNews(newsID, title, content, category, coverImageURL, author);
        kingCampAppData.app.AddNews(news);
        SaveKingCampAppData();
        Debug.Log("成功添加新闻: " + title);
    }
    
    public void UpdateNews(string newsID, string title, string content, string category)
    {
        CampNews news = kingCampAppData.app.GetNews(newsID);
        if (news != null)
        {
            news.title = title;
            news.content = content;
            news.category = category;
            SaveKingCampAppData();
            Debug.Log("成功更新新闻: " + title);
        }
        else
        {
            Debug.LogError("新闻不存在: " + newsID);
        }
    }
    
    public void DeleteNews(string newsID)
    {
        CampNews news = kingCampAppData.app.GetNews(newsID);
        if (news != null)
        {
            kingCampAppData.app.news.Remove(news);
            SaveKingCampAppData();
            Debug.Log("成功删除新闻: " + newsID);
        }
        else
        {
            Debug.LogError("新闻不存在: " + newsID);
        }
    }
    
    public List<CampNews> GetAllNews()
    {
        return kingCampAppData.app.news;
    }
    
    public List<CampNews> GetNewsByCategory(string category)
    {
        return kingCampAppData.app.news.FindAll(n => n.category == category);
    }
    
    // 用户相关方法
    public void AddUserProfile(string userID, string username, string avatarURL)
    {
        CampUserProfile profile = new CampUserProfile(userID, username, avatarURL);
        kingCampAppData.app.AddUserProfile(profile);
        SaveKingCampAppData();
        Debug.Log("成功添加用户: " + username);
    }
    
    public void UpdateUserProfile(string userID, string bio, string level, string rank)
    {
        CampUserProfile profile = kingCampAppData.app.GetUserProfile(userID);
        if (profile != null)
        {
            profile.UpdateProfile(bio, level, rank);
            SaveKingCampAppData();
            Debug.Log("成功更新用户资料: " + userID);
        }
        else
        {
            Debug.LogError("用户不存在: " + userID);
        }
    }
    
    public CampUserProfile GetUserProfile(string userID)
    {
        return kingCampAppData.app.GetUserProfile(userID);
    }
    
    // 帖子相关方法
    public void AddPost(string authorID, string content, string type, List<string> imageURLs)
    {
        string postID = "post_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CampPost post = new CampPost(postID, authorID, content, type);
        if (imageURLs != null)
        {
            foreach (string imageURL in imageURLs)
            {
                post.AddImage(imageURL);
            }
        }
        kingCampAppData.app.AddPost(post);
        
        CampUserProfile author = kingCampAppData.app.GetUserProfile(authorID);
        if (author != null)
        {
            author.IncrementPostsCount();
        }
        
        SaveKingCampAppData();
        Debug.Log("成功发布帖子: " + postID);
    }
    
    public void DeletePost(string postID)
    {
        CampPost post = kingCampAppData.app.GetPost(postID);
        if (post != null)
        {
            CampUserProfile author = kingCampAppData.app.GetUserProfile(post.authorID);
            if (author != null)
            {
                author.DecrementPostsCount();
            }
            
            kingCampAppData.app.posts.Remove(post);
            SaveKingCampAppData();
            Debug.Log("成功删除帖子: " + postID);
        }
        else
        {
            Debug.LogError("帖子不存在: " + postID);
        }
    }
    
    public List<CampPost> GetAllPosts()
    {
        return kingCampAppData.app.posts;
    }
    
    public List<CampPost> GetPostsByUser(string userID)
    {
        return kingCampAppData.app.GetPostsByUser(userID);
    }
    
    // 评论相关方法
    public void AddComment(string authorID, string targetID, string targetType, string content)
    {
        string commentID = "comment_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CampComment comment = new CampComment(commentID, authorID, targetID, targetType, content);
        kingCampAppData.app.AddComment(comment);
        
        if (targetType == "post")
        {
            CampPost post = kingCampAppData.app.GetPost(targetID);
            if (post != null)
            {
                post.IncrementCommentCount();
            }
        }
        
        SaveKingCampAppData();
        Debug.Log("成功添加评论: " + commentID);
    }
    
    public void DeleteComment(string commentID)
    {
        CampComment comment = kingCampAppData.app.comments.Find(c => c.commentID == commentID);
        if (comment != null)
        {
            if (comment.targetType == "post")
            {
                CampPost post = kingCampAppData.app.GetPost(comment.targetID);
                if (post != null)
                {
                    post.DecrementCommentCount();
                }
            }
            
            kingCampAppData.app.comments.Remove(comment);
            SaveKingCampAppData();
            Debug.Log("成功删除评论: " + commentID);
        }
        else
        {
            Debug.LogError("评论不存在: " + commentID);
        }
    }
    
    public List<CampComment> GetCommentsByPost(string postID)
    {
        return kingCampAppData.app.GetCommentsByPost(postID);
    }
    
    // 点赞相关方法
    public void AddLike(string userID, string targetID, string targetType)
    {
        // 检查是否已经点赞
        bool alreadyLiked = kingCampAppData.app.likes.Exists(l => l.userID == userID && l.targetID == targetID && l.targetType == targetType);
        if (!alreadyLiked)
        {
            string likeID = "like_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            CampLike like = new CampLike(likeID, userID, targetID, targetType);
            kingCampAppData.app.AddLike(like);
            
            if (targetType == "post")
            {
                CampPost post = kingCampAppData.app.GetPost(targetID);
                if (post != null)
                {
                    post.IncrementLikeCount();
                }
            }
            else if (targetType == "comment")
            {
                CampComment comment = kingCampAppData.app.comments.Find(c => c.commentID == targetID);
                if (comment != null)
                {
                    comment.IncrementLikeCount();
                }
            }
            else if (targetType == "news")
            {
                CampNews news = kingCampAppData.app.GetNews(targetID);
                if (news != null)
                {
                    news.IncrementLikeCount();
                }
            }
            
            SaveKingCampAppData();
            Debug.Log("成功点赞: " + targetID);
        }
        else
        {
            Debug.Log("已经点赞过: " + targetID);
        }
    }
    
    public void RemoveLike(string userID, string targetID, string targetType)
    {
        CampLike like = kingCampAppData.app.likes.Find(l => l.userID == userID && l.targetID == targetID && l.targetType == targetType);
        if (like != null)
        {
            kingCampAppData.app.likes.Remove(like);
            
            if (targetType == "post")
            {
                CampPost post = kingCampAppData.app.GetPost(targetID);
                if (post != null)
                {
                    post.DecrementLikeCount();
                }
            }
            else if (targetType == "comment")
            {
                CampComment comment = kingCampAppData.app.comments.Find(c => c.commentID == targetID);
                if (comment != null)
                {
                    comment.DecrementLikeCount();
                }
            }
            else if (targetType == "news")
            {
                CampNews news = kingCampAppData.app.GetNews(targetID);
                if (news != null)
                {
                    news.DecrementLikeCount();
                }
            }
            
            SaveKingCampAppData();
            Debug.Log("成功取消点赞: " + targetID);
        }
        else
        {
            Debug.LogError("点赞不存在: " + targetID);
        }
    }
    
    // 关注相关方法
    public void FollowUser(string followerID, string targetID)
    {
        // 检查是否已经关注
        bool alreadyFollowing = kingCampAppData.app.followers.Exists(f => f.followerID == followerID && f.targetID == targetID);
        if (!alreadyFollowing)
        {
            CampFollower follower = new CampFollower(followerID, targetID);
            kingCampAppData.app.AddFollower(follower);
            
            CampUserProfile targetUser = kingCampAppData.app.GetUserProfile(targetID);
            if (targetUser != null)
            {
                targetUser.IncrementFollowersCount();
            }
            
            CampUserProfile followerUser = kingCampAppData.app.GetUserProfile(followerID);
            if (followerUser != null)
            {
                followerUser.IncrementFollowingCount();
            }
            
            SaveKingCampAppData();
            Debug.Log("成功关注用户: " + targetID);
        }
        else
        {
            Debug.Log("已经关注过: " + targetID);
        }
    }
    
    public void UnfollowUser(string followerID, string targetID)
    {
        CampFollower follower = kingCampAppData.app.followers.Find(f => f.followerID == followerID && f.targetID == targetID);
        if (follower != null)
        {
            kingCampAppData.app.followers.Remove(follower);
            
            CampUserProfile targetUser = kingCampAppData.app.GetUserProfile(targetID);
            if (targetUser != null)
            {
                targetUser.DecrementFollowersCount();
            }
            
            CampUserProfile followerUser = kingCampAppData.app.GetUserProfile(followerID);
            if (followerUser != null)
            {
                followerUser.DecrementFollowingCount();
            }
            
            SaveKingCampAppData();
            Debug.Log("成功取消关注: " + targetID);
        }
        else
        {
            Debug.LogError("关注关系不存在: " + targetID);
        }
    }
    
    // 通知相关方法
    public void AddNotification(string userID, string type, string content)
    {
        string notificationID = "notification_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        CampNotification notification = new CampNotification(notificationID, userID, type, content);
        kingCampAppData.app.AddNotification(notification);
        SaveKingCampAppData();
        Debug.Log("成功添加通知: " + userID);
    }
    
    public void MarkNotificationAsRead(string notificationID)
    {
        CampNotification notification = kingCampAppData.app.notifications.Find(n => n.notificationID == notificationID);
        if (notification != null)
        {
            notification.MarkAsRead();
            SaveKingCampAppData();
            Debug.Log("成功标记通知为已读: " + notificationID);
        }
        else
        {
            Debug.LogError("通知不存在: " + notificationID);
        }
    }
    
    public List<CampNotification> GetNotificationsByUser(string userID)
    {
        return kingCampAppData.app.GetNotificationsByUser(userID);
    }
    
    // 数据持久化
    public void SaveKingCampAppData()
    {
        string path = Application.dataPath + "/Data/king_camp_app_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, kingCampAppData);
        stream.Close();
    }
    
    public void LoadKingCampAppData()
    {
        string path = Application.dataPath + "/Data/king_camp_app_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            kingCampAppData = (KingCampAppManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            kingCampAppData = new KingCampAppManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CommunitySystemDetailedManager : MonoBehaviour
{
    public static CommunitySystemDetailedManager Instance { get; private set; }
    
    public CommunitySystemDetailedManagerData communityData;
    
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
        LoadCommunityData();
        
        if (communityData == null)
        {
            communityData = new CommunitySystemDetailedManagerData();
            InitializeDefaultCommunitySystem();
        }
    }
    
    private void InitializeDefaultCommunitySystem()
    {
        // 分类
        Category category1 = new Category("category_001", "游戏资讯", "游戏最新资讯和更新", "icons/category_001");
        Category category2 = new Category("category_002", "英雄攻略", "英雄使用技巧和攻略", "icons/category_002");
        Category category3 = new Category("category_003", "玩家动态", "玩家分享的游戏动态", "icons/category_003");
        Category category4 = new Category("category_004", "赛事资讯", "赛事相关信息和报道", "icons/category_004");
        Category category5 = new Category("category_005", "皮肤展示", "皮肤展示和评测", "icons/category_005");
        
        communityData.system.AddCategory(category1);
        communityData.system.AddCategory(category2);
        communityData.system.AddCategory(category3);
        communityData.system.AddCategory(category4);
        communityData.system.AddCategory(category5);
        
        // 标签
        Tag tag1 = new Tag("tag_001", "王者荣耀", "王者荣耀相关内容");
        Tag tag2 = new Tag("tag_002", "李白", "李白英雄相关内容");
        Tag tag3 = new Tag("tag_003", "诸葛亮", "诸葛亮英雄相关内容");
        Tag tag4 = new Tag("tag_004", "KPL", "KPL职业联赛相关内容");
        Tag tag5 = new Tag("tag_005", "皮肤", "皮肤相关内容");
        
        communityData.system.AddTag(tag1);
        communityData.system.AddTag(tag2);
        communityData.system.AddTag(tag3);
        communityData.system.AddTag(tag4);
        communityData.system.AddTag(tag5);
        
        // 用户资料
        UserProfile profile1 = new UserProfile("profile_001", "user_001", "张三", "avatars/user_001", "热爱王者荣耀的玩家", "北京");
        profile1.AddInterest("王者荣耀");
        profile1.AddInterest("李白");
        profile1.AddInterest("KPL");
        
        UserProfile profile2 = new UserProfile("profile_002", "user_002", "李四", "avatars/user_002", "王者荣耀资深玩家", "上海");
        profile2.AddInterest("王者荣耀");
        profile2.AddInterest("诸葛亮");
        profile2.AddInterest("皮肤");
        
        UserProfile profile3 = new UserProfile("profile_003", "user_003", "王五", "avatars/user_003", "王者荣耀主播", "广州");
        profile3.AddInterest("王者荣耀");
        profile3.AddInterest("KPL");
        profile3.AddInterest("直播");
        
        communityData.system.AddUserProfile(profile1);
        communityData.system.AddUserProfile(profile2);
        communityData.system.AddUserProfile(profile3);
        
        // 帖子
        Post post1 = new Post("post_001", "user_001", "张三", "李白新皮肤上线，特效超酷！", "category_005", "skin", "media/skin_001.jpg", "media/skin_001_thumb.jpg");
        post1.AddTag("tag_001");
        post1.AddTag("tag_002");
        post1.AddTag("tag_005");
        
        Post post2 = new Post("post_002", "user_002", "李四", "诸葛亮玩法攻略，轻松上王者", "category_002", "guide");
        post2.AddTag("tag_001");
        post2.AddTag("tag_003");
        
        Post post3 = new Post("post_003", "user_003", "王五", "KPL春季赛前瞻，AG超玩会能否夺冠？", "category_004", "news");
        post3.AddTag("tag_001");
        post3.AddTag("tag_004");
        
        Post post4 = new Post("post_004", "user_001", "张三", "今天排位赛遇到了大神，躺赢了一把", "category_003", "share");
        post4.AddTag("tag_001");
        
        Post post5 = new Post("post_005", "user_002", "李四", "游戏更新公告：新英雄上线", "category_001", "news");
        post5.AddTag("tag_001");
        
        communityData.system.AddPost(post1);
        communityData.system.AddPost(post2);
        communityData.system.AddPost(post3);
        communityData.system.AddPost(post4);
        communityData.system.AddPost(post5);
        
        // 更新分类帖子数
        category1.IncrementPostCount();
        category2.IncrementPostCount();
        category3.IncrementPostCount();
        category4.IncrementPostCount();
        category5.IncrementPostCount();
        
        // 更新用户帖子数
        profile1.IncrementPostCount();
        profile1.IncrementPostCount();
        profile2.IncrementPostCount();
        profile2.IncrementPostCount();
        profile3.IncrementPostCount();
        
        // 评论
        Comment comment1 = new Comment("comment_001", "post_001", "user_002", "李四", "这个皮肤真的很帅！");
        Comment comment2 = new Comment("comment_002", "post_001", "user_003", "王五", "确实不错，值得入手");
        Comment comment3 = new Comment("comment_003", "post_002", "user_001", "张三", "学到了，谢谢分享");
        Comment comment4 = new Comment("comment_004", "post_003", "user_002", "李四", "AG超玩会加油！");
        Comment comment5 = new Comment("comment_005", "post_004", "user_003", "王五", "羡慕啊，我也想躺赢");
        
        communityData.system.AddComment(comment1);
        communityData.system.AddComment(comment2);
        communityData.system.AddComment(comment3);
        communityData.system.AddComment(comment4);
        communityData.system.AddComment(comment5);
        
        // 更新帖子评论数
        post1.IncrementCommentCount();
        post1.IncrementCommentCount();
        post2.IncrementCommentCount();
        post3.IncrementCommentCount();
        post4.IncrementCommentCount();
        
        // 点赞
        Like like1 = new Like("like_001", "post_001", "user_002");
        Like like2 = new Like("like_002", "post_001", "user_003");
        Like like3 = new Like("like_003", "post_002", "user_001");
        Like like4 = new Like("like_004", "post_002", "user_003");
        Like like5 = new Like("like_005", "post_003", "user_001");
        
        communityData.system.AddLike(like1);
        communityData.system.AddLike(like2);
        communityData.system.AddLike(like3);
        communityData.system.AddLike(like4);
        communityData.system.AddLike(like5);
        
        // 更新帖子点赞数
        post1.IncrementLikeCount();
        post1.IncrementLikeCount();
        post2.IncrementLikeCount();
        post2.IncrementLikeCount();
        post3.IncrementLikeCount();
        
        // 更新用户获赞数
        profile1.IncrementLikeCount();
        profile1.IncrementLikeCount();
        profile2.IncrementLikeCount();
        profile2.IncrementLikeCount();
        profile3.IncrementLikeCount();
        
        // 关注
        Follow follow1 = new Follow("follow_001", "user_001", "张三", "user_003", "王五");
        Follow follow2 = new Follow("follow_002", "user_002", "李四", "user_003", "王五");
        Follow follow3 = new Follow("follow_003", "user_003", "王五", "user_001", "张三");
        
        communityData.system.AddFollow(follow1);
        communityData.system.AddFollow(follow2);
        communityData.system.AddFollow(follow3);
        
        // 更新用户关注数和粉丝数
        profile1.IncrementFollowingCount();
        profile1.IncrementFollowerCount();
        profile2.IncrementFollowingCount();
        profile3.IncrementFollowerCount();
        profile3.IncrementFollowerCount();
        profile3.IncrementFollowingCount();
        
        // 通知
        Notification notification1 = new Notification("notification_001", "user_001", "like", "李四点赞了你的帖子", "post_001");
        Notification notification2 = new Notification("notification_002", "user_001", "comment", "王五评论了你的帖子", "post_001");
        Notification notification3 = new Notification("notification_003", "user_001", "follow", "王五关注了你", "user_003");
        Notification notification4 = new Notification("notification_004", "user_002", "like", "张三点赞了你的帖子", "post_002");
        Notification notification5 = new Notification("notification_005", "user_003", "follow", "张三关注了你", "user_001");
        
        communityData.system.AddNotification(notification1);
        communityData.system.AddNotification(notification2);
        communityData.system.AddNotification(notification3);
        communityData.system.AddNotification(notification4);
        communityData.system.AddNotification(notification5);
        
        SaveCommunityData();
    }
    
    // 帖子管理
    public void AddPost(string userID, string userName, string content, string categoryID, string postType, string mediaPath = "", string thumbnailPath = "")
    {
        string postID = "post_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Post post = new Post(postID, userID, userName, content, categoryID, postType, mediaPath, thumbnailPath);
        communityData.system.AddPost(post);
        
        // 更新分类帖子数
        Category category = communityData.system.GetCategory(categoryID);
        if (category != null)
        {
            category.IncrementPostCount();
        }
        
        // 更新用户帖子数
        UserProfile profile = communityData.system.GetUserProfile(userID);
        if (profile != null)
        {
            profile.IncrementPostCount();
            profile.UpdateLastActive();
        }
        
        SaveCommunityData();
        Debug.Log("成功发布帖子: " + content.Substring(0, Mathf.Min(20, content.Length)) + "...");
    }
    
    public void AddTagToPost(string postID, string tagID)
    {
        Post post = communityData.system.GetPost(postID);
        if (post != null)
        {
            post.AddTag(tagID);
            
            // 更新标签使用数
            Tag tag = communityData.system.GetTag(tagID);
            if (tag != null)
            {
                tag.IncrementUsageCount();
            }
            
            SaveCommunityData();
            Debug.Log("成功为帖子添加标签: " + tagID);
        }
        else
        {
            Debug.LogError("帖子不存在: " + postID);
        }
    }
    
    public void UpdatePostContent(string postID, string content)
    {
        Post post = communityData.system.GetPost(postID);
        if (post != null)
        {
            post.UpdateContent(content);
            SaveCommunityData();
            Debug.Log("成功更新帖子内容");
        }
        else
        {
            Debug.LogError("帖子不存在: " + postID);
        }
    }
    
    public void PinPost(string postID, bool pinned)
    {
        Post post = communityData.system.GetPost(postID);
        if (post != null)
        {
            post.SetPinned(pinned);
            SaveCommunityData();
            Debug.Log("成功" + (pinned ? "置顶" : "取消置顶") + "帖子");
        }
        else
        {
            Debug.LogError("帖子不存在: " + postID);
        }
    }
    
    public List<Post> GetPostsByCategory(string categoryID)
    {
        return communityData.system.GetPostsByCategory(categoryID);
    }
    
    public List<Post> GetPostsByUser(string userID)
    {
        return communityData.system.GetPostsByUser(userID);
    }
    
    public List<Post> GetPostsByTag(string tagID)
    {
        return communityData.system.GetPostsByTag(tagID);
    }
    
    // 评论管理
    public void AddComment(string postID, string userID, string userName, string content, string parentCommentID = "")
    {
        string commentID = "comment_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Comment comment = new Comment(commentID, postID, userID, userName, content, parentCommentID);
        communityData.system.AddComment(comment);
        
        // 更新帖子评论数
        Post post = communityData.system.GetPost(postID);
        if (post != null)
        {
            post.IncrementCommentCount();
        }
        
        // 更新用户最后活跃时间
        UserProfile profile = communityData.system.GetUserProfile(userID);
        if (profile != null)
        {
            profile.UpdateLastActive();
        }
        
        // 发送通知给帖子作者
        if (post != null && post.userID != userID)
        {
            string notificationID = "notification_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            Notification notification = new Notification(notificationID, post.userID, "comment", userName + "评论了你的帖子", postID);
            communityData.system.AddNotification(notification);
        }
        
        SaveCommunityData();
        Debug.Log("成功发表评论: " + content.Substring(0, Mathf.Min(20, content.Length)) + "...");
    }
    
    public void UpdateCommentContent(string commentID, string content)
    {
        Comment comment = communityData.system.GetComment(commentID);
        if (comment != null)
        {
            comment.UpdateContent(content);
            SaveCommunityData();
            Debug.Log("成功更新评论内容");
        }
        else
        {
            Debug.LogError("评论不存在: " + commentID);
        }
    }
    
    public List<Comment> GetCommentsByPost(string postID)
    {
        return communityData.system.GetCommentsByPost(postID);
    }
    
    // 点赞管理
    public void AddLike(string postID, string userID, string commentID = "")
    {
        string likeID = "like_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Like like = new Like(likeID, postID, userID, commentID);
        communityData.system.AddLike(like);
        
        if (!string.IsNullOrEmpty(postID))
        {
            // 更新帖子点赞数
            Post post = communityData.system.GetPost(postID);
            if (post != null)
            {
                post.IncrementLikeCount();
                
                // 更新用户获赞数
                UserProfile profile = communityData.system.GetUserProfile(post.userID);
                if (profile != null)
                {
                    profile.IncrementLikeCount();
                }
                
                // 发送通知给帖子作者
                if (post.userID != userID)
                {
                    string notificationID = "notification_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                    UserProfile likerProfile = communityData.system.GetUserProfile(userID);
                    string likerName = likerProfile != null ? likerProfile.userName : "用户";
                    Notification notification = new Notification(notificationID, post.userID, "like", likerName + "点赞了你的帖子", postID);
                    communityData.system.AddNotification(notification);
                }
            }
        }
        else if (!string.IsNullOrEmpty(commentID))
        {
            // 更新评论点赞数
            Comment comment = communityData.system.GetComment(commentID);
            if (comment != null)
            {
                comment.IncrementLikeCount();
                
                // 更新用户获赞数
                UserProfile profile = communityData.system.GetUserProfile(comment.userID);
                if (profile != null)
                {
                    profile.IncrementLikeCount();
                }
                
                // 发送通知给评论作者
                if (comment.userID != userID)
                {
                    string notificationID = "notification_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                    UserProfile likerProfile = communityData.system.GetUserProfile(userID);
                    string likerName = likerProfile != null ? likerProfile.userName : "用户";
                    Notification notification = new Notification(notificationID, comment.userID, "like", likerName + "点赞了你的评论", commentID);
                    communityData.system.AddNotification(notification);
                }
            }
        }
        
        // 更新用户最后活跃时间
        UserProfile userProfile = communityData.system.GetUserProfile(userID);
        if (userProfile != null)
        {
            userProfile.UpdateLastActive();
        }
        
        SaveCommunityData();
        Debug.Log("成功点赞");
    }
    
    public List<Like> GetLikesByPost(string postID)
    {
        return communityData.system.GetLikesByPost(postID);
    }
    
    public List<Like> GetLikesByUser(string userID)
    {
        return communityData.system.GetLikesByUser(userID);
    }
    
    // 关注管理
    public void AddFollow(string followerID, string followerName, string followingID, string followingName)
    {
        string followID = "follow_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Follow follow = new Follow(followID, followerID, followerName, followingID, followingName);
        communityData.system.AddFollow(follow);
        
        // 更新关注数和粉丝数
        UserProfile followerProfile = communityData.system.GetUserProfile(followerID);
        if (followerProfile != null)
        {
            followerProfile.IncrementFollowingCount();
            followerProfile.UpdateLastActive();
        }
        
        UserProfile followingProfile = communityData.system.GetUserProfile(followingID);
        if (followingProfile != null)
        {
            followingProfile.IncrementFollowerCount();
        }
        
        // 发送通知给被关注者
        string notificationID = "notification_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Notification notification = new Notification(notificationID, followingID, "follow", followerName + "关注了你", followerID);
        communityData.system.AddNotification(notification);
        
        SaveCommunityData();
        Debug.Log("成功关注用户: " + followingName);
    }
    
    public List<Follow> GetFollowsByFollower(string followerID)
    {
        return communityData.system.GetFollowsByFollower(followerID);
    }
    
    public List<Follow> GetFollowsByFollowing(string followingID)
    {
        return communityData.system.GetFollowsByFollowing(followingID);
    }
    
    // 用户资料管理
    public void AddUserProfile(string userID, string userName, string avatarPath = "", string bio = "", string region = "")
    {
        UserProfile existing = communityData.system.GetUserProfile(userID);
        if (existing == null)
        {
            string profileID = "profile_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            UserProfile profile = new UserProfile(profileID, userID, userName, avatarPath, bio, region);
            communityData.system.AddUserProfile(profile);
            SaveCommunityData();
            Debug.Log("成功创建用户资料: " + userName);
        }
    }
    
    public void UpdateUserProfile(string userID, string bio, string region)
    {
        UserProfile profile = communityData.system.GetUserProfile(userID);
        if (profile != null)
        {
            profile.UpdateProfile(bio, region);
            profile.UpdateLastActive();
            SaveCommunityData();
            Debug.Log("成功更新用户资料");
        }
        else
        {
            Debug.LogError("用户资料不存在: " + userID);
        }
    }
    
    public void AddInterestToUser(string userID, string interest)
    {
        UserProfile profile = communityData.system.GetUserProfile(userID);
        if (profile != null)
        {
            profile.AddInterest(interest);
            profile.UpdateLastActive();
            SaveCommunityData();
            Debug.Log("成功添加兴趣标签: " + interest);
        }
        else
        {
            Debug.LogError("用户资料不存在: " + userID);
        }
    }
    
    public UserProfile GetUserProfile(string userID)
    {
        return communityData.system.GetUserProfile(userID);
    }
    
    // 通知管理
    public void MarkNotificationAsRead(string notificationID)
    {
        Notification notification = communityData.system.GetNotification(notificationID);
        if (notification != null)
        {
            notification.MarkAsRead();
            SaveCommunityData();
            Debug.Log("成功标记通知为已读");
        }
        else
        {
            Debug.LogError("通知不存在: " + notificationID);
        }
    }
    
    public List<Notification> GetNotificationsByUser(string userID)
    {
        return communityData.system.GetNotificationsByUser(userID);
    }
    
    // 分类管理
    public void AddCategory(string name, string desc, string iconPath = "")
    {
        string categoryID = "category_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Category category = new Category(categoryID, name, desc, iconPath);
        communityData.system.AddCategory(category);
        SaveCommunityData();
        Debug.Log("成功添加分类: " + name);
    }
    
    public List<Category> GetAllCategories()
    {
        return communityData.system.categories;
    }
    
    // 标签管理
    public void AddTag(string name, string desc)
    {
        string tagID = "tag_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Tag tag = new Tag(tagID, name, desc);
        communityData.system.AddTag(tag);
        SaveCommunityData();
        Debug.Log("成功添加标签: " + name);
    }
    
    public List<Tag> GetAllTags()
    {
        return communityData.system.tags;
    }
    
    // 数据持久化
    public void SaveCommunityData()
    {
        string path = Application.dataPath + "/Data/community_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, communityData);
        stream.Close();
    }
    
    public void LoadCommunityData()
    {
        string path = Application.dataPath + "/Data/community_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            communityData = (CommunitySystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            communityData = new CommunitySystemDetailedManagerData();
        }
    }
}
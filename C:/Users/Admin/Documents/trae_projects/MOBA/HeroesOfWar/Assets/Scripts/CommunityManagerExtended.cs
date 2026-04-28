using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CommunityManagerExtended : MonoBehaviour
{
    public static CommunityManagerExtended Instance { get; private set; }
    
    public CommunityManagerData communityData;
    
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
            communityData = new CommunityManagerData();
            InitializeDefaultCommunity();
        }
    }
    
    private void InitializeDefaultCommunity()
    {
        // 创建默认分类
        CommunityCategory category1 = new CommunityCategory(
            "category1",
            "英雄攻略",
            "分享英雄使用技巧和出装推荐"
        );
        
        CommunityCategory category2 = new CommunityCategory(
            "category2",
            "游戏资讯",
            "最新游戏更新和活动信息"
        );
        
        CommunityCategory category3 = new CommunityCategory(
            "category3",
            "玩家动态",
            "分享游戏生活和精彩瞬间"
        );
        
        communityData.system.AddCategory(category1);
        communityData.system.AddCategory(category2);
        communityData.system.AddCategory(category3);
        
        // 创建默认标签
        CommunityTag tag1 = new CommunityTag(
            "tag1",
            "关羽",
            "关羽相关内容"
        );
        
        CommunityTag tag2 = new CommunityTag(
            "tag2",
            "诸葛亮",
            "诸葛亮相关内容"
        );
        
        CommunityTag tag3 = new CommunityTag(
            "tag3",
            "攻略",
            "游戏攻略相关内容"
        );
        
        communityData.system.AddTag(tag1);
        communityData.system.AddTag(tag2);
        communityData.system.AddTag(tag3);
        
        // 创建默认帖子
        CommunityPost post1 = new CommunityPost(
            "post1",
            "user1",
            "Player1",
            "avatar1.png",
            "关羽的最佳出装推荐：抵抗之靴+暗影战斧+破军+名刀司命+碎星锤+复活甲",
            "关羽出装攻略",
            "category1",
            "英雄攻略",
            "Guide"
        );
        post1.AddTag("tag1");
        post1.AddTag("tag3");
        post1.Feature();
        
        CommunityPost post2 = new CommunityPost(
            "post2",
            "user2",
            "Player2",
            "avatar2.png",
            "新版本更新：新英雄上线，新皮肤推出，快来体验！",
            "新版本更新公告",
            "category2",
            "游戏资讯",
            "News"
        );
        post2.Pin();
        
        communityData.system.AddPost(post1);
        communityData.system.AddPost(post2);
        
        // 更新分类帖子数
        category1.IncrementPostCount();
        category2.IncrementPostCount();
        
        // 更新标签帖子数
        tag1.IncrementPostCount();
        tag3.IncrementPostCount();
        
        SaveCommunityData();
    }
    
    public string CreatePost(string authorID, string authorName, string authorAvatar, string content, string title, string categoryID, string categoryName, string type, List<string> tagIDs = null, List<string> mediaURLs = null)
    {
        string postID = System.Guid.NewGuid().ToString();
        CommunityPost newPost = new CommunityPost(postID, authorID, authorName, authorAvatar, content, title, categoryID, categoryName, type);
        
        if (tagIDs != null)
        {
            foreach (string tagID in tagIDs)
            {
                newPost.AddTag(tagID);
                CommunityTag tag = communityData.system.GetTag(tagID);
                if (tag != null)
                {
                    tag.IncrementPostCount();
                }
            }
        }
        
        if (mediaURLs != null)
        {
            foreach (string url in mediaURLs)
            {
                newPost.AddMedia(url);
            }
        }
        
        communityData.system.AddPost(newPost);
        
        // 更新分类帖子数
        CommunityCategory category = communityData.system.GetCategory(categoryID);
        if (category != null)
        {
            category.IncrementPostCount();
        }
        
        SaveCommunityData();
        Debug.Log($"成功创建帖子: {title}");
        return postID;
    }
    
    public string CreateComment(string authorID, string authorName, string authorAvatar, string targetID, string targetType, string content, string parentCommentID = "")
    {
        string commentID = System.Guid.NewGuid().ToString();
        CommunityComment newComment = new CommunityComment(commentID, authorID, authorName, authorAvatar, targetID, targetType, content, parentCommentID);
        communityData.system.AddComment(newComment);
        
        // 更新帖子评论数
        if (targetType == "Post")
        {
            CommunityPost post = communityData.system.GetPost(targetID);
            if (post != null)
            {
                post.IncrementCommentCount();
            }
        }
        
        SaveCommunityData();
        Debug.Log("成功创建评论");
        return commentID;
    }
    
    public void LikePost(string userID, string postID)
    {
        // 检查是否已经点赞
        List<CommunityLike> likes = communityData.system.GetLikesByPost(postID);
        if (!likes.Exists(l => l.userID == userID))
        {
            string likeID = System.Guid.NewGuid().ToString();
            CommunityLike newLike = new CommunityLike(likeID, userID, postID, "Post");
            communityData.system.AddLike(newLike);
            
            // 更新帖子点赞数
            CommunityPost post = communityData.system.GetPost(postID);
            if (post != null)
            {
                post.IncrementLikeCount();
            }
            
            SaveCommunityData();
            Debug.Log("成功点赞帖子");
        }
    }
    
    public void UnlikePost(string userID, string postID)
    {
        List<CommunityLike> likes = communityData.system.GetLikesByPost(postID);
        CommunityLike like = likes.Find(l => l.userID == userID);
        if (like != null)
        {
            communityData.system.likes.Remove(like);
            
            // 更新帖子点赞数
            CommunityPost post = communityData.system.GetPost(postID);
            if (post != null)
            {
                post.DecrementLikeCount();
            }
            
            SaveCommunityData();
            Debug.Log("成功取消点赞帖子");
        }
    }
    
    public void LikeComment(string userID, string commentID)
    {
        // 检查是否已经点赞
        CommunityLike existingLike = communityData.system.likes.Find(l => l.userID == userID && l.targetID == commentID && l.targetType == "Comment");
        if (existingLike == null)
        {
            string likeID = System.Guid.NewGuid().ToString();
            CommunityLike newLike = new CommunityLike(likeID, userID, commentID, "Comment");
            communityData.system.AddLike(newLike);
            
            // 更新评论点赞数
            CommunityComment comment = communityData.system.comments.Find(c => c.commentID == commentID);
            if (comment != null)
            {
                comment.IncrementLikeCount();
            }
            
            SaveCommunityData();
            Debug.Log("成功点赞评论");
        }
    }
    
    public void UnlikeComment(string userID, string commentID)
    {
        CommunityLike like = communityData.system.likes.Find(l => l.userID == userID && l.targetID == commentID && l.targetType == "Comment");
        if (like != null)
        {
            communityData.system.likes.Remove(like);
            
            // 更新评论点赞数
            CommunityComment comment = communityData.system.comments.Find(c => c.commentID == commentID);
            if (comment != null)
            {
                comment.DecrementLikeCount();
            }
            
            SaveCommunityData();
            Debug.Log("成功取消点赞评论");
        }
    }
    
    public void FollowUser(string followerID, string followerName, string targetID, string targetName)
    {
        // 检查是否已经关注
        List<CommunityFollow> follows = communityData.system.GetFollowing(followerID);
        if (!follows.Exists(f => f.targetID == targetID))
        {
            string followID = System.Guid.NewGuid().ToString();
            CommunityFollow newFollow = new CommunityFollow(followID, followerID, followerName, targetID, targetName);
            communityData.system.AddFollow(newFollow);
            SaveCommunityData();
            Debug.Log($"成功关注用户: {targetName}");
        }
    }
    
    public void UnfollowUser(string followerID, string targetID)
    {
        CommunityFollow follow = communityData.system.follows.Find(f => f.followerID == followerID && f.targetID == targetID);
        if (follow != null)
        {
            communityData.system.follows.Remove(follow);
            SaveCommunityData();
            Debug.Log("成功取消关注用户");
        }
    }
    
    public void DeletePost(string postID)
    {
        CommunityPost post = communityData.system.GetPost(postID);
        if (post != null)
        {
            // 更新分类帖子数
            CommunityCategory category = communityData.system.GetCategory(post.categoryID);
            if (category != null)
            {
                category.DecrementPostCount();
            }
            
            // 更新标签帖子数
            foreach (string tagID in post.tagIDs)
            {
                CommunityTag tag = communityData.system.GetTag(tagID);
                if (tag != null)
                {
                    tag.DecrementPostCount();
                }
            }
            
            // 删除相关评论和点赞
            List<CommunityComment> postComments = communityData.system.GetCommentsByPost(postID);
            foreach (CommunityComment comment in postComments)
            {
                communityData.system.comments.Remove(comment);
            }
            
            List<CommunityLike> postLikes = communityData.system.GetLikesByPost(postID);
            foreach (CommunityLike like in postLikes)
            {
                communityData.system.likes.Remove(like);
            }
            
            communityData.system.posts.Remove(post);
            SaveCommunityData();
            Debug.Log($"成功删除帖子: {post.title}");
        }
    }
    
    public void DeleteComment(string commentID)
    {
        CommunityComment comment = communityData.system.comments.Find(c => c.commentID == commentID);
        if (comment != null)
        {
            // 更新帖子评论数
            if (comment.targetType == "Post")
            {
                CommunityPost post = communityData.system.GetPost(comment.targetID);
                if (post != null)
                {
                    post.DecrementCommentCount();
                }
            }
            
            // 删除相关点赞
            List<CommunityLike> commentLikes = communityData.system.likes.FindAll(l => l.targetID == commentID && l.targetType == "Comment");
            foreach (CommunityLike like in commentLikes)
            {
                communityData.system.likes.Remove(like);
            }
            
            communityData.system.comments.Remove(comment);
            SaveCommunityData();
            Debug.Log("成功删除评论");
        }
    }
    
    public void PinPost(string postID, bool pin)
    {
        CommunityPost post = communityData.system.GetPost(postID);
        if (post != null)
        {
            if (pin)
            {
                post.Pin();
            }
            else
            {
                post.Unpin();
            }
            SaveCommunityData();
            Debug.Log($"成功{(pin ? "置顶" : "取消置顶")}帖子");
        }
    }
    
    public void FeaturePost(string postID, bool feature)
    {
        CommunityPost post = communityData.system.GetPost(postID);
        if (post != null)
        {
            if (feature)
            {
                post.Feature();
            }
            else
            {
                post.Unfeature();
            }
            SaveCommunityData();
            Debug.Log($"成功{(feature ? "推荐" : "取消推荐")}帖子");
        }
    }
    
    public void UpdatePost(string postID, string content, string title, List<string> tagIDs = null)
    {
        CommunityPost post = communityData.system.GetPost(postID);
        if (post != null)
        {
            post.UpdateContent(content, title);
            
            if (tagIDs != null)
            {
                // 先减少旧标签的帖子数
                foreach (string oldTagID in post.tagIDs)
                {
                    CommunityTag oldTag = communityData.system.GetTag(oldTagID);
                    if (oldTag != null)
                    {
                        oldTag.DecrementPostCount();
                    }
                }
                
                // 清空旧标签
                post.tagIDs.Clear();
                
                // 添加新标签并增加帖子数
                foreach (string newTagID in tagIDs)
                {
                    post.AddTag(newTagID);
                    CommunityTag newTag = communityData.system.GetTag(newTagID);
                    if (newTag != null)
                    {
                        newTag.IncrementPostCount();
                    }
                }
            }
            
            SaveCommunityData();
            Debug.Log("成功更新帖子");
        }
    }
    
    public List<CommunityPost> GetLatestPosts(int limit = 20)
    {
        List<CommunityPost> posts = new List<CommunityPost>(communityData.system.posts);
        posts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return posts.GetRange(0, Mathf.Min(limit, posts.Count));
    }
    
    public List<CommunityPost> GetPostsByUser(string userID, int limit = 20)
    {
        List<CommunityPost> posts = communityData.system.GetPostsByUser(userID);
        posts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return posts.GetRange(0, Mathf.Min(limit, posts.Count));
    }
    
    public List<CommunityPost> GetPostsByCategory(string categoryID, int limit = 20)
    {
        List<CommunityPost> posts = communityData.system.GetPostsByCategory(categoryID);
        posts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return posts.GetRange(0, Mathf.Min(limit, posts.Count));
    }
    
    public List<CommunityPost> GetPostsByTag(string tagID, int limit = 20)
    {
        List<CommunityPost> posts = communityData.system.GetPostsByTag(tagID);
        posts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return posts.GetRange(0, Mathf.Min(limit, posts.Count));
    }
    
    public List<CommunityPost> GetFeaturedPosts(int limit = 10)
    {
        List<CommunityPost> featuredPosts = communityData.system.posts.FindAll(p => p.isFeatured);
        featuredPosts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return featuredPosts.GetRange(0, Mathf.Min(limit, featuredPosts.Count));
    }
    
    public List<CommunityPost> GetPinnedPosts(int limit = 5)
    {
        List<CommunityPost> pinnedPosts = communityData.system.posts.FindAll(p => p.isPinned);
        pinnedPosts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return pinnedPosts.GetRange(0, Mathf.Min(limit, pinnedPosts.Count));
    }
    
    public List<CommunityComment> GetCommentsByPost(string postID, int limit = 50)
    {
        List<CommunityComment> comments = communityData.system.GetCommentsByPost(postID);
        comments.Sort((a, b) => a.createdAt.CompareTo(b.createdAt));
        return comments.GetRange(0, Mathf.Min(limit, comments.Count));
    }
    
    public List<CommunityFollow> GetFollowers(string userID, int limit = 100)
    {
        List<CommunityFollow> followers = communityData.system.GetFollowers(userID);
        followers.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return followers.GetRange(0, Mathf.Min(limit, followers.Count));
    }
    
    public List<CommunityFollow> GetFollowing(string userID, int limit = 100)
    {
        List<CommunityFollow> following = communityData.system.GetFollowing(userID);
        following.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return following.GetRange(0, Mathf.Min(limit, following.Count));
    }
    
    public CommunityPost GetPost(string postID)
    {
        return communityData.system.GetPost(postID);
    }
    
    public CommunityCategory GetCategory(string categoryID)
    {
        return communityData.system.GetCategory(categoryID);
    }
    
    public CommunityTag GetTag(string tagID)
    {
        return communityData.system.GetTag(tagID);
    }
    
    public List<CommunityCategory> GetAllCategories()
    {
        return communityData.system.categories;
    }
    
    public List<CommunityTag> GetAllTags()
    {
        return communityData.system.tags;
    }
    
    public void SaveCommunityData()
    {
        string path = Application.dataPath + "/Data/community_system_extended_data.dat";
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
        string path = Application.dataPath + "/Data/community_system_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            communityData = (CommunityManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            communityData = new CommunityManagerData();
        }
    }
}
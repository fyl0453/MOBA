using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CommunityManagerDetailed : MonoBehaviour
{
    public static CommunityManagerDetailed Instance { get; private set; }
    
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
        // 创建默认用户
        CommunityUser user1 = new CommunityUser("user_1", "Player1", "avatar_1");
        user1.SetBio("王者荣耀爱好者，喜欢玩关羽");
        user1.IncrementPostCount();
        
        CommunityUser user2 = new CommunityUser("user_2", "Player2", "avatar_2");
        user2.SetBio("王者荣耀职业玩家，擅长法师");
        user2.IncrementPostCount();
        user2.Verify();
        
        communityData.system.AddUser(user1);
        communityData.system.AddUser(user2);
        
        // 创建默认帖子
        CommunityPost post1 = new CommunityPost("post_1", "user_1", "Player1", "关羽玩法攻略", "关羽是一个非常强大的战士英雄，需要掌握好冲锋状态...", "攻略", "Guide");
        post1.AddMedia("https://example.com/guanyu_1.jpg");
        post1.AddMedia("https://example.com/guanyu_2.jpg");
        post1.Feature();
        
        CommunityPost post2 = new CommunityPost("post_2", "user_2", "Player2", "诸葛亮连招技巧", "诸葛亮的核心在于技能的命中，需要利用被动技能来增加伤害...", "攻略", "Guide");
        post2.AddMedia("https://example.com/zhugeliang_1.jpg");
        
        communityData.system.AddPost(post1);
        communityData.system.AddPost(post2);
        
        // 创建默认评论
        CommunityComment comment1 = new CommunityComment("comment_1", "user_2", "Player2", "post_1", "Post", "非常详细的攻略，学习了");
        CommunityComment comment2 = new CommunityComment("comment_2", "user_1", "Player1", "post_2", "Post", "诸葛亮的连招确实很重要");
        
        communityData.system.AddComment(comment1);
        communityData.system.AddComment(comment2);
        
        // 更新帖子评论数
        post1.IncrementCommentCount();
        post2.IncrementCommentCount();
        
        // 创建默认点赞
        CommunityLike like1 = new CommunityLike("like_1", "user_2", "post_1", "Post");
        CommunityLike like2 = new CommunityLike("like_2", "user_1", "post_2", "Post");
        
        communityData.system.AddLike(like1);
        communityData.system.AddLike(like2);
        
        // 更新帖子点赞数
        post1.IncrementLikeCount();
        post2.IncrementLikeCount();
        
        SaveCommunityData();
    }
    
    public string CreatePost(string authorID, string authorName, string title, string content, string category, string type, List<string> mediaURLs = null)
    {
        string postID = System.Guid.NewGuid().ToString();
        CommunityPost newPost = new CommunityPost(postID, authorID, authorName, title, content, category, type);
        
        if (mediaURLs != null)
        {
            foreach (string url in mediaURLs)
            {
                newPost.AddMedia(url);
            }
        }
        
        communityData.system.AddPost(newPost);
        
        // 更新用户帖子数
        CommunityUser user = communityData.system.GetUser(authorID);
        if (user != null)
        {
            user.IncrementPostCount();
        }
        
        SaveCommunityData();
        Debug.Log($"成功创建帖子: {title}");
        return postID;
    }
    
    public string CreateComment(string authorID, string authorName, string targetID, string targetType, string content, string parentCommentID = "")
    {
        string commentID = System.Guid.NewGuid().ToString();
        CommunityComment newComment = new CommunityComment(commentID, authorID, authorName, targetID, targetType, content, parentCommentID);
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
            CommunityComment comment = communityData.system.GetComment(commentID);
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
            CommunityComment comment = communityData.system.GetComment(commentID);
            if (comment != null)
            {
                comment.DecrementLikeCount();
            }
            
            SaveCommunityData();
            Debug.Log("成功取消点赞评论");
        }
    }
    
    public void DeletePost(string postID)
    {
        CommunityPost post = communityData.system.GetPost(postID);
        if (post != null)
        {
            // 更新用户帖子数
            CommunityUser user = communityData.system.GetUser(post.authorID);
            if (user != null)
            {
                user.DecrementPostCount();
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
            Debug.Log($"成功删除帖子: {post.postTitle}");
        }
    }
    
    public void DeleteComment(string commentID)
    {
        CommunityComment comment = communityData.system.GetComment(commentID);
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
            Debug.Log($"成功{(pin ? "置顶" : "取消置顶")}帖子: {post.postTitle}");
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
            Debug.Log($"成功{(feature ? "推荐" : "取消推荐")}帖子: {post.postTitle}");
        }
    }
    
    public List<CommunityPost> GetPostsByCategory(string category, int limit = 20)
    {
        List<CommunityPost> posts = communityData.system.GetPostsByCategory(category);
        posts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return posts.GetRange(0, Mathf.Min(limit, posts.Count));
    }
    
    public List<CommunityPost> GetPostsByUser(string userID, int limit = 20)
    {
        List<CommunityPost> posts = communityData.system.GetPostsByUser(userID);
        posts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return posts.GetRange(0, Mathf.Min(limit, posts.Count));
    }
    
    public List<CommunityComment> GetCommentsByPost(string postID, int limit = 50)
    {
        List<CommunityComment> comments = communityData.system.GetCommentsByPost(postID);
        comments.Sort((a, b) => a.createdAt.CompareTo(b.createdAt));
        return comments.GetRange(0, Mathf.Min(limit, comments.Count));
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
    
    public CommunityPost GetPost(string postID)
    {
        CommunityPost post = communityData.system.GetPost(postID);
        if (post != null)
        {
            post.IncrementViewCount();
            SaveCommunityData();
        }
        return post;
    }
    
    public CommunityUser GetUser(string userID)
    {
        return communityData.system.GetUser(userID);
    }
    
    public void UpdateUserProfile(string userID, string bio, string avatarURL = "")
    {
        CommunityUser user = communityData.system.GetUser(userID);
        if (user != null)
        {
            if (!string.IsNullOrEmpty(bio))
            {
                user.SetBio(bio);
            }
            if (!string.IsNullOrEmpty(avatarURL))
            {
                user.avatarURL = avatarURL;
            }
            SaveCommunityData();
            Debug.Log("成功更新用户资料");
        }
    }
    
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
            communityData = (CommunityManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            communityData = new CommunityManagerData();
        }
    }
}
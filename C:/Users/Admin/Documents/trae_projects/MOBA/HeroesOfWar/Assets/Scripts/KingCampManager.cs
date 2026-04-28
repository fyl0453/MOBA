using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class KingCampManager : MonoBehaviour
{
    public static KingCampManager Instance { get; private set; }
    
    public KingCampManagerData kingCampData;
    
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
        LoadKingCampData();
        
        if (kingCampData == null)
        {
            kingCampData = new KingCampManagerData();
            InitializeDefaultKingCamp();
        }
    }
    
    private void InitializeDefaultKingCamp()
    {
        // 创建默认用户
        CampUser user1 = new CampUser("user_1", "Player1", "avatar_1");
        user1.SetBio("王者荣耀爱好者，喜欢玩关羽");
        user1.SetSignature("关羽是信仰");
        user1.IncrementPostCount();
        
        CampUser user2 = new CampUser("user_2", "Player2", "avatar_2");
        user2.SetBio("王者荣耀职业玩家，擅长法师");
        user2.SetSignature("法师是我的强项");
        user2.IncrementPostCount();
        user2.Verify();
        
        kingCampData.system.AddUser(user1);
        kingCampData.system.AddUser(user2);
        
        // 创建默认帖子
        CampPost post1 = new CampPost("post_1", "user_1", "Player1", "avatar_1", "今天玩关羽又拿了五杀，开心！", "Normal");
        post1.AddMedia("https://example.com/guanyu_1.jpg");
        post1.Feature();
        
        CampPost post2 = new CampPost("post_2", "user_2", "Player2", "avatar_2", "诸葛亮的连招技巧分享，希望对大家有帮助。", "Guide");
        post2.AddMedia("https://example.com/zhugeliang_1.jpg");
        
        kingCampData.system.AddPost(post1);
        kingCampData.system.AddPost(post2);
        
        // 创建默认评论
        CampComment comment1 = new CampComment("comment_1", "user_2", "Player2", "avatar_2", "post_1", "Post", "厉害！关羽确实很强");
        CampComment comment2 = new CampComment("comment_2", "user_1", "Player1", "avatar_1", "post_2", "Post", "感谢分享，学习了");
        
        kingCampData.system.AddComment(comment1);
        kingCampData.system.AddComment(comment2);
        
        // 更新帖子评论数
        post1.IncrementCommentCount();
        post2.IncrementCommentCount();
        
        // 创建默认点赞
        CampLike like1 = new CampLike("like_1", "user_2", "post_1", "Post");
        CampLike like2 = new CampLike("like_2", "user_1", "post_2", "Post");
        
        kingCampData.system.AddLike(like1);
        kingCampData.system.AddLike(like2);
        
        // 更新帖子点赞数
        post1.IncrementLikeCount();
        post2.IncrementLikeCount();
        
        SaveKingCampData();
    }
    
    public string CreatePost(string authorID, string authorName, string authorAvatar, string content, string type, List<string> mediaURLs = null)
    {
        string postID = System.Guid.NewGuid().ToString();
        CampPost newPost = new CampPost(postID, authorID, authorName, authorAvatar, content, type);
        
        if (mediaURLs != null)
        {
            foreach (string url in mediaURLs)
            {
                newPost.AddMedia(url);
            }
        }
        
        kingCampData.system.AddPost(newPost);
        
        // 更新用户帖子数
        CampUser user = kingCampData.system.GetUser(authorID);
        if (user != null)
        {
            user.IncrementPostCount();
        }
        
        SaveKingCampData();
        Debug.Log($"成功创建帖子: {content}");
        return postID;
    }
    
    public string CreateComment(string authorID, string authorName, string authorAvatar, string targetID, string targetType, string content, string parentCommentID = "")
    {
        string commentID = System.Guid.NewGuid().ToString();
        CampComment newComment = new CampComment(commentID, authorID, authorName, authorAvatar, targetID, targetType, content, parentCommentID);
        kingCampData.system.AddComment(newComment);
        
        // 更新帖子评论数
        if (targetType == "Post")
        {
            CampPost post = kingCampData.system.GetPost(targetID);
            if (post != null)
            {
                post.IncrementCommentCount();
            }
        }
        
        SaveKingCampData();
        Debug.Log("成功创建评论");
        return commentID;
    }
    
    public void LikePost(string userID, string postID)
    {
        // 检查是否已经点赞
        List<CampLike> likes = kingCampData.system.GetLikesByPost(postID);
        if (!likes.Exists(l => l.userID == userID))
        {
            string likeID = System.Guid.NewGuid().ToString();
            CampLike newLike = new CampLike(likeID, userID, postID, "Post");
            kingCampData.system.AddLike(newLike);
            
            // 更新帖子点赞数
            CampPost post = kingCampData.system.GetPost(postID);
            if (post != null)
            {
                post.IncrementLikeCount();
            }
            
            SaveKingCampData();
            Debug.Log("成功点赞帖子");
        }
    }
    
    public void UnlikePost(string userID, string postID)
    {
        List<CampLike> likes = kingCampData.system.GetLikesByPost(postID);
        CampLike like = likes.Find(l => l.userID == userID);
        if (like != null)
        {
            kingCampData.system.likes.Remove(like);
            
            // 更新帖子点赞数
            CampPost post = kingCampData.system.GetPost(postID);
            if (post != null)
            {
                post.DecrementLikeCount();
            }
            
            SaveKingCampData();
            Debug.Log("成功取消点赞帖子");
        }
    }
    
    public void LikeComment(string userID, string commentID)
    {
        // 检查是否已经点赞
        CampLike existingLike = kingCampData.system.likes.Find(l => l.userID == userID && l.targetID == commentID && l.targetType == "Comment");
        if (existingLike == null)
        {
            string likeID = System.Guid.NewGuid().ToString();
            CampLike newLike = new CampLike(likeID, userID, commentID, "Comment");
            kingCampData.system.AddLike(newLike);
            
            // 更新评论点赞数
            CampComment comment = kingCampData.system.comments.Find(c => c.commentID == commentID);
            if (comment != null)
            {
                comment.IncrementLikeCount();
            }
            
            SaveKingCampData();
            Debug.Log("成功点赞评论");
        }
    }
    
    public void UnlikeComment(string userID, string commentID)
    {
        CampLike like = kingCampData.system.likes.Find(l => l.userID == userID && l.targetID == commentID && l.targetType == "Comment");
        if (like != null)
        {
            kingCampData.system.likes.Remove(like);
            
            // 更新评论点赞数
            CampComment comment = kingCampData.system.comments.Find(c => c.commentID == commentID);
            if (comment != null)
            {
                comment.DecrementLikeCount();
            }
            
            SaveKingCampData();
            Debug.Log("成功取消点赞评论");
        }
    }
    
    public void DeletePost(string postID)
    {
        CampPost post = kingCampData.system.GetPost(postID);
        if (post != null)
        {
            // 更新用户帖子数
            CampUser user = kingCampData.system.GetUser(post.authorID);
            if (user != null)
            {
                user.DecrementPostCount();
            }
            
            // 删除相关评论和点赞
            List<CampComment> postComments = kingCampData.system.GetCommentsByPost(postID);
            foreach (CampComment comment in postComments)
            {
                kingCampData.system.comments.Remove(comment);
            }
            
            List<CampLike> postLikes = kingCampData.system.GetLikesByPost(postID);
            foreach (CampLike like in postLikes)
            {
                kingCampData.system.likes.Remove(like);
            }
            
            kingCampData.system.posts.Remove(post);
            SaveKingCampData();
            Debug.Log($"成功删除帖子");
        }
    }
    
    public void DeleteComment(string commentID)
    {
        CampComment comment = kingCampData.system.comments.Find(c => c.commentID == commentID);
        if (comment != null)
        {
            // 更新帖子评论数
            if (comment.targetType == "Post")
            {
                CampPost post = kingCampData.system.GetPost(comment.targetID);
                if (post != null)
                {
                    post.DecrementCommentCount();
                }
            }
            
            // 删除相关点赞
            List<CampLike> commentLikes = kingCampData.system.likes.FindAll(l => l.targetID == commentID && l.targetType == "Comment");
            foreach (CampLike like in commentLikes)
            {
                kingCampData.system.likes.Remove(like);
            }
            
            kingCampData.system.comments.Remove(comment);
            SaveKingCampData();
            Debug.Log("成功删除评论");
        }
    }
    
    public void PinPost(string postID, bool pin)
    {
        CampPost post = kingCampData.system.GetPost(postID);
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
            SaveKingCampData();
            Debug.Log($"成功{(pin ? "置顶" : "取消置顶")}帖子");
        }
    }
    
    public void FeaturePost(string postID, bool feature)
    {
        CampPost post = kingCampData.system.GetPost(postID);
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
            SaveKingCampData();
            Debug.Log($"成功{(feature ? "推荐" : "取消推荐")}帖子");
        }
    }
    
    public List<CampPost> GetLatestPosts(int limit = 20)
    {
        List<CampPost> posts = new List<CampPost>(kingCampData.system.posts);
        posts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return posts.GetRange(0, Mathf.Min(limit, posts.Count));
    }
    
    public List<CampPost> GetPostsByUser(string userID, int limit = 20)
    {
        List<CampPost> posts = kingCampData.system.GetPostsByUser(userID);
        posts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return posts.GetRange(0, Mathf.Min(limit, posts.Count));
    }
    
    public List<CampComment> GetCommentsByPost(string postID, int limit = 50)
    {
        List<CampComment> comments = kingCampData.system.GetCommentsByPost(postID);
        comments.Sort((a, b) => a.createdAt.CompareTo(b.createdAt));
        return comments.GetRange(0, Mathf.Min(limit, comments.Count));
    }
    
    public List<CampPost> GetFeaturedPosts(int limit = 10)
    {
        List<CampPost> featuredPosts = kingCampData.system.posts.FindAll(p => p.isFeatured);
        featuredPosts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return featuredPosts.GetRange(0, Mathf.Min(limit, featuredPosts.Count));
    }
    
    public List<CampPost> GetPinnedPosts(int limit = 5)
    {
        List<CampPost> pinnedPosts = kingCampData.system.posts.FindAll(p => p.isPinned);
        pinnedPosts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return pinnedPosts.GetRange(0, Mathf.Min(limit, pinnedPosts.Count));
    }
    
    public CampPost GetPost(string postID)
    {
        return kingCampData.system.GetPost(postID);
    }
    
    public CampUser GetUser(string userID)
    {
        return kingCampData.system.GetUser(userID);
    }
    
    public void UpdateUserProfile(string userID, string bio, string signature, string avatarURL = "")
    {
        CampUser user = kingCampData.system.GetUser(userID);
        if (user != null)
        {
            if (!string.IsNullOrEmpty(bio))
            {
                user.SetBio(bio);
            }
            if (!string.IsNullOrEmpty(signature))
            {
                user.SetSignature(signature);
            }
            if (!string.IsNullOrEmpty(avatarURL))
            {
                user.avatarURL = avatarURL;
            }
            SaveKingCampData();
            Debug.Log("成功更新用户资料");
        }
    }
    
    public void SaveKingCampData()
    {
        string path = Application.dataPath + "/Data/king_camp_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, kingCampData);
        stream.Close();
    }
    
    public void LoadKingCampData()
    {
        string path = Application.dataPath + "/Data/king_camp_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            kingCampData = (KingCampManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            kingCampData = new KingCampManagerData();
        }
    }
}
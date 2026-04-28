using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CommunityManager : MonoBehaviour
{
    public static CommunityManager Instance { get; private set; }
    
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
            InitializeDefaultData();
        }
    }
    
    private void InitializeDefaultData()
    {
        // 创建默认用户
        UserProfile user1 = new UserProfile("user_1", "Player1", "avatar_1", "游戏爱好者");
        UserProfile user2 = new UserProfile("user_2", "Player2", "avatar_2", "职业玩家");
        UserProfile user3 = new UserProfile("user_3", "Player3", "avatar_3", "休闲玩家");
        
        communityData.system.AddUserProfile(user1);
        communityData.system.AddUserProfile(user2);
        communityData.system.AddUserProfile(user3);
        
        // 创建默认帖子
        Post post1 = new Post("post_1", "user_1", "Player1", "游戏攻略：关羽玩法技巧", "关羽是一个非常强大的战士英雄，需要掌握好冲锋状态...", "攻略", "Guide");
        Post post2 = new Post("post_2", "user_2", "Player2", "春季赛冠军预测", "今年春季赛冠军会是哪支队伍呢？...", "赛事", "Event");
        Post post3 = new Post("post_3", "user_3", "Player3", "新英雄什么时候出？", "听说要出新英雄了，有人知道具体时间吗？...", "讨论", "Discussion");
        
        communityData.system.AddPost(post1);
        communityData.system.AddPost(post2);
        communityData.system.AddPost(post3);
        
        // 创建默认评论
        Comment comment1 = new Comment("comment_1", "post_1", "user_2", "Player2", "关羽确实需要很好的操作技巧");
        Comment comment2 = new Comment("comment_2", "post_1", "user_3", "Player3", "学习了，谢谢分享");
        Comment comment3 = new Comment("comment_3", "post_2", "user_1", "Player1", "我预测Team Alpha会赢");
        
        communityData.system.AddComment(comment1);
        communityData.system.AddComment(comment2);
        communityData.system.AddComment(comment3);
        
        // 创建默认点赞
        Like like1 = new Like("like_1", "user_2", "post_1", "Post");
        Like like2 = new Like("like_2", "user_3", "post_1", "Post");
        Like like3 = new Like("like_3", "user_1", "comment_1", "Comment");
        
        communityData.system.AddLike(like1);
        communityData.system.AddLike(like2);
        communityData.system.AddLike(like3);
        
        // 更新帖子统计
        post1.IncrementLikeCount();
        post1.IncrementLikeCount();
        post1.IncrementCommentCount();
        post1.IncrementCommentCount();
        
        post2.IncrementCommentCount();
        
        // 更新用户统计
        user1.IncrementPostCount();
        user2.IncrementPostCount();
        user3.IncrementPostCount();
        
        SaveCommunityData();
    }
    
    public string CreatePost(string authorID, string authorName, string title, string content, string category, string type)
    {
        string postID = System.Guid.NewGuid().ToString();
        Post post = new Post(postID, authorID, authorName, title, content, category, type);
        communityData.system.AddPost(post);
        
        // 更新用户统计
        UserProfile user = communityData.system.GetUserProfile(authorID);
        if (user != null)
        {
            user.IncrementPostCount();
        }
        
        SaveCommunityData();
        return postID;
    }
    
    public string CreateComment(string postID, string authorID, string authorName, string content)
    {
        Post post = communityData.system.GetPost(postID);
        if (post != null && !post.isLocked)
        {
            string commentID = System.Guid.NewGuid().ToString();
            Comment comment = new Comment(commentID, postID, authorID, authorName, content);
            communityData.system.AddComment(comment);
            post.IncrementCommentCount();
            SaveCommunityData();
            return commentID;
        }
        return "";
    }
    
    public void LikePost(string postID, string userID)
    {
        Post post = communityData.system.GetPost(postID);
        if (post != null)
        {
            // 检查是否已经点赞
            List<Like> likes = communityData.system.GetLikesByPost(postID);
            if (!likes.Exists(l => l.userID == userID))
            {
                string likeID = System.Guid.NewGuid().ToString();
                Like like = new Like(likeID, userID, postID, "Post");
                communityData.system.AddLike(like);
                post.IncrementLikeCount();
                SaveCommunityData();
            }
        }
    }
    
    public void UnlikePost(string postID, string userID)
    {
        Post post = communityData.system.GetPost(postID);
        if (post != null)
        {
            List<Like> likes = communityData.system.GetLikesByPost(postID);
            Like like = likes.Find(l => l.userID == userID);
            if (like != null)
            {
                communityData.system.likes.Remove(like);
                post.DecrementLikeCount();
                SaveCommunityData();
            }
        }
    }
    
    public void LikeComment(string commentID, string userID)
    {
        Comment comment = communityData.system.comments.Find(c => c.commentID == commentID);
        if (comment != null)
        {
            // 检查是否已经点赞
            List<Like> likes = communityData.system.GetLikesByComment(commentID);
            if (!likes.Exists(l => l.userID == userID))
            {
                string likeID = System.Guid.NewGuid().ToString();
                Like like = new Like(likeID, userID, commentID, "Comment");
                communityData.system.AddLike(like);
                comment.IncrementLikeCount();
                SaveCommunityData();
            }
        }
    }
    
    public void UnlikeComment(string commentID, string userID)
    {
        Comment comment = communityData.system.comments.Find(c => c.commentID == commentID);
        if (comment != null)
        {
            List<Like> likes = communityData.system.GetLikesByComment(commentID);
            Like like = likes.Find(l => l.userID == userID);
            if (like != null)
            {
                communityData.system.likes.Remove(like);
                comment.DecrementLikeCount();
                SaveCommunityData();
            }
        }
    }
    
    public void FollowUser(string userID, string followerID)
    {
        UserProfile user = communityData.system.GetUserProfile(userID);
        UserProfile follower = communityData.system.GetUserProfile(followerID);
        if (user != null && follower != null)
        {
            user.AddFollower(followerID);
            follower.AddFollowing(userID);
            SaveCommunityData();
        }
    }
    
    public void UnfollowUser(string userID, string followerID)
    {
        UserProfile user = communityData.system.GetUserProfile(userID);
        UserProfile follower = communityData.system.GetUserProfile(followerID);
        if (user != null && follower != null)
        {
            user.RemoveFollower(followerID);
            follower.RemoveFollowing(userID);
            SaveCommunityData();
        }
    }
    
    public void ViewPost(string postID)
    {
        Post post = communityData.system.GetPost(postID);
        if (post != null)
        {
            post.IncrementViewCount();
            SaveCommunityData();
        }
    }
    
    public List<Post> GetPosts(int limit = 20, string category = "")
    {
        List<Post> posts = category == "" ? communityData.system.posts : communityData.system.GetPostsByCategory(category);
        posts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return posts.GetRange(0, Mathf.Min(limit, posts.Count));
    }
    
    public List<Post> GetPostsByUser(string userID, int limit = 20)
    {
        List<Post> posts = communityData.system.GetPostsByUser(userID);
        posts.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return posts.GetRange(0, Mathf.Min(limit, posts.Count));
    }
    
    public List<Comment> GetCommentsByPost(string postID, int limit = 50)
    {
        List<Comment> comments = communityData.system.GetCommentsByPost(postID);
        comments.Sort((a, b) => a.createdAt.CompareTo(b.createdAt));
        return comments.GetRange(0, Mathf.Min(limit, comments.Count));
    }
    
    public UserProfile GetUserProfile(string userID)
    {
        return communityData.system.GetUserProfile(userID);
    }
    
    public void UpdateUserProfile(string userID, string bio, string avatar)
    {
        UserProfile user = communityData.system.GetUserProfile(userID);
        if (user != null)
        {
            user.bio = bio;
            if (!string.IsNullOrEmpty(avatar))
            {
                user.avatar = avatar;
            }
            SaveCommunityData();
        }
    }
    
    public void DeletePost(string postID)
    {
        Post post = communityData.system.GetPost(postID);
        if (post != null)
        {
            // 更新用户统计
            UserProfile user = communityData.system.GetUserProfile(post.authorID);
            if (user != null)
            {
                user.DecrementPostCount();
            }
            
            // 删除相关评论和点赞
            List<Comment> comments = communityData.system.GetCommentsByPost(postID);
            foreach (Comment comment in comments)
            {
                List<Like> commentLikes = communityData.system.GetLikesByComment(comment.commentID);
                foreach (Like like in commentLikes)
                {
                    communityData.system.likes.Remove(like);
                }
                communityData.system.comments.Remove(comment);
            }
            
            List<Like> postLikes = communityData.system.GetLikesByPost(postID);
            foreach (Like like in postLikes)
            {
                communityData.system.likes.Remove(like);
            }
            
            communityData.system.posts.Remove(post);
            SaveCommunityData();
        }
    }
    
    public void DeleteComment(string commentID)
    {
        Comment comment = communityData.system.comments.Find(c => c.commentID == commentID);
        if (comment != null)
        {
            // 更新帖子评论数
            Post post = communityData.system.GetPost(comment.postID);
            if (post != null)
            {
                post.DecrementCommentCount();
            }
            
            // 删除相关点赞
            List<Like> commentLikes = communityData.system.GetLikesByComment(commentID);
            foreach (Like like in commentLikes)
            {
                communityData.system.likes.Remove(like);
            }
            
            communityData.system.comments.Remove(comment);
            SaveCommunityData();
        }
    }
    
    public void SaveCommunityData()
    {
        string path = Application.dataPath + "/Data/community_data.dat";
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
        string path = Application.dataPath + "/Data/community_data.dat";
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
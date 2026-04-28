[System.Serializable]
public class CommunitySystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Post> posts;
    public List<Comment> comments;
    public List<UserProfile> userProfiles;
    public List<Like> likes;
    
    public CommunitySystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        posts = new List<Post>();
        comments = new List<Comment>();
        userProfiles = new List<UserProfile>();
        likes = new List<Like>();
    }
    
    public void AddPost(Post post)
    {
        posts.Add(post);
    }
    
    public void AddComment(Comment comment)
    {
        comments.Add(comment);
    }
    
    public void AddUserProfile(UserProfile profile)
    {
        userProfiles.Add(profile);
    }
    
    public void AddLike(Like like)
    {
        likes.Add(like);
    }
    
    public Post GetPost(string postID)
    {
        return posts.Find(p => p.postID == postID);
    }
    
    public List<Comment> GetCommentsByPost(string postID)
    {
        return comments.FindAll(c => c.postID == postID);
    }
    
    public UserProfile GetUserProfile(string userID)
    {
        return userProfiles.Find(up => up.userID == userID);
    }
    
    public List<Like> GetLikesByPost(string postID)
    {
        return likes.FindAll(l => l.targetID == postID && l.targetType == "Post");
    }
    
    public List<Like> GetLikesByComment(string commentID)
    {
        return likes.FindAll(l => l.targetID == commentID && l.targetType == "Comment");
    }
    
    public List<Post> GetPostsByUser(string userID)
    {
        return posts.FindAll(p => p.authorID == userID);
    }
    
    public List<Post> GetPostsByCategory(string category)
    {
        return posts.FindAll(p => p.category == category);
    }
}

[System.Serializable]
public class Post
{
    public string postID;
    public string authorID;
    public string authorName;
    public string title;
    public string content;
    public string category;
    public string postType;
    public string createdAt;
    public int likeCount;
    public int commentCount;
    public int viewCount;
    public bool isPinned;
    public bool isLocked;
    
    public Post(string id, string author, string authorName, string title, string content, string category, string type)
    {
        postID = id;
        this.authorID = author;
        this.authorName = authorName;
        this.title = title;
        this.content = content;
        this.category = category;
        postType = type;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        likeCount = 0;
        commentCount = 0;
        viewCount = 0;
        isPinned = false;
        isLocked = false;
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
    
    public void PinPost()
    {
        isPinned = true;
    }
    
    public void UnpinPost()
    {
        isPinned = false;
    }
    
    public void LockPost()
    {
        isLocked = true;
    }
    
    public void UnlockPost()
    {
        isLocked = false;
    }
}

[System.Serializable]
public class Comment
{
    public string commentID;
    public string postID;
    public string authorID;
    public string authorName;
    public string content;
    public string createdAt;
    public int likeCount;
    
    public Comment(string id, string post, string author, string authorName, string content)
    {
        commentID = id;
        postID = post;
        authorID = author;
        this.authorName = authorName;
        this.content = content;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
public class UserProfile
{
    public string userID;
    public string userName;
    public string avatar;
    public string bio;
    public int postCount;
    public int followerCount;
    public int followingCount;
    public List<string> followers;
    public List<string> following;
    
    public UserProfile(string id, string name, string avatar, string bio)
    {
        userID = id;
        userName = name;
        this.avatar = avatar;
        this.bio = bio;
        postCount = 0;
        followerCount = 0;
        followingCount = 0;
        followers = new List<string>();
        following = new List<string>();
    }
    
    public void IncrementPostCount()
    {
        postCount++;
    }
    
    public void DecrementPostCount()
    {
        if (postCount > 0)
        {
            postCount--;
        }
    }
    
    public void AddFollower(string followerID)
    {
        if (!followers.Contains(followerID))
        {
            followers.Add(followerID);
            followerCount++;
        }
    }
    
    public void RemoveFollower(string followerID)
    {
        if (followers.Contains(followerID))
        {
            followers.Remove(followerID);
            followerCount--;
        }
    }
    
    public void AddFollowing(string followingID)
    {
        if (!following.Contains(followingID))
        {
            following.Add(followingID);
            followingCount++;
        }
    }
    
    public void RemoveFollowing(string followingID)
    {
        if (following.Contains(followingID))
        {
            following.Remove(followingID);
            followingCount--;
        }
    }
}

[System.Serializable]
public class Like
{
    public string likeID;
    public string userID;
    public string targetID;
    public string targetType;
    public string createdAt;
    
    public Like(string id, string user, string target, string type)
    {
        likeID = id;
        userID = user;
        targetID = target;
        targetType = type;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CommunityManagerData
{
    public CommunitySystem system;
    
    public CommunityManagerData()
    {
        system = new CommunitySystem("community", "社区系统", "游戏内社区");
    }
}
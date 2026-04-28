[System.Serializable]
public class KingCampSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<CampPost> posts;
    public List<CampUser> users;
    public List<CampComment> comments;
    public List<CampLike> likes;
    
    public KingCampSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        posts = new List<CampPost>();
        users = new List<CampUser>();
        comments = new List<CampComment>();
        likes = new List<CampLike>();
    }
    
    public void AddPost(CampPost post)
    {
        posts.Add(post);
    }
    
    public void AddUser(CampUser user)
    {
        users.Add(user);
    }
    
    public void AddComment(CampComment comment)
    {
        comments.Add(comment);
    }
    
    public void AddLike(CampLike like)
    {
        likes.Add(like);
    }
    
    public CampPost GetPost(string postID)
    {
        return posts.Find(p => p.postID == postID);
    }
    
    public CampUser GetUser(string userID)
    {
        return users.Find(u => u.userID == userID);
    }
    
    public List<CampPost> GetPostsByUser(string userID)
    {
        return posts.FindAll(p => p.authorID == userID);
    }
    
    public List<CampComment> GetCommentsByPost(string postID)
    {
        return comments.FindAll(c => c.targetID == postID);
    }
    
    public List<CampLike> GetLikesByPost(string postID)
    {
        return likes.FindAll(l => l.targetID == postID);
    }
}

[System.Serializable]
public class CampUser
{
    public string userID;
    public string userName;
    public string avatarURL;
    public string bio;
    public int followers;
    public int following;
    public int postCount;
    public bool isVerified;
    public string level;
    public string signature;
    
    public CampUser(string id, string name, string avatar = "")
    {
        userID = id;
        userName = name;
        avatarURL = avatar;
        bio = "";
        followers = 0;
        following = 0;
        postCount = 0;
        isVerified = false;
        level = "1";
        signature = "";
    }
    
    public void SetBio(string bio)
    {
        this.bio = bio;
    }
    
    public void SetSignature(string signature)
    {
        this.signature = signature;
    }
    
    public void IncrementFollowers()
    {
        followers++;
    }
    
    public void DecrementFollowers()
    {
        if (followers > 0)
        {
            followers--;
        }
    }
    
    public void IncrementFollowing()
    {
        following++;
    }
    
    public void DecrementFollowing()
    {
        if (following > 0)
        {
            following--;
        }
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
    
    public void Verify()
    {
        isVerified = true;
    }
    
    public void SetLevel(string level)
    {
        this.level = level;
    }
}

[System.Serializable]
public class CampPost
{
    public string postID;
    public string authorID;
    public string authorName;
    public string authorAvatar;
    public string content;
    public string postType;
    public List<string> mediaURLs;
    public string createdAt;
    public int likeCount;
    public int commentCount;
    public int shareCount;
    public bool isPinned;
    public bool isFeatured;
    
    public CampPost(string id, string author, string authorName, string authorAvatar, string content, string type)
    {
        postID = id;
        authorID = author;
        this.authorName = authorName;
        this.authorAvatar = authorAvatar;
        this.content = content;
        postType = type;
        mediaURLs = new List<string>();
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        likeCount = 0;
        commentCount = 0;
        shareCount = 0;
        isPinned = false;
        isFeatured = false;
    }
    
    public void AddMedia(string url)
    {
        mediaURLs.Add(url);
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
    
    public void Pin()
    {
        isPinned = true;
    }
    
    public void Unpin()
    {
        isPinned = false;
    }
    
    public void Feature()
    {
        isFeatured = true;
    }
    
    public void Unfeature()
    {
        isFeatured = false;
    }
}

[System.Serializable]
public class CampComment
{
    public string commentID;
    public string authorID;
    public string authorName;
    public string authorAvatar;
    public string targetID;
    public string targetType;
    public string content;
    public string createdAt;
    public int likeCount;
    public string parentCommentID;
    
    public CampComment(string id, string author, string authorName, string authorAvatar, string target, string targetType, string content, string parent = "")
    {
        commentID = id;
        authorID = author;
        this.authorName = authorName;
        this.authorAvatar = authorAvatar;
        this.targetID = target;
        this.targetType = targetType;
        this.content = content;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        likeCount = 0;
        parentCommentID = parent;
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
    public string createdAt;
    
    public CampLike(string id, string user, string target, string type)
    {
        likeID = id;
        userID = user;
        targetID = target;
        targetType = type;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class KingCampManagerData
{
    public KingCampSystem system;
    
    public KingCampManagerData()
    {
        system = new KingCampSystem("king_camp_system", "王者营地系统", "管理官方社区应用");
    }
}
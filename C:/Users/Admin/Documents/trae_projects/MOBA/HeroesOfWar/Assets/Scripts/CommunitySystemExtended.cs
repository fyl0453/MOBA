[System.Serializable]
public class CommunitySystemExtended
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<CommunityPost> posts;
    public List<CommunityComment> comments;
    public List<CommunityLike> likes;
    public List<CommunityFollow> follows;
    public List<CommunityTag> tags;
    public List<CommunityCategory> categories;
    
    public CommunitySystemExtended(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        posts = new List<CommunityPost>();
        comments = new List<CommunityComment>();
        likes = new List<CommunityLike>();
        follows = new List<CommunityFollow>();
        tags = new List<CommunityTag>();
        categories = new List<CommunityCategory>();
    }
    
    public void AddPost(CommunityPost post)
    {
        posts.Add(post);
    }
    
    public void AddComment(CommunityComment comment)
    {
        comments.Add(comment);
    }
    
    public void AddLike(CommunityLike like)
    {
        likes.Add(like);
    }
    
    public void AddFollow(CommunityFollow follow)
    {
        follows.Add(follow);
    }
    
    public void AddTag(CommunityTag tag)
    {
        tags.Add(tag);
    }
    
    public void AddCategory(CommunityCategory category)
    {
        categories.Add(category);
    }
    
    public CommunityPost GetPost(string postID)
    {
        return posts.Find(p => p.postID == postID);
    }
    
    public CommunityComment GetComment(string commentID)
    {
        return comments.Find(c => c.commentID == commentID);
    }
    
    public CommunityTag GetTag(string tagID)
    {
        return tags.Find(t => t.tagID == tagID);
    }
    
    public CommunityCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public List<CommunityPost> GetPostsByUser(string userID)
    {
        return posts.FindAll(p => p.authorID == userID);
    }
    
    public List<CommunityPost> GetPostsByCategory(string categoryID)
    {
        return posts.FindAll(p => p.categoryID == categoryID);
    }
    
    public List<CommunityPost> GetPostsByTag(string tagID)
    {
        return posts.FindAll(p => p.tagIDs.Contains(tagID));
    }
    
    public List<CommunityComment> GetCommentsByPost(string postID)
    {
        return comments.FindAll(c => c.targetID == postID);
    }
    
    public List<CommunityLike> GetLikesByPost(string postID)
    {
        return likes.FindAll(l => l.targetID == postID);
    }
    
    public List<CommunityFollow> GetFollowers(string userID)
    {
        return follows.FindAll(f => f.targetID == userID);
    }
    
    public List<CommunityFollow> GetFollowing(string userID)
    {
        return follows.FindAll(f => f.followerID == userID);
    }
}

[System.Serializable]
public class CommunityPost
{
    public string postID;
    public string authorID;
    public string authorName;
    public string authorAvatar;
    public string content;
    public string title;
    public string categoryID;
    public string categoryName;
    public List<string> tagIDs;
    public List<string> mediaURLs;
    public string postType;
    public int likeCount;
    public int commentCount;
    public int shareCount;
    public int viewCount;
    public bool isPinned;
    public bool isFeatured;
    public string createdAt;
    public string updatedAt;
    
    public CommunityPost(string id, string author, string authorName, string authorAvatar, string content, string title, string categoryID, string categoryName, string type)
    {
        postID = id;
        authorID = author;
        this.authorName = authorName;
        this.authorAvatar = authorAvatar;
        this.content = content;
        this.title = title;
        this.categoryID = categoryID;
        this.categoryName = categoryName;
        tagIDs = new List<string>();
        mediaURLs = new List<string>();
        postType = type;
        likeCount = 0;
        commentCount = 0;
        shareCount = 0;
        viewCount = 0;
        isPinned = false;
        isFeatured = false;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        updatedAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void AddTag(string tagID)
    {
        tagIDs.Add(tagID);
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
    
    public void IncrementViewCount()
    {
        viewCount++;
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
    
    public void UpdateContent(string content, string title)
    {
        this.content = content;
        this.title = title;
        updatedAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CommunityComment
{
    public string commentID;
    public string authorID;
    public string authorName;
    public string authorAvatar;
    public string targetID;
    public string targetType;
    public string content;
    public int likeCount;
    public string parentCommentID;
    public string createdAt;
    
    public CommunityComment(string id, string author, string authorName, string authorAvatar, string target, string targetType, string content, string parent = "")
    {
        commentID = id;
        authorID = author;
        this.authorName = authorName;
        this.authorAvatar = authorAvatar;
        this.targetID = target;
        this.targetType = targetType;
        this.content = content;
        likeCount = 0;
        parentCommentID = parent;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
public class CommunityLike
{
    public string likeID;
    public string userID;
    public string targetID;
    public string targetType;
    public string createdAt;
    
    public CommunityLike(string id, string user, string target, string type)
    {
        likeID = id;
        userID = user;
        targetID = target;
        targetType = type;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CommunityFollow
{
    public string followID;
    public string followerID;
    public string followerName;
    public string targetID;
    public string targetName;
    public string createdAt;
    
    public CommunityFollow(string id, string follower, string followerName, string target, string targetName)
    {
        followID = id;
        followerID = follower;
        this.followerName = followerName;
        this.targetID = target;
        this.targetName = targetName;
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class CommunityTag
{
    public string tagID;
    public string tagName;
    public string tagDescription;
    public int postCount;
    
    public CommunityTag(string id, string name, string desc)
    {
        tagID = id;
        tagName = name;
        tagDescription = desc;
        postCount = 0;
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
}

[System.Serializable]
public class CommunityCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    public int postCount;
    
    public CommunityCategory(string id, string name, string desc)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = desc;
        postCount = 0;
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
}

[System.Serializable]
public class CommunityManagerData
{
    public CommunitySystemExtended system;
    
    public CommunityManagerData()
    {
        system = new CommunitySystemExtended("community_system_extended", "社区系统扩展", "提供玩家动态、英雄攻略、游戏资讯等内容的进一步丰富");
    }
}
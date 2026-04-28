[System.Serializable]
public class CommunitySystemDetailed
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<Post> posts;
    public List<Comment> comments;
    public List<Like> likes;
    public List<Follow> follows;
    public List<Tag> tags;
    public List<Category> categories;
    public List<Notification> notifications;
    public List<UserProfile> userProfiles;
    
    public CommunitySystemDetailed(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        posts = new List<Post>();
        comments = new List<Comment>();
        likes = new List<Like>();
        follows = new List<Follow>();
        tags = new List<Tag>();
        categories = new List<Category>();
        notifications = new List<Notification>();
        userProfiles = new List<UserProfile>();
    }
    
    public void AddPost(Post post)
    {
        posts.Add(post);
    }
    
    public void AddComment(Comment comment)
    {
        comments.Add(comment);
    }
    
    public void AddLike(Like like)
    {
        likes.Add(like);
    }
    
    public void AddFollow(Follow follow)
    {
        follows.Add(follow);
    }
    
    public void AddTag(Tag tag)
    {
        tags.Add(tag);
    }
    
    public void AddCategory(Category category)
    {
        categories.Add(category);
    }
    
    public void AddNotification(Notification notification)
    {
        notifications.Add(notification);
    }
    
    public void AddUserProfile(UserProfile profile)
    {
        userProfiles.Add(profile);
    }
    
    public Post GetPost(string postID)
    {
        return posts.Find(p => p.postID == postID);
    }
    
    public Comment GetComment(string commentID)
    {
        return comments.Find(c => c.commentID == commentID);
    }
    
    public Like GetLike(string likeID)
    {
        return likes.Find(l => l.likeID == likeID);
    }
    
    public Follow GetFollow(string followID)
    {
        return follows.Find(f => f.followID == followID);
    }
    
    public Tag GetTag(string tagID)
    {
        return tags.Find(t => t.tagID == tagID);
    }
    
    public Category GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public Notification GetNotification(string notificationID)
    {
        return notifications.Find(n => n.notificationID == notificationID);
    }
    
    public UserProfile GetUserProfile(string userID)
    {
        return userProfiles.Find(p => p.userID == userID);
    }
    
    public List<Post> GetPostsByUser(string userID)
    {
        return posts.FindAll(p => p.userID == userID);
    }
    
    public List<Post> GetPostsByCategory(string categoryID)
    {
        return posts.FindAll(p => p.categoryID == categoryID);
    }
    
    public List<Post> GetPostsByTag(string tagID)
    {
        return posts.FindAll(p => p.tagIDs.Contains(tagID));
    }
    
    public List<Comment> GetCommentsByPost(string postID)
    {
        return comments.FindAll(c => c.postID == postID);
    }
    
    public List<Like> GetLikesByPost(string postID)
    {
        return likes.FindAll(l => l.postID == postID);
    }
    
    public List<Like> GetLikesByUser(string userID)
    {
        return likes.FindAll(l => l.userID == userID);
    }
    
    public List<Follow> GetFollowsByFollower(string followerID)
    {
        return follows.FindAll(f => f.followerID == followerID);
    }
    
    public List<Follow> GetFollowsByFollowing(string followingID)
    {
        return follows.FindAll(f => f.followingID == followingID);
    }
    
    public List<Notification> GetNotificationsByUser(string userID)
    {
        return notifications.FindAll(n => n.userID == userID);
    }
}

[System.Serializable]
public class Post
{
    public string postID;
    public string userID;
    public string userName;
    public string content;
    public string categoryID;
    public string postType;
    public string mediaPath;
    public string thumbnailPath;
    public int likeCount;
    public int commentCount;
    public int shareCount;
    public string createTime;
    public string updateTime;
    public bool isPublic;
    public bool isPinned;
    public List<string> tagIDs;
    
    public Post(string id, string userID, string userName, string content, string categoryID, string postType, string mediaPath = "", string thumbnailPath = "")
    {
        postID = id;
        this.userID = userID;
        this.userName = userName;
        this.content = content;
        this.categoryID = categoryID;
        this.postType = postType;
        this.mediaPath = mediaPath;
        this.thumbnailPath = thumbnailPath;
        likeCount = 0;
        commentCount = 0;
        shareCount = 0;
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        updateTime = createTime;
        isPublic = true;
        isPinned = false;
        tagIDs = new List<string>();
    }
    
    public void AddTag(string tagID)
    {
        tagIDs.Add(tagID);
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
    
    public void UpdateContent(string content)
    {
        this.content = content;
        updateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void SetPinned(bool pinned)
    {
        isPinned = pinned;
    }
    
    public void SetPublic(bool isPublic)
    {
        this.isPublic = isPublic;
    }
}

[System.Serializable]
public class Comment
{
    public string commentID;
    public string postID;
    public string userID;
    public string userName;
    public string content;
    public string parentCommentID;
    public int likeCount;
    public string createTime;
    public string updateTime;
    
    public Comment(string id, string postID, string userID, string userName, string content, string parentCommentID = "")
    {
        commentID = id;
        this.postID = postID;
        this.userID = userID;
        this.userName = userName;
        this.content = content;
        this.parentCommentID = parentCommentID;
        likeCount = 0;
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        updateTime = createTime;
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
    
    public void UpdateContent(string content)
    {
        this.content = content;
        updateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class Like
{
    public string likeID;
    public string postID;
    public string userID;
    public string commentID;
    public string createTime;
    
    public Like(string id, string postID, string userID, string commentID = "")
    {
        likeID = id;
        this.postID = postID;
        this.userID = userID;
        this.commentID = commentID;
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class Follow
{
    public string followID;
    public string followerID;
    public string followerName;
    public string followingID;
    public string followingName;
    public string createTime;
    
    public Follow(string id, string followerID, string followerName, string followingID, string followingName)
    {
        followID = id;
        this.followerID = followerID;
        this.followerName = followerName;
        this.followingID = followingID;
        this.followingName = followingName;
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class Tag
{
    public string tagID;
    public string tagName;
    public string description;
    public int usageCount;
    public bool isEnabled;
    
    public Tag(string id, string name, string desc)
    {
        tagID = id;
        tagName = name;
        description = desc;
        usageCount = 0;
        isEnabled = true;
    }
    
    public void IncrementUsageCount()
    {
        usageCount++;
    }
    
    public void DecrementUsageCount()
    {
        if (usageCount > 0)
        {
            usageCount--;
        }
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class Category
{
    public string categoryID;
    public string categoryName;
    public string description;
    public string iconPath;
    public int postCount;
    public bool isEnabled;
    
    public Category(string id, string name, string desc, string iconPath = "")
    {
        categoryID = id;
        categoryName = name;
        description = desc;
        this.iconPath = iconPath;
        postCount = 0;
        isEnabled = true;
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
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
}

[System.Serializable]
public class Notification
{
    public string notificationID;
    public string userID;
    public string type;
    public string content;
    public string relatedID;
    public bool isRead;
    public string createTime;
    
    public Notification(string id, string userID, string type, string content, string relatedID = "")
    {
        notificationID = id;
        this.userID = userID;
        this.type = type;
        this.content = content;
        this.relatedID = relatedID;
        isRead = false;
        createTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void MarkAsRead()
    {
        isRead = true;
    }
}

[System.Serializable]
public class UserProfile
{
    public string profileID;
    public string userID;
    public string userName;
    public string avatarPath;
    public string bio;
    public string region;
    public int postCount;
    public int followerCount;
    public int followingCount;
    public int likeCount;
    public string joinDate;
    public string lastActiveDate;
    public List<string> interests;
    
    public UserProfile(string id, string userID, string userName, string avatarPath = "", string bio = "", string region = "")
    {
        profileID = id;
        this.userID = userID;
        this.userName = userName;
        this.avatarPath = avatarPath;
        this.bio = bio;
        this.region = region;
        postCount = 0;
        followerCount = 0;
        followingCount = 0;
        likeCount = 0;
        joinDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        lastActiveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        interests = new List<string>();
    }
    
    public void AddInterest(string interest)
    {
        interests.Add(interest);
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
    
    public void IncrementFollowerCount()
    {
        followerCount++;
    }
    
    public void DecrementFollowerCount()
    {
        if (followerCount > 0)
        {
            followerCount--;
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
    
    public void UpdateLastActive()
    {
        lastActiveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public void UpdateProfile(string bio, string region)
    {
        this.bio = bio;
        this.region = region;
    }
}

[System.Serializable]
public class CommunitySystemDetailedManagerData
{
    public CommunitySystemDetailed system;
    
    public CommunitySystemDetailedManagerData()
    {
        system = new CommunitySystemDetailed("community_system_detailed", "社区系统详细", "管理社区的详细功能，包括玩家动态、英雄攻略、游戏资讯等内容");
    }
}
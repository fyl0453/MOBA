[System.Serializable]
public class HeroGuideSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<HeroGuide> guides;
    public List<Comment> comments;
    public List<Like> likes;
    
    public HeroGuideSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        guides = new List<HeroGuide>();
        comments = new List<Comment>();
        likes = new List<Like>();
    }
    
    public void AddGuide(HeroGuide guide)
    {
        guides.Add(guide);
    }
    
    public void AddComment(Comment comment)
    {
        comments.Add(comment);
    }
    
    public void AddLike(Like like)
    {
        likes.Add(like);
    }
    
    public HeroGuide GetGuide(string guideID)
    {
        return guides.Find(g => g.guideID == guideID);
    }
    
    public List<HeroGuide> GetGuidesByHero(string heroID)
    {
        return guides.FindAll(g => g.heroID == heroID);
    }
    
    public List<HeroGuide> GetGuidesByUser(string userID)
    {
        return guides.FindAll(g => g.authorID == userID);
    }
    
    public List<Comment> GetCommentsByGuide(string guideID)
    {
        return comments.FindAll(c => c.targetID == guideID);
    }
    
    public List<Like> GetLikesByGuide(string guideID)
    {
        return likes.FindAll(l => l.targetID == guideID);
    }
    
    public List<HeroGuide> GetTopGuides(int limit = 10)
    {
        List<HeroGuide> sortedGuides = new List<HeroGuide>(guides);
        sortedGuides.Sort((a, b) => b.likeCount.CompareTo(a.likeCount));
        return sortedGuides.GetRange(0, Mathf.Min(limit, sortedGuides.Count));
    }
}

[System.Serializable]
public class HeroGuide
{
    public string guideID;
    public string heroID;
    public string heroName;
    public string authorID;
    public string authorName;
    public string title;
    public string description;
    public string content;
    public string guideType;
    public List<ItemBuild> itemBuilds;
    public List<SkillBuild> skillBuilds;
    public List<RuneBuild> runeBuilds;
    public string createdAt;
    public int viewCount;
    public int likeCount;
    public int commentCount;
    public float rating;
    public int ratingCount;
    
    public HeroGuide(string id, string heroID, string heroName, string authorID, string authorName, string title, string desc, string content, string type)
    {
        guideID = id;
        this.heroID = heroID;
        this.heroName = heroName;
        this.authorID = authorID;
        this.authorName = authorName;
        this.title = title;
        description = desc;
        this.content = content;
        guideType = type;
        itemBuilds = new List<ItemBuild>();
        skillBuilds = new List<SkillBuild>();
        runeBuilds = new List<RuneBuild>();
        createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        viewCount = 0;
        likeCount = 0;
        commentCount = 0;
        rating = 0f;
        ratingCount = 0;
    }
    
    public void AddItemBuild(ItemBuild itemBuild)
    {
        itemBuilds.Add(itemBuild);
    }
    
    public void AddSkillBuild(SkillBuild skillBuild)
    {
        skillBuilds.Add(skillBuild);
    }
    
    public void AddRuneBuild(RuneBuild runeBuild)
    {
        runeBuilds.Add(runeBuild);
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
    
    public void AddRating(float score)
    {
        rating = (rating * ratingCount + score) / (ratingCount + 1);
        ratingCount++;
    }
}

[System.Serializable]
public class ItemBuild
{
    public string buildID;
    public string buildName;
    public List<string> itemIDs;
    public string buildDescription;
    
    public ItemBuild(string id, string name, List<string> items, string desc)
    {
        buildID = id;
        buildName = name;
        itemIDs = items;
        buildDescription = desc;
    }
}

[System.Serializable]
public class SkillBuild
{
    public string buildID;
    public string buildName;
    public List<string> skillOrder;
    public string buildDescription;
    
    public SkillBuild(string id, string name, List<string> order, string desc)
    {
        buildID = id;
        buildName = name;
        skillOrder = order;
        buildDescription = desc;
    }
}

[System.Serializable]
public class RuneBuild
{
    public string buildID;
    public string buildName;
    public List<string> runeIDs;
    public string buildDescription;
    
    public RuneBuild(string id, string name, List<string> runes, string desc)
    {
        buildID = id;
        buildName = name;
        runeIDs = runes;
        buildDescription = desc;
    }
}

[System.Serializable]
public class Comment
{
    public string commentID;
    public string targetID;
    public string authorID;
    public string authorName;
    public string content;
    public string createdAt;
    public int likeCount;
    
    public Comment(string id, string target, string author, string authorName, string content)
    {
        commentID = id;
        targetID = target;
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
public class HeroGuideManagerData
{
    public HeroGuideSystem system;
    
    public HeroGuideManagerData()
    {
        system = new HeroGuideSystem("hero_guide", "英雄攻略系统", "管理英雄攻略");
    }
}
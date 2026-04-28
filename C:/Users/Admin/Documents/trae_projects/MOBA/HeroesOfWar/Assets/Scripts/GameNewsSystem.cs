[System.Serializable]
public class GameNewsSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<GameNews> news;
    public List<NewsCategory> categories;
    
    public GameNewsSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        news = new List<GameNews>();
        categories = new List<NewsCategory>();
    }
    
    public void AddNews(GameNews gameNews)
    {
        news.Add(gameNews);
    }
    
    public void AddCategory(NewsCategory category)
    {
        categories.Add(category);
    }
    
    public GameNews GetNews(string newsID)
    {
        return news.Find(n => n.newsID == newsID);
    }
    
    public NewsCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }
    
    public List<GameNews> GetNewsByCategory(string categoryID)
    {
        return news.FindAll(n => n.categoryID == categoryID);
    }
    
    public List<GameNews> GetLatestNews(int limit = 20)
    {
        List<GameNews> sorted = new List<GameNews>(news);
        sorted.Sort((a, b) => b.publishTime.CompareTo(a.publishTime));
        return sorted.GetRange(0, Mathf.Min(limit, sorted.Count));
    }
}

[System.Serializable]
public class NewsCategory
{
    public string categoryID;
    public string categoryName;
    public string categoryDescription;
    public string categoryIcon;
    
    public NewsCategory(string id, string name, string desc)
    {
        categoryID = id;
        categoryName = name;
        categoryDescription = desc;
        categoryIcon = "";
    }
}

[System.Serializable]
public class GameNews
{
    public string newsID;
    public string title;
    public string content;
    public string summary;
    public string categoryID;
    public string categoryName;
    public string author;
    public string publishTime;
    public string updateTime;
    public List<string> imageURLs;
    public int viewCount;
    public int likeCount;
    public bool isPinned;
    public bool isFeatured;
    
    public GameNews(string id, string title, string content, string summary, string categoryID, string categoryName, string author)
    {
        newsID = id;
        this.title = title;
        this.content = content;
        this.summary = summary;
        this.categoryID = categoryID;
        this.categoryName = categoryName;
        this.author = author;
        publishTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        updateTime = "";
        imageURLs = new List<string>();
        viewCount = 0;
        likeCount = 0;
        isPinned = false;
        isFeatured = false;
    }
    
    public void AddImage(string url)
    {
        imageURLs.Add(url);
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
public class GameNewsManagerData
{
    public GameNewsSystem system;
    
    public GameNewsManagerData()
    {
        system = new GameNewsSystem("game_news_system", "游戏资讯系统", "管理游戏新闻和公告");
    }
}
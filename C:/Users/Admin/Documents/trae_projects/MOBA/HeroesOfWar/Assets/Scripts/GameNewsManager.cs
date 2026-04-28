using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameNewsManager : MonoBehaviour
{
    public static GameNewsManager Instance { get; private set; }
    
    public GameNewsManagerData gameNewsData;
    
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
        LoadGameNewsData();
        
        if (gameNewsData == null)
        {
            gameNewsData = new GameNewsManagerData();
            InitializeDefaultNews();
        }
    }
    
    private void InitializeDefaultNews()
    {
        // 创建默认新闻分类
        NewsCategory category1 = new NewsCategory("category_news", "新闻", "游戏最新新闻");
        NewsCategory category2 = new NewsCategory("category_announcement", "公告", "游戏官方公告");
        NewsCategory category3 = new NewsCategory("category_activity", "活动", "游戏活动资讯");
        NewsCategory category4 = new NewsCategory("category_update", "更新", "游戏更新内容");
        
        gameNewsData.system.AddCategory(category1);
        gameNewsData.system.AddCategory(category2);
        gameNewsData.system.AddCategory(category3);
        gameNewsData.system.AddCategory(category4);
        
        // 创建默认新闻
        GameNews news1 = new GameNews(
            "news_1",
            "《Heroes of War》正式上线！",
            "亲爱的玩家，《Heroes of War》今日正式上线！感谢大家的支持与等待。\n\n《Heroes of War》是一款全新MOBA手游，为玩家带来极致的游戏体验。\n\n游戏特色：\n- 精美画质\n- 流畅操作\n- 丰富英雄\n- 多种模式",
            "《Heroes of War》今日正式上线，感谢大家的支持与等待。",
            "category_news",
            "新闻",
            "官方运营团队"
        );
        news1.Feature();
        
        GameNews news2 = new GameNews(
            "news_2",
            "首发福利活动公告",
            "为庆祝游戏上线，我们准备了丰富的首发福利活动。\n\n活动时间：2024年1月1日至2024年1月31日\n\n活动内容：\n1. 每日登录送好礼\n2. 累计充值返利\n3. 邀请好友得奖励",
            "首发福利活动公告，登录即送好礼。",
            "category_activity",
            "活动",
            "官方运营团队"
        );
        news2.Pin();
        
        GameNews news3 = new GameNews(
            "news_3",
            "版本更新通知 V1.1.0",
            "亲爱的玩家，我们将于2024年2月1日进行版本更新。\n\n更新内容：\n1. 新增3个英雄\n2. 优化游戏性能\n3. 修复已知bug\n4. 调整部分英雄平衡性",
            "版本更新通知 V1.1.0，新增3个英雄。",
            "category_update",
            "更新",
            "官方运营团队"
        );
        
        gameNewsData.system.AddNews(news1);
        gameNewsData.system.AddNews(news2);
        gameNewsData.system.AddNews(news3);
        
        SaveGameNewsData();
    }
    
    public string CreateNews(string title, string content, string summary, string categoryID, string author, List<string> imageURLs = null)
    {
        NewsCategory category = gameNewsData.system.GetCategory(categoryID);
        if (category != null)
        {
            string newsID = System.Guid.NewGuid().ToString();
            GameNews newNews = new GameNews(newsID, title, content, summary, categoryID, category.categoryName, author);
            
            if (imageURLs != null)
            {
                foreach (string url in imageURLs)
                {
                    newNews.AddImage(url);
                }
            }
            
            gameNewsData.system.AddNews(newNews);
            SaveGameNewsData();
            Debug.Log($"成功创建新闻: {title}");
            return newsID;
        }
        return "";
    }
    
    public void PinNews(string newsID, bool pin)
    {
        GameNews news = gameNewsData.system.GetNews(newsID);
        if (news != null)
        {
            if (pin)
            {
                news.Pin();
            }
            else
            {
                news.Unpin();
            }
            SaveGameNewsData();
            Debug.Log($"成功{(pin ? "置顶" : "取消置顶")}新闻: {news.title}");
        }
    }
    
    public void FeatureNews(string newsID, bool feature)
    {
        GameNews news = gameNewsData.system.GetNews(newsID);
        if (news != null)
        {
            if (feature)
            {
                news.Feature();
            }
            else
            {
                news.Unfeature();
            }
            SaveGameNewsData();
            Debug.Log($"成功{(feature ? "推荐" : "取消推荐")}新闻: {news.title}");
        }
    }
    
    public void ViewNews(string newsID)
    {
        GameNews news = gameNewsData.system.GetNews(newsID);
        if (news != null)
        {
            news.IncrementViewCount();
            SaveGameNewsData();
        }
    }
    
    public void LikeNews(string newsID)
    {
        GameNews news = gameNewsData.system.GetNews(newsID);
        if (news != null)
        {
            news.IncrementLikeCount();
            SaveGameNewsData();
            Debug.Log($"成功点赞新闻: {news.title}");
        }
    }
    
    public void UnlikeNews(string newsID)
    {
        GameNews news = gameNewsData.system.GetNews(newsID);
        if (news != null)
        {
            news.DecrementLikeCount();
            SaveGameNewsData();
            Debug.Log($"成功取消点赞新闻: {news.title}");
        }
    }
    
    public List<GameNews> GetLatestNews(int limit = 20)
    {
        return gameNewsData.system.GetLatestNews(limit);
    }
    
    public List<GameNews> GetNewsByCategory(string categoryID, int limit = 20)
    {
        List<GameNews> newsList = gameNewsData.system.GetNewsByCategory(categoryID);
        newsList.Sort((a, b) => b.publishTime.CompareTo(a.publishTime));
        return newsList.GetRange(0, Mathf.Min(limit, newsList.Count));
    }
    
    public List<GameNews> GetPinnedNews(int limit = 5)
    {
        List<GameNews> pinnedNews = gameNewsData.system.news.FindAll(n => n.isPinned);
        pinnedNews.Sort((a, b) => b.publishTime.CompareTo(a.publishTime));
        return pinnedNews.GetRange(0, Mathf.Min(limit, pinnedNews.Count));
    }
    
    public List<GameNews> GetFeaturedNews(int limit = 10)
    {
        List<GameNews> featuredNews = gameNewsData.system.news.FindAll(n => n.isFeatured);
        featuredNews.Sort((a, b) => b.publishTime.CompareTo(a.publishTime));
        return featuredNews.GetRange(0, Mathf.Min(limit, featuredNews.Count));
    }
    
    public GameNews GetNews(string newsID)
    {
        return gameNewsData.system.GetNews(newsID);
    }
    
    public List<NewsCategory> GetAllCategories()
    {
        return gameNewsData.system.categories;
    }
    
    public void AddCategory(string name, string description)
    {
        string categoryID = System.Guid.NewGuid().ToString();
        NewsCategory newCategory = new NewsCategory(categoryID, name, description);
        gameNewsData.system.AddCategory(newCategory);
        SaveGameNewsData();
        Debug.Log($"成功添加新闻分类: {name}");
    }
    
    public void SaveGameNewsData()
    {
        string path = Application.dataPath + "/Data/game_news_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, gameNewsData);
        stream.Close();
    }
    
    public void LoadGameNewsData()
    {
        string path = Application.dataPath + "/Data/game_news_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            gameNewsData = (GameNewsManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            gameNewsData = new GameNewsManagerData();
        }
    }
}
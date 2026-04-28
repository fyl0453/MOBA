using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HeroGuideManager : MonoBehaviour
{
    public static HeroGuideManager Instance { get; private set; }
    
    public HeroGuideManagerData guideData;
    
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
        LoadGuideData();
        
        if (guideData == null)
        {
            guideData = new HeroGuideManagerData();
            InitializeDefaultGuides();
        }
    }
    
    private void InitializeDefaultGuides()
    {
        // 创建关羽的攻略
        List<string> items = new List<string> { "equipment_blade", "equipment_armor", "equipment_boots" };
        List<string> skills = new List<string> { "skill_guanyu_1", "skill_guanyu_2", "skill_guanyu_ult" };
        List<string> runes = new List<string> { "rune_strength", "rune_vitality", "rune_focus" };
        
        ItemBuild itemBuild = new ItemBuild("item_build_1", "战士装", items, "适合关羽的战士出装");
        SkillBuild skillBuild = new SkillBuild("skill_build_1", "技能加点", skills, "主1副2，有大点大");
        RuneBuild runeBuild = new RuneBuild("rune_build_1", "通用符文", runes, "适合战士的通用符文");
        
        HeroGuide guide1 = new HeroGuide("guide_1", "hero_guanyu", "关羽", "user_1", "Player1", "关羽玩法攻略", "关羽是一个非常强大的战士英雄，需要掌握好冲锋状态", "关羽的核心在于冲锋状态的利用，需要不断移动来保持冲锋状态...", "Guide");
        guide1.AddItemBuild(itemBuild);
        guide1.AddSkillBuild(skillBuild);
        guide1.AddRuneBuild(runeBuild);
        
        guideData.system.AddGuide(guide1);
        
        // 创建诸葛亮的攻略
        List<string> items2 = new List<string> { "equipment_book", "equipment_staff", "equipment_magic_resist" };
        List<string> skills2 = new List<string> { "skill_zhugeliang_1", "skill_zhugeliang_2", "skill_zhugeliang_ult" };
        List<string> runes2 = new List<string> { "rune_intellect", "rune_wisdom", "rune_energy" };
        
        ItemBuild itemBuild2 = new ItemBuild("item_build_2", "法师装", items2, "适合诸葛亮的法师出装");
        SkillBuild skillBuild2 = new SkillBuild("skill_build_2", "技能加点", skills2, "主1副2，有大点大");
        RuneBuild runeBuild2 = new RuneBuild("rune_build_2", "法师符文", runes2, "适合法师的通用符文");
        
        HeroGuide guide2 = new HeroGuide("guide_2", "hero_zhugeliang", "诸葛亮", "user_2", "Player2", "诸葛亮玩法攻略", "诸葛亮是一个高爆发的法师英雄，需要掌握好技能连招", "诸葛亮的核心在于技能的命中，需要利用被动技能来增加伤害...", "Guide");
        guide2.AddItemBuild(itemBuild2);
        guide2.AddSkillBuild(skillBuild2);
        guide2.AddRuneBuild(runeBuild2);
        
        guideData.system.AddGuide(guide2);
        
        // 创建默认评论
        Comment comment1 = new Comment("comment_1", "guide_1", "user_2", "Player2", "非常详细的攻略，学习了");
        Comment comment2 = new Comment("comment_2", "guide_1", "user_3", "Player3", "关羽确实需要很好的操作技巧");
        Comment comment3 = new Comment("comment_3", "guide_2", "user_1", "Player1", "诸葛亮的连招很重要");
        
        guideData.system.AddComment(comment1);
        guideData.system.AddComment(comment2);
        guideData.system.AddComment(comment3);
        
        // 创建默认点赞
        Like like1 = new Like("like_1", "user_2", "guide_1", "Guide");
        Like like2 = new Like("like_2", "user_3", "guide_1", "Guide");
        Like like3 = new Like("like_3", "user_1", "guide_2", "Guide");
        
        guideData.system.AddLike(like1);
        guideData.system.AddLike(like2);
        guideData.system.AddLike(like3);
        
        // 更新攻略统计
        guide1.IncrementLikeCount();
        guide1.IncrementLikeCount();
        guide1.IncrementCommentCount();
        guide1.IncrementCommentCount();
        
        guide2.IncrementLikeCount();
        guide2.IncrementCommentCount();
        
        SaveGuideData();
    }
    
    public string CreateGuide(string heroID, string heroName, string authorID, string authorName, string title, string description, string content, string type)
    {
        string guideID = System.Guid.NewGuid().ToString();
        HeroGuide guide = new HeroGuide(guideID, heroID, heroName, authorID, authorName, title, description, content, type);
        guideData.system.AddGuide(guide);
        SaveGuideData();
        return guideID;
    }
    
    public void AddItemBuild(string guideID, string buildName, List<string> itemIDs, string description)
    {
        HeroGuide guide = guideData.system.GetGuide(guideID);
        if (guide != null)
        {
            string buildID = System.Guid.NewGuid().ToString();
            ItemBuild itemBuild = new ItemBuild(buildID, buildName, itemIDs, description);
            guide.AddItemBuild(itemBuild);
            SaveGuideData();
        }
    }
    
    public void AddSkillBuild(string guideID, string buildName, List<string> skillOrder, string description)
    {
        HeroGuide guide = guideData.system.GetGuide(guideID);
        if (guide != null)
        {
            string buildID = System.Guid.NewGuid().ToString();
            SkillBuild skillBuild = new SkillBuild(buildID, buildName, skillOrder, description);
            guide.AddSkillBuild(skillBuild);
            SaveGuideData();
        }
    }
    
    public void AddRuneBuild(string guideID, string buildName, List<string> runeIDs, string description)
    {
        HeroGuide guide = guideData.system.GetGuide(guideID);
        if (guide != null)
        {
            string buildID = System.Guid.NewGuid().ToString();
            RuneBuild runeBuild = new RuneBuild(buildID, buildName, runeIDs, description);
            guide.AddRuneBuild(runeBuild);
            SaveGuideData();
        }
    }
    
    public string CreateComment(string guideID, string authorID, string authorName, string content)
    {
        HeroGuide guide = guideData.system.GetGuide(guideID);
        if (guide != null)
        {
            string commentID = System.Guid.NewGuid().ToString();
            Comment comment = new Comment(commentID, guideID, authorID, authorName, content);
            guideData.system.AddComment(comment);
            guide.IncrementCommentCount();
            SaveGuideData();
            return commentID;
        }
        return "";
    }
    
    public void LikeGuide(string guideID, string userID)
    {
        HeroGuide guide = guideData.system.GetGuide(guideID);
        if (guide != null)
        {
            // 检查是否已经点赞
            List<Like> likes = guideData.system.GetLikesByGuide(guideID);
            if (!likes.Exists(l => l.userID == userID))
            {
                string likeID = System.Guid.NewGuid().ToString();
                Like like = new Like(likeID, userID, guideID, "Guide");
                guideData.system.AddLike(like);
                guide.IncrementLikeCount();
                SaveGuideData();
            }
        }
    }
    
    public void UnlikeGuide(string guideID, string userID)
    {
        HeroGuide guide = guideData.system.GetGuide(guideID);
        if (guide != null)
        {
            List<Like> likes = guideData.system.GetLikesByGuide(guideID);
            Like like = likes.Find(l => l.userID == userID);
            if (like != null)
            {
                guideData.system.likes.Remove(like);
                guide.DecrementLikeCount();
                SaveGuideData();
            }
        }
    }
    
    public void RateGuide(string guideID, float rating)
    {
        HeroGuide guide = guideData.system.GetGuide(guideID);
        if (guide != null)
        {
            guide.AddRating(rating);
            SaveGuideData();
        }
    }
    
    public void ViewGuide(string guideID)
    {
        HeroGuide guide = guideData.system.GetGuide(guideID);
        if (guide != null)
        {
            guide.IncrementViewCount();
            SaveGuideData();
        }
    }
    
    public List<HeroGuide> GetGuidesByHero(string heroID, int limit = 20)
    {
        List<HeroGuide> guides = guideData.system.GetGuidesByHero(heroID);
        guides.Sort((a, b) => b.likeCount.CompareTo(a.likeCount));
        return guides.GetRange(0, Mathf.Min(limit, guides.Count));
    }
    
    public List<HeroGuide> GetGuidesByUser(string userID, int limit = 20)
    {
        List<HeroGuide> guides = guideData.system.GetGuidesByUser(userID);
        guides.Sort((a, b) => b.createdAt.CompareTo(a.createdAt));
        return guides.GetRange(0, Mathf.Min(limit, guides.Count));
    }
    
    public List<HeroGuide> GetTopGuides(int limit = 10)
    {
        return guideData.system.GetTopGuides(limit);
    }
    
    public HeroGuide GetGuide(string guideID)
    {
        return guideData.system.GetGuide(guideID);
    }
    
    public List<Comment> GetCommentsByGuide(string guideID, int limit = 50)
    {
        List<Comment> comments = guideData.system.GetCommentsByGuide(guideID);
        comments.Sort((a, b) => a.createdAt.CompareTo(b.createdAt));
        return comments.GetRange(0, Mathf.Min(limit, comments.Count));
    }
    
    public void DeleteGuide(string guideID)
    {
        HeroGuide guide = guideData.system.GetGuide(guideID);
        if (guide != null)
        {
            // 删除相关评论和点赞
            List<Comment> comments = guideData.system.GetCommentsByGuide(guideID);
            foreach (Comment comment in comments)
            {
                guideData.system.comments.Remove(comment);
            }
            
            List<Like> likes = guideData.system.GetLikesByGuide(guideID);
            foreach (Like like in likes)
            {
                guideData.system.likes.Remove(like);
            }
            
            guideData.system.guides.Remove(guide);
            SaveGuideData();
        }
    }
    
    public void DeleteComment(string commentID)
    {
        Comment comment = guideData.system.comments.Find(c => c.commentID == commentID);
        if (comment != null)
        {
            HeroGuide guide = guideData.system.GetGuide(comment.targetID);
            if (guide != null)
            {
                guide.DecrementCommentCount();
            }
            guideData.system.comments.Remove(comment);
            SaveGuideData();
        }
    }
    
    public void SaveGuideData()
    {
        string path = Application.dataPath + "/Data/hero_guide_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, guideData);
        stream.Close();
    }
    
    public void LoadGuideData()
    {
        string path = Application.dataPath + "/Data/hero_guide_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            guideData = (HeroGuideManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            guideData = new HeroGuideManagerData();
        }
    }
}
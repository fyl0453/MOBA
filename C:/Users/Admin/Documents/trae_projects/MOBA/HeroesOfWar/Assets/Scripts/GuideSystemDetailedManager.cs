using System;
using System.Collections.Generic;

public class GuideSystemDetailedManager
{
    private static GuideSystemDetailedManager _instance;
    public static GuideSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GuideSystemDetailedManager();
            }
            return _instance;
        }
    }

    private GuideSystemData guideData;
    private GuideSystemDataManager dataManager;

    private GuideSystemDetailedManager()
    {
        dataManager = GuideSystemDataManager.Instance;
        guideData = dataManager.guideData;
    }

    public void CreateGuide(string heroID, string guideTitle, string authorID, string authorName, int guideType, string content, List<string> recommendedItems, List<string> skillOrder, List<string> talentConfig, bool isOfficial = false)
    {
        string guideID = "guide_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HeroGuide guide = new HeroGuide(guideID, heroID, guideTitle, authorID, authorName, guideType, content);
        guide.RecommendedItems = recommendedItems;
        guide.SkillOrder = skillOrder;
        guide.TalentConfig = talentConfig;
        guide.IsOfficial = isOfficial;

        guideData.AddGuide(guide);
        dataManager.CreateGuideEvent("guide_create", authorID, guideID, "创建攻略: " + guideTitle);
        dataManager.SaveGuideData();
        Debug.Log("创建攻略成功: " + guideTitle);
    }

    public void UpdateGuide(string guideID, string content, List<string> recommendedItems, List<string> skillOrder)
    {
        HeroGuide guide = guideData.AllGuides.Find(g => g.GuideID == guideID);
        if (guide != null)
        {
            guide.Content = content;
            guide.RecommendedItems = recommendedItems;
            guide.SkillOrder = skillOrder;
            guide.LastUpdateTime = DateTime.Now;
            dataManager.CreateGuideEvent("guide_update", guide.AuthorID, guideID, "更新攻略");
            dataManager.SaveGuideData();
            Debug.Log("更新攻略成功");
        }
    }

    public void DeleteGuide(string guideID)
    {
        HeroGuide guide = guideData.AllGuides.Find(g => g.GuideID == guideID);
        if (guide != null)
        {
            guideData.AllGuides.Remove(guide);
            if (guideData.HeroGuides.ContainsKey(guide.HeroID))
            {
                guideData.HeroGuides[guide.HeroID].Remove(guide);
            }
            dataManager.CreateGuideEvent("guide_delete", guide.AuthorID, guideID, "删除攻略");
            dataManager.SaveGuideData();
            Debug.Log("删除攻略成功");
        }
    }

    public HeroGuide GetGuide(string guideID)
    {
        return guideData.AllGuides.Find(g => g.GuideID == guideID);
    }

    public List<HeroGuide> GetHeroGuides(string heroID)
    {
        if (guideData.HeroGuides.ContainsKey(heroID))
        {
            return guideData.HeroGuides[heroID];
        }
        return new List<HeroGuide>();
    }

    public List<HeroGuide> GetHeroGuidesByType(string heroID, int guideType)
    {
        if (guideData.HeroGuides.ContainsKey(heroID))
        {
            return guideData.HeroGuides[heroID].FindAll(g => g.GuideType == guideType);
        }
        return new List<HeroGuide>();
    }

    public List<HeroGuide> GetTopRatedGuides(string heroID, int count = 10)
    {
        List<HeroGuide> guides = GetHeroGuides(heroID);
        guides.Sort((a, b) => b.Rating.CompareTo(a.Rating));
        if (count < guides.Count)
        {
            return guides.GetRange(0, count);
        }
        return guides;
    }

    public List<HeroGuide> GetFeaturedGuides()
    {
        return guideData.AllGuides.FindAll(g => g.IsFeatured);
    }

    public List<HeroGuide> GetOfficialGuides()
    {
        return guideData.AllGuides.FindAll(g => g.IsOfficial);
    }

    public List<HeroGuide> SearchGuides(string keyword)
    {
        return guideData.AllGuides.FindAll(g =>
            g.GuideTitle.Contains(keyword) ||
            g.Content.Contains(keyword) ||
            g.Tags.Contains(keyword));
    }

    public void UpvoteGuide(string playerID, string guideID)
    {
        if (guideData.PlayerDownvotes.ContainsKey(playerID) && guideData.PlayerDownvotes[playerID].Contains(guideID))
        {
            Debug.LogWarning("已经踩过了");
            return;
        }

        HeroGuide guide = guideData.AllGuides.Find(g => g.GuideID == guideID);
        if (guide != null)
        {
            guide.UpvoteCount++;
            if (!guideData.PlayerUpvotes.ContainsKey(playerID))
            {
                guideData.PlayerUpvotes[playerID] = new List<string>();
            }
            guideData.PlayerUpvotes[playerID].Add(guideID);
            UpdateGuideRating(guide);
            dataManager.CreateGuideEvent("guide_upvote", playerID, guideID, "点赞攻略");
            dataManager.SaveGuideData();
            Debug.Log("点赞攻略成功");
        }
    }

    public void DownvoteGuide(string playerID, string guideID)
    {
        if (guideData.PlayerUpvotes.ContainsKey(playerID) && guideData.PlayerUpvotes[playerID].Contains(guideID))
        {
            Debug.LogWarning("已经赞过了");
            return;
        }

        HeroGuide guide = guideData.AllGuides.Find(g => g.GuideID == guideID);
        if (guide != null)
        {
            guide.DownvoteCount++;
            if (!guideData.PlayerDownvotes.ContainsKey(playerID))
            {
                guideData.PlayerDownvotes[playerID] = new List<string>();
            }
            guideData.PlayerDownvotes[playerID].Add(guideID);
            UpdateGuideRating(guide);
            dataManager.CreateGuideEvent("guide_downvote", playerID, guideID, "踩攻略");
            dataManager.SaveGuideData();
            Debug.Log("踩攻略成功");
        }
    }

    private void UpdateGuideRating(HeroGuide guide)
    {
        int totalVotes = guide.UpvoteCount + guide.DownvoteCount;
        if (totalVotes > 0)
        {
            guide.Rating = (double)guide.UpvoteCount / totalVotes * 100.0;
        }
    }

    public void FavoriteGuide(string playerID, string guideID)
    {
        if (!guideData.PlayerFavoriteGuides.ContainsKey(playerID))
        {
            guideData.PlayerFavoriteGuides[playerID] = new List<string>();
        }
        if (!guideData.PlayerFavoriteGuides[playerID].Contains(guideID))
        {
            guideData.PlayerFavoriteGuides[playerID].Add(guideID);
            dataManager.CreateGuideEvent("guide_favorite", playerID, guideID, "收藏攻略");
            dataManager.SaveGuideData();
            Debug.Log("收藏攻略成功");
        }
    }

    public void UnfavoriteGuide(string playerID, string guideID)
    {
        if (guideData.PlayerFavoriteGuides.ContainsKey(playerID))
        {
            guideData.PlayerFavoriteGuides[playerID].Remove(guideID);
            dataManager.SaveGuideData();
            Debug.Log("取消收藏攻略成功");
        }
    }

    public List<HeroGuide> GetFavoriteGuides(string playerID)
    {
        if (guideData.PlayerFavoriteGuides.ContainsKey(playerID))
        {
            List<HeroGuide> favorites = new List<HeroGuide>();
            foreach (string guideID in guideData.PlayerFavoriteGuides[playerID])
            {
                HeroGuide guide = guideData.AllGuides.Find(g => g.GuideID == guideID);
                if (guide != null)
                {
                    favorites.Add(guide);
                }
            }
            return favorites;
        }
        return new List<HeroGuide>();
    }

    public void IncrementGuideUsage(string guideID)
    {
        HeroGuide guide = guideData.AllGuides.Find(g => g.GuideID == guideID);
        if (guide != null)
        {
            guide.UsageCount++;
            dataManager.SaveGuideData();
        }
    }

    public void SetGuideFeatured(string guideID, bool featured)
    {
        HeroGuide guide = guideData.AllGuides.Find(g => g.GuideID == guideID);
        if (guide != null)
        {
            guide.IsFeatured = featured;
            if (featured && !guideData.FeaturedGuideIDs.Contains(guideID))
            {
                guideData.FeaturedGuideIDs.Add(guideID);
            }
            else if (!featured)
            {
                guideData.FeaturedGuideIDs.Remove(guideID);
            }
            dataManager.SaveGuideData();
            Debug.Log("设置精选攻略: " + featured);
        }
    }

    public void CreateCombo(string heroID, string comboName, List<string> skillSequence, string timing, string description, int difficulty)
    {
        string comboID = "combo_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ComboSkill combo = new ComboSkill(comboID, heroID, comboName, skillSequence, timing, description, difficulty);
        guideData.AddCombo(combo);
        dataManager.CreateGuideEvent("combo_create", "", heroID, "创建连招: " + comboName);
        dataManager.SaveGuideData();
        Debug.Log("创建连招成功: " + comboName);
    }

    public List<ComboSkill> GetHeroCombos(string heroID)
    {
        return guideData.AllCombos.FindAll(c => c.HeroID == heroID);
    }

    public List<ComboSkill> GetCombosByDifficulty(string heroID, int difficulty)
    {
        return guideData.AllCombos.FindAll(c => c.HeroID == heroID && c.Difficulty == difficulty);
    }

    public void CreateEquipmentBuild(string heroID, string buildName, string creatorID, List<string> coreItems, List<string> optionalItems, List<string> earlyGameItems, List<string> lateGameItems, string strategy)
    {
        string buildID = "build_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        EquipmentBuild build = new EquipmentBuild(buildID, heroID, buildName, creatorID);
        build.CoreItems = coreItems;
        build.OptionalItems = optionalItems;
        build.EarlyGameItems = earlyGameItems;
        build.LateGameItems = lateGameItems;
        build.Strategy = strategy;
        guideData.AddBuild(build);
        dataManager.CreateGuideEvent("build_create", creatorID, heroID, "创建出装: " + buildName);
        dataManager.SaveGuideData();
        Debug.Log("创建出装成功: " + buildName);
    }

    public List<EquipmentBuild> GetHeroBuilds(string heroID)
    {
        return guideData.AllBuilds.FindAll(b => b.HeroID == heroID);
    }

    public List<EquipmentBuild> GetTopWinRateBuilds(string heroID, int count = 5)
    {
        List<EquipmentBuild> builds = GetHeroBuilds(heroID);
        builds.Sort((a, b) => b.WinRate.CompareTo(a.WinRate));
        if (count < builds.Count)
        {
            return builds.GetRange(0, count);
        }
        return builds;
    }

    public void UpdateBuildWinRate(string buildID, bool isWin)
    {
        EquipmentBuild build = guideData.AllBuilds.Find(b => b.BuildID == buildID);
        if (build != null)
        {
            int totalGames = build.UsageCount;
            int totalWins = (int)(build.WinRate / 100.0 * totalGames);
            if (isWin) totalWins++;
            totalGames++;
            build.WinRate = (double)totalWins / totalGames * 100.0;
            build.UsageCount = totalGames;
            dataManager.SaveGuideData();
        }
    }

    public void AddComment(string guideID, string playerID, string playerName, string content, string parentCommentID = "")
    {
        string commentID = "comment_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuideComment comment = new GuideComment(commentID, guideID, playerID, playerName, content, parentCommentID);
        guideData.AddComment(comment);
        dataManager.CreateGuideEvent("comment_add", playerID, guideID, "添加评论");
        dataManager.SaveGuideData();
        Debug.Log("添加评论成功");
    }

    public List<GuideComment> GetGuideComments(string guideID)
    {
        return guideData.AllComments.FindAll(c => c.GuideID == guideID);
    }

    public List<GuideComment> GetRootComments(string guideID)
    {
        return guideData.AllComments.FindAll(c => c.GuideID == guideID && string.IsNullOrEmpty(c.ParentCommentID));
    }

    public List<GuideComment> GetReplies(string parentCommentID)
    {
        return guideData.AllComments.FindAll(c => c.ParentCommentID == parentCommentID);
    }

    public void LikeComment(string commentID)
    {
        GuideComment comment = guideData.AllComments.Find(c => c.CommentID == commentID);
        if (comment != null)
        {
            comment.LikeCount++;
            dataManager.SaveGuideData();
        }
    }

    public List<GuideEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveGuideData();
    }

    public void LoadData()
    {
        dataManager.LoadGuideData();
    }
}
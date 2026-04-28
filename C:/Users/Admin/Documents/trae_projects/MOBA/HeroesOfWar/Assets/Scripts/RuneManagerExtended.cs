using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RuneManagerExtended : MonoBehaviour
{
    public static RuneManagerExtended Instance { get; private set; }
    
    public RuneSystemExtendedData runeData;
    
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
        LoadRuneData();
        
        if (runeData == null)
        {
            runeData = new RuneSystemExtendedData();
            InitializeDefaultRunes();
        }
    }
    
    private void InitializeDefaultRunes()
    {
        // 创建默认铭文
        Rune rune1 = new Rune("rune_strength", "力量", "增加攻击力", "Primary", "Physical", 500, 1);
        rune1.AddAttribute("AttackDamage", 2.0f);
        
        Rune rune2 = new Rune("rune_vitality", "活力", "增加生命值", "Primary", "Defense", 500, 1);
        rune2.AddAttribute("Health", 40.0f);
        
        Rune rune3 = new Rune("rune_focus", "专注", "增加冷却缩减", "Primary", "Utility", 500, 1);
        rune3.AddAttribute("CooldownReduction", 0.05f);
        
        Rune rune4 = new Rune("rune_intellect", "智力", "增加法术强度", "Primary", "Magic", 500, 1);
        rune4.AddAttribute("AbilityPower", 2.0f);
        
        Rune rune5 = new Rune("rune_wisdom", "智慧", "增加法力值", "Secondary", "Magic", 300, 1);
        rune5.AddAttribute("Mana", 20.0f);
        
        Rune rune6 = new Rune("rune_energy", "能量", "增加能量回复", "Secondary", "Utility", 300, 1);
        rune6.AddAttribute("ManaRegen", 1.0f);
        
        runeData.system.AddRune(rune1);
        runeData.system.AddRune(rune2);
        runeData.system.AddRune(rune3);
        runeData.system.AddRune(rune4);
        runeData.system.AddRune(rune5);
        runeData.system.AddRune(rune6);
        
        // 创建默认铭文页
        RunePage page1 = new RunePage("page_physical", "物理输出", "适合物理输出英雄", "system", "System", "hero_guanyu");
        page1.AddRune("rune_strength");
        page1.AddRune("rune_vitality");
        page1.AddRune("rune_focus");
        page1.SetPublic(true);
        
        RunePage page2 = new RunePage("page_magic", "法术输出", "适合法术输出英雄", "system", "System", "hero_zhugeliang");
        page2.AddRune("rune_intellect");
        page2.AddRune("rune_wisdom");
        page2.AddRune("rune_energy");
        page2.SetPublic(true);
        
        runeData.system.AddRunePage(page1);
        runeData.system.AddRunePage(page2);
        
        // 创建默认推荐
        RuneRecommendation rec1 = new RuneRecommendation("rec_guanyu_1", "hero_guanyu", "关羽", "page_physical", "物理输出", "战士", "适合关羽的物理输出铭文");
        RuneRecommendation rec2 = new RuneRecommendation("rec_zhugeliang_1", "hero_zhugeliang", "诸葛亮", "page_magic", "法术输出", "法师", "适合诸葛亮的法术输出铭文");
        
        runeData.system.AddRecommendation(rec1);
        runeData.system.AddRecommendation(rec2);
        
        SaveRuneData();
    }
    
    public void PurchaseRune(string playerID, string runeID)
    {
        Rune rune = runeData.system.GetRune(runeID);
        if (rune != null)
        {
            PlayerRuneData playerData = GetOrCreatePlayerData(playerID);
            
            if (!playerData.OwnsRune(runeID))
            {
                if (ProfileManager.Instance.currentProfile.gold >= rune.price)
                {
                    ProfileManager.Instance.currentProfile.gold -= rune.price;
                    ProfileManager.Instance.SaveProfile();
                    
                    playerData.AddRune(runeID);
                    SaveRuneData();
                    Debug.Log($"成功购买铭文: {rune.runeName}");
                }
                else
                {
                    Debug.Log("金币不足");
                }
            }
            else
            {
                Debug.Log("已经拥有该铭文");
            }
        }
    }
    
    public string CreateRunePage(string playerID, string playerName, string pageName, string pageDescription, string heroID = "")
    {
        string pageID = System.Guid.NewGuid().ToString();
        RunePage page = new RunePage(pageID, pageName, pageDescription, playerID, playerName, heroID);
        runeData.system.AddRunePage(page);
        
        PlayerRuneData playerData = GetOrCreatePlayerData(playerID);
        playerData.AddRunePage(pageID);
        
        SaveRuneData();
        Debug.Log($"成功创建铭文页: {pageName}");
        return pageID;
    }
    
    public void EditRunePage(string pageID, List<string> runeIDs)
    {
        RunePage page = runeData.system.GetRunePage(pageID);
        if (page != null)
        {
            page.runeIDs.Clear();
            foreach (string runeID in runeIDs)
            {
                page.AddRune(runeID);
            }
            SaveRuneData();
            Debug.Log($"成功编辑铭文页: {page.pageName}");
        }
    }
    
    public void DeleteRunePage(string playerID, string pageID)
    {
        RunePage page = runeData.system.GetRunePage(pageID);
        if (page != null)
        {
            PlayerRuneData playerData = GetOrCreatePlayerData(playerID);
            if (playerData.HasRunePage(pageID))
            {
                playerData.RemoveRunePage(pageID);
                runeData.system.runePages.Remove(page);
                SaveRuneData();
                Debug.Log($"成功删除铭文页: {page.pageName}");
            }
        }
    }
    
    public void ShareRunePage(string pageID, bool isPublic)
    {
        RunePage page = runeData.system.GetRunePage(pageID);
        if (page != null)
        {
            page.SetPublic(isPublic);
            SaveRuneData();
            Debug.Log($"成功{(isPublic ? "分享" : "取消分享")}铭文页: {page.pageName}");
        }
    }
    
    public void LikeRunePage(string pageID)
    {
        RunePage page = runeData.system.GetRunePage(pageID);
        if (page != null)
        {
            page.IncrementLikes();
            SaveRuneData();
            Debug.Log($"成功点赞铭文页: {page.pageName}");
        }
    }
    
    public void UseRunePage(string pageID)
    {
        RunePage page = runeData.system.GetRunePage(pageID);
        if (page != null)
        {
            page.IncrementUses();
            SaveRuneData();
        }
    }
    
    public void AddFavoritePage(string playerID, string pageID)
    {
        PlayerRuneData playerData = GetOrCreatePlayerData(playerID);
        playerData.AddFavoritePage(pageID);
        SaveRuneData();
        Debug.Log("成功添加到收藏");
    }
    
    public void RemoveFavoritePage(string playerID, string pageID)
    {
        PlayerRuneData playerData = GetOrCreatePlayerData(playerID);
        playerData.RemoveFavoritePage(pageID);
        SaveRuneData();
        Debug.Log("成功取消收藏");
    }
    
    public List<Rune> GetRunesByType(string runeType)
    {
        return runeData.system.GetRunesByType(runeType);
    }
    
    public List<RunePage> GetPlayerRunePages(string playerID)
    {
        PlayerRuneData playerData = GetOrCreatePlayerData(playerID);
        List<RunePage> playerPages = new List<RunePage>();
        
        foreach (string pageID in playerData.runePages)
        {
            RunePage page = runeData.system.GetRunePage(pageID);
            if (page != null)
            {
                playerPages.Add(page);
            }
        }
        
        return playerPages;
    }
    
    public List<RunePage> GetPublicRunePages()
    {
        return runeData.system.runePages.FindAll(p => p.isPublic);
    }
    
    public List<RunePage> GetFavoritePages(string playerID)
    {
        PlayerRuneData playerData = GetOrCreatePlayerData(playerID);
        List<RunePage> favoritePages = new List<RunePage>();
        
        foreach (string pageID in playerData.favoritePages)
        {
            RunePage page = runeData.system.GetRunePage(pageID);
            if (page != null)
            {
                favoritePages.Add(page);
            }
        }
        
        return favoritePages;
    }
    
    public List<RuneRecommendation> GetRecommendationsByHero(string heroID)
    {
        return runeData.system.GetRecommendationsByHero(heroID);
    }
    
    public Rune GetRune(string runeID)
    {
        return runeData.system.GetRune(runeID);
    }
    
    public RunePage GetRunePage(string pageID)
    {
        return runeData.system.GetRunePage(pageID);
    }
    
    public bool OwnsRune(string playerID, string runeID)
    {
        PlayerRuneData playerData = GetOrCreatePlayerData(playerID);
        return playerData.OwnsRune(runeID);
    }
    
    private PlayerRuneData GetOrCreatePlayerData(string playerID)
    {
        PlayerRuneData playerData = runeData.GetPlayerData(playerID);
        if (playerData == null)
        {
            playerData = new PlayerRuneData(playerID);
            runeData.AddPlayerData(playerData);
        }
        return playerData;
    }
    
    public void SaveRuneData()
    {
        string path = Application.dataPath + "/Data/rune_system_extended_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, runeData);
        stream.Close();
    }
    
    public void LoadRuneData()
    {
        string path = Application.dataPath + "/Data/rune_system_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            runeData = (RuneSystemExtendedData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            runeData = new RuneSystemExtendedData();
        }
    }
}
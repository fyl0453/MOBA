using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PersonalizationSystemDetailedManager : MonoBehaviour
{
    public static PersonalizationSystemDetailedManager Instance { get; private set; }
    
    public PersonalizationSystemDetailedManagerData personalizationData;
    
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
        LoadPersonalizationData();
        
        if (personalizationData == null)
        {
            personalizationData = new PersonalizationSystemDetailedManagerData();
            InitializeDefaultPersonalizationSystem();
        }
    }
    
    private void InitializeDefaultPersonalizationSystem()
    {
        // 头像框
        AvatarFrame frame1 = new AvatarFrame("frame_001", "王者头像框", "rare", "王者荣耀专属头像框", "currency_diamond", 888, "icons/frame_001", "frames/frame_001", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        AvatarFrame frame2 = new AvatarFrame("frame_002", "KPL头像框", "event", "KPL职业联赛专属头像框", "currency_honor", 2000, "icons/frame_002", "frames/frame_002", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        AvatarFrame frame3 = new AvatarFrame("frame_003", "春节头像框", "festival", "春节限定头像框", "currency_diamond", 666, "icons/frame_003", "frames/frame_003", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        AvatarFrame frame4 = new AvatarFrame("frame_004", "新手头像框", "basic", "新手专属头像框", "currency_gold", 1000, "icons/frame_004", "frames/frame_004", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        
        personalizationData.system.AddAvatarFrame(frame1);
        personalizationData.system.AddAvatarFrame(frame2);
        personalizationData.system.AddAvatarFrame(frame3);
        personalizationData.system.AddAvatarFrame(frame4);
        
        // 名片
        BusinessCard card1 = new BusinessCard("card_001", "王者名片", "rare", "王者荣耀专属名片", "currency_diamond", 1288, "icons/card_001", "cards/card_001", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        BusinessCard card2 = new BusinessCard("card_002", "KPL名片", "event", "KPL职业联赛专属名片", "currency_honor", 3000, "icons/card_002", "cards/card_002", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        BusinessCard card3 = new BusinessCard("card_003", "春节名片", "festival", "春节限定名片", "currency_diamond", 888, "icons/card_003", "cards/card_003", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        BusinessCard card4 = new BusinessCard("card_004", "新手名片", "basic", "新手专属名片", "currency_gold", 2000, "icons/card_004", "cards/card_004", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        
        personalizationData.system.AddBusinessCard(card1);
        personalizationData.system.AddBusinessCard(card2);
        personalizationData.system.AddBusinessCard(card3);
        personalizationData.system.AddBusinessCard(card4);
        
        // 个人主页装饰
        ProfileDecoration decoration1 = new ProfileDecoration("decoration_001", "王者背景", "background", "王者荣耀专属背景", "currency_diamond", 1688, "icons/decoration_001", "decorations/decoration_001", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        ProfileDecoration decoration2 = new ProfileDecoration("decoration_002", "KPL背景", "background", "KPL职业联赛专属背景", "currency_honor", 4000, "icons/decoration_002", "decorations/decoration_002", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        ProfileDecoration decoration3 = new ProfileDecoration("decoration_003", "春节背景", "background", "春节限定背景", "currency_diamond", 1288, "icons/decoration_003", "decorations/decoration_003", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        ProfileDecoration decoration4 = new ProfileDecoration("decoration_004", "新手背景", "background", "新手专属背景", "currency_gold", 3000, "icons/decoration_004", "decorations/decoration_004", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        
        personalizationData.system.AddProfileDecoration(decoration1);
        personalizationData.system.AddProfileDecoration(decoration2);
        personalizationData.system.AddProfileDecoration(decoration3);
        personalizationData.system.AddProfileDecoration(decoration4);
        
        // 主题
        Theme theme1 = new Theme("theme_001", "王者主题", "rare", "王者荣耀专属主题", "currency_diamond", 1988, "icons/theme_001", "themes/theme_001", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        Theme theme2 = new Theme("theme_002", "KPL主题", "event", "KPL职业联赛专属主题", "currency_honor", 5000, "icons/theme_002", "themes/theme_002", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        Theme theme3 = new Theme("theme_003", "春节主题", "festival", "春节限定主题", "currency_diamond", 1688, "icons/theme_003", "themes/theme_003", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        Theme theme4 = new Theme("theme_004", "默认主题", "basic", "默认主题", "currency_gold", 0, "icons/theme_004", "themes/theme_004", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        
        personalizationData.system.AddTheme(theme1);
        personalizationData.system.AddTheme(theme2);
        personalizationData.system.AddTheme(theme3);
        personalizationData.system.AddTheme(theme4);
        
        // 边框
        Border border1 = new Border("border_001", "王者边框", "rare", "王者荣耀专属边框", "currency_diamond", 1288, "icons/border_001", "borders/border_001", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        Border border2 = new Border("border_002", "KPL边框", "event", "KPL职业联赛专属边框", "currency_honor", 3000, "icons/border_002", "borders/border_002", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        Border border3 = new Border("border_003", "春节边框", "festival", "春节限定边框", "currency_diamond", 888, "icons/border_003", "borders/border_003", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        Border border4 = new Border("border_004", "默认边框", "basic", "默认边框", "currency_gold", 0, "icons/border_004", "borders/border_004", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        
        personalizationData.system.AddBorder(border1);
        personalizationData.system.AddBorder(border2);
        personalizationData.system.AddBorder(border3);
        personalizationData.system.AddBorder(border4);
        
        // 称号
        Title title1 = new Title("title_001", "王者荣耀", "rare", "王者荣耀专属称号", "currency_diamond", 2888, "icons/title_001", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        Title title2 = new Title("title_002", "KPL冠军", "event", "KPL职业联赛冠军称号", "currency_honor", 10000, "icons/title_002", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        Title title3 = new Title("title_003", "春节使者", "festival", "春节限定称号", "currency_diamond", 1688, "icons/title_003", true, System.DateTime.Now.ToString("yyyy-MM-dd"), System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        Title title4 = new Title("title_004", "新手玩家", "basic", "新手专属称号", "currency_gold", 5000, "icons/title_004", false, System.DateTime.Now.ToString("yyyy-MM-dd"), "2099-12-31");
        
        personalizationData.system.AddTitle(title1);
        personalizationData.system.AddTitle(title2);
        personalizationData.system.AddTitle(title3);
        personalizationData.system.AddTitle(title4);
        
        // 玩家个性化设置
        PlayerPersonalization personalization1 = new PlayerPersonalization("personalization_001", "user_001");
        personalization1.AddAvatarFrame("frame_001");
        personalization1.AddAvatarFrame("frame_004");
        personalization1.AddBusinessCard("card_001");
        personalization1.AddBusinessCard("card_004");
        personalization1.AddProfileDecoration("decoration_001");
        personalization1.AddProfileDecoration("decoration_004");
        personalization1.AddTheme("theme_001");
        personalization1.AddTheme("theme_004");
        personalization1.AddBorder("border_001");
        personalization1.AddBorder("border_004");
        personalization1.AddTitle("title_001");
        personalization1.AddTitle("title_004");
        personalization1.SetCurrentAvatarFrame("frame_001");
        personalization1.SetCurrentBusinessCard("card_001");
        personalization1.SetCurrentTheme("theme_001");
        personalization1.SetCurrentBorder("border_001");
        personalization1.SetCurrentTitle("title_001");
        
        PlayerPersonalization personalization2 = new PlayerPersonalization("personalization_002", "user_002");
        personalization2.AddAvatarFrame("frame_002");
        personalization2.AddAvatarFrame("frame_004");
        personalization2.AddBusinessCard("card_002");
        personalization2.AddBusinessCard("card_004");
        personalization2.AddProfileDecoration("decoration_002");
        personalization2.AddProfileDecoration("decoration_004");
        personalization2.AddTheme("theme_002");
        personalization2.AddTheme("theme_004");
        personalization2.AddBorder("border_002");
        personalization2.AddBorder("border_004");
        personalization2.AddTitle("title_002");
        personalization2.AddTitle("title_004");
        personalization2.SetCurrentAvatarFrame("frame_002");
        personalization2.SetCurrentBusinessCard("card_002");
        personalization2.SetCurrentTheme("theme_002");
        personalization2.SetCurrentBorder("border_002");
        personalization2.SetCurrentTitle("title_002");
        
        PlayerPersonalization personalization3 = new PlayerPersonalization("personalization_003", "user_003");
        personalization3.AddAvatarFrame("frame_003");
        personalization3.AddAvatarFrame("frame_004");
        personalization3.AddBusinessCard("card_003");
        personalization3.AddBusinessCard("card_004");
        personalization3.AddProfileDecoration("decoration_003");
        personalization3.AddProfileDecoration("decoration_004");
        personalization3.AddTheme("theme_003");
        personalization3.AddTheme("theme_004");
        personalization3.AddBorder("border_003");
        personalization3.AddBorder("border_004");
        personalization3.AddTitle("title_003");
        personalization3.AddTitle("title_004");
        personalization3.SetCurrentAvatarFrame("frame_003");
        personalization3.SetCurrentBusinessCard("card_003");
        personalization3.SetCurrentTheme("theme_003");
        personalization3.SetCurrentBorder("border_003");
        personalization3.SetCurrentTitle("title_003");
        
        personalizationData.system.AddPlayerPersonalization(personalization1);
        personalizationData.system.AddPlayerPersonalization(personalization2);
        personalizationData.system.AddPlayerPersonalization(personalization3);
        
        SavePersonalizationData();
    }
    
    // 头像框管理
    public void AddAvatarFrame(string name, string type, string desc, string priceCurrency, float price, string iconPath, string framePath, bool limited, string releaseDate, string expiryDate)
    {
        string frameID = "frame_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        AvatarFrame frame = new AvatarFrame(frameID, name, type, desc, priceCurrency, price, iconPath, framePath, limited, releaseDate, expiryDate);
        personalizationData.system.AddAvatarFrame(frame);
        SavePersonalizationData();
        Debug.Log("成功添加头像框: " + name);
    }
    
    public List<AvatarFrame> GetAvatarFramesByType(string type)
    {
        return personalizationData.system.GetAvatarFramesByType(type);
    }
    
    public List<AvatarFrame> GetAllAvatarFrames()
    {
        return personalizationData.system.avatarFrames;
    }
    
    // 名片管理
    public void AddBusinessCard(string name, string type, string desc, string priceCurrency, float price, string iconPath, string cardPath, bool limited, string releaseDate, string expiryDate)
    {
        string cardID = "card_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        BusinessCard card = new BusinessCard(cardID, name, type, desc, priceCurrency, price, iconPath, cardPath, limited, releaseDate, expiryDate);
        personalizationData.system.AddBusinessCard(card);
        SavePersonalizationData();
        Debug.Log("成功添加名片: " + name);
    }
    
    public List<BusinessCard> GetBusinessCardsByType(string type)
    {
        return personalizationData.system.GetBusinessCardsByType(type);
    }
    
    public List<BusinessCard> GetAllBusinessCards()
    {
        return personalizationData.system.businessCards;
    }
    
    // 个人主页装饰管理
    public void AddProfileDecoration(string name, string type, string desc, string priceCurrency, float price, string iconPath, string decorationPath, bool limited, string releaseDate, string expiryDate)
    {
        string decorationID = "decoration_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        ProfileDecoration decoration = new ProfileDecoration(decorationID, name, type, desc, priceCurrency, price, iconPath, decorationPath, limited, releaseDate, expiryDate);
        personalizationData.system.AddProfileDecoration(decoration);
        SavePersonalizationData();
        Debug.Log("成功添加个人主页装饰: " + name);
    }
    
    public List<ProfileDecoration> GetProfileDecorationsByType(string type)
    {
        return personalizationData.system.GetProfileDecorationsByType(type);
    }
    
    public List<ProfileDecoration> GetAllProfileDecorations()
    {
        return personalizationData.system.profileDecorations;
    }
    
    // 主题管理
    public void AddTheme(string name, string type, string desc, string priceCurrency, float price, string iconPath, string themePath, bool limited, string releaseDate, string expiryDate)
    {
        string themeID = "theme_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Theme theme = new Theme(themeID, name, type, desc, priceCurrency, price, iconPath, themePath, limited, releaseDate, expiryDate);
        personalizationData.system.AddTheme(theme);
        SavePersonalizationData();
        Debug.Log("成功添加主题: " + name);
    }
    
    public List<Theme> GetThemesByType(string type)
    {
        return personalizationData.system.GetThemesByType(type);
    }
    
    public List<Theme> GetAllThemes()
    {
        return personalizationData.system.themes;
    }
    
    // 边框管理
    public void AddBorder(string name, string type, string desc, string priceCurrency, float price, string iconPath, string borderPath, bool limited, string releaseDate, string expiryDate)
    {
        string borderID = "border_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Border border = new Border(borderID, name, type, desc, priceCurrency, price, iconPath, borderPath, limited, releaseDate, expiryDate);
        personalizationData.system.AddBorder(border);
        SavePersonalizationData();
        Debug.Log("成功添加边框: " + name);
    }
    
    public List<Border> GetBordersByType(string type)
    {
        return personalizationData.system.GetBordersByType(type);
    }
    
    public List<Border> GetAllBorders()
    {
        return personalizationData.system.borders;
    }
    
    // 称号管理
    public void AddTitle(string name, string type, string desc, string priceCurrency, float price, string iconPath, bool limited, string releaseDate, string expiryDate)
    {
        string titleID = "title_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Title title = new Title(titleID, name, type, desc, priceCurrency, price, iconPath, limited, releaseDate, expiryDate);
        personalizationData.system.AddTitle(title);
        SavePersonalizationData();
        Debug.Log("成功添加称号: " + name);
    }
    
    public List<Title> GetTitlesByType(string type)
    {
        return personalizationData.system.GetTitlesByType(type);
    }
    
    public List<Title> GetAllTitles()
    {
        return personalizationData.system.titles;
    }
    
    // 玩家个性化管理
    public void AddPlayerPersonalization(string playerID)
    {
        PlayerPersonalization existing = personalizationData.system.GetPlayerPersonalization(playerID);
        if (existing == null)
        {
            string personalizationID = "personalization_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            PlayerPersonalization personalization = new PlayerPersonalization(personalizationID, playerID);
            personalizationData.system.AddPlayerPersonalization(personalization);
            SavePersonalizationData();
            Debug.Log("成功创建玩家个性化设置: " + playerID);
        }
    }
    
    public void AddAvatarFrameToPlayer(string playerID, string frameID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.AddAvatarFrame(frameID);
            SavePersonalizationData();
            Debug.Log("成功为玩家添加头像框: " + frameID);
        }
        else
        {
            AddPlayerPersonalization(playerID);
            AddAvatarFrameToPlayer(playerID, frameID);
        }
    }
    
    public void AddBusinessCardToPlayer(string playerID, string cardID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.AddBusinessCard(cardID);
            SavePersonalizationData();
            Debug.Log("成功为玩家添加名片: " + cardID);
        }
        else
        {
            AddPlayerPersonalization(playerID);
            AddBusinessCardToPlayer(playerID, cardID);
        }
    }
    
    public void AddProfileDecorationToPlayer(string playerID, string decorationID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.AddProfileDecoration(decorationID);
            SavePersonalizationData();
            Debug.Log("成功为玩家添加个人主页装饰: " + decorationID);
        }
        else
        {
            AddPlayerPersonalization(playerID);
            AddProfileDecorationToPlayer(playerID, decorationID);
        }
    }
    
    public void AddThemeToPlayer(string playerID, string themeID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.AddTheme(themeID);
            SavePersonalizationData();
            Debug.Log("成功为玩家添加主题: " + themeID);
        }
        else
        {
            AddPlayerPersonalization(playerID);
            AddThemeToPlayer(playerID, themeID);
        }
    }
    
    public void AddBorderToPlayer(string playerID, string borderID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.AddBorder(borderID);
            SavePersonalizationData();
            Debug.Log("成功为玩家添加边框: " + borderID);
        }
        else
        {
            AddPlayerPersonalization(playerID);
            AddBorderToPlayer(playerID, borderID);
        }
    }
    
    public void AddTitleToPlayer(string playerID, string titleID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.AddTitle(titleID);
            SavePersonalizationData();
            Debug.Log("成功为玩家添加称号: " + titleID);
        }
        else
        {
            AddPlayerPersonalization(playerID);
            AddTitleToPlayer(playerID, titleID);
        }
    }
    
    public void SetPlayerAvatarFrame(string playerID, string frameID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.SetCurrentAvatarFrame(frameID);
            SavePersonalizationData();
            Debug.Log("成功设置玩家当前头像框: " + frameID);
        }
        else
        {
            Debug.LogError("玩家个性化设置不存在: " + playerID);
        }
    }
    
    public void SetPlayerBusinessCard(string playerID, string cardID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.SetCurrentBusinessCard(cardID);
            SavePersonalizationData();
            Debug.Log("成功设置玩家当前名片: " + cardID);
        }
        else
        {
            Debug.LogError("玩家个性化设置不存在: " + playerID);
        }
    }
    
    public void SetPlayerTheme(string playerID, string themeID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.SetCurrentTheme(themeID);
            SavePersonalizationData();
            Debug.Log("成功设置玩家当前主题: " + themeID);
        }
        else
        {
            Debug.LogError("玩家个性化设置不存在: " + playerID);
        }
    }
    
    public void SetPlayerBorder(string playerID, string borderID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.SetCurrentBorder(borderID);
            SavePersonalizationData();
            Debug.Log("成功设置玩家当前边框: " + borderID);
        }
        else
        {
            Debug.LogError("玩家个性化设置不存在: " + playerID);
        }
    }
    
    public void SetPlayerTitle(string playerID, string titleID)
    {
        PlayerPersonalization personalization = personalizationData.system.GetPlayerPersonalization(playerID);
        if (personalization != null)
        {
            personalization.SetCurrentTitle(titleID);
            SavePersonalizationData();
            Debug.Log("成功设置玩家当前称号: " + titleID);
        }
        else
        {
            Debug.LogError("玩家个性化设置不存在: " + playerID);
        }
    }
    
    public PlayerPersonalization GetPlayerPersonalization(string playerID)
    {
        return personalizationData.system.GetPlayerPersonalization(playerID);
    }
    
    // 数据持久化
    public void SavePersonalizationData()
    {
        string path = Application.dataPath + "/Data/personalization_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, personalizationData);
        stream.Close();
    }
    
    public void LoadPersonalizationData()
    {
        string path = Application.dataPath + "/Data/personalization_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            personalizationData = (PersonalizationSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            personalizationData = new PersonalizationSystemDetailedManagerData();
        }
    }
}
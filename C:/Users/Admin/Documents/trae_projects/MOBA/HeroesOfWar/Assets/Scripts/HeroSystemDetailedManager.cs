using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HeroSystemDetailedManager : MonoBehaviour
{
    public static HeroSystemDetailedManager Instance { get; private set; }
    
    public HeroSystemDetailedManagerData heroData;
    
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
        LoadHeroData();
        
        if (heroData == null)
        {
            heroData = new HeroSystemDetailedManagerData();
            InitializeDefaultHeroSystem();
        }
    }
    
    private void InitializeDefaultHeroSystem()
    {
        // 英雄
        Hero hero1 = new Hero("hero_001", "李白", "青莲剑仙，擅长突进和爆发", "assassin", "Hard", "currency_diamond", 18888, 188, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "models/hero_001", "icons/hero_001");
        Hero hero2 = new Hero("hero_002", "诸葛亮", "卧龙先生，擅长法术伤害和控制", "mage", "Medium", "currency_diamond", 18888, 188, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "models/hero_002", "icons/hero_002");
        Hero hero3 = new Hero("hero_003", "关羽", "武圣，擅长冲锋和控制", "warrior", "Hard", "currency_gold", 13888, 138, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "models/hero_003", "icons/hero_003");
        Hero hero4 = new Hero("hero_004", "后羿", "神箭手，擅长远程输出", "marksman", "Easy", "currency_gold", 8888, 88, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "models/hero_004", "icons/hero_004");
        Hero hero5 = new Hero("hero_005", "张飞", "燕人张翼德，擅长坦克和控制", "tank", "Medium", "currency_gold", 13888, 138, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "models/hero_005", "icons/hero_005");
        
        heroData.system.AddHero(hero1);
        heroData.system.AddHero(hero2);
        heroData.system.AddHero(hero3);
        heroData.system.AddHero(hero4);
        heroData.system.AddHero(hero5);
        
        // 技能
        HeroSkill skill1_1 = new HeroSkill("skill_001", "hero_001", "将进酒", "向前突进并造成伤害", "normal", 10, 100, 5, 5);
        skill1_1.AddEffect(new SkillEffect("effect_001", "伤害", "damage", "对敌人造成物理伤害", 100, 0));
        skill1_1.AddEffect(new SkillEffect("effect_002", "位移", "movement", "向前突进一段距离", 0, 0));
        skill1_1.AddUpgrade(new SkillUpgrade("upgrade_001", 2, 0.5f, 20, 0, "冷却时间减少0.5秒，伤害增加20"));
        
        HeroSkill skill1_2 = new HeroSkill("skill_002", "hero_001", "神来之笔", "画出剑阵并造成伤害", "normal", 8, 150, 8, 5);
        skill1_2.AddEffect(new SkillEffect("effect_003", "伤害", "damage", "对敌人造成物理伤害", 150, 0));
        skill1_2.AddEffect(new SkillEffect("effect_004", "减速", "slow", "减慢敌人移动速度", 0.5f, 2));
        
        HeroSkill skill1_3 = new HeroSkill("skill_003", "hero_001", "青莲剑歌", "连续攻击并无法被选中", "ultimate", 40, 200, 10, 3);
        skill1_3.AddEffect(new SkillEffect("effect_005", "伤害", "damage", "对敌人造成物理伤害", 200, 0));
        skill1_3.AddEffect(new SkillEffect("effect_006", "无敌", "invulnerable", "短暂无敌状态", 0, 2));
        
        HeroSkill skill2_1 = new HeroSkill("skill_004", "hero_002", "东风破袭", "发射能量球并造成伤害", "normal", 6, 80, 10, 5);
        skill2_1.AddEffect(new SkillEffect("effect_007", "伤害", "damage", "对敌人造成法术伤害", 80, 0));
        
        HeroSkill skill2_2 = new HeroSkill("skill_005", "hero_002", "时空穿梭", "闪烁到指定位置", "normal", 12, 0, 5, 5);
        skill2_2.AddEffect(new SkillEffect("effect_008", "位移", "movement", "瞬间移动到指定位置", 0, 0));
        
        HeroSkill skill2_3 = new HeroSkill("skill_006", "hero_002", "元气弹", "蓄力后发射强力能量球", "ultimate", 35, 300, 15, 3);
        skill2_3.AddEffect(new SkillEffect("effect_009", "伤害", "damage", "对敌人造成大量法术伤害", 300, 0));
        
        heroData.system.AddHeroSkill(skill1_1);
        heroData.system.AddHeroSkill(skill1_2);
        heroData.system.AddHeroSkill(skill1_3);
        heroData.system.AddHeroSkill(skill2_1);
        heroData.system.AddHeroSkill(skill2_2);
        heroData.system.AddHeroSkill(skill2_3);
        
        // 添加技能到英雄
        hero1.AddSkill("skill_001");
        hero1.AddSkill("skill_002");
        hero1.AddSkill("skill_003");
        hero2.AddSkill("skill_004");
        hero2.AddSkill("skill_005");
        hero2.AddSkill("skill_006");
        
        // 属性
        HeroAttribute attr1_1 = new HeroAttribute("attr_001", "hero_001", "攻击力", "attack", 170, 10, "英雄的基础攻击力");
        HeroAttribute attr1_2 = new HeroAttribute("attr_002", "hero_001", "生命值", "health", 3200, 200, "英雄的基础生命值");
        HeroAttribute attr1_3 = new HeroAttribute("attr_003", "hero_001", "物理防御", "defense", 50, 5, "英雄的基础物理防御");
        
        HeroAttribute attr2_1 = new HeroAttribute("attr_004", "hero_002", "法术强度", "magic", 160, 12, "英雄的基础法术强度");
        HeroAttribute attr2_2 = new HeroAttribute("attr_005", "hero_002", "生命值", "health", 2800, 180, "英雄的基础生命值");
        HeroAttribute attr2_3 = new HeroAttribute("attr_006", "hero_002", "法术防御", "magic_defense", 60, 6, "英雄的基础法术防御");
        
        heroData.system.AddHeroAttribute(attr1_1);
        heroData.system.AddHeroAttribute(attr1_2);
        heroData.system.AddHeroAttribute(attr1_3);
        heroData.system.AddHeroAttribute(attr2_1);
        heroData.system.AddHeroAttribute(attr2_2);
        heroData.system.AddHeroAttribute(attr2_3);
        
        // 添加属性到英雄
        hero1.AddAttribute("attr_001");
        hero1.AddAttribute("attr_002");
        hero1.AddAttribute("attr_003");
        hero2.AddAttribute("attr_004");
        hero2.AddAttribute("attr_005");
        hero2.AddAttribute("attr_006");
        
        // 玩家英雄
        PlayerHero playerHero1 = new PlayerHero("user_001", "hero_001", true);
        PlayerHero playerHero2 = new PlayerHero("user_001", "hero_002", true);
        PlayerHero playerHero3 = new PlayerHero("user_001", "hero_003", false);
        PlayerHero playerHero4 = new PlayerHero("user_002", "hero_001", false);
        PlayerHero playerHero5 = new PlayerHero("user_002", "hero_004", true);
        
        heroData.system.AddPlayerHero(playerHero1);
        heroData.system.AddPlayerHero(playerHero2);
        heroData.system.AddPlayerHero(playerHero3);
        heroData.system.AddPlayerHero(playerHero4);
        heroData.system.AddPlayerHero(playerHero5);
        
        // 英雄熟练度
        HeroMastery mastery1 = new HeroMastery("user_001", "hero_001");
        mastery1.AddMasteryPoints(500);
        mastery1.AddGame(true);
        mastery1.AddGame(true);
        mastery1.AddGame(false);
        
        HeroMastery mastery2 = new HeroMastery("user_001", "hero_002");
        mastery2.AddMasteryPoints(300);
        mastery2.AddGame(true);
        mastery2.AddGame(false);
        
        heroData.system.AddHeroMastery(mastery1);
        heroData.system.AddHeroMastery(mastery2);
        
        // 英雄推荐
        HeroRecommendation rec1 = new HeroRecommendation("rec_001", "hero_001", "build", "李白推荐出装", "适合李白的装备推荐");
        rec1.AddItem("装备1");
        rec1.AddItem("装备2");
        rec1.AddItem("装备3");
        rec1.AddSkill("将进酒");
        rec1.AddSkill("神来之笔");
        rec1.AddSkill("青莲剑歌");
        rec1.AddTip("李白需要利用技能的位移来躲避敌人的攻击");
        rec1.AddTip("青莲剑歌可以用来躲避关键技能");
        
        HeroRecommendation rec2 = new HeroRecommendation("rec_002", "hero_002", "build", "诸葛亮推荐出装", "适合诸葛亮的装备推荐");
        rec2.AddItem("装备4");
        rec2.AddItem("装备5");
        rec2.AddItem("装备6");
        rec2.AddSkill("东风破袭");
        rec2.AddSkill("时空穿梭");
        rec2.AddSkill("元气弹");
        rec2.AddTip("诸葛亮的时空穿梭可以用来逃命或追杀");
        rec2.AddTip("元气弹需要蓄力才能发挥最大伤害");
        
        heroData.system.AddHeroRecommendation(rec1);
        heroData.system.AddHeroRecommendation(rec2);
        
        SaveHeroData();
    }
    
    // 英雄管理
    public void AddHero(string name, string desc, string type, string difficulty, string priceCurrency, float price, int fragmentCost, string releaseDate, string modelPath, string iconPath)
    {
        string heroID = "hero_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        Hero hero = new Hero(heroID, name, desc, type, difficulty, priceCurrency, price, fragmentCost, releaseDate, modelPath, iconPath);
        heroData.system.AddHero(hero);
        SaveHeroData();
        Debug.Log("成功添加英雄: " + name);
    }
    
    public List<Hero> GetHeroesByType(string type)
    {
        return heroData.system.GetHeroesByType(type);
    }
    
    public List<Hero> GetHeroesByDifficulty(string difficulty)
    {
        return heroData.system.GetHeroesByDifficulty(difficulty);
    }
    
    public List<Hero> GetAllHeroes()
    {
        return heroData.system.heroes;
    }
    
    // 技能管理
    public void AddHeroSkill(string heroID, string name, string desc, string type, float cooldown, float damage, float range, int maxLevel)
    {
        string skillID = "skill_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HeroSkill skill = new HeroSkill(skillID, heroID, name, desc, type, cooldown, damage, range, maxLevel);
        heroData.system.AddHeroSkill(skill);
        
        Hero hero = heroData.system.GetHero(heroID);
        if (hero != null)
        {
            hero.AddSkill(skillID);
        }
        
        SaveHeroData();
        Debug.Log("成功添加英雄技能: " + name);
    }
    
    public void AddSkillEffect(string skillID, string name, string type, string desc, float value, float duration)
    {
        HeroSkill skill = heroData.system.GetHeroSkill(skillID);
        if (skill != null)
        {
            string effectID = "effect_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            SkillEffect effect = new SkillEffect(effectID, name, type, desc, value, duration);
            skill.AddEffect(effect);
            SaveHeroData();
            Debug.Log("成功添加技能特效: " + name);
        }
        else
        {
            Debug.LogError("技能不存在: " + skillID);
        }
    }
    
    public void AddSkillUpgrade(string skillID, int level, float cooldownReduction, float damageIncrease, float rangeIncrease, string desc)
    {
        HeroSkill skill = heroData.system.GetHeroSkill(skillID);
        if (skill != null)
        {
            string upgradeID = "upgrade_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            SkillUpgrade upgrade = new SkillUpgrade(upgradeID, level, cooldownReduction, damageIncrease, rangeIncrease, desc);
            skill.AddUpgrade(upgrade);
            SaveHeroData();
            Debug.Log("成功添加技能升级: " + level + "级");
        }
        else
        {
            Debug.LogError("技能不存在: " + skillID);
        }
    }
    
    // 属性管理
    public void AddHeroAttribute(string heroID, string name, string type, float baseValue, float growthValue, string desc)
    {
        string attributeID = "attr_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HeroAttribute attribute = new HeroAttribute(attributeID, heroID, name, type, baseValue, growthValue, desc);
        heroData.system.AddHeroAttribute(attribute);
        
        Hero hero = heroData.system.GetHero(heroID);
        if (hero != null)
        {
            hero.AddAttribute(attributeID);
        }
        
        SaveHeroData();
        Debug.Log("成功添加英雄属性: " + name);
    }
    
    // 玩家英雄管理
    public void AddPlayerHero(string playerID, string heroID, bool owned = false)
    {
        PlayerHero existing = heroData.system.GetPlayerHero(playerID, heroID);
        if (existing == null)
        {
            PlayerHero playerHero = new PlayerHero(playerID, heroID, owned);
            heroData.system.AddPlayerHero(playerHero);
            SaveHeroData();
            Debug.Log("成功添加玩家英雄: " + heroID);
        }
        else
        {
            Debug.Log("玩家已拥有该英雄: " + heroID);
        }
    }
    
    public void SetPlayerHeroOwned(string playerID, string heroID, bool owned)
    {
        PlayerHero playerHero = heroData.system.GetPlayerHero(playerID, heroID);
        if (playerHero != null)
        {
            playerHero.SetOwned(owned);
            SaveHeroData();
            Debug.Log("成功设置英雄拥有状态: " + owned);
        }
        else
        {
            AddPlayerHero(playerID, heroID, owned);
        }
    }
    
    public void AddHeroExperience(string playerID, string heroID, int exp)
    {
        PlayerHero playerHero = heroData.system.GetPlayerHero(playerID, heroID);
        if (playerHero != null)
        {
            playerHero.AddExperience(exp);
            SaveHeroData();
            Debug.Log("成功添加英雄经验: " + exp);
        }
        else
        {
            Debug.LogError("玩家未拥有该英雄: " + heroID);
        }
    }
    
    public List<PlayerHero> GetPlayerHeroes(string playerID)
    {
        return heroData.system.GetPlayerHeroes(playerID);
    }
    
    // 英雄熟练度管理
    public void AddHeroMastery(string playerID, string heroID)
    {
        HeroMastery existing = heroData.system.GetHeroMastery(playerID, heroID);
        if (existing == null)
        {
            HeroMastery mastery = new HeroMastery(playerID, heroID);
            heroData.system.AddHeroMastery(mastery);
            SaveHeroData();
            Debug.Log("成功添加英雄熟练度: " + heroID);
        }
    }
    
    public void AddMasteryPoints(string playerID, string heroID, int points)
    {
        HeroMastery mastery = heroData.system.GetHeroMastery(playerID, heroID);
        if (mastery != null)
        {
            mastery.AddMasteryPoints(points);
            SaveHeroData();
            Debug.Log("成功添加熟练度点数: " + points);
        }
        else
        {
            AddHeroMastery(playerID, heroID);
            AddMasteryPoints(playerID, heroID, points);
        }
    }
    
    public void RecordHeroGame(string playerID, string heroID, bool won)
    {
        HeroMastery mastery = heroData.system.GetHeroMastery(playerID, heroID);
        if (mastery != null)
        {
            mastery.AddGame(won);
            SaveHeroData();
            Debug.Log("成功记录英雄游戏: " + (won ? "胜利" : "失败"));
        }
        else
        {
            AddHeroMastery(playerID, heroID);
            RecordHeroGame(playerID, heroID, won);
        }
    }
    
    public List<HeroMastery> GetHeroMasteries(string playerID)
    {
        return heroData.system.GetHeroMasteries(playerID);
    }
    
    // 英雄推荐管理
    public void AddHeroRecommendation(string heroID, string type, string title, string desc)
    {
        string recommendationID = "rec_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        HeroRecommendation recommendation = new HeroRecommendation(recommendationID, heroID, type, title, desc);
        heroData.system.AddHeroRecommendation(recommendation);
        SaveHeroData();
        Debug.Log("成功添加英雄推荐: " + title);
    }
    
    public void AddItemToRecommendation(string recommendationID, string item)
    {
        HeroRecommendation recommendation = heroData.system.GetHeroRecommendation(recommendationID);
        if (recommendation != null)
        {
            recommendation.AddItem(item);
            SaveHeroData();
            Debug.Log("成功添加推荐装备: " + item);
        }
        else
        {
            Debug.LogError("推荐不存在: " + recommendationID);
        }
    }
    
    public void AddSkillToRecommendation(string recommendationID, string skill)
    {
        HeroRecommendation recommendation = heroData.system.GetHeroRecommendation(recommendationID);
        if (recommendation != null)
        {
            recommendation.AddSkill(skill);
            SaveHeroData();
            Debug.Log("成功添加推荐技能: " + skill);
        }
        else
        {
            Debug.LogError("推荐不存在: " + recommendationID);
        }
    }
    
    public void AddTipToRecommendation(string recommendationID, string tip)
    {
        HeroRecommendation recommendation = heroData.system.GetHeroRecommendation(recommendationID);
        if (recommendation != null)
        {
            recommendation.AddTip(tip);
            SaveHeroData();
            Debug.Log("成功添加推荐技巧: " + tip);
        }
        else
        {
            Debug.LogError("推荐不存在: " + recommendationID);
        }
    }
    
    public List<HeroRecommendation> GetRecommendationsByHero(string heroID)
    {
        return heroData.system.GetRecommendationsByHero(heroID);
    }
    
    // 数据持久化
    public void SaveHeroData()
    {
        string path = Application.dataPath + "/Data/hero_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, heroData);
        stream.Close();
    }
    
    public void LoadHeroData()
    {
        string path = Application.dataPath + "/Data/hero_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            heroData = (HeroSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            heroData = new HeroSystemDetailedManagerData();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HeroManagerExtended : MonoBehaviour
{
    public static HeroManagerExtended Instance { get; private set; }
    
    public HeroManagerExtendedData heroData;
    
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
            heroData = new HeroManagerExtendedData();
            InitializeDefaultHeroes();
        }
    }
    
    private void InitializeDefaultHeroes()
    {
        // 关羽 - 战士
        Hero guanyu = new Hero("hero_guanyu", "关羽", "战士", "近战", "物理", 1800, 1800);
        
        // 基础属性
        guanyu.AddAttribute(new HeroAttribute("Attack", "攻击力", "物理攻击", 175, 10.5f));
        guanyu.AddAttribute(new HeroAttribute("Health", "生命值", "最大生命值", 3500, 280f));
        guanyu.AddAttribute(new HeroAttribute("Armor", "物理防御", "物理防御", 100, 5f));
        guanyu.AddAttribute(new HeroAttribute("MagicResist", "法术防御", "法术防御", 80, 3.5f));
        guanyu.AddAttribute(new HeroAttribute("AttackSpeed", "攻击速度", "攻击速度", 0.8f, 0.015f));
        guanyu.AddAttribute(new HeroAttribute("MovementSpeed", "移动速度", "移动速度", 400, 0f));
        
        // 技能
        HeroSkill skill1 = new HeroSkill("skill_guanyu_1", "青龙偃月", "向前劈砍，对敌人造成伤害", "Active", 5f, 50f);
        skill1.AddEffect("Damage", "造成物理伤害", 200f);
        skill1.AddEffect("Slow", "减少目标移动速度", 30f);
        skill1.AddUpgrade(2, "伤害提升20%");
        skill1.AddUpgrade(3, "冷却时间减少1秒");
        skill1.AddUpgrade(4, "伤害提升20%");
        skill1.AddUpgrade(5, "冷却时间减少1秒");
        guanyu.AddSkill(skill1);
        
        HeroSkill skill2 = new HeroSkill("skill_guanyu_2", "铁骑冲锋", "向前冲锋，击飞敌人", "Active", 10f, 80f);
        skill2.AddEffect("Damage", "造成物理伤害", 300f);
        skill2.AddEffect("Knockup", "击飞敌人", 1f);
        skill2.AddUpgrade(2, "伤害提升20%");
        skill2.AddUpgrade(3, "冷却时间减少1秒");
        skill2.AddUpgrade(4, "伤害提升20%");
        skill2.AddUpgrade(5, "冷却时间减少1秒");
        guanyu.AddSkill(skill2);
        
        HeroSkill ult = new HeroSkill("skill_guanyu_ult", "威震华夏", "大范围伤害和控制", "Ultimate", 40f, 120f);
        ult.AddEffect("Damage", "造成物理伤害", 500f);
        ult.AddEffect("Stun", "眩晕敌人", 1.5f);
        ult.AddEffect("Slow", "减少敌人移动速度", 50f);
        ult.AddUpgrade(2, "伤害提升20%");
        ult.AddUpgrade(3, "冷却时间减少5秒");
        ult.AddUpgrade(4, "伤害提升20%");
        ult.AddUpgrade(5, "冷却时间减少5秒");
        guanyu.AddSkill(ult);
        
        heroData.AddHero(guanyu);
        
        // 诸葛亮 - 法师
        Hero zhugeliang = new Hero("hero_zhugeliang", "诸葛亮", "法师", "远程", "法术", 1200, 1200);
        
        // 基础属性
        zhugeliang.AddAttribute(new HeroAttribute("AP", "法术强度", "法术攻击", 100, 8f));
        zhugeliang.AddAttribute(new HeroAttribute("Health", "生命值", "最大生命值", 2800, 200f));
        zhugeliang.AddAttribute(new HeroAttribute("Armor", "物理防御", "物理防御", 60, 3f));
        zhugeliang.AddAttribute(new HeroAttribute("MagicResist", "法术防御", "法术防御", 100, 5f));
        zhugeliang.AddAttribute(new HeroAttribute("AttackSpeed", "攻击速度", "攻击速度", 0.6f, 0.01f));
        zhugeliang.AddAttribute(new HeroAttribute("MovementSpeed", "移动速度", "移动速度", 380, 0f));
        
        // 技能
        HeroSkill skill1_zg = new HeroSkill("skill_zhugeliang_1", "东风破袭", "发射东风，对敌人造成伤害", "Active", 3f, 40f);
        skill1_zg.AddEffect("Damage", "造成法术伤害", 180f);
        skill1_zg.AddEffect("Pierce", "穿透敌人", 0f);
        skill1_zg.AddUpgrade(2, "伤害提升20%");
        skill1_zg.AddUpgrade(3, "冷却时间减少0.5秒");
        skill1_zg.AddUpgrade(4, "伤害提升20%");
        skill1_zg.AddUpgrade(5, "冷却时间减少0.5秒");
        zhugeliang.AddSkill(skill1_zg);
        
        HeroSkill skill2_zg = new HeroSkill("skill_zhugeliang_2", "时空穿梭", "短暂位移并留下印记", "Active", 8f, 60f);
        skill2_zg.AddEffect("Damage", "造成法术伤害", 250f);
        skill2_zg.AddEffect("Dash", "短暂位移", 0f);
        skill2_zg.AddUpgrade(2, "伤害提升20%");
        skill2_zg.AddUpgrade(3, "冷却时间减少1秒");
        skill2_zg.AddUpgrade(4, "伤害提升20%");
        skill2_zg.AddUpgrade(5, "冷却时间减少1秒");
        zhugeliang.AddSkill(skill2_zg);
        
        HeroSkill ult_zg = new HeroSkill("skill_zhugeliang_ult", "元气弹", "蓄力发射元气弹", "Ultimate", 35f, 100f);
        ult_zg.AddEffect("Damage", "造成法术伤害", 800f);
        ult_zg.AddEffect("Slow", "减少敌人移动速度", 60f);
        ult_zg.AddEffect("Execution", "基于目标已损失生命值增加伤害", 0f);
        ult_zg.AddUpgrade(2, "伤害提升20%");
        ult_zg.AddUpgrade(3, "冷却时间减少5秒");
        ult_zg.AddUpgrade(4, "伤害提升20%");
        ult_zg.AddUpgrade(5, "冷却时间减少5秒");
        zhugeliang.AddSkill(ult_zg);
        
        heroData.AddHero(zhugeliang);
        
        // 皮肤
        HeroSkin guanyuSkin = new HeroSkin("skin_guanyu_spring", "关羽-新春限定", "新春限定皮肤", 888, "Gems");
        guanyuSkin.AddEffect("ModelChange", "新春主题模型");
        guanyuSkin.AddEffect("TextureChange", "新春主题纹理");
        guanyuSkin.AddEffect("SkillEffect", "新春主题技能特效");
        guanyuSkin.isLimited = true;
        guanyuSkin.limitedTime = "2024-01-01 to 2024-02-01";
        heroData.AddSkin(guanyuSkin);
        
        HeroSkin zhugeliangSkin = new HeroSkin("skin_zhugeliang_dragon", "诸葛亮-龙胆亮银", "龙胆亮银皮肤", 1688, "Gems");
        zhugeliangSkin.AddEffect("ModelChange", "龙胆亮银主题模型");
        zhugeliangSkin.AddEffect("TextureChange", "龙胆亮银主题纹理");
        zhugeliangSkin.AddEffect("SkillEffect", "龙胆亮银主题技能特效");
        heroData.AddSkin(zhugeliangSkin);
        
        // 语音
        HeroVoice guanyuVoice = new HeroVoice("voice_guanyu", "关羽语音", "关羽的语音台词");
        guanyuVoice.AddVoiceLine("关羽在此！");
        guanyuVoice.AddVoiceLine("一夫当关，万夫莫开！");
        guanyuVoice.AddVoiceLine("青龙偃月刀！");
        guanyuVoice.AddVoiceLine("威震华夏！");
        heroData.AddVoice(guanyuVoice);
        
        HeroVoice zhugeliangVoice = new HeroVoice("voice_zhugeliang", "诸葛亮语音", "诸葛亮的语音台词");
        zhugeliangVoice.AddVoiceLine("运筹帷幄之中，决胜千里之外！");
        zhugeliangVoice.AddVoiceLine("东风破袭！");
        zhugeliangVoice.AddVoiceLine("时空穿梭！");
        zhugeliangVoice.AddVoiceLine("元气弹！");
        heroData.AddVoice(zhugeliangVoice);
        
        SaveHeroData();
    }
    
    public Hero GetHero(string heroID)
    {
        return heroData.GetHero(heroID);
    }
    
    public List<Hero> GetHeroesByType(string type)
    {
        return heroData.GetHeroesByType(type);
    }
    
    public List<Hero> GetAllHeroes()
    {
        return heroData.heroes;
    }
    
    public HeroSkin GetSkin(string skinID)
    {
        return heroData.GetSkin(skinID);
    }
    
    public List<HeroSkin> GetSkinsByHero(string heroID)
    {
        return heroData.GetSkinsByHero(heroID);
    }
    
    public HeroVoice GetVoice(string voiceID)
    {
        return heroData.GetVoice(voiceID);
    }
    
    public List<HeroVoice> GetVoicesByHero(string heroID)
    {
        return heroData.GetVoicesByHero(heroID);
    }
    
    public float GetHeroAttributeAtLevel(string heroID, string attributeID, int level)
    {
        Hero hero = GetHero(heroID);
        if (hero != null)
        {
            HeroAttribute attribute = hero.attributes.Find(a => a.attributeID == attributeID);
            if (attribute != null)
            {
                return attribute.GetValueAtLevel(level);
            }
        }
        return 0f;
    }
    
    public List<Hero> GetRecommendedHeroes(string playerLevel)
    {
        List<Hero> recommended = new List<Hero>();
        int level = int.Parse(playerLevel);
        
        if (level < 10)
        {
            // 推荐简单英雄
            recommended.Add(GetHero("hero_guanyu"));
        }
        else if (level < 20)
        {
            // 推荐中等难度英雄
            recommended.Add(GetHero("hero_zhugeliang"));
        }
        else
        {
            // 推荐高难度英雄
            recommended.Add(GetHero("hero_guanyu"));
            recommended.Add(GetHero("hero_zhugeliang"));
        }
        
        return recommended;
    }
    
    public void SaveHeroData()
    {
        string path = Application.dataPath + "/Data/hero_extended_data.dat";
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
        string path = Application.dataPath + "/Data/hero_extended_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            heroData = (HeroManagerExtendedData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            heroData = new HeroManagerExtendedData();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    public static RuneManager Instance { get; private set; }
    
    private List<Rune> allRunes = new List<Rune>();
    private List<RunePage> runePages = new List<RunePage>();
    private RunePage currentPage;
    
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
        InitializeRunes();
        CreateDefaultRunePage();
    }
    
    private void InitializeRunes()
    {
        // 创建所有符文
        // 攻击符文
        for (int i = 0; i < 3; i++)
        {
            allRunes.Add(new AttackDamageRune((Rune.RuneTier)i));
            allRunes.Add(new AttackSpeedRune((Rune.RuneTier)i));
        }
        
        // 法术强度符文
        for (int i = 0; i < 3; i++)
        {
            allRunes.Add(new AbilityPowerRune((Rune.RuneTier)i));
        }
        
        // 防御符文
        for (int i = 0; i < 3; i++)
        {
            allRunes.Add(new ArmorRune((Rune.RuneTier)i));
            allRunes.Add(new MagicResistRune((Rune.RuneTier)i));
            allRunes.Add(new HealthRune((Rune.RuneTier)i));
        }
        
        // 通用符文
        for (int i = 0; i < 3; i++)
        {
            allRunes.Add(new MovementSpeedRune((Rune.RuneTier)i));
            allRunes.Add(new CooldownReductionRune((Rune.RuneTier)i));
            allRunes.Add(new LifeStealRune((Rune.RuneTier)i));
        }
    }
    
    private void CreateDefaultRunePage()
    {
        RunePage defaultPage = new RunePage("默认符文页");
        
        // 添加攻击符文
        for (int i = 0; i < 9; i++)
        {
            defaultPage.marks.Add(new AttackDamageRune(Rune.RuneTier.Tier3));
        }
        
        // 添加防御符文
        for (int i = 0; i < 9; i++)
        {
            defaultPage.seals.Add(new ArmorRune(Rune.RuneTier.Tier3));
        }
        
        // 添加法术强度符文
        for (int i = 0; i < 9; i++)
        {
            defaultPage.glyphs.Add(new AbilityPowerRune(Rune.RuneTier.Tier3));
        }
        
        // 添加移动速度符文
        for (int i = 0; i < 3; i++)
        {
            defaultPage.quintessences.Add(new MovementSpeedRune(Rune.RuneTier.Tier3));
        }
        
        runePages.Add(defaultPage);
        currentPage = defaultPage;
    }
    
    public void SetCurrentPage(RunePage page)
    {
        if (currentPage != null)
        {
            // 移除当前符文页的效果
        }
        
        currentPage = page;
        
        if (currentPage != null)
        {
            // 应用新符文页的效果
        }
    }
    
    public RunePage GetCurrentPage()
    {
        return currentPage;
    }
    
    public List<RunePage> GetAllPages()
    {
        return runePages;
    }
    
    public List<Rune> GetAllRunes()
    {
        return allRunes;
    }
    
    public void CreateNewPage(string name)
    {
        RunePage newPage = new RunePage(name);
        runePages.Add(newPage);
    }
}
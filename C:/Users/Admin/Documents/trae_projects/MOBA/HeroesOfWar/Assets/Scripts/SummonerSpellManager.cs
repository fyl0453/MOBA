using System.Collections.Generic;
using UnityEngine;

public class SummonerSpellManager : MonoBehaviour
{
    public static SummonerSpellManager Instance { get; private set; }
    
    private List<SummonerSpell> availableSpells = new List<SummonerSpell>();
    private SummonerSpell spell1;
    private SummonerSpell spell2;
    
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
        InitializeSpells();
    }
    
    private void InitializeSpells()
    {
        // 添加所有可用的召唤师技能
        availableSpells.Add(new FlashSpell());
        availableSpells.Add(new HealSpell());
        availableSpells.Add(new IgniteSpell());
        availableSpells.Add(new ExhaustSpell());
        availableSpells.Add(new BarrierSpell());
        availableSpells.Add(new TeleportSpell());
        availableSpells.Add(new CleanseSpell());
        availableSpells.Add(new GhostSpell());
        
        // 默认设置
        spell1 = new FlashSpell();
        spell2 = new HealSpell();
    }
    
    public void SetSpell1(SummonerSpell spell)
    {
        spell1 = spell;
    }
    
    public void SetSpell2(SummonerSpell spell)
    {
        spell2 = spell;
    }
    
    public SummonerSpell GetSpell1()
    {
        return spell1;
    }
    
    public SummonerSpell GetSpell2()
    {
        return spell2;
    }
    
    public void CastSpell1(HeroController caster)
    {
        if (spell1 != null && spell1.CanCast())
        {
            spell1.Cast(caster);
        }
    }
    
    public void CastSpell2(HeroController caster)
    {
        if (spell2 != null && spell2.CanCast())
        {
            spell2.Cast(caster);
        }
    }
    
    public List<SummonerSpell> GetAvailableSpells()
    {
        return availableSpells;
    }
    
    public void UpdateCooldowns(float deltaTime)
    {
        if (spell1 != null)
        {
            spell1.UpdateCooldown(deltaTime);
        }
        if (spell2 != null)
        {
            spell2.UpdateCooldown(deltaTime);
        }
    }
}
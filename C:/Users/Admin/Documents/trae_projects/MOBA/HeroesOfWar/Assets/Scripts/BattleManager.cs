using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    
    public List<GameObject> heroesInBattle = new List<GameObject>();
    public float battleTime = 0f;
    public int maxBattleTime = 300; // 5分钟
    
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
    
    private void Update()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.InGame)
        {
            battleTime += Time.deltaTime;
            
            // 检查战斗时间是否超过限制
            if (battleTime >= maxBattleTime)
            {
                EndBattleByTimeOut();
            }
        }
    }
    
    public void AddHeroToBattle(GameObject hero)
    {
        if (!heroesInBattle.Contains(hero))
        {
            heroesInBattle.Add(hero);
        }
    }
    
    public void RemoveHeroFromBattle(GameObject hero)
    {
        if (heroesInBattle.Contains(hero))
        {
            heroesInBattle.Remove(hero);
            CheckBattleEnd();
        }
    }
    
    public void ProcessAttack(GameObject attacker, GameObject target, float damage, Skill.DamageType damageType = Skill.DamageType.Physical)
    {
        HeroController targetController = target.GetComponent<HeroController>();
        if (targetController != null)
        {
            // 计算实际伤害
            float actualDamage = CalculateDamage(damage, damageType, targetController.heroData.baseStats.armor, targetController.heroData.baseStats.magicResistance);
            targetController.TakeDamage(actualDamage);
            
            // 显示伤害数字
            ShowDamageNumber(target, actualDamage);
            
            // 检查目标是否死亡
            if (targetController.heroData.baseStats.currentHealth <= 0)
            {
                // 处理英雄死亡
                HandleHeroDeath(attacker, target);
            }
        }
    }
    
    public void ProcessSkill(GameObject caster, Skill skill, GameObject target = null)
    {
        HeroController casterController = caster.GetComponent<HeroController>();
        if (casterController != null && skill.CanCast())
        {
            if (skill.isAOE)
            {
                // 处理AOE技能
                foreach (GameObject hero in heroesInBattle)
                {
                    if (hero != caster)
                    {
                        float distance = Vector3.Distance(caster.transform.position, hero.transform.position);
                        if (distance <= skill.range)
                        {
                            ProcessAttack(caster, hero, skill.damage, skill.damageType);
                            skill.ApplyCrowdControl(hero);
                        }
                    }
                }
            }
            else if (target != null)
            {
                // 处理单体技能
                float distance = Vector3.Distance(caster.transform.position, target.transform.position);
                if (distance <= skill.range)
                {
                    ProcessAttack(caster, target, skill.damage, skill.damageType);
                    skill.ApplyCrowdControl(target);
                }
            }
            
            skill.StartCooldown();
        }
    }
    
    private float CalculateDamage(float baseDamage, Skill.DamageType damageType, float armor, float magicResistance)
    {
        switch (damageType)
        {
            case Skill.DamageType.Physical:
                // 物理伤害计算公式：实际伤害 = 基础伤害 * (100 / (100 +  armor))
                return baseDamage * (100f / (100f + armor));
            case Skill.DamageType.Magic:
                // 魔法伤害计算公式：实际伤害 = 基础伤害 * (100 / (100 +  magicResistance))
                return baseDamage * (100f / (100f + magicResistance));
            case Skill.DamageType.True:
                // 真实伤害：直接造成基础伤害
                return baseDamage;
            default:
                return baseDamage;
        }
    }
    
    private void ShowDamageNumber(GameObject target, float damage)
    {
        // 创建伤害数字UI
        // 这里可以实现一个伤害数字显示系统
        Debug.Log($"{target.name} 受到 {damage:F1} 点伤害");
    }
    
    private void HandleHeroDeath(GameObject attacker, GameObject victim)
    {
        // 处理英雄死亡逻辑
        Debug.Log($"{victim.name} 被 {attacker.name} 击杀");
        
        // 移除死亡的英雄
        RemoveHeroFromBattle(victim);
        
        // 检查战斗是否结束
        CheckBattleEnd();
    }
    
    public void CheckBattleEnd()
    {
        // 检查战斗是否结束
        int aliveHeroes = 0;
        GameObject lastAliveHero = null;
        
        foreach (GameObject hero in heroesInBattle)
        {
            HeroController controller = hero.GetComponent<HeroController>();
            if (controller != null && controller.heroData.baseStats.currentHealth > 0)
            {
                aliveHeroes++;
                lastAliveHero = hero;
            }
        }
        
        if (aliveHeroes <= 1)
        {
            // 战斗结束
            EndBattle(lastAliveHero);
        }
    }
    
    public void StartBattle()
    {
        // 开始战斗
        ResetBattle();
        
        // 初始化小兵管理器
        if (MinionManager.Instance != null)
        {
            MinionManager.Instance.Reset();
        }
        
        Debug.Log("战斗开始");
    }
    
    private void EndBattle(GameObject winner)
    {
        // 确定战斗结果
        bool playerWon = false;
        int playerKills = 0;
        int playerDeaths = 0;
        int playerAssists = 0;
        
        // 这里可以根据实际情况计算玩家的击杀、死亡和助攻数
        // 暂时使用默认值
        playerKills = 5;
        playerDeaths = 2;
        playerAssists = 3;
        
        // 调用GameFlowManager结束战斗
        GameFlowManager.Instance.EndBattle(playerWon, playerKills, playerDeaths, playerAssists);
    }
    
    private void EndBattleByTimeOut()
    {
        // 时间到，战斗结束
        Debug.Log("战斗时间到，战斗结束");
        
        // 调用GameFlowManager结束战斗
        GameFlowManager.Instance.EndBattle(false, 0, 0, 0);
    }
    
    public void ResetBattle()
    {
        // 重置战斗状态
        heroesInBattle.Clear();
        battleTime = 0f;
    }
}
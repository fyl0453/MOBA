using UnityEngine;
using System.Collections.Generic;

public class AIHeroController : MonoBehaviour
{
    public HeroController heroController;
    public AILevel aiLevel = AILevel.Normal;
    public bool isAlive = true;
    
    private float decisionInterval = 1.0f;
    private float decisionTimer = 0f;
    private GameObject target;
    private AIState currentState;
    
    private enum AIState
    {
        Idle,
        MoveToLane,
        AttackMinions,
        AttackHeroes,
        AttackTower,
        Retreat,
        Chase
    }
    
    private void Start()
    {
        currentState = AIState.MoveToLane;
        if (AIManager.Instance != null)
        {
            AIManager.Instance.AddAIHero(this);
        }
    }
    
    private void Update()
    {
        if (!isAlive)
            return;
        
        decisionTimer += Time.deltaTime;
        if (decisionTimer >= decisionInterval)
        {
            MakeDecision();
            decisionTimer = 0f;
        }
        
        ExecuteState();
    }
    
    public void UpdateAI()
    {
        if (!isAlive)
            return;
        
        MakeDecision();
        ExecuteState();
    }
    
    private void MakeDecision()
    {
        // 检查生命值
        float healthPercentage = heroController.heroData.baseStats.currentHealth / heroController.heroData.baseStats.maxHealth;
        
        // 如果生命值过低，撤退
        if (healthPercentage < 0.3f)
        {
            currentState = AIState.Retreat;
            return;
        }
        
        // 寻找目标
        target = FindTarget();
        
        if (target != null)
        {
            // 检查距离
            float distance = Vector3.Distance(transform.position, target.transform.position);
            
            // 如果目标是英雄且在攻击范围内，攻击英雄
            if (target.GetComponent<HeroController>() != null && distance <= heroController.heroData.baseStats.attackRange)
            {
                currentState = AIState.AttackHeroes;
                return;
            }
            
            // 如果目标是小兵且在攻击范围内，攻击小兵
            if (target.GetComponent<Minion>() != null && distance <= heroController.heroData.baseStats.attackRange)
            {
                currentState = AIState.AttackMinions;
                return;
            }
            
            // 如果目标是防御塔且在攻击范围内，攻击防御塔
            if (target.GetComponent<Tower>() != null && distance <= heroController.heroData.baseStats.attackRange)
            {
                currentState = AIState.AttackTower;
                return;
            }
            
            // 如果目标在视野范围内，追逐
            if (distance < 10f)
            {
                currentState = AIState.Chase;
                return;
            }
        }
        
        // 默认移动到兵线
        currentState = AIState.MoveToLane;
    }
    
    private void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                break;
            
            case AIState.MoveToLane:
                MoveToLane();
                break;
            
            case AIState.AttackMinions:
                AttackTarget(target);
                break;
            
            case AIState.AttackHeroes:
                AttackTarget(target);
                break;
            
            case AIState.AttackTower:
                AttackTarget(target);
                break;
            
            case AIState.Retreat:
                Retreat();
                break;
            
            case AIState.Chase:
                ChaseTarget(target);
                break;
        }
    }
    
    private GameObject FindTarget()
    {
        // 优先攻击敌方英雄
        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");
        foreach (GameObject hero in heroes)
        {
            if (hero != gameObject && hero.GetComponent<HeroController>() != null)
            {
                return hero;
            }
        }
        
        // 其次攻击敌方小兵
        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
        foreach (GameObject minion in minions)
        {
            if (minion.GetComponent<Minion>() != null)
            {
                return minion;
            }
        }
        
        // 最后攻击敌方防御塔
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        foreach (GameObject tower in towers)
        {
            if (tower.GetComponent<Tower>() != null)
            {
                return tower;
            }
        }
        
        return null;
    }
    
    private void MoveToLane()
    {
        // 移动到兵线位置
        Vector3 lanePosition = new Vector3(0, 0, 0); // 中路位置
        MoveToPosition(lanePosition);
    }
    
    private void AttackTarget(GameObject target)
    {
        if (target == null)
            return;
        
        // 面向目标
        transform.LookAt(target.transform);
        
        // 攻击目标
        heroController.Attack(target);
        
        // 使用技能
        UseSkills(target);
    }
    
    private void Retreat()
    {
        // 移动到基地方向
        Vector3 basePosition = new Vector3(-10, 0, 0); // 假设基地在左方
        MoveToPosition(basePosition);
    }
    
    private void ChaseTarget(GameObject target)
    {
        if (target == null)
            return;
        
        // 追逐目标
        MoveToPosition(target.transform.position);
    }
    
    private void MoveToPosition(Vector3 position)
    {
        // 移动到指定位置
        Vector3 direction = (position - transform.position).normalized;
        transform.position += direction * heroController.heroData.baseStats.moveSpeed * Time.deltaTime;
        transform.LookAt(position);
    }
    
    private void UseSkills(GameObject target)
    {
        // 根据AI等级决定是否使用技能
        if (aiLevel >= AILevel.Normal)
        {
            // 简单的技能使用逻辑
            foreach (Skill skill in heroController.heroData.skills)
            {
                if (skill.CanCast())
                {
                    heroController.UseSkill(skill, target);
                    break;
                }
            }
        }
    }
    
    public void SetAILevel(AILevel level)
    {
        aiLevel = level;
        
        // 根据AI等级调整决策间隔
        switch (level)
        {
            case AILevel.Easy:
                decisionInterval = 2.0f;
                break;
            case AILevel.Normal:
                decisionInterval = 1.0f;
                break;
            case AILevel.Hard:
                decisionInterval = 0.5f;
                break;
            case AILevel.Expert:
                decisionInterval = 0.3f;
                break;
        }
    }
    
    public void OnDeath()
    {
        isAlive = false;
    }
}

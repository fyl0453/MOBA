using UnityEngine;

public class HeroController : MonoBehaviour
{
    public Hero heroData;
    public float moveSpeed;
    public float rotationSpeed = 10f;
    
    private Vector3 moveDirection;
    private Animator animator;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    
    private void Start()
    {
        moveSpeed = heroData.baseStats.movementSpeed;
        animator = GetComponent<Animator>();
        BattleManager.Instance.AddHeroToBattle(gameObject);
    }
    
    private void Update()
    {
        // 更新技能冷却
        foreach (Skill skill in heroData.skills)
        {
            skill.UpdateCooldown(Time.deltaTime);
        }
        
        // 处理攻击冷却
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= 1f / heroData.baseStats.attackSpeed)
            {
                isAttacking = false;
                attackTimer = 0f;
            }
        }
        
        // 处理移动
        HandleMovement();
        
        // 处理攻击
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Attack(hit.collider.gameObject);
                }
            }
        }
        
        // 处理技能释放
        HandleSkills();
    }
    
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        
        if (moveDirection != Vector3.zero)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            // 播放移动动画
            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
            }
        }
        else
        {
            // 停止移动动画
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }
    
    private void HandleSkills()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Skill skill1 = heroData.skills.Find(s => s.skillType == Skill.SkillType.Skill1);
            if (skill1 != null)
            {
                CastSkill(skill1);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            Skill skill2 = heroData.skills.Find(s => s.skillType == Skill.SkillType.Skill2);
            if (skill2 != null)
            {
                CastSkill(skill2);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Skill skill3 = heroData.skills.Find(s => s.skillType == Skill.SkillType.Skill3);
            if (skill3 != null)
            {
                CastSkill(skill3);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Skill ultimate = heroData.skills.Find(s => s.skillType == Skill.SkillType.Ultimate);
            if (ultimate != null)
            {
                CastSkill(ultimate);
            }
        }
    }
    
    private void Attack(GameObject target)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = 0f;
            
            // 播放攻击动画
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // 处理攻击伤害
            BattleManager.Instance.ProcessAttack(gameObject, target, heroData.baseStats.attackDamage);
        }
    }
    
    private void CastSkill(Skill skill)
    {
        if (skill.CanCast())
        {
            // 播放技能动画
            if (animator != null)
            {
                animator.SetTrigger("CastSkill");
            }
            
            // 处理技能释放
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    BattleManager.Instance.ProcessSkill(gameObject, skill, hit.collider.gameObject);
                }
                else
                {
                    BattleManager.Instance.ProcessSkill(gameObject, skill);
                }
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        heroData.baseStats.TakeDamage(damage);
        
        // 播放受击动画
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }
        
        if (heroData.baseStats.currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        // 播放死亡动画
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        // 从战斗中移除
        BattleManager.Instance.RemoveHeroFromBattle(gameObject);
        
        // 禁用控制
        enabled = false;
        
        // 禁用碰撞器
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        
        // 禁用刚体
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    public Image[] skillIcons;
    public Text[] skillCooldowns;
    private HeroController heroController;
    
    private void Start()
    {
        heroController = FindObjectOfType<HeroController>();
        if (heroController != null)
        {
            InitializeSkillIcons();
        }
    }
    
    private void Update()
    {
        if (heroController != null)
        {
            UpdateSkillCooldowns();
        }
    }
    
    private void InitializeSkillIcons()
    {
        // 这里可以设置技能图标
        // 暂时使用占位符
    }
    
    private void UpdateSkillCooldowns()
    {
        int skillIndex = 0;
        foreach (Skill skill in heroController.heroData.skills)
        {
            if (skillIndex < skillCooldowns.Length)
            {
                if (skill.currentCooldown > 0)
                {
                    skillCooldowns[skillIndex].text = skill.currentCooldown.ToString("F1");
                }
                else
                {
                    skillCooldowns[skillIndex].text = "";
                }
            }
            skillIndex++;
        }
    }
}
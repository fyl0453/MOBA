using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Text healthText;
    private HeroController heroController;
    
    private void Start()
    {
        // 找到玩家控制的英雄
        heroController = FindObjectOfType<HeroController>();
        if (heroController != null)
        {
            UpdateHealthBar();
        }
    }
    
    private void Update()
    {
        if (heroController != null)
        {
            UpdateHealthBar();
        }
    }
    
    private void UpdateHealthBar()
    {
        float maxHealth = heroController.heroData.baseStats.maxHealth;
        float currentHealth = heroController.heroData.baseStats.currentHealth;
        float healthPercentage = currentHealth / maxHealth;
        
        if (healthSlider != null)
        {
            healthSlider.value = healthPercentage;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Round(currentHealth)}/{Mathf.Round(maxHealth)}";
        }
    }
}
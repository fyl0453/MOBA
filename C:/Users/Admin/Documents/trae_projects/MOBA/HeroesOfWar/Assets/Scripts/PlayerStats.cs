using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    
    public int gold = 500;
    public float experience = 0;
    public int level = 1;
    public int maxLevel = 15;
    public float[] experienceToNextLevel;
    
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
        InitializeExperienceTable();
    }
    
    private void InitializeExperienceTable()
    {
        // 初始化升级所需经验表
        experienceToNextLevel = new float[maxLevel];
        for (int i = 0; i < maxLevel; i++)
        {
            experienceToNextLevel[i] = 100 * (i + 1) * 1.5f;
        }
    }
    
    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"Gained {amount} gold. Total: {gold}");
    }
    
    public void RemoveGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            Debug.Log($"Spent {amount} gold. Remaining: {gold}");
        }
        else
        {
            Debug.Log("Not enough gold");
        }
    }
    
    public void AddExperience(float amount)
    {
        experience += amount;
        Debug.Log($"Gained {amount} experience. Total: {experience}");
        CheckLevelUp();
    }
    
    private void CheckLevelUp()
    {
        if (level < maxLevel && experience >= experienceToNextLevel[level - 1])
        {
            experience -= experienceToNextLevel[level - 1];
            level++;
            OnLevelUp();
        }
    }
    
    private void OnLevelUp()
    {
        Debug.Log($"Level up! Now level {level}");
        // 这里可以实现等级提升的效果，比如增加属性、解锁技能等
    }
    
    public float GetExperiencePercentage()
    {
        if (level >= maxLevel)
        {
            return 1.0f;
        }
        return experience / experienceToNextLevel[level - 1];
    }
    
    public int GetGold()
    {
        return gold;
    }
    
    public int GetLevel()
    {
        return level;
    }
    
    public float GetExperience()
    {
        return experience;
    }
}
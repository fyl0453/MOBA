[System.Serializable]
public class Task
{
    public string taskID;
    public string taskName;
    public string taskDescription;
    public TaskType taskType;
    public int targetValue;
    public int currentValue;
    public bool isCompleted;
    public bool isClaimed;
    public TaskReward reward;
    
    public enum TaskType
    {
        KillHeroes,
        WinGames,
        PlayGames,
        GetAssists,
        DealDamage,
        TakeDamage,
        UseSkills,
        BuyItems,
        LevelUp
    }
    
    public Task(string id, string name, string description, TaskType type, int target, TaskReward taskReward)
    {
        taskID = id;
        taskName = name;
        taskDescription = description;
        taskType = type;
        targetValue = target;
        currentValue = 0;
        isCompleted = false;
        isClaimed = false;
        reward = taskReward;
    }
    
    public void UpdateProgress(int amount)
    {
        if (!isCompleted)
        {
            currentValue += amount;
            if (currentValue >= targetValue)
            {
                currentValue = targetValue;
                isCompleted = true;
            }
        }
    }
    
    public void ClaimReward()
    {
        if (isCompleted && !isClaimed)
        {
            isClaimed = true;
        }
    }
    
    public float GetProgress()
    {
        return (float)currentValue / targetValue;
    }
}
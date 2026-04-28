using System;

[Serializable]
public class Achievement
{
    public string achievementName;
    public string achievementDescription;
    public bool isUnlocked;
    public DateTime unlockDate;
    public int rewardGold;
    public Sprite icon;
    
    public enum AchievementType
    {
        Win,
        Kill,
        Assist,
        Play,
        Collect
    }
    
    public AchievementType type;
    
    public Achievement(string name, string description, AchievementType achievementType, int goldReward)
    {
        achievementName = name;
        achievementDescription = description;
        type = achievementType;
        isUnlocked = false;
        unlockDate = DateTime.MinValue;
        rewardGold = goldReward;
    }
    
    public void Unlock()
    {
        if (!isUnlocked)
        {
            isUnlocked = true;
            unlockDate = DateTime.Now;
            OnUnlock();
        }
    }
    
    protected virtual void OnUnlock()
    {
        PlayerStats.Instance.AddGold(rewardGold);
        Debug.Log($"成就解锁: {achievementName}，奖励: {rewardGold} 金币");
    }
}

public class FirstWinAchievement : Achievement
{
    public FirstWinAchievement() : base("首胜", "赢得第一场游戏", AchievementType.Win, 100)
    {
    }
}

public class TenWinsAchievement : Achievement
{
    public TenWinsAchievement() : base("十连胜", "赢得十场游戏", AchievementType.Win, 500)
    {
    }
}

public class FirstBloodAchievement : Achievement
{
    public FirstBloodAchievement() : base("第一滴血", "获得第一滴血", AchievementType.Kill, 50)
    {
    }
}

public class TripleKillAchievement : Achievement
{
    public TripleKillAchievement() : base("三杀", "在一场游戏中获得三次击杀", AchievementType.Kill, 200)
    {
    }
}

public class QuadraKillAchievement : Achievement
{
    public QuadraKillAchievement() : base("四杀", "在一场游戏中获得四次击杀", AchievementType.Kill, 400)
    {
    }
}

public class PentaKillAchievement : Achievement
{
    public PentaKillAchievement() : base("五杀", "在一场游戏中获得五次击杀", AchievementType.Kill, 1000)
    {
    }
}

public class FirstGameAchievement : Achievement
{
    public FirstGameAchievement() : base("初出茅庐", "完成第一场游戏", AchievementType.Play, 20)
    {
    }
}

public class TenGamesAchievement : Achievement
{
    public TenGamesAchievement() : base("常客", "完成十场游戏", AchievementType.Play, 100)
    {
    }
}

public class CollectorAchievement : Achievement
{
    public CollectorAchievement() : base("收藏家", "拥有三个英雄", AchievementType.Collect, 150)
    {
    }
}
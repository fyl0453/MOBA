using System;

[System.Serializable]
public class Activity
{
    public string activityID;
    public string activityName;
    public string activityDescription;
    public ActivityType activityType;
    public DateTime startTime;
    public DateTime endTime;
    public bool isActive;
    public ActivityReward reward;
    public int progress;
    public int targetProgress;
    
    public enum ActivityType
    {
        LoginReward,
        LimitedTime,
        Holiday,
        Event,
        Challenge
    }
    
    public Activity(string id, string name, string description, ActivityType type, DateTime start, DateTime end, ActivityReward activityReward, int target = 1)
    {
        activityID = id;
        activityName = name;
        activityDescription = description;
        activityType = type;
        startTime = start;
        endTime = end;
        isActive = false;
        reward = activityReward;
        progress = 0;
        targetProgress = target;
    }
    
    public void UpdateProgress(int amount)
    {
        progress += amount;
        if (progress >= targetProgress)
        {
            progress = targetProgress;
            isActive = false;
        }
    }
    
    public bool IsExpired()
    {
        return DateTime.Now > endTime;
    }
    
    public bool IsActive()
    {
        return DateTime.Now >= startTime && DateTime.Now <= endTime;
    }
    
    public float GetProgress()
    {
        return (float)progress / targetProgress;
    }
}
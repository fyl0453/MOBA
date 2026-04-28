using System;
using System.Collections.Generic;

public class MentorSystemDetailedManager
{
    private static MentorSystemDetailedManager _instance;
    public static MentorSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MentorSystemDetailedManager();
            }
            return _instance;
        }
    }

    private MentorSystemData mentorData;
    private MentorSystemDataManager dataManager;

    private MentorSystemDetailedManager()
    {
        dataManager = MentorSystemDataManager.Instance;
        mentorData = dataManager.mentorData;
    }

    public void InitializePlayerMentorData(string playerID)
    {
        if (!mentorData.PlayerMentorData.ContainsKey(playerID))
        {
            PlayerMentorData playerData = new PlayerMentorData(playerID);
            mentorData.AddPlayerMentorData(playerID, playerData);
            dataManager.SaveMentorData();
            Debug.Log("初始化师徒数据成功");
        }
    }

    public string RequestMentorship(string apprenticeID, string apprenticeName, string masterID, string masterName, int apprenticeLevel, int masterLevel)
    {
        if (masterLevel < mentorData.MinMasterLevel)
        {
            Debug.LogError("师父等级不足，需要达到" + mentorData.MinMasterLevel + "级");
            return "";
        }

        InitializePlayerMentorData(apprenticeID);
        InitializePlayerMentorData(masterID);

        PlayerMentorData masterData = mentorData.PlayerMentorData[masterID];
        PlayerMentorData apprenticeData = mentorData.PlayerMentorData[apprenticeID];

        if (masterData.ApprenticeRelationships.Count >= mentorData.MaxApprenticesPerMaster)
        {
            Debug.LogError("师父的徒弟数量已达上限");
            return "";
        }

        if (apprenticeData.MasterRelationships.Count >= mentorData.MaxMastersPerApprentice)
        {
            Debug.LogError("徒弟只能有一个师父");
            return "";
        }

        if (HasExistingRelationship(masterID, apprenticeID))
        {
            Debug.LogError("师徒关系已存在");
            return "";
        }

        string relationshipID = "mentor_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MentorRelationship relationship = new MentorRelationship(relationshipID, masterID, masterName, apprenticeID, apprenticeName, masterLevel, apprenticeLevel);
        mentorData.AddRelationship(relationship);

        masterData.ApprenticeRelationships.Add(relationship);
        masterData.TotalApprentices++;
        masterData.LastMentorActivity = DateTime.Now;

        apprenticeData.MasterRelationships.Add(relationship);
        apprenticeData.TotalMasters++;
        apprenticeData.LastMentorActivity = DateTime.Now;

        InitializeRelationshipTasks(relationshipID);
        
        dataManager.CreateMentorEvent("mentorship_request", apprenticeID, masterID, "徒弟请求拜师");
        dataManager.SaveMentorData();
        Debug.Log("拜师请求成功");
        return relationshipID;
    }

    public void AcceptMentorship(string relationshipID)
    {
        MentorRelationship relationship = mentorData.AllRelationships.Find(r => r.RelationshipID == relationshipID);
        if (relationship != null && relationship.Status == 0)
        {
            relationship.Status = 1;
            relationship.LastInteractionTime = DateTime.Now;
            
            dataManager.CreateMentorEvent("mentorship_accept", relationship.MasterID, relationship.ApprenticeID, "师父接受拜师");
            dataManager.SaveMentorData();
            Debug.Log("拜师成功，师徒关系已建立");
        }
    }

    public void RejectMentorship(string relationshipID)
    {
        MentorRelationship relationship = mentorData.AllRelationships.Find(r => r.RelationshipID == relationshipID);
        if (relationship != null && relationship.Status == 0)
        {
            mentorData.AllRelationships.Remove(relationship);
            
            if (mentorData.PlayerMentorData.ContainsKey(relationship.MasterID))
            {
                PlayerMentorData masterData = mentorData.PlayerMentorData[relationship.MasterID];
                masterData.ApprenticeRelationships.Remove(relationship);
                masterData.TotalApprentices--;
            }
            
            if (mentorData.PlayerMentorData.ContainsKey(relationship.ApprenticeID))
            {
                PlayerMentorData apprenticeData = mentorData.PlayerMentorData[relationship.ApprenticeID];
                apprenticeData.MasterRelationships.Remove(relationship);
                apprenticeData.TotalMasters--;
            }
            
            dataManager.CreateMentorEvent("mentorship_reject", relationship.MasterID, relationship.ApprenticeID, "师父拒绝拜师");
            dataManager.SaveMentorData();
            Debug.Log("拜师请求已拒绝");
        }
    }

    public void UpdateTaskProgress(string relationshipID, string taskID, int progress)
    {
        MentorTaskProgress taskProgress = mentorData.TaskProgresses.Find(p => p.RelationshipID == relationshipID && p.TaskID == taskID);
        if (taskProgress != null && !taskProgress.IsCompleted)
        {
            taskProgress.CurrentCount += progress;
            taskProgress.LastUpdateTime = DateTime.Now;
            
            MentorTask task = mentorData.AllTasks.Find(t => t.TaskID == taskID);
            if (task != null && taskProgress.CurrentCount >= task.RequiredCount)
            {
                taskProgress.IsCompleted = true;
                taskProgress.CompleteTime = DateTime.Now;
                
                MentorRelationship relationship = mentorData.AllRelationships.Find(r => r.RelationshipID == relationshipID);
                if (relationship != null)
                {
                    relationship.Progress++;
                    relationship.LastInteractionTime = DateTime.Now;
                    
                    GenerateTaskRewards(relationshipID, taskID);
                    CheckRelationshipCompletion(relationship);
                }
                
                dataManager.CreateMentorEvent("task_complete", relationshipID, taskID, "任务完成: " + task.TaskName);
                Debug.Log("任务完成: " + task.TaskName);
            }
            
            dataManager.SaveMentorData();
        }
    }

    public void ClaimReward(string rewardID)
    {
        MentorRewards reward = mentorData.Rewards.Find(r => r.RewardID == rewardID);
        if (reward != null && !reward.IsClaimed)
        {
            reward.IsClaimed = true;
            reward.ClaimTime = DateTime.Now;
            
            dataManager.CreateMentorEvent("reward_claim", "", reward.RelationshipID, "领取奖励: " + reward.RewardName);
            dataManager.SaveMentorData();
            Debug.Log("领取奖励成功: " + reward.RewardName);
        }
    }

    public void TerminateRelationship(string relationshipID)
    {
        MentorRelationship relationship = mentorData.AllRelationships.Find(r => r.RelationshipID == relationshipID);
        if (relationship != null)
        {
            mentorData.AllRelationships.Remove(relationship);
            
            if (mentorData.PlayerMentorData.ContainsKey(relationship.MasterID))
            {
                PlayerMentorData masterData = mentorData.PlayerMentorData[relationship.MasterID];
                masterData.ApprenticeRelationships.Remove(relationship);
                masterData.TotalApprentices--;
            }
            
            if (mentorData.PlayerMentorData.ContainsKey(relationship.ApprenticeID))
            {
                PlayerMentorData apprenticeData = mentorData.PlayerMentorData[relationship.ApprenticeID];
                apprenticeData.MasterRelationships.Remove(relationship);
                apprenticeData.TotalMasters--;
            }
            
            dataManager.CreateMentorEvent("relationship_terminate", relationship.MasterID, relationship.ApprenticeID, "解除师徒关系");
            dataManager.SaveMentorData();
            Debug.Log("解除师徒关系成功");
        }
    }

    private void InitializeRelationshipTasks(string relationshipID)
    {
        foreach (MentorTask task in mentorData.AllTasks)
        {
            if (task.IsActive)
            {
                string progressID = "progress_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                MentorTaskProgress progress = new MentorTaskProgress(progressID, relationshipID, task.TaskID);
                mentorData.AddTaskProgress(progress);
            }
        }
    }

    private void GenerateTaskRewards(string relationshipID, string taskID)
    {
        MentorTask task = mentorData.AllTasks.Find(t => t.TaskID == taskID);
        if (task != null)
        {
            foreach (string rewardStr in task.Rewards)
            {
                string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                string rewardType = "task";
                string rewardName = rewardStr;
                int rewardAmount = 1;
                
                MentorRewards reward = new MentorRewards(rewardID, relationshipID, rewardType, rewardName, rewardAmount);
                mentorData.AddReward(reward);
            }
        }
    }

    private void CheckRelationshipCompletion(MentorRelationship relationship)
    {
        int totalTasks = mentorData.AllTasks.Count;
        if (relationship.Progress >= totalTasks && !relationship.IsCompleted)
        {
            relationship.IsCompleted = true;
            relationship.Status = 2;
            
            if (mentorData.PlayerMentorData.ContainsKey(relationship.MasterID))
            {
                PlayerMentorData masterData = mentorData.PlayerMentorData[relationship.MasterID];
                masterData.CompletedApprentices++;
                masterData.MasterLevel = CalculateMasterLevel(masterData.CompletedApprentices);
            }
            
            GenerateCompletionRewards(relationship.RelationshipID);
            dataManager.CreateMentorEvent("relationship_complete", relationship.MasterID, relationship.ApprenticeID, "师徒关系完成");
            Debug.Log("师徒关系完成");
        }
    }

    private void GenerateCompletionRewards(string relationshipID)
    {
        string rewardID1 = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MentorRewards masterReward = new MentorRewards(rewardID1, relationshipID, "completion", "专属师父称号", 1);
        mentorData.AddReward(masterReward);
        
        string rewardID2 = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        MentorRewards apprenticeReward = new MentorRewards(rewardID2, relationshipID, "completion", "专属徒弟称号", 1);
        mentorData.AddReward(apprenticeReward);
    }

    private int CalculateMasterLevel(int completedApprentices)
    {
        if (completedApprentices >= 10)
            return 5;
        else if (completedApprentices >= 5)
            return 4;
        else if (completedApprentices >= 3)
            return 3;
        else if (completedApprentices >= 1)
            return 2;
        else
            return 1;
    }

    private bool HasExistingRelationship(string masterID, string apprenticeID)
    {
        foreach (MentorRelationship relationship in mentorData.AllRelationships)
        {
            if ((relationship.MasterID == masterID && relationship.ApprenticeID == apprenticeID) ||
                (relationship.MasterID == apprenticeID && relationship.ApprenticeID == masterID))
            {
                return true;
            }
        }
        return false;
    }

    public List<MentorRelationship> GetPlayerMasterRelationships(string playerID)
    {
        if (mentorData.PlayerMentorData.ContainsKey(playerID))
        {
            return mentorData.PlayerMentorData[playerID].MasterRelationships;
        }
        return new List<MentorRelationship>();
    }

    public List<MentorRelationship> GetPlayerApprenticeRelationships(string playerID)
    {
        if (mentorData.PlayerMentorData.ContainsKey(playerID))
        {
            return mentorData.PlayerMentorData[playerID].ApprenticeRelationships;
        }
        return new List<MentorRelationship>();
    }

    public List<MentorTask> GetAllTasks()
    {
        return mentorData.AllTasks;
    }

    public List<MentorTaskProgress> GetRelationshipTaskProgresses(string relationshipID)
    {
        return mentorData.TaskProgresses.FindAll(p => p.RelationshipID == relationshipID);
    }

    public List<MentorRewards> GetRelationshipRewards(string relationshipID)
    {
        return mentorData.Rewards.FindAll(r => r.RelationshipID == relationshipID);
    }

    public List<MentorRewards> GetUnclaimedRewards(string relationshipID)
    {
        return mentorData.Rewards.FindAll(r => r.RelationshipID == relationshipID && !r.IsClaimed);
    }

    public int GetPlayerMasterLevel(string playerID)
    {
        if (mentorData.PlayerMentorData.ContainsKey(playerID))
        {
            return mentorData.PlayerMentorData[playerID].MasterLevel;
        }
        return 1;
    }

    public int GetPlayerCompletedApprentices(string playerID)
    {
        if (mentorData.PlayerMentorData.ContainsKey(playerID))
        {
            return mentorData.PlayerMentorData[playerID].CompletedApprentices;
        }
        return 0;
    }

    public void CleanupInactiveRelationships()
    {
        List<MentorRelationship> inactiveRelationships = new List<MentorRelationship>();
        foreach (MentorRelationship relationship in mentorData.AllRelationships)
        {
            if (relationship.Status == 2 || (relationship.Status == 0 && (DateTime.Now - relationship.EstablishTime).TotalDays > 7))
            {
                inactiveRelationships.Add(relationship);
            }
        }
        
        foreach (MentorRelationship relationship in inactiveRelationships)
        {
            mentorData.AllRelationships.Remove(relationship);
        }
        
        if (inactiveRelationships.Count > 0)
        {
            dataManager.CreateMentorEvent("relationship_cleanup", "system", "", "清理无效师徒关系: " + inactiveRelationships.Count);
            dataManager.SaveMentorData();
            Debug.Log("清理无效师徒关系成功: " + inactiveRelationships.Count);
        }
    }

    public void SaveData()
    {
        dataManager.SaveMentorData();
    }

    public void LoadData()
    {
        dataManager.LoadMentorData();
    }

    public List<MentorEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
[System.Serializable]
public class MentorApprentice
{
    public string relationshipID;
    public string mentorID;
    public string mentorName;
    public string apprenticeID;
    public string apprenticeName;
    public string establishDate;
    public string status;
    public List<MentorTask> tasks;
    public bool hasGraduated;
    
    public MentorApprentice(string id, string mentor, string mentorName, string apprentice, string apprenticeName)
    {
        relationshipID = id;
        this.mentorID = mentor;
        this.mentorName = mentorName;
        this.apprenticeID = apprentice;
        this.apprenticeName = apprenticeName;
        establishDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        status = "Active";
        tasks = new List<MentorTask>();
        hasGraduated = false;
    }
    
    public void AddTask(string taskID, string taskName, string taskDescription, int requiredProgress)
    {
        MentorTask task = new MentorTask(taskID, taskName, taskDescription, requiredProgress);
        tasks.Add(task);
    }
    
    public void UpdateTaskProgress(string taskID, int progress)
    {
        MentorTask task = tasks.Find(t => t.taskID == taskID);
        if (task != null)
        {
            task.AddProgress(progress);
        }
    }
    
    public void Graduate()
    {
        hasGraduated = true;
        status = "Graduated";
    }
    
    public void EndRelationship()
    {
        status = "Ended";
    }
}

[System.Serializable]
public class MentorTask
{
    public string taskID;
    public string taskName;
    public string taskDescription;
    public int requiredProgress;
    public int currentProgress;
    public bool isCompleted;
    
    public MentorTask(string id, string name, string desc, int reqProgress)
    {
        taskID = id;
        taskName = name;
        taskDescription = desc;
        requiredProgress = reqProgress;
        currentProgress = 0;
        isCompleted = false;
    }
    
    public void AddProgress(int amount)
    {
        if (!isCompleted)
        {
            currentProgress += amount;
            if (currentProgress >= requiredProgress)
            {
                currentProgress = requiredProgress;
                isCompleted = true;
            }
        }
    }
}

[System.Serializable]
public class MentorManagerData
{
    public List<MentorApprentice> mentorApprentices;
    
    public MentorManagerData()
    {
        mentorApprentices = new List<MentorApprentice>();
    }
    
    public void AddMentorApprentice(MentorApprentice mentorApprentice)
    {
        mentorApprentices.Add(mentorApprentice);
    }
    
    public MentorApprentice GetMentorApprentice(string id)
    {
        return mentorApprentices.Find(ma => ma.relationshipID == id);
    }
    
    public List<MentorApprentice> GetMentorRelationships(string playerID)
    {
        return mentorApprentices.FindAll(ma => ma.mentorID == playerID && ma.status == "Active");
    }
    
    public List<MentorApprentice> GetApprenticeRelationships(string playerID)
    {
        return mentorApprentices.FindAll(ma => ma.apprenticeID == playerID && ma.status == "Active");
    }
    
    public MentorApprentice GetRelationshipBetweenPlayers(string mentorID, string apprenticeID)
    {
        return mentorApprentices.Find(ma => ma.mentorID == mentorID && ma.apprenticeID == apprenticeID);
    }
}
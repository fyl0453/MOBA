using System.Collections.Generic;

[System.Serializable]
public class Team
{
    public string teamName;
    public List<string> members = new List<string>();
    public bool isReady = false;
    
    public Team(string name)
    {
        teamName = name;
    }
    
    public void AddMember(string memberName)
    {
        if (!members.Contains(memberName))
        {
            members.Add(memberName);
        }
    }
    
    public void RemoveMember(string memberName)
    {
        if (members.Contains(memberName))
        {
            members.Remove(memberName);
        }
    }
    
    public List<string> GetMembers()
    {
        return members;
    }
    
    public int GetMemberCount()
    {
        return members.Count;
    }
    
    public void SetReady(bool ready)
    {
        isReady = ready;
    }
    
    public bool IsReady()
    {
        return isReady;
    }
}
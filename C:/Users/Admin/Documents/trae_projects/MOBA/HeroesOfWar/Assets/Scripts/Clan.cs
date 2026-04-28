using System.Collections.Generic;

[System.Serializable]
public class Clan
{
    public string clanName;
    public string clanDescription;
    public string leader;
    public List<string> members = new List<string>();
    public int clanLevel;
    public int clanPoints;
    
    public Clan(string name, string description)
    {
        clanName = name;
        clanDescription = description;
        clanLevel = 1;
        clanPoints = 0;
    }
    
    public void AddMember(string memberName)
    {
        if (!members.Contains(memberName))
        {
            members.Add(memberName);
            if (leader == null)
            {
                leader = memberName;
            }
        }
    }
    
    public void RemoveMember(string memberName)
    {
        if (members.Contains(memberName))
        {
            members.Remove(memberName);
            if (leader == memberName && members.Count > 0)
            {
                leader = members[0];
            }
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
    
    public void SetLeader(string memberName)
    {
        if (members.Contains(memberName))
        {
            leader = memberName;
        }
    }
    
    public string GetLeader()
    {
        return leader;
    }
    
    public void AddPoints(int points)
    {
        clanPoints += points;
        CheckLevelUp();
    }
    
    private void CheckLevelUp()
    {
        int requiredPoints = clanLevel * 1000;
        if (clanPoints >= requiredPoints)
        {
            clanLevel++;
            clanPoints -= requiredPoints;
        }
    }
    
    public int GetLevel()
    {
        return clanLevel;
    }
    
    public int GetPoints()
    {
        return clanPoints;
    }
}
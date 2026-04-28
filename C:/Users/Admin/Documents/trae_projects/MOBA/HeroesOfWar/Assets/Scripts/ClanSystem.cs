using UnityEngine;
using System.Collections.Generic;

public class ClanSystem : MonoBehaviour
{
    public static ClanSystem Instance { get; private set; }
    
    private Dictionary<string, ClanData> clans = new Dictionary<string, ClanData>();
    private Dictionary<string, string> playerClans = new Dictionary<string, string>(); // 玩家ID -> 战队ID
    
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
    
    public bool CreateClan(string clanName, string leaderID, string leaderName)
    {
        // 检查战队名称是否已存在
        foreach (var clan in clans.Values)
        {
            if (clan.clanName == clanName)
            {
                Debug.LogError("战队名称已存在");
                return false;
            }
        }
        
        // 创建新战队
        string clanID = System.Guid.NewGuid().ToString();
        ClanData newClan = new ClanData
        {
            clanID = clanID,
            clanName = clanName,
            leaderID = leaderID,
            leaderName = leaderName,
            createDate = System.DateTime.Now.ToString(),
            members = new List<ClanMember>
            {
                new ClanMember
                {
                    memberID = leaderID,
                    memberName = leaderName,
                    role = ClanRole.Leader,
                    joinDate = System.DateTime.Now.ToString()
                }
            },
            level = 1,
            experience = 0,
            clanPoints = 0
        };
        
        clans[clanID] = newClan;
        playerClans[leaderID] = clanID;
        
        Debug.Log($"创建战队成功: {clanName}");
        return true;
    }
    
    public bool JoinClan(string playerID, string playerName, string clanID)
    {
        // 检查玩家是否已加入战队
        if (playerClans.ContainsKey(playerID))
        {
            Debug.LogError("玩家已加入战队");
            return false;
        }
        
        // 检查战队是否存在
        if (!clans.ContainsKey(clanID))
        {
            Debug.LogError("战队不存在");
            return false;
        }
        
        // 添加玩家到战队
        ClanData clan = clans[clanID];
        clan.members.Add(new ClanMember
        {
            memberID = playerID,
            memberName = playerName,
            role = ClanRole.Member,
            joinDate = System.DateTime.Now.ToString()
        });
        
        playerClans[playerID] = clanID;
        
        Debug.Log($"加入战队成功: {clan.clanName}");
        return true;
    }
    
    public bool LeaveClan(string playerID)
    {
        // 检查玩家是否加入了战队
        if (!playerClans.ContainsKey(playerID))
        {
            Debug.LogError("玩家未加入战队");
            return false;
        }
        
        string clanID = playerClans[playerID];
        ClanData clan = clans[clanID];
        
        // 检查是否是队长
        if (clan.leaderID == playerID)
        {
            // 如果是队长，需要转让队长或解散战队
            if (clan.members.Count > 1)
            {
                // 转让队长给第一个成员
                ClanMember newLeader = clan.members.Find(m => m.memberID != playerID);
                if (newLeader != null)
                {
                    clan.leaderID = newLeader.memberID;
                    clan.leaderName = newLeader.memberName;
                    newLeader.role = ClanRole.Leader;
                }
            }
            else
            {
                // 解散战队
                clans.Remove(clanID);
                playerClans.Remove(playerID);
                Debug.Log("战队已解散");
                return true;
            }
        }
        
        // 移除玩家
        clan.members.RemoveAll(m => m.memberID == playerID);
        playerClans.Remove(playerID);
        
        Debug.Log("离开战队成功");
        return true;
    }
    
    public ClanData GetClan(string clanID)
    {
        if (clans.ContainsKey(clanID))
        {
            return clans[clanID];
        }
        return null;
    }
    
    public ClanData GetPlayerClan(string playerID)
    {
        if (playerClans.ContainsKey(playerID))
        {
            string clanID = playerClans[playerID];
            return GetClan(clanID);
        }
        return null;
    }
    
    public List<ClanData> GetClans()
    {
        return new List<ClanData>(clans.Values);
    }
    
    public void UpgradeClan(string clanID)
    {
        if (clans.ContainsKey(clanID))
        {
            ClanData clan = clans[clanID];
            clan.level++;
            clan.experience = 0;
            Debug.Log($"战队升级了！现在是 {clan.level} 级");
        }
    }
    
    public void AddClanExperience(string clanID, int amount)
    {
        if (clans.ContainsKey(clanID))
        {
            ClanData clan = clans[clanID];
            clan.experience += amount;
            
            // 检查是否升级
            while (clan.experience >= GetRequiredExperience(clan.level))
            {
                UpgradeClan(clanID);
            }
        }
    }
    
    private int GetRequiredExperience(int level)
    {
        // 简单的经验计算公式
        return 1000 * level;
    }
}

[System.Serializable]
public class ClanData
{
    public string clanID;
    public string clanName;
    public string leaderID;
    public string leaderName;
    public string createDate;
    public List<ClanMember> members;
    public int level;
    public int experience;
    public int clanPoints;
}

[System.Serializable]
public class ClanMember
{
    public string memberID;
    public string memberName;
    public ClanRole role;
    public string joinDate;
}

public enum ClanRole
{
    Leader,
    Officer,
    Member
}

using System.Collections.Generic;
using UnityEngine;

public class ClanManager : MonoBehaviour
{
    public static ClanManager Instance { get; private set; }
    
    private List<Clan> clans = new List<Clan>();
    private Clan currentClan;
    
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
        // 模拟数据
        CreateClan("Warriors", "We are the strongest!");
        CreateClan("Mages", "Magic is power!");
        CreateClan("Archers", "Precision and speed!");
        
        // 添加成员
        clans[0].AddMember("Player1");
        clans[0].AddMember("Player2");
        clans[1].AddMember("Player3");
        clans[2].AddMember("Player4");
        
        // 设置当前战队
        currentClan = clans[0];
    }
    
    public Clan CreateClan(string name, string description)
    {
        Clan clan = new Clan(name, description);
        clans.Add(clan);
        return clan;
    }
    
    public void JoinClan(Clan clan)
    {
        currentClan = clan;
    }
    
    public void LeaveClan()
    {
        currentClan = null;
    }
    
    public Clan GetCurrentClan()
    {
        return currentClan;
    }
    
    public List<Clan> GetClans()
    {
        return clans;
    }
    
    public Clan GetClanByName(string name)
    {
        return clans.Find(clan => clan.clanName == name);
    }
    
    public void InviteToClan(string playerName, Clan clan)
    {
        // 发送战队邀请
        Debug.Log($"Invited {playerName} to {clan.clanName}");
    }
    
    public void KickFromClan(string playerName, Clan clan)
    {
        clan.RemoveMember(playerName);
    }
    
    public void PromoteToLeader(string playerName, Clan clan)
    {
        clan.SetLeader(playerName);
    }
}
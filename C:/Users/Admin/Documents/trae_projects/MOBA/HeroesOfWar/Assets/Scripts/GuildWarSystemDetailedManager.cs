using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GuildWarSystemDetailedManager : MonoBehaviour
{
    public static GuildWarSystemDetailedManager Instance { get; private set; }
    
    public GuildWarSystemDetailedManagerData guildWarData;
    
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
        LoadGuildWarData();
        
        if (guildWarData == null)
        {
            guildWarData = new GuildWarSystemDetailedManagerData();
            InitializeDefaultGuildWarSystem();
        }
    }
    
    private void InitializeDefaultGuildWarSystem()
    {
        // 公会战
        string registrationStartTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string registrationEndTime = System.DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
        string startTime = System.DateTime.Now.AddDays(4).ToString("yyyy-MM-dd HH:mm:ss");
        string endTime = System.DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss");
        
        GuildWar war1 = new GuildWar("war_001", "春季公会战", "春季公会战赛季", "seasonal", startTime, endTime, registrationStartTime, registrationEndTime, 16, "map_001", "公会战规则：双方公会各5名成员参战，通过摧毁对方基地获得胜利");
        
        guildWarData.system.AddGuildWar(war1);
        
        // 公会战参与者
        GuildWarParticipant participant1 = new GuildWarParticipant("participant_001", "war_001", "guild_001", "荣耀公会", "user_001", "张三");
        GuildWarParticipant participant2 = new GuildWarParticipant("participant_002", "war_001", "guild_002", "王者公会", "user_002", "李四");
        GuildWarParticipant participant3 = new GuildWarParticipant("participant_003", "war_001", "guild_003", "传奇公会", "user_003", "王五");
        
        guildWarData.system.AddGuildWarParticipant(participant1);
        guildWarData.system.AddGuildWarParticipant(participant2);
        guildWarData.system.AddGuildWarParticipant(participant3);
        
        // 更新公会战参与者数量
        war1.AddParticipant();
        war1.AddParticipant();
        war1.AddParticipant();
        
        // 公会战匹配
        GuildWarMatch match1 = new GuildWarMatch("match_001", "war_001", "guild_001", "荣耀公会", "guild_002", "王者公会", "map_001");
        GuildWarMatch match2 = new GuildWarMatch("match_002", "war_001", "guild_002", "王者公会", "guild_003", "传奇公会", "map_001");
        GuildWarMatch match3 = new GuildWarMatch("match_003", "war_001", "guild_003", "传奇公会", "guild_001", "荣耀公会", "map_001");
        
        guildWarData.system.AddGuildWarMatch(match1);
        guildWarData.system.AddGuildWarMatch(match2);
        guildWarData.system.AddGuildWarMatch(match3);
        
        // 公会战奖励
        GuildWarReward reward1 = new GuildWarReward("reward_001", "war_001", "公会战冠军奖励", "package", "champion_package", 1, 1, 100, "icon_champion");
        GuildWarReward reward2 = new GuildWarReward("reward_002", "war_001", "公会战亚军奖励", "package", "runner_up_package", 1, 2, 80, "icon_runner_up");
        GuildWarReward reward3 = new GuildWarReward("reward_003", "war_001", "公会战季军奖励", "package", "third_package", 1, 3, 60, "icon_third");
        GuildWarReward reward4 = new GuildWarReward("reward_004", "war_001", "公会战参与奖励", "currency", "gold", 1000, 16, 20, "icon_gold");
        
        guildWarData.system.AddGuildWarReward(reward1);
        guildWarData.system.AddGuildWarReward(reward2);
        guildWarData.system.AddGuildWarReward(reward3);
        guildWarData.system.AddGuildWarReward(reward4);
        
        // 公会战事件
        GuildWarEvent event1 = new GuildWarEvent("event_001", "war_001", "register", "guild_001", "公会注册参与公会战");
        GuildWarEvent event2 = new GuildWarEvent("event_002", "war_001", "match", "guild_001", "公会匹配到对手");
        GuildWarEvent event3 = new GuildWarEvent("event_003", "war_001", "victory", "guild_001", "公会获得胜利");
        
        guildWarData.system.AddGuildWarEvent(event1);
        guildWarData.system.AddGuildWarEvent(event2);
        guildWarData.system.AddGuildWarEvent(event3);
        
        // 开始公会战注册
        war1.StartRegistration();
        
        SaveGuildWarData();
    }
    
    // 公会战管理
    public void CreateGuildWar(string warName, string warDescription, string warType, string startTime, string endTime, string registrationStartTime, string registrationEndTime, int maxParticipants, string mapID, string rules)
    {
        string warID = "war_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuildWar guildWar = new GuildWar(warID, warName, warDescription, warType, startTime, endTime, registrationStartTime, registrationEndTime, maxParticipants, mapID, rules);
        guildWarData.system.AddGuildWar(guildWar);
        
        // 开始注册
        guildWar.StartRegistration();
        
        SaveGuildWarData();
        Debug.Log("成功创建公会战: " + warName);
    }
    
    public void StartGuildWar(string warID)
    {
        GuildWar guildWar = guildWarData.system.GetGuildWar(warID);
        if (guildWar != null && guildWar.status == "registration")
        {
            guildWar.StartWar();
            
            // 生成匹配
            GenerateMatches(warID);
            
            SaveGuildWarData();
            Debug.Log("成功开始公会战: " + guildWar.warName);
        }
        else
        {
            Debug.LogError("公会战不存在或不在注册状态");
        }
    }
    
    public void EndGuildWar(string warID)
    {
        GuildWar guildWar = guildWarData.system.GetGuildWar(warID);
        if (guildWar != null && guildWar.status == "active")
        {
            guildWar.EndWar();
            
            // 计算排名
            CalculateRanking(warID);
            
            // 发放奖励
            DistributeRewards(warID);
            
            SaveGuildWarData();
            Debug.Log("成功结束公会战: " + guildWar.warName);
        }
        else
        {
            Debug.LogError("公会战不存在或不在活跃状态");
        }
    }
    
    private void GenerateMatches(string warID)
    {
        List<GuildWarParticipant> participants = guildWarData.system.GetGuildWarParticipantsByWar(warID);
        
        // 简单的循环匹配
        for (int i = 0; i < participants.Count; i++)
        {
            for (int j = i + 1; j < participants.Count; j++)
            {
                string matchID = "match_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                GuildWarMatch match = new GuildWarMatch(matchID, warID, participants[i].guildID, participants[i].guildName, participants[j].guildID, participants[j].guildName, "map_001");
                guildWarData.system.AddGuildWarMatch(match);
            }
        }
    }
    
    private void CalculateRanking(string warID)
    {
        List<GuildWarParticipant> participants = guildWarData.system.GetGuildWarParticipantsByWar(warID);
        
        // 按积分排序
        participants.Sort((a, b) => b.points.CompareTo(a.points));
        
        // 设置排名
        for (int i = 0; i < participants.Count; i++)
        {
            participants[i].SetRank(i + 1);
        }
    }
    
    private void DistributeRewards(string warID)
    {
        List<GuildWarParticipant> participants = guildWarData.system.GetGuildWarParticipantsByWar(warID);
        List<GuildWarReward> rewards = guildWarData.system.GetGuildWarRewardsByWar(warID);
        
        foreach (GuildWarParticipant participant in participants)
        {
            foreach (GuildWarReward reward in rewards)
            {
                if (reward.IsAvailable(participant.rank, participant.points))
                {
                    // 这里可以添加发放奖励的逻辑
                    Debug.Log("发放公会战奖励: " + reward.rewardName + " 给公会: " + participant.guildName);
                }
            }
        }
    }
    
    public List<GuildWar> GetGuildWarsByStatus(string status)
    {
        return guildWarData.system.GetGuildWarsByStatus(status);
    }
    
    public List<GuildWar> GetActiveGuildWars()
    {
        return guildWarData.system.GetGuildWarsByStatus("active");
    }
    
    // 公会战参与管理
    public void RegisterGuildToWar(string warID, string guildID, string guildName, string leaderID, string leaderName)
    {
        GuildWar guildWar = guildWarData.system.GetGuildWar(warID);
        if (guildWar != null && guildWar.CanRegister())
        {
            // 检查是否已经注册
            List<GuildWarParticipant> participants = guildWarData.system.GetGuildWarParticipantsByWar(warID);
            GuildWarParticipant existingParticipant = participants.Find(p => p.guildID == guildID);
            
            if (existingParticipant == null)
            {
                string participantID = "participant_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                GuildWarParticipant guildWarParticipant = new GuildWarParticipant(participantID, warID, guildID, guildName, leaderID, leaderName);
                guildWarData.system.AddGuildWarParticipant(guildWarParticipant);
                guildWar.AddParticipant();
                
                // 创建公会战事件
                CreateGuildWarEvent("register", warID, guildID, "公会注册参与公会战");
                
                SaveGuildWarData();
                Debug.Log("成功注册公会参与公会战: " + guildName);
            }
            else
            {
                Debug.LogError("公会已经注册参与该公会战");
            }
        }
        else
        {
            Debug.LogError("公会战不存在或注册已结束");
        }
    }
    
    public void WithdrawGuildFromWar(string warID, string guildID)
    {
        List<GuildWarParticipant> participants = guildWarData.system.GetGuildWarParticipantsByWar(warID);
        GuildWarParticipant guildWarParticipant = participants.Find(p => p.guildID == guildID);
        
        if (guildWarParticipant != null)
        {
            guildWarParticipant.Leave();
            
            GuildWar guildWar = guildWarData.system.GetGuildWar(warID);
            if (guildWar != null)
            {
                guildWar.RemoveParticipant();
            }
            
            // 创建公会战事件
            CreateGuildWarEvent("withdraw", warID, guildID, "公会退出公会战");
            
            SaveGuildWarData();
            Debug.Log("成功退出公会战");
        }
        else
        {
            Debug.LogError("公会未参与该公会战");
        }
    }
    
    public List<GuildWarParticipant> GetGuildWarParticipants(string warID)
    {
        return guildWarData.system.GetGuildWarParticipantsByWar(warID);
    }
    
    // 公会战匹配管理
    public void StartGuildWarMatch(string matchID)
    {
        GuildWarMatch match = guildWarData.system.GetGuildWarMatch(matchID);
        if (match != null && match.status == "scheduled")
        {
            match.StartMatch();
            
            // 创建公会战事件
            CreateGuildWarEvent("match_start", match.warID, match.guild1ID, "公会战匹配开始");
            CreateGuildWarEvent("match_start", match.warID, match.guild2ID, "公会战匹配开始");
            
            SaveGuildWarData();
            Debug.Log("成功开始公会战匹配: " + match.guild1Name + " vs " + match.guild2Name);
        }
        else
        {
            Debug.LogError("匹配不存在或不在预定状态");
        }
    }
    
    public void EndGuildWarMatch(string matchID, string winnerID, string loserID, int guild1Points, int guild2Points, string matchResult)
    {
        GuildWarMatch match = guildWarData.system.GetGuildWarMatch(matchID);
        if (match != null && match.status == "active")
        {
            match.EndMatch(winnerID, loserID, guild1Points, guild2Points, matchResult);
            
            // 更新公会积分和胜负
            List<GuildWarParticipant> participants = guildWarData.system.GetGuildWarParticipantsByWar(match.warID);
            
            GuildWarParticipant winner = participants.Find(p => p.guildID == winnerID);
            if (winner != null)
            {
                winner.AddPoints(10);
                winner.AddWin();
            }
            
            GuildWarParticipant loser = participants.Find(p => p.guildID == loserID);
            if (loser != null)
            {
                loser.AddPoints(2);
                loser.AddLoss();
            }
            
            // 创建公会战事件
            CreateGuildWarEvent("victory", match.warID, winnerID, "公会获得胜利");
            CreateGuildWarEvent("defeat", match.warID, loserID, "公会遭遇失败");
            
            SaveGuildWarData();
            Debug.Log("成功结束公会战匹配: " + matchResult);
        }
        else
        {
            Debug.LogError("匹配不存在或不在活跃状态");
        }
    }
    
    public void CancelGuildWarMatch(string matchID)
    {
        GuildWarMatch match = guildWarData.system.GetGuildWarMatch(matchID);
        if (match != null && match.status == "scheduled")
        {
            match.CancelMatch();
            
            // 创建公会战事件
            CreateGuildWarEvent("match_cancel", match.warID, match.guild1ID, "公会战匹配取消");
            CreateGuildWarEvent("match_cancel", match.warID, match.guild2ID, "公会战匹配取消");
            
            SaveGuildWarData();
            Debug.Log("成功取消公会战匹配");
        }
        else
        {
            Debug.LogError("匹配不存在或不在预定状态");
        }
    }
    
    public List<GuildWarMatch> GetGuildWarMatches(string warID)
    {
        return guildWarData.system.GetGuildWarMatchesByWar(warID);
    }
    
    public List<GuildWarMatch> GetGuildWarMatchesByGuild(string guildID)
    {
        return guildWarData.system.GetGuildWarMatchesByGuild(guildID);
    }
    
    // 公会战奖励管理
    public void AddGuildWarReward(string warID, string rewardName, string rewardType, string rewardValue, int quantity, int requiredRank, int requiredPoints, string icon)
    {
        string rewardID = "reward_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuildWarReward guildWarReward = new GuildWarReward(rewardID, warID, rewardName, rewardType, rewardValue, quantity, requiredRank, requiredPoints, icon);
        guildWarData.system.AddGuildWarReward(guildWarReward);
        
        SaveGuildWarData();
        Debug.Log("成功添加公会战奖励: " + rewardName);
    }
    
    public List<GuildWarReward> GetGuildWarRewards(string warID)
    {
        return guildWarData.system.GetGuildWarRewardsByWar(warID);
    }
    
    // 公会战事件管理
    public string CreateGuildWarEvent(string eventType, string warID, string guildID, string description)
    {
        string eventID = "event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        GuildWarEvent guildWarEvent = new GuildWarEvent(eventID, warID, eventType, guildID, description);
        guildWarData.system.AddGuildWarEvent(guildWarEvent);
        SaveGuildWarData();
        Debug.Log("成功创建公会战事件: " + eventType);
        return eventID;
    }
    
    public void MarkEventAsCompleted(string eventID)
    {
        GuildWarEvent guildWarEvent = guildWarData.system.GetGuildWarEvent(eventID);
        if (guildWarEvent != null)
        {
            guildWarEvent.MarkAsCompleted();
            SaveGuildWarData();
            Debug.Log("成功标记公会战事件为完成");
        }
        else
        {
            Debug.LogError("公会战事件不存在: " + eventID);
        }
    }
    
    public void MarkEventAsFailed(string eventID)
    {
        GuildWarEvent guildWarEvent = guildWarData.system.GetGuildWarEvent(eventID);
        if (guildWarEvent != null)
        {
            guildWarEvent.MarkAsFailed();
            SaveGuildWarData();
            Debug.Log("成功标记公会战事件为失败");
        }
        else
        {
            Debug.LogError("公会战事件不存在: " + eventID);
        }
    }
    
    public List<GuildWarEvent> GetGuildWarEvents(string warID)
    {
        return guildWarData.system.GetGuildWarEventsByWar(warID);
    }
    
    // 数据持久化
    public void SaveGuildWarData()
    {
        string path = Application.dataPath + "/Data/guild_war_system_detailed_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, guildWarData);
        stream.Close();
    }
    
    public void LoadGuildWarData()
    {
        string path = Application.dataPath + "/Data/guild_war_system_detailed_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            guildWarData = (GuildWarSystemDetailedManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            guildWarData = new GuildWarSystemDetailedManagerData();
        }
    }
}
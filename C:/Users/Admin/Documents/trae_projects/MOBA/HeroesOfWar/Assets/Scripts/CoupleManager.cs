using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CoupleManager : MonoBehaviour
{
    public static CoupleManager Instance { get; private set; }
    
    public CoupleManagerData coupleData;
    
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
        LoadCoupleData();
        
        if (coupleData == null)
        {
            coupleData = new CoupleManagerData();
            InitializeDefaultCoupleRewards();
        }
    }
    
    private void InitializeDefaultCoupleRewards()
    {
        // 创建默认情侣奖励
        CoupleReward reward1 = new CoupleReward("reward_1", "情侣头像框", "情侣专属头像框", 1, "AvatarFrame", "couple_avatar_frame_1");
        CoupleReward reward2 = new CoupleReward("reward_2", "情侣名片", "情侣专属名片", 2, "BusinessCard", "couple_business_card_1");
        CoupleReward reward3 = new CoupleReward("reward_3", "情侣回城", "情侣专属回城特效", 3, "RecallEffect", "couple_recall_effect_1");
        CoupleReward reward4 = new CoupleReward("reward_4", "情侣动作", "情侣专属互动动作", 4, "EmoteAction", "couple_emote_action_1");
        CoupleReward reward5 = new CoupleReward("reward_5", "情侣皮肤", "情侣专属皮肤", 5, "Skin", "couple_skin_1");
        
        coupleData.system.AddReward(reward1);
        coupleData.system.AddReward(reward2);
        coupleData.system.AddReward(reward3);
        coupleData.system.AddReward(reward4);
        coupleData.system.AddReward(reward5);
        
        SaveCoupleData();
    }
    
    public string CreateCouple(string player1ID, string player1Name, string player2ID, string player2Name)
    {
        // 检查双方是否已经有情侣关系
        if (GetCoupleByPlayer(player1ID) != null || GetCoupleByPlayer(player2ID) != null)
        {
            Debug.Log("至少有一方已经有情侣关系");
            return "";
        }
        
        string coupleID = System.Guid.NewGuid().ToString();
        CoupleRelation couple = new CoupleRelation(coupleID, player1ID, player1Name, player2ID, player2Name);
        coupleData.system.AddCouple(couple);
        SaveCoupleData();
        Debug.Log($"成功创建情侣关系: {player1Name} 和 {player2Name}");
        return coupleID;
    }
    
    public void BreakCouple(string playerID)
    {
        CoupleRelation couple = GetCoupleByPlayer(playerID);
        if (couple != null)
        {
            string partnerID = couple.GetPartnerID(playerID);
            string partnerName = couple.GetPartnerName(playerID);
            string playerName = playerID == couple.player1ID ? couple.player1Name : couple.player2Name;
            
            couple.SetStatus("Broken");
            SaveCoupleData();
            Debug.Log($"{playerName} 和 {partnerName} 的情侣关系已解除");
        }
    }
    
    public void AddIntimacy(string playerID, int amount)
    {
        CoupleRelation couple = GetCoupleByPlayer(playerID);
        if (couple != null && couple.status == "Active")
        {
            couple.AddIntimacy(amount);
            SaveCoupleData();
            Debug.Log($"成功增加 {amount} 点亲密度");
        }
    }
    
    public void ClaimCoupleReward(string playerID, string rewardID)
    {
        CoupleRelation couple = GetCoupleByPlayer(playerID);
        if (couple != null)
        {
            CoupleReward reward = coupleData.system.GetReward(rewardID);
            if (reward != null && couple.coupleLevel >= reward.requiredLevel && !reward.isClaimed)
            {
                reward.Claim();
                SaveCoupleData();
                Debug.Log($"成功领取情侣奖励: {reward.rewardName}");
            }
            else
            {
                Debug.Log("领取条件不满足");
            }
        }
    }
    
    public CoupleRelation GetCouple(string coupleID)
    {
        return coupleData.system.GetCouple(coupleID);
    }
    
    public CoupleRelation GetCoupleByPlayer(string playerID)
    {
        return coupleData.system.GetCoupleByPlayer(playerID);
    }
    
    public List<CoupleReward> GetAvailableRewards(string playerID)
    {
        CoupleRelation couple = GetCoupleByPlayer(playerID);
        if (couple != null)
        {
            return coupleData.system.GetRewardsByLevel(couple.coupleLevel);
        }
        return new List<CoupleReward>();
    }
    
    public string GetPartnerID(string playerID)
    {
        CoupleRelation couple = GetCoupleByPlayer(playerID);
        return couple != null ? couple.GetPartnerID(playerID) : "";
    }
    
    public string GetPartnerName(string playerID)
    {
        CoupleRelation couple = GetCoupleByPlayer(playerID);
        return couple != null ? couple.GetPartnerName(playerID) : "";
    }
    
    public int GetCoupleLevel(string playerID)
    {
        CoupleRelation couple = GetCoupleByPlayer(playerID);
        return couple != null ? couple.coupleLevel : 0;
    }
    
    public int GetIntimacy(string playerID)
    {
        CoupleRelation couple = GetCoupleByPlayer(playerID);
        return couple != null ? couple.intimacy : 0;
    }
    
    public bool IsInCouple(string playerID)
    {
        CoupleRelation couple = GetCoupleByPlayer(playerID);
        return couple != null && couple.status == "Active";
    }
    
    public void SaveCoupleData()
    {
        string path = Application.dataPath + "/Data/couple_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, coupleData);
        stream.Close();
    }
    
    public void LoadCoupleData()
    {
        string path = Application.dataPath + "/Data/couple_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            coupleData = (CoupleManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            coupleData = new CoupleManagerData();
        }
    }
}
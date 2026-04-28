using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ShareManager : MonoBehaviour
{
    public static ShareManager Instance { get; private set; }
    
    public ShareManagerData shareData;
    
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
        LoadShareData();
        
        if (shareData == null)
        {
            shareData = new ShareManagerData();
            InitializeDefaultShareContents();
        }
    }
    
    private void InitializeDefaultShareContents()
    {
        ShareContent matchShare = new ShareContent(
            "share_match",
            "Match",
            "精彩比赛",
            "分享我的精彩比赛",
            "我在《王者荣耀》中刚刚进行了一场精彩的比赛！#王者荣耀 #MOBA"
        );
        shareData.AddShareContent(matchShare);
        
        ShareContent rankShare = new ShareContent(
            "share_rank",
            "Rank",
            "段位提升",
            "分享我的段位成就",
            "恭喜自己！终于达到【{0}】段位了！#王者荣耀 #段位"
        );
        shareData.AddShareContent(rankShare);
        
        ShareContent skinShare = new ShareContent(
            "share_skin",
            "Skin",
            "新皮肤",
            "分享我的新皮肤",
            "获得了新皮肤「{0}」，太帅了！#王者荣耀 #皮肤"
        );
        shareData.AddShareContent(skinShare);
        
        ShareContent heroShare = new ShareContent(
            "share_hero",
            "Hero",
            "英雄成就",
            "分享我的英雄成就",
            "我的{0}已经达到了【{1}】熟练度！#王者荣耀 #英雄"
        );
        shareData.AddShareContent(heroShare);
        
        ShareContent mvpShare = new ShareContent(
            "share_mvp",
            "MVP",
            "MVP时刻",
            "分享我的MVP时刻",
            "在本场比赛中获得了MVP！#王者荣耀 #MVP"
        );
        shareData.AddShareContent(mvpShare);
        
        SaveShareData();
    }
    
    public void ShareMatchResult(string matchID, string matchResult)
    {
        string recordID = "share_" + System.DateTime.Now.Ticks;
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        
        ShareRecord record = new ShareRecord(recordID, playerID, "share_match", "Match", "Default");
        shareData.AddShareRecord(record);
        shareData.totalShareCount++;
        
        SaveShareData();
    }
    
    public void ShareRankUp(int newRank)
    {
        string recordID = "share_" + System.DateTime.Now.Ticks;
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        
        ShareRecord record = new ShareRecord(recordID, playerID, "share_rank", "Rank", "Default");
        shareData.AddShareRecord(record);
        shareData.totalShareCount++;
        
        SaveShareData();
    }
    
    public void ShareNewSkin(string skinID, string skinName)
    {
        string recordID = "share_" + System.DateTime.Now.Ticks;
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        
        ShareRecord record = new ShareRecord(recordID, playerID, "share_skin", "Skin", "Default");
        shareData.AddShareRecord(record);
        shareData.totalShareCount++;
        
        SaveShareData();
    }
    
    public void ShareHeroMastery(string heroID, string heroName, int masteryLevel)
    {
        string recordID = "share_" + System.DateTime.Now.Ticks;
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        
        ShareRecord record = new ShareRecord(recordID, playerID, "share_hero", "Hero", "Default");
        shareData.AddShareRecord(record);
        shareData.totalShareCount++;
        
        SaveShareData();
    }
    
    public void ShareMVP(string matchID)
    {
        string recordID = "share_" + System.DateTime.Now.Ticks;
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        
        ShareRecord record = new ShareRecord(recordID, playerID, "share_mvp", "MVP", "Default");
        shareData.AddShareRecord(record);
        shareData.totalShareCount++;
        
        SaveShareData();
    }
    
    public void RecordShare(string contentID, string platform)
    {
        string recordID = "share_" + System.DateTime.Now.Ticks;
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        
        ShareContent content = shareData.GetShareContent(contentID);
        string contentType = content != null ? content.contentType : "Unknown";
        
        ShareRecord record = new ShareRecord(recordID, playerID, contentID, contentType, platform);
        record.MarkAsSuccessful();
        shareData.AddShareRecord(record);
        shareData.totalShareCount++;
        
        SaveShareData();
    }
    
    public string GetShareText(string contentID, params string[] args)
    {
        ShareContent content = shareData.GetShareContent(contentID);
        if (content != null)
        {
            return string.Format(content.templateText, args);
        }
        return "";
    }
    
    public List<ShareContent> GetShareContentsByType(string type)
    {
        return shareData.GetShareContentsByType(type);
    }
    
    public List<ShareRecord> GetPlayerShareRecords()
    {
        return shareData.GetShareRecordsByPlayer(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public int GetTotalShareCount()
    {
        return shareData.totalShareCount;
    }
    
    public void SaveShareData()
    {
        string path = Application.dataPath + "/Data/share_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, shareData);
        stream.Close();
    }
    
    public void LoadShareData()
    {
        string path = Application.dataPath + "/Data/share_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            shareData = (ShareManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            shareData = new ShareManagerData();
        }
    }
}
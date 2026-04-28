using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DailyQuizManager : MonoBehaviour
{
    public static DailyQuizManager Instance { get; private set; }
    
    public DailyQuizManagerData dailyQuizData;
    
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
        LoadDailyQuizData();
        
        if (dailyQuizData == null)
        {
            dailyQuizData = new DailyQuizManagerData();
            InitializeDefaultQuiz();
        }
    }
    
    private void InitializeDefaultQuiz()
    {
        // 创建默认题目分类
        string[] categories = { "英雄知识", "游戏技巧", "游戏规则", "游戏历史" };
        string[] difficulties = { "Easy", "Medium", "Hard" };
        
        // 创建默认题目
        List<string> options1 = new List<string> { "刺客", "战士", "坦克", "辅助" };
        QuizQuestion question1 = new QuizQuestion(
            "q1",
            "关羽在王者荣耀中的定位是什么？",
            options1,
            1,
            "英雄知识",
            "Easy",
            "关羽的定位是战士，以高机动性和爆发伤害著称。"
        );
        
        List<string> options2 = new List<string> { "1000", "2000", "3000", "4000" };
        QuizQuestion question2 = new QuizQuestion(
            "q2",
            "王者荣耀地图中一条线路有多少个防御塔？",
            options2,
            1,
            "游戏规则",
            "Medium",
            "每条线路有4个防御塔，包括1个一塔、1个二塔、1个高地塔和1个水晶塔。"
        );
        
        List<string> options3 = new List<string> { "15分钟", "20分钟", "25分钟", "30分钟" };
        QuizQuestion question3 = new QuizQuestion(
            "q3",
            "王者荣耀标准比赛时长大约是多少？",
            options3,
            1,
            "游戏规则",
            "Medium",
            "王者荣耀标准比赛时长大约为15-20分钟。"
        );
        
        List<string> options4 = new List<string> { "2015年", "2016年", "2017年", "2018年" };
        QuizQuestion question4 = new QuizQuestion(
            "q4",
            "王者荣耀首次上线是在哪一年？",
            options4,
            0,
            "游戏历史",
            "Easy",
            "王者荣耀首次上线是在2015年。"
        );
        
        List<string> options5 = new List<string> { "韩信", "李白", "孙悟空", "铠" };
        QuizQuestion question5 = new QuizQuestion(
            "q5",
            "以下哪个英雄被玩家称为'韩信'？",
            options5,
            0,
            "英雄知识",
            "Easy",
            "韩信是王者荣耀中的一个刺客英雄，以高机动性著称。"
        );
        
        dailyQuizData.system.AddQuestion(question1);
        dailyQuizData.system.AddQuestion(question2);
        dailyQuizData.system.AddQuestion(question3);
        dailyQuizData.system.AddQuestion(question4);
        dailyQuizData.system.AddQuestion(question5);
        
        // 创建默认奖励
        QuizReward reward1 = new QuizReward("r1", "答对1题奖励", 1, "Gold", 50);
        QuizReward reward2 = new QuizReward("r2", "答对3题奖励", 3, "Gold", 150);
        QuizReward reward3 = new QuizReward("r3", "答对5题奖励", 5, "Gold", 300);
        
        dailyQuizData.system.AddReward(reward1);
        dailyQuizData.system.AddReward(reward2);
        dailyQuizData.system.AddReward(reward3);
        
        SaveDailyQuizData();
    }
    
    public string StartQuizSession(string playerID, string playerName, int questionCount = 5)
    {
        List<QuizQuestion> questions = dailyQuizData.system.GetRandomQuestions(questionCount);
        if (questions.Count == 0)
        {
            Debug.Log("没有可用的题目");
            return "";
        }
        
        string sessionID = System.Guid.NewGuid().ToString();
        QuizSession newSession = new QuizSession(sessionID, playerID, playerName, questions);
        dailyQuizData.system.AddSession(newSession);
        SaveDailyQuizData();
        Debug.Log($"成功开始答题: {sessionID}");
        return sessionID;
    }
    
    public void SubmitAnswer(string sessionID, int questionIndex, int answer)
    {
        QuizSession session = dailyQuizData.system.GetSession(sessionID);
        if (session != null && !session.isCompleted && questionIndex < session.questions.Count)
        {
            session.SubmitAnswer(questionIndex, answer);
            SaveDailyQuizData();
            
            if (session.answers.Count == session.questions.Count)
            {
                CompleteQuizSession(sessionID);
            }
        }
    }
    
    public void CompleteQuizSession(string sessionID)
    {
        QuizSession session = dailyQuizData.system.GetSession(sessionID);
        if (session != null)
        {
            session.Complete();
            
            // 发放奖励
            foreach (QuizReward reward in dailyQuizData.system.rewards)
            {
                if (session.correctCount >= reward.requiredCorrect)
                {
                    if (reward.rewardType == "Gold")
                    {
                        ProfileManager.Instance.currentProfile.gold += reward.rewardAmount;
                    }
                    else if (reward.rewardType == "Gems")
                    {
                        ProfileManager.Instance.currentProfile.gems += reward.rewardAmount;
                    }
                }
            }
            
            ProfileManager.Instance.SaveProfile();
            SaveDailyQuizData();
            Debug.Log($"答题完成: 正确{session.correctCount}题，错误{session.wrongCount}题，总分{session.score}");
        }
    }
    
    public QuizSession GetQuizSession(string sessionID)
    {
        return dailyQuizData.system.GetSession(sessionID);
    }
    
    public List<QuizQuestion> GetQuestionsByCategory(string category)
    {
        return dailyQuizData.system.GetQuestionsByCategory(category);
    }
    
    public List<QuizReward> GetAllRewards()
    {
        return dailyQuizData.system.rewards;
    }
    
    public QuizReward GetReward(string rewardID)
    {
        return dailyQuizData.system.GetReward(rewardID);
    }
    
    public void AddQuestion(string text, List<string> options, int correct, string category, string difficulty, string explanation)
    {
        string questionID = System.Guid.NewGuid().ToString();
        QuizQuestion newQuestion = new QuizQuestion(questionID, text, options, correct, category, difficulty, explanation);
        dailyQuizData.system.AddQuestion(newQuestion);
        SaveDailyQuizData();
        Debug.Log($"成功添加题目: {text}");
    }
    
    public void AddReward(string name, int required, string type, int amount, string itemID = "")
    {
        string rewardID = System.Guid.NewGuid().ToString();
        QuizReward newReward = new QuizReward(rewardID, name, required, type, amount, itemID);
        dailyQuizData.system.AddReward(newReward);
        SaveDailyQuizData();
        Debug.Log($"成功添加奖励: {name}");
    }
    
    public List<QuizSession> GetPlayerSessions(string playerID, int limit = 50)
    {
        List<QuizSession> sessions = new List<QuizSession>();
        foreach (QuizSession session in dailyQuizData.system.sessions)
        {
            if (session.playerID == playerID)
            {
                sessions.Add(session);
            }
        }
        sessions.Sort((a, b) => b.startTime.CompareTo(a.startTime));
        return sessions.GetRange(0, Mathf.Min(limit, sessions.Count));
    }
    
    public void SaveDailyQuizData()
    {
        string path = Application.dataPath + "/Data/daily_quiz_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, dailyQuizData);
        stream.Close();
    }
    
    public void LoadDailyQuizData()
    {
        string path = Application.dataPath + "/Data/daily_quiz_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            dailyQuizData = (DailyQuizManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            dailyQuizData = new DailyQuizManagerData();
        }
    }
}
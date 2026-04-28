using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class QuizQuestion
{
    public string QuestionID;
    public string QuestionContent;
    public List<string> Options;
    public int CorrectAnswer;
    public int QuestionType;
    public string Explanation;
    public int Difficulty;
    public string Category;
    public int PointValue;
    public DateTime CreateTime;

    public QuizQuestion(string questionID, string questionContent, List<string> options, int correctAnswer, string explanation, int difficulty, string category)
    {
        QuestionID = questionID;
        QuestionContent = questionContent;
        Options = options;
        CorrectAnswer = correctAnswer;
        QuestionType = 1;
        Explanation = explanation;
        Difficulty = difficulty;
        Category = category;
        PointValue = 10;
        CreateTime = DateTime.Now;
    }
}

[Serializable]
public class PlayerQuizRecord
{
    public string PlayerID;
    public int TotalAnswered;
    public int CorrectAnswers;
    public int WrongAnswers;
    public double AccuracyRate;
    public int TotalPoints;
    public int TodayAnswered;
    public int TodayCorrect;
    public DateTime LastAnswerTime;
    public List<string> AnsweredQuestionIDs;
    public int ConsecutiveCorrectDays;
    public DateTime LastCorrectDay;

    public PlayerQuizRecord(string playerID)
    {
        PlayerID = playerID;
        TotalAnswered = 0;
        CorrectAnswers = 0;
        WrongAnswers = 0;
        AccuracyRate = 0.0;
        TotalPoints = 0;
        TodayAnswered = 0;
        TodayCorrect = 0;
        LastAnswerTime = DateTime.Now;
        AnsweredQuestionIDs = new List<string>();
        ConsecutiveCorrectDays = 0;
        LastCorrectDay = DateTime.MinValue;
    }

    public void UpdateStats()
    {
        if (TotalAnswered > 0)
        {
            AccuracyRate = (double)CorrectAnswers / TotalAnswered * 100.0;
        }
    }
}

[Serializable]
public class DailyQuiz
{
    public string QuizID;
    public DateTime QuizDate;
    public List<string> QuestionIDs;
    public int TotalQuestions;
    public bool IsCompleted;
    public int PlayerScore;
    public DateTime? CompletedTime;

    public DailyQuiz(string quizID, DateTime quizDate)
    {
        QuizID = quizID;
        QuizDate = quizDate;
        QuestionIDs = new List<string>();
        TotalQuestions = 5;
        IsCompleted = false;
        PlayerScore = 0;
        CompletedTime = null;
    }
}

[Serializable]
public class QuizReward
{
    public string RewardID;
    public string RewardName;
    public int RequiredScore;
    public int RewardType;
    public int RewardValue;
    public string Description;

    public QuizReward(string rewardID, string rewardName, int requiredScore, int rewardType, int rewardValue, string description)
    {
        RewardID = rewardID;
        RewardName = rewardName;
        RequiredScore = requiredScore;
        RewardType = rewardType;
        RewardValue = rewardValue;
        Description = description;
    }
}

[Serializable]
public class QuizSystemData
{
    public List<QuizQuestion> AllQuestions;
    public Dictionary<string, PlayerQuizRecord> PlayerRecords;
    public List<DailyQuiz> DailyQuizzes;
    public List<QuizReward> Rewards;
    public Dictionary<string, List<string>> PlayerDailyQuizHistory;
    public List<string> FeaturedQuestionIDs;
    public DateTime LastCleanupTime;

    public QuizSystemData()
    {
        AllQuestions = new List<QuizQuestion>();
        PlayerRecords = new Dictionary<string, PlayerQuizRecord>();
        DailyQuizzes = new List<DailyQuiz>();
        Rewards = new List<QuizReward>();
        PlayerDailyQuizHistory = new Dictionary<string, List<string>>();
        FeaturedQuestionIDs = new List<string>();
        LastCleanupTime = DateTime.Now;
        InitializeDefaultQuestions();
        InitializeDefaultRewards();
    }

    private void InitializeDefaultQuestions()
    {
        List<string> options1 = new List<string> { "1技能", "2技能", "3技能", "普通攻击" };
        AddQuestion(new QuizQuestion("q_001", "孙悟空的1技能是什么？", options1, 0, "孙悟空的1技能是护身咒法，可以抵挡一次敌方技能", 1, "英雄技能"));

        List<string> options2 = new List<string> { "物理攻击", "法术攻击", "物理防御", "法术防御" };
        AddQuestion(new QuizQuestion("q_002", "破军属于什么类型的装备？", options2, 0, "破军是一件物理攻击装备，提供高额物理攻击力", 1, "装备理解"));

        List<string> options3 = new List<string> { "击杀小兵", "击杀英雄", "击杀野怪", "助攻" };
        AddQuestion(new QuizQuestion("q_003", "王者荣耀中，获得金币的主要方式是什么？", options3, 0, "击杀小兵是获得金币的主要方式", 1, "游戏机制"));

        List<string> options4 = new List<string> { "增加移动速度", "增加攻击速度", "减少技能冷却", "增加生命值" };
        AddQuestion(new QuizQuestion("q_004", "闪电匕首的主要属性是什么？", options4, 1, "闪电匕首主要提供攻击速度加成", 1, "装备理解"));
    }

    private void AddQuestion(QuizQuestion question)
    {
        AllQuestions.Add(question);
        if (question.Difficulty >= 3)
        {
            FeaturedQuestionIDs.Add(question.QuestionID);
        }
    }

    private void InitializeDefaultRewards()
    {
        Rewards.Add(new QuizReward("reward_001", "初出茅庐", 1, 1, 10, "回答正确1次"));
        Rewards.Add(new QuizReward("reward_002", "知识达人", 50, 1, 100, "累计获得50积分"));
        Rewards.Add(new QuizReward("reward_003", "答题专家", 100, 1, 200, "累计获得100积分"));
        Rewards.Add(new QuizReward("reward_004", "全对奖励", 5, 1, 50, "每日答题全部正确"));
    }

    public void AddPlayerRecord(string playerID, PlayerQuizRecord record)
    {
        PlayerRecords[playerID] = record;
    }

    public void AddDailyQuiz(DailyQuiz quiz)
    {
        DailyQuizzes.Add(quiz);
    }
}

[Serializable]
public class QuizEvent
{
    public string EventID;
    public string EventType;
    public DateTime EventTime;
    public string PlayerID;
    public string QuestionID;
    public string EventData;

    public QuizEvent(string eventID, string eventType, string playerID, string questionID, string eventData)
    {
        EventID = eventID;
        EventType = eventType;
        EventTime = DateTime.Now;
        PlayerID = playerID;
        QuestionID = questionID;
        EventData = eventData;
    }
}

public class QuizSystemDataManager
{
    private static QuizSystemDataManager _instance;
    public static QuizSystemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new QuizSystemDataManager();
            }
            return _instance;
        }
    }

    public QuizSystemData quizData;
    private List<QuizEvent> recentEvents;
    private const int MaxRecentEvents = 100;

    private QuizSystemDataManager()
    {
        quizData = new QuizSystemData();
        recentEvents = new List<QuizEvent>();
        LoadQuizData();
    }

    public void SaveQuizData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "QuizSystemData.dat");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, quizData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存答题系统数据失败: " + e.Message);
        }
    }

    public void LoadQuizData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "SaveData", "QuizSystemData.dat");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    quizData = (QuizSystemData)bf.Deserialize(fs);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("加载答题系统数据失败: " + e.Message);
            quizData = new QuizSystemData();
        }
    }

    public void CreateQuizEvent(string eventType, string playerID, string questionID, string eventData)
    {
        string eventID = "quiz_event_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        QuizEvent quizEvent = new QuizEvent(eventID, eventType, playerID, questionID, eventData);
        recentEvents.Add(quizEvent);
        if (recentEvents.Count > MaxRecentEvents)
        {
            recentEvents.RemoveAt(0);
        }
    }

    public List<QuizEvent> GetRecentEvents(int count)
    {
        if (count > recentEvents.Count)
        {
            count = recentEvents.Count;
        }
        return recentEvents.GetRange(recentEvents.Count - count, count);
    }
}
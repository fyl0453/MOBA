using System;
using System.Collections.Generic;

public class QuizSystemDetailedManager
{
    private static QuizSystemDetailedManager _instance;
    public static QuizSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new QuizSystemDetailedManager();
            }
            return _instance;
        }
    }

    private QuizSystemData quizData;
    private QuizSystemDataManager dataManager;

    private QuizSystemDetailedManager()
    {
        dataManager = QuizSystemDataManager.Instance;
        quizData = dataManager.quizData;
    }

    public void InitializePlayerRecord(string playerID)
    {
        if (!quizData.PlayerRecords.ContainsKey(playerID))
        {
            quizData.PlayerRecords[playerID] = new PlayerQuizRecord(playerID);
            dataManager.CreateQuizEvent("record_init", playerID, "", "初始化答题记录");
            dataManager.SaveQuizData();
            Debug.Log("初始化玩家答题记录: " + playerID);
        }
    }

    public void CreateQuestion(string questionContent, List<string> options, int correctAnswer, string explanation, int difficulty, string category)
    {
        string questionID = "q_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        QuizQuestion question = new QuizQuestion(questionID, questionContent, options, correctAnswer, explanation, difficulty, category);
        quizData.AllQuestions.Add(question);
        dataManager.CreateQuizEvent("question_create", "", questionID, "创建题目: " + questionContent.Substring(0, Math.Min(20, questionContent.Length)));
        dataManager.SaveQuizData();
        Debug.Log("创建题目成功");
    }

    public DailyQuiz StartDailyQuiz(string playerID)
    {
        if (!quizData.PlayerRecords.ContainsKey(playerID))
        {
            InitializePlayerRecord(playerID);
        }

        DateTime today = DateTime.Today;
        string todayKey = today.ToString("yyyyMMdd");

        if (quizData.PlayerDailyQuizHistory.ContainsKey(playerID))
        {
            if (quizData.PlayerDailyQuizHistory[playerID].Contains(todayKey))
            {
                Debug.LogWarning("今日已完成答题");
                return null;
            }
        }

        string quizID = "daily_" + todayKey + "_" + playerID.Substring(0, 4);
        DailyQuiz dailyQuiz = new DailyQuiz(quizID, today);

        List<QuizQuestion> availableQuestions = GetRandomQuestions(5);
        foreach (QuizQuestion question in availableQuestions)
        {
            dailyQuiz.QuestionIDs.Add(question.QuestionID);
        }

        quizData.AddDailyQuiz(dailyQuiz);
        if (!quizData.PlayerDailyQuizHistory.ContainsKey(playerID))
        {
            quizData.PlayerDailyQuizHistory[playerID] = new List<string>();
        }
        quizData.PlayerDailyQuizHistory[playerID].Add(todayKey);

        dataManager.CreateQuizEvent("quiz_start", playerID, "", "开始每日答题: " + quizID);
        dataManager.SaveQuizData();
        Debug.Log("开始每日答题: " + quizID);
        return dailyQuiz;
    }

    private List<QuizQuestion> GetRandomQuestions(int count)
    {
        List<QuizQuestion> questions = new List<QuizQuestion>(quizData.AllQuestions);
        Random random = new Random();
        int n = questions.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            QuizQuestion temp = questions[k];
            questions[k] = questions[n];
            questions[n] = temp;
        }
        if (count < questions.Count)
        {
            return questions.GetRange(0, count);
        }
        return questions;
    }

    public bool AnswerQuestion(string playerID, string quizID, string questionID, int answer)
    {
        if (!quizData.PlayerRecords.ContainsKey(playerID))
        {
            InitializePlayerRecord(playerID);
        }

        QuizQuestion question = quizData.AllQuestions.Find(q => q.QuestionID == questionID);
        if (question == null)
        {
            Debug.LogError("题目不存在: " + questionID);
            return false;
        }

        PlayerQuizRecord record = quizData.PlayerRecords[playerID];
        DailyQuiz dailyQuiz = quizData.DailyQuizzes.Find(q => q.QuizID == quizID);
        if (dailyQuiz == null)
        {
            Debug.LogError("答题不存在: " + quizID);
            return false;
        }

        bool isCorrect = (answer == question.CorrectAnswer);
        record.TotalAnswered++;
        record.TodayAnswered++;
        record.LastAnswerTime = DateTime.Now;

        if (isCorrect)
        {
            record.CorrectAnswers++;
            record.TodayCorrect++;
            record.TotalPoints += question.PointValue;
            dailyQuiz.PlayerScore += question.PointValue;
            dataManager.CreateQuizEvent("answer_correct", playerID, questionID, "回答正确");
        }
        else
        {
            record.WrongAnswers++;
            dataManager.CreateQuizEvent("answer_wrong", playerID, questionID, "回答错误");
        }

        record.AnsweredQuestionIDs.Add(questionID);
        record.UpdateStats();

        if (record.ConsecutiveCorrectDays > 0 || isCorrect)
        {
            if (isCorrect && (DateTime.Today - record.LastCorrectDay).TotalDays == 1)
            {
                record.ConsecutiveCorrectDays++;
            }
            else if (isCorrect && record.ConsecutiveCorrectDays == 0)
            {
                record.ConsecutiveCorrectDays = 1;
            }
            else if (!isCorrect)
            {
                record.ConsecutiveCorrectDays = 0;
            }
            record.LastCorrectDay = DateTime.Today;
        }

        int answeredCount = 0;
        foreach (string qid in dailyQuiz.QuestionIDs)
        {
            if (record.AnsweredQuestionIDs.Contains(qid))
            {
                answeredCount++;
            }
        }

        if (answeredCount >= dailyQuiz.TotalQuestions)
        {
            CompleteDailyQuiz(playerID, quizID);
        }

        dataManager.SaveQuizData();
        Debug.Log("回答: " + (isCorrect ? "正确" : "错误"));
        return isCorrect;
    }

    public void CompleteDailyQuiz(string playerID, string quizID)
    {
        DailyQuiz dailyQuiz = quizData.DailyQuizzes.Find(q => q.QuizID == quizID);
        if (dailyQuiz != null)
        {
            dailyQuiz.IsCompleted = true;
            dailyQuiz.CompletedTime = DateTime.Now;
            dataManager.CreateQuizEvent("quiz_complete", playerID, "", "完成每日答题: 得分" + dailyQuiz.PlayerScore);
            dataManager.SaveQuizData();
            Debug.Log("完成每日答题: " + dailyQuiz.PlayerScore);
        }
    }

    public QuizQuestion GetQuestion(string questionID)
    {
        return quizData.AllQuestions.Find(q => q.QuestionID == questionID);
    }

    public List<QuizQuestion> GetQuestionsByCategory(string category)
    {
        return quizData.AllQuestions.FindAll(q => q.Category == category);
    }

    public List<QuizQuestion> GetQuestionsByDifficulty(int difficulty)
    {
        return quizData.AllQuestions.FindAll(q => q.Difficulty == difficulty);
    }

    public List<QuizQuestion> GetRandomQuestionsByDifficulty(int difficulty, int count)
    {
        List<QuizQuestion> questions = GetQuestionsByDifficulty(difficulty);
        Random random = new Random();
        int n = questions.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            QuizQuestion temp = questions[k];
            questions[k] = questions[n];
            questions[n] = temp;
        }
        if (count < questions.Count)
        {
            return questions.GetRange(0, count);
        }
        return questions;
    }

    public PlayerQuizRecord GetPlayerRecord(string playerID)
    {
        if (quizData.PlayerRecords.ContainsKey(playerID))
        {
            return quizData.PlayerRecords[playerID];
        }
        return null;
    }

    public DailyQuiz GetDailyQuiz(string quizID)
    {
        return quizData.DailyQuizzes.Find(q => q.QuizID == quizID);
    }

    public DailyQuiz GetTodayQuiz(string playerID)
    {
        DateTime today = DateTime.Today;
        return quizData.DailyQuizzes.FindLast(q => q.QuizDate == today && q.QuizID.Contains(playerID.Substring(0, 4)));
    }

    public bool HasCompletedTodayQuiz(string playerID)
    {
        if (quizData.PlayerDailyQuizHistory.ContainsKey(playerID))
        {
            string todayKey = DateTime.Today.ToString("yyyyMMdd");
            return quizData.PlayerDailyQuizHistory[playerID].Contains(todayKey);
        }
        return false;
    }

    public List<QuizReward> GetAllRewards()
    {
        return quizData.Rewards;
    }

    public QuizReward GetReward(string rewardID)
    {
        return quizData.Rewards.Find(r => r.RewardID == rewardID);
    }

    public List<QuizReward> GetAvailableRewards(string playerID)
    {
        if (!quizData.PlayerRecords.ContainsKey(playerID))
        {
            return new List<QuizReward>();
        }

        int totalPoints = quizData.PlayerRecords[playerID].TotalPoints;
        return quizData.Rewards.FindAll(r => totalPoints >= r.RequiredScore);
    }

    public int GetPlayerTotalPoints(string playerID)
    {
        if (quizData.PlayerRecords.ContainsKey(playerID))
        {
            return quizData.PlayerRecords[playerID].TotalPoints;
        }
        return 0;
    }

    public double GetPlayerAccuracyRate(string playerID)
    {
        if (quizData.PlayerRecords.ContainsKey(playerID))
        {
            return quizData.PlayerRecords[playerID].AccuracyRate;
        }
        return 0.0;
    }

    public int GetTodayCorrectCount(string playerID)
    {
        if (quizData.PlayerRecords.ContainsKey(playerID))
        {
            return quizData.PlayerRecords[playerID].TodayCorrect;
        }
        return 0;
    }

    public void ResetDailyStats()
    {
        foreach (var kvp in quizData.PlayerRecords)
        {
            kvp.Value.TodayAnswered = 0;
            kvp.Value.TodayCorrect = 0;
        }
        dataManager.SaveQuizData();
        Debug.Log("重置每日答题统计成功");
    }

    public List<QuizQuestion> SearchQuestions(string keyword)
    {
        return quizData.AllQuestions.FindAll(q =>
            q.QuestionContent.Contains(keyword) ||
            q.Category.Contains(keyword));
    }

    public int GetQuestionCount()
    {
        return quizData.AllQuestions.Count;
    }

    public Dictionary<string, int> GetCategoryStatistics()
    {
        Dictionary<string, int> stats = new Dictionary<string, int>();
        foreach (QuizQuestion question in quizData.AllQuestions)
        {
            if (stats.ContainsKey(question.Category))
            {
                stats[question.Category]++;
            }
            else
            {
                stats[question.Category] = 1;
            }
        }
        return stats;
    }

    public List<QuizEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }

    public void SaveData()
    {
        dataManager.SaveQuizData();
    }

    public void LoadData()
    {
        dataManager.LoadQuizData();
    }
}
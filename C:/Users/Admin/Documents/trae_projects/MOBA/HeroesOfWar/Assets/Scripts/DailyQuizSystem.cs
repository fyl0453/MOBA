[System.Serializable]
public class DailyQuizSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<QuizQuestion> questions;
    public List<QuizSession> sessions;
    public List<QuizReward> rewards;
    
    public DailyQuizSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        questions = new List<QuizQuestion>();
        sessions = new List<QuizSession>();
        rewards = new List<QuizReward>();
    }
    
    public void AddQuestion(QuizQuestion question)
    {
        questions.Add(question);
    }
    
    public void AddSession(QuizSession session)
    {
        sessions.Add(session);
    }
    
    public void AddReward(QuizReward reward)
    {
        rewards.Add(reward);
    }
    
    public QuizQuestion GetQuestion(string questionID)
    {
        return questions.Find(q => q.questionID == questionID);
    }
    
    public QuizSession GetSession(string sessionID)
    {
        return sessions.Find(s => s.sessionID == sessionID);
    }
    
    public QuizReward GetReward(string rewardID)
    {
        return rewards.Find(r => r.rewardID == rewardID);
    }
    
    public List<QuizQuestion> GetQuestionsByCategory(string category)
    {
        return questions.FindAll(q => q.category == category);
    }
    
    public List<QuizQuestion> GetRandomQuestions(int count)
    {
        List<QuizQuestion> shuffled = new List<QuizQuestion>(questions);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            QuizQuestion temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }
        return shuffled.GetRange(0, Mathf.Min(count, shuffled.Count));
    }
}

[System.Serializable]
public class QuizQuestion
{
    public string questionID;
    public string questionText;
    public List<string> options;
    public int correctAnswer;
    public string category;
    public string difficulty;
    public string explanation;
    
    public QuizQuestion(string id, string text, List<string> options, int correct, string category, string difficulty, string explanation)
    {
        questionID = id;
        questionText = text;
        this.options = options;
        correctAnswer = correct;
        this.category = category;
        this.difficulty = difficulty;
        this.explanation = explanation;
    }
    
    public bool CheckAnswer(int answer)
    {
        return answer == correctAnswer;
    }
}

[System.Serializable]
public class QuizSession
{
    public string sessionID;
    public string playerID;
    public string playerName;
    public List<QuizQuestion> questions;
    public List<int> answers;
    public int correctCount;
    public int wrongCount;
    public int score;
    public string startTime;
    public string endTime;
    public string status;
    public bool isCompleted;
    
    public QuizSession(string id, string playerID, string playerName, List<QuizQuestion> questions)
    {
        sessionID = id;
        this.playerID = playerID;
        this.playerName = playerName;
        this.questions = questions;
        answers = new List<int>();
        correctCount = 0;
        wrongCount = 0;
        score = 0;
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        endTime = "";
        status = "InProgress";
        isCompleted = false;
    }
    
    public void SubmitAnswer(int questionIndex, int answer)
    {
        answers.Add(answer);
        if (questions[questionIndex].CheckAnswer(answer))
        {
            correctCount++;
            score += 10;
        }
        else
        {
            wrongCount++;
        }
    }
    
    public void Complete()
    {
        status = "Completed";
        endTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        isCompleted = true;
    }
}

[System.Serializable]
public class QuizReward
{
    public string rewardID;
    public string rewardName;
    public int requiredCorrect;
    public string rewardType;
    public int rewardAmount;
    public string rewardItemID;
    
    public QuizReward(string id, string name, int required, string type, int amount, string itemID = "")
    {
        rewardID = id;
        rewardName = name;
        requiredCorrect = required;
        rewardType = type;
        rewardAmount = amount;
        rewardItemID = itemID;
    }
}

[System.Serializable]
public class DailyQuizManagerData
{
    public DailyQuizSystem system;
    
    public DailyQuizManagerData()
    {
        system = new DailyQuizSystem("daily_quiz_system", "每日答题系统", "管理游戏知识问答");
    }
}
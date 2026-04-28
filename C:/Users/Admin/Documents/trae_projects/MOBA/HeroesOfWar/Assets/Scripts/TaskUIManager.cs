using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TaskUIManager : MonoBehaviour
{
    public static TaskUIManager Instance { get; private set; }
    
    public Canvas taskCanvas;
    public ScrollRect tasksScrollRect;
    public Transform tasksContent;
    public Text taskNameText;
    public Text taskDescriptionText;
    public Text taskProgressText;
    public Text taskRewardText;
    public Button claimRewardButton;
    public GameObject taskPrefab;
    
    private enum TaskTypeFilter
    {
        Daily,
        Weekly,
        Monthly,
        Achievement
    }
    
    private TaskTypeFilter currentFilter = TaskTypeFilter.Daily;
    private Task selectedTask;
    
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
        taskCanvas.gameObject.SetActive(false);
        claimRewardButton.onClick.AddListener(ClaimReward);
    }
    
    public void OpenTaskUI()
    {
        taskCanvas.gameObject.SetActive(true);
        UpdateTasksList();
        ClearTaskDetails();
    }
    
    public void CloseTaskUI()
    {
        taskCanvas.gameObject.SetActive(false);
    }
    
    public void SetTaskTypeFilter(int filterIndex)
    {
        switch (filterIndex)
        {
            case 0:
                currentFilter = TaskTypeFilter.Daily;
                break;
            case 1:
                currentFilter = TaskTypeFilter.Weekly;
                break;
            case 2:
                currentFilter = TaskTypeFilter.Monthly;
                break;
            case 3:
                currentFilter = TaskTypeFilter.Achievement;
                break;
        }
        UpdateTasksList();
    }
    
    public void UpdateTasksList()
    {
        // 清空现有内容
        foreach (Transform child in tasksContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示任务列表
        List<Task> tasks = GetTasksByFilter();
        foreach (Task task in tasks)
        {
            GameObject taskObj = Instantiate(taskPrefab, tasksContent);
            Text[] texts = taskObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = task.taskName;
                texts[1].text = task.taskDescription;
                texts[2].text = $"{task.currentValue}/{task.targetValue}";
            }
            
            // 设置任务状态
            Image[] images = taskObj.GetComponentsInChildren<Image>();
            if (images.Length >= 2)
            {
                // 任务状态图标
                if (task.isCompleted)
                {
                    images[0].color = Color.green;
                }
                else
                {
                    images[0].color = Color.gray;
                }
                
                // 进度条
                images[1].fillAmount = task.GetProgress();
            }
            
            // 添加点击事件
            Button button = taskObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectTask(task));
            }
        }
    }
    
    private List<Task> GetTasksByFilter()
    {
        switch (currentFilter)
        {
            case TaskTypeFilter.Daily:
                return TaskManager.Instance.GetDailyTasks();
            case TaskTypeFilter.Weekly:
                return TaskManager.Instance.GetWeeklyTasks();
            case TaskTypeFilter.Monthly:
                return TaskManager.Instance.GetMonthlyTasks();
            case TaskTypeFilter.Achievement:
                return TaskManager.Instance.GetAchievementTasks();
            default:
                return new List<Task>();
        }
    }
    
    public void SelectTask(Task task)
    {
        selectedTask = task;
        taskNameText.text = task.taskName;
        taskDescriptionText.text = task.taskDescription;
        taskProgressText.text = $"进度: {task.currentValue}/{task.targetValue}";
        taskRewardText.text = $"奖励: {task.reward.gold} 金币, {task.reward.experience} 经验";
        
        // 更新领取按钮状态
        if (task.isCompleted && !task.isClaimed)
        {
            claimRewardButton.gameObject.SetActive(true);
        }
        else
        {
            claimRewardButton.gameObject.SetActive(false);
        }
    }
    
    public void ClearTaskDetails()
    {
        selectedTask = null;
        taskNameText.text = "";
        taskDescriptionText.text = "";
        taskProgressText.text = "";
        taskRewardText.text = "";
        claimRewardButton.gameObject.SetActive(false);
    }
    
    public void ClaimReward()
    {
        if (selectedTask != null && selectedTask.isCompleted && !selectedTask.isClaimed)
        {
            TaskManager.Instance.ClaimTaskReward(selectedTask);
            UpdateTasksList();
            ClearTaskDetails();
        }
    }
}
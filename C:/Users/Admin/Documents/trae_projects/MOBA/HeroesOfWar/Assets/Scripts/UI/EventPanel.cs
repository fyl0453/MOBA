using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EventPanel : UIPanel
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button allButton;
    [SerializeField] private Button activeButton;
    [SerializeField] private Button newbieButton;
    [SerializeField] private Button limitedButton;
    [SerializeField] private Button holidayButton;
    [SerializeField] private Button weekendButton;
    [SerializeField] private Button loginButton;
    
    [SerializeField] private GameObject eventListContent;
    [SerializeField] private GameObject eventItemPrefab;
    [SerializeField] private GameObject eventDetailPanel;
    
    // 活动详情
    [SerializeField] private Text eventNameText;
    [SerializeField] private Text eventDescriptionText;
    [SerializeField] private Text eventTimeText;
    [SerializeField] private GameObject taskListContent;
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private Button backToEventListButton;
    
    private List<EventItem> eventItems = new List<EventItem>();
    private List<TaskItem> taskItems = new List<TaskItem>();
    private Event selectedEvent;
    
    private void Start()
    {
        backButton.onClick.AddListener(OnBack);
        allButton.onClick.AddListener(() => ShowEvents("All"));
        activeButton.onClick.AddListener(() => ShowEvents("Active"));
        newbieButton.onClick.AddListener(() => ShowEventsByType(EventType.Newbie));
        limitedButton.onClick.AddListener(() => ShowEventsByType(EventType.Limited));
        holidayButton.onClick.AddListener(() => ShowEventsByType(EventType.Holiday));
        weekendButton.onClick.AddListener(() => ShowEventsByType(EventType.Weekend));
        loginButton.onClick.AddListener(() => ShowEventsByType(EventType.Login));
        backToEventListButton.onClick.AddListener(() => SwitchPanel("EventList"));
        
        SwitchPanel("EventList");
        ShowEvents("All");
    }
    
    private void SwitchPanel(string panelName)
    {
        eventListContent.SetActive(panelName == "EventList");
        eventDetailPanel.SetActive(panelName == "EventDetail");
    }
    
    private void ShowEvents(string filter)
    {
        // 清空现有列表
        foreach (var item in eventItems)
        {
            Destroy(item.gameObject);
        }
        eventItems.Clear();
        
        // 获取活动列表
        List<Event> events = new List<Event>();
        
        switch (filter)
        {
            case "All":
                events = EventSystem.Instance.GetAllEvents();
                break;
            case "Active":
                events = EventSystem.Instance.GetActiveEvents();
                break;
        }
        
        foreach (Event e in events)
        {
            GameObject eventItemGO = Instantiate(eventItemPrefab, eventListContent.transform);
            EventItem eventItem = eventItemGO.GetComponent<EventItem>();
            if (eventItem != null)
            {
                eventItem.Setup(e, OnEventSelect);
                eventItems.Add(eventItem);
            }
        }
    }
    
    private void ShowEventsByType(EventType eventType)
    {
        // 清空现有列表
        foreach (var item in eventItems)
        {
            Destroy(item.gameObject);
        }
        eventItems.Clear();
        
        // 获取活动列表
        List<Event> events = EventSystem.Instance.GetEventsByType(eventType);
        
        foreach (Event e in events)
        {
            GameObject eventItemGO = Instantiate(eventItemPrefab, eventListContent.transform);
            EventItem eventItem = eventItemGO.GetComponent<EventItem>();
            if (eventItem != null)
            {
                eventItem.Setup(e, OnEventSelect);
                eventItems.Add(eventItem);
            }
        }
    }
    
    private void OnEventSelect(Event e)
    {
        selectedEvent = e;
        ShowEventDetail(e);
    }
    
    private void ShowEventDetail(Event e)
    {
        eventNameText.text = e.eventName;
        eventDescriptionText.text = e.description;
        eventTimeText.text = $"{e.startTime} 至 {e.endTime}";
        
        // 清空现有任务列表
        foreach (var item in taskItems)
        {
            Destroy(item.gameObject);
        }
        taskItems.Clear();
        
        // 显示任务列表
        foreach (EventTask task in e.tasks)
        {
            GameObject taskItemGO = Instantiate(taskItemPrefab, taskListContent.transform);
            TaskItem taskItem = taskItemGO.GetComponent<TaskItem>();
            if (taskItem != null)
            {
                taskItem.Setup(task, e.eventID);
                taskItems.Add(taskItem);
            }
        }
        
        SwitchPanel("EventDetail");
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

public class EventItem : MonoBehaviour
{
    [SerializeField] private Text eventNameText;
    [SerializeField] private Text eventDescriptionText;
    [SerializeField] private Text eventTimeText;
    [SerializeField] private Button selectButton;
    
    private Event _event;
    private System.Action<Event> onSelectCallback;
    
    public void Setup(Event e, System.Action<Event> selectCallback)
    {
        _event = e;
        eventNameText.text = e.eventName;
        eventDescriptionText.text = e.description;
        eventTimeText.text = $"{e.startTime} 至 {e.endTime}";
        onSelectCallback = selectCallback;
        
        selectButton.onClick.AddListener(() => onSelectCallback?.Invoke(_event));
    }
}

public class TaskItem : MonoBehaviour
{
    [SerializeField] private Text taskNameText;
    [SerializeField] private Text progressText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text rewardText;
    [SerializeField] private Button claimButton;
    
    private EventTask task;
    private string eventID;
    
    public void Setup(EventTask t, string eID)
    {
        task = t;
        eventID = eID;
        taskNameText.text = t.taskName;
        progressText.text = $"{t.currentValue}/{t.targetValue}";
        progressSlider.value = (float)t.currentValue / t.targetValue;
        rewardText.text = $"奖励: {t.rewardValue} {t.rewardType}";
        
        claimButton.gameObject.SetActive(t.isCompleted && !t.isClaimed);
        claimButton.onClick.AddListener(OnClaimReward);
    }
    
    private void OnClaimReward()
    {
        bool success = EventSystem.Instance.ClaimEventTaskReward(eventID, task.taskID);
        if (success)
        {
            claimButton.gameObject.SetActive(false);
        }
    }
}

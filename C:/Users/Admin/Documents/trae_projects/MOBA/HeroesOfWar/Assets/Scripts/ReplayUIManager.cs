using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class ReplayUIManager : MonoBehaviour
{
    public static ReplayUIManager Instance { get; private set; }
    
    public Canvas replayCanvas;
    public ScrollRect replaysScrollRect;
    public Transform replaysContent;
    public Button playReplayButton;
    public Button pauseReplayButton;
    public Button resumeReplayButton;
    public Button stopReplayButton;
    public Slider replaySpeedSlider;
    public Slider replayProgressSlider;
    public Text replaySpeedText;
    public Text replayTimeText;
    public GameObject replayPrefab;
    
    private string selectedReplayFile;
    
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
        replayCanvas.gameObject.SetActive(false);
        playReplayButton.onClick.AddListener(PlayReplay);
        pauseReplayButton.onClick.AddListener(PauseReplay);
        resumeReplayButton.onClick.AddListener(ResumeReplay);
        stopReplayButton.onClick.AddListener(StopReplay);
        replaySpeedSlider.onValueChanged.AddListener(OnSpeedChanged);
        replayProgressSlider.onValueChanged.AddListener(OnProgressChanged);
    }
    
    public void OpenReplayUI()
    {
        replayCanvas.gameObject.SetActive(true);
        UpdateReplaysList();
    }
    
    public void CloseReplayUI()
    {
        replayCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateReplaysList()
    {
        // 清空现有内容
        foreach (Transform child in replaysContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示回放文件列表
        List<string> replayFiles = ReplayManager.Instance.GetReplayFiles();
        foreach (string file in replayFiles)
        {
            GameObject replayObj = Instantiate(replayPrefab, replaysContent);
            Text[] texts = replayObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 1)
            {
                texts[0].text = file;
            }
            
            // 添加点击事件
            Button button = replayObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectReplay(file));
            }
        }
    }
    
    public void SelectReplay(string fileName)
    {
        selectedReplayFile = fileName;
        Debug.Log($"选择回放文件: {fileName}");
    }
    
    public void PlayReplay()
    {
        if (!string.IsNullOrEmpty(selectedReplayFile))
        {
            string replayPath = Application.dataPath + "/Replays/" + selectedReplayFile;
            ReplayManager.Instance.LoadReplay(replayPath);
            ReplayManager.Instance.StartPlaying();
        }
    }
    
    public void PauseReplay()
    {
        ReplayManager.Instance.PausePlaying();
    }
    
    public void ResumeReplay()
    {
        ReplayManager.Instance.ResumePlaying();
    }
    
    public void StopReplay()
    {
        ReplayManager.Instance.StopPlaying();
    }
    
    public void OnSpeedChanged(float value)
    {
        ReplayManager.Instance.SetReplaySpeed(value);
        replaySpeedText.text = $"速度: {value}x";
    }
    
    public void OnProgressChanged(float value)
    {
        // 计算回放时间
        float totalTime = 600; // 假设总时间为10分钟
        float currentTime = value * totalTime;
        ReplayManager.Instance.SeekReplay(currentTime);
    }
    
    public void StartRecording()
    {
        ReplayManager.Instance.StartRecording();
    }
    
    public void StopRecording()
    {
        ReplayManager.Instance.StopRecording();
    }
    
    private void Update()
    {
        // 更新回放进度
        if (ReplayManager.Instance.isPlaying)
        {
            // 这里应该根据实际的回放时间来更新进度条
            // 暂时使用模拟数据
            float progress = 0.5f;
            replayProgressSlider.value = progress;
            replayTimeText.text = "时间: 05:00 / 10:00";
        }
    }
}
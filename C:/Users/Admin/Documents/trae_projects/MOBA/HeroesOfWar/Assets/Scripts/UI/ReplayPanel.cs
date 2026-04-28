using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ReplayPanel : UIPanel
{
    [SerializeField] private GameObject replayItemPrefab;
    [SerializeField] private Transform replayListContent;
    [SerializeField] private Button backButton;
    [SerializeField] private Text noReplaysText;
    
    private List<ReplayItem> replayItems = new List<ReplayItem>();
    
    private void Start()
    {
        backButton.onClick.AddListener(OnBack);
        RefreshReplayList();
    }
    
    public void RefreshReplayList()
    {
        // 清空现有列表
        foreach (var item in replayItems)
        {
            Destroy(item.gameObject);
        }
        replayItems.Clear();
        
        // 从ReplaySystem获取回放列表
        if (ReplaySystem.Instance != null)
        {
            List<ReplayData> replays = ReplaySystem.Instance.GetReplayList();
            
            if (replays.Count > 0)
            {
                noReplaysText.gameObject.SetActive(false);
                
                foreach (ReplayData replay in replays)
                {
                    GameObject replayItemGO = Instantiate(replayItemPrefab, replayListContent);
                    ReplayItem replayItem = replayItemGO.GetComponent<ReplayItem>();
                    if (replayItem != null)
                    {
                        replayItem.Setup(replay, OnReplaySelect, OnReplayDelete);
                        replayItems.Add(replayItem);
                    }
                }
            }
            else
            {
                noReplaysText.gameObject.SetActive(true);
            }
        }
    }
    
    private void OnReplaySelect(ReplayData replay)
    {
        // 播放回放
        if (ReplaySystem.Instance != null)
        {
            ReplaySystem.Instance.PlayReplay(replay.replayID);
        }
    }
    
    private void OnReplayDelete(ReplayData replay)
    {
        // 删除回放
        if (ReplaySystem.Instance != null)
        {
            ReplaySystem.Instance.DeleteReplay(replay.replayID);
            RefreshReplayList();
        }
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

public class ReplayItem : MonoBehaviour
{
    [SerializeField] private Text replayNameText;
    [SerializeField] private Text replayDateText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button deleteButton;
    
    private ReplayData replay;
    private System.Action<ReplayData> onPlayCallback;
    private System.Action<ReplayData> onDeleteCallback;
    
    public void Setup(ReplayData r, System.Action<ReplayData> playCallback, System.Action<ReplayData> deleteCallback)
    {
        replay = r;
        replayNameText.text = r.matchName;
        replayDateText.text = r.recordTime.ToString("yyyy-MM-dd HH:mm");
        onPlayCallback = playCallback;
        onDeleteCallback = deleteCallback;
        
        playButton.onClick.AddListener(OnPlay);
        deleteButton.onClick.AddListener(OnDelete);
    }
    
    private void OnPlay()
    {
        onPlayCallback?.Invoke(replay);
    }
    
    private void OnDelete()
    {
        onDeleteCallback?.Invoke(replay);
    }
}

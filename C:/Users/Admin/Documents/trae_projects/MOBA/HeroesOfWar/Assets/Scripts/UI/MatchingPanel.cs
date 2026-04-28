using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MatchingPanel : UIPanel
{
    [SerializeField] private Text matchingText;
    [SerializeField] private Image progressBar;
    [SerializeField] private Button cancelButton;
    
    private float matchingProgress;
    private bool isMatching;
    
    private void Start()
    {
        cancelButton.onClick.AddListener(OnCancelMatching);
    }
    
    public override void OnShow()
    {
        base.OnShow();
        StartMatching();
    }
    
    private void StartMatching()
    {
        matchingProgress = 0f;
        isMatching = true;
        StartCoroutine(UpdateMatchingProgress());
    }
    
    private IEnumerator UpdateMatchingProgress()
    {
        string[] matchingMessages = {
            "正在寻找对手...",
            "正在匹配队友...",
            "正在确认玩家...",
            "准备进入游戏..."
        };
        
        int messageIndex = 0;
        matchingText.text = matchingMessages[messageIndex];
        
        while (isMatching && matchingProgress < 1f)
        {
            matchingProgress += Time.deltaTime * 0.1f;
            progressBar.fillAmount = matchingProgress;
            
            if (matchingProgress > 0.25f && messageIndex == 0)
            {
                messageIndex = 1;
                matchingText.text = matchingMessages[messageIndex];
            }
            else if (matchingProgress > 0.5f && messageIndex == 1)
            {
                messageIndex = 2;
                matchingText.text = matchingMessages[messageIndex];
            }
            else if (matchingProgress > 0.75f && messageIndex == 2)
            {
                messageIndex = 3;
                matchingText.text = matchingMessages[messageIndex];
            }
            
            yield return null;
        }
    }
    
    private void OnCancelMatching()
    {
        isMatching = false;
        GameFlowManager.Instance.ReturnToMainMenu();
    }
    
    public void StopMatching()
    {
        isMatching = false;
    }
}

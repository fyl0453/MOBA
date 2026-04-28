using UnityEngine;
using UnityEngine.UI;

public class ReportUIManager : MonoBehaviour
{
    public static ReportUIManager Instance { get; private set; }
    
    public Canvas reportCanvas;
    public Dropdown playerDropdown;
    public Dropdown reasonDropdown;
    public InputField descriptionInput;
    public Button submitButton;
    
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
        reportCanvas.gameObject.SetActive(false);
        InitializeDropdowns();
    }
    
    private void InitializeDropdowns()
    {
        // 初始化玩家下拉菜单
        if (playerDropdown != null)
        {
            playerDropdown.options.Clear();
            playerDropdown.options.Add(new Dropdown.OptionData("Player1"));
            playerDropdown.options.Add(new Dropdown.OptionData("Player2"));
            playerDropdown.options.Add(new Dropdown.OptionData("Player3"));
            playerDropdown.options.Add(new Dropdown.OptionData("Player4"));
            playerDropdown.options.Add(new Dropdown.OptionData("Player5"));
            playerDropdown.value = 0;
        }
        
        // 初始化举报原因下拉菜单
        if (reasonDropdown != null)
        {
            reasonDropdown.options.Clear();
            reasonDropdown.options.Add(new Dropdown.OptionData("作弊"));
            reasonDropdown.options.Add(new Dropdown.OptionData(" toxicity"));
            reasonDropdown.options.Add(new Dropdown.OptionData("AFK"));
            reasonDropdown.options.Add(new Dropdown.OptionData("骚扰"));
            reasonDropdown.options.Add(new Dropdown.OptionData("其他"));
            reasonDropdown.value = 0;
        }
    }
    
    public void OpenReportUI()
    {
        reportCanvas.gameObject.SetActive(true);
    }
    
    public void CloseReportUI()
    {
        reportCanvas.gameObject.SetActive(false);
    }
    
    public void SubmitReport()
    {
        if (playerDropdown != null && reasonDropdown != null)
        {
            string reportedPlayer = playerDropdown.options[playerDropdown.value].text;
            ReportManager.ReportReason reason = (ReportManager.ReportReason)reasonDropdown.value;
            string description = descriptionInput.text;
            
            ReportManager.Instance.SubmitReport(reportedPlayer, reason, description);
            
            // 显示成功消息
            Debug.Log("举报提交成功！");
            
            // 关闭UI
            CloseReportUI();
        }
    }
}
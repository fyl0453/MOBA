using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    
    public Canvas tutorialCanvas;
    public Text tutorialText;
    public Image tutorialArrow;
    public Button nextButton;
    
    private int currentStep = 0;
    private bool isTutorialActive = false;
    
    private string[] tutorialSteps = {
        "欢迎来到 Heroes of War！",
        "使用 WASD 键移动你的英雄",
        "点击鼠标左键攻击敌人",
        "使用 Q、W、E 键释放技能",
        "使用 R 键释放终极技能",
        "击败野怪可以获得 Buff 和金币",
        "购买装备提升你的战斗力",
        "摧毁敌方基地获得胜利！"
    };
    
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
        // 检查是否是第一次启动游戏
        if (!PlayerPrefs.HasKey("TutorialCompleted"))
        {
            StartTutorial();
        }
    }
    
    public void StartTutorial()
    {
        isTutorialActive = true;
        tutorialCanvas.gameObject.SetActive(true);
        currentStep = 0;
        UpdateTutorialStep();
    }
    
    public void NextStep()
    {
        currentStep++;
        if (currentStep < tutorialSteps.Length)
        {
            UpdateTutorialStep();
        }
        else
        {
            EndTutorial();
        }
    }
    
    private void UpdateTutorialStep()
    {
        if (tutorialText != null)
        {
            tutorialText.text = tutorialSteps[currentStep];
        }
        
        // 根据当前步骤调整箭头位置
        AdjustArrowPosition();
    }
    
    private void AdjustArrowPosition()
    {
        switch (currentStep)
        {
            case 0:
                // 欢迎界面
                tutorialArrow.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                break;
            case 1:
                // 移动操作
                tutorialArrow.transform.position = new Vector3(Screen.width / 4, Screen.height / 4, 0);
                break;
            case 2:
                // 攻击操作
                tutorialArrow.transform.position = new Vector3(Screen.width * 3 / 4, Screen.height / 2, 0);
                break;
            case 3:
            case 4:
                // 技能操作
                tutorialArrow.transform.position = new Vector3(Screen.width / 4, Screen.height * 3 / 4, 0);
                break;
            case 5:
                // 野怪提示
                tutorialArrow.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                break;
            case 6:
                // 装备提示
                tutorialArrow.transform.position = new Vector3(Screen.width * 3 / 4, Screen.height * 3 / 4, 0);
                break;
            case 7:
                // 胜利条件
                tutorialArrow.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                break;
        }
    }
    
    private void EndTutorial()
    {
        isTutorialActive = false;
        tutorialCanvas.gameObject.SetActive(false);
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
    }
    
    public bool IsTutorialActive()
    {
        return isTutorialActive;
    }
    
    public void SkipTutorial()
    {
        EndTutorial();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeSelectionManager : MonoBehaviour
{
    public static GameModeSelectionManager Instance { get; private set; }
    
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
    
    public void SelectClassicMode()
    {
        GameModeManager.Instance.SetGameMode(GameModeManager.GameMode.Classic);
        StartMatch();
    }
    
    public void SelectARAMMode()
    {
        GameModeManager.Instance.SetGameMode(GameModeManager.GameMode.ARAM);
        StartMatch();
    }
    
    public void SelectURFMode()
    {
        GameModeManager.Instance.SetGameMode(GameModeManager.GameMode.URF);
        StartMatch();
    }
    
    public void SelectCustomMode()
    {
        GameModeManager.Instance.SetGameMode(GameModeManager.GameMode.Custom);
        StartMatch();
    }
    
    private void StartMatch()
    {
        // 加载游戏场景
        SceneManager.LoadScene("GameScene");
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
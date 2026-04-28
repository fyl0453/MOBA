using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private bool isGameStarted = false;
    private bool isGamePaused = false;
    private bool isGameOver = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeSystems()
    {
        Debug.Log("初始化游戏系统...");
        
        // 确保必要的系统被初始化
        InitializeSystem<ServerManager>();
        InitializeSystem<AchievementSystem>();
        InitializeSystem<LeaderboardSystem>();
        InitializeSystem<EventSystem>();
        InitializeSystem<SkinSystem>();
        InitializeSystem<RuneSystem>();
        InitializeSystem<FriendSystem>();
        InitializeSystem<ClanSystem>();
        
        Debug.Log("游戏系统初始化完成");
    }
    
    private void InitializeSystem<T>() where T : MonoBehaviour
    {
        // 检查是否已经存在该系统
        if (FindObjectOfType<T>() == null)
        {
            // 创建一个新的GameObject并添加系统组件
            GameObject systemObject = new GameObject(typeof(T).Name);
            systemObject.AddComponent<T>();
            DontDestroyOnLoad(systemObject);
        }
    }
    
    public void StartGame()
    {
        if (!isGameStarted)
        {
            isGameStarted = true;
            isGamePaused = false;
            isGameOver = false;
            
            Debug.Log("游戏开始");
            
            // 加载游戏场景
            SceneManager.LoadScene("GameScene");
        }
    }
    
    public void PauseGame()
    {
        if (isGameStarted && !isGamePaused && !isGameOver)
        {
            isGamePaused = true;
            Time.timeScale = 0f;
            Debug.Log("游戏暂停");
        }
    }
    
    public void ResumeGame()
    {
        if (isGameStarted && isGamePaused && !isGameOver)
        {
            isGamePaused = false;
            Time.timeScale = 1f;
            Debug.Log("游戏继续");
        }
    }
    
    public void EndGame()
    {
        if (isGameStarted && !isGameOver)
        {
            isGameOver = true;
            isGameStarted = false;
            
            Debug.Log("游戏结束");
            
            // 加载主菜单场景
            SceneManager.LoadScene("MainMenu");
        }
    }
    
    public bool IsGameStarted()
    {
        return isGameStarted;
    }
    
    public bool IsGamePaused()
    {
        return isGamePaused;
    }
    
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void RestartGame()
    {
        isGameStarted = false;
        isGamePaused = false;
        isGameOver = false;
        
        Debug.Log("游戏重新开始");
        
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}

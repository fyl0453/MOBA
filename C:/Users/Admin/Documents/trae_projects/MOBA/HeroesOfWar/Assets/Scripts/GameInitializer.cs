using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameObject gameManagerPrefab;
    
    private void Awake()
    {
        // 确保GameManager被初始化
        if (GameManager.Instance == null)
        {
            if (gameManagerPrefab != null)
            {
                Instantiate(gameManagerPrefab);
            }
            else
            {
                // 如果没有预制体，创建一个新的GameObject并添加GameManager组件
                GameObject gameManagerObject = new GameObject("GameManager");
                gameManagerObject.AddComponent<GameManager>();
            }
        }
        
        Debug.Log("游戏初始化完成");
    }
    
    private void Start()
    {
        // 加载主菜单场景
        GameManager.Instance.LoadScene("MainMenu");
    }
}

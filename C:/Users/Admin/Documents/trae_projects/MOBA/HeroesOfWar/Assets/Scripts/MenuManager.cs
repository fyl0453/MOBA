using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    
    public void OpenHeroSelect()
    {
        // 打开英雄选择界面
        Debug.Log("Opening hero select screen");
    }
    
    public void OpenSettings()
    {
        // 打开设置界面
        Debug.Log("Opening settings screen");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    private float gameTime = 0f;
    
    private void Update()
    {
        gameTime += Time.deltaTime;
        UpdateTimerText();
    }
    
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        
        if (timerText != null)
        {
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
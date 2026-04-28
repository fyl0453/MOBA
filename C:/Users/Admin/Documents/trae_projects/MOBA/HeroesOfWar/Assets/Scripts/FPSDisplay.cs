using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public Text fpsText;
    public float updateInterval = 0.5f;
    
    private float accum = 0f;
    private int frames = 0;
    private float timeleft;
    private float fps = 0f;
    
    private void Start()
    {
        timeleft = updateInterval;
    }
    
    private void Update()
    {
        // 检查是否显示FPS
        if (SettingsManager.Instance != null && !SettingsManager.Instance.settingsData.showFPS)
        {
            if (fpsText != null)
            {
                fpsText.gameObject.SetActive(false);
            }
            return;
        }
        
        if (fpsText != null)
        {
            fpsText.gameObject.SetActive(true);
        }
        
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;
        
        if (timeleft <= 0.0)
        {
            fps = accum / frames;
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
            
            if (fpsText != null)
            {
                fpsText.text = $"FPS: {fps:F1}";
            }
        }
    }
}

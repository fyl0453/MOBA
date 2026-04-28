using UnityEngine;
using UnityEngine.UI;

public class PerformanceTest : MonoBehaviour
{
    public Text fpsText;
    public Text memoryText;
    public Text cpuText;
    
    private float deltaTime = 0.0f;
    private int frameCount = 0;
    private float frameRate = 0.0f;
    
    private void Update()
    {
        // 计算帧率
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        frameCount++;
        if (frameCount >= 10)
        {
            frameRate = 1.0f / deltaTime;
            frameCount = 0;
        }
        
        // 更新UI
        if (fpsText != null)
        {
            fpsText.text = "FPS: " + Mathf.Round(frameRate).ToString();
        }
        
        if (memoryText != null)
        {
            memoryText.text = "Memory: " + (System.GC.GetTotalMemory(false) / 1024 / 1024).ToString("F2") + " MB";
        }
        
        if (cpuText != null)
        {
            cpuText.text = "CPU: " + (Time.deltaTime * 1000).ToString("F2") + " ms";
        }
    }
    
    public void OptimizePerformance()
    {
        // 性能优化建议
        Debug.Log("Optimizing performance...");
        
        // 1. 降低阴影质量
        QualitySettings.shadowResolution = ShadowResolution.Low;
        
        // 2. 降低抗锯齿
        QualitySettings.antiAliasing = 0;
        
        // 3. 降低纹理质量
        QualitySettings.masterTextureLimit = 1;
        
        // 4. 关闭垂直同步
        QualitySettings.vSyncCount = 0;
        
        Debug.Log("Performance optimized");
    }
}
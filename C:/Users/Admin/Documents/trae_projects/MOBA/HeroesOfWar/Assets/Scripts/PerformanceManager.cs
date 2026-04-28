using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public class PerformanceManager : MonoBehaviour
{
    public static PerformanceManager Instance { get; private set; }
    
    public bool enableProfiling = false;
    public float updateInterval = 1.0f;
    
    private float frameCount = 0f;
    private float frameTime = 0f;
    private float fps = 0f;
    private float lastUpdateTime = 0f;
    
    private Dictionary<string, Stopwatch> profilers = new Dictionary<string, Stopwatch>();
    private Dictionary<string, float> profilerResults = new Dictionary<string, float>();
    
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
    
    private void Update()
    {
        // 计算FPS
        frameCount++;
        frameTime += Time.deltaTime;
        
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            fps = frameCount / frameTime;
            frameCount = 0f;
            frameTime = 0f;
            lastUpdateTime = Time.time;
            
            if (enableProfiling)
            {
                Debug.Log($"FPS: {fps:F2}");
                LogProfilerResults();
            }
        }
    }
    
    public float GetFPS()
    {
        return fps;
    }
    
    public void StartProfiling(string name)
    {
        if (!enableProfiling)
            return;
        
        if (!profilers.ContainsKey(name))
        {
            profilers[name] = new Stopwatch();
        }
        
        profilers[name].Start();
    }
    
    public void StopProfiling(string name)
    {
        if (!enableProfiling)
            return;
        
        if (profilers.ContainsKey(name))
        {
            profilers[name].Stop();
            profilerResults[name] = profilers[name].ElapsedMilliseconds;
            profilers[name].Reset();
        }
    }
    
    private void LogProfilerResults()
    {
        foreach (var result in profilerResults)
        {
            Debug.Log($"Profiler {result.Key}: {result.Value}ms");
        }
        
        profilerResults.Clear();
    }
    
    public void Optimize()
    {
        // 执行性能优化
        OptimizeGraphics();
        OptimizeMemory();
        OptimizeCPU();
    }
    
    private void OptimizeGraphics()
    {
        // 图形优化
        QualitySettings.vSyncCount = SettingsManager.Instance.settingsData.vsync ? 1 : 0;
        QualitySettings.SetQualityLevel(SettingsManager.Instance.settingsData.graphicsQuality);
    }
    
    private void OptimizeMemory()
    {
        // 内存优化
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
    
    private void OptimizeCPU()
    {
        // CPU优化
        // 这里可以添加CPU优化逻辑
    }
    
    public void SetProfilingEnabled(bool enabled)
    {
        enableProfiling = enabled;
    }
}

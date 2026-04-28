using UnityEngine;
using System.Collections.Generic;

public class MinionManager : MonoBehaviour
{
    public static MinionManager Instance { get; private set; }
    
    public GameObject minionPrefab;
    public float spawnInterval = 30f;
    public int minionsPerWave = 3;
    
    private float spawnTimer = 0f;
    private int waveCount = 0;
    
    // 小兵路径点
    private List<Vector3> blueMinionPath = new List<Vector3>
    {
        new Vector3(-10, 0, 0),  // 蓝方出生点
        new Vector3(0, 0, 0),    // 中路
        new Vector3(10, 0, 0)    // 红方基地
    };
    
    private List<Vector3> redMinionPath = new List<Vector3>
    {
        new Vector3(10, 0, 0),   // 红方出生点
        new Vector3(0, 0, 0),    // 中路
        new Vector3(-10, 0, 0)   // 蓝方基地
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
    
    private void Update()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.InGame)
        {
            spawnTimer += Time.deltaTime;
            
            if (spawnTimer >= spawnInterval)
            {
                SpawnMinionWave();
                spawnTimer = 0f;
            }
        }
    }
    
    private void SpawnMinionWave()
    {
        waveCount++;
        Debug.Log($"第 {waveCount} 波小兵生成");
        
        // 生成蓝方小兵
        for (int i = 0; i < minionsPerWave; i++)
        {
            SpawnMinion(Tower.Team.Blue, blueMinionPath);
        }
        
        // 生成红方小兵
        for (int i = 0; i < minionsPerWave; i++)
        {
            SpawnMinion(Tower.Team.Red, redMinionPath);
        }
    }
    
    private void SpawnMinion(Tower.Team team, List<Vector3> path)
    {
        if (minionPrefab == null)
            return;
        
        // 生成小兵
        GameObject minionObj = Instantiate(minionPrefab, path[0], Quaternion.identity);
        Minion minion = minionObj.GetComponent<Minion>();
        
        if (minion != null)
        {
            minion.team = team;
            minion.Initialize(path);
        }
    }
    
    public void Reset()
    {
        waveCount = 0;
        spawnTimer = 0f;
        
        // 清除所有小兵
        Minion[] minions = FindObjectsOfType<Minion>();
        foreach (Minion minion in minions)
        {
            Destroy(minion.gameObject);
        }
    }
}

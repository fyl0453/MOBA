using System.Collections.Generic;
using UnityEngine;

public class VisionManager : MonoBehaviour
{
    public static VisionManager Instance { get; private set; }
    
    public List<Grass> grasses = new List<Grass>();
    public List<Ward> wards = new List<Ward>();
    public Dictionary<int, List<GameObject>> teamVision = new Dictionary<int, List<GameObject>>();
    
    public float baseVisionRange = 15f;
    public float wardVisionRange = 10f;
    
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
        
        teamVision[1] = new List<GameObject>();
        teamVision[2] = new List<GameObject>();
    }
    
    private void Start()
    {
        InitializeGrass();
        InitializeWards();
    }
    
    private void InitializeGrass()
    {
        // 创建草丛
        CreateGrass("Grass1", new Vector3(-10, 0, 10), 3f);
        CreateGrass("Grass2", new Vector3(10, 0, -10), 3f);
        CreateGrass("Grass3", new Vector3(-20, 0, 20), 4f);
        CreateGrass("Grass4", new Vector3(20, 0, -20), 4f);
        CreateGrass("Grass5", new Vector3(0, 0, 0), 5f);
    }
    
    private void InitializeWards()
    {
        // 创建守卫
        CreateWard("Team1_Ward1", new Vector3(-15, 0, 15), 1);
        CreateWard("Team2_Ward1", new Vector3(15, 0, -15), 2);
    }
    
    private void CreateGrass(string name, Vector3 position, float radius)
    {
        GameObject grassObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        grassObj.name = name;
        grassObj.transform.position = position;
        grassObj.transform.localScale = new Vector3(radius * 2, 1, radius * 2);
        
        Renderer renderer = grassObj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0.2f, 0.8f, 0.2f, 0.5f);
        }
        
        Grass grass = grassObj.AddComponent<Grass>();
        grass.radius = radius;
        grasses.Add(grass);
    }
    
    private void CreateWard(string name, Vector3 position, int teamId)
    {
        GameObject wardObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        wardObj.name = name;
        wardObj.transform.position = position;
        wardObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        Ward ward = wardObj.AddComponent<Ward>();
        ward.teamId = teamId;
        ward.visionRange = wardVisionRange;
        wards.Add(ward);
        
        teamVision[teamId].Add(wardObj);
    }
    
    public bool CanSee(GameObject viewer, GameObject target)
    {
        HeroController viewerHero = viewer.GetComponent<HeroController>();
        if (viewerHero == null) return false;
        
        int viewerTeam = viewerHero.teamId;
        float distance = Vector3.Distance(viewer.transform.position, target.transform.position);
        
        // 检查基础视野范围
        if (distance > baseVisionRange) return false;
        
        // 检查是否在草丛中
        bool viewerInGrass = IsInGrass(viewer.transform.position);
        bool targetInGrass = IsInGrass(target.transform.position);
        
        // 如果目标在草丛中但观察者不在，无法看见
        if (targetInGrass && !viewerInGrass) return false;
        
        // 检查守卫提供的视野
        if (teamVision.ContainsKey(viewerTeam))
        {
            foreach (GameObject ward in teamVision[viewerTeam])
            {
                float wardDistance = Vector3.Distance(ward.transform.position, target.transform.position);
                if (wardDistance <= wardVisionRange)
                {
                    return true;
                }
            }
        }
        
        // 如果目标不在草丛中，可以看见
        if (!targetInGrass) return true;
        
        // 如果双方都在草丛中，可以看见
        if (viewerInGrass && targetInGrass) return true;
        
        return false;
    }
    
    public bool IsInGrass(Vector3 position)
    {
        foreach (Grass grass in grasses)
        {
            float distance = Vector3.Distance(position, grass.transform.position);
            if (distance <= grass.radius)
            {
                return true;
            }
        }
        return false;
    }
    
    public void PlaceWard(Vector3 position, int teamId)
    {
        string wardName = $"PlacedWard_Team{teamId}_{wards.Count}";
        CreateWard(wardName, position, teamId);
    }
    
    public void RemoveWard(Ward ward)
    {
        if (wards.Contains(ward))
        {
            teamVision[ward.teamId].Remove(ward.gameObject);
            wards.Remove(ward);
            Destroy(ward.gameObject);
        }
    }
}

public class Grass : MonoBehaviour
{
    public float radius;
}

public class Ward : MonoBehaviour
{
    public int teamId;
    public float visionRange;
    public float lifetime = 180f; // 3分钟
    private float timer = 0f;
    
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            VisionManager.Instance.RemoveWard(this);
        }
    }
}
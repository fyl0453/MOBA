using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecommendUIManager : MonoBehaviour
{
    public static RecommendUIManager Instance { get; private set; }
    
    public Canvas recommendCanvas;
    public Transform recommendedBuildsContent;
    public Transform itemRecommendationsContent;
    public Button closeButton;
    public Button refreshButton;
    public Button popularBuildsButton;
    public Button highWinRateButton;
    public GameObject buildItemPrefab;
    public GameObject itemRecommendationPrefab;
    
    private string currentSortType = "popular";
    
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
    
    private void Start()
    {
        recommendCanvas.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseRecommendUI);
        refreshButton.onClick.AddListener(RefreshRecommendations);
        popularBuildsButton.onClick.AddListener(() => OnSortTypeChanged("popular"));
        highWinRateButton.onClick.AddListener(() => OnSortTypeChanged("winrate"));
    }
    
    public void OpenRecommendUI()
    {
        recommendCanvas.gameObject.SetActive(true);
        UpdateRecommendedBuilds();
        UpdateItemRecommendations();
    }
    
    public void CloseRecommendUI()
    {
        recommendCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateRecommendedBuilds()
    {
        foreach (Transform child in recommendedBuildsContent)
        {
            Destroy(child.gameObject);
        }
        
        List<EquipmentBuild> builds;
        if (currentSortType == "popular")
        {
            builds = RecommendManager.Instance.GetPopularBuilds(10);
        }
        else
        {
            builds = RecommendManager.Instance.GetHighWinRateBuilds(10);
        }
        
        foreach (EquipmentBuild build in builds)
        {
            GameObject buildItem = Instantiate(buildItemPrefab, recommendedBuildsContent);
            
            Text[] texts = buildItem.GetComponentsInChildren<Text>();
            if (texts.Length >= 4)
            {
                texts[0].text = build.buildName;
                texts[1].text = build.description;
                texts[2].text = $"热度: {build.popularity}";
                texts[3].text = $"胜率: {build.winRate:F1}%";
            }
            
            Button buildButton = buildItem.GetComponent<Button>();
            string buildID = build.buildID;
            buildButton.onClick.AddListener(() => OnBuildSelected(buildID));
        }
    }
    
    public void UpdateItemRecommendations()
    {
        foreach (Transform child in itemRecommendationsContent)
        {
            Destroy(child.gameObject);
        }
        
        RecommendManager.Instance.GenerateRecommendations();
        List<ItemRecommendation> recommendations = RecommendManager.Instance.GetCurrentRecommendations();
        
        foreach (ItemRecommendation rec in recommendations)
        {
            GameObject recItem = Instantiate(itemRecommendationPrefab, itemRecommendationsContent);
            
            Text[] texts = recItem.GetComponentsInChildren<Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = rec.itemName;
                texts[1].text = rec.reason;
                texts[2].text = $"优先级: {rec.priority}";
            }
            
            Button recButton = recItem.GetComponent<Button>();
            string itemID = rec.itemID;
            recButton.onClick.AddListener(() => OnItemRecommended(itemID));
        }
    }
    
    public void OnBuildSelected(string buildID)
    {
        EquipmentBuild build = RecommendManager.Instance.GetBuildByID(buildID);
        if (build != null)
        {
            Debug.Log($"选择出装方案: {build.buildName}");
            ShowBuildDetails(build);
        }
    }
    
    public void OnItemRecommended(string itemID)
    {
        Debug.Log($"推荐装备: {itemID}");
    }
    
    public void ShowBuildDetails(EquipmentBuild build)
    {
        Debug.Log($"=== {build.buildName} ===");
        Debug.Log($"类型: {build.buildType}");
        Debug.Log($"热度: {build.popularity}");
        Debug.Log($"胜率: {build.winRate:F1}%");
        Debug.Log("装备列表:");
        foreach (string itemID in build.itemIDs)
        {
            Debug.Log($"  - {itemID}");
        }
    }
    
    public void OnSortTypeChanged(string sortType)
    {
        currentSortType = sortType;
        UpdateRecommendedBuilds();
    }
    
    public void RefreshRecommendations()
    {
        RecommendManager.Instance.GenerateRecommendations();
        UpdateItemRecommendations();
    }
    
    public void ApplyBuildToLoadout(string buildID)
    {
        EquipmentBuild build = RecommendManager.Instance.GetBuildByID(buildID);
        if (build != null)
        {
            Debug.Log($"应用出装方案到加载页面: {build.buildName}");
        }
    }
    
    public void CreateCustomBuild()
    {
        Debug.Log("创建自定义出装方案");
    }
}
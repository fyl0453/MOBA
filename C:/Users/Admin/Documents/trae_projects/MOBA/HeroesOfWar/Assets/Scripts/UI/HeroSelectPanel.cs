using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroSelectPanel : UIPanel
{
    [SerializeField] private GameObject heroItemPrefab;
    [SerializeField] private Transform heroListContent;
    [SerializeField] private Button startMatchButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Text selectedHeroText;
    [SerializeField] private Image selectedHeroIcon;
    [SerializeField] private Text heroDescriptionText;
    
    private List<HeroItem> heroItems = new List<HeroItem>();
    private string selectedHeroID;
    
    private void Start()
    {
        startMatchButton.onClick.AddListener(OnStartMatch);
        backButton.onClick.AddListener(OnBack);
        
        InitializeHeroList();
    }
    
    private void InitializeHeroList()
    {
        // 清空现有列表
        foreach (var item in heroItems)
        {
            Destroy(item.gameObject);
        }
        heroItems.Clear();
        
        // 从HeroManager获取英雄列表
        if (HeroManager.Instance != null)
        {
            List<HeroData> heroes = HeroManager.Instance.GetAllHeroes();
            
            foreach (HeroData hero in heroes)
            {
                GameObject heroItemGO = Instantiate(heroItemPrefab, heroListContent);
                HeroItem heroItem = heroItemGO.GetComponent<HeroItem>();
                if (heroItem != null)
                {
                    heroItem.Setup(hero.heroID, hero.heroName, OnHeroSelect);
                    heroItems.Add(heroItem);
                }
            }
        }
    }
    
    private void OnHeroSelect(string heroID, string heroName)
    {
        selectedHeroID = heroID;
        selectedHeroText.text = "已选择: " + heroName;
        
        // 从HeroManager获取英雄详细信息
        if (HeroManager.Instance != null)
        {
            HeroData heroData = HeroManager.Instance.GetHeroByID(heroID);
            if (heroData != null)
            {
                // 设置英雄描述
                heroDescriptionText.text = heroData.description;
                
                // 这里可以设置英雄图标
                // if (selectedHeroIcon != null)
                // {
                //     // selectedHeroIcon.sprite = heroData.icon;
                // }
            }
        }
    }
    
    private void OnStartMatch()
    {
        if (!string.IsNullOrEmpty(selectedHeroID))
        {
            Debug.Log("开始匹配，选择的英雄: " + selectedHeroID);
            // 调用游戏流程管理器开始匹配
            GameFlowManager.Instance.StartMatching(selectedHeroID);
        }
        else
        {
            Debug.Log("请先选择一个英雄");
        }
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}

public class HeroItem : MonoBehaviour
{
    [SerializeField] private Text heroNameText;
    [SerializeField] private Button selectButton;
    
    private string heroID;
    private System.Action<string, string> onSelectCallback;
    
    public void Setup(string id, string name, System.Action<string, string> callback)
    {
        heroID = id;
        heroNameText.text = name;
        onSelectCallback = callback;
        
        selectButton.onClick.AddListener(OnSelect);
    }
    
    private void OnSelect()
    {
        onSelectCallback?.Invoke(heroID, heroNameText.text);
    }
}

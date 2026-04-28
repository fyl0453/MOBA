using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TalentUIManager : MonoBehaviour
{
    public static TalentUIManager Instance { get; private set; }
    
    public Canvas talentCanvas;
    public Transform talentGrid;
    public Transform pageSelector;
    public Button confirmButton;
    public Button cancelButton;
    public Button addPageButton;
    public GameObject talentSlotPrefab;
    public GameObject pageButtonPrefab;
    public Text pageNameText;
    
    private int selectedSlotIndex = -1;
    private string selectedTalentID = "";
    
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
        talentCanvas.gameObject.SetActive(false);
        confirmButton.onClick.AddListener(ConfirmTalentSelection);
        cancelButton.onClick.AddListener(CancelTalentSelection);
        addPageButton.onClick.AddListener(CreateNewPage);
    }
    
    public void OpenTalentUI()
    {
        talentCanvas.gameObject.SetActive(true);
        UpdateTalentGrid();
        UpdatePageSelector();
        UpdateCurrentPageName();
    }
    
    public void CloseTalentUI()
    {
        talentCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateTalentGrid()
    {
        foreach (Transform child in talentGrid)
        {
            Destroy(child.gameObject);
        }
        
        TalentPage currentPage = TalentManager.Instance.currentPage;
        if (currentPage == null) return;
        
        foreach (TalentSlot slot in currentPage.slots)
        {
            GameObject slotObj = Instantiate(talentSlotPrefab, talentGrid);
            
            Button slotButton = slotObj.GetComponent<Button>();
            int slotIndex = slot.slotIndex;
            slotButton.onClick.AddListener(() => SelectSlot(slotIndex));
            
            Text[] texts = slotObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = $"Tier {slot.tierLevel}";
                
                if (!string.IsNullOrEmpty(slot.equippedTalentID))
                {
                    Talent talent = TalentManager.Instance.GetTalentByID(slot.equippedTalentID);
                    if (talent != null)
                    {
                        texts[1].text = talent.talentName;
                    }
                }
                else
                {
                    texts[1].text = "Empty";
                }
            }
            
            if (slotIndex == selectedSlotIndex)
            {
                slotObj.GetComponent<Image>().color = new Color(1, 1, 0, 1);
            }
        }
    }
    
    public void UpdatePageSelector()
    {
        foreach (Transform child in pageSelector)
        {
            Destroy(child.gameObject);
        }
        
        List<TalentPage> pages = TalentManager.Instance.talentPages;
        foreach (TalentPage page in pages)
        {
            GameObject pageObj = Instantiate(pageButtonPrefab, pageSelector);
            
            Text[] texts = pageObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 1)
            {
                texts[0].text = page.pageName;
            }
            
            Button pageButton = pageObj.GetComponent<Button>();
            string pageID = page.pageID;
            pageButton.onClick.AddListener(() => SelectPage(pageID));
            
            if (page == TalentManager.Instance.currentPage)
            {
                pageObj.GetComponent<Image>().color = new Color(0, 1, 0, 1);
            }
        }
    }
    
    public void UpdateCurrentPageName()
    {
        if (TalentManager.Instance.currentPage != null)
        {
            pageNameText.text = TalentManager.Instance.currentPage.pageName;
        }
    }
    
    public void SelectSlot(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        UpdateTalentGrid();
        ShowTalentSelectionPanel();
    }
    
    public void ShowTalentSelectionPanel()
    {
        Debug.Log($"显示天赋选择面板 for slot {selectedSlotIndex}");
    }
    
    public void SelectTalent(string talentID)
    {
        selectedTalentID = talentID;
        Talent talent = TalentManager.Instance.GetTalentByID(talentID);
        if (talent != null)
        {
            Debug.Log($"选择了天赋: {talent.talentName}");
        }
    }
    
    public void ConfirmTalentSelection()
    {
        if (selectedSlotIndex >= 0 && !string.IsNullOrEmpty(selectedTalentID))
        {
            TalentManager.Instance.EquipTalent(selectedSlotIndex, selectedTalentID);
            selectedSlotIndex = -1;
            selectedTalentID = "";
            UpdateTalentGrid();
        }
    }
    
    public void CancelTalentSelection()
    {
        selectedSlotIndex = -1;
        selectedTalentID = "";
        UpdateTalentGrid();
    }
    
    public void SelectPage(string pageID)
    {
        TalentManager.Instance.SelectPage(pageID);
        UpdatePageSelector();
        UpdateCurrentPageName();
        UpdateTalentGrid();
    }
    
    public void CreateNewPage()
    {
        string newPageName = $"Page {TalentManager.Instance.talentPages.Count + 1}";
        TalentManager.Instance.CreateNewPage(newPageName);
        UpdatePageSelector();
    }
    
    public void DeleteCurrentPage()
    {
        if (TalentManager.Instance.currentPage != null && !TalentManager.Instance.currentPage.isDefault)
        {
            TalentManager.Instance.DeletePage(TalentManager.Instance.currentPage.pageID);
            TalentManager.Instance.currentPage = TalentManager.Instance.talentPages[0];
            UpdatePageSelector();
            UpdateCurrentPageName();
            UpdateTalentGrid();
        }
    }
    
    public void ApplyRecommendedTalents(string heroID)
    {
        List<Talent> recommendedTalents = TalentManager.Instance.GetTalentsForHero(heroID);
        Debug.Log($"应用推荐天赋 for hero {heroID}: {recommendedTalents.Count} talents");
    }
}
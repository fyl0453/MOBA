using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RuneUIManager : MonoBehaviour
{
    public static RuneUIManager Instance { get; private set; }
    
    public Canvas runeCanvas;
    public ScrollRect runePagesScrollRect;
    public Transform runePagesContent;
    public Transform runeDetailsContent;
    public GameObject runePagePrefab;
    public GameObject runeSlotPrefab;
    
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
        runeCanvas.gameObject.SetActive(false);
    }
    
    public void OpenRuneUI()
    {
        runeCanvas.gameObject.SetActive(true);
        UpdateRunePagesList();
        UpdateRuneDetails();
    }
    
    public void CloseRuneUI()
    {
        runeCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateRunePagesList()
    {
        // 清空现有内容
        foreach (Transform child in runePagesContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示符文页列表
        List<RunePage> runePages = RuneManager.Instance.GetAllPages();
        foreach (RunePage page in runePages)
        {
            GameObject pageObj = Instantiate(runePagePrefab, runePagesContent);
            Text[] texts = pageObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 1)
            {
                texts[0].text = page.pageName;
            }
            
            // 添加点击事件
            Button button = pageObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectRunePage(page));
            }
        }
    }
    
    public void SelectRunePage(RunePage page)
    {
        RuneManager.Instance.SetCurrentPage(page);
        UpdateRuneDetails();
    }
    
    public void UpdateRuneDetails()
    {
        // 清空现有内容
        foreach (Transform child in runeDetailsContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示当前符文页的符文
        RunePage currentPage = RuneManager.Instance.GetCurrentPage();
        if (currentPage != null)
        {
            // 显示印记
            for (int i = 0; i < currentPage.marks.Count; i++)
            {
                Rune rune = currentPage.marks[i];
                GameObject slotObj = Instantiate(runeSlotPrefab, runeDetailsContent);
                Text[] texts = slotObj.GetComponentsInChildren<Text>();
                if (texts.Length >= 2)
                {
                    texts[0].text = "印记";
                    texts[1].text = rune.runeName;
                }
            }
            
            // 显示符印
            for (int i = 0; i < currentPage.seals.Count; i++)
            {
                Rune rune = currentPage.seals[i];
                GameObject slotObj = Instantiate(runeSlotPrefab, runeDetailsContent);
                Text[] texts = slotObj.GetComponentsInChildren<Text>();
                if (texts.Length >= 2)
                {
                    texts[0].text = "符印";
                    texts[1].text = rune.runeName;
                }
            }
            
            // 显示雕文
            for (int i = 0; i < currentPage.glyphs.Count; i++)
            {
                Rune rune = currentPage.glyphs[i];
                GameObject slotObj = Instantiate(runeSlotPrefab, runeDetailsContent);
                Text[] texts = slotObj.GetComponentsInChildren<Text>();
                if (texts.Length >= 2)
                {
                    texts[0].text = "雕文";
                    texts[1].text = rune.runeName;
                }
            }
            
            // 显示精华
            for (int i = 0; i < currentPage.quintessences.Count; i++)
            {
                Rune rune = currentPage.quintessences[i];
                GameObject slotObj = Instantiate(runeSlotPrefab, runeDetailsContent);
                Text[] texts = slotObj.GetComponentsInChildren<Text>();
                if (texts.Length >= 2)
                {
                    texts[0].text = "精华";
                    texts[1].text = rune.runeName;
                }
            }
        }
    }
    
    public void OpenCreateRunePageUI()
    {
        // 打开创建符文页的UI
        Debug.Log("打开创建符文页界面");
    }
    
    public void CreateRunePage(string name)
    {
        RuneManager.Instance.CreateNewPage(name);
        UpdateRunePagesList();
    }
}
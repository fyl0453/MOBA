using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EmoteUIManager : MonoBehaviour
{
    public static EmoteUIManager Instance { get; private set; }
    
    public Canvas emoteCanvas;
    public Transform quickEmotesGrid;
    public Transform emoteShopGrid;
    public Button closeButton;
    public Button refreshButton;
    public GameObject emoteSlotPrefab;
    public GameObject emoteShopItemPrefab;
    
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
        emoteCanvas.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseEmoteUI);
        refreshButton.onClick.AddListener(RefreshEmoteShop);
    }
    
    public void OpenEmoteUI()
    {
        emoteCanvas.gameObject.SetActive(true);
        UpdateQuickEmotes();
        UpdateEmoteShop();
    }
    
    public void CloseEmoteUI()
    {
        emoteCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateQuickEmotes()
    {
        foreach (Transform child in quickEmotesGrid)
        {
            Destroy(child.gameObject);
        }
        
        List<Emote> quickEmotes = EmoteManager.Instance.GetQuickEmotes();
        foreach (Emote emote in quickEmotes)
        {
            GameObject emoteObj = Instantiate(emoteSlotPrefab, quickEmotesGrid);
            
            Text[] texts = emoteObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 1)
            {
                texts[0].text = emote.emoteName;
            }
            
            Button emoteButton = emoteObj.GetComponent<Button>();
            string emoteID = emote.emoteID;
            emoteButton.onClick.AddListener(() => OnEmoteClicked(emoteID));
        }
    }
    
    public void UpdateEmoteShop()
    {
        foreach (Transform child in emoteShopGrid)
        {
            Destroy(child.gameObject);
        }
        
        List<Emote> allEmotes = EmoteManager.Instance.allEmotes;
        foreach (Emote emote in allEmotes)
        {
            GameObject shopItem = Instantiate(emoteShopItemPrefab, emoteShopGrid);
            
            Text[] texts = shopItem.GetComponentsInChildren<Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = emote.emoteName;
                texts[1].text = emote.description;
                texts[2].text = emote.isOwned ? "已拥有" : $"{emote.cost}金币";
            }
            
            Button shopButton = shopItem.GetComponent<Button>();
            string emoteID = emote.emoteID;
            shopButton.onClick.AddListener(() => OnShopItemClicked(emoteID));
        }
    }
    
    public void OnEmoteClicked(string emoteID)
    {
        Emote emote = EmoteManager.Instance.GetEmoteByID(emoteID);
        if (emote != null && emote.isOwned)
        {
            Vector3 playerPos = Vector3.zero;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerPos = player.transform.position;
            }
            
            EmoteManager.Instance.UseEmote(emoteID, playerPos);
        }
    }
    
    public void OnShopItemClicked(string emoteID)
    {
        Emote emote = EmoteManager.Instance.GetEmoteByID(emoteID);
        if (emote != null)
        {
            if (emote.isOwned)
            {
                ShowEquipDialog(emoteID);
            }
            else
            {
                ShowPurchaseDialog(emoteID);
            }
        }
    }
    
    public void ShowPurchaseDialog(string emoteID)
    {
        Emote emote = EmoteManager.Instance.GetEmoteByID(emoteID);
        if (emote != null)
        {
            Debug.Log($"购买表情: {emote.emoteName}, 价格: {emote.cost}金币");
            EmoteManager.Instance.PurchaseEmote(emoteID);
            UpdateEmoteShop();
        }
    }
    
    public void ShowEquipDialog(string emoteID)
    {
        Debug.Log($"装备表情: {emoteID}");
        ShowEmoteSlotSelection(emoteID);
    }
    
    public void ShowEmoteSlotSelection(string emoteID)
    {
        Debug.Log($"选择要装备的快捷栏位 for {emoteID}");
    }
    
    public void EquipEmoteToSlot(string emoteID, int slotIndex)
    {
        EmoteManager.Instance.EquipEmoteToSlot(slotIndex, emoteID);
        UpdateQuickEmotes();
    }
    
    public void RefreshEmoteShop()
    {
        UpdateEmoteShop();
    }
    
    public void OnQuickEmoteKeyPressed(int slotIndex)
    {
        EmoteManager.Instance.UseQuickEmote(slotIndex);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ClanUIManager : MonoBehaviour
{
    public static ClanUIManager Instance { get; private set; }
    
    public Canvas clanCanvas;
    public Text clanNameText;
    public Text clanDescriptionText;
    public Text clanLevelText;
    public Text clanPointsText;
    public Text clanLeaderText;
    public ScrollRect clanMembersScrollRect;
    public Transform clanMembersContent;
    public GameObject memberPrefab;
    
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
        clanCanvas.gameObject.SetActive(false);
    }
    
    public void OpenClanUI()
    {
        clanCanvas.gameObject.SetActive(true);
        UpdateClanInfo();
        UpdateClanMembers();
    }
    
    public void CloseClanUI()
    {
        clanCanvas.gameObject.SetActive(false);
    }
    
    public void UpdateClanInfo()
    {
        Clan currentClan = ClanManager.Instance.GetCurrentClan();
        if (currentClan != null)
        {
            clanNameText.text = currentClan.clanName;
            clanDescriptionText.text = currentClan.clanDescription;
            clanLevelText.text = "等级: " + currentClan.GetLevel();
            clanPointsText.text = "积分: " + currentClan.GetPoints();
            clanLeaderText.text = "队长: " + currentClan.GetLeader();
        }
        else
        {
            clanNameText.text = "未加入战队";
            clanDescriptionText.text = "";
            clanLevelText.text = "";
            clanPointsText.text = "";
            clanLeaderText.text = "";
        }
    }
    
    public void UpdateClanMembers()
    {
        // 清空现有内容
        foreach (Transform child in clanMembersContent)
        {
            Destroy(child.gameObject);
        }
        
        // 显示战队成员
        Clan currentClan = ClanManager.Instance.GetCurrentClan();
        if (currentClan != null)
        {
            foreach (string member in currentClan.GetMembers())
            {
                GameObject memberObj = Instantiate(memberPrefab, clanMembersContent);
                Text[] texts = memberObj.GetComponentsInChildren<Text>();
                if (texts.Length >= 1)
                {
                    texts[0].text = member;
                }
                
                // 添加操作按钮（只有队长可以操作）
                if (currentClan.GetLeader() == "Player1") // 假设当前用户是Player1
                {
                    Button[] buttons = memberObj.GetComponentsInChildren<Button>();
                    if (buttons.Length >= 2)
                    {
                        buttons[0].onClick.AddListener(() => KickMember(member));
                        buttons[1].onClick.AddListener(() => PromoteMember(member));
                    }
                }
            }
        }
    }
    
    public void KickMember(string memberName)
    {
        Clan currentClan = ClanManager.Instance.GetCurrentClan();
        if (currentClan != null && currentClan.GetLeader() == "Player1")
        {
            ClanManager.Instance.KickFromClan(memberName, currentClan);
            UpdateClanMembers();
        }
    }
    
    public void PromoteMember(string memberName)
    {
        Clan currentClan = ClanManager.Instance.GetCurrentClan();
        if (currentClan != null && currentClan.GetLeader() == "Player1")
        {
            ClanManager.Instance.PromoteToLeader(memberName, currentClan);
            UpdateClanInfo();
            UpdateClanMembers();
        }
    }
    
    public void OpenCreateClanUI()
    {
        // 打开创建战队的UI
        Debug.Log("打开创建战队界面");
    }
    
    public void CreateClan(string name, string description)
    {
        ClanManager.Instance.CreateClan(name, description);
        UpdateClanInfo();
        UpdateClanMembers();
    }
    
    public void JoinClan(string clanName)
    {
        Clan clan = ClanManager.Instance.GetClanByName(clanName);
        if (clan != null)
        {
            ClanManager.Instance.JoinClan(clan);
            UpdateClanInfo();
            UpdateClanMembers();
        }
    }
    
    public void LeaveClan()
    {
        ClanManager.Instance.LeaveClan();
        UpdateClanInfo();
        UpdateClanMembers();
    }
}
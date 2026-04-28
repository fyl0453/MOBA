using UnityEngine;

public class UITestManager : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("开始初始化系统...");
        
        // 初始化所有系统
        InitializeSystems();
        
        // 测试数据
        TestData();
        
        Debug.Log("系统初始化完成");
    }
    
    private void InitializeSystems()
    {
        // 初始化职业生涯系统
        var careerManager = KingCareerSystemDetailedManager.Instance;
        Debug.Log("职业生涯系统初始化完成");
        
        // 初始化经济系统
        var economyManager = EconomySystemDetailedManager.Instance;
        Debug.Log("经济系统初始化完成");
        
        // 初始化社交系统
        var socialManager = SocialSystemDetailedManager.Instance;
        Debug.Log("社交系统初始化完成");
        
        // 初始化邮件系统
        var mailManager = MailSystemDetailedManager.Instance;
        Debug.Log("邮件系统初始化完成");
        
        // 初始化任务系统
        var questManager = QuestSystemDetailedManager.Instance;
        Debug.Log("任务系统初始化完成");
        
        // 初始化活动系统
        var eventManager = EventSystemDetailedManager.Instance;
        Debug.Log("活动系统初始化完成");
        
        // 初始化排行榜系统
        var leaderboardManager = LeaderboardSystemDetailedManager.Instance;
        Debug.Log("排行榜系统初始化完成");
        
        // 初始化背包系统
        var inventoryManager = InventorySystemDetailedManager.Instance;
        Debug.Log("背包系统初始化完成");
        
        // 初始化装备系统
        var equipmentManager = EquipmentSystemDetailedManager.Instance;
        Debug.Log("装备系统初始化完成");
        
        // 初始化皮肤系统
        var skinManager = SkinSystemDetailedManager.Instance;
        Debug.Log("皮肤系统初始化完成");
    }
    
    private void TestData()
    {
        // 测试职业生涯系统
        var careerManager = KingCareerSystemDetailedManager.Instance;
        careerManager.UpdateMatchResult(true, 5, 2, 3, 10000, 5000, 2000, 8000, 20, 2, 1, 0, 5);
        Debug.Log("职业生涯系统测试完成");
        
        // 测试经济系统
        var economyManager = EconomySystemDetailedManager.Instance;
        economyManager.AddCurrency("player_001", "gold", 1000);
        economyManager.AddCurrency("player_001", "diamond", 100);
        Debug.Log("经济系统测试完成");
        
        // 测试社交系统
        var socialManager = SocialSystemDetailedManager.Instance;
        socialManager.AddFriend("player_001", "player_002", "好友2");
        socialManager.AddFriend("player_001", "player_003", "好友3");
        Debug.Log("社交系统测试完成");
        
        // 测试邮件系统
        var mailManager = MailSystemDetailedManager.Instance;
        mailManager.SendSystemMail("player_001", "系统通知", "欢迎来到游戏！");
        mailManager.SendRewardMail("player_001", "奖励邮件", "恭喜获得100金币！", new string[] { "金币:100" });
        Debug.Log("邮件系统测试完成");
        
        // 测试任务系统
        var questManager = QuestSystemDetailedManager.Instance;
        questManager.AddCustomQuest("quest_001", "日常任务", "daily", "完成1场游戏", 1, System.DateTime.Now.AddDays(1), new string[] { "金币:50" });
        Debug.Log("任务系统测试完成");
        
        // 测试排行榜系统
        var leaderboardManager = LeaderboardSystemDetailedManager.Instance;
        leaderboardManager.UpdatePlayerScore("player_001", "玩家1", "ranking", 1000, 10, 5, 3);
        leaderboardManager.UpdatePlayerScore("player_002", "玩家2", "ranking", 1200, 12, 3, 4);
        Debug.Log("排行榜系统测试完成");
    }
}

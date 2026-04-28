using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{
    public HeroController heroController;
    
    [SyncVar(hook = "OnPlayerNameChanged")]
    public string playerName;
    
    [SyncVar(hook = "OnPlayerTeamChanged")]
    public int playerTeam;
    
    [SyncVar(hook = "OnPlayerHealthChanged")]
    public float playerHealth;
    
    private void Start()
    {
        if (isLocalPlayer)
        {
            // 本地玩家
            Debug.Log("Local player spawned");
        }
        else
        {
            // 远程玩家
            Debug.Log("Remote player spawned: " + playerName);
        }
    }
    
    private void OnPlayerNameChanged(string newName)
    {
        playerName = newName;
    }
    
    private void OnPlayerTeamChanged(int newTeam)
    {
        playerTeam = newTeam;
    }
    
    private void OnPlayerHealthChanged(float newHealth)
    {
        playerHealth = newHealth;
        if (heroController != null)
        {
            heroController.heroData.baseStats.currentHealth = newHealth;
        }
    }
    
    [Command]
    public void CmdUpdateHealth(float health)
    {
        playerHealth = health;
    }
    
    [Command]
    public void CmdAttack(GameObject target, float damage)
    {
        // 处理攻击命令
    }
    
    [Command]
    public void CmdCastSkill(int skillIndex, Vector3 targetPosition)
    {
        // 处理技能释放命令
    }
}
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public static TurretManager Instance { get; private set; }
    
    public List<Turret> turrets = new List<Turret>();
    public List<Base> bases = new List<Base>();
    
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
        InitializeTurrets();
        InitializeBases();
    }
    
    private void InitializeTurrets()
    {
        // 创建防御塔
        // 队伍1防御塔
        CreateTurret("Team1_Inner_Turret", new Vector3(-25, 0, 25), 1, Turret.TurretType.Inner, 1);
        CreateTurret("Team1_Outer_Turret1", new Vector3(-30, 0, 15), 1, Turret.TurretType.Outer, 1);
        CreateTurret("Team1_Outer_Turret2", new Vector3(-15, 0, 30), 1, Turret.TurretType.Outer, 1);
        
        // 队伍2防御塔
        CreateTurret("Team2_Inner_Turret", new Vector3(25, 0, -25), 2, Turret.TurretType.Inner, 2);
        CreateTurret("Team2_Outer_Turret1", new Vector3(30, 0, -15), 2, Turret.TurretType.Outer, 2);
        CreateTurret("Team2_Outer_Turret2", new Vector3(15, 0, -30), 2, Turret.TurretType.Outer, 2);
    }
    
    private void InitializeBases()
    {
        // 创建基地
        CreateBase("Team1_Base", new Vector3(-40, 0, 40), 1);
        CreateBase("Team2_Base", new Vector3(40, 0, -40), 2);
    }
    
    private void CreateTurret(string name, Vector3 position, int team, Turret.TurretType type, int teamId)
    {
        GameObject turretObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        turretObj.name = name;
        turretObj.transform.position = position;
        turretObj.transform.localScale = new Vector3(3, 5, 3);
        
        Turret turret = turretObj.AddComponent<Turret>();
        turret.turretName = name;
        turret.turretType = type;
        turret.teamId = teamId;
        
        // 根据防御塔类型设置属性
        switch (type)
        {
            case Turret.TurretType.Inner:
                turret.maxHealth = 3500;
                turret.attackDamage = 300;
                turret.attackRange = 10;
                turret.attackSpeed = 0.5f;
                turret.armor = 100;
                turret.magicResistance = 50;
                break;
            case Turret.TurretType.Outer:
                turret.maxHealth = 3000;
                turret.attackDamage = 250;
                turret.attackRange = 8;
                turret.attackSpeed = 0.6f;
                turret.armor = 80;
                turret.magicResistance = 40;
                break;
            case Turret.TurretType.Inhibitor:
                turret.maxHealth = 4000;
                turret.attackDamage = 350;
                turret.attackRange = 12;
                turret.attackSpeed = 0.4f;
                turret.armor = 120;
                turret.magicResistance = 60;
                break;
            case Turret.TurretType.Nexus:
                turret.maxHealth = 5000;
                turret.attackDamage = 400;
                turret.attackRange = 15;
                turret.attackSpeed = 0.3f;
                turret.armor = 150;
                turret.magicResistance = 80;
                break;
        }
        
        turret.currentHealth = turret.maxHealth;
        turrets.Add(turret);
    }
    
    private void CreateBase(string name, Vector3 position, int teamId)
    {
        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseObj.name = name;
        baseObj.transform.position = position;
        baseObj.transform.localScale = new Vector3(8, 8, 8);
        
        Base baseComponent = baseObj.AddComponent<Base>();
        baseComponent.baseName = name;
        baseComponent.teamId = teamId;
        baseComponent.maxHealth = 10000;
        baseComponent.currentHealth = baseComponent.maxHealth;
        baseComponent.armor = 200;
        baseComponent.magicResistance = 100;
        
        bases.Add(baseComponent);
    }
    
    public List<Turret> GetTurretsForTeam(int teamId)
    {
        List<Turret> teamTurrets = new List<Turret>();
        foreach (Turret turret in turrets)
        {
            if (turret.teamId == teamId)
            {
                teamTurrets.Add(turret);
            }
        }
        return teamTurrets;
    }
    
    public Base GetBaseForTeam(int teamId)
    {
        return bases.Find(b => b.teamId == teamId);
    }
    
    public Turret GetTurretByName(string name)
    {
        return turrets.Find(t => t.turretName == name);
    }
}
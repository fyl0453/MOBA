using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour
{
    public RectTransform miniMapRect;
    public Image playerIcon;
    public Image[] enemyIcons;
    
    public GameObject towerIconPrefab;
    public GameObject minionIconPrefab;
    public GameObject monsterIconPrefab;
    public GameObject grassIconPrefab;
    
    private HeroController heroController;
    private Vector3 mapCenter;
    private float mapScale = 100f;
    
    private Dictionary<GameObject, Transform> objectIcons = new Dictionary<GameObject, Transform>();
    
    private void Start()
    {
        heroController = FindObjectOfType<HeroController>();
        mapCenter = new Vector3(0, 0, 0);
    }
    
    private void Update()
    {
        if (heroController != null)
        {
            UpdatePlayerIcon();
            UpdateMiniMapObjects();
        }
    }
    
    private void UpdatePlayerIcon()
    {
        Vector3 playerPosition = heroController.transform.position;
        Vector3 relativePosition = playerPosition - mapCenter;
        Vector2 iconPosition = new Vector2(relativePosition.x, relativePosition.z) * mapScale;
        
        if (playerIcon != null && miniMapRect != null)
        {
            playerIcon.rectTransform.anchoredPosition = iconPosition;
        }
    }
    
    private void UpdateMiniMapObjects()
    {
        // 更新防御塔
        UpdateTowers();
        
        // 更新小兵
        UpdateMinions();
        
        // 更新野怪
        UpdateMonsters();
        
        // 更新草丛
        UpdateGrass();
    }
    
    private void UpdateTowers()
    {
        Turret[] towers = FindObjectsOfType<Turret>();
        foreach (Turret tower in towers)
        {
            if (!objectIcons.ContainsKey(tower.gameObject))
            {
                Transform icon = Instantiate(towerIconPrefab, miniMapRect).transform;
                objectIcons[tower.gameObject] = icon;
            }
            
            Transform iconTransform = objectIcons[tower.gameObject];
            Vector3 towerPos = tower.transform.position;
            Vector3 relativePos = towerPos - mapCenter;
            Vector2 minimapPos = new Vector2(relativePos.x, relativePos.z) * mapScale;
            iconTransform.GetComponent<RectTransform>().anchoredPosition = minimapPos;
            
            // 根据防御塔状态设置图标颜色
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.color = tower.teamId == 1 ? Color.blue : Color.red;
            }
        }
    }
    
    private void UpdateMinions()
    {
        Minion[] minions = FindObjectsOfType<Minion>();
        foreach (Minion minion in minions)
        {
            if (!objectIcons.ContainsKey(minion.gameObject))
            {
                Transform icon = Instantiate(minionIconPrefab, miniMapRect).transform;
                objectIcons[minion.gameObject] = icon;
            }
            
            Transform iconTransform = objectIcons[minion.gameObject];
            Vector3 minionPos = minion.transform.position;
            Vector3 relativePos = minionPos - mapCenter;
            Vector2 minimapPos = new Vector2(relativePos.x, relativePos.z) * mapScale;
            iconTransform.GetComponent<RectTransform>().anchoredPosition = minimapPos;
            
            // 根据小兵队伍设置图标颜色
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.color = minion.teamId == 1 ? Color.blue : Color.red;
            }
        }
    }
    
    private void UpdateMonsters()
    {
        Minion[] monsters = FindObjectsOfType<Minion>();
        foreach (Minion monster in monsters)
        {
            if (monster.minionType == Minion.MinionType.Buff || monster.minionType == Minion.MinionType.Boss)
            {
                if (!objectIcons.ContainsKey(monster.gameObject))
                {
                    Transform icon = Instantiate(monsterIconPrefab, miniMapRect).transform;
                    objectIcons[monster.gameObject] = icon;
                }
                
                Transform iconTransform = objectIcons[monster.gameObject];
                Vector3 monsterPos = monster.transform.position;
                Vector3 relativePos = monsterPos - mapCenter;
                Vector2 minimapPos = new Vector2(relativePos.x, relativePos.z) * mapScale;
                iconTransform.GetComponent<RectTransform>().anchoredPosition = minimapPos;
                
                // 根据野怪类型设置图标颜色
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    if (monster.minionType == Minion.MinionType.Boss)
                    {
                        iconImage.color = Color.yellow;
                    }
                    else
                    {
                        iconImage.color = Color.green;
                    }
                }
            }
        }
    }
    
    private void UpdateGrass()
    {
        Grass[] grasses = FindObjectsOfType<Grass>();
        foreach (Grass grass in grasses)
        {
            if (!objectIcons.ContainsKey(grass.gameObject))
            {
                Transform icon = Instantiate(grassIconPrefab, miniMapRect).transform;
                objectIcons[grass.gameObject] = icon;
            }
            
            Transform iconTransform = objectIcons[grass.gameObject];
            Vector3 grassPos = grass.transform.position;
            Vector3 relativePos = grassPos - mapCenter;
            Vector2 minimapPos = new Vector2(relativePos.x, relativePos.z) * mapScale;
            iconTransform.GetComponent<RectTransform>().anchoredPosition = minimapPos;
        }
    }
    
    private void CleanupIcons()
    {
        List<GameObject> objectsToRemove = new List<GameObject>();
        foreach (var pair in objectIcons)
        {
            if (pair.Key == null)
            {
                Destroy(pair.Value.gameObject);
                objectsToRemove.Add(pair.Key);
            }
        }
        
        foreach (GameObject obj in objectsToRemove)
        {
            objectIcons.Remove(obj);
        }
    }
    
    private void OnDestroy()
    {
        foreach (var pair in objectIcons)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value.gameObject);
            }
        }
    }
}
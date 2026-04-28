using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    
    private List<Item> allItems = new List<Item>();
    private List<Item> shopItems = new List<Item>();
    private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();
    
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
        InitializeItems();
    }
    
    private void InitializeItems()
    {
        // 创建基础装备
        CreateBasicItems();
        
        // 创建合成装备
        CreateCompositeItems();
        
        // 初始化商店
        InitializeShop();
    }
    
    private void CreateBasicItems()
    {
        // 武器
        Item longsword = new Item("长剑", "增加攻击力", 350, Item.ItemType.Weapon, Item.ItemRarity.Common);
        longsword.attackDamage = 20;
        allItems.Add(longsword);
        itemDictionary.Add(longsword.itemName, longsword);
        
        Item dagger = new Item("匕首", "增加攻击速度", 300, Item.ItemType.Weapon, Item.ItemRarity.Common);
        dagger.attackSpeed = 0.15f;
        allItems.Add(dagger);
        itemDictionary.Add(dagger.itemName, dagger);
        
        // 防具
        Item clothArmor = new Item("布甲", "增加护甲", 300, Item.ItemType.Armor, Item.ItemRarity.Common);
        clothArmor.armor = 20;
        allItems.Add(clothArmor);
        itemDictionary.Add(clothArmor.itemName, clothArmor);
        
        Item magicResistCloak = new Item("抗魔披风", "增加魔法抗性", 250, Item.ItemType.Armor, Item.ItemRarity.Common);
        magicResistCloak.magicResistance = 20;
        allItems.Add(magicResistCloak);
        itemDictionary.Add(magicResistCloak.itemName, magicResistCloak);
        
        // 饰品
        Item healthPotion = new Item("生命药水", "恢复生命值", 50, Item.ItemType.Consumable, Item.ItemRarity.Common);
        healthPotion.isConsumable = true;
        healthPotion.maxStack = 5;
        allItems.Add(healthPotion);
        itemDictionary.Add(healthPotion.itemName, healthPotion);
        
        // 鞋子
        Item boots = new Item("神速之靴", "增加移动速度", 300, Item.ItemType.Boots, Item.ItemRarity.Common);
        allItems.Add(boots);
        itemDictionary.Add(boots.itemName, boots);
    }
    
    private void CreateCompositeItems()
    {
        // 创建合成装备
        Item infinityEdge = new Item("无尽战刃", "增加大量攻击力和暴击", 3400, Item.ItemType.Weapon, Item.ItemRarity.Legendary);
        infinityEdge.attackDamage = 80;
        allItems.Add(infinityEdge);
        itemDictionary.Add(infinityEdge.itemName, infinityEdge);
        
        Item guardianAngel = new Item("守护天使", "增加护甲和魔法抗性，死亡后复活", 2800, Item.ItemType.Armor, Item.ItemRarity.Legendary);
        guardianAngel.armor = 60;
        guardianAngel.magicResistance = 40;
        allItems.Add(guardianAngel);
        itemDictionary.Add(guardianAngel.itemName, guardianAngel);
    }
    
    private void InitializeShop()
    {
        // 添加基础装备到商店
        shopItems.Add(itemDictionary["长剑"]);
        shopItems.Add(itemDictionary["匕首"]);
        shopItems.Add(itemDictionary["布甲"]);
        shopItems.Add(itemDictionary["抗魔披风"]);
        shopItems.Add(itemDictionary["生命药水"]);
        shopItems.Add(itemDictionary["神速之靴"]);
    }
    
    public List<Item> GetShopItems()
    {
        return shopItems;
    }
    
    public Item GetItemByName(string name)
    {
        if (itemDictionary.ContainsKey(name))
        {
            return itemDictionary[name];
        }
        return null;
    }
    
    public bool CanAffordItem(Item item, int gold)
    {
        return gold >= item.price;
    }
    
    public void PurchaseItem(Item item, HeroStats stats, ref int gold)
    {
        if (CanAffordItem(item, gold))
        {
            gold -= item.price;
            item.ApplyEffects(stats);
            Debug.Log($"Purchased {item.itemName}");
        }
    }
    
    public void SellItem(Item item, HeroStats stats, ref int gold)
    {
        item.RemoveEffects(stats);
        gold += item.sellPrice;
        Debug.Log($"Sold {item.itemName} for {item.sellPrice} gold");
    }
}
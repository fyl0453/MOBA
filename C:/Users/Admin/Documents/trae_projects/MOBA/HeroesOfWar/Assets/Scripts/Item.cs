[System.Serializable]
public class Item
{
    public string itemName;
    public string itemDescription;
    public int price;
    public int sellPrice;
    public ItemType itemType;
    public ItemRarity rarity;
    
    public float attackDamage;
    public float attackSpeed;
    public float armor;
    public float magicResistance;
    public float health;
    public float mana;
    public float cooldownReduction;
    public float lifeSteal;
    public float magicPenetration;
    public float armorPenetration;
    
    public bool isUnique;
    public bool isConsumable;
    public int maxStack;
    public int currentStack;
    
    public enum ItemType
    {
        Weapon,
        Armor,
        Accessory,
        Consumable,
        Boots
    }
    
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    
    public Item(string name, string description, int itemPrice, ItemType type, ItemRarity itemRarity)
    {
        itemName = name;
        itemDescription = description;
        price = itemPrice;
        sellPrice = price / 2;
        itemType = type;
        rarity = itemRarity;
        
        attackDamage = 0;
        attackSpeed = 0;
        armor = 0;
        magicResistance = 0;
        health = 0;
        mana = 0;
        cooldownReduction = 0;
        lifeSteal = 0;
        magicPenetration = 0;
        armorPenetration = 0;
        
        isUnique = false;
        isConsumable = false;
        maxStack = 1;
        currentStack = 1;
    }
    
    public void ApplyEffects(HeroStats stats)
    {
        stats.attackDamage += attackDamage;
        stats.attackSpeed += attackSpeed;
        stats.armor += armor;
        stats.magicResistance += magicResistance;
        stats.maxHealth += health;
        stats.currentHealth += health;
    }
    
    public void RemoveEffects(HeroStats stats)
    {
        stats.attackDamage -= attackDamage;
        stats.attackSpeed -= attackSpeed;
        stats.armor -= armor;
        stats.magicResistance -= magicResistance;
        stats.maxHealth -= health;
        if (stats.currentHealth > stats.maxHealth)
        {
            stats.currentHealth = stats.maxHealth;
        }
    }
}
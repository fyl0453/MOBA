using UnityEngine;

[System.Serializable]
public class Skin
{
    public string skinName;
    public string skinDescription;
    public int price;
    public bool isUnlocked;
    public string modelPath;
    public string iconPath;
    public Color primaryColor;
    public Color secondaryColor;
    public string particleEffectPath;
    public string soundEffectPath;
    
    public enum SkinRarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Limited
    }
    
    public SkinRarity rarity;
    
    public Skin(string name, string description, int skinPrice, SkinRarity skinRarity, string model, string icon)
    {
        skinName = name;
        skinDescription = description;
        price = skinPrice;
        rarity = skinRarity;
        modelPath = model;
        iconPath = icon;
        isUnlocked = false;
    }
    
    public void Unlock()
    {
        isUnlocked = true;
    }
    
    public bool IsUnlocked()
    {
        return isUnlocked;
    }
    
    public void ApplySkin(GameObject hero)
    {
        // 应用皮肤模型
        if (!string.IsNullOrEmpty(modelPath))
        {
            // 这里应该加载皮肤模型并替换当前模型
        }
        
        // 应用皮肤颜色
        Renderer[] renderers = hero.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material != null)
            {
                renderer.material.color = primaryColor;
            }
        }
        
        // 应用特效
        if (!string.IsNullOrEmpty(particleEffectPath))
        {
            // 加载并应用粒子特效
        }
    }
}
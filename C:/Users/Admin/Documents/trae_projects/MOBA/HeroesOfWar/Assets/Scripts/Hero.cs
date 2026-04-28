using System.Collections.Generic;

[System.Serializable]
public class Hero
{
    public string heroName;
    public string heroDescription;
    public HeroStats baseStats;
    public List<Skill> skills;
    public string modelPath;
    public string iconPath;
    
    public Hero(string name, string description, HeroStats stats, List<Skill> skillList, string model, string icon)
    {
        heroName = name;
        heroDescription = description;
        baseStats = stats;
        skills = skillList;
        modelPath = model;
        iconPath = icon;
    }
}
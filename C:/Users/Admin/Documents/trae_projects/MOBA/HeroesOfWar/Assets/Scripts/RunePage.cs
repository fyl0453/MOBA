using System.Collections.Generic;

[System.Serializable]
public class RunePage
{
    public string pageName;
    public List<Rune> marks = new List<Rune>();
    public List<Rune> seals = new List<Rune>();
    public List<Rune> glyphs = new List<Rune>();
    public List<Rune> quintessences = new List<Rune>();
    
    public RunePage(string name)
    {
        pageName = name;
    }
    
    public void ApplyRunes(HeroStats stats)
    {
        foreach (Rune rune in marks)
        {
            rune.ApplyEffect(stats);
        }
        foreach (Rune rune in seals)
        {
            rune.ApplyEffect(stats);
        }
        foreach (Rune rune in glyphs)
        {
            rune.ApplyEffect(stats);
        }
        foreach (Rune rune in quintessences)
        {
            rune.ApplyEffect(stats);
        }
    }
    
    public void RemoveRunes(HeroStats stats)
    {
        foreach (Rune rune in marks)
        {
            rune.RemoveEffect(stats);
        }
        foreach (Rune rune in seals)
        {
            rune.RemoveEffect(stats);
        }
        foreach (Rune rune in glyphs)
        {
            rune.RemoveEffect(stats);
        }
        foreach (Rune rune in quintessences)
        {
            rune.RemoveEffect(stats);
        }
    }
}
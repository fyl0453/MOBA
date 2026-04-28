using UnityEngine;
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }
    
    public List<AIHeroController> aiHeroes = new List<AIHeroController>();
    public AILevel aiLevel = AILevel.Normal;
    
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
    
    public void AddAIHero(AIHeroController aiHero)
    {
        if (!aiHeroes.Contains(aiHero))
        {
            aiHeroes.Add(aiHero);
        }
    }
    
    public void RemoveAIHero(AIHeroController aiHero)
    {
        aiHeroes.Remove(aiHero);
    }
    
    public void SetAILevel(AILevel level)
    {
        aiLevel = level;
        
        foreach (AIHeroController aiHero in aiHeroes)
        {
            aiHero.SetAILevel(level);
        }
    }
    
    public void UpdateAI()
    {
        foreach (AIHeroController aiHero in aiHeroes)
        {
            if (aiHero != null && aiHero.isAlive)
            {
                aiHero.UpdateAI();
            }
        }
    }
}

public enum AILevel
{
    Easy,
    Normal,
    Hard,
    Expert
}

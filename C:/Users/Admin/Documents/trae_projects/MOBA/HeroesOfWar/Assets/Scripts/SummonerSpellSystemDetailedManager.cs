using System;
using System.Collections.Generic;

public class SummonerSpellSystemDetailedManager
{
    private static SummonerSpellSystemDetailedManager _instance;
    public static SummonerSpellSystemDetailedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SummonerSpellSystemDetailedManager();
            }
            return _instance;
        }
    }

    private SummonerSpellSystemData spellData;
    private SummonerSpellSystemDataManager dataManager;

    private SummonerSpellSystemDetailedManager()
    {
        dataManager = SummonerSpellSystemDataManager.Instance;
        spellData = dataManager.spellData;
    }

    public void InitializePlayerSpells(string playerID)
    {
        if (!spellData.PlayerSpells.ContainsKey(playerID))
        {
            spellData.PlayerSpells[playerID] = new List<PlayerSummonerSpell>();
        }
        if (!spellData.PlayerSpellSlots.ContainsKey(playerID))
        {
            spellData.PlayerSpellSlots[playerID] = new List<SummonerSpellSlot>();
            for (int i = 0; i < 2; i++)
            {
                string slotID = "slot_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                SummonerSpellSlot slot = new SummonerSpellSlot(slotID, playerID, i);
                spellData.AddPlayerSpellSlot(playerID, slot);
            }
        }
        dataManager.SaveSpellData();
        Debug.Log("初始化召唤师技能成功");
    }

    public void UnlockSpell(string playerID, string spellID, int playerLevel)
    {
        SummonerSpell spell = GetSpell(spellID);
        if (spell != null && playerLevel >= spell.LevelRequired)
        {
            InitializePlayerSpells(playerID);
            List<PlayerSummonerSpell> playerSpells = spellData.PlayerSpells[playerID];
            if (!playerSpells.Exists(s => s.SpellID == spellID))
            {
                PlayerSummonerSpell playerSpell = new PlayerSummonerSpell(playerID, spellID);
                playerSpell.IsEquipped = false;
                spellData.AddPlayerSpell(playerID, playerSpell);
                spell.IsUnlocked = true;
                dataManager.CreateSpellEvent("spell_unlock", playerID, spellID, "解锁技能: " + spell.SpellName);
                dataManager.SaveSpellData();
                Debug.Log("解锁召唤师技能成功: " + spell.SpellName);
            }
        }
    }

    public void EquipSpell(string playerID, string spellID, int slotIndex)
    {
        InitializePlayerSpells(playerID);
        List<PlayerSummonerSpell> playerSpells = spellData.PlayerSpells[playerID];
        PlayerSummonerSpell playerSpell = playerSpells.Find(s => s.SpellID == spellID);
        if (playerSpell != null)
        {
            List<SummonerSpellSlot> slots = spellData.PlayerSpellSlots[playerID];
            if (slotIndex >= 0 && slotIndex < slots.Count)
            {
                SummonerSpellSlot slot = slots[slotIndex];
                if (!string.IsNullOrEmpty(slot.SpellID))
                {
                    PlayerSummonerSpell oldSpell = playerSpells.Find(s => s.SpellID == slot.SpellID);
                    if (oldSpell != null)
                    {
                        oldSpell.IsEquipped = false;
                    }
                }
                slot.SpellID = spellID;
                slot.EquipTime = DateTime.Now;
                playerSpell.IsEquipped = true;
                dataManager.CreateSpellEvent("spell_equip", playerID, spellID, "装备技能到槽位" + slotIndex);
                dataManager.SaveSpellData();
                Debug.Log("装备召唤师技能成功: " + spellID);
            }
        }
    }

    public void UnequipSpell(string playerID, int slotIndex)
    {
        InitializePlayerSpells(playerID);
        List<SummonerSpellSlot> slots = spellData.PlayerSpellSlots[playerID];
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            SummonerSpellSlot slot = slots[slotIndex];
            if (!string.IsNullOrEmpty(slot.SpellID))
            {
                List<PlayerSummonerSpell> playerSpells = spellData.PlayerSpells[playerID];
                PlayerSummonerSpell playerSpell = playerSpells.Find(s => s.SpellID == slot.SpellID);
                if (playerSpell != null)
                {
                    playerSpell.IsEquipped = false;
                }
                slot.SpellID = "";
                dataManager.CreateSpellEvent("spell_unequip", playerID, "", "从槽位" + slotIndex + "卸下技能");
                dataManager.SaveSpellData();
                Debug.Log("卸下召唤师技能成功");
            }
        }
    }

    public void UseSpell(string playerID, string spellID)
    {
        List<PlayerSummonerSpell> playerSpells = spellData.PlayerSpells[playerID];
        PlayerSummonerSpell playerSpell = playerSpells.Find(s => s.SpellID == spellID);
        if (playerSpell != null)
        {
            playerSpell.UsageCount++;
            playerSpell.LastUsed = DateTime.Now;
            playerSpell.Experience += 10;
            CheckSpellLevelUp(playerSpell);
            dataManager.CreateSpellEvent("spell_use", playerID, spellID, "使用技能");
            dataManager.SaveSpellData();
            Debug.Log("使用召唤师技能成功: " + spellID);
        }
    }

    private void CheckSpellLevelUp(PlayerSummonerSpell playerSpell)
    {
        for (int i = playerSpell.Level; i < spellData.SpellLevels.Count; i++)
        {
            SpellLevel nextLevel = spellData.SpellLevels[i];
            if (playerSpell.Experience >= nextLevel.RequiredExperience)
            {
                playerSpell.Level = nextLevel.Level;
                dataManager.CreateSpellEvent("spell_level_up", playerSpell.PlayerID, playerSpell.SpellID, "技能升级到" + nextLevel.Level + "级");
                Debug.Log("召唤师技能升级成功: " + nextLevel.Level + "级");
            }
            else
            {
                break;
            }
        }
    }

    public SummonerSpell GetSpell(string spellID)
    {
        return spellData.AllSpells.Find(s => s.SpellID == spellID);
    }

    public List<SummonerSpell> GetAllSpells()
    {
        return spellData.AllSpells;
    }

    public List<SummonerSpell> GetUnlockedSpells(int playerLevel)
    {
        return spellData.AllSpells.FindAll(s => s.LevelRequired <= playerLevel);
    }

    public List<PlayerSummonerSpell> GetPlayerSpells(string playerID)
    {
        if (spellData.PlayerSpells.ContainsKey(playerID))
        {
            return spellData.PlayerSpells[playerID];
        }
        return new List<PlayerSummonerSpell>();
    }

    public List<SummonerSpellSlot> GetPlayerSpellSlots(string playerID)
    {
        if (spellData.PlayerSpellSlots.ContainsKey(playerID))
        {
            return spellData.PlayerSpellSlots[playerID];
        }
        return new List<SummonerSpellSlot>();
    }

    public PlayerSummonerSpell GetPlayerSpell(string playerID, string spellID)
    {
        if (spellData.PlayerSpells.ContainsKey(playerID))
        {
            return spellData.PlayerSpells[playerID].Find(s => s.SpellID == spellID);
        }
        return null;
    }

    public int GetSpellCooldown(string playerID, string spellID)
    {
        SummonerSpell spell = GetSpell(spellID);
        if (spell != null)
        {
            PlayerSummonerSpell playerSpell = GetPlayerSpell(playerID, spellID);
            if (playerSpell != null)
            {
                SpellLevel level = GetSpellLevel(playerSpell.Level);
                if (level != null)
                {
                    return Math.Max(0, spell.Cooldown - level.CooldownReduction);
                }
            }
            return spell.Cooldown;
        }
        return 0;
    }

    public int GetSpellManaCost(string playerID, string spellID)
    {
        SummonerSpell spell = GetSpell(spellID);
        if (spell != null)
        {
            PlayerSummonerSpell playerSpell = GetPlayerSpell(playerID, spellID);
            if (playerSpell != null)
            {
                SpellLevel level = GetSpellLevel(playerSpell.Level);
                if (level != null)
                {
                    return Math.Max(0, spell.ManaCost - level.ManaCostReduction);
                }
            }
            return spell.ManaCost;
        }
        return 0;
    }

    public SpellLevel GetSpellLevel(int level)
    {
        return spellData.SpellLevels.Find(l => l.Level == level);
    }

    public List<SpellLevel> GetAllSpellLevels()
    {
        return spellData.SpellLevels;
    }

    public void CreateSpell(string spellName, string description, int cooldown, int manaCost, int levelRequired, string spellType)
    {
        string spellID = "spell_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        SummonerSpell spell = new SummonerSpell(spellID, spellName, description, cooldown, manaCost, levelRequired, spellType);
        spellData.AddSpell(spell);
        dataManager.SaveSpellData();
        Debug.Log("创建召唤师技能成功: " + spellName);
    }

    public void AddSpellLevel(int level, int requiredExperience, int cooldownReduction, int manaCostReduction, string bonusEffect)
    {
        SpellLevel newLevel = new SpellLevel(level, requiredExperience, cooldownReduction, manaCostReduction, bonusEffect);
        spellData.AddSpellLevel(newLevel);
        dataManager.SaveSpellData();
        Debug.Log("添加技能等级成功: " + level);
    }

    public void SaveData()
    {
        dataManager.SaveSpellData();
    }

    public void LoadData()
    {
        dataManager.LoadSpellData();
    }

    public List<SummonerSpellEvent> GetRecentEvents(int count)
    {
        return dataManager.GetRecentEvents(count);
    }
}
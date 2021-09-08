using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    private ItemFinder itemFinder;
    private CouCouFinder coucouFinder;
    private BattleSystem battleSystem;
    private BattleManager battleManager;

    public InventoryList playerInventory;
    public InventoryList enemyInventory;
    public ItemsDatabase itemDatabase;
    public CouCouDatabase couCouDatabase;
    public List<ItemsDatabase.ItemData> itemsDatabaseList;
    public List<CouCouDatabase.CouCouVariant> coucouVariantList;

    public bool experienceIncrement = false;
    public bool partyExperienceAdd = false;
    public int experienceGained = 0;
    public int restOfPartyGains = 0;
    public List<string> partyLevelUps;

    private void Awake()
    {
        battleSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleSystem>();
        battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();
        coucouFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CouCouFinder>();
        itemFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ItemFinder>();

        partyLevelUps = new List<string>();
        coucouVariantList = new List<CouCouDatabase.CouCouVariant>();
        itemsDatabaseList = new List<ItemsDatabase.ItemData>();

        LoadInventory(playerInventory);
        LoadInventory(enemyInventory);
    }

    private void Start()
    {
        foreach (ItemsDatabase.ItemData item in itemDatabase.itemData)
        {
            itemsDatabaseList.Add(item);
        }
        foreach (CouCouDatabase.CouCouVariant variant in couCouDatabase.coucouVariant)
        {
            coucouVariantList.Add(variant);
        }
        
        /* - GIVES PLAYER EVERY ITEM AND COUCOU - 
        foreach (ItemsDatabase.ItemData item in itemDatabase.itemData)
        {
            FoundItem(item.itemName, 3);
        }
        foreach (CouCouDatabase.CouCouData coucou in couCouDatabase.coucouData)
        {
            AddCouCou(coucou.coucouName, 1);
        }
        */

        SortItemInventory();
    }

    public void SortItemInventory()
    {
        playerInventory.itemInventory = playerInventory.itemInventory.OrderBy(e => itemFinder.FindItem(e.itemName).positionIndex).ToList();
    }

    public void SortCouCouInventory()
    {
        for (int i = 0; i < playerInventory.couCouInventory.Count; i++)
        {
            if (playerInventory.couCouInventory[i].hasCollapsed)
            {
                playerInventory.couCouInventory[i].lineupOrder = 100 + i;
            }
        }

        playerInventory.couCouInventory = playerInventory.couCouInventory.OrderBy(e => e.lineupOrder).ToList();

        for (int i = 0; i < playerInventory.couCouInventory.Count; i++)
        {
            playerInventory.couCouInventory[i].lineupOrder = i + 1;
        }
    }

    public void MoveCouCou(InventoryList.CouCouInventory coucou, int position)
    {
        playerInventory.couCouInventory.Remove(coucou);
        playerInventory.couCouInventory.Insert(position, coucou);
        for (int i = 0; i < playerInventory.couCouInventory.Count; i++)
        {
            playerInventory.couCouInventory[i].lineupOrder = i;
        }
    }

    public void LoadInventory(InventoryList inventory)
    {
        InventoryData data = SaveSystem.LoadInventory(inventory);

        if(data == null)
        {
            return;
        }
        inventory.couCouInventory.Clear();
        inventory.itemInventory.Clear();
        foreach (InventoryList.CouCouInventory c in data.coucouInventory)
        {
            inventory.couCouInventory.Add(c);
        }
        foreach (InventoryList.ItemInventory i in data.itemInventory)
        {
            inventory.itemInventory.Add(i);
        }
    }

    public void SaveInventory()
    {
        SortItemInventory();
        SortCouCouInventory();
        SaveSystem.SaveInventory(playerInventory);
        SaveSystem.SaveInventory(enemyInventory);
    }

    public void FoundItem(string name, int amount)
    {
        InventoryList.ItemInventory item = null;
        foreach (InventoryList.ItemInventory i in playerInventory.itemInventory.ToList())
        {
            if (name == i.itemName)
            {
                item = i;
                i.itemAmount += amount;
            }
        }

        if (item == null)
        {
            foreach (ItemsDatabase.ItemData i in itemsDatabaseList)
            {
                if (name == i.itemName)
                {
                    item = new InventoryList.ItemInventory()
                    { 
                        itemName = i.itemName, 
                        itemAmount = amount, 
                        element = i.element, 
                        itemAttribute = i.itemAttribute
                    };
                    playerInventory.itemInventory.Add(item);
                    break;
                }
            }
        }

        SortItemInventory();
    }

    public void UsedItem(string name)
    {
        foreach (InventoryList.ItemInventory i in playerInventory.itemInventory.ToList())
        {
            if (name == i.itemName)
            {
                i.itemAmount--;
                if (i.itemAmount <= 0)
                {
                    playerInventory.itemInventory.Remove(i);
                }
            }
        }
    }

    public void EnemyUsedItem(string name)
    {
        foreach (InventoryList.ItemInventory i in enemyInventory.itemInventory.ToList())
        {
            if (name == i.itemName)
            {
                i.itemAmount--;
                if (i.itemAmount <= 0)
                {
                    enemyInventory.itemInventory.Remove(i);
                }
            }
        }
    }

    public void AddCouCou(string coucouName, int level)
    {
        CouCouDatabase.CouCouData coucouData = coucouFinder.FindCouCou(coucouName);
        InventoryList.CouCouInventory coucou = new InventoryList.CouCouInventory()
        {
            coucouName = coucouData.coucouName,
            coucouLevel = level,
            lineupOrder = playerInventory.couCouInventory.Count,
            coucouVariant = coucouData.coucouVariant,
            element = coucouData.coucouElement
        };

        int bonusStatsPer5;
        int bonusStatsPer1;

        for (int a = 0; a < coucouVariantList.Count; a++)
        {
            if (coucou.coucouVariant == coucouVariantList[a].variant)
            {
                bonusStatsPer5 = Mathf.FloorToInt(level / 5);
                bonusStatsPer1 = level - bonusStatsPer5 - 1;

                coucou.maxHealth = coucouVariantList[a].hp + (coucouVariantList[a].bonusHP * bonusStatsPer1) + (coucouVariantList[a].bonusHPPer5 * bonusStatsPer5);
                coucou.currentAttack = coucouVariantList[a].attack + (coucouVariantList[a].bonusAttack * bonusStatsPer1) + (coucouVariantList[a].bonusAttackPer5 * bonusStatsPer5);
                coucou.currentResistance = coucouVariantList[a].resistance + (coucouVariantList[a].bonusResistance * bonusStatsPer1) + (coucouVariantList[a].bonusResistancePer5 * bonusStatsPer5);
                coucou.currentMindset = coucouVariantList[a].mindset;
                coucou.currentDetermination = coucouVariantList[a].determination;

                coucou.currentHealth = coucou.maxHealth;
            }
        }

        playerInventory.couCouInventory.Add(coucou);
    }

    public bool HasPlayableCouCou()
    {
        int amount = 0;
        foreach (InventoryList.CouCouInventory coucou in playerInventory.couCouInventory)
        {
            if (!coucou.hasCollapsed)
            {
                amount++;
            }
        }
        if (amount == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void AddExperience(InventoryList.CouCouInventory playerCouCou, int playerLevel, int enemyLevel, bool caught)
    {
        float expToAdd = Mathf.Max(enemyLevel - playerLevel, 1) * 100 * playerLevel;
        if (caught)
        {
            expToAdd *= 1.5f;
        }
        Debug.Log("exp to add " + expToAdd);
        experienceIncrement = false;
        experienceGained = Mathf.RoundToInt(expToAdd);
        StartCoroutine(IncrementallyIncreaseEXP(expToAdd, playerCouCou, enemyLevel, playerLevel));
    }

    public IEnumerator IncrementallyIncreaseEXP(float desiredEXP, InventoryList.CouCouInventory coucou, int enemyLevel, int playerLevel)
    {
        float increase = coucou.currentEXP;
        float maxEXP = Mathf.Pow(10 * coucou.coucouLevel, 2) / 4;

        while (increase != desiredEXP && coucou.currentEXP < maxEXP)
        {
            increase = Mathf.MoveTowards(increase, desiredEXP, Time.deltaTime * 1000f);
            battleSystem.experienceBar.fillAmount = increase / maxEXP;
            coucou.currentEXP = increase;
            yield return new WaitForSeconds(0.02f);
        }
        if (coucou.currentEXP >= maxEXP)
        {
            Debug.Log("level up");
            coucou.coucouLevel++;
            battleSystem.experienceBar.fillAmount = 0;
            coucou.currentEXP = 0;

            battleSystem.dialogueText.text = coucou.coucouName + " leveled up!";
            battleManager.allyLevelText.text = coucou.coucouLevel.ToString();
            battleManager.UpdateCouCouStats();
            yield return new WaitForSeconds(2f);

            if (coucou.currentEXP < desiredEXP)
            {
                StartCoroutine(IncrementallyIncreaseEXP(desiredEXP - increase, coucou, enemyLevel, playerLevel));
            }
        }
        else
        {
            restOfPartyGains = experienceGained / 3;

            for (int i = 0; i < playerInventory.couCouInventory.Count; i++)
            {
                if (playerInventory.couCouInventory[i].lineupOrder != 1 && playerInventory.couCouInventory[i].lineupOrder < 5)
                {
                    float maxEXPForI = Mathf.Pow(10 * playerInventory.couCouInventory[i].coucouLevel, 2) / 4;
                    playerInventory.couCouInventory[i].currentEXP += restOfPartyGains;

                    if (playerInventory.couCouInventory[i].currentEXP > maxEXPForI)
                    {
                        playerInventory.couCouInventory[i].coucouLevel++;
                        partyLevelUps.Add(playerInventory.couCouInventory[i].coucouName);
                        playerInventory.couCouInventory[i].currentEXP -= maxEXPForI;
                    }
                }
            }

            partyExperienceAdd = true;
        }
        experienceIncrement = true;
        yield break;
    }
}

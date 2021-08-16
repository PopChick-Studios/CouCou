using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    private AbilityDisplay abilityDisplay;
    private AbilityFinder abilityFinder;

    public InventoryList inventory;
    public CouCouDatabase coucouDatabase;
    public AbilitiesDatabase abilitiesDatabase;
    public ItemsDatabase itemsDatabase;

    // CHANGE THESE !!! ONLY FOR DEBUGGING
    [Space]
    public InventoryList.CouCouInventory activeCouCou;
    public List<InventoryList.CouCouInventory> coucouParty;

    private AbilitiesDatabase.AttackAbilityData attackAbility;
    private AbilitiesDatabase.UtilityAbilityData utilityAbility;
    private int psychicAbilitiesUsed = 0;

    public List<CouCouDatabase.CouCouData> coucouDataList;
    public List<CouCouDatabase.CouCouVariant> coucouVariantList;

    public List<ItemsDatabase> ItemsDatabaseList;

    [Header("Health Bars")]
    public GameObject allyHealthBar;
    public TextMeshProUGUI allyNameText;
    public TextMeshProUGUI allyLevelText;
    public TextMeshProUGUI allyHealthText;
    public Image allyElementSprite;
    public GameObject enemyHealthBar;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyLevelText;
    public TextMeshProUGUI enemyHealthText;
    public Image enemyElementSprite;

    private void Awake()
    {
        abilityFinder = GetComponent<AbilityFinder>();
        abilityDisplay = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<AbilityDisplay>();

        activeCouCou = new InventoryList.CouCouInventory();
        coucouParty = new List<InventoryList.CouCouInventory>();

        coucouDataList = new List<CouCouDatabase.CouCouData>();
        coucouVariantList = new List<CouCouDatabase.CouCouVariant>();

        ItemsDatabaseList = new List<ItemsDatabase>();
    }

    private void Start()
    {

        foreach (InventoryList.CouCouInventory c in inventory.couCouInventory)
        {
            if (c.onParty)
            {
                coucouParty.Insert(Mathf.Min(coucouParty.Count, c.lineupOrder), c);
            }
            if (c.isCurrentlyActive)
            {
                activeCouCou = c;
                if (c.onParty)
                {
                    coucouParty.Remove(c);
                }
                coucouParty.Insert(0, c);
            }
        }

        // If they haven't set up a party, default to the list given.
        for (int i = 0; coucouParty.Count < Mathf.Min(5, inventory.couCouInventory.Count); i++)
        {
            inventory.couCouInventory[coucouParty.Count].onParty = true;
            inventory.couCouInventory[coucouParty.Count].lineupOrder = coucouParty.Count;
            coucouParty.Add(inventory.couCouInventory[coucouParty.Count]);
        }

        // If they haven't set up an active CouCou, default to the first on the list
        if (activeCouCou == null)
        {
            coucouParty[0].isCurrentlyActive = true;
            inventory.couCouInventory[0].isCurrentlyActive = true;
            activeCouCou = coucouParty[0];
        }

        foreach (CouCouDatabase.CouCouData c in coucouDatabase.coucouData)
        {
            coucouDataList.Add(c);
        }
        foreach (CouCouDatabase.CouCouVariant c in coucouDatabase.coucouVariant)
        {
            coucouVariantList.Add(c);
        }

        // Give the CouCou in party stats
        int level;
        int bonusStatsPer5;
        int bonusStatsPer1;

        for (int i = 0; i < coucouParty.Count; i++)
        {
            for (int a = 1; a < coucouVariantList.Count; a++)
            {

                // Be sure too add variant to the inventory when catching CouCou

                if (coucouParty[i].coucouVariant == coucouVariantList[a].variant)
                {
                    level = coucouParty[i].coucouLevel;
                    bonusStatsPer5 = Mathf.FloorToInt(level / 5);
                    bonusStatsPer1 = level - bonusStatsPer5 - 1;

                    coucouParty[i].maxHealth = coucouVariantList[a].hp + (coucouVariantList[a].bonusHP * bonusStatsPer1) + (coucouVariantList[a].bonusHPPer5 * bonusStatsPer5);
                    coucouParty[i].currentAttack = coucouVariantList[a].attack + (coucouVariantList[a].bonusAttack * bonusStatsPer1) + (coucouVariantList[a].bonusAttackPer5 * bonusStatsPer5);
                    coucouParty[i].currentResistance = coucouVariantList[a].resistance + (coucouVariantList[a].bonusResistance * bonusStatsPer1) + (coucouVariantList[a].bonusResistancePer5 * bonusStatsPer5);
                    coucouParty[i].currentMindset = 10;
                    coucouParty[i].currentDetermination = 50;

                    // *********** REMOVE THIS ONCE YOU MADE THE CURRENT HEALTH SCRIPT *********** //
                    coucouParty[i].currentHealth = coucouParty[i].maxHealth;
                }
            }
        }

        UpdateHealthBarAlly();

        UpdateAbilities();
    }

    public void UpdateHealthBarAlly()
    {
        allyNameText.text = activeCouCou.coucouName;
        allyHealthBar.GetComponent<Image>().fillAmount = activeCouCou.currentHealth / activeCouCou.maxHealth;
        allyHealthText.text = activeCouCou.currentHealth + "/" + activeCouCou.maxHealth;
        allyLevelText.text = activeCouCou.coucouLevel.ToString();
        allyElementSprite = null; // Fix this when sprites are made
    }

    public void UpdateAbilities()
    {
        abilityDisplay.DisplayAbilities(activeCouCou.ability1, activeCouCou.ability2, activeCouCou.ability3, activeCouCou.ability4);
    }

    public void UseItem(string name)
    {

        Debug.Log("Item used");

        foreach (InventoryList.ItemInventory i in inventory.itemInventory)
        {
            if (name == i.itemName)
            {
                switch (i.itemAttribute)
                {
                    case ItemsDatabase.ItemAttribute.Health:

                        activeCouCou.currentHealth = Mathf.Min(activeCouCou.maxHealth, Mathf.CeilToInt(activeCouCou.currentHealth * 1.3f));

                        break;

                    case ItemsDatabase.ItemAttribute.Resistance:

                        activeCouCou.currentResistance = Mathf.CeilToInt(activeCouCou.currentResistance * 1.15f);

                        break;

                    case ItemsDatabase.ItemAttribute.Attack:

                        activeCouCou.currentAttack = Mathf.CeilToInt(activeCouCou.currentAttack * 1.15f);

                        break;

                    case ItemsDatabase.ItemAttribute.ElementalMindset:

                        if (activeCouCou.element != i.element)
                        {

                            // Dialogue "I can't use this on your CouCou"

                            break;
                        }

                        activeCouCou.currentMindset += 5;

                        break;

                    default:

                        // Dialogue "I can't use that right now"

                        break;
                }
            }
        }
    }

    public void ChangeCouCou(string name)
    {

        Debug.Log("CouCou Changed");

        // Recall Animation
        // Dialogue "Come back " + coucouname / "Have a rest ..." / "You've done a good job ..."

        foreach (InventoryList.CouCouInventory c in coucouParty)
        {
            if (name == c.coucouName)
            {
                activeCouCou = c;
            }
        }
    }

    public void UseAbility(AbilityUID button)
    {
        int abilityUID = button.abilityUID;
        bool isUtility = button.isUtility;

        if (isUtility)
        {
            if (button.utilityAbility.enemyMindset)
            {
                psychicAbilitiesUsed++;
                // Decrease enemy mindset
            }
            if (button.utilityAbility.canStun)
            {
                //DO ONCE YOU'VE ADDED ENEMY COUCOU
            }
        }
        else
        {
            //EnemyManager.TakeDamage(activeCouCou.currentAttack * button.attackAbility.damageMultiplier);
        }

    }
}

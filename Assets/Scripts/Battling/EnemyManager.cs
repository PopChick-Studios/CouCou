using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private AbilityFinder abilityFinder;
    private BattleManager battleManager;

    public InventoryList enemyInventory;
    public InventoryList.CouCouInventory enemyActiveCouCou = null;
    public List<InventoryList.CouCouInventory> enemyCouCouParty;

    public List<AbilitiesDatabase.AttackAbilityData> attackAbilities;
    public List<AbilitiesDatabase.UtilityAbilityData> utilityAbilities;

    public float[] cumulativeProbability;
    public float[] abilityProbability;
    public bool[] abilityAdvantage;

    public bool wild;

    public int turn;
    public int previousAbilityIndex;

    private void Awake()
    {
        attackAbilities = new List<AbilitiesDatabase.AttackAbilityData>();
        utilityAbilities = new List<AbilitiesDatabase.UtilityAbilityData>();
        cumulativeProbability = new float[4];
        abilityProbability = new float[4];

        abilityFinder = gameObject.GetComponent<AbilityFinder>();
        battleManager = gameObject.GetComponent<BattleManager>();
    }

    void Start()
    {
        foreach (InventoryList.CouCouInventory c in enemyInventory.couCouInventory)
        {
            if (c.onParty)
            {
                enemyCouCouParty.Insert(Mathf.Min(enemyCouCouParty.Count, c.lineupOrder), c);
            }
            if (c.isCurrentlyActive)
            {
                enemyActiveCouCou = c;
                if (c.onParty)
                {
                    enemyCouCouParty.Remove(c);
                }
                enemyCouCouParty.Insert(0, c);
            }
        }

        for (int i = 0; enemyCouCouParty.Count < Mathf.Min(5, enemyInventory.couCouInventory.Count); i++)
        {
            enemyInventory.couCouInventory[enemyCouCouParty.Count].onParty = true;
            enemyInventory.couCouInventory[enemyCouCouParty.Count].lineupOrder = enemyCouCouParty.Count;
            enemyCouCouParty.Add(enemyInventory.couCouInventory[enemyCouCouParty.Count]);
        }

        // If they haven't set up an active CouCou, default to the first on the list
        if (enemyActiveCouCou == null)
        {
            enemyCouCouParty[0].isCurrentlyActive = true;
            enemyInventory.couCouInventory[0].isCurrentlyActive = true;
            enemyActiveCouCou = enemyCouCouParty[0];
        }
        InitializeEnemyCouCou();
    }

    public void InitializeEnemyCouCou()
    {
        int abilityUID = -1;
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    abilityUID = enemyActiveCouCou.ability1;
                    break;

                case 1:
                    abilityUID = enemyActiveCouCou.ability2;
                    break;

                case 2:
                    abilityUID = enemyActiveCouCou.ability3;
                    break;

                case 3:
                    abilityUID = enemyActiveCouCou.ability4;
                    break;
            }
            if (abilityUID > 99)
            {
                utilityAbilities.Add(abilityFinder.FindUtilityAbility(abilityUID));
                attackAbilities.Add(null);
            }
            else
            {
                attackAbilities.Add(abilityFinder.FindAttackAbility(abilityUID));
                utilityAbilities.Add(null);
            }
        }
    }

    public void WildCouCouAttack()
    {
        turn++;
        if (turn == 1)
        {
            for(int i = 0; i < 4; i++)
            {
                abilityProbability.SetValue(25, i);
            }
        }
        else
        {
            for(int i = 0; i < 4; i++)
            {
                if (i == previousAbilityIndex)
                {
                    abilityProbability.SetValue(abilityProbability[i] - 9, i);
                }
                else
                {
                    abilityProbability.SetValue(abilityProbability[i] + 3, i);
                }
                
            }
        }
       
        previousAbilityIndex = PerformEnemyAbility(abilityProbability);

        if (attackAbilities[previousAbilityIndex] != null)
        {
            // Attack display
            Debug.Log(enemyActiveCouCou.coucouName + " used " + attackAbilities[previousAbilityIndex].abilityName);

        }
        else
        {
            Debug.Log(enemyActiveCouCou.coucouName + " used " + utilityAbilities[previousAbilityIndex].abilityName);
        }

        FinishedTurn();
    }

    public int PerformEnemyAbility(float[] probability)
    {
        float rnd = UnityEngine.Random.Range(1, 101); //Get a random number between 0 and 100

        if (!MakeCumulativeProbability(probability))
            return -1;

        for (int i = 0; i < probability.Length; i++)
        {
            if (rnd <= cumulativeProbability[i]) //if the probility reach the correct sum
            {
                return i;
            }
        }
        return -1; // return -1 if some error happens
    }

    bool MakeCumulativeProbability(float[] probability)
    {
        float probabilitiesSum = 0;

        cumulativeProbability = new float[4]; //reset the Array

        for (int i = 0; i < probability.Length; i++)
        {
            probabilitiesSum += probability[i]; //add the probability to the sum
            cumulativeProbability.SetValue(probabilitiesSum, i); //add the new sum to the list

            //All Probabilities need to be under 100% or it'll throw an exception
            if (probabilitiesSum > 100f)
            {
                Debug.LogError("Probabilities exceed 100%");
                return false;
            }
        }
        return true;
    }

    public void EnemyTurnStart()
    {
        if (wild)
        {
            WildCouCouAttack();
        }
        else
        {

        }
    }

    public void FinishedTurn()
    {
        battleManager.StartTurn();
    }
}

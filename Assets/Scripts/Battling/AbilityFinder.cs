using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFinder : MonoBehaviour
{
    public AbilitiesDatabase abilitiesDatabase;

    private List<AbilitiesDatabase.AttackAbilityData> attackAbilityData;
    private List<AbilitiesDatabase.UtilityAbilityData> utilityAbilityData;

    private void Awake()
    {
        attackAbilityData = new List<AbilitiesDatabase.AttackAbilityData>();
        utilityAbilityData = new List<AbilitiesDatabase.UtilityAbilityData>();
    }

    void Start()
    {
        foreach (AbilitiesDatabase.AttackAbilityData a in abilitiesDatabase.attackAbilities)
        {
            attackAbilityData.Add(a);
        }
        foreach (AbilitiesDatabase.UtilityAbilityData a in abilitiesDatabase.utilityAbilities)
        {
            utilityAbilityData.Add(a);
        }
    }

    public AbilitiesDatabase.AttackAbilityData FindAttackAbility(int abilityUID)
    {
        for (int i = 0; i < attackAbilityData.Count - 1; i++)
        {
            if (abilityUID == attackAbilityData[i].uniqueIdentifier)
            {
                return attackAbilityData[i];
            }
        }

        return null;
    }

    public AbilitiesDatabase.UtilityAbilityData FindUtilityAbility(int abilityUID)
    {
        for (int i = 0; i < attackAbilityData.Count - 1; i++)
        {
            if (abilityUID == utilityAbilityData[i].uniqueIdentifier)
            {
                return utilityAbilityData[i];
            }
        }

        return null;
    }
}

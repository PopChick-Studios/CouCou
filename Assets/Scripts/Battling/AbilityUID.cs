using UnityEngine;

public class AbilityUID : MonoBehaviour
{
    private AbilityFinder abilityFinder;
    public AbilitiesDatabase.AttackAbilityData attackAbility;
    public AbilitiesDatabase.UtilityAbilityData utilityAbility;

    public int order;
    public int abilityUID;
    public bool isUtility;

    public void LoadAbility()
    {
        abilityFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AbilityFinder>();

        if (abilityUID > 99)
        {
            isUtility = true;
            utilityAbility = abilityFinder.FindUtilityAbility(abilityUID);
        }
        else
        {
            isUtility = false;
            attackAbility = abilityFinder.FindAttackAbility(abilityUID);
        }
    }
}

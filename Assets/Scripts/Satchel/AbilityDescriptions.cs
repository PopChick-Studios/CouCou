using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDescriptions : MonoBehaviour
{
    private SatchelManager satchelManager;
    private CouCouFinder couCouFinder;
    private AbilityFinder abilityFinder;
    private BattleSystem battleSystem;

    public AbilitiesDatabase abilitiesDatabase;
    public List<AbilitiesDatabase.AttackAbilityData> attackAbilitiesList;
    public List<AbilitiesDatabase.UtilityAbilityData> utilityAbilitiesList;

    [Header("Ability 1")]
    public TextMeshProUGUI a1_abilityName;
    public TextMeshProUGUI a1_abilityDescription;
    public Image a1_abilityElement;

    [Header("Ability 2")]
    public TextMeshProUGUI a2_abilityName;
    public TextMeshProUGUI a2_abilityDescription;
    public Image a2_abilityElement;

    [Header("Ability 3")]
    public TextMeshProUGUI a3_abilityName;
    public TextMeshProUGUI a3_abilityDescription;
    public Image a3_abilityElement;

    [Header("Ability 4")]
    public TextMeshProUGUI a4_abilityName;
    public TextMeshProUGUI a4_abilityDescription;
    public Image a4_abilityElement;

    private void Awake()
    {
        satchelManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SatchelManager>();
        couCouFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CouCouFinder>();
        abilityFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AbilityFinder>();
        battleSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleSystem>();

        attackAbilitiesList = new List<AbilitiesDatabase.AttackAbilityData>();
        utilityAbilitiesList = new List<AbilitiesDatabase.UtilityAbilityData>();
    }

    private void Start()
    {
        foreach (AbilitiesDatabase.AttackAbilityData a in abilitiesDatabase.attackAbilities)
        {
            attackAbilitiesList.Add(a);
        }
        foreach (AbilitiesDatabase.UtilityAbilityData a in abilitiesDatabase.utilityAbilities)
        {
            utilityAbilitiesList.Add(a);
        }
    }

    public void DisplayAbilityDescriptions(CouCouDatabase.CouCouData coucou)
    {
        for (int i = 0; i < 4; i++)
        {
            int abilityUID = -1;
            Image abilityElement = null;
            TextMeshProUGUI nameDisplay = null;
            TextMeshProUGUI descriptionDisplay = null;
            AbilitiesDatabase.AttackAbilityData attackAbility = null;
            AbilitiesDatabase.UtilityAbilityData utilityAbility = null;

            switch (i)
            {
                case 0:
                    abilityUID = coucou.ability1;
                    nameDisplay = a1_abilityName;
                    descriptionDisplay = a1_abilityDescription;
                    abilityElement = a1_abilityElement;
                    break;
                case 1:
                    abilityUID = coucou.ability2;
                    nameDisplay = a2_abilityName;
                    descriptionDisplay = a2_abilityDescription;
                    abilityElement = a2_abilityElement;
                    break;
                case 2:
                    abilityUID = coucou.ability3;
                    nameDisplay = a3_abilityName;
                    descriptionDisplay = a3_abilityDescription;
                    abilityElement = a3_abilityElement;
                    break;
                case 3:
                    abilityUID = coucou.ability4;
                    nameDisplay = a4_abilityName;
                    descriptionDisplay = a4_abilityDescription;
                    abilityElement = a4_abilityElement;
                    break;
            }

            if (nameDisplay == null || descriptionDisplay == null || abilityElement == null)
            {
                Debug.LogError("Abilities UI has a null value");
            }

            if (abilityUID > 99)
            {
                utilityAbility = abilityFinder.FindUtilityAbility(abilityUID);

                nameDisplay.text = utilityAbility.abilityName;
                descriptionDisplay.text = utilityAbility.description;
                abilityElement.sprite = couCouFinder.GetElementSprite(utilityAbility.coucouElement);
            }
            else if (abilityUID <= 99 && abilityUID != -1)
            {
                attackAbility = abilityFinder.FindAttackAbility(abilityUID);

                nameDisplay.text = attackAbility.abilityName;
                descriptionDisplay.text = attackAbility.description;
                abilityElement.sprite = couCouFinder.GetElementSprite(attackAbility.coucouElement);
            }
            else
            {
                Debug.LogError("Couldn't get requested ability with uid of " + abilityUID);
            }
        }

        Debug.Log("Finished Displaying Abilities");
    }
}

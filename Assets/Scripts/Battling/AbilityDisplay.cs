using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityDisplay : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public AbilitiesDatabase abilitiesDatabase;
    public InventoryList inventory;

    private List<AbilitiesDatabase.AttackAbilityData> attackAbilitiesList;
    private List<AbilitiesDatabase.UtilityAbilityData> utilityAbilitiesList;

    [Header("Buttons")]
    [SerializeField] private Button ability1;
    [SerializeField] private Button ability2;
    [SerializeField] private Button ability3;
    [SerializeField] private Button ability4;
    private List<int> currentAbilites;

    private bool startDone = false;

    private void Awake()
    {
        attackAbilitiesList = new List<AbilitiesDatabase.AttackAbilityData>();
        utilityAbilitiesList = new List<AbilitiesDatabase.UtilityAbilityData>();
        currentAbilites = new List<int>();
    }

    private void Start()
    {
        // Add the list data from the scriptable object to the lists
        foreach (AbilitiesDatabase.AttackAbilityData a in abilitiesDatabase.attackAbilities)
        {
            attackAbilitiesList.Add(a);
        }
        foreach (AbilitiesDatabase.UtilityAbilityData a in abilitiesDatabase.utilityAbilities)
        {
            utilityAbilitiesList.Add(a);
        }
        startDone = true;

    }

    public IEnumerator DisplayAbilities(int ability1ID, int ability2ID, int ability3ID, int ability4ID)
    {
        yield return new WaitUntil(() => startDone);
        int checkAbilityID = -1;
        Button updateButton = null;

        for (int a = 0; a < 4; a++)
        {
            switch (a)
            {
                case 0:
                    checkAbilityID = ability1ID;
                    updateButton = ability1;
                    break;

                case 1:
                    checkAbilityID = ability2ID;
                    updateButton = ability2;
                    break;

                case 2:
                    checkAbilityID = ability3ID;
                    updateButton = ability3;
                    break;

                case 3:
                    checkAbilityID = ability4ID;
                    updateButton = ability4;
                    break;
            }

            // Attack IDs are only up to 99
            if (checkAbilityID <= 99 && checkAbilityID != -1)
            {
                for (int i = 0; i < attackAbilitiesList.Count; i++)
                {
                    if (checkAbilityID == attackAbilitiesList[i].uniqueIdentifier)
                    {
                        currentAbilites.Add(attackAbilitiesList[i].uniqueIdentifier);
                        updateButton.GetComponentInChildren<TextMeshProUGUI>().text = attackAbilitiesList[i].abilityName;
                        updateButton.GetComponent<Image>().color = attackAbilitiesList[i].abilityColor;
                        updateButton.GetComponent<AbilityUID>().abilityUID = checkAbilityID;
                        break;
                    }
                }
            }

            // Utility IDs are 100 and above
            else if (checkAbilityID > 99)
            {
                for (int i = 0; i < utilityAbilitiesList.Count; i++)
                {
                    if (checkAbilityID == utilityAbilitiesList[i].uniqueIdentifier)
                    {
                        currentAbilites.Add(utilityAbilitiesList[i].uniqueIdentifier);
                        updateButton.GetComponentInChildren<TextMeshProUGUI>().text = utilityAbilitiesList[i].abilityName;
                        updateButton.GetComponent<Image>().color = utilityAbilitiesList[i].abilityColor;
                        updateButton.GetComponent<AbilityUID>().abilityUID = checkAbilityID;
                        break;
                    }
                }
            }

            else
            {
                Debug.LogError("Ability IDs aren't correct");
            }
        }
    }
}

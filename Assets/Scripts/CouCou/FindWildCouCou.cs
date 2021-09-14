using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindWildCouCou : MonoBehaviour
{
    private GameManager gameManager;
    private CouCouFinder coucouFinder;
    private InventoryManager inventoryManager;

    public InventoryList playerInventory;
    public InventoryList enemyInventory;
    public CouCouDatabase coucouDatabase;

    private void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
        coucouFinder = GetComponent<CouCouFinder>();
        gameManager = GetComponent<GameManager>();
    }

    public int FindAverageLevel()
    {
        int average;
        List<int> levels = new List<int>();
        foreach (InventoryList.CouCouInventory coucou in playerInventory.couCouInventory)
        {
            levels.Add(coucou.coucouLevel);
        }
        if (levels.Count == 0)
        {
            average = -1;
        }
        else
        {
            average = Mathf.RoundToInt((float)Queryable.Average(levels.AsQueryable()));
        }

        return average;
    }

    public void ChooseWildCouCou(string coucouName, int level)
    {
        inventoryManager.AddCouCou(coucouName, level);

        if (string.IsNullOrEmpty(playerInventory.starterCouCou.coucouName))
        {
            playerInventory.starterCouCou = playerInventory.couCouInventory[0];
        }
        
        CouCouDatabase.CouCouData chosenCouCou = coucouFinder.FindCouCou(coucouName);
        List<CouCouDatabase.Element> elementList = coucouFinder.FindAdvantages(chosenCouCou.coucouElement);


        int randomElement = Random.Range(0, elementList.Count);
        List<CouCouDatabase.CouCouData> possibleEnemies = coucouFinder.GetElementalCouCou(elementList[randomElement]);



        int randomCouCou = Random.Range(0, possibleEnemies.Count);
        enemyInventory.preGameDialogue = new Dialogue();
        enemyInventory.couCouInventory.Clear();
        enemyInventory.itemInventory.Clear();
        InventoryList.CouCouInventory newEnemyCouCou = new InventoryList.CouCouInventory()
        {
            coucouName = possibleEnemies[randomCouCou].coucouName,
            coucouLevel = level,
            lineupOrder = 0,
            coucouVariant = possibleEnemies[randomCouCou].coucouVariant,
            element = elementList[randomElement]
        };
        enemyInventory.preGameDialogue.name = "Typhus";
        enemyInventory.preGameDialogue.sentences = new string[1];
        enemyInventory.preGameDialogue.sentences[0] = "Lets see how good you are";
        enemyInventory.couCouInventory.Add(newEnemyCouCou);

        WildCouCouFound();
    }

    public void ChooseSpecificWildCouCou(string coucouName)
    {
        CouCouDatabase.CouCouData chosenCouCou = coucouFinder.FindCouCou(coucouName);

        enemyInventory.preGameDialogue = new Dialogue();
        enemyInventory.couCouInventory.Clear();
        enemyInventory.itemInventory.Clear();
        InventoryList.CouCouInventory newEnemyCouCou = new InventoryList.CouCouInventory()
        {
            coucouName = chosenCouCou.coucouName,
            coucouLevel = FindAverageLevel(),
            lineupOrder = 0,
            coucouVariant = chosenCouCou.coucouVariant,
            element = chosenCouCou.coucouElement
        };
        Debug.Log(coucouName + " " + newEnemyCouCou.coucouName);
        enemyInventory.couCouInventory.Add(newEnemyCouCou);
        SaveSystem.SaveInventory(enemyInventory);
        WildCouCouFound();
    }

    public void WildCouCouAttack(CouCouDatabase.Element element)
    {
        enemyInventory.preGameDialogue = new Dialogue();
        enemyInventory.couCouInventory.Clear();
        enemyInventory.itemInventory.Clear();

        List<CouCouDatabase.CouCouData> possibleEnemies = coucouFinder.GetElementalCouCou(element);
        int randomCouCou = Random.Range(0, possibleEnemies.Count);
        int randomLevelIncrease = Random.Range(-3, 3);

        InventoryList.CouCouInventory newEnemyCouCou = new InventoryList.CouCouInventory()
        {
            coucouName = possibleEnemies[randomCouCou].coucouName,
            coucouLevel = FindAverageLevel() + randomLevelIncrease,
            lineupOrder = 0,
            coucouVariant = possibleEnemies[randomCouCou].coucouVariant,
            element = element
        };

        if (newEnemyCouCou.coucouLevel == -1)
        {
            return;
        }

        enemyInventory.couCouInventory.Add(newEnemyCouCou);

        WildCouCouFound();
    }

    public void WildCouCouFound()
    {
        gameManager.SetState(GameManager.GameState.Battling);
    }
}

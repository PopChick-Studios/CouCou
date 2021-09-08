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
        List<int> levels = new List<int>();
        foreach (InventoryList.CouCouInventory coucou in playerInventory.couCouInventory)
        {
            levels.Add(coucou.coucouLevel);
        }

        int average = Mathf.RoundToInt((float)Queryable.Average(levels.AsQueryable()));
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
        enemyInventory.couCouInventory.Add(newEnemyCouCou);

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
        enemyInventory.couCouInventory.Add(newEnemyCouCou);

        WildCouCouFound();
    }

    public void WildCouCouFound()
    {
        gameManager.SetState(GameManager.GameState.Battling);
    }
}

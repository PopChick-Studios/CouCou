using System.Collections;
using System.Collections.Generic;
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

    public void ChooseWildCouCou(string coucouName, int level)
    {
        inventoryManager.AddCouCou(coucouName, level);

        if (playerInventory.couCouInventory.Count == 1)
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

    public void WildCouCouFound()
    {
        gameManager.SetState(GameManager.GameState.Battling);
    }
}

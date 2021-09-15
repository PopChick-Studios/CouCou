using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntoFight : MonoBehaviour
{
    private GameManager gameManager;
    private CouCouFinder coucouFinder;

    public InventoryList playerInventory;
    public InventoryList enemyInventory;
    public InventoryList shorter1Inventory;
    public InventoryList shorter2Inventory;
    public InventoryList skip1Inventory;
    public InventoryList skip2Inventory;
    public InventoryList umbriInventory;
    public InventoryList crimsonDukeInventory;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        coucouFinder = GetComponent<CouCouFinder>();
    }

    public void GoIntoFight(string enemyName, int battleNumber)
    {
        InventoryList inventoryChange = new InventoryList();

        enemyInventory.couCouInventory.Clear();
        enemyInventory.itemInventory.Clear();

        switch (enemyName, battleNumber)
        {
            case ("Shorter", 1):
                inventoryChange = shorter1Inventory;
                break;

            case ("Shorter", 2):
                inventoryChange = shorter2Inventory;
                break;

            case ("Skip", 1):
                inventoryChange = skip1Inventory;
                break;

            case ("Skip", 2):
                inventoryChange = skip2Inventory;
                break;

            case ("Umbri", 1):
                inventoryChange = umbriInventory;
                break;

            case ("Crimson Duke", 1):
                inventoryChange = shorter1Inventory;
                break;
        }

        enemyInventory.preGameDialogue = inventoryChange.preGameDialogue;

        foreach (InventoryList.CouCouInventory coucou in inventoryChange.couCouInventory)
        {
            Debug.Log("adding " + coucou.coucouName);
            enemyInventory.couCouInventory.Add(coucou);
        }

        if (string.IsNullOrEmpty(inventoryChange.starterCouCou.coucouName))
        {
            InventoryList.CouCouInventory starter = ChoosePunkStarter(inventoryChange);
            inventoryChange.starterCouCou = starter;
            inventoryChange.couCouInventory.Insert(0, starter);
            enemyInventory.starterCouCou = starter;
            enemyInventory.couCouInventory.Insert(0, starter);
        }
        else
        {
            enemyInventory.starterCouCou = inventoryChange.starterCouCou;
        }

        for (int i = 0; i < inventoryChange.couCouInventory.Count; i++)
        {
            enemyInventory.couCouInventory[i].lineupOrder = i + 1;
        }

        foreach (InventoryList.ItemInventory items in inventoryChange.itemInventory)
        {
            enemyInventory.itemInventory.Add(items);
        }

        gameManager.SetState(GameManager.GameState.Battling);
    }

    public InventoryList.CouCouInventory ChoosePunkStarter(InventoryList inventory)
    {
        CouCouDatabase.CouCouData starterCouCou = coucouFinder.FindCouCou(playerInventory.starterCouCou.coucouName);
        List<CouCouDatabase.Element> elementList = new List<CouCouDatabase.Element>();
        if (inventory.preGameDialogue.name == "Shorter")
        {
            elementList = coucouFinder.FindAdvantages(starterCouCou.coucouElement);
        }
        else if (inventory.preGameDialogue.name == "Skip")
        {
            elementList = coucouFinder.FindDisadvantages(starterCouCou.coucouElement);
        }
        else
        {
            Debug.LogError("There is no punk with the name " + inventory.preGameDialogue.name);
        }


        int randomElement = Random.Range(0, elementList.Count);
        List<CouCouDatabase.CouCouData> possibleEnemies = coucouFinder.GetElementalCouCou(elementList[randomElement]);

        int randomCouCou = Random.Range(0, possibleEnemies.Count);

        InventoryList.CouCouInventory newEnemyCouCou = new InventoryList.CouCouInventory()
        {
            coucouName = possibleEnemies[randomCouCou].coucouName,
            coucouLevel = 15,
            lineupOrder = 1,
            coucouVariant = possibleEnemies[randomCouCou].coucouVariant,
            element = elementList[randomElement]
        };
        return newEnemyCouCou;
    }

}

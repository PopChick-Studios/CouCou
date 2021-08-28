using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWildCouCou : MonoBehaviour
{
    private GameManager gameManager;
    private CouCouFinder coucouFinder;
    private InventoryManager inventoryManager;

    public InventoryList enemyInventory;
    public CouCouDatabase coucouDatabase;

    private void Awake()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>();
        coucouFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CouCouFinder>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        
    }

    public void ChooseWildCouCouStarter(string coucouName, int level)
    {
        inventoryManager.AddCouCou(coucouName, level);

        CouCouDatabase.CouCouData starterCouCou = coucouFinder.FindCouCou(coucouName);
        List<CouCouDatabase.Element> elementList = coucouFinder.FindDisadvantages(starterCouCou.coucouElement);

        int randomElement = Random.Range(0, elementList.Count);
        List<CouCouDatabase.CouCouData> possibleEnemies = coucouFinder.GetElementalCouCou(elementList[randomElement]);

        int randomCouCou = Random.Range(0, possibleEnemies.Count);
        enemyInventory.couCouInventory.Clear();

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    private ItemFinder itemFinder;

    public InventoryList playerInventory;
    public InventoryList enemyInventory;
    public ItemsDatabase itemDatabase;
    public List<ItemsDatabase.ItemData> itemsDatabaseList;

    private void Awake()
    {
        itemFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ItemFinder>();

        itemsDatabaseList = new List<ItemsDatabase.ItemData>();
        LoadInventory(playerInventory);
        LoadInventory(enemyInventory);
    }

    private void Start()
    {
        SortItemInventory();
    }

    public void SortItemInventory()
    {
        playerInventory.itemInventory = playerInventory.itemInventory.OrderBy(e => itemFinder.FindItem(e.itemName).positionIndex).ToList();
    }

    public void LoadInventory(InventoryList inventory)
    {
        InventoryData data = SaveSystem.LoadInventory(inventory);

        if(data == null)
        {
            return;
        }
        inventory.couCouInventory.Clear();
        inventory.itemInventory.Clear();
        foreach (InventoryList.CouCouInventory c in data.coucouInventory)
        {
            inventory.couCouInventory.Add(c);
        }
        foreach (InventoryList.ItemInventory i in data.itemInventory)
        {
            inventory.itemInventory.Add(i);
        }
    }

    public void SaveInventory()
    {
        SaveSystem.SaveInventory(playerInventory);
        SaveSystem.SaveInventory(enemyInventory);
    }

    public void FoundItem(string name)
    {
        InventoryList.ItemInventory item = null;
        foreach (InventoryList.ItemInventory i in playerInventory.itemInventory.ToList())
        {
            if (name == i.itemName)
            {
                item = i;
                i.itemAmount++;
                return;
                
            }
        }

        if (item == null)
        {
            foreach (ItemsDatabase.ItemData i in itemsDatabaseList)
            {
                if (name == i.itemName)
                {
                    item = new InventoryList.ItemInventory()
                    { 
                        itemName = i.itemName, 
                        itemAmount = 1, 
                        element = i.element, 
                        itemAttribute = i.itemAttribute
                    };
                    playerInventory.itemInventory.Add(item);
                    break;
                }
            }
        }
    }

    public void UsedItem(string name)
    {
        foreach (InventoryList.ItemInventory i in playerInventory.itemInventory.ToList())
        {
            if (name == i.itemName)
            {
                i.itemAmount--;
                if (i.itemAmount <= 0)
                {
                    playerInventory.itemInventory.Remove(i);
                }
            }
        }
    }

    public void AddCouCou(InventoryList.CouCouInventory coucou)
    {
        coucou.currentHealth = coucou.maxHealth;
        playerInventory.couCouInventory.Add(coucou);
    }
}

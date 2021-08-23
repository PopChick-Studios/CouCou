using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    private ItemFinder itemFinder;

    public InventoryList inventory;
    public ItemsDatabase itemDatabase;
    public List<ItemsDatabase.ItemData> itemsDatabaseList;

    private void Awake()
    {
        itemFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ItemFinder>();

        itemsDatabaseList = new List<ItemsDatabase.ItemData>();
        LoadInventory();
    }

    private void Start()
    {
        SortItemInventory();
    }

    public void SortItemInventory()
    {
        inventory.itemInventory = inventory.itemInventory.OrderBy(e => itemFinder.FindItem(e.itemName).positionIndex).ToList();
    }

    public void LoadInventory()
    {
        InventoryData data = SaveSystem.LoadInventory();

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
        SaveSystem.SaveInventory(inventory);
    }

    public void FoundItem(string name)
    {
        InventoryList.ItemInventory item = null;
        foreach (InventoryList.ItemInventory i in inventory.itemInventory.ToList())
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
                    inventory.itemInventory.Add(item);
                    break;
                }
            }
        }
    }

    public void UsedItem(string name)
    {
        foreach (InventoryList.ItemInventory i in inventory.itemInventory.ToList())
        {
            if (name == i.itemName)
            {
                i.itemAmount--;
                if (i.itemAmount <= 0)
                {
                    inventory.itemInventory.Remove(i);
                }
            }
        }
    }
}

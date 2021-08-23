using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFinder : MonoBehaviour
{
    public ItemsDatabase itemDatabase;

    public List<ItemsDatabase.ItemData> itemDataList;

    private void Awake()
    {
        itemDataList = new List<ItemsDatabase.ItemData>();

        foreach (ItemsDatabase.ItemData a in itemDatabase.itemData)
        {
            itemDataList.Add(a);
        }
    }

    public ItemsDatabase.ItemData FindItem(string name)
    {
        for (int i = 0; i < itemDataList.Count; i++)
        {
            if (name == itemDataList[i].itemName)
            {
                return itemDataList[i];
            }
        }

        return null;
    }
}

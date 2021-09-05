using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayModelsInSatchel : MonoBehaviour
{
    private CouCouFinder coucouFinder;
    private ItemFinder itemFinder;

    public CouCouDatabase coucouDatabase;
    public List<CouCouDatabase.CouCouData> coucouList;
    public ItemsDatabase itemDatabase;
    public List<ItemsDatabase.ItemData> itemList;

    public GameObject modelDisplay;

    private void Awake()
    {
        coucouFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CouCouFinder>();
        itemFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ItemFinder>();

        coucouList = new List<CouCouDatabase.CouCouData>();
        coucouList = new List<CouCouDatabase.CouCouData>();

        foreach (CouCouDatabase.CouCouData coucouData in coucouDatabase.coucouData)
        {
            coucouList.Add(coucouData);
        }
        foreach (ItemsDatabase.ItemData itemData in itemDatabase.itemData)
        {
            itemList.Add(itemData);
        }
    }

    private void Start()
    {

    }

    public void DisplayCouCou(string coucouName)
    {
        CouCouDatabase.CouCouData coucou = new CouCouDatabase.CouCouData();
        for (int i = 0; i < coucouList.Count; i++)
        {
            if (coucouName == coucouList[i].coucouName)
            {
                coucou = coucouList[i];
            }
        }
        modelDisplay = Instantiate(coucou.coucouModel, transform);
    }

    public void DisplayItem(string itemName)
    {
        ItemsDatabase.ItemData item = new ItemsDatabase.ItemData();
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemName == itemList[i].itemName)
            {
                item = itemList[i];
            }
        }
        modelDisplay = Instantiate(item.itemModel, transform);
    }

    public void DestroyModelDisplay()
    {
        Destroy(modelDisplay);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouCouFinder : MonoBehaviour
{
    public CouCouDatabase coucouDatabase;

    public List<CouCouDatabase.CouCouData> coucouList;
    public List<CouCouDatabase.CouCouVariant> coucouVariant;

    private void Awake()
    {
        coucouList = new List<CouCouDatabase.CouCouData>();
        coucouVariant = new List<CouCouDatabase.CouCouVariant>();

        foreach (CouCouDatabase.CouCouData a in coucouDatabase.coucouData)
        {
            coucouList.Add(a);
        }
        foreach (CouCouDatabase.CouCouVariant a in coucouDatabase.coucouVariant)
        {
            coucouVariant.Add(a);
        }
    }

    public CouCouDatabase.CouCouData FindCouCou(string name)
    {
        for (int i = 0; i < coucouList.Count; i++)
        {
            if (name == coucouList[i].coucouName)
            {
                return coucouList[i];
            }
        }

        return null;
    }
}

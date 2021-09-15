using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouCouFinder : MonoBehaviour
{
    public CouCouDatabase coucouDatabase;

    public List<CouCouDatabase.CouCouData> coucouList;
    public List<CouCouDatabase.CouCouVariant> coucouVariant;

    public Sprite flameIcon;
    public Sprite aquaIcon;
    public Sprite natureIcon;
    public Sprite luxIcon;
    public Sprite umbralIcon;
    public Sprite normalIcon;
    public Sprite psychicIcon;

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

    public Sprite GetElementSprite(CouCouDatabase.Element element)
    {
        switch (element)
        {
            case CouCouDatabase.Element.Flame:
                return flameIcon;
            case CouCouDatabase.Element.Aqua:
                return aquaIcon;
            case CouCouDatabase.Element.Nature:
                return natureIcon;
            case CouCouDatabase.Element.Lux:
                return luxIcon;
            case CouCouDatabase.Element.Umbral:
                return umbralIcon;
            case CouCouDatabase.Element.Normal:
                return normalIcon;
            case CouCouDatabase.Element.Psychic:
                return psychicIcon;
            default:
                return null;
        }
    }

    public List<CouCouDatabase.Element> FindAdvantages(CouCouDatabase.Element element)
    {
        List<CouCouDatabase.Element> elementList = new List<CouCouDatabase.Element>();

        switch (element)
        {
            case CouCouDatabase.Element.Flame:
                elementList.Add(CouCouDatabase.Element.Nature);
                elementList.Add(CouCouDatabase.Element.Umbral);
                break;

            case CouCouDatabase.Element.Aqua:
                elementList.Add(CouCouDatabase.Element.Flame);
                elementList.Add(CouCouDatabase.Element.Umbral);
                break;

            case CouCouDatabase.Element.Nature:
                elementList.Add(CouCouDatabase.Element.Aqua);
                elementList.Add(CouCouDatabase.Element.Lux);
                break;

            case CouCouDatabase.Element.Umbral:
                elementList.Add(CouCouDatabase.Element.Nature);
                elementList.Add(CouCouDatabase.Element.Lux);
                break;

            case CouCouDatabase.Element.Lux:
                elementList.Add(CouCouDatabase.Element.Flame);
                elementList.Add(CouCouDatabase.Element.Aqua);
                break;
        }

        return elementList;
    }

    public List<CouCouDatabase.Element> FindDisadvantages(CouCouDatabase.Element element)
    {
        List<CouCouDatabase.Element> elementList = new List<CouCouDatabase.Element>();

        switch (element)
        {
            case CouCouDatabase.Element.Flame:
                elementList.Add(CouCouDatabase.Element.Aqua);
                elementList.Add(CouCouDatabase.Element.Lux);
                break;

            case CouCouDatabase.Element.Aqua:
                elementList.Add(CouCouDatabase.Element.Nature);
                elementList.Add(CouCouDatabase.Element.Lux);
                break;

            case CouCouDatabase.Element.Nature:
                elementList.Add(CouCouDatabase.Element.Flame);
                elementList.Add(CouCouDatabase.Element.Umbral);
                break;

            case CouCouDatabase.Element.Umbral:
                elementList.Add(CouCouDatabase.Element.Flame);
                elementList.Add(CouCouDatabase.Element.Aqua);
                break;

            case CouCouDatabase.Element.Lux:
                elementList.Add(CouCouDatabase.Element.Nature);
                elementList.Add(CouCouDatabase.Element.Umbral);
                break;
        }

        return elementList;
    }

    public List<CouCouDatabase.CouCouData> GetElementalCouCou(CouCouDatabase.Element element)
    {
        List<CouCouDatabase.CouCouData> elementalCouCouList = new List<CouCouDatabase.CouCouData>();

        foreach (CouCouDatabase.CouCouData coucou in coucouList)
        {
            if (coucou.coucouElement == element && coucou.coucouName != "Krontril")
            {
                elementalCouCouList.Add(coucou);
            }
        }

        return elementalCouCouList;
    }
}

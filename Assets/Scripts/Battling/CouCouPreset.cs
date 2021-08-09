using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CouCou Preset", menuName = "Scriptables/CouCou Preset", order = 5)]
public class CouCouPreset : ScriptableObject
{
    public enum Element
    {
        Flame,
        Nature,
        Aqua,
        Lux,
        Umbral
    }

    public string coucouName;
    public GameObject coucouModel;
    public Element coucouElement;

    public int coucouLevel;
    public int coucouVariant;

    public float coucouCurrentHealth;
}

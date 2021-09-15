using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Quest Database", menuName = "Databases/Quest Database", order = 6)]
public class QuestScriptable : ScriptableObject
{
    public int questProgress;
    public int subquestProgress;
}

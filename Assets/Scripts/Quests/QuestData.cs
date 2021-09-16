using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public int questProgress;
    public int subquestProgress;

    public QuestData(QuestScriptable questScriptable)
    {
        questProgress = questScriptable.questProgress;
        subquestProgress = questScriptable.subquestProgress;
    }
}

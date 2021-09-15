using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoundairesList
{
    public int requiredQuestProgress;
    public int requiredSubquestProgress;
    public List<GameObject> boundaries;
    public List<GameObject> triggers;
    public List<InteractableUI> interactableUI;
}

public class QuestBoundaries : MonoBehaviour
{
    public QuestScriptable questScriptable;

    public List<BoundairesList> boundairesList;
    private int previousSubquestProgress = -1;
    private int index;

    void Update()
    {
        if (questScriptable.subquestProgress != previousSubquestProgress)
        {
            Debug.Log("Subquest changed value. Changing all boundaries");
            for (int i = 0; i < boundairesList.Count; i++)
            {
                if (boundairesList[i].requiredQuestProgress != questScriptable.questProgress || boundairesList[i].requiredSubquestProgress != questScriptable.subquestProgress)
                {
                    for (int a = 0; a < boundairesList[i].boundaries.Count; a++)
                    {
                        boundairesList[i].boundaries[a].SetActive(false);
                    }
                    for (int a = 0; a < boundairesList[i].triggers.Count; a++)
                    {
                        boundairesList[i].triggers[a].SetActive(false);
                    }
                    for (int a = 0; a < boundairesList[i].interactableUI.Count; a++)
                    {
                        boundairesList[i].interactableUI[a].enabled = false;
                    }
                }
                else
                {
                    index = i;
                }
            }

            Debug.Log(index);

            for (int a = 0; a < boundairesList[index].boundaries.Count; a++)
            {
                boundairesList[index].boundaries[a].SetActive(true);
            }
            for (int a = 0; a < boundairesList[index].triggers.Count; a++)
            {
                boundairesList[index].triggers[a].SetActive(true);
            }
            for (int a = 0; a < boundairesList[index].interactableUI.Count; a++)
            {
                boundairesList[index].interactableUI[a].enabled = true;
            }
        }
        previousSubquestProgress = questScriptable.subquestProgress;
    }
}

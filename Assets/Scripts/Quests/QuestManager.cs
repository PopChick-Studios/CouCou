using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private GameManager gameManager;
    private DialogueManager dialogueManager;
    private ChangeCamera changeCamera;
    public QuestScriptable questScriptable;

    public List<Dialogue> starterCouCouDialogue;
    public List<Dialogue> wildCouCouDialogue;
    public List<Dialogue> fishingCatchDialogue;
    public List<Dialogue> firstPunksDialogue;
    public List<Dialogue> secondPunksDialogue;
    public List<Dialogue> umbriDialogue;

    public int previousSubquest;

    private void Awake()
    {
        changeCamera = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<ChangeCamera>();
        gameManager = GetComponent<GameManager>();
        dialogueManager = GetComponent<DialogueManager>();
        
    }

    public void Update()
    {
        if (previousSubquest != questScriptable.subquestProgress)
        {
            switch (questScriptable.questProgress)
            {
                case 1:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1:
                            break;
                        case 2:
                            
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        case 5:
                            StartCoroutine(dialogueManager.StartDialogue(starterCouCouDialogue));
                            break;
                        case 6:
                            StartCoroutine(dialogueManager.StartDialogue(wildCouCouDialogue));
                            break;
                        case 7:
                            StartCoroutine(changeCamera.SwitchToCamera(changeCamera.tortureRoomDoorCamera));
                            break;
                        case 8:
                            StartCoroutine(gameManager.CompleteQuest(1));
                            break;
                    }
                    break;


                case 2:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1:
                            break;
                        case 2:
                            StartCoroutine(dialogueManager.StartDialogue(fishingCatchDialogue));
                            break;
                        case 3:
                            StartCoroutine(gameManager.CompleteQuest(2));
                            break;
                    }
                    break;


                case 3:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            StartCoroutine(dialogueManager.StartDialogue(firstPunksDialogue));
                            break;
                        case 4:
                            StartCoroutine(changeCamera.SwitchToCamera(changeCamera.punksNoteCamera));
                            break;
                        case 5:
                            StartCoroutine(gameManager.CompleteQuest(3));
                            break;
                    }
                    break;


                case 4:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            StartCoroutine(changeCamera.SwitchToCamera(changeCamera.diggleTownCamera));
                            break;
                        case 4:
                            StartCoroutine(gameManager.CompleteQuest(4));
                            break;
                    }
                    break;


                case 5:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            StartCoroutine(dialogueManager.StartDialogue(secondPunksDialogue));
                            break;
                        case 4:
                            StartCoroutine(gameManager.CompleteQuest(5));
                            break;
                    }
                    break;


                case 6:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            StartCoroutine(dialogueManager.StartDialogue(umbriDialogue));
                            break;
                        case 4:
                            StartCoroutine(gameManager.CompleteQuest(6));
                            break;
                    }
                    break;


                case 7:
                    StartCoroutine(gameManager.CompleteQuest(7));
                    break;
            }
        }
        previousSubquest = questScriptable.subquestProgress;
    }
}

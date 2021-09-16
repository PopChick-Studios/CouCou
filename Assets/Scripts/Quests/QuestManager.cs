using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private GameManager gameManager;
    private DialogueManager dialogueManager;
    private ChangeCamera changeCamera;
    private InventoryManager inventoryManager;
    private IncreaseCouCouHealth increaseCouCouHealth;
    public QuestScriptable questScriptable;
    public List<NPCMovement> npcMovementList;

    public List<GameObject> punksList;

    public List<Dialogue> starterCouCouDialogue;
    public List<Dialogue> wildCouCouDialogue;
    public List<Dialogue> fishingCatchDialogue;
    public List<Dialogue> afterFishingCatchDialogue;
    public List<Dialogue> firstPunksSecondDialogue;
    public List<Dialogue> afterPunksNoteDialogue;
    public List<Dialogue> postmanDialogue;
    public List<Dialogue> secondPunksDialogue;
    public List<Dialogue> umbriDialogue;

    public int previousSubquest;

    private void Awake()
    {
        changeCamera = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<ChangeCamera>();
        gameManager = GetComponent<GameManager>();
        inventoryManager = GetComponent<InventoryManager>();
        dialogueManager = GetComponent<DialogueManager>();
        increaseCouCouHealth = GetComponent<IncreaseCouCouHealth>();
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
                            inventoryManager.FoundItem("CouCou Capsule", 10);
                            break;
                        case 6:
                            StartCoroutine(changeCamera.SwitchToCamera(changeCamera.tortureRoomDoorCamera));
                            break;
                        case 7:
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
                            break;
                        case 3:
                            StartCoroutine(dialogueManager.StartDialogue(fishingCatchDialogue));
                            break;
                        case 4:
                            StartCoroutine(dialogueManager.StartDialogue(afterFishingCatchDialogue));
                            break;
                        case 5:
                            StartCoroutine(gameManager.CompleteQuest(2));
                            break;
                    }
                    break;


                case 3:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1: // Shorter
                            break;
                        case 2: // Skip
                            StartCoroutine(dialogueManager.StartDialogue(firstPunksSecondDialogue));
                            break;
                        case 3: // First time beating Punks
                            StartCoroutine(RemovePunksAfterDelay(1));
                            StartCoroutine(gameManager.FadeToBlack(null));
                            break;
                        case 4:
                            StartCoroutine(dialogueManager.StartDialogue(afterPunksNoteDialogue));
                            break;
                        case 5:
                            StartCoroutine(gameManager.CompleteQuest(3));
                            break;
                    }
                    break;


                case 4:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1: // Meet the postman + start walking
                            StartCoroutine(npcMovementList[0].MoveNPC(postmanDialogue));
                            break;
                        case 2: // camera then go to docks
                            StartCoroutine(changeCamera.SwitchToCamera(changeCamera.docksCamera));
                            break;
                        case 3:
                            StartCoroutine(gameManager.CompleteQuest(4));
                            break;
                    }
                    break;


                case 5:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1:
                            break;
                        case 2: // Second time beating the punks
                            StartCoroutine(RemovePunksAfterDelay(1));
                            StartCoroutine(gameManager.FadeToBlack(null));
                            break;
                        case 3:
                            break;
                        case 4: // Finish Krontril
                            StartCoroutine(gameManager.CompleteQuest(5));
                            break;
                    }
                    break;


                case 6:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1: // Walk near the CouCorp
                            break;
                        case 2: // Beat Umbri
                            break;
                        case 3: // Have Umbri heal your party
                            increaseCouCouHealth.FullPartyHeal();
                            break;
                        case 4:
                            StartCoroutine(gameManager.CompleteQuest(6));
                            break;
                    }
                    break;


                case 7:
                    switch (questScriptable.subquestProgress)
                    {
                        case 1:
                            StartCoroutine(gameManager.CompleteQuest(7));
                            break;
                    }
                    break;
            }
        }
        previousSubquest = questScriptable.subquestProgress;
    }

    public IEnumerator RemovePunksAfterDelay(int time)
    {
        yield return new WaitForSeconds(1.5f);
        switch (time)
        {
            case 1:
                punksList[0].SetActive(false);
                punksList[1].SetActive(false);
                break;

            case 2:
                punksList[2].SetActive(false);
                punksList[3].SetActive(false);
                break;
        }
    }
}

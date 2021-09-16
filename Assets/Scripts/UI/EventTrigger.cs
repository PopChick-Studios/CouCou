using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    private DisplayManager displayManager;
    private DialogueManager dialogueManager;
    private PlayerMovement playerMovement;
    private QuestBoundaries questBoundaries;

    public QuestScriptable questScriptable;
    public Transform warpPosition;
    public int requiredQuest;
    public int requiredSubquest;
    public bool greaterThan;

    public enum EventTriggerType
    {
        Quest,
        EnterTriggerDialogue,
        EnterTriggerDialogueQuest,
        Interaction,
        InteractionFight, // Gives Quest Point
        InteractionQuest,
        Warp,
        InteractionWildCouCou,
        EnterTriggerDialogueFight // Gives Quest Point
    }

    public EventTriggerType eventTriggerType;
    public List<Dialogue> dialogue;

    private void Awake()
    {
        displayManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DisplayManager>();
        dialogueManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DialogueManager>();
        questBoundaries = GameObject.FindGameObjectWithTag("GameManager").GetComponent<QuestBoundaries>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    public void TriggerDialogue()
    {
        StartCoroutine(dialogueManager.StartDialogue(dialogue));
    }

    public IEnumerator TriggerDialogueQuest()
    {
        StartCoroutine(dialogueManager.StartDialogue(dialogue));
        yield return new WaitUntil(() => dialogueManager.dialogueFinished);
        questScriptable.subquestProgress++;
    }

    public IEnumerator Interact(string coucouName, int coucouLevel)
    {
        switch (eventTriggerType)
        {
            case EventTriggerType.Interaction:
                StartCoroutine(dialogueManager.StartDialogue(dialogue));
                break;
            case EventTriggerType.InteractionFight:
                if (requiredQuest == questScriptable.questProgress && requiredSubquest == questScriptable.subquestProgress)
                {
                    StartCoroutine(dialogueManager.StartDialogue(dialogue));
                    StartCoroutine(StartFight());
                    questBoundaries.stall = true;
                    questScriptable.subquestProgress++;
                }
                break;
            case EventTriggerType.InteractionQuest:
                if (requiredQuest == questScriptable.questProgress && requiredSubquest == questScriptable.subquestProgress)
                {
                    StartCoroutine(dialogueManager.StartDialogue(dialogue));
                    yield return new WaitUntil(() => dialogueManager.dialogueFinished);
                    questScriptable.subquestProgress++;
                }
                break;
            case EventTriggerType.InteractionWildCouCou:
                if (requiredQuest == questScriptable.questProgress && requiredSubquest == questScriptable.subquestProgress)
                {
                    StartCoroutine(dialogueManager.StartDialogue(dialogue));
                    yield return new WaitUntil(() => dialogueManager.dialogueFinished);
                    Debug.Log("Starting Fight");
                    if (string.IsNullOrEmpty(coucouName))
                    {
                        displayManager.OnChooseCouCou();
                    }
                    else
                    {
                        displayManager.OnChooseSpecificCouCou(coucouName, coucouLevel);
                    }
                    questScriptable.subquestProgress++;
                }
                break;

            case EventTriggerType.Warp:
                if ((requiredQuest == questScriptable.questProgress && requiredSubquest == questScriptable.subquestProgress) || (greaterThan && (requiredQuest <= questScriptable.questProgress || requiredSubquest <= questScriptable.subquestProgress)))
                {
                    StartCoroutine(playerMovement.WarpPlayer(warpPosition.position));
                }
                break;
        }
    }

    public IEnumerator StartFight()
    {
        yield return new WaitUntil(() => dialogueManager.dialogueFinished);
        Debug.Log("Starting Fight");
        displayManager.OnFightingCouCorp(gameObject.GetComponent<InteractableUI>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && requiredQuest == questScriptable.questProgress && requiredSubquest == questScriptable.subquestProgress)
        {
            switch (eventTriggerType)
            {
                case EventTriggerType.Quest:
                    questScriptable.subquestProgress++;
                    break;
                case EventTriggerType.EnterTriggerDialogue:
                    TriggerDialogue();
                    break;
                case EventTriggerType.EnterTriggerDialogueQuest:
                    StartCoroutine(TriggerDialogueQuest());
                    break;
                case EventTriggerType.EnterTriggerDialogueFight:
                    TriggerDialogue();
                    StartCoroutine(StartFight());
                    questBoundaries.stall = true;
                    questScriptable.subquestProgress++;
                    break;

                case EventTriggerType.Warp:
                    StartCoroutine(playerMovement.WarpPlayer(warpPosition.position));
                    break;
            }
        }
    }
}

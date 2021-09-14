using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    private DisplayManager displayManager;
    private DialogueManager dialogueManager;
    private PlayerMovement playerMovement;

    public QuestScriptable questScriptable;
    public Transform warpPosition;
    public int requiredQuest;
    public int requiredSubquest;

    public enum EventTriggerType
    {
        Quest,
        EnterTriggerDialogue,
        EnterTriggerDialogueQuest,
        Interaction,
        InteractionFight,
        InteractionQuest,
        Warp,
        InteractionWildCouCou
    }

    public EventTriggerType eventTriggerType;
    public List<Dialogue> dialogue;

    private void Awake()
    {
        displayManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DisplayManager>();
        dialogueManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DialogueManager>();
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

    public IEnumerator Interact(string coucouName)
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
                    displayManager.OnChooseSpecificCouCou(coucouName);
                    questScriptable.subquestProgress++;
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

                case EventTriggerType.Warp:
                    StartCoroutine(playerMovement.WarpPlayer(warpPosition.position));
                    break;
            }
        }
    }
}

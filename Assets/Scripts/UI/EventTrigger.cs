using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    private DisplayManager displayManager;
    private DialogueManager dialogueManager;

    public QuestScriptable questScriptable;
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
        Warp
    }

    public EventTriggerType eventTriggerType;
    public List<Dialogue> dialogue;

    private void Awake()
    {
        displayManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DisplayManager>();
        dialogueManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DialogueManager>();
    }

    public void TriggerDialogue()
    {
        StartCoroutine(dialogueManager.StartDialogue(dialogue));
    }

    public IEnumerator Interact()
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
        if (other.CompareTag("Player"))
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
                    TriggerDialogue();
                    break;
            }
        }
    }
}

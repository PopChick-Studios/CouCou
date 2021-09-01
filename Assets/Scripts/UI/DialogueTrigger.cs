using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private DisplayManager displayManager;
    private DialogueManager dialogueManager;

    public enum DialogueTriggerType
    {
        EnterTrigger,
        Interaction
    }

    public DialogueTriggerType dialogueTriggerType;
    public Dialogue dialogue;

    private void Awake()
    {
        displayManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DisplayManager>();
        dialogueManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DialogueManager>();
    }

    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }

    public void InteractDialogue()
    {
        dialogueManager.StartDialogue(dialogue);

        StartCoroutine(StartFight());
    }


    public IEnumerator StartFight()
    {
        yield return new WaitUntil(() => dialogueManager.dialogueFinished);
        Debug.Log("Starting Fight");
        displayManager.OnFightingCouCorp(gameObject.GetComponent<InteractableUI>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && dialogueTriggerType == DialogueTriggerType.EnterTrigger)
        {
            TriggerDialogue();
        }
    }
}

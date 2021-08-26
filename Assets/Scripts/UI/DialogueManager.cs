using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Animator animatior;

    private PlayerInputActions playerInputActions;
    private Queue<string> sentences;
    private string currentSentence = "";

    private bool dialogueOccuring = false;
    private bool dialogueFinished = true;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Click.started += x => OnDialogueSkip();
        playerInputActions.UI.Submit.started += x => OnDialogueSkip();
        playerInputActions.UI.Cancel.started += x => OnDialogueSkip();
    }

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void OnDialogueSkip()
    {
        if (!dialogueFinished && dialogueOccuring)
        {
            CompleteSentence();
        }
        else if (!dialogueFinished && !dialogueOccuring)
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animatior.SetBool("DialogueIsOpen", true);
        dialogueFinished = false;
        nameText.text = dialogue.name;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void CompleteSentence()
    {
        Debug.Log("Completed Sentence");
        dialogueOccuring = false;
        StopAllCoroutines();
        dialogueText.text = currentSentence;
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        Debug.Log("Loading Sentence");
        currentSentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(currentSentence));
    }

    public IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        dialogueOccuring = true;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }

        dialogueOccuring = false;
    }

    public void EndDialogue()
    {
        animatior.SetBool("DialogueIsOpen", false);
        dialogueFinished = true;
    }

    #region - Enable/Disable -

    private void OnEnable()
    {
        playerInputActions.UI.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.UI.Disable();
    }

    #endregion
}

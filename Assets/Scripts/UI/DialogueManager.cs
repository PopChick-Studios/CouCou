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

    public bool dialogueOccuring = false;
    public bool dialogueFinished = true;
    public bool speechFinished = true;

    private Coroutine lastCoroutine;

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

    public IEnumerator StartDialogue(List<Dialogue> dialogue)
    {
        if (dialogueFinished)
        {
            animatior.SetBool("DialogueIsOpen", true);
            dialogueFinished = false;
            sentences.Clear();
            for (int i = 0; i < dialogue.Count; i++)
            {
                nameText.text = dialogue[i].name;
                sentences.Clear();
                foreach (string sentence in dialogue[i].sentences)
                {
                    sentences.Enqueue(sentence);
                }
                speechFinished = false;
                DisplayNextSentence();
                yield return new WaitUntil(() => speechFinished);
            }

            EndDialogue();
        }
    }

    public void CompleteSentence()
    {
        dialogueOccuring = false;
        if (lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
        }
        dialogueText.text = currentSentence;
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            speechFinished = true;
            return;
        }
        dialogueOccuring = true;
        currentSentence = sentences.Dequeue();
        lastCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    public IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.04f);
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

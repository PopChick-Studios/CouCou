using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private GameManager gameManager;
    private InteractableUI interactableUI;
    private DisplayManager displayManager;
    private InventoryManager inventoryManager;
    private DialogueTrigger dialogueTrigger;
    private DialogueManager dialogueManager;
    private Fishing fishing;
    private PlayerMovement playerMovement;

    public Dialogue dialogue;

    // Saving game
    public bool onSaveButton;
    public bool onCancelSaveButton;
    public bool interacting;
    public bool canFinishInteracting;

    // Animator
    public Animator animator;

    // Inputs
    PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        inventoryManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        displayManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DisplayManager>();
        dialogueManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DialogueManager>();
        fishing = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Fishing>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.Interact.started += x => StartCoroutine(Interact());
        playerInputActions.UI.Cancel.started += x => OnCouCouCancelButton();
        playerInputActions.UI.Submit.started += x => StartCoroutine(FinishInteraction(interactableUI.interactionType));
        playerInputActions.Fishing.Interact.started += x => FinishFishingInteraction();
        playerInputActions.UI.Cancel.started += x => StartCoroutine(FinishInteraction(interactableUI.interactionType));
        playerInputActions.Fishing.Cancel.started += x => FinishFishingInteraction();
    }

    private IEnumerator Interact()
    {
        if (interactableUI != null && !interacting && gameManager.State == GameManager.GameState.Wandering)
        {
            if (interactableUI.canInteract && interactableUI.interactionType != DisplayManager.InteractionTypes.CouCorp)
            {
                interacting = true;

                playerInputActions.UI.Enable();
                playerInputActions.Wandering.Disable();

                if (interactableUI.interactionType == DisplayManager.InteractionTypes.Collect)
                {
                    playerMovement.canMove = false;
                    animator.SetTrigger("grabbingItem");
                    yield return new WaitForSeconds(1.3f);
                    displayManager.OnInteraction(interactableUI.interactionType, interactableUI.itemName, interactableUI.itemAmount);
                    inventoryManager.FoundItem(interactableUI.itemName, interactableUI.itemAmount);
                }
                else
                {
                    displayManager.OnInteraction(interactableUI.interactionType, interactableUI.itemName, interactableUI.itemAmount);
                }

                // Pause the game
                Time.timeScale = 0;
                canFinishInteracting = true;
            }
            else if (interactableUI.interactionType == DisplayManager.InteractionTypes.CouCorp && dialogueManager.dialogueFinished)
            {
                if (!inventoryManager.HasPlayableCouCou())
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
                }
                else
                {
                    dialogueTrigger = interactableUI.gameObject.GetComponent<DialogueTrigger>();
                    dialogueTrigger.InteractDialogue();
                }
                canFinishInteracting = true;
            }
        }
    }

    public IEnumerator FinishInteraction(DisplayManager.InteractionTypes interactionType)
    {
        if (!canFinishInteracting)
        {
            yield break;
        }
        if (interactionType == DisplayManager.InteractionTypes.Collect || interactionType == DisplayManager.InteractionTypes.Letter)
        {
            displayManager.HeadsUpDisplay();
            gameManager.SetState(GameManager.GameState.Wandering);
            // Swap inputs
            playerInputActions.UI.Disable();
            playerInputActions.Wandering.Enable();

            if (interactableUI.gameObject != null)
            {
                interactableUI.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(1.7f);
            playerMovement.canMove = true;
        }
        else if (onSaveButton)
        {
            interactableUI.gameObject.SetActive(false);
            playerInputActions.UI.Disable();
            playerInputActions.Wandering.Enable();

            onSaveButton = false;
        }
        else if (onCancelSaveButton)
        {
            playerInputActions.UI.Disable();
            playerInputActions.Wandering.Enable();

            onCancelSaveButton = false;
        }

        interacting = false;
        canFinishInteracting = false;
    }

    public void ChangeToFishingInput()
    {
        playerInputActions.Wandering.Disable();
        playerInputActions.UI.Disable();
        playerInputActions.Fishing.Enable();
    }

    public void FinishFishingInteraction()
    {
        if (fishing.caughtSomething)
        {
            displayManager.HeadsUpDisplay();
            gameManager.SetState(GameManager.GameState.Wandering);
            playerInputActions.Wandering.Enable();
            fishing.caughtSomething = false;
            fishing.InstantiateUI();
        }
    }

    public void OnSaveGame()
    {
        displayManager.HeadsUpDisplay();
        gameManager.SetState(GameManager.GameState.Wandering);

        onSaveButton = true;

        StartCoroutine(FinishInteraction(interactableUI.interactionType));
    }

    public void OnCancelSave()
    {
        displayManager.HeadsUpDisplay();
        gameManager.SetState(GameManager.GameState.Wandering);

        onCancelSaveButton = true;

        StartCoroutine(FinishInteraction(interactableUI.interactionType));
    }

    public void OnCouCouCancelButton()
    {
        displayManager.HeadsUpDisplay();
        playerInputActions.UI.Disable();
        playerInputActions.Wandering.Enable();

        interacting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            interactableUI = other.GetComponent<InteractableUI>();
        }
    }

    #region - Enable/Disable -

    private void OnEnable()
    {
        playerInputActions.Wandering.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private GameManager gameManager;
    private InteractableUI interactableUI;
    private DisplayManager displayManager;
    private InventoryManager inventoryManager;
    private EventTrigger eventTrigger;
    private DialogueManager dialogueManager;
    private Fishing fishing;
    private PlayerMovement playerMovement;
    public QuestScriptable questScriptable;
    public List<Dialogue> dialogue;
    public InventoryList playerInventory;

    public bool onSaveButton;
    public bool onCancelSaveButton;
    public bool interacting;
    public bool canFinishInteracting;
    public bool hasQuestMarker;


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
        playerInputActions.Wandering.Interact.started += x => FinishQuestRewards();
        playerInputActions.UI.Cancel.started += x => FinishQuestRewards();
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
            eventTrigger = interactableUI.GetComponent<EventTrigger>();
            if (interactableUI.canInteract && eventTrigger == null)
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
            else if (interactableUI.canInteract && eventTrigger != null && dialogueManager.dialogueFinished)
            {
                if (!inventoryManager.HasPlayableCouCou() && eventTrigger.eventTriggerType == EventTrigger.EventTriggerType.InteractionFight)
                {
                    StartCoroutine(FindObjectOfType<DialogueManager>().StartDialogue(dialogue));
                }
                else
                {
                    StartCoroutine(eventTrigger.Interact());
                }
                canFinishInteracting = true;
            }
        }
    }

    public IEnumerator FinishInteraction(DisplayManager.InteractionTypes interactionType)
    {
        interactableUI.GiveProgress();
        if (!canFinishInteracting)
        {
            yield break;
        }
        if (interactionType == DisplayManager.InteractionTypes.Collect)
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
        else if (interactionType == DisplayManager.InteractionTypes.Letter)
        {
            displayManager.HeadsUpDisplay();
            gameManager.SetState(GameManager.GameState.Wandering);
            // Swap inputs
            playerInputActions.UI.Disable();
            playerInputActions.Wandering.Enable();
        }
        else if (onSaveButton)
        {
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

        if (hasQuestMarker)
        {
            questScriptable.subquestProgress++;
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

    public void FinishQuestRewards()
    {
        if (gameManager.questRewardFinish)
        {
            displayManager.HeadsUpDisplay();
            gameManager.SetState(GameManager.GameState.Wandering);
            playerInputActions.Wandering.Enable();
            gameManager.questRewardFinish = false;
        }
    }

    public void OnSaveGame()
    {
        SaveSystem.SavePlayer(GetComponent<Player>(), questScriptable);
        SaveSystem.SaveInventory(playerInventory);

        displayManager.HeadsUpDisplay();
        gameManager.SetState(GameManager.GameState.Wandering);

        onSaveButton = true;
        canFinishInteracting = true;
        StartCoroutine(FinishInteraction(interactableUI.interactionType));
    }

    public void OnCancelSave()
    {
        displayManager.HeadsUpDisplay();
        gameManager.SetState(GameManager.GameState.Wandering);

        onCancelSaveButton = true;
        canFinishInteracting = true;
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

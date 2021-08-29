using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    private GameManager gameManager;
    private InteractableUI interactableUI;
    private DisplayManager displayManager;
    private InventoryManager inventoryManager;

    // Saving game
    public bool OnSaveButton;
    public bool OnCancelSaveButton;

    // Animator
    // public Animator animator;

    // Inputs
    PlayerInputActions playerInputActions;

    private void Awake()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        displayManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DisplayManager>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.Interact.performed += x => Interact();
        playerInputActions.UI.Cancel.started += x => OnCouCouCancelButton();
        playerInputActions.UI.Submit.performed += x => FinishInteraction(interactableUI.interactionType);
    }

    private void Interact()
    {
        if (interactableUI != null && gameManager.State == GameManager.GameState.Wandering)
        {
            if (interactableUI.canInteract)
            {
                displayManager.OnInteraction(interactableUI.interactionType, interactableUI);
                // animator.SetTrigger("interactPickUp");

                // Pause the game
                Time.timeScale = 0;

                playerInputActions.UI.Enable();
                playerInputActions.Wandering.Disable();

                if (interactableUI.interactionType == DisplayManager.InteractionTypes.Collect)
                {
                    inventoryManager.FoundItem(interactableUI.itemName, interactableUI.itemAmount);
                }
            }
        }
    }

    public void FinishInteraction(DisplayManager.InteractionTypes interactionType)
    {
        if (interactionType == DisplayManager.InteractionTypes.Collect || interactionType == DisplayManager.InteractionTypes.Letter)
        {
            displayManager.HeadsUpDisplay();
            gameManager.SetState(GameManager.GameState.Wandering);

            // Swap inputs
            playerInputActions.UI.Disable();
            playerInputActions.Wandering.Enable();

            interactableUI.gameObject.SetActive(false);
        }
        else if (OnSaveButton == true)
        {
            interactableUI.gameObject.SetActive(false);
            playerInputActions.UI.Disable();
            playerInputActions.Wandering.Enable();

            OnSaveButton = false;
        }
        else if (OnCancelSaveButton == true)
        {
            playerInputActions.UI.Disable();
            playerInputActions.Wandering.Enable();

            OnCancelSaveButton = false;
        }
    }

    public void OnSaveGame()
    {
        displayManager.HeadsUpDisplay();
        gameManager.SetState(GameManager.GameState.Wandering);

        OnSaveButton = true;

        FinishInteraction(interactableUI.interactionType);
    }
    public void OnCancelSave()
    {
        displayManager.HeadsUpDisplay();
        gameManager.SetState(GameManager.GameState.Wandering);

        OnCancelSaveButton = true;

        FinishInteraction(interactableUI.interactionType);
    }

    public void OnCouCouCancelButton()
    {
        displayManager.HeadsUpDisplay();
        playerInputActions.UI.Disable();
        playerInputActions.Wandering.Enable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
            interactableUI = other.GetComponent<InteractableUI>();
    }

    #region - Enable/Disable -

    private void OnEnable()
    {
        playerInputActions.Wandering.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Wandering.Disable();
    }

    #endregion
}

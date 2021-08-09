using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    private GameManager gameManager;
    private InteractableUI interactableUI;

    // Animator
    // [SerializeField] private Animator animator;

    // Inputs
    PlayerInputActions playerInputActions;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        interactableUI = GameObject.FindGameObjectWithTag("Player").GetComponent<InteractableUI>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.Interact.performed += x => Interact();
    }

    private void Interact()
    {
        if (interactableUI.canInteract)
        {
            // animator.SetBool("interactPickUp", true);
            gameManager.SetState(GameManager.GameState.Interacting);
        }
    }

    #region - Enable/Disable -

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    #endregion
}

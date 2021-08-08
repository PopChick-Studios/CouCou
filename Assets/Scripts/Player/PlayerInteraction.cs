using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    private GameManager gameManager;

    public bool canInteract;

    // Inputs
    PlayerInputActions playerInputActions;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.Interact.performed += x => Interact();
    }

    private void Interact()
    {
        if (canInteract)
        {
            // Start interaction animation here
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

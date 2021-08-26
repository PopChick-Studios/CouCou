using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class DisplayManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject interaction;
    [SerializeField] private GameObject satchel;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject crossfade;

    [SerializeField] private GameObject collectUI;
    [SerializeField] private GameObject letterUI;
    [SerializeField] private GameObject saveUI;

    [SerializeField] private Button continueButton;

    public enum InteractionTypes
    {
        Collect,
        Letter,
        Save
    }

    // Inputs
    PlayerInputActions playerInputActions;
    public bool isPaused = false;

    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.Pause.performed += x => PauseMenu();

        HeadsUpDisplay();
    }

    public void PauseMenu()
    {
        Time.timeScale = 0;

        HUD.SetActive(false);
        interaction.SetActive(false);
        satchel.SetActive(false);
        pause.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

        gameManager.SetState(GameManager.GameState.Paused);

        continueButton.Select();
    }

    public void HeadsUpDisplay()
    {
        Time.timeScale = 1;

        HUD.SetActive(true);
        interaction.SetActive(false);
        satchel.SetActive(false);
        pause.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;

        gameManager.SetState(GameManager.GameState.Wandering);
    }

    public void OpenSatchel()
    {
        satchel.SetActive(true);
        interaction.SetActive(false);
        pause.SetActive(false);
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            Debug.Log("Application is paused");
            PauseMenu();
        }
    }

    public void OnInteraction(InteractionTypes type)
    {
        HUD.SetActive(false);
        interaction.SetActive(true);
        satchel.SetActive(false);
        pause.SetActive(false);

        Cursor.lockState = CursorLockMode.None;

        switch (type)
        {
            case InteractionTypes.Collect:
                collectUI.SetActive(true);
                letterUI.SetActive(false);
                saveUI.SetActive(false);
                break;

            case InteractionTypes.Letter:
                letterUI.SetActive(true);
                saveUI.SetActive(false);
                collectUI.SetActive(false);
                break;

            case InteractionTypes.Save:
                saveUI.SetActive(true);
                letterUI.SetActive(false);
                collectUI.SetActive(false);
                break;

            default:
                interaction.SetActive(false);
                break;
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

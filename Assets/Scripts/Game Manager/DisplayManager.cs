using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class DisplayManager : MonoBehaviour
{
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject Interaction;
    [SerializeField] private GameObject Satchel;
    [SerializeField] private GameObject Pause;

    [SerializeField] private GameObject CollectUI;
    [SerializeField] private GameObject LetterUI;
    [SerializeField] private GameObject SaveUI;

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
        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.Pause.performed += x => PauseMenu();
    }

    public void PauseMenu()
    {
        Time.timeScale = 0;

        HUD.SetActive(false);
        Interaction.SetActive(false);
        Satchel.SetActive(false);
        Pause.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
    }

    public void HeadsUpDisplay()
    {
        Time.timeScale = 1;

        HUD.SetActive(true);
        Interaction.SetActive(false);
        Satchel.SetActive(false);
        Pause.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnApplicationPause(bool pause)
    {
        Debug.Log("Application is paused");
        PauseMenu();
    }

    public void OnInteraction(InteractionTypes type)
    {
        HUD.SetActive(false);
        Interaction.SetActive(true);
        Satchel.SetActive(false);
        Pause.SetActive(false);

        Cursor.lockState = CursorLockMode.None;

        switch (type)
        {
            case InteractionTypes.Collect:
                CollectUI.SetActive(true);
                LetterUI.SetActive(false);
                SaveUI.SetActive(false);
                break;

            case InteractionTypes.Letter:
                LetterUI.SetActive(true);
                SaveUI.SetActive(false);
                CollectUI.SetActive(false);
                break;

            case InteractionTypes.Save:
                SaveUI.SetActive(true);
                LetterUI.SetActive(false);
                CollectUI.SetActive(false);
                break;

            default:
                Interaction.SetActive(false);
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

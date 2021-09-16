using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    private GameManager gameManager;
    private SceneLoader sceneLoader;

    public Button continueButton;
    public Button newGameCancelButton;
    public GameObject optionsMenu;
    public GameObject controlMenu;
    public GameObject newGameWarning;
    public GameObject mainMenu;

    PlayerInputActions playerInputActions;

    public bool hasData;

    private void Awake()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data == null)
        {
            hasData = false;
        }
        else
        {
            hasData = true;
        }

        sceneLoader = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneLoader>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Cancel.started += x => GoBack();
    }

    private void Start()
    {
        if (!hasData)
        {
            continueButton.interactable = false;
        }

        gameManager.SetState(GameManager.GameState.TitleScreen);
        GoBack();
    }

    public void GoBack()
    {
        continueButton.Select();
        newGameWarning.SetActive(false);
        optionsMenu.SetActive(false);
        controlMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OnOptionMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OnControlsMenu()
    {
        mainMenu.SetActive(false);
        controlMenu.SetActive(true);
    }

    public void OnNewGame()
    {
        if (hasData)
        {
            newGameWarning.SetActive(true);
            newGameCancelButton.Select();
        }
        else
        {
            sceneLoader.LoadNewGame();
        }

    }

    public void OnQuit()
    {
        Debug.Log("Application Quit");
        Application.Quit();
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

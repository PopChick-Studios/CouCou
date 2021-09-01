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
    private FindWildCouCou findWildCouCou;
    private SatchelAdventureManager satchelAdventureManager;
    private PlayerInteraction playerInteraction;
    private IntoFight intoFight;

    [Header("Blur Camera")]
    public GameObject blurCamera;

    [Header("UI Elements")]
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject interaction;
    [SerializeField] private GameObject satchel;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject confirmation;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject crossfade;
    [SerializeField] private GameObject coucouCamera;

    [Header("Interactables")]
    [SerializeField] private GameObject collectUI;
    [SerializeField] private TextMeshProUGUI collectText;
    [SerializeField] private GameObject letterUI;
    [SerializeField] private GameObject saveUI;
    [SerializeField] private GameObject coucouUI;
    [SerializeField] private TextMeshProUGUI coucouUIText;


    [Header("Buttons")]
    [SerializeField] private Button coucouUIYes;
    [SerializeField] private Button continueButton;
    public Button confirmationQuitButton;

    private string coucouInteractingName;

    public enum InteractionTypes
    {
        Collect,
        Letter,
        Save,
        CouCou,
        CouCorp
    }

    // Inputs
    PlayerInputActions playerInputActions;


    private void Awake()
    {
        intoFight = GetComponent<IntoFight>();
        findWildCouCou = GetComponent<FindWildCouCou>();
        gameManager = GetComponent<GameManager>();
        satchelAdventureManager = GameObject.FindGameObjectWithTag("AdventureUI").GetComponent<SatchelAdventureManager>();
        playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.Pause.performed += x => PauseMenu();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HeadsUpDisplay();

        confirmation.SetActive(false);
    }

    public void PauseMenu()
    {
        if (satchelAdventureManager.inSubmit || satchelAdventureManager.changingCouCou)
        {
            satchelAdventureManager.GoBack();
        }
        else if (!playerInteraction.interacting)
        {
            Time.timeScale = 0;
            options.SetActive(false);
            HUD.SetActive(false);
            interaction.SetActive(false);
            satchel.SetActive(false);
            pause.SetActive(true);
            coucouCamera.SetActive(false);
            confirmation.SetActive(false);
            blurCamera.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            gameManager.SetState(GameManager.GameState.Paused);

            continueButton.Select();
        }
    }

    public void OnConfirmation()
    {
        pause.SetActive(false);
        confirmation.SetActive(true);
        confirmationQuitButton.Select();
    }

    public void HeadsUpDisplay()
    {
        gameManager.SetState(GameManager.GameState.Wandering);
        Time.timeScale = 1;
        blurCamera.SetActive(false);
        options.SetActive(false);
        HUD.SetActive(true);
        interaction.SetActive(false);
        satchel.SetActive(false);
        pause.SetActive(false);
        coucouCamera.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSatchel()
    {
        satchel.SetActive(true);
        satchelAdventureManager.ClearCouCou();
        satchelAdventureManager.DisplayItems();
        interaction.SetActive(false);
        pause.SetActive(false);
        coucouCamera.SetActive(true);
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            Debug.Log("Application is paused");
            PauseMenu();
        }
    }

    public void OpenOptions()
    {
        options.SetActive(true);
        pause.SetActive(false);
    }

    public void OnInteraction(InteractionTypes type, InteractableUI interactingWith)
    {
        if (interactingWith.itemName != "")
        {
            coucouInteractingName = interactingWith.itemName;
        }

        HUD.SetActive(false);
        interaction.SetActive(true);
        satchel.SetActive(false);
        pause.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        switch (type)
        {
            case InteractionTypes.Collect:
                collectUI.SetActive(true);
                letterUI.SetActive(false);
                saveUI.SetActive(false);
                coucouUI.SetActive(false);

                collectText.text = interactingWith.itemName + " x" + interactingWith.itemAmount;
                break;

            case InteractionTypes.Letter:
                letterUI.SetActive(true);
                saveUI.SetActive(false);
                collectUI.SetActive(false);
                coucouUI.SetActive(false);
                break;

            case InteractionTypes.Save:
                saveUI.SetActive(true);
                letterUI.SetActive(false);
                collectUI.SetActive(false);
                coucouUI.SetActive(false);
                break;

            case InteractionTypes.CouCou:
                saveUI.SetActive(false);
                letterUI.SetActive(false);
                collectUI.SetActive(false);
                coucouUI.SetActive(true);
                coucouUIText.text = "Do you want to choose " + interactingWith.itemName + "?";
                coucouUIYes.Select();
                break;

            case InteractionTypes.CouCorp:
                saveUI.SetActive(false);
                letterUI.SetActive(false);
                collectUI.SetActive(false);
                coucouUI.SetActive(false);
                break;

            default:
                interaction.SetActive(false);
                break;
        }
    }

    public void OnChooseCouCou()
    {
        Time.timeScale = 1;
        findWildCouCou.ChooseWildCouCou(coucouInteractingName, 15);
    }

    public void OnFightingCouCorp(InteractableUI coucorp)
    {
        Time.timeScale = 1;
        intoFight.GoIntoFight(coucorp.itemName, coucorp.itemAmount);
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

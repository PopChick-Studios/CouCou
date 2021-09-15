using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayManager : MonoBehaviour
{
    private GameManager gameManager;
    private FindWildCouCou findWildCouCou;
    private SatchelAdventureManager satchelAdventureManager;
    private LetterManager letterManager;
    private PlayerInteraction playerInteraction;
    private IntoFight intoFight;
    private QuestBook questBook;
    public QuestScriptable questScriptable;

    [Header("Blur Camera")]
    public GameObject blurCamera;

    [Header("UI Elements")]
    public GameObject HUD;
    public GameObject interaction;
    [SerializeField] private GameObject satchel;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject confirmation;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject crossfade;
    [SerializeField] private GameObject coucouCamera;
    [SerializeField] private GameObject questBookDisplay;

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
    [SerializeField] private Button saveButton;
    public Button confirmationQuitButton;

    private string coucouInteractingName;

    public enum InteractionTypes
    {
        Door,
        Warp,
        Collect,
        Letter,
        Save,
        StarterCouCou,
        CouCou,
        CouCorp,
        NPC
    }

    // Inputs
    PlayerInputActions playerInputActions;


    private void Awake()
    {
        letterManager = GameObject.FindGameObjectWithTag("AdventureUI").GetComponent<LetterManager>();
        intoFight = GetComponent<IntoFight>();
        findWildCouCou = GetComponent<FindWildCouCou>();
        gameManager = GetComponent<GameManager>();
        satchelAdventureManager = GameObject.FindGameObjectWithTag("AdventureUI").GetComponent<SatchelAdventureManager>();
        playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();
        questBook = GameObject.FindGameObjectWithTag("QuestBook").GetComponent<QuestBook>();
        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.Pause.started += x => PauseMenu();
        //playerInputActions.UI.Cancel.started += x => PauseMenu();
        playerInputActions.Wandering.QuestBook.started += x => OnQuestBook();
        playerInputActions.Wandering.Satchel.started += x => OpenSatchel();
        playerInputActions.Wandering.GoBack.started += x => GoBack();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        confirmation.SetActive(false);
        questBookDisplay.SetActive(false);
    }

    private void Start()
    {
        HeadsUpDisplay();
    }

    public void PauseMenu()
    {
        Debug.Log("Pause Menu");
        if (satchel.activeInHierarchy)
        {
            satchelAdventureManager.GoBack();
        }
        else if (questBookDisplay.activeInHierarchy)
        {
            OnQuestBook();
        }
        else if (!playerInteraction.interacting && gameManager.State != GameManager.GameState.Fishing && !pause.activeInHierarchy)
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
        else if (pause.activeInHierarchy)
        {
            HeadsUpDisplay();
        }
    }

    public void GoBack()
    {
        if (satchel.activeInHierarchy)
        {
            satchelAdventureManager.GoBack();
        }
        else if (questBookDisplay.activeInHierarchy)
        {
            OnQuestBook();
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
        satchelAdventureManager.ClearItems();
        satchelAdventureManager.ClearCouCou();
        pause.SetActive(false);
        coucouCamera.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSatchel()
    {
        if (!satchel.activeInHierarchy && (gameManager.State == GameManager.GameState.Wandering || pause.activeInHierarchy) && !interaction.activeInHierarchy)
        {
            Time.timeScale = 0;
            satchel.SetActive(true);
            satchelAdventureManager.ClearCouCou();
            satchelAdventureManager.OnItemSection();
            interaction.SetActive(false);
            pause.SetActive(false);
            coucouCamera.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameManager.SetState(GameManager.GameState.Paused);
        }
        else if (satchel.activeInHierarchy && gameManager.State == GameManager.GameState.Paused)
        {
            Time.timeScale = 1;
            satchel.SetActive(false);
            blurCamera.gameObject.SetActive(false);
            satchelAdventureManager.ClearCouCou();
            satchelAdventureManager.ClearItems();
            satchelAdventureManager.selectedSection = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            coucouCamera.SetActive(false);
            gameManager.SetState(GameManager.GameState.Wandering);
        }
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause == true && gameManager.State == GameManager.GameState.Wandering)
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

    public void OnQuestBook()
    {
        if (questBookDisplay.activeInHierarchy && gameManager.State == GameManager.GameState.Paused)
        {
            Time.timeScale = 1;
            questBookDisplay.SetActive(false);
            questBook.CloseBook();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gameManager.SetState(GameManager.GameState.Wandering);
        }
        else if (!questBookDisplay.activeInHierarchy && gameManager.State == GameManager.GameState.Wandering && !interaction.activeInHierarchy)
        {
            Time.timeScale = 0;
            questBookDisplay.SetActive(true);
            questBook.OpenBook();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameManager.SetState(GameManager.GameState.Paused);
        }
    }

    public void OnInteraction(InteractionTypes type, string name, int amount)
    {
        if (!string.IsNullOrEmpty(name))
        {
            coucouInteractingName = name;
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

                collectText.text = name + " x" + amount;
                break;

            case InteractionTypes.Letter:
                letterManager.DisplayQuestLetter(questScriptable.questProgress, questScriptable.subquestProgress);
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
                saveButton.Select();
                break;

            case InteractionTypes.StarterCouCou:
                saveUI.SetActive(false);
                letterUI.SetActive(false);
                collectUI.SetActive(false);
                coucouUI.SetActive(true);
                coucouUIText.text = "Do you want to choose " + name + "?";
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
        findWildCouCou.ChooseWildCouCou(coucouInteractingName, 1);
    }

    public void OnChooseSpecificCouCou(string coucouName)
    {
        Time.timeScale = 1;
        findWildCouCou.ChooseSpecificWildCouCou(coucouName);
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

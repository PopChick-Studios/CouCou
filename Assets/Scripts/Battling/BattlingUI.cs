using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattlingUI : MonoBehaviour
{
    private AbilityDisplay abilityDisplay;
    private BattleSystem battleSystem;
    private SatchelManager satchelManager;

    public GameObject satchel;
    public GameObject blurCamera;
    public GameObject fightButtons;
    public GameObject menu;
    public GameObject healthBars;
    public GameObject dialogueBox;
    public GameObject prompt;
    public GameObject pause;
    public GameObject options;
    public GameObject confirmation;

    public Button continueButton;
    public Button confirmationQuitButton;

    public GameObject menuFirstButton;
    public GameObject abilitiesFirstButton;
    private GameObject lastButtonPressed;

    private PlayerInputActions playerInputActions;

    public bool inFightMenu = false;
    public bool inPause = false;
    public bool inSatchel = false;

    private void Awake()
    {
        battleSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleSystem>();
        abilityDisplay = GetComponent<AbilityDisplay>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Cancel.started += x => BackToMenu();
        options.SetActive(false);
        pause.SetActive(false);
        prompt.SetActive(false);
        fightButtons.SetActive(false);
        menu.SetActive(true);
        confirmation.SetActive(false);

        lastButtonPressed = menuFirstButton;
    }

    private void Start()
    {
        satchelManager = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<SatchelManager>();
        OnNewRound();
    }

    public void OnPause()
    {
        Time.timeScale = 0;
        inPause = true;
        options.SetActive(false);
        confirmation.SetActive(false);
        pause.SetActive(true);
        continueButton.Select();
    }

    public void OnOptions()
    {
        options.SetActive(true);
        pause.SetActive(false);
        inPause = false;
    }

    public void OnConfirmation()
    {
        pause.SetActive(false);
        confirmation.SetActive(true);
        confirmationQuitButton.Select();
        inPause = false;
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            Debug.Log("Application is paused");
            OnPause();
        }
    }

    public void LastButtonPressed(GameObject button)
    {
        lastButtonPressed = button;
    }

    public void OnFight()
    {
        fightButtons.SetActive(true);
        menu.SetActive(false);

        abilitiesFirstButton.GetComponent<Button>().Select();

        inFightMenu = true;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        if (battleSystem.state == BattleState.PLAYERTURN && inFightMenu)
        {
            fightButtons.SetActive(false);
            menu.SetActive(true);
            healthBars.SetActive(true);
            dialogueBox.SetActive(true);
            lastButtonPressed.GetComponent<Button>().Select();

            inFightMenu = false;
        }
        else if (inPause)
        {
            pause.SetActive(false);
            inPause = false;
            lastButtonPressed.GetComponent<Button>().Select();
        }
        else if (inSatchel && !satchelManager.inPrompt && !satchelManager.inSubmit)
        {
            fightButtons.SetActive(false);
            inFightMenu = false;
            menu.SetActive(true);
            healthBars.SetActive(true);
            dialogueBox.SetActive(true);
            lastButtonPressed.GetComponent<Button>().Select();

            inSatchel = false;
        }
        else
        {
            OnPause();
        }
    }

    public void OnNewRound()
    {
        Time.timeScale = 1;

        menuFirstButton.GetComponent<Button>().Select();

        satchel.SetActive(false);
        menu.SetActive(true);
    }

    public void OnFinishTurn()
    {
        Time.timeScale = 1;

        satchelManager.ClearItems();
        satchelManager.ClearCouCou();

        inFightMenu = false;
        blurCamera.SetActive(false);
        satchel.SetActive(false);
        healthBars.SetActive(true);
        fightButtons.SetActive(false);
        menu.SetActive(false);
        dialogueBox.SetActive(true);
    }

    public void OnSatchel()
    {
        dialogueBox.SetActive(false);
        fightButtons.SetActive(false);
        menu.SetActive(false);
        healthBars.SetActive(false);
        inFightMenu = false;
        inSatchel = true;
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }
    private void OnDisable()
    {
        playerInputActions.Disable();
    }
}

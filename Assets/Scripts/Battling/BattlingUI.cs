using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public GameObject menuFirstButton;
    public GameObject abilitiesFirstButton;
    private GameObject lastButtonPressed;

    private PlayerInputActions playerInputActions;

    public bool inFightMenu;

    private void Awake()
    {
        battleSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleSystem>();
        abilityDisplay = GetComponent<AbilityDisplay>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Cancel.performed += x => BackToMenu();

        prompt.SetActive(false);
        fightButtons.SetActive(false);
        menu.SetActive(true);

        lastButtonPressed = menuFirstButton;
    }

    private void Start()
    {
        satchelManager = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<SatchelManager>();
        OnNewRound();
    }

    public void LastButtonPressed(GameObject button)
    {
        lastButtonPressed = button;
    }

    public void OnFight()
    {
        // create selected object
        EventSystem.current.SetSelectedGameObject(null);

        fightButtons.SetActive(true);
        menu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(abilitiesFirstButton);

        inFightMenu = true;
    }

    public void BackToMenu()
    {
        if (battleSystem.state == BattleState.PLAYERTURN && inFightMenu)
        {
            // create selected object
            EventSystem.current.SetSelectedGameObject(null);

            fightButtons.SetActive(false);
            menu.SetActive(true);
            healthBars.SetActive(true);
            dialogueBox.SetActive(true);

            EventSystem.current.SetSelectedGameObject(lastButtonPressed);

            inFightMenu = false;
        }
    }

    public void BackToMenuFromSatchel()
    {
        // create selected object
        EventSystem.current.SetSelectedGameObject(null);

        fightButtons.SetActive(false);
        menu.SetActive(true);
        healthBars.SetActive(true);
        dialogueBox.SetActive(true);

        EventSystem.current.SetSelectedGameObject(lastButtonPressed);
    }

    public void OnNewRound()
    {
        // create selected object
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuFirstButton);

        satchel.SetActive(false);
        menu.SetActive(true);
    }

    public void OnFinishTurn()
    {
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

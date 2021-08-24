using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattlingUI : MonoBehaviour
{
    private SatchelOpenClose satchelOpenClose;
    private AbilityDisplay abilityDisplay;

    public GameObject fightButtons;
    public GameObject menuButtons;
    public GameObject healthBars;

    public GameObject menuFirstButton;
    public GameObject abilitiesFirstButton;
    private GameObject lastButtonPressed;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Cancel.performed += x => BackToMenu();

        abilityDisplay = GetComponent<AbilityDisplay>();
        satchelOpenClose = GameObject.FindGameObjectWithTag("Satchel").GetComponent<SatchelOpenClose>();
        
        fightButtons.SetActive(false);
        menuButtons.SetActive(true);

        lastButtonPressed = menuFirstButton;
    }

    private void Start()
    {
        OnNewRound();
    }

    public void InitializeCouCou()
    {

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
        menuButtons.SetActive(false);

        EventSystem.current.SetSelectedGameObject(abilitiesFirstButton);
    }

    public void BackToMenu()
    {
        // create selected object
        EventSystem.current.SetSelectedGameObject(null);

        fightButtons.SetActive(false);
        menuButtons.SetActive(true);
        healthBars.SetActive(true);

        EventSystem.current.SetSelectedGameObject(lastButtonPressed);
    }

    public void OnNewRound()
    {
        // create selected object
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuFirstButton);

        menuButtons.SetActive(true);
    }

    public void OnFinishTurn()
    {
        fightButtons.SetActive(false);
        menuButtons.SetActive(false);
    }

    public void OnSatchel()
    {
        EventSystem.current.SetSelectedGameObject(null);

        fightButtons.SetActive(false);
        menuButtons.SetActive(false);
        healthBars.SetActive(false);
        satchelOpenClose.gameObject.SetActive(true);
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

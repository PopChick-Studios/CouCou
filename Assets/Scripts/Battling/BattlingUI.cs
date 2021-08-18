using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattlingUI : MonoBehaviour
{
    private AbilityDisplay abilityDisplay;

    public GameObject fightButtons;
    public GameObject menu;
    public GameObject healthBars;
    public GameObject dialogueBox;

    public GameObject menuFirstButton;
    public GameObject abilitiesFirstButton;
    private GameObject lastButtonPressed;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        abilityDisplay = GetComponent<AbilityDisplay>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Cancel.performed += x => BackToMenu();

        fightButtons.SetActive(false);
        menu.SetActive(true);

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
        menu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(abilitiesFirstButton);
    }

    public void BackToMenu()
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

        menu.SetActive(true);
    }

    public void OnFinishTurn()
    {
        fightButtons.SetActive(false);
        menu.SetActive(false);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattlingUI : MonoBehaviour
{
    private AbilityDisplay abilityDisplay;

    public GameObject fightButtons;
    public GameObject menuButtons;
    public GameObject healthBars;

    public GameObject menuFirstButton;
    public GameObject abilitiesFirstButton;
    private GameObject lastButtonPressed;

    private void Awake()
    {
        abilityDisplay = GetComponent<AbilityDisplay>();
        
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
        fightButtons.SetActive(false);
        menuButtons.SetActive(false);
        healthBars.SetActive(false);
    }
}

using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // References to other scripts
    private DisplayManager displayManager;
    private IncreaseCouCouHealth increaseCouCouHealth;

    public InventoryList playerInventory;

    // Create the states of the game
    public enum GameState
    {
        TitleScreen,
        Wandering,
        Paused,
        Interacting,
        Dialogue,
        Battling
    }
    [SerializeField] private GameState gameState;
    public GameState State { get { return gameState; } }

    public void SetState(GameState state)
    {
        gameState = state;
        Debug.Log("State changed into " + state);

    }

    // Start is called before the first frame update
    void Start()
    {
        increaseCouCouHealth = gameObject.GetComponent<IncreaseCouCouHealth>();
        displayManager = gameObject.GetComponent<DisplayManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.TitleScreen:

                break;
            
            case GameState.Wandering:
                
                break;

            case GameState.Paused:
                
                break;

            case GameState.Interacting:
                //displayManager.OnInteraction(DisplayManager.InteractionTypes.Letter);
                break;

            case GameState.Dialogue:

                break;
            
            case GameState.Battling:

                break;
        }
    }

    public void OnQuit()
    {
        Debug.Log("Application Quit");
        // FOR TESTING ONLY
        //SaveSystem.DeleteAllSaveFiles();
        Application.Quit();
    }

    public void OnApplicationQuit()
    {
        //SaveSystem.DeleteAllSaveFiles();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // References to other scripts
    private DisplayManager displayManager;

    // Create the states of the game
    public enum GameState
    {
        TitleScreen,
        Wandering,
        Interacting,
        Dialogue,
        Battling
    }
    private GameState gameState;
    public GameState State { get { return gameState; } }

    public void SetState(GameState state)
    {
        gameState = state;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Wandering;

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

            case GameState.Interacting:
                //displayManager.OnInteraction(DisplayManager.InteractionTypes.Letter);
                break;

            case GameState.Dialogue:

                break;
            
            case GameState.Battling:

                break;
        }
    }
}

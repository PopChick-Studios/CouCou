using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInputActions playerInputActions;
    public static event Action<InputActionMap> actionMapChange;

    // Start is called before the first frame update
    public void Start()
    {
        playerInputActions = new PlayerInputActions();

        // Start with UI inputs since we start at the title screen
        ToggleActionMap(playerInputActions.Wandering);
    }

    public static void ToggleActionMap(InputActionMap inputActions)
    {
        if (inputActions.enabled)
        {
            Debug.Log("Not gonna change input bc it already enabled " + inputActions);
            return;
        }

        //This is not working, may want to delete

        actionMapChange?.Invoke(inputActions); // Only if you need to check if inputs have been changed
        inputActions.Enable();

        Debug.Log("Current inputActions " + inputActions);
    }
}

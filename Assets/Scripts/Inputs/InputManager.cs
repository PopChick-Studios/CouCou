using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum eInputState
    {
        MouseKeyboard,
        Controller
    };
    private eInputState m_State = eInputState.MouseKeyboard;

    void OnGUI()
    {
        switch (m_State)
        {
            case eInputState.MouseKeyboard:

                // Hide cursor when using mouse/keyboard
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (isControllerInput())
                {
                    m_State = eInputState.Controller;
                }
                break;
            case eInputState.Controller:

                // Show cursor when using controller
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (Input.GetMouseButtonDown(0))
                {
                    m_State = eInputState.MouseKeyboard;
                }
                break;
        }
    }

    public eInputState GetInputState()
    {
        return m_State;
    }

    private bool isControllerInput()
    {
        // joystick buttons
        if (Input.GetKey(KeyCode.Joystick1Button0) ||
           Input.GetKey(KeyCode.Joystick1Button1) ||
           Input.GetKey(KeyCode.Joystick1Button2) ||
           Input.GetKey(KeyCode.Joystick1Button3) ||
           Input.GetKey(KeyCode.Joystick1Button4) ||
           Input.GetKey(KeyCode.Joystick1Button5) ||
           Input.GetKey(KeyCode.Joystick1Button6) ||
           Input.GetKey(KeyCode.Joystick1Button7) ||
           Input.GetKey(KeyCode.Joystick1Button8) ||
           Input.GetKey(KeyCode.Joystick1Button9) ||
           Input.GetKey(KeyCode.Joystick1Button10) ||
           Input.GetKey(KeyCode.Joystick1Button11) ||
           Input.GetKey(KeyCode.Joystick1Button12) ||
           Input.GetKey(KeyCode.Joystick1Button13) ||
           Input.GetKey(KeyCode.Joystick1Button14) ||
           Input.GetKey(KeyCode.Joystick1Button15) ||
           Input.GetKey(KeyCode.Joystick1Button16) ||
           Input.GetKey(KeyCode.Joystick1Button17) ||
           Input.GetKey(KeyCode.Joystick1Button18) ||
           Input.GetKey(KeyCode.Joystick1Button19))
        {
            return true;
        }

        // joystick axis
        if (Input.GetAxis("RightStickHorizontal") != 0.0f ||
           Input.GetAxis("RightStickVertical") != 0.0f ||
           Input.GetAxis("LeftStickHorizontal") != 0.0f ||
           Input.GetAxis("LeftStickVertical") != 0.0f)
        {
            return true;
        }

        return false;
    }
}

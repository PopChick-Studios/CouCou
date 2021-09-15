using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class CameraFollow : MonoBehaviour
{
    private GameManager gameManager;

    public float CameraMoveSpeed = 120f;
    public GameObject playerCameraFollow;
    public Transform panningCameraFollow;
    public float upperClampAngle;
    public float lowerClampAngle;
    public float inputSensitivity = 150.0f;

    private float rotY = 0.0f;
    private float rotX = 0.0f;
    
    // Inputs
    PlayerInputActions playerInputActions;
    public Vector2 inputRotationMouse;
    public Vector2 inputRotationJoystick;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.CameraMouse.performed += x => inputRotationMouse = x.ReadValue<Vector2>();
        playerInputActions.Wandering.CameraMouse.canceled += x => inputRotationMouse = Vector2.zero;
        playerInputActions.Wandering.CameraJoystick.performed += x => inputRotationJoystick = x.ReadValue<Vector2>();
        playerInputActions.Wandering.CameraJoystick.canceled += x => inputRotationJoystick = Vector2.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    private void Update()
    {
        if (gameManager.State == GameManager.GameState.Wandering || gameManager.State == GameManager.GameState.Fishing)
        {
            int inverted = 1;
            Vector2 rotation = (inputRotationMouse * 0.1f) + inputRotationJoystick;

            if((Mathf.Abs(inputRotationJoystick.y) < 0.3 && inputRotationJoystick.magnitude != 0) || (Mathf.Abs(inputRotationMouse.y) < 1.4 && inputRotationMouse.magnitude != 0))
            {
                rotation.y = 0;
            }

            if (PlayerPrefs.HasKey("InvertY"))
            {
                inverted = PlayerPrefs.GetInt("InvertY");
            }

            // Rotate according to the input and the sensitivity
            rotY += rotation.x * inputSensitivity * Time.deltaTime;
            rotX += inverted * rotation.y * inputSensitivity * Time.deltaTime;

            // Clamps the angle so it can't go above or below certain angles
            rotX = Mathf.Clamp(rotX, lowerClampAngle, upperClampAngle);

            transform.rotation = Quaternion.Euler(rotX, rotY, 0.0f);
        }
    }

    void LateUpdate()
    {
        switch (gameManager.State)
        {
            case GameManager.GameState.Wandering:
                panningCameraFollow = playerCameraFollow.transform;
                CameraUpdater();
                break;
            case GameManager.GameState.Fishing:
                panningCameraFollow = playerCameraFollow.transform;
                CameraUpdater();
                break;
            case GameManager.GameState.Dialogue:
                CameraUpdater();
                break;
            case GameManager.GameState.Paused:

                break;
        }
    }

    void CameraUpdater()
    {
        // Set target to follow
        Transform target = playerCameraFollow.transform;

        // Move towards the target
        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
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

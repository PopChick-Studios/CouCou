using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private InputManager inputManager;

    public float CameraMoveSpeed = 120f;
    public GameObject CameraFollowObject;
    public float clampAngle = 80f;
    public float inputSensitivity = 150.0f;

    // Mouse position
    private float finalInputX;
    private float finalInputZ;
    public float mouseSensitivity = 3f;

    private float rotY = 0.0f;
    private float rotX = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InputManager>();

        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Set up the rotation of the joysticks
        float inputX = Input.GetAxis("RightStickHorizontal");
        float inputZ = Input.GetAxis("RightStickVertical");

        // Get the input from the mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Get the input manager and the input that is being used
        InputManager.eInputState inputState = inputManager.GetInputState();
        if (inputState == InputManager.eInputState.Controller)
        {
            mouseX = 0;
            mouseY = 0;
        }

        // Combine the mouse and joystick input together
        finalInputX = inputX + mouseX;
        finalInputZ = inputZ + mouseY;

        // Rotate according to the final input and the sensitivity
        rotY += finalInputX * inputSensitivity * Time.deltaTime;
        rotX += finalInputZ * inputSensitivity * Time.deltaTime;

        // Clamps the angle so it can't go above or below certain angles
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        transform.rotation = Quaternion.Euler(rotX, rotY, 0.0f);
    }

    void LateUpdate()
    {
        CameraUpdater();
    }

    void CameraUpdater()
    {
        // Set target to follow
        Transform target = CameraFollowObject.transform;

        // Move towards the target
        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}

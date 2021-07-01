using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour {

    public Transform PlayerTransform;

    private Vector3 _cameraOffset;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    public bool LookAtPlayer = false;

    public bool RotateAroundPlayer = true;

    public float RotationsSpeed = 5.0f;

    // Use this for initialization
    void Start ()
    {
        _cameraOffset = transform.position - PlayerTransform.position;

    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    // LateUpdate is called after Update methods
    void LateUpdate ()
    {
        if(RotateAroundPlayer)
        {
            Quaternion camTurnAngleX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * RotationsSpeed, Vector3.up);
            Quaternion camTurnAngleY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * RotationsSpeed, Vector3.forward);

            _cameraOffset = camTurnAngleY * camTurnAngleX * _cameraOffset;
        }

        Vector3 newPos = PlayerTransform.position + _cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        if (LookAtPlayer || RotateAroundPlayer)
            transform.LookAt(PlayerTransform);
	}
}
